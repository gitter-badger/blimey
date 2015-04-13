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

    // This example show cases two different approachs to rendering sprites
    // #1 - Using Blimey's Primitive system (Not yet implemented).
    // #2 - Using Blimey's Scene graph system
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Scene_Sprites
        : Scene
    {
        static class Settings
        {
            static Settings ()
            {
                Maths.Pi (out PI);
            }

            public static readonly Single PI;
            public static readonly Single MaxVelocity = 200;
            public static readonly Int32 MinHares = 16;
            public static readonly Int32 MaxHares = 256;
            public static readonly Single SpriteDepth = 0.01f; // n.b. i think this should be 0f, however when set to 0f there is lots on Z fighting.  todo: investigate
            public static readonly Single ScaleVariation = 0.2f;

            public static readonly Single PixelToWorldSpace = 0.01f;
            public static readonly Single WorldToPixelSpace = 100f;

            public static readonly BlendMode[] BlendModes = {
                BlendMode.Default,
                BlendMode.Additive,
                BlendMode.Subtract,
                BlendMode.Opaque
            };
        }

        class Hare
            : Trait
        {
            Vector2         velocity;
            Single          deltaScale;
            Single          deltaRotation;
            SpriteTrait     sprite;
			Int32           currentBlendMode = 0;

            public override void OnAwake ()
            {
                Single x = RandomGenerator.Default.GetRandomSingle (-(Single) Scene_Sprites.AppWidth / 2f, (Single) Scene_Sprites.AppWidth / 2f);
                Single y = RandomGenerator.Default.GetRandomSingle (-(Single) Scene_Sprites.AppHeight / 2f , (Single) Scene_Sprites.AppHeight / 2f);

                this.velocity                       = RandomGenerator.Default.GetRandomVector2 (-Settings.MaxVelocity, Settings.MaxVelocity);
                this.deltaRotation                  = RandomGenerator.Default.GetRandomSingle (-0.5f, 0.5f);
                this.deltaScale                     = RandomGenerator.Default.GetRandomSingle (-0.2f, 0.2f);

                this.sprite                         = this.Parent.AddTrait<SpriteTrait>();
                this.sprite.Texture                 = Scene_Sprites.texZa;
				this.sprite.Material.BlendMode      = Settings.BlendModes [currentBlendMode];
                this.sprite.DebugRender             = null;
                this.sprite.Rotation                = RandomGenerator.Default.GetRandomSingle (0, Settings.PI);
                this.sprite.Position                = new Vector2 (x, y);
                this.sprite.Scale                   = RandomGenerator.Default.GetRandomSingle (1f - Settings.ScaleVariation, 1f + Settings.ScaleVariation);
                this.sprite.Depth                   = Settings.SpriteDepth;
                this.sprite.Width                   = Scene_Sprites.texZa.Width;
                this.sprite.Height                  = Scene_Sprites.texZa.Height;
                this.sprite.Colour                  = RandomGenerator.Default.GetRandomColour ();
                this.sprite.FlipVertical            = RandomGenerator.Default.GetRandomBoolean ();
                this.sprite.FlipHorizontal          = RandomGenerator.Default.GetRandomBoolean ();
            }

			public void NextBlendMode ()
			{
				this.sprite.Material.BlendMode =
                    Settings.BlendModes [++currentBlendMode >= Settings.BlendModes.Length ? currentBlendMode = 0 : currentBlendMode];
			}

            public void EnabledDebugRenderer (Boolean on)
            {
                this.sprite.DebugRender = on ? "Default" : null;
            }

            public override void OnUpdate (AppTime time)
            {
                Single width = (Single) Scene_Sprites.AppWidth / 2f;
                Single height = (Single) Scene_Sprites.AppHeight / 2f;

                this.sprite.Position += this.velocity * time.Delta;

                if (this.sprite.Position.X > width)
                {
                    this.velocity.X = -this.velocity.X;
                    this.sprite.Position = new Vector2 (width, this.sprite.Position.Y);
                }

                if (this.sprite.Position.X < -width)
                {
                    this.velocity.X = -this.velocity.X;
                    this.sprite.Position = new Vector2 (-width, this.sprite.Position.Y);
                }

                if (this.sprite.Position.Y > height)
                {
                    this.velocity.Y = -this.velocity.Y;
                    this.sprite.Position = new Vector2 (this.sprite.Position.X, height);
                }

                if (this.sprite.Position.Y < -height)
                {
                    this.velocity.Y = -this.velocity.Y;
                    this.sprite.Position = new Vector2 (this.sprite.Position.X, -height);
                }

                this.sprite.Scale += this.deltaScale * time.Delta;

                if (this.sprite.Scale > 1.2f)
                {
                    this.deltaScale = -this.deltaScale;
                    this.sprite.Scale = 1.2f;
                }

                if (this.sprite.Scale < 0.8f)
                {
                    this.deltaScale = -this.deltaScale;
                    this.sprite.Scale = 0.8f;
                }

                this.sprite.Rotation += this.deltaRotation * time.Delta;
            }

            public override void OnDestroy ()
            {

            }
        }

        Scene returnScene;
        Int32 currentNumHares = 16;
        Boolean debugLinesOn = false;
        Single timer = 0f;

        readonly List<Hare> hares = new List<Hare>();

        public static Int32 AppWidth;
        public static Int32 AppHeight;
        public static Texture texZa;
        public static Texture texBg;

        SpritePrimitive bgSprite;

        public Scene_Sprites ()
        {
            returnScene = this;
        }

        public override void Start ()
        {
            ShaderAsset unlitShaderAsset = this.Engine.Assets.Load<ShaderAsset> ("assets/unlit.bba");
            SpriteTrait.SpriteShader = this.Platform.Graphics.CreateShader (unlitShaderAsset);

			AppWidth = this.Platform.Status.Width;
			AppHeight = this.Platform.Status.Height;

            var ta_za = this.Engine.Assets.Load<TextureAsset> ("assets/zazaka.bba");
            texZa = this.Platform.Graphics.CreateTexture (ta_za);

            // Create the background.
            var ta_bg = this.Engine.Assets.Load<TextureAsset> ("assets/bg2.bba");
            texBg = this.Platform.Graphics.CreateTexture (ta_bg);
            /*
            var soBG = this.SceneGraph.CreateSceneObject ("bg");
            var spr = soBG.AddTrait <SpriteTrait> ();
            spr.Width = 64f;
            spr.Height = 64f;
            spr.Texture = texBg;
            spr.Depth = 1f;
            //spr.Material.Offset = new Vector2 (0.5f, 0.5f);
            spr.Material.SetColour ("MaterialColour", Rgba32.Yellow);

            */

            bgSprite = new SpritePrimitive (
                this.Engine.PrimitiveRenderer, texBg,
                64, 64,
                this.Platform.Host.ScreenSpecification.ScreenResolutionWidth,
                this.Platform.Host.ScreenSpecification.ScreenResolutionHeight);


            Single pi = 0;
            Maths.Pi (out pi);

            var newCamSo = this.SceneGraph.CreateSceneObject ("ortho");
            newCamSo.Transform.LocalPosition = new Vector3 (0, 0, 1);

            var orthoCam = newCamSo.AddTrait<CameraTrait>();
            orthoCam.NearPlaneDistance = 0;
            orthoCam.FarPlaneDistance = 2;
            orthoCam.Projection = CameraProjectionType.Orthographic;

            orthoCam.TempWORKOUTANICERWAY = true;

            this.RuntimeConfiguration.SetRenderPassCameraTo ("Debug", newCamSo);
            this.RuntimeConfiguration.SetRenderPassCameraTo ("Default", newCamSo);
			this.Configuration.BackgroundColour = Rgba32.Aquamarine;

            while (hares.Count != Settings.MaxHares)
            {
                var so = this.SceneGraph.CreateSceneObject ("hare #" + hares.Count);
                var hareTrait = so.AddTrait<Hare>();
                hares.Add (hareTrait);
            }

            for (Int32 i = 0; i < hares.Count; ++i)
            {
                var hareTrait = hares[i];
                hareTrait.Parent.Enabled = (i < currentNumHares);
            }

            this.Engine.InputEventSystem.Tap += this.HandleTap;

        }

        void HandleTap (Gesture gesture)
        {
            ChangeNumHares ();
        }

        void ChangeNumHares ()
        {
            currentNumHares = currentNumHares << 1;

            if (currentNumHares > Settings.MaxHares)
            {
                currentNumHares = Settings.MinHares;
            }

            for (Int32 i = 0; i < hares.Count; ++i)
            {
                var hareTrait = hares[i];
                hareTrait.Parent.Enabled = (i < currentNumHares);
            }
        }

        public override void Shutdown ()
        {
            this.Engine.InputEventSystem.Tap -= this.HandleTap;

			// Clean up the things we allocated on the GPU.
			this.Platform.Graphics.DestroyShader (SpriteTrait.SpriteShader);
            this.Platform.Graphics.DestroyTexture (texZa);
            this.Platform.Graphics.DestroyTexture (texBg);
			SpriteTrait.SpriteShader = null;
            texZa = null;
			texBg = null;
        }

        public override Scene Update (AppTime time)
        {
            AppWidth = this.Platform.Status.Width;
            AppHeight = this.Platform.Status.Height;

            bgSprite.DrawEx ("Debug", 0f, 0f, 0.0f, 1f / 100f, 1f / 100f);

            if (timer > 0f)
            {
                timer -= time.Delta;
            }

            if (timer <= 0f)
            {
				if (Platform.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
					Platform.Input.Keyboard.IsFunctionalKeyDown (FunctionalKey.Escape) ||
					Platform.Input.Keyboard.IsFunctionalKeyDown (FunctionalKey.Backspace))
                {
                    returnScene = new Scene_MainMenu ();
                }

				if (Platform.Input.GenericGamepad.Buttons.North == ButtonState.Pressed ||
                    Platform.Input.Keyboard.IsCharacterKeyDown ('d'))
                {
                    debugLinesOn = !debugLinesOn;
                    hares.ForEach (x => x.EnabledDebugRenderer (debugLinesOn));
                }

				if (Platform.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
                    Platform.Input.Keyboard.IsCharacterKeyDown ('b'))
                {
					hares.ForEach (x => x.NextBlendMode ());
                }

                if (Platform.Input.GenericGamepad.Buttons.South == ButtonState.Pressed ||
                    Platform.Input.Keyboard.IsFunctionalKeyDown (FunctionalKey.Spacebar))
                {
                    ChangeNumHares ();
                }

                timer = 0.2f;
            }

            if (debugLinesOn)
                this.Engine.DebugRenderer.AddGrid ("Debug");

            if (debugLinesOn)
            {
                Single left = -(Single)(AppWidth / 2) * Settings.PixelToWorldSpace;
                Single right = (Single)(AppWidth / 2) * Settings.PixelToWorldSpace;
                Single top = (Single)(AppHeight / 2) * Settings.PixelToWorldSpace;
                Single bottom = -(Single)(AppHeight / 2) * Settings.PixelToWorldSpace;

                this.Engine.DebugRenderer.AddQuad (
                    "Default",
                    new Vector3 (left, bottom, 0),
                    new Vector3 (right, bottom, 0),
                    new Vector3 (right, top, 0),
                    new Vector3 (left, top, 0),
                    Rgba32.Yellow);
            }


            return returnScene;
        }

    }
}

