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
using Cor;
using System.Collections.Generic;

namespace Blimey.Demo
{
    public class Scene1
        : Scene
    {
        List<SceneObject> _objects;

        SceneObject _alternateCamera;

        GridRenderer gr;

        const Single _cameraChangeTime = 5f;

        Single _timer = _cameraChangeTime;

        bool _defaultCamIsCurrent = true;

        Scene _returnScene;

        LookAtSubject las = null;

        public override void Start ()
        {
            //this.ClearColour = Colour.LightGray;
            gr = new GridRenderer(this.Blimey.DebugShapeRenderer, "Default");

            _alternateCamera = this.CreateSceneObject("Alternate Camera");

            _alternateCamera.AddTrait<Camera>();

            _alternateCamera.Transform.Position = new Vector3(0.65f, 1f, -2.50f) * 3;
            _alternateCamera.Transform.LookAt(Vector3.Zero);


            las = _alternateCamera.AddTrait<LookAtSubject> ();

            _objects = RandomObjectHelper.Generate(this);

            var landmarkGo = this.CreateLandmark();
            _objects.Add(landmarkGo);

            _returnScene = this;

            this.Blimey.InputEventSystem.Tap += this.OnTap;
        }

        SceneObject CreateLandmark()
        {
            var landmarkGo = this.CreateSceneObject("landmark");

            landmarkGo.Transform.LocalPosition = new Vector3(0f, 0f, 0f);
            landmarkGo.Transform.LocalScale = new Vector3(0.64f, 0.64f, 0.64f);

            var cowMesh = new TeapotPrimitive(this.Cor.Graphics);

            var mr = landmarkGo.AddTrait<MeshRenderer>();

            mr.Mesh = cowMesh;

            IShader shader = null;
            
            try
            {
                shader = this.Cor.Assets.Load<IShader> ("pixel_lit.cba");
            }
            catch
            {
                shader = this.Cor.Resources.LoadShader(ShaderType.PixelLit);
            }
            
            var mat = new Material("Default", shader);

            mat.SetColour("MaterialColour", Rgba32.CornflowerBlue);

            mr.Material = mat;

            return landmarkGo;
        }

        public override Scene Update(AppTime time)
        {
            if (Cor.Input.GenericGamepad.East == ButtonState.Pressed ||
                Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Escape))
            {
                _returnScene = new MainMenuScene();
            }

            gr.Update();

            _timer -= time.Delta;

            if( _timer < 0f )
            {
                _timer = _cameraChangeTime;

                if( _defaultCamIsCurrent )
                {
                    this.SetRenderPassCameraTo("Default", _alternateCamera);
                }
                else
                {
                    this.SetRenderPassCameraToDefault("Default");
                }

                _defaultCamIsCurrent = !_defaultCamIsCurrent;
            }

            return _returnScene;
        }

        public override void Shutdown ()
        {
            _objects = null;
            this.Blimey.InputEventSystem.Tap -= this.OnTap;
        }

        void OnTap(Gesture gesture)
        {
            _returnScene = new MainMenuScene();
        }
    }
}

