namespace EngineDemo
{
    using System;
    using Blimey.Platform;
    using Blimey.Engine;

	public class Program
	{
		public static void Main( string[] args )
		{
			var appSettings = new AppSettings ("Blimey Engine Demo") {
				FullScreen = true,
				MouseGeneratesTouches = true
			};

			var entryPoint = new Demo();

			IApi api = new Api ();

            using (var platform = new Platform (api))
            {
                platform.Start (appSettings, entryPoint);

                (api as Api).Run ();

                platform.Stop ();
            }
		}
	}
}

