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
        static long unproductiveCap = 1000 * 60 * 5;
        // Every second a user is productive, their unproductive timer decreases. 
        // I.E.: unrpoductiveTime -= productiveTimeElapsed * unproductiveRegeration
        static double unproductiveRegeration = 1; 

        Image[] octoCostume = new Image[4];

        int frameCounter = 0;

        long productiveWork = 0;
        long unproductiveWork = 0;
        long nahMan = 0;
        
        string[] productiveWindows = { "visual studio", "stack overflow", "github", "vs code", "google", "duckduckgo", "devpost" };
        string[] unproductiveWindows = { "steam", "facebook", "reddit", "discord", "youtube", "instagram", "league of legends", "clubpenguin" };

        string activeWindow;
        Stopwatch timeStopwatch;
        //Image octocat = Image.FromFile("octocat.jpg");

        // Gets called automatically, passes in a class that contains pointers to
        // useful functions we need to interface with the goose.
        void IMod.Init()
        {
            // Subscribe to whatever events we want
            InjectionPoints.PostTickEvent += WindowCheck;
            InjectionPoints.PreTickEvent += OneTimeInitialization;
            InjectionPoints.PostRenderEvent += OctoPostRender;

            Console.WriteLine(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "idle_0.png"));

            octoCostume[0] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "idle_0.png"));
            octoCostume[1] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "idle_1.png"));
            octoCostume[2] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "idle_2.png"));
            octoCostume[3] = Image.FromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "idle_3.png"));

            // Figure out current active window.
            timeStopwatch = new Stopwatch();
            activeWindow = GetActiveWindow();
            timeStopwatch.Start();
        }

        public void OneTimeInitialization(GooseEntity g)
        {
            Console.WriteLine("Goose OneTimeInitialization");

            // Make goose invisible.
            g.renderData.brushGooseOutline = new SolidBrush(Color.Transparent);
            g.renderData.brushGooseWhite = new SolidBrush(Color.Transparent);
            g.renderData.brushGooseOrange = new SolidBrush(Color.Transparent);
            // g.renderData.shadowBrush = new TextureBrush(transparent);

            g.parameters.StepTimeNormal = 500f;
            g.parameters.StepTimeCharged = 500f;

            // octoCostume[0] = Sonicgoose.Properties.Resources.frame_1;

            InjectionPoints.PreTickEvent -= OneTimeInitialization;
        }

        public void WindowCheck(GooseEntity g)
        {
            bool productive = false;
            bool unproductive = false;
            string checkedWindow = GetActiveWindow();


            if (g.currentTask != API.TaskDatabase.getTaskIndexByID("HappyOctocat"))
            {
                API.Goose.setCurrentTaskByID(g, "HappyOctocat");
                Console.WriteLine("Octocat should have found happiness by now.");
            }



            if (checkedWindow == null) return;
            else checkedWindow = checkedWindow.ToLower();
            if (activeWindow == null)
            {
                activeWindow = checkedWindow;
                return;
            }
            else if (checkedWindow != activeWindow)
            {


                foreach (var windowName in productiveWindows)
                {
                    if (activeWindow.Contains(windowName))
                    {
                        productive = true;
                        break;
                    }
                }
                if (!productive)
                {
                    foreach (var windowName in unproductiveWindows)
                    {
                        if (activeWindow.Contains(windowName))
                        {
                            unproductive = true;
                            break;
                        }
                    }
                }

                if (productive)
                {
                    productiveWork += timeStopwatch.ElapsedMilliseconds;
                    unproductiveWork -= (long)(timeStopwatch.ElapsedMilliseconds * unproductiveRegeration);
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
                string output = checkedWindow + " ProductiveWork: " + productiveWork + " || UnproductiveWork: " + unproductiveWork;
                Console.WriteLine(output);
            }


            //if (g.currentTask == API.TaskDatabase.getTaskIndexByID("HappyOctocat"))
            //{
            //    if (unproductive && unproductiveWork >= unproductiveCap)
            //    {
            //        API.Goose.setCurrentTaskByID(g, "AngryOctocat");
            //    }
            //}
            //else if (g.currentTask == API.TaskDatabase.getTaskIndexByID("AngryOctocat"))
            //{
            //    if (productive)
            //    {
            //        API.Goose.setCurrentTaskByID(g, "HappyOctocat");
            //    }
            //}
            //else
            //{
            //    API.Goose.setCurrentTaskByID(g, "HappyOctocat");
            //}
            //g.render(g, Graphics.FromImage(octocat));
            // logic
        }

        public void OctoPostRender(GooseEntity g, Graphics graph)
        {
            int temp = frameCounter;
            Image currentOcto;

            if (Math.Abs(g.velocity.x) > 1 && Math.Abs(g.velocity.y) > 1)
                temp *= 2;

            if (temp / 10 < 0 || temp / 10 > 3)
            {
                temp = 0;
                frameCounter = 0;
            }

            currentOcto = octoCostume[temp/10];

            var direction = g.direction+90;
            var headPoint = g.rig.bodyCenter;

            var verticalOffset = currentOcto.Height /4;
            var horizontalOffset = currentOcto.Width / 2;

            Bitmap newSprite = RotateImage(new Bitmap(currentOcto), direction);

            graph.DrawImage(newSprite, headPoint.x, headPoint.y);

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
