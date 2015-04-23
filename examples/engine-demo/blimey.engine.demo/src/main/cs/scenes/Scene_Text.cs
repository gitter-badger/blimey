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
    using System.Text.RegularExpressions;

    class SpriteFontData
    {
        public String FontFace { get; set; }
        public Int32 FontSize { get; set; }
        public Int32 LineHeight { get; set; }
        public Int32 BaseLine { get; set; }

        public readonly Dictionary <Char, SpriteFontCharacterData> CharacterData =
            new Dictionary<Char, SpriteFontCharacterData> ();

        public readonly Dictionary <SpriteFontKerningPair, Int32> Kerning =
            new Dictionary<SpriteFontKerningPair, Int32> ();

        public static SpriteFontData Parse (String[] lines)
        {
            var spriteFontData = new SpriteFontData ();

            foreach (var line in lines)
            {
                var m0 = Regex.Match (line, "#?fontFace=\"([A-Za-z]+)\" +#?fontSize=([0-9]+)");
                if (m0.Success)
                {
                    spriteFontData.FontFace = m0.GetString (0);
                    spriteFontData.FontSize = m0.GetInt32 (1);
                    continue;
                }

                var m1 = Regex.Match (line, "#?lineHeight=([0-9]+) +#?baseLine=([0-9]+)");
                if (m1.Success)
                {
                    spriteFontData.LineHeight = m1.GetInt32 (0);
                    spriteFontData.BaseLine = m1.GetInt32 (1);
                    continue;
                }

                var m2 = Regex.Match (line, "#?charId=([0-9]+) +#?uvStart=\\(([0-9]+.[0-9]+), ([0-9]+.[0-9]+)\\) +#?uvEnd=\\(([0-9]+.[0-9]+), ([0-9]+.[0-9]+)\\) +#?size=\\(([0-9]+.[0-9]+), ([0-9]+.[0-9]+)\\) +#?xOffset=([0-9]+) +#?yOffset=([0-9]+) +#?xAdvance=([0-9]+)");
                if (m2.Success)
                {
                    Char character = (Char) m2.GetInt32 (0);
                    var data = new SpriteFontCharacterData ();
                    data.StartUV = new Vector2 { X = m2.GetSingle (1), Y =  m2.GetSingle (2) };
                    data.EndUV = new Vector2 { X = m2.GetSingle (3), Y =  m2.GetSingle (4) };
                    data.Size = new Vector2 { X = m2.GetSingle (5), Y =  m2.GetSingle (6) };
                    data.XOffset = m2.GetInt32 (7);
                    data.YOffset = m2.GetInt32 (8);
                    data.XAdvance = m2.GetInt32 (9);
                    spriteFontData.CharacterData.Add (character, data);
                    continue;
                }

                var m3 = Regex.Match (line, "#?kerning #?first=([0-9]+) +#?second=([0-9]+) +#?amount=(-?[0-9]+)");
                if (m3.Success)
                {
                    var kerningPair = new SpriteFontKerningPair { First = (Char) m3.GetInt32 (0), Second = (Char) m3.GetInt32 (1) };
                    var kerningAmount = m3.GetInt32 (2);
                    spriteFontData.Kerning.Add (kerningPair, kerningAmount);
                    continue;
                }
            }

            return spriteFontData;
        }
    }

    struct SpriteFontKerningPair
    {
        public Char First;
        public Char Second;

        public override Int32 GetHashCode () { return First.GetHashCode () ^ Second.GetHashCode (); }

        public override Boolean Equals (Object obj)
        {
            if (obj == null) return false;
            if (obj.GetType () != base.GetType ()) return false;
            return (this == ((SpriteFontKerningPair)obj));
        }

        public static Boolean operator == (SpriteFontKerningPair left, SpriteFontKerningPair right)
        {
            return (left.First == right.First)
                && (left.Second == right.Second);
        }
        public static Boolean operator != (SpriteFontKerningPair left, SpriteFontKerningPair right) { return !(left == right); }
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

            //#charId=64 #uvStart=(0.0052, 0.0063) #uvEnd=(0.1354, 0.1625) #size=(50.0, 50.0) #xOffset=3 #yOffset=8 #xAdvance=51
            s.SetColour (Rgba32.Yellow);
            s.SetTextureRect (
                0.0052f * fntTex.Width, 0.0063f * fntTex.Height,
                (0.1354f-0.0052f) * fntTex.Width, (0.1625f-0.0063f) * fntTex.Height, false);
            s.DrawEx ("Gui", 0.0f, 0f, 0f, 1f / 8f / 256f , 1f / 8f / 256f);

            //#charId=65 #uvStart=(0.1406, 0.3812) #uvEnd=(0.2344, 0.5000) #size=(36.0, 38.0) #xOffset=0 #yOffset=10 #xAdvance=34
            //#charId=65 #uvStart=(0.1406, 0.3812) #uvEnd=(0.2344, 0.5000) #size=(36.0, 38.0) #xOffset=0 #yOffset=10 #xAdvance=34
            s.SetColour (Rgba32.Red);
            s.SetTextureRect (
                0.1406f * fntTex.Width, 0.3812f * fntTex.Height,
                (0.2344f-0.1406f) * fntTex.Width, (0.5000f-0.3812f) * fntTex.Height, false);
            s.DrawEx ("Gui", 0.3f, 0f, 0f, 1f / 8f / 256f , 1f / 8f / 256f);

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
            this.Engine.DebugRenderer.AddAxis ("Gui");

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

