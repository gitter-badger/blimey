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
/*
using System;
using Abacus.SinglePrecision;
using Fudge;
using Cor;
using Cor.Platform;

namespace Cor.Demo
{
    public interface IElement
    {
        void Load (Engine engine);
        void Unload ();

        void Update(Engine engine, AppTime time);
        void Render (Engine engine);
    }



    public class BasicApp
        : IApp
    {
        Shader unlitEffect;
		Shader vertexLitEffect;
		Shader pixelLitEffect;
		Texture checkerTexture;
		Texture vanTexture;

        Rgba32 colour = Rgba32.DarkRed;
        Single changeColourTime = 1f;

        public void Start (Engine engine)
        {
            var unlitAsset = engine.Assets.Load<ShaderAsset> ("unlit.cba");
			var vertexLitAsset = engine.Assets.Load<ShaderAsset> ("vertex_lit.cba");
			var pixelLitAsset = engine.Assets.Load<ShaderAsset> ("pixel_lit.cba");
			this.unlitEffect = engine.Graphics.CreateShader (unlitAsset.Definition, unlitAsset.Data);
			this.vertexLitEffect = engine.Graphics.CreateShader (vertexLitAsset.Definition, vertexLitAsset.Data);
			this.pixelLitEffect = engine.Graphics.CreateShader (pixelLitAsset.Definition, pixelLitAsset.Data);

			var checkerTextureAsset = engine.Assets.Load<TextureAsset> ("bg1.cba");
			this.checkerTexture = engine.Graphics.CreateTexture (
                new TextureDefinition ()
                {
                    Height =  checkerTextureAsset.Height,
                    Width =  checkerTextureAsset.Width,
                    SurfaceFormat =  checkerTextureAsset.SurfaceFormat,
                },
                checkerTextureAsset.Data);

			var vanTextureAsset = engine.Assets.Load<TextureAsset> ("cvan01.cba");
			this.vanTexture = engine.Graphics.CreateTexture (
                new TextureDefinition ()
                {
                    Height =  vanTextureAsset.Height,
                    Width =  vanTextureAsset.Width,
                    SurfaceFormat =  vanTextureAsset.SurfaceFormat,
                },
                vanTextureAsset.Data);

			engine.Log.Info ("Start loading shapes.");
			this.LoadShape1(engine);
			//this.LoadShape2(engine);
			//this.LoadShape3(engine);
			//this.LoadShape4(engine);
			engine.Log.Info ("Finished loading");
        }

		public Boolean Update(Engine engine, AppTime time)
        {
            if(time.Elapsed > changeColourTime)
            {
                changeColourTime += 1f;
                colour = RandomColours.GetNext();
            }
            Vector3 a = Vector3.Backward;

            Single delta = Maths.Sin(time.Elapsed);

            Matrix44.CreateFromAxisAngle(ref a, ref delta,  out this.rotation1);

            //Matrix44.CreateFromYawPitchRoll( ref delta, ref delta, ref delta, out this.rotation2);

            //Matrix44.CreateFromYawPitchRoll( ref delta, ref delta, ref delta, out this.rotation3);

            return false;
        }

		public void Render(Engine engine)
        {
            engine.Graphics.ClearColourBuffer(colour);
            engine.Graphics.ClearDepthBuffer(1f);


			this.RenderShape1(engine);
			//this.RenderShape2(engine);
			//this.RenderShape3(engine);
			//this.RenderShape4(engine);

        }

		public void Stop(Engine engine)
		{
			engine.Graphics.DestroyShader (this.unlitEffect);
			engine.Graphics.DestroyShader (this.vertexLitEffect);
			engine.Graphics.DestroyShader (this.pixelLitEffect);
			engine.Graphics.DestroyTexture (this.checkerTexture);
			engine.Graphics.DestroyTexture (this.vanTexture);

            this.unlitEffect = null;
            this.vertexLitEffect = null;
            this.pixelLitEffect = null;
            this.checkerTexture = null;
            this.vanTexture = null;
		}

        #region shape1

        VertexBuffer shape1VertexBuffer;
        IndexBuffer shape1IndexBuffer;
        Int32 shape1VertCount;
        Int32 shape1IndexCount;
        Matrix44 rotation1;

		void LoadShape1(Engine engine)
        {
            var vertBuffer = CustomShape_PositionColour.VertArray;
            var indexBuffer = CustomShape_PositionColour.IndexArray;

            this.shape1VertCount = vertBuffer.Length;
            this.shape1IndexCount = indexBuffer.Length;

            this.shape1VertexBuffer = engine.Graphics.CreateVertexBuffer (
                VertexPositionColour.Default.VertexDeclaration, this.shape1VertCount);
            
            this.shape1IndexBuffer = engine.Graphics.CreateIndexBuffer (this.shape1IndexCount);

            this.shape1VertexBuffer.SetData(vertBuffer);
            this.shape1IndexBuffer.SetData(indexBuffer);
        
            // don't need these now as they live on the GPU
            vertBuffer = null;
            indexBuffer = null;
        }

        void RenderShape1 (Engine engine)
        {
            engine.Graphics.GpuUtils.BeginEvent (Rgba32.Red, "Render Shape 1");

            engine.Graphics.SetActiveVertexBuffer (this.shape1VertexBuffer);
            engine.Graphics.SetActiveIndexBuffer (this.shape1IndexBuffer);

            Matrix44 worldScale = Matrix44.CreateScale (0.5f);

            Matrix44 shape1Translation = Matrix44.CreateTranslation (-0.5f, 0.5f, 0f);

            var world = worldScale * this.rotation1;
            world = world * shape1Translation;

            var a = Vector3.UnitZ; var b = Vector3.Forward; var c = Vector3.Up;
            Matrix44 view; Matrix44.CreateLookAt(
                ref a,
                ref b,
                ref c,
                out view);

            Matrix44 proj = Matrix44.CreateOrthographicOffCenter (-1f, 1f, -1f, 1f, 1f, -1f);

            // set the variable on the shader to our desired variables
            unlitEffect.ResetVariables ();
            unlitEffect.SetVariable ("World", world);
            unlitEffect.SetVariable ("View", view);
            unlitEffect.SetVariable ("Projection", proj);
            unlitEffect.SetVariable ("MaterialColour", Rgba32.White);

            foreach (var effectPass in this.unlitEffect.Passes)
            {
                effectPass.Activate(VertexPositionColour.Default.VertexDeclaration);

                engine.Graphics.DrawIndexedPrimitives (
                    PrimitiveType.TriangleList, 0, 0,
                    this.shape1VertCount, 0, this.shape1IndexCount / 3, VertexPositionColour.Default.VertexDeclaration);
            }

            engine.Graphics.GpuUtils.EndEvent();
        }

        #endregion

        #region shape2

        VertexBuffer shape2VertexBuffer;
        IndexBuffer shape2IndexBuffer;
        Int32 shape2VertCount;
        Int32 shape2IndexCount;
        Matrix44 rotation2;

        void LoadShape2 (Engine engine)
        {
            var vertBuffer = CustomCube_PositionTexture.VertArray;
            var indexBuffer = CustomCube_PositionTexture.IndexArray;

            this.shape2VertCount = vertBuffer.Length;
            this.shape2IndexCount = indexBuffer.Length;

            this.shape2VertexBuffer = engine.Graphics.CreateVertexBuffer (
                VertexPositionColour.Default.VertexDeclaration, this.shape2VertCount);

            this.shape2IndexBuffer = engine.Graphics.CreateIndexBuffer (this.shape2IndexCount);

            this.shape2VertexBuffer.SetData(vertBuffer);
            this.shape2IndexBuffer.SetData(indexBuffer);

            // don't need these now as they live on the GPU
            vertBuffer = null;
            indexBuffer = null;
        }

        void RenderShape2(Engine engine)
        {
            engine.Graphics.GpuUtils.BeginEvent(Rgba32.Red, "Render Shape 2");

            engine.Graphics.SetActiveVertexBuffer (this.shape2VertexBuffer);
            engine.Graphics.SetActiveIndexBuffer (this.shape2IndexBuffer);

            Matrix44 worldScale = Matrix44.CreateScale(0.5f);

            Matrix44 shape2Translation =
                Matrix44.CreateTranslation(0.5f, 0.5f, 0f);


            var world = worldScale * this.rotation2;
            world = world * shape2Translation;

            var a = Vector3.UnitZ; var b = Vector3.Forward; var c = Vector3.Up;
            Matrix44 view; Matrix44.CreateLookAt(
                ref a,
                ref b,
                ref c,
                out view);

            Matrix44 proj = Matrix44.CreateOrthographicOffCenter(
                -1f, 1f, -1f, 1f, 1f, -1f);

            // set the variable on the shader to our desired variables
            unlitEffect.ResetVariables ();
            unlitEffect.SetVariable ("World", world);
            unlitEffect.SetVariable ("View", view);
            unlitEffect.SetVariable ("Projection", proj);
            unlitEffect.SetVariable ("MaterialColour", Rgba32.White);
            unlitEffect.SetSamplerTarget ("TextureSampler", this.vanTexture.Handle);

            engine.Graphics.SetActiveTexture(this.vanTexture, 0);


            foreach (var effectPass in this.unlitEffect.Passes)
            {
                effectPass.Activate (CustomCube_PositionTexture.VertexDeclaration);

                engine.Graphics.DrawIndexedPrimitives (
                    PrimitiveType.TriangleList, 0, 0,
                    this.shape2VertCount, 0, this.shape2IndexCount / 3);
            }

            engine.Graphics.GpuUtils.EndEvent();
        }

        #endregion

        #region shape3

        VertexBuffer shape3VertexBuffer;
        IndexBuffer shape3IndexBuffer;
        Int32 shape3VertCount;
        Int32 shape3IndexCount;
        Matrix44 rotation3;

        void LoadShape3 (Engine engine)
        {
            var vertBuffer = CustomCylinder_PositionNormalTexture.VertArray;
            var indexBuffer = CustomCylinder_PositionNormalTexture.IndexArray;

            this.shape3VertCount = vertBuffer.Length;
            this.shape3IndexCount = indexBuffer.Length;

            this.shape3VertexBuffer = engine.Graphics.CreateVertexBuffer (
                VertexPositionColour.Default.VertexDeclaration, this.shape3VertCount);

            this.shape3IndexBuffer = engine.Graphics.CreateIndexBuffer (this.shape3IndexCount);

            this.shape3VertexBuffer.SetData(vertBuffer);
            this.shape3IndexBuffer.SetData(indexBuffer);

            // don't need these now as they live on the GPU
            vertBuffer = null;
            indexBuffer = null;
        }

        void RenderShape3(Engine engine)
        {
            engine.Graphics.GpuUtils.BeginEvent(Rgba32.Red, "Render Shape 3");

            engine.Graphics.SetActiveVertexBuffer (this.shape3VertexBuffer);
            engine.Graphics.SetActiveIndexBuffer (this.shape3IndexBuffer);

            Matrix44 worldScale = Matrix44.CreateScale(0.5f);

            Matrix44 shape3Translation =
                Matrix44.CreateTranslation(-0.5f, -0.5f, 0f);


            var world = worldScale * this.rotation3;
            world = world * shape3Translation;

            var a = Vector3.UnitZ; var b = Vector3.Forward; var c = Vector3.Up;
            Matrix44 view; Matrix44.CreateLookAt(
                ref a,
                ref b,
                ref c,
                out view);

            Matrix44 proj = Matrix44.CreateOrthographicOffCenter(
                -1f, 1f, -1f, 1f, 1f, -1f);

            // set the variable on the shader to our desired variables
			pixelLitEffect.ResetVariables ();
			pixelLitEffect.SetVariable ("World", world);
			pixelLitEffect.SetVariable ("View", view);
			pixelLitEffect.SetVariable ("Projection", proj);
			pixelLitEffect.SetVariable ("MaterialColour", Rgba32.White);
			pixelLitEffect.SetSamplerTarget ("TextureSampler", 0);

            engine.Graphics.SetActiveTexture(0, this.checkerTexture);

			foreach (var effectPass in this.pixelLitEffect.Passes)
            {
                effectPass.Activate (CustomCylinder_PositionNormalTexture.VertexDeclaration);

                engine.Graphics.DrawIndexedPrimitives (
                    PrimitiveType.TriangleList, 0, 0,
                    this.shape3VertCount, 0, this.shape3IndexCount / 3);
            }

            engine.Graphics.GpuUtils.EndEvent();
        }

        #endregion

        #region shape4

        VertexBuffer shape4VertexBuffer;
        IndexBuffer shape4IndexBuffer;
        Int32 shape4VertCount;
        Int32 shape4IndexCount;

        void LoadShape4(Engine engine)
        {
            var vertBuffer = CustomBillboard_PositionTextureColour.VertArray;
            var indexBuffer = CustomBillboard_PositionTextureColour.IndexArray;

            this.shape4VertCount = vertBuffer.Length;
            this.shape4IndexCount = indexBuffer.Length;

            this.shape4VertexBuffer = engine.Graphics.CreateVertexBuffer (
                VertexPositionColour.Default.VertexDeclaration, this.shape4VertCount);

            this.shape4IndexBuffer = engine.Graphics.CreateIndexBuffer (this.shape4IndexCount);

            this.shape4VertexBuffer.SetData(vertBuffer);
            this.shape4IndexBuffer.SetData(indexBuffer);

            // don't need these now as they live on the GPU
            vertBuffer = null;
            indexBuffer = null;
        }

        void RenderShape4(Engine engine)
        {
            engine.Graphics.GpuUtils.BeginEvent(Rgba32.Red, "Render Shape 4");

            engine.Graphics.SetActiveVertexBuffer (this.shape4VertexBuffer);
            engine.Graphics.SetActiveIndexBuffer (this.shape4IndexBuffer);

            Matrix44 worldScale = Matrix44.CreateScale(0.5f);

            Matrix44 shape4Translation =
                Matrix44.CreateTranslation(0.5f, -0.5f, 0f);

            var world = worldScale;
            world = world * shape4Translation;

            var a = Vector3.UnitZ; var b = Vector3.Forward; var c = Vector3.Up;
            Matrix44 view; Matrix44.CreateLookAt(
                ref a,
                ref b,
                ref c,
                out view);


            Matrix44 proj =
                Matrix44.CreateOrthographicOffCenter(-1f, 1f, -1f, 1f, 1f, -1f);

            // set the variable on the shader to our desired variables
			unlitEffect.ResetVariables ();
			unlitEffect.SetVariable ("World", world);
			unlitEffect.SetVariable ("View", view);
			unlitEffect.SetVariable ("Projection", proj);
			unlitEffect.SetVariable ("MaterialColour", Rgba32.White);
			unlitEffect.SetSamplerTarget ("TextureSampler", 0);

			engine.Graphics.SetActiveTexture(0, this.vanTexture);
			engine.Graphics.SetActiveGeometryBuffer(this.shape4GeomBuffer);

			foreach (var effectPass in this.unlitEffect.Passes)
            {
				effectPass.Activate(VertexPositionTextureColour.Default.VertexDeclaration);

                engine.Graphics.DrawIndexedPrimitives (
                    PrimitiveType.TriangleList, 0, 0,
                    this.shape4VertCount, 0, this.shape4IndexCount / 3);
            }

            engine.Graphics.GpuUtils.EndEvent();
        }

        #endregion

    }
}

*/