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
    public class Scene4
        : Scene
    {
        Scene _returnScene;

        //const Int32 MinVans = 25;
        const Int32 MaxVans = 64;

        const Int32 currentNumVans = 64;

        readonly List<CaliforniaVan> vans = new List<CaliforniaVan>();

        GridRenderer gr;

        bool debugLinesOn = false;

        Single timer = 0f;

        public static Int32 screenWidth;
        public static Int32 screenHeight;
        public static Texture2D texVan1;
        public static Texture2D texVan2;

        public Scene4()
        {
            _returnScene = this;
        }

        public override void Start ()
        {
            gr = new GridRenderer (this.Blimey.DebugShapeRenderer, "Default");

            screenWidth = this.Cor.Graphics.DisplayStatus.CurrentWidth;
            screenHeight = this.Cor.Graphics.DisplayStatus.CurrentHeight;

            if (texVan1 == null)
            {
                try
                {
                    texVan1 = this.Cor.Assets.Load<Texture2D> ("cvan01.cba");
                }
                catch
                {
                    texVan1 = this.Cor.Resources.Load<Texture2D> ("resources/cvan01.png");
                }
            }
            
            if( texVan2 == null )
            {
                try
                {
                    texVan2 = this.Cor.Assets.Load<Texture2D> ("cvan02.cba");
                }
                catch
                {
                    texVan2 = this.Cor.Resources.Load<Texture2D> ("resources/cvan02.png");
                }
            }
            
            IShader unlitShader = null;
            
            try
            {
                unlitShader = this.Cor.Assets.Load<IShader> ("unlit.cba");
            }
            catch
            {
                unlitShader = this.Cor.Resources.LoadShader(ShaderType.Unlit);
            }

            Sprite.SpriteShader = unlitShader;

            /*
            var go = this.CreateSceneObject("block");

            go.Transform.LocalScale = new Vector3(0.5f, 0.1f, 2f);


            Single piOver2 = 0;
            RealMaths.PiOver2(out piOver2);

            Vector3 x = Vector3.Right;
            Quaternion q;
            Quaternion.CreateFromAxisAngle(ref x, -piOver2, out q);
            go.Transform.LocalRotation = q;


            var mr = go.AddTrait<MeshRenderer>();

            mr.Mesh = new CubePrimitive(this.Cor.Graphics);
            mr.Material = new Material("Default", this.Cor.Resources.GetShader(
                ShaderType.Gouraud,mr.Mesh.VertDecl));

            */

            Single pi = 0;
            RealMaths.Pi(out pi);

            var newCamSo = this.CreateSceneObject("ortho");
            newCamSo.Transform.LocalPosition = new Vector3(0, 0, 1);

            var orthoCam = newCamSo.AddTrait<Camera>();
            orthoCam.NearPlaneDistance = 0;
            orthoCam.FarPlaneDistance = 2;
            orthoCam.Projection = CameraProjectionType.Orthographic;

            orthoCam.TempWORKOUTANICERWAY = true;
            //Quaternion q;
            //Quaternion.CreateFromYawPitchRoll(pi, 0, 0, out q);
            //newCamSo.Transform.LocalRotation = q;

            this.SetRenderPassCameraTo("Default", newCamSo);

            /*
            var camSo = this.GetRenderPassCamera("Default");

            //camSo.Transform.LocalPosition = new Vector3(0,300,0);

            this.Cor.System.GetEffectiveDisplaySize(ref screenWidth, ref screenHeight);

            var cam = camSo.GetTrait<Camera>();

            //cam.Projection = CameraProjectionType.Orthographic;
            camSo.GetTrait<OrbitAroundSubject>().Speed = -0.01f;

*/


            this.Settings.BackgroundColour = Rgba32.Aquamarine;

            while (vans.Count != MaxVans )
            {
                var so = this.CreateSceneObject("van #" + vans.Count);
                var vanTrait = so.AddTrait<CaliforniaVan>();
                vans.Add(vanTrait);

            }

            this.Blimey.InputEventSystem.Tap += this.HandleTap;

        }

        void HandleTap(Gesture gesture)
        {
            _returnScene = new MainMenuScene();
            /*
            currentNumVans += 100;

            if( currentNumVans > MaxVans )
            {
                currentNumVans = MinVans;
            }
            */
        }

        void AdjustNumVans()
        {
            for(Int32 i = 0; i < vans.Count; ++i)
            {
                var vanTrait = vans[i];

                vanTrait.Parent.Enabled = (i < currentNumVans);
            }
        }

        public override void Shutdown()
        {
            this.Blimey.InputEventSystem.Tap -= this.HandleTap;
        }

        public override Scene Update (AppTime time)
        {
            if (timer > 0f)
            {
                timer -= time.Delta;

                if( timer < 0f )
                    timer = 0f;
            }

            if (timer == 0f)
            {
                if (Cor.Input.GenericGamepad.East == ButtonState.Pressed ||
                Cor.Input.Keyboard.IsFunctionalKeyDown (FunctionalKey.Escape))
                {
                    _returnScene = new MainMenuScene ();
                }

                if (Cor.Input.GenericGamepad.North == ButtonState.Pressed ||
                Cor.Input.Keyboard.IsCharacterKeyDown ('d'))
                {
                    debugLinesOn = !debugLinesOn;

                    vans.ForEach( x => x.EnabledDebugRenderer(debugLinesOn) );
                }

                timer = 0.5f;
            }

            gr.Update ();

            if (debugLinesOn)
            {
                float left = -(float)(screenWidth / 2) / 100f;
                float right = (float)(screenWidth / 2) / 100f;
                float top = (float)(screenHeight / 2) / 100f;
                float bottom = -(float)(screenHeight / 2) / 100f;

                this.Blimey.DebugShapeRenderer.AddQuad (
                    "Default",
                    new Vector3 (left, bottom, 0),
                    new Vector3 (right, bottom, 0),
                    new Vector3 (right, top, 0),
                    new Vector3 (left, top, 0),
                    Rgba32.Yellow);
            }

            return _returnScene;
        }

    }
}

