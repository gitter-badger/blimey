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

namespace Blimey.Engine
{
    using System;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.IO;

    using Abacus.SinglePrecision;
    using Fudge;
    using Blimey.Platform;
    using Blimey.Asset;
    using Oats;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Abacus.SinglePrecision.Matrix44 type.
    /// </summary>
    public class Matrix44Serialiser
        : Serialiser <Matrix44>
    {
        /// <summary>
        /// Returns an Abacus.SinglePrecision.Matrix44 object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override Matrix44 Read (ISerialisationChannel ss)
        {
            Single m11 = ss.Read <Single> ();
            Single m12 = ss.Read <Single> ();
            Single m13 = ss.Read <Single> ();
            Single m14 = ss.Read <Single> ();

            Single m21 = ss.Read <Single> ();
            Single m22 = ss.Read <Single> ();
            Single m23 = ss.Read <Single> ();
            Single m24 = ss.Read <Single> ();

            Single m31 = ss.Read <Single> ();
            Single m32 = ss.Read <Single> ();
            Single m33 = ss.Read <Single> ();
            Single m34 = ss.Read <Single> ();

            Single m41 = ss.Read <Single> ();
            Single m42 = ss.Read <Single> ();
            Single m43 = ss.Read <Single> ();
            Single m44 = ss.Read <Single> ();

            return new Matrix44(
                m11, m12, m13, m14,
                m21, m22, m23, m24,
                m31, m32, m33, m34,
                m41, m42, m43, m44
            );
        }

        /// <summary>
        /// Writes an Abacus.SinglePrecision.Matrix44 object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, Matrix44 obj)
        {
            ss.Write <Single> (obj.R0C0);
            ss.Write <Single> (obj.R0C1);
            ss.Write <Single> (obj.R0C2);
            ss.Write <Single> (obj.R0C3);

            ss.Write <Single> (obj.R1C0);
            ss.Write <Single> (obj.R1C1);
            ss.Write <Single> (obj.R1C2);
            ss.Write <Single> (obj.R1C3);

            ss.Write <Single> (obj.R2C0);
            ss.Write <Single> (obj.R2C1);
            ss.Write <Single> (obj.R2C2);
            ss.Write <Single> (obj.R2C3);

            ss.Write <Single> (obj.R3C0);
            ss.Write <Single> (obj.R3C1);
            ss.Write <Single> (obj.R3C2);
            ss.Write <Single> (obj.R3C3);
        }
    }
}
