using System;

using Cor;
using Blimey.Assets.Pipeline;

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
            throw new NotImplementedException ();
        }
    }
}

