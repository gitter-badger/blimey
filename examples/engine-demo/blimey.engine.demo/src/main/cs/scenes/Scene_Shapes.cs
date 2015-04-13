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

    public class Scene_Shapes
        : Scene
    {
        List<Entity> _objects;

        Entity _alternateCamera;

        const Single _cameraChangeTime = 5f;

        Single _timer = _cameraChangeTime;

        bool _defaultCamIsCurrent = true;

        Scene _returnScene;

        public override void Start ()
        {
            _alternateCamera = this.SceneGraph.CreateSceneObject("Alternate Camera");

            CommonDemoResources.Create (Platform, Engine);

            _alternateCamera.AddTrait<CameraTrait>();

            _alternateCamera.Transform.Position = new Vector3(0.65f, 1f, -2.50f) * 3;
            _alternateCamera.Transform.LookAt(Vector3.Zero);

            _objects = RandomObjectHelper.Generate(this);

            var landmarkGo = this.CreateLandmark();
            _objects.Add(landmarkGo);

            _returnScene = this;

            this.Engine.InputEventSystem.Tap += this.OnTap;
        }

        Entity CreateLandmark()
        {
            var landmarkGo = this.SceneGraph.CreateSceneObject("landmark");

            landmarkGo.Transform.LocalPosition = new Vector3(0f, 0f, 0f);
            landmarkGo.Transform.LocalScale = new Vector3(0.64f, 0.64f, 0.64f);

            var cowMesh = new TeapotPrimitive(this.Platform.Graphics);

            var mr = landmarkGo.AddTrait<MeshRendererTrait>();

            mr.Mesh = cowMesh.Mesh;

            var mat = new Material("Default", CommonDemoResources.PixelLitShader);

            mat.SetColour("MaterialColour", Rgba32.CornflowerBlue);

            mr.Material = mat;

            return landmarkGo;
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

            _timer -= time.Delta;

            if( _timer < 0f )
            {
                _timer = _cameraChangeTime;

                if( _defaultCamIsCurrent )
                {
                    this.RuntimeConfiguration.SetRenderPassCameraTo("Default", _alternateCamera);
                }
                else
                {
                    this.RuntimeConfiguration.SetRenderPassCameraToDefault("Default");
                }

                _defaultCamIsCurrent = !_defaultCamIsCurrent;
            }

            return _returnScene;
        }

        public override void Shutdown ()
        {
            _objects = null;
            this.Engine.InputEventSystem.Tap -= this.OnTap;

            // Clean up the things we allocated on the GPU.
            CommonDemoResources.Destroy ();
        }

        void OnTap(Gesture gesture)
        {
            _returnScene = new Scene_MainMenu();
        }
    }
}

