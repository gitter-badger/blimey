// ┌────────────────────────────────────────────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor - A Low Level, Cross Platform, 3D App Engine                                                               │ \\
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

namespace Cor
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Reflection;
    
    using Fudge;
    using Abacus.SinglePrecision;
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class VertexBufferHandle : GpuResourceHandle {}
    public class IndexBufferHandle : GpuResourceHandle {}
    public class TextureHandle : GpuResourceHandle {}
    public class ShaderHandle : GpuResourceHandle {}

    public class GpuResourceHandle : Handle {}
    public class Handle {}


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public interface IPlatform
    {
        Single                  sfx_GetVolume                           ();
        void                    sfx_SetVolume                           (Single volume);
		
		#if false

        void                    gfx_Reset                               ();
        void                    gfx_ClearColourBuffer                   (Rgba32 color);
        void                    gfx_ClearDepthBuffer                    (Single depth);
        void                    gfx_SetCullMode                         (CullMode cullMode);
        void                    gfx_SetBlendEquation                    (BlendFunction rgbBlendFunction, BlendFactor sourceRgb, BlendFactor destinationRgb, BlendFunction alphaBlendFunction, BlendFactor sourceAlpha, BlendFactor destinationAlpha);

        VertexBufferHandle      gfx_CreateVertexBuffer                  (VertexDeclaration vertexDeclaration, Int32 vertexCount);
        IndexBufferHandle       gfx_CreateIndexBuffer                   (Int32 indexCount);
        TextureHandle           gfx_CreateTexture                       (TextureDefinition definition, Byte[] source);
        ShaderHandle            gfx_CreateShader                        (ShaderDefinition definition, params Byte[] sources);

        void                    gfx_DestroyVertexBuffer                 (VertexBufferHandle handle);
        void                    gfx_DestroyIndexBuffer                  (IndexBufferHandle handle);
        void                    gfx_DestroyTexture                      (TextureHandle handle);
        void                    gfx_DestroyShader                       (ShaderHandle handle);

        void                    gfx_SetActiveVertexBuffer               (VertexBufferHandle handle);
        void                    gfx_SetActiveIndexBuffer                (IndexBufferHandle handle);

        void                    gfx_DrawPrimitives                      (PrimitiveType primitiveType, Int32 startVertex, Int32 primitiveCount);
        void                    gfx_DrawIndexedPrimitives               (PrimitiveType primitiveType, Int32 baseVertex, Int32 minVertexIndex,Int32 numVertices, Int32 startIndex, Int32 primitiveCount);
        void                    gfx_DrawUserPrimitives <T>              (PrimitiveType primitiveType, T[] vertexData, Int32 vertexOffset,Int32 primitiveCount, VertexDeclaration vertexDeclaration) where T: struct, IVertexType;
        void                    gfx_DrawUserIndexedPrimitives <T>       (PrimitiveType primitiveType, T[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount, VertexDeclaration vertexDeclaration) where T: struct, IVertexType;

        Byte[]                  gfx_CompileShader                       (String source);

        Int32                   gfx_dbg_BeginEvent                      (Rgba32 colour, String eventName);
        Int32                   gfx_dbg_EndEvent                        ();
        void                    gfx_dbg_SetMarker                       (Rgba32 colour, String marker);
        void                    gfx_dbg_SetRegion                       (Rgba32 colour, String region);

        Int32                   gfx_vbff_GetVertexCount                 (VertexBufferHandle h);
        VertexDeclaration       gfx_vbff_GetVertexDeclaration           (VertexBufferHandle h);
        void                    gfx_vbff_SetData<T>                     (VertexBufferHandle h, T[] data, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType;
        T[]                     gfx_vbff_GetData<T>                     (VertexBufferHandle h, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType;

        Int32                   gfx_ibff_GetIndexCount                  (IndexBufferHandle h);
        void                    gfx_ibff_SetData                        (IndexBufferHandle h, Int32[] data, Int32 startIndex, Int32 elementCount);
        void                    gfx_ibff_GetData                        (IndexBufferHandle h, Int32[] data, Int32 startIndex, Int32 elementCount);

        Int32                   gfx_tex_GetWidth                        (TextureHandle h);
        Int32                   gfx_tex_GetHeight                       (TextureHandle h);
        SurfaceFormat           gfx_tex_GetSurfaceFormat                (TextureHandle h);
        Byte[]                  gfx_tex_GetData                         (TextureHandle h);

        void                    gfx_shdr_ResetVariables                 (ShaderHandle handle);
        void                    gfx_shdr_ResetSamplers                  (ShaderHandle handle);
        void                    gfx_shdr_SetVariable<T>                 (ShaderHandle handle, String name, T value);
        void                    gfx_shdr_SetSampler                     (ShaderHandle handle, String name, TextureHandle textureHandle);
        void                    gfx_shdr_Activate                       (ShaderHandle handle, VertexDeclaration vertexDeclaration, String passName);

#endif
        
        Stream                  res_GetFileStream                       (String fileName);
        
#if false
        
        String                  sys_GetMachineIdentifier                ();
        String                  sys_GetOperatingSystemIdentifier        ();
        String                  sys_GetVirtualMachineIdentifier         ();

        Boolean?                app_IsFullscreen                        ();
        Int32                   app_GetWidth                            ();
        Int32                   app_GetHeight                           ();

        DeviceOrientation?      hid_GetCurrentOrientation               ();

        //IScreenSpecification    sys_GetScreenSpecification { get; }
        //IPanelSpecification     sys_GetPanelSpecification { get; }

        //IXbox360Gamepad         hid_Xbox360Gamepad { get; }
        //IPsmGamepad             hid_PsmGamepad { get; }
        //IMultiTouchController   hid_MultiTouchController { get; }
        //IGenericGamepad         hid_GenericGamepad { get; }
        //IMouse                  hid_Mouse { get; }
        //IKeyboard               hid_Keyboard { get; }

        //ButtonState Left { get; }
        //ButtonState Middle { get; }
        //ButtonState Right { get; }
        //Int32 ScrollWheelValue { get; }
        //Int32 X { get; }
        //Int32 Y { get; }

        //FunctionalKey[] GetPressedFunctionalKey ();
        //Boolean IsFunctionalKeyDown (FunctionalKey key);
        //Boolean IsFunctionalKeyUp (FunctionalKey key);
        //KeyState this [FunctionalKey key] { get; }
        //Char[] GetPressedCharacterKeys ();
        //Boolean IsCharacterKeyDown (Char key);
        //Boolean IsCharacterKeyUp (Char key);
        //KeyState this [Char key] { get; }
		
#endif
		
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
	
    /// <summary>
    /// 
    /// </summary>
    public interface IVertexType
    {
        /// <summary>
        /// 
        /// </summary>
        VertexDeclaration VertexDeclaration { get; }

        /// <summary>
        /// 
        /// </summary>
        /// IntPtr GetAddress (Int32 elementIndex);
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Defines how to create Cor.Xios's implementation
    /// of IShader.
    /// </summary>
    public sealed class ShaderDefinition
    {
        /// <summary>
        /// Defines a global name for this shader
        /// </summary>
        public String Name { get; set; }

        /// Defines which passes this shader is made from
        /// (ex: a toon shader is made for a cel-shading pass
        /// followed by an edge detection pass)
        /// </summary>
        public List<String> PassNames { get; set; }

        /// <summary>
        /// Lists all of the supported inputs into this shader and
        /// defines whether or not they are optional to an implementation.
        /// </summary>
        public List<ShaderInputDefinition> InputDefinitions { get; set; }

        /// <summary>
        /// Defines all of the variables supported by this shader.  Every
        /// variant must support all of the variables.
        /// </summary>
        public List<ShaderVariableDefinition> VariableDefinitions { get; set; }


        /// <summary>
        /// ?
        /// </summary>
        public List<ShaderSamplerDefinition> SamplerDefinitions { get; set; }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class ShaderInputDefinition
    {
        String niceName;
        Type defaultType;
        Object defaultValue;

        public ShaderInputDefinition ()
        {
            this.Name = String.Empty;
        }

        // Defines which Cor Types the DefaultValue can be set to.
        // The order of this list is important as the Cor Serialisation
        // of this class depends upon indexing into it.
        public static Type [] SupportedTypes
        { 
            get
            {
                return new [] 
                {
                    typeof (Matrix44),
                    typeof (Int32),
                    typeof (Single),
                    typeof (Abacus.SinglePrecision.Vector2),
                    typeof (Abacus.SinglePrecision.Vector3),
                    typeof (Abacus.SinglePrecision.Vector4),
                    typeof (Rgba32)
                };
            }
        }

        public String NiceName
        {
            get { return (niceName == null) ? Name : niceName; }
            set { niceName = value; }
        }
        
        public String Name { get; set; }

        public VertexElementUsage Usage { get; set; }

        public Type Type
        {
            get { return defaultType; }
        }

        public Object DefaultValue
        {
            get { return defaultValue; }
            set
            {
                Type t = value.GetType ();
                if (!SupportedTypes.ToList ().Contains (t))
                {
                    throw new Exception ();
                }

                defaultType = t;
                defaultValue = value;
            }
        }

        public Boolean Optional { get; set; }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class ShaderSamplerDefinition
    {
        String niceName;

        public ShaderSamplerDefinition ()
        {
            this.Name = String.Empty;
        }

        public String NiceName
        {
            get { return (niceName == null) ? Name : niceName; }
            set { niceName = value; }
        }
        
        public String Name { get; set; }
        public Boolean Optional { get; set; }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class ShaderVariableDefinition
    {
        String niceName;
        Type defaultType;
        Object defaultValue;

        public ShaderVariableDefinition ()
        {
            this.Name = String.Empty;
        }

        // Defines which Cor Types the DefaultValue can be set to.
        // The order of this list is important as the Cor Serialisation
        // of this class depends upon indexing into it.
        public static Type [] SupportedTypes
        { 
            get
            {
                return new [] 
                {
                    typeof (Matrix44),
                    typeof (Int32),
                    typeof (Single),
                    typeof (Abacus.SinglePrecision.Vector2),
                    typeof (Abacus.SinglePrecision.Vector3),
                    typeof (Abacus.SinglePrecision.Vector4),
                    typeof (Rgba32)
                };
            }
        }

        public String NiceName
        {
            get { return (niceName == null) ? Name : niceName; }
            set { niceName = value; }
        }
        
        public String Name { get; set; }

        public Type Type
        {
            get { return defaultType; }
        }
        
        public Object DefaultValue
        {
            get { return defaultValue; }
            set
            {
                Type t = value.GetType ();
                if (!SupportedTypes.ToList ().Contains (t))
                {
                    throw new Exception ();
                }

                defaultType = t;
                defaultValue = value;
            }
        }
    }
    
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class TextureDefinition
    {
        public SurfaceFormat SurfaceFormat { get; set; }
        public Int32 Width { get; set; }
        public Int32 Height { get; set; }
    }
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public class VertexDeclaration
    {
        /// <summary>
        /// 
        /// </summary>
        VertexElement[] _elements;
        
        /// <summary>
        /// 
        /// </summary>
        Int32 _vertexStride;

        /// <summary>
        /// 
        /// </summary>
        public VertexDeclaration (params VertexElement[] elements)
        {
            if ((elements == null) || (elements.Length == 0))
            {
                throw new ArgumentNullException ("elements - NullNotAllowed");
            }
            else
            {
                VertexElement[] elementArray = 
                    (VertexElement[]) elements.Clone ();

                this._elements = elementArray;

                Int32 vertexStride = 
                    VertexElementValidator.GetVertexStride (elementArray);

                this._vertexStride = vertexStride;

                VertexElementValidator.Validate (vertexStride, this._elements);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean Equals (VertexDeclaration other)
        {
            if (other == null)
                return false;

            return other == this;
        }

        /// <summary>
        /// 
        /// </summary>
        public override int GetHashCode ()
        {
            int hash = _vertexStride.GetHashCode ();

            foreach (var elm in _elements)
            {
                hash = hash.ShiftAndWrap() ^ elm.GetHashCode ();
            }

            return hash;
        }

        /// <summary>
        /// 
        /// </summary>
        public override Boolean Equals (object obj)
        {
            if (obj != null)
            {
                var other = obj as VertexDeclaration;

                if (other != null)
                {
                    return other == this;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public static Boolean operator != 
            (VertexDeclaration one, VertexDeclaration other)
        {
            return !(one == other);
        }

        /// <summary>
        /// 
        /// </summary>
        public static Boolean operator == 
            (VertexDeclaration one, VertexDeclaration other)
        {
            if ((object)one == null && (object)other == null)
            {
                return true;
            }

            if ((object)one == null || (object)other == null)
            {
                return false;
            }

            if (one._vertexStride != other._vertexStride)
                return false;

            for (int i = 0; i < one._elements.Length; ++i)
            {
                if (one._elements[i] != other._elements[i] )
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public override String ToString ()
        {
            string s = string.Empty;

            for (int i = 0; i < _elements.Length; ++i)
            {
                s += _elements[i]._usage;

                if (i + 1 < _elements.Length)
                {
                    s += ","; 
                }

            }

            return string.Format (
                "[VertexDeclaration: Elements=({0}), Stride={1}]", 
                s, 
                _vertexStride);
        }

        /// <summary>
        /// 
        /// </summary>
        public VertexDeclaration (Int32 vertexStride, params VertexElement[] elements)
        {
            if ((elements == null) || (elements.Length == 0))
            {
                throw new ArgumentNullException ("NullNotAllowed");
            }
            else
            {
                VertexElement[] elementArray = 
                    (VertexElement[])elements.Clone ();

                this._elements = elementArray;
                
                this._vertexStride = vertexStride;
                
                VertexElementValidator.Validate (vertexStride, elementArray);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        internal static VertexDeclaration FromType (Type vertexType)
        {
            if (vertexType == null)
            {
                throw new ArgumentNullException (
                    "vertexType - NullNotAllowed");
            }

#if !NETFX_CORE
            if (!vertexType.IsValueType)
            {
                throw new ArgumentException (
                    String.Format ("VertexTypeNotValueType"));
            }
#endif

            IVertexType type = 
                Activator.CreateInstance (vertexType) as IVertexType;

            if (type == null)
            {
                throw new ArgumentException (
                    String.Format ("VertexTypeNotIVertexType"));
            }

            VertexDeclaration vertexDeclaration = type.VertexDeclaration;

            if (vertexDeclaration == null)
            {
                throw new InvalidOperationException (
                    "VertexTypeNullDeclaration");
            }

            return vertexDeclaration;
        }

        /// <summary>
        /// 
        /// </summary>
        public VertexElement[] GetVertexElements ()
        {
            return (VertexElement[])this._elements.Clone ();
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 VertexStride { get { return this._vertexStride; } }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    [StructLayout (LayoutKind.Sequential)]
    public struct VertexElement
    {
        /// <summary>
        /// 
        /// </summary>
        internal Int32 _offset;

        /// <summary>
        /// 
        /// </summary>
        internal VertexElementFormat _format;

        /// <summary>
        /// 
        /// </summary>
        internal VertexElementUsage _usage;

        /// <summary>
        /// 
        /// </summary>
        internal Int32 _usageIndex;

        /// <summary>
        /// 
        /// </summary>
        public Int32 Offset
        {
            get { return this._offset; }
            set { this._offset = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public VertexElementFormat VertexElementFormat
        {
            get { return this._format; }
            set { this._format = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public VertexElementUsage VertexElementUsage
        {
            get{ return this._usage; }
            set { this._usage = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 UsageIndex
        {
            get { return this._usageIndex; }
            set { this._usageIndex = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public VertexElement (
            Int32 offset,
            VertexElementFormat elementFormat,
            VertexElementUsage elementUsage,
            Int32 usageIndex)
        {
            this._offset = offset;
            this._usageIndex = usageIndex;
            this._format = elementFormat;
            this._usage = elementUsage;
        }

        /// <summary>
        /// 
        /// </summary>
        public override String ToString ()
        {
            return String.Format (
                "[Offset:{0} Format:{1}, Usage:{2}, UsageIndex:{3}]",
                this.Offset,
                this.VertexElementFormat,
                this.VertexElementUsage,
                this.UsageIndex
            );
        }

        /// <summary>
        /// 
        /// </summary>
        public override Int32 GetHashCode ()
        {
            return base.GetHashCode ();
        }

        /// <summary>
        /// 
        /// </summary>
        public override Boolean Equals (Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType () != base.GetType ())
            {
                return false;
            }

            return (this == ((VertexElement)obj));
        }

        /// <summary>
        /// 
        /// </summary>
        public static Boolean operator ==
            (VertexElement left, VertexElement right)
        {
            return
                (left._offset == right._offset) &&
                (left._usageIndex == right._usageIndex) &&
                (left._usage == right._usage) &&
                (left._format == right._format);
        }

        /// <summary>
        /// 
        /// </summary>
        public static Boolean operator !=
            (VertexElement left, VertexElement right)
        {
            return !(left == right);
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// A touch in a single frame definition of a finger on the screen.
    /// </summary>
    public struct Touch
    {
        /// <summary>
        /// 
        /// </summary>
        Int32 id;

        /// <summary>
        /// The position of a touch ranges between -0.5 and 0.5 in both X and Y
        /// </summary>
        Vector2 normalisedEngineSpacePosition;

        /// <summary>
        /// 
        /// </summary>
        TouchPhase phase;

        /// <summary>
        /// 
        /// </summary>
        Int64 frameNumber;

        /// <summary>
        /// 
        /// </summary>
        Single timestamp;

        /// <summary>
        /// 
        /// </summary>
        static Touch invalidTouch;

        /// <summary>
        /// 
        /// </summary>
        public Int32 ID { get { return id; } }

        /// <summary>
        /// 
        /// </summary>
        public Vector2 Position
        {
            get { return normalisedEngineSpacePosition; }
        }

        /// <summary>
        /// 
        /// </summary>
        public TouchPhase Phase { get { return phase; } }

        /// <summary>
        /// 
        /// </summary>
        public Int64 FrameNumber { get { return frameNumber; } }

        /// <summary>
        /// 
        /// </summary>
        public Single Timestamp { get { return timestamp; } }

        /// <summary>
        /// 
        /// </summary>
        public Touch (
            Int32 id,
            Vector2 normalisedEngineSpacePosition,
            TouchPhase phase,
            Int64 frame,
            Single timestamp)
        {
            if (normalisedEngineSpacePosition.X > 0.5f || 
                normalisedEngineSpacePosition.X < -0.5f)
            {
                throw new Exception (
                    "Touch has a bad X coordinate: " + 
                    normalisedEngineSpacePosition.X);
            }

            if (normalisedEngineSpacePosition.Y > 0.5f || 
                normalisedEngineSpacePosition.X < -0.5f)
            {
                throw new Exception (
                    "Touch has a bad Y coordinate: " + 
                    normalisedEngineSpacePosition.Y);
            }

            this.id = id;
            this.normalisedEngineSpacePosition = normalisedEngineSpacePosition;
            this.phase = phase;
            this.frameNumber = frame;
            this.timestamp = timestamp;
        }

        /// <summary>
        /// 
        /// </summary>
        static Touch ()
        {
            invalidTouch = new Touch (
                -1, 
                Vector2.Zero, 
                TouchPhase.Invalid, 
                -1, 
                0f);
        }

        /// <summary>
        /// 
        /// </summary>
        public static Touch Invalid { get { return invalidTouch; } }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public sealed class TouchCollection
        : IEnumerable<Touch>
    {
        /// <summary>
        /// 
        /// </summary>
        List<Touch> touchBuffer = new List<Touch>();

        /// <summary>
        /// 
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator ();
        }

        /// <summary>
        /// 
        /// </summary>
        IEnumerator<Touch> IEnumerable<Touch>.GetEnumerator ()
        {
            return GetEnumerator ();
        }

        /// <summary>
        /// 
        /// </summary>
        internal void ClearBuffer ()
        {
            this.touchBuffer.Clear ();
        }

        /// <summary>
        /// 
        /// </summary>
        internal void RegisterTouch (
            Int32 id,
            Vector2 normalisedEngineSpacePosition,
            TouchPhase phase,
            Int64 frameNum,
            Single timestamp)
        {
            Boolean die = false;

            if (normalisedEngineSpacePosition.X > 0.5f ||
                normalisedEngineSpacePosition.X < -0.5f)
            {
                InternalUtils.Log.Info (
                    "Touch has a bad X coordinate: " +
                    normalisedEngineSpacePosition.X);

                die = true;
            }

            if (normalisedEngineSpacePosition.Y > 0.5f ||
                normalisedEngineSpacePosition.X < -0.5f)
            {
                InternalUtils.Log.Info (
                    "Touch has a bad Y coordinate: " +
                    normalisedEngineSpacePosition.Y);

                die = true;
            }

            if (die)
            {
                InternalUtils.Log.Info ("Discarding Bad Touch");
                return;
            }

            var touch = new Touch (
                id,
                normalisedEngineSpacePosition,
                phase,
                frameNum,
                timestamp);

            this.touchBuffer.Add (touch);
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerator<Touch> GetEnumerator ()
        {
            return new TouchCollectionEnumerator (this.touchBuffer);
        }

        /// <summary>
        /// 
        /// </summary>
        public int TouchCount
        {
            get
            {
                return touchBuffer.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Touch GetTouchFromTouchID (int zTouchID)
        {
            foreach (var touch in touchBuffer)
            {
                if (touch.ID == zTouchID) return touch;
            }

            //System.Diagnostics.Debug.WriteLine (
            //    "The touch requested no longer exists.");

            return Touch.Invalid;
        }
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    internal sealed class TouchCollectionEnumerator
        : IEnumerator<Touch>
    {
        /// <summary>
        /// 
        /// </summary>
        List<Touch> touches;

        /// <summary>
        /// Enumerators are positioned before the first element
        /// until the first MoveNext () call.
        /// </summary>
        Int32 position = -1;

        /// <summary>
        /// 
        /// </summary>
        internal TouchCollectionEnumerator (List<Touch> touches)
        {
            this.touches = touches;
        }

        /// <summary>
        /// 
        /// </summary>
        void IDisposable.Dispose ()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean MoveNext ()
        {
            position++;
            return (position < touches.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset ()
        {
            position = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        Object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Touch Current
        {
            get
            {
                try
                {
                    return touches[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException ();
                }
            }
        }
    }
}
