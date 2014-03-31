using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Cor.ContentBuilder
{
	public class Program
	{
		[DllImport("libassimp")]
		static extern int aiGetVersionMajor ();

		[DllImport("libassimp")]
		static extern int aiGetVersionMinor ();

		public static void Main ()
		{
			Console.WriteLine(Environment.GetEnvironmentVariable("DYLD_FALLBACK_LIBRARY_PATH"));

			try
			{
				Console.WriteLine("AssImp Version: " + aiGetVersionMajor() + "." + aiGetVersionMinor());
			}
			catch(Exception ex)
			{
				Console.WriteLine (ex.GetType() + " : " + ex.Message);
			}
		}
	}
}

