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

			using( var engine = new Engine(platform, appSettings, entryPoint) )
			{
				platform.Run();
			}
		}
	}
}

