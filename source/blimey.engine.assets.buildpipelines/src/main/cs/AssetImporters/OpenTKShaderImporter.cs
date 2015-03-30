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
using System.IO;
using ServiceStack;
using ServiceStack.Text;
using Oats;
using System.Collections.Generic;

using Blimey.Assets.Pipeline;
using System.Text;

namespace Blimey.Assets.Builders
{
	public class OpenTKShaderImporter
		: AssetImporter <ShaderAsset>
	{
        sealed class ShaderVariantDefinition
        {
            public String VariantName { get; set; }
            public String VertexShaderPath { get; set; }
            public String PixelShaderPath { get; set; }
        }

        sealed class ShaderDefinition
        {
            public List<ShaderVariantDefinition> VariantDefinitions { get; set; }
        }

		public override String [] SupportedSourceFileExtensions
		{
			get { return new [] { "fsh", "vsh" }; }
		}

		public override AssetImporterOutput <ShaderAsset> Import (
			AssetImporterInput input, String platformId)
		{
			if (input.Files.Count != 2)
				throw new Exception ("ShaderAsset requires two input files.");

			if (!File.Exists (input.Files[0]) || !File.Exists (input.Files[1]))
				throw new Exception ("ShaderAsset cannot find the input files.");

			String format = (String) input.AssetImporterSettings.Settings ["Format"];

            ShaderFormat? shaderFormat = null;

            if (format == "OpenGL 4") shaderFormat = ShaderFormat.GLSL;

            if (format == "OpenGL ES 2") shaderFormat = ShaderFormat.GLSL_ES;

            if (!shaderFormat.HasValue)
                throw new Exception ("Format " + format + " not supported.");

            // this is a hack
            Abacus.SinglePrecision.Matrix44 m ;
            m = Abacus.SinglePrecision.Matrix44.Identity;
            // hack complete

			String commonShaderDecl = File.ReadAllText (input.Files [0]);
            String platformShaderDef = File.ReadAllText (input.Files [1]);

			// Import the platform agnostic shader definition.
            ShaderDeclaration shaderDeclaration = commonShaderDecl.FromXml <ShaderDeclaration> ();

			// Import the KR Shader definition.
			var platformDefinition = platformShaderDef.FromJson<ShaderDefinition> ();


			// BEGIN PLATFORM SPECIFIC RUNTIME DATA FORMAT ------------------------ //

            // Jagged array
            Byte[][] sources = new Byte[platformDefinition.VariantDefinitions.Count][];

            string path = Path.GetFullPath (input.Files [1]);
            path = Path.GetDirectoryName (path);

            for (Int32 i = 0; i < platformDefinition.VariantDefinitions.Count; ++i)
            {
                var vdef = platformDefinition.VariantDefinitions [i];

                string vpath = Path.Combine (path, vdef.VertexShaderPath);
                string ppath = Path.Combine (path, vdef.PixelShaderPath);

                if (!File.Exists (vpath))
                    throw new Exception ("Could not find: " + vpath);
                if (!File.Exists (ppath))
                    throw new Exception ("Could not find: " + ppath);

                String vertexShaderSource = File.ReadAllText (vpath);
                String pixelShaderSource = File.ReadAllText (ppath);

                String s =
                    vdef.VariantName + '\n' +
                    "=VSH=" + '\n' +
                    vertexShaderSource + '\n' +
                    "=FSH=" + '\n' +
                    pixelShaderSource + '\n';

                sources [i] = Encoding.UTF8.GetBytes (s);
            }

            using (var mem = new MemoryStream ())
            {
                using (var binW = new BinaryWriter (mem))
                {
                    binW.Write ((Byte)sources.Length);
                    foreach (var variant in sources)
                    {
                        binW.Write (variant.Length);
                        binW.Write (variant);
                    }
                }

                Byte[] platformSource = mem.GetBuffer ();

    			// END PLATFORM SPECIFIC RUNTIME DATA FORMAT -------------------------- //

    			// Make our in memory result
    			var result = new ShaderAsset {
                    Declaration = shaderDeclaration,
                    Format = shaderFormat.Value,
                    Source = platformSource
    			};

    			return new AssetImporterOutput <ShaderAsset> {
    				OutputAsset = result
    			};

            }
		}
	}
}

