// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor! - Low Level 3D App Engine                                         │ \\
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
using System.Runtime.InteropServices;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

using Abacus;
using Abacus.Packed;
using Abacus.SinglePrecision;
using Abacus.Int32Precision;

namespace Sungiant.Cor
{
    /// <summary>
    /// The Cor! framework provides a user's app access to Cor!
    /// features via this interface.
    /// </summary>
    public interface ICor
    {
        /// <summary>
        /// Provides access to Cor's audio manager.
        /// </summary>
        IAudioManager Audio { get; }

        /// <summary>
        /// Provides access to Cor's graphics manager, which
        /// provides an interface to working with the GPU.
        /// </summary>
        IGraphicsManager Graphics { get; }

        /// <summary>
        /// Provides access to Cor's resource manager.
        /// </summary>
        IResourceManager Resources { get; }

        /// <summary>
        /// Provides access to Cor's input manager.
        /// </summary>
        IInputManager Input { get; }

        /// <summary>
        /// Provides access to Cor's system manager.
        /// </summary>
        ISystemManager System { get; }

        /// <summary>
        /// Provides access to Cor's Asset sysetm.
        /// </summary>
        AssetManager Assets { get; }

        /// <summary>
        /// Provides access to Cor's logging system.
        /// </summary>
        LogManager Log { get; }

        /// <summary>
        /// Gets the <see cref="Sungiant.Cor.AppSettings"/>
        /// value used by the Cor! framework when initilising the app.
        /// </summary>
        AppSettings Settings { get; }
    }

    #region Interfaces

    /// <summary>
    /// todo
    /// </summary>
    public interface IAudioManager
    {
        /// <summary>
        /// Sets the volume, between 0.0 - 1.0
        /// </summary>
        Single Volume { get; set; }
    }

    /// <summary>
    /// This interface provides access to the gpu.  It's behaves as a state
    /// machine, change settings, then call and draw function, rinse, repeat.
    ///
    /// IGraphicsManager Todo List
    /// --------------------------
    /// - stencil buffers
    /// - decided whether or not to stick with the geom-buffer abstraction
    ///   or ditch it, dropping support for Psm, but adding support for
    ///   independent Vert and Index buffers.
    /// - Work out a consistent way to deal with AOT limitations on Generics.
    /// </summary>
    public interface IGraphicsManager
    {
        /// <summary>
        /// Provides information about the current back buffer.
        /// </summary>
        IDisplayStatus DisplayStatus { get; }

        /// <summary>
        // Debugging utilies, if not supported on your platform, this will
        // still exist, but the functions will do nothing.
        /// </summary>
        IGpuUtils GpuUtils { get; }

        /// <summary>
        /// Resets the graphics manager to it's default state.
        /// </summary>
        void Reset();

        /// <summary>
        /// Clears the colour buffer to the specified colour.
        /// </summary>
        void ClearColourBuffer(Rgba32 color = new Rgba32());

        /// <summary>
        /// Clears the depth buffer to the specified depth.
        /// </summary>
        void ClearDepthBuffer(Single depth = 1f);

        /// <summary>
        /// Sets the GPU's current culling mode to the value specified.
        /// </summary>
        void SetCullMode(CullMode cullMode);
        
        /// <summary>
        /// With the current design the only way you can create geom buffers is 
        /// here.  This is to maintain consistency across platforms by bowing to
        /// the quirks of PlayStation Mobile. Each IGeometryBuffer has vert
        /// data, and optionally index data.  Normally this data would be
        /// seperate, so you can upload one chunk of vert data, and, say, 5 sets
        /// of index data, then achive neat optimisations like switching on
        /// index data whilst keeping the vert data the same, resulting in  
        /// defining different shapes, saving on memory and context switching 
        /// (this is how the grass worked on Pure).
        ///
        /// Right now I am endevouring to support PlayStation Mobile so vert and
        /// index buffers are combined into a single geom buffer.
        ///
        /// EDIT: I think this should be split up again.  And the get the Psm
        ///       runtime to internally create a load of geom-buffers for 
        ///       index and vert buffer combinations as they arise...
        ///       Hmmm... Still thinking...
        /// </summary>
        IGeometryBuffer CreateGeometryBuffer (
            VertexDeclaration vertexDeclaration,
            Int32 vertexCount,
            Int32 indexCount );

        /// <summary>
        /// Sets the active geometry buffer.
        /// </summary>
        void SetActiveGeometryBuffer(IGeometryBuffer buffer);

        /// <summary>
        /// Sets the active texture for a given slot.
        /// </summary>
        void SetActiveTexture(Int32 slot, Texture2D tex);

        /// <summary>
        /// Defines how we blend colours
        /// </summary>
        void SetBlendEquation(
            BlendFunction rgbBlendFunction, 
            BlendFactor sourceRgb, 
            BlendFactor destinationRgb,
            BlendFunction alphaBlendFunction, 
            BlendFactor sourceAlpha, 
            BlendFactor destinationAlpha
            );

        /// <summary>
        /// Renders a sequence of non-indexed geometric primitives of the 
        /// specified type from the active geometry buffer (which sits in GRAM).
        ///
        /// Info: From GRAM - Non-Indexed.
        ///
        /// Arguments:
        ///   primitiveType  -> Describes the type of primitive to render.
        ///   startVertex    -> Index of the first vertex to load. Beginning at 
        ///                     startVertex, the correct number of vertices is 
        ///                     read out of the vertex buffer.
        ///   primitiveCount -> Number of primitives to render. The 
        ///                     primitiveCount is the number of primitives as 
        ///                     determined by the primitive type. If it is a 
        ///                     line list, each primitive has two vertices. If 
        ///                     it is a triangle list, each primitive has three 
        ///                      vertices.
        /// </summary>
        void DrawPrimitives(
            PrimitiveType primitiveType,            
            Int32 startVertex,                      
            Int32 primitiveCount );                 

        /// <summary>
        /// Renders a sequence of indexed geometric primitives of the 
        /// specified type from the active geometry buffer (which sits in GRAM).
        ///
        /// Info: From GRAM - Indexed.
        ///
        /// Arguments:
        ///   primitiveType  -> Describes the type of primitive to render. 
        ///                     PrimitiveType.PointList is not supported with 
        ///                     this method.
        ///   baseVertex     -> Offset to add to each vertex index in the index 
        ///                     buffer.
        ///   minVertexIndex -> Minimum vertex index for vertices used during 
        ///                     the call. The minVertexIndex parameter and all 
        ///                     of the indices in the index stream are relative 
        ///                     to the baseVertex parameter.
        ///   numVertices    -> Number of vertices used during the call. The 
        ///                     first vertex is located at index: baseVertex + 
        ///                     minVertexIndex.
        ///   startIndex     -> Location in the index array at which to start 
        ///                     reading vertices.
        ///   primitiveCount -> Number of primitives to render. The number of 
        ///                     vertices used is a function of primitiveCount 
        ///                     and primitiveType.
        /// </summary>
        void DrawIndexedPrimitives (
            PrimitiveType primitiveType,
            Int32 baseVertex,
            Int32 minVertexIndex,
            Int32 numVertices,
            Int32 startIndex,
            Int32 primitiveCount
            );

        /// <summary>
        /// Draws un-indexed vertex data uploaded straight from RAM.
        ///
        /// Info: From System RAM - Non-Indexed
        ///
        /// Arguments:
        /// primitiveType     -> Describes the type of primitive to render.
        /// vertexData        -> The vertex data.
        /// vertexOffset      -> Offset (in vertices) from the beginning of the 
        ///                      buffer to start reading data.
        /// primitiveCount    -> Number of primitives to render.
        /// vertexDeclaration -> The vertex declaration, which defines 
        ///                      per-vertex data.
        /// </summary>
        void DrawUserPrimitives <T> (
            PrimitiveType primitiveType,
            T[] vertexData,
            Int32 vertexOffset,
            Int32 primitiveCount,
            VertexDeclaration vertexDeclaration )
            where T : struct, IVertexType;

        /// <summary>
        /// Draws indexed vertex data uploaded straight from RAM.
        ///
        /// Info: From System RAM - Indexed
        ///
        /// Arguments:
        /// primitiveType     -> Describes the type of primitive to render.
        /// vertexData        -> The vertex data.
        /// vertexOffset      -> Offset (in vertices) from the beginning of the 
        ///                      vertex buffer to the first vertex to draw.
        /// numVertices       -> Number of vertices to draw.
        /// indexData         -> The index data.
        /// indexOffset       -> Offset (in indices) from the beginning of the
        ///                      index buffer to the first index to use.
        /// primitiveCount    -> Number of primitives to render.
        /// vertexDeclaration -> The vertex declaration, which defines 
        ///                      per-vertex data.
        /// </summary>
        void DrawUserIndexedPrimitives <T> (
            PrimitiveType primitiveType,
            T[] vertexData,
            Int32 vertexOffset,
            Int32 numVertices,
            Int32[] indexData,
            Int32 indexOffset,
            Int32 primitiveCount,
            VertexDeclaration vertexDeclaration ) 
            where T : struct, IVertexType;
    }

    /// <summary>
    /// Depending on the implementation you are running against
    /// various input devices will be avaiable.  Those that are
    /// not will be returned as NULL.  It is down to your app to
    /// deal with only some of input devices being available.
    /// For example, if you are running on iPad, the GetXbox360Gamepad
    /// method will return NULL.  The way to make your app deal with
    /// multiple platforms is to poll the input devices at bootup
    /// and then query only those that are avaible in your update loop.  
    /// </summary>
    public interface IInputManager
    {
        /// <summary>
        // An Xbox 360 gamepad.
        /// </summary>
        IXbox360Gamepad Xbox360Gamepad { get; }

        /// <summary>
        // The virtual gamepad used by PlayStation Mobile systems, 
        // if you are running on Vita this will be the Vita itself.
        /// </summary>
        IPsmGamepad PsmGamepad { get; }

        /// <summary>
        // A generalised multitouch pad, which may or may
        // not have a screen.
        /// </summary>
        IMultiTouchController MultiTouchController { get; }

        /// <summary>
        // A very basic gamepad, supported by most implementations.
        /// </summary>
        IGenericGamepad GenericGamepad { get; }

        /// <summary>
        // A computer mouse.
        /// </summary>
        IMouse Mouse { get; }

        /// <summary>
        // A computer keyboard.
        /// </summary>
        IKeyboard Keyboard { get; }
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IResourceManager
    {
        /// <summary>
        /// todo
        /// </summary>
        T Load<T>(String path) where T : IResource;

        /// <summary>
        /// todo
        /// </summary>
        T Open<T>(String path) where T : IDisposable;

        /// <summary>
        /// todo
        /// </summary>
        IShader LoadShader(ShaderType shaderType);
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface ISystemManager
    {
        /// <summary>
        /// todo
        /// </summary>
        Point2 CurrentDisplaySize { get; }

        /// <summary>
        /// todo
        /// </summary>
        String OperatingSystem { get; }

        /// <summary>
        /// todo
        /// </summary>
        String DeviceName { get; }

        /// <summary>
        /// todo
        /// </summary>
        String DeviceModel { get; }

        /// <summary>
        /// todo
        /// </summary>
        String SystemName { get; }

        /// <summary>
        /// todo
        /// </summary>
        String SystemVersion { get; }

        /// <summary>
        /// todo
        /// </summary>
        DeviceOrientation CurrentOrientation { get; }

        /// <summary>
        /// todo
        /// </summary>
        IScreenSpecification ScreenSpecification { get; }

        /// <summary>
        /// todo
        /// </summary>
        IPanelSpecification PanelSpecification { get; }
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IApp
    {
        /// <summary>
        /// todo
        /// </summary>
        void Initilise(ICor cor);

        /// <summary>
        /// todo
        /// </summary>
        Boolean Update(AppTime time);

        /// <summary>
        /// todo
        /// </summary>
        void Render();
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IDisplayStatus
    {
        /// <summary>
        /// Is the device running in fullscreen mode,
        /// for things that don't support fullscreen mode this will always be
        /// true.
        /// </summary>
        Boolean Fullscreen { get; }

        /// <summary>
        // Returns the current width of the frame buffer.
        // On most devices this will be the same as the screen size.
        // However on a PC or Mac the app could be running in windowed mode
        // and not take up the whole screen.  Or the screen could be a phone
        // where the screen res is 320x480, however at the current time it
        // is actually 480x320.
        /// </summary>
        Int32 CurrentWidth { get; }

        /// <summary>
        // Returns the current height of the frame buffer.
        /// On most devices this will be the same as the screen size.
        /// However on a PC or Mac the app could be running in windowed mode
        /// and not take up the whole screen.  Or the screen could be a phone
        // where the screen res is 320x480, however at the current time it
        // is actually 480x320.
        /// </summary>
        Int32 CurrentHeight { get; }
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IGpuUtils
    {
        /// <summary>
        /// todo
        /// </summary>
        Int32 BeginEvent(Rgba32 colour, String eventName);
            
        /// <summary>
        /// todo
        /// </summary>
        Int32 EndEvent();

        /// <summary>
        /// todo
        /// </summary>
        void SetMarker(Rgba32 colour, String eventName);

        /// <summary>
        /// todo
        /// </summary>
        void SetRegion(Rgba32 colour, String eventName);
    }

    /// <summary>
    /// Specifies the attributes a panel,
    /// a panel could be a screen, a touch device, or both.
    /// </summary>
    public interface IPanelSpecification
    {
        /// <summary>
        /// todo
        /// </summary>
        Vector2 PanelPhysicalSize { get; }

        /// <summary>
        /// todo
        /// </summary>
        Single PanelPhysicalAspectRatio { get; }

        /// <summary>
        /// todo
        /// </summary>
        PanelType PanelType { get; }
    }

    /// <summary>
    /// objects that Cor's! resource manager can load
    /// and track.
    /// </summary>
    public interface IResource
    {
    }

    /// <summary>
    /// Screen refers to the entire screen, not your frame
    /// buffer.  So if you had a monitor with a resolution of
    /// 1024x768 and a window with size 640x360 this would
    /// return 1024x768.
    /// </summary>
    public interface IScreenSpecification
    {
        /// <summary>
        /// Defines the total width of the screen in question in pixels when
        /// the device is in it's default orientation.
        /// </summary>
        Int32 ScreenResolutionWidth { get; }

        /// <summary>
        /// Defines the total height of the screen in question in pixels when
        /// the device is in it's default orientation.
        /// </summary>
        Int32 ScreenResolutionHeight { get; }

        /// <summary>
        /// This is just the ratio of the width / and height from the
        /// two functions above.
        /// </summary>
        Single ScreenResolutionAspectRatio { get; }
    }
    /// <summary>
    /// todo
    /// </summary>
    public interface IGeometryBuffer
    {
        /// <summary>
        /// todo
        /// </summary>
        IVertexBuffer VertexBuffer { get; }

        /// <summary>
        /// todo
        /// </summary>
        IIndexBuffer IndexBuffer { get; }
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IVertexBuffer
    {
        /// <summary>
        /// todo
        /// </summary>
        Int32 VertexCount { get; }

        /// <summary>
        /// todo
        /// </summary>
        VertexDeclaration VertexDeclaration { get; }

        /// <summary>
        /// todo
        /// </summary>
        void SetData<T> (T[] data)
        where T
            : struct
            , IVertexType;

        /// <summary>
        /// todo
        /// </summary>
        T[] GetData<T> () 
        where T
            : struct
            , IVertexType;

        /// <summary>
        /// todo
        /// </summary>
        void SetData<T> (
            T[] data,
            Int32 startIndex,
            Int32 elementCount)
        where T
            : struct
            , IVertexType;

        /// <summary>
        /// todo
        /// </summary>
        T[] GetData<T> (
            Int32 startIndex,
            Int32 elementCount)
        where T
            : struct
            , IVertexType;
        
        /// <summary>
        /// todo
        /// </summary>
        void SetRawData (
            Byte[] data, 
            Int32 startIndex, 
            Int32 elementCount);
        
        /// <summary>
        /// todo
        /// </summary>
        Byte[] GetRawData (
            Int32 startIndex, 
            Int32 elementCount);
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IVertexType
    {
        /// <summary>
        /// todo
        /// </summary>
        VertexDeclaration VertexDeclaration { get; }

        /// <summary>
        /// todo
        /// </summary>
        /// IntPtr GetAddress(Int32 elementIndex);
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IIndexBuffer
    {
        /// <summary>
        /// todo
        /// </summary>
        Int32 IndexCount { get; }

        /// <summary>
        /// todo
        /// </summary>
        void SetData(Int32[] data);

        /// <summary>
        /// todo
        /// </summary>
        void GetData(Int32[] data);

        /// <summary>
        /// todo
        /// </summary>
        void SetData(
            Int32[] data, 
            Int32 startIndex, 
            Int32 elementCount);

        /// <summary>
        /// todo
        /// </summary>
        void GetData(
            Int32[] data, 
            Int32 startIndex, 
            Int32 elementCount);

        /// <summary>
        /// todo
        /// </summary>
        void SetRawData(
            Byte[] data, 
            Int32 startIndex, 
            Int32 elementCount);

        /// <summary>
        /// todo
        /// </summary>
        Byte[] GetRawData(
            Int32 startIndex, 
            Int32 elementCount);
    }

    /// <summary>
    /// this interface provides a way to interact with a
    /// shader loaded on the GPU
    /// </summary>
    public interface IShader
    {
        /// <summary>
        /// Resets all the shader's variables to their default values.
        /// </summary>
        void ResetVariables();

        /// <summary>
        /// Resets all the shader's samplers to null textures.
        /// </summary>
        void ResetSamplerTargets();
  
        /// <summary>
        /// Sets the texture slot that a texture sampler should sample from.
        /// </summary>
        void SetSamplerTarget(String name, Int32 textureSlot);

        /// <summary>
        /// Provides access to the individual passes in this shader.
        /// the calling code can itterate though these and apply them 
        /// to the graphics context before it makes a draw call.
        /// </summary>
        IShaderPass[] Passes { get; }
        
        /// <summary>
        /// Defines which vertex elements are required by this shader.
        /// </summary>
        VertexElementUsage[] RequiredVertexElements { get; }

        /// <summary>
        /// Defines which vertex elements are optionally used by this
        /// shader if they happen to be present.
        /// </summary>
        VertexElementUsage[] OptionalVertexElements { get; }

        /// <summary>
        /// The name of this shader.
        /// </summary>
        String Name { get; }

        /// <summary>
        /// Sets the value of a specified shader variable.
        /// </summary>
        void SetVariable<T>(String name, T value);
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IShaderPass
    {
        /// <summary>
        /// todo
        /// </summary>
        String Name { get; }

        /// <summary>
        /// todo
        /// </summary>
        void Activate (VertexDeclaration vertexDeclaration);
    }

    #endregion

    #region Enums

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
        /// Each component of the colour is multiplied by the source colour.
        /// This can be represented as (Rs, Gs, Bs, As), where R, G, B, and A 
        /// respectively stand for the red, green, blue, and alpha source 
        /// values.
        /// </summary>
        SourceColour,

        /// <summary>
        /// Each component of the colour is multiplied by the inverse of the 
        /// source colour. This can be represented as
        /// (1 − Rs, 1 − Gs, 1 − Bs, 1 − As) where R, G, B, and A respectively 
        /// stand for the red, green, blue, and alpha destination values.
        /// </summary>
        InverseSourceColour,

        /// <summary>
        /// Each component of the colour is multiplied by the alpha value of the 
        /// source. This can be represented as (As, As, As, As), where As is the 
        /// alpha source value.
        /// </summary>
        SourceAlpha,

        /// <summary>
        /// Each component of the colour is multiplied by the inverse of the 
        /// alpha value of the source. This can be represented as 
        /// (1 − As, 1 − As, 1 − As, 1 − As), where As is the alpha destination 
        /// value.
        /// </summary>
        InverseSourceAlpha,

        /// <summary>
        /// Each component of the colour is multiplied by the alpha value of the 
        /// destination. This can be represented as (Ad, Ad, Ad, Ad), where Ad 
        /// is the destination alpha value.
        /// </summary>
        DestinationAlpha,

        /// <summary>
        /// Each component of the colour is multiplied by the inverse of the 
        /// alpha value of the destination. This can be represented as
        /// (1 − Ad, 1 − Ad, 1 − Ad, 1 − Ad), where Ad is the alpha destination 
        /// value.
        /// </summary>
        InverseDestinationAlpha,

        /// <summary>
        /// Each component colour is multiplied by the destination colour. This 
        /// can be represented as (Rd, Gd, Bd, Ad), where R, G, B, and A 
        /// respectively stand for red, green, blue, and alpha destination 
        /// values.
        /// </summary>
        DestinationColour,

        /// <summary>
        /// Each component of the colour is multiplied by the inverse of the 
        /// destination colour. This can be represented as 
        /// (1 − Rd, 1 − Gd, 1 − Bd, 1 − Ad), where Rd, Gd, Bd, and Ad 
        /// respectively stand for the red, green, blue, and alpha destination 
        /// values.
        /// </summary>
        InverseDestinationColour,

        /// <summary>
        /// Each component of the colour is multiplied by either the alpha of  
        /// the source colour, or the inverse of the alpha of the source colour, 
        /// whichever is greater. This can be represented as (f, f, f, 1), 
        /// where f = min(A, 1 − Ad).
        /// </summary>
        //SourceAlphaSaturation,

        /// <summary>
        /// Each component of the colour is multiplied by a constant set in 
        /// BlendFactor.
        /// </summary>
        //ConstantColour,

        /// <summary>
        /// Each component of the colour is multiplied by the inverse of a 
        /// constant set in BlendFactor.
        /// </summary>
        //InverseConstantColour,
    }

    /// <summary>
    /// todo
    /// </summary>
    public enum BlendFunction
    {
        /// <summary>
        /// The result is the destination added to the source.
        /// Result = (Source Colour * Source Blend) + 
        ///          (Destination Colour * Destination Blend)
        /// </summary>
        Add,

        /// <summary>
        /// The result is the destination subtracted from the source.
        /// Result = (Source Colour * Source Blend) − 
        ///          (Destination Colour * Destination Blend)
        /// </summary>
        Subtract,

        /// <summary>
        /// The result is the source subtracted from the destination.
        /// Result = (Destination Colour * Destination Blend) − 
        ///          (Source Colour * Source Blend)
        /// </summary>
        ReverseSubtract,

        /// <summary>
        /// The result is the maximum of the source and destination.
        /// Result = max( (Source Colour * Source Blend), 
        ///               (Destination Colour * Destination Blend) )
        /// </summary>
        Max,

        /// <summary>
        /// The result is the minimum of the source and destination.
        /// Result = min( (Source Colour * Source Blend), 
        ///               (Destination Colour * Destination Blend) )
        /// </summary>
        Min
    }

    /// <summary>
    /// todo
    /// </summary>
    public enum CullMode
    {
        /// <summary>
        /// todo
        /// </summary>
        None,
        
        /// <summary>
        /// todo
        /// </summary>
        CW,
        
        /// <summary>
        /// todo
        /// </summary>
        CCW,
    }

    /// <summary>
    /// todo
    /// </summary>
    public enum ButtonState
    {
        /// <summary>
        /// todo
        /// </summary>
        Released,

        /// <summary>
        /// todo
        /// </summary>
        Pressed,
    }

    /// <summary>
    /// todo
    /// </summary>
    [Flags]
    enum ClearOptions
    {
        /// <summary>
        /// todo
        /// </summary>
        DepthBuffer = 2,

        /// <summary>
        /// todo
        /// </summary>
        Stencil = 4,
        
        /// <summary>
        /// todo
        /// </summary>
        Target = 1
    }

    /// <summary>
    /// todo
    /// </summary>    
    public enum DeviceOrientation
    {
        /// <summary>
        /// todo
        /// </summary>
        Default,
        
        /// <summary>
        /// todo
        /// </summary>
        Rightside,
        
        /// <summary>
        /// todo
        /// </summary>
        Upsidedown,
        
        /// <summary>
        /// todo
        /// </summary>
        Leftside,
    }

    /// <summary>
    /// todo
    /// </summary>
    public enum PanelType
    {
        /// <summary>
        /// todo
        /// </summary>
        Screen,
        
        /// <summary>
        /// todo
        /// </summary>
        Touch,
        
        /// <summary>
        /// todo
        /// </summary>
        TouchScreen
    }

    /// <summary>
    /// todo
    /// </summary>
    public enum PlayerIndex
    {
        /// <summary>
        /// todo
        /// </summary>
        One,
        
        /// <summary>
        /// todo
        /// </summary>
        Two,
        
        /// <summary>
        /// todo
        /// </summary>
        Three,
        
        /// <summary>
        /// todo
        /// </summary>
        Four,
    }

    /// <summary>
    /// todo
    /// </summary>
    public enum PrimitiveType
    {
        /// <summary>
        /// todo
        /// </summary>
        TriangleList = 0,
        
        /// <summary>
        /// todo
        /// </summary>
        TriangleStrip = 1,
        
        /// <summary>
        /// todo
        /// </summary>
        LineList = 2,
        
        /// <summary>
        /// todo
        /// </summary>
        LineStrip = 3
    }

    /// <summary>
    /// todo
    /// </summary>
    public enum ShaderType
    {
        /// <summary>
        /// todo
        /// </summary>
        Unlit,
        
        /// <summary>
        /// todo
        /// </summary>
        VertexLit,
        
        /// <summary>
        /// todo
        /// </summary>
        PixelLit,
        
        /// <summary>
        /// todo
        /// </summary>
        Toon
    }

    /// <summary>
    /// todo
    /// </summary>
    public enum TouchPhase
    {
        /// <summary>
        /// todo
        /// </summary>
        Invalid = 0,
        
        /// <summary>
        /// todo
        /// </summary>
        JustReleased = 1,
        
        /// <summary>
        /// todo
        /// </summary>
        JustPressed = 2,
        
        /// <summary>
        /// todo
        /// </summary>
        Active = 3,
    }

    /// <summary>
    /// todo
    /// </summary>
    public enum VertexElementFormat
    {
        /// <summary>
        /// todo
        /// </summary>
        Single,
        
        /// <summary>
        /// todo
        /// </summary>
        Vector2,
        
        /// <summary>
        /// todo
        /// </summary>
        Vector3,
        
        /// <summary>
        /// todo
        /// </summary>
        Vector4,
        
        /// <summary>
        /// todo
        /// </summary>
        Colour,
        
        /// <summary>
        /// todo
        /// </summary>
        Byte4,
        
        /// <summary>
        /// todo
        /// </summary>
        Short2,
        
        /// <summary>
        /// todo
        /// </summary>
        Short4,
        
        /// <summary>
        /// todo
        /// </summary>
        NormalisedShort2,
        
        /// <summary>
        /// todo
        /// </summary>
        NormalisedShort4,
        
        /// <summary>
        /// todo
        /// </summary>
        HalfVector2,
        
        /// <summary>
        /// todo
        /// </summary>
        HalfVector4
    }

    /// <summary>
    /// todo
    /// </summary>
    public static class VertexElementFormatHelper
    {
        /// <summary>
        /// todo
        /// </summary>
        public static Type FromEnum(VertexElementFormat format)
        {
            switch(format)
            {
                case VertexElementFormat.Single: 
                    return typeof(Single);
                case VertexElementFormat.Vector2: 
                    return typeof(Vector2);
                case VertexElementFormat.Vector3: 
                    return typeof(Vector3);
                case VertexElementFormat.Vector4: 
                    return typeof(Vector4);
                case VertexElementFormat.Colour: 
                    return typeof(Rgba32);
                case VertexElementFormat.Byte4: 
                    return typeof(Byte4);
                case VertexElementFormat.Short2: 
                    return typeof(Short2);
                case VertexElementFormat.Short4: 
                    return typeof(Short4);
                case VertexElementFormat.NormalisedShort2: 
                    return typeof(NormalisedShort2);
                case VertexElementFormat.NormalisedShort4: 
                    return typeof(NormalisedShort4);
                //case VertexElementFormat.HalfVector2: 
                //    return typeof(Abacus.HalfPrecision.Vector2);
                //case VertexElementFormat.HalfVector4: 
                //    return typeof(Abacus.HalfPrecision.Vector4);
            }

            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    public enum VertexElementUsage
    {
        /// <summary>
        /// todo
        /// </summary>
        Position,
        
        /// <summary>
        /// todo
        /// </summary>
        Colour,
        
        /// <summary>
        /// todo
        /// </summary>
        TextureCoordinate,
        
        /// <summary>
        /// todo
        /// </summary>
        Normal,
        
        /// <summary>
        /// todo
        /// </summary>
        Binormal,
        
        /// <summary>
        /// todo
        /// </summary>
        Tangent,
        
        /// <summary>
        /// todo
        /// </summary>
        BlendIndices,
        
        /// <summary>
        /// todo
        /// </summary>
        BlendWeight,
        
        /// <summary>
        /// todo
        /// </summary>
        Depth,
        
        /// <summary>
        /// todo
        /// </summary>
        Fog,
        
        /// <summary>
        /// todo
        /// </summary>
        PointSize,
        
        /// <summary>
        /// todo
        /// </summary>
        Sample,
        
        /// <summary>
        /// todo
        /// </summary>
        TessellateFactor
    }

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

    #endregion

    #region Types

    /// <summary>
    /// Defines the initial startup settings for the Cor! App
    /// Framework.
    /// </summary>
    public class AppSettings
    {
        readonly String appName;
        readonly LogManagerSettings logSettings;

        Boolean mouseGeneratesTouches = true;
        Boolean fullScreen = false;

        public AppSettings(
            String appName)
        {
            this.appName = appName;
            this.logSettings = new LogManagerSettings(this.appName);
        }

        public String AppName
        {
            get { return appName; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the mouse
        /// input device (if it exists on the platform) should generates
        /// touch events to simulate a touch controller inside the
        /// Cor! framework.
        /// </summary>
        /// <value>
        /// <c>true</c> if the mouse should generates touch
        /// events; otherwise, <c>false</c>.
        /// </value>
        public Boolean MouseGeneratesTouches
        {
            get { return mouseGeneratesTouches; }
            set { mouseGeneratesTouches = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the app
        /// should be run in fullscreen mode inside the Cor! framework.
        /// On platforms where running in a windowed mode is not possible
        /// this variable is ignored.
        /// </summary>
        /// <value>
        /// <c>true</c> if fullscreen; otherwise, <c>false</c>.
        /// </value>
        public Boolean FullScreen
        {
            get { return fullScreen; }
            set { fullScreen = value; }
        }

        public LogManagerSettings LogSettings
        {
            get { return logSettings; }
        }


    }

    /// <summary>
    /// AppTime is a value provided by the Cor! Framework to
    /// the user's app on a frame by frame basis via the app's update
    /// loop.  It provides timing information via properties
    /// relative to both the initisation of the app and the
    /// current frame.
    /// </summary>
    public struct AppTime
    {
        Int64 frameNumber;
        Single dt;
        Single elapsed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Sungiant.Cor.AppTime"/>
        /// struct.  Internal so it can only be instantiated by friend 
        /// assemblies, namely the platform specific Cor! Runtime frameworks.
        /// </summary>
        /// <param name="dt">Delta time.</param>
        /// <param name="elapsed">Total elapsed time.</param>
        /// <param name="frameNumber">Frame number.</param>
        internal AppTime(Single dt, Single elapsed, Int64 frameNumber)
        {
            this.dt = dt;
            this.elapsed = elapsed;
            this.frameNumber = frameNumber;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the current 
        /// <see cref="Sungiant.Cor.AppTime"/>.
        /// </summary>
        public override string ToString ()
        {
            return string.Format (
                "[AppTime: Delta={0}, Elapsed={1}, FrameNumber={2}]", 
                Delta, 
                Elapsed, 
                FrameNumber);
        }
        
        /// <summary>
        /// Gets the time in seconds since between this frame and the last.
        /// </summary>
        /// <value>The delta.</value>
        public Single Delta { get { return dt; } }

        /// <summary>
        /// Gets the total time in seconds since the initilisation of the app.
        /// </summary>
        /// <value>The elapsed time.</value>
        public Single Elapsed { get { return elapsed; } }

        
        /// <summary>
        /// Gets the index of the current frame, the first frame is given
        /// the number zero, programmers count from zero.
        /// </summary>
        /// <value>The frame number.</value>
        public Int64 FrameNumber { get { return frameNumber; } }
    }
    /// <summary>
    /// todo
    /// </summary>
    public class VertexDeclaration
    {
        /// <summary>
        /// todo
        /// </summary>
        VertexElement[] _elements;
        
        /// <summary>
        /// todo
        /// </summary>
        Int32 _vertexStride;

        /// <summary>
        /// todo
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
        /// todo
        /// </summary>
        public Boolean Equals(VertexDeclaration other)
        {
            if( other == null)
                return false;

            return other == this;
        }

        /// <summary>
        /// todo
        /// </summary>
        public override int GetHashCode ()
        {
            int hash = _vertexStride.GetHashCode ();

            foreach (var elm in _elements)
            {
                hash = hash ^ elm.GetHashCode ();
            }

            return hash;
        }

        /// <summary>
        /// todo
        /// </summary>
        public override Boolean Equals (object obj)
        {
            if( obj != null )
            {
                var other = obj as VertexDeclaration;

                if( other != null )
                {
                    return other == this;
                }
            }

            return false;
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Boolean operator != 
            (VertexDeclaration one, VertexDeclaration other)
        {
            return !(one == other);
        }

        /// <summary>
        /// todo
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

            for(int i = 0; i < one._elements.Length; ++i)
            {
                if( one._elements[i] != other._elements[i] )
                    return false;
            }

            return true;
        }

        /// <summary>
        /// todo
        /// </summary>
        public override String ToString()
        {
            string s = string.Empty;

            for(int i = 0; i < _elements.Length; ++i)
            {
                s += _elements[i];

                if( i + 1 < _elements.Length )
                {
                    s += ", "; 
                }

            }

            return string.Format (
                "[VertexDeclaration: Elements={0}, Stride={1}]", 
                s, 
                _vertexStride);
        }

        /// <summary>
        /// todo
        /// </summary>
        public VertexDeclaration (
            Int32 vertexStride, 
            params VertexElement[] elements)
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
        /// todo
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
        /// todo
        /// </summary>
        public VertexElement[] GetVertexElements ()
        {
            return (VertexElement[])this._elements.Clone ();
        }

        /// <summary>
        /// todo
        /// </summary>
        public Int32 VertexStride { get { return this._vertexStride; } }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexElement
    {
        /// <summary>
        /// todo
        /// </summary>
        internal int _offset;
        
        /// <summary>
        /// todo
        /// </summary>
        internal VertexElementFormat _format;
        
        /// <summary>
        /// todo
        /// </summary>
        internal VertexElementUsage _usage;
        
        /// <summary>
        /// todo
        /// </summary>
        internal int _usageIndex;

        /// <summary>
        /// todo
        /// </summary>
        public int Offset
        {
            get { return this._offset; }
            set { this._offset = value; }
        }

        /// <summary>
        /// todo
        /// </summary>
        public VertexElementFormat VertexElementFormat
        {
            get { return this._format; }
            set { this._format = value; }
        }

        /// <summary>
        /// todo
        /// </summary>
        public VertexElementUsage VertexElementUsage
        {
            get{ return this._usage; }
            set { this._usage = value; }
        }

        /// <summary>
        /// todo
        /// </summary>
        public int UsageIndex
        {
            get { return this._usageIndex; }
            set { this._usageIndex = value; }
        }

        /// <summary>
        /// todo
        /// </summary>
        public VertexElement (
            int offset, 
            VertexElementFormat elementFormat, 
            VertexElementUsage elementUsage, 
            int usageIndex)
        {
            this._offset = offset;
            this._usageIndex = usageIndex;
            this._format = elementFormat;
            this._usage = elementUsage;
        }

        /// <summary>
        /// todo
        /// </summary>
        public override String ToString ()
        {
            return string.Format (
                "[Offset:{0} Format:{1}, Usage:{2}, UsageIndex:{3}]",
                this.Offset, 
                this.VertexElementFormat, 
                this.VertexElementUsage, 
                this.UsageIndex
            );
        }

        /// <summary>
        /// todo
        /// </summary>
        public override Int32 GetHashCode ()
        {
            return base.GetHashCode ();
        }

        /// <summary>
        /// todo
        /// </summary>
        public override Boolean Equals (object obj)
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
        /// todo
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
        /// todo
        /// </summary>
        public static Boolean operator != 
            (VertexElement left, VertexElement right)
        {
            return !(left == right);
        }
    }


    public class LogManagerSettings
    {
        readonly HashSet<String> enabledLogChannels;
        readonly List<LogManager.WriteLogDelegate> logWriters;
        Boolean useLogChannels = false;
        readonly String tag;

        internal LogManagerSettings(String tag)
        {
            this.tag = tag;

            this.enabledLogChannels = new HashSet<String>();

            this.logWriters = new List<LogManager.WriteLogDelegate>()
            {
                this.DefaultWriteLogFunction
            };
        }

        void DefaultWriteLogFunction(
            String assembly,
            String tag,
            String channel,
            String type,
            String time,
            String[] lines)
        {
            String startString = String.Format(
                "[{3}][{1}][{0}][{2}] ",
                time,
                type,
                channel,
                tag);

            if (!String.IsNullOrWhiteSpace(assembly))
                startString = String.Format ("[{0}]{1}", assembly, startString);

            String customNewLine = Environment.NewLine + new String(' ', startString.Length);

            String formatedLine = lines
                    .Join (customNewLine);

            String log = startString + formatedLine;

            Console.WriteLine (log);
        }

        public String Tag
        {
            get { return tag; }
        }

        public Boolean UseLogChannels
        {
            get { return useLogChannels; }
            set { useLogChannels = value; }
        }

        public HashSet<String> EnabledLogChannels
        {
            get { return enabledLogChannels; }
        }

        public List<LogManager.WriteLogDelegate> LogWriters
        {
            get { return logWriters; }
        }
    }

    public sealed class LogManager
    {
        public delegate void WriteLogDelegate (
            String assembly,
            String tag,
            String channel,
            String type,
            String time,
            String[] lines);

        public void Debug(String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Debug, assembly, line);
        }

        public void Debug(String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Debug, assembly, line, args);
        }

        public void Debug(String channel, String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Debug, assembly, channel, line, args);
        }

        public void Debug(String channel, String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Debug, assembly, channel, line);
        }

        public void Info(String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Info, assembly, line);
        }

        public void Info(String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Info, assembly, line, args);
        }

        public void Info(String channel, String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Info, assembly, channel, line, args);
        }

        public void Info(String channel, String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Info, assembly, channel, line);
        }

        public void Warning(String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Warning, assembly, line);
        }

        public void Warning(String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Warning, assembly, line, args);
        }

        public void Warning(String channel, String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Warning, assembly, channel, line, args);
        }

        public void Warning(String channel, String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Warning, assembly, channel, line);
        }

        public void Error(String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Error, assembly, line);
        }

        public void Error(String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Error, assembly, line, args);
        }

        public void Error(String channel, String line, params Object[] args)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Error, assembly, channel, line, args);
        }

        public void Error(String channel, String line)
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            WriteLine(LogType.Error, assembly, channel, line);
        }


        enum LogType
        {
            Debug,
            Info,
            Warning,
            Error,
        }

        readonly LogManagerSettings settings;

        internal LogManager(LogManagerSettings settings)
        {
            this.settings = settings;
        }

        // This should be user customisable
        void DoWriteLog(
            String assembly,
            String tag,
            String channel,
            String type,
            String time,
            String[] lines)
        {
            foreach(var writeLogFn in settings.LogWriters)
            {
                writeLogFn(assembly, tag, channel, type, time, lines);
            }
        }

        void WriteLine(LogType type, Assembly callingAssembly, String line)
        {
            WriteLine(type, callingAssembly, "Default", line);
        }


        void WriteLine(LogType type, Assembly callingAssembly, String line, params object[] args)
        {
            WriteLine(type, callingAssembly, "Default", line, args);
        }

        void WriteLine(LogType type, Assembly callingAssembly, String channel, String line, params object[] args)
        {
            String main = String.Format(line, args);

            WriteLine(type, callingAssembly, channel, main);
        }

        void WriteLine(LogType type, Assembly callingAssembly, String channel, String line)
        {
            if (settings.UseLogChannels &&
                !settings.EnabledLogChannels.Contains(channel))
            {
                return;
            }

            if (String.IsNullOrWhiteSpace (line))
            {
                return;
            }

            String assembyStr = Path.GetFileNameWithoutExtension (callingAssembly.Location);
            String typeStr = type.ToString().ToUpper();
            String timeStr = DateTime.Now.ToString("HH:mm:ss.ffffff");
            String[] lines = line.Split(Environment.NewLine.ToCharArray())
                .Where (x => !String.IsNullOrWhiteSpace(x))
                .ToArray();

            DoWriteLog(assembyStr, settings.Tag, channel, typeStr, timeStr, lines);
        }
    }

    #endregion

    #region Input

    /// <summary>
    /// A touch in a single frame definition of a finger on the screen.
    /// </summary>
    public struct Touch
    {
        /// <summary>
        /// todo
        /// </summary>
        Int32 id;

        /// <summary>
        /// The position of a touch ranges between -0.5 and 0.5 in both X and Y
        /// </summary>
        Vector2 normalisedEngineSpacePosition;

        /// <summary>
        /// todo
        /// </summary>
        TouchPhase phase;

        /// <summary>
        /// todo
        /// </summary>
        Int64 frameNumber;

        /// <summary>
        /// todo
        /// </summary>
        Single timestamp;

        /// <summary>
        /// todo
        /// </summary>
        static Touch invalidTouch;

        /// <summary>
        /// todo
        /// </summary>
        public Int32 ID { get { return id; } }

        /// <summary>
        /// todo
        /// </summary>
        public Vector2 Position
        {
            get { return normalisedEngineSpacePosition; }
        }

        /// <summary>
        /// todo
        /// </summary>
        public TouchPhase Phase { get { return phase; } }

        /// <summary>
        /// todo
        /// </summary>
        public Int64 FrameNumber { get { return frameNumber; } }

        /// <summary>
        /// todo
        /// </summary>
        public Single Timestamp { get { return timestamp; } }

        /// <summary>
        /// todo
        /// </summary>
        public Touch(
            Int32 id,
            Vector2 normalisedEngineSpacePosition,
            TouchPhase phase,
            Int64 frame,
            Single timestamp)
        {
            if( normalisedEngineSpacePosition.X > 0.5f || 
                normalisedEngineSpacePosition.X < -0.5f )
            {
                throw new Exception(
                    "Touch has a bad X coordinate: " + 
                    normalisedEngineSpacePosition.X);
            }

            if( normalisedEngineSpacePosition.Y > 0.5f || 
                normalisedEngineSpacePosition.X < -0.5f )
            {
                throw new Exception(
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
        /// todo
        /// </summary>
        static Touch()
        {
            invalidTouch = new Touch(
                -1, 
                Vector2.Zero, 
                TouchPhase.Invalid, 
                -1, 
                0f);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static Touch Invalid { get { return invalidTouch; } }
    }

    /// <summary>
    /// todo
    /// </summary>
    public sealed class TouchCollection
        : IEnumerable<Touch>
    {
        /// <summary>
        /// todo
        /// </summary>
        List<Touch> touchBuffer = new List<Touch>();

        /// <summary>
        /// todo
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// todo
        /// </summary>
        IEnumerator<Touch> IEnumerable<Touch>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// todo
        /// </summary>
        internal void ClearBuffer()
        {
            this.touchBuffer.Clear();
        }

        /// <summary>
        /// todo
        /// </summary>
        internal void RegisterTouch(
            Int32 id,
            Vector2 normalisedEngineSpacePosition,
            TouchPhase phase,
            Int64 frameNum,
            Single timestamp)
        {
            Boolean die = false;

            if( normalisedEngineSpacePosition.X > 0.5f ||
                normalisedEngineSpacePosition.X < -0.5f )
            {
                InternalUtils.Log.Info(
                    "Touch has a bad X coordinate: " +
                    normalisedEngineSpacePosition.X);

                die = true;
            }

            if( normalisedEngineSpacePosition.Y > 0.5f ||
                normalisedEngineSpacePosition.X < -0.5f )
            {
                InternalUtils.Log.Info(
                    "Touch has a bad Y coordinate: " +
                    normalisedEngineSpacePosition.Y);

                die = true;
            }

            if (die)
            {
                InternalUtils.Log.Info("Discarding Bad Touch");
                return;
            }

            var touch = new Touch(
                id,
                normalisedEngineSpacePosition,
                phase,
                frameNum,
                timestamp);

            this.touchBuffer.Add(touch);
        }

        /// <summary>
        /// todo
        /// </summary>
        public IEnumerator<Touch> GetEnumerator()
        {
            return new TouchCollectionEnumerator(this.touchBuffer);
        }

        /// <summary>
        /// todo
        /// </summary>
        public int TouchCount
        {
            get
            {
                return touchBuffer.Count;
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        public Touch GetTouchFromTouchID(int zTouchID)
        {
            foreach (var touch in touchBuffer)
            {
                if (touch.ID == zTouchID) return touch;
            }

            //System.Diagnostics.Debug.WriteLine(
            //    "The touch requested no longer exists.");

            return Touch.Invalid;
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    internal sealed class TouchCollectionEnumerator
        : IEnumerator<Touch>
    {
        /// <summary>
        /// todo
        /// </summary>
        List<Touch> touches;

        /// <summary>
        /// Enumerators are positioned before the first element
        /// until the first MoveNext() call.
        /// </summary>
        Int32 position = -1;

        /// <summary>
        /// todo
        /// </summary>
        internal TouchCollectionEnumerator(List<Touch> touches)
        {
            this.touches = touches;
        }

        /// <summary>
        /// todo
        /// </summary>
        void IDisposable.Dispose()
        {

        }

        /// <summary>
        /// todo
        /// </summary>
        public Boolean MoveNext()
        {
            position++;
            return (position < touches.Count);
        }

        /// <summary>
        /// todo
        /// </summary>
        public void Reset()
        {
            position = -1;
        }

        /// <summary>
        /// todo
        /// </summary>
        Object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// todo
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
                    throw new InvalidOperationException();
                }
            }
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IPsmGamepad
    {
        /// <summary>
        /// todo
        /// </summary>
        IPsmGamepadButtons Buttons { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        IPsmGamepadDPad DPad { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        IPsmGamepadThumbsticks Thumbsticks { get; }
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IPsmGamepadButtons
    {   
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Triangle { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Square { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Circle { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Cross { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Start { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Select { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState LeftShoulder { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState RightShoulder { get; }
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IPsmGamepadDPad
    {
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Down { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Left { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Right { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Up { get; }
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IPsmGamepadThumbsticks
    {
        /// <summary>
        /// todo
        /// </summary>
        Vector2 Left { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        Vector2 Right { get; }
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IXbox360Gamepad
    {        
        /// <summary>
        /// todo
        /// </summary>
        IXbox360GamepadButtons Buttons { get; }

        /// <summary>
        /// todo
        /// </summary>
        IXbox360GamepadDPad DPad { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        IXbox360GamepadThumbsticks Thumbsticks { get; }

        /// <summary>
        /// todo
        /// </summary>
        IXbox360GamepadTriggers Triggers { get; }
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IXbox360GamepadButtons
    {
        /// <summary>
        /// todo
        /// </summary>
        ButtonState A { get; }

        /// <summary>
        /// todo
        /// </summary>
        ButtonState B { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Back { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState LeftShoulder { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState LeftStick { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState RightShoulder { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState RightStick { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Start { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState X { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Y { get; }
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IXbox360GamepadDPad
    {
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Down { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Left { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Right { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Up { get; }
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IXbox360GamepadThumbsticks
    {
        /// <summary>
        /// todo
        /// </summary>
        Vector2 Left { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        Vector2 Right { get; }
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IXbox360GamepadTriggers
    {
        /// <summary>
        /// todo
        /// </summary>
        Single Left { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        Single Right { get; }
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IMultiTouchController
    {
        /// <summary>
        /// todo
        /// </summary>
        IPanelSpecification PanelSpecification { get; }

        /// <summary>
        /// todo
        /// </summary>
        TouchCollection TouchCollection { get; }
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IGenericGamepad
    {
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Down { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Left { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Right { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Up { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState North { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState South { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState East { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState West { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Option { get; }
        
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Pause { get; }
    }

    /// <summary>
    /// todo
    /// </summary>
    public interface IMouse
    {
        /// <summary>
        /// todo
        /// </summary>
        ButtonState Left { get; }

        /// <summary>
        /// todo
        /// </summary>
        ButtonState Middle { get; }

        /// <summary>
        /// todo
        /// </summary>
        ButtonState Right { get; }

        /// <summary>
        /// todo
        /// </summary>
        Int32 ScrollWheelValue { get; }

        /// <summary>
        /// todo
        /// </summary>
        Int32 X { get; }

        /// <summary>
        /// todo
        /// </summary>
        Int32 Y { get; }
    }

    public interface IKeyboard
    {
        FunctionalKey[] GetPressedFunctionalKey ();
        Boolean IsFunctionalKeyDown (FunctionalKey key);
        Boolean IsFunctionalKeyUp (FunctionalKey key);
        KeyState this [FunctionalKey key] { get; }

        Char[] GetPressedCharacterKeys();
        Boolean IsCharacterKeyDown (Char key);
        Boolean IsCharacterKeyUp (Char key);
        KeyState this [Char key] { get; }
    }

    #endregion

    #region Vertices

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPosition
        : IVertexType
    {
        /// <summary>
        /// todo
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// todo
        /// </summary>
        public VertexPosition(Vector3 position)
        {
            this.Position = position;
        }

        /// <summary>
        /// todo
        /// </summary>
        static VertexPosition()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement(
                    0, 
                    VertexElementFormat.Vector3, 
                    VertexElementUsage.Position, 
                    0)
                );

            _default = new VertexPosition(Vector3.Zero);
        }

        /// <summary>
        /// todo
        /// </summary>
        readonly static VertexPosition _default;
        
        /// <summary>
        /// todo
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;

        /// <summary>
        /// todo
        /// </summary>
        public static IVertexType Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return _vertexDeclaration;
            }
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionColour
        : IVertexType
    {
        /// <summary>
        /// todo
        /// </summary>
        public Vector3 Position;
        
        /// <summary>
        /// todo
        /// </summary>
        public Rgba32 Colour;

        /// <summary>
        /// todo
        /// </summary>
        public VertexPositionColour(
            Vector3 position, 
            Rgba32 color)
        {
            this.Position = position;
            this.Colour = color;
        }

        /// <summary>
        /// todo
        /// </summary>
        static VertexPositionColour()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement(
                    0, 
                    VertexElementFormat.Vector3, 
                    VertexElementUsage.Position, 
                    0),
                new VertexElement(
                    12, 
                    VertexElementFormat.Colour, 
                    VertexElementUsage.Colour, 
                    0)
                );

            _default = new VertexPositionColour(
                Vector3.Zero, 
                Rgba32.Magenta);
        }

        /// <summary>
        /// todo
        /// </summary>
        readonly static VertexPositionColour _default;
        
        /// <summary>
        /// todo
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;

        /// <summary>
        /// todo
        /// </summary>
        public static IVertexType Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return _vertexDeclaration;
            }
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionNormal
        : IVertexType
    {
        /// <summary>
        /// todo
        /// </summary>
        public Vector3 Position;
        
        /// <summary>
        /// todo
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// todo
        /// </summary>
        public VertexPositionNormal (
            Vector3 position, 
            Vector3 normal)
        {
            this.Position = position;
            this.Normal = normal;
        }

        /// <summary>
        /// todo
        /// </summary>
        readonly static VertexPositionNormal _default;
        
        /// <summary>
        /// todo
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;

        /// <summary>
        /// todo
        /// </summary>
        static VertexPositionNormal ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0, 
                    VertexElementFormat.Vector3, 
                    VertexElementUsage.Position, 
                    0),
                new VertexElement (
                    sizeof(Single) * 3, 
                    VertexElementFormat.Vector3, 
                    VertexElementUsage.Normal, 
                    0)
                );

            _default = new VertexPositionNormal (
                Vector3.Zero, 
                Vector3.Zero);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static IVertexType Default
        {
            get
            {
                return _default;
            }
        }
        
        /// <summary>
        /// todo
        /// </summary>
        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return _vertexDeclaration; 
            }
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionNormalColour
        : IVertexType
    {
        /// <summary>
        /// todo
        /// </summary>
        public Vector3 Position;
        
        /// <summary>
        /// todo
        /// </summary>
        public Vector3 Normal;
        
        /// <summary>
        /// todo
        /// </summary>
        public Rgba32 Colour;

        /// <summary>
        /// todo
        /// </summary>
        public VertexPositionNormalColour(
            Vector3 position, 
            Vector3 normal, 
            Rgba32 color)
        {
            this.Position = position;
            this.Normal = normal;
            this.Colour = color;
        }

        /// <summary>
        /// todo
        /// </summary>
        static VertexPositionNormalColour()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement(
                    0, 
                    VertexElementFormat.Vector3, 
                    VertexElementUsage.Position, 
                    0),
                new VertexElement(
                    12, 
                    VertexElementFormat.Vector3, 
                    VertexElementUsage.Normal, 
                    0),
                new VertexElement(
                    24, 
                    VertexElementFormat.Colour, 
                    VertexElementUsage.Colour, 
                    0)
                );

            _default = new VertexPositionNormalColour(
                Vector3.Zero, 
                Vector3.Zero, 
                Rgba32.White);
        }

        /// <summary>
        /// todo
        /// </summary>
        readonly static VertexPositionNormalColour _default;
        
        /// <summary>
        /// todo
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;

        /// <summary>
        /// todo
        /// </summary>
        public static IVertexType Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return _vertexDeclaration;
            }
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionNormalTexture
        : IVertexType
    {
        /// <summary>
        /// todo
        /// </summary>
        public Vector3 Position;
        
        /// <summary>
        /// todo
        /// </summary>
        public Vector3 Normal;
        
        /// <summary>
        /// todo
        /// </summary>
        public Vector2 UV;

        /// <summary>
        /// todo
        /// </summary>
        public VertexPositionNormalTexture (
            Vector3 position, 
            Vector3 normal, 
            Vector2 uv)
        {
            this.Position = position;
            this.Normal = normal;
            this.UV = uv;
        }

        /// <summary>
        /// todo
        /// </summary>
        readonly static VertexPositionNormalTexture _default;
        
        /// <summary>
        /// todo
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;

        /// <summary>
        /// todo
        /// </summary>
        static VertexPositionNormalTexture ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0, 
                    VertexElementFormat.Vector3, 
                    VertexElementUsage.Position, 
                    0),
                new VertexElement (
                    12, 
                    VertexElementFormat.Vector3, 
                    VertexElementUsage.Normal, 
                    0),
                new VertexElement (
                    24, 
                    VertexElementFormat.Vector2, 
                    VertexElementUsage.TextureCoordinate, 
                    0)
                );

            _default = new VertexPositionNormalTexture (
                Vector3.Zero, 
                Vector3.Zero, 
                Vector2.Zero);
        }

        /// <summary>
        /// todo
        /// </summary>
        public static IVertexType Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        public VertexDeclaration VertexDeclaration
        { 
            get
            { 
                return _vertexDeclaration; 
            } 
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionNormalTextureColour
        : IVertexType
    {
        /// <summary>
        /// todo
        /// </summary>
        public Vector3 Position;
        
        /// <summary>
        /// todo
        /// </summary>
        public Vector3 Normal;
        
        /// <summary>
        /// todo
        /// </summary>
        public Vector2 UV;
        
        /// <summary>
        /// todo
        /// </summary>
        public Rgba32 Colour;

        /// <summary>
        /// todo
        /// </summary>
        public VertexPositionNormalTextureColour (
            Vector3 position, 
            Vector3 normal, 
            Vector2 uv, 
            Rgba32 color)
        {
            this.Position = position;
            this.Normal = normal;
            this.UV = uv;
            this.Colour = color;
        }

        /// <summary>
        /// todo
        /// </summary>
        static VertexPositionNormalTextureColour ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0, 
                    VertexElementFormat.Vector3, 
                    VertexElementUsage.Position, 
                    0),
                new VertexElement (
                    12, 
                    VertexElementFormat.Vector3, 
                    VertexElementUsage.Normal, 
                    0),
                new VertexElement (
                    24, 
                    VertexElementFormat.Vector2, 
                    VertexElementUsage.TextureCoordinate, 
                    0),
                new VertexElement (
                    32, 
                    VertexElementFormat.Colour, 
                    VertexElementUsage.Colour, 
                    0)
                );

            _default = new VertexPositionNormalTextureColour (
                Vector3.Zero, 
                Vector3.Zero, 
                Vector2.Zero, 
                Rgba32.White);
        }

        /// <summary>
        /// todo
        /// </summary>
        readonly static VertexPositionNormalTextureColour _default;
        
        /// <summary>
        /// todo
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;

        /// <summary>
        /// todo
        /// </summary>
        public static IVertexType Default
        {
            get
            {
                return _default;
            }
        }

        /// <summary>
        /// todo
        /// </summary>
        public VertexDeclaration VertexDeclaration
        { 
            get
            { 
                return _vertexDeclaration; 
            } 
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionTexture
        : IVertexType
    {
        /// <summary>
        /// todo
        /// </summary>
        public Vector3 Position;
        
        /// <summary>
        /// todo
        /// </summary>
        public Vector2 UV;
        
        /// <summary>
        /// todo
        /// </summary>
        public VertexPositionTexture (
            Vector3 position, 
            Vector2 uv)
        {
            this.Position = position;
            this.UV = uv;
        }
        
        /// <summary>
        /// todo
        /// </summary>
        static VertexPositionTexture ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0, 
                    VertexElementFormat.Vector3, 
                    VertexElementUsage.Position, 
                    0),
                new VertexElement (
                    12, 
                    VertexElementFormat.Vector2, 
                    VertexElementUsage.TextureCoordinate, 
                    0)
                );

            _default = new VertexPositionTexture (
                Vector3.Zero, 
                Vector2.Zero);
        }
        
        /// <summary>
        /// todo
        /// </summary>
        readonly static VertexPositionTexture _default;
        
        /// <summary>
        /// todo
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;
        
        /// <summary>
        /// todo
        /// </summary>
        public static IVertexType Default
        {
            get
            {
                return _default;
            }
        }
        
        /// <summary>
        /// todo
        /// </summary>
        public VertexDeclaration VertexDeclaration
        { 
            get
            { 
                return _vertexDeclaration; 
            } 
        }
    }

    /// <summary>
    /// todo
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionTextureColour
        : IVertexType
    {
        /// <summary>
        /// todo
        /// </summary>
        public Vector3 Position;
        
        /// <summary>
        /// todo
        /// </summary>
        public Vector2 UV;
        
        /// <summary>
        /// todo
        /// </summary>
        public Rgba32 Colour;
        
        /// <summary>
        /// todo
        /// </summary>
        public VertexPositionTextureColour (
            Vector3 position, 
            Vector2 uv, 
            Rgba32 color)
        {
            this.Position = position;
            this.UV = uv;
            this.Colour = color;
        }
        
        /// <summary>
        /// todo
        /// </summary>
        static VertexPositionTextureColour ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0, 
                    VertexElementFormat.Vector3, 
                    VertexElementUsage.Position, 
                    0),
                new VertexElement (
                    12, 
                    VertexElementFormat.Vector2, 
                    VertexElementUsage.TextureCoordinate, 
                    0),
                new VertexElement (
                    20, 
                    VertexElementFormat.Colour, 
                    VertexElementUsage.Colour, 
                    0)
                );

            _default = new VertexPositionTextureColour (
                Vector3.Zero, 
                Vector2.Zero, 
                Rgba32.White);
        }
        
        /// <summary>
        /// todo
        /// </summary>
        readonly static VertexPositionTextureColour _default;
        
        /// <summary>
        /// todo
        /// </summary>
        readonly static VertexDeclaration _vertexDeclaration;
        
        /// <summary>
        /// todo
        /// </summary>
        public static IVertexType Default
        {
            get
            {
                return _default;
            }
        }
        
        /// <summary>
        /// todo
        /// </summary>
        public VertexDeclaration VertexDeclaration
        { 
            get
            { 
                return _vertexDeclaration; 
            } 
        }
    }

    #endregion

    #region Resources

    /// <summary>
    /// todo
    /// </summary>
    public abstract class AudioClip
        : IResource
    {
        /// <summary>
        /// todo
        /// </summary>
        public abstract void Play ();

        /// <summary>
        /// todo
        /// </summary>
        public abstract void Stop ();

        /// <summary>
        /// todo
        /// </summary>
        public abstract Boolean IsPlaying { get; }
    }
    /// <summary>
    /// Each model part represents a piece of geometry that uses one
    /// single effect. Multiple parts are needed for models that use
    /// more than one effect.
    /// </summary>
    public abstract class Mesh
        : IResource
    {
        /// <summary>
        /// todo
        /// </summary>
        public Int32 TriangleCount;

        /// <summary>
        /// todo
        /// </summary>
        public Int32 VertexCount;

        /// <summary>
        /// todo
        /// </summary>
        public abstract VertexDeclaration VertDecl { get; }

        /// <summary>
        /// todo
        /// </summary>
        public IGeometryBuffer GeomBuffer;
    }

    /// <summary>
    /// todo
    /// </summary>
    public abstract class Texture2D
        : IResource
    {
        /// <summary>
        /// todo
        /// </summary>
        public abstract Int32 Width { get; } 

        /// <summary>
        /// todo
        /// </summary>
        public abstract Int32 Height { get; } 
    }

    #endregion

    #region Internal

    /// <summary>
    /// todo
    /// </summary>
    public static class PrimitiveHelper
    {
        /// <summary>
        /// todo
        /// </summary>
        public static Int32 NumVertsIn(PrimitiveType type)
        {
            switch(type)
            {
                case PrimitiveType.TriangleList: 
                    return 3;
                case PrimitiveType.TriangleStrip: 
                    throw new NotImplementedException();
                case PrimitiveType.LineList: 
                    return 2;
                case PrimitiveType.LineStrip: 
                    throw new NotImplementedException();
                default: 
                    throw new NotImplementedException();   
            }
        }
    }

    /// <summary>
    /// todo
    /// </summary>    
    internal static class VertexElementValidator
    {
        /// <summary>
        /// todo
        /// </summary>
        internal static Int32 GetTypeSize (VertexElementFormat format)
        {
            switch (format) {
            case VertexElementFormat.Single:
                return 4;

            case VertexElementFormat.Vector2:
                return 8;

            case VertexElementFormat.Vector3:
                return 12;

            case VertexElementFormat.Vector4:
                return 0x10;

            case VertexElementFormat.Colour:
                return 4;

            case VertexElementFormat.Byte4:
                return 4;

            case VertexElementFormat.Short2:
                return 4;

            case VertexElementFormat.Short4:
                return 8;

            case VertexElementFormat.NormalisedShort2:
                return 4;

            case VertexElementFormat.NormalisedShort4:
                return 8;

            case VertexElementFormat.HalfVector2:
                return 4;

            case VertexElementFormat.HalfVector4:
                return 8;
            }
            return 0;
        }

        /// <summary>
        /// todo
        /// </summary>
        internal static int GetVertexStride (VertexElement[] elements)
        {
            Int32 num2 = 0;

            for (Int32 i = 0; i < elements.Length; i++)
            {
                Int32 num3 = elements [i].Offset + 
                    GetTypeSize (elements [i].VertexElementFormat);
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
        internal static void Validate(
            IShader effect, 
            VertexDeclaration vertexDeclaration)
        {
            throw new NotImplementedException ();
        }

        /// <summary>
        /// todo
        /// </summary>
        internal static void Validate (
            int vertexStride, 
            VertexElement[] elements)
        {
            if (vertexStride <= 0)
            {
                throw new ArgumentOutOfRangeException ("vertexStride");
            }
            
            if ((vertexStride & 3) != 0)
            {
                throw new ArgumentException (
                    "VertexElementOffsetNotMultipleFour");
            }
            
            Int32[] numArray = new Int32[vertexStride];
            
            for (Int32 i = 0; i < vertexStride; i++)
            {
                numArray [i] = -1;
            }
            
            for (Int32 j = 0; j < elements.Length; j++)
            {
                Int32 offset = elements [j].Offset;
                
                Int32 typeSize = GetTypeSize (elements [j].VertexElementFormat);
                
                if (
                    (elements [j].VertexElementUsage < 
                     VertexElementUsage.Position ) || 
                    (elements [j].VertexElementUsage > 
                     VertexElementUsage.TessellateFactor)
                    ) 
                {
                    throw new ArgumentException (
                        String.Format (
                            "FrameworkResources.VertexElementBadUsage"));
                }
                
                if ((offset < 0) || ((offset + typeSize) > vertexStride))
                {
                    throw new ArgumentException (
                        String.Format (
                            "FrameworkResources.VertexElementOutsideStride"));
                }
                
                if ((offset & 3) != 0)
                {
                    throw new ArgumentException (
                        "VertexElementOffsetNotMultipleFour");
                }
                
                for (Int32 k = 0; k < j; k++)
                {
                    if (
                        (elements [j].VertexElementUsage == 
                         elements [k].VertexElementUsage) && 
                        (elements [j].UsageIndex == 
                         elements [k].UsageIndex))
                    {
                        throw new ArgumentException (
                            String.Format ("DuplicateVertexElement"));
                    }
                }

                for (Int32 m = offset; m < (offset + typeSize); m++)
                {
                    if (numArray [m] >= 0)
                    {
                        throw new ArgumentException (
                            String.Format ("VertexElementsOverlap"));
                    }

                    numArray [m] = j;
                }
            }
        }
    }

    static class InternalUtils
    {
        static readonly LogManager log;

        static InternalUtils()
        {
            var settings = new LogManagerSettings("INTERNAL");
            log = new LogManager(settings);
        }

        public static LogManager Log
        {
            get { return log; }
        }
    }

    #endregion

    #region Extentions

    public static class ListExtensions
    {
        public static string Join<T>(this IEnumerable<T> values, string seperator)
        {
            var sb = new StringBuilder();
            foreach (var value in values)
            {
                if (sb.Length > 0)
                    sb.Append(seperator);
                sb.Append(value);
            }
            return sb.ToString();
        }
    }

    #endregion

    #region Assets

    public abstract class AssetTypeSerialiser
    {
        readonly Type targetType;

        public Type TargetType
        {
            get { return this.targetType; }
        }

        protected AssetTypeSerialiser(Type targetType)
        {
            this.targetType = targetType;
        }

        protected internal abstract Object BaseRead (AssetBinaryReader abr);

        protected internal abstract void BaseWrite (AssetBinaryWriter abw, Object obj);
    }

    public abstract class AssetTypeSerialiser<T>
        : AssetTypeSerialiser
    {
        protected AssetTypeSerialiser()
            : base(typeof(T))
        {

        }
        
        protected internal virtual void Initialise (AssetTypeSerialiserManager manager) {}

        protected internal override Object BaseRead (AssetBinaryReader abr)
        {
            return this.Read (abr);
        }

        protected internal override void BaseWrite (AssetBinaryWriter abw, Object obj)
        {
            this.Write(abw, (T) obj);
        }

        protected internal abstract T Read (AssetBinaryReader abr);

        protected internal abstract void Write (AssetBinaryWriter abw, T obj);
    }

    public sealed class AssetManager
        : IDisposable
    {
        internal AssetManager(IGraphicsManager Graphics)
        {

        }

        public T Load<T>(String assetName)
        {
            return default(T);
        }

        public void Unload()
        {
            
        }

        public void Dispose()
        {
            
        }
    }

    public sealed class AssetTypeSerialiserManager
    {
        Dictionary<Type, AssetTypeSerialiser> assetTypeSerialisers;

        internal AssetTypeSerialiserManager()
        {
            assetTypeSerialisers = new Dictionary<Type, AssetTypeSerialiser>();
        }

        public AssetTypeSerialiser<T> GetTypeSerialiser<T>()
        {
            throw new NotImplementedException();
            //return assetTypeSerialisers[typeof(T)] as AssetTypeSerialiser<T>;
        }

        public AssetTypeSerialiser GetTypeSerialiser(Type type)
        {
            throw new NotImplementedException();
            //return assetTypeSerialisers[type];
        }

        public AssetTypeSerialiser GetTypeSerialiserFromId(Int32 id)
        {
            throw new NotImplementedException();
        }
    }

    public class AssetBinaryReader
        : BinaryReader
    {
        internal AssetBinaryReader(Stream stream)
            : base(stream)
        {
        }

        public Int32 Read7BitEncodedInt32()
        {
            Int32 result = 0;
            Int32 bitsRead = 0;
            Int32 value;

            do
            {
                value = ReadByte();
                result |= (value & 0x7f) << bitsRead;
                bitsRead += 7;
            }
            while ((value & 0x80) != 0);

            return result;
        }
    }

    public class AssetBinaryWriter
        : BinaryWriter
    {
        internal AssetBinaryWriter(Stream stream)
            : base(stream)
        {
        }
    }

    #endregion

    #region AssetTypeSerialisers

    class Int64Serialiser 
        : AssetTypeSerialiser<Int64>
    {
        internal Int64Serialiser () {}

        protected internal override Int64 Read(AssetBinaryReader abr)
        {
            return abr.ReadInt64 ();
        }

        protected internal override void Write(AssetBinaryWriter abw, Int64 obj)
        {
            abw.Write(obj);
        }
    }

    class BooleanSerialiser 
        : AssetTypeSerialiser<Boolean>
    {
        internal BooleanSerialiser () {}

        protected internal override Boolean Read(AssetBinaryReader abr)
        {
            return abr.ReadBoolean ();
        }

        protected internal override void Write(AssetBinaryWriter abw, Boolean obj)
        {
            abw.Write(obj);
        }
    }

    class ByteSerialiser 
        : AssetTypeSerialiser<Byte>
    {
        internal ByteSerialiser () {}

        protected internal override Byte Read(AssetBinaryReader abr)
        {
            return abr.ReadByte ();
        }

        protected internal override void Write(AssetBinaryWriter abw, Byte obj)
        {
            abw.Write(obj);
        }
    }

    class CharSerialiser 
        : AssetTypeSerialiser<Char>
    {
        internal CharSerialiser () {}

        protected internal override Char Read(AssetBinaryReader abr)
        {
            return abr.ReadChar ();
        }

        protected internal override void Write(AssetBinaryWriter abw, Char obj)
        {
            abw.Write(obj);
        }
    }

    class DecimalSerialiser
        : AssetTypeSerialiser<Decimal>
    {
        internal DecimalSerialiser () {}

        protected internal override Decimal Read(AssetBinaryReader abr)
        {
            return abr.ReadDecimal ();
        }

        protected internal override void Write(AssetBinaryWriter abw, Decimal obj)
        {
            abw.Write(obj);
        }
    }

    class DoubleSerialiser 
        : AssetTypeSerialiser<Double>
    {
        internal DoubleSerialiser () {}

        protected internal override Double Read(AssetBinaryReader abr)
        {
            return abr.ReadDouble ();
        }

        protected internal override void Write(AssetBinaryWriter abw, Double obj)
        {
            abw.Write(obj);
        }
    }

    class Int16Serialiser 
        : AssetTypeSerialiser<Int16>
    {
        internal Int16Serialiser () {}

        protected internal override Int16 Read(AssetBinaryReader abr)
        {
            return abr.ReadInt16 ();
        }

        protected internal override void Write(AssetBinaryWriter abw, Int16 obj)
        {
            abw.Write(obj);
        }
    }

    class Int32Serialiser 
        : AssetTypeSerialiser<Int32>
    {
        internal Int32Serialiser () {}

        protected internal override Int32 Read(AssetBinaryReader abr)
        {
            return abr.ReadInt32 ();
        }

        protected internal override void Write(AssetBinaryWriter abw, Int32 obj)
        {
            abw.Write(obj);
        }
    }

    class SByteSerialiser 
        : AssetTypeSerialiser<SByte>
    {
        internal SByteSerialiser () {}

        protected internal override SByte Read(AssetBinaryReader abr)
        {
            return abr.ReadSByte ();
        }

        protected internal override void Write(AssetBinaryWriter abw, SByte obj)
        {
            abw.Write(obj);
        }
    }

    class SingleSerialiser 
        : AssetTypeSerialiser<Single>
    {
        internal SingleSerialiser () {}

        protected internal override Single Read(AssetBinaryReader abr)
        {
            return abr.ReadSingle ();
        }

        protected internal override void Write(AssetBinaryWriter abw, Single obj)
        {
            abw.Write(obj);
        }
    }

    class StringSerialiser 
        : AssetTypeSerialiser<String>
    {
        internal StringSerialiser () {}

        protected internal override String Read(AssetBinaryReader abr)
        {
            return abr.ReadString ();
        }

        protected internal override void Write(AssetBinaryWriter abw, String obj)
        {
            abw.Write(obj);
        }
    }

    class TimeSpanSerialiser 
        : AssetTypeSerialiser<TimeSpan>
    {
        internal TimeSpanSerialiser () {}

        protected internal override TimeSpan Read(AssetBinaryReader abr)
        {
            Int64 ticks = abr.ReadInt64 ();
            
            return new TimeSpan(ticks);
        }

        protected internal override void Write(AssetBinaryWriter abw, TimeSpan obj)
        {
            abw.Write(obj.Ticks);
        }
    }

    class UInt16Serialiser 
        : AssetTypeSerialiser<UInt16>
    {
        internal UInt16Serialiser () {}

        protected internal override UInt16 Read(AssetBinaryReader abr)
        {
            return abr.ReadUInt16 ();
        }

        protected internal override void Write(AssetBinaryWriter abw, UInt16 obj)
        {
            abw.Write(obj);
        }
    }

    class UInt32Serialiser 
        : AssetTypeSerialiser<UInt32>
    {
        internal UInt32Serialiser () {}

        protected internal override UInt32 Read(AssetBinaryReader abr)
        {
            return abr.ReadUInt32 ();
        }

        protected internal override void Write(AssetBinaryWriter abw, UInt32 obj)
        {
            abw.Write(obj);
        }
    }

    class UInt64Serialiser 
        : AssetTypeSerialiser<UInt64>
    {
        internal UInt64Serialiser () {}

        protected internal override UInt64 Read(AssetBinaryReader cbr)
        {
            return cbr.ReadUInt64 ();
        }

        protected internal override void Write(AssetBinaryWriter abw, UInt64 obj)
        {
            abw.Write(obj);
        }
    }

    // test reading lists containing different items from a chain of inheritance.
    class ListSerialiser<T>
        : AssetTypeSerialiser<List<T>>
    {
        AssetTypeSerialiser<T> elementSerialiser;

        AssetTypeSerialiserManager manager;

        internal ListSerialiser () {}

        protected internal override void Initialise (AssetTypeSerialiserManager manager)
        {
            this.manager = manager;
            elementSerialiser = manager.GetTypeSerialiser<T> ();
        }

        protected internal override List<T> Read(AssetBinaryReader abr)
        {
            UInt32 count = abr.ReadUInt32 ();

            var list = new List<T> ();

            Type objectType = typeof(T);
            
            if (objectType.IsValueType)
            {
                for (Int32 i = 0; i < count; ++i)
                {
                    // no inheritance for structs
                    T item = elementSerialiser.Read (abr);
                    list.Add(item);
                }
            }
            else
            {
                for (Int32 i = 0; i < count; ++i)
                {
                    // Get the id of the type reader for this element,
                    // as this element might not be of Type T, it might be
                    // polymorphic.
                    Int32 objectTypeSerialiserId = abr.Read7BitEncodedInt32();

                    if (objectTypeSerialiserId > 0)
                    {
                        // Locate the correct serialiser for this element.
                        AssetTypeSerialiser virtualElementSerialiser = 
                            this.manager.GetTypeSerialiserFromId(objectTypeSerialiserId);
                        
                        //
                        Object item = virtualElementSerialiser.BaseRead (abr);
                        
                        // add to list then move on
                        list.Add((T)item);
                    }
                    else
                    {
                        // the element is null
                        list.Add(default(T));
                    }
                }
            }

            return list;
        }

        protected internal override void Write(AssetBinaryWriter abw, List<T> obj)
        {
            throw new NotImplementedException();
        }
    }

    class NullableSerialiser<T>
        : AssetTypeSerialiser<T?>
    where T 
        : struct
    {
        AssetTypeSerialiser<T> valueSerialiser;

        internal NullableSerialiser () {}

        protected internal override void Initialise (AssetTypeSerialiserManager manager)
        {
            valueSerialiser = manager.GetTypeSerialiser<T>();
        }
        
        protected internal override T? Read(AssetBinaryReader abr)
        {
            if(abr.ReadBoolean())
            {
                return valueSerialiser.Read (abr);
            }
            
            return null;
        }

        protected internal override void Write(AssetBinaryWriter abw, T? obj)
        {
            if( obj.HasValue )
            {
                abw.Write(true);
                valueSerialiser.Write(abw, obj.Value);
            }
            else
            {
                abw.Write(false);
            }
        }
    }

    // EnumSerialiser<T>
    // test
    // enum Foo : long { One, Two };
    // enum Bar : byte { x = 255 };
    class EnumSerialiser<T>
        : AssetTypeSerialiser<T>
    {
        AssetTypeSerialiser underlyingTypeSerialiser;

        internal EnumSerialiser () {}

        protected internal override void Initialise(AssetTypeSerialiserManager manager)
        {
            // can we not get this at compile time? -_-
            // lets stick with Int32 for now
            Type readerType = Enum.GetUnderlyingType(typeof(T));

            underlyingTypeSerialiser = manager.GetTypeSerialiser(readerType);
        }
        
        protected internal override T Read(AssetBinaryReader abr)
        {
            Object underlyingValue = underlyingTypeSerialiser.BaseRead(abr);

            return (T) underlyingValue;
        }

        protected internal override void Write(AssetBinaryWriter abw, T obj)
        {
            underlyingTypeSerialiser.BaseWrite(abw, obj);
        }
    }

    class ArraySerialiser<T>
        : AssetTypeSerialiser<T[]>
    {
        AssetTypeSerialiser<T> elementSerialiser;

        AssetTypeSerialiserManager manager;

        internal ArraySerialiser () {}

        protected internal override void Initialise(AssetTypeSerialiserManager manager)
        {
            this.manager = manager;
            elementSerialiser = manager.GetTypeSerialiser<T>();
        }

        protected internal override T[] Read (AssetBinaryReader abr)
        {
            UInt32 count = abr.ReadUInt32 ();

            var array = new T[count];
            
            Type objectType = typeof(T);
            
            if (objectType.IsValueType)
            {
                for (UInt32 i = 0; i < count; ++i)
                {
                    array [i] = elementSerialiser.Read (abr);
                }
            }
            else
            {
                for (UInt32 i = 0; i < count; ++i)
                {
                    // Get the id of the type reader for this element,
                    // as this element might not be of Type T, it might be
                    // polymorphic.
                    Int32 objectTypeSerialiserId = abr.Read7BitEncodedInt32 ();
                    
                    if (objectTypeSerialiserId > 0)
                    {
                        // Locate the correct serialiser for this element.
                        AssetTypeSerialiser virtualElementSerialiser = 
                            this.manager.GetTypeSerialiserFromId (objectTypeSerialiserId);
                    
                        //
                        Object item = virtualElementSerialiser.BaseRead (abr);
                    
                        // add to array then move on
                        array [i] = (T)item;
                    }
                    else
                    {
                        // the element is null
                        array [i] = default(T);
                    }
                }
            }
            
            return array;
        }

        protected internal override void Write(AssetBinaryWriter abw, T[] obj)
        {
            throw new NotSupportedException();
        }
    }

    class DictionarySerialiser<TKey, TValue> 
        : AssetTypeSerialiser<Dictionary<TKey, TValue>>
    {
        AssetTypeSerialiser<TKey> keySerialiser;
        AssetTypeSerialiser<TValue> valueSerialiser;

        AssetTypeSerialiserManager manager;
        
        internal DictionarySerialiser () {}

        protected internal override void Initialise(AssetTypeSerialiserManager manager)
        {
            this.manager = manager;
            keySerialiser = manager.GetTypeSerialiser<TKey>();
            valueSerialiser = manager.GetTypeSerialiser<TValue>();
        }

        protected internal override Dictionary<TKey, TValue> Read(AssetBinaryReader abr)
        {
            UInt32 count = abr.ReadUInt32();

            var dictionary = new Dictionary<TKey, TValue>();

            Type keyType = typeof(TKey);
            Type valueType = typeof(TValue);

            for (UInt32 i = 0; i < count; ++i)
            {
                TKey key;
                TValue value;

                if (keyType.IsValueType)
                {
                    key = keySerialiser.Read(abr);
                }
                else
                {
                    // Get the id of the type reader for this element,
                    // as this element might not be of Type T, it might be
                    // polymorphic.
                    Int32 keyTypeSerialiserId = abr.Read7BitEncodedInt32 ();
                    
                    if (keyTypeSerialiserId > 0)
                    {
                        // Locate the correct serialiser for this element.
                        AssetTypeSerialiser virtualElementSerialiser = 
                            this.manager.GetTypeSerialiserFromId (keyTypeSerialiserId);
                    
                        //
                        Object item = virtualElementSerialiser.BaseRead (abr);
                    
                        // add to array then move on
                        key = (TKey)item;
                    }
                    else
                    {
                        // the element is null
                        key = default(TKey);
                    }
                }

                if (valueType.IsValueType)
                {
                    value = valueSerialiser.Read(abr);
                }
                else
                {
                    // Get the id of the type reader for this element,
                    // as this element might not be of Type T, it might be
                    // polymorphic.
                    Int32 valueTypeSerialiserId = abr.Read7BitEncodedInt32 ();
                    
                    if (valueTypeSerialiserId > 0)
                    {
                        // Locate the correct serialiser for this element.
                        AssetTypeSerialiser virtualElementSerialiser = 
                            this.manager.GetTypeSerialiserFromId (valueTypeSerialiserId);
                    
                        //
                        Object item = virtualElementSerialiser.BaseRead (abr);
                    
                        // add to array then move on
                        value = (TValue)item;
                    }
                    else
                    {
                        // the element is null
                        value = default(TValue);
                    }
                }
                
                dictionary.Add(key, value);
            }

            return dictionary;
        }

        protected internal override void Write(AssetBinaryWriter abw, Dictionary<TKey, TValue> obj)
        {
            throw new NotImplementedException();
        }
    }

    class Rgba32Serialiser
        : AssetTypeSerialiser<Rgba32>
    {
        internal Rgba32Serialiser () {}

        protected internal override Rgba32 Read(AssetBinaryReader abr)
        {
            Byte r = abr.ReadByte();
            Byte g = abr.ReadByte();
            Byte b = abr.ReadByte();
            Byte a = abr.ReadByte();
            
            return new Rgba32(r, g, b, a);
        }

        protected internal override void Write(AssetBinaryWriter abw, Rgba32 obj)
        {
            abw.Write(obj.R);
            abw.Write(obj.G);
            abw.Write(obj.B);
            abw.Write(obj.A);
        }
    }

    class Matrix44Serialiser
        : AssetTypeSerialiser<Matrix44>
    {
        internal Matrix44Serialiser () {}

        protected internal override Matrix44 Read(AssetBinaryReader abr)
        {
            Single m11 = abr.ReadSingle();
            Single m12 = abr.ReadSingle();
            Single m13 = abr.ReadSingle();
            Single m14 = abr.ReadSingle();

            Single m21 = abr.ReadSingle();
            Single m22 = abr.ReadSingle();
            Single m23 = abr.ReadSingle();
            Single m24 = abr.ReadSingle();

            Single m31 = abr.ReadSingle();
            Single m32 = abr.ReadSingle();
            Single m33 = abr.ReadSingle();
            Single m34 = abr.ReadSingle();

            Single m41 = abr.ReadSingle();
            Single m42 = abr.ReadSingle();
            Single m43 = abr.ReadSingle();
            Single m44 = abr.ReadSingle();

            return new Matrix44(
                m11, m12, m13, m14,
                m21, m22, m23, m24,
                m31, m32, m33, m34,
                m41, m42, m43, m44
            );
        }

        protected internal override void Write(AssetBinaryWriter abw, Matrix44 obj)
        {
            abw.Write(obj.M11);
            abw.Write(obj.M12);
            abw.Write(obj.M13);
            abw.Write(obj.M14);

            abw.Write(obj.M21);
            abw.Write(obj.M22);
            abw.Write(obj.M23);
            abw.Write(obj.M24);

            abw.Write(obj.M31);
            abw.Write(obj.M32);
            abw.Write(obj.M33);
            abw.Write(obj.M34);

            abw.Write(obj.M41);
            abw.Write(obj.M42);
            abw.Write(obj.M43);
            abw.Write(obj.M44);
        }
    }

    class QuaternionSerialiser
        : AssetTypeSerialiser<Quaternion>
    {
        internal QuaternionSerialiser () {}

        protected internal override Quaternion Read(AssetBinaryReader abr)
        {
            Single x = abr.ReadSingle();
            Single y = abr.ReadSingle();
            Single z = abr.ReadSingle();
            Single w = abr.ReadSingle();
            
            return new Quaternion(x, y, z, w);
        }

        protected internal override void Write(AssetBinaryWriter abw, Quaternion obj)
        {
            abw.Write(obj.X);
            abw.Write(obj.Y);
            abw.Write(obj.Z);
            abw.Write(obj.W);
        }
    }

    class Vector2Serialiser
        : AssetTypeSerialiser<Vector2>
    {
        internal Vector2Serialiser () {}

        protected internal override Vector2 Read(AssetBinaryReader abr)
        {
            Single x = abr.ReadSingle();
            Single y = abr.ReadSingle();

            return new Vector2(x, y);
        }

        protected internal override void Write(AssetBinaryWriter abw, Vector2 obj)
        {
            abw.Write(obj.X);
            abw.Write(obj.Y);
        }
    }

    class Vector3Serialiser
        : AssetTypeSerialiser<Vector3>
    {
        internal Vector3Serialiser () {}

        protected internal override Vector3 Read(AssetBinaryReader abr)
        {
            Single x = abr.ReadSingle();
            Single y = abr.ReadSingle();
            Single z = abr.ReadSingle();
            
            return new Vector3(x, y, z);
        }

        protected internal override void Write(AssetBinaryWriter abw, Vector3 obj)
        {
            abw.Write(obj.X);
            abw.Write(obj.Y);
            abw.Write(obj.Z);
        }
    }

    class Vector4Serialiser
        : AssetTypeSerialiser<Vector4>
    {
        internal Vector4Serialiser () {}

        protected internal override Vector4 Read(AssetBinaryReader abr)
        {
            Single x = abr.ReadSingle();
            Single y = abr.ReadSingle();
            Single z = abr.ReadSingle();
            Single w = abr.ReadSingle();
            
            return new Vector4(x, y, z, w);
        }

        protected internal override void Write(AssetBinaryWriter abw, Vector4 obj)
        {
            abw.Write(obj.X);
            abw.Write(obj.Y);
            abw.Write(obj.Z);
            abw.Write(obj.W);
        }
    }

    class VertexDeclarationSerialiser
        : AssetTypeSerialiser<VertexDeclaration>
    {
        AssetTypeSerialiser<VertexElement> vertexElementSerialiser;

        internal VertexDeclarationSerialiser () {}

        protected internal override void Initialise(AssetTypeSerialiserManager manager)
        {
            vertexElementSerialiser = manager.GetTypeSerialiser<VertexElement>();
        }

        protected internal override VertexDeclaration Read(AssetBinaryReader abr)
        {
            var elementCount = abr.ReadUInt32();

            VertexElement[] elements = new VertexElement[elementCount];
            
            for (UInt32 i = 0; i < elementCount; ++i)
            {
                VertexElement element = vertexElementSerialiser.Read(abr);

                elements[i] = element;
            }

            return new VertexDeclaration(elements);
        }

        protected internal override void Write(AssetBinaryWriter abw, VertexDeclaration obj)
        {
            throw new NotImplementedException();
        }
    }

    class VertexElementSerialiser
        : AssetTypeSerialiser<VertexElement>
    {
        AssetTypeSerialiser<VertexElementFormat> formatSerialiser;
        AssetTypeSerialiser<VertexElementUsage> usageSerialiser;

        internal VertexElementSerialiser () {}

        protected internal override void Initialise(AssetTypeSerialiserManager manager)
        {
            formatSerialiser = manager.GetTypeSerialiser<VertexElementFormat>();
            usageSerialiser = manager.GetTypeSerialiser<VertexElementUsage>();
        }

        protected internal override VertexElement Read(AssetBinaryReader abr)
        {
            Int32 offset = abr.ReadInt32();

            VertexElementFormat elementFormat = formatSerialiser.Read(abr);
            VertexElementUsage elementUsage = usageSerialiser.Read(abr);

            Int32 usageIndex = abr.ReadInt32();

            return new VertexElement(offset, elementFormat, elementUsage, usageIndex);
        }

        protected internal override void Write(AssetBinaryWriter abw, VertexElement obj)
        {
            throw new NotImplementedException();
        }
    }



    class GeometryBufferSerialiser
        : AssetTypeSerialiser<IGeometryBuffer>
    {
        AssetTypeSerialiser<VertexDeclaration> vertexDeclSerialiser;

        internal GeometryBufferSerialiser () {}

        protected internal override void Initialise(AssetTypeSerialiserManager manager)
        {
            vertexDeclSerialiser = manager.GetTypeSerialiser<VertexDeclaration>();
        }

        protected internal override IGeometryBuffer Read(AssetBinaryReader abr)
        {
            VertexDeclaration declaration = vertexDeclSerialiser.Read(abr);
            Int32 vertexCount = abr.ReadInt32();

            Byte[] vertData = abr.ReadBytes(vertexCount * declaration.VertexStride);
            
            Int32 indexCount = abr.ReadInt32();

            Byte[] indexData = abr.ReadBytes(vertexCount * sizeof(Int32));
            
            IGraphicsManager gfx = null;
            
            IGeometryBuffer buffer = gfx.CreateGeometryBuffer (
                declaration,
                vertexCount,
                indexCount );

            buffer.VertexBuffer.SetRawData (vertData, 0, vertexCount);

            buffer.IndexBuffer.SetRawData (indexData, 0, indexCount);
            
            return buffer;
        }

        protected internal override void Write(AssetBinaryWriter abw, IGeometryBuffer obj)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}
