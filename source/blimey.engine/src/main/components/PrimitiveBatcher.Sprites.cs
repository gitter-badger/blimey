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
// │ ~ AzSprH Pook (http://www.ajpook.com)                                     │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining  │ \\
// │ a copy of this software and associated documentation files (the        │ \\
// │ "Software"), to deal in the Software without restriction, including    │ \\
// │ without limitation the rights to use, copy, modify, merge, publizSprH,    │ \\
// │ distribute, sublicense, and/or sellcopies of the Software, and to      │ \\
// │ permit persons to whom the Software is furnizSprHed to do so, subject to  │ \\
// │ the following conditions:                                              │ \\
// │                                                                        │ \\
// │ The above copyright notice and this permission notice zSprHall be         │ \\
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

    partial class PrimitiveBatcher
    {

        // TODO
        // + Change usage of Single, Single to Vector2.
        // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

        public class Sprite
        {
            // The primitive used to render this sprite.
            protected readonly PrimitiveBatcher.Quad quad;

            // This is the UV start point for the current sprite, measured in texels, not UV space.
            protected Single texX, texY;

            // The width and height of the sprite in texels.  So this combined with texX & texY
            // above allows us to work out what portion of the texture the sprite is using.
            protected Single sprWidth, sprHeight;

            // The actual width and height of the underlying texture in texels.
            protected Single textureWidth, textureHeight;

            // The rendering hotspot in texels.
            protected Single hotX, hotY;

            // Flags.
            protected Boolean bXFlip, bYFlip, bHSFlip;

            // Reference to the primitive renderer.
            readonly PrimitiveBatcher primitiveBatcher;

            /// <summary>
            /// Creates a new sprite object from a given texture.
            /// </summary>
            public Sprite (PrimitiveBatcher zPrimitiveBatcher, Texture zTexture)
            {
                primitiveBatcher = zPrimitiveBatcher;

                // Units: Texels
                Single sprW = (Single) zTexture.Width;
                Single sprH = (Single) zTexture.Height;

                quad = new Quad();

                Init (zTexture, 0, 0, sprW, sprH );
                SetHotSpot (sprW / 2f, sprH / 2f);
            }

            /// <summary>
            /// Creates a new sprite object by cloning an existing sprite object.
            /// </summary>
            public Sprite (PrimitiveBatcher zPrimitiveBatcher, Sprite zFrom)
            {
                primitiveBatcher = zPrimitiveBatcher;

                quad = new Quad(quad);

                texX = zFrom.texX;
                texY = zFrom.texY;

                sprWidth = zFrom.sprWidth;
                sprHeight = zFrom.sprHeight;

                textureWidth = zFrom.textureWidth;
                textureHeight = zFrom.textureHeight;

                hotX = zFrom.hotX;
                hotY = zFrom.hotY;

                bXFlip = zFrom.bXFlip;
                bYFlip = zFrom.bYFlip;
                bHSFlip = zFrom.bHSFlip;
            }


            /// <summary>
            /// Creates a new sprite object from part of a given texture.
            /// - zTexX & zTexY represent the an offset in texels that map to the start of the UVs used for the sprite.
            /// - zSprW & zSprH also measured in texels, indicate how much of the texture will be used.
            /// </summary>
            public Sprite (PrimitiveBatcher zPrimitiveBatcher, Texture zTexture, Single zTexX, Single zTexY, Single zSprW, Single zSprH)
            {
                primitiveBatcher = zPrimitiveBatcher;

                quad = new Quad ();

                Init (zTexture, zTexX, zTexY, zSprW, zSprH);
                SetHotSpot (zSprW / 2f, zSprH / 2f);
            }

            void Init (Texture zTexture, Single zTexX, Single zTexY, Single zSprW, Single zSprH)
            {
                texX = zTexX;
                texY = zTexY;

                sprWidth = zSprW;
                sprHeight = zSprH;

                if (zTexture != null)
                {
                    textureWidth = (Single) zTexture.Width;
                    textureHeight = (Single) zTexture.Height;
                }
                else
                {
                    // If the texture is NULL, then we say the texture is 1 texel by 1 texel.
                    textureWidth = 1f;
                    textureHeight = 1f;
                }

                hotX = 0;
                hotY = 0;
                bXFlip = false;
                bYFlip = false;
                bHSFlip = false;
                quad.tex = zTexture;

                Single uStart = zTexX / textureWidth;
                Single vStart = zTexY / textureHeight;
                Single uEnd = (zTexX + zSprW) / textureWidth;
                Single vEnd = (zTexY + zSprH) / textureHeight;

                quad.v[0].UV.X = uStart; quad.v[0].UV.Y = vStart;
                quad.v[1].UV.X = uEnd;   quad.v[1].UV.Y = vStart;
                quad.v[2].UV.X = uEnd;   quad.v[2].UV.Y = vEnd;
                quad.v[3].UV.X = uStart; quad.v[3].UV.Y = vEnd;

                quad.v[0].Position.Z =
                quad.v[1].Position.Z =
                quad.v[2].Position.Z =
                quad.v[3].Position.Z = 0.5f;

                quad.v[0].Colour =
                quad.v[1].Colour =
                quad.v[2].Colour =
                quad.v[3].Colour = Rgba32.White;

                quad.blend = BlendMode.Default;
            }


            // GETTERS
            // ────────────────────────────────────────────────────────────────────────────────────────────────────────── //


            // In pixels
            public Single GetWidth ()
            {
                return sprWidth;
            }

            // In pixels
            public Single GetHeight ()
            {
                return sprHeight;
            }

            public Rgba32 GetColour ()
            {
                return quad.v[0].Colour;
            }

            public Rgba32 GetColour (Int32 zI)
            {
                System.Diagnostics.Debug.Assert (zI < 4 && zI >= 0);
                return quad.v[zI].Colour;
            }

            public Vector2 GetHotSpot ()
            {
                return new Vector2 (hotX, hotY);
            }

            public void GetHotSpot (ref Single x, ref Single y)
            {
                x = hotX;
                y = hotY;
            }

            public Single GetZ()
            {
                return quad.v[0].Position.Z;
            }

            public Single GetZ (Int32 zI)
            {
                System.Diagnostics.Debug.Assert (zI < 4 && zI >= 0);
                return quad.v[zI].Position.Z;
            }

            public Texture GetTexture ()
            {
                return quad.tex;
            }

            public void GetTextureRect (ref Single x, ref Single y, ref Single zSprW, ref Single zSprH)
            {
                x = texX;
                y = texY;
                zSprW = sprWidth;
                zSprH = sprHeight;
            }

            public BlendMode GetBlendMode ()
            {
                return quad.blend;
            }


            // SETTERS
            // ────────────────────────────────────────────────────────────────────────────────────────────────────────── //

            public void SetColour (Rgba32 zColour)
            {
                quad.v[0].Colour =
                quad.v[1].Colour =
                quad.v[2].Colour =
                quad.v[3].Colour = zColour;
            }

            /// <summary>
            /// Sets the colour of an individual Quad vertex.
            /// </summary>
            public void SetColour (Rgba32 zColour, Int32 zI)
            {
                System.Diagnostics.Debug.Assert (zI < 4 && zI >= 0);
                quad.v[zI].Colour = zColour;
            }

            public void SetHotSpot (Single zX, Single zY)
            {
                hotX = zX;
                hotY = zY;
            }

            public void CenterHotSpot ()
            {
                hotX = sprWidth / 2f;
                hotY = sprHeight / 2f;
            }

            public void SetZ (Single zZ)
            {
                quad.v[0].Position.Z =
                quad.v[1].Position.Z =
                quad.v[2].Position.Z =
                quad.v[3].Position.Z = zZ;
            }

            public void SetZ (Single zZ, Int32 zI)
            {
                System.Diagnostics.Debug.Assert (zI < 4 && zI >= 0);
                quad.v[zI].Position.Z = zZ;
            }

            public void SetFlip (Boolean zHorizontal, Boolean zVertical)
            {
                SetFlip (zHorizontal, zVertical, false);
            }

            public void SetFlip (Boolean zHorizontal, Boolean zVertical, Boolean zHotSpot)
            {
                Single texX, texY;

                if (bHSFlip && bXFlip) hotX = sprWidth - hotX;
                if (bHSFlip && bYFlip) hotY = sprHeight - hotY;

                bHSFlip = zHotSpot;

                if (bHSFlip && bXFlip) hotX = sprWidth - hotX;
                if (bHSFlip && bYFlip) hotY = sprHeight - hotY;

                if (zHorizontal != bXFlip)
                {
                    texX = quad.v[0].UV.X;
                    quad.v[0].UV.X = quad.v[1].UV.X;
                    quad.v[1].UV.X = texX;

                    texY = quad.v[0].UV.Y;
                    quad.v[0].UV.Y = quad.v[1].UV.Y;
                    quad.v[1].UV.Y = texY;

                    texX = quad.v[3].UV.X;
                    quad.v[3].UV.X = quad.v[2].UV.X;
                    quad.v[2].UV.X = texX;

                    texY = quad.v[3].UV.Y;
                    quad.v[3].UV.Y = quad.v[2].UV.Y;
                    quad.v[2].UV.Y = texY;

                    bXFlip = !bXFlip;
                }

                if (zVertical != bYFlip)
                {
                    texX = quad.v[0].UV.X;
                    quad.v[0].UV.X = quad.v[3].UV.X;
                    quad.v[3].UV.X = texX;

                    texY = quad.v[0].UV.Y;
                    quad.v[0].UV.Y = quad.v[3].UV.Y;
                    quad.v[3].UV.Y = texY;

                    texX = quad.v[1].UV.X;
                    quad.v[1].UV.X = quad.v[2].UV.X;
                    quad.v[2].UV.X = texX;

                    texY = quad.v[1].UV.Y;
                    quad.v[1].UV.Y = quad.v[2].UV.Y;
                    quad.v[2].UV.Y = texY;

                    bYFlip = !bYFlip;
                }
            }

            public void SetBlendMode (BlendMode zBlend)
            {
                quad.blend = zBlend;
            }

            public void SetTexture (Texture zTexture)
            {
                Single tw, th;

                quad.tex = zTexture;

                if (zTexture != null )
                {
                    tw = (Single) zTexture.Width;
                    th = (Single) zTexture.Height;
                }
                else
                {
                    tw = 1.0f;
                    th = 1.0f;
                }

                //if the size of the texture has changed
                if (tw != textureWidth || th != textureHeight)
                {
                    Single uStart = quad.v[0].UV.X * textureWidth;
                    Single vStart = quad.v[0].UV.Y * textureHeight;
                    Single uEnd =   quad.v[2].UV.X * textureWidth;
                    Single vEnd =   quad.v[2].UV.Y * textureHeight;

                    textureWidth = tw;
                    textureHeight = th;

                    uStart /= tw; vStart /= th;
                    uEnd /= tw; vEnd /= th;

                    quad.v[0].UV.X = uStart; quad.v[0].UV.Y = vStart;
                    quad.v[1].UV.X = uEnd;   quad.v[1].UV.Y = vStart;
                    quad.v[2].UV.X = uEnd;   quad.v[2].UV.Y = vEnd;
                    quad.v[3].UV.X = uStart; quad.v[3].UV.Y = vEnd;
                }
            }

            // In texel space, not UV space
            public void SetTextureRect (Single zStartX, Single zStartY, Single zWidth, Single zHeight)
            {
                SetTextureRect (zStartX, zStartY, zWidth, zHeight, true);
            }

            // In texel space, not UV space
            public void SetTextureRect (Single zStartX, Single zStartY, Single zWidth, Single zHeight, Boolean zAdjustSize)
            {
                texX = zStartX;
                texY = zStartY;

                if (zAdjustSize)
                {
                    sprWidth = zWidth;
                    sprHeight = zHeight;
                }

                Single uStart = texX / textureWidth;
                Single vStart = texY / textureHeight;
                Single uEnd = (texX + zWidth) / textureWidth;
                Single vEnd = (texY + zHeight) / textureHeight;

                quad.v[0].UV.X = uStart; quad.v[0].UV.Y = vStart;
                quad.v[1].UV.X = uEnd;   quad.v[1].UV.Y = vStart;
                quad.v[2].UV.X = uEnd;   quad.v[2].UV.Y = vEnd;
                quad.v[3].UV.X = uStart; quad.v[3].UV.Y = vEnd;

                Boolean bX = bXFlip;
                Boolean bY = bYFlip;
                Boolean bHS = bHSFlip;
                bXFlip = false;
                bYFlip = false;

                SetFlip(bX, bY, bHS);
            }


            // RENDERING
            // ────────────────────────────────────────────────────────────────────────────────────────────────────────── //

            public void Draw (String zRenderPass, Single zX, Single zY)
            {
                Single posx1 = zX - hotX;
                Single posy1 = zY - hotY;
                Single posx2 = zX + sprWidth - hotX;
                Single posy2 = zY + sprHeight - hotY;

                quad.v[0].Position.X = posx1; quad.v[0].Position.Y = posy1;
                quad.v[1].Position.X = posx2; quad.v[1].Position.Y = posy1;
                quad.v[2].Position.X = posx2; quad.v[2].Position.Y = posy2;
                quad.v[3].Position.X = posx1; quad.v[3].Position.Y = posy2;

                primitiveBatcher.AddQuad (zRenderPass, quad);
            }

            public void DrawEx (String zRenderPass, Single zX, Single zY, Single zRotation, Single zHorizontalScale, Single zVerticalScale)
            {
                if (zVerticalScale == 0) zVerticalScale = zHorizontalScale;

                Single aX = -hotX * zHorizontalScale;
                Single aY = -hotY * zVerticalScale;
                Single bX = (sprWidth - hotX) * zHorizontalScale;
                Single bY = (sprHeight - hotY) * zVerticalScale;

                if (zRotation != 0.0f)
                {
                    Single cost = (Single) Math.Cos((double) zRotation);
                    Single sint = (Single) Math.Sin((double)zRotation);

                    quad.v[0].Position.X = aX * cost - aY * sint + zX;
                    quad.v[0].Position.Y = aX * sint + aY * cost + zY;

                    quad.v[1].Position.X = bX * cost - aY * sint + zX;
                    quad.v[1].Position.Y = bX * sint + aY * cost + zY;

                    quad.v[2].Position.X = bX * cost - bY * sint + zX;
                    quad.v[2].Position.Y = bX * sint + bY * cost + zY;

                    quad.v[3].Position.X = aX * cost - bY * sint + zX;
                    quad.v[3].Position.Y = aX * sint + bY * cost + zY;
                }
                else
                {
                    quad.v[3].Position.X = aX + zX; quad.v[3].Position.Y = aY + zY;
                    quad.v[2].Position.X = bX + zX; quad.v[2].Position.Y = aY + zY;
                    quad.v[1].Position.X = bX + zX; quad.v[1].Position.Y = bY + zY;
                    quad.v[0].Position.X = aX + zX; quad.v[0].Position.Y = bY + zY;
                }

                primitiveBatcher.AddQuad (zRenderPass, quad);

            }

            public void DrawEx (String zRenderPass, Single zX, Single zY, Single zRotation, Single zScale)
            {
                DrawEx (zRenderPass, zX, zY, zRotation, zScale, zScale);
            }

            public void DrawStretch (String zRenderPass, Single zX1, Single zY1, Single zX2, Single zY2)
            {
                quad.v[0].Position.X = zX1; quad.v[0].Position.Y = zY1;
                quad.v[1].Position.X = zX2; quad.v[1].Position.Y = zY1;
                quad.v[2].Position.X = zX2; quad.v[2].Position.Y = zY2;
                quad.v[3].Position.X = zX1; quad.v[3].Position.Y = zY2;

                primitiveBatcher.AddQuad (zRenderPass, quad);

            }

            public void Draw4V (String zRenderPass, Single zX0, Single zY0, Single zX1, Single zY1, Single zX2, Single zY2, Single zX3, Single zY3)
            {
                quad.v[0].Position.X = zX0; quad.v[0].Position.Y = zY0;
                quad.v[1].Position.X = zX1; quad.v[1].Position.Y = zY1;
                quad.v[2].Position.X = zX2; quad.v[2].Position.Y = zY2;
                quad.v[3].Position.X = zX3; quad.v[3].Position.Y = zY3;

                primitiveBatcher.AddQuad (zRenderPass, quad);
            }
        }
    }
}
