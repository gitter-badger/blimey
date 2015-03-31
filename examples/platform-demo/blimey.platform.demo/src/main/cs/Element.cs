// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__  .__                                                     │ \\
// │ \______   \  | |__| _____   ____ ___.__.                               │ \\
// │  |    |  _/  | |  |/     \_/ __ <   |  |                               │ \\
// │  |    |   \  |_|  |  Y Y  \  ___/\___  |                               │ \\
// │  |______  /____/__|__|_|  /\___  > ____|                               │ \\
// │         \/              \/     \/\/                                    │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey Engine (http://www.blimey.io)         │ \\
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

namespace PlatformDemo
{
    using System;
    using System.Text;
    using System.IO;
    using Abacus.SinglePrecision;
    using Fudge;
    using Blimey;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public interface IElement
    {
        void Load (Engine engine);
        void Unload ();
        void Update(Engine engine, AppTime time);
        void Render (Engine engine, Matrix44 projection);

        Matrix44 World { get; }
        Matrix44 View { get; }
    }

    public sealed class Element <TMesh, TVertType> : IElement
        where TMesh
            : class
            , IMesh<TVertType>
            , new ()
        where TVertType
            : struct
            , IVertexType
    {
        readonly Shader shader;
        readonly Texture texture;

        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;

        public Rgba32 Colour { get; private set; }
        public Matrix44 World { get; private set; }
        public Matrix44 View { get; private set; }

        public Element (Shader shader, Texture texture)
        {
            this.shader = shader;
            this.texture = texture;
            this.Colour = Rgba32.LightGoldenrodYellow;
            View = Matrix44.CreateLookAt (Vector3.UnitZ, Vector3.Forward, Vector3.Up);
        }

        public void Load (Engine engine)
        {
            var meshResource = new TMesh ();

            // put the mesh resource onto the gpu
            vertexBuffer = engine.Graphics.CreateVertexBuffer (meshResource.VertexDeclaration, meshResource.VertArray.Length);
            indexBuffer = engine.Graphics.CreateIndexBuffer (meshResource.IndexArray.Length);

            vertexBuffer.SetData <TVertType>(meshResource.VertArray);
            indexBuffer.SetData(meshResource.IndexArray);

            // don't need a reference to the mesh now as it lives on the GPU
            meshResource = null;
        }

        public void Unload ()
        {
            indexBuffer.Dispose ();
            vertexBuffer.Dispose ();

            indexBuffer = null;
            vertexBuffer = null;
        }

        public void Update(Engine engine, AppTime time)
        {
            Matrix44 rotation =
                Matrix44.CreateFromAxisAngle(Vector3.Backward, Maths.Sin(time.Elapsed)) *
                Matrix44.CreateFromAxisAngle(Vector3.Left, Maths.Sin(time.Elapsed)) *
                Matrix44.CreateFromAxisAngle(Vector3.Down, Maths.Sin(time.Elapsed));

            Matrix44 worldScale = Matrix44.CreateScale (0.9f);

            World = worldScale * rotation;
        }

        public void Render (Engine engine, Matrix44 projection)
        {
            engine.Graphics.SetActive (vertexBuffer);
            engine.Graphics.SetActive (indexBuffer);
            engine.Graphics.SetActive (texture, 0);

            // set the variable on the shader to our desired variables
            shader.ResetSamplers ();
            shader.ResetVariables ();
            shader.SetVariable ("World", World);
            shader.SetVariable ("View", View);
            shader.SetVariable ("Projection", projection);
            shader.SetVariable ("Colour", Colour);
            shader.SetSamplerTarget ("TextureSampler", 0);

            engine.Graphics.SetActive (shader);

            engine.Graphics.DrawIndexedPrimitives (
                PrimitiveType.TriangleList, 0, 0, vertexBuffer.VertexCount, 0, indexBuffer.IndexCount / 3);
        }
    }
}
