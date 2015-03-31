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

namespace EngineDemo
{
    using System;
    using Fudge;
    using Abacus.SinglePrecision;
    using Blimey;
    using System.Collections.Generic;

    class SpriteFontData
    {
        public String FontFace { get; set; }
        public Int32 FontSize { get; set; }
        public Int32 LineHeight { get; set; }
        public Int32 BaseLine { get; set; }

        readonly Dictionary <Char, SpriteFontCharacterData> characterData = new Dictionary<Char, SpriteFontCharacterData> ();

        readonly Dictionary <SpriteFontKerningPair, Int32> kerning = new Dictionary<SpriteFontKerningPair, Int32> ();

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
        PrimitiveRenderer.Sprite s;

        public SpriteFont (Engine engine, Blimey blimey, TextAsset fntUvAsset, TextureAsset fntTexAsset)
        {
            fntUvData = fntUvAsset.Text;
            fntTex = engine.Graphics.CreateTexture (fntTexAsset);
            Init (engine ,blimey);
        }

        public SpriteFont (Engine engine, Blimey blimey, String fntUvData, Texture fntTex)
        {
            this.fntUvData = fntUvData;
            this.fntTex = fntTex;
            Init (engine, blimey);
        }

        void Init (Engine engine, Blimey blimey)
        {
            Console.WriteLine (fntUvData);

            s = new PrimitiveRenderer.Sprite (blimey.PrimitiveRenderer, fntTex);
            s.SetBlendMode (BlendMode.Default);
            s.SetColour (Rgba32.Red);
            s.SetTextureRect (0.0052f, 0.0063f, 0.1354f, 0.1625f);
        }

        public void Draw (Single x, Single y, String text)
        {
            s.DrawEx ("Gui", 0f, 0f, 0f, 1f / 256f / 4f, 1f / 256f / 4f);
        }
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

	public class Scene_Text
		: Scene
	{
		Scene returnScene;
        Shader unlitShader = null;
        Texture fntTex = null;
        SpriteFont sprFnt = null;

		public override void Start()
		{
			this.Configuration.BackgroundColour = Rgba32.DarkSlateGrey;

			// set up the debug renderer
            ShaderAsset unlitShaderAsset = this.Blimey.Assets.Load<ShaderAsset> ("assets/unlit.bba");

			SpriteTrait.SpriteShader = this.Cor.Graphics.CreateShader(unlitShaderAsset);

            TextAsset fntUvAsset = this.Blimey.Assets.Load <TextAsset> ("assets/blimey_fnt_uv.bba");
            TextureAsset fntTexAsset = this.Blimey.Assets.Load <TextureAsset> ("assets/blimey_fnt_tex.bba");

            fntTex = this.Cor.Graphics.CreateTexture (fntTexAsset);

            sprFnt = new SpriteFont (this.Cor, this.Blimey, fntUvAsset.Text, fntTex);

			returnScene = this;

			var cam = this.CameraManager.GetRenderPassCamera ("Default");
			cam.GetTrait <OrbitAroundSubjectTrait> ().Active = false;

			// create a sprite
			var so = this.SceneGraph.CreateSceneObject ("fnt_spr");

			var spr = so.AddTrait <SpriteTrait> ();
			spr.Width = 256f;
			spr.Height = 256f;
			spr.Texture = fntTex;
			//spr.Material.Offset = new Vector2 (0.5f, 0.5f);
			spr.Material.SetColour ("MaterialColour", Rgba32.Yellow);

			unlitShader = this.Cor.Graphics.CreateShader (unlitShaderAsset);


			this.Blimey.InputEventSystem.Tap += this.OnTap;
		}

		public override void Shutdown()
		{
			this.Blimey.InputEventSystem.Tap -= this.OnTap;
		}

		public override Scene Update(AppTime time)
        {
            this.Blimey.DebugRenderer.AddGrid ("Debug", 1f, 10);

            sprFnt.Draw (0, 0, "");

			if (Cor.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
				Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Escape) ||
				Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Backspace))
			{
				returnScene = new Scene_MainMenu();
			}

			return returnScene;
		}

		void OnTap(Gesture gesture)
		{
			returnScene = new Scene_MainMenu();

			// Clean up the things we allocated on the GPU.
			this.Cor.Graphics.DestroyShader (unlitShader);
			unlitShader = null;
		}
	}
}

