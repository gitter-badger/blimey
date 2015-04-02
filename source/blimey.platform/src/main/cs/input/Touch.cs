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
    
    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    /// <summary>
    /// A touch in a single frame definition of a finger on the primary panel.
    /// </summary>
    public struct Touch
    {
        /// <summary>
        ///
        /// </summary>
        String id;

        /// <summary>
        /// The position of a touch ranges between -0.5 and 0.5 in both X and Y
        /// </summary>
        Vector2 normalisedEngineSpacePosition;

        /// <summary>
        ///
        /// </summary>
        TouchPhase phase;

        /// <summary>
        ///
        /// </summary>
        Int64 frameNumber;

        /// <summary>
        ///
        /// </summary>
        Single timestamp;

        /// <summary>
        ///
        /// </summary>
        static Touch invalidTouch;

        /// <summary>
        ///
        /// </summary>
        public String ID { get { return id; } }

        /// <summary>
        ///
        /// </summary>
        public Vector2 Position
        {
            get { return normalisedEngineSpacePosition; }
        }

        /// <summary>
        ///
        /// </summary>
        public TouchPhase Phase { get { return phase; } }

        /// <summary>
        ///
        /// </summary>
        public Int64 FrameNumber { get { return frameNumber; } }

        /// <summary>
        ///
        /// </summary>
        public Single Timestamp { get { return timestamp; } }

        /// <summary>
        ///
        /// </summary>
        public Touch (
            String id,
            Vector2 normalisedEngineSpacePosition,
            TouchPhase phase,
            Int64 frame,
            Single timestamp)
        {
            if (normalisedEngineSpacePosition.X > 0.5f ||
                normalisedEngineSpacePosition.X < -0.5f)
            {
                throw new Exception (
                    "Touch has a bad X coordinate: " +
                    normalisedEngineSpacePosition.X);
            }

            if (normalisedEngineSpacePosition.Y > 0.5f ||
                normalisedEngineSpacePosition.X < -0.5f)
            {
                throw new Exception (
                    "Touch has a bad Y coordinate: " +
                    normalisedEngineSpacePosition.Y);
            }

            this.id = id;
            this.normalisedEngineSpacePosition = normalisedEngineSpacePosition;
            this.phase = phase;
            this.frameNumber = frame;
            this.timestamp = timestamp;
        }

        /// <summary>
        ///
        /// </summary>
        static Touch ()
        {
            invalidTouch = new Touch (
                null,
                Vector2.Zero,
                TouchPhase.Invalid,
                -1,
                0f);
        }

        /// <summary>
        ///
        /// </summary>
        public static Touch Invalid { get { return invalidTouch; } }
    }
}