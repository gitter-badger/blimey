namespace Cor.Demo
{
    using System;
    using System.Text;
    using System.IO;
    using Abacus.SinglePrecision;
    using Fudge;
    using Cor;
    using System.Collections.Generic;
    using Platform;
    using System.Runtime.InteropServices;

    [StructLayout (LayoutKind.Sequential)]
    public struct VertPos : IVertexType
    {
        readonly static VertexDeclaration _vertexDeclaration;

        static VertPos ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0));
        }

        public Vector3 Position;

        public VertPos (Vector3 position)
        {
            this.Position = position;
        }

        public VertexDeclaration VertexDeclaration { get { return _vertexDeclaration; } }
    }

    [StructLayout (LayoutKind.Sequential)]
    public struct VertPosTexCol : IVertexType
    {
        readonly static VertexDeclaration _vertexDeclaration;

        static VertPosTexCol ()
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
                    0));
        }

        public Vector3 Position;
        public Vector2 UV;
        public Rgba32 Colour;

        public VertPosTexCol (Vector3 position, Vector2 uv, Rgba32 colour)
        {
            this.Position = position;
            this.UV = uv;
            this.Colour = colour;
        }

        public VertexDeclaration VertexDeclaration { get { return _vertexDeclaration; } }
    }

    [StructLayout (LayoutKind.Sequential)]
    public struct VertPosTex : IVertexType
    {
        readonly static VertexDeclaration _vertexDeclaration;

        static VertPosTex ()
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
                    0));
        }

        public Vector3 Position;
        public Vector2 UV;

        public VertPosTex (Vector3 position, Vector2 uv)
        {
            this.Position = position;
            this.UV = uv;
        }

        public VertexDeclaration VertexDeclaration { get { return _vertexDeclaration; } }
    }

    [StructLayout (LayoutKind.Sequential)]
    public struct VertPosNormTex : IVertexType
    {
        readonly static VertexDeclaration _vertexDeclaration;

        static VertPosNormTex ()
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
                    0));
        }

        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 UV;

        public VertPosNormTex (Vector3 position, Vector3 normal, Vector2 uv)
        {
            this.Position = position;
            this.Normal = normal;
            this.UV = uv;
        }

        public VertexDeclaration VertexDeclaration { get { return _vertexDeclaration; } }
    }

    [StructLayout (LayoutKind.Sequential)]
    public struct VertNormTexPos : IVertexType
    {
        readonly static VertexDeclaration _vertexDeclaration;

        static VertNormTexPos ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Normal,
                    0),
                new VertexElement (
                    12,
                    VertexElementFormat.Vector2,
                    VertexElementUsage.TextureCoordinate,
                    0),
                new VertexElement (
                    20,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0));
        }

        public Vector3 Normal;
        public Vector2 UV;
        public Vector3 Position;

        public VertNormTexPos (Vector3 normal, Vector2 uv, Vector3 position)
        {
            this.Normal = normal;
            this.UV = uv;
            this.Position = position;
        }

        public VertexDeclaration VertexDeclaration { get { return _vertexDeclaration; } }
    }

    [StructLayout (LayoutKind.Sequential)]
    public struct VertPosCol : IVertexType
    {
        readonly static VertexDeclaration _vertexDeclaration;

        static VertPosCol ()
        {
            _vertexDeclaration = new VertexDeclaration (
                new VertexElement (
                    0,
                    VertexElementFormat.Vector3,
                    VertexElementUsage.Position,
                    0),
                new VertexElement (
                    12,
                    VertexElementFormat.Colour,
                    VertexElementUsage.Colour,
                    0)
            );
        }

        public Vector3 Position;
        public Rgba32 Colour;

        public VertPosCol (Vector3 position, Rgba32 colour)
        {
            this.Position = position;
            this.Colour = colour;
        }

        public VertexDeclaration VertexDeclaration
        {
            get
            {
                return _vertexDeclaration;
            }
        }
    }
}
