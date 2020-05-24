/*
	OneLoneCoder.com - Command Line First Person Shooter (FPS) Engine
	"Why were games not done like this is 1990?" - @Javidx9

	Ported to .NET Core by Gavin Ward (@PunkRockCoder)

	License
	~~~~~~~
	Copyright (C) 2018  Javidx9
	This program comes with ABSOLUTELY NO WARRANTY.
	This is free software, and you are welcome to redistribute it
	under certain conditions; See license for details. 
	Original works located at:
	https://www.github.com/onelonecoder
	https://www.onelonecoder.com
	https://www.youtube.com/javidx9

	GNU GPLv3
	https://github.com/OneLoneCoder/videos/blob/master/LICENSE

	From Javidx9 :)
	~~~~~~~~~~~~~~~
	Hello! Ultimately I don't care what you use this for. It's intended to be 
	educational, and perhaps to the oddly minded - a little bit of fun. 
	Please hack this, change it and use it in any way you see fit. You acknowledge 
	that I am not responsible for anything bad that happens as a result of 
	your actions. However this code is protected by GNU GPLv3, see the license in the
	github repo. This means you must attribute me if you use it. You can view this
	license here: https://github.com/OneLoneCoder/videos/blob/master/LICENSE
	Cheers!

	Background
	~~~~~~~~~~
	Whilst waiting for TheMexicanRunner to start the finale of his NesMania project,
	his Twitch stream had a counter counting down for a couple of hours until it started.
	With some time on my hands, I thought it might be fun to see what the graphical
	capabilities of the console are. Turns out, not very much, but hey, it's nice to think
	Wolfenstein could have existed a few years earlier, and in just ~200 lines of code.

	IMPORTANT!!!!
	~~~~~~~~~~~~~
	READ ME BEFORE RUNNING!!! This program expects the console dimensions to be set to 
	120 Columns by 41 Rows. I recommend a small font "Consolas" at size 16. You can do this
	by running the program, and right clicking on the console title bar, and specifying 
	the properties. You can also choose to default to them in the future.
	
	Controls: A = Turn Left, D = Turn Right, W = Walk Forwards, S = Walk Backwards, Esc = Quit

	Future Modifications
	~~~~~~~~~~~~~~~~~~~~
	1) Shade block segments based on angle from player, i.e. less light reflected off
	walls at side of player. Walls straight on are brightest.
	2) Find an interesting and optimised ray-tracing method. I'm sure one must exist
	to more optimally search the map space
	3) Add bullets!
	4) Add bad guys!

	Author
	~~~~~~
	Twitter: @javidx9
	Blog: www.onelonecoder.com

	Video:
	~~~~~~	
	https://youtu.be/xW8skO7MFYw

	Last Updated: 27/02/2017
*/



namespace CommandLineFPSDotNET
{

using System;
using System.Diagnostics;

    class Program
    {
        static void Main(string[] args)
        {
			Console.CursorVisible = false;
			var game = new CommandLineFPSDotNETEngine();

			while (true)
			{
				
				char keyPress;

				if(!Console.KeyAvailable)
				{
					keyPress = ' ';
				}
				else
				{
					var key = Console.ReadKey(true);

					if (key.Key == ConsoleKey.Escape)
						return;

					keyPress = key.KeyChar;
				}

				Console.SetCursorPosition(0,0);
				Console.Write(game.NextFrame(keyPress));
			
			}
		}

    }
}
