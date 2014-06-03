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
// │ Copyright © 2014 A.J.Pook (http://ajpook.github.io)                    │ \\
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
using Cor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Blimey.Content;

namespace Blimey.Demo
{
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
	public class Scene5
		: Scene
	{
		Scene returnScene;
		SceneObject earthGo;

		GridRenderer gr;

		List<Airport> airports = new List<Airport>();

		IShader shader = null;
		IShader shader2 = null;

		public override void Start()
		{
			ShaderAsset unlitShaderAsset = this.Cor.Assets.Load<ShaderAsset> ("unlit.cba");
			this.Blimey.DebugShapeRenderer.DebugShader = 
				this.Cor.Graphics.CreateShader (unlitShaderAsset);
			gr = new GridRenderer(this.Blimey.DebugShapeRenderer, "Debug");
            
            var lines = Cor.Assets.Load <TextAsset> ("airports.cba")
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

			this.Settings.BackgroundColour = Rgba32.Red;

			returnScene = this;

			float radius = 1.5f;
			// create a sprite
			var sphereMesh = new SpherePrimitive(this.Cor.Graphics);

			ShaderAsset shaderAsset = this.Cor.Assets.Load<ShaderAsset> ("vertex_lit.cba");
			shader = this.Cor.Graphics.CreateShader (shaderAsset);

            var mat = new Material("Default",shader);
            mat.SetColour("MaterialColour", Rgba32.LightGrey);
			earthGo = this.CreateSceneObject("earth");

			SceneObject camSo = CreateSceneObject ("Scene 5 Camera");
			camSo.AddTrait<Camera>();
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

			ShaderAsset shaderAsset2 = this.Cor.Assets.Load<ShaderAsset> ("unlit.cba");
			shader2 = this.Cor.Graphics.CreateShader (shaderAsset2);

			var mat2 = new Material("Default", shader2);
			mat2.SetColour("MaterialColour", Rgba32.Blue);

            foreach (var airport in airports)
			{
				var so = this.CreateSceneObject(airport.Iata);

                so.Transform.Parent = earthGo.Transform;

				var sodr = so.AddTrait<DebugRenderer>();
				//sodr.RenderPass = "Default";
				sodr.Colour = Rgba32.Blue;
				//var somr = so.AddTrait<MeshRenderer>();
				//somr.Mesh = sphereMesh;
				//somr.Material = mat2;

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
				so.Transform.LocalScale = new Vector3(0.015f, 0.015f, 0.015f);

			}

			this.Blimey.InputEventSystem.Tap += this.OnTap;
		}

		public override void Shutdown()
		{
			this.Blimey.InputEventSystem.Tap -= this.OnTap;
			this.Cor.Graphics.DestroyShader (shader2);
			this.Cor.Graphics.DestroyShader (shader);
		}

		public override Scene Update(AppTime time)
		{
			gr.Update ();

			if (Cor.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
				Cor.Input.Keyboard.IsFunctionalKeyDown (FunctionalKey.Escape) ||
				Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Backspace))
			{
				returnScene = new MainMenuScene ();
			}


			return returnScene;
		}

		void OnTap(Gesture gesture)
		{
			returnScene = new MainMenuScene();

			// Clean up the things we allocated on the GPU.
			this.Cor.Graphics.DestroyShader (shader);
			shader = null;
		}
	}
}

