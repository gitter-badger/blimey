using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace Cor
{
    public class BitmapResource
        : IResource
    {
        /// <summary>
        /// A 32 BPP ARGB bitmap.
        /// </summary>
        public Bitmap Bitmap { get; set; }
    }
}