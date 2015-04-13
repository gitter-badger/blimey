// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ __________.__  .__                                                     │ \\
// │ \______   \  | |__| _____   ____ ___.__.                               │ \\
// │  |    |  _/  | |  |/     \_/ __ <   |  |                               │ \\
// │  |    |   \  |_|  |  Y Y  \  ___/\___  |                               │ \\
// │  |______  /____/__|__|_|  /\___  > ____|                               │ \\
// │         \/              \/     \/\/                                    │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2012 - 2015 ~ Blimey Engine (http://www.blimey.io)         │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Authors:                                                               │ \\
// │ ~ Ash Pook (http://www.ajpook.com)                                     │ \\
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


namespace Blimey.Asset
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using ServiceStack;
    using ServiceStack.Text;
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
    using Oats;
    using NDesk.Options;

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
        static ProjectDefinition projectDefinition;

        static OptionSet OptionSet;

        const string Version = "0.1.1";

        static Int32 LogLevel = 0;
        static Boolean ShowHelp = false;
        static String BuildDirectory = null;
        static String TargetPlatform = null;
        static List<String> AdditionalDlls = new List<String> ();

        static Program ()
        {
            OptionSet = new OptionSet()
            {
                { "q|quiet=", x => LogLevel-- },
                { "v|verbose=", x => LogLevel++ },
                { "h|?|help", x => ShowHelp = x != null },
                { "d|directory=", x => BuildDirectory = x },
                { "pt|platform=", x => TargetPlatform = x },
                { "a|assembly=", x => AdditionalDlls.Add (x)}
            };
        }

        static void PrintHelp()
        {
            Console.WriteLine (string.Empty);
        }

        public static void Main (string[] args)
        {
            Console.WriteLine ("Blimey Asset Builder v" + Version);

            String homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                   Environment.OSVersion.Platform == PlatformID.MacOSX)
                        ? Environment.GetEnvironmentVariable("HOME")
                        : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

            String installPath = Path.Combine (homePath, ".bab.installation");

            try
            {
                Console.WriteLine ("");

                var installedVersion =
                    File.ReadAllText (installPath)
                        .FromJson<InstallInfo> ()
                        .InstallDateTime;

                Console.WriteLine ("BAB build version: " + installedVersion);

                Console.WriteLine ("");
            }
            catch (Exception ex)
            {
                Console.WriteLine ("No installation found.");
            }

            OptionSet.Parse (args);

            if (ShowHelp)
            {
                PrintHelp ();
                return;
            }

            Console.WriteLine ("LogLevel:" + LogLevel);
            Console.WriteLine ("TargetPlatform:" + TargetPlatform);
            Console.WriteLine ("AdditionalDlls:");
            var pipelineAssemblies = AdditionalDlls.Select (x => {
                var assemblyPath = Path.GetFullPath (x);
                Console.WriteLine (" + " + assemblyPath);
                var SampleAssembly = Assembly.LoadFrom (assemblyPath);
                var types = SampleAssembly.GetTypes();
                Console.WriteLine ("Num Types: " + types.Count ());
                return SampleAssembly;
            }).ToList ();


            if (BuildDirectory != null)
            {
                Console.WriteLine ("Setting working dir to: " + BuildDirectory);

                if (BuildDirectory.Contains ("~/"))
                {
                    BuildDirectory = BuildDirectory.Replace ("~/", "");
                    BuildDirectory = Path.Combine (
                        Environment.GetEnvironmentVariable ("HOME"),
                        BuildDirectory
                    );
                }

                Directory.SetCurrentDirectory (BuildDirectory);
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
                    .FromJson<ProjectDefinition> ();

            Console.WriteLine ("\tResources Folder: " + projectDefinition.ResourcesFolder);
            Console.WriteLine ("\tAsset Definitions Folder: " + projectDefinition.AssetDefinitionsFolder);
            Console.WriteLine ("\tDestination Folder: " + projectDefinition.DestinationFolder);
            Console.WriteLine ("");

            string[] assetDefinitionFiles = Directory
				.GetFiles (projectDefinition.AssetDefinitionsFolder)
				.Where (x => !Path.GetFileName(x).StartsWith("."))
				.ToArray ();

            var assetDefinitions = assetDefinitionFiles
                .Select (file => file.ReadAllText ().FromJson<AssetDefinition> ())
                .ToList ();

            var platformIds = assetDefinitions
                .SelectMany (x => x.SourceSets)
                .SelectMany (x => x.Platforms)
                .Distinct ()
                .ToList ();

            Console.WriteLine ("Target Platforms:");
            platformIds.ForEach (x => Console.WriteLine ("\t" + x));
            Console.WriteLine ("");

            if (TargetPlatform != null)
                platformIds = new List<string> { TargetPlatform };

            foreach (var platformId in platformIds)
            {
                var pId = platformId;
                var platformAssetDeinitions = assetDefinitions
                    .Where (x => x.HasSourceSetForPlatform (pId))
                    .ToList ();

                ProcessAssetsForPlatform (pId, platformAssetDeinitions);
            }
		}

        static void ProcessAssetsForPlatform (
            String platformId,
            List<AssetDefinition> assetDefinitions)
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

