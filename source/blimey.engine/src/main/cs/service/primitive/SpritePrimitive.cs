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

    public class SpritePrimitive
    {
        protected Quad quad;
        protected float tx, ty, width, height;
        protected float tex_width, tex_height;
        protected float hotX, hotY;
        protected bool bXFlip, bYFlip, bHSFlip;

        readonly PrimitiveRenderer primitiveRenderer;

        public SpritePrimitive (PrimitiveRenderer zPrimitiveRenderer, SpritePrimitive zFrom)
        {
            primitiveRenderer = zPrimitiveRenderer;
            quad = new Quad(quad);
            tx = zFrom.tx;
            ty = zFrom.ty;
            width = zFrom.width;
            height = zFrom.height;
            tex_width = zFrom.tex_width;
            tex_height = zFrom.tex_height;
            hotX = zFrom.hotX;
            hotY = zFrom.hotY;
            bXFlip = zFrom.bXFlip;
            bYFlip = zFrom.bYFlip;
            bHSFlip = zFrom.bHSFlip;
        }

        public SpritePrimitive (PrimitiveRenderer zPrimitiveRenderer, Texture zTexture)
        {
            primitiveRenderer = zPrimitiveRenderer;
            float w = (float) zTexture.Width;
            float h = (float) zTexture.Height;
            Init(zTexture, 0, 0, w, h );
            SetHotSpot(w/2.0f, h/2.0f);
        }

        public SpritePrimitive(PrimitiveRenderer zPrimitiveRenderer, Texture texture, float texx, float texy, float w, float h)
        {
            primitiveRenderer = zPrimitiveRenderer;
            Init(texture, texx, texy, w, h);
            SetHotSpot(w/2.0f, h/2.0f);
        }

        void Init (Texture texture, float texx, float texy, float w, float h)
        {
            quad = new Quad();
            float texx1, texy1, texx2, texy2;

            tx = texx; ty = texy;
            width = w; height = h;

            if (texture != null)
            {
                tex_width = (float)texture.Width;
                tex_height = (float)texture.Height;
            }
            else
            {
                tex_width = 1.0f;
                tex_height = 1.0f;
            }

            hotX = 0;
            hotY = 0;
            bXFlip = false;
            bYFlip = false;
            bHSFlip = false;
            quad.tex = texture;

            texx1 = texx / tex_width;
            texy1 = texy / tex_height;
            texx2 = (texx + w) / tex_width;
            texy2 = (texy + h) / tex_height;

            quad.v[2].UV.X = texx1; quad.v[2].UV.Y = texy1;
            quad.v[3].UV.X = texx2; quad.v[3].UV.Y = texy1;
            quad.v[0].UV.X = texx2; quad.v[0].UV.Y = texy2;
            quad.v[1].UV.X = texx1; quad.v[1].UV.Y = texy2;

            quad.v[0].Position.Z = quad.v[1].Position.Z = quad.v[2].Position.Z = quad.v[3].Position.Z = 0.5f;
            quad.v[0].Colour = quad.v[1].Colour = quad.v[2].Colour = quad.v[3].Colour = Rgba32.White;

            quad.blend = BlendMode.Default;
        }

        public void SetColour(Rgba32 _col)
        {
            quad.v[0].Colour = quad.v[1].Colour = quad.v[2].Colour = quad.v[3].Colour = _col;
        }

        public void SetColour(Rgba32 _col, uint i )
        {
            System.Diagnostics.Debug.Assert(i < 4);

            quad.v[i].Colour = _col;
        }

        public Rgba32 GetColour()
        {
            return quad.v[0].Colour;
        }

        public Rgba32 GetColour( uint i )
        {
            System.Diagnostics.Debug.Assert(i < 4);

            return quad.v[i].Colour;


        }

        public void SetHotSpot(float _x, float _y)
        {
            hotX = _x;
            hotY = _y;
        }

        public void CenterHotSpot()
        {
            hotX = width / 2;
            hotY = height / 2;
        }

        public Vector2 GetHotSpot()
        {
            return new Vector2( hotX, hotY );
        }

        public void GetHotSpot( ref float x, ref float y )
        {
            x = hotX;
            y = hotY;
        }

        public void SetZ(float _z)
        {
            quad.v[0].Position.Z = quad.v[1].Position.Z = quad.v[2].Position.Z = quad.v[3].Position.Z = _z;
        }

        public float GetZ()
        {
            return quad.v[0].Position.Z;
        }

        public void SetZ(float _z, uint i)
        {
            System.Diagnostics.Debug.Assert(i < 4);

            quad.v[i].Position.Z = _z;
        }

        public float GetZ( uint i )
        {
            System.Diagnostics.Debug.Assert(i < 4);
            return quad.v[i].Position.Z;

        }

        public void SetFlip(bool _horizontal, bool _vertical)
        {
            SetFlip(_horizontal, _vertical, false);
        }

        public void SetFlip(bool _horizontal, bool _vertical, bool _bHotSpot)
        {
            float tx, ty;

            if (bHSFlip && bXFlip) hotX = width - hotX;
            if (bHSFlip && bYFlip) hotY = height - hotY;

            bHSFlip = _bHotSpot;

            if (bHSFlip && bXFlip) hotX = width - hotX;
            if (bHSFlip && bYFlip) hotY = height - hotY;

            if (_horizontal != bXFlip)
            {
                tx = quad.v[0].UV.X;
                quad.v[0].UV.X = quad.v[1].UV.X;
                quad.v[1].UV.X = tx;
                ty = quad.v[0].UV.Y;
                quad.v[0].UV.Y = quad.v[1].UV.Y;
                quad.v[1].UV.Y = ty;
                tx = quad.v[3].UV.X;
                quad.v[3].UV.X = quad.v[2].UV.X;
                quad.v[2].UV.X = tx;
                ty = quad.v[3].UV.Y;
                quad.v[3].UV.Y = quad.v[2].UV.Y;
                quad.v[2].UV.Y = ty;

                bXFlip = !bXFlip;
            }

            if (_vertical != bYFlip)
            {
                tx = quad.v[0].UV.X;
                quad.v[0].UV.X = quad.v[3].UV.X;
                quad.v[3].UV.X = tx;
                ty = quad.v[0].UV.Y;
                quad.v[0].UV.Y = quad.v[3].UV.Y;
                quad.v[3].UV.Y = ty;
                tx = quad.v[1].UV.X;
                quad.v[1].UV.X = quad.v[2].UV.X;
                quad.v[2].UV.X = tx;
                ty = quad.v[1].UV.Y;
                quad.v[1].UV.Y = quad.v[2].UV.Y;
                quad.v[2].UV.Y = ty;

                bYFlip = !bYFlip;
            }
        }

        public void Draw(String renderPass, float _x, float _y)
        {
            float tempx1, tempy1, tempx2, tempy2;

            tempx1 = _x - hotX;
            tempy1 = _y - hotY;
            tempx2 = _x + width - hotX;
            tempy2 = _y + height - hotY;

            quad.v[0].Position.X = tempx1; quad.v[0].Position.Y = tempy1;
            quad.v[1].Position.X = tempx2; quad.v[1].Position.Y = tempy1;
            quad.v[2].Position.X = tempx2; quad.v[2].Position.Y = tempy2;
            quad.v[3].Position.X = tempx1; quad.v[3].Position.Y = tempy2;

            primitiveRenderer.AddQuad(renderPass, quad);
        }

        public void DrawEx(String renderPass, float _x, float _y, float _rot, float _hscale, float _vscale)
        {

            float tx1, ty1, tx2, ty2;
            float sint, cost;

            if (_vscale == 0) _vscale = _hscale;

            tx1 = -hotX * _hscale;
            ty1 = -hotY * _vscale;
            tx2 = (width - hotX) * _hscale;
            ty2 = (height - hotY) * _vscale;

            if (_rot != 0.0f)
            {
                cost = (float) Math.Cos((double) _rot);
                sint = (float) Math.Sin((double)_rot);

                quad.v[0].Position.X = tx1 * cost - ty1 * sint + _x;
                quad.v[0].Position.Y = tx1 * sint + ty1 * cost + _y;

                quad.v[1].Position.X = tx2 * cost - ty1 * sint + _x;
                quad.v[1].Position.Y = tx2 * sint + ty1 * cost + _y;

                quad.v[2].Position.X = tx2 * cost - ty2 * sint + _x;
                quad.v[2].Position.Y = tx2 * sint + ty2 * cost + _y;

                quad.v[3].Position.X = tx1 * cost - ty2 * sint + _x;
                quad.v[3].Position.Y = tx1 * sint + ty2 * cost + _y;
            }
            else
            {
                quad.v[0].Position.X = tx1 + _x;
                quad.v[0].Position.Y = ty1 + _y;
                quad.v[1].Position.X = tx2 + _x;
                quad.v[1].Position.Y = ty1 + _y;
                quad.v[2].Position.X = tx2 + _x;
                quad.v[2].Position.Y = ty2 + _y;
                quad.v[3].Position.X = tx1 + _x;
                quad.v[3].Position.Y = ty2 + _y;
            }

            primitiveRenderer.AddQuad(renderPass, quad);

        }

        public void DrawEx(String renderPass, float _x, float _y, float _rot, float _hscale)
        {
            DrawEx(renderPass, _x, _y, _rot, _hscale, _hscale);
        }

        public void DrawStretch(String renderPass, float _x1, float _y1, float _x2, float _y2)
        {
            quad.v[0].Position.X = _x1; quad.v[0].Position.Y = _y1;
            quad.v[1].Position.X = _x2; quad.v[1].Position.Y = _y1;
            quad.v[2].Position.X = _x2; quad.v[2].Position.Y = _y2;
            quad.v[3].Position.X = _x1; quad.v[3].Position.Y = _y2;

            primitiveRenderer.AddQuad(renderPass, quad);

        }

        public void Draw4V(String renderPass, float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3)
        {
            quad.v[0].Position.X = x0; quad.v[0].Position.Y = y0;
            quad.v[1].Position.X = x1; quad.v[1].Position.Y = y1;
            quad.v[2].Position.X = x2; quad.v[2].Position.Y = y2;
            quad.v[3].Position.X = x3; quad.v[3].Position.Y = y3;


            primitiveRenderer.AddQuad(renderPass, quad);
        }

        public Texture GetTexture()
        {
            return quad.tex;
        }

        public void GetTextureRect(ref float x, ref float y, ref float w, ref float h)
        {
            x=tx;
            y=ty;
            w=width;
            h=height;
        }

        public BlendMode GetBlendMode()
        {
            return quad.blend;
        }

        public void SetBlendMode(BlendMode _blend)
        {
            quad.blend = _blend;
        }

        public void SetTexture(Texture tex)
        {
            float tx1, ty1, tx2, ty2;
            float tw, th;

            quad.tex = tex;

            if (tex != null )
            {
                tw = (float) tex.Width;
                th = (float) tex.Height;
            }
            else
            {
                tw = 1.0f;
                th = 1.0f;
            }

            //if the size of the texture has changed
            if (tw != tex_width || th != tex_height)
            {
                tx1 = quad.v[0].UV.X * tex_width;
                ty1 = quad.v[0].UV.Y * tex_height;
                tx2 = quad.v[2].UV.X * tex_width;
                ty2 = quad.v[2].UV.Y * tex_height;

                tex_width = tw;
                tex_height = th;

                tx1 /= tw; ty1 /= th;
                tx2 /= tw; ty2 /= th;

                quad.v[0].UV.X = tx1;
                quad.v[0].UV.Y = ty1;
                quad.v[1].UV.X = tx2;
                quad.v[1].UV.Y = ty1;
                quad.v[2].UV.X = tx2;
                quad.v[2].UV.Y = ty2;
                quad.v[3].UV.X = tx1;
                quad.v[3].UV.Y = ty2;
            }
        }

        public void SetTextureRect(float x, float y, float w, float h)
        {
            SetTextureRect( x, y, w, h, true);
        }

        public void SetTextureRect(float x, float y, float w, float h, bool adjSize)
        {
            float tx1, ty1, tx2, ty2;
            bool bX, bY, bHS;

            tx = x;
            ty = y;

            if (adjSize)
            {
                width = w;
                height = h;
            }

            tx1 = tx / tex_width;
            ty1 = ty / tex_height;
            tx2 = (tx + w) / tex_width;
            ty2 = (ty + h) / tex_height;

            quad.v[0].UV.X = tx1;
            quad.v[0].UV.Y = ty1;
            quad.v[1].UV.X = tx2;
            quad.v[1].UV.Y = ty1;
            quad.v[2].UV.X = tx2;
            quad.v[2].UV.Y = ty2;
            quad.v[3].UV.X = tx1;
            quad.v[3].UV.Y = ty2;

            bX = bXFlip;
            bY = bYFlip;
            bHS = bHSFlip;
            bXFlip = false;
            bYFlip = false;
            SetFlip(bX, bY, bHS);
        }

        public float GetWidth()
        {
            return width;
        }

        public float GetHeight()
        {
            return height;
        }

    }
}