using System;
using Cor;
using Platform.Linux;
using Platform;

namespace Cor.Demo
{
	public class Program
	{
		public static void Main( string[] args )
		{
			var appSettings = new AppSettings ("Cor") {
				FullScreen = true,
				MouseGeneratesTouches = true
			};

			var entryPoint = new BasicApp();

			MonoLinuxPlatform platform = new MonoLinuxPlatform ();

			#if LOGGED_PROXY
			memory = new FileStream (Path.Combine (Environment.GetEnvironmentVariable ("HOME"), "log.txt"), FileMode.Create);
			writer = new StreamWriter (memory);
			using( var engine = new Engine(new LoggedProxyPlatform (platform, writer), appSettings, entryPoint))
			{
				platform.run();
			}
			#else
			using( var engine = new Engine(
				platform,
				appSettings,
				entryPoint) )
			{
				platform.Run();
			}
			#endif
		}
	}
}

