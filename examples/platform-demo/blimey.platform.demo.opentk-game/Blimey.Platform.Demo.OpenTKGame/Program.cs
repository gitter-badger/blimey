namespace PlatformDemo
{
    using System;
    using Blimey;

	public class Program
	{
		public static void Main( string[] args )
		{
			var appSettings = new AppSettings ("Blimey Platform Demo") {
				FullScreen = true,
				MouseGeneratesTouches = true
			};

			var entryPoint = new BasicApp();

			var platform = new Platform ();

			using( var engine = new Engine(platform, appSettings, entryPoint) )
			{
				platform.Run();
			}
		}
	}
}

