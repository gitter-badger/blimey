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
using Sungiant.Cor;

namespace Sungiant.Blimey
{
	internal class LoggingHelper
	{
		ISystemManager sys;

		internal LoggingHelper(ISystemManager sys)
		{
			this.sys = sys;
			Teletype.OpenChannel("Blimey");
			Teletype.OpenChannel("Blimey.Engine");
			Teletype.OpenChannel("Blimey.Input");
			Teletype.OpenChannel("Blimey.Graphics");
			Teletype.OpenChannel("Blimey.Resources");
			Teletype.OpenChannel("Blimey.Resources");
		}

		internal void Heading()
		{
			Teletype.WriteLine("Blimey", " #### #   # #   #    #### #####  ###  #   # #####");
			Teletype.WriteLine("Blimey", "#     #   # ##  #   #       #   #   # ##  #   #  ");
			Teletype.WriteLine("Blimey", "##### #   # # # #   #  ##   #   ##### # # #   #  ");
			Teletype.WriteLine("Blimey", "    # #   # #  ##   #   #   #   #   # #  ##   #  ");
			Teletype.WriteLine("Blimey", "####   ###  #   #    ###  ##### #   # #   #   #  ");
		}

		internal void SystemDetails()
		{
			Teletype.WriteLine("Blimey.Engine", "Operating System: " + sys.OperatingSystem);
			Teletype.WriteLine("Blimey.Engine", "Device Name: " + sys.DeviceName);
			Teletype.WriteLine("Blimey.Engine", "Device Model: " + sys.DeviceModel);
			Teletype.WriteLine("Blimey.Engine", "System Name: " + sys.SystemName);
			Teletype.WriteLine("Blimey.Engine", "System Version: " + sys.SystemVersion);
			Teletype.WriteLine(
				"Blimey.Engine",
				"Screen Spec: w: " + 
				sys.ScreenSpecification.ScreenResolutionWidth +
				" h: " + 
				sys.ScreenSpecification.ScreenResolutionHeight
				);

			Teletype.WriteLine("Blimey.Engine", "System Version: " + sys.SystemVersion);
		}
	}
}

