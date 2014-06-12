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

namespace Blimey.Demo
{
    public class Scene3
        : Scene
    {
        Scene _returnScene;

        Mesh teapotGPUMesh;
        Int32 teapotCounter;
        GridRenderer gr;

		
		// GPU Resources.
		IShader shader = null;

        public override void Start()
        {
            this.Settings.BackgroundColour = Rgba32.Black;

            SceneObject camSo = CreateSceneObject ("Scene 3 Camera");
			camSo.AddTrait <Camera> ();
            var lookatTrait = camSo.AddTrait<LookAtSubject>();
            lookatTrait.Subject = Transform.Origin;
            var orbitTrait = camSo.AddTrait<OrbitAroundSubject>();
            orbitTrait.CameraSubject = Transform.Origin;

            camSo.Transform.LocalPosition = new Vector3(1f,0.5f,5f);

            this.SetRenderPassCameraTo("Debug", camSo);
			this.SetRenderPassCameraTo("Default", camSo);

            _returnScene = this;
            this.Blimey.InputEventSystem.Tap += this.OnTap;

            teapotGPUMesh = new TeapotPrimitive(Cor.Graphics);

			// set up the debug renderer
			ShaderAsset shaderAsset = this.Cor.Assets.Load<ShaderAsset>("unlit.cba");
			this.Blimey.DebugShapeRenderer.DebugShader = 
				this.Cor.Graphics.CreateShader (shaderAsset);
            gr = new GridRenderer(this.Blimey.DebugShapeRenderer, "Debug");

            AddTeapot(0);
            AddTeapot(-1.5f);
            AddTeapot(1.5f);
        }

        void AddTeapot(Single z)
        {
            // create a game object
            SceneObject testGO = CreateSceneObject ("teapot #" + ++teapotCounter);

            Single scale = 1f;


            // size it
            testGO.Transform.LocalPosition = new Vector3(
                0,
                0,
                z);

            testGO.Transform.LocalScale = new Vector3(scale, scale, scale);


			ShaderAsset shaderAsset = this.Cor.Assets.Load<ShaderAsset> ("pixel_lit.cba");

			shader = this.Cor.Graphics.CreateShader (shaderAsset);

            var mat = new Material("Default", shader);

            //mat.SetTexture("_texture", null);
            // add a mesh renderer
            var meshRendererTrait = testGO.AddTrait<MeshRenderer> ();

            // set the mesh renderer's material
            meshRendererTrait.Material = mat;

            meshRendererTrait.Material.SetColour("MaterialColour", RandomGenerator.Default.GetRandomColour());

            // and it's model
            meshRendererTrait.Mesh = teapotGPUMesh;
        }

        public override void Shutdown()
        {
            this.Blimey.InputEventSystem.Tap -= this.OnTap;
        }

        public override Scene Update(AppTime time)
        {
			if (Cor.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
				Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Escape) ||
					Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Backspace))
            {
                _returnScene = new MainMenuScene();
            }

            gr.Update();

            this.Blimey.DebugShapeRenderer.AddLine(
                "Gui",
                new Vector3(-0.5f, -0.5f, 0),
                new Vector3(0.5f, 0.5f, 0),
                Rgba32.Yellow);

            return _returnScene;
        }

        void OnTap(Gesture gesture)
        {
			_returnScene = new MainMenuScene();

			// Clean up the things we allocated on the GPU.
			this.Cor.Graphics.DestroyShader (shader);
			shader = null;
        }
    }
}

