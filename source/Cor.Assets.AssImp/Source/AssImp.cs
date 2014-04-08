using System;
using System.Runtime.InteropServices;

namespace AssImp
{
    public static class AssImp
    {
        [DllImport("libassimp")]
        static extern int aiGetVersionMajor ();

        [DllImport("libassimp")]
        static extern int aiGetVersionMinor ();
        
        public static void PrintVersion ()
        {
            Console.WriteLine (Environment.GetEnvironmentVariable ("DYLD_FALLBACK_LIBRARY_PATH"));

            try
            {
                Console.WriteLine (
                    "AssImp Version: " + aiGetVersionMajor () + "." + aiGetVersionMinor ());
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.GetType () + " : " + ex.Message);
            }   
        }
    }
}

