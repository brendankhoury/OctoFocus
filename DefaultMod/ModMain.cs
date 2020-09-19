using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
// 1. Added the "GooseModdingAPI" project as a reference.
// 2. Compile this.
// 3. Create a folder with this DLL in the root, and *no GooseModdingAPI DLL*
using GooseShared;
using SamEngine;

namespace DefaultMod
{
    public class ModEntryPoint : IMod
    {
        string activeWindow;

        // Gets called automatically, passes in a class that contains pointers to
        // useful functions we need to interface with the goose.
        void IMod.Init()
        {
            // Subscribe to whatever events we want
            InjectionPoints.PostTickEvent += PostTick;
        }

        public void PostTick(GooseEntity g)
        {
            string checkedWindow = GetActiveWindow();
            if (checkedWindow == null) return;
            else if (checkedWindow != activeWindow) {
                activeWindow = checkedWindow;

            }

            // logic
           
            // Do whatever you want here.
            API.Goose.setCurrentTaskByID(g, "FollowMouseDrifty");

            // If we're running our mod's task
            if (g.currentTask == API.TaskDatabase.getTaskIndexByID("FollowMouseDrifty"))
            {
                // Lock our goose facing one direction for some reason?
                g.direction = 0;
            }
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
                Console.WriteLine(Buff.ToString());
                return Buff.ToString();
            }

            return null;
        }
    }
}
