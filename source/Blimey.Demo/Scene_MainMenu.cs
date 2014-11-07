// ┌────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐ \\
// │ Blimey - Fast, efficient, high level engine built upon Cor & Abacus                                            │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │                     Brought to you by:                                                                         │ \\
// │                              _________                    .__               __                                 │ \\
// │                             /   _____/__ __  ____    ____ |__|____    _____/  |_                               │ \\
// │                             \_____  \|  |  \/    \  / ___\|  \__  \  /    \   __\                              │ \\
// │                             /        \  |  /   |  \/ /_/  >  |/ __ \|   |  \  |                                │ \\
// │                            /_______  /____/|___|  /\___  /|__(____  /___|  /__|                                │ \\
// │                                    \/           \//_____/         \/     \/                                    │ \\
// │                                                                                                                │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2008-2014 A.J.Pook (http://ajpook.github.io)                                                       │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors: A.J.Pook                                                                                              │ \\
// ├────────────────────────────────────────────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated   │ \\
// │ documentation files (the "Software"), to deal in the Software without restriction, including without           │ \\
// │ limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sellcopies of the   │ \\
// │ Software, and to permit persons to whom the Software is furnished to do so, subject to the following           │ \\
// │ conditions:                                                                                                    │ \\
// │                                                                                                                │ \\
// │ The above copyright notice and this permission notice shall be included in all copies or substantial portions  │ \\
// │ of the Software.                                                                                               │ \\
// │                                                                                                                │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED  │ \\
// │ TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL │ \\
// │ THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF  │ \\
// │ CONTRACT, TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER        │ \\
// │ DEALINGS IN THE SOFTWARE.                                                                                      │ \\
// └────────────────────────────────────────────────────────────────────────────────────────────────────────────────┘ \\

namespace Blimey.Demo
{
    using System;
    using Fudge;
    using Abacus.SinglePrecision;
    using Cor;
    using System.Collections.Generic;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Scene_MainMenu
        : Scene
    {
        const Single scaleBig = 0.075f;
        const Single scaleSmall = 0.05f;

        const Single _totalTime = 2f;
        Single _timer = _totalTime;

        const Single _doneSomethingTime = 0.2f;
        Single _inputTimer = _doneSomethingTime;

		Rgba32 _startCol = Rgba32.LightYellow;
		Rgba32 _endCol = Rgba32.LightBlue;

        static Int32 _selectedIndex = 0;

        List<Material> _menuItemMaterials = new List<Material>();
        List<Entity> _menuSceneObjects = new List<Entity>();

        Scene _returnScene;

        PrimitiveRenderer.Triple q;

        public override void Start ()
        {
            q = new PrimitiveRenderer.Triple ();
            q.blend = BlendMode.Additive;
           
            q.v [0].Colour = Rgba32.Blue;
            q.v [0].Position.X = -0.3f;
            q.v [0].Position.Y = -0.3f;
            q.v [1].Colour = Rgba32.Green;
            q.v [1].Position.X = 0.3f;
            q.v [1].Position.Y = 0.3f;
            q.v [2].Colour = Rgba32.Red;
            q.v [2].Position.X = 0f;
            q.v [2].Position.Y = 0.3f;

            _returnScene = this;

            CommonDemoResources.Create (Cor, Blimey);

			this.Configuration.BackgroundColour = _startCol;
            var teaPotModel = new TeapotPrimitive(this.Cor.Graphics);

			var so1 = RandomObjectHelper.CreateShapeGO(this, "Gui", teaPotModel, 1);
			var so2 = RandomObjectHelper.CreateShapeGO(this, "Gui", teaPotModel, 1);
			var so3 = RandomObjectHelper.CreateShapeGO(this, "Gui", teaPotModel, 1);
			var so4 = RandomObjectHelper.CreateShapeGO(this, "Gui", teaPotModel, 1);
			var so5 = RandomObjectHelper.CreateShapeGO(this, "Gui", teaPotModel, 1);
			var so6 = RandomObjectHelper.CreateShapeGO(this, "Gui", teaPotModel, 1);

			so1.Transform.LocalPosition = new Vector3(-0.25f, 0f, 0f);
			so2.Transform.LocalPosition = new Vector3(-0.15f, 0f, 0f);
			so3.Transform.LocalPosition = new Vector3(-0.05f, 0f, 0f);
			so4.Transform.LocalPosition = new Vector3(+0.05f, 0f, 0f);
			so5.Transform.LocalPosition = new Vector3(+0.15f, 0f, 0f);
			so6.Transform.LocalPosition = new Vector3(+0.25f, 0f, 0f);


			_menuItemMaterials.Add(so1.GetTrait<MeshRendererTrait>().Material);
			_menuItemMaterials.Add(so2.GetTrait<MeshRendererTrait>().Material);
			_menuItemMaterials.Add(so3.GetTrait<MeshRendererTrait>().Material);
			_menuItemMaterials.Add(so4.GetTrait<MeshRendererTrait>().Material);
			_menuItemMaterials.Add(so5.GetTrait<MeshRendererTrait>().Material);
			_menuItemMaterials.Add(so6.GetTrait<MeshRendererTrait>().Material);

			_menuSceneObjects.Add(so1);
			_menuSceneObjects.Add(so2);
			_menuSceneObjects.Add(so3);
			_menuSceneObjects.Add(so4);
			_menuSceneObjects.Add(so5);
			_menuSceneObjects.Add(so6);

            this.Blimey.InputEventSystem.Tap += this.OnTap;
            this.Blimey.InputEventSystem.Flick += this.OnFlick;

        }

        public override void Shutdown ()
        {
            _menuSceneObjects.Clear();
            _menuSceneObjects = null;
            _menuItemMaterials = null;
            this.Blimey.InputEventSystem.Tap -= this.OnTap;
            this.Blimey.InputEventSystem.Flick -= this.OnFlick;

            CommonDemoResources.Destroy ();
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
				if (Cor.Input.GenericGamepad.DPad.Down == ButtonState.Pressed ||
                    Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Left))
                {
                    this.DecreaseSelected();
                    _inputTimer = _doneSomethingTime;
                }

				if (Cor.Input.GenericGamepad.DPad.Right == ButtonState.Pressed ||
                    Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Right))
                {
                    this.IncreaseSelected();
                    _inputTimer = _doneSomethingTime;

                }

				if (Cor.Input.GenericGamepad.Buttons.South == ButtonState.Pressed ||
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
                return new Scene_Shapes1 ();

            if (_selectedIndex == 1)
                return new Scene_Airports ();

            if (_selectedIndex == 2)
                return new Scene_Sprites ();

            if (_selectedIndex == 3)
				return new Scene_Particles ();

			if (_selectedIndex == 4)
				return new Scene_Text ();

			if (_selectedIndex == 5)
				return new Scene_Boids ();

            return this;
        }

        public override Scene Update(AppTime time)
        {
            var menuResult = this.CheckForMenuInput();

            this.Blimey.PrimitiveRenderer.AddTriple ("Gui", q);

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

            _timer -= time.Delta; if (_timer <= 0f) _timer = 0f;
            _inputTimer -= time.Delta; if (_inputTimer <= 0f) _inputTimer = 0f;

			Rgba32 c = Rgba32.Lerp(_startCol, _endCol, (Maths.Sin(time.Elapsed) / 2f) + 0.5f);
			this.RuntimeConfiguration.ChangeBackgroundColour (c);

            return this;
        }
    }
}

