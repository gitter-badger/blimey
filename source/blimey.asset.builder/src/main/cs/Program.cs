using System;
using System.IO;
using System.Runtime.InteropServices;
using ServiceStack.Text;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Blimey.Assets.Builders;
using Blimey.Assets.Pipeline;
using Oats;
using Blimey;

namespace Blimey.AssetBuilder
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
        static Configuration.ProjectDefinition projectDefinition;

        static string currentPlatform;

        public static void Main (string[] args)
        {
            Console.WriteLine ("Blimey Asset Builder");
            Console.WriteLine ("====================");

            Console.WriteLine ("");

            AssImp.AssImp.PrintVersion ();

            Console.WriteLine (
                "BAB build version: " +
                File.ReadAllText (
                    Path.Combine (
                        Environment.GetEnvironmentVariable ("HOME"),
						".bab.installation"))
                .FromJson<Configuration.InstallInfo> ()
                .InstallDateTime);

            Console.WriteLine ("");



            if (args.Length > 0)
            {
                currentPlatform = args [0];
            }

            if (args.Length > 1)
            {
                Console.WriteLine ("Setting working dir to: " + args [0]);

                if (args [1].Contains ("~/"))
                {
                    args [1] = args [1].Replace ("~/", "");
                    args [1] = Path.Combine (
                        Environment.GetEnvironmentVariable ("HOME"),
                        args [1]
                    );
                }

                Directory.SetCurrentDirectory (args [1]);
                Console.WriteLine ("");
                Console.WriteLine (Directory.GetCurrentDirectory () + " $");
            }
            Console.WriteLine ("");
            Console.Write ("Looking for project definition .bab file... ");

            if (File.Exists (".bab"))
            {
                Console.WriteLine ("OK");
            }
            else
            {
                Console.WriteLine ("Failed to find .bab file.");
                return;
            }

            projectDefinition =
                File.ReadAllText (".bab")
                    .FromJson<Configuration.ProjectDefinition> ();

            Console.WriteLine ("\tResources Folder: " + projectDefinition.ResourcesFolder);
            Console.WriteLine ("\tAsset Definitions Folder: " + projectDefinition.AssetDefinitionsFolder);
            Console.WriteLine ("\tDestination Folder: " + projectDefinition.DestinationFolder);
            Console.WriteLine ("");

            string[] assetDefinitionFiles = Directory
				.GetFiles (projectDefinition.AssetDefinitionsFolder)
				.Where (x => !Path.GetFileName(x).StartsWith("."))
				.ToArray ();

            var assetDefinitions = assetDefinitionFiles
                .Select (
                    file =>
                    file.ReadAllText ()
                        .FromJson<Configuration.AssetDefinition> ())
                .ToList ();

            var platformIds = assetDefinitions
                .SelectMany (x => x.SourceSets)
                .SelectMany (x => x.Platforms)
                .Distinct ()
                .ToList ();

            Console.WriteLine ("Target Platforms:");
            platformIds.ForEach (x => Console.WriteLine ("\t" + x));
            Console.WriteLine ("");

            if (currentPlatform != null)
                platformIds = new List<string> { currentPlatform };

            Builders.Init ();

            foreach (var platformId in platformIds)
            {
                var pId = platformId;
                var platformAssetDeinitions = assetDefinitions
                    .Where (
                                                  x =>
                        x.HasSourceSetForPlatform (pId))
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
                var sourceset = assetDefinition.GetSourceForPlatform (platformId);

                var sourcefiles = sourceset.Importer.Files
					.Where (x => x != null)
                    .Select (x => Path.Combine (projectDefinition.ResourcesFolder, x))
                    .ToList ();

                sourcefiles.ForEach (x => Console.WriteLine ("\t+ " + x));

				String destFolder = 
					Path.Combine (
	                    projectDefinition.DestinationFolder,
	                    platformId);

				if (!Directory.Exists (destFolder))
					Directory.CreateDirectory (destFolder);

                String assetfile =
                    Path.Combine (
                        projectDefinition.DestinationFolder,
                        platformId,
						assetDefinition.AssetId + ".bba");

                Console.WriteLine ("\t= " + assetfile);

                try
                {
                    Type assetImporterType = Type.GetType (sourceset.Importer.AssetImporterType + ",.dll");
                    
                    if (assetImporterType == null)
                    {
                        throw new Exception ("Failed to find Asset Importer Type: " + sourceset.Importer.AssetImporterType);
                    }
                    
					IAsset ass = ImportAsset (assetImporterType, sourcefiles, sourceset.Importer.AssetImporterSettings, platformId);
                    
                    foreach (var processor in sourceset.Processors)
                    {
                        Type assetProcessorType = Type.GetType (processor.AssetProcessorType);
						ass = ProcessAsset (assetProcessorType, ass, processor.AssetProcessorSettings, platformId);
                    }
                    
                    WriteAsset (ass, assetfile);
                }
                catch (Exception ex)
                {
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine ("\t\t! FAILED TO BUILD: " + ex.GetType () + " - " + ex.Message.Replace (Environment.NewLine, "  "));

                    string st = ex.StackTrace;
                    string [] sta = st.Split (new String []{Environment.NewLine}, StringSplitOptions.None);
                    foreach (var x in sta)
                    {
                        Console.WriteLine ("\t\t!\t"+x);
                    }
					Console.ResetColor ();
                }

                Console.WriteLine ("");
            }
        }

        static IAsset ImportAsset (
            Type assetImporterType,
            List<String> sourceFiles,
			Dictionary<String, Object> settings,
			String platformId)
        {
			Console.WriteLine ("\t\tabout to import resource with " + assetImporterType);

            var assetImporter = Activator.CreateInstance (assetImporterType) as AssetImporter;

            var assetImporterInput = new AssetImporterInput ();
            assetImporterInput.Files = sourceFiles;
            assetImporterInput.AssetImporterSettings = new AssetImporterSettings ();
            assetImporterInput.AssetImporterSettings.Settings = settings;
			var output = assetImporter.BaseImport (assetImporterInput, platformId);

            var resourceType = assetImporterType.BaseType ().GenericTypeArguments ()[0];


            Console.WriteLine ("\t\tresource type = " + resourceType);

            return output.OutputAsset;
        }

        static IAsset ProcessAsset (
            Type assetProcessorType,
            IAsset inputAsset,
			Dictionary<String, Object> settings,
			String platformId)
        {
			Console.WriteLine ("\t\tabout to process asset with " + assetProcessorType);

            var assetProcessor = Activator.CreateInstance (assetProcessorType) as AssetProcessor;

            var resourceType = assetProcessorType.BaseType ().GenericTypeArguments ()[0];
            var assetType = assetProcessorType.BaseType ().GenericTypeArguments ()[1];
            Console.WriteLine ("\t\tasset type = " + assetType);

            var abiType = typeof (AssetProcessorInput<>);

            Type genericAbiType = abiType.MakeGenericType(resourceType);
            var assetProcessorInputObject =  Activator.CreateInstance(genericAbiType);

            var assetProcessorInput = assetProcessorInputObject as AssetProcessorInput;

            assetProcessorInput.InputAsset = inputAsset;
            assetProcessorInput.AssetProcessorSettings = new AssetProcessorSettings ();
            assetProcessorInput.AssetProcessorSettings.Settings = settings;

			var output = assetProcessor.BaseProcess (assetProcessorInput, platformId);

            return output.OutputAsset;
        }
        
		static void WriteFileHeader (ISerialisationChannel sc)
        {
			// file type
			sc.Write <Byte> ((Byte)'C');
			sc.Write <Byte> ((Byte)'B');
			sc.Write <Byte> ((Byte)'A');
            
			// file version
			sc.Write <Byte> ((Byte)0);
            
			// platform index
			sc.Write <Byte> ((Byte)0);
            
            // total filesize
            // ? why does xna have this ?
        }
			
        static void WriteAsset (IAsset a, String destination)
        {
            Console.WriteLine ("\t\tabout to write asset to " + destination);
            
            using (var stream = new FileStream (destination, FileMode.OpenOrCreate))
            {
				using (var sc = new SerialisationChannel
					<BinaryStreamSerialiser> 
						(stream, ChannelMode.Write))
                {
                    // Cor Binary Asset File Header
                    //------------------------------------------------------------------------------------------------//
					WriteFileHeader (sc);
                    
					// Now write the object
                    //------------------------------------------------------------------------------------------------//
					Type assetType = a.GetType ();
					sc.WriteReflective (assetType, a);

					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine ("\t\tBUILT " + destination);
					Console.ResetColor ();
                }
            }
        }
	}
}
