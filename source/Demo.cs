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
using Sungiant.Abacus;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Abacus.Packed;
using Sungiant.Cor;
using System.Collections.Generic;

namespace Sungiant.Cor.Demo
{
	public static class Demo
	{
		public static IApp GetEntryPoint() { return basicApp; }
		public static AppSettings GetAppSettings() { return appSettings; }
		static IApp basicApp;
		static AppSettings appSettings;

		static Demo()
		{
			appSettings.FullScreen = true;
			appSettings.MouseGeneratesTouches = true;

			basicApp = new BasicApp();
		}
	}

	public class BasicApp
		: IApp
	{
		ICor engine;
		
		IShader unlitEffect;

		Rgba32 colour = Rgba32.DarkRed;
		Single changeColourTime = 1f;

		public void Initilise(ICor engine)
		{
			this.engine = engine;
			
			this.unlitEffect = engine.Resources.LoadShader(ShaderType.Unlit);

            this.LoadShape1();
            this.LoadShape2();
            this.LoadShape3();
		}

		public Boolean Update(AppTime time)
		{
			if(time.Elapsed > changeColourTime)
			{
				changeColourTime += 1f;
				colour = RandomColours.GetNext();
			}
            Vector3 a = Vector3.Backward;

            Single delta = Sungiant.Abacus.RealMaths.Sin(time.Elapsed);

            Matrix44.CreateFromAxisAngle(ref a, delta,  out this.rotation1);

            Matrix44.CreateFromYawPitchRoll( delta, delta, delta, out this.rotation2);

            Matrix44.CreateFromYawPitchRoll( delta, delta, delta, out this.rotation3);

			return false;
		}

		public void Render()
		{
			this.engine.Graphics.ClearColourBuffer(colour);
			this.engine.Graphics.ClearDepthBuffer(1f);

			if (this.shape1GeomBuffer == null)
				return;
						
            this.RenderShape1();
            this.RenderShape2();
            this.RenderShape3();

		}

		#region shape1

		IGeometryBuffer shape1GeomBuffer;
        Int32 shape1VertCount;
        Int32 shape1IndexCount;
        Matrix44 rotation1;

        void LoadShape1()
        {
            var vertBuffer = CustomShape_PositionColour.VertArray;
            
            var indexBuffer = CustomShape_PositionColour.IndexArray;
            
            
            this.shape1VertCount = vertBuffer.Length;
            this.shape1IndexCount = indexBuffer.Length;
            
            this.shape1GeomBuffer = engine.Graphics.CreateGeometryBuffer(
                VertexPositionColour.Default.VertexDeclaration, this.shape1VertCount, this.shape1IndexCount);
            
            if (this.shape1GeomBuffer != null)
            {
                this.shape1GeomBuffer.VertexBuffer.SetData(vertBuffer);
                this.shape1GeomBuffer.IndexBuffer.SetData(indexBuffer);
            }
            
            // don't need these now as they live on the GPU
            vertBuffer = null;
            indexBuffer = null;
        }

        void RenderShape1()
        {
            this.engine.Graphics.GpuUtils.BeginEvent(Rgba32.Red, "Render Shape 1");
            
            this.engine.Graphics.SetActiveGeometryBuffer(this.shape1GeomBuffer);

            Matrix44 worldScale;
            Matrix44.CreateScale(0.5f, out worldScale);
            
            Matrix44 shape1Translation;
            Matrix44.CreateTranslation(-0.5f, 0.5f, 0f, out shape1Translation);
            
            var world = worldScale * this.rotation1;
            world = world * shape1Translation;
            
            var a = Vector3.UnitZ; var b = Vector3.Forward; var c = Vector3.Up;
            Matrix44 view; Matrix44.CreateLookAt(
                ref a,
                ref b,
                ref c,
                out view);
            
            Matrix44 proj; Matrix44.CreateOrthographicOffCenter(-1f, 1f, -1f, 1f, 1f, -1f, out proj);
            
			// set the variable on the shader to our desired variables
			unlitEffect.ResetVariables ();
			unlitEffect.SetVariable ("World", world);
			unlitEffect.SetVariable ("View", view);
			unlitEffect.SetVariable ("Projection", proj);
			unlitEffect.SetVariable ("MaterialColour", Rgba32.White);

			foreach (var effectPass in this.unlitEffect.Passes)
            {
				effectPass.Activate();
                
                this.engine.Graphics.DrawIndexedPrimitives (
                    PrimitiveType.TriangleList, 0, 0,
                    this.shape1VertCount, 0, this.shape1IndexCount / 3);
            }
            
            this.engine.Graphics.GpuUtils.EndEvent();
        }

        #endregion


        #region shape2

        IGeometryBuffer shape2GeomBuffer;
        Int32 shape2VertCount;
        Int32 shape2IndexCount;
        Texture2D shape2Texture;
        Matrix44 rotation2;

        void LoadShape2()
        {
            var vertBuffer = CustomCube_PositionTexture.VertArray;
            
            var indexBuffer = CustomCube_PositionTexture.IndexArray;
            
            this.shape2VertCount = vertBuffer.Length;
            this.shape2IndexCount = indexBuffer.Length;
            
            this.shape2Texture = engine.Resources.Load<Texture2D>("Assets/cvan01.png");
            
            this.shape2GeomBuffer = engine.Graphics.CreateGeometryBuffer(
                CustomCube_PositionTexture.VertexDeclaration, 
                this.shape2VertCount, this.shape2IndexCount);
            
            if (this.shape2GeomBuffer != null)
            {
                this.shape2GeomBuffer.VertexBuffer.SetData(vertBuffer);
                this.shape2GeomBuffer.IndexBuffer.SetData(indexBuffer);
            }
            
            // don't need these now as they live on the GPU
            vertBuffer = null;
            indexBuffer = null;
        }

        void RenderShape2()
        {
            this.engine.Graphics.GpuUtils.BeginEvent(Rgba32.Red, "Render Shape 2");
            
            this.engine.Graphics.SetActiveGeometryBuffer(this.shape2GeomBuffer);
            
            Matrix44 worldScale;
            Matrix44.CreateScale(0.5f, out worldScale);
            
            Matrix44 shape2Translation;
            Matrix44.CreateTranslation(0.5f, 0.5f, 0f, out shape2Translation);
            
            
            var world = worldScale * this.rotation2;
            world = world * shape2Translation;
            
            var a = Vector3.UnitZ; var b = Vector3.Forward; var c = Vector3.Up;
            Matrix44 view; Matrix44.CreateLookAt(
                ref a,
                ref b,
                ref c,
                out view);
            
            Matrix44 proj; Matrix44.CreateOrthographicOffCenter(-1f, 1f, -1f, 1f, 1f, -1f, out proj);
            
			// set the variable on the shader to our desired variables
			unlitEffect.ResetVariables ();
			unlitEffect.SetVariable ("World", world);
			unlitEffect.SetVariable ("View", view);
			unlitEffect.SetVariable ("Projection", proj);
			unlitEffect.SetVariable ("MaterialColour", Rgba32.White);
			unlitEffect.SetVariable ("TextureSlot", 0);

			this.engine.Graphics.SetActiveTexture(0, this.shape2Texture);
			

            foreach (var effectPass in this.unlitEffect.Passes)
            {
                effectPass.Activate ();
                
                this.engine.Graphics.DrawIndexedPrimitives (
                    PrimitiveType.TriangleList, 0, 0,
                    this.shape2VertCount, 0, this.shape2IndexCount / 3);
            }
            
            this.engine.Graphics.GpuUtils.EndEvent();
        }

        #endregion


        #region shape3

        IGeometryBuffer shape3GeomBuffer;
        Int32 shape3VertCount;
        Int32 shape3IndexCount;
        Texture2D shape3Texture;
        Matrix44 rotation3;
        
        void LoadShape3()
        {
            var vertBuffer = CustomCylinder_PositionNormalTexture.VertArray;
            
            var indexBuffer = CustomCylinder_PositionNormalTexture.IndexArray;
            
            this.shape3VertCount = vertBuffer.Length;
            this.shape3IndexCount = indexBuffer.Length;
            
            
            this.shape3Texture = engine.Resources.Load<Texture2D>("Assets/bg1.png");
            
            this.shape3GeomBuffer = engine.Graphics.CreateGeometryBuffer(
                CustomCylinder_PositionNormalTexture.VertexDeclaration, 
                this.shape3VertCount, this.shape3IndexCount);
            
            if (this.shape3GeomBuffer != null)
            {
                this.shape3GeomBuffer.VertexBuffer.SetData(vertBuffer);
                this.shape3GeomBuffer.IndexBuffer.SetData(indexBuffer);
            }
            
            // don't need these now as they live on the GPU
            vertBuffer = null;
            indexBuffer = null;
        }
        
        void RenderShape3()
        {
            this.engine.Graphics.GpuUtils.BeginEvent(Rgba32.Red, "Render Shape 3");
            
            this.engine.Graphics.SetActiveGeometryBuffer(this.shape3GeomBuffer);
            
            Matrix44 worldScale;
            Matrix44.CreateScale(0.5f, out worldScale);
            
            Matrix44 shape3Translation;
            Matrix44.CreateTranslation(-0.5f, -0.5f, 0f, out shape3Translation);
            
            
            var world = worldScale * this.rotation3;
            world = world * shape3Translation;
            
            var a = Vector3.UnitZ; var b = Vector3.Forward; var c = Vector3.Up;
            Matrix44 view; Matrix44.CreateLookAt(
                ref a,
                ref b,
                ref c,
                out view);
            
            Matrix44 proj; Matrix44.CreateOrthographicOffCenter(-1f, 1f, -1f, 1f, 1f, -1f, out proj);
            
			// set the variable on the shader to our desired variables
			unlitEffect.ResetVariables ();
			unlitEffect.SetVariable ("World", world);
			unlitEffect.SetVariable ("View", view);
			unlitEffect.SetVariable ("Projection", proj);
			unlitEffect.SetVariable ("MaterialColour", Rgba32.White);
			unlitEffect.SetVariable ("TextureSlot", 0);

			this.engine.Graphics.SetActiveTexture(0, this.shape3Texture);

            foreach (var effectPass in this.unlitEffect.Passes)
            {
                effectPass.Activate ();
                
                this.engine.Graphics.DrawIndexedPrimitives (
                    PrimitiveType.TriangleList, 0, 0,
                    this.shape3VertCount, 0, this.shape3IndexCount / 3);
            }
            
            this.engine.Graphics.GpuUtils.EndEvent();
        }

        #endregion
	}


    public static class CustomCube
    {
        static Vector3[] normals;
        static List<Int32> indexArray = new List<Int32>();
        static List<VertexPositionTexture> vertArray = 
            new List<VertexPositionTexture>();

        public static VertexDeclaration VertexDeclaration { get {
                return VertexPositionTexture.Default.VertexDeclaration; } }

        static CustomCube()
        {
            // A cube has six faces, each one pointing in a different direction.
            normals = new Vector3[]
            {
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1),
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, -1, 0),
            };

            // Create each face in turn.
            foreach (Vector3 normal in normals)
            {
                // Get two vectors perpendicular to the face normal and to each other.
                Vector3 side1 = new Vector3(normal.Y, normal.Z, normal.X);
                Vector3 side2;

                Vector3 n = normal;
                Vector3.Cross(ref n, ref side1, out side2);
                
                // Six indices (two triangles) per face.
                AddIndex(CurrentVertex + 0);
                AddIndex(CurrentVertex + 1);
                AddIndex(CurrentVertex + 2);

                AddIndex(CurrentVertex + 0);
                AddIndex(CurrentVertex + 2);
                AddIndex(CurrentVertex + 3);
                
                // Four vertices per face.
                AddVertex((normal - side1 - side2) / 2, normal, new Vector2(0f, 0f));
                AddVertex((normal - side1 + side2) / 2, normal, new Vector2(1f, 0f));
                AddVertex((normal + side1 + side2) / 2, normal, new Vector2(1f, 1f));
                AddVertex((normal + side1 - side2) / 2, normal, new Vector2(0f, 1f));
            }
        }

        static int CurrentVertex
        {
            get { return vertArray.Count; }
        }

        static void AddVertex(Vector3 position, Vector3 normal, Vector2 texCoord)
        {
            vertArray.Add(new VertexPositionTexture(position, /*normal,*/ texCoord));
        }

        static void AddIndex(int index)
        {
            if (index > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("index");
            
            indexArray.Add((ushort)index);
        }

        public static VertexPositionTexture[] VertArray
        {
            get
            {
                return vertArray.ToArray();
            }
        }
        
        public static Int32[] IndexArray
        {
            get
            {
                return indexArray.ToArray();
            }
        }
        
    }

    public static class CustomCube_PositionTexture
    {
        static Vector3[] normals;
        static List<Int32> indexArray = new List<Int32>();
        static List<VertexPositionTexture> vertArray = 
            new List<VertexPositionTexture>();

        public static VertexDeclaration VertexDeclaration { get {
                return VertexPositionTexture.Default.VertexDeclaration; } }

        static CustomCube_PositionTexture()
        {
            // A cube has six faces, each one pointing in a different direction.
            normals = new Vector3[]
            {
                new Vector3(0, 0, 1),
                new Vector3(0, 0, -1),
                new Vector3(1, 0, 0),
                new Vector3(-1, 0, 0),
                new Vector3(0, 1, 0),
                new Vector3(0, -1, 0),
            };

            // Create each face in turn.
            foreach (Vector3 normal in normals)
            {
                // Get two vectors perpendicular to the face normal and to each other.
                Vector3 side1 = new Vector3(normal.Y, normal.Z, normal.X);
                Vector3 side2;

                Vector3 n = normal;
                Vector3.Cross(ref n, ref side1, out side2);
                
                // Six indices (two triangles) per face.
                AddIndex(CurrentVertex + 0);
                AddIndex(CurrentVertex + 1);
                AddIndex(CurrentVertex + 2);

                AddIndex(CurrentVertex + 0);
                AddIndex(CurrentVertex + 2);
                AddIndex(CurrentVertex + 3);
                
                // Four vertices per face.
                AddVertex((normal - side1 - side2) / 2, normal, new Vector2(0f, 0f));
                AddVertex((normal - side1 + side2) / 2, normal, new Vector2(1f, 0f));
                AddVertex((normal + side1 + side2) / 2, normal, new Vector2(1f, 1f));
                AddVertex((normal + side1 - side2) / 2, normal, new Vector2(0f, 1f));
            }
        }

        static int CurrentVertex
        {
            get { return vertArray.Count; }
        }

        static void AddVertex(Vector3 position, Vector3 normal, Vector2 texCoord)
        {
            vertArray.Add(new VertexPositionTexture(position, /*normal,*/ texCoord));
        }

        static void AddIndex(int index)
        {
            if (index > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("index");
            
            indexArray.Add((ushort)index);
        }

        public static VertexPositionTexture[] VertArray
        {
            get
            {
                return vertArray.ToArray();
            }
        }
        
        public static Int32[] IndexArray
        {
            get
            {
                return indexArray.ToArray();
            }
        }
        
    }


    public class CustomCylinder_PositionNormalTexture
    {
        const int tessellation = 9; // must be greater than 2
        const float height = 0.5f;
        const float radius = 0.5f;

        static List<Int32> indexArray = new List<Int32>();
        static List<VertexPositionNormalTexture> vertArray = 
            new List<VertexPositionNormalTexture>();
        
        public static VertexDeclaration VertexDeclaration { get {
                return VertexPositionNormalTexture.Default.VertexDeclaration; } }

        static CustomCylinder_PositionNormalTexture()
        {
            // Create a ring of triangles around the outside of the cylinder.
            for (int i = 0; i <= tessellation; i++)
            {
                Vector3 normal = GetCircleVector(i, tessellation);
                
                Vector3 topPos = normal * radius + Vector3.Up * height;
                Vector3 botPos = normal * radius + Vector3.Down * height;
                
                float howFarRound = (float)i / (float)(tessellation);
                
                
                Vector2 topUV = new Vector2(howFarRound, 0f);
                Vector2 botUV = new Vector2(howFarRound, 1f);
                
                AddVertex(topPos, normal, topUV);
                AddVertex(botPos, normal, botUV);
            }
            
            for (int i = 0; i < tessellation; i++)
            {
                AddIndex(i * 2);
                AddIndex(i * 2 + 1);
                AddIndex((i * 2 + 2));
                
                AddIndex(i * 2 + 1);
                AddIndex(i * 2 + 3);
                AddIndex(i * 2 + 2);
            }
            
            
            // Create flat triangle fan caps to seal the top and bottom.
            CreateCap(tessellation, height, radius, Vector3.Up);
            CreateCap(tessellation, height, radius, Vector3.Down);
        }

        /// Helper method creates a triangle fan to close the ends of the cylinder.
        static void CreateCap(int tessellation, float height, float radius, Vector3 normal)
        {
            // Create cap indices.
            for (int i = 0; i < tessellation - 2; i++)
            {
                if (normal.Y > 0)
                {
                    AddIndex(CurrentVertex);
                    AddIndex(CurrentVertex + (i + 1) % tessellation);
                    AddIndex(CurrentVertex + (i + 2) % tessellation);
                }
                else
                {
                    AddIndex(CurrentVertex);
                    AddIndex(CurrentVertex + (i + 2) % tessellation);
                    AddIndex(CurrentVertex + (i + 1) % tessellation);
                }
            }
            
            // Create cap vertices.
            for (int i = 0; i < tessellation; i++)
            {
                Vector3 circleVec = GetCircleVector(i, tessellation);
                Vector3 position = circleVec * radius +
                    normal * height;
                
                AddVertex(position, normal, new Vector2((circleVec.X + 1f) / 2f, (circleVec.Z + 1f) / 2f));
            }
        }
        

        /// Helper method computes a point on a circle.
        static Vector3 GetCircleVector(int i, int tessellation)
        {
            Single tau; RealMaths.Tau(out tau);
            float angle = i * tau / tessellation;
            
            float dx = (float)Math.Cos(angle);
            float dz = (float)Math.Sin(angle);
            
            return new Vector3(dx, 0, dz);
        }

        static int CurrentVertex
        {
            get { return vertArray.Count; }
        }
        
        static void AddVertex(Vector3 position, Vector3 normal, Vector2 texCoord)
        {
            vertArray.Add(new VertexPositionNormalTexture(position, normal, texCoord));
        }
        
        static void AddIndex(int index)
        {
            if (index > ushort.MaxValue)
                throw new ArgumentOutOfRangeException("index");
            
            indexArray.Add((ushort)index);
        }
        
        public static VertexPositionNormalTexture[] VertArray
        {
            get
            {
                return vertArray.ToArray();
            }
        }
        
        public static Int32[] IndexArray
        {
            get
            {
                return indexArray.ToArray();
            }
        }

    }


    public static class CustomShape
    {

        public static VertexPositionColour[] VertArray
        {
            get
            {
                return new VertexPositionColour[]
                {
                    new VertexPositionColour( new Vector3(0.0f, 0.0f, 0.0f), RandomColours.GetNext() ),
                    // Top
                    new VertexPositionColour( new Vector3(-0.2f, 0.8f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(0.2f, 0.8f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(0.0f, 0.8f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(0.0f, 1.0f, 0.0f), RandomColours.GetNext() ),
                    // Bottom
                    new VertexPositionColour( new Vector3(-0.2f, -0.8f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(0.2f, -0.8f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(0.0f, -0.8f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(0.0f, -1.0f, 0.0f), RandomColours.GetNext() ),
                    // Left
                    new VertexPositionColour( new Vector3(-0.8f, -0.2f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(-0.8f, 0.2f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(-0.8f, 0.0f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(-1.0f, 0.0f, 0.0f), RandomColours.GetNext() ),
                    // Right
                    new VertexPositionColour( new Vector3(0.8f, -0.2f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(0.8f, 0.2f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(0.8f, 0.0f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(1.0f, 0.0f, 0.0f), RandomColours.GetNext() ),
                };
            }
        }
        
        public static Int32[] IndexArray
        {
            get
            {
                return new Int32[] {
                    // Top
                    0, 1, 3,
                    0, 3, 2,
                    3, 1, 4,
                    3, 4, 2,
                    // Bottom
                    0, 7, 5,
                    0, 6, 7,
                    7, 8, 5,
                    7, 6, 8,
                    // Left
                    0, 9, 11,
                    0, 11, 10,
                    11, 9, 12,
                    11, 12, 10,
                    // Right
                    0, 15, 13, 
                    0, 14, 15,
                    15, 16, 13,
                    15, 14, 16
                };
            }
        }

    }


    public static class CustomShape_PositionColour
    {

        public static VertexPositionColour[] VertArray
        {
            get
            {
                return new VertexPositionColour[]
                {
                    new VertexPositionColour( new Vector3(0.0f, 0.0f, 0.0f), RandomColours.GetNext() ),
                    // Top
                    new VertexPositionColour( new Vector3(-0.2f, 0.8f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(0.2f, 0.8f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(0.0f, 0.8f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(0.0f, 1.0f, 0.0f), RandomColours.GetNext() ),
                    // Bottom
                    new VertexPositionColour( new Vector3(-0.2f, -0.8f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(0.2f, -0.8f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(0.0f, -0.8f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(0.0f, -1.0f, 0.0f), RandomColours.GetNext() ),
                    // Left
                    new VertexPositionColour( new Vector3(-0.8f, -0.2f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(-0.8f, 0.2f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(-0.8f, 0.0f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(-1.0f, 0.0f, 0.0f), RandomColours.GetNext() ),
                    // Right
                    new VertexPositionColour( new Vector3(0.8f, -0.2f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(0.8f, 0.2f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(0.8f, 0.0f, 0.0f), RandomColours.GetNext() ),
                    new VertexPositionColour( new Vector3(1.0f, 0.0f, 0.0f), RandomColours.GetNext() ),
                };
            }
        }
        
        public static Int32[] IndexArray
        {
            get
            {
                return new Int32[] {
                    // Top
                    0, 1, 3,
                    0, 3, 2,
                    3, 1, 4,
                    3, 4, 2,
                    // Bottom
                    0, 7, 5,
                    0, 6, 7,
                    7, 8, 5,
                    7, 6, 8,
                    // Left
                    0, 9, 11,
                    0, 11, 10,
                    11, 9, 12,
                    11, 12, 10,
                    // Right
                    0, 15, 13, 
                    0, 14, 15,
                    15, 16, 13,
                    15, 14, 16
                };
            }
        }

    }



    public static class RandomColours
    {
        static Random random = new Random();
        
        public static Rgba32 GetNext()
        {
            Single min = 0.25f;
            Single max = 1f;
            
            Single r = (Single)random.NextDouble() * (max - min) + min;
            Single g = (Single)random.NextDouble() * (max - min) + min;
            Single b = (Single)random.NextDouble() * (max - min) + min;
            Single a = 1f;
            
            return new Rgba32(r, g, b, a);
        }

    }


}

