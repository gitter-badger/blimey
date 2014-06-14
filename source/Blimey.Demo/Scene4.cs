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
// │ Copyright © 2014 A.J.Pook (http://ajpook.github.io)                    │ \\
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
        public class Hare
            : Trait
        {
            Vector2 velocity;
            Single deltaScale;
            Single deltaRotation;
    
            Sprite sprite;
			
			int currentBlendMode = 0;
			
			readonly BlendMode[] blendModes = new BlendMode[]
			{
				BlendMode.Default,
				BlendMode.Additive,
				BlendMode.Subtract,
				BlendMode.Opaque
			};
    
            public override void OnAwake()
            {
                this.sprite = this.Parent.AddTrait<Sprite>();
    
                this.sprite.Texture = Scene4.texZa;
    
				this.sprite.Material.BlendMode = blendModes [currentBlendMode];
    
                this.sprite.DebugRender = null;
    
                Single width = (Single) Scene4.screenWidth / 2f;
                Single height = (Single) Scene4.screenHeight / 2f;
    
                this.velocity = RandomGenerator.Default.GetRandomVector2(-200, 200);
                this.deltaRotation = RandomGenerator.Default.GetRandomSingle(-0.5f, 0.5f);
    
                this.deltaScale = RandomGenerator.Default.GetRandomSingle(-0.2f, 0.2f);
    
                Single pi;
                RealMaths.Pi(out pi);
    
                this.sprite.Rotation = RandomGenerator.Default.GetRandomSingle(0, pi);
                Single x = RandomGenerator.Default.GetRandomSingle( 0, width );
                Single y = RandomGenerator.Default.GetRandomSingle( 0 , height );
                this.sprite.Position = new Vector2(x, y);
                this.sprite.Scale = RandomGenerator.Default.GetRandomSingle(0.8f, 1.2f);
    
                this.sprite.Width = 64f;
                this.sprite.Height = 64f;
    
                this.sprite.Colour = RandomGenerator.Default.GetRandomColour();
                //this.sprite.FlipVertical = RandomGenerator.Default.GetRandomBoolean();
                //this.sprite.FlipHorizontal = RandomGenerator.Default.GetRandomBoolean();
            }
			
			public void NextBlendMode ()
			{
				this.sprite.Material.BlendMode = 
					blendModes [++currentBlendMode >= blendModes.Length ? currentBlendMode = 0 : currentBlendMode];
			}
    
            public void EnabledDebugRenderer (Boolean on)
            {
                this.sprite.DebugRender = on ? "Default" : null;
            }
    
            public override void OnUpdate(AppTime time)
            {
                Single width = (Single) Scene4.screenWidth / 2f;
                Single height = (Single) Scene4.screenHeight / 2f;
    
                this.sprite.Position += this.velocity * time.Delta;
    
                if (this.sprite.Position.X > width || this.sprite.Position.X < -width)
                {
                    this.velocity.X = -this.velocity.X;
                }
    
                if (this.sprite.Position.Y > height || this.sprite.Position.Y < -height)
                {
                    this.velocity.Y = -this.velocity.Y;
                }
    
                this.sprite.Scale += this.deltaScale * time.Delta;
    
                if (this.sprite.Scale > 1.2f || this.sprite.Scale < 0.8f)
                {
                    this.deltaScale = -this.deltaScale;
                }
    
                this.sprite.Rotation += this.deltaRotation * time.Delta;
            }
    
            public override void OnDestroy ()
            {
    
            }
        }
        Scene _returnScene;

        const Int32 MinHares = 16;
        const Int32 MaxHares = 256;

        Int32 currentNumVans = 32;

        readonly List<Hare> hares = new List<Hare>();

        GridRenderer gr;

        bool debugLinesOn = false;

        Single timer = 0f;

        public static Int32 screenWidth;
        public static Int32 screenHeight;
        public static ITexture texZa;
        public static ITexture texBg;

        public Scene4()
        {
            _returnScene = this;
        }

        public override void Start ()
        {
			ShaderAsset unlitShaderAsset = this.Cor.Assets.Load<ShaderAsset> ("unlit.cba");
			this.Blimey.DebugShapeRenderer.DebugShader = this.Cor.Graphics.CreateShader (unlitShaderAsset);
            Sprite.SpriteShader = this.Cor.Graphics.CreateShader(unlitShaderAsset);
            
            gr = new GridRenderer (this.Blimey.DebugShapeRenderer, "Default");

			screenWidth = this.Cor.AppStatus.Width;
			screenHeight = this.Cor.AppStatus.Height;

			var ta_za = this.Cor.Assets.Load<TextureAsset> ("zazaka.cba");
            var ta_bg = this.Cor.Assets.Load<TextureAsset> ("bg2.cba");
			texZa = this.Cor.Graphics.UploadTexture (ta_za);
            texBg = this.Cor.Graphics.UploadTexture (ta_bg);
            
            var soBG = this.SceneGraph.CreateSceneObject ("bg");

            var spr = soBG.AddTrait <Sprite> ();
            spr.Width = 256f;
            spr.Height = 256f;
            spr.Texture = texBg;
            spr.Depth = 1f;
            //spr.Material.Offset = new Vector2 (0.5f, 0.5f);
            spr.Material.SetColour ("MaterialColour", Rgba32.Yellow);
            

            Single pi = 0;
            RealMaths.Pi(out pi);

            var newCamSo = this.SceneGraph.CreateSceneObject("ortho");
            newCamSo.Transform.LocalPosition = new Vector3(0, 0, 1);

            var orthoCam = newCamSo.AddTrait<Camera>();
            orthoCam.NearPlaneDistance = 0;
            orthoCam.FarPlaneDistance = 2;
            orthoCam.Projection = CameraProjectionType.Orthographic;

            orthoCam.TempWORKOUTANICERWAY = true;

            this.RuntimeConfiguration.SetRenderPassCameraTo("Default", newCamSo);
			this.Configuration.BackgroundColour = Rgba32.Aquamarine;

            while (hares.Count != MaxHares )
            {
                var so = this.SceneGraph.CreateSceneObject("van #" + hares.Count);
                var vanTrait = so.AddTrait<Hare>();
                hares.Add(vanTrait);

            }

            this.Blimey.InputEventSystem.Tap += this.HandleTap;

        }
        
        void HandleTap (Gesture gesture)
        {
            ChangeNumHares ();
        }
            
            
        void ChangeNumHares ()
        { 
            currentNumVans = currentNumVans << 1;

            if( currentNumVans > MaxHares )
            {
                currentNumVans = MinHares;
            }
            
            for(Int32 i = 0; i < hares.Count; ++i)
            {
                var vanTrait = hares[i];

                vanTrait.Parent.Enabled = (i < currentNumVans);
                
            }
        }

        public override void Shutdown()
        {
            this.Blimey.InputEventSystem.Tap -= this.HandleTap;

			// Clean up the things we allocated on the GPU.
			this.Cor.Graphics.DestroyShader (Sprite.SpriteShader);
			this.Cor.Graphics.UnloadTexture (texZa);
            this.Cor.Graphics.UnloadTexture (texBg);
			Sprite.SpriteShader = null;
            texZa = null;
			texBg = null;
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
				if (Cor.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
					Cor.Input.Keyboard.IsFunctionalKeyDown (FunctionalKey.Escape) ||
					Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Backspace))
                {
                    _returnScene = new MainMenuScene ();
                }

				if (Cor.Input.GenericGamepad.Buttons.North == ButtonState.Pressed ||
                    Cor.Input.Keyboard.IsCharacterKeyDown ('d'))
                {
                    debugLinesOn = !debugLinesOn;

                    hares.ForEach( x => x.EnabledDebugRenderer(debugLinesOn) );
                }

				if (Cor.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
                    Cor.Input.Keyboard.IsCharacterKeyDown ('b'))
                {
					hares.ForEach(x => x.NextBlendMode());
                }
                
                if (Cor.Input.GenericGamepad.Buttons.South == ButtonState.Pressed ||
                    Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Spacebar))
                {
                    ChangeNumHares ();
                }

                timer = 0.1f;
            }
            
            if (debugLinesOn)
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

