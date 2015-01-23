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

namespace Blimey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    
    using Fudge;
    using Abacus.SinglePrecision;
    
    using System.Linq;
    using Cor;
    using Platform;
	
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
	
	public static class GraphicsExtensions
	{
		static BlendMode lastSet = BlendMode.Default;
        static Boolean neverSet = true;

        public static void SetBlendEquation (this Graphics graphics, BlendMode blendMode)
        {
            if (neverSet || lastSet != blendMode)
            {
                graphics.SetBlendEquation (
                    blendMode.RgbBlendFunction, blendMode.SourceRgb, blendMode.DestinationRgb,
                    blendMode.AlphaBlendFunction, blendMode.SourceAlpha, blendMode.DestinationAlpha
                    );

                neverSet = false;
                lastSet = blendMode;
            }
        }

        public static Shader CreateShader (this Graphics graphics, ShaderAsset shaderAsset)
        {
            return graphics.CreateShader (shaderAsset.Declaration, shaderAsset.Format, shaderAsset.Source);
        }

        public static Texture CreateTexture (this Graphics graphics, TextureAsset textureAsset)
        {
            return graphics.CreateTexture (textureAsset.TextureFormat, textureAsset.Width, textureAsset.Height, textureAsset.Data);
        }
	}


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public enum CameraProjectionType
    {
        Perspective,
        Orthographic,
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

	/// <summary>
	/// Provides Blimey consumers a means to configure the GPU's blending equation.
	/// </summary>
    public struct BlendMode
        : IEquatable<BlendMode>
    {
        BlendFunction rgbBlendFunction;
        BlendFactor sourceRgb;
        BlendFactor destinationRgb;

        BlendFunction alphaBlendFunction;
        BlendFactor sourceAlpha;
        BlendFactor destinationAlpha;
		
		internal BlendFunction RgbBlendFunction { get { return rgbBlendFunction;} }
        internal BlendFactor SourceRgb { get { return sourceRgb;} }
        internal BlendFactor DestinationRgb { get { return destinationRgb;} }

        internal BlendFunction AlphaBlendFunction { get { return alphaBlendFunction;} }
        internal BlendFactor SourceAlpha { get { return sourceAlpha;} }
        internal BlendFactor DestinationAlpha { get { return destinationAlpha;} }

        public override String ToString ()
        {
            return string.Format (
                "{{rgbBlendFunction:{0} sourceRgb:{1} destinationRgb:{2}" + 
				" alphaBlendFunction:{3} sourceAlpha:{4} destinationAlpha:{5}}}",
				rgbBlendFunction.ToString (), sourceRgb.ToString (), destinationRgb.ToString (),
                alphaBlendFunction.ToString (), sourceAlpha.ToString (), destinationAlpha.ToString ()
            );
        }

        public Boolean Equals (BlendMode other)
        {
            return this == other;
        }

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;
            if (obj is BlendMode) {
                flag = this.Equals ((BlendMode)obj);
            }
            return flag;
        }

        public override Int32 GetHashCode ()
        {
            int a = (int) rgbBlendFunction.GetHashCode();
            int b = (int) sourceRgb.GetHashCode();
            int c = (int) destinationRgb.GetHashCode();

            int d = (int) alphaBlendFunction.GetHashCode();
            int e = (int) sourceAlpha.GetHashCode();
            int f = (int) destinationAlpha.GetHashCode();

			return a.ShiftAndWrap(10) ^ b.ShiftAndWrap(8) ^ c.ShiftAndWrap(6) ^ d.ShiftAndWrap(4) ^ e.ShiftAndWrap(2) ^ f;
        }

        public static Boolean operator != (BlendMode value1, BlendMode value2)
        {
            return !(value1 == value2);
        }

        public static Boolean operator == (BlendMode value1, BlendMode value2)
        {
            if (value1.rgbBlendFunction != value2.rgbBlendFunction) return false;
            if (value1.sourceRgb != value2.sourceRgb) return false;
            if (value1.destinationRgb != value2.destinationRgb) return false;
            if (value1.alphaBlendFunction != value2.alphaBlendFunction) return false;
            if (value1.sourceAlpha != value2.sourceAlpha) return false;
            if (value1.destinationAlpha != value2.destinationAlpha) return false;

            return true;
        }

        public static BlendMode Default
        {
            get
            {
                var blendMode = new BlendMode();

                blendMode.rgbBlendFunction =    BlendFunction.Add;
                blendMode.sourceRgb =           BlendFactor.SourceAlpha;
                blendMode.destinationRgb =      BlendFactor.InverseSourceAlpha;

                blendMode.alphaBlendFunction =  BlendFunction.Add;
                blendMode.sourceAlpha =         BlendFactor.One;
                blendMode.destinationAlpha =    BlendFactor.InverseSourceAlpha;

                return blendMode;
            }
        }

        public static BlendMode Opaque
        {
            get
            {
                var blendMode = new BlendMode();

                blendMode.rgbBlendFunction =    BlendFunction.Add;
                blendMode.sourceRgb =           BlendFactor.One;
                blendMode.destinationRgb =      BlendFactor.Zero;

                blendMode.alphaBlendFunction =  BlendFunction.Add;
                blendMode.sourceAlpha =         BlendFactor.One;
                blendMode.destinationAlpha =    BlendFactor.Zero;

                return blendMode;
            }
        }

        public static BlendMode Subtract
        {
            get
            {
                var blendMode = new BlendMode();

                blendMode.rgbBlendFunction =    BlendFunction.ReverseSubtract;
                blendMode.sourceRgb =           BlendFactor.SourceAlpha;
                blendMode.destinationRgb =      BlendFactor.One;

                blendMode.alphaBlendFunction =  BlendFunction.ReverseSubtract;
                blendMode.sourceAlpha =         BlendFactor.SourceAlpha;
                blendMode.destinationAlpha =    BlendFactor.One;

                return blendMode;
            }
        }

        public static BlendMode Additive
        {
            get
            {
                var blendMode = new BlendMode();

                blendMode.rgbBlendFunction =    BlendFunction.Add;
                blendMode.sourceRgb =           BlendFactor.SourceAlpha;
                blendMode.destinationRgb =      BlendFactor.One;

                blendMode.alphaBlendFunction =  BlendFunction.Add;
                blendMode.sourceAlpha =         BlendFactor.SourceAlpha;
                blendMode.destinationAlpha =    BlendFactor.One;

                return blendMode;
            }
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Material
    {
        Shader shader;
        string renderPass;

        public BlendMode BlendMode { get; set; }
        public string RenderPass { get { return renderPass; } }

        public Material(string renderPass, Shader shader)
        {
            this.BlendMode = BlendMode.Default;

            this.renderPass = renderPass;
            this.shader = shader;
        }

        internal void UpdateShaderVariables(Matrix44 world, Matrix44 view, Matrix44 proj)
        {
            if(shader == null)
                return;

            // Right now we need to make sure that the shader variables are all set with this
            // settings this material has defined.

            // We don't know if the shader being used is exclusive to this material, or if it
            // is shared between many.

            // Therefore to be 100% sure we could reset every variable on the shader to the defaults,
            // then set the ones that this material knows about, thus avoiding running with shader settings
            // that this material doesn't know about that are being changed by something else that
            // shares the shader.  This would be bad, as it will likely involve setting the same variable multiple times

            // So instead, as an optimisation, iterate over all settings that this material knows about,
            // and ask the shader to change them, this compare those changes against a full list of
            // all of the shader's variables, if any were missed by the material, then set them to
            // their default values.

            // Right now, just use the easy option and optimise later ;-D

            shader.ResetVariables();

            shader.SetVariable ("World", world);
            shader.SetVariable ("View", view);
            shader.SetVariable ("Projection", proj);

            foreach(var propertyName in colourSettings.Keys)
            {
                shader.SetVariable (propertyName, colourSettings[propertyName]);
            }

            foreach(var propertyName in floatSettings.Keys)
            {
                shader.SetVariable (propertyName, floatSettings[propertyName]);
            }

            foreach(var propertyName in matrixSettings.Keys)
            {
                shader.SetVariable (propertyName, matrixSettings[propertyName]);
            }

            foreach(var propertyName in vector3Settings.Keys)
            {
                shader.SetVariable (propertyName, vector3Settings[propertyName]);
            }

            foreach(var propertyName in vector4Settings.Keys)
            {
                shader.SetVariable (propertyName, vector4Settings[propertyName]);
            }

            foreach(var propertyName in scaleSettings.Keys)
            {
                shader.SetVariable (propertyName, scaleSettings[propertyName]);
            }

            foreach(var propertyName in textureOffsetSettings.Keys)
            {
                shader.SetVariable (propertyName, textureOffsetSettings[propertyName]);
            }

            int i = 0;
            foreach(var key in textureSamplerSettings.Keys)
            {
                shader.SetSamplerTarget (key, i);
                i++;
            }
        }

        internal Shader GetShader()
        {
            return shader;
        }

        public Vector2 Tiling
        {
            get { throw new NotImplementedException (); }
            set { throw new NotImplementedException (); }
        }

        public Vector2 Offset
        {
            get { throw new NotImplementedException (); }
            set { throw new NotImplementedException (); }
        }

        internal void UpdateGpuSettings(Graphics graphics)
        {
            // Update the render states on the gpu
            graphics.SetBlendEquation (this.BlendMode);

            // Set the active textures on the gpu
            int i = 0;
            foreach(var key in textureSamplerSettings.Keys)
            {
                graphics.SetActive (textureSamplerSettings [key], i++);
            }
        }

        Dictionary<string, Rgba32> colourSettings = new Dictionary<string, Rgba32>();
        Dictionary<string, Single> floatSettings = new Dictionary<string, Single>();
        Dictionary<string, Matrix44> matrixSettings = new Dictionary<string, Matrix44>();
        Dictionary<string, Vector3> vector3Settings = new Dictionary<string, Vector3>();
        Dictionary<string, Vector4> vector4Settings = new Dictionary<string, Vector4>();
        Dictionary<string, Vector2> scaleSettings = new Dictionary<string, Vector2>();
        Dictionary<string, Vector2> textureOffsetSettings = new Dictionary<string, Vector2>();
        Dictionary<string, Texture> textureSamplerSettings = new Dictionary<string, Texture>();

        public void SetColour(string propertyName, Rgba32 colour)
        {
            colourSettings[propertyName] = colour;
        }

        public void SetFloat(string propertyName, Single value)
        {
            floatSettings[propertyName] = value;
        }

        public void SetMatrix(string propertyName, Matrix44 matrix)
        {
            matrixSettings[propertyName] = matrix;
        }

        public void SetVector4(string propertyName, Vector4 vector)
        {
            vector4Settings[propertyName] = vector;
        }

        public void SetVector3(string propertyName, Vector3 vector)
        {
            vector3Settings[propertyName] = vector;
        }

        public void SetTextureOffset(string propertyName, Vector2 offset)
        {
            textureOffsetSettings[propertyName] = offset;
        }

        public void SetTextureScale(string propertyName, Vector2 scale)
        {
            scaleSettings[propertyName] = scale;
        }


        public void SetTexture(string propertyName, Texture texture)
        {
            textureSamplerSettings[propertyName] = texture;
        }
    }
}
