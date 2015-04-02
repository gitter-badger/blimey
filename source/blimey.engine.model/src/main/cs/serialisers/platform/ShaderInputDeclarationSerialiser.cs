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
    using System.IO;

    using Abacus.SinglePrecision;
    using Fudge;
    using Oats;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// An explict Oats.Serialiser for the Cor.ShaderInputDefinition type.
    /// </summary>
    public class ShaderInputDeclarationSerialiser
        : Serialiser<ShaderInputDeclaration>
    {
        /// <summary>
        /// Returns a Cor.ShaderInputDefinition object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ShaderInputDeclaration Read (ISerialisationChannel ss)
        {
            var sid = new ShaderInputDeclaration ();

            // Name
            sid.Name = ss.Read <String> ();

            // Nice Name
            sid.NiceName = ss.Read <String> ();

            // Optional
            sid.Optional = ss.Read <Boolean> ();

            // Usage
            sid.Usage = ss.Read <VertexElementUsage> ();

            // Null
            if (ss.Read <Boolean> ())
            {
                // Default Value
                Byte typeIndex = ss.Read <Byte> ();
                Type defaultValueType = ShaderInputDeclaration.SupportedTypes [typeIndex];
                sid.DefaultValue = ss.ReadReflective (defaultValueType);
            }

            return sid;
        }

        /// <summary>
        /// Writes a Cor.ShaderInputDefinition object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ShaderInputDeclaration sid)
        {
            // Name
            ss.Write <String> (sid.Name);

            // Nice Name
            ss.Write <String> (sid.NiceName);

            // Optional
            ss.Write <Boolean> (sid.Optional);

            // Usage
            ss.Write <VertexElementUsage> (sid.Usage);

            // Null
            ss.Write <Boolean> (sid.DefaultValue != null);

            // Default Value
            if (sid.DefaultValue != null)
            {
                Type defaultValueType = sid.DefaultValue.GetType ();
                Byte typeIndex = (Byte)
                    ShaderInputDeclaration.SupportedTypes
                    .ToList ()
                    .IndexOf (defaultValueType);

                ss.Write<Byte> (typeIndex);
                ss.WriteReflective (defaultValueType, sid.DefaultValue);
            }
        }
    }
}