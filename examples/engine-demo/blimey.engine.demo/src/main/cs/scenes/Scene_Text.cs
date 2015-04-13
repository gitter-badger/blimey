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

    class SpriteFontData
    {
        public String FontFace { get; set; }
        public Int32 FontSize { get; set; }
        public Int32 LineHeight { get; set; }
        public Int32 BaseLine { get; set; }

        readonly Dictionary <Char, SpriteFontCharacterData> characterData =
            new Dictionary<Char, SpriteFontCharacterData> ();

        readonly Dictionary <SpriteFontKerningPair, Int32> kerning =
            new Dictionary<SpriteFontKerningPair, Int32> ();

        public SpriteFontData (String bmFontFile)
        {
        }
        /*
        #fontFace="Arial" #fontSize=50
        #lineHeight=56 #baseLine=46 textureWidth=384 #textureHeight=320
        #textureFile="blimey.png"
        #charsCount=95
        #charId=64 #uvStart=(0.0052, 0.0063) #uvEnd=(0.1354, 0.1625) #size=(50.0, 50.0) #xOffset=3 #yOffset=8 #xAdvance=51
        ...
        #kerningsCount=64
        #kerning #first=86 #second=117 #amount=-1
        ...
        */

    }

    struct SpriteFontKerningPair
    {
        public Char First;
        public Char Second;
    }

    class SpriteFontCharacterData
    {
        public Vector2 StartUV { get; set; }
        public Vector2 EndUV { get; set; }
        public Vector2 Size { get; set; }
        public Int32 XOffset { get; set; }
        public Int32 YOffset { get; set; }
        public Int32 XAdvance { get; set; }
    }

    class SpriteFont
    {
        readonly Texture fntTex = null;
        readonly String fntUvData = null;
        SpritePrimitive s;

        public SpriteFont (Platform platform, Engine engine, TextAsset fntUvAsset, TextureAsset fntTexAsset)
        {
            fntUvData = fntUvAsset.Text;
            fntTex = platform.Graphics.CreateTexture (fntTexAsset);
            Init (platform, engine);
        }

        public SpriteFont (Platform platform, Engine engine, String fntUvData, Texture fntTex)
        {
            this.fntUvData = fntUvData;
            this.fntTex = fntTex;
            Init (platform, engine);
        }

        void Init (Platform platform, Engine engine)
        {
            Console.WriteLine (fntUvData);

            s = new SpritePrimitive (engine.PrimitiveRenderer, fntTex);
            s.SetBlendMode (BlendMode.Default);
        }

        public void Draw (Single x, Single y, String text)
        {
            s.SetColour (Rgba32.Red);
            s.SetTextureRect (0.2396f * fntTex.Width, 0.9500f * fntTex.Height, 0.0312f * fntTex.Width, 0.0312f * fntTex.Height, false);
            s.DrawEx ("Gui", 0f, 0f, 0f, 1f / 4f / 256f , 1f / 4f / 256f);

            s.SetColour (Rgba32.Yellow);
            s.SetTextureRect (0.0052f * fntTex.Width, 0.0063f * fntTex.Height, 0.1354f * fntTex.Width, 0.1625f * fntTex.Height, false);
            s.DrawEx ("Gui", 0.2f, 0f, 0f, 1f / 4f / 256f , 1f / 4f / 256f);
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

	public class Scene_Text
		: Scene
	{
		Scene returnScene;
        Texture fntTex = null;
        SpriteFont sprFnt = null;

		public override void Start()
		{
			this.Configuration.BackgroundColour = Rgba32.DarkSlateGrey;

            TextAsset fntUvAsset = this.Engine.Assets.Load <TextAsset> ("assets/blimey_fnt_uv.bba");
            TextureAsset fntTexAsset = this.Engine.Assets.Load <TextureAsset> ("assets/blimey_fnt_tex.bba");

            fntTex = this.Platform.Graphics.CreateTexture (fntTexAsset);
            sprFnt = new SpriteFont (this.Platform, this.Engine, fntUvAsset, fntTexAsset);

			returnScene = this;

			this.CameraManager.GetRenderPassCamera ("Debug").GetTrait<CameraTrait> ().Projection = CameraProjectionType.Orthographic;
            this.CameraManager.GetRenderPassCamera ("Default").GetTrait<CameraTrait> ().Projection = CameraProjectionType.Orthographic;
            this.CameraManager.GetRenderPassCamera ("Gui").GetTrait<CameraTrait> ().Projection = CameraProjectionType.Orthographic;


            /*

            var newCamSo = this.SceneGraph.CreateSceneObject("ortho");
            newCamSo.Transform.LocalPosition = new Vector3(0, 0, 1);

            var orthoCam = newCamSo.AddTrait<CameraTrait>();
            orthoCam.NearPlaneDistance = 0;
            orthoCam.FarPlaneDistance = 2;
            orthoCam.Projection = CameraProjectionType.Orthographic;
            orthoCam.ortho_width = this.Platform.Status.Width;
            orthoCam.ortho_height = this.Platform.Status.Height;
            orthoCam.ortho_zoom = 8f;

            this.RuntimeConfiguration.SetRenderPassCameraTo("Default", newCamSo);
            this.RuntimeConfiguration.SetRenderPassCameraTo("Gui", newCamSo);
            this.RuntimeConfiguration.SetRenderPassCameraTo("Debug", newCamSo);

            */

			this.Engine.InputEventSystem.Tap += this.OnTap;
		}

		public override void Shutdown()
		{
			this.Engine.InputEventSystem.Tap -= this.OnTap;
		}

		public override Scene Update(AppTime time)
        {
            this.Engine.DebugRenderer.AddGrid ("Gui", 1f, 10);

            sprFnt.Draw (0, 0, "Hello");

			if (Platform.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
				Platform.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Escape) ||
				Platform.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Backspace))
			{
				returnScene = new Scene_MainMenu();
			}

			return returnScene;
		}

		void OnTap(Gesture gesture)
		{
			returnScene = new Scene_MainMenu();
		}
	}
}

