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
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;

    using Fudge;
    using Abacus.SinglePrecision;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class ShaderDeclaration
        : IEquatable <ShaderDeclaration>
    {
        /// <summary>
        /// Defines a global name for this shader
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Lists all of the supported inputs into this shader and
        /// defines whether or not they are optional to an implementation.
        /// </summary>
        public List<ShaderInputDeclaration> InputDeclarations { get; set; }

        /// <summary>
        /// Defines all of the variables supported by this shader.  Every
        /// variant must support all of the variables.
        /// </summary>
        public List<ShaderVariableDeclaration> VariableDeclarations { get; set; }


        /// <summary>
        /// ?
        /// </summary>
        public List<ShaderSamplerDeclaration> SamplerDeclarations { get; set; }

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;

            if (obj is ShaderDeclaration)
            {
                flag = this.Equals ((ShaderDeclaration) obj);
            }

            return flag;
        }

        public Boolean Equals (ShaderDeclaration other)
        {
            if (this.Name != other.Name)
                return false;

            if (this.InputDeclarations == null && other.InputDeclarations != null)
                return false;

            if (this.InputDeclarations != null && other.InputDeclarations == null)
                return false;

            for (Int32 i = 0; i < this.InputDeclarations.Count; ++i)
                if (this.InputDeclarations [i] != other.InputDeclarations [i])
                    return false;

            if (this.VariableDeclarations == null && other.VariableDeclarations != null)
                return false;

            if (this.VariableDeclarations != null && other.VariableDeclarations == null)
                return false;

            for (Int32 i = 0; i < this.VariableDeclarations.Count; ++i)
                if (this.VariableDeclarations [i] != other.VariableDeclarations [i])
                    return false;

            if (this.SamplerDeclarations == null && other.SamplerDeclarations != null)
                return false;

            if (this.SamplerDeclarations != null && other.SamplerDeclarations == null)
                return false;

            for (Int32 i = 0; i < this.SamplerDeclarations.Count; ++i)
                if (this.SamplerDeclarations [i] != other.SamplerDeclarations [i])
                    return false;

            return true;
        }

        public static Boolean operator == (ShaderDeclaration a, ShaderDeclaration b)
        {
            return Equals (a, b);
        }

        public static Boolean operator != (ShaderDeclaration a, ShaderDeclaration b)
        {
            return !Equals (a, b);
        }
    }
}