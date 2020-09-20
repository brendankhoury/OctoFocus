using System;
using System.Windows;
// using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Input;
using System.Media;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
// 1. Added the "GooseModdingAPI" project as a reference.
// 2. Compile this.
// 3. Create a folder with this DLL in the root, and *no GooseModdingAPI DLL*
using GooseShared;
using SamEngine;
using System.Collections.Specialized;

namespace DefaultMod
{
    public class ModEntryPoint : IMod
    {
        // The maximum amount of time that a user can be unproductive for before octocat harrasses the cursor. 
        static long unproductiveCap = (long) (1000 * 60 * 0.35);
        // Every second a user is productive, their unproductive timer decreases. 
        // I.E.: unrpoductiveTime -= productiveTimeElapsed * unproductiveRegeration
        static double unproductiveRegeration = 1; 

        Image[] octoCostume = new Image[8];
        Image[] madCostume = new Image[8];
        Image grabbyCostume;
        Image transparent;

        int frameCounter = 0;

        long productiveWork = 0;
        long unproductiveWork = 0;
        long nahMan = 0;

        int direction = -1;

        bool locked = false;
        bool prevMoving = false;    
        
        string[] productiveWindows = { "visual studio", "stack overflow", "github", "vs code", "duckduckgo", "devpost" };
        string[] unproductiveWindows = { "steam", "facebook", "reddit", "discord", "youtube", "instagram", "league of legends", "clubpenguin", "netflix", "valorant" };

        string activeWindow;
        Stopwatch timeStopwatch;
        Stopwatch angryTimer;
        Stopwatch deltaTimer;
        //Image octocat = Image.FromFile("octocat.jpg");

        // Gets called automatically, passes in a class that contains pointers to
        // useful functions we need to interface with the goose.
        void IMod.Init()
        {
            // Subscribe to whatever events we want
            InjectionPoints.PostTickEvent += WindowCheck;
            InjectionPoints.PreTickEvent += OneTimeInitialization;
            // InjectionPoints.PostRenderEvent += OctoPostRender; // post render

            Console.WriteLine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "idle_0.png"));

            octoCostume[0] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "idle_0.png"));
            octoCostume[1] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "idle_1.png"));
            octoCostume[2] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "idle_2.png"));
            octoCostume[3] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "idle_3.png"));
            octoCostume[4] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "idle_4.png"));
            octoCostume[5] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "idle_3.png"));
            octoCostume[6] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "idle_2.png"));
            octoCostume[7] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "idle_1.png"));
            
            madCostume[0] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "angry_0.png"));
            madCostume[1] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "angry_1.png"));
            madCostume[2] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "angry_2.png"));
            madCostume[3] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "angry_3.png"));
            madCostume[4] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "angry_4.png"));
            madCostume[5] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "angry_3.png"));
            madCostume[6] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "angry_2.png"));
            madCostume[7] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "angry_1.png"));

            grabbyCostume = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "grab_0_angry.png"));

            transparent = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "transparent.png"));

            // Figure out current active window.
            timeStopwatch = new Stopwatch();
            angryTimer = new Stopwatch();
            deltaTimer = new Stopwatch();
            deltaTimer.Start();
            activeWindow = GetActiveWindow();
            timeStopwatch.Start();
        }

        public void OneTimeInitialization(GooseEntity g)
        {
            Console.WriteLine("Goose OneTimeInitialization");

            // Make goose invisible.
            /*g.renderData.brushGooseOutline = new SolidBrush(Color.Transparent);
            g.renderData.brushGooseWhite = new SolidBrush(Color.Transparent);
            g.renderData.brushGooseOrange = new SolidBrush(Color.Transparent);
            g.renderData.shadowBrush = new TextureBrush(transparent);
            g.renderData.DrawingPen = new Pen(Color.Transparent);
*/
            g.render = OctoPostRender;


            g.parameters.StepTimeNormal = 500f;
            g.parameters.StepTimeCharged = 500f;

            // octoCostume[0] = Sonicgoose.Properties.Resources.frame_1;
            API.Goose.setCurrentTaskByID(g, "AngryOctocat");
            InjectionPoints.PreTickEvent -= OneTimeInitialization;
        }

        public void WindowCheck(GooseEntity g)
        {
            bool productive = false;
            bool unproductive = false;
            string checkedWindow = GetActiveWindow();



            if (checkedWindow == null) return;
            else checkedWindow = checkedWindow.ToLower();
            if (activeWindow == null)
            {
                activeWindow = checkedWindow;
                return;
            }
            else
            {
                foreach (var windowName in unproductiveWindows)
                {
                    if (activeWindow.Contains(windowName))
                    {
                        unproductive = true;
                        break;
                    }
                }
                if (!unproductive)
                {
                    foreach (var windowName in productiveWindows)
                    {
                        if (activeWindow.Contains(windowName))
                        {
                            productive = true;
                            break;
                        }
                    }
                }

                if (productive)
                {
                    productiveWork += timeStopwatch.ElapsedMilliseconds;
                    if (unproductiveWork > 0)
                    {
                        unproductiveWork -= (long)(timeStopwatch.ElapsedMilliseconds * unproductiveRegeration);
                    }
                }
                else if (unproductive)
                {
                    unproductiveWork += timeStopwatch.ElapsedMilliseconds;
                }
                else
                {
                    nahMan += timeStopwatch.ElapsedMilliseconds;
                }
                activeWindow = checkedWindow;
                timeStopwatch.Restart();
                //string output = "Productive?: " + productive + " ProductiveWork: " + productiveWork + " || UnproductiveWork: " + unproductiveWork;
                //Console.WriteLine(output);
            }


            if (g.currentTask == API.TaskDatabase.getTaskIndexByID("HappyOctocat"))
            {
                if (unproductive && unproductiveWork >= unproductiveCap && angryTimer.ElapsedMilliseconds / 10000 % 2 == 0)
                {
                    Vector2 mousePos = new Vector2(Input.mouseX, Input.mouseY);

                    if (Vector2.Distance(mousePos, g.rig.bodyCenter) < 50)
                    {
                        API.Goose.setCurrentTaskByID(g, "GrabbingOctocat");
                    } else {
                        API.Goose.setCurrentTaskByID(g, "AngryOctocat");
                    }
                    unproductiveWork = 0;
                }
            }
            else if (g.currentTask == API.TaskDatabase.getTaskIndexByID("AngryOctocat") || g.currentTask == API.TaskDatabase.getTaskIndexByID("GrabbingOctocat"))
            {
                if (productive || angryTimer.ElapsedMilliseconds / 10000 % 2 != 0) // Should cause octocat to become happy 10 seconds after an angry state is triggered. 
                {
                    API.Goose.setCurrentTaskByID(g, "HappyOctocat");
                    angryTimer.Reset();
                }
            }
            else
            {
                API.Goose.setCurrentTaskByID(g, "HappyOctocat");
            }
            //g.render(g, Graphics.FromImage(octocat));
            // logic
        }

        public void OctoPostRender(GooseEntity g, Graphics graph)
        {

            int temp = frameCounter;
            Image currentOcto;

            double speed = Math.Sqrt(g.velocity.x * g.velocity.x + g.velocity.y * g.velocity.y);
            
            if (speed > 70)
                temp *= 2;

            if (temp / 30 < 0 || temp / 30 > 7)
            {
                temp = 0;
                frameCounter = 0;
            }

            // Set octocat costume based on grabbing vs idle state

            // Brendan Breaks Code:
            int frameId = Math.Abs(((int)deltaTimer.ElapsedMilliseconds % 2000)/200 - 5);

            if(g.currentTask == API.TaskDatabase.getTaskIndexByID("GrabbingOctocat"))
                currentOcto = grabbyCostume;
            else if(g.currentTask == API.TaskDatabase.getTaskIndexByID("AngryOctocat"))
                currentOcto = madCostume[frameId];
            else
                currentOcto = octoCostume[frameId];

            var headPoint = g.rig.bodyCenter;

            var verticalOffset = currentOcto.Height /2;
            var horizontalOffset = currentOcto.Width / 2;

            int target;

            
            if (speed > 75 && g.currentTask != API.TaskDatabase.getTaskIndexByID("GrabbingOctocat")) {
                // State moving and not grabbing cursor
                target = (int)g.direction + 90;
                if(!prevMoving)
                    locked = false;
                prevMoving = true;
            } 
            else {
                // State idle or grabbing cursor
                target = 0;
                if(prevMoving)
                    locked = false;
                prevMoving = false;
            }


            if(!locked) {

                int current = direction - target;
                if(Math.Abs(current) <= 3){
                    locked = true;
                    direction = target;
                }
                else if(current > 180) {
                    direction += 2;
                } else if(current < 180) {// > 3
                    direction -= 2;
                }
            } else {
                direction = target;
            }

            if(direction >= 360)
                direction -= 360;
            else if(direction < 0) 
                direction += 360;

            // Brendan Breaks Code:
//            Vector2 bottomRightCorner = new Vector2( (int) SystemParameters.WorkArea.Width- 70, (int) SystemParameters.WorkArea.Height - 150);
//            if (Vector2.Distance(bottomRightCorner, g.rig.bodyCenter) < 10 && direction < 10 && (direction < 10 || direction > 350)) {
//                direction = 0;
//            } 
            direction = 0;
            
                
            Console.WriteLine("target: " + target);
            Console.WriteLine("locked: " + locked);
            Console.WriteLine("speed: " + speed);

            Bitmap newSprite = RotateImage(new Bitmap(currentOcto), (float) direction);
            
            if(g.currentTask == API.TaskDatabase.getTaskIndexByID("GrabbingOctocat"))
                graph.DrawImage(newSprite, headPoint.x-horizontalOffset, headPoint.y-(int)(1.5f*verticalOffset));
            else
                graph.DrawImage(newSprite, headPoint.x-horizontalOffset, headPoint.y-verticalOffset);

            frameCounter++;

        }

        private static Bitmap RotateImage(Bitmap b, float angle)
        {

            Bitmap returnBitmap = new Bitmap(b.Width, b.Height);

            using(Graphics g = Graphics.FromImage(returnBitmap))
            {
                g.TranslateTransform((float)b.Width / 2, (float)b.Height / 2);
                g.RotateTransform(angle);
                g.TranslateTransform(-(float)b.Width / 2, -(float)b.Height / 2);
                g.DrawImage(b, new System.Drawing.Point(0, 0));
            }

            return returnBitmap;

        }



        private static float Sin(float deg)
            => (float)Math.Sin(deg * (Math.PI / 180d));
        private static float Cos(float deg)
            => (float)Math.Cos(deg * (Math.PI / 180d));

        private static System.Drawing.Point ToPoint(Vector2 vector)
            => new System.Drawing.Point((int)vector.x, (int)vector.y);


        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        string GetActiveWindow()
        {
            const int nChars = 256;
            IntPtr handle;
            StringBuilder Buff = new StringBuilder(nChars);
            handle = GetForegroundWindow();
            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }

            return null;
        }
    }
}
