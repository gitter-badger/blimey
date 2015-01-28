using System;

using Cor;
using Blimey.Assets.Pipeline;
using System.IO;
using System.Collections.Generic;
using Platform;

namespace Blimey.Assets.Builders
{
    public class MeshImporter
        : AssetImporter <MeshAsset>
    {
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

            var verts = new List<String> ();
            var normals = new List<String> ();
            var textureCoordinates = new List<String> ();
            var faces = new List<String> ();

            foreach (var line in objFile)
            {
                if (line.IndexOf ("v ") == 0)
                    verts.Add (line.Remove (2));
                else if (line.IndexOf ("vn ") == 0)
                    normals.Add (line.Remove (3));
                else if (line.IndexOf ("vt ") == 0)
                    textureCoordinates.Add (line.Remove (3));
                else if (line.IndexOf ("f ") == 0)
                    faces.Add (line.Remove (2));
            }

            var outputResource = new MeshAsset ();

            if (normals.Count > 0 && textureCoordinates.Count > 0)
            {
                outputResource.VertexDeclaration = VertexPositionNormalTexture.Default.VertexDeclaration;
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

