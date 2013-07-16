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
using Sungiant.Cor;

namespace Sungiant.Blimey
{
	internal class PotentialFlickGesture
		: PotentialGesture
	{
		const float velocityRequired = 0.05f;
		const float displacementRequired = 0.01f;

		internal PotentialFlickGesture(
			InputEventSystem inputEventSystem,
			Int32[] touchIDs)
			: base(inputEventSystem, GestureType.Flick, touchIDs)
		{

		}

		internal override Gesture Update(
			float dt,
			List<TouchTracker> touchTrackers)
		{
			var touchTracker = inputEventSystem.GetTouchTracker(touchIDs[0]);

			if (touchTracker == null)
			{
				failedGesture = true;
				return null;
			}

			var velocity = touchTracker.GetVelocity(TouchPositionSpace.RealWorld).Length();


			float distanceTravelled = touchTracker.GetDistanceTraveled(TouchPositionSpace.RealWorld);

			if (velocity >= velocityRequired &&
				distanceTravelled >= displacementRequired &&
				touchTracker.Phase == TouchPhase.JustReleased )
			{
				completedGesture = true;
				return new Gesture(this.inputEventSystem, this.type, this.touchIDs);
			}

			if( touchTracker.Phase == TouchPhase.JustReleased )
			{
				failedGesture = true;
			}

			return null;
		}
	}


}

