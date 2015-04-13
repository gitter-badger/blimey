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

namespace PlatformDemo
{
    using System;
    using System.Text;
    using System.IO;
    using Abacus.SinglePrecision;
    using Fudge;
    using Blimey.Platform;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class BasicApp : IApp
    {
        IElement[] elements;
        Rgba32 currentColour = Rgba32.Black;
        Rgba32 nextColour = Rgba32.DarkSlateBlue;
        readonly Single colourChangeTime = 10f;
        Single colourChangeProgress = 0f;
        Int32 numCols;
        Int32 numRows;
        Texture tex;
        Shader shader;


        VertPosCol[] testLines = {
            new VertPosCol (new Vector3 (-1, -1, 0.5f), Rgba32.Red),
            new VertPosCol (new Vector3 (1f, 1f, 0.5f), Rgba32.Yellow),
            new VertPosCol (new Vector3 (-1, 1, 0.5f), Rgba32.Blue),
            new VertPosCol (new Vector3 (1f, -1f, 0.5f), Rgba32.Green)
        };

        public void Start (Platform platform)
        {
            shader = Shaders.CreateUnlit (platform);

            Int32 texSize = 256;
            Int32 gridSize = 4;
            Int32 squareSize = texSize / gridSize;

            var colours = new Rgba32 [gridSize*gridSize];

            for (Int32 x = 0; x < gridSize; ++x)
            {
                for (Int32 y = 0; y < gridSize; ++y)
                {
                    colours [x + (y * gridSize)] = RandomColours.GetNext ();
                }
            }

            var texData = new byte[texSize*texSize*4];

            Int32 index = 0;
            for (Int32 x = 0; x < texSize; ++x)
            {
                for (Int32 y = 0; y < texSize; ++y)
                {
                    texData [index++] = colours[(x/squareSize) + (y/squareSize*gridSize)].A;
                    texData [index++] = colours[(x/squareSize) + (y/squareSize*gridSize)].R;
                    texData [index++] = colours[(x/squareSize) + (y/squareSize*gridSize)].G;
                    texData [index++] = colours[(x/squareSize) + (y/squareSize*gridSize)].B;
                }
            }

            tex = platform.Graphics.CreateTexture (TextureFormat.Rgba32, texSize, texSize, texData );

            elements = new IElement[]
            {
                new Element <CubePosTex, VertPosTex> (shader, tex),
                new Element <CylinderPosTex, VertPosTex> (shader, tex),
                new Element <BillboardPosTexCol, VertPosTexCol> (shader, tex),
                new Element <BillboardPosTex, VertPosTex> (shader, tex),
                new Element <CylinderPosNormTex, VertPosNormTex> (shader, tex),
                new Element <CylinderNormTexPos, VertNormTexPos> (shader, tex),
                new Element <FlowerPosCol, VertPosCol> (shader, null),
                new Element <FlowerPos, VertPos> (shader, null),
            };

            Double s = Math.Sqrt (elements.Length);

            numCols = (Int32) Math.Ceiling (s);

            numRows = (Int32) Math.Floor (s);

            while (elements.Length > numCols * numRows) ++numRows;


            foreach (var element in elements) element.Load (platform);
        }

        public Boolean Update (Platform platform, AppTime time)
        {
            if (platform.Input.Keyboard.IsFunctionalKeyDown (FunctionalKey.Escape))
                return true;

            colourChangeProgress += time.Delta / colourChangeTime;

            if (colourChangeProgress >= 1f)
            {
                colourChangeProgress = 0f;
                currentColour = nextColour;
                nextColour = RandomColours.GetNext();
            }

            foreach (var element in elements)
                element.Update (platform, time);

            return false;
        }

        public void Render (Platform platform)
        {
            platform.Graphics.Reset ();
            platform.Graphics.ClearColourBuffer(Rgba32.Lerp (currentColour, nextColour, colourChangeProgress));
            platform.Graphics.ClearDepthBuffer(1f);

            var world = Matrix44.Identity;
            var view = Matrix44.CreateLookAt (Vector3.UnitZ, Vector3.Forward, Vector3.Up);
            var projection = Matrix44.CreateOrthographicOffCenter (-1f, 1f, -1f, 1f, 1f, -1f);
            shader.ResetVariables ();
            shader.ResetSamplers ();
            shader.SetVariable ("World", world);
            shader.SetVariable ("View", view);
            shader.SetVariable ("Projection", projection);
            shader.SetVariable ("Colour", Rgba32.White);
            shader.SetSamplerTarget ("TextureSampler", 0);
            platform.Graphics.SetActive (shader, testLines[0].VertexDeclaration);
            platform.Graphics.DrawUserPrimitives (PrimitiveType.LineList, testLines, 0, testLines.Length / 2);

            // grid index
            Int32 x = 0;
            Int32 y = numRows - 1;

            foreach (var element in elements)
            {
                Single left     = -1f - (x*2f);
                Single right    = -1f + (2f * numCols) - (x*2f);
                Single bottom   = -1f - (y*2f);
                Single top      = -1f + (2f * numRows) - (y*2f);

                Matrix44 proj = Matrix44.CreateOrthographicOffCenter (left, right, bottom, top, 1f, -1f);

                element.Render (platform, proj);

                if (++x >= numCols) {x = 0; --y;}
            }
        }

        public void Stop (Platform platform)
        {
            foreach (var element in elements)
                element.Unload ();
        }
    }
}
