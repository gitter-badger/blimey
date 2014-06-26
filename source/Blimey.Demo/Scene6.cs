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
using Fudge;
using Abacus.SinglePrecision;
using Cor;
using System.Collections.Generic;

namespace Blimey.Demo
{
	public class Scene6
		: Scene
	{
		Scene returnScene;
		GridRenderer gr;
		IShader unlitShader = null;
		ITexture fntTex = null;

		public override void Start()
		{
			this.Configuration.BackgroundColour = Rgba32.DarkSlateGrey;

			// set up the debug renderer
			ShaderAsset unlitShaderAsset = this.Cor.Assets.Load<ShaderAsset> ("unlit.cba");
			this.Blimey.DebugShapeRenderer.DebugShader = 
				this.Cor.Graphics.CreateShader (unlitShaderAsset);
			gr = new GridRenderer(this.Blimey.DebugShapeRenderer, "Default", 1f, 10);

			Sprite.SpriteShader = this.Cor.Graphics.CreateShader(unlitShaderAsset);

			TextAsset fntUvAsset = this.Cor.Assets.Load <TextAsset> ("blimey_fnt_uv.cba");

			Console.WriteLine (fntUvAsset.Text);

			TextureAsset fntTexAsset = this.Cor.Assets.Load <TextureAsset> ("blimey_fnt_tex.cba");

			fntTex = this.Cor.Graphics.UploadTexture (fntTexAsset);

			returnScene = this;

			var cam = this.CameraManager.GetRenderPassCamera ("Default");
			cam.GetTrait <OrbitAroundSubject> ().Active = false;

			// create a sprite
			var so = this.SceneGraph.CreateSceneObject ("fnt_spr");

			var spr = so.AddTrait <Sprite> ();
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
			gr.Update ();

			if (Cor.Input.GenericGamepad.Buttons.East == ButtonState.Pressed ||
				Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Escape) ||
				Cor.Input.Keyboard.IsFunctionalKeyDown(FunctionalKey.Backspace))
			{
				returnScene = new MainMenuScene();
			}

			return returnScene;
		}

		void OnTap(Gesture gesture)
		{
			returnScene = new MainMenuScene();

			// Clean up the things we allocated on the GPU.
			this.Cor.Graphics.DestroyShader (unlitShader);
			unlitShader = null;
		}
	}
}

