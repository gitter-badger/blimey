// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor! XNA 4 Platform Implementation                                     │ \\
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
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

using Abacus;
using Abacus.Packed;
using Abacus.SinglePrecision;
using Abacus.Int32Precision;

namespace Cor.Platform.Managed.Xna4
{
    public class BasicEffectShaderAdapter
        : IShader
    {
        public class ShaderPassWrapper
            : IShaderPass
        {
            public delegate void AdjustBasicEffectForVertDeclDelegate(VertexDeclaration vertexDeclaration);

            readonly AdjustBasicEffectForVertDeclDelegate ajustBasicEffectForVertDecl;
            readonly Action applySettings;

            public string Name { get { return this.xnaEffectPass.Name; } }

            readonly Microsoft.Xna.Framework.Graphics.EffectPass xnaEffectPass;

            internal ShaderPassWrapper(
                Microsoft.Xna.Framework.Graphics.EffectPass xnaEffectPass,
                AdjustBasicEffectForVertDeclDelegate ajustBasicEffectForVertDecl,
                Action applySettings)
            {
                this.xnaEffectPass = xnaEffectPass;
                this.ajustBasicEffectForVertDecl = ajustBasicEffectForVertDecl;
                this.applySettings = applySettings;
            }

            public void Activate(VertexDeclaration vertexDeclaration)
            {
                this.ajustBasicEffectForVertDecl(vertexDeclaration);
                this.applySettings();
                this.xnaEffectPass.Apply();
            }
        }

        public enum BasicEffectVertFormat
        {
            VertPos,
            VertPosTex,
            VertPosCol,
            VertPosTexCol,
            VertPosNorm,
            VertPosNormTex,
            VertPosNormCol,
            VertPosNormTexCol
        }

        readonly IShaderPass[] passArray;
        readonly ShaderType corShaderType;
        readonly String name;
        readonly Microsoft.Xna.Framework.Graphics.BasicEffect xnaBasicEffect;
        readonly VertexElementUsage[] requiredVertexElements;
        readonly VertexElementUsage[] optionalVertexElements;
        readonly Dictionary<VertexDeclaration, BasicEffectVertFormat> fastBasicEffectVertFormatLookup = new Dictionary<VertexDeclaration, BasicEffectVertFormat>();

        public BasicEffectShaderAdapter(
            Microsoft.Xna.Framework.Graphics.GraphicsDevice gfxDevice,
            ShaderType corShaderType
            )
        {
            this.corShaderType = corShaderType;
            this.xnaBasicEffect = new Microsoft.Xna.Framework.Graphics.BasicEffect(gfxDevice);
            this.xnaBasicEffect.DirectionalLight0.Enabled = true;
            this.xnaBasicEffect.DirectionalLight1.Enabled = true;
            this.xnaBasicEffect.DirectionalLight2.Enabled = true;

            switch (corShaderType)
            {
                case ShaderType.Unlit:
                    this.name = "Unlit";
                    this.requiredVertexElements = new VertexElementUsage[] { VertexElementUsage.Position };
                    this.optionalVertexElements = new VertexElementUsage[] { VertexElementUsage.TextureCoordinate, VertexElementUsage.Colour };
                    this.xnaBasicEffect.LightingEnabled = false;
                    this.xnaBasicEffect.PreferPerPixelLighting = false;
                    this.xnaBasicEffect.FogEnabled = false;
                    break;
                case ShaderType.VertexLit:
                    this.name = "VertexLit";
                    this.requiredVertexElements = new VertexElementUsage[] { VertexElementUsage.Position, VertexElementUsage.Normal };
                    this.optionalVertexElements = new VertexElementUsage[] { VertexElementUsage.TextureCoordinate, VertexElementUsage.Colour };
                    this.xnaBasicEffect.LightingEnabled = true;
                    this.xnaBasicEffect.PreferPerPixelLighting = false;
                    this.xnaBasicEffect.FogEnabled = true;
                    break;
                case ShaderType.PixelLit:
                    this.name = "PixelLit";
                    this.requiredVertexElements = new VertexElementUsage[] { VertexElementUsage.Position, VertexElementUsage.Normal };
                    this.optionalVertexElements = new VertexElementUsage[] { VertexElementUsage.TextureCoordinate, VertexElementUsage.Colour };
                    this.xnaBasicEffect.LightingEnabled = true;
                    this.xnaBasicEffect.PreferPerPixelLighting = true;
                    this.xnaBasicEffect.FogEnabled = true;
                    break;
                default: throw new Exception("Shader Type: " + corShaderType.ToString() + " not supported by BasicEffectShaderAdapter");
            }

            int numPasses = this.xnaBasicEffect.CurrentTechnique.Passes.Count;

            this.passArray = new IShaderPass[numPasses];

            for (int i = 0; i < numPasses; ++i)
            {
                this.passArray[i] = new ShaderPassWrapper(this.xnaBasicEffect.CurrentTechnique.Passes[i], this.AdjustBasicEffectForVertDecl, this.ApplySettings);
            }
        }

        void AdjustBasicEffectForVertDecl(VertexDeclaration vertexDeclaration)
        {
            if (!fastBasicEffectVertFormatLookup.ContainsKey(vertexDeclaration))
            {
                var elems = vertexDeclaration
               .GetVertexElements()
               .Select(x => x.VertexElementUsage)
               .ToList();

                bool col = elems.Contains(VertexElementUsage.Colour);
                bool tex = elems.Contains(VertexElementUsage.TextureCoordinate);
                bool norm = elems.Contains(VertexElementUsage.Normal);

                var mode = BasicEffectVertFormat.VertPos;

                if (norm)
                {
                    if (!col && !tex) mode = BasicEffectVertFormat.VertPosNorm;
                    else if (col && !tex) mode = BasicEffectVertFormat.VertPosNormCol;
                    else if (!col && tex) mode = BasicEffectVertFormat.VertPosNormTex;
                    else mode = BasicEffectVertFormat.VertPosNormTexCol;
                }
                else
                {
                    if (!col && !tex) mode = BasicEffectVertFormat.VertPos;
                    else if (col && !tex) mode = BasicEffectVertFormat.VertPosCol;
                    else if (!col && tex) mode = BasicEffectVertFormat.VertPosTex;
                    else mode = BasicEffectVertFormat.VertPosTexCol;
                }
                fastBasicEffectVertFormatLookup[vertexDeclaration] = mode;
            }

            var beMode = fastBasicEffectVertFormatLookup[vertexDeclaration];


            switch (beMode)
            {
                case BasicEffectVertFormat.VertPos:
                    this.xnaBasicEffect.TextureEnabled = false;
                    this.xnaBasicEffect.VertexColorEnabled = false;
                    this.xnaBasicEffect.LightingEnabled = false;
                    break;
                case BasicEffectVertFormat.VertPosTex:
                    this.xnaBasicEffect.TextureEnabled = true;
                    this.xnaBasicEffect.VertexColorEnabled = false;
                    this.xnaBasicEffect.LightingEnabled = false;
                    break;
                case BasicEffectVertFormat.VertPosCol:
                    this.xnaBasicEffect.TextureEnabled = false;
                    this.xnaBasicEffect.VertexColorEnabled = true;
                    this.xnaBasicEffect.LightingEnabled = false;
                    break;
                case BasicEffectVertFormat.VertPosTexCol:
                    this.xnaBasicEffect.TextureEnabled = true;
                    this.xnaBasicEffect.VertexColorEnabled = true;
                    this.xnaBasicEffect.LightingEnabled = false;
                    break;

                case BasicEffectVertFormat.VertPosNorm:
                    this.xnaBasicEffect.TextureEnabled = false;
                    this.xnaBasicEffect.VertexColorEnabled = false;
                    this.xnaBasicEffect.LightingEnabled = true;
                    break;
                case BasicEffectVertFormat.VertPosNormTex:
                    this.xnaBasicEffect.TextureEnabled = true;
                    this.xnaBasicEffect.VertexColorEnabled = false;
                    this.xnaBasicEffect.LightingEnabled = true;
                    break;
                case BasicEffectVertFormat.VertPosNormCol:
                    this.xnaBasicEffect.TextureEnabled = false;
                    this.xnaBasicEffect.VertexColorEnabled = true;
                    this.xnaBasicEffect.LightingEnabled = true;
                    break;
                case BasicEffectVertFormat.VertPosNormTexCol:
                    this.xnaBasicEffect.TextureEnabled = true;
                    this.xnaBasicEffect.VertexColorEnabled = true;
                    this.xnaBasicEffect.LightingEnabled = true;
                    break;
            }
        }

        HashSet<String> warningsLogged = new HashSet<string>();

        void ApplySettings()
        {
            foreach (var name in variableCache.Keys)
            {
                object obj = variableCache[name];

                if (name == "World")
                {
                    xnaBasicEffect.World = ((Matrix44)obj).ToXNA();
                }
                else if (name == "View")
                {
                    xnaBasicEffect.View = ((Matrix44)obj).ToXNA();
                }
                else if (name == "Projection")
                {
                    xnaBasicEffect.Projection = ((Matrix44)obj).ToXNA();
                }
                else if (name == "MaterialColour")
                {
                    var matCol = ((Rgba32)obj).ToXNA();
                    xnaBasicEffect.DiffuseColor = matCol.ToVector3();
                    xnaBasicEffect.Alpha = matCol.ToVector4().W;
                }
                else if (name == "AmbientLightColour")
                {
                    xnaBasicEffect.AmbientLightColor = ((Rgba32)obj).ToXNA().ToVector3();
                }
                else if (name == "EmissiveColour")
                {
                    xnaBasicEffect.EmissiveColor = ((Rgba32)obj).ToXNA().ToVector3();
                }
                else if (name == "SpecularColour")
                {
                    xnaBasicEffect.SpecularColor = ((Rgba32)obj).ToXNA().ToVector3();
                }
                else if (name == "SpecularPower")
                {
                    xnaBasicEffect.SpecularPower = (Single)obj;
                }
                else if (name == "FogEnabled")
                {
                    xnaBasicEffect.FogEnabled = (((Single)obj) == 1f) ? true : false;
                }
                else if (name == "FogStart")
                {
                    xnaBasicEffect.FogStart = (Single)obj;
                }
                else if (name == "FogEnd")
                {
                    xnaBasicEffect.FogEnd = (Single)obj;
                }
                else if (name == "FogColour")
                {
                    xnaBasicEffect.FogColor = ((Rgba32)obj).ToXNA().ToVector3();
                }
                else if (name == "DirectionalLight0Direction")
                {
                    xnaBasicEffect.DirectionalLight0.Direction = ((Vector3)obj).ToXNA();
                }
                else if (name == "DirectionalLight0DiffuseColour")
                {
                    xnaBasicEffect.DirectionalLight0.DiffuseColor = ((Rgba32)obj).ToXNA().ToVector3();
                }
                else if (name == "DirectionalLight0SpecularColour")
                {
                    xnaBasicEffect.DirectionalLight0.SpecularColor = ((Rgba32)obj).ToXNA().ToVector3();
                }
                else if (name == "DirectionalLight1Direction")
                {
                    xnaBasicEffect.DirectionalLight1.Direction = ((Vector3)obj).ToXNA();
                }
                else if (name == "DirectionalLight1DiffuseColour")
                {
                    xnaBasicEffect.DirectionalLight1.DiffuseColor = ((Rgba32)obj).ToXNA().ToVector3();
                }
                else if (name == "DirectionalLight1SpecularColour")
                {
                    xnaBasicEffect.DirectionalLight1.SpecularColor = ((Rgba32)obj).ToXNA().ToVector3();
                }
            
                else if (name == "DirectionalLight2Direction")
                {
                    xnaBasicEffect.DirectionalLight2.Direction = ((Vector3)obj).ToXNA();
                }
                else if (name == "DirectionalLight2DiffuseColour")
                {
                    xnaBasicEffect.DirectionalLight2.DiffuseColor = ((Rgba32)obj).ToXNA().ToVector3();
                }
                else if (name == "DirectionalLight2SpecularColour")
                {
                    xnaBasicEffect.DirectionalLight2.SpecularColor = ((Rgba32)obj).ToXNA().ToVector3();
                }
                else if (name == "EyePosition")
                {
                }
                else
                {
                    if (!warningsLogged.Contains(name) )
                    {
                        Console.WriteLine(string.Format("Shader {0}: Failed to set {1} to {2}", Name, name, obj));
                        warningsLogged.Add(name);
                    }
                }
            }
        }

        #region IShader


        Dictionary<string, object> variableCache = new Dictionary<string, object>();

        public void ResetVariables()
        {
            variableCache.Clear();
        }

        public void ResetSamplerTargets()
        {
            //Console.WriteLine("Not implemented: BasicEffectShaderAdapter.ResetSamplerTargets()");
        }

        public void SetVariable<T>(string name, T value)
        {
            variableCache[name] = value;
        }

        public void SetSamplerTarget(string name, Int32 textureSlot )
        {
        }

        public IShaderPass[] Passes { get { return this.passArray; } }

        public VertexElementUsage[] RequiredVertexElements { get { return this.requiredVertexElements; } }

        public VertexElementUsage[] OptionalVertexElements { get { return this.optionalVertexElements; } }

        public string Name { get { return this.name; } }

        #endregion
    }

    public static class ColourConverter
    {
        public static Microsoft.Xna.Framework.Color ToXNA(this Rgba32 colour)
        {
            return new Microsoft.Xna.Framework.Color(colour.R, colour.G, colour.B, colour.A);
        }

        public static Rgba32 ToBlimey(this Microsoft.Xna.Framework.Color color)
        {
            return new Rgba32(color.R, color.G, color.B, color.A);
        }
    }

    public static class Vector2Converter
    {
        // VECTOR 2
        public static Microsoft.Xna.Framework.Vector2 ToXNA(this Vector2 vec)
        {
            return new Microsoft.Xna.Framework.Vector2(vec.X, vec.Y);
        }

        public static Vector2 ToBlimey(this Microsoft.Xna.Framework.Vector2 vec)
        {
            return new Vector2(vec.X, vec.Y);
        }
    }

    public static class Vector3Converter
    {
        // VECTOR 3
        public static Microsoft.Xna.Framework.Vector3 ToXNA(this Vector3 vec)
        {
            return new Microsoft.Xna.Framework.Vector3(vec.X, vec.Y, vec.Z);
        }

        public static Vector3 ToBlimey(this Microsoft.Xna.Framework.Vector3 vec)
        {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }
    }

    public static class Vector4Converter
    {
        // VECTOR 4
        public static Microsoft.Xna.Framework.Vector4 ToXNA(this Vector4 vec)
        {
            return new Microsoft.Xna.Framework.Vector4(vec.X, vec.Y, vec.Z, vec.W);
        }

        public static Vector4 ToBlimey(this Microsoft.Xna.Framework.Vector4 vec)
        {
            return new Vector4(vec.X, vec.Y, vec.Z, vec.W);
        }
    }
    public static class MatrixConverter
    {
        // MATRIX
        public static Microsoft.Xna.Framework.Matrix ToXNA(this Matrix44 mat)
        {
            return new Microsoft.Xna.Framework.Matrix(
                mat.R0C0, mat.R0C1, mat.R0C2, mat.R0C3,
                mat.R1C0, mat.R1C1, mat.R1C2, mat.R1C3,
                mat.R2C0, mat.R2C1, mat.R2C2, mat.R2C3,
                mat.R3C0, mat.R3C1, mat.R3C2, mat.R3C3
                );
        }

        public static Matrix44 ToBlimey(this Microsoft.Xna.Framework.Matrix mat)
        {
            return new Matrix44(
                mat.M11, mat.M12, mat.M13, mat.M14,
                mat.M21, mat.M22, mat.M23, mat.M24,
                mat.M31, mat.M32, mat.M33, mat.M34,
                mat.M41, mat.M42, mat.M43, mat.M44
                );
        }

    }

    public static class EnumConverter
    {
        // PRIMITIVE TYPE
        public static Microsoft.Xna.Framework.Graphics.PrimitiveType ToXNA(PrimitiveType blimey)
        {
            switch (blimey)
            {
                case PrimitiveType.LineList: return Microsoft.Xna.Framework.Graphics.PrimitiveType.LineList;
                case PrimitiveType.LineStrip: return Microsoft.Xna.Framework.Graphics.PrimitiveType.LineStrip;
                case PrimitiveType.TriangleList: return Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList;
                case PrimitiveType.TriangleStrip: return Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip;

                default: throw new Exception("problem");
            }
        }

        public static TouchPhase ToBlimey(Microsoft.Xna.Framework.Input.Touch.TouchLocationState xna)
        {
            switch (xna)
            {
                case Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Invalid: return TouchPhase.Invalid;
                case Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Moved: return TouchPhase.Active;
                case Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Pressed: return TouchPhase.JustPressed;
                case Microsoft.Xna.Framework.Input.Touch.TouchLocationState.Released: return TouchPhase.JustReleased;

                default: throw new Exception("problem");
            }
        }

        public static PrimitiveType ToBlimey(Microsoft.Xna.Framework.Graphics.PrimitiveType xna)
        {
            switch (xna)
            {
                case Microsoft.Xna.Framework.Graphics.PrimitiveType.LineList: return PrimitiveType.LineList;
                case Microsoft.Xna.Framework.Graphics.PrimitiveType.LineStrip: return PrimitiveType.LineStrip;
                case Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleList: return PrimitiveType.TriangleList;
                case Microsoft.Xna.Framework.Graphics.PrimitiveType.TriangleStrip: return PrimitiveType.TriangleStrip;

                default: throw new Exception("problem");
            }
        }

        public static DeviceOrientation ToBlimey(Microsoft.Xna.Framework.DisplayOrientation xna)
        {
            switch (xna)
            {
                case Microsoft.Xna.Framework.DisplayOrientation.Default: return DeviceOrientation.Default;
                case Microsoft.Xna.Framework.DisplayOrientation.LandscapeRight: return DeviceOrientation.Rightside;
                case Microsoft.Xna.Framework.DisplayOrientation.Portrait: return DeviceOrientation.Upsidedown;
                case Microsoft.Xna.Framework.DisplayOrientation.LandscapeLeft: return DeviceOrientation.Leftside;

                default: throw new Exception("problem");
            }
        }

        // VERTEX ELEMENT FORMAT
        public static Microsoft.Xna.Framework.Graphics.VertexElementFormat ToXNA(VertexElementFormat blimey)
        {
            switch (blimey)
            {
                case VertexElementFormat.Byte4: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.Byte4;
                case VertexElementFormat.Colour: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.Color;
                case VertexElementFormat.HalfVector2: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.HalfVector2;
                case VertexElementFormat.HalfVector4: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.HalfVector4;
                //case VertexElementFormat.NormalizedShort2: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.NormalizedShort2;
                //case VertexElementFormat.NormalizedShort4: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.NormalizedShort4;
                case VertexElementFormat.Short2: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.Short2;
                case VertexElementFormat.Short4: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.Short4;
                case VertexElementFormat.Single: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.Single;
                case VertexElementFormat.Vector2: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector2;
                case VertexElementFormat.Vector3: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector3;
                case VertexElementFormat.Vector4: return Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector4;

                default: throw new Exception("problem");
            }
        }

        public static VertexElementFormat ToBlimey(Microsoft.Xna.Framework.Graphics.VertexElementFormat xna)
        {
            switch (xna)
            {
                case Microsoft.Xna.Framework.Graphics.VertexElementFormat.Byte4: return VertexElementFormat.Byte4;
                case Microsoft.Xna.Framework.Graphics.VertexElementFormat.Color: return VertexElementFormat.Colour;
                case Microsoft.Xna.Framework.Graphics.VertexElementFormat.HalfVector2: return VertexElementFormat.HalfVector2;
                case Microsoft.Xna.Framework.Graphics.VertexElementFormat.HalfVector4: return VertexElementFormat.HalfVector4;
                //case Microsoft.Xna.Framework.Graphics.VertexElementFormat.NormalizedShort2: return VertexElementFormat.NormalizedShort2;
                //case Microsoft.Xna.Framework.Graphics.VertexElementFormat.NormalizedShort4: return VertexElementFormat.NormalizedShort4;
                case Microsoft.Xna.Framework.Graphics.VertexElementFormat.Short2: return VertexElementFormat.Short2;
                case Microsoft.Xna.Framework.Graphics.VertexElementFormat.Short4: return VertexElementFormat.Short4;
                case Microsoft.Xna.Framework.Graphics.VertexElementFormat.Single: return VertexElementFormat.Single;
                case Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector2: return VertexElementFormat.Vector2;
                case Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector3: return VertexElementFormat.Vector3;
                case Microsoft.Xna.Framework.Graphics.VertexElementFormat.Vector4: return VertexElementFormat.Vector4;

                default: throw new Exception("problem");
            }
        }

        // VERTEX ELEMENT USAGE
        public static Microsoft.Xna.Framework.Graphics.VertexElementUsage ToXNA(VertexElementUsage blimey)
        {
            var val = (Int32)blimey;

            return (Microsoft.Xna.Framework.Graphics.VertexElementUsage)val;
        }

        public static VertexElementUsage ToBlimey(Microsoft.Xna.Framework.Graphics.VertexElementUsage xna)
        {
            var val = (Int32)xna;

            return (VertexElementUsage)val;
        }
    }


    public static class VertexDeclarationConverter
    {
        public static Microsoft.Xna.Framework.Graphics.VertexDeclaration ToXNA(this VertexDeclaration blimey)
        {
            Int32 blimeyStride = blimey.VertexStride;

            VertexElement[] blimeyElements = blimey.GetVertexElements();

            var xnaElements = new Microsoft.Xna.Framework.Graphics.VertexElement[blimeyElements.Length];

            for (Int32 i = 0; i < blimeyElements.Length; ++i)
            {
                VertexElement elem = blimeyElements[i];
                xnaElements[i] = elem.ToXNA();
            }

            var xnaVertDecl = new Microsoft.Xna.Framework.Graphics.VertexDeclaration(blimey.VertexStride, xnaElements);

            return xnaVertDecl;
        }
    }

    public static class VertexElementConverter
    {
        public static Microsoft.Xna.Framework.Graphics.VertexElement ToXNA(this VertexElement blimey)
        {
            Int32 bliOffset = blimey.Offset;
            var bliElementFormat = blimey.VertexElementFormat;
            var bliElementUsage = blimey.VertexElementUsage;
            Int32 bliUsageIndex = blimey.UsageIndex;


            var xnaVertElem = new Microsoft.Xna.Framework.Graphics.VertexElement(
                bliOffset,
                EnumConverter.ToXNA(bliElementFormat),
                EnumConverter.ToXNA(bliElementUsage),
                bliUsageIndex
                );

            return xnaVertElem;
        }
    }

    public class Engine
        : ICor
    {

        readonly GraphicsManager graphics;
        readonly ResourceManager resources;
        readonly SystemManager system;
        readonly InputManager input;
        readonly AppSettings settings;

        public Engine(
            Microsoft.Xna.Framework.GraphicsDeviceManager gfxManager,
            Microsoft.Xna.Framework.Content.ContentManager content,
            AppSettings settings,
            IApp startGame
            )
        {
            this.settings = settings;

            this.App = startGame;

            this.XnaGfxManager = gfxManager;

            this.graphics = new GraphicsManager(this, gfxManager);
            this.resources = new ResourceManager(this, gfxManager.GraphicsDevice, content);
            this.system = new SystemManager(this, gfxManager);
            this.input = new InputManager(this);

            this.App.Initilise(this);

        }

        public IGraphicsManager Graphics { get { return graphics;  } }

        public IOldResourceManager Resources { get { return resources; } }

        public IInputManager Input { get { return input; } }

        public ISystemManager System { get { return system; } }

        public AppSettings Settings { get { return settings; } }

        public IAudioManager Audio { get { return null; } }

        private IApp App { get; set; }

        private Microsoft.Xna.Framework.GraphicsDeviceManager XnaGfxManager { get; set; }

        public Boolean Update(AppTime time)
        {
            this.input.Update(time);
            var retVal = this.App.Update(time);

            return retVal;
        }

        public void Render()
        {
            this.App.Render();
        }

    }

    public class GeometryBuffer
        : IGeometryBuffer
    {
        VertexBufferWrapper _vertexBufWrap;
        IndexBufferWrapper _indexBufWrap;
        

        public GeometryBuffer(Microsoft.Xna.Framework.Graphics.GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, Int32 vertexCount, Int32 indexCount)
        {
            _vertexBufWrap = new VertexBufferWrapper(graphicsDevice, vertexDeclaration, vertexCount);
            _indexBufWrap = new IndexBufferWrapper(graphicsDevice, indexCount);
        }

        public IVertexBuffer VertexBuffer { get { return _vertexBufWrap; } }
        public IIndexBuffer IndexBuffer { get { return _indexBufWrap; } }
    }
#if TARGET_WINDOWS && DEBUG
    public class GpuUtils
        : IGpuUtils
    {

        [DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        static extern int D3DPERF_BeginEvent (uint col, String wszName);

        [DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi)]
        static extern int D3DPERF_EndEvent ();

        [DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        static extern int D3DPERF_SetMarker (uint col, String wszName);

        [DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
        static extern int D3DPERF_SetRegion (uint col, String wszName);

        [DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi)]
        static extern int D3DPERF_QueryRepeatFrame ();

        [DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi)]
        static extern void D3DPERF_SetOptions (uint dwOptions);

        [DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi)]
        static extern uint D3DPERF_GetStatus ();


        int _nestedBegins = 0;

        public int BeginEvent (Rgba colour, String eventName)
        {
            _nestedBegins++;

            return D3DPERF_BeginEvent (colour.PackedValue, eventName);
        }

        public int BeginEvent (String eventName)
        {
            return BeginEvent (Rgba.Black, eventName);
        }

        public int EndEvent ()
        {
            if (_nestedBegins == 0)
                throw new Exception ("BeginEvent must be called prior to a EndEvent call.");

            _nestedBegins--;

            return D3DPERF_EndEvent ();
        }

        public void SetMarker (Rgba colour, String eventName)
        {
            D3DPERF_SetMarker (colour.PackedValue, eventName);
        }

        public void SetRegion (Rgba colour, String eventName)
        {
            D3DPERF_SetRegion (colour.PackedValue, eventName);
        }

    }

#else

    public class GpuUtils
        : IGpuUtils
    {
        public Int32 BeginEvent(Rgba32 colour, String eventName) { return 0; }
        public Int32 EndEvent() { return 0; }

        public void SetMarker(Rgba32 colour, String eventName) { }
        public void SetRegion(Rgba32 colour, String eventName) { }
    }

#endif    internal class DisplayStatus
        : IDisplayStatus
    {
        Microsoft.Xna.Framework.GraphicsDeviceManager gfxManager;

        internal DisplayStatus(Microsoft.Xna.Framework.GraphicsDeviceManager gfxManager)
        {
            this.gfxManager = gfxManager;
        }

        public Boolean Fullscreen { get { return this.gfxManager.IsFullScreen; } }

        // What is the size of the frame buffer?
        // On most devices this will be the same as the screen size.
        // However on a PC or Mac the app could be running in windowed mode
        // and not take up the whole screen.
        public Int32 CurrentWidth { get { return this.gfxManager.GraphicsDevice.PresentationParameters.BackBufferWidth; } }
        public Int32 CurrentHeight { get { return this.gfxManager.GraphicsDevice.PresentationParameters.BackBufferHeight; } }
    }

    public class GraphicsManager
        : IGraphicsManager
    {
        Microsoft.Xna.Framework.GraphicsDeviceManager _xnaGfxDeviceManager;
        GpuUtils _gpuUtils;
        DisplayStatus displayStatus;

        public GraphicsManager(ICor engine, Microsoft.Xna.Framework.GraphicsDeviceManager gfxManager)
        {
            _xnaGfxDeviceManager = gfxManager;

            _xnaGfxDeviceManager.GraphicsDevice.RasterizerState = Microsoft.Xna.Framework.Graphics.RasterizerState.CullNone;
            _xnaGfxDeviceManager.GraphicsDevice.BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Opaque;
            _xnaGfxDeviceManager.GraphicsDevice.DepthStencilState = Microsoft.Xna.Framework.Graphics.DepthStencilState.Default;

            _gpuUtils = new GpuUtils();

            displayStatus = new DisplayStatus(_xnaGfxDeviceManager);
        }



        #region IGraphicsManager

        public IDisplayStatus DisplayStatus
        {
            get
            {
                return displayStatus;
            }
        }

        public IGpuUtils GpuUtils
        {
            get
            {
                return _gpuUtils;
            }
        }

        public void Reset()
        {

        }

        public void ClearColourBuffer(Rgba32 col = new Rgba32())
        {
            var xnaCol = col.ToXNA();

            _xnaGfxDeviceManager.GraphicsDevice.Clear(xnaCol);
        }

        public void ClearDepthBuffer(Single z = 1f)
        {

            _xnaGfxDeviceManager.GraphicsDevice.Clear(
                Microsoft.Xna.Framework.Graphics.ClearOptions.DepthBuffer,
                Microsoft.Xna.Framework.Vector4.Zero,
                z,
                0);
        }


        public void SetCullMode(CullMode cullMode)
        {

        }

        public IGeometryBuffer CreateGeometryBuffer(
            VertexDeclaration vertexDeclaration,
            Int32 vertexCount,
            Int32 indexCount)
        {
            return new GeometryBuffer(_xnaGfxDeviceManager.GraphicsDevice, vertexDeclaration, vertexCount, indexCount);
        }

        public void SetActiveGeometryBuffer(IGeometryBuffer buffer)
        {
            var vbuf = buffer.VertexBuffer as VertexBufferWrapper;

            _xnaGfxDeviceManager.GraphicsDevice.SetVertexBuffer(vbuf.XNAVertexBuffer);

            var ibuf = buffer.IndexBuffer as IndexBufferWrapper;

            _xnaGfxDeviceManager.GraphicsDevice.Indices = ibuf.XNAIndexBuffer;
        }

        public void SetActiveTexture(Int32 slot, Texture2D tex)
        {

        }


        public void SetBlendEquation(
            BlendFunction rgbBlendFunction, BlendFactor sourceRgb, BlendFactor destinationRgb,
            BlendFunction alphaBlendFunction, BlendFactor sourceAlpha, BlendFactor destinationAlpha
            )
        {

        }


        public void DrawPrimitives(
            PrimitiveType primitiveType,            // Describes the type of primitive to render.
            Int32 startVertex,                      // Index of the first vertex to load. Beginning at startVertex, the correct number of vertices is read out of the vertex buffer.
            Int32 primitiveCount)                  // Number of primitives to render. The primitiveCount is the number of primitives as determined by the primitive type. If it is a line list, each primitive has two vertices. If it is a triangle list, each primitive has three vertices.
        {
            throw new NotImplementedException();
        }

        public void DrawIndexedPrimitives(
            PrimitiveType primitiveType,            // Describes the type of primitive to render. PrimitiveType.PointList is not supported with this method.
            Int32 baseVertex,                       // . Offset to add to each vertex index in the index buffer.
            Int32 minVertexIndex,                   // . Minimum vertex index for vertices used during the call. The minVertexIndex parameter and all of the indices in the index stream are relative to the baseVertex parameter.
            Int32 numVertices,                      // Number of vertices used during the call. The first vertex is located at index: baseVertex + minVertexIndex.
            Int32 startIndex,                       // . Location in the index array at which to start reading vertices.
            Int32 primitiveCount                    // Number of primitives to render. The number of vertices used is a function of primitiveCount and primitiveType.
            )
        {
            var xnaPrimType = EnumConverter.ToXNA(primitiveType);
            _xnaGfxDeviceManager.GraphicsDevice.DrawIndexedPrimitives(xnaPrimType, 0, 0, numVertices, 0, primitiveCount);
        }

        public void DrawUserPrimitives<T>(
            PrimitiveType primitiveType,            // Describes the type of primitive to render.
            T[] vertexData,                         // The vertex data.
            Int32 vertexOffset,                     // Offset (in vertices) from the beginning of the buffer to start reading data.
            Int32 primitiveCount,                   // Number of primitives to render.
            VertexDeclaration vertexDeclaration)   // The vertex declaration, which defines per-vertex data.
            where T : struct, IVertexType
        {
            var xnaPrimType = EnumConverter.ToXNA(primitiveType);
            var xnaVertDecl = vertexDeclaration.ToXNA();

            _xnaGfxDeviceManager.GraphicsDevice.DrawUserPrimitives(
                xnaPrimType, vertexData, vertexOffset, primitiveCount, xnaVertDecl);
        }

        public void DrawUserIndexedPrimitives<T>(
            PrimitiveType primitiveType,            // Describes the type of primitive to render.
            T[] vertexData,                         // The vertex data.
            Int32 vertexOffset,                     // Offset (in vertices) from the beginning of the vertex buffer to the first vertex to draw.
            Int32 numVertices,                      // Number of vertices to draw.
            Int32[] indexData,                      // The index data.
            Int32 indexOffset,                      // Offset (in indices) from the beginning of the index buffer to the first index to use.
            Int32 primitiveCount,                   // Number of primitives to render.
            VertexDeclaration vertexDeclaration)
            where T : struct, IVertexType
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class IndexBufferWrapper
        : IIndexBuffer
    {
        Microsoft.Xna.Framework.Graphics.IndexBuffer _xnaIndexBuf;

        internal Microsoft.Xna.Framework.Graphics.IndexBuffer XNAIndexBuffer
        {
            get
            {
                return _xnaIndexBuf;
            }
        }

        public IndexBufferWrapper(Microsoft.Xna.Framework.Graphics.GraphicsDevice gfx, Int32 indexCount)
        {
            _xnaIndexBuf = new Microsoft.Xna.Framework.Graphics.IndexBuffer(
                gfx, 
                typeof(UInt16), 
                indexCount, 
                Microsoft.Xna.Framework.Graphics.BufferUsage.None
                );

        }

        public void GetData(Int32[] data)
        {
            throw new System.NotImplementedException();
            //_xnaIndexBuf.GetData<UInt16>(data);
        }

        /*
        public void GetData(UInt16[] data, int startIndex, int elementCount)
        {
            _xnaIndexBuf.GetData<UInt16>(data, startIndex, elementCount);
        }

        public void GetData(int offsetInBytes, UInt16[] data, int startIndex, int elementCount)
        {
            _xnaIndexBuf.GetData<UInt16>(offsetInBytes, data, startIndex, elementCount);
        }
         */

        public void SetData(Int32[] data)
        {
            UInt16[] udata = new UInt16[data.Length];

            for (Int32 i = 0; i < data.Length; ++i)
            {
                udata[i] = (UInt16)data[i];
            }

            _xnaIndexBuf.SetData<UInt16>(udata);
        }

        /*
        public void SetData(UInt16[] data, int startIndex, int elementCount)
        {
            _xnaIndexBuf.SetData<UInt16>(data, startIndex, elementCount);
        }

        public void SetData(int offsetInBytes, UInt16[] data, int startIndex, int elementCount)
        {
            _xnaIndexBuf.SetData(offsetInBytes, data, startIndex, elementCount);
        }
        */

        public int IndexCount 
        { 
            get
            {
                return _xnaIndexBuf.IndexCount;
            }
        }
    }

    public class InputManager
        : IInputManager
    {
        public MultiTouchController GetMultiTouchController() { return _touchScreen; }
        public PsmGamepad GetPsmGamepad() { return null; }
        public GenericGamepad GetGenericGamepad(){ return _genericPad; }
        
        TouchScreenImplementation _touchScreen;

        GenericGamepad _genericPad;
        Xbox360ControllerImplementation _pad1;
        Xbox360ControllerImplementation _pad2;
        Xbox360ControllerImplementation _pad3;
        Xbox360ControllerImplementation _pad4;



        public Xbox360Gamepad GetXbox360Gamepad(PlayerIndex player)
        {
            switch(player)
            {
                case PlayerIndex.One: return _pad1;
                case PlayerIndex.Two: return _pad2;
                case PlayerIndex.Three: return _pad3;
                case PlayerIndex.Four: return _pad4;
                default: throw new System.NotSupportedException();
            }
            
        }

        public InputManager(ICor engine)
        {

#if WINDOWS || XBOX

            _pad1 = new Xbox360ControllerImplementation(Microsoft.Xna.Framework.PlayerIndex.One);
            _pad2 = new Xbox360ControllerImplementation(Microsoft.Xna.Framework.PlayerIndex.Two);
            _pad3 = new Xbox360ControllerImplementation(Microsoft.Xna.Framework.PlayerIndex.Three);
            _pad4 = new Xbox360ControllerImplementation(Microsoft.Xna.Framework.PlayerIndex.Four);

            _genericPad = new GenericGamepad(this);

#endif


#if WP7
            _touchScreen = new TouchScreenImplementation(engine);
#endif

#if WINDOWS
            if (engine.Settings.MouseGeneratesTouches)
            {
                _touchScreen = new TouchScreenImplementation(engine);
            }
#endif
        }

        public void Update(AppTime time)
        {

#if WINDOWS || XBOX
            _pad1.Update(time);
            _pad2.Update(time);
            _pad3.Update(time);
            _pad4.Update(time);

            _genericPad.Update(time);
#endif

#if WINDOWS
            if (_touchScreen != null)
            {
                _touchScreen.Update(time);
            }
#endif

#if WP7
            _touchScreen.Update(time);
#endif
        }
    }

    public class ResourceManager
        : IOldResourceManager
    {
        Microsoft.Xna.Framework.Content.ContentManager _content;


        IShader _pixelLit;
        IShader _vertexLit;
        IShader _unlit;

        public ResourceManager(ICor engine, Microsoft.Xna.Framework.Graphics.GraphicsDevice gfxDevice, Microsoft.Xna.Framework.Content.ContentManager content)
        {
            _content = content;

            _pixelLit = new BasicEffectShaderAdapter(gfxDevice, ShaderType.PixelLit);
            _vertexLit = new BasicEffectShaderAdapter(gfxDevice, ShaderType.VertexLit);
            _unlit = new BasicEffectShaderAdapter(gfxDevice, ShaderType.Unlit);
        }

        public T Load<T>(String uri) where T
            : IOldResource
        {
            return default(T);
        }

        public IShader LoadShader(ShaderType shaderType)
        {
            switch(shaderType)
            {
                case ShaderType.VertexLit: return _vertexLit;
                case ShaderType.PixelLit: return _pixelLit;
                case ShaderType.Unlit: return _unlit;
                default: return null;
            }

        }
    }

    public class ScreenImplementation
        : IPanelSpecification
        , IScreenSpecification
    {
        Microsoft.Xna.Framework.Graphics.GraphicsDevice gfxDevice;
        Vector2 realWorldSize = Vector2.Zero;

        ICor engine;

        public ScreenImplementation(ICor engine, Microsoft.Xna.Framework.Graphics.GraphicsDevice gfxDevice)
        {
            this.engine = engine;
            this.gfxDevice = gfxDevice;

            this.EstimatePhysicalSize();
        }

        void EstimatePhysicalSize()
        {
#if WINDOWS
            realWorldSize = new Vector2(
                this.ScreenResolutionWidth, 
                this.ScreenResolutionHeight
                ) / 5000f;
#endif

#if WP7
            // do lookup here into all device types
            realWorldSize = new Vector2(0.048f, 0.08f);
#endif


#if XBOX
            //guess 
            realWorldSize = new Vector2(0.8f, 0.45f);
#endif
        }

        public float ScreenResolutionAspectRatio
        {
            get
            {
                return this.gfxDevice.Adapter.CurrentDisplayMode.AspectRatio;
            }
        }

        public Int32 ScreenResolutionHeight
        {
            get
            {
                return this.gfxDevice.Adapter.CurrentDisplayMode.Height;
            }
        }

        public Int32 ScreenResolutionWidth
        {
            get
            {
                return this.gfxDevice.Adapter.CurrentDisplayMode.Width;
            }
        }

        public Vector2 PanelPhysicalSize
        {
            get
            {
                return realWorldSize;
            }
        }

        public float PanelPhysicalAspectRatio
        {
            get
            {
                return realWorldSize.X / realWorldSize.Y;
            }
        }

        public PanelType PanelType
        { 
            get 
            {

#if TARGET_XBOX
                return PanelType.Screen; 
#elif TARGET_WINDOWS_PHONE
                return PanelType.TouchScreen;
#elif TARGET_WINDOWS || WINDOWS

                if (engine.Settings.MouseGeneratesTouches)
                {
                    return PanelType.TouchScreen;
                }
                else
                {
                    return PanelType.Screen; 
                }
#endif
            } 

        }
    }

    public class SystemManager
        : ISystemManager
    {
        ScreenImplementation mainDisplayPanel;
        

        internal ScreenImplementation MainDisplayPanel
        {
            get
            {
                return mainDisplayPanel;
            }
        }

        public void GetEffectiveDisplaySize(ref Int32 frameBufferWidth, ref Int32 frameBufferHeight)
        {
            if (this.CurrentOrientation == DeviceOrientation.Default ||
                this.CurrentOrientation == DeviceOrientation.Upsidedown)
            {
                return;
            }
            else
            {
                Int32 temp = frameBufferWidth;
                frameBufferWidth = frameBufferHeight;
                frameBufferHeight = frameBufferWidth;
            }

        }

        public SystemManager(ICor engine, Microsoft.Xna.Framework.GraphicsDeviceManager gfxManager)
        {
            
            mainDisplayPanel = new ScreenImplementation(engine, gfxManager.GraphicsDevice);
        }

        public Point2 CurrentDisplaySize
        {
            get
            {
                Int32 w = ScreenSpecification.ScreenResolutionWidth;
                Int32 h = ScreenSpecification.ScreenResolutionHeight;

                GetEffectiveDisplaySize(ref w, ref h);

                return new Point2(w, h);

            }
        }

        public String OperatingSystem { get { return System.Environment.OSVersion.Platform.ToString(); } }

        public String DeviceName { get { return string.Empty; } }

        public String DeviceModel { get { return string.Empty; } }

        public String SystemName { get { return string.Empty; } }

        public String SystemVersion { get { return string.Empty; } }


        internal void SetDeviceOrientation(DeviceOrientation orientation)
        {
            _orientation = orientation;
        }

        DeviceOrientation _orientation = DeviceOrientation.Default;
        public DeviceOrientation CurrentOrientation
        {
            get
            {
                return _orientation;
            }
            internal set
            {
                _orientation = value;
            }
        }

        public IScreenSpecification ScreenSpecification
        {
            get
            {
                return this.mainDisplayPanel;
            }
        }

        public IPanelSpecification PanelSpecification
        {
            get
            {
                return this.mainDisplayPanel;
            }
        }
    }

    public class TouchScreenImplementation
        : MultiTouchController
    {

#if WINDOWS

        Microsoft.Xna.Framework.Input.MouseState previousMouseState;
        bool doneFirstUpdateFlag = false;

#endif


        ScreenImplementation screen;

        internal TouchScreenImplementation(ICor engine)
            : base(engine)
        {

            this.screen = (engine.System as SystemManager).MainDisplayPanel;
        }

        public override IPanelSpecification PanelSpecification { get { return screen; } }

        internal override void Update(AppTime time)
        {

            this.collection.ClearBuffer();

#if WP7
            foreach (var xnaTouch in Microsoft.Xna.Framework.Input.Touch.TouchPanel.GetState())
            {
                Int32 id = xnaTouch.Id;
                Vector2 pos = xnaTouch.Position.ToBlimey();


                pos.X = pos.X / (Single) Microsoft.Xna.Framework.Input.Touch.TouchPanel.DisplayWidth;
                pos.Y = pos.Y / (Single) Microsoft.Xna.Framework.Input.Touch.TouchPanel.DisplayHeight;

                pos -= new Vector2(0.5f, 0.5f);

                var state = EnumConverter.ToBlimey(xnaTouch.State);

                this.touchCollection.RegisterTouch(id, pos, state, time.FrameNumber, time.Elapsed);
            }
#endif


#if WINDOWS

            var mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();

            if( doneFirstUpdateFlag )
            {

                bool pressedThisFrame = (mouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed);
                bool pressedLastFrame = (previousMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed);


                Int32 id = -42;
                Vector2 pos = new Vector2(mouseState.X, mouseState.Y);

                Int32 w = engine.Graphics.DisplayStatus.CurrentWidth;
                Int32 h = engine.Graphics.DisplayStatus.CurrentHeight;

                pos.X = pos.X / (Single)w;
                pos.Y = pos.Y / (Single)h;

                pos -= new Vector2(0.5f, 0.5f);

                pos.Y = -pos.Y;

                var state = TouchPhase.Invalid;
                
                if (pressedThisFrame && !pressedLastFrame)
                {
                    // new press
                    state = TouchPhase.JustPressed;
                }
                else if (pressedLastFrame && pressedThisFrame)
                {
                    // press in progress
                    state = TouchPhase.Active;
                }
                else if (pressedLastFrame && !pressedThisFrame)
                {
                    // released
                    state = TouchPhase.JustReleased;
                }

                if (state != TouchPhase.Invalid)
                {
                    this.collection.RegisterTouch(id, pos, state, time.FrameNumber, time.Elapsed);
                }


            }
            else
            {
                doneFirstUpdateFlag = true;
            }

            previousMouseState = mouseState;

#endif
        }
    }

    public class VertexBufferWrapper
        : IVertexBuffer
    {
        Microsoft.Xna.Framework.Graphics.VertexBuffer _xnaVertBuf;
        VertexDeclaration _vertexDeclaration;

        public VertexDeclaration VertexDeclaration 
        { 
            get
            {
                return _vertexDeclaration;
            }
        }

        internal Microsoft.Xna.Framework.Graphics.VertexBuffer XNAVertexBuffer
        {
            get
            {
                return _xnaVertBuf;
            }
        }

        public VertexBufferWrapper(Microsoft.Xna.Framework.Graphics.GraphicsDevice gfx, VertexDeclaration vertexDeclaration, int vertexCount)
        {
            Microsoft.Xna.Framework.Graphics.VertexDeclaration xnaVertDecl = vertexDeclaration.ToXNA();

            _vertexDeclaration = vertexDeclaration;
            _xnaVertBuf = new Microsoft.Xna.Framework.Graphics.VertexBuffer(
                gfx,
                xnaVertDecl, 
                vertexCount, 
                Microsoft.Xna.Framework.Graphics.BufferUsage.None
                );
        }

        public void GetData<T>(T[] data) where T : struct, IVertexType
        {
            _xnaVertBuf.GetData<T>(data);
        }

        public void GetData<T>(T[] data, int startIndex, int elementCount) where T : struct, IVertexType
        {
            _xnaVertBuf.GetData<T>(data, startIndex, elementCount);
        }

        public void GetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride) where T : struct, IVertexType
        {
            _xnaVertBuf.GetData<T>(offsetInBytes, data, startIndex, elementCount, vertexStride);
        }

        public void SetData<T>(T[] data) where T : struct, IVertexType
        {
            _xnaVertBuf.SetData<T>(data);
        }

        public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct, IVertexType
        {
            _xnaVertBuf.SetData<T>(data, startIndex, elementCount);
        }

        public void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride) where T : struct, IVertexType
        {
            _xnaVertBuf.SetData<T>(offsetInBytes, data, startIndex, elementCount, vertexStride);
        }


        public int VertexCount
        {
            get
            {
                return _xnaVertBuf.VertexCount;
            }
        }
    }

    public class Xbox360ControllerImplementation
        : Xbox360Gamepad
    {
        Microsoft.Xna.Framework.PlayerIndex _playerIndex;
        internal Xbox360ControllerImplementation(Microsoft.Xna.Framework.PlayerIndex playerIndex)
        {
            _playerIndex = playerIndex;
        }

        internal void Update(AppTime time)
        {
            base.Reset();

            var state = Microsoft.Xna.Framework.Input.GamePad.GetState(_playerIndex);
            var chatPad = Microsoft.Xna.Framework.Input.Keyboard.GetState(_playerIndex);

            if (state.DPad.Down == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.DPad.Down = ButtonState.Pressed;

            if (state.DPad.Up == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.DPad.Up = ButtonState.Pressed;

            if (state.DPad.Left == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.DPad.Left = ButtonState.Pressed;

            if (state.DPad.Right == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.DPad.Right = ButtonState.Pressed;

            if (state.Buttons.A == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.A = ButtonState.Pressed;

            if (state.Buttons.B == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.B = ButtonState.Pressed;

            if (state.Buttons.X == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.X = ButtonState.Pressed;

            if (state.Buttons.Y == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.Y = ButtonState.Pressed;

            if (state.Buttons.Start == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.Start = ButtonState.Pressed;

            if (state.Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.Back = ButtonState.Pressed;

            if (state.Buttons.RightShoulder == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.RightShoulder = ButtonState.Pressed;

            if (state.Buttons.LeftShoulder == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.LeftShoulder = ButtonState.Pressed;

            if (state.Buttons.RightStick == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.RightStick = ButtonState.Pressed;

            if (state.Buttons.LeftStick == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                base.Buttons.LeftStick = ButtonState.Pressed;

        }
    }

#if !WP7
    public class Xna4App
        : IDisposable // http://msdn.microsoft.com/en-us/library/system.idisposable.aspx
    {
        XnaGame game;

        // Track whether Dispose has been called. 
        private bool disposed = false;


        public Xna4App(AppSettings startSettings, IApp startGame)
        {
            game = new XnaGame(startSettings, startGame);
        }

        public void Run()
        {
            game.Run();
        }

        // Implement IDisposable. 
        // Do not make this method virtual. 
        // A derived class should not be able to override this method. 
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios. 
        // If disposing equals true, the method has been called directly 
        // or indirectly by a user's code. Managed and unmanaged resources 
        // can be disposed. 
        // If disposing equals false, the method has been called by the 
        // runtime from inside the finalizer and you should not reference 
        // other objects. Only unmanaged resources can be disposed. 
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    game.Dispose();
                }

                // Call the appropriate methods to clean up 
                // unmanaged resources here. 
                // If disposing is false, 
                // only the following code is executed.
                // ..

                // Note disposing has been done.
                disposed = true;

            }
        }
    }

    internal 
#else
    public
#endif
        
    class XnaGame
        : Microsoft.Xna.Framework.Game
    {
        Engine engine;
        Microsoft.Xna.Framework.GraphicsDeviceManager graphics;
        IApp startGame;
        Single elapsed;
        Int64 frameNumber = -1;
        AppSettings startSettings;

        public XnaGame(AppSettings startSettings, IApp startGame)
        {
            this.graphics = new Microsoft.Xna.Framework.GraphicsDeviceManager(this);
            this.startGame = startGame;

            this.startSettings = startSettings;
            this.Content.RootDirectory = "Content";
            this.IsFixedTimeStep = false;
            this.InactiveSleepTime = TimeSpan.FromSeconds(1);

            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            graphics.PreferredDepthStencilFormat = Microsoft.Xna.Framework.Graphics.DepthFormat.Depth24;
#if XBOX
            
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = 1920;
            graphics.PreferredBackBufferHeight = 1200;
            graphics.ApplyChanges();

#elif WINDOWS
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            graphics.ApplyChanges();

#elif WP7

            //graphics.SupportedOrientations = Microsoft.Xna.Framework.DisplayOrientation.Portrait;

            //graphics.PreferredBackBufferWidth = 480;
            //graphics.PreferredBackBufferHeight = 800;
            //graphics.ApplyChanges();
#endif
            engine = new Engine(graphics, Content, startSettings, startGame);
        }


        protected override void LoadContent()
        {

        }


        protected override void UnloadContent()
        {

        }

        protected override void Update(Microsoft.Xna.Framework.GameTime xnaGameTime)
        {

#if WP7
            (engine.SystemManager as SystemManager).SetDeviceOrientation(EnumConverter.ToBlimey(this.Window.CurrentOrientation));
#endif

            Single dt = (float)xnaGameTime.ElapsedGameTime.TotalSeconds;
            elapsed += dt;
            var appTime = new AppTime(dt, elapsed, ++frameNumber);
            engine.Update(appTime);

            base.Update(xnaGameTime);
        }


        protected override void Draw(Microsoft.Xna.Framework.GameTime xnaGameTime)
        {
            engine.Render();

            base.Draw(xnaGameTime);
        }
    }

}
