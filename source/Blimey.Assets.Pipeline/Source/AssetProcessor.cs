using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using Cor;

namespace Blimey.Assets.Pipline
{
    public abstract class AssetProcessor
    {
        public abstract AssetProcessorOutput
		BaseProcess (AssetProcessorInput input, String platformId);
    }

    public abstract class AssetProcessor <TFrom, TTo>
        : AssetProcessor
    where TFrom
        : IAsset
    where TTo
        : IAsset
    {
        public override AssetProcessorOutput
		BaseProcess (AssetProcessorInput input, String platformId)
        {
			var result = Process (input as AssetProcessorInput <TFrom>, platformId);

			if (result.OutputAsset == null) {
				throw new Exception ("Asset processor produced a null asset.");
			}

			return result;
        }

        public abstract AssetProcessorOutput <TTo>
		Process (AssetProcessorInput <TFrom> input, String platformId);
    }
}
