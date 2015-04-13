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

    public class Scene_Shapes2
        : Scene
    {
        Scene returnScene;
        Entity billboardGo;

        LookAtSubjectTrait las;

        Entity cam;

        Transform target;


        float timer = timeWindow;
        const float timeWindow = 5f;
        bool x = true;
        bool goOut = true;

        Entity markerGo;


        public override void Start()
        {
            this.Configuration.BackgroundColour = Rgba32.LightSlateGrey;

            CommonDemoResources.Create (Platform, Engine);

            returnScene = this;

            // create a sprite
            var billboard = new BillboardPrimitive(this.Platform.Graphics);


            billboardGo = this.SceneGraph.CreateSceneObject("billboard");

            var mr = billboardGo.AddTrait<MeshRendererTrait>();
            mr.Mesh = billboard.Mesh;
            mr.Material = new Material("Default", CommonDemoResources.UnlitShader);
            mr.Material.SetColour("MaterialColour", RandomGenerator.Default.GetRandomColour());

            target = billboardGo.Transform;

            markerGo = this.SceneGraph.CreateSceneObject ("marker");

            markerGo.Transform.LocalScale = new Vector3 (0.05f, 0.05f, 0.05f);

            var markerMR = markerGo.AddTrait<MeshRendererTrait> ();
            markerMR.Mesh = new CubePrimitive(this.Platform.Graphics).Mesh;
            markerMR.Material = new Material("Default", CommonDemoResources.UnlitShader);
            markerMR.Material.SetColour("MaterialColour", Rgba32.Red);

            cam = this.CameraManager.GetRenderPassCamera ("Default");

            this.SceneGraph.DestroySceneObject(this.CameraManager.GetRenderPassCamera ("Debug"));
            this.SceneGraph.DestroySceneObject(this.CameraManager.GetRenderPassCamera ("Gui"));

            this.RuntimeConfiguration.SetRenderPassCameraTo ("Debug", cam);
            cam.Transform.Position = new Vector3(2, 1, 5);
            cam.RemoveTrait<OrbitAroundSubjectTrait> ();

            las = cam.GetTrait<LookAtSubjectTrait> ();
            las.Subject = billboardGo.Transform;

            this.Engine.InputEventSystem.Tap += this.OnTap;
        }

        public override void Shutdown()
        {
            this.Engine.InputEventSystem.Tap -= this.OnTap;
            CommonDemoResources.Destroy ();
        }

        public override Scene Update(AppTime time)
        {
            this.Engine.DebugRenderer.AddGrid ("Debug", 1f, 10);

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

            this.Engine.DebugRenderer.AddLine (
                "Default",
                target.Position,
                target.Position + new Vector3 (0f, 10f, 0f),
                Rgba32.Orange);

            this.Engine.DebugRenderer.AddLine (
                "Default",
                las.Subject.Position,
                new Vector3(cam.Transform.Position.X, 0f, cam.Transform.Position.Z),
                Rgba32.Lime);

            markerGo.Transform.Position = target.Position + new Vector3 (0f, 0.2f, 0f);

            if (Platform.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
                Platform.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Escape) ||
                    Platform.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Backspace))
            {
                returnScene = new Scene_Shapes3();
            }

            return returnScene;
        }

        void OnTap(Gesture gesture)
        {
            returnScene = new Scene_Shapes3();
        }
    }
}

