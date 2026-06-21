The project this code was made for was a first person drone simulator.
I worked on this project through an assignment during the second half of one of my Fontys ICT Game Design & Technology semesters.
My tasks for that project were only for the tutorial, but systems that could be made more versatile (modular, expandable, usable in other levels & projects) were made more versatile (which the developers of the company this project was from liked a lot).
Here are some of the main tasks I completed for the project:
-3D tutorial level environment.
-Task system.
-Pop-up system.
-Tutorial flow design.
-Drone auto correction movement system for after crashes.
-User tests for the tasks above.

The task system was made to be versatile and easy to use and expand upon by developers.
Here are some of the requirements I set up for the task system:
-Easy to use by other developers.
-Easy to expand upon like adding new types of conditions in code and finding them in the code.
-Tasks can have multiple conditions (like collide with the landing platform and have the motor turned off).
-Completion with multiple conditions needs some control for developers (like having to complete 1 or any out of all conditions).
-Task initiation and completion need events (could be used for showing/hiding info pop-ups, starting tasks in order or setting up the environment/location markers etc.)
-Existing condition types need to allow the first designed tutorial level to work, other condition types can be added later.

How I went about making it easy to use:
-Unity editor script that adjusts the UI with clear buttons and layout (tested with peers at Fontys ICT).
-Allowing tasks to be managed from one point in the scene (a game object with a clear name related to tasks with children of which the names match the task can be an example).
-Not a lot of different components required (like a manager, separate conditions etc), the system is simplified by only using one Task component per task.
A manager could be useful for handling the order of tasks etc, but it was not necesary for this part of the project.
-Different types of conditions can be added through a single button with options.

How I went about making it easy to expand upon:
-The condition types are all derrived from a single abstract class with overridable boolean used for all the checks.
-There can be condition sub-categories as long as the sub-category class derrives from the abstract class.
-Code for condition types is split into files by category.
-Tasks only check conditions per frame when it is enabled which prevents unnecesary code from constantly running.

Within the code of the classes "TaskRotationCondition" and "TaskMoveTrackerCondition" there is mention of a class called "TransformTracker".
The TransformTracker is a component I made to track movement and rotation, but it has some references to project specific code I am not sure I am allowed to show.
Here is a description of what it does:
Per axis for movement (XYZ) and rotation (XYZ) it has positive and negative values (using a struct for consistency).
Per axis for both movement and rotation there are booleans to enable/disable tracking.
Tracking can be done on local- or worldspace.
Values that are enabled update every frame using methods, others get ignored.
Each time an axis is enabled its values gets reset to 0 first.
Position gets tracked using standard Unity units 1 = 1 meter.
Rotation gets tracked using an angle between 0 and 359 (because 0 = 360) per axis (pitch, yaw, tilt) with some math to smoothly calculate the angle correctly because Unity uses Quaternion which uses 4 values from 0 to 1 for angles instead of 3 values from 0 to 359.