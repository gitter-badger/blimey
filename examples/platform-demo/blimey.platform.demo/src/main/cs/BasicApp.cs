namespace Cor.Demo
{
    using System;
    using System.Text;
    using System.IO;
    using Abacus.SinglePrecision;
    using Fudge;
    using Cor;
    using System.Collections.Generic;
    using Platform;
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

        public void Start (Engine engine)
        {
            shader = Shaders.CreateUnlit (engine);

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

            tex = engine.Graphics.CreateTexture (TextureFormat.Rgba32, texSize, texSize, texData );

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


            foreach (var element in elements) element.Load (engine);
        }

        public Boolean Update (Engine cor, AppTime time)
        {
            if (cor.Input.Keyboard.IsFunctionalKeyDown (FunctionalKey.Escape))
                return true;

            colourChangeProgress += time.Delta / colourChangeTime;

            if (colourChangeProgress >= 1f)
            {
                colourChangeProgress = 0f;
                currentColour = nextColour;
                nextColour = RandomColours.GetNext();
            }

            foreach (var element in elements)
                element.Update (cor, time);

            return false;
        }

        public void Render (Engine cor)
        {
            cor.Graphics.Reset ();
            cor.Graphics.ClearColourBuffer(Rgba32.Lerp (currentColour, nextColour, colourChangeProgress));
            cor.Graphics.ClearDepthBuffer(1f);

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
            cor.Graphics.SetActive (shader, testLines[0].VertexDeclaration);
            cor.Graphics.DrawUserPrimitives (PrimitiveType.LineList, testLines, 0, testLines.Length / 2);

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

                element.Render (cor, proj);

                if (++x >= numCols) {x = 0; --y;}
            }
        }

        public void Stop (Engine cor)
        {
            foreach (var element in elements)
                element.Unload ();
        }
    }
}
