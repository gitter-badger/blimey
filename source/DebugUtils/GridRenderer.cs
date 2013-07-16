// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Blimey - Fast, efficient, high level engine built upon Cor & Abacus    │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Brought to you by:                                                     │ \\
// │          _________                    .__               __             │ \\
// │         /   _____/__ __  ____    ____ |__|____    _____/  |_           │ \\
// │         \_____  \|  |  \/    \  / ___\|  \__  \  /    \   __\          │ \\
// │         /        \  |  /   |  \/ /_/  >  |/ __ \|   |  \  |            │ \\
// │        /_______  /____/|___|  /\___  /|__(____  /___|  /__|            │ \\
// │                \/           \//_____/         \/     \/                │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2013 A.J.Pook (http://sungiant.github.com)                 │ \\
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

using Sungiant.Abacus;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;

namespace Sungiant.Blimey
{

	public class GridRenderer
	{
		public Rgba32 AxisColourX = Rgba32.Red; // going to the right of the screen
		public Rgba32 AxisColourY = Rgba32.Green; // up
		public Rgba32 AxisColourZ = Rgba32.Blue; // coming out of the screen

		float gridSquareSize = 0.50f;  // in meters
		int numberOfGridSquares = 10;

		Rgba32 gridColour = Rgba32.LightGray;

        public bool ShowXZPlane = true;
        public bool ShowXYPlane = false;
        public bool ShowYZPlane = false;

		string renderPass;
		DebugShapeRenderer debugRenderer;

		public GridRenderer(DebugShapeRenderer debugRenderer, string renderPass)
		{
			this.debugRenderer = debugRenderer;
			this.renderPass = renderPass;
		}

		public void Update()
		{
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

			debugRenderer.AddLine(
					renderPass,
				new Vector3(-numberOfGridSquares * gridSquareSize, 0, 0),
				new Vector3(0, 0, 0),
				Rgba32.White);

			debugRenderer.AddLine(
					renderPass,
				new Vector3(0, -numberOfGridSquares * gridSquareSize, 0),
				new Vector3(0, 0, 0),
				Rgba32.White);

			debugRenderer.AddLine(
					renderPass,
				new Vector3(0, 0, -numberOfGridSquares * gridSquareSize),
				new Vector3(0, 0, 0),
				Rgba32.White);


			debugRenderer.AddLine(
					renderPass,
				new Vector3(0, 0, 0),
				new Vector3(numberOfGridSquares * gridSquareSize, 0, 0),
				AxisColourX);

			debugRenderer.AddLine(
					renderPass,
				new Vector3(0, 0, 0),
				new Vector3(0, numberOfGridSquares * gridSquareSize, 0),
				AxisColourY);

			debugRenderer.AddLine(
					renderPass,
				new Vector3(0, 0, 0),
				new Vector3(0, 0, numberOfGridSquares * gridSquareSize),
				AxisColourZ);

		}
	}
}

