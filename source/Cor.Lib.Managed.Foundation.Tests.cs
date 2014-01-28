// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Foundation.Tests                                                       │ \\
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

using System.Diagnostics;
using System.Threading;
using NUnit.Framework;

//
// StopwatchTest.cs
//
// Author:
//  Atsushi Enomoto  <atsushi@ximian.com>
//
// Copyright (C) 2006 Novell, Inc.
//
namespace MonoTests.System.Diagnostics
{
    [TestFixture]
    public class StopwatchTest
    {
        [Test]
        public void TestSimple ()
        {
            // It starts at started state.
            Stopwatch sw = Stopwatch.StartNew ();
            Thread.Sleep (1000);
            sw.Stop ();
            long ticks = sw.ElapsedTicks;
            Assert.IsTrue (sw.ElapsedMilliseconds > 100, "#1");
            Thread.Sleep (1000);
            // do not increment resuts
            Assert.AreEqual (ticks, sw.ElapsedTicks, "#2");
            sw.Start ();
            Thread.Sleep (1000);
            // increment results
            Assert.IsTrue (sw.ElapsedTicks > ticks, "#3");
            ticks = sw.ElapsedTicks;
            sw.Stop ();
            Assert.IsTrue (sw.ElapsedTicks >= ticks, "#4");
            sw.Reset ();
            Assert.AreEqual (0, sw.ElapsedTicks, "#5");
            sw.Start ();
            Thread.Sleep (1000);
            Assert.IsTrue (sw.ElapsedTicks > 100, "#5");
            // This test is not strict but would mostly work.
            Assert.IsTrue (ticks > sw.ElapsedTicks, "#6");
            sw.Stop ();
        }
    }
}
