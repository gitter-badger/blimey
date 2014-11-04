using Cor;
using Cor.Platform;
using Cor.Platform.Xna4;

namespace Cor.Demo
{
    static class Program
    {
        static void Main(string[] args)
        {
            var appSettings = new AppSettings("Cor Demo")
            {
                FullScreen = true,
                MouseGeneratesTouches = true
            };

            var entryPoint = new BasicApp();

            using (var engine = new Engine(new Xna4Platform(), appSettings, entryPoint)){}
        }
    }
}
