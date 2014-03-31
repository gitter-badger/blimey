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
    public class MainMenuScene
        : Scene
    {
        const Single scaleBig = 0.075f;
        const Single scaleSmall = 0.05f;

        const Single _totalTime = 2f;
        Single _timer = _totalTime;

        const Single _doneSomethingTime = 0.2f;
        Single _inputTimer = _doneSomethingTime;

        Rgba32 _startCol = Rgba32.Bisque;
        Rgba32 _endCol = Rgba32.LightGrey;

        static Int32 _selectedIndex = 0;

        List<Material> _menuItemMaterials = new List<Material>();
        List<SceneObject> _menuSceneObjects = new List<SceneObject>();

        Scene _returnScene;

        public override void Start ()
        {
            _returnScene = this;

            this.Settings.BackgroundColour = _startCol;

            var cubeModel = new CubePrimitive(this.Cor.Graphics);
            var cylinderModel = new CylinderPrimitive(this.Cor.Graphics);
            var torusModel = new TorusPrimitive(this.Cor.Graphics);
            var teaPotModel = new TeapotPrimitive(this.Cor.Graphics);
            var sphereModel = new SpherePrimitive(this.Cor.Graphics);

            var cube = RandomObjectHelper.CreateShapeGO(this, "Gui", cubeModel, 1);
            var cylinder = RandomObjectHelper.CreateShapeGO(this, "Gui", cylinderModel, 1);
            var torus = RandomObjectHelper.CreateShapeGO(this, "Gui", torusModel, 2);
            var teaPot = RandomObjectHelper.CreateShapeGO(this, "Gui", teaPotModel, 2);
            var sphere = RandomObjectHelper.CreateShapeGO(this, "Gui", sphereModel, 2);

            teaPot.Transform.LocalPosition = new Vector3(-0.3f, 0f, 0f);
            cube.Transform.LocalPosition = new Vector3(-0.15f, 0f, 0f);
            cylinder.Transform.LocalPosition = new Vector3(0f, 0f, 0f);
            torus.Transform.LocalPosition = new Vector3(0.15f, 0f, 0f);
            sphere.Transform.LocalPosition = new Vector3(0.3f, 0f, 0f);


            _menuItemMaterials.Add(teaPot.GetTrait<MeshRenderer>().Material);
            _menuItemMaterials.Add(cube.GetTrait<MeshRenderer>().Material);
            _menuItemMaterials.Add(cylinder.GetTrait<MeshRenderer>().Material);
            _menuItemMaterials.Add(torus.GetTrait<MeshRenderer>().Material);
            _menuItemMaterials.Add(sphere.GetTrait<MeshRenderer>().Material);


            _menuSceneObjects.Add(teaPot);
            _menuSceneObjects.Add(cube);
            _menuSceneObjects.Add(cylinder);
            _menuSceneObjects.Add(torus);
            _menuSceneObjects.Add(sphere);

            /*
            var orthoCameraGo = this.CreateSceneObject("Ortho Camera");

            var orthoCam = orthoCameraGo.AddTrait<Camera>();

            orthoCam.Projection = CameraProjectionType.Orthographic;

            orthoCameraGo.Transform.Position = new Vector3(0f, 0f, 5f);

            orthoCameraGo.Transform.LookAt(Vector3.Zero);

            this.SetRenderPassCameraTo("Gui", orthoCameraGo);
            */


            this.Blimey.InputEventSystem.Tap += this.OnTap;
            this.Blimey.InputEventSystem.Flick += this.OnFlick;

        }

        public override void Shutdown ()
        {
            _menuSceneObjects.Clear();

            this.Blimey.InputEventSystem.Tap -= this.OnTap;
            this.Blimey.InputEventSystem.Flick -= this.OnFlick;
        }

        void OnFlick(Gesture gesture)
        {
            var v = gesture.TouchTrackers[0].GetVelocity(TouchPositionSpace.NormalisedEngine);

            if (v.X > 0)
            {
                IncreaseSelected();
            }
            else
            {
                DecreaseSelected();
            }
        }

        void OnTap(Gesture gesture)
        {
            _returnScene = GetSceneForCurrentSelection();

            var pos = gesture.GetFinishingPosition(TouchPositionSpace.NormalisedEngine);

        }

        void IncreaseSelected()
        {
            _selectedIndex++;
            _selectedIndex = BlimeyMathsHelper.Clamp(_selectedIndex, 0, _menuSceneObjects.Count - 1);
        }

        void DecreaseSelected()
        {
            _selectedIndex--;
            _selectedIndex = BlimeyMathsHelper.Clamp(_selectedIndex, 0, _menuSceneObjects.Count - 1);
        }

        Scene CheckForMenuInput()
        {
            if (_inputTimer == 0f)
            {
                if (Cor.Input.GenericGamepad.Left == ButtonState.Pressed ||
                    Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Left))
                {
                    this.DecreaseSelected();
                    _inputTimer = _doneSomethingTime;
                }

                if (Cor.Input.GenericGamepad.Right == ButtonState.Pressed ||
                    Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Right))
                {
                    this.IncreaseSelected();
                    _inputTimer = _doneSomethingTime;

                }

                if (Cor.Input.GenericGamepad.South == ButtonState.Pressed ||
                    Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Enter))
                {
                    return GetSceneForCurrentSelection();
                }
            }

            return _returnScene;

        }

        Scene GetSceneForCurrentSelection()
        {
            if (_selectedIndex == 0)
                return new Scene1();

            if (_selectedIndex == 1)
                return new Scene2();

            if (_selectedIndex == 2)
                return new Scene3();

            if (_selectedIndex == 3)
				return new Scene4();

			if (_selectedIndex == 4)
				return new Scene5();

            return this;
        }

        public override Scene Update(AppTime time)
        {
            var menuResult = this.CheckForMenuInput();

            if (menuResult != this)
                return menuResult;

            for (int i = 0; i < _menuSceneObjects.Count; ++i)
            {
                if( i == _selectedIndex )
                {
                    _menuSceneObjects[i].Transform.LocalScale = new Vector3(scaleBig, scaleBig, scaleBig);
                }
                else
                {
                    _menuSceneObjects[i].Transform.LocalScale = new Vector3(scaleSmall, scaleSmall, scaleSmall);
                }

            }

            if (_timer <= 0f)
            {

                _timer = 0f;
            }
            else
            {
                _timer -= time.Delta;
            }

            if (_inputTimer <= 0f)
            {

                _inputTimer = 0f;
            }
            else
            {
                _inputTimer -= time.Delta;
            }

            //this.Settings.BackgroundColour = Colour.Lerp(_startCol, _endCol, 1f - (_timer / _totalTime));

            return this;
        }
    }
}

