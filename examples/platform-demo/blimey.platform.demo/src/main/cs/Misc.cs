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

    public static class RandomColours
    {
        readonly static Random random = new Random();

        public static Rgba32 GetNext()
        {
            const Single min = 0.25f;
            const Single max = 1f;

            Single r = (Single)random.NextDouble() * (max - min) + min;
            Single g = (Single)random.NextDouble() * (max - min) + min;
            Single b = (Single)random.NextDouble() * (max - min) + min;
            Single a = 1f;

            return new Rgba32(r, g, b, a);
        }
    }
}
