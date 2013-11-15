// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor! Mono Mac Platform Implementation                                  │ \\
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
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

using Sungiant.Abacus;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Abacus.Int32Precision;

using Sungiant.Cor.Lib.Managed.Khronos;
using Sungiant.Cor.Platform.Stub;

using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.CoreVideo;
using MonoMac.CoreGraphics;
using MonoMac.CoreImage;
using MonoMac.ImageIO;
using MonoMac.ImageKit;

namespace Sungiant.Cor.Platform.Managed.MonoMac
{
    public sealed class Engine
        : ICor
    {
        readonly AudioManager audio;
        readonly GraphicsManager graphics;
        readonly ResourceManager resources;
        readonly InputManager input;
        readonly SystemManager system;
        readonly AppSettings settings;
        readonly IApp app;
        readonly LogManager log;

        public Engine(
            AppSettings settings,
            IApp app)
        {
            InternalUtils.Log.Info(
                "Engine -> ()");

            this.audio = new AudioManager();
            this.graphics = new GraphicsManager();
            this.resources = new ResourceManager();
            this.input = new InputManager();
            this.system = new SystemManager();
            this.settings = settings;
            this.app = app;
            this.log = new LogManager(this.settings.LogSettings);
            this.app.Initilise(this);
        }

        internal AudioManager AudioImplementation { get { return this.audio; } }

        internal GraphicsManager GraphicsImplementation { get { return this.graphics; } }

        internal ResourceManager ResourcesImplementation { get { return this.resources; } }

        internal InputManager InputImplementation { get { return this.input; } }

        internal SystemManager SystemImplementation { get { return this.system; } }

        #region ICor

        public IAudioManager Audio { get { return this.audio; } }

        public IGraphicsManager Graphics { get { return this.graphics; } }

        public IResourceManager Resources { get { return this.resources; } }

        public IInputManager Input { get { return this.input; } }

        public ISystemManager System { get { return this.system; } }

        public LogManager Log { get { return this.log; } }

        public AppSettings Settings { get { return this.settings; } }

        #endregion


        internal Boolean Update(AppTime time)
        {
            return app.Update(time);
        }

        internal void Render()
        {
            app.Render();
        }
    }

    public sealed class AudioManager
        : IAudioManager
    {
        public Single volume = 1f;

        public Single Volume
        {
            get { return this.volume; }
            set
            {
                this.volume = value;

                InternalUtils.Log.Info(
                    "AudioManager -> Setting Volume:" + value);
            }
        }

        #region IAudioManager

        public AudioManager()
        {
            InternalUtils.Log.Info(
                "AudioManager -> ()");

            this.volume = 1f;
        }

        #endregion
    }

    public sealed class GraphicsManager
        : IGraphicsManager
    {
        readonly IDisplayStatus displayStatus;
        readonly IGpuUtils gpuUtils;

        GeometryBuffer currentGeomBuffer;
        CullMode? currentCullMode;

        public GraphicsManager()
        {
            InternalUtils.Log.Info(
                "GraphicsManager -> ()");

            this.displayStatus = new DisplayStatus();
            this.gpuUtils = new MonoMacGpuUtils();

            global::MonoMac.OpenGL.GL.Enable(global::MonoMac.OpenGL.EnableCap.Blend);
            ErrorHandler.Check();

            this.SetBlendEquation(
                BlendFunction.Add, BlendFactor.SourceAlpha, BlendFactor.InverseSourceAlpha,
                BlendFunction.Add, BlendFactor.One, BlendFactor.InverseSourceAlpha);

            global::MonoMac.OpenGL.GL.Enable(global::MonoMac.OpenGL.EnableCap.DepthTest);
            ErrorHandler.Check();

            global::MonoMac.OpenGL.GL.DepthMask(true);
            ErrorHandler.Check();

            global::MonoMac.OpenGL.GL.DepthRange(0f, 1f);
            ErrorHandler.Check();

            global::MonoMac.OpenGL.GL.DepthFunc(global::MonoMac.OpenGL.DepthFunction.Lequal);
            ErrorHandler.Check();

            SetCullMode (CullMode.CW);
        }

        [ReliabilityContract (Consistency.MayCorruptInstance, Cer.MayFail)]
        static IntPtr Add (IntPtr pointer, int offset)
        {
            unsafe
            {
                return (IntPtr) (unchecked (((byte *) pointer) + offset));
            }
        }

        [ReliabilityContract (Consistency.MayCorruptInstance, Cer.MayFail)]
        static IntPtr Subtract (IntPtr pointer, int offset)
        {
            unsafe
            {
                return (IntPtr) (unchecked (((byte *) pointer) - offset));
            }
        }
        void EnableVertAttribs(VertexDeclaration vertDecl, IntPtr pointer)
        {
            var vertElems = vertDecl.GetVertexElements();

            IntPtr ptr = pointer;

            int counter = 0;
            foreach(var elem in vertElems)
            {
                global::MonoMac.OpenGL.GL.EnableVertexAttribArray(counter);
                ErrorHandler.Check();

                //var vertElemUsage = elem.VertexElementUsage;
                var vertElemFormat = elem.VertexElementFormat;
                var vertElemOffset = elem.Offset;

                Int32 numComponentsInVertElem = 0;
                Boolean vertElemNormalized = false;
                global::MonoMac.OpenGL.VertexAttribPointerType glVertElemFormat;

                EnumConverter.ToOpenGL(vertElemFormat, out glVertElemFormat, out vertElemNormalized, out numComponentsInVertElem);

                if( counter != 0)
                {
                    ptr = Add(ptr, vertElemOffset);
                }

                global::MonoMac.OpenGL.GL.VertexAttribPointer(
                    counter,                // index - specifies the generic vertex attribute index.  This value is 0 to
                                            //         max vertex attributes supported - 1.
                    numComponentsInVertElem,// size - number of components specified in the vertex array for the
                                            //        vertex attribute referenced by index.  Valid values are 1 - 4.
                    glVertElemFormat,       // type - Data format, valid values are GL_BYTE, GL_UNSIGNED_BYTE, GL_SHORT, GL_UNSIGNED_SHORT,
                                            //        GL_FLOAT, GL_FIXED, GL_HALF_FLOAT_OES*(Optional feature of es2)
                    vertElemNormalized,     // normalised - used to indicate whether the non-floating data format type should be normalised
                                            //              or not when converted to floating point.
                    vertDecl.VertexStride,  // stride - the components of vertex attribute specified by size are stored sequentially for each
                                            //          vertex.  stride specifies the delta between data for vertex index 1 and vertex (1 + 1).
                                            //          If stride is 0, attribute data for all vertices are stored sequentially.
                                            //          If stride is > 0, then we use the stride valude tas the pitch to get vertex data
                                            //          for the next index.
                    ptr

                    );

                ErrorHandler.Check();

                counter++;

            }
        }

        void DisableVertAttribs(VertexDeclaration vertDecl)
        {
            var vertElems = vertDecl.GetVertexElements();

            for(int i = 0; i < vertElems.Length; ++i)
            {
                global::MonoMac.OpenGL.GL.DisableVertexAttribArray(i);
                ErrorHandler.Check();
            }
        }


        #region IGraphicsManager

        public IDisplayStatus DisplayStatus { get { return this.displayStatus; } }

        public IGpuUtils GpuUtils { get { return this.gpuUtils; } }

        public void Reset()
        {
            this.ClearDepthBuffer();
            this.ClearColourBuffer();
            this.SetActiveGeometryBuffer(null);

            // todo, here we need to set all the texture slots to point to null
            this.SetActiveTexture(0, null);
        }

        public void ClearColourBuffer(Rgba32 col = new Rgba32())
        {
            Vector4 c;

            col.UnpackTo(out c);

            global::MonoMac.OpenGL.GL.ClearColor (c.X, c.Y, c.Z, c.W);

            var mask = global::MonoMac.OpenGL.ClearBufferMask.ColorBufferBit;

            global::MonoMac.OpenGL.GL.Clear ( mask );

            ErrorHandler.Check();
        }

        public void ClearDepthBuffer(Single val = 1)
        {
            global::MonoMac.OpenGL.GL.ClearDepth(val);

            var mask = global::MonoMac.OpenGL.ClearBufferMask.DepthBufferBit;

            global::MonoMac.OpenGL.GL.Clear ( mask );

            ErrorHandler.Check();
        }

        public void SetCullMode(CullMode cullMode)
        {
            if (!currentCullMode.HasValue || currentCullMode.Value != cullMode)
            {
                if (cullMode == CullMode.None)
                {
                    global::MonoMac.OpenGL.GL.Disable (global::MonoMac.OpenGL.EnableCap.CullFace);
                    ErrorHandler.Check ();

                }
                else
                {
                    global::MonoMac.OpenGL.GL.Enable(global::MonoMac.OpenGL.EnableCap.CullFace);
                    ErrorHandler.Check();

                    global::MonoMac.OpenGL.GL.FrontFace(global::MonoMac.OpenGL.FrontFaceDirection.Cw);
                    ErrorHandler.Check();

                    if (cullMode == CullMode.CW)
                    {
                        global::MonoMac.OpenGL.GL.CullFace (global::MonoMac.OpenGL.CullFaceMode.Back);
                        ErrorHandler.Check ();
                    }
                    else if (cullMode == CullMode.CCW)
                    {
                        global::MonoMac.OpenGL.GL.CullFace (global::MonoMac.OpenGL.CullFaceMode.Front);
                        ErrorHandler.Check ();
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }

                currentCullMode = cullMode;
            }
        }

        public IGeometryBuffer CreateGeometryBuffer (
            VertexDeclaration vertexDeclaration,
            Int32 vertexCount,
            Int32 indexCount )
        {
            return new GeometryBuffer(vertexDeclaration, vertexCount, indexCount);
        }

        public void SetActiveGeometryBuffer(IGeometryBuffer buffer)
        {
            var temp = buffer as GeometryBuffer;

            if( temp != this.currentGeomBuffer )
            {
                if( this.currentGeomBuffer != null )
                {
                    this.currentGeomBuffer.Deactivate();

                    this.currentGeomBuffer = null;
                }

                if( temp != null )
                {
                    temp.Activate();
                }

                this.currentGeomBuffer = temp;
            }
        }

        public void SetActiveTexture(Int32 slot, Texture2D tex)
        {
            global::MonoMac.OpenGL.TextureUnit oglTexSlot = EnumConverter.ToOpenGLTextureSlot(slot);
            global::MonoMac.OpenGL.GL.ActiveTexture(oglTexSlot);

            var oglt0 = tex as OpenGLTexture;

            if( oglt0 != null )
            {
                var textureTarget = global::MonoMac.OpenGL.TextureTarget.Texture2D;

                // we need to bind the texture object so that we can opperate on it.
                global::MonoMac.OpenGL.GL.BindTexture(textureTarget, oglt0.glTextureId);
                ErrorHandler.Check();
            }

        }

        public void SetBlendEquation(
            BlendFunction rgbBlendFunction,
            BlendFactor sourceRgb,
            BlendFactor destinationRgb,
            BlendFunction alphaBlendFunction,
            BlendFactor sourceAlpha,
            BlendFactor destinationAlpha
            )
        {
            global::MonoMac.OpenGL.GL.BlendEquationSeparate(
                EnumConverter.ToOpenGL(rgbBlendFunction),
                EnumConverter.ToOpenGL(alphaBlendFunction) );
            ErrorHandler.Check();

            global::MonoMac.OpenGL.GL.BlendFuncSeparate(
                EnumConverter.ToOpenGLSrc(sourceRgb),
                EnumConverter.ToOpenGLDest(destinationRgb),
                EnumConverter.ToOpenGLSrc(sourceAlpha),
                EnumConverter.ToOpenGLDest(destinationAlpha) );
            ErrorHandler.Check();
        }

        public void DrawPrimitives(
            PrimitiveType primitiveType,
            Int32 startVertex,
            Int32 primitiveCount )
        {
            throw new NotImplementedException();
        }

        public void DrawIndexedPrimitives (
            PrimitiveType primitiveType,
            Int32 baseVertex,
            Int32 minVertexIndex,
            Int32 numVertices,
            Int32 startIndex,
            Int32 primitiveCount
            )
        {
            if( baseVertex != 0 || minVertexIndex != 0 || startIndex != 0 )
            {
                throw new NotImplementedException();
            }

            var otkpType =  EnumConverter.ToOpenGL(primitiveType);
            //Int32 numVertsInPrim = numVertices / primitiveCount;

            Int32 nVertsInPrim = PrimitiveHelper.NumVertsIn(primitiveType);
            Int32 count = primitiveCount * nVertsInPrim;

            var vertDecl = currentGeomBuffer.VertexBuffer.VertexDeclaration;

            this.EnableVertAttribs( vertDecl, (IntPtr) 0 );

            global::MonoMac.OpenGL.GL.DrawElements (
                otkpType,
                count,
                global::MonoMac.OpenGL.DrawElementsType.UnsignedShort,
                (System.IntPtr) 0 );

            ErrorHandler.Check();

            this.DisableVertAttribs(vertDecl);
        }

        public void DrawUserPrimitives <T> (
            PrimitiveType primitiveType,
            T[] vertexData,
            Int32 vertexOffset,
            Int32 primitiveCount,
            VertexDeclaration vertexDeclaration )
            where T : struct, IVertexType
        {
            // do i need to do this? todo: find out
            this.SetActiveGeometryBuffer(null);

            var vertDecl = vertexData[0].VertexDeclaration;

            //MSDN
            //
            //The GCHandle structure is used with the GCHandleType
            //enumeration to create a handle corresponding to any managed
            //object. This handle can be one of four types: Weak,
            //WeakTrackResurrection, Normal, or Pinned. When the handle has
            //been allocated, you can use it to prevent the managed object
            //from being collected by the garbage collector when an unmanaged
            //client holds the only reference. Without such a handle,
            //the object can be collected by the garbage collector before
            //completing its work on behalf of the unmanaged client.
            //
            //You can also use GCHandle to create a pinned object that
            //returns a memory address to prevent the garbage collector
            //from moving the object in memory.
            //
            //When the handle goes out of scope you must explicitly release
            //it by calling the Free method; otherwise, memory leaks may
            //occur. When you free a pinned handle, the associated object
            //will be unpinned and will become eligible for garbage
            //collection, if there are no other references to it.
            //
            GCHandle pinnedArray = GCHandle.Alloc(vertexData, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();

            if( vertexOffset != 0 )
            {
                pointer = Add(pointer, vertexOffset * vertDecl.VertexStride * sizeof(byte));
            }

            var glDrawMode = EnumConverter.ToOpenGL(primitiveType);
            var glDrawModeAll = glDrawMode;


            var bindTarget = global::MonoMac.OpenGL.BufferTarget.ArrayBuffer;

            global::MonoMac.OpenGL.GL.BindBuffer(bindTarget, 0);
            ErrorHandler.Check();


            this.EnableVertAttribs( vertDecl, pointer );

            Int32 nVertsInPrim = PrimitiveHelper.NumVertsIn(primitiveType);
            Int32 count = primitiveCount * nVertsInPrim;

            global::MonoMac.OpenGL.GL.DrawArrays(
                glDrawModeAll, // specifies the primitive to render
                vertexOffset,  // specifies the starting vertex index in the enabled vertex arrays
                count ); // specifies the number of indicies to be drawn

            ErrorHandler.Check();


            this.DisableVertAttribs(vertDecl);


            pinnedArray.Free();
        }

        public void DrawUserIndexedPrimitives <T> (
            PrimitiveType primitiveType,
            T[] vertexData,
            Int32 vertexOffset,
            Int32 numVertices,
            Int32[] indexData,
            Int32 indexOffset,
            Int32 primitiveCount,
            VertexDeclaration vertexDeclaration )
            where T : struct, IVertexType
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public sealed class DisplayStatus
        : IDisplayStatus
    {
        public DisplayStatus()
        {
            InternalUtils.Log.Info(
                "DisplayStatus -> ()");
        }

        #region IDisplayStatus

        public Boolean Fullscreen { get { return true; } }

        public Int32 CurrentWidth { get { return 800; } }

        public Int32 CurrentHeight { get { return 600; } }

        #endregion
    }
    public sealed class IndexBuffer
        : IIndexBuffer
        , IDisposable
    {
        static Int32 resourceCounter;

        Int32 indexCount;
        global::MonoMac.OpenGL.BufferTarget type;
        UInt32 bufferHandle;
        global::MonoMac.OpenGL.BufferUsageHint bufferUsage;

        public IndexBuffer (Int32 indexCount)
        {
            this.indexCount = indexCount;

            this.type = global::MonoMac.OpenGL.BufferTarget.ElementArrayBuffer;

            this.bufferUsage = global::MonoMac.OpenGL.BufferUsageHint.DynamicDraw;

            global::MonoMac.OpenGL.GL.GenBuffers(1, out this.bufferHandle);
            
            ErrorHandler.Check();

            if( this.bufferHandle == 0 )
            {
                throw new Exception("Failed to generate vert buffer.");
            }

            this.Activate();

            global::MonoMac.OpenGL.GL.BufferData(
                this.type,
                (System.IntPtr) (sizeof(UInt16) * this.indexCount),
                (System.IntPtr) null,
                this.bufferUsage);

            ErrorHandler.Check();

            resourceCounter++;

        }

        ~IndexBuffer()
        {
            CleanUpNativeResources();
        }

        void CleanUpManagedResources()
        {

        }

        void CleanUpNativeResources()
        {
            global::MonoMac.OpenGL.GL.DeleteBuffers(1, ref this.bufferHandle);
            ErrorHandler.Check();

            bufferHandle = 0;

            resourceCounter--;
        }

        public void Dispose()
        {
            CleanUpManagedResources();
            CleanUpNativeResources();
            GC.SuppressFinalize(this);
        }

        internal void Activate()
        {
            global::MonoMac.OpenGL.GL.BindBuffer(this.type, this.bufferHandle);
            ErrorHandler.Check();
        }

        internal void Deactivate()
        {
            global::MonoMac.OpenGL.GL.BindBuffer(this.type, 0);
            ErrorHandler.Check();
        }


        public void SetData (Int32[] data)
        {

            if( data.Length != indexCount )
            {
                throw new Exception("?");
            }

            UInt16[] udata = new UInt16[data.Length];

            for(Int32 i = 0; i < data.Length; ++i)
            {
                udata[i] = (UInt16) data[i];
            }
            
            this.Activate();

            // glBufferData FN will reserve appropriate data storage based on the value of size.  The data argument can
            // be null indicating that the reserved data store remains uninitiliazed.  If data is a valid pointer,
            // then content of data are copied to the allocated data store.  The contents of the buffer object data
            // store can be initialized or updated using the glBufferSubData FN
            global::MonoMac.OpenGL.GL.BufferSubData(
                this.type,
                (System.IntPtr) 0,
                (System.IntPtr) (sizeof(UInt16) * this.indexCount),
                udata);

            udata = null;

            ErrorHandler.Check();
        }

        public int IndexCount
        {
            get
            {
                return indexCount;
            }
        }

        public void GetData(Int32[] data)
        {
            throw new NotImplementedException();    
        }

        public void GetData(Int16[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException();    
        }

        public void GetData(Int32 offsetInBytes, Int16[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException();    
        }

        public void SetData(Int16[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException();    
        }

        public void SetData(Int32 offsetInBytes, Int16[] data, Int32 startIndex, Int32 elementCount)
        {
            throw new NotImplementedException();    
        }

    }

    public sealed class InputManager
        : IInputManager
    {
        readonly Keyboard keyboard;
        readonly Mouse mouse;

        readonly IXbox360Gamepad xbox360Gamepad = new StubXbox360Gamepad();
        readonly IPsmGamepad psmGamepad = new StubPsmGamepad();
        readonly IMultiTouchController multiTouchController = new StubMultiTouchController();
        readonly IGenericGamepad genericGamepad = new StubGenericGamepad();

        public InputManager()
        {
            InternalUtils.Log.Info(
                "InputManager -> ()");

            keyboard = new Keyboard();
            mouse = new Mouse();
        }

        internal Keyboard KeyboardImplemenatation { get { return keyboard; } }
        internal Mouse MouseImplemenatation { get { return mouse; } }

        #region IInputManager

        public IXbox360Gamepad Xbox360Gamepad
        {
            get { return xbox360Gamepad; }
        }

        public IPsmGamepad PsmGamepad
        {
            get { return psmGamepad; }
        }

        public IMultiTouchController MultiTouchController
        {
            get { return multiTouchController; }
        }

        public IGenericGamepad GenericGamepad
        {
            get { return genericGamepad; }
        }

        public IMouse Mouse
        {
            get
            {
                return mouse;
            }
        }

        public IKeyboard Keyboard
        {
            get
            {
                return keyboard;
            }
        }

        #endregion
    }

    public sealed class ResourceManager
        : IResourceManager
    {
        Dictionary<ShaderType, IShader> shaderCache;

        public ResourceManager()
        {
            shaderCache = new Dictionary<ShaderType, IShader>();

            shaderCache[ShaderType.Unlit] = CorShaders.CreateUnlit();
            shaderCache[ShaderType.VertexLit] = CorShaders.CreatePhongVertexLit();
            shaderCache[ShaderType.PixelLit] = CorShaders.CreatePhongPixelLit();
        }

        static string GetBundlePath(string path)
        {
            string rtype = Path.GetExtension(path);
            string rname = Path.Combine(
                Path.GetDirectoryName(path),
                Path.GetFileNameWithoutExtension(path));

            var correctPath =
                global::MonoMac.Foundation.NSBundle.MainBundle.PathForResource(rname, rtype);

            if(!File.Exists(correctPath))
            {
                throw new FileNotFoundException(correctPath);
            }

            return correctPath;
        }

        public T Load<T>(string path) where T : IResource
        {
            path = GetBundlePath(path);


            if(typeof(T) == typeof(Texture2D))
            {
                var tex = OpenGLTexture.CreateFromFile(path);

                return (T)(IResource) tex;
            }

            throw new NotImplementedException();
        }

        public T Open<T>(string path) where T : IDisposable
        {
            path = GetBundlePath(path);

            if(typeof(T) == typeof(StreamReader))
            {
                return (T)(IDisposable) new StreamReader(path);
            }

            throw new NotImplementedException();
        }

        public IShader LoadShader(ShaderType shaderType)
        {
            if( !shaderCache.ContainsKey(shaderType) )
            {
                throw new NotImplementedException();
            }

            return shaderCache[shaderType];
        }
    }

    public sealed class PanelSpecification
        : IPanelSpecification
    {
        public PanelSpecification()
        {
            InternalUtils.Log.Info(
                "PanelSpecification -> ()");
        }

        #region IPanelSpecification

        public Vector2 PanelPhysicalSize
        {
            get { return new Vector2(0.20f, 0.15f ); }
        }

        public Single PanelPhysicalAspectRatio
        {
            get { return PanelPhysicalSize.X / PanelPhysicalSize.Y; }
        }

        public PanelType PanelType { get { return PanelType.TouchScreen; } }

        #endregion
    }

    public sealed class ScreenSpecification
        : IScreenSpecification
    {
        Int32 width = 800;
        Int32 height = 600;

        public ScreenSpecification()
        {
            InternalUtils.Log.Info(
                "ScreenSpecification -> ()");
        }

        #region IScreenSpecification

        public Int32 ScreenResolutionWidth
        {
            get { return width; }
        }

        public Int32 ScreenResolutionHeight
        {
            get { return height; }
        }

        public Single ScreenResolutionAspectRatio
        {
            get
            {
                return
                    (Single)this.ScreenResolutionWidth /
                    (Single)this.ScreenResolutionHeight;
            }
        }

        #endregion
    }

    public sealed class GeometryBuffer
        : IGeometryBuffer
    {
        IndexBuffer _iBuf;
        VertexBuffer _vBuf;
        
        public GeometryBuffer (VertexDeclaration vertexDeclaration, Int32 vertexCount, Int32 indexCount)
        {

            if(vertexCount == 0)
            {
                throw new Exception("A geometry buffer must have verts");
            }

            if( indexCount != 0 )
            {
                _iBuf = new IndexBuffer(indexCount);
            }

            _vBuf = new VertexBuffer(vertexDeclaration, vertexCount);

        }

        internal void Activate()
        {
            _vBuf.Activate();

            if( _iBuf != null )
                _iBuf.Activate();
        }

        internal void Deactivate()
        {
            _vBuf.Deactivate();

            if( _iBuf != null )
                _iBuf.Deactivate();
        }


        
        public IVertexBuffer VertexBuffer { get { return _vBuf; } }
        public IIndexBuffer IndexBuffer { get { return _iBuf; } }

        internal VertexBuffer OpenTKVertexBuffer { get { return _vBuf; } }
    }

    public sealed class SystemManager
        : ISystemManager
    {
        readonly IScreenSpecification screen;
        readonly IPanelSpecification panel;

        public SystemManager()
        {
            InternalUtils.Log.Info(
                "SystemManager -> ()");

            screen = new ScreenSpecification();
            panel = new PanelSpecification();
        }

        void GetEffectiveDisplaySize(
            ref Int32 screenSpecWidth,
            ref Int32 screenSpecHeight)
        {
            if (this.CurrentOrientation == DeviceOrientation.Default ||
                this.CurrentOrientation == DeviceOrientation.Upsidedown )
            {
                return;
            }
            else
            {
                Int32 temp = screenSpecWidth;
                screenSpecWidth = screenSpecHeight;
                screenSpecHeight = temp;
            }
        }

        #region ISystemManager

        public String OperatingSystem { get { return " OS 2013"; } }

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

        public String DeviceName { get { return "The New  Pad"; } }

        public String DeviceModel { get { return "xf4bs2013"; } }

        public String SystemName { get { return "Sungiant's System"; } }

        public String SystemVersion { get { return "1314.0.1.29"; } }

        public DeviceOrientation CurrentOrientation
        {
            get { return DeviceOrientation.Default; }
        }

        public IScreenSpecification ScreenSpecification
        {
            get { return this.screen; }
        }

        public IPanelSpecification PanelSpecification
        {
            get { return this.panel; }
        }

        #endregion
    }

    public sealed class VertexBuffer
        : IVertexBuffer
        , IDisposable
    {
        Int32 resourceCounter;
        VertexDeclaration vertDecl;

        Int32 vertexCount;

        UInt32 bufferHandle;

        global::MonoMac.OpenGL.BufferTarget type;
        global::MonoMac.OpenGL.BufferUsageHint bufferUsage;

        public VertexBuffer (VertexDeclaration vd, Int32 vertexCount)
        {
            this.vertDecl = vd;
            this.vertexCount = vertexCount;

            this.type = global::MonoMac.OpenGL.BufferTarget.ArrayBuffer;

            this.bufferUsage = global::MonoMac.OpenGL.BufferUsageHint.DynamicDraw;

            global::MonoMac.OpenGL.GL.GenBuffers(1, out this.bufferHandle);
            ErrorHandler.Check();


            if( this.bufferHandle == 0 )
            {
                throw new Exception("Failed to generate vert buffer.");
            }
            

            this.Activate();

            global::MonoMac.OpenGL.GL.BufferData(
                this.type,
                (System.IntPtr) (vertDecl.VertexStride * this.vertexCount),
                (System.IntPtr) null,
                this.bufferUsage);

            ErrorHandler.Check();

            resourceCounter++;

        }

        internal void Activate()
        {
            global::MonoMac.OpenGL.GL.BindBuffer(this.type, this.bufferHandle);
            ErrorHandler.Check();
        }

        internal void Deactivate()
        {
            global::MonoMac.OpenGL.GL.BindBuffer(this.type, 0);
            ErrorHandler.Check();
        }

        ~VertexBuffer()
        {
            CleanUpNativeResources();
        }

        void CleanUpManagedResources()
        {

        }

        void CleanUpNativeResources()
        {
            global::MonoMac.OpenGL.GL.DeleteBuffers(1, ref this.bufferHandle);
            ErrorHandler.Check();

            bufferHandle = 0;

            resourceCounter--;
        }

        public void Dispose()
        {
            CleanUpManagedResources();
            CleanUpNativeResources();
            GC.SuppressFinalize(this);
        }

        public void SetData<T> (T[] data)
            where T: struct, IVertexType
        {
            if( data.Length != vertexCount )
            {
                throw new Exception("?");
            }
            
            this.Activate();

            // glBufferData FN will reserve appropriate data storage based on the value of size.  The data argument can
            // be null indicating that the reserved data store remains uninitiliazed.  If data is a valid pointer,
            // then content of data are copied to the allocated data store.  The contents of the buffer object data
            // store can be initialized or updated using the glBufferSubData FN
            global::MonoMac.OpenGL.GL.BufferSubData(
                this.type,
                (System.IntPtr) 0,
                (System.IntPtr) (vertDecl.VertexStride * this.vertexCount),
                data);

            ErrorHandler.Check();
        }


        public Int32 VertexCount
        {
            get
            {
                return this.vertexCount;
            }
        }

        public VertexDeclaration VertexDeclaration 
        {
            get
            {
                return this.vertDecl;
            }
        } 

        public void GetData<T> (T[] data) where T: struct, IVertexType { throw new System.NotSupportedException(); }
        
        public void GetData<T> (T[] data, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType { throw new System.NotSupportedException(); }
        
        public void GetData<T> (Int32 offsetInBytes, T[] data, Int32 startIndex, Int32 elementCount, Int32 vertexStride) where T: struct, IVertexType { throw new System.NotSupportedException(); }
        
        public void SetData<T> (T[] data, Int32 startIndex, Int32 elementCount) where T: struct, IVertexType { throw new System.NotSupportedException(); }
        
        public void SetData<T> (Int32 offsetInBytes, T[] data, Int32 startIndex, Int32 elementCount, Int32 vertexStride) where T: struct, IVertexType { throw new System.NotSupportedException(); }

    }

    public sealed class MonoMacApp
        : IDisposable
    {
        MacGameNSWindow mainWindow;
        OpenGLView openGLView;
        readonly AppSettings settings;
        readonly IApp entryPoint;

        public MonoMacApp(AppSettings settings, IApp entryPoint)
        {
            this.settings = settings;
            this.entryPoint = entryPoint;
        }

        void InitializeMainWindow()
        {
            RectangleF frame = new RectangleF(
                0, 0,
                800,
                600);

            mainWindow = new MacGameNSWindow(
                frame,
                NSWindowStyle.Titled | NSWindowStyle.Closable | NSWindowStyle.Miniaturizable,
                NSBackingStore.Buffered,
                true);

            mainWindow.WindowController = new NSWindowController(mainWindow);
            mainWindow.Delegate = new MainWindowDelegate(this);

            mainWindow.IsOpaque = true;
            mainWindow.EnableCursorRects();
            mainWindow.AcceptsMouseMovedEvents = false;
            mainWindow.Center();

            openGLView = new OpenGLView(this.settings, this.entryPoint, frame);

            mainWindow.ContentView.AddSubview(openGLView);

            mainWindow.MakeKeyAndOrderFront(mainWindow);
            
            openGLView.StartRunLoop(60f);
        }

        public void Run()
        {
            InitializeMainWindow();
        }

        public void Dispose()
        {
            // No need to dispose openGLView or mainWindow.  They will be released by the
            // nearest NSAutoreleasePool.
        }

        float GetTitleBarHeight()
        {
            RectangleF contentRect = NSWindow.ContentRectFor(
                mainWindow.Frame, mainWindow.StyleMask);

            return mainWindow.Frame.Height - contentRect.Height;
        }
    }

    public sealed class OpenGLView
        : global::MonoMac.OpenGL.MonoMacGameView
    {
        Rectangle clientBounds;
        NSTrackingArea trackingArea;
        bool _needsToResetElapsedTime = false;
        //Scene scene;

        Engine gameEngine;
        Stopwatch timer = new Stopwatch();
        Single elapsedTime;
        Int64 frameCounter = -1;
        TimeSpan previousTimeSpan;
        Int32 frameInterval;

        uint _depthRenderbuffer;

        readonly AppSettings settings;
        readonly IApp entryPoint;


        public OpenGLView(AppSettings settings, IApp entryPoint, RectangleF frame)
            : base (frame)
        {
            this.settings = settings;
            this.entryPoint = entryPoint;

            this.AutoresizingMask =
                global::MonoMac.AppKit.NSViewResizingMask.HeightSizable |
                global::MonoMac.AppKit.NSViewResizingMask.MaxXMargin |
                global::MonoMac.AppKit.NSViewResizingMask.MinYMargin |
                global::MonoMac.AppKit.NSViewResizingMask.WidthSizable;

            RectangleF rect = NSScreen.MainScreen.Frame;
            clientBounds = new Rectangle (0,0,(int)rect.Width,(int)rect.Height);



            Resize += delegate {
                //scene.ResizeGLScene(Bounds);
            };

            Load += OnLoad;

            UpdateFrame += delegate(object src, global::MonoMac.OpenGL.FrameEventArgs fea) {

                Single dt = (Single)(timer.Elapsed.TotalSeconds - previousTimeSpan.TotalSeconds);
                previousTimeSpan = timer.Elapsed;

                if (dt > 0.5f)
                {
                    dt = 0.0f;
                }

                elapsedTime += dt;

                var appTime = new AppTime(dt, elapsedTime, ++frameCounter);

                gameEngine.Update(appTime);
            };

            RenderFrame += delegate(object src, global::MonoMac.OpenGL.FrameEventArgs fea) {

                gameEngine.Render();
            };

        }

        void OnLoad (object src, EventArgs fea)
        {
            //CreateFrameBuffer();
            //scene = new Scene();

            gameEngine = new Engine(
                this.settings,
                this.entryPoint
                //this,
                //this.GraphicsContext,
                //this.touchState
                );
            timer.Start();

            //InternalUtils.Log.Info("load ");
            //InitGL();
            //UpdateView();

        }

        // All Setup For OpenGL Goes Here
        bool InitGL ()
        {
            // Enables Smooth Shading
            global::MonoMac.OpenGL.GL.ShadeModel (global::MonoMac.OpenGL.ShadingModel.Smooth);
            // Set background color to black
            global::MonoMac.OpenGL.GL.ClearColor (Color.Black);

            // Setup Depth Testing

            // Depth Buffer setup
            global::MonoMac.OpenGL.GL.ClearDepth (1.0);
            // Enables Depth testing
            global::MonoMac.OpenGL.GL.Enable (global::MonoMac.OpenGL.EnableCap.DepthTest);
            // The type of depth testing to do
            global::MonoMac.OpenGL.GL.DepthFunc (global::MonoMac.OpenGL.DepthFunction.Lequal);

            // Really Nice Perspective Calculations
            global::MonoMac.OpenGL.GL.Hint (global::MonoMac.OpenGL.HintTarget.PerspectiveCorrectionHint, global::MonoMac.OpenGL.HintMode.Nicest);

            return true;
        }

        //[Export("toggleFullScreen:")]
        void toggleFullScreen (NSObject sender)
        {
            if (WindowState == global::MonoMac.OpenGL.WindowState.Fullscreen)
                WindowState = global::MonoMac.OpenGL.WindowState.Normal;
            else
                WindowState = global::MonoMac.OpenGL.WindowState.Fullscreen;
        }

        void CreateFrameBuffer()
        {
            //
            // Enable the depth buffer
            //
            global::MonoMac.OpenGL.GL.GenRenderbuffers(1, out _depthRenderbuffer);
            ErrorHandler.Check();

            global::MonoMac.OpenGL.GL.BindRenderbuffer(
                global::MonoMac.OpenGL.RenderbufferTarget.Renderbuffer,
                _depthRenderbuffer);
            ErrorHandler.Check();

            global::MonoMac.OpenGL.GL.RenderbufferStorage(
                global::MonoMac.OpenGL.RenderbufferTarget.Renderbuffer,
                global::MonoMac.OpenGL.RenderbufferStorage.DepthComponent16,
                Size.Width, Size.Height);
            ErrorHandler.Check();

            global::MonoMac.OpenGL.GL.FramebufferRenderbuffer(
                global::MonoMac.OpenGL.FramebufferTarget.Framebuffer,
                global::MonoMac.OpenGL.FramebufferAttachment.DepthAttachment,
                global::MonoMac.OpenGL.RenderbufferTarget.Renderbuffer,
                _depthRenderbuffer);
            ErrorHandler.Check();

        }

        public void StartRunLoop(double updateRate)
        {
            Run(updateRate);
        }

        public void ResetElapsedTime ()
        {
            _needsToResetElapsedTime = true;
        }

        protected override void OnRenderFrame (global::MonoMac.OpenGL.FrameEventArgs e)
        {
            base.OnRenderFrame (e);
            // tick
        }

        public override bool AcceptsFirstResponder ()
        {
            // We want this view to be able to receive key events
            return true;
        }

        public override bool BecomeFirstResponder ()
        {
            return true;
        }

        public override void ViewWillMoveToWindow (NSWindow newWindow)
        {
            if (trackingArea != null)
                RemoveTrackingArea(trackingArea);

            trackingArea = new NSTrackingArea(
                Frame,
                NSTrackingAreaOptions.MouseMoved |
                NSTrackingAreaOptions.MouseEnteredAndExited |
                NSTrackingAreaOptions.EnabledDuringMouseDrag |
                NSTrackingAreaOptions.ActiveWhenFirstResponder |
                NSTrackingAreaOptions.InVisibleRect |
                NSTrackingAreaOptions.CursorUpdate,
                this,
                new NSDictionary()
            );

            AddTrackingArea(trackingArea);

        }

        // Keyboard //--------------------------------------------------------//
        public override void KeyDown (NSEvent theEvent)
        {
            this.gameEngine.InputImplementation.KeyboardImplemenatation.KeyDown (theEvent);
        }

        public override void KeyUp (NSEvent theEvent)
        {
            this.gameEngine.InputImplementation.KeyboardImplemenatation.KeyUp (theEvent);
        }

        public override void FlagsChanged (NSEvent theEvent)
        {
            base.FlagsChanged (theEvent);
        }


        // Mouse //-----------------------------------------------------------//
        public override void MouseDown (NSEvent theEvent)
        {
            base.MouseDown (theEvent);
        }

        public override void MouseUp (NSEvent theEvent)
        {
            base.MouseUp (theEvent);
        }

        public override void MouseDragged (NSEvent theEvent)
        {
            base.MouseDragged (theEvent);
        }

        public override void RightMouseDown (NSEvent theEvent)
        {
            base.RightMouseDown (theEvent);
        }

        public override void RightMouseUp (NSEvent theEvent)
        {
            base.RightMouseUp (theEvent);
        }

        public override void RightMouseDragged (NSEvent theEvent)
        {
            base.RightMouseDragged (theEvent);
        }

        public override void OtherMouseDown (NSEvent theEvent)
        {
            base.OtherMouseDown (theEvent);
        }

        public override void OtherMouseUp (NSEvent theEvent)
        {
            base.OtherMouseUp (theEvent);
        }

        public override void OtherMouseDragged (NSEvent theEvent)
        {
            base.OtherMouseDragged (theEvent);
        }

        public override void ScrollWheel (NSEvent theEvent)
        {
            base.ScrollWheel (theEvent);
        }

        public override void MouseMoved (NSEvent theEvent)
        {
            base.MouseMoved (theEvent);
        }
    }

    public sealed class MacGameNSWindow 
        : NSWindow
    {
        [Export ("initWithContentRect:styleMask:backing:defer:")]
        public MacGameNSWindow (
            RectangleF rect, 
            NSWindowStyle style, 
            NSBackingStore backing, 
            Boolean defer)
            : base (rect, style, backing, defer)
        {

        }

        public override Boolean CanBecomeKeyWindow
        {
            get
            {
                return true;
            }
        }
    }

    internal sealed class MainWindowDelegate 
        : NSWindowDelegate
    {
        private readonly MonoMacApp owner;

        public MainWindowDelegate(MonoMacApp owner)
        {
            if (owner == null)
                throw new ArgumentNullException("owner");
            this.owner = owner;
        }

        public override bool ShouldZoom (NSWindow window, RectangleF newFrame)
        {
            return true;
        }
    }

    internal static class Vector2Converter
    {
        // VECTOR 2
        internal static global::MonoMac.OpenGL.Vector2 ToOpenGL (this Vector2 vec)
        {
            return new global::MonoMac.OpenGL.Vector2 (vec.X, vec.Y);
        }

        internal static Vector2 ToAbacus (this global::MonoMac.OpenGL.Vector2 vec)
        {
            return new Vector2 (vec.X, vec.Y);
        }

        
        internal static System.Drawing.PointF ToSystemDrawing(this Vector2 vec)
        {
            return new System.Drawing.PointF (vec.X, vec.Y);
        }

        internal static Vector2 ToAbacus (this System.Drawing.PointF vec)
        {
            return new Vector2 (vec.X, vec.Y);
        }
    }

    
    internal static class Vector3Converter
    {
        // VECTOR 3
        internal static global::MonoMac.OpenGL.Vector3 ToOpenGL (this Vector3 vec)
        {
            return new global::MonoMac.OpenGL.Vector3 (vec.X, vec.Y, vec.Z);
        }

        internal static Vector3 ToAbacus (this global::MonoMac.OpenGL.Vector3 vec)
        {
            return new Vector3 (vec.X, vec.Y, vec.Z);
        }
    }
    
    internal static class Vector4Converter
    {
        // VECTOR 3
        internal static global::MonoMac.OpenGL.Vector4 ToOpenGL (this Vector4 vec)
        {
            return new global::MonoMac.OpenGL.Vector4 (vec.X, vec.Y, vec.Z, vec.W);
        }

        internal static Vector4 ToAbacus (this global::MonoMac.OpenGL.Vector4 vec)
        {
            return new Vector4 (vec.X, vec.Y, vec.Z, vec.W);
        }
    }

    internal static class MatrixConverter
    {
        static bool flip = false;

        // MATRIX
        internal static global::MonoMac.OpenGL.Matrix4 ToOpenGL (this Matrix44 mat)
        {
            if( flip )
            {
                return new global::MonoMac.OpenGL.Matrix4(
                    mat.M11, mat.M21, mat.M31, mat.M41,
                    mat.M12, mat.M22, mat.M32, mat.M42,
                    mat.M13, mat.M23, mat.M33, mat.M43,
                    mat.M14, mat.M24, mat.M34, mat.M44
                    );
            }
            else
            {
                return new global::MonoMac.OpenGL.Matrix4(
                    mat.M11, mat.M12, mat.M13, mat.M14,
                    mat.M21, mat.M22, mat.M23, mat.M24,
                    mat.M31, mat.M32, mat.M33, mat.M34,
                    mat.M41, mat.M42, mat.M43, mat.M44
                    );
            }
        }

        internal static Matrix44 ToAbacus (this global::MonoMac.OpenGL.Matrix4 mat)
        {

            if( flip )
            {
                return new Matrix44(
                    mat.M11, mat.M21, mat.M31, mat.M41,
                    mat.M12, mat.M22, mat.M32, mat.M42,
                    mat.M13, mat.M23, mat.M33, mat.M43,
                    mat.M14, mat.M24, mat.M34, mat.M44
                    );
            }
            else
            {
                return new Matrix44(
                    mat.M11, mat.M12, mat.M13, mat.M14,
                    mat.M21, mat.M22, mat.M23, mat.M24,
                    mat.M31, mat.M32, mat.M33, mat.M34,
                    mat.M41, mat.M42, mat.M43, mat.M44
                    );
            }
        }

    }


    internal static class EnumConverter
    {
        internal static global::MonoMac.OpenGL.TextureUnit ToOpenGLTextureSlot(Int32 slot)
        {
            switch(slot)
            {
                case 0: return global::MonoMac.OpenGL.TextureUnit.Texture0;
                case 1: return global::MonoMac.OpenGL.TextureUnit.Texture1;
                case 2: return global::MonoMac.OpenGL.TextureUnit.Texture2;
                case 3: return  global::MonoMac.OpenGL.TextureUnit.Texture3;
                case 4: return global::MonoMac.OpenGL.TextureUnit.Texture4;
                case 5: return global::MonoMac.OpenGL.TextureUnit.Texture5;
                case 6: return global::MonoMac.OpenGL.TextureUnit.Texture6;
                case 7: return global::MonoMac.OpenGL.TextureUnit.Texture7;
                case 8: return global::MonoMac.OpenGL.TextureUnit.Texture8;
                case 9: return global::MonoMac.OpenGL.TextureUnit.Texture9;
                case 10: return global::MonoMac.OpenGL.TextureUnit.Texture10;
                case 11: return global::MonoMac.OpenGL.TextureUnit.Texture11;
                case 12: return global::MonoMac.OpenGL.TextureUnit.Texture12;
                case 13: return global::MonoMac.OpenGL.TextureUnit.Texture13;
                case 14: return global::MonoMac.OpenGL.TextureUnit.Texture14;
                case 15: return global::MonoMac.OpenGL.TextureUnit.Texture15;
                case 16: return global::MonoMac.OpenGL.TextureUnit.Texture16;
                case 17: return global::MonoMac.OpenGL.TextureUnit.Texture17;
                case 18: return global::MonoMac.OpenGL.TextureUnit.Texture18;
                case 19: return global::MonoMac.OpenGL.TextureUnit.Texture19;
                case 20: return global::MonoMac.OpenGL.TextureUnit.Texture20;
                case 21: return global::MonoMac.OpenGL.TextureUnit.Texture21;
                case 22: return global::MonoMac.OpenGL.TextureUnit.Texture22;
                case 23: return global::MonoMac.OpenGL.TextureUnit.Texture23;
                case 24: return global::MonoMac.OpenGL.TextureUnit.Texture24;
                case 25: return global::MonoMac.OpenGL.TextureUnit.Texture25;
                case 26: return global::MonoMac.OpenGL.TextureUnit.Texture26;
                case 27: return global::MonoMac.OpenGL.TextureUnit.Texture27;
                case 28: return global::MonoMac.OpenGL.TextureUnit.Texture28;
                case 29: return global::MonoMac.OpenGL.TextureUnit.Texture29;
                case 30: return global::MonoMac.OpenGL.TextureUnit.Texture30;
            }

            throw new NotSupportedException();
        }


        internal static Type ToType (global::MonoMac.OpenGL.ActiveAttribType ogl)
        {
            switch(ogl)
            {
            case global::MonoMac.OpenGL.ActiveAttribType.Float: return typeof(Single);
            case global::MonoMac.OpenGL.ActiveAttribType.FloatMat2: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveAttribType.FloatMat3: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveAttribType.FloatMat4: return typeof(Matrix44);
            case global::MonoMac.OpenGL.ActiveAttribType.FloatVec2: return typeof(Vector2);
            case global::MonoMac.OpenGL.ActiveAttribType.FloatVec3: return typeof(Vector3);
            case global::MonoMac.OpenGL.ActiveAttribType.FloatVec4: return typeof(Vector4);
            }

            throw new NotSupportedException();
        }

        internal static Type ToType (global::MonoMac.OpenGL.ActiveUniformType ogl)
        {
            switch(ogl)
            {
            case global::MonoMac.OpenGL.ActiveUniformType.Bool: return typeof(Boolean);
            case global::MonoMac.OpenGL.ActiveUniformType.BoolVec2: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveUniformType.BoolVec3: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveUniformType.BoolVec4: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveUniformType.Float: return typeof(Single);
            case global::MonoMac.OpenGL.ActiveUniformType.FloatMat2: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveUniformType.FloatMat3: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveUniformType.FloatMat4: return typeof(Matrix44);
            case global::MonoMac.OpenGL.ActiveUniformType.FloatVec2: return typeof(Vector2);
            case global::MonoMac.OpenGL.ActiveUniformType.FloatVec3: return typeof(Vector3);
            case global::MonoMac.OpenGL.ActiveUniformType.FloatVec4: return typeof(Vector4);
            case global::MonoMac.OpenGL.ActiveUniformType.Int: return typeof(Boolean);
            case global::MonoMac.OpenGL.ActiveUniformType.IntVec2: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveUniformType.IntVec3: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveUniformType.IntVec4: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveUniformType.Sampler2D: throw new NotSupportedException();
            case global::MonoMac.OpenGL.ActiveUniformType.SamplerCube: throw new NotSupportedException();
            }
            
            throw new NotSupportedException();
        }

        internal static void ToOpenGL (
            VertexElementFormat blimey,
            out global::MonoMac.OpenGL.VertexAttribPointerType dataFormat,
            out bool normalized,
            out int size)
        {
            normalized = false;
            size = 0;
            dataFormat = global::MonoMac.OpenGL.VertexAttribPointerType.Float;

            switch(blimey)
            {
                case VertexElementFormat.Single: 
                dataFormat = global::MonoMac.OpenGL.VertexAttribPointerType.Float;
                    size = 1;
                    break;
                case VertexElementFormat.Vector2: 
                dataFormat = global::MonoMac.OpenGL.VertexAttribPointerType.Float; 
                    size = 2;
                    break;
                case VertexElementFormat.Vector3: 
                dataFormat = global::MonoMac.OpenGL.VertexAttribPointerType.Float; 
                    size = 3;
                    break;
                case VertexElementFormat.Vector4: 
                dataFormat = global::MonoMac.OpenGL.VertexAttribPointerType.Float; 
                    size = 4;
                    break;
                case VertexElementFormat.Colour: 
                dataFormat = global::MonoMac.OpenGL.VertexAttribPointerType.UnsignedByte; 
                    normalized = true;
                    size = 4;
                    break;
                case VertexElementFormat.Byte4: throw new Exception("?");
                case VertexElementFormat.Short2: throw new Exception("?");
                case VertexElementFormat.Short4: throw new Exception("?");
                case VertexElementFormat.NormalisedShort2: throw new Exception("?");
                case VertexElementFormat.NormalisedShort4: throw new Exception("?");
                case VertexElementFormat.HalfVector2: throw new Exception("?");
                case VertexElementFormat.HalfVector4: throw new Exception("?");
            }
        }

        internal static global::MonoMac.OpenGL.BlendingFactorSrc ToOpenGLSrc(BlendFactor blimey)
        {
            switch(blimey)
            {
                case BlendFactor.Zero: return global::MonoMac.OpenGL.BlendingFactorSrc.Zero;
                case BlendFactor.One: return global::MonoMac.OpenGL.BlendingFactorSrc.One;
                case BlendFactor.SourceColour: return global::MonoMac.OpenGL.BlendingFactorSrc.Src1Color; // todo: check this src1 stuff
                case BlendFactor.InverseSourceColour: return global::MonoMac.OpenGL.BlendingFactorSrc.OneMinusSrc1Color;
                case BlendFactor.SourceAlpha: return global::MonoMac.OpenGL.BlendingFactorSrc.SrcAlpha;
                case BlendFactor.InverseSourceAlpha: return global::MonoMac.OpenGL.BlendingFactorSrc.OneMinusSrcAlpha;
                case BlendFactor.DestinationAlpha: return global::MonoMac.OpenGL.BlendingFactorSrc.DstAlpha;
                case BlendFactor.InverseDestinationAlpha: return global::MonoMac.OpenGL.BlendingFactorSrc.OneMinusDstAlpha;
                case BlendFactor.DestinationColour: return global::MonoMac.OpenGL.BlendingFactorSrc.DstColor;
                case BlendFactor.InverseDestinationColour: return global::MonoMac.OpenGL.BlendingFactorSrc.OneMinusDstColor;
            }

            throw new Exception();
        }

        internal static global::MonoMac.OpenGL.BlendingFactorDest ToOpenGLDest(BlendFactor blimey)
        {
            switch(blimey)
            {
                case BlendFactor.Zero: return global::MonoMac.OpenGL.BlendingFactorDest.Zero;
                case BlendFactor.One: return global::MonoMac.OpenGL.BlendingFactorDest.One;
                case BlendFactor.SourceColour: return global::MonoMac.OpenGL.BlendingFactorDest.SrcColor;
                case BlendFactor.InverseSourceColour: return global::MonoMac.OpenGL.BlendingFactorDest.OneMinusSrcColor;
                case BlendFactor.SourceAlpha: return global::MonoMac.OpenGL.BlendingFactorDest.SrcAlpha;
                case BlendFactor.InverseSourceAlpha: return global::MonoMac.OpenGL.BlendingFactorDest.OneMinusSrcAlpha;
                case BlendFactor.DestinationAlpha: return global::MonoMac.OpenGL.BlendingFactorDest.DstAlpha;
                case BlendFactor.InverseDestinationAlpha: return global::MonoMac.OpenGL.BlendingFactorDest.OneMinusDstAlpha;
                case BlendFactor.DestinationColour: return global::MonoMac.OpenGL.BlendingFactorDest.SrcColor;
                case BlendFactor.InverseDestinationColour: return global::MonoMac.OpenGL.BlendingFactorDest.OneMinusSrcColor;
            }
            
            throw new Exception();
        }

        internal static BlendFactor ToCorDestinationBlendFactor (global::MonoMac.OpenGL.All ogl)
        {
            switch(ogl)
            {
                case global::MonoMac.OpenGL.All.Zero: return BlendFactor.Zero;
                case global::MonoMac.OpenGL.All.One: return BlendFactor.One;
                case global::MonoMac.OpenGL.All.SrcColor: return BlendFactor.SourceColour;
                case global::MonoMac.OpenGL.All.OneMinusSrcColor: return BlendFactor.InverseSourceColour;
                case global::MonoMac.OpenGL.All.SrcAlpha: return BlendFactor.SourceAlpha;
                case global::MonoMac.OpenGL.All.OneMinusSrcAlpha: return BlendFactor.InverseSourceAlpha;
                case global::MonoMac.OpenGL.All.DstAlpha: return BlendFactor.DestinationAlpha;
                case global::MonoMac.OpenGL.All.OneMinusDstAlpha: return BlendFactor.InverseDestinationAlpha;
                case global::MonoMac.OpenGL.All.DstColor: return BlendFactor.DestinationColour;
                case global::MonoMac.OpenGL.All.OneMinusDstColor: return BlendFactor.InverseDestinationColour;
            }

            throw new Exception();
        }

        internal static global::MonoMac.OpenGL.BlendEquationMode ToOpenGL(BlendFunction blimey)
        {
            switch(blimey)
            {
                case BlendFunction.Add: return global::MonoMac.OpenGL.BlendEquationMode.FuncAdd;
                case BlendFunction.Max: throw new NotSupportedException();
                case BlendFunction.Min: throw new NotSupportedException();
                case BlendFunction.ReverseSubtract: return global::MonoMac.OpenGL.BlendEquationMode.FuncReverseSubtract;
                case BlendFunction.Subtract: return global::MonoMac.OpenGL.BlendEquationMode.FuncSubtract;
            }
            
            throw new Exception();
        }

        internal static BlendFunction ToCorDestinationBlendFunction (global::MonoMac.OpenGL.All ogl)
        {
            switch(ogl)
            {
                case global::MonoMac.OpenGL.All.FuncAdd: return BlendFunction.Add;
                case global::MonoMac.OpenGL.All.MaxExt: return BlendFunction.Max;
                case global::MonoMac.OpenGL.All.MinExt: return BlendFunction.Min;
                case global::MonoMac.OpenGL.All.FuncReverseSubtract: return BlendFunction.ReverseSubtract;
                case global::MonoMac.OpenGL.All.FuncSubtract: return BlendFunction.Subtract;
            }
            
            throw new Exception();
        }

        // PRIMITIVE TYPE
        internal static global::MonoMac.OpenGL.BeginMode ToOpenGL (PrimitiveType blimey)
        {
            switch (blimey) {
            case PrimitiveType.LineList:
                return  global::MonoMac.OpenGL.BeginMode.Lines;
            case PrimitiveType.LineStrip:
                return  global::MonoMac.OpenGL.BeginMode.LineStrip;
            case PrimitiveType.TriangleList:
                return  global::MonoMac.OpenGL.BeginMode.Triangles;
            case PrimitiveType.TriangleStrip:
                return  global::MonoMac.OpenGL.BeginMode.TriangleStrip;
                    
            default:
                throw new Exception ("problem");
            }
        }

        internal static PrimitiveType ToCorPrimitiveType (global::MonoMac.OpenGL.All ogl)
        {
            switch (ogl) {
            case global::MonoMac.OpenGL.All.Lines:
                return  PrimitiveType.LineList;
            case global::MonoMac.OpenGL.All.LineStrip:
                return  PrimitiveType.LineStrip;
            case global::MonoMac.OpenGL.All.Points:
                throw new Exception ("Not supported by Cor");
            case global::MonoMac.OpenGL.All.TriangleFan:
                throw new Exception ("Not supported by Cor");
            case global::MonoMac.OpenGL.All.Triangles:
                return  PrimitiveType.TriangleList;
            case global::MonoMac.OpenGL.All.TriangleStrip:
                return  PrimitiveType.TriangleStrip;
                
            default:
                throw new Exception ("problem");

            }
        }
    }

    public sealed class MonoMacGpuUtils
        : IGpuUtils
    {
        public MonoMacGpuUtils()
        {
        }

        #region IGpuUtils

        public Int32 BeginEvent(Rgba32 colour, String eventName)
        {
            return 0;
        }

        public Int32 EndEvent()
        {
            return 0;
        }

        public void SetMarker(Rgba32 colour, String eventName)
        {

        }

        public void SetRegion(Rgba32 colour, String eventName)
        {

        }

        #endregion
    }
    public sealed class StubShader
        : IShader
    {
        IShaderPass[] passes = new IShaderPass[0];
        VertexElementUsage[] requiredVertexElements = new VertexElementUsage[0];
        VertexElementUsage[] optionalVertexElements = new VertexElementUsage[0];

        #region IShader

        public void ResetVariables()
        {
            
        }

        public void ResetSamplerTargets()
        {
            
        }

        public void SetSamplerTarget(String name, Int32 textureSlot)
        {

        }

        public IShaderPass[] Passes { get { return passes; } }

        public VertexElementUsage[] RequiredVertexElements { get { return requiredVertexElements; } }

        public VertexElementUsage[] OptionalVertexElements { get { return optionalVertexElements; } }

        public String Name { get { return "StubShader"; } }

        public void SetVariable<T>(String name, T value)
        {

        }

        #endregion
    }

    /// <summary>
    /// The Cor.Xios implementation of Cor's IShader interface.
    /// </summary>
    public sealed class Shader
        : IShader
        , IDisposable
    {
        //static Dictionary<string, parp>


        #region IShader

        /// <summary>
        /// Resets all the shader's variables to their default values.
        /// </summary>
        public void ResetVariables()
        {
            // the shader definition defines the default values for the variables
            foreach (var variableDefinition in cachedShaderDefinition.VariableDefinitions)
            {
                string varName = variableDefinition.Name;
                object value = variableDefinition.DefaultValue;

                if( variableDefinition.Type == typeof(Matrix44) )
                {
                    this.SetVariable(varName, (Matrix44) value);
                }
                else if( variableDefinition.Type == typeof(Int32) )
                {
                    this.SetVariable(varName, (Int32) value);
                }
                else if( variableDefinition.Type == typeof(Single) )
                {
                    this.SetVariable(varName, (Single) value);
                }
                else if( variableDefinition.Type == typeof(Vector2) )
                {
                    this.SetVariable(varName, (Vector2) value);
                }
                else if( variableDefinition.Type == typeof(Vector3) )
                {
                    this.SetVariable(varName, (Vector3) value);
                }
                else if( variableDefinition.Type == typeof(Vector4) )
                {
                    this.SetVariable(varName, (Vector4) value);
                }
                else if( variableDefinition.Type == typeof(Rgba32) )
                {
                    this.SetVariable(varName, (Rgba32) value);
                }
                else
                {
                    throw new NotSupportedException();
                }

            }
        }

        /// <summary>
        /// Resets all the shader's texture samplers point at texture slot 0.
        /// </summary>
        public void ResetSamplerTargets()
        {
            foreach (var samplerDefinition in cachedShaderDefinition.SamplerDefinitions)
            {
                this.SetSamplerTarget(samplerDefinition.Name, 0);
            }
        }

        /// <summary>
        /// Sets the value of a specified shader variable.
        /// </summary>
        public void SetVariable<T>(string name, T value)
        {
            passes.ForEach( x => x.SetVariable(name, value));
        }

        /// <summary>
        /// Sets the texture slot that a texture sampler should sample from.
        /// </summary>
        public void SetSamplerTarget(string name, Int32 textureSlot)
        {
            foreach (var pass in passes)
            {
                pass.SetSamplerTarget(name, textureSlot);
            }
        }


        /// <summary>
        /// Provides access to the individual passes in this shader.
        /// the calling code can itterate though these and apply them
        ///to the graphics context before it makes a draw call.
        /// </summary>
        public IShaderPass[] Passes
        {
            get
            {
                return passes.ToArray();
            }
        }

        /// <summary>
        /// Defines which vertex elements are required by this shader.
        /// </summary>
        public VertexElementUsage[] RequiredVertexElements
        {
            get
            {
                // todo: an array of vert elem usage doesn't uniquely identify anything...
                return requiredVertexElements.ToArray();
            }
        }

        /// <summary>
        /// Defines which vertex elements are optionally used by this
        /// shader if they happen to be present.
        /// </summary>
        public VertexElementUsage[] OptionalVertexElements
        {
            get
            {
                // todo: an array of vert elem usage doesn't uniquely identify anything...
                return optionalVertexElements.ToArray();
            }
        }

        public String Name { get; private set; }

        #endregion

        #region IDisposable

        /// <summary>
        /// Releases all resource used by the <see cref="Sungiant.Cor.MonoTouchRuntime.Shader"/> object.
        /// </summary>
        public void Dispose()
        {
            foreach (var pass in passes)
            {
                pass.Dispose();
            }
        }

        #endregion


        List<VertexElementUsage> requiredVertexElements = new List<VertexElementUsage>();
        List<VertexElementUsage> optionalVertexElements = new List<VertexElementUsage>();


        /// <summary>
        /// The <see cref="ShaderPass"/> objects that need to each, in turn,  be individually activated and used to
        /// draw with to apply the effect of this containing <see cref="Shader"/> object.
        /// </summary>
        List<ShaderPass> passes = new List<ShaderPass>();

        /// <summary>
        /// Cached reference to the <see cref="ShaderDefinition"/> object used
        /// to create this <see cref="Shader"/> object.
        /// </summary>
        readonly ShaderDefinition cachedShaderDefinition;

        public ShaderDefinition ShaderDefinition { get { return cachedShaderDefinition; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Shader"/> class from a
        /// <see cref="ShaderDefinition"/> object.
        /// </summary>
        internal Shader (ShaderDefinition shaderDefinition)
        {
            InternalUtils.Log.Info("\n");
            InternalUtils.Log.Info("\n");
            InternalUtils.Log.Info("=====================================================================");
            InternalUtils.Log.Info("Creating Shader: " + shaderDefinition.Name);
            this.cachedShaderDefinition = shaderDefinition;
            this.Name = shaderDefinition.Name;
            CalculateRequiredInputs(shaderDefinition);
            InitilisePasses (shaderDefinition);

            this.ResetVariables();
        }

        /// <summary>
        /// Works out and caches a copy of which shader inputs are required/optional, needed as the
        /// <see cref="IShader"/> interface requires this information.
        /// </summary>
        void CalculateRequiredInputs(ShaderDefinition shaderDefinition)
        {
            foreach (var input in shaderDefinition.InputDefinitions)
            {
                if( input.Optional )
                {
                    optionalVertexElements.Add(input.Usage);
                }
                else
                {
                    requiredVertexElements.Add(input.Usage);
                }
            }
        }

        /// <summary>
        /// Triggers the creation of all of this <see cref="Shader"/> object's passes.
        /// </summary>
        void InitilisePasses(ShaderDefinition shaderDefinition)
        {
            // This function builds up an in memory object for each shader pass in this shader.
            // The different shader varients are defined outside of the scope of a conceptual shader pass,
            // therefore this function must traverse the shader definition and to create shader pass objects
            // that only contain the varient data for that specific pass.


            // For each named shader pass.
            foreach (var definedPassName in shaderDefinition.PassNames)
            {

                InternalUtils.Log.Info(" Preparing to initilising Shader Pass: " + definedPassName);
                //

                // itterate over the defined pass names, ex: cel, outline...



                //shaderDefinition.VariantDefinitions
                //  .Select(x => x.PassDefinitions.Select(y => y.PassName == definedPassName))
                //  .ToList();

                // Find all of the variants that are defined in this shader object's definition
                // that support the current shaderpass.
                var passVariants___Name_AND_passVariantDefinition = new List<Tuple<string, ShaderVarientPassDefinition>>();

                // itterate over every shader variant in the definition
                foreach (var shaderVariantDefinition in shaderDefinition.VariantDefinitions)
                {
                    // each shader varient has a name
                    string shaderVariantName = shaderVariantDefinition.VariantName;

                    // find the pass in the shader variant definition that corresponds to the pass we are
                    // currently trying to initilise.
                    var variantPassDefinition =
                        shaderVariantDefinition.VariantPassDefinitions
                            .Find(x => x.PassName == definedPassName);


                    // now we have a Variant name, say:
                    //   - Unlit_PositionTextureColour
                    // and a pass definition, say :
                    //   - Main
                    //   - Shaders/Unlit_PositionTextureColour.vsh
                    //   - Shaders/Unlit_PositionTextureColour.fsh
                    //

                    passVariants___Name_AND_passVariantDefinition.Add(
                        new Tuple<string, ShaderVarientPassDefinition>(shaderVariantName, variantPassDefinition));

                }

                // Create one shader pass for each defined pass name.
                var shaderPass = new ShaderPass( definedPassName, passVariants___Name_AND_passVariantDefinition );

                shaderPass.BindAttributes (shaderDefinition.InputDefinitions.Select(x => x.Name).ToList());
                shaderPass.Link ();
                shaderPass.ValidateInputs(shaderDefinition.InputDefinitions);
                shaderPass.ValidateVariables(shaderDefinition.VariableDefinitions);
                shaderPass.ValidateSamplers(shaderDefinition.SamplerDefinitions);

                passes.Add(shaderPass);
            }
        }
    }

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

    public static class ShaderHelper
    {
        /// <summary>
        /// This function takes a VertexDeclaration and a collection of OpenGL shader passes and works out which
        /// pass is the best fit for the VertexDeclaration.
        /// </summary>
        public static OpenGLShader WorkOutBestVariantFor(VertexDeclaration vertexDeclaration, IList<OpenGLShader> variants)
        {
            InternalUtils.Log.Info("\n");
            InternalUtils.Log.Info("\n");
            InternalUtils.Log.Info("=====================================================================");
            InternalUtils.Log.Info("Working out the best shader variant for: " + vertexDeclaration);
            InternalUtils.Log.Info("Possible variants:");

            int best = 0;

            int bestNumMatchedVertElems = 0;
            int bestNumUnmatchedVertElems = 0;
            int bestNumMissingNonOptionalInputs = 0;

            // foreach variant
            for (int i = 0; i < variants.Count; ++i)
            {
                // work out how many vert inputs match


                var matchResult = CompareShaderInputs(vertexDeclaration, variants[i]);

                int numMatchedVertElems = matchResult.NumMatchedInputs;
                int numUnmatchedVertElems = matchResult.NumUnmatchedInputs;
                int numMissingNonOptionalInputs = matchResult.NumUnmatchedRequiredInputs;

                InternalUtils.Log.Info(" - " + variants[i]);

                if( i == 0 )
                {
                    bestNumMatchedVertElems = numMatchedVertElems;
                    bestNumUnmatchedVertElems = numUnmatchedVertElems;
                    bestNumMissingNonOptionalInputs = numMissingNonOptionalInputs;
                }
                else
                {
                    if(
                        (
                            numMatchedVertElems > bestNumMatchedVertElems &&
                            bestNumMissingNonOptionalInputs == 0
                        )
                        ||
                        (
                            numMatchedVertElems == bestNumMatchedVertElems &&
                            bestNumMissingNonOptionalInputs == 0 &&
                            numUnmatchedVertElems < bestNumUnmatchedVertElems
                        )
                      )
                    {
                        bestNumMatchedVertElems = numMatchedVertElems;
                        bestNumUnmatchedVertElems = numUnmatchedVertElems;
                        bestNumMissingNonOptionalInputs = numMissingNonOptionalInputs;
                        best = i;
                    }

                }

            }

            //best = 2;
            InternalUtils.Log.Info("Chosen variant: " + variants[best].VariantName);

            return variants[best];
        }

        struct CompareShaderInputsResult
        {
            // the nume
            public int NumMatchedInputs;
            public int NumUnmatchedInputs;
            public int NumUnmatchedRequiredInputs;
        }

        static CompareShaderInputsResult CompareShaderInputs (
            VertexDeclaration vertexDeclaration,
            OpenGLShader oglesShader
            )
        {
            var result = new CompareShaderInputsResult();

            var oglesShaderInputsUsed = new List<OpenGLShaderInput>();

            var vertElems = vertexDeclaration.GetVertexElements();

            // itterate over each input defined in the vert decl
            foreach(var vertElem in vertElems)
            {
                var usage = vertElem.VertexElementUsage;

                var format = vertElem.VertexElementFormat;
                /*

                foreach( var input in oglesShader.Inputs )
                {
                    // the vertDecl knows what each input's intended use is,
                    // so lets match up
                    if( input.Usage == usage )
                    {
                        // intended use seems good
                    }
                }

                // find all inputs that could match
                var matchingInputs = oglesShader.Inputs.FindAll(
                    x =>

                        x.Usage == usage &&
                        (x.Type == VertexElementFormatHelper.FromEnum(format) ||
                        ( (x.Type.GetType() == typeof(Vector4)) && (format == VertexElementFormat.Colour) ))

                 );*/

                var matchingInputs = oglesShader.Inputs.FindAll(x => x.Usage == usage);

                // now make sure it's not been used already

                while(matchingInputs.Count > 0)
                {
                    var potentialInput = matchingInputs[0];

                    if( oglesShaderInputsUsed.Find(x => x == potentialInput) != null)
                    {
                        matchingInputs.RemoveAt(0);
                    }
                    else
                    {
                        oglesShaderInputsUsed.Add(potentialInput);
                    }
                }
            }

            result.NumMatchedInputs = oglesShaderInputsUsed.Count;

            result.NumUnmatchedInputs = vertElems.Length - result.NumMatchedInputs;

            result.NumUnmatchedRequiredInputs = 0;

            foreach (var input in oglesShader.Inputs)
            {
                if(!oglesShaderInputsUsed.Contains(input) )
                {
                    if( !input.Optional )
                    {
                        result.NumUnmatchedRequiredInputs++;
                    }
                }

            }

            InternalUtils.Log.Info(string.Format("[{0}, {1}, {2}]", result.NumMatchedInputs, result.NumUnmatchedInputs, result.NumUnmatchedRequiredInputs));
            return result;
        }

    }

    /// <summary>
    /// Represents in individual pass of a Cor.Xios high level Shader object.
    /// </summary>
    public sealed class ShaderPass
        : IShaderPass
        , IDisposable
    {
        /// <summary>
        /// A collection of OpenGL shaders, all with slight variations in their
        /// input parameters, that are suitable for rendering this ShaderPass object.
        /// </summary>
        List<OpenGLShader> Variants { get; set; }

        /// <summary>
        /// A nice name for the shader pass, for example: Main or Cel -> Outline.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Whenever this ShaderPass object gets asked to activate itself whilst a VertexDeclaration it has not seen
        /// before is active, the best matching shader pass variant is found and then stored in this map to fast
        /// access.
        /// </summary>
        Dictionary<VertexDeclaration, OpenGLShader> BestVariantMap { get; set; }

        Dictionary<String, Object>  currentVariables = new Dictionary<String, Object>();
        Dictionary<String, Int32>   currentSamplerSlots = new Dictionary<String, Int32>();

        Dictionary<String, bool> logHistory = new Dictionary<String, bool>();

        internal void SetVariable<T>(string name, T value)
        {
            currentVariables[name] = value;
        }

        internal void SetSamplerTarget(string name, Int32 textureSlot)
        {
            currentSamplerSlots[name] = textureSlot;
        }

        public ShaderPass(string passName, List<Tuple<string, ShaderVarientPassDefinition>> passVariants___Name_AND_passVariantDefinition)
        {
            InternalUtils.Log.Info("Creating ShaderPass: " + passName);
            this.Name = passName;
            this.Variants =
                passVariants___Name_AND_passVariantDefinition
                    .Select (x => new OpenGLShader (x.Item1, passName, x.Item2.PassDefinition))
                    .ToList();

            this.BestVariantMap = new Dictionary<VertexDeclaration, OpenGLShader>();
        }


        internal void BindAttributes(IList<String> inputNames)
        {
            foreach (var variant in this.Variants)
            {
                variant.BindAttributes(inputNames);
            }
        }

        internal void Link()
        {
            foreach (var variant in this.Variants)
            {
                variant.Link();
            }
        }

        internal void ValidateInputs(List<ShaderInputDefinition> definitions)
        {
            foreach(var variant in this.Variants)
            {
                variant.ValidateInputs(definitions);
            }
        }

        internal void ValidateVariables(List<ShaderVariableDefinition> definitions)
        {
            foreach(var variant in this.Variants)
            {
                variant.ValidateVariables(definitions);
            }
        }

        internal void ValidateSamplers(List<ShaderSamplerDefinition> definitions)
        {
            foreach(var variant in this.Variants)
            {
                variant.ValidateSamplers(definitions);
            }
        }


        public void Activate(VertexDeclaration vertexDeclaration)
        {
            if (!BestVariantMap.ContainsKey (vertexDeclaration))
            {
                BestVariantMap[vertexDeclaration] = ShaderHelper.WorkOutBestVariantFor(vertexDeclaration, Variants);
            }
            var bestVariant = BestVariantMap[vertexDeclaration];
            // select the correct shader pass variant and then activate it
            bestVariant.Activate ();

            foreach (var key1 in currentVariables.Keys)
            {
                var variable = bestVariant
                    .Variables
                    .Find(x => x.NiceName == key1 || x.Name == key1);

                if( variable == null )
                {
                    string warning = "WARNING: missing variable: " + key1;

                    if( !logHistory.ContainsKey(warning) )
                    {
                        InternalUtils.Log.Info(warning);

                        logHistory.Add(warning, true);
                    }
                }
                else
                {
                    var val = currentVariables[key1];

                    variable.Set(val);
                }
            }

            foreach (var key2 in currentSamplerSlots.Keys)
            {
                var sampler = bestVariant
                    .Samplers
                    .Find(x => x.NiceName == key2 || x.Name == key2);

                if( sampler == null )
                {
                    //InternalUtils.Log.Info("missing sampler: " + key2);
                }
                else
                {
                    var slot = currentSamplerSlots[key2];

                    sampler.SetSlot(slot);
                }
            }

        }

        public void Dispose()
        {
            foreach (var oglesShader in Variants)
            {
                oglesShader.Dispose ();
            }
        }
    }

    internal sealed class OpenGLTexture
        : Texture2D
    {
        public int glTextureId {get; private set;}

        NSImage nsImage;

        int pixelsWide;
        int pixelsHigh;  

        internal static OpenGLTexture CreateFromFile(string path)
        {
            using(var fStream = new FileStream(path, FileMode.Open))
            {
                var nsImage = NSImage.FromStream( fStream );
    
                var texture = new OpenGLTexture(nsImage);
    
                return texture;
            }
        }

        private OpenGLTexture(NSImage nsImage)
        {
            this.nsImage = nsImage;
            IntPtr dataPointer = RequestImagePixelData(nsImage);

            CreateTexture2D((int)nsImage.Size.Width, (int)nsImage.Size.Height, dataPointer);
        }


        //Store pixel data as an ARGB Bitmap
        IntPtr RequestImagePixelData (NSImage inImage)
        {
            var imageSize = inImage.Size;
            
            CGBitmapContext ctxt = CreateRgbaBitmapContext (inImage.CGImage);
            
            var rect = new RectangleF (0, 0, imageSize.Width, imageSize.Height);
            
            ctxt.DrawImage (rect, inImage.CGImage);
            var data = ctxt.Data;
            
            return data;
        }

        CGBitmapContext CreateRgbaBitmapContext (CGImage inImage)
        {
            pixelsWide = inImage.Width;
            pixelsHigh = inImage.Height;

            using (var colorSpace = CGColorSpace.CreateDeviceRGB())
            {
                var bitmapBytesPerRow = pixelsWide * 4;
                var bitmapByteCount = bitmapBytesPerRow * pixelsHigh;
                var bitmapData = Marshal.AllocHGlobal (bitmapByteCount);

                if (bitmapData == IntPtr.Zero)
                {
                    throw new Exception ("Memory not allocated.");
                }
                
                var context = new CGBitmapContext (
                    bitmapData, 
                    pixelsWide, 
                    pixelsHigh, 
                    8,
                    bitmapBytesPerRow, 
                    colorSpace, 
                    CGImageAlphaInfo.PremultipliedLast);

                if (context == null)
                {
                    throw new Exception ("Context not created");
                }

                return context;
            }
        }
        


        public override int Width
        {
            get
            {
                return pixelsWide;
            }
        }
        public override int Height
        {
            get
            {
                return pixelsHigh;
            }
        }

        
        void CreateTexture2D(int width, int height, IntPtr pixelDataRgba32)
        {
            int textureId = -1;
            
            
            // this sets the unpack alignment.  which is used when reading pixels
            // in the fragment shader.  when the textue data is uploaded via glTexImage2d,
            // the rows of pixels are assumed to be aligned to the value set for GL_UNPACK_ALIGNMENT.
            // By default, the value is 4, meaning that rows of pixels are assumed to begin
            // on 4-byte boundaries.  this is a global STATE.
            global::MonoMac.OpenGL.GL.PixelStore(global::MonoMac.OpenGL.PixelStoreParameter.UnpackAlignment, 4);
            ErrorHandler.Check();

            // the first sept in the application of texture is to create the
            // texture object.  this is a container object that holds the 
            // texture data.  this function returns a handle to a texture
            // object.
            global::MonoMac.OpenGL.GL.GenTextures(1, out textureId);
            ErrorHandler.Check();

            this.glTextureId = textureId;

            var textureTarget = global::MonoMac.OpenGL.TextureTarget.Texture2D;            
            
            // we need to bind the texture object so that we can opperate on it.
            global::MonoMac.OpenGL.GL.BindTexture(textureTarget, textureId);
            ErrorHandler.Check();

            var internalFormat = global::MonoMac.OpenGL.PixelInternalFormat.Rgba;
            var format = global::MonoMac.OpenGL.PixelFormat.Rgba;
            
            var textureDataFormat = global::MonoMac.OpenGL.PixelType.UnsignedByte;
            
            // now use the bound texture object to load the image data.
            global::MonoMac.OpenGL.GL.TexImage2D(
                
                // specifies the texture target, either GL_TEXTURE_2D or one of the cubemap face targets.
                textureTarget,
                
                // specifies which mip level to load.  the base level is
                // specified by 0 following by an increasing level for each
                // successive mipmap.
                0,
                
                // internal format for the texture storage, can be:
                // - GL_RGBA
                // - GL_RGB
                // - GL_LUMINANCE_ALPHA
                // - GL_LUMINANCE
                // - GL_ALPHA
                internalFormat,
                
                // the width of the image in pixels
                width,
                
                // the height of the image in pixels
                height,
                
                // boarder - set to zero, only here for compatibility with OpenGL desktop
                0,
                
                // the format of the incoming texture data, in opengl es this 
                // has to be the same as the internal format
                format,
                
                // the type of the incoming pixel data, can be:
                // - unsigned byte
                // - unsigned short 4444
                // - unsigned short 5551
                // - unsigned short 565
                textureDataFormat, // this refers to each individual channel
                
                
                pixelDataRgba32
                
                );

            ErrorHandler.Check();

            // sets the minification and maginfication filtering modes.  required
            // because we have not loaded a complete mipmap chain for the texture
            // so we must select a non mipmapped minification filter.
            global::MonoMac.OpenGL.GL.TexParameter(textureTarget, global::MonoMac.OpenGL.TextureParameterName.TextureMinFilter, (int) global::MonoMac.OpenGL.All.Nearest );

            ErrorHandler.Check();

            global::MonoMac.OpenGL.GL.TexParameter(textureTarget, global::MonoMac.OpenGL.TextureParameterName.TextureMagFilter, (int) global::MonoMac.OpenGL.All.Nearest );

            ErrorHandler.Check();
        }
        
        
        
        void DeleteTexture(Texture2D texture)
        {
            int textureId = (texture as OpenGLTexture).glTextureId;
            
            global::MonoMac.OpenGL.GL.DeleteTextures(1, ref textureId);
        }
    }

    public sealed class Keyboard
        : IKeyboard
    {
        readonly HashSet<Char> characterKeysThatAreDown = new HashSet<Char>();
        readonly HashSet<FunctionalKey> functionalKeysThatAreDown = new HashSet<FunctionalKey>();
        
        internal void KeyDown (NSEvent theEvent)
        {
            theEvent.Characters
                .ToCharArray ()
                .Where(x => !IsFunctionalKey(x))
                .ToList ()
                .ForEach (x => characterKeysThatAreDown.Add(x));
                
            var fKey = GetFunctionalKey(theEvent.KeyCode);
            if( fKey.HasValue ) functionalKeysThatAreDown.Add(fKey.Value);
        }

        internal void KeyUp (NSEvent theEvent)
        {
            theEvent.Characters
                .ToCharArray ()
                .Where(x => !IsFunctionalKey(x))
                .ToList ()
                .ForEach (x => characterKeysThatAreDown.Remove(x));
                
            var fKey = GetFunctionalKey(theEvent.KeyCode);
            if( fKey.HasValue ) functionalKeysThatAreDown.Remove(fKey.Value);
        }
        
        static FunctionalKey? GetFunctionalKey (UInt16 hardwareIndependantKeyCode)
        {
            if (hardwareIndependantKeyCode == 0x24) return FunctionalKey.Enter;
            if (hardwareIndependantKeyCode == 0x7c) return FunctionalKey.Right;
            if (hardwareIndependantKeyCode == 0x7b) return FunctionalKey.Left;
            if (hardwareIndependantKeyCode == 0x7e) return FunctionalKey.Up;
            if (hardwareIndependantKeyCode == 0x7d) return FunctionalKey.Down;
            if (hardwareIndependantKeyCode == 0x31) return FunctionalKey.Spacebar;
            if (hardwareIndependantKeyCode == 0x35) return FunctionalKey.Escape;
            
            return null;
        }
        
        static Boolean IsFunctionalKey (Char c)
        {
            if (c == '\r') return true;
            if (c == '\n') return true;
            if (c == '\t') return true;
            
            return false;
        }

        #region IKeyboard

        public FunctionalKey[] GetPressedFunctionalKey ()
        {
            return functionalKeysThatAreDown.ToArray();
        }

        public Boolean IsFunctionalKeyDown (FunctionalKey key)
        {
            return functionalKeysThatAreDown.Contains(key);
        }

        public Boolean IsFunctionalKeyUp (FunctionalKey key)
        {
            return !functionalKeysThatAreDown.Contains(key);
        }

        public KeyState this [FunctionalKey key]
        {
            get
            {
                return functionalKeysThatAreDown.Contains(key) ? KeyState.Down : KeyState.Up;
            }
        }

        public Char[] GetPressedCharacterKeys()
        {
            return characterKeysThatAreDown.ToArray();
        }
        
        public Boolean IsCharacterKeyDown (Char key)
        {
            return characterKeysThatAreDown.Contains(key);
        }
        
        public Boolean IsCharacterKeyUp (Char key)
        {
            return !characterKeysThatAreDown.Contains(key);
        }

        public KeyState this [Char key]
        {
            get
            {
                return characterKeysThatAreDown.Contains(key) ? KeyState.Down : KeyState.Up;
            }
        }

        #endregion
    }

    public sealed class Mouse
        : IMouse
    {
        public ButtonState Left
        {
            get
            {
                return ButtonState.Released;
            }
        }

        public ButtonState Middle
        {
            get
            {
                return ButtonState.Released;
            }
        }

        public ButtonState Right
        {
            get
            {
                return ButtonState.Released;
            }
        }

        public Int32 ScrollWheelValue
        {
            get
            {
                return 0;
            }
        }

        public Int32 X
        {
            get
            {
                return 0;
            }
        }

        public Int32 Y
        {
            get
            {
                return 0;
            }
        }
    }


    #region OpenGL ES Shaders

    public sealed class OpenGLShader
        : IDisposable
    {
        public List<OpenGLShaderInput> Inputs { get; private set; }
        public List<OpenGLShaderVariable> Variables { get; private set; }
        public List<OpenGLShaderSampler> Samplers { get; private set; }

        internal string VariantName { get { return variantName; }}
        Int32 programHandle;
        Int32 fragShaderHandle;
        Int32 vertShaderHandle;

        // for debugging
        string variantName;
        string passName;

        string pixelShaderPath;
        string vertexShaderPath;

        public override string ToString ()
        {
            //string a = Inputs.Select(x => x.Name).Join(", ");
            //string b = Variables.Select(x => x.Name).Join(", ");

            string a = string.Empty;

            for(int i = 0; i < Inputs.Count; ++i)
            {
                a += Inputs[i].Name; if( i + 1 < Inputs.Count ) { a += ", "; }
            }

            string b = string.Empty;
            for(int i = 0; i < Variables.Count; ++i)
            {
                b += Variables[i].Name; if( i + 1 < Variables.Count ) { b += ", "; }
            }

            return string.Format (
                "[OpenGLShader: Variant {0}, Pass {1}: Inputs: [{2}], Variables: [{3}]]",
                variantName,
                passName,
                a,
                b);
        }

        internal void ValidateInputs(List<ShaderInputDefinition> definitions)
        {
            InternalUtils.Log.Info(string.Format ("Pass: {1} => ValidateInputs({0})", variantName, passName ));

            // Make sure that this shader implements all of the non-optional defined inputs.
            var nonOptionalDefinitions = definitions.Where(y => !y.Optional).ToList();

            foreach(var definition in nonOptionalDefinitions)
            {
                var find = Inputs.Find(x => x.Name == definition.Name/* && x.Type == definition.Type */);

                if( find == null )
                {
                    throw new Exception("problem");
                }
            }

            // Make sure that every implemented input is defined.
            foreach(var input in Inputs)
            {
                var find = definitions.Find(x => x.Name == input.Name
                    /*&& (x.Type == input.Type || (x.Type == typeof(Rgba32) && input.Type == typeof(Vector4)))*/
                    );

                if( find == null )
                {
                    throw new Exception("problem");
                }
                else
                {
                    input.RegisterExtraInfo(find);
                }
            }
        }

        internal void ValidateVariables(List<ShaderVariableDefinition> definitions)
        {
            InternalUtils.Log.Info(string.Format ("Pass: {1} => ValidateVariables({0})", variantName, passName ));


            // Make sure that every implemented input is defined.
            foreach(var variable in Variables)
            {
                var find = definitions.Find(
                    x =>
                    x.Name == variable.Name //&&
                    //(x.Type == variable.Type || (x.Type == typeof(Rgba32) && variable.Type == typeof(Vector4)))
                    );

                if( find == null )
                {
                    throw new Exception("problem");
                }
                else
                {
                    variable.RegisterExtraInfo(find);
                }
            }
        }

        internal void ValidateSamplers(List<ShaderSamplerDefinition> definitions)
        {
            InternalUtils.Log.Info(string.Format ("Pass: {1} => ValidateSamplers({0})", variantName, passName ));

            var nonOptionalSamplers = definitions.Where(y => !y.Optional).ToList();

            foreach(var definition in nonOptionalSamplers)
            {
                var find = this.Samplers.Find(x => x.Name == definition.Name);

                if( find == null )
                {
                    throw new Exception("problem");
                }
            }

            // Make sure that every implemented input is defined.
            foreach(var sampler in this.Samplers)
            {
                var find = definitions.Find(x => x.Name == sampler.Name);

                if( find == null )
                {
                    throw new Exception("problem");
                }
                else
                {
                    sampler.RegisterExtraInfo(find);
                }
            }
        }

        /*
        static void CheckVariableCompatibility(List<OpenGLShaderVariable> definedVariables )
        {
            throw new NotImplementedException();
        }

        static void CheckInputCompatibility(List<OpenGLShaderInput> definedInputs, Dictionary<string, global::MonoMac.OpenGL.ActiveAttribType> actualAttributes )
        {
            // make sure that the shader we just loaded will work with this shader definition
            if( actualAttributes.Count != definedInputs.Count )
            {
                throw new Exception("shader doesn't implement definition");
            }

            foreach( var key in actualAttributes.Keys )
            {
                var item = definedInputs.Find(x => x.Name == key);

                if( item == null )
                {
                    throw new Exception("shader doesn't implement definition - missing variable");
                }

                if( item.Type != EnumConverter.ToType( actualAttributes[key] ) )
                {
                    throw new Exception("shader doesn't implement definition - variable is of the wrong type");
                }
            }
        }
        */

        static string GetResourcePath(string path)
        {
            string ext = Path.GetExtension(path);

            string filename = path.Substring(0, path.Length - ext.Length);

            var resourcePathname =
                global::MonoMac.Foundation.NSBundle.MainBundle.PathForResource (
                    filename,
                    ext.Substring(1, ext.Length - 1)
                );

            if( resourcePathname == null )
            {
                throw new Exception("Resource [" + path + "] not found");
            }

            return resourcePathname;
        }

        internal OpenGLShader(String variantName, String passName, OpenGLShaderDefinition definition)
        {
            InternalUtils.Log.Info ("  Creating Pass Variant: " + variantName);
            this.variantName = variantName;
            this.passName = passName;
            this.vertexShaderPath = definition.VertexShaderPath;
            this.pixelShaderPath = definition.PixelShaderPath;

            //Variables =
            programHandle = ShaderUtils.CreateShaderProgram ();

            vertShaderHandle = ShaderUtils.CreateVertexShader (GetResourcePath(this.vertexShaderPath));
            fragShaderHandle = ShaderUtils.CreateFragmentShader (GetResourcePath(this.pixelShaderPath));

            ShaderUtils.AttachShader (programHandle, vertShaderHandle);
            ShaderUtils.AttachShader (programHandle, fragShaderHandle);

        }

        internal void BindAttributes(IList<String> orderedAttributes)
        {
            int index = 0;

            foreach(var attName in orderedAttributes)
            {
                global::MonoMac.OpenGL.GL.BindAttribLocation(programHandle, index, attName);
                ErrorHandler.Check();
                bool success = ShaderUtils.LinkProgram (programHandle);
                if (success)
                {
                    index++;
                }

            }
        }

        internal void Link()
        {
            // bind atts here
            //ShaderUtils.LinkProgram (programHandle);

            InternalUtils.Log.Info("  Finishing linking");

            InternalUtils.Log.Info("  Initilise Attributes");
            var attributes = ShaderUtils.GetAttributes(programHandle);

            Inputs = attributes
                .Select(x => new OpenGLShaderInput(programHandle, x))
                .OrderBy(y => y.AttributeLocation)
                .ToList();

            String logInputs = "  Inputs : ";
            foreach (var input in Inputs) {
                logInputs += input.Name + ", ";
            }
            InternalUtils.Log.Info (logInputs);

            InternalUtils.Log.Info("  Initilise Uniforms");
            var uniforms = ShaderUtils.GetUniforms(programHandle);


            Variables = uniforms
                .Where(y =>
                       y.Type != global::MonoMac.OpenGL.ActiveUniformType.Sampler2D &&
                       y.Type != global::MonoMac.OpenGL.ActiveUniformType.SamplerCube)
                .Select(x => new OpenGLShaderVariable(programHandle, x))
                .OrderBy(z => z.UniformLocation)
                .ToList();
            String logVars = "  Variables : ";
            foreach (var variable in Variables) {
                logVars += variable.Name + ", ";
            }
            InternalUtils.Log.Info (logVars);

            InternalUtils.Log.Info("  Initilise Samplers");
            Samplers = uniforms
                .Where(y =>
                       y.Type == global::MonoMac.OpenGL.ActiveUniformType.Sampler2D ||
                       y.Type == global::MonoMac.OpenGL.ActiveUniformType.SamplerCube)
                .Select(x => new OpenGLShaderSampler(programHandle, x))
                .OrderBy(z => z.UniformLocation)
                .ToList();

            #if DEBUG
            ShaderUtils.ValidateProgram (programHandle);
            #endif

            ShaderUtils.DetachShader(programHandle, fragShaderHandle);
            ShaderUtils.DetachShader(programHandle, vertShaderHandle);

            ShaderUtils.DeleteShader(programHandle, fragShaderHandle);
            ShaderUtils.DeleteShader(programHandle, vertShaderHandle);
        }

        public void Activate ()
        {
            global::MonoMac.OpenGL.GL.UseProgram (programHandle);
            ErrorHandler.Check ();
        }

        public void Dispose()
        {
            ShaderUtils.DestroyShaderProgram(programHandle);
            ErrorHandler.Check();
        }
    }

    public sealed class OpenGLShaderDefinition
    {
        public string VertexShaderPath { get; set; }
        public string PixelShaderPath { get; set; }
    }

    /// <summary>
    /// Represents an Open GL ES shader input, all the data is read dynamically from
    /// the shader at runtime, not from the ShaderInputDefinition.  This way we can compare the
    /// two and check to see that we have what we are expecting.
    /// </summary>
    public sealed class OpenGLShaderInput
    {
        int ProgramHandle { get; set; }
        internal int AttributeLocation { get; private set; }

        public String Name { get; private set; }
        public Type Type { get; private set; }
        public VertexElementUsage Usage { get; private set; }
        public Object DefaultValue { get; private set; }
        public Boolean Optional { get; private set; }

        public OpenGLShaderInput(
            int programHandle, ShaderUtils.ShaderAttribute attribute)
        {
            int attLocation = global::MonoMac.OpenGL.GL.GetAttribLocation(programHandle, attribute.Name);

            ErrorHandler.Check();

            InternalUtils.Log.Info(string.Format(
                "    Binding Shader Input: [Prog={0}, AttIndex={1}, AttLocation={4}, AttName={2}, AttType={3}]",
                programHandle, attribute.Index, attribute.Name, attribute.Type, attLocation));

            this.ProgramHandle = programHandle;
            this.AttributeLocation = attLocation;
            this.Name = attribute.Name;
            this.Type = EnumConverter.ToType(attribute.Type);


        }

        internal void RegisterExtraInfo(ShaderInputDefinition definition)
        {
            Usage = definition.Usage;
            DefaultValue = definition.DefaultValue;
            Optional = definition.Optional;
        }
    }

    public sealed class OpenGLShaderSampler
    {
        int ProgramHandle { get; set; }
        internal int UniformLocation { get; private set; }

        public String NiceName { get; set; }
        public String Name { get; set; }

        public OpenGLShaderSampler(
            int programHandle, ShaderUtils.ShaderUniform uniform )
        {
            this.ProgramHandle = programHandle;

            int uniformLocation = global::MonoMac.OpenGL.GL.GetUniformLocation(programHandle, uniform.Name);

            ErrorHandler.Check();


            this.UniformLocation = uniformLocation;
            this.Name = uniform.Name;
        }

        internal void RegisterExtraInfo(ShaderSamplerDefinition definition)
        {
            NiceName = definition.NiceName;
        }

        public void SetSlot(Int32 slot)
        {
            // set the sampler texture unit to 0
            global::MonoMac.OpenGL.GL.Uniform1( this.UniformLocation, slot );
            ErrorHandler.Check();
        }

    }

    public sealed class OpenGLShaderVariable
    {
        int ProgramHandle { get; set; }
        internal int UniformLocation { get; private set; }

        public String NiceName { get; private set; }
        public String Name { get; private set; }
        public Type Type { get; private set; }
        public Object DefaultValue { get; private set; }

        public OpenGLShaderVariable(
            int programHandle, ShaderUtils.ShaderUniform uniform)
        {

            this.ProgramHandle = programHandle;

            int uniformLocation = global::MonoMac.OpenGL.GL.GetUniformLocation(programHandle, uniform.Name);

            ErrorHandler.Check();

            if( uniformLocation == -1 )
                throw new Exception();

            this.UniformLocation = uniformLocation;
            this.Name = uniform.Name;
            this.Type = EnumConverter.ToType(uniform.Type);

            InternalUtils.Log.Info(string.Format(
                "    Caching Reference to Shader Variable: [Prog={0}, UniIndex={1}, UniLocation={2}, UniName={3}, UniType={4}]",
                programHandle, uniform.Index, uniformLocation, uniform.Name, uniform.Type));

        }

        internal void RegisterExtraInfo(ShaderVariableDefinition definition)
        {
            NiceName = definition.NiceName;
            DefaultValue = definition.DefaultValue;
        }

        public void Set(object value)
        {
            //todo this should be using convert turn the data into proper opengl es types.
            Type t = value.GetType();

            if( t == typeof(Matrix44) )
            {
                var castValue = (Matrix44) value;
                var otkValue = MatrixConverter.ToOpenGL(castValue);
                global::MonoMac.OpenGL.GL.UniformMatrix4( UniformLocation, false, ref otkValue );
            }
            else if( t == typeof(Int32) )
            {
                var castValue = (Int32) value;
                global::MonoMac.OpenGL.GL.Uniform1( UniformLocation, 1, ref castValue );
            }
            else if( t == typeof(Single) )
            {
                var castValue = (Single) value;
                global::MonoMac.OpenGL.GL.Uniform1( UniformLocation, 1, ref castValue );
            }
            else if( t == typeof(Vector2) )
            {
                var castValue = (Vector2) value;
                global::MonoMac.OpenGL.GL.Uniform2( UniformLocation, 1, ref castValue.X );
            }
            else if( t == typeof(Vector3) )
            {
                var castValue = (Vector3) value;
                global::MonoMac.OpenGL.GL.Uniform3( UniformLocation, 1, ref castValue.X );
            }
            else if( t == typeof(Vector4) )
            {
                var castValue = (Vector4) value;
                global::MonoMac.OpenGL.GL.Uniform4( UniformLocation, 1, ref castValue.X );
            }
            else if( t == typeof(Rgba32) )
            {
                var castValue = (Rgba32) value;

                Vector4 vec4Value;
                castValue.UnpackTo(out vec4Value);

                // does this rgba value need to be packed in to a vector3 or a vector4
                if( this.Type == typeof(Vector4) )
                    global::MonoMac.OpenGL.GL.Uniform4( UniformLocation, 1, ref vec4Value.X );
                else if( this.Type == typeof(Vector3) )
                    global::MonoMac.OpenGL.GL.Uniform3( UniformLocation, 1, ref vec4Value.X );
                else
                    throw new Exception("Not supported");
            }
            else
            {
                throw new Exception("Not supported");
            }

            ErrorHandler.Check();

        }
    }


    #endregion

    #region Shader Definitions

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

        String name;
        public String Name
        { 
            get { return name; }
            set { 
                if (value.Length > 16)
                    name = value.Substring (0, 16);
                else
                    name = value;
            }
        }
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
        public OpenGLShaderDefinition PassDefinition { get; set; }
    }


    #endregion

    #region Shader Definitions

    public static partial class CorShaders
    {
        public static IShader CreatePhongPixelLit()
        {
            var parameter = new ShaderDefinition()
            {
                Name = "PixelLit",
                PassNames = new List<string>() { "Main" },
                InputDefinitions = new List<ShaderInputDefinition>()
                {
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertPos",
                        Type = typeof(Vector3),
                        Usage = VertexElementUsage.Position,
                        DefaultValue = Vector3.Zero,
                        Optional = false,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertNormal",
                        Type = typeof(Vector3),
                        Usage = VertexElementUsage.Normal,
                        DefaultValue = Vector3.Zero,
                        Optional = false,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertTexcoord",
                        Type = typeof(Vector2),
                        Usage = VertexElementUsage.TextureCoordinate,
                        DefaultValue = Vector2.Zero,
                        Optional = true,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertColour",
                        Type = typeof(Rgba32),
                        Usage = VertexElementUsage.Colour,
                        DefaultValue = Rgba32.White,
                        Optional = true,
                    },
                },
                SamplerDefinitions = new List<ShaderSamplerDefinition>()
                {
                    new ShaderSamplerDefinition()
                    {
                        NiceName = "TextureSampler",
                        Name = "s_tex0",
                        Optional = true,
                    }
                },
                VariableDefinitions = new List<ShaderVariableDefinition>()
                {
                    new ShaderVariableDefinition()
                    {
                        NiceName = "World",
                        Name = "u_world",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "View",
                        Name = "u_view",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "Projection",
                        Name = "u_proj",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "MaterialColour",
                        Name = "u_colour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.White,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "AmbientLightColour",
                        Name = "u_liAmbient",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Gray,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "EmissiveColour",
                        Name = "u_emissiveColour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Black,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "SpecularColour",
                        Name = "u_specularColour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.White,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "SpecularPower",
                        Name = "u_specularPower",
                        Type = typeof(Single),
                        DefaultValue = 0.7f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "EyePosition",
                        Name = "u_eyePosition",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Zero,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogEnabled",
                        Name = "u_fogEnabled",
                        Type = typeof(Single),
                        DefaultValue = 1f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogStart",
                        Name = "u_fogStart",
                        Type = typeof(Single),
                        DefaultValue = 100f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogEnd",
                        Name = "u_fogEnd",
                        Type = typeof(Single),
                        DefaultValue = 1000f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogColour",
                        Name = "u_fogColour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Blue,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight0Direction",
                        Name = "u_li0Dir",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Down,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight0DiffuseColour",
                        Name = "u_li0Diffuse",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Red,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight0SpecularColour",
                        Name = "u_li0Spec",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Salmon,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight1Direction",
                        Name = "u_li1Dir",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Down,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight1DiffuseColour",
                        Name = "u_li1Diffuse",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Red,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight1SpecularColour",
                        Name = "u_li1Spec",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Salmon,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight2Direction",
                        Name = "u_li2Dir",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Down,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight2DiffuseColour",
                        Name = "u_li2Diffuse",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Red,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight2SpecularColour",
                        Name = "u_li2Spec",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Salmon,
                    },
                },
                VariantDefinitions = new List<ShaderVariantDefinition>()
                {
                    new ShaderVariantDefinition()
                    {
                        VariantName = "PixelLit_PositionNormal",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OpenGLShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/PixelLit_PositionNormal.vsh",
                                    PixelShaderPath = "shaders/PixelLit_PositionNormal.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "PixelLit_PositionNormalTexture",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OpenGLShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/PixelLit_PositionNormalTexture.vsh",
                                    PixelShaderPath = "shaders/PixelLit_PositionNormalTexture.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "PixelLit_PositionNormalColour",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OpenGLShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/PixelLit_PositionNormalColour.vsh",
                                    PixelShaderPath = "shaders/PixelLit_PositionNormalColour.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "PixelLit_PositionNormalTextureColour",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OpenGLShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/PixelLit_PositionNormalTextureColour.vsh",
                                    PixelShaderPath = "shaders/PixelLit_PositionNormalTextureColour.fsh",
                                },
                            },
                        },
                    },
                },
            };

            var s = new Shader (parameter);

			InternalUtils.Log.Info(s.ToString());

            return s;
        }
    }

    public static partial class CorShaders
    {
        public static IShader CreatePhongVertexLit()
        {
            var parameter = new ShaderDefinition()
            {
                Name = "VertexLit",
                PassNames = new List<string>() { "Main" },
                InputDefinitions = new List<ShaderInputDefinition>()
                {
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertPos",
                        Type = typeof(Vector3),
                        Usage = VertexElementUsage.Position,
                        DefaultValue = Vector3.Zero,
                        Optional = false,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertNormal",
                        Type = typeof(Vector3),
                        Usage = VertexElementUsage.Normal,
                        DefaultValue = Vector3.Zero,
                        Optional = false,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertTexcoord",
                        Type = typeof(Vector2),
                        Usage = VertexElementUsage.TextureCoordinate,
                        DefaultValue = Vector2.Zero,
                        Optional = true,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertColour",
                        Type = typeof(Rgba32),
                        Usage = VertexElementUsage.Colour,
                        DefaultValue = Rgba32.White,
                        Optional = true,
                    },
                },
                SamplerDefinitions = new List<ShaderSamplerDefinition>()
                {
                    new ShaderSamplerDefinition()
                    {
                        NiceName = "TextureSampler",
                        Name = "s_tex0",
                        Optional = true,
                    }
                },
                VariableDefinitions = new List<ShaderVariableDefinition>()
                {
                    new ShaderVariableDefinition()
                    {
                        NiceName = "World",
                        Name = "u_world",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "View",
                        Name = "u_view",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "Projection",
                        Name = "u_proj",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "MaterialColour",
                        Name = "u_colour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.White,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "AmbientLightColour",
                        Name = "u_liAmbient",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Gray,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "EmissiveColour",
                        Name = "u_emissiveColour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Black,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "SpecularColour",
                        Name = "u_specularColour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.White,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "SpecularPower",
                        Name = "u_specularPower",
                        Type = typeof(Single),
                        DefaultValue = 0.7f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "EyePosition",
                        Name = "u_eyePosition",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Zero,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogEnabled",
                        Name = "u_fogEnabled",
                        Type = typeof(Single),
                        DefaultValue = 1f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogStart",
                        Name = "u_fogStart",
                        Type = typeof(Single),
                        DefaultValue = 100f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogEnd",
                        Name = "u_fogEnd",
                        Type = typeof(Single),
                        DefaultValue = 1000f,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "FogColour",
                        Name = "u_fogColour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Blue,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight0Direction",
                        Name = "u_li0Dir",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Down,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight0DiffuseColour",
                        Name = "u_li0Diffuse",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Red,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight0SpecularColour",
                        Name = "u_li0Spec",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Salmon,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight1Direction",
                        Name = "u_li1Dir",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Down,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight1DiffuseColour",
                        Name = "u_li1Diffuse",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Red,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight1SpecularColour",
                        Name = "u_li1Spec",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Salmon,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight2Direction",
                        Name = "u_li2Dir",
                        Type = typeof(Vector3),
                        DefaultValue = Vector3.Down,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight2DiffuseColour",
                        Name = "u_li2Diffuse",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Red,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "DirectionalLight2SpecularColour",
                        Name = "u_li2Spec",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.Salmon,
                    },
                },
                VariantDefinitions = new List<ShaderVariantDefinition>()
                {
                    new ShaderVariantDefinition()
                    {
                        VariantName = "VertexLit_PositionNormal",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OpenGLShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/VertexLit_PositionNormal.vsh",
                                    PixelShaderPath = "shaders/VertexLit_PositionNormal.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "VertexLit_PositionNormalTexture",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OpenGLShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/VertexLit_PositionNormalTexture.vsh",
                                    PixelShaderPath = "shaders/VertexLit_PositionNormalTexture.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "VertexLit_PositionNormalColour",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OpenGLShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/VertexLit_PositionNormalColour.vsh",
                                    PixelShaderPath = "shaders/VertexLit_PositionNormalColour.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "VertexLit_PositionNormalTextureColour",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OpenGLShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/VertexLit_PositionNormalTextureColour.vsh",
                                    PixelShaderPath = "shaders/VertexLit_PositionNormalTextureColour.fsh",
                                },
                            },
                        },
                    },
                },
            };


            var s = new Shader (parameter);

			InternalUtils.Log.Info(s.ToString());

            return s;
        }
    }

    public static partial class CorShaders
    {
        public static IShader CreateUnlit()
        {
            var parameter = new ShaderDefinition()
            {
                Name = "Unlit",
                PassNames = new List<string>() { "Main" },
                InputDefinitions = new List<ShaderInputDefinition>()
                {
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertPos",
                        Type = typeof(Vector3),
                        Usage = VertexElementUsage.Position,
                        DefaultValue = Vector3.Zero,
                        Optional = false,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertTexcoord",
                        Type = typeof(Vector2),
                        Usage = VertexElementUsage.TextureCoordinate,
                        DefaultValue = Vector2.Zero,
                        Optional = true,
                    },
                    new ShaderInputDefinition()
                    {
                        Name = "a_vertColour",
                        Type = typeof(Rgba32),
                        Usage = VertexElementUsage.Colour,
                        DefaultValue = Rgba32.White,
                        Optional = true,
                    },
                },
                SamplerDefinitions = new List<ShaderSamplerDefinition>()
                {
                    new ShaderSamplerDefinition()
                    {
                        NiceName = "TextureSampler",
                        Name = "s_tex0",
                        Optional = true,
                    }
                },
                VariableDefinitions = new List<ShaderVariableDefinition>()
                {
                    new ShaderVariableDefinition()
                    {
                        NiceName = "MaterialColour",
                        Name = "u_colour",
                        Type = typeof(Rgba32),
                        DefaultValue = Rgba32.White,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "World",
                        Name = "u_world",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "View",
                        Name = "u_view",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                    new ShaderVariableDefinition()
                    {
                        NiceName = "Projection",
                        Name = "u_proj",
                        Type = typeof(Matrix44),
                        DefaultValue = Matrix44.Identity,
                    },
                },
                VariantDefinitions = new List<ShaderVariantDefinition>()
                {
                    new ShaderVariantDefinition()
                    {
                        VariantName = "Unlit_Position",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OpenGLShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/Unlit_Position.vsh",
                                    PixelShaderPath = "shaders/Unlit_Position.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "Unlit_PositionTexture",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OpenGLShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/Unlit_PositionTexture.vsh",
                                    PixelShaderPath = "shaders/Unlit_PositionTexture.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "Unlit_PositionColour",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OpenGLShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/Unlit_PositionColour.vsh",
                                    PixelShaderPath = "shaders/Unlit_PositionColour.fsh",
                                },
                            },
                        },
                    },
                    new ShaderVariantDefinition()
                    {
                        VariantName = "Unlit_PositionTextureColour",
                        VariantPassDefinitions = new List<ShaderVarientPassDefinition>()
                        {
                            new ShaderVarientPassDefinition()
                            {
                                PassName = "Main",
                                PassDefinition = new OpenGLShaderDefinition()
                                {
                                    VertexShaderPath = "shaders/Unlit_PositionTextureColour.vsh",
                                    PixelShaderPath = "shaders/Unlit_PositionTextureColour.fsh",
                                },
                            },
                        },
                    },
                },
            };


            var s = new Shader (parameter);

			InternalUtils.Log.Info(s.ToString());

            return s;
        }
    }


    #endregion
}
