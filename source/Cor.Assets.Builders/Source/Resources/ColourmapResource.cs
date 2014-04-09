using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using Abacus.Packed;

namespace Cor
{
    public class ColourmapResource
        : IResource
    {
        /// <summary>
        /// A 32 BPP RGBA pixmap.
        /// </summary>
        public Rgba32[,] Data { get; set; }
    }
}