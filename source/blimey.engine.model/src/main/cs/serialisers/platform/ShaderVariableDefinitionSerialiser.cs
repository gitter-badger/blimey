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
    /// An explict Oats.Serialiser for the Cor.ShaderVariableDefinition type.
    /// </summary>
    public class ShaderVariableDefinitionSerialiser
        : Serialiser<ShaderVariableDeclaration>
    {
        /// <summary>
        /// Returns a Cor.ShaderVariableDefinition object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ShaderVariableDeclaration Read (ISerialisationChannel ss)
        {
            var svd = new ShaderVariableDeclaration ();

            // Name
            svd.Name = ss.Read <String> ();

            // Nice Name
            svd.NiceName = ss.Read <String> ();

            // Null
            if (ss.Read <Boolean> ())
            {
                Byte typeIndex = ss.Read <Byte> ();
                Type defaultValueType = ShaderVariableDeclaration.SupportedTypes [typeIndex];
                svd.DefaultValue = ss.ReadReflective (defaultValueType);
            }

            return svd;
        }

        /// <summary>
        /// Writes a Cor.ShaderVariableDefinition object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ShaderVariableDeclaration svd)
        {
            // Name
            ss.Write <String> (svd.Name);

            // Nice Name
            ss.Write <String> (svd.NiceName);

            // Null
            ss.Write <Boolean> (svd.DefaultValue != null);

            // Default Value
            if (svd.DefaultValue != null)
            {
                Type defaultValueType = svd.DefaultValue.GetType ();
                Byte typeIndex = (Byte)
                    ShaderVariableDeclaration.SupportedTypes
                    .ToList ()
                    .IndexOf (defaultValueType);

                ss.Write<Byte> (typeIndex);
                ss.WriteReflective (defaultValueType, svd.DefaultValue);
            }
        }
    }
}
