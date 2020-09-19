using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using GooseShared;
using SamEngine;

namespace DefaultMod
{
    // This is a "Task" class. It defines an AI state for the goose.
    // It automatically gets indexed by the mod loader due to being a subclass of GooseTaskInfo.

    // Inside, we define a data structure that persists while the task is running.
    // We also program its 'tick' function, which gets called every frame.

    // 1. Subclass "GooseTaskInfo"
    class HappyOctocat : GooseTaskInfo
    {

        // 2. Construct this task.
        public HappyOctocat()
        {
            // Should this Task be picked at random by the goose?
            canBePickedRandomly = false;

            // Describes the task, for readability by other interfaces.
            shortName = "Octocat is happy.";
            description = "The octocat is happy because its user is in a good mood.";

            // The key used to access this task in the GooseTaskDatabase:
            // "Task.GetTaskByID" in the API takes this as an argument.
            taskID = "HappyOctocat";
            // Hot tip: can be nice to set this from a public constant string. Easier access by other parts of your mod.
        }
        // 3. Define the task's 'data' / state.
        // This defines the 'state' of the Task.
        public class HappyOctocatTaskData : GooseTaskData
        {
            public int screenHeight;
            public int screenWidth;
        }

        // 4. Override "GetNewTaskData"
        // Create a 'blank state' on the given goose. Called just before the Task begins running.
        public override GooseTaskData GetNewTaskData(GooseEntity goose)
        {
            HappyOctocatTaskData taskData = new HappyOctocatTaskData();
           
            taskData.screenHeight = (int) SystemParameters.WorkArea.Height;
            taskData.screenWidth = (int) SystemParameters.WorkArea.Width;

            Console.WriteLine(taskData.screenHeight + " " + taskData.screenWidth);

            return taskData;
        }

        // 4. Override "RunTask"
        // Run a frame of this Task on the given goose.
        public override void RunTask(GooseEntity goose)
        {
            // This function is only called when we're the currently running task.
            // The goose's taskData will be of this task's type.
            HappyOctocatTaskData data = (HappyOctocatTaskData)goose.currentTaskData;

            //goose.currentAcceleration = 1000;
            //goose.setSpeed(100);
            int offsetFromLeft = 70;
            Vector2 bottomRightCorner = new Vector2(data.screenWidth - offsetFromLeft, data.screenHeight - 150);
            if (Vector2.Distance(bottomRightCorner, goose.rig.bodyCenter) < 100 && Vector2.Distance(bottomRightCorner, goose.rig.bodyCenter) > 30)
            {
                API.Goose.setSpeed(goose, GooseEntity.SpeedTiers.Walk);
                // Set code which stops the octocat from moving if it gets close enough to the poin
                goose.targetPos = bottomRightCorner;
            } else if (Vector2.Distance(bottomRightCorner, goose.rig.bodyCenter) >= 100)
            {
                API.Goose.setSpeed(goose, GooseEntity.SpeedTiers.Charge);
                // Set code which stops the octocat from moving if it gets close enough to the poin
                goose.targetPos = bottomRightCorner;
            }

        }
    }
}
