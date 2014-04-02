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

            Console.WriteLine ("Cor Content Builder");

            Console.WriteLine (Environment.GetEnvironmentVariable ("DYLD_FALLBACK_LIBRARY_PATH"));

            try
            {
                Console.WriteLine ("AssImp Version: " + aiGetVersionMajor () + "." + aiGetVersionMinor ());
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.GetType () + " : " + ex.Message);
            }

            Console.WriteLine ("");

            Console.WriteLine(
                File.ReadAllText (
                    Path.Combine(
                        Environment.GetEnvironmentVariable ("HOME"), ".cba.installation")));

            Console.Write ("Looking for .cba file... ");

            if (File.Exists (".cba"))
            {
                Console.WriteLine ("OK");
            }
            else
            {
                Console.WriteLine ("Failed to find .cba file.");
                return;
            }

			Console.Write ("\n\n\n");

		}
	}
}

