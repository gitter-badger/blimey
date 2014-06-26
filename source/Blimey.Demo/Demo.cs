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
	public class Demo
		: global::Blimey.App
	{
        public Demo()
			: base (new MainMenuScene())
        {
        }
		
        public override void Start (ICor cor)
		{
			base.Start (cor);
		}

        public override Boolean Update (ICor cor, AppTime time)
		{
			return base.Update (cor, time);
		}

        public override void Render (ICor cor)
		{
			base.Render (cor);
		}

        public override void Stop (ICor cor)
		{
			base.Stop (cor);
		}
	}
}

