// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor! PlayStation Mobile Platform Implementation                        │ \\
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

using Abacus;
using Abacus.Packed;
using Abacus.SinglePrecision;
using Abacus.Int32Precision;

namespace Cor.Platform.Managed.MonoMac
{
    public static class Vector2Converter
    {
        // VECTOR 2
        public static Sce.Pss.Core.Vector2 ToPSS(this Vector2 vec)
        {
            return new Sce.Pss.Core.Vector2(vec.X, vec.Y);
        }

        public static Vector2 ToBlimey(this Sce.Pss.Core.Vector2 vec)
        {
            return new Vector2(vec.X, vec.Y);
        }
    }
    
    public static class Vector3Converter
    {
        // VECTOR 3
        public static Sce.Pss.Core.Vector3 ToPSS(this Vector3 vec)
        {
            return new Sce.Pss.Core.Vector3(vec.X, vec.Y, vec.Z);
        }

        public static Vector3 ToBlimey(this Sce.Pss.Core.Vector3 vec)
        {
            return new Vector3(vec.X, vec.Y, vec.Z);
        }
    }
    
    public static class Vector4Converter
    {
        // VECTOR 3
        public static Sce.Pss.Core.Vector4 ToPSS(this Vector4 vec)
        {
            return new Sce.Pss.Core.Vector4(vec.X, vec.Y, vec.Z, vec.W);
        }

        public static Vector4 ToBlimey(this Sce.Pss.Core.Vector4 vec)
        {
            return new Vector4(vec.X, vec.Y, vec.Z, vec.W);
        }
    }

    // http://gamedev.stackexchange.com/questions/16011/direct3d-and-opengl-matrix-representation
    //
    // Such things as right-handed or left-handed matrices don't exist. 
    // Be it a right- or left- handed coordinate system, math and the matrix stays the same, 
    // matrix operations give the same result for both of these systems. 
    // How we will interpret the data later, thats the difference.
    //
    // In OpenGL, matrix operations are pre-concatenated, which means concatenated 
    // transformations are applied from right to left. Direct3D, however, applies 
    // concatenated transformations from left to right.
    //
    // So what you need to do is to match the order of operation in OpenGL to 
    // the order of Direct3D. When writing OpenGL code, you apply the transform that you 
    // want to occur first last; when writing DirectX you're doing the opposite. Let's say 
    // you just transformed vector v in OpenGL by matrices A and B: v' = ABv In Direct3D 
    // the same transformation would be v' = vBA.
    public static class MatrixConverter
    {
        static bool flip = false;
        
        // MATRIX
        public static Sce.Pss.Core.Matrix4 ToPSS(this Matrix mat)
        {
            if( flip )
            {
                return new Sce.Pss.Core.Matrix4(
                    mat.M11, mat.M21, mat.M31, mat.M41,
                    mat.M12, mat.M22, mat.M32, mat.M42,
                    mat.M13, mat.M23, mat.M33, mat.M43,
                    mat.M14, mat.M24, mat.M34, mat.M44
                    );
            }
            else
            {
                return new Sce.Pss.Core.Matrix4(
                    mat.M11, mat.M12, mat.M13, mat.M14,
                    mat.M21, mat.M22, mat.M23, mat.M24,
                    mat.M31, mat.M32, mat.M33, mat.M34,
                    mat.M41, mat.M42, mat.M43, mat.M44
                    );
            }

        }

        public static Matrix ToBlimey(this Sce.Pss.Core.Matrix4 mat)
        {
            if( flip )
            {
                return new Matrix(
                    mat.R0C0, mat.R1C0, mat.R2C0, mat.R3C0,
                    mat.R0C1, mat.R1C1, mat.R2C1, mat.R3C1,
                    mat.R0C2, mat.R1C2, mat.R2C2, mat.R3C2,
                    mat.R0C3, mat.R1C3, mat.R2C3, mat.R3C3
                    );
            }
            else
            {
                return new Matrix(
                    mat.R0C0, mat.R0C1, mat.R0C2, mat.R0C3,
                    mat.R1C0, mat.R1C1, mat.R1C2, mat.R1C3,
                    mat.R2C0, mat.R2C1, mat.R2C2, mat.R2C3,
                    mat.R3C0, mat.R3C1, mat.R3C2, mat.R3C3
                    );
            }
        }

    }
    

    public static class EnumConverter
    {
        public static TouchPhase ToBlimey(Sce.Pss.Core.Input.TouchStatus pss)
        {
            switch (pss)
            {
                case Sce.Pss.Core.Input.TouchStatus.Canceled: return TouchPhase.Invalid;
                case Sce.Pss.Core.Input.TouchStatus.None: return TouchPhase.Invalid;
                case Sce.Pss.Core.Input.TouchStatus.Move: return TouchPhase.Active;
                case Sce.Pss.Core.Input.TouchStatus.Down: return TouchPhase.JustPressed;
                case Sce.Pss.Core.Input.TouchStatus.Up: return TouchPhase.JustReleased;

                default: throw new Exception("problem");
            }
        }
        
        // PRIMITIVE TYPE
        public static Sce.Pss.Core.Graphics.DrawMode ToPSS(PrimitiveType blimey)
        {
            switch(blimey)
            {
                case PrimitiveType.LineList: return Sce.Pss.Core.Graphics.DrawMode.Lines;
                case PrimitiveType.LineStrip: return    Sce.Pss.Core.Graphics.DrawMode.LineStrip;
                case PrimitiveType.TriangleList: return Sce.Pss.Core.Graphics.DrawMode.Triangles;
                case PrimitiveType.TriangleStrip: return    Sce.Pss.Core.Graphics.DrawMode.TriangleStrip;
                    
                default: throw new Exception("problem");
            }
        }

        public static PrimitiveType ToBlimey(Sce.Pss.Core.Graphics.DrawMode pss)
        {
            switch(pss)
            {
                case Sce.Pss.Core.Graphics.DrawMode.Lines: return   PrimitiveType.LineList;
                case Sce.Pss.Core.Graphics.DrawMode.LineStrip: return   PrimitiveType.LineStrip;
                case Sce.Pss.Core.Graphics.DrawMode.Points:  throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.DrawMode.TriangleFan:  throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.DrawMode.Triangles: return   PrimitiveType.TriangleList;
                case Sce.Pss.Core.Graphics.DrawMode.TriangleStrip: return   PrimitiveType.TriangleStrip;
                
                default: throw new Exception("problem");

            }
        }

        // VERTEX ELEMENT FORMAT
        public static Sce.Pss.Core.Graphics.VertexFormat ToPSS(VertexElementFormat blimey)
        {
            switch(blimey)
            {
                case VertexElementFormat.Byte4: return Sce.Pss.Core.Graphics.VertexFormat.Byte4;
                case VertexElementFormat.Color: return Sce.Pss.Core.Graphics.VertexFormat.Float4;
                case VertexElementFormat.HalfVector2: return Sce.Pss.Core.Graphics.VertexFormat.Half2;
                case VertexElementFormat.HalfVector4: return Sce.Pss.Core.Graphics.VertexFormat.Half4;
                case VertexElementFormat.NormalizedShort2: return Sce.Pss.Core.Graphics.VertexFormat.Short2N;
                case VertexElementFormat.NormalizedShort4: return Sce.Pss.Core.Graphics.VertexFormat.Short4N;
                case VertexElementFormat.Short2: return Sce.Pss.Core.Graphics.VertexFormat.Short2;
                case VertexElementFormat.Short4: return Sce.Pss.Core.Graphics.VertexFormat.Short4;
                case VertexElementFormat.Single: return Sce.Pss.Core.Graphics.VertexFormat.Float;
                case VertexElementFormat.Vector2: return Sce.Pss.Core.Graphics.VertexFormat.Float2;
                case VertexElementFormat.Vector3: return Sce.Pss.Core.Graphics.VertexFormat.Float3;
                case VertexElementFormat.Vector4: return Sce.Pss.Core.Graphics.VertexFormat.Float4;
                
                default: throw new Exception("problem");
            }
        }

        public static VertexElementFormat ToBlimey(Sce.Pss.Core.Graphics.VertexFormat pss)
        {
            switch(pss)
            {
                case Sce.Pss.Core.Graphics.VertexFormat.None: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.Float: return VertexElementFormat.Single;
                case Sce.Pss.Core.Graphics.VertexFormat.Float2: return VertexElementFormat.Vector2;
                case Sce.Pss.Core.Graphics.VertexFormat.Float3: return VertexElementFormat.Vector3;
                case Sce.Pss.Core.Graphics.VertexFormat.Float4: return VertexElementFormat.Vector4;
                case Sce.Pss.Core.Graphics.VertexFormat.Half: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.Half2: return VertexElementFormat.HalfVector2;
                case Sce.Pss.Core.Graphics.VertexFormat.Half3: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.Half4: return VertexElementFormat.HalfVector4;
                case Sce.Pss.Core.Graphics.VertexFormat.Short: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.Short2: return VertexElementFormat.Short2;
                case Sce.Pss.Core.Graphics.VertexFormat.Short3: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.Short4: return VertexElementFormat.Short4;
                case Sce.Pss.Core.Graphics.VertexFormat.UShort: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.UShort2: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.UShort3: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.UShort4: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.Byte: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.Byte2: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.Byte3: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.Byte4: return VertexElementFormat.Byte4;
                case Sce.Pss.Core.Graphics.VertexFormat.UByte: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.UByte2: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.UByte3: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.UByte4: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.ShortN: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.Short2N: return VertexElementFormat.NormalizedShort2;
                case Sce.Pss.Core.Graphics.VertexFormat.Short3N: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.Short4N: return VertexElementFormat.NormalizedShort4;
                case Sce.Pss.Core.Graphics.VertexFormat.UShortN: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.UShort2N: throw new Exception("Not supported by Blimey");   
                case Sce.Pss.Core.Graphics.VertexFormat.UShort3N: throw new Exception("Not supported by Blimey");   
                case Sce.Pss.Core.Graphics.VertexFormat.UShort4N: throw new Exception("Not supported by Blimey");   
                case Sce.Pss.Core.Graphics.VertexFormat.ByteN: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.Byte2N: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.Byte3N: throw new Exception("Not supported by Blimey"); 
                case Sce.Pss.Core.Graphics.VertexFormat.Byte4N: throw new Exception("Not supported by Blimey");
                case Sce.Pss.Core.Graphics.VertexFormat.UByteN: throw new Exception("Not supported by Blimey");     
                case Sce.Pss.Core.Graphics.VertexFormat.UByte2N: throw new Exception("Not supported by Blimey");    
                case Sce.Pss.Core.Graphics.VertexFormat.UByte3N: throw new Exception("Not supported by Blimey");    
                case Sce.Pss.Core.Graphics.VertexFormat.UByte4N: throw new Exception("Not supported by Blimey");
                
                default: throw new Exception("problem");
            }
        }        
    }


    public static class VertexDeclarationConverter
    {

        public static Sce.Pss.Core.Graphics.VertexFormat[] ToPSS(this VertexDeclaration blimey)
        {
            Int32 blimeyStride = blimey.VertexStride;

            VertexElement[] blimeyElements = blimey.GetVertexElements();

            var pssElements = new Sce.Pss.Core.Graphics.VertexFormat[blimeyElements.Length];

            for (Int32 i = 0; i < blimeyElements.Length; ++i)
            {
                VertexElement elem = blimeyElements[i];
                pssElements[i] = elem.ToPSS();
            }

            return pssElements;
        }
    }
    

    public static class VertexElementConverter
    {

        public static Sce.Pss.Core.Graphics.VertexFormat ToPSS(this VertexElement blimey)
        {
            Int32 bliOffset = blimey.Offset;
            var bliElementFormat = blimey.VertexElementFormat;
            var bliElementUsage = blimey.VertexElementUsage;
            Int32 bliUsageIndex = blimey.UsageIndex;
            
            return EnumConverter.ToPSS(bliElementFormat);
        }
    }

    public class Engine
        : EngineCommon
        , IDisposable
        , IEngine
    {
        // PSS stuff
        Sce.Pss.Core.Graphics.GraphicsContext _graphicsContext;

        Stopwatch _timer = new Stopwatch();
        TimeSpan _previousTimeSpan;
        
        Single _elapsed;
        
        Int64 _frameCount;
        
        bool _running;
        
        EngineSettings settings;
        
        TouchScreen touchScreen;
        
        public EngineSettings Settings { get { return settings; } }
        
        public void Run()
        {
            _running = true;
            _timer.Start();
            
            while (_running) {
                Sce.Pss.Core.Environment.SystemEvents.CheckEvents ();
                this.Update ();
                this.Render ();
            }
            
            _timer.Stop();
        }

        public Engine(GameScene startScene, EngineSettings settings)
        {   
            this.settings = settings;
            
            // create new gfx device
            _graphicsContext = new Sce.Pss.Core.Graphics.GraphicsContext ();
            _graphicsContext.Enable(Sce.Pss.Core.Graphics.EnableMode.Blend);
            _graphicsContext.Enable(Sce.Pss.Core.Graphics.EnableMode.DepthTest);
            _graphicsContext.Enable(Sce.Pss.Core.Graphics.EnableMode.CullFace);
            
            _graphicsManager = new GraphicsManager(_graphicsContext);
            _resourceManager = new ResourceManager(_graphicsContext);
            touchScreen = new TouchScreen(this, _graphicsContext);
            _inputManager = new InputManager(this, touchScreen);
            _sceneManager = new SceneManager(this, startScene);
            
            
            _systemManager = new SystemManager(this, _graphicsContext, touchScreen);
        }
        

        void Update()
        {
            float dt = (float)(_timer.Elapsed.TotalSeconds - _previousTimeSpan.TotalSeconds);
            _previousTimeSpan = _timer.Elapsed;
            
            _elapsed += dt;
            
            var gt = new GameTime(dt, _elapsed, ++_frameCount);

            Boolean exit = Update(gt);
            
            if( exit )
            {
                _running = false;
            }
        }
        
        public override bool Update(GameTime time)
        {
            var gamePadData = Sce.Pss.Core.Input.GamePad.GetData (0);
            
            return base.Update(time);
        }

        public override void Render()
        {
            _graphicsContext.SetViewport(
                0, 
                0, 
                _graphicsContext.Screen.Width, 
                _graphicsContext.Screen.Height);
            
            base.Render();
            
            // Present the screen
            _graphicsContext.SwapBuffers ();

        }
        
        public void Dispose()
        {
            _graphicsContext.Dispose();
        }
    }

    public class GeometryBuffer
        : IGeometryBuffer
    {
        Sce.Pss.Core.Graphics.VertexBuffer _pssVertexBuf;
        
        IndexBuffer _iBuf;
        VertexBuffer _vBuf;
        
        public GeometryBuffer (VertexDeclaration vertexDeclaration, Int32 vertexCount, Int32 indexCount)
        {
            var pssDecl = vertexDeclaration.ToPSS();
            _pssVertexBuf = new Sce.Pss.Core.Graphics.VertexBuffer(vertexCount, indexCount, pssDecl);
            
            _iBuf = new IndexBuffer(_pssVertexBuf);
            _vBuf = new VertexBuffer(_pssVertexBuf, vertexDeclaration);
        }
        
        public IVertexBuffer VertexBuffer { get { return _vBuf; } }
        public IIndexBuffer IndexBuffer { get { return _iBuf; } }
        
        public Sce.Pss.Core.Graphics.VertexBuffer PSSVertexBuffer { get { return _pssVertexBuf; } }
        
    }

    public class GpuUtils
        : IGpuUtils
    {       
        public Int32 BeginEvent(Colour colour, String eventName) { return 0; }
        public Int32 EndEvent() { return 0; }

        public void SetMarker (Colour colour, String eventName){ }
        public void SetRegion (Colour colour, String eventName){ }
    }
    
    public class GraphicsManager
        : IGraphicsManager
    {
        Sce.Pss.Core.Graphics.GraphicsContext _graphicsContext;
        
        IGeometryBuffer _currentGeomBuffer;
        GpuUtils _gpuUtils;
        
        
        Int32 _drawCallCount = 0;
        
        public Int32 DrawCallCount { get { return _drawCallCount; } }
        
        public void MarkNewFrame(){ _drawCallCount = 0; }
        
        public GraphicsManager(Sce.Pss.Core.Graphics.GraphicsContext graphicsContext)
        {

            _graphicsContext = graphicsContext;
            _gpuUtils = new GpuUtils();
            
            _graphicsContext.Enable( Sce.Pss.Core.Graphics.EnableMode.CullFace, false );
            _graphicsContext.Enable( Sce.Pss.Core.Graphics.EnableMode.Blend, true );
            _graphicsContext.Enable( Sce.Pss.Core.Graphics.EnableMode.DepthTest, true );
        
        }

        public void Clear(Colour col)
        {
            _graphicsContext.SetClearColor (col.R, col.G, col.B, col.A);
            _graphicsContext.Clear ();
        }
        
        public void ClearDepthBuffer()
        {
            _graphicsContext.SetClearDepth(1f);
            _graphicsContext.Clear (Sce.Pss.Core.Graphics.ClearMask.Depth);
        }

        public IGeometryBuffer CreateGeometryBuffer(
            VertexDeclaration vertexDeclaration, Int32 vertexCount, Int32 indexCount)
        {
            return new GeometryBuffer(vertexDeclaration, vertexCount, indexCount);
        }
        
        public IGpuUtils GpuUtils
        {
            get
            {
                return _gpuUtils;
            }
        }
        
        public void SetGeometryBuffer(IGeometryBuffer buffer)
        {
            var pssGeomBuff = buffer as GeometryBuffer;
            
            _graphicsContext.SetVertexBuffer(0, pssGeomBuff.PSSVertexBuffer);
            
            _currentGeomBuffer = buffer;
        }

        public void DrawIndexedPrimitives(
            PrimitiveType primitiveType, 
            int baseVertex, 
            int minVertexIndex, 
            int numVertices, 
            int startIndex, 
            int primitiveCount)
        {
            var pssPrimType = EnumConverter.ToPSS(primitiveType);
            
            int numVertsInPrim = numVertices / primitiveCount;
            
            _graphicsContext.DrawArrays(pssPrimType, 0, primitiveCount * PrimitiveHelper.NumVertsIn(primitiveType));
        }

        public void DrawUserPrimitives<T>(
            PrimitiveType primitiveType, 
            T[] vertexData, 
            int vertexOffset, 
            int primitiveCount, 
            VertexDeclaration vertexDeclaration) 
            where T : struct, IVertexType
        {
            var pssDrawMode = EnumConverter.ToPSS(primitiveType);
            
            var geomBuff = this.CreateGeometryBuffer(vertexDeclaration, vertexData.Length, 0) as GeometryBuffer;
            
            geomBuff.VertexBuffer.SetData(vertexData);
            
            this.SetGeometryBuffer(geomBuff);
            
            _graphicsContext.DrawArrays(pssDrawMode, 0, primitiveCount * PrimitiveHelper.NumVertsIn(primitiveType));
            
            geomBuff.PSSVertexBuffer.Dispose();
        }
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class IndexBuffer
        : IIndexBuffer
    {
        Sce.Pss.Core.Graphics.VertexBuffer _pssBuffer;
        
        public IndexBuffer(Sce.Pss.Core.Graphics.VertexBuffer buffer)
        {
            _pssBuffer = buffer;
        }

        public void GetData(UInt16[] data)
        {
            throw new System.NotImplementedException();
        }

        public void GetData(UInt16[] data, int startIndex, int elementCount)
        {
            throw new System.NotImplementedException();
        }

        public void GetData(int offsetInBytes, UInt16[] data, int startIndex, int elementCount)
        {
            throw new System.NotImplementedException();
        }

        public void SetData(UInt16[] data)
        {
            _pssBuffer.SetIndices(data);
        }

        public void SetData(UInt16[] data, int startIndex, int elementCount)
        {
            throw new System.NotImplementedException();
        }

        public void SetData(int offsetInBytes, UInt16[] data, int startIndex, int elementCount)
        {
            throw new System.NotImplementedException();
        }

        public int IndexCount 
        { 
            get
            {
                return _pssBuffer.IndexCount;
            }
        }
    }

    public class InputManager
        : IInputManager
    {
        public Xbox360Controller GetXbox360Controller(PlayerIndex player) { return null; }
        public MultiTouchController GetTouchScreen() { return _vitaTouchScreen; }
        public VitaController GetVitaController() { return _controls; }
        public GenericGamepad GetGenericGamepad() { return _genericPad; }
        
        
        TouchScreen _vitaTouchScreen;
        VitaControllerImplementation _controls;
        GenericGamepad _genericPad;
        
        public InputManager(IEngine engine, TouchScreen screen)
        {
            _controls = new VitaControllerImplementation();
            _genericPad = new GenericGamepad(this);
            _vitaTouchScreen = screen;
        }
    
        public void Update(GameTime time)
        {
            _vitaTouchScreen.Update(time);
            _controls.Update(time);
            _genericPad.Update(time);
        }
    }

    public class Phong_PositionNormal_ShaderEffectPass
        : IEffectPass
    {
        Sce.Pss.Core.Graphics.ShaderProgram _program;
        Sce.Pss.Core.Graphics.GraphicsContext _gfxContext;
        
        public Phong_PositionNormal_ShaderEffectPass(Sce.Pss.Core.Graphics.GraphicsContext gfxContext, Sce.Pss.Core.Graphics.ShaderProgram program)
        {
            _gfxContext = gfxContext;
            _program = program;
        }
        
        public void Apply()
        {
            _gfxContext.SetShaderProgram( _program );
        }
    }
    
    public class Phong_PositionNormal
        : IShader
    {
        Sce.Pss.Core.Graphics.ShaderProgram _program;
        Sce.Pss.Core.Graphics.GraphicsContext _gfxContext;
            
        Phong_PositionNormal_ShaderEffectPass[] _pass;
        

        public Phong_PositionNormal(
            Sce.Pss.Core.Graphics.GraphicsContext gfxContext, 
            Sce.Pss.Core.Graphics.ShaderProgram program)
        {
            _gfxContext = gfxContext;
            _program = program;
            _program.SetUniformBinding(0, "WorldViewProj");     
            _program.SetAttributeBinding( 0, "a_Position" ) ;
            _program.SetAttributeBinding( 1, "a_Normal" ) ;
            
            _pass = new Phong_PositionNormal_ShaderEffectPass[] { new Phong_PositionNormal_ShaderEffectPass(_gfxContext, _program) }; 
        }

        public IEffectPass[] Passes
        {
            get
            {
                return _pass;
            }
        }

        public void Calibrate(Dictionary<string, ShaderSettingsData> settings)
        {
            Matrix world = Matrix.Identity;
            Matrix view = Matrix.Identity;
            Matrix proj = Matrix.Identity;
            Colour col = Colour.White;
            
            foreach (var param in settings.Keys)
            {
                if (param == "_world")
                {

                    world = ((Matrix) settings[param].Value);
                    continue;
                }

                if (param == "_view")
                {
                    view = ((Matrix)settings[param].Value);
                    continue;
                }

                if (param == "_proj")
                {
                    proj = ((Matrix)settings[param].Value);
                    continue;
                }

                if (param == "_colour")
                {
                    col = ((Colour)settings[param].Value);
                    continue;
                }
            }
            
            
            var worldViewProj = (world * view * proj).ToPSS();
            
            _program.SetUniformValue( _program.FindUniform( "WorldViewProj" ), ref worldViewProj );
            
            Matrix invWorld = Matrix.Invert(world);
            
            
            Sce.Pss.Core.Vector3 lightPos = new Sce.Pss.Core.Vector3( 0.0f, 15.0f, 13.0f );
            
            // model local Light Direction
            Sce.Pss.Core.Matrix4 worldInverse = invWorld.ToPSS();
            Sce.Pss.Core.Vector4 localLightPos4 = 
                worldInverse * (new Sce.Pss.Core.Vector4( lightPos, 1.0f ));

            Sce.Pss.Core.Vector3 localLightDir = 
                new Sce.Pss.Core.Vector3( -localLightPos4.X, -localLightPos4.Y, -localLightPos4.Z );
            
            localLightDir = localLightDir.Normalize();
    
            _program.SetUniformValue( _program.FindUniform( "LocalLightDirection" ), ref localLightDir );
                
            
            Sce.Pss.Core.Vector3 camPos = proj.Translation.ToPSS();
            
            // model local eye
            Sce.Pss.Core.Vector4 localEye4 = worldInverse * (new Sce.Pss.Core.Vector4( camPos, 1.0f ));
            Sce.Pss.Core.Vector3 localEye = new Sce.Pss.Core.Vector3( localEye4.X, localEye4.Y, localEye4.Z);
    
            _program.SetUniformValue( _program.FindUniform( "EyePosition" ), ref localEye );
    

            // light
            Sce.Pss.Core.Vector4 i_a = new Sce.Pss.Core.Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
            _program.SetUniformValue( _program.FindUniform( "IAmbient" ), ref i_a );
            
            
            Sce.Pss.Core.Vector4 i_d = new Sce.Pss.Core.Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
            _program.SetUniformValue( _program.FindUniform( "IDiffuse" ), ref i_d );
    
            // material
            Sce.Pss.Core.Vector4 k_a = new Sce.Pss.Core.Vector4( 0.2f, 0.2f, 0.2f, 1.0f );
            _program.SetUniformValue( _program.FindUniform( "KAmbient" ), ref k_a );
    
            Sce.Pss.Core.Vector4 k_d    = col.ToVector4().ToPSS();
            _program.SetUniformValue( _program.FindUniform( "KDiffuse" ), ref k_d );
    
            Sce.Pss.Core.Vector4 k_s = new Sce.Pss.Core.Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
            _program.SetUniformValue( _program.FindUniform( "KSpecular" ), ref k_s );
            _program.SetUniformValue( _program.FindUniform( "Shininess" ), 5.0f );
        }
    }

    public class ResourceManager
        : IOldResourceManager
    {

        IShader _phong_positionNormal;
        IShader _gouraud_positionNormal;
        IShader _unlit_position;
        IShader _unlit_positionTexture;
        IShader _unlit_positionColour;
        IShader _unlit_positionColourTexture;

        public ResourceManager(Sce.Pss.Core.Graphics.GraphicsContext gfxContext)
        {
            var program1 = new Sce.Pss.Core.Graphics.ShaderProgram("/Application/Shaders/Phong.cgx");
            _phong_positionNormal = new Phong_PositionNormal(gfxContext, program1);

            var program2 = new Sce.Pss.Core.Graphics.ShaderProgram("/Application/Shaders/Gouraud.cgx");
            _gouraud_positionNormal = new Gouraud_PositionNormal(gfxContext, program2);

            var program3 = new Sce.Pss.Core.Graphics.ShaderProgram("/Application/Shaders/Unlit_Position.cgx");
            _unlit_position = new Unlit_Position(gfxContext, program3);

            var program4 = new Sce.Pss.Core.Graphics.ShaderProgram("/Application/Shaders/Unlit_PositionTexture.cgx");
            _unlit_positionTexture = new Unlit_PositionTexture(gfxContext, program4);

            var program5 = new Sce.Pss.Core.Graphics.ShaderProgram("/Application/Shaders/Unlit_PositionColour.cgx");
            _unlit_positionColour = new Unlit_PositionColour(gfxContext, program5);

            var program6 = new Sce.Pss.Core.Graphics.ShaderProgram("/Application/Shaders/Unlit_PositionColourTexture.cgx");
            _unlit_positionColourTexture = new Unlit_PositionColourTexture(gfxContext, program6);

        }

        public T Load<T>(Uri uri) where T : IOldResource
        {
            return default(T);
        }

        public IShader GetShader(ShaderType shaderType, VertexDeclaration vertDecl)
        {
            var vertElems = vertDecl.GetVertexElements();

            var usage = new HashSet<VertexElementUsage>();

            foreach (var elem in vertElems)
            {
                usage.Add(elem.VertexElementUsage);
            }

            switch(shaderType)
            {
                case ShaderType.Gouraud: return GetGouraudShaderFor(usage);
                case ShaderType.Phong: return GetPhongShaderFor(usage);
                case ShaderType.Unlit: return GetUnlitShaderFor(usage);
                default: return null;
            }

        }

        IShader GetGouraudShaderFor(HashSet<VertexElementUsage> usage)
        {
            if ( usage.Contains(VertexElementUsage.Position) && usage.Contains(VertexElementUsage.Normal) )
            {
                return _gouraud_positionNormal;
            }

            throw new Exception("No suitable shader for this vertDecl");
        }

        IShader GetPhongShaderFor(HashSet<VertexElementUsage> usage)
        {
            if (usage.Contains(VertexElementUsage.Position) && usage.Contains(VertexElementUsage.Normal))
            {
                return _phong_positionNormal;
            }

            return null;
        }

        IShader GetUnlitShaderFor(HashSet<VertexElementUsage> usage)
        {

            if (usage.Contains(VertexElementUsage.Position) && usage.Contains(VertexElementUsage.Colour) && usage.Contains(VertexElementUsage.TextureCoordinate))
            {
                return _unlit_positionColourTexture;
            }

            if (usage.Contains(VertexElementUsage.Position) && usage.Contains(VertexElementUsage.Colour))
            {
                return _unlit_positionColour;
            }

            if (usage.Contains(VertexElementUsage.Position) && usage.Contains(VertexElementUsage.TextureCoordinate))
            {
                return _unlit_positionTexture;
            }

            if (usage.Contains(VertexElementUsage.Position))
            {
                return _unlit_position;
            }

            throw new Exception("No suitable shader for this vertDecl");
        }
    }

    public class SystemManager
        : ISystemManager
    {
        TouchScreen touchScreen;
        
        public SystemManager(
            IEngine engine, 
            Sce.Pss.Core.Graphics.GraphicsContext gfxContext,
            TouchScreen touchScreen)
        {
            this.touchScreen = touchScreen;
        }

        public string OS
        { 
            get
            {
                return System.Environment.OSVersion.Platform.ToString();
            }
        }

        internal void SetDeviceOrientation(Blimey.DeviceOrientation orientation)
        {
            _orientation = orientation;
        }

        DeviceOrientation _orientation = Blimey.DeviceOrientation.Default;
        public DeviceOrientation DeviceOrientation
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
                return touchScreen;
            }
        }

        public IPanelSpecification PanelSpecification
        {
            get
            {
                return touchScreen;
            }
        }
    }

    public class TouchScreen
        : MultiTouchController
        , IScreenSpecification
        , IPanelSpecification
    {
        Int32 touchIDCounter = -1;
        
        HashSet<Int32> previousActiveFingerIDs = new HashSet<Int32>();
        HashSet<Int32> currentActiveFingerIDs = new HashSet<Int32>();
        
        Dictionary<Int32, Int32> currentFingerIDToTouchID = new Dictionary<Int32, Int32>();
        
        public override IPanelSpecification PanelSpecification { get{ return this; } }
        Sce.Pss.Core.Graphics.GraphicsContext gfx;
        
        internal TouchScreen(IEngine engine, Sce.Pss.Core.Graphics.GraphicsContext gfx)
            : base(engine)
        {
            this.gfx = gfx;
        }

        internal override void Update(GameTime time)
        {
            this.touchCollection.ClearBuffer();
            
            previousActiveFingerIDs = currentActiveFingerIDs;
            currentActiveFingerIDs = new HashSet<Int32>();

            var data = Sce.Pss.Core.Input.Touch.GetData(0);
            
            var keys = currentFingerIDToTouchID.Keys.ToArray();
            foreach(var fingerId in keys)
            {
                var selectResult = data.Select(x => x.ID == fingerId);
                if( selectResult.Count() == 0 )
                    currentFingerIDToTouchID.Remove(fingerId);
            }
                
            foreach (var touchData in data) 
            {       
                var phase = EnumConverter.ToBlimey(touchData.Status);

                if( phase != TouchPhase.Invalid )
                {
                    Int32 fingerID = touchData.ID;
                    currentActiveFingerIDs.Add(fingerID);
                }
            }
            
            foreach (var touchData in data) 
            {
                var phase = EnumConverter.ToBlimey(touchData.Status);

                if( phase != TouchPhase.Invalid )
                {
                    Int32 fingerID = touchData.ID;

                    if( !previousActiveFingerIDs.Contains(fingerID) &&
                       currentActiveFingerIDs.Contains(fingerID) )
                    {
                        currentFingerIDToTouchID.Add(fingerID, ++touchIDCounter);                       
                    }
                    
                    if( previousActiveFingerIDs.Contains(fingerID) &&
                       !currentActiveFingerIDs.Contains(fingerID) )
                    {
                        currentFingerIDToTouchID.Remove(fingerID);                      
                    }
                }
            }
            
            foreach (var touchData in data)
            {
                var phase = EnumConverter.ToBlimey(touchData.Status);

                if( phase != TouchPhase.Invalid )
                {
                    Int32 fingerID = touchData.ID;
                    
                    Int32 touchID = currentFingerIDToTouchID[fingerID];
                    
                    Vector2 pos = new Vector2(touchData.X, touchData.Y);

                    this.touchCollection.RegisterTouch(touchID, pos, phase, time.FrameNumber, time.Elapsed);
                }
            }
            
            base.Update(time);
        }
        
        
        public Single ScreenResolutionAspectRatio
        { 
            get
            {
                return ScreenResolutionWidth / ScreenResolutionHeight;
            }
        }
        public Int32 ScreenResolutionWidth
        { 
            get
            {
                return gfx.Screen.Width;
            }
        }
        public Int32 ScreenResolutionHeight
        { 
            get
            {
                return gfx.Screen.Height;
            }
        }
        
        public Vector2 PanelPhysicalSize
        { 
            get
            {
                return new Vector2(0.11f, 0.63f);
            }
        }
        public Single PanelPhysicalAspectRatio
        { 
            get
            {
                return PanelPhysicalSize.X / PanelPhysicalSize.Y;
            }
        }

        public PanelType PanelType
        { 
            get
            {
                return PanelType.TouchScreen;
            }
        }
    }

    public class Unlit_Position_ShaderEffectPass
        : IEffectPass
    {
        Sce.Pss.Core.Graphics.ShaderProgram _program;
        Sce.Pss.Core.Graphics.GraphicsContext _gfxContext;
        
        public Unlit_Position_ShaderEffectPass(Sce.Pss.Core.Graphics.GraphicsContext gfxContext, Sce.Pss.Core.Graphics.ShaderProgram program)
        {
            _gfxContext = gfxContext;
            _program = program;
        }
        
        public void Apply()
        {
            _gfxContext.SetShaderProgram( _program );
        }
    }
    
    public class Unlit_Position
        : IShader
    {
        Sce.Pss.Core.Graphics.ShaderProgram _program;
        Sce.Pss.Core.Graphics.GraphicsContext _gfxContext;
            
        Unlit_Position_ShaderEffectPass[] _pass;
        

        public Unlit_Position(
            Sce.Pss.Core.Graphics.GraphicsContext gfxContext, 
            Sce.Pss.Core.Graphics.ShaderProgram program)
        {
            _gfxContext = gfxContext;
            _program = program;
            _program.SetUniformBinding(0, "WorldViewProj");     
            _program.SetAttributeBinding( 0, "a_Position" ) ;
            
            _pass = new Unlit_Position_ShaderEffectPass[] { new Unlit_Position_ShaderEffectPass(_gfxContext, _program) }; 
        }

        public IEffectPass[] Passes
        {
            get
            {
                return _pass;
            }
        }

        public void Calibrate(Dictionary<string, ShaderSettingsData> settings)
        {
            Matrix world = Matrix.Identity;
            Matrix view = Matrix.Identity;
            Matrix proj = Matrix.Identity;
            Colour col = Colour.White;
            
            foreach (var param in settings.Keys)
            {
                if (param == "_world")
                {

                    world = ((Matrix) settings[param].Value);
                    continue;
                }

                if (param == "_view")
                {
                    view = ((Matrix)settings[param].Value);
                    continue;
                }

                if (param == "_proj")
                {
                    proj = ((Matrix)settings[param].Value);
                    continue;
                }

                if (param == "_colour")
                {
                    col = ((Colour)settings[param].Value);
                    continue;
                }
            }
            
            
            var worldViewProj = (world * view * proj).ToPSS();
            
            _program.SetUniformValue( 0, ref worldViewProj );
            
    
            Sce.Pss.Core.Vector4 materialColour = col.ToVector4().ToPSS();
            _program.SetUniformValue( _program.FindUniform( "MaterialColor" ), ref materialColour );

        }
    }

    public class Unlit_PositionColour_ShaderEffectPass
        : IEffectPass
    {
        Sce.Pss.Core.Graphics.ShaderProgram _program;
        Sce.Pss.Core.Graphics.GraphicsContext _gfxContext;
        
        public Unlit_PositionColour_ShaderEffectPass(Sce.Pss.Core.Graphics.GraphicsContext gfxContext, Sce.Pss.Core.Graphics.ShaderProgram program)
        {
            _gfxContext = gfxContext;
            _program = program;
        }
        
        public void Apply()
        {
            _gfxContext.SetShaderProgram( _program );
        }
    }
    
    public class Unlit_PositionColour
        : IShader
    {
        Sce.Pss.Core.Graphics.ShaderProgram _program;
        Sce.Pss.Core.Graphics.GraphicsContext _gfxContext;
            
        Unlit_PositionColour_ShaderEffectPass[] _pass;
        

        public Unlit_PositionColour(
            Sce.Pss.Core.Graphics.GraphicsContext gfxContext, 
            Sce.Pss.Core.Graphics.ShaderProgram program)
        {
            _gfxContext = gfxContext;
            _program = program;
            _program.SetUniformBinding(0, "WorldViewProj");     
            _program.SetAttributeBinding( 0, "a_Position" ) ;
            _program.SetAttributeBinding( 1, "a_Color0" ) ;
            
            _pass = new Unlit_PositionColour_ShaderEffectPass[] { new Unlit_PositionColour_ShaderEffectPass(_gfxContext, _program) }; 
        }

        public IEffectPass[] Passes
        {
            get
            {
                return _pass;
            }
        }

        public void Calibrate(Dictionary<string, ShaderSettingsData> settings)
        {
            Matrix world = Matrix.Identity;
            Matrix view = Matrix.Identity;
            Matrix proj = Matrix.Identity;
            Colour col = Colour.White;
            
            foreach (var param in settings.Keys)
            {
                if (param == "_world")
                {

                    world = ((Matrix) settings[param].Value);
                    continue;
                }

                if (param == "_view")
                {
                    view = ((Matrix)settings[param].Value);
                    continue;
                }

                if (param == "_proj")
                {
                    proj = ((Matrix)settings[param].Value);
                    continue;
                }

                if (param == "_colour")
                {
                    col = ((Colour)settings[param].Value);
                    continue;
                }
            }
            
            
            var worldViewProj = (world * view * proj).ToPSS();
            
            _program.SetUniformValue( 0, ref worldViewProj );
            
            Sce.Pss.Core.Vector4 materialColour = col.ToVector4().ToPSS();
            _program.SetUniformValue( _program.FindUniform( "MaterialColor" ), ref materialColour );
        }
    }


    public class Unlit_PositionColourTexture_ShaderEffectPass
        : IEffectPass
    {
        Sce.Pss.Core.Graphics.ShaderProgram _program;
        Sce.Pss.Core.Graphics.GraphicsContext _gfxContext;
        
        public Unlit_PositionColourTexture_ShaderEffectPass(Sce.Pss.Core.Graphics.GraphicsContext gfxContext, Sce.Pss.Core.Graphics.ShaderProgram program)
        {
            _gfxContext = gfxContext;
            _program = program;
        }
        
        public void Apply()
        {
            _gfxContext.SetShaderProgram( _program );
        }
    }
    
    public class Unlit_PositionColourTexture
        : IShader
    {
        Sce.Pss.Core.Graphics.ShaderProgram _program;
        Sce.Pss.Core.Graphics.GraphicsContext _gfxContext;
            
        Unlit_PositionColourTexture_ShaderEffectPass[] _pass;
        

        public Unlit_PositionColourTexture(
            Sce.Pss.Core.Graphics.GraphicsContext gfxContext, 
            Sce.Pss.Core.Graphics.ShaderProgram program)
        {
            _gfxContext = gfxContext;
            _program = program;
            _program.SetUniformBinding(0, "WorldViewProj");     
            _program.SetAttributeBinding( 0, "a_Position" ) ;
            _program.SetAttributeBinding( 1, "a_Color0" ) ;
            _program.SetAttributeBinding( 2, "a_TexCoord" ) ;
            
            _pass = new Unlit_PositionColourTexture_ShaderEffectPass[] { new Unlit_PositionColourTexture_ShaderEffectPass(_gfxContext, _program) }; 
        }

        public IEffectPass[] Passes
        {
            get
            {
                return _pass;
            }
        }

        public void Calibrate(Dictionary<string, ShaderSettingsData> settings)
        {
            Matrix world = Matrix.Identity;
            Matrix view = Matrix.Identity;
            Matrix proj = Matrix.Identity;
            Colour col = Colour.White;
            
            foreach (var param in settings.Keys)
            {
                if (param == "_world")
                {

                    world = ((Matrix) settings[param].Value);
                    continue;
                }

                if (param == "_view")
                {
                    view = ((Matrix)settings[param].Value);
                    continue;
                }

                if (param == "_proj")
                {
                    proj = ((Matrix)settings[param].Value);
                    continue;
                }

                if (param == "_colour")
                {
                    col = ((Colour)settings[param].Value);
                    continue;
                }
            }
            
            
            var worldViewProj = (world * view * proj).ToPSS();
            
            _program.SetUniformValue( 0, ref worldViewProj );
            
            Sce.Pss.Core.Vector4 materialColour = col.ToVector4().ToPSS();
            _program.SetUniformValue( _program.FindUniform( "MaterialColor" ), ref materialColour );
        }
    }


    public class Unlit_PositionTexture_ShaderEffectPass
        : IEffectPass
    {
        Sce.Pss.Core.Graphics.ShaderProgram _program;
        Sce.Pss.Core.Graphics.GraphicsContext _gfxContext;
        
        public Unlit_PositionTexture_ShaderEffectPass(Sce.Pss.Core.Graphics.GraphicsContext gfxContext, Sce.Pss.Core.Graphics.ShaderProgram program)
        {
            _gfxContext = gfxContext;
            _program = program;
        }
        
        public void Apply()
        {
            _gfxContext.SetShaderProgram( _program );
        }
    }
    
    public class Unlit_PositionTexture
        : IShader
    {
        Sce.Pss.Core.Graphics.ShaderProgram _program;
        Sce.Pss.Core.Graphics.GraphicsContext _gfxContext;
            
        Unlit_PositionTexture_ShaderEffectPass[] _pass;
        

        public Unlit_PositionTexture(
            Sce.Pss.Core.Graphics.GraphicsContext gfxContext, 
            Sce.Pss.Core.Graphics.ShaderProgram program)
        {
            _gfxContext = gfxContext;
            _program = program;
            _program.SetUniformBinding(0, "WorldViewProj");
            _program.SetAttributeBinding( 0, "a_Position" ) ;
            _program.SetAttributeBinding( 1, "a_TexCoord" ) ;
            
            _pass = new Unlit_PositionTexture_ShaderEffectPass[] { new Unlit_PositionTexture_ShaderEffectPass(_gfxContext, _program) }; 
        }

        public IEffectPass[] Passes
        {
            get
            {
                return _pass;
            }
        }

        public void Calibrate(Dictionary<string, ShaderSettingsData> settings)
        {
            Matrix world = Matrix.Identity;
            Matrix view = Matrix.Identity;
            Matrix proj = Matrix.Identity;
            Colour col = Colour.White;
            
            foreach (var param in settings.Keys)
            {
                if (param == "_world")
                {

                    world = ((Matrix) settings[param].Value);
                    continue;
                }

                if (param == "_view")
                {
                    view = ((Matrix)settings[param].Value);
                    continue;
                }

                if (param == "_proj")
                {
                    proj = ((Matrix)settings[param].Value);
                    continue;
                }

                if (param == "_colour")
                {
                    col = ((Colour)settings[param].Value);
                    continue;
                }
            }
            
            
            var worldViewProj = (world * view * proj).ToPSS();
            
            _program.SetUniformValue( 0, ref worldViewProj );

                                                           
            Sce.Pss.Core.Vector4 materialColour = col.ToVector4().ToPSS();
            _program.SetUniformValue( _program.FindUniform( "MaterialColor" ), ref materialColour );
        }
    }

    public class Unlit_PositionTexture_ShaderEffectPass
        : IEffectPass
    {
        Sce.Pss.Core.Graphics.ShaderProgram _program;
        Sce.Pss.Core.Graphics.GraphicsContext _gfxContext;
        
        public Unlit_PositionTexture_ShaderEffectPass(Sce.Pss.Core.Graphics.GraphicsContext gfxContext, Sce.Pss.Core.Graphics.ShaderProgram program)
        {
            _gfxContext = gfxContext;
            _program = program;
        }
        
        public void Apply()
        {
            _gfxContext.SetShaderProgram( _program );
        }
    }
    
    public class Unlit_PositionTexture
        : IShader
    {
        Sce.Pss.Core.Graphics.ShaderProgram _program;
        Sce.Pss.Core.Graphics.GraphicsContext _gfxContext;
            
        Unlit_PositionTexture_ShaderEffectPass[] _pass;
        

        public Unlit_PositionTexture(
            Sce.Pss.Core.Graphics.GraphicsContext gfxContext, 
            Sce.Pss.Core.Graphics.ShaderProgram program)
        {
            _gfxContext = gfxContext;
            _program = program;
            _program.SetUniformBinding(0, "WorldViewProj");
            _program.SetAttributeBinding( 0, "a_Position" ) ;
            _program.SetAttributeBinding( 1, "a_TexCoord" ) ;
            
            _pass = new Unlit_PositionTexture_ShaderEffectPass[] { new Unlit_PositionTexture_ShaderEffectPass(_gfxContext, _program) }; 
        }

        public IEffectPass[] Passes
        {
            get
            {
                return _pass;
            }
        }

        public void Calibrate(Dictionary<string, ShaderSettingsData> settings)
        {
            Matrix world = Matrix.Identity;
            Matrix view = Matrix.Identity;
            Matrix proj = Matrix.Identity;
            Colour col = Colour.White;
            
            foreach (var param in settings.Keys)
            {
                if (param == "_world")
                {

                    world = ((Matrix) settings[param].Value);
                    continue;
                }

                if (param == "_view")
                {
                    view = ((Matrix)settings[param].Value);
                    continue;
                }

                if (param == "_proj")
                {
                    proj = ((Matrix)settings[param].Value);
                    continue;
                }

                if (param == "_colour")
                {
                    col = ((Colour)settings[param].Value);
                    continue;
                }
            }
            
            
            var worldViewProj = (world * view * proj).ToPSS();
            
            _program.SetUniformValue( 0, ref worldViewProj );

                                                           
            Sce.Pss.Core.Vector4 materialColour = col.ToVector4().ToPSS();
            _program.SetUniformValue( _program.FindUniform( "MaterialColor" ), ref materialColour );
        }
    }

    public class VertexBuffer
        : IVertexBuffer
    {
        Sce.Pss.Core.Graphics.VertexBuffer _pssBuffer;
        VertexDeclaration _vertexDeclaration;

        public VertexBuffer(Sce.Pss.Core.Graphics.VertexBuffer buffer, VertexDeclaration vertexDeclaration)
        {
            _vertexDeclaration = vertexDeclaration;
            _pssBuffer = buffer;
        }
        public VertexDeclaration VertexDeclaration 
        { 
            get
            {
                return _vertexDeclaration;
            }
        }

        public void SetData<T>(T[] data) where T : struct, IVertexType
        {
            // TODO: FIX THIS METHOD, IT IS SHIT AND SLOW, SHOULD NOT BE HARDCODED
            
            var vertPosData = data as VertexPosition[];
            if( vertPosData != null ){ this.SetVertPosData( vertPosData ); return; }
            
            var vertPosColData = data as VertexPositionColour[];
            if( vertPosColData != null ){ this.SetVertPosColData( vertPosColData ); return; }
            
            var vertPosNormData = data as VertexPositionNormal[];
            if( vertPosNormData != null ){ this.SetVertPosNormData( vertPosNormData ); return; }
            
            var vertPosNormColData = data as VertexPositionNormalColour[];
            if( vertPosNormColData != null ){ this.SetVertPosNormColData( vertPosNormColData ); return; }
            
            var vertPosNormTexData = data as VertexPositionNormalTexture[];
            if( vertPosNormTexData != null ){ this.SetVertPosNormTexData( vertPosNormTexData ); return; }
            
            var vertPosNormTexColData = data as VertexPositionNormalTextureColour[];
            if( vertPosNormTexColData != null ){ this.SetVertPosNormTexColData( vertPosNormTexColData ); return; }
            
            throw new System.NotImplementedException();
        }
        
        public void GetData<T>(T[] data) where T : struct, IVertexType
        {
            throw new System.NotImplementedException();
        }

        void SetVertPosData(VertexPosition[] data)
        {
            var _pos = new List<Sce.Pss.Core.Vector3>();
            
            foreach(var dat in data)
            {
                _pos.Add(dat.Position.ToPSS());
            }
            
            _pssBuffer.SetVertices(0, _pos.ToArray());
        }
        
        void SetVertPosColData(VertexPositionColour[] data)
        {
            var _pos = new List<Sce.Pss.Core.Vector3>();
            var _colour = new List<Sce.Pss.Core.Vector4>();
            
            foreach(var dat in data)
            {
                _pos.Add(dat.Position.ToPSS());
                _colour.Add(dat.Colour.ToVector4().ToPSS());
            }
            
            _pssBuffer.SetVertices(0, _pos.ToArray());
            _pssBuffer.SetVertices(1, _colour.ToArray());
        }
        
        void SetVertPosNormData(VertexPositionNormal[] data)
        {
            var _pos = new List<Sce.Pss.Core.Vector3>();
            var _normal = new List<Sce.Pss.Core.Vector3>();
            
            foreach(var dat in data)
            {
                _pos.Add(dat.Position.ToPSS());
                _normal.Add(dat.Normal.ToPSS());
            }
            
            _pssBuffer.SetVertices(0, _pos.ToArray());
            _pssBuffer.SetVertices(1, _normal.ToArray());
        }
        
        void SetVertPosNormColData(VertexPositionNormalColour[] data)
        {
            var _pos = new List<Sce.Pss.Core.Vector3>();
            var _normal = new List<Sce.Pss.Core.Vector3>();
            var _colour = new List<Sce.Pss.Core.Vector4>();
            
            foreach(var dat in data)
            {
                _pos.Add(dat.Position.ToPSS());
                _normal.Add(dat.Normal.ToPSS());
                _colour.Add(dat.Colour.ToVector4().ToPSS());
            }
            
            _pssBuffer.SetVertices(0, _pos.ToArray());
            _pssBuffer.SetVertices(1, _normal.ToArray());
            _pssBuffer.SetVertices(2, _colour.ToArray());
            
        }
        
        void SetVertPosNormTexData(VertexPositionNormalTexture[] data)
        {
            var _pos = new List<Sce.Pss.Core.Vector3>();
            var _normal = new List<Sce.Pss.Core.Vector3>();
            var _texture = new List<Sce.Pss.Core.Vector2>();
            
            foreach(var dat in data)
            {
                _pos.Add(dat.Position.ToPSS());
                _normal.Add(dat.Normal.ToPSS());
                _texture.Add(dat.UV.ToPSS());
            }
            
            _pssBuffer.SetVertices(0, _pos.ToArray());
            _pssBuffer.SetVertices(1, _normal.ToArray());
            _pssBuffer.SetVertices(2, _texture.ToArray());
        }
        
        void SetVertPosNormTexColData(VertexPositionNormalTextureColour[] data)
        {
            var _pos = new List<Sce.Pss.Core.Vector3>();
            var _normal = new List<Sce.Pss.Core.Vector3>();
            var _texture = new List<Sce.Pss.Core.Vector2>();
            var _colour = new List<Sce.Pss.Core.Vector4>();
            
            foreach(var dat in data)
            {
                _pos.Add(dat.Position.ToPSS());
                _normal.Add(dat.Normal.ToPSS());
                _texture.Add(dat.UV.ToPSS());
                _colour.Add(dat.Colour.ToVector4().ToPSS());
            }
            
            _pssBuffer.SetVertices(0, _pos.ToArray());
            _pssBuffer.SetVertices(1, _normal.ToArray());
            _pssBuffer.SetVertices(2, _texture.ToArray());
            _pssBuffer.SetVertices(3, _colour.ToArray());
        }
        
        public int VertexCount
        {
            get
            {
                return _pssBuffer.VertexCount;
            }
        }
    }

    public class Gouraud_PositionNormal_EffectPass
        : IEffectPass
    {
        Sce.Pss.Core.Graphics.ShaderProgram _program;
        Sce.Pss.Core.Graphics.GraphicsContext _gfxContext;
        
        public Gouraud_PositionNormal_EffectPass(Sce.Pss.Core.Graphics.GraphicsContext gfxContext, Sce.Pss.Core.Graphics.ShaderProgram program)
        {
            _gfxContext = gfxContext;
            _program = program;
        }
        
        public void Apply()
        {
            _gfxContext.SetShaderProgram( _program );
        }
    }
    
    public class Gouraud_PositionNormal
        : IShader
    {
        Sce.Pss.Core.Graphics.ShaderProgram _program;
        Sce.Pss.Core.Graphics.GraphicsContext _gfxContext;
            
        Gouraud_PositionNormal_EffectPass[] _pass;
        

        public Gouraud_PositionNormal(
            Sce.Pss.Core.Graphics.GraphicsContext gfxContext, 
            Sce.Pss.Core.Graphics.ShaderProgram program)
        {
            _gfxContext = gfxContext;
            _program = program;
            _program.SetUniformBinding(0, "WorldViewProj");     
            _program.SetAttributeBinding( 0, "a_Position" ) ;
            _program.SetAttributeBinding( 1, "a_Normal" ) ;
            
            _pass = new Gouraud_PositionNormal_EffectPass[] { new Gouraud_PositionNormal_EffectPass(_gfxContext, _program) }; 
        }

        public IEffectPass[] Passes
        {
            get
            {
                return _pass;
            }
        }

        public void Calibrate(Dictionary<string, ShaderSettingsData> settings)
        {
            Matrix world = Matrix.Identity;
            Matrix view = Matrix.Identity;
            Matrix proj = Matrix.Identity;
            Colour col = Colour.White;
            
            foreach (var param in settings.Keys)
            {
                if (param == "_world")
                {

                    world = ((Matrix) settings[param].Value);
                    continue;
                }

                if (param == "_view")
                {
                    view = ((Matrix)settings[param].Value);
                    continue;
                }

                if (param == "_proj")
                {
                    proj = ((Matrix)settings[param].Value);
                    continue;
                }

                if (param == "_colour")
                {
                    col = ((Colour)settings[param].Value);
                    continue;
                }
            }
            
            
            Matrix blimeyWorldViewProj = (world * view * proj);
            
            Sce.Pss.Core.Matrix4 sceWorldViewProj = blimeyWorldViewProj.ToPSS();
            
            _program.SetUniformValue( 0, ref sceWorldViewProj );
            
            Matrix invWorld = Matrix.Invert(world);
            
            
            Sce.Pss.Core.Vector3 lightPos = new Sce.Pss.Core.Vector3( 0.0f, 15.0f, 13.0f );
            // model local Light Direction
            Sce.Pss.Core.Matrix4 worldInverse = invWorld.ToPSS();
            Sce.Pss.Core.Vector4 localLightPos4 = 
                worldInverse * (new Sce.Pss.Core.Vector4( lightPos, 1.0f ));
            Sce.Pss.Core.Vector3 localLightPos = 
                new Sce.Pss.Core.Vector3( localLightPos4.X, localLightPos4.Y, localLightPos4.Z );

            _program.SetUniformValue( _program.FindUniform( "LocalLightPosition" ), ref localLightPos );
    
            
            Sce.Pss.Core.Vector3 camPos = proj.Translation.ToPSS();
            
            // model local eye
            Sce.Pss.Core.Vector4 localEye4 = worldInverse * (new Sce.Pss.Core.Vector4( camPos, 1.0f ));
            Sce.Pss.Core.Vector3 localEye = new Sce.Pss.Core.Vector3( localEye4.X, localEye4.Y, localEye4.Z);
    
            _program.SetUniformValue( _program.FindUniform( "EyePosition" ), ref localEye );
    

            // light
            Sce.Pss.Core.Vector4 i_a = new Sce.Pss.Core.Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
            _program.SetUniformValue( _program.FindUniform( "IAmbient" ), ref i_a );
            
            Sce.Pss.Core.Vector4 i_d = new Sce.Pss.Core.Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
            _program.SetUniformValue( _program.FindUniform( "IDiffuse" ), ref i_d );
    
            // material
            Sce.Pss.Core.Vector4 k_a = new Sce.Pss.Core.Vector4( 0.2f, 0.2f, 0.2f, 1.0f );
            _program.SetUniformValue( _program.FindUniform( "KAmbient" ), ref k_a );
    
            Sce.Pss.Core.Vector4 k_d    = col.ToVector4().ToPSS();
            _program.SetUniformValue( _program.FindUniform( "KDiffuse" ), ref k_d );
    
            Sce.Pss.Core.Vector4 k_s = new Sce.Pss.Core.Vector4( 1.0f, 1.0f, 1.0f, 1.0f );
            _program.SetUniformValue( _program.FindUniform( "KSpecular" ), ref k_s );
            _program.SetUniformValue( _program.FindUniform( "Shininess" ), 5.0f );
        }
    }

    public class VitaControllerImplementation
        : VitaController
    {
        internal void Update(GameTime time)
        {
            base.Reset();

            var gamePadData = Sce.Pss.Core.Input.GamePad.GetData(0);
            
            if (gamePadData.Buttons.HasFlag(Sce.Pss.Core.Input.GamePadButtons.Down))
                base.DPad.Down = ButtonState.Pressed;

            if (gamePadData.Buttons.HasFlag(Sce.Pss.Core.Input.GamePadButtons.Up))
                base.DPad.Up = ButtonState.Pressed;

            if (gamePadData.Buttons.HasFlag(Sce.Pss.Core.Input.GamePadButtons.Left))
                base.DPad.Left = ButtonState.Pressed;

            if (gamePadData.Buttons.HasFlag(Sce.Pss.Core.Input.GamePadButtons.Right))
                base.DPad.Right = ButtonState.Pressed;

            if (gamePadData.Buttons.HasFlag(Sce.Pss.Core.Input.GamePadButtons.Cross))
                base.Buttons.Cross = ButtonState.Pressed;

            if (gamePadData.Buttons.HasFlag(Sce.Pss.Core.Input.GamePadButtons.Square))
                base.Buttons.Square = ButtonState.Pressed;

            if (gamePadData.Buttons.HasFlag(Sce.Pss.Core.Input.GamePadButtons.Triangle))
                base.Buttons.Triangle = ButtonState.Pressed;

            if (gamePadData.Buttons.HasFlag(Sce.Pss.Core.Input.GamePadButtons.Circle))
                base.Buttons.Circle = ButtonState.Pressed;

            if (gamePadData.Buttons.HasFlag(Sce.Pss.Core.Input.GamePadButtons.Start))
                base.Buttons.Start = ButtonState.Pressed;

            if (gamePadData.Buttons.HasFlag(Sce.Pss.Core.Input.GamePadButtons.Select))
                base.Buttons.Select = ButtonState.Pressed;

            if (gamePadData.Buttons.HasFlag(Sce.Pss.Core.Input.GamePadButtons.Right))
                base.Buttons.RightShoulder = ButtonState.Pressed;

            if (gamePadData.Buttons.HasFlag(Sce.Pss.Core.Input.GamePadButtons.Left))
                base.Buttons.LeftShoulder = ButtonState.Pressed;
        }
    }

}
