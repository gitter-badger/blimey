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

    public sealed class Quad
    {
        public VertexPositionTextureColour[] v;
        public Texture tex;
        public BlendMode blend = BlendMode.Default;

        public Quad()
        {
            v = new VertexPositionTextureColour[4];
            v[0].Position.Z = 0.5f;
            v[1].Position.Z = 0.5f;
            v[2].Position.Z = 0.5f;
            v[3].Position.Z = 0.5f;
        }

        public Quad(Quad from)
        {
            v = new VertexPositionTextureColour[4];

            for (int i = 0; i < 4; ++i)
            {
                v[i] = from.v[i];
            }
            tex = from.tex;
            blend = from.blend;
        }

        public static Quad Create (Vector3 a, Vector3 b, Vector3 c, Vector3 d, Rgba32 colour)
        {
            var q = new Quad ();
            q.v = new [] {
                new VertexPositionTextureColour (a, new Vector2 (0, 0), colour),
                new VertexPositionTextureColour (b, new Vector2 (0, 1), colour),
                new VertexPositionTextureColour (c, new Vector2 (1, 1), colour),
                new VertexPositionTextureColour (d, new Vector2 (1, 0), colour),
            };
            q.blend = BlendMode.Default;
            q.tex = null;

            return q;
        }
    }
}