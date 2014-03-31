// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Blimey - Fast, efficient, high level engine built upon Cor & Abacus    │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Brought to you by:                                                     │ \\
// │          _________                    .__               __             │ \\
// │         /   _____/__ __  ____    ____ |__|____    _____/  |_           │ \\
// │         \_____  \|  |  \/    \  / ___\|  \__  \  /    \   __\          │ \\
// │         /        \  |  /   |  \/ /_/  >  |/ __ \|   |  \  |            │ \\
// │        /_______  /____/|___|  /\___  /|__(____  /___|  /__|            │ \\
// │                \/           \//_____/         \/     \/                │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2013 A.J.Pook (http://sungiant.github.com)                 │ \\
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

using System;
using Abacus;
using Abacus.Packed;
using Abacus.SinglePrecision;
using Sungiant.Cor;
using System.Collections.Generic;
using System.IO;

namespace Sungiant.Blimey.Demo
{
	public class Scene5
		: Scene
	{
		Scene returnScene;
		SceneObject earthGo;

		GridRenderer gr;

		readonly List<Airport> airports = new List<Airport>();

		public override void Start()
		{
			gr = new GridRenderer(this.Blimey.DebugShapeRenderer, "Debug");


			using (var streamReader = this.Cor.Resources.Open<StreamReader>("resources/airports.dat"))
			{
				while (!streamReader.EndOfStream)
				{
					string line = streamReader.ReadLine();

					string[] items = line.Split(',');

					if (items.Length == 11)
					{
						airports.Add(new Airport(items));
					}
				}

			}
			Console.WriteLine("num airports: " + airports.Count);


			this.Settings.BackgroundColour = Rgba32.Red;

			returnScene = this;

			float radius = 1.5f;
			// create a sprite
			var sphereMesh = new SpherePrimitive(this.Cor.Graphics);
			var shader = this.Cor.Resources.LoadShader(ShaderType.VertexLit);
			var mat = new Material("Default",shader);
            mat.SetColour("MaterialColour", Rgba32.LightGrey);
			earthGo = this.CreateSceneObject("earth");

			SceneObject camSo = CreateSceneObject ("Scene 5 Camera");
			var camTrait = camSo.AddTrait<Camera>();
			var lookatTrait = camSo.AddTrait<LookAtSubject>();
			lookatTrait.Subject = Transform.Origin;
			var orbitTrait = camSo.AddTrait<OrbitAroundSubject>();
			orbitTrait.CameraSubject = Transform.Origin;

			camSo.Transform.LocalPosition = new Vector3(10f,4f,10f);

			this.SetRenderPassCameraTo("Debug", camSo);
			this.SetRenderPassCameraTo("Default", camSo);

			earthGo.Transform.LocalScale = new Vector3(2 * radius, 2 * radius, 2 * radius);

			var mr = earthGo.AddTrait<MeshRenderer>();
			mr.Mesh = sphereMesh;
			mr.Material = mat;

			var shader2 = this.Cor.Resources.LoadShader(ShaderType.Unlit);
			var mat2 = new Material("Default", shader2);
			mat2.SetColour("MaterialColour", Rgba32.Blue);

            foreach (var airport in airports)
			{
				var so = this.CreateSceneObject(airport.Iata);

                so.Transform.Parent = earthGo.Transform;


				var somr = so.AddTrait<MeshRenderer>();
				somr.Mesh = sphereMesh;
				somr.Material = mat2;

				var lat = airport.Latitude;
				var lon = airport.Longitude;

				Vector3 pos = new Vector3(
					radius * RealMaths.Cos(RealMaths.ToRadians(lat)),
					radius * RealMaths.Sin(RealMaths.ToRadians(lat)),
					0f);

				Single t = RealMaths.ToRadians (lon);

                Matrix44 rot; Matrix44.CreateRotationY(ref t, out rot);

				Vector3 r; Vector3.Transform(ref pos, ref rot, out r);
				so.Transform.Position = r;
				so.Transform.LocalScale = new Vector3(0.005f, 0.005f, 0.005f);

			}

			this.Blimey.InputEventSystem.Tap += this.OnTap;
		}

		public override void Shutdown()
		{
			this.Blimey.InputEventSystem.Tap -= this.OnTap;
		}

		public override Scene Update(AppTime time)
		{
			gr.Update ();

			if (Cor.Input.GenericGamepad.East == ButtonState.Pressed ||
			    Cor.Input.Keyboard.IsFunctionalKeyDown (FunctionalKey.Escape))
			{
				returnScene = new MainMenuScene ();
			}


			return returnScene;
		}

		void OnTap(Gesture gesture)
		{
			returnScene = new MainMenuScene();
		}
	}
}

