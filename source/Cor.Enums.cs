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
    /// A keyboard key can be described as being in one of these states.
    /// </summary>
    public enum KeyState
    {
        /// <summary>
        /// The key is pressed.
        /// </summary>
        Down,

        /// <summary>
        /// The key is released.
        /// </summary>
        Up,  
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// All supported functional keyboard keys (not including character keys).
    /// </summary>
    [Flags]
    public enum FunctionalKey
    {
        /// <summary>
        /// BACKSPACE key.
        /// </summary>
        Backspace,

        /// <summary>
        /// TAB key.
        /// </summary> 
        Tab,

        /// <summary>
        /// ENTER key.
        /// </summary>
        Enter,

        /// <summary>
        /// CAPS LOCK key.
        /// </summary>
        CapsLock,

        /// <summary>
        /// ESC key.
        /// </summary>
        Escape,

        // SPACEBAR
        Spacebar,

        /// <summary>
        /// PAGE UP key.
        /// </summary>
        PageUp,

        /// <summary>
        /// PAGE DOWN key.
        /// </summary>
        PageDown,

        /// <summary>
        /// END key.
        /// </summary>   
        End,

        /// <summary>
        /// HOME key.
        /// </summary>
        Home,

        /// <summary>
        /// LEFT ARROW key.
        /// </summary>
        Left,

        /// <summary>
        /// UP ARROW key.
        /// </summary>
        Up,

        /// <summary>
        /// RIGHT ARROW key.
        /// </summary>
        Right,

        /// <summary>
        /// DOWN ARROW key.
        /// </summary>
        Down,

        /// <summary>
        /// SELECT key.
        /// </summary>
        Select,

        /// <summary>
        /// PRINT key.
        /// </summary>
        Print,        

        /// <summary>
        /// EXECUTE key.
        /// </summary>
        Execute,

        /// <summary>
        /// PRINT SCREEN key.
        /// </summary>
        PrintScreen,

        /// <summary>
        /// INS key.
        /// </summary>
        Insert,

        /// <summary>
        /// DEL key.
        /// </summary>
        Delete,

        /// <summary>
        /// HELP key.
        /// </summary>
        Help,

        /// <summary>
        /// Left Windows key.
        /// </summary>
        LeftWindows,

        /// <summary>
        /// Right Windows key.
        /// </summary>
        RightWindows,

        /// <summary>
        /// Left Windows key.
        /// </summary>
        LeftFlower,

        /// <summary>
        /// Right Windows key.
        /// </summary>
        RightFlower,

        /// <summary>
        /// Applications key.
        /// </summary>
        Apps,

        /// <summary>
        /// Computer Sleep key.
        /// </summary>
        Sleep,

        /// <summary>
        /// Numeric pad 0 key.
        /// </summary>
        NumPad0,

        /// <summary>
        /// Numeric pad 1 key.
        /// </summary>
        NumPad1,

        /// <summary>
        /// Numeric pad 2 key.
        /// </summary>
        NumPad2,

        /// <summary>
        /// Numeric key
        /// pad 3 key.
        /// </summary>
        NumPad3,

        /// <summary>
        /// Numeric key
        /// pad 4 key.
        /// </summary>
        NumPad4,

        /// <summary>
        /// Numeric pad 5 key.
        /// </summary>
        NumPad5,

        /// <summary>
        /// Numeric pad 6 key.
        /// </summary>
        NumPad6,

        /// <summary>
        /// Numeric pad 7 key.
        /// </summary>
        NumPad7,

        /// <summary>
        /// Numeric pad 8 key.
        /// </summary>
        NumPad8,

        /// <summary>
        /// Numeric pad 9 key.
        /// </summary>
        NumPad9,

        /// <summary>
        /// Multiply key.
        /// </summary>
        Multiply,

        /// <summary>
        /// Add key.
        /// </summary>
        Add,

        /// <summary>
        /// Separator key.
        /// </summary>
        Separator,

        /// <summary>
        /// Subtract key.
        /// </summary>
        Subtract,

        /// <summary>
        /// Decimal key.
        /// </summary>
        Decimal,

        /// <summary>
        /// Divide key.
        /// </summary>
        Divide,

        /// <summary>
        /// F1 key.
        /// </summary>
        F1,

        /// <summary>
        /// F2 key.
        /// </summary>
        F2,

        /// <summary>
        /// F3 key.
        /// </summary>
        F3,

        /// <summary>
        /// F4 key.
        /// </summary>
        F4,

        /// <summary>
        /// F5 key.
        /// </summary>
        F5,

        /// <summary>
        /// F6 key.
        /// </summary>
        F6,

        /// <summary>
        /// F7 key.
        /// </summary>
        F7,

        /// <summary>
        /// F8 key.
        /// </summary>
        F8,

        /// <summary>
        /// F9 key.
        /// </summary>
        F9,

        /// <summary>
        /// F10 key.
        /// </summary>
        F10,

        /// <summary>
        /// F11 key.
        /// </summary>
        F11,

        /// <summary>
        /// F12 key.
        /// </summary>
        F12,

        /// <summary>
        /// F13 key.
        /// </summary>
        F13,

        /// <summary>
        /// F14 key.
        /// </summary>
        F14,

        /// <summary>
        /// F15 key.
        /// </summary>
        F15,

        /// <summary>
        /// F16 key.
        /// </summary>
        F16,

        /// <summary>
        /// F17 key.
        /// </summary>
        F17,

        /// <summary>
        /// F18 key.
        /// </summary>
        F18,

        /// <summary>
        /// F19 key.
        /// </summary>
        F19,

        /// <summary>
        /// F20 key.
        /// </summary>
        F20,

        /// <summary>
        /// F21 key.
        /// </summary>
        F21,

        /// <summary>
        /// F22 key.
        /// </summary>
        F22,

        /// <summary>
        /// F23 key.
        /// </summary>
        F23,

        /// <summary>
        /// F24 key.
        /// </summary>
        F24,

        /// <summary>
        /// NUM LOCK key.
        /// </summary>
        NumLock,

        /// <summary>
        /// SCROLL LOCK key.
        /// </summary>
        ScrollLock,

        /// <summary>
        /// Left SHIFT key.
        /// </summary>
        LeftShift,

        /// <summary>
        /// Right SHIFT key.
        /// </summary>
        RightShift,

        /// <summary>
        /// Left CONTROL key.
        /// </summary>
        LeftControl,

        /// <summary>
        /// Right CONTROL key.
        /// </summary>
        RightControl,

        /// <summary>
        /// Left ALT key.
        /// </summary>
        LeftAlt,

        /// <summary>
        /// Right ALT key.
        /// </summary>
        RightAlt,
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public enum SurfaceFormat
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

    /// <summary>
    /// 
    /// </summary>
    public enum ButtonState
    {
        /// <summary>
        /// 
        /// </summary>
        Released,

        /// <summary>
        /// 
        /// </summary>
        Pressed,
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
    [Flags]
    enum ClearOptions
    {
        /// <summary>
        /// 
        /// </summary>
        DepthBuffer = 2,

        /// <summary>
        /// 
        /// </summary>
        Stencil = 4,
        
        /// <summary>
        /// 
        /// </summary>
        Target = 1,
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>    
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

    /// <summary>
    /// 
    /// </summary>
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

    /// <summary>
    /// 
    /// </summary>
    public enum PlayerIndex
    {
        /// <summary>
        /// 
        /// </summary>
        One,
        
        /// <summary>
        /// 
        /// </summary>
        Two,
        
        /// <summary>
        /// 
        /// </summary>
        Three,
        
        /// <summary>
        /// 
        /// </summary>
        Four,
    }
    

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// 
    /// </summary>
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
}