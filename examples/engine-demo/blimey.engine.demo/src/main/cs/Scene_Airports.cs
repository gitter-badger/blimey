// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__  .__                                                     │ \\
// │ \______   \  | |__| _____   ____ ___.__.                               │ \\
// │  |    |  _/  | |  |/     \_/ __ <   |  |                               │ \\
// │  |    |   \  |_|  |  Y Y  \  ___/\___  |                               │ \\
// │  |______  /____/__|__|_|  /\___  > ____|                               │ \\
// │         \/              \/     \/\/                                    │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey Engine (http://www.blimey.io)         │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors:                                                               │ \\
// │ ~ Ash Pook (http://www.ajpook.com)                                     │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining  │ \\
// │ a copy of this software and associated documentation files (the        │ \\
// │ "Software"), to deal in the Software without restriction, including    │ \\
// │ without limitation the rights to use, copy, modify, merge, publish,    │ \\
// │ distribute, sublicense, and/or sellcopies of the Software, and to      │ \\
// │ permit persons to whom the Software is furnished to do so, subject to  │ \\
// │ the following conditions:                                              │ \\
// │                                                                        │ \\
// │ The above copyright notice and this permission notice shall be         │ \\
// │ included in all copies or substantial portions of the Software.        │ \\
// │                                                                        │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,        │ \\
// │ EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF     │ \\
// │ MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. │ \\
// │ IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY   │ \\
// │ CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,   │ \\
// │ TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE       │ \\
// │ SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                 │ \\
// └────────────────────────────────────────────────────────────────────────┘ \\

namespace EngineDemo
{
    using System;
    using Fudge;
    using Abacus.SinglePrecision;
    using Blimey;
    using System.Collections.Generic;
	using System.Linq;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Airport
    {
        public Airport(string[] items)
        {

            ID = int.Parse(items[0]);
            Name = items[1].Replace("\"", string.Empty);
            City = items[2].Replace("\"", string.Empty);
            Country = items[3].Replace("\"", string.Empty);
            Iata = items[4].Replace("\"", string.Empty);
            Icao = items[5].Replace("\"", string.Empty);
            Latitude = float.Parse(items[6]);
            Longitude = float.Parse(items[7]);
            Altitude = int.Parse(items[8]);
            Timezone = float.Parse(items[9]);
            Dst = char.Parse(items[10].Replace("\"", string.Empty));
        }

        public int ID {get; private set; }
        public string Name { get; private set; }
        public string City { get; private set; }
        public string Country { get; private set; }
        public string Iata { get; private set; }
        public string Icao { get; private set; }
        public float Latitude { get; private set; }
        public float Longitude { get; private set; }
        public int Altitude { get; private set; }
        public float Timezone { get; private set; }
        public char Dst { get; private set; }

    }

	public static class ListExtensions
	{
		public static void Shuffle<T>(this IList<T> list)
		{
			Random rng = new Random();
			int n = list.Count;
			while (n > 1) {
				n--;
				int k = rng.Next(n + 1);
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
	}

	public class Scene_Airports
		: Scene
	{
		Scene returnScene;
		Entity earthGo;

		List<Airport> airports = new List<Airport>();

		public override void Start()
		{
            CommonDemoResources.Create (Cor, Blimey);

            var lines = Blimey.Assets.Load <TextAsset> ("assets/airports.bba")
                .Text
                .Split ('\n')
                .ToList ();

            foreach (var line in lines)
			{
				string[] items = line.Split(',');

    			if (items.Length == 11)
    			{
    				airports.Add(new Airport(items));
    			}
            }

			airports.Shuffle ();

			airports = airports.GetRange (0, 250);

			Console.WriteLine("num airports: " + airports.Count);

			this.Configuration.BackgroundColour = Rgba32.Red;

			returnScene = this;

			float radius = 1.5f;
			// create a sprite
			var sphereMesh = new SpherePrimitive(this.Cor.Graphics);


            var mat = new Material("Default",CommonDemoResources.VertexLitShader);
            mat.SetColour("MaterialColour", Rgba32.LightGrey);
			earthGo = this.SceneGraph.CreateSceneObject("earth");

			Entity camSo = SceneGraph.CreateSceneObject ("Scene 5 Camera");
			camSo.AddTrait<CameraTrait>();
			var lookatTrait = camSo.AddTrait<LookAtSubjectTrait>();
			lookatTrait.Subject = Transform.Origin;
			var orbitTrait = camSo.AddTrait<OrbitAroundSubjectTrait>();
			orbitTrait.CameraSubject = Transform.Origin;

			camSo.Transform.LocalPosition = new Vector3(10f,4f,10f);

			this.RuntimeConfiguration.SetRenderPassCameraTo("Debug", camSo);
			this.RuntimeConfiguration.SetRenderPassCameraTo("Default", camSo);

			earthGo.Transform.LocalScale = new Vector3(2 * radius, 2 * radius, 2 * radius);

			var mr = earthGo.AddTrait<MeshRendererTrait>();
            mr.Mesh = sphereMesh.Mesh;
			mr.Material = mat;

            //var mat2 = new Material("Default", CommonDemoResources.UnlitShader);
			//mat2.SetColour("MaterialColour", Rgba32.Blue);

            foreach (var airport in airports)
			{
				var so = this.SceneGraph.CreateSceneObject(airport.Iata);

                so.Transform.Parent = earthGo.Transform;

				var sodr = so.AddTrait<DebugRendererTrait>();
				//sodr.RenderPass = "Default";
				sodr.Colour = Rgba32.Blue;
				//var somr = so.AddTrait<MeshRenderer>();
				//somr.Mesh = sphereMesh;
				//somr.Material = mat2;

				var lat = airport.Latitude;
				var lon = airport.Longitude;

				Vector3 pos = new Vector3(
					radius * Maths.Cos(Maths.ToRadians(lat)),
					radius * Maths.Sin(Maths.ToRadians(lat)),
					0f);

				Single t = Maths.ToRadians (lon);

                Matrix44 rot; Matrix44.CreateRotationY(ref t, out rot);

				Vector3 r; Vector3.Transform(ref pos, ref rot, out r);
				so.Transform.Position = r;
				so.Transform.LocalScale = new Vector3(0.015f, 0.015f, 0.015f);

			}

			this.Blimey.InputEventSystem.Tap += this.OnTap;
		}

		public override void Shutdown()
		{
			this.Blimey.InputEventSystem.Tap -= this.OnTap;
            CommonDemoResources.Destroy ();
		}

		public override Scene Update(AppTime time)
		{
            this.Blimey.DebugRenderer.AddGrid ("Debug");

			if (Cor.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
				Cor.Input.Keyboard.IsFunctionalKeyDown (FunctionalKey.Escape) ||
				Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Backspace))
			{
				returnScene = new Scene_MainMenu ();
			}


			return returnScene;
		}

		void OnTap(Gesture gesture)
		{
			returnScene = new Scene_MainMenu();
		}
	}
}

