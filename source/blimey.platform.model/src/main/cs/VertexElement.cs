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

    [StructLayout (LayoutKind.Sequential)]
    public struct VertexElement
    {
        /// <summary>
        ///
        /// </summary>
        public Int32 _offset;

        /// <summary>
        ///
        /// </summary>
        public VertexElementFormat _format;

        /// <summary>
        ///
        /// </summary>
        public VertexElementUsage _usage;

        /// <summary>
        ///
        /// </summary>
        public Int32 _usageIndex;

        /// <summary>
        ///
        /// </summary>
        public Int32 Offset
        {
            get { return this._offset; }
            set { this._offset = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public VertexElementFormat VertexElementFormat
        {
            get { return this._format; }
            set { this._format = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public VertexElementUsage VertexElementUsage
        {
            get{ return this._usage; }
            set { this._usage = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public Int32 UsageIndex
        {
            get { return this._usageIndex; }
            set { this._usageIndex = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public VertexElement (
            Int32 offset,
            VertexElementFormat elementFormat,
            VertexElementUsage elementUsage,
            Int32 usageIndex)
        {
            this._offset = offset;
            this._usageIndex = usageIndex;
            this._format = elementFormat;
            this._usage = elementUsage;
        }

        /// <summary>
        ///
        /// </summary>
        public override String ToString ()
        {
            return String.Format (
                "[Offset:{0} Format:{1}, Usage:{2}, UsageIndex:{3}]",
                this.Offset,
                this.VertexElementFormat,
                this.VertexElementUsage,
                this.UsageIndex
            );
        }

        /// <summary>
        ///
        /// </summary>
        public override Int32 GetHashCode ()
        {
            return base.GetHashCode ();
        }

        /// <summary>
        ///
        /// </summary>
        public override Boolean Equals (Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType () != base.GetType ())
            {
                return false;
            }

            return (this == ((VertexElement)obj));
        }

        /// <summary>
        ///
        /// </summary>
        public static Boolean operator ==
            (VertexElement left, VertexElement right)
        {
            return
                (left._offset == right._offset) &&
                (left._usageIndex == right._usageIndex) &&
                (left._usage == right._usage) &&
                (left._format == right._format);
        }

        /// <summary>
        ///
        /// </summary>
        public static Boolean operator !=
            (VertexElement left, VertexElement right)
        {
            return !(left == right);
        }
    }
}
