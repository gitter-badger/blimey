using System;
using Sungiant.Abacus;

namespace Sungiant.Blimey.PsmRuntime
{

	public class GpuUtils
		: IGpuUtils
	{		
		public Int32 BeginEvent(Colour colour, String eventName) { return 0; }
		public Int32 EndEvent() { return 0; }

		public void SetMarker (Colour colour, String eventName){ }
		public void SetRegion (Colour colour, String eventName){ }
	}
	
}