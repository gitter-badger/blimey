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
// │ Copyright © 2014 A.J.Pook (http://ajpook.github.io)                    │ \\
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
using Abacus;
using Fudge;
using Abacus.SinglePrecision;
using Cor;
using System.Collections.Generic;

namespace Blimey.Demo
{
    public class ColourChanger
        : Trait
    {
        Rgba32 _current;
        Rgba32 _target;

        Single _colourChangeTime;

        Single _timer;

        MeshRenderer _renderer;

        public ColourChanger()
        {
            _current = RandomGenerator.Default.GetRandomColour();
            _target = RandomGenerator.Default.GetRandomColour();
            _colourChangeTime = RandomGenerator.Default.GetRandomSingle(3f, 10f);
            _timer = _colourChangeTime;
        }

        public override void OnAwake()
        {
            _renderer = this.Parent.GetTrait<MeshRenderer>();
        }

        public override void OnUpdate(AppTime time)
        {
            _timer -= time.Delta;

            if( _timer < 0f )
            {
                _timer = _colourChangeTime - _timer;
                _current = _target;
                _target = RandomGenerator.Default.GetRandomColour();
            }

            Single lerpVal = 1f - _timer;
            if(lerpVal < 0f) lerpVal = 0f;

            Rgba32 colToSet = Rgba32.Lerp(_current, _target, lerpVal);

            _renderer.Material.SetColour("MaterialColour", colToSet);
        }
    }
}

