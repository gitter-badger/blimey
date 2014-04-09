using System;
using System.IO;

namespace Cor.Assets.Builders
{
    internal static class Debug
    {
        internal static void DumpToPPM (ColourmapResource cmap, string file)
        {
            using (var writer = new StreamWriter (file))
            {
                int w = cmap.Data.GetLength (0);
                int h = cmap.Data.GetLength (1);
                writer.WriteLine ("P3");
                writer.WriteLine (w + " " + h);
                writer.WriteLine ("255");
                
                for (int y = 0; y < h; ++y)
                {
                    for (int x = 0; x < w; ++x)
                    {
                        writer.Write (cmap.Data [x, y].R);
                        writer.Write (" ");
                        writer.Write (cmap.Data [x, y].G);
                        writer.Write (" ");
                        writer.Write (cmap.Data [x, y].B);
                        
                        if (x == w - 1)
                            writer.Write ("\n");
                        else
                            writer.Write ("  ");
                    }
                }
            }
        }
    }
}

