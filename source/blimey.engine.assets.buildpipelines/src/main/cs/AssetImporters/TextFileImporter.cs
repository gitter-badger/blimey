using System;
using System.Runtime.InteropServices;
using Fudge;
using System.IO;
using Blimey.Assets.Pipeline;

namespace Blimey.Assets.Builders
{
    public class TextFileImporter
        : AssetImporter <TextAsset>
    {
        public override String [] SupportedSourceFileExtensions
        {
            get { return new [] { "dat", "txt" }; }
        }

		public override AssetImporterOutput <TextAsset> Import (
			AssetImporterInput input, String platformId)
        {
            var output = new AssetImporterOutput <TextAsset> ();
            var outputAsset = new TextAsset ();
            output.OutputAsset = outputAsset;

            if (input.Files.Count != 1)
                throw new Exception ("TextAssetImporter only supports one input file.");

            if (!File.Exists (input.Files[0]))
                throw new Exception ("TextAssetImporter cannot find input file.");

            outputAsset.Text = File.ReadAllText (input.Files[0]);
          
            return output;
        }

    }
}
