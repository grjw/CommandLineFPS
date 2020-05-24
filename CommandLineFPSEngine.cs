using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace CommandLineFPSDotNET
{
	public class CommandLineFPSDotNETEngine
	{

		readonly int screenWidth = 120;			// Console Screen Size X (columns)
		readonly int screenHeight = 40;			// Console Screen Size Y (rows)
		readonly int mapWidth = 16;				// World Dimensions
		readonly int mapHeight = 16;
		readonly float fov = 3.14159f / 4.0f;	// Field of View
		readonly float depth = 16.0f;			// Maximum rendering distance
		readonly float speed = 5.0f;			// Walking Speed
		readonly float rotationArc = 0.75f;

		string map = String.Empty;
		char[] screen;
		Stopwatch timer;
		TimeSpan previousFrameTime;

		Player player;
		
		public CommandLineFPSDotNETEngine()
		{
			// Create Screen Buffer
			screen = new char[screenWidth*screenHeight];

			// Create Map of world space # = wall block, . = space
			map += "#########.......";
			map += "#...............";
			map += "#.......########";
			map += "#..............#";
			map += "#......##......#";
			map += "#......##......#";
			map += "#..............#";
			map += "###............#";
			map += "##.............#";
			map += "#......####..###";
			map += "#......#.......#";
			map += "#......#.......#";
			map += "#..............#";
			map += "#......#########";
			map += "#..............#";
			map += "################";

			player = new Player() { X = 14.7f, Y = 5.09f, Angle = 0.0f};

			timer = Stopwatch.StartNew();

		}

		public string NextFrame (char keyPressed)
		{

			// We'll need time differential per frame to calculate modification
			// to movement speeds, to ensure consistant movement, as ray-tracing
			// is non-deterministic
			var timeDifference = timer.Elapsed - previousFrameTime;
			previousFrameTime = timer.Elapsed;
			float elapsedTime = timeDifference.Ticks / (float)Stopwatch.Frequency;

			var keyPressedStr = keyPressed.ToString().ToUpper();

			// Handle CCW Rotation
			if (keyPressedStr == "A")
				player.Angle -= (speed * rotationArc) * elapsedTime;

			// Handle CW Rotation
			if (keyPressedStr == "D")
				player.Angle += (speed * rotationArc) * elapsedTime;
			
			// Handle Forwards movement & collision
			if (keyPressedStr == "W")
			{
				player.X += (float)Math.Sin(player.Angle) * speed * elapsedTime;
				player.Y += (float)Math.Cos(player.Angle) * speed * elapsedTime;
				if (map[(int)player.X * mapWidth + (int)player.Y] == '#')
				{
					player.X -= (float)Math.Sin(player.Angle) * speed * elapsedTime;
					player.Y -= (float)Math.Cos(player.Angle) * speed * elapsedTime;
				}			
			}

			// Handle backwards movement & collision
			if (keyPressedStr == "S")
			{
				player.X -= (float)Math.Sin(player.Angle) * speed * elapsedTime;
				player.Y -= (float)Math.Cos(player.Angle) * speed * elapsedTime;
				if (map[(int)player.X * mapWidth + (int)player.Y] == '#')
				{
					player.X += (float)Math.Sin(player.Angle) * speed * elapsedTime;
					player.Y += (float)Math.Cos(player.Angle) * speed * elapsedTime;
				}
			}

			for (int x = 0; x < screenWidth; x++)
			{
				// For each column, calculate the projected ray angle into world space
				float rayAngle = (player.Angle - fov/2.0f) + ((float)x / (float)screenWidth) * fov;

				// Find distance to wall
				float stepSize = 0.1f;		  // Increment size for ray casting, decrease to increase										
				float distanceToWall = 0.0f; //                                      resolution

				bool hitWall = false;		// Set when ray hits wall block
				bool boundary = false;		// Set when ray hits boundary between two wall blocks

				float eyeX = (float)Math.Sin(rayAngle); // Unit vector for ray in player space
				float eyeY = (float)Math.Cos(rayAngle);

				// Incrementally cast ray from player, along ray angle, testing for 
				// intersection with a block
				while (!hitWall && distanceToWall < depth)
				{
					distanceToWall += stepSize;
					int testX = (int)(player.X + eyeX * distanceToWall);
					int testY = (int)(player.Y + eyeY * distanceToWall);
					
					// Test if ray is out of bounds
					if (testX < 0 || testX >= mapWidth || testY < 0 || testY >= mapHeight)
					{
						hitWall = true;			// Just set distance to maximum depth
						distanceToWall = depth;
					}
					else
					{
						// Ray is inbounds so test to see if the ray cell is a wall block
						if (map[testX * mapWidth + testY] == '#')
						{
							// Ray has hit wall
							hitWall = true;

							// To highlight tile boundaries, cast a ray from each corner
							// of the tile, to the player. The more coincident this ray
							// is to the rendering ray, the closer we are to a tile 
							// boundary, which we'll shade to add detail to the walls
							var p = new List<(float,float)>();

							// Test each corner of hit tile, storing the distance from
							// the player, and the calculated dot product of the two rays
							for (int tx = 0; tx < 2; tx++)
							{
								for (int ty = 0; ty < 2; ty++)
								{
									// Angle of corner to eye
									float vy = (float)testY + ty - player.Y;
									float vx = (float)testX + tx - player.X;
									float d = (float)Math.Sqrt(vx*vx + vy*vy); 
									float dot = (eyeX * vx / d) + (eyeY * vy / d);
									p.Add((d, dot));
								}
							}

							// Sort Pairs from closest to farthest
							p.Sort((x, y) => x.Item1 < y.Item1 ? -1 : 1);
							
							// First two/three are closest (we will never see all four)
							float bound = 0.01f;
							if (Math.Acos(p[0].Item2) < bound) boundary = true;
							if (Math.Acos(p[1].Item2) < bound) boundary = true;
							if (Math.Acos(p[2].Item2) < bound) boundary = true;
						}
					}
				}
			
				// Calculate distance to ceiling and floor
				int ceiling = (int)((screenHeight/2) - (screenHeight / distanceToWall));
				int floor = screenHeight - ceiling;

				// Shader walls based on distance
				char wallShade = WallShade.Distant;
				if (distanceToWall <= depth / 4.0f)			wallShade = WallShade.Close;	// Very close	
				else if (distanceToWall < depth / 3.0f)		wallShade = WallShade.Medium;
				else if (distanceToWall < depth / 2.0f)		wallShade = WallShade.Far;
				else if (distanceToWall < depth)			wallShade = WallShade.VeryFar;
				else										wallShade = WallShade.Distant;		// Too far away

				if (boundary)		
					wallShade = WallShade.Distant; // Black it out
				
				for (int y = 0; y < screenHeight; y++)
				{
					// Each Row
					if(y <= ceiling)
						screen[y*screenWidth + x] = ' ';
					else if(y > ceiling && y <= floor)
						screen[y*screenWidth + x] = wallShade;
					else // Floor
					{	
						char floorShade = FloorShade.Distant;		
						// Shade floor based on distance
						float b = 1.0f - (((float)y -screenHeight/2.0f) / ((float)screenHeight / 2.0f));
						if (b < 0.25)		floorShade = FloorShade.Close;
						else if (b < 0.5)	floorShade = FloorShade.Medium;
						else if (b < 0.75)	floorShade = FloorShade.Far;
						else if (b < 0.9)	floorShade = FloorShade.VeryFar;
						else				floorShade = FloorShade.Distant;
						screen[y*screenWidth + x] = floorShade;
					}
				}
			}

			// Display Stats
			var stats = $"X={player.X}, Y={player.Y}, A={player.Angle} FPS={1.0f/elapsedTime}";

			for (int i = 0; i < stats.Length; i++)
			{
				screen[i] = stats[i];
			}

			// Display Map
			for (int nx = 0; nx < mapWidth; nx++)
			{
				for (int ny = 0; ny < mapWidth; ny++)
				{
					screen[(ny+1)*screenWidth + nx] = map[ny * mapWidth + nx];
				}
			}
			screen[((int)player.X+1) * screenWidth + (int)player.Y] = 'P';

			return new String(screen);
		}
	}
}

		