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

namespace Cor
{
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


//----------------------------------------------------------------------------//

    #region Platform

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
        /// Provides information about the current back buffer.
        /// </summary>
        IDisplayStatus DisplayStatus { get; }

        /// <summary>
        /// Provides access to Cor's resource manager.
        /// The resource manager loads raw resources into
        /// the engine, avoiding the asset build.  Eventually
        /// it will get removed once everything uses the newer
        /// asset system.
        /// </summary>
        //[Obsolete]
        //IOldResourceManager Resources { get; }

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
        /// Gets the <see cref="Cor.AppSettings"/>
        /// value used by the Cor! framework when initilising the app.
        /// </summary>
        AppSettings Settings { get; }
    }

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
        void SetActiveGeometryBuffer (IGeometryBuffer buffer);

        /// <summary>
        /// Takes a texture asset and uploads it to the GPU Memory.
        /// Once done you should unload the texture asset.
        /// </summary>
        ITexture UploadTexture (TextureAsset tex);

        /// <summary>
        /// Removes the texture from the GPU Memory.
        /// </summary>
        void UnloadTexture (ITexture texture);

        /// <summary>
        /// Sets the active texture for a given slot.
        /// </summary>
        void SetActiveTexture (Int32 slot, ITexture tex);

        // Creates a new shader program on the GPU.
        IShader CreateShader (ShaderAsset asset);

        // Removes a shader program from the GPU.
        void DestroyShader (IShader shader);

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

        /// <summary>
        /// todo
        /// </summary>
        Stream GetAssetStream (String assetId);
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

    public interface ITexture
    {
        Int32 Width { get; }
        Int32 Height { get; }

        SurfaceFormat SurfaceFormat { get; }

        Byte[] Primary { get; }
        Byte[,] Mipmaps { get; }
    }

    #region Input

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

    #endregion

//----------------------------------------------------------------------------//

    #region Common

    #region Assets

        public sealed class ColourmapAsset
            : IAsset
        {
            public Rgba32[,] Data { get; set; }

            public Int32 Width { get { return Data.GetLength (0); } }

            public Int32 Height { get { return Data.GetLength (1); } }
        }

        public sealed class ShaderAsset
            : IAsset
        {
            public ShaderDefinition Definition { get; set; }
            public Byte [,] Data { get; set; }
        }

        public sealed class TextAsset
            : IAsset
        {
            public String Text { get; set; }
        }

        public sealed class TextureAsset
            : IAsset
        {
            // Data allocated in standard system RAM
            public Byte[] Data { get; set; }

            // Data allocated in standard system RAM
            //public Byte[,] Mipmaps { get; set; }

            public Int32 Width { get; set; }
            public Int32 Height { get; set; }

            public SurfaceFormat SurfaceFormat { get; set; }
        }

    #endregion

    #region Interfaces

        public interface IAsset
        {

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

    public enum SurfaceFormat
    {
        // Pixel maps
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

    #endregion

    #region Serialisers

    public abstract class TypeSerialiser
    {
        readonly Type targetType;

        public Type TargetType
        {
            get { return this.targetType; }
        }

        protected TypeSerialiser(Type targetType)
        {
            this.targetType = targetType;
        }

        public abstract Object ReadObject (BinaryReader abr);

        public abstract void WriteObject (BinaryWriter abw, Object obj);
    }

    public abstract class TypeSerialiser<T>
        : TypeSerialiser
    {
        protected TypeSerialiser()
            : base(typeof(T))
        {

        }

        public virtual void Initialise (TypeSerialiserDatabase manager) {}

        public override Object ReadObject (BinaryReader abr)
        {
            return this.Read (abr);
        }

        public override void WriteObject (BinaryWriter abw, Object obj)
        {
            this.Write (abw, (T) obj);
        }

        public abstract T Read (BinaryReader abr);

        public abstract void Write (BinaryWriter abw, T obj);
    }

    #endregion

    #region Shaders

    /// <summary>
    /// Defines how to create Cor.Xios's implementation
    /// of IShader.
    /// </summary>
    public sealed class ShaderDefinition
    {
        /// <summary>
        /// Defines a global name for this shader
        /// </summary>
        public string Name { get; set; }

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

        /// <summary>
        /// Defines the variants.  Done for optimisation, instead of having one
        /// massive shader that supports all the the Inputs and attempts to
        /// process them accordingly, we load slight variants of effectively
        /// the same shader, then we select the most optimal variant to run
        /// based upon the VertexDeclaration the calling code is about to draw.
        /// </summary>
        public List<ShaderVariantDefinition> VariantDefinitions { get; set; }
    }

    public sealed class ShaderInputDefinition
    {
        public String Name { get; set; }
        public Type Type { get; set; }
        public VertexElementUsage Usage { get; set; }
        public Object DefaultValue { get; set; }
        public Boolean Optional { get; set; }
    }

    public sealed class ShaderSamplerDefinition
    {
        public String NiceName { get; set; }
        public String Name { get; set; }
        public Boolean Optional { get; set; }
    }

    public sealed class ShaderVariableDefinition
    {
        public String NiceName { get; set; }
        public String Name { get; set; }
        public Type Type { get; set; }
        public Object DefaultValue { get; set; }
    }

    public sealed class ShaderVariantDefinition
    {
        public string VariantName { get; set; }

        public List<ShaderVarientPassDefinition> VariantPassDefinitions { get; set; }
    }

    public sealed class ShaderVarientPassDefinition
    {
        public string PassName { get; set; }
    }

    #endregion

    #region Primitive Types

    class Int64Serialiser
        : TypeSerialiser<Int64>
    {
        internal Int64Serialiser () {}

        public override Int64 Read(BinaryReader abr)
        {
            return abr.ReadInt64 ();
        }

        public override void Write(BinaryWriter abw, Int64 obj)
        {
            abw.Write(obj);
        }
    }

    class BooleanSerialiser
        : TypeSerialiser<Boolean>
    {
        internal BooleanSerialiser () {}

        public override Boolean Read(BinaryReader abr)
        {
            return abr.ReadBoolean ();
        }

        public override void Write(BinaryWriter abw, Boolean obj)
        {
            abw.Write(obj);
        }
    }

    class ByteSerialiser
        : TypeSerialiser<Byte>
    {
        internal ByteSerialiser () {}

        public override Byte Read(BinaryReader abr)
        {
            return abr.ReadByte ();
        }

        public override void Write(BinaryWriter abw, Byte obj)
        {
            abw.Write(obj);
        }
    }

    class CharSerialiser
        : TypeSerialiser<Char>
    {
        internal CharSerialiser () {}

        public override Char Read(BinaryReader abr)
        {
            return abr.ReadChar ();
        }

        public override void Write(BinaryWriter abw, Char obj)
        {
            abw.Write(obj);
        }
    }

    class DoubleSerialiser
        : TypeSerialiser<Double>
    {
        internal DoubleSerialiser () {}

        public override Double Read(BinaryReader abr)
        {
            return abr.ReadDouble ();
        }

        public override void Write(BinaryWriter abw, Double obj)
        {
            abw.Write(obj);
        }
    }

    class Int16Serialiser
        : TypeSerialiser<Int16>
    {
        internal Int16Serialiser () {}

        public override Int16 Read(BinaryReader abr)
        {
            return abr.ReadInt16 ();
        }

        public override void Write(BinaryWriter abw, Int16 obj)
        {
            abw.Write(obj);
        }
    }

    class Int32Serialiser
        : TypeSerialiser<Int32>
    {
        internal Int32Serialiser () {}

        public override Int32 Read(BinaryReader abr)
        {
            return abr.ReadInt32 ();
        }

        public override void Write(BinaryWriter abw, Int32 obj)
        {
            abw.Write(obj);
        }
    }

    class SByteSerialiser
        : TypeSerialiser<SByte>
    {
        internal SByteSerialiser () {}

        public override SByte Read(BinaryReader abr)
        {
            return abr.ReadSByte ();
        }

        public override void Write(BinaryWriter abw, SByte obj)
        {
            abw.Write(obj);
        }
    }

    class SingleSerialiser
        : TypeSerialiser<Single>
    {
        internal SingleSerialiser () {}

        public override Single Read(BinaryReader abr)
        {
            return abr.ReadSingle ();
        }

        public override void Write(BinaryWriter abw, Single obj)
        {
            abw.Write(obj);
        }
    }

    class UInt16Serialiser
        : TypeSerialiser<UInt16>
    {
        internal UInt16Serialiser () {}

        public override UInt16 Read(BinaryReader abr)
        {
            return abr.ReadUInt16 ();
        }

        public override void Write(BinaryWriter abw, UInt16 obj)
        {
            abw.Write(obj);
        }
    }

    class UInt32Serialiser
        : TypeSerialiser<UInt32>
    {
        internal UInt32Serialiser () {}

        public override UInt32 Read(BinaryReader abr)
        {
            return abr.ReadUInt32 ();
        }

        public override void Write(BinaryWriter abw, UInt32 obj)
        {
            abw.Write(obj);
        }
    }

    class UInt64Serialiser
        : TypeSerialiser<UInt64>
    {
        internal UInt64Serialiser () {}

        public override UInt64 Read(BinaryReader cbr)
        {
            return cbr.ReadUInt64 ();
        }

        public override void Write(BinaryWriter abw, UInt64 obj)
        {
            abw.Write(obj);
        }
    }

    #endregion

    #region System Types

    class StringSerialiser
        : TypeSerialiser<String>
    {
        internal StringSerialiser () {}

        public override String Read(BinaryReader abr)
        {
            return abr.ReadString ();
        }

        public override void Write(BinaryWriter abw, String obj)
        {
            abw.Write(obj);
        }
    }

    class DecimalSerialiser
        : TypeSerialiser<Decimal>
    {
        internal DecimalSerialiser () {}

        public override Decimal Read(BinaryReader abr)
        {
            return abr.ReadDecimal ();
        }

        public override void Write(BinaryWriter abw, Decimal obj)
        {
            abw.Write(obj);
        }
    }

    class TimeSpanSerialiser
        : TypeSerialiser<TimeSpan>
    {
        internal TimeSpanSerialiser () {}

        public override TimeSpan Read(BinaryReader abr)
        {
            Int64 ticks = abr.ReadInt64 ();

            return new TimeSpan(ticks);
        }

        public override void Write(BinaryWriter abw, TimeSpan obj)
        {
            abw.Write(obj.Ticks);
        }
    }

    // test reading lists containing different items from a chain of inheritance.
    class ListSerialiser<T>
        : TypeSerialiser<List<T>>
    {
        TypeSerialiser<T> elementSerialiser;

        TypeSerialiserDatabase manager;

        internal ListSerialiser () {}

        public override void Initialise (TypeSerialiserDatabase manager)
        {
            this.manager = manager;
            elementSerialiser = manager.GetTypeSerialiser<T> ();
        }

        public override List<T> Read(BinaryReader abr)
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
                        TypeSerialiser virtualElementSerialiser =
                            this.manager.GetTypeSerialiserFromId(objectTypeSerialiserId);

                        //
                        Object item = virtualElementSerialiser.ReadObject (abr);

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

        public override void Write(BinaryWriter abw, List<T> obj)
        {
            throw new NotImplementedException();
        }
    }

    class NullableSerialiser<T>
        : TypeSerialiser<T?>
    where T
        : struct
    {
        TypeSerialiser<T> valueSerialiser;

        internal NullableSerialiser () {}

        public override void Initialise (TypeSerialiserDatabase manager)
        {
            valueSerialiser = manager.GetTypeSerialiser<T>();
        }

        public override T? Read(BinaryReader abr)
        {
            if(abr.ReadBoolean())
            {
                return valueSerialiser.Read (abr);
            }

            return null;
        }

        public override void Write(BinaryWriter abw, T? obj)
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
        : TypeSerialiser<T>
    {
        TypeSerialiser underlyingTypeSerialiser;

        internal EnumSerialiser () {}

        public override void Initialise(TypeSerialiserDatabase manager)
        {
            // can we not get this at compile time? -_-
            // lets stick with Int32 for now
            Type readerType = Enum.GetUnderlyingType(typeof(T));

            underlyingTypeSerialiser = manager.GetTypeSerialiser(readerType);
        }

        public override T Read(BinaryReader abr)
        {
            Object underlyingValue = underlyingTypeSerialiser.ReadObject(abr);

            return (T) underlyingValue;
        }

        public override void Write(BinaryWriter abw, T obj)
        {
            underlyingTypeSerialiser.WriteObject (abw, obj);
        }
    }

    class ArraySerialiser<T>
        : TypeSerialiser<T[]>
    {
        TypeSerialiser<T> elementSerialiser;

        TypeSerialiserDatabase manager;

        internal ArraySerialiser () {}

        public override void Initialise(TypeSerialiserDatabase manager)
        {
            this.manager = manager;
            elementSerialiser = manager.GetTypeSerialiser<T>();
        }

        public override T[] Read (BinaryReader abr)
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
                        TypeSerialiser virtualElementSerialiser =
                            this.manager.GetTypeSerialiserFromId (objectTypeSerialiserId);

                        //
                        Object item = virtualElementSerialiser.ReadObject (abr);

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

        public override void Write(BinaryWriter abw, T[] obj)
        {
            throw new NotSupportedException();
        }
    }

    class DictionarySerialiser<TKey, TValue>
        : TypeSerialiser<Dictionary<TKey, TValue>>
    {
        TypeSerialiser<TKey> keySerialiser;
        TypeSerialiser<TValue> valueSerialiser;

        TypeSerialiserDatabase manager;

        internal DictionarySerialiser () {}

        public override void Initialise(TypeSerialiserDatabase manager)
        {
            this.manager = manager;
            keySerialiser = manager.GetTypeSerialiser<TKey>();
            valueSerialiser = manager.GetTypeSerialiser<TValue>();
        }

        public override Dictionary<TKey, TValue> Read(BinaryReader abr)
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
                        TypeSerialiser virtualElementSerialiser =
                            this.manager.GetTypeSerialiserFromId (keyTypeSerialiserId);

                        //
                        Object item = virtualElementSerialiser.ReadObject (abr);

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
                        TypeSerialiser virtualElementSerialiser =
                            this.manager.GetTypeSerialiserFromId (valueTypeSerialiserId);

                        //
                        Object item = virtualElementSerialiser.ReadObject (abr);

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

        public override void Write(BinaryWriter abw, Dictionary<TKey, TValue> obj)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    #region Abacus Types

    class Rgba32Serialiser
        : TypeSerialiser<Rgba32>
    {
        internal Rgba32Serialiser () {}

        public override Rgba32 Read(BinaryReader abr)
        {
            Byte r = abr.ReadByte();
            Byte g = abr.ReadByte();
            Byte b = abr.ReadByte();
            Byte a = abr.ReadByte();

            return new Rgba32(r, g, b, a);
        }

        public override void Write(BinaryWriter abw, Rgba32 obj)
        {
            abw.Write(obj.R);
            abw.Write(obj.G);
            abw.Write(obj.B);
            abw.Write(obj.A);
        }
    }

    class Matrix44Serialiser
        : TypeSerialiser<Matrix44>
    {
        internal Matrix44Serialiser () {}

        public override Matrix44 Read(BinaryReader abr)
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

        public override void Write(BinaryWriter abw, Matrix44 obj)
        {
            abw.Write(obj.R0C0);
            abw.Write(obj.R0C1);
            abw.Write(obj.R0C2);
            abw.Write(obj.R0C3);

            abw.Write(obj.R1C0);
            abw.Write(obj.R1C1);
            abw.Write(obj.R1C2);
            abw.Write(obj.R1C3);

            abw.Write(obj.R2C0);
            abw.Write(obj.R2C1);
            abw.Write(obj.R2C2);
            abw.Write(obj.R2C3);

            abw.Write(obj.R3C0);
            abw.Write(obj.R3C1);
            abw.Write(obj.R3C2);
            abw.Write(obj.R3C3);
        }
    }

    class QuaternionSerialiser
        : TypeSerialiser<Quaternion>
    {
        internal QuaternionSerialiser () {}

        public override Quaternion Read(BinaryReader abr)
        {
            Single i = abr.ReadSingle();
            Single j = abr.ReadSingle();
            Single k = abr.ReadSingle();
            Single u = abr.ReadSingle();

            return new Quaternion(i, j, k, u);
        }

        public override void Write(BinaryWriter abw, Quaternion obj)
        {
            abw.Write(obj.I);
            abw.Write(obj.J);
            abw.Write(obj.K);
            abw.Write(obj.U);
        }
    }

    class Vector2Serialiser
        : TypeSerialiser<Vector2>
    {
        internal Vector2Serialiser () {}

        public override Vector2 Read(BinaryReader abr)
        {
            Single x = abr.ReadSingle();
            Single y = abr.ReadSingle();

            return new Vector2(x, y);
        }

        public override void Write(BinaryWriter abw, Vector2 obj)
        {
            abw.Write(obj.X);
            abw.Write(obj.Y);
        }
    }

    class Vector3Serialiser
        : TypeSerialiser<Vector3>
    {
        internal Vector3Serialiser () {}

        public override Vector3 Read(BinaryReader abr)
        {
            Single x = abr.ReadSingle();
            Single y = abr.ReadSingle();
            Single z = abr.ReadSingle();

            return new Vector3(x, y, z);
        }

        public override void Write(BinaryWriter abw, Vector3 obj)
        {
            abw.Write(obj.X);
            abw.Write(obj.Y);
            abw.Write(obj.Z);
        }
    }

    class Vector4Serialiser
        : TypeSerialiser<Vector4>
    {
        internal Vector4Serialiser () {}

        public override Vector4 Read(BinaryReader abr)
        {
            Single x = abr.ReadSingle();
            Single y = abr.ReadSingle();
            Single z = abr.ReadSingle();
            Single w = abr.ReadSingle();

            return new Vector4(x, y, z, w);
        }

        public override void Write(BinaryWriter abw, Vector4 obj)
        {
            abw.Write(obj.X);
            abw.Write(obj.Y);
            abw.Write(obj.Z);
            abw.Write(obj.W);
        }
    }

    #endregion

    #region Cor Types

    class VertexDeclarationSerialiser
        : TypeSerialiser<VertexDeclaration>
    {
        TypeSerialiser<VertexElement> vertexElementSerialiser;

        internal VertexDeclarationSerialiser () {}

        public override void Initialise(TypeSerialiserDatabase manager)
        {
            vertexElementSerialiser = manager.GetTypeSerialiser<VertexElement>();
        }

        public override VertexDeclaration Read(BinaryReader abr)
        {
            var elementCount = abr.ReadInt32();

            VertexElement[] elements = new VertexElement[elementCount];

            for (Int32 i = 0; i < elementCount; ++i)
            {
                VertexElement element = vertexElementSerialiser.Read(abr);

                elements[i] = element;
            }

            return new VertexDeclaration(elements);
        }

        public override void Write(BinaryWriter abw, VertexDeclaration obj)
        {
            throw new NotImplementedException();/*
            abr.Write (obj.ElementCount);

            for (Int32 i = 0; i < obj.ElementCount; ++i)
            {
                vertexElementSerialiser.Write(abw, obj.Element[i]);
            }*/
        }
    }

    class VertexElementSerialiser
        : TypeSerialiser<VertexElement>
    {
        TypeSerialiser<VertexElementFormat> formatSerialiser;
        TypeSerialiser<VertexElementUsage> usageSerialiser;

        internal VertexElementSerialiser () {}

        public override void Initialise(TypeSerialiserDatabase manager)
        {
            formatSerialiser = manager.GetTypeSerialiser<VertexElementFormat>();
            usageSerialiser = manager.GetTypeSerialiser<VertexElementUsage>();
        }

        public override VertexElement Read(BinaryReader abr)
        {
            Int32 offset = abr.ReadInt32();
            VertexElementFormat elementFormat = formatSerialiser.Read(abr);
            VertexElementUsage elementUsage = usageSerialiser.Read(abr);
            Int32 usageIndex = abr.ReadInt32();

            return new VertexElement(offset, elementFormat, elementUsage, usageIndex);
        }

        public override void Write(BinaryWriter abw, VertexElement obj)
        {
            throw new NotImplementedException();/*
            abr.Write(obj.Offset);
            formatSerialiser.Write(abw, obj.VertexElementFormat);
            usageSerialiser.Write(abw, obj.VertexElementUsage);
            abr.Write(obj.UsageIndex);*/
        }
    }



    class GeometryBufferSerialiser
        : TypeSerialiser<IGeometryBuffer>
    {
        TypeSerialiser<VertexDeclaration> vertexDeclSerialiser;

        internal GeometryBufferSerialiser () {}

        public override void Initialise(TypeSerialiserDatabase manager)
        {
            vertexDeclSerialiser = manager.GetTypeSerialiser<VertexDeclaration>();
        }

        public override IGeometryBuffer Read(BinaryReader abr)
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

        public override void Write(BinaryWriter abw, IGeometryBuffer obj)
        {
            throw new NotImplementedException();/*
            declaration.Write (abw, obj.VertexDeclaration);

            abw.Write (obj.VertexCount);
            Byte[] rawVertData = obj.VertexBuffer.GetRawData (0, obj.VertexCount);
            abw.Write (rawVertData);

            abw.Write (obj.IndexCount);
            Byte[] rawIndexData = obj.IndexBuffer.GetRawData (0, obj.IndexCount);
            abw.Write (rawIndexData);*/

        }
    }

    #endregion

    #region Systems

    public sealed class AssetManager
        : IDisposable
    {
        readonly IGraphicsManager graphics;
        readonly ISystemManager systemManager;

        internal AssetManager (
            IGraphicsManager graphics,
            ISystemManager systemManager)
        {
            this.graphics = graphics;
            this.systemManager = systemManager;
        }

        public T Load<T> (String assetId)
        where T
            : IAsset
        {
            using (Stream stream = this.systemManager.GetAssetStream (assetId))
            {
                using (var br = new BinaryReader (stream))
                {
                    var tsdb = new TypeSerialiserDatabase ();
                    Byte f0 = tsdb.GetTypeSerialiser <Byte> ().Read (br);
                    Byte f1 = tsdb.GetTypeSerialiser <Byte> ().Read (br);
                    Byte f2 = tsdb.GetTypeSerialiser <Byte> ().Read (br);

                    if (f0 != (Byte) 'C' || f1 != (Byte) 'B' || f2 != (Byte) 'B')
                        throw new Exception ();

                    return tsdb.GetTypeSerialiser <T> ().Read (br);
                }
            }
        }

        public void Unload ()
        {

        }

        public void Dispose ()
        {

        }
    }

    public class TypeSerialiserDatabase
    {
        readonly Dictionary<Type, TypeSerialiser> assetTypeSerialisers;

        public TypeSerialiserDatabase()
        {
            assetTypeSerialisers = new Dictionary<Type, TypeSerialiser> ();
        }

        public TypeSerialiser<T> GetTypeSerialiser<T>()
        {
            return GetTypeSerialiser(typeof (T)) as TypeSerialiser<T>;
        }

        public TypeSerialiser GetTypeSerialiser(Type type)
        {
            if (!assetTypeSerialisers.ContainsKey (type))
                assetTypeSerialisers [type] = Activator.CreateInstance (type) as TypeSerialiser;

            return assetTypeSerialisers [type];
        }

        public TypeSerialiser GetTypeSerialiserFromId (Int32 id)
        {
            throw new NotImplementedException ();
        }
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
        /// Initializes a new instance of the <see cref="Cor.AppTime"/>
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
        /// <see cref="Cor.AppTime"/>.
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
        internal Int32 _offset;

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
        internal Int32 _usageIndex;

        /// <summary>
        /// todo
        /// </summary>
        public Int32 Offset
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
        public Int32 UsageIndex
        {
            get { return this._usageIndex; }
            set { this._usageIndex = value; }
        }

        /// <summary>
        /// todo
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
        /// todo
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
        /// todo
        /// </summary>
        public override Int32 GetHashCode ()
        {
            return base.GetHashCode ();
        }

        /// <summary>
        /// todo
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

    #endregion

//----------------------------------------------------------------------------//

    #region Extensions

    public static class BinaryReaderExtensions
    {
        public static Int32 Read7BitEncodedInt32(this BinaryReader me)
        {
            Int32 result = 0;
            Int32 bitsRead = 0;
            Int32 value;

            do
            {
                value = me.ReadByte();
                result |= (value & 0x7f) << bitsRead;
                bitsRead += 7;
            }
            while ((value & 0x80) != 0);

            return result;
        }
    }

    public static class BinaryWriterExtensions
    {
        public static void Write7BitEncodedInt32 (this BinaryWriter me, Int32 value)
        {
            throw new NotImplementedException ();
        }
    }

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

//----------------------------------------------------------------------------//

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

}
