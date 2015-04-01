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

    using Fudge;
    using Abacus.SinglePrecision;

    using System.Linq;

    // ────────────────────────────────────────────────────────────────────────────────────────────────────────────── //

    public class TouchTracker
    {
        const Int32 NumFramesPerTrackedTouch = 15;

        Int32 trackCounter = -1;
        string id;
        List<Touch> samples = new List<Touch>();
        ScreenSpecification screenSpec;
        PanelSpecification panelSpec;
        Engine engine;

        internal TouchTracker(
            Engine engine,
            ScreenSpecification displayMode,
            PanelSpecification panelMode,
            string id )
        {
            this.engine = engine;
            this.screenSpec = displayMode;
            this.panelSpec = panelMode;
            this.id = id;

        }

        internal void RegisterTouch(Touch t)
        {
            if( trackCounter == -1 )
            {
                this.samples.Add(t);
            }
            else
            {
                if( trackCounter % NumFramesPerTrackedTouch == 0 )
                {
                    this.samples.Add(t);
                }
                else
                {
                    this.samples[this.samples.Count -1] = t;
                }
            }


            trackCounter++;

        }

        internal Touch LatestTouch { get { return this.samples.Last(); } }

        internal String TouchID { get { return this.id; } }

        internal TouchPhase Phase { get { return samples.Last().Phase; } }

        Vector2 GetPositionOfSampleAtIndex(int index, TouchPositionSpace space)
        {
            var pos = this.samples[index].Position;

            var multiplier = Vector2.One;
            var pps = panelSpec.PanelPhysicalSize.GetValueOrDefault (new Vector2 (0.16f, 0.09f));
            switch (space)
            {
                case TouchPositionSpace.RealWorld:

                    if(engine.Host.CurrentOrientation == DeviceOrientation.Default ||
                       engine.Host.CurrentOrientation == DeviceOrientation.Upsidedown)
                    {
                        multiplier = new Vector2(pps.X, pps.Y);
                    }
                    else
                    {
                        multiplier = new Vector2(pps.Y, pps.X);
                    }

                    break;

                case TouchPositionSpace.Screen:

                    if (this.engine.Host.CurrentOrientation == DeviceOrientation.Upsidedown )
                    {
                        pos.Y = - pos.Y;
                        pos.X = - pos.X;
                    }
                    else if (this.engine.Host.CurrentOrientation == DeviceOrientation.Leftside )
                    {
                        Single temp = pos.X;
                        pos.X = -pos.Y;
                        pos.Y = temp;
                    }
                    else if(this.engine.Host.CurrentOrientation == DeviceOrientation.Rightside )
                    {
                        Single temp = pos.X;
                        pos.X = pos.Y;
                        pos.Y = -temp;
                    }

                    Int32 w = this.engine.Status.Width;
                    Int32 h = this.engine.Status.Height;

                    //this.engine.System.GetEffectiveDisplaySize(ref w, ref h);

                    multiplier = new Vector2(w, h);

                    break;

            }
            pos *= multiplier;

            return pos;
        }

        public Vector2 GetPosition(TouchPositionSpace space)
        {
            int numSamples = samples.Count;

            var curPos = this.GetPositionOfSampleAtIndex(numSamples - 1, space);



            return curPos;
        }

        public Vector2 GetVelocity(TouchPositionSpace space)
        {
            int numSamples = samples.Count;

            if (numSamples > 1)
            {
                var currentTouch = this.samples[numSamples - 1];
                var previousTouch = this.samples[numSamples - 2];

                var currentPos = this.GetPositionOfSampleAtIndex(numSamples - 1, space);
                var previousPos = this.GetPositionOfSampleAtIndex(numSamples - 2, space);

                Single dt = currentTouch.Timestamp - previousTouch.Timestamp;

                return (currentPos - previousPos) / dt;
            }



            return Vector2.Zero;
        }

        public Single GetDistanceTraveled(TouchPositionSpace posType)
        {
            Single distance = 0f;

            for (Int32 i = 0; i < samples.Count; ++i)
            {
                if (i > 0)
                {
                    var currentPosition = this.GetPositionOfSampleAtIndex(i, posType);
                    var previousPosition = this.GetPositionOfSampleAtIndex(i - 1, posType);

                    Single mag = (currentPosition - previousPosition).Length();

                    distance += mag;
                }
            }

            return distance;
        }
    }
}