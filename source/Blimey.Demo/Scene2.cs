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
    public class Scene2
        : Scene
    {
        Scene returnScene;
        SceneObject billboardGo;

        GridRenderer gr;

        LookAtSubject las;

        SceneObject cam;

        Transform target;


        float timer = timeWindow;
        const float timeWindow = 5f;
        bool x = true;
        bool goOut = true;

        SceneObject markerGo;

        public override void Start()
        {
            this.Settings.BackgroundColour = Rgba32.LightSlateGrey;
            gr = new GridRenderer(this.Blimey.DebugShapeRenderer, "Default", 1f, 10);

            returnScene = this;

            // create a sprite
            var billboard = new BillboardPrimitive(this.Cor.Graphics);

            IShader unlitShader = this.Cor.Assets.Load<IShader> ("unlit.cba");

            billboardGo = this.CreateSceneObject("billboard");

            var mr = billboardGo.AddTrait<MeshRenderer>();
            mr.Mesh = billboard;
            mr.Material = new Material("Default", unlitShader);
            mr.Material.SetColour("MaterialColour", RandomGenerator.Default.GetRandomColour());

            target = billboardGo.Transform;

            markerGo = CreateSceneObject ("marker");

            markerGo.Transform.LocalScale = new Vector3 (0.05f, 0.05f, 0.05f);

            var markerMR = markerGo.AddTrait<MeshRenderer> ();
            markerMR.Mesh = new CubePrimitive(this.Cor.Graphics);
            markerMR.Material = new Material("Default", unlitShader);
            markerMR.Material.SetColour("MaterialColour", Rgba32.Red);

            cam = this.GetRenderPassCamera ("Default");

            this.DestroySceneObject(this.GetRenderPassCamera ("Debug"));
            this.DestroySceneObject(this.GetRenderPassCamera ("Gui"));

            this.SetRenderPassCameraTo ("Debug", cam);
            cam.Transform.Position = new Vector3(2, 1, 5);
            cam.RemoveTrait<OrbitAroundSubject> ();

            las = cam.GetTrait<LookAtSubject> ();
            las.Subject = billboardGo.Transform;

            this.Blimey.InputEventSystem.Tap += this.OnTap;
        }

        public override void Shutdown()
        {
            this.Blimey.InputEventSystem.Tap -= this.OnTap;
        }

        public override Scene Update(AppTime time)
        {
            gr.Update ();

            timer -= time.Delta;

            if (timer < 0f)
            {
                timer = timeWindow;

                goOut = !goOut;

                if (goOut)
                    x = !x;
            }

            float f = timer / timeWindow;

            if (goOut)
                f = 1f - f;

            f = f * 2f;

            target.Position = new Vector3 (
                x ? f : 0f,
                0,
                x ? 0f : f);

            this.Blimey.DebugShapeRenderer.AddLine (
                "Default",
                target.Position,
                target.Position + new Vector3 (0f, 10f, 0f),
                Rgba32.Orange);

            this.Blimey.DebugShapeRenderer.AddLine (
                "Default",
                las.Subject.Position,
                new Vector3(cam.Transform.Position.X, 0f, cam.Transform.Position.Z),
                Rgba32.Lime);

            markerGo.Transform.Position = target.Position + new Vector3 (0f, 0.2f, 0f);

            if (Cor.Input.GenericGamepad.East == ButtonState.Pressed ||
                Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Escape))
            {
                returnScene = new MainMenuScene();
            }

            return returnScene;
        }

        void OnTap(Gesture gesture)
        {
            returnScene = new MainMenuScene();
        }
    }
}

