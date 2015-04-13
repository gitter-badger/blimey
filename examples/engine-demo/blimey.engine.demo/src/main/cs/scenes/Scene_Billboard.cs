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

    // A simple example that shows using the Scene Graph service's SpriteTrait
    // to render a billboard in a 3D scene.
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Scene_Billboard
        : Scene
    {
        Scene returnScene;
        Shader shader = null;
        Texture tex = null;

        public override void Start()
        {
            this.Configuration.BackgroundColour = Rgba32.DarkSlateGrey;
            returnScene = this;

            ShaderAsset shaderAsset = this.Engine.Assets.Load<ShaderAsset> ("assets/unlit.bba");
            TextureAsset texAsset = this.Engine.Assets.Load <TextureAsset> ("assets/bg1.bba");

            SpriteTrait.SpriteShader = this.Platform.Graphics.CreateShader (shaderAsset);
            tex = this.Platform.Graphics.CreateTexture (texAsset);

            var cam = this.CameraManager.GetRenderPassCamera ("Default");
            cam.GetTrait <OrbitAroundSubjectTrait> ().Active = false;

            // create a sprite
            var so = this.SceneGraph.CreateSceneObject ("billboard");

            var spr = so.AddTrait <SpriteTrait> ();
            spr.Width = 256f;
            spr.Height = 256f;
            spr.Texture = tex;

            // TODO: Material Offset has not been implemented in Engine yet,
            //       if it were we could use it to adjust the UVs used to render
            //       this SpriteTrait's texture.
            //spr.Material.Offset = new Vector2 (0.5f, 0.5f);
            spr.Material.SetColour ("MaterialColour", Rgba32.Yellow);

            shader = this.Platform.Graphics.CreateShader (shaderAsset);

            this.Engine.InputEventSystem.DoubleTap += this.OnDoubleTap;
        }

        public override void Shutdown()
        {
            this.Engine.InputEventSystem.DoubleTap -= this.OnDoubleTap;
        }

        public override Scene Update(AppTime time)
        {
            this.Engine.DebugRenderer.AddGrid ("Debug", 1f, 10);

            if (Platform.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
                Platform.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Escape) ||
                Platform.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Backspace))
            {
                returnScene = new Scene_MainMenu();
            }

            return returnScene;
        }

        void OnDoubleTap (Gesture gesture)
        {
            returnScene = new Scene_MainMenu ();

            // Clean up the things we allocated on the GPU.
            this.Platform.Graphics.DestroyShader (shader);
            this.Platform.Graphics.DestroyTexture (tex);
            shader = null;
        }
    }
}

