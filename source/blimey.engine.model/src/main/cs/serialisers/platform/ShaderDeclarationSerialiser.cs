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
    /// An explict Oats.Serialiser for the Cor.ShaderDefinition type.
    /// </summary>
    public class ShaderDeclarationSerialiser
        : Serialiser<ShaderDeclaration>
    {
        /// <summary>
        /// Returns a Cor.ShaderDefinition object read from an Oats.ISerialisationChannel.
        /// </summary>
        public override ShaderDeclaration Read (ISerialisationChannel ss)
        {
            var sd = new ShaderDeclaration ();

            sd.Name =                   ss.Read <String> ();
            sd.InputDeclarations =      new List <ShaderInputDeclaration> ();
            sd.SamplerDeclarations =    new List <ShaderSamplerDeclaration> ();
            sd.VariableDeclarations =   new List <ShaderVariableDeclaration> ();

            Int32 numInputDefintions = (Int32) ss.Read <Byte> ();
            Int32 numSamplerDefinitions = (Int32) ss.Read <Byte> ();
            Int32 numVariableDefinitions = (Int32) ss.Read <Byte> ();

            for (Int32 i = 0; i < numInputDefintions; ++i)
            {
                var inputDef = ss.Read <ShaderInputDeclaration> ();
                sd.InputDeclarations.Add (inputDef);
            }

            for (Int32 i = 0; i < numSamplerDefinitions; ++i)
            {
                var samplerDef = ss.Read <ShaderSamplerDeclaration> ();
                sd.SamplerDeclarations.Add (samplerDef);
            }

            for (Int32 i = 0; i < numVariableDefinitions; ++i)
            {
                var variableDef = ss.Read <ShaderVariableDeclaration> ();
                sd.VariableDeclarations.Add (variableDef);
            }

            return sd;
        }

        /// <summary>
        /// Writes a Cor.ShaderDefinition object to an Oats.ISerialisationChannel.
        /// </summary>
        public override void Write (ISerialisationChannel ss, ShaderDeclaration sd)
        {
            if (sd.InputDeclarations.Count > Byte.MaxValue ||
                sd.SamplerDeclarations.Count > Byte.MaxValue ||
                sd.VariableDeclarations.Count > Byte.MaxValue)
            {
                throw new SerialisationException ("Too much!");
            }

            ss.Write <String> (sd.Name);

            ss.Write <Byte> ((Byte) sd.InputDeclarations.Count);
            ss.Write <Byte> ((Byte) sd.SamplerDeclarations.Count);
            ss.Write <Byte> ((Byte) sd.VariableDeclarations.Count);

            foreach (var inputDef in sd.InputDeclarations)
            {
                ss.Write <ShaderInputDeclaration> (inputDef);
            }

            foreach (var samplerDef in sd.SamplerDeclarations)
            {
                ss.Write <ShaderSamplerDeclaration> (samplerDef);
            }

            foreach (var variableDef in sd.VariableDeclarations)
            {
                ss.Write <ShaderVariableDeclaration> (variableDef);
            }
        }
    }
}
