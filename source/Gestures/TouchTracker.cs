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

using System;
using System.Collections.Generic;
using System.Linq;
using Sungiant.Abacus;using Sungiant.Abacus.Packed;using Sungiant.Abacus.SinglePrecision;
using Sungiant.Cor;

namespace Sungiant.Blimey
{

	public class TouchTracker
	{
		const Int32 NumFramesPerTrackedTouch = 15;

		Int32 trackCounter = -1;
		Int32 id;
		List<Touch> samples = new List<Touch>();
		IScreenSpecification screenSpec;
		IPanelSpecification panelSpec;
		ICor engine;

		internal TouchTracker(
			ICor engine,
			IScreenSpecification displayMode,
			IPanelSpecification panelMode,
			Int32 id )
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

		internal Int32 TouchID { get { return this.id; } }

		internal TouchPhase Phase { get { return samples.Last().Phase; } }

		Vector2 GetPositionOfSampleAtIndex(int index, TouchPositionSpace space)
		{
			var pos = this.samples[index].Position;

			var multiplier = Vector2.One;
			switch (space)
			{
				case TouchPositionSpace.RealWorld:

					if(engine.System.CurrentOrientation == DeviceOrientation.Default ||
					   engine.System.CurrentOrientation == DeviceOrientation.Upsidedown)
					{
						multiplier = new Vector2(panelSpec.PanelPhysicalSize.X, panelSpec.PanelPhysicalSize.Y);
					}
					else
					{
						multiplier = new Vector2(panelSpec.PanelPhysicalSize.Y, panelSpec.PanelPhysicalSize.X);
					}

					break;

				case TouchPositionSpace.Screen:

					if (this.engine.System.CurrentOrientation == DeviceOrientation.Upsidedown )
					{
						pos.Y = - pos.Y;
						pos.X = - pos.X;
					}
					else if (this.engine.System.CurrentOrientation == DeviceOrientation.Leftside )
					{
						Single temp = pos.X;
						pos.X = -pos.Y;
						pos.Y = temp;
					}
					else if(this.engine.System.CurrentOrientation == DeviceOrientation.Rightside )
					{
						Single temp = pos.X;
						pos.X = pos.Y;
						pos.Y = -temp;
					}

					Int32 w = this.engine.Graphics.DisplayStatus.CurrentWidth;
					Int32 h = this.engine.Graphics.DisplayStatus.CurrentHeight;

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
