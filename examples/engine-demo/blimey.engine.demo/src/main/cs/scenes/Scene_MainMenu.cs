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

        Triple q;

        Texture tex = null;

        public override void Start ()
        {
            var ta = Engine.Assets.Load <TextureAsset> ("assets/blimey_fnt_tex.bba");
            tex = Platform.Graphics.CreateTexture (ta);
            q = new Triple ();
            q.blend = BlendMode.Default;
            q.tex = tex;
            q.v [0].Colour = Rgba32.Blue;
            q.v [0].Position.X = -0.5f;
            q.v [0].Position.Y = 0f;
            q.v [0].UV = new Vector2 (0, 1);
            q.v [1].Colour = Rgba32.Green;
            q.v [1].Position.X = 0f;
            q.v [1].Position.Y = 0.5f;
            q.v [1].UV = new Vector2 (1, 0);
            q.v [2].Colour = Rgba32.Red;
            q.v [2].Position.X = -0.5f;
            q.v [2].Position.Y = 0.5f;
            q.v [2].UV = new Vector2 (0, 0);

            _returnScene = this;

            CommonDemoResources.Create (Platform, Engine);

			this.Configuration.BackgroundColour = _startCol;
            var teaPotModel = new TeapotPrimitive(this.Platform.Graphics);

            var so1 = RandomObjectHelper.CreateShapeGO(this, "Gui", teaPotModel.Mesh, 1);
            var so2 = RandomObjectHelper.CreateShapeGO(this, "Gui", teaPotModel.Mesh, 1);
            var so3 = RandomObjectHelper.CreateShapeGO(this, "Gui", teaPotModel.Mesh, 1);
            var so4 = RandomObjectHelper.CreateShapeGO(this, "Gui", teaPotModel.Mesh, 1);
            var so5 = RandomObjectHelper.CreateShapeGO(this, "Gui", teaPotModel.Mesh, 1);
            var so6 = RandomObjectHelper.CreateShapeGO(this, "Gui", teaPotModel.Mesh, 1);
            var so7 = RandomObjectHelper.CreateShapeGO(this, "Gui", teaPotModel.Mesh, 1);
            var so8 = RandomObjectHelper.CreateShapeGO(this, "Gui", teaPotModel.Mesh, 1);

            so1.Transform.LocalPosition = new Vector3(-0.35f, 0f, 0f);
			so2.Transform.LocalPosition = new Vector3(-0.25f, 0f, 0f);
			so3.Transform.LocalPosition = new Vector3(-0.15f, 0f, 0f);
			so4.Transform.LocalPosition = new Vector3(-0.05f, 0f, 0f);
			so5.Transform.LocalPosition = new Vector3(+0.05f, 0f, 0f);
			so6.Transform.LocalPosition = new Vector3(+0.15f, 0f, 0f);
			so7.Transform.LocalPosition = new Vector3(+0.25f, 0f, 0f);
            so8.Transform.LocalPosition = new Vector3(+0.35f, 0f, 0f);


			_menuItemMaterials.Add(so1.GetTrait<MeshRendererTrait>().Material);
			_menuItemMaterials.Add(so2.GetTrait<MeshRendererTrait>().Material);
			_menuItemMaterials.Add(so3.GetTrait<MeshRendererTrait>().Material);
			_menuItemMaterials.Add(so4.GetTrait<MeshRendererTrait>().Material);
			_menuItemMaterials.Add(so5.GetTrait<MeshRendererTrait>().Material);
			_menuItemMaterials.Add(so6.GetTrait<MeshRendererTrait>().Material);
            _menuItemMaterials.Add(so7.GetTrait<MeshRendererTrait>().Material);
            _menuItemMaterials.Add(so8.GetTrait<MeshRendererTrait>().Material);

			_menuSceneObjects.Add(so1);
			_menuSceneObjects.Add(so2);
			_menuSceneObjects.Add(so3);
			_menuSceneObjects.Add(so4);
			_menuSceneObjects.Add(so5);
			_menuSceneObjects.Add(so6);
            _menuSceneObjects.Add(so7);
            _menuSceneObjects.Add(so8);

            this.Engine.InputEventSystem.Tap += this.OnTap;
            this.Engine.InputEventSystem.Flick += this.OnFlick;

        }

        public override void Shutdown ()
        {
            _menuSceneObjects.Clear();
            _menuSceneObjects = null;
            _menuItemMaterials = null;
            this.Engine.InputEventSystem.Tap -= this.OnTap;
            this.Engine.InputEventSystem.Flick -= this.OnFlick;
            this.Platform.Graphics.DestroyTexture (q.tex);
            tex.Dispose ();
            tex = null;
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
            _selectedIndex = MathsUtils.Clamp(_selectedIndex, 0, _menuSceneObjects.Count - 1);
        }

        void DecreaseSelected()
        {
            _selectedIndex--;
            _selectedIndex = MathsUtils.Clamp(_selectedIndex, 0, _menuSceneObjects.Count - 1);
        }

        Scene CheckForMenuInput()
        {
            if (_inputTimer == 0f)
            {
				if (Platform.Input.GenericGamepad.DPad.Down == ButtonState.Pressed ||
                    Platform.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Left))
                {
                    this.DecreaseSelected();
                    _inputTimer = _doneSomethingTime;
                }

				if (Platform.Input.GenericGamepad.DPad.Right == ButtonState.Pressed ||
                    Platform.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Right))
                {
                    this.IncreaseSelected();
                    _inputTimer = _doneSomethingTime;

                }

				if (Platform.Input.GenericGamepad.Buttons.South == ButtonState.Pressed ||
                    Platform.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Enter))
                {
                    return GetSceneForCurrentSelection();
                }
            }

            return _returnScene;

        }

        Scene GetSceneForCurrentSelection()
        {
            if (_selectedIndex == 0)
                return new Scene_Darius ();

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

            if (_selectedIndex == 6)
                return new Scene_Mushrooms ();

            if (_selectedIndex == 7)
                return new Scene_Parallax ();

            return this;
        }

        public override Scene Update(AppTime time)
        {
            this.Engine.DebugRenderer.AddGrid ("Debug");

            var menuResult = this.CheckForMenuInput();

            this.Engine.PrimitiveRenderer.AddTriple ("Gui", q);

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

