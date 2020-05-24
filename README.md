# CommandLineFPS.NET
A First Person Shooter at the command line? Yup...

This is a port of the nifty little console-based FPS to something a bit more 21st century - .NET Core 😜  It's more-or-less a direct port with some light refactoring. Can be refactored a lot more and made a lot more SOLID. I thought it would be fun little exercise to try this as I haven't used C++ in man years so thought it would be a fun exercise. Somethings are much easier (like types) but other things, like console input seem much harder. It probably took me longer to port it than @Javidx9 to write it

This was designed for MS Windows but will run on Linux (and presumably OSX) but the config values need updating to speed it up.

See license in CommandLineFPS.cs for more info.

## Usage

Set your console to be 120 x 41 (see below)

W = Forward  
A = Turn anti-clockwise  
S = Backward  
D = Turn clockwise  
Esc = Quit

## Issues

 - Because the Windows console input consumes a character on screen (even when cursor is hidden), it rolls over to the next line, causing it to flicker. So you need to either add an extra line to your console window than your game screen. Not sure if there is a way around this?

 - Running it on Windows (even in release mode) the FPS seems quite slow at around 70fps. Running it on Linux runs a lot higher ~3000fps. Not sure if Windows is capping it or the way I ported the game timer.

 ## Thanks

 Many thanks to @Javidx9 for his excellent video - a great primer into basic 3D graphics and maths. 
