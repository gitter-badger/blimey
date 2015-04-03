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

namespace Blimey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Fudge;
    using Abacus.SinglePrecision;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class Material
        : IEquatable <Material>
    {
        readonly String renderPass;
        readonly Shader shader;

        readonly Dictionary<String, Rgba32> colourSettings = new Dictionary<String, Rgba32>();
        readonly Dictionary<String, Single> floatSettings = new Dictionary<String, Single>();
        readonly Dictionary<String, Matrix44> matrixSettings = new Dictionary<String, Matrix44>();
        readonly Dictionary<String, Vector4> vector4Settings = new Dictionary<String, Vector4>();
        readonly Dictionary<String, Vector3> vector3Settings = new Dictionary<String, Vector3>();
        readonly Dictionary<String, Vector2> vector2Settings = new Dictionary<String, Vector2>();
        readonly Dictionary<String, Vector2> scaleSettings = new Dictionary<String, Vector2>();
        readonly Dictionary<String, Vector2> texOffsetSettings = new Dictionary<String, Vector2>();
        readonly Dictionary<String, Texture> texSamplerSettings = new Dictionary<String, Texture>();

        public String RenderPass { get { return renderPass; } }
        public Shader Shader { get { return shader; } }

        public BlendMode BlendMode { get; set; }
        public Vector2 Tiling { get; set; }
        public Vector2 Offset { get; set; }

        public override int GetHashCode ()
        {
            return renderPass.GetHashCode ()
                ^ shader.GetHashCode ().ShiftAndWrap(1)
                ^ colourSettings.GetHashCode ().ShiftAndWrap(2)
                ^ floatSettings.GetHashCode ().ShiftAndWrap(3)
                ^ matrixSettings.GetHashCode ().ShiftAndWrap(4)
                ^ vector4Settings.GetHashCode ().ShiftAndWrap(5)
                ^ vector3Settings.GetHashCode ().ShiftAndWrap(6)
                ^ vector2Settings.GetHashCode ().ShiftAndWrap(7)
                ^ scaleSettings.GetHashCode ().ShiftAndWrap(8)
                ^ texOffsetSettings.GetHashCode ().ShiftAndWrap(9)
                ^ texSamplerSettings.GetHashCode ().ShiftAndWrap(10)
                ^ BlendMode.GetHashCode ().ShiftAndWrap(11)
                ^ Tiling.GetHashCode ().ShiftAndWrap(12)
                ^ Offset.GetHashCode ().ShiftAndWrap(13);
        }


        public Material(String renderPass, Shader shader)
        {
            this.renderPass = renderPass;
            this.shader = shader;

            BlendMode = BlendMode.Default;
            Tiling = Vector2.One;
            Offset = Vector2.Zero;
        }

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;
            if (obj is Material) flag = Equals ((Material) obj);
            return flag;
        }

        public Boolean Equals (Material other)
        {
            if (renderPass != other.renderPass)
                return false;

            if (shader != other.shader)
                return false;

            if (colourSettings.Count != other.colourSettings.Count) return false;
            if (floatSettings.Count != other.floatSettings.Count) return false;
            if (matrixSettings.Count != other.matrixSettings.Count) return false;
            if (vector4Settings.Count != other.vector4Settings.Count) return false;
            if (vector3Settings.Count != other.vector3Settings.Count) return false;
            if (vector2Settings.Count != other.vector2Settings.Count) return false;
            if (scaleSettings.Count != other.scaleSettings.Count) return false;
            if (texOffsetSettings.Count != other.texOffsetSettings.Count) return false;
            if (texSamplerSettings.Count != other.texSamplerSettings.Count) return false;

            foreach (var key in colourSettings.Keys) if (!other.colourSettings.ContainsKey (key)) return false;
            foreach (var key in floatSettings.Keys) if (!other.floatSettings.ContainsKey (key)) return false;
            foreach (var key in matrixSettings.Keys) if (!other.matrixSettings.ContainsKey (key)) return false;
            foreach (var key in vector4Settings.Keys) if (!other.vector4Settings.ContainsKey (key)) return false;
            foreach (var key in vector3Settings.Keys) if (!other.vector3Settings.ContainsKey (key)) return false;
            foreach (var key in vector2Settings.Keys) if (!other.vector2Settings.ContainsKey (key)) return false;
            foreach (var key in scaleSettings.Keys) if (!other.scaleSettings.ContainsKey (key)) return false;
            foreach (var key in texOffsetSettings.Keys) if (!other.texOffsetSettings.ContainsKey (key)) return false;
            foreach (var key in texSamplerSettings.Keys) if (!other.texSamplerSettings.ContainsKey (key)) return false;

            foreach (var key in colourSettings.Keys) if (colourSettings[key] != other.colourSettings[key]) return false;
            foreach (var key in floatSettings.Keys) if (Math.Abs (floatSettings [key] - other.floatSettings [key]) > 0.0001f) return false;
            foreach (var key in matrixSettings.Keys) if (matrixSettings[key] != other.matrixSettings[key]) return false;
            foreach (var key in vector4Settings.Keys) if (vector4Settings[key] != other.vector4Settings[key]) return false;
            foreach (var key in vector3Settings.Keys) if (vector3Settings[key] != other.vector3Settings[key]) return false;
            foreach (var key in vector2Settings.Keys) if (vector2Settings[key] != other.vector2Settings[key]) return false;
            foreach (var key in scaleSettings.Keys) if (scaleSettings[key] != other.scaleSettings[key]) return false;
            foreach (var key in texOffsetSettings.Keys) if (texOffsetSettings[key] != other.texOffsetSettings[key]) return false;
            foreach (var key in texSamplerSettings.Keys) if (texSamplerSettings[key] != other.texSamplerSettings[key]) return false;

            return true;
        }

        public static Boolean operator == (Material a, Material b) { return Equals (a, b); }
        public static Boolean operator != (Material a, Material b) { return !Equals (a, b); }

        public void SetColour (String id, Rgba32 colour) { colourSettings[id] = colour; }
        public void SetFloat (String id, Single value) { floatSettings[id] = value; }
        public void SetMatrix (String id, Matrix44 matrix) { matrixSettings[id] = matrix; }
        public void SetVector4 (String id, Vector4 vector) { vector4Settings[id] = vector; }
        public void SetVector3 (String id, Vector3 vector) { vector3Settings[id] = vector; }
        public void SetVector2 (String id, Vector2 vector) { vector2Settings[id] = vector; }
        public void SetTextureScale (String id, Vector2 scale) { scaleSettings[id] = scale; }
        public void SetTextureOffset (String id, Vector2 offset) { texOffsetSettings[id] = offset; }
        public void SetTexture (String id, Texture texture) { texSamplerSettings[id] = texture; }

        internal void UpdateShaderState ()
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

            // The solution is to add a SET BATCHED function to the shader so the shader itself can work out what needs
            // to change.

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

            foreach(var propertyName in vector4Settings.Keys)
            {
                shader.SetVariable (propertyName, vector4Settings[propertyName]);
            }

            foreach(var propertyName in vector3Settings.Keys)
            {
                shader.SetVariable (propertyName, vector3Settings[propertyName]);
            }

            foreach(var propertyName in vector2Settings.Keys)
            {
                shader.SetVariable (propertyName, vector2Settings[propertyName]);
            }

            foreach(var propertyName in scaleSettings.Keys)
            {
                shader.SetVariable (propertyName, scaleSettings[propertyName]);
            }

            foreach(var propertyName in texOffsetSettings.Keys)
            {
                shader.SetVariable (propertyName, texOffsetSettings[propertyName]);
            }

            int i = 0;
            foreach(var key in texSamplerSettings.Keys)
            {
                shader.SetSamplerTarget (key, i);
                i++;
            }
        }

        internal void UpdateRenderState (Graphics graphics)
        {
            // Update the render states on the gpu
            graphics.SetBlendEquation (this.BlendMode);

            // Set the active textures on the gpu
            int i = 0;
            foreach(var key in texSamplerSettings.Keys)
            {
                graphics.SetActive (texSamplerSettings [key], i++);
            }
        }
    }
}
