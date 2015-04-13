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

namespace Blimey.Platform
{
    using System;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;

    using Fudge;
    using Abacus.SinglePrecision;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public sealed class ShaderVariableDeclaration
        : IEquatable <ShaderVariableDeclaration>
    {
        String niceName;
        Type defaultType;
        Object defaultValue;

        public ShaderVariableDeclaration ()
        {
            this.Name = String.Empty;
        }

        // Defines which Cor Types the DefaultValue can be set to.
        // The order of this list is important as the Cor Serialisation
        // of this class depends upon indexing into it.
        public static Type [] SupportedTypes
        {
            get
            {
                return new []
                {
                    typeof (Matrix44),
                    typeof (Int32),
                    typeof (Single),
                    typeof (Abacus.SinglePrecision.Vector2),
                    typeof (Abacus.SinglePrecision.Vector3),
                    typeof (Abacus.SinglePrecision.Vector4),
                    typeof (Rgba32)
                };
            }
        }

        public String NiceName
        {
            get { return (niceName == null) ? Name : niceName; }
            set { niceName = value; }
        }

        public String Name { get; set; }

        public Type Type
        {
            get { return defaultType; }
        }

        public Object DefaultValue
        {
            get { return defaultValue; }
            set
            {
                Type t = value.GetType ();
                if (!SupportedTypes.ToList ().Contains (t))
                {
                    throw new Exception ();
                }

                defaultType = t;
                defaultValue = value;
            }
        }

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;

            if (obj is ShaderVariableDeclaration)
            {
                flag = this.Equals ((ShaderVariableDeclaration) obj);
            }

            return flag;
        }

        public Boolean Equals (ShaderVariableDeclaration other)
        {
            if (this.niceName != other.niceName)
                return false;

            if (this.Name != other.Name)
                return false;

            if (this.defaultType != other.defaultType)
                return false;

            if (this.defaultValue.ToString () != other.defaultValue.ToString ())
                return false;

            return true;
        }

        public static Boolean operator == (ShaderVariableDeclaration a, ShaderVariableDeclaration b)
        {
            return Equals (a, b);
        }

        public static Boolean operator != (ShaderVariableDeclaration a, ShaderVariableDeclaration b)
        {
            return !Equals (a, b);
        }
    }
}
