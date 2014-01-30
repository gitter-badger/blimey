using Sungiant.Cor.Platform.Managed.Xna4;

namespace Sungiant.Cor.Demo.Xna4Kickstart
{

#if NETFW_WP75

	public class Program
		: Sungiant.Castle.Xna4Runtime.XnaGame
	{
		public Program()
			: base(Demo.GetAppSettings(), Demo.GetEntryPoint())
		{

		}
	}
#else
    static class Program
    {
        static void Main(string[] args)
        {
            using (var engine = new Xna4App(
                Demo.GetAppSettings(),
                Demo.GetEntryPoint()))
            {
                engine.Run();
            }
        }
    }
#endif
}
