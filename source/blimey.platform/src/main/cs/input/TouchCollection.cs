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
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.IO;

    using Abacus.SinglePrecision;
    using Fudge;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    ///
    /// </summary>
    public sealed class TouchCollection
        : IEnumerable<Touch>
    {
        /// <summary>
        ///
        /// </summary>
        List<Touch> touchBuffer = new List<Touch>();

        /// <summary>
        ///
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator ();
        }

        /// <summary>
        ///
        /// </summary>
        IEnumerator<Touch> IEnumerable<Touch>.GetEnumerator ()
        {
            return GetEnumerator ();
        }

        /// <summary>
        ///
        /// </summary>
        internal void ClearBuffer ()
        {
            this.touchBuffer.Clear ();
        }

        /// <summary>
        ///
        /// </summary>
        internal void RegisterTouch (
            String id,
            Vector2 normalisedEngineSpacePosition,
            TouchPhase phase,
            Int64 frameNum,
            Single timestamp)
        {
            Boolean die = false;

            if (normalisedEngineSpacePosition.X > 0.5f ||
                normalisedEngineSpacePosition.X < -0.5f)
            {
                InternalUtils.Log.Info (
                    "Touch has a bad X coordinate: " +
                    normalisedEngineSpacePosition.X);

                die = true;
            }

            if (normalisedEngineSpacePosition.Y > 0.5f ||
                normalisedEngineSpacePosition.X < -0.5f)
            {
                InternalUtils.Log.Info (
                    "Touch has a bad Y coordinate: " +
                    normalisedEngineSpacePosition.Y);

                die = true;
            }

            if (die)
            {
                InternalUtils.Log.Info ("Discarding Bad Touch");
                return;
            }

            var touch = new Touch (
                id,
                normalisedEngineSpacePosition,
                phase,
                frameNum,
                timestamp);

            this.touchBuffer.Add (touch);
        }

        /// <summary>
        ///
        /// </summary>
        public IEnumerator<Touch> GetEnumerator ()
        {
            return new TouchCollectionEnumerator (this.touchBuffer);
        }

        /// <summary>
        ///
        /// </summary>
        public int TouchCount
        {
            get
            {
                return touchBuffer.Count;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public Touch GetTouchFromTouchID (String zTouchID)
        {
            foreach (var touch in touchBuffer)
            {
                if (touch.ID == zTouchID)
                    return touch;
            }

            //System.Diagnostics.Debug.WriteLine (
            //    "The touch requested no longer exists.");

            return Touch.Invalid;
        }
    }
}
