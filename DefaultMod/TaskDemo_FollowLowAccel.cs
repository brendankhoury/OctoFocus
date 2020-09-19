using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
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
    class CheckWindowTask : GooseTaskInfo
    {
        

        // 2. Construct this task.
        public CheckWindowTask()
        {
            // Should this Task be picked at random by the goose?
            canBePickedRandomly = false;

            // Describes the task, for readability by other interfaces.
            shortName = "Check Window task";
            description = "This is a demo task that checks what application is open.";

            // The key used to access this task in the GooseTaskDatabase:
            // "Task.GetTaskByID" in the API takes this as an argument.
            taskID = "CheckWindowTask";
            // Hot tip: can be nice to set this from a public constant string. Easier access by other parts of your mod.
        }


        // 3. Define the task's 'data' / state.
        // This defines the 'state' of the Task.
        public class CheckWindowTaskData : GooseTaskData
        {
            public float timeStarted;
            public string activeWindow;
        }

        // 4. Override "GetNewTaskData"
        // Create a 'blank state' on the given goose. Called just before the Task begins running.
        public override GooseTaskData GetNewTaskData(GooseEntity goose)
        {
            CheckWindowTaskData taskData = new CheckWindowTaskData();
            taskData.timeStarted = Time.time;
            return taskData;
        }

        // 4. Override "RunTask"
        // Run a frame of this Task on the given goose.
        public override void RunTask(GooseEntity goose)
        {
            // This function is only called when we're the currently running task.
            // The goose's taskData will be of this task's type.
            CheckWindowTaskData data = (CheckWindowTaskData)goose.currentTaskData;

            /*goose.currentTaskData.activeWindow = GetActiveWindow();*/
        }
    }
}
