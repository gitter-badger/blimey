using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;

namespace Cor
{
    public abstract class AssetProcessor
    {
        public abstract AssetProcessorOutput
        BaseProcess (AssetProcessorInput input);
    }

    public abstract class AssetProcessor <TFrom, TTo>
        : AssetProcessor
    where TFrom
        : IAsset
    where TTo
        : IAsset
    {
        public override AssetProcessorOutput
        BaseProcess (AssetProcessorInput input)
        {
            return Process (input as AssetProcessorInput <TFrom>);
        }

        public abstract AssetProcessorOutput <TTo>
        Process (AssetProcessorInput <TFrom> input);
    }
}
