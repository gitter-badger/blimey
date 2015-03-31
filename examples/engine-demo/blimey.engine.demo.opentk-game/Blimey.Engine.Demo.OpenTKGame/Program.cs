namespace EngineDemo
{
    using System;
    using Blimey;

	public class Program
	{
		public static void Main( string[] args )
		{
			var appSettings = new AppSettings ("Blimey Engine Demo") {
				FullScreen = true,
				MouseGeneratesTouches = true
			};

			var entryPoint = new Demo();

			var platform = new Platform ();

			using( var engine = new Engine(platform, appSettings, entryPoint) )
			{
				platform.Run();
			}
		}
	}
}

