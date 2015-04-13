namespace PlatformDemo
{
    using System;
    using Blimey.Platform;

	public class Program
	{
		public static void Main (String[] args)
		{
			var appSettings = new AppSettings ("Blimey Platform Demo") {
				FullScreen = true,
				MouseGeneratesTouches = true
			};

			var entryPoint = new BasicApp();

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

