using Cor.Platform.Psm;

namespace Cor.Demo
{
	public class AppMain
	{
		public static void Main (string[] args)
		{
            var appSettings = new AppSettings("Cor Demo")
            {
                FullScreen = true,
                MouseGeneratesTouches = true
            };

            var entryPoint = new BasicApp();

            using (var engine = new Engine(new PsmPlatform(), appSettings, entryPoint)){}
		}
	}
}