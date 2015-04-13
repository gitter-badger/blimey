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
    using System.Collections.Generic;
    using System.Linq;
    using Abacus.SinglePrecision;
    using Fudge;
    using global::Blimey.Platform;
    using global::Blimey.Asset;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public static class DebugRendererExtensions
    {
        public static void AddAxis (
            this DebugRenderer debugRenderer,
            string renderPass, float size = 50f)
        {
            Rgba32 AxisColourX = Rgba32.Red;
            Rgba32 AxisColourY = Rgba32.Green;
            Rgba32 AxisColourZ = Rgba32.Blue;

            debugRenderer.AddLine(
                renderPass,
                new Vector3(-size, 0, 0),
                new Vector3(0, 0, 0),
                Rgba32.White);

            debugRenderer.AddLine(
                renderPass,
                new Vector3(0, -size, 0),
                new Vector3(0, 0, 0),
                Rgba32.White);

            debugRenderer.AddLine(
                renderPass,
                new Vector3(0, 0, -size),
                new Vector3(0, 0, 0),
                Rgba32.White);


            debugRenderer.AddLine(
                renderPass,
                new Vector3(0, 0, 0),
                new Vector3(size, 0, 0),
                AxisColourX);

            debugRenderer.AddLine(
                renderPass,
                new Vector3(0, 0, 0),
                new Vector3(0, size, 0),
                AxisColourY);

            debugRenderer.AddLine(
                renderPass,
                new Vector3(0, 0, 0),
                new Vector3(0, 0, size),
                AxisColourZ);
        }

        public static void AddGrid(
            this DebugRenderer debugRenderer,
            string renderPass, float gridSquareSize = 0.50f, int numberOfGridSquares = 10,
            bool ShowXZPlane = true, bool ShowXYPlane = false, bool ShowYZPlane = false)
        {
            Rgba32 AxisColourX = Rgba32.Red;
            Rgba32 AxisColourY = Rgba32.Green;
            Rgba32 AxisColourZ = Rgba32.Blue;
            Rgba32 gridColour = Rgba32.LightGrey;

            float length = numberOfGridSquares * 2 * gridSquareSize;
            float halfLength = length / 2;

            if( ShowXZPlane )
            {
                for (int i = 0; i < (numberOfGridSquares * 2 + 1); i++)
                {
                    if (i * gridSquareSize - halfLength == 0)
                        continue;

                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(-halfLength, 0.0f, i * gridSquareSize - halfLength),
                        new Vector3(halfLength, 0.0f, i * gridSquareSize - halfLength),
                        gridColour);

                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(i * gridSquareSize - halfLength, 0.0f, -halfLength),
                        new Vector3(i * gridSquareSize - halfLength, 0.0f, halfLength),
                        gridColour);
                }
            }

            if( ShowXYPlane )
            {
                for (int i = 0; i < (numberOfGridSquares * 2 + 1); i++)
                {
                    if (i * gridSquareSize - halfLength == 0)
                        continue;

                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(-halfLength, i * gridSquareSize - halfLength, 0f),
                        new Vector3(halfLength, i * gridSquareSize - halfLength, 0f),
                        gridColour);

                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(i * gridSquareSize - halfLength, -halfLength, 0f),
                        new Vector3(i * gridSquareSize - halfLength, halfLength, 0f),
                        gridColour);
                }
            }

            if( ShowYZPlane )
            {
                for (int i = 0; i < (numberOfGridSquares * 2 + 1); i++)
                {
                    if (i * gridSquareSize - halfLength == 0)
                        continue;

                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(0f, -halfLength, i * gridSquareSize - halfLength),
                        new Vector3(0f, halfLength, i * gridSquareSize - halfLength),
                        gridColour);

                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(0f, i * gridSquareSize - halfLength, -halfLength),
                        new Vector3(0f, i * gridSquareSize - halfLength, halfLength),
                        gridColour);
                }
            }


            if( ShowXYPlane )
            {
                for (int i = 0; i < (numberOfGridSquares * 2 + 1); i++)
                {
                    if (i * gridSquareSize - halfLength == 0)
                        continue;

                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(-halfLength, i * gridSquareSize - halfLength, 0f),
                        new Vector3(halfLength, i * gridSquareSize - halfLength, 0f),
                        gridColour);

                    debugRenderer.AddLine(
                        renderPass,
                        new Vector3(i * gridSquareSize - halfLength, -halfLength, 0f),
                        new Vector3(i * gridSquareSize - halfLength, halfLength, 0f),
                        gridColour);
                }
            }

            AddAxis (debugRenderer, renderPass, numberOfGridSquares * gridSquareSize);
        }
    }
}
