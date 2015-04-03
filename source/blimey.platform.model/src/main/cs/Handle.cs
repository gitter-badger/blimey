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

    public abstract class Handle
        : IDisposable
        , IEquatable <Handle>
    {
        readonly String guid;
        Boolean alreadyDisposed;

        protected Handle ()
        {
            var g = Guid.NewGuid();
            guid = g.ToString ();
        }

        public override String ToString ()
        {
            return guid;
        }

        public String Identifier { get { return guid; } }

        protected virtual void CleanUpManagedResources () {}
        protected virtual void CleanUpNativeResources () {}

        public void Dispose ()
        {
            RunDispose (true);
            GC.SuppressFinalize (this);
        }

        public void RunDispose (bool isDisposing)
        {
            if (alreadyDisposed) return;
            if (isDisposing) CleanUpNativeResources ();

            // FREE UNMANAGED STUFF HERE
            CleanUpManagedResources ();

            alreadyDisposed = true;
        }

        public override Boolean Equals (Object obj)
        {
            Boolean flag = false;
            if (obj is Handle) flag = this.Equals ((Handle) obj);
            return flag;
        }

        public Boolean Equals (Handle other)
        {
            if (this.guid != other.guid)
                return false;

            return true;
        }

        public static Boolean operator == (Handle a, Handle b) { return Equals (a, b); }
        public static Boolean operator != (Handle a, Handle b) { return !Equals (a, b); }
    }
}