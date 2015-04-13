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

using System;

using Blimey.Platform;
using Blimey.Asset;
using System.IO;
using System.Collections.Generic;
using Abacus.SinglePrecision;
using System.Text.RegularExpressions;
using System.Linq;

namespace Blimey.Engine
{
    public class MeshImporter
        : AssetImporter <MeshAsset>
    {
        struct Vert
        {
            public Int32 VertIndex;
            public Int32? NormIndex;
            public Int32? TexIndex;

            public static Vert Parse (String s, Int32 vCount, Int32 nCount, Int32 tCount)
            {
                var v = new Vert ();
                Int32 t = 0;
                String b = "";

                foreach (var c in (s + "E").ToCharArray ())
                {
                    int x;
                    if (Int32.TryParse (c.ToString (), out x))
                        b += c;
                    else
                    {
                        Int32 idx = Int32.Parse (b) - 1;
                        b = "";
                        if (t == 0) v.VertIndex = idx;
                        if (t == 1) v.TexIndex = idx;
                        if (t == 2) v.NormIndex = idx;
                        t++;
                    }
                }

                if (v.VertIndex >= vCount)
                    throw new Exception ();

                if (v.NormIndex.HasValue && v.NormIndex.Value >= nCount)
                    throw new Exception ();

                if (v.TexIndex.HasValue && v.TexIndex.Value >= tCount)
                    throw new Exception ();

                return v;
            }
        }

        struct Tri
        {
            public Vert A;
            public Vert B;
            public Vert C;
        }

        class Face
        {
            public List<Tri> Tris = new List<Tri> ();
            public static Face Parse (String s, Int32 vCount, Int32 nCount, Int32 tCount)
            {
                var f = new Face ();
                var verts = s.Split (' ').Select (x => x.Trim ()).ToArray ();;

                if (verts.Length == 3)
                {
                    var t = new Tri ();
                    t.A = Vert.Parse (verts [0], vCount, nCount, tCount);
                    t.B = Vert.Parse (verts [1], vCount, nCount, tCount);
                    t.C = Vert.Parse (verts [2], vCount, nCount, tCount);
                    f.Tris.Add (t);
                }
                else if (verts.Length == 4)
                {
                    var ta = new Tri ();
                    var tb = new Tri ();
                    ta.A = Vert.Parse (verts [0], vCount, nCount, tCount);
                    ta.B = Vert.Parse (verts [2], vCount, nCount, tCount);
                    ta.C = Vert.Parse (verts [3], vCount, nCount, tCount);
                    tb.A = Vert.Parse (verts [0], vCount, nCount, tCount);
                    tb.B = Vert.Parse (verts [1], vCount, nCount, tCount);
                    tb.C = Vert.Parse (verts [2], vCount, nCount, tCount);

                    f.Tris.Add (ta);
                    f.Tris.Add (tb);
                }
                else throw new NotSupportedException ();

                return f;
            }
        }

        public override String [] SupportedSourceFileExtensions
        {
            get { return new [] { "obj" }; }
        }

        public override AssetImporterOutput <MeshAsset> Import (
            AssetImporterInput input, String platformId)
        {
            if (input.Files.Count != 1)
                throw new Exception ("MeshImporter only supports one input file.");

            if (!File.Exists (input.Files[0]))
                throw new Exception ("MeshImporter cannot find input file.");

            String filename = input.Files[0];

            var objFile = File.ReadAllText (filename).Split ('\n');

            var verts = new List<Vector3> ();
            var normals = new List<Vector3> ();
            var textureCoordinates = new List<Vector2> ();
            var faces = new List<Face> ();

            Int32 lineNumber = 0;
            foreach (var line in objFile)
            {
                lineNumber++;

                try
                {
                    if (line.IndexOf ("v ") == 0)
                    {
                        var strs = line.Substring (2, line.Length - 2).Trim ().Split (' ').Select (x => x.Trim ()).ToArray ();
                        var val = new Vector3 (Single.Parse (strs [0]), Single.Parse (strs [1]), Single.Parse (strs [2]));
                        verts.Add (val);
                    }
                    else if (line.IndexOf ("vn ") == 0)
                    {
                        var strs = line.Substring (3, line.Length - 3).Trim ().Split (' ').Select (x => x.Trim ()).ToArray ();
                        var val = new Vector3 (Single.Parse (strs [0]), Single.Parse (strs [1]), Single.Parse (strs [2]));
                        normals.Add (val);
                    }
                    else if (line.IndexOf ("vt ") == 0)
                    {
                        var strs = line.Substring (3, line.Length - 3).Trim ().Split (' ').Select (x => x.Trim ()).ToArray ();
                        var val = new Vector2 (Single.Parse (strs [0]), 1f - Single.Parse (strs [1]));
                        textureCoordinates.Add (val);
                    }
                    else if (line.IndexOf ("f ") == 0)
                    {
                        var str = line.Substring (2, line.Length - 2).Trim ();
                        faces.Add (Face.Parse (str, verts.Count, normals.Count, textureCoordinates.Count));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine ("!! Problem processing line #" + lineNumber);
                    Console.WriteLine (line);
                }
            }


            var outputResource = new MeshAsset ();

            if (normals.Count > 0 && textureCoordinates.Count > 0)
            {
                outputResource.VertexDeclaration = VertexPositionNormalTexture.Default.VertexDeclaration;

                var vertDict = new Dictionary <VertexPositionNormalTexture, Int32> ();
                var vb = new List <IVertexType> ();
                var ib = new List <Int32> ();

                foreach (var face in faces)
                {
                    foreach (var tri in face.Tris)
                    {
                        var v0 = new VertexPositionNormalTexture ();
                        v0.Position = verts [tri.A.VertIndex];
                        v0.Normal = normals [tri.A.NormIndex.Value];
                        v0.UV = textureCoordinates [tri.A.TexIndex.Value];
                        if (!vertDict.ContainsKey (v0))
                        {
                            vertDict.Add (v0, vb.Count);
                            vb.Add (v0);
                        }

                        var v1 = new VertexPositionNormalTexture ();
                        v1.Position = verts [tri.B.VertIndex];
                        v1.Normal = normals [tri.B.NormIndex.Value];
                        v1.UV = textureCoordinates [tri.B.TexIndex.Value];
                        if (!vertDict.ContainsKey (v1))
                        {
                            vertDict.Add (v1, vb.Count);
                            vb.Add (v1);
                        }

                        var v2 = new VertexPositionNormalTexture ();
                        v2.Position = verts [tri.C.VertIndex];
                        v2.Normal = normals [tri.C.NormIndex.Value];
                        v2.UV = textureCoordinates [tri.C.TexIndex.Value];
                        if (!vertDict.ContainsKey (v2))
                        {
                            vertDict.Add (v2, vb.Count);
                            vb.Add (v2);
                        }
                    }
                }

                foreach (var face in faces)
                {
                    foreach (var tri in face.Tris)
                    {
                        var v0 = new VertexPositionNormalTexture ();
                        v0.Position = verts [tri.A.VertIndex];
                        v0.Normal = normals [tri.A.NormIndex.Value];
                        v0.UV = textureCoordinates [tri.A.TexIndex.Value];
                        ib.Add (vertDict [v0]);

                        var v1 = new VertexPositionNormalTexture ();
                        v1.Position = verts [tri.B.VertIndex];
                        v1.Normal = normals [tri.B.NormIndex.Value];
                        v1.UV = textureCoordinates [tri.B.TexIndex.Value];
                        ib.Add (vertDict [v1]);

                        var v2 = new VertexPositionNormalTexture ();
                        v2.Position = verts [tri.C.VertIndex];
                        v2.Normal = normals [tri.C.NormIndex.Value];
                        v2.UV = textureCoordinates [tri.C.TexIndex.Value];
                        ib.Add (vertDict [v2]);
                    }
                }

                outputResource.IndexData = ib.ToArray ();
                outputResource.VertexData = vb.ToArray ();
            }
            else if (normals.Count > 0)
            {
                outputResource.VertexDeclaration = VertexPositionNormal.Default.VertexDeclaration;
            }
            else if (textureCoordinates.Count > 0)
            {
                outputResource.VertexDeclaration = VertexPositionTexture.Default.VertexDeclaration;
            }
            else
            {
                outputResource.VertexDeclaration = VertexPosition.Default.VertexDeclaration;
            }


            var output = new AssetImporterOutput <MeshAsset> ();
            output.OutputAsset = outputResource;

            return output;
        }
    }
}

