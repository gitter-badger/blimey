// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__          __    _____                                     │ \\
// │ \______   \  | _____ _/  |__/ ____\___________  _____                  │ \\
// │  |     ___/  | \__  \\   __\   __\/  _ \_  __ \/     \                 │ \\
// │  |    |   |  |__/ __ \|  |  |  | (  <_> )  | \/  Y Y  \                │ \\
// │  |____|   |____(____  /__|  |__|  \____/|__|  |__|_|  /                │ \\
// │                     \/                              \/                 │ \\
// │                                                                        │ \\
// │ A low level, cross platform API for building graphical apps.           │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey3D (http://www.blimey3d.com)           │ \\
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

namespace Platform
{
    using System;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    
    using Fudge;
    using Abacus.SinglePrecision;
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public interface IPlatform
    {
        IProgram Program { get; }
        IApi Api { get; }
    }
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    
    public interface IProgram
    {
        void Start (IApi platformImplementation, Action update, Action render);
        void Stop ();
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public interface IApi
    {
        /*
         * Audio
         */
        Single                  sfx_GetVolume                           ();
        void                    sfx_SetVolume                           (Single volume);

        /*
         * Graphics
         */
        void                    gfx_ClearColourBuffer                   (Rgba32 color);
        void                    gfx_ClearDepthBuffer                    (Single depth);
        void                    gfx_SetCullMode                         (CullMode cullMode);
        void                    gfx_SetBlendEquation                    (BlendFunction rgbBlendFunction, BlendFactor sourceRgb, BlendFactor destinationRgb, BlendFunction alphaBlendFunction, BlendFactor sourceAlpha, BlendFactor destinationAlpha);
    
        Handle                  gfx_CreateVertexBuffer                  (VertexDeclaration vertexDeclaration, Int32 vertexCount);
        Handle                  gfx_CreateIndexBuffer                   (Int32 indexCount);
        Handle                  gfx_CreateTexture                       (TextureFormat textureFormat, Int32 width, Int32 height, Byte[] source);
        Handle                  gfx_CreateShader                        (ShaderDeclaration shaderDeclaration, ShaderFormat shaderFormat, Byte[] source);
        
        void                    gfx_DestroyVertexBuffer                 (Handle vertexBufferHandle);
        void                    gfx_DestroyIndexBuffer                  (Handle indexBufferHandle);
        void                    gfx_DestroyTexture                      (Handle textureHandle);
        void                    gfx_DestroyShader                       (Handle shaderHandle);

        void                    gfx_DrawPrimitives                      (PrimitiveType primitiveType, Int32 startVertex, Int32 primitiveCount);
        void                    gfx_DrawIndexedPrimitives               (PrimitiveType primitiveType, Int32 baseVertex, Int32 minVertexIndex,Int32 numVertices, Int32 startIndex, Int32 primitiveCount);
        void                    gfx_DrawUserPrimitives <T>              (PrimitiveType primitiveType, T[] vertexData, Int32 vertexOffset,Int32 primitiveCount) where T: struct, IVertexType;
        void                    gfx_DrawUserIndexedPrimitives <T>       (PrimitiveType primitiveType, T[] vertexData, Int32 vertexOffset, Int32 numVertices, Int32[] indexData, Int32 indexOffset, Int32 primitiveCount) where T: struct, IVertexType;

        Byte[]                  gfx_CompileShader                       (String source);

        Int32                   gfx_vbff_GetVertexCount                 (Handle vertexBufferHandle);
        VertexDeclaration       gfx_vbff_GetVertexDeclaration           (Handle vertexBufferHandle);
        void                    gfx_vbff_SetData<T>                     (Handle vertexBufferHandle, T[] data, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType;
        T[]                     gfx_vbff_GetData<T>                     (Handle vertexBufferHandle, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType;

        /// <summary>
        /// This function looks at the vertex elements in the given vertex buffer and activate an array of generic
        /// vertex attribute data that correspond to the given indices.  If null, all will be activated.
        /// </summary>
        void
        gfx_vbff_Bind
        (VertexDeclaration vertexDeclaration, Int32[] vertexElementIndices);

        /// <summary>
        // Makes the vertex buffer associated with the given handle active on the GPU.
        // Activate does not have to be called if you only want to call the other gfx_vbff_* functions to change 
        // or get something about the vertex buffer.
        /// </summary>
        void
        gfx_vbff_Activate
        (Handle vertexBufferHandle);

        Int32                   gfx_ibff_GetIndexCount                  (Handle indexBufferHandle);
        void                    gfx_ibff_SetData                        (Handle indexBufferHandle, Int32[] data, Int32 startIndex, Int32 elementCount);
        Int32[]                 gfx_ibff_GetData                        (Handle indexBufferHandle, Int32 startIndex, Int32 elementCount);
        void                    gfx_ibff_Activate                       (Handle indexBufferHandle);

        Int32                   gfx_tex_GetWidth                        (Handle textureHandle);
        Int32                   gfx_tex_GetHeight                       (Handle textureHandle);
        TextureFormat           gfx_tex_GetTextureFormat                (Handle textureHandle);
        Byte[]                  gfx_tex_GetData                         (Handle textureHandle);
        void                    gfx_tex_Activate                        (Handle textureHandle, Int32 slot);

        void                    gfx_shdr_SetVariable<T>                 (Handle shaderHandle, Int32 variantIndex, String name, T value);
        void                    gfx_shdr_SetSampler                     (Handle shaderHandle, Int32 variantIndex, String name, Int32 slot);
        Int32                   gfx_shdr_GetVariantCount                (Handle shaderHandle);
        String                  gfx_shdr_GetIdentifier                  (Handle shaderHandle, Int32 variantIndex);
        ShaderInputInfo[]       gfx_shdr_GetInputs                      (Handle shaderHandle, Int32 variantIndex);
        ShaderVariableInfo[]    gfx_shdr_GetVariables                   (Handle shaderHandle, Int32 variantIndex);
        ShaderSamplerInfo[]     gfx_shdr_GetSamplers                    (Handle shaderHandle, Int32 variantIndex);
        void                    gfx_shdr_Activate                       (Handle shaderHandle, Int32 variantIndex);

        /*
         * Resources
         */
        Stream                  res_GetFileStream                       (String fileName);

        /*
         * System
         */
        String                  sys_GetMachineIdentifier                ();
        String                  sys_GetOperatingSystemIdentifier        ();
        String                  sys_GetVirtualMachineIdentifier         ();
        //                      todo, multiple screen support
        Int32                   sys_GetPrimaryScreenResolutionWidth     ();
        Int32                   sys_GetPrimaryScreenResolutionHeight    ();
        Vector2?                sys_GetPrimaryPanelPhysicalSize         (); // In meters.
        PanelType               sys_GetPrimaryPanelType                 ();

        /*
         * Application
         */
        Boolean?                app_IsFullscreen                        ();
        Int32                   app_GetWidth                            ();
        Int32                   app_GetHeight                           ();

        /*
         * Input
         */
        DeviceOrientation?                                  hid_GetCurrentOrientation                   ();
        Dictionary <DigitalControlIdentifier, Int32>        hid_GetDigitalControlStates                 ();
        Dictionary <AnalogControlIdentifier, Single>        hid_GetAnalogControlStates                  ();
        HashSet <BinaryControlIdentifier>                   hid_GetBinaryControlStates                  ();
        HashSet <Char>                                      hid_GetPressedCharacters                    ();
        HashSet <RawTouch>                                  hid_GetActiveTouches                        ();
    }
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public abstract class Handle
        : IDisposable
        , IEquatable <Handle>
    {
        readonly String guid;
        Boolean alreadyDisposed;
        
        protected Handle ()
        {
            var g = Guid.NewGuid();
            guid = g.ToString ();
        }

        public override String ToString ()
        {
            return guid;
        }
        
        public String Identifier { get { return guid; } }
        
        protected virtual void CleanUpManagedResources () {}
        protected virtual void CleanUpNativeResources () {}

        public void Dispose ()
        {
            RunDispose (true);
            GC.SuppressFinalize (this);
        }

        public void RunDispose (bool isDisposing)
        {
            if (alreadyDisposed) return;
            if (isDisposing) CleanUpNativeResources ();

            // FREE UNMANAGED STUFF HERE
            CleanUpManagedResources ();

            alreadyDisposed = true;
        }

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;
            if (obj is Handle) flag = this.Equals ((Handle) obj);
            return flag;
        }

        public Boolean Equals (Handle other)
        {
            if (this.guid != other.guid)
                return false;

            return true;
        }

        public static Boolean operator == (Handle a, Handle b) { return Equals (a, b); }
        public static Boolean operator != (Handle a, Handle b) { return !Equals (a, b); }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
  
    public enum DeviceOrientation
    {
        /// <summary>
        /// 
        /// </summary>
        Default,
        
        /// <summary>
        /// 
        /// </summary>
        Rightside,
        
        /// <summary>
        /// 
        /// </summary>
        Upsidedown,
        
        /// <summary>
        /// 
        /// </summary>
        Leftside,
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public enum PanelType
    {
        /// <summary>
        /// 
        /// </summary>
        Screen,
        
        /// <summary>
        /// 
        /// </summary>
        Touch,
        
        /// <summary>
        /// 
        /// </summary>
        TouchScreen,
    }
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public enum PrimitiveType
    {
        /// <summary>
        /// 
        /// </summary>
        TriangleList = 0,
        
        /// <summary>
        /// 
        /// </summary>
        TriangleStrip = 1,
        
        /// <summary>
        /// 
        /// </summary>
        LineList = 2,
        
        /// <summary>
        /// 
        /// </summary>
        LineStrip = 3
    }
    
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    
    public enum BinaryControlIdentifier
    {
        Xbox360_0_DPad_Up,
        Xbox360_0_DPad_Down,
        Xbox360_0_DPad_Left,
        Xbox360_0_DPad_Right,
        Xbox360_0_A,
        Xbox360_0_B,
        Xbox360_0_X,
        Xbox360_0_Y,
        Xbox360_0_LeftSholder,
        Xbox360_0_RightSholder,
        Xbox360_0_LeftThumbstick,
        Xbox360_0_RightThumbstick,
        Xbox360_0_Start,
        Xbox360_0_Back,
        
        Xbox360_1_DPad_Up,
        Xbox360_1_DPad_Down,
        Xbox360_1_DPad_Left,
        Xbox360_1_DPad_Right,
        Xbox360_1_A,
        Xbox360_1_B,
        Xbox360_1_X,
        Xbox360_1_Y,
        Xbox360_1_LeftSholder,
        Xbox360_1_RightSholder,
        Xbox360_1_LeftThumbstick,
        Xbox360_1_RightThumbstick,
        Xbox360_1_Start,
        Xbox360_1_Back,
        
        Xbox360_2_DPad_Up,
        Xbox360_2_DPad_Down,
        Xbox360_2_DPad_Left,
        Xbox360_2_DPad_Right,
        Xbox360_2_A,
        Xbox360_2_B,
        Xbox360_2_X,
        Xbox360_2_Y,
        Xbox360_2_LeftSholder,
        Xbox360_2_RightSholder,
        Xbox360_2_LeftThumbstick,
        Xbox360_2_RightThumbstick,
        Xbox360_2_Start,
        Xbox360_2_Back,
        
        Xbox360_3_DPad_Up,
        Xbox360_3_DPad_Down,
        Xbox360_3_DPad_Left,
        Xbox360_3_DPad_Right,
        Xbox360_3_A,
        Xbox360_3_B,
        Xbox360_3_X,
        Xbox360_3_Y,
        Xbox360_3_LeftSholder,
        Xbox360_3_RightSholder,
        Xbox360_3_LeftThumbstick,
        Xbox360_3_RightThumbstick,
        Xbox360_3_Start,
        Xbox360_3_Back,
        
        PlayStationMobile_DPad_Up,
        PlayStationMobile_DPad_Down,
        PlayStationMobile_DPad_Left,
        PlayStationMobile_DPad_Right,
        PlayStationMobile_Cross,
        PlayStationMobile_Circle,
        PlayStationMobile_Triangle,
        PlayStationMobile_Square,
        PlayStationMobile_Start,
        PlayStationMobile_Select,
        PlayStationMobile_LeftSholder,
        PlayStationMobile_RightSholder,
        
        Mouse_Left,
        Mouse_Middle,
        Mouse_Right,
        
        Keyboard_Backspace,
        Keyboard_Tab,
        Keyboard_Enter,
        Keyboard_CapsLock,
        Keyboard_Escape,
        Keyboard_Spacebar,
        Keyboard_PageUp,
        Keyboard_PageDown,  
        Keyboard_End,
        Keyboard_Home,
        Keyboard_Left,
        Keyboard_Up,
        Keyboard_Right,
        Keyboard_Down,
        Keyboard_Select,
        Keyboard_Print,
        Keyboard_Execute,
        Keyboard_PrintScreen,
        Keyboard_Insert,
        Keyboard_Delete,
        Keyboard_Help,
        Keyboard_LeftWindows,
        Keyboard_RightWindows,
        Keyboard_LeftFlower,
        Keyboard_RightFlower,
        Keyboard_Apps,
        Keyboard_Sleep,
        Keyboard_NumPad0,
        Keyboard_NumPad1,
        Keyboard_NumPad2,
        Keyboard_NumPad3,
        Keyboard_NumPad4,
        Keyboard_NumPad5,
        Keyboard_NumPad6,
        Keyboard_NumPad7,
        Keyboard_NumPad8,
        Keyboard_NumPad9,
        Keyboard_Multiply,
        Keyboard_Add,
        Keyboard_Separator,
        Keyboard_Subtract,
        Keyboard_Decimal,
        Keyboard_Divide,
        Keyboard_F1,
        Keyboard_F2,
        Keyboard_F3,
        Keyboard_F4,
        Keyboard_F5,
        Keyboard_F6,
        Keyboard_F7,
        Keyboard_F8,
        Keyboard_F9,
        Keyboard_F10,
        Keyboard_F11,
        Keyboard_F12,
        Keyboard_F13,
        Keyboard_F14,
        Keyboard_F15,
        Keyboard_F16,
        Keyboard_F17,
        Keyboard_F18,
        Keyboard_F19,
        Keyboard_F20,
        Keyboard_F21,
        Keyboard_F22,
        Keyboard_F23,
        Keyboard_F24,
        Keyboard_NumLock,
        Keyboard_ScrollLock,
        Keyboard_LeftShift,
        Keyboard_RightShift,
        Keyboard_LeftControl,
        Keyboard_RightControl,
        Keyboard_LeftAlt,
        Keyboard_RightAlt,
        
        
    }
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    
    public enum DigitalControlIdentifier
    {
        Mouse_Wheel,
        Mouse_X,
        Mouse_Y,
    }
    
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    
    public enum AnalogControlIdentifier
    {
        Xbox360_0_Leftstick_X,
        Xbox360_0_Leftstick_Y,
        Xbox360_0_Rightstick_X,
        Xbox360_0_Rightstick_Y,
        Xbox360_0_LeftTrigger,
        Xbox360_0_RightTrigger,
        
        Xbox360_1_Leftstick_X,
        Xbox360_1_Leftstick_Y,
        Xbox360_1_Rightstick_X,
        Xbox360_1_Rightstick_Y,
        Xbox360_1_LeftTrigger,
        Xbox360_1_RightTrigger,
        
        Xbox360_2_Leftstick_X,
        Xbox360_2_Leftstick_Y,
        Xbox360_2_Rightstick_X,
        Xbox360_2_Rightstick_Y,
        Xbox360_2_LeftTrigger,
        Xbox360_2_RightTrigger,
        
        Xbox360_3_Leftstick_X,
        Xbox360_3_Leftstick_Y,
        Xbox360_3_Rightstick_X,
        Xbox360_3_Rightstick_Y,
        Xbox360_3_LeftTrigger,
        Xbox360_3_RightTrigger,
        
        PlayStationMobile_Leftstick_X,
        PlayStationMobile_Leftstick_Y,
        PlayStationMobile_Rightstick_X,
        PlayStationMobile_Rightstick_Y,
    }
    
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public enum CullMode
    {
        /// <summary>
        /// 
        /// </summary>
        None,
        
        /// <summary>
        /// 
        /// </summary>
        CW,
        
        /// <summary>
        /// 
        /// </summary>
        CCW,
    }
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public enum TextureFormat
    {
        /// <summary>
        /// 
        /// </summary>
        Alpha8,
        Bgr_5_6_5,
        Bgra16,
        Bgra_5_5_5_1,
        NormalisedByte2,
        NormalisedByte4,
        NormalisedShort2,
        NormalisedShort4,
        Rg32,
        Rgba32,
        Rgba64,
        Rgba_10_10_10_2,
        Short2,
        Short4,

        // Compressed formats

        Dxt1,
        Dxt1a,
        Dxt3,
        Dxt5,
        RgbPvrtc2Bpp,
        RgbPvrtc4Bpp,
        RgbaPvrtc2Bpp,
        RgbaPvrtc4Bpp,

        // Some extras from MonoGame for future reference

        // BGRA formats are required for compatibility with WPF D3DImage.
        Bgr32,     // B8G8R8X8
        Bgra32,    // B8G8R8A8

        // Good explanation of compressed formats for mobile devices (aimed at Android, but describes PVRTC)
        // http://developer.motorola.com/docstools/library/understanding-texture-compression/

        // Ericcson Texture Compression (Android)
        RgbEtc1,
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Defines colour blending factors.
    ///               ogles2.0
    /// 
    /// factor        | s | d |
    /// ------------------------------------
    /// zero          | o | o |
    /// one           | o | o |
    /// src-col       | o | o |
    /// inv-src-col   | o | o |
    /// src-a         | o | o |
    /// inv-src-a     | o | o |
    /// dest-a        | o | o |
    /// inv-dest-a    | o | o |
    /// dest-col      | o | o |
    /// inv-dest-col  | o | o |
    /// src-a-sat     | o | x |  -- ignore for now
    ///
    /// xna deals with the following as one colour...
    ///
    /// const-col     | o | o |  -- ignore for now
    /// inv-const-col | o | o |  -- ignore for now
    /// const-a       | o | o |  -- ignore for now
    /// inv-const-a   | o | o |  -- ignore for now
    /// </summary>
    public enum BlendFactor
    {
        /// <summary>
        /// Each component of the colour is multiplied by (0, 0, 0, 0).
        /// </summary>
        Zero,

        /// <summary>
        /// Each component of the colour is multiplied by (1, 1, 1, 1).
        /// </summary>
        One,

        /// <summary>
        /// Each component of the colour is multiplied by the source colour.  This can be represented as
        /// (Rs, Gs, Bs, As), where R, G, B, and A respectively stand for the red, green, blue, and alpha source 
        /// values.
        /// </summary>
        SourceColour,

        /// <summary>
        /// Each component of the colour is multiplied by the inverse of the source colour. This can be represented as
        /// (1 − Rs, 1 − Gs, 1 − Bs, 1 − As) where R, G, B, and A respectively stand for the red, green, blue,
        /// and alpha destination values.
        /// </summary>
        InverseSourceColour,

        /// <summary>
        /// Each component of the colour is multiplied by the alpha value of the source. This can be represented as 
        /// (As, As, As, As), where As is the alpha source value.
        /// </summary>
        SourceAlpha,

        /// <summary>
        /// Each component of the colour is multiplied by the inverse of the alpha value of the source. This can be 
        /// represented as (1 − As, 1 − As, 1 − As, 1 − As), where As is the alpha destination value.
        /// </summary>
        InverseSourceAlpha,

        /// <summary>
        /// Each component of the colour is multiplied by the alpha value of the destination. This can be represented
        /// as (Ad, Ad, Ad, Ad), where Ad is the destination alpha value.
        /// </summary>
        DestinationAlpha,

        /// <summary>
        /// Each component of the colour is multiplied by the inverse of the alpha value of the destination. This can
        /// be represented as (1 − Ad, 1 − Ad, 1 − Ad, 1 − Ad), where Ad is the alpha destination value.
        /// </summary>
        InverseDestinationAlpha,

        /// <summary>
        /// Each component colour is multiplied by the destination colour. This can be represented as (Rd, Gd, Bd, Ad),
        /// where R, G, B, and A respectively stand for red, green, blue, and alpha destination values.
        /// </summary>
        DestinationColour,

        /// <summary>
        /// Each component of the colour is multiplied by the inverse of the destination colour. This can be 
        /// represented as (1 − Rd, 1 − Gd, 1 − Bd, 1 − Ad), where Rd, Gd, Bd, and Ad respectively stand for the red,
        /// green, blue, and alpha destination values.
        /// </summary>
        InverseDestinationColour,

        /// <summary>
        /// Each component of the colour is multiplied by either the alpha of the source colour, or the inverse of the
        /// alpha of the source colour, whichever is greater. This can be represented as (f, f, f, 1), where
        /// f = min (A, 1 − Ad).
        /// </summary>
        //SourceAlphaSaturation,

        /// <summary>
        /// Each component of the colour is multiplied by a constant set in BlendFactor.
        /// </summary>
        //ConstantColour,

        /// <summary>
        /// Each component of the colour is multiplied by the inverse of a constant set in BlendFactor.
        /// </summary>
        //InverseConstantColour,
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public enum BlendFunction
    {
        /// <summary>
        /// The result is the destination added to the source.
        /// Result = (Source Colour * Source Blend) + (Destination Colour * Destination Blend)
        /// </summary>
        Add,

        /// <summary>
        /// The result is the destination subtracted from the source.
        /// Result = (Source Colour * Source Blend) − (Destination Colour * Destination Blend)
        /// </summary>
        Subtract,

        /// <summary>
        /// The result is the source subtracted from the destination.
        /// Result = (Destination Colour * Destination Blend) − (Source Colour * Source Blend)
        /// </summary>
        ReverseSubtract,

        /// <summary>
        /// The result is the maximum of the source and destination.
        /// Result = max ((Source Colour * Source Blend), (Destination Colour * Destination Blend))
        /// </summary>
        Max,

        /// <summary>
        /// The result is the minimum of the source and destination.
        /// Result = min ((Source Colour * Source Blend), (Destination Colour * Destination Blend))
        /// </summary>
        Min,
    }
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public enum VertexElementUsage
    {
        /// <summary>
        /// 
        /// </summary>
        Position,
        
        /// <summary>
        /// 
        /// </summary>
        Colour,
        
        /// <summary>
        /// 
        /// </summary>
        TextureCoordinate,
        
        /// <summary>
        /// 
        /// </summary>
        Normal,
        
        /// <summary>
        /// 
        /// </summary>
        Binormal,
        
        /// <summary>
        /// 
        /// </summary>
        Tangent,
        
        /// <summary>
        /// 
        /// </summary>
        BlendIndices,
        
        /// <summary>
        /// 
        /// </summary>
        BlendWeight,
        
        /// <summary>
        /// 
        /// </summary>
        Depth,
        
        /// <summary>
        /// 
        /// </summary>
        Fog,
        
        /// <summary>
        /// 
        /// </summary>
        PointSize,
        
        /// <summary>
        /// 
        /// </summary>
        Sample,
        
        /// <summary>
        /// 
        /// </summary>
        TessellateFactor
    }
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    public enum VertexElementFormat
    {
        /// <summary>
        /// 
        /// </summary>
        Single,
        
        /// <summary>
        /// 
        /// </summary>
        Vector2,
        
        /// <summary>
        /// 
        /// </summary>
        Vector3,
        
        /// <summary>
        /// 
        /// </summary>
        Vector4,
        
        /// <summary>
        /// 
        /// </summary>
        Colour,
        
        /// <summary>
        /// 
        /// </summary>
        Byte4,
        
        /// <summary>
        /// 
        /// </summary>
        Short2,
        
        /// <summary>
        /// 
        /// </summary>
        Short4,
        
        /// <summary>
        /// 
        /// </summary>
        NormalisedShort2,
        
        /// <summary>
        /// 
        /// </summary>
        NormalisedShort4,
        
        /// <summary>
        /// 
        /// </summary>
        HalfVector2,
        
        /// <summary>
        /// 
        /// </summary>
        HalfVector4
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
	
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

    public enum ShaderFormat
    {
        GLSL,
        GLSL_ES,
        HLSL
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class ShaderDeclaration
        : IEquatable <ShaderDeclaration>
    {
        /// <summary>
        /// Defines a global name for this shader
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Lists all of the supported inputs into this shader and
        /// defines whether or not they are optional to an implementation.
        /// </summary>
        public List<ShaderInputDeclaration> InputDeclarations { get; set; }

        /// <summary>
        /// Defines all of the variables supported by this shader.  Every
        /// variant must support all of the variables.
        /// </summary>
        public List<ShaderVariableDeclaration> VariableDeclarations { get; set; }


        /// <summary>
        /// ?
        /// </summary>
        public List<ShaderSamplerDeclaration> SamplerDeclarations { get; set; }

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;

            if (obj is ShaderDeclaration)
            {
                flag = this.Equals ((ShaderDeclaration) obj);
            }

            return flag;
        }

        public Boolean Equals (ShaderDeclaration other)
        {
            if (this.Name != other.Name)
                return false;

            if (this.InputDeclarations == null && other.InputDeclarations != null)
                return false;

            if (this.InputDeclarations != null && other.InputDeclarations == null)
                return false;

            for (Int32 i = 0; i < this.InputDeclarations.Count; ++i)
                if (this.InputDeclarations [i] != other.InputDeclarations [i])
                    return false;

            if (this.VariableDeclarations == null && other.VariableDeclarations != null)
                return false;

            if (this.VariableDeclarations != null && other.VariableDeclarations == null)
                return false;

            for (Int32 i = 0; i < this.VariableDeclarations.Count; ++i)
                if (this.VariableDeclarations [i] != other.VariableDeclarations [i])
                    return false;

            if (this.SamplerDeclarations == null && other.SamplerDeclarations != null)
                return false;

            if (this.SamplerDeclarations != null && other.SamplerDeclarations == null)
                return false;

            for (Int32 i = 0; i < this.SamplerDeclarations.Count; ++i)
                if (this.SamplerDeclarations [i] != other.SamplerDeclarations [i])
                    return false;

            return true;
        }

        public static Boolean operator == (ShaderDeclaration a, ShaderDeclaration b)
        {
            return Equals (a, b);
        }

        public static Boolean operator != (ShaderDeclaration a, ShaderDeclaration b)
        {
            return !Equals (a, b);
        }
    }


    public sealed class ShaderInputDeclaration
        : IEquatable <ShaderInputDeclaration>
    {
        String niceName;
        Type defaultType;
        Object defaultValue;

        public ShaderInputDeclaration ()
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

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;

            if (obj is ShaderInputDeclaration)
            {
                flag = this.Equals ((ShaderInputDeclaration) obj);
            }

            return flag;
        }

        public Boolean Equals (ShaderInputDeclaration other)
        {
            if (this.niceName != other.niceName)
                return false;

            if (this.defaultType != other.defaultType)
                return false;

            if (this.defaultValue.ToString () != other.defaultValue.ToString ())
                return false;

            if (this.Name != other.Name)
                return false;

            if (this.Usage != other.Usage)
                return false;

            if (this.Optional != other.Optional)
                return false;

            return true;
        }

        public static Boolean operator == (ShaderInputDeclaration a, ShaderInputDeclaration b)
        {
            return Equals (a, b);
        }

        public static Boolean operator != (ShaderInputDeclaration a, ShaderInputDeclaration b)
        {
            return !Equals (a, b);
        }
    }

    public sealed class ShaderSamplerDeclaration
        : IEquatable <ShaderSamplerDeclaration>
    {
        String niceName;

        public ShaderSamplerDeclaration ()
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

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;

            if (obj is ShaderSamplerDeclaration)
            {
                flag = this.Equals ((ShaderSamplerDeclaration) obj);
            }

            return flag;
        }

        public Boolean Equals (ShaderSamplerDeclaration other)
        {
            if (this.niceName != other.niceName)
                return false;

            if (this.Name != other.Name)
                return false;

            if (this.Optional != other.Optional)
                return false;

            return true;
        }

        public static Boolean operator == (ShaderSamplerDeclaration a, ShaderSamplerDeclaration b)
        {
            return Equals (a, b);
        }

        public static Boolean operator != (ShaderSamplerDeclaration a, ShaderSamplerDeclaration b)
        {
            return !Equals (a, b);
        }
    }

    public sealed class ShaderVariableDeclaration
        : IEquatable <ShaderVariableDeclaration>
    {
        String niceName;
        Type defaultType;
        Object defaultValue;

        public ShaderVariableDeclaration ()
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

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;

            if (obj is ShaderVariableDeclaration)
            {
                flag = this.Equals ((ShaderVariableDeclaration) obj);
            }

            return flag;
        }

        public Boolean Equals (ShaderVariableDeclaration other)
        {
            if (this.niceName != other.niceName)
                return false;

            if (this.Name != other.Name)
                return false;

            if (this.defaultType != other.defaultType)
                return false;

            if (this.defaultValue.ToString () != other.defaultValue.ToString ())
                return false;

            return true;
        }

        public static Boolean operator == (ShaderVariableDeclaration a, ShaderVariableDeclaration b)
        {
            return Equals (a, b);
        }

        public static Boolean operator != (ShaderVariableDeclaration a, ShaderVariableDeclaration b)
        {
            return !Equals (a, b);
        }
    }

    public sealed class ShaderInputInfo
    {
        // The index of the input with respect to this shader's other inputs.
        public Int32 Index { get; internal set; }

        // The name in the shader's source code for the input.
        public String Name { get; internal set; }

        // The C# type used to represent the input.
        public Type Type { get; internal set; }
    }

    public sealed class ShaderVariableInfo
    {
        // The index of the variable with respect to this shader's other variables.
        public Int32 Index { get; internal set; }

        // The name in the shader's source code for the variable.
        public String Name { get; internal set; }

        // The C# type used to represent the variable.
        public Type Type { get; internal set; }
    }

    public sealed class ShaderSamplerInfo
    {
        // The index of the sampler with respect to this shader's other samplers.
        public Int32 Index { get; internal set; }

        // The name in the shader's source code for the sampler.
        public String Name { get; internal set; }
    }
    
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

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
        public static Boolean operator != (VertexDeclaration one, VertexDeclaration other)
        {
            return !(one == other);
        }

        /// <summary>
        /// 
        /// </summary>
        public static Boolean operator == (VertexDeclaration one, VertexDeclaration other)
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
    /// 
    /// </summary>
    public enum TouchPhase
    {
        /// <summary>
        /// 
        /// </summary>
        Invalid = 0,
        
        /// <summary>
        /// 
        /// </summary>
        JustReleased = 1,
        
        /// <summary>
        /// 
        /// </summary>
        JustPressed = 2,
        
        /// <summary>
        /// 
        /// </summary>
        Active = 3,
    }

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    
    public class RawTouch
    {
        public String Id { get; set; }
        public Vector2 Position { get; set; }
        public TouchPhase Phase { get; set; }
    }
    
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //
    
    internal static class Int32Extensions
    {
        // http://msdn.microsoft.com/en-us/library/system.object.gethashcode(v=vs.110).aspx
        public static Int32 ShiftAndWrap (this Int32 value, Int32 positions = 2)
        {
            positions = positions & 0x1F;
    
            // Save the existing bit pattern, but interpret it as an unsigned integer. 
            uint number = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
            // Preserve the bits to be discarded. 
            uint wrapped = number >> (32 - positions);
            // Shift and wrap the discarded bits. 
            return BitConverter.ToInt32(BitConverter.GetBytes((number << positions) | wrapped), 0);
        }
    }
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    internal static class VertexElementFormatHelper
    {
        /// <summary>
        ///
        /// </summary>
        internal static Type FromEnum (VertexElementFormat format)
        {
            switch (format)
            {
                case VertexElementFormat.Single:
                    return typeof (Single);
                case VertexElementFormat.Vector2:
                    return typeof (Vector2);
                case VertexElementFormat.Vector3:
                    return typeof (Vector3);
                case VertexElementFormat.Vector4:
                    return typeof (Vector4);
                case VertexElementFormat.Colour:
                    return typeof (Rgba32);
                case VertexElementFormat.Byte4:
                    return typeof (Byte4);
                case VertexElementFormat.Short2:
                    return typeof (Short2);
                case VertexElementFormat.Short4:
                    return typeof (Short4);
                case VertexElementFormat.NormalisedShort2:
                    return typeof (NormalisedShort2);
                case VertexElementFormat.NormalisedShort4:
                    return typeof (NormalisedShort4);
                //case VertexElementFormat.HalfVector2:
                //    return typeof (Abacus.HalfPrecision.Vector2);
                //case VertexElementFormat.HalfVector4:
                //    return typeof (Abacus.HalfPrecision.Vector4);
            }

            throw new NotSupportedException ();
        }
    }


    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    internal static class PrimitiveHelper
    {
        /// <summary>
        ///
        /// </summary>
        internal static Int32 NumVertsIn (PrimitiveType type)
        {
            switch (type)
            {
                case PrimitiveType.TriangleList:
                    return 3;
                case PrimitiveType.TriangleStrip:
                    throw new NotImplementedException ();
                case PrimitiveType.LineList:
                    return 2;
                case PrimitiveType.LineStrip:
                    throw new NotImplementedException ();
                default:
                    throw new NotImplementedException ();
            }
        }
    }
    
        
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>    
    internal static class VertexElementValidator
    {
        /// <summary>
        /// 
        /// </summary>
        internal static Int32 GetTypeSize (VertexElementFormat format)
        {
            switch (format)
            {
                case VertexElementFormat.Single: return 4;
                case VertexElementFormat.Vector2: return 8;
                case VertexElementFormat.Vector3: return 12;
                case VertexElementFormat.Vector4: return 0x10;
                case VertexElementFormat.Colour: return 4;
                case VertexElementFormat.Byte4: return 4;
                case VertexElementFormat.Short2: return 4;
                case VertexElementFormat.Short4: return 8;
                case VertexElementFormat.NormalisedShort2: return 4;
                case VertexElementFormat.NormalisedShort4: return 8;
                case VertexElementFormat.HalfVector2: return 4;
                case VertexElementFormat.HalfVector4: return 8;
            }

            throw new Exception ("Unsupported");
        }

        /// <summary>
        /// 
        /// </summary>
        internal static int GetVertexStride (VertexElement[] elements)
        {
            Int32 num2 = 0;

            for (Int32 i = 0; i < elements.Length; i++)
            {
                Int32 num3 = elements [i].Offset + GetTypeSize (elements [i].VertexElementFormat);

                if (num2 < num3)
                {
                    num2 = num3;
                }
            }

            return num2;
        }

        /// <summary>
        /// checks that an effect supports the given vert decl
        /// </summary>
        //internal static void Validate (IShader effect, VertexDeclaration vertexDeclaration)
        //{
        //    throw new NotImplementedException ();
       // }

        /// <summary>
        /// 
        /// </summary>
        internal static void Validate (int vertexStride, VertexElement[] elements)
        {
            if (vertexStride <= 0)
            {
                throw new ArgumentOutOfRangeException ("vertexStride");
            }
            
            if ((vertexStride & 3) != 0)
            {
                throw new ArgumentException ("VertexElementOffsetNotMultipleFour");
            }
            
            var numArray = new Int32[vertexStride];
            
            for (Int32 i = 0; i < vertexStride; i++)
            {
                numArray [i] = -1;
            }
            
            for (Int32 j = 0; j < elements.Length; j++)
            {
                Int32 offset = elements [j].Offset;
                
                Int32 typeSize = GetTypeSize (elements [j].VertexElementFormat);
                
                if ((elements [j].VertexElementUsage < VertexElementUsage.Position) || 
                    (elements [j].VertexElementUsage > VertexElementUsage.TessellateFactor)) 
                {
                    throw new ArgumentException ("FrameworkResources.VertexElementBadUsage");
                }
                
                if ((offset < 0) || ((offset + typeSize) > vertexStride))
                {
                    throw new ArgumentException ("FrameworkResources.VertexElementOutsideStride");
                }
                
                if ((offset & 3) != 0)
                {
                    throw new ArgumentException ("VertexElementOffsetNotMultipleFour");
                }
                
                for (Int32 k = 0; k < j; k++)
                {
                    if ((elements [j].VertexElementUsage == elements [k].VertexElementUsage) && 
                        (elements [j].UsageIndex == elements [k].UsageIndex))
                    {
                        throw new ArgumentException ("DuplicateVertexElement");
                    }
                }

                for (Int32 m = offset; m < (offset + typeSize); m++)
                {
                    if (numArray [m] >= 0)
                    {
                        throw new ArgumentException ("VertexElementsOverlap");
                    }

                    numArray [m] = j;
                }
            }
        }
    }
}
