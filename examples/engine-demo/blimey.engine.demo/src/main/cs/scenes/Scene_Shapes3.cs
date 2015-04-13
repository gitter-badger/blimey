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
    using Blimey.Platform;
    using Blimey.Asset;
    using Blimey.Engine;
    using System.Collections.Generic;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Scene_Shapes3
        : Scene
    {
        Scene _returnScene;

        Mesh teapotGPUMesh;
        Int32 teapotCounter;

        // GPU Resources.
        Shader shader = null;

        public override void Start()
        {
            this.Configuration.BackgroundColour = Rgba32.Black;

            CommonDemoResources.Create (Platform, Engine);

            Entity camSo = SceneGraph.CreateSceneObject ("Scene 3 Camera");
            camSo.AddTrait <CameraTrait> ();
            var lookatTrait = camSo.AddTrait<LookAtSubjectTrait>();
            lookatTrait.Subject = Transform.Origin;
            var orbitTrait = camSo.AddTrait<OrbitAroundSubjectTrait>();
            orbitTrait.CameraSubject = Transform.Origin;

            camSo.Transform.LocalPosition = new Vector3(1f,0.5f,5f);

            this.RuntimeConfiguration.SetRenderPassCameraTo("Debug", camSo);
            this.RuntimeConfiguration.SetRenderPassCameraTo("Default", camSo);

            _returnScene = this;
            this.Engine.InputEventSystem.Tap += this.OnTap;

            teapotGPUMesh = new TeapotPrimitive(Platform.Graphics).Mesh;

            AddTeapot(0);
            AddTeapot(-1.5f);
            AddTeapot(1.5f);
        }

        void AddTeapot(Single z)
        {
            // create a game object
            Entity testGO = SceneGraph.CreateSceneObject ("teapot #" + ++teapotCounter);

            Single scale = 1f;


            // size it
            testGO.Transform.LocalPosition = new Vector3(
                0,
                0,
                z);

            testGO.Transform.LocalScale = new Vector3(scale, scale, scale);

            var mat = new Material("Default", CommonDemoResources.PixelLitShader);

            //mat.SetTexture("_texture", null);
            // add a mesh renderer
            var meshRendererTrait = testGO.AddTrait<MeshRendererTrait> ();

            // set the mesh renderer's material
            meshRendererTrait.Material = mat;

            meshRendererTrait.Material.SetColour("MaterialColour", RandomGenerator.Default.GetRandomColour());

            // and it's model
            meshRendererTrait.Mesh = teapotGPUMesh;
        }

        public override void Shutdown()
        {
            this.Engine.InputEventSystem.Tap -= this.OnTap;
            CommonDemoResources.Destroy ();
        }

        public override Scene Update(AppTime time)
        {
            if (Platform.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
                Platform.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Escape) ||
                    Platform.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Backspace))
            {
                _returnScene = new Scene_MainMenu();
            }

            this.Engine.DebugRenderer.AddGrid ("Debug");

            this.Engine.DebugRenderer.AddLine(
                "Gui",
                new Vector3(-0.5f, -0.5f, 0),
                new Vector3(0.5f, 0.5f, 0),
                Rgba32.Yellow);

            return _returnScene;
        }

        void OnTap(Gesture gesture)
        {
            _returnScene = new Scene_MainMenu();
        }
    }
}

