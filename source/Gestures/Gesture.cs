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
using Sungiant.Abacus;using Sungiant.Abacus.Packed;using Sungiant.Abacus.SinglePrecision;

namespace Sungiant.Blimey
{
	public class Gesture
	{
		static int GestureIDAssigner = 0;

		InputEventSystem inputEventSystem;
		Int32 id;
		GestureType type;
		Int32[] touchIDs;

		public Vector2 GetFinishingPosition(TouchPositionSpace space)
		{

			Vector2 averageFinishPos = Vector2.Zero ;
			foreach (Int32 touchID in TouchIDs)
			{
				var tracker = inputEventSystem.GetTouchTracker(touchID);

				var p = tracker.GetPosition(space);

				averageFinishPos += p;
			}

			averageFinishPos /= TouchIDs.Length;

			return averageFinishPos;  
		}

		public Gesture(InputEventSystem inputEventSystem, GestureType type, Int32[] touchIDs)
		{
			this.inputEventSystem = inputEventSystem;
			this.id = GestureIDAssigner;
			GestureIDAssigner++;
			this.type = type;
			this.touchIDs = touchIDs;
		}

		public Int32 ID
		{
			get
			{
				return this.id;
			}
		}

		public GestureType Type
		{
			get
			{
				return this.type;
			}
		}

		public Int32[] TouchIDs
		{
			get
			{
				return this.touchIDs;
			}
		}

		public List<TouchTracker> TouchTrackers
		{
			get
			{
				var tt = new List<TouchTracker>();

				foreach (Int32 touchID in TouchIDs)
				{
					var tracker = inputEventSystem.GetTouchTracker(touchID);
					tt.Add(tracker);
				}

				return tt;
			}
		}
	}
}
