using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using GooseShared;
using SamEngine;

namespace DefaultMod
{
    // This is a "Task" class. It defines an AI state for the goose.
    // It automatically gets indexed by the mod loader due to being a subclass of GooseTaskInfo.

    // Inside, we define a data structure that persists while the task is running.
    // We also program its 'tick' function, which gets called every frame.

    // 1. Subclass "GooseTaskInfo"
    class TaskGrabbingOctocat : GooseTaskInfo
    {


        // 2. Construct this task.
        public TaskGrabbingOctocat()
        {
            // Should this Task be picked at random by the goose?
            canBePickedRandomly = false;

            // Describes the task, for readability by other interfaces.
            shortName = "Octocat grabs mouse";
            description = "Octocat grabs mouse and sends it near the minimize button.";

            // The key used to access this task in the GooseTaskDatabase:
            // "Task.GetTaskByID" in the API takes this as an argument.
            taskID = "GrabbingOctocat";
            // Hot tip: can be nice to set this from a public constant string. Easier access by other parts of your mod.
        }


        // 3. Define the task's 'data' / state.
        // This defines the 'state' of the Task.
        public class GrabbingOctocatTaskData : GooseTaskData
        {
            public float timeStarted;
            public Vector2 activeWindowTopRightCorner;
        }


        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);


        // 4. Override "GetNewTaskData"
        // Create a 'blank state' on the given goose. Called just before the Task begins running.
        public override GooseTaskData GetNewTaskData(GooseEntity goose)
        {
            GrabbingOctocatTaskData taskData = new GrabbingOctocatTaskData();
            IntPtr activeWindow = GetForegroundWindow();
            Rect lpRect = new Rect();
            bool sucess = GetWindowRect(activeWindow, out lpRect);

            string output = "Top: " + lpRect.Top + " Right: " + lpRect.Right;
            Console.WriteLine(output);


            taskData.activeWindowTopRightCorner = new Vector2((float)lpRect.Right, (float)lpRect.Top);

            return taskData;
        }

        // 4. Override "RunTask"
        // Run a frame of this Task on the given goose.
        public override void RunTask(GooseEntity goose)
        {
            // This function is only called when we're the currently running task.
            // The goose's taskData will be of this task's type.
            GrabbingOctocatTaskData data = (GrabbingOctocatTaskData)goose.currentTaskData;
            goose.targetPos = data.activeWindowTopRightCorner;

            //TODO, detect if near top right, if near top right, sleep for a bit :/
            /*goose.currentTaskData.activeWindow = GetActiveWindow();*/
        }
    }
}
