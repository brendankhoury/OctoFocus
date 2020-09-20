<<<<<<< HEAD
## Inspiration
We were inspired by the funny but useless DesktopGoose application. DesktopGoose places a goose on the screen which preforms random actions, pulling open notes, memes, moving the cursor etc. We also set out to solve a common problem: Getting distracted while coding.

## What it does
Octofocus creates an Octocat in the bottom right corner of the users screen. Whenever the Octocat detects the user is not staying on task, it gets angry, steals the cursor, and moves the cursor to the minimize button on the current window. 

## How we built it
OctoFocus was written in C# as a mod for DesktopGoose.
<br/>
<br/>
To detect whether the user was on task or not, we did the following. We defined a set of applications/websites to be productive and another set to be unproductive. From there, we compared the title of the active window to the sets. If the title is productive, then the Octocat becomes happy. If the title is unproductive and the user stays on the application for more than 15 seconds, the AI controlling the Octocat switches to angry which follows the cursor, grabs it and places it on the minimize button.

## Challenges we ran into
It took us a while to understand the API because it had almost no documentation. One of our members also didn't know any C#. Another issue surrounded the rotation of Octocat. Whenever we tried to rotate the bitmap, an orange halo would form around the sprite. After  a couple hours of debugging, we decided to move on and finish the project.

## Accomplishments that we're proud of
We are proud of how we were able to figure out the API despite it not being very clear. We also are proud of our idea as it is a fairly unique take to keeping users on task.

## What we learned
We learned a lot about C# and understanding how to use an API without documentation. 


## What's next for OctoFocus
In the future, OctoFocus could be expanded to include more animations and emotions. Rewarding users for being on task, giving words of encouragement, etc. It would be funny if the Octocat would drag windows around the more a user stays off task. Finally, implementing a config file for identifying which applications are productive/unproductive.
=======
# OctoFocus
Keep coding with OctoCat to keep you on track!
>>>>>>> dev
