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
using System.Collections.Generic;
using Sungiant.Abacus;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Cor;

namespace Sungiant.Blimey
{
	public class Material
	{
		IShader shader;
		string renderPass;
		//Dictionary<string, EffectSettingsData> materialSettings = new Dictionary<string, EffectSettingsData>();

		public BlendMode BlendMode { get; set; }
		public string RenderPass { get { return renderPass; } }

		public Material(string renderPass, IShader shader)
		{
			this.BlendMode = BlendMode.Default;

			this.renderPass = renderPass;
			this.shader = shader;
		}

		internal void CalibrateShader(Matrix44 world, Matrix44 view, Matrix44 proj)
		{
			if(shader == null)
				return;

			shader.SetVariable ("World", world);
			shader.SetVariable ("View", view);
			shader.SetVariable ("Projection", proj);
		}

		internal IShader GetShader()
		{
			return shader;
		}

		public Vector2 Tiling { get; set; }
		public Vector2 Offset { get; set; }

		internal void CalibrateGpu(IGraphicsManager graphics)
		{
			BlendMode.Apply (this.BlendMode, graphics);

			int i = 0;
			foreach(var key in t.Keys)
			{
				
				shader.SetVariable (key, i);
				graphics.SetActiveTexture (i, t[key]);

				i++;

			}
			/*
			// this needs to be better, should have to set this every frame, only if it's changed.
			if (materialSettings.ContainsKey("_tex0"))
			{
				var effectSetting = materialSettings["_tex0"];

				if( effectSetting.Type == typeof(Texture2D) )
				{
					var tex = (effectSetting.Value as Texture2D);
					// so we know which texture we want to use, we have the texture object,
					// therefore it's in gpu memory however it might not be bound to a texture
					// slot, therefore we should get request that cor! to binds it to a slot.
					graphics.SetActiveTexture (0, tex.texture);
				}
			}*/

		}

		Dictionary<string, Texture2D> t = new Dictionary<string, Texture2D>();


		public void SetColour(string propertyName, Rgba32 colour)
		{
			shader.SetVariable (propertyName, colour);
		}

        public void SetFloat(string propertyName, Single value)
		{
			shader.SetVariable (propertyName, value);
		}

		public void SetMatrix(string propertyName, Matrix44 matrix)
		{
			shader.SetVariable (propertyName, matrix);
		}

		public void SetVector(string propertyName, Vector4 vector)
		{
			shader.SetVariable (propertyName, vector);
		}

		public void SetTexture(string propertyName, Texture2D texture)
		{
			t[ propertyName ] = texture;

			//shader.SetVariable (propertyName, texture);
		}

		public void SetTextureOffset(string propertyName, Vector2 offset)
		{
			shader.SetVariable (propertyName, offset);
		}

		public void SetTextureScale(string propertyName, Vector2 scale)
		{
			shader.SetVariable (propertyName, scale);
		}

	}
}
