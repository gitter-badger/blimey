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

namespace Blimey
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Fudge;
    using Abacus.SinglePrecision;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// Provides Blimey consumers a means to configure the GPU's blending equation.
    /// </summary>
    public struct BlendMode
        : IEquatable<BlendMode>
    {
        BlendFunction rgbBlendFunction;
        BlendFactor sourceRgb;
        BlendFactor destinationRgb;

        BlendFunction alphaBlendFunction;
        BlendFactor sourceAlpha;
        BlendFactor destinationAlpha;

        internal BlendFunction RgbBlendFunction { get { return rgbBlendFunction;} }
        internal BlendFactor SourceRgb { get { return sourceRgb;} }
        internal BlendFactor DestinationRgb { get { return destinationRgb;} }

        internal BlendFunction AlphaBlendFunction { get { return alphaBlendFunction;} }
        internal BlendFactor SourceAlpha { get { return sourceAlpha;} }
        internal BlendFactor DestinationAlpha { get { return destinationAlpha;} }

        public override String ToString ()
        {
            return string.Format (
                "{{rgbBlendFunction:{0} sourceRgb:{1} destinationRgb:{2}" +
        " alphaBlendFunction:{3} sourceAlpha:{4} destinationAlpha:{5}}}",
        rgbBlendFunction.ToString (), sourceRgb.ToString (), destinationRgb.ToString (),
                alphaBlendFunction.ToString (), sourceAlpha.ToString (), destinationAlpha.ToString ()
            );
        }

        public Boolean Equals (BlendMode other)
        {
            return this == other;
        }

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;
            if (obj is BlendMode) flag = this.Equals ((BlendMode)obj);
            return flag;
        }

        public override Int32 GetHashCode ()
        {
            int a = rgbBlendFunction.GetHashCode();
            int b = sourceRgb.GetHashCode();
            int c = destinationRgb.GetHashCode();

            int d = alphaBlendFunction.GetHashCode();
            int e = sourceAlpha.GetHashCode();
            int f = destinationAlpha.GetHashCode();

      return a
                ^ b.ShiftAndWrap(2)
                ^ c.ShiftAndWrap(4)
                ^ d.ShiftAndWrap(6)
                ^ e.ShiftAndWrap(8)
                ^ f.ShiftAndWrap(10);
        }

        public static Boolean operator != (BlendMode value1, BlendMode value2)
        {
            return !(value1 == value2);
        }

        public static Boolean operator == (BlendMode value1, BlendMode value2)
        {
            if (value1.rgbBlendFunction != value2.rgbBlendFunction) return false;
            if (value1.sourceRgb != value2.sourceRgb) return false;
            if (value1.destinationRgb != value2.destinationRgb) return false;
            if (value1.alphaBlendFunction != value2.alphaBlendFunction) return false;
            if (value1.sourceAlpha != value2.sourceAlpha) return false;
            if (value1.destinationAlpha != value2.destinationAlpha) return false;

            return true;
        }

        public static BlendMode Default
        {
            get
            {
                var blendMode = new BlendMode();

                blendMode.rgbBlendFunction =    BlendFunction.Add;
                blendMode.sourceRgb =           BlendFactor.SourceAlpha;
                blendMode.destinationRgb =      BlendFactor.InverseSourceAlpha;

                blendMode.alphaBlendFunction =  BlendFunction.Add;
                blendMode.sourceAlpha =         BlendFactor.One;
                blendMode.destinationAlpha =    BlendFactor.InverseSourceAlpha;

                return blendMode;
            }
        }

        public static BlendMode Opaque
        {
            get
            {
                var blendMode = new BlendMode();

                blendMode.rgbBlendFunction =    BlendFunction.Add;
                blendMode.sourceRgb =           BlendFactor.One;
                blendMode.destinationRgb =      BlendFactor.Zero;

                blendMode.alphaBlendFunction =  BlendFunction.Add;
                blendMode.sourceAlpha =         BlendFactor.One;
                blendMode.destinationAlpha =    BlendFactor.Zero;

                return blendMode;
            }
        }

        public static BlendMode Subtract
        {
            get
            {
                var blendMode = new BlendMode();

                blendMode.rgbBlendFunction =    BlendFunction.ReverseSubtract;
                blendMode.sourceRgb =           BlendFactor.SourceAlpha;
                blendMode.destinationRgb =      BlendFactor.One;

                blendMode.alphaBlendFunction =  BlendFunction.ReverseSubtract;
                blendMode.sourceAlpha =         BlendFactor.SourceAlpha;
                blendMode.destinationAlpha =    BlendFactor.One;

                return blendMode;
            }
        }

        public static BlendMode Additive
        {
            get
            {
                var blendMode = new BlendMode();

                blendMode.rgbBlendFunction =    BlendFunction.Add;
                blendMode.sourceRgb =           BlendFactor.SourceAlpha;
                blendMode.destinationRgb =      BlendFactor.One;

                blendMode.alphaBlendFunction =  BlendFunction.Add;
                blendMode.sourceAlpha =         BlendFactor.SourceAlpha;
                blendMode.destinationAlpha =    BlendFactor.One;

                return blendMode;
            }
        }
    }
}
