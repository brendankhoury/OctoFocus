using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GooseShared;
using SamEngine;

namespace DefaultMod
{
    // This is a "Task" class. It defines an AI state for the goose.
    // It automatically gets indexed by the mod loader due to being a subclass of GooseTaskInfo.

    // Inside, we define a data structure that persists while the task is running.
    // We also program its 'tick' function, which gets called every frame.

    // 1. Subclass "GooseTaskInfo"
    class TaskAngryOctocat : GooseTaskInfo
    {


        // 2. Construct this task.
        public TaskAngryOctocat()
        {
            // Should this Task be picked at random by the goose?
            canBePickedRandomly = false;

            // Describes the task, for readability by other interfaces.
            shortName = "Angry Octocat";
            description = "The Octocat is angry because its user is not productive!";

            // The key used to access this task in the GooseTaskDatabase:
            // "Task.GetTaskByID" in the API takes this as an argument.
            taskID = "AngryOctocat";
            // Hot tip: can be nice to set this from a public constant string. Easier access by other parts of your mod.
        }

        // 3. Define the task's 'data' / state.
        // This defines the 'state' of the Task.
        public class AngryOctocatTaskData : GooseTaskData
        {
            public float timeStarted;

        }

        // 4. Override "GetNewTaskData"
        // Create a 'blank state' on the given goose. Called just before the Task begins running.
        public override GooseTaskData GetNewTaskData(GooseEntity goose)
        {
            AngryOctocatTaskData taskData = new AngryOctocatTaskData();

            //SystemParameters sysParam = new SystemParameters();
            taskData.timeStarted = Time.time;

            return taskData;
        }

        // 4. Override "RunTask"
        // Run a frame of this Task on the given goose.
        public override void RunTask(GooseEntity goose)
        {
            AngryOctocatTaskData data = (AngryOctocatTaskData)goose.currentTaskData;

            Vector2 mousePos = new Vector2(Input.mouseX, Input.mouseY);

            if (Vector2.Distance(mousePos, goose.rig.bodyCenter) < 50)
            {
                API.Goose.setCurrentTaskByID(goose, "GrabbingOctocat");
                //Console.WriteLine("Changed To grabbing cursor");
            }
            //if (Time.time - data.timeStarted > 10)
            //{
            //    API.Goose.setCurrentTaskByID(goose, "HappyOctocat");

            //}
            API.Goose.setSpeed(goose, GooseEntity.SpeedTiers.Charge);

            goose.targetPos = mousePos; 
        }
    }
}
