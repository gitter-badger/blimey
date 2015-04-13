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
    using System.Runtime.InteropServices;
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    using Fudge;
    using global::Blimey.Platform;
    using global::Blimey.Asset;
    using Abacus.SinglePrecision;

    using System.Linq;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class BoundingRectangle
    {

        public float    x1, y1, x2, y2;
        bool    bClean;

        public BoundingRectangle(float _x1, float _y1, float _x2, float _y2)
        {
            x1=_x1;
            y1=_y1;
            x2=_x2;
            y2=_y2;
            bClean=false;
        }

        public BoundingRectangle()
        {
            bClean=true;
        }

        public void Clear()
        {
            bClean=true;
        }

        public bool IsClean()
        {
            return bClean;
        }

        public void Set(float _x1, float _y1, float _x2, float _y2)
        {
            x1=_x1;
            x2=_x2;
            y1=_y1;
            y2=_y2;
            bClean=false;
        }

        public void SetRadius(float x, float y, float r)
        {
            x1=x-r;
            x2=x+r;
            y1=y-r;
            y2=y+r;
            bClean=false;
        }

        public void Encapsulate(float x, float y)
        {
            if(bClean)
            {
                x1=x2=x;
                y1=y2=y;
                bClean=false;
            }
            else
            {
                if(x<x1) x1=x;
                if(x>x2) x2=x;
                if(y<y1) y1=y;
                if(y>y2) y2=y;
            }
        }
        public bool TestPoint(float x, float y)
        {
            if(x>=x1 && x<x2 && y>=y1 && y<y2)
                return true;

            return false;
        }

        public bool Intersect(BoundingRectangle rect)
        {
            if(Math.Abs(x1 + x2 - rect.x1 - rect.x2) < (x2 - x1 + rect.x2 - rect.x1))
            if (Math.Abs(y1 + y2 - rect.y1 - rect.y2) < (y2 - y1 + rect.y2 - rect.y1))
                return true;

            return false;
        }

    }
}
