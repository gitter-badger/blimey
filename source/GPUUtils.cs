
#if TARGET_WINDOWS && DEBUG

using System;
using System.Runtime.InteropServices;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using Sungiant.Cor;

namespace Sungiant.Cor.Xna4Runtime
{

	public class GpuUtils
		: IGpuUtils
	{

		[DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
		static extern int D3DPERF_BeginEvent (uint col, String wszName);

		[DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi)]
		static extern int D3DPERF_EndEvent ();

		[DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
		static extern int D3DPERF_SetMarker (uint col, String wszName);

		[DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Auto)]
		static extern int D3DPERF_SetRegion (uint col, String wszName);

		[DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi)]
		static extern int D3DPERF_QueryRepeatFrame ();

		[DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi)]
		static extern void D3DPERF_SetOptions (uint dwOptions);

		[DllImport("d3d9.dll", CallingConvention = CallingConvention.Winapi)]
		static extern uint D3DPERF_GetStatus ();


		int _nestedBegins = 0;

		public int BeginEvent (Rgba colour, String eventName)
		{
			_nestedBegins++;

			return D3DPERF_BeginEvent (colour.PackedValue, eventName);
		}

		public int BeginEvent (String eventName)
		{
			return BeginEvent (Rgba.Black, eventName);
		}

		public int EndEvent ()
		{
			if (_nestedBegins == 0)
				throw new Exception ("BeginEvent must be called prior to a EndEvent call.");

			_nestedBegins--;

			return D3DPERF_EndEvent ();
		}

		public void SetMarker (Rgba colour, String eventName)
		{
			D3DPERF_SetMarker (colour.PackedValue, eventName);
		}

		public void SetRegion (Rgba colour, String eventName)
		{
			D3DPERF_SetRegion (colour.PackedValue, eventName);
		}

	}

}

#else

using System;
using Sungiant.Abacus.Packed;
using Sungiant.Cor;

namespace Sungiant.Cor.Xna4Runtime
{

	public class GpuUtils
		: IGpuUtils
	{
		public Int32 BeginEvent(Rgba32 colour, String eventName) { return 0; }
		public Int32 EndEvent() { return 0; }

        public void SetMarker(Rgba32 colour, String eventName) { }
        public void SetRegion(Rgba32 colour, String eventName) { }
	}
	
}

#endif