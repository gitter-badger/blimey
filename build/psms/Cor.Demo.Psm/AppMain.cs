namespace Cor.Demo.Psm
{
	public class AppMain
	{
		public static void Main (string[] args)
		{
			var sceneSettings = new GameSceneSettings();
			var scene1 = new MainMenuScene(sceneSettings);
			var settings = new EngineSettings();

			using (var engine = new Cor.Psm.Engine(
				scene1, settings))
            {
                engine.Run();
            }
		}
	}
}
