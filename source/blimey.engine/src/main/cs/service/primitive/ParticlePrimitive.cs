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
    using System.Collections.Generic;
    using System.Linq;
    using Abacus.SinglePrecision;
    using Fudge;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class ParticlePrimitive
    {
        public Vector2 vecLocation;
        public Vector2 vecVelocity;

        public float fGravity;
        public float fRadialAccel;
        public float fTangentialAccel;

        public float fSpin;
        public float fSpinDelta;

        public float fSize;
        public float fSizeDelta;

        public Rgba32 colColour;      // + alpha
        public Rgba32 colColourStart;
        public Rgba32 colColourEnd;

        public float fAge;
        public float fTerminalAge;

        public ParticlePrimitive(){}


        public ParticlePrimitive (ParticlePrimitive o)
        {
            this.vecLocation = o.vecLocation;
            this.vecVelocity = o.vecVelocity;

            this.fGravity = o.fGravity;
            this.fRadialAccel = o.fRadialAccel;
            this.fTangentialAccel = o.fTangentialAccel;
            this.fSpin = o.fSpin;
            this.fSpinDelta = o.fSpinDelta;

            this.fSize = o.fSize;
            this.fSizeDelta = o.fSizeDelta;

            this.colColour = o.colColour;     // + alpha
            this.colColourStart = o.colColourStart;
            this.colColourEnd = o.colColourEnd;

            this.fAge = o.fAge;
            this.fTerminalAge = o.fTerminalAge;
        }
    }
}