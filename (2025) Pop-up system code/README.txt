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

The pop-up system was made to be easy to use for showing content on the side of the screen to users temporarily.
Here are some of the requirements I set up for the pop-up system:
-Clearly visible title field (allows users to get info on what the pop-up is about fast).
-An optional description text visible to users (for more details on the subject).
-Option for developers to add (multiple) images to the pop-up message (research indicated that images can get some information across a lot faster than text).
-Scalable UI (meaning that the UI scales depending on content it is showing, but pop-up size should also be easy to alter by developers).
-Pop-ups should enter and exit the screen in a visibly clear manner (so users can see in the corner of their eyes there is something coming/going).
-Pop-ups should not clutter the screen and/or make it harder to play the simulation because of where on the screen they are located or how they are colored.
-The colors used for the UI should fit the "military drone simulator" theme while not distracting the player too much while also remaining clearly visible.
-Text on the pop-up should remain easy to read for the average user (readable font, font size large enough for the intended screen size, font color contrast with background, color clashing etc).
-It should be easy for developers to create a new pop-up message and it should also be easy to control for them when the pop-up enters/exits the screen.
-Allow developers to easily alter how the UI looks and behaves with few steps.
-Optional: Multiple pop-ups should be allowed on screen (in case it is ever used for quests and there can be multiple quests).

How I validated the clarity, visuals and functionality:
1-Peer reviews with group members and classmates.
2-User tests (with peers and teachers at Fontys ICT)
3-Requesting feedback and requirements from stakeholders.
4-User tests (at the Innovations Insights event where many people interested in IT projects could test the entire tutorial prototype).

How the UI is made scalable:
The UI is all put into a UI prefab that can be put in a canvas (Unity's standard UI parent object).
Transform settings on the object are set to a percentage of the screen width and put on the right position to not overlap with existing UI.
Changing the prefab will change it across all of the instances where the prefab is used, but if a variant of the prefab is made, looks and sizes can be changed for specific scenes.
To make the UI fit it's content, I made use of vertical layouts and content size fitters where needed.
I also wrote a small custom script that can be used to make image objects scale to fit the aspect ratio of the file that is put into the image object to prevent stretching.

How the system works:
There is a Pop-Up_Panel object which is the main object of a prefab and has a PopUpManager class.
Any requests from show/hide to what content should be put on the next pop-up is handled through this class.
The manager assigns received pop-up profile data to the right PopUpUIController child objects and keeps them show up in the right order.
The UI controller invokes an event with the data as output when the manager gives it data.
In the prefab, that event calls a PopUpDataController script method (that class is on the same object on the prefab).
The data controller then assigns all data to the text/images etc.
Once that data is assigned, the UI controller will then slide the pop-up into view.
Closing any pop-up requires the manager to receive data a pop-up so the pop-up with that data will slide out of view.

How multiple pop-ups are handled:
Developers can put as many pop-up prefabs under the manager as they want and that will act as the maximum amount of pop-ups on screen at a time.
As long as the pop-up child objects have the PopUpUIController class on them they will be detected and listed as pop-up before the first frame during gameplay (on Awake()).
This also allows developers to see how much of the screen it may take up before pressing play.