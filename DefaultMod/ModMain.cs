using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Diagnostics;
using System.IO;
// 1. Added the "GooseModdingAPI" project as a reference.
// 2. Compile this.
// 3. Create a folder with this DLL in the root, and *no GooseModdingAPI DLL*
using GooseShared;
using SamEngine;

namespace DefaultMod
{
    public class ModEntryPoint : IMod
    {
        StringBuilder logString = new StringBuilder();

        long productiveWork = 0;
        long unproductiveWork = 0;
        long nahMan = 0;
        string[] productiveWindows = { "visual studio", "stack overflow", "github", "vs code", "google", "duckduckgo" };
        string[] unproductiveWindows = { "steam", "facebook", "reddit", "discord", "youtube", "instagram", "league of legends" };

        string activeWindow;
        Stopwatch timeStopwatch;
        //Image octocat = Image.FromFile("octocat.jpg");

        // Gets called automatically, passes in a class that contains pointers to
        // useful functions we need to interface with the goose.
        void IMod.Init()
        {
            // Subscribe to whatever events we want
            InjectionPoints.PostTickEvent += PostTick;

            // Figure out current active window.
            timeStopwatch = new Stopwatch();
            activeWindow = GetActiveWindow();
            timeStopwatch.Start();
        }

        public void PostTick(GooseEntity g)
        {
            string checkedWindow = GetActiveWindow();
            if (checkedWindow == null) return;
            else checkedWindow = checkedWindow.ToLower();
            if (activeWindow == null)
            {
                activeWindow = checkedWindow;
                return;
            }
            else if (checkedWindow != activeWindow)
            {
                // windowAnalysis[activeWindow] += timeStopwatch.ElapsedMilliseconds;

                bool productive = false;
                bool unproductive = false;

                foreach (var windowName in productiveWindows )
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
                } else if (unproductive) 
                { 
                    unproductiveWork += timeStopwatch.ElapsedMilliseconds;
                } else
                {
                    nahMan += timeStopwatch.ElapsedMilliseconds;
                }
                
                logString.AppendLine(checkedWindow + " ProductiveWork: " + productiveWork + " || UnproductiveWork: " + unproductiveWork);
                activeWindow = checkedWindow;
                timeStopwatch.Restart();
                string output = checkedWindow + " ProductiveWork: " + productiveWork + " || UnproductiveWork: " + unproductiveWork;
                Console.WriteLine(output);
/*                try
                {
                    string logPath = "C:\\Users\\Unit 01\\Documents\\logfile.txt";
                    // Path.Combine(Directory.GetCurrentDirectory(), "logFile.txt")
                    //Path.Combine(Directory.GetCurrentDirectory(), "logFile.txt")
                    if (!File.Exists(logPath))
                    {
                        File.Create(logPath);
                    }
                    File.WriteAllText(logPath, logString.ToString());

                }
                catch
                {
                    Environment.Exit(1);
                }*/

            }
            //g.render(g, Graphics.FromImage(octocat));
            // logic

            // Do whatever you want here.
            //API.Goose.setCurrentTaskByID(g, "FollowMouseDrifty");

            // If we're running our mod's task
/*            if (g.currentTask == API.TaskDatabase.getTaskIndexByID("FollowMouseDrifty"))
            {
                // Lock our goose facing one direction for some reason?
                g.direction = 0;
            }*/
        }


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
                /*Console.WriteLine(Buff.ToString());*/
                return Buff.ToString();
            }

            return null;
        }
    }
}
