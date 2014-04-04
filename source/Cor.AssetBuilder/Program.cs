using System;
using System.IO;
using System.Runtime.InteropServices;
using ServiceStack.Text;
using System.Linq;
using System.Collections.Generic;

namespace Cor.AssetBuilder
{
    public static class DateTimeHelper
    {
        public static DateTime FromUnixTime (Int32 epoch)
        {
            var dateTime = new DateTime (1970, 1, 1, 0, 0, 0, 0);
            return dateTime.AddSeconds (epoch);
        }
        
        public static Int32 ToUnixTime(DateTime time)
        {
            var span = time - new DateTime (1970, 1, 1, 0, 0, 0, 0);
            return (Int32) span.TotalSeconds;
        }
    }
    
	public class Program
	{
		[DllImport("libassimp")]
		static extern int aiGetVersionMajor ();

		[DllImport("libassimp")]
		static extern int aiGetVersionMinor ();
        
        static Configuration.ProjectDefinition projectDefinition;

        public static void Main (string[] args)
        {
            Console.WriteLine ("Cor Asset Builder");
            Console.WriteLine ("=================");
            



            Console.WriteLine ("");

            Console.WriteLine (
                "CAB build version: " +
                File.ReadAllText (
                    Path.Combine (
                        Environment.GetEnvironmentVariable ("HOME"), 
                        ".cba.installation"))
                .FromJson<Configuration.InstallInfo> ()
                .InstallDateTime);

            //Console.WriteLine (Environment.GetEnvironmentVariable ("DYLD_FALLBACK_LIBRARY_PATH"));

            try
            {
                Console.WriteLine (
                    "AssImp Version: " + aiGetVersionMajor () + "." + aiGetVersionMinor ());
            }
            catch (Exception ex)
            {
                Console.WriteLine (ex.GetType () + " : " + ex.Message);
            }
            
            Console.WriteLine ("");
            
            if (args.Length > 0)
            {
                Console.WriteLine ("Setting working dir to: " + args [0]);
                
                if (args [0].Contains ("~/"))
                {
                    args [0] = args [0].Replace ("~/", "");
                    args [0] = Path.Combine (
                        Environment.GetEnvironmentVariable ("HOME"), 
                        args [0]
                    );
                }
                
                Directory.SetCurrentDirectory (args [0]);
                Console.WriteLine ("");
                Console.WriteLine (Directory.GetCurrentDirectory () + " $");
            }
            Console.WriteLine ("");
            Console.Write ("Looking for project definition .cab file... ");

            if (File.Exists (".cab"))
            {
                Console.WriteLine ("OK");
            }
            else
            {
                Console.WriteLine ("Failed to find .cab file.");
                return;
            }
            
            projectDefinition = 
                File.ReadAllText (".cab")
                    .FromJson<Configuration.ProjectDefinition> ();

            Console.WriteLine ("\tResources Folder: " + projectDefinition.ResourcesFolder);
            Console.WriteLine ("\tAsset Definitions Folder: " + projectDefinition.AssetDefinitionsFolder);
            Console.WriteLine ("\tDestination Folder: " + projectDefinition.DestinationFolder);
            Console.WriteLine ("");
            
            string[] assetDefinitionFiles = Directory.GetFiles (projectDefinition.AssetDefinitionsFolder);
            
            var assetDefinitions = assetDefinitionFiles
                .Select (
                    file =>
                    file.ReadAllText ()
                        .FromJson<Configuration.AssetDefinition> ())
                .ToList ();
       
            var platformIds = assetDefinitions
                .SelectMany (x => x.Sources)
                .SelectMany (x => x.Platforms)
                .Distinct ()
                .ToList ();
            
            Console.WriteLine ("Target Platforms:");
            platformIds.ForEach (x => Console.WriteLine ("\t" + x));
            Console.WriteLine ("");
            
            foreach (var platformId in platformIds)
            {
                var pId = platformId;
                var platformAssetDeinitions = assetDefinitions
                    .Where (
                        x =>
                        x.HasSourceForPlatform (pId))
                    .ToList ();
                
                ProcessAssetsForPlatform (pId, platformAssetDeinitions);
            }
		}
        
        static void ProcessAssetsForPlatform (
            String platformId, 
            List<Configuration.AssetDefinition> assetDefinitions)
        {
            Console.WriteLine ("Processing Platform: " + platformId);
            Console.WriteLine ("");
            foreach (var assetDefinition in assetDefinitions)
            {
                var source = assetDefinition.GetSourceForPlatform (platformId);
                
                source.Files.ForEach (
                    x => 
                    Console.WriteLine (
                        "\t+ " + Path.Combine(
                            projectDefinition.ResourcesFolder, 
                            x)));
                
                String assetFile = 
                    Path.Combine (
                        projectDefinition.DestinationFolder, 
                        platformId, 
                        assetDefinition.AssetId + ".cba");
                
                Console.WriteLine ("\t= " + assetFile);
                Console.WriteLine ("");
            }
        }
	}
}

