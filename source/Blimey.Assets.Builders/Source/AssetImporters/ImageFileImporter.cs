using System;
#if OSX
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
#endif
using System.Drawing;
using System.Runtime.InteropServices;
using Fudge;

#if WINDOWS
using System.Drawing;
using System.Drawing.Imaging;
#endif

using System.IO;

using Cor;
using Blimey.Assets.Pipeline;
using Hjg.Pngcs;

namespace Blimey.Assets.Builders
{
    public class ImageFileImporter
        : AssetImporter <ColourmapAsset>
    {
        public override String [] SupportedSourceFileExtensions
        {
            get { return new [] { "png" }; }
        }

		public override AssetImporterOutput <ColourmapAsset> Import (
			AssetImporterInput input, String platformId)
        {
            var output = new AssetImporterOutput <ColourmapAsset> ();
            var outputResource = new ColourmapAsset ();
            output.OutputAsset = outputResource;

            if (input.Files.Count != 1)
                throw new Exception ("ColourmapAssetImporter only supports one input file.");

            if (!File.Exists (input.Files[0]))
                throw new Exception ("ColourmapAssetImporter cannot find input file.");
            
            string filename = input.Files[0];

            PngReader pngr = FileHelper.CreatePngReader(filename);
            Console.WriteLine(pngr.ToString()); // just information
            var pixmap = new Rgba32[pngr.ImgInfo.Cols, pngr.ImgInfo.Rows];

            for (int row = 0; row < pngr.ImgInfo.Rows; row++)
            {
                ImageLine line = pngr.ReadRowInt(row); // format: RGBRGB... or RGBARGBA...

                for (int col = 0; col < pngr.ImgInfo.Cols;col++)
                {
                    if (pngr.ImgInfo.Indexed)
                        throw new NotSupportedException ("Indexed PNG files are not yet supported.");

                    if (pngr.ImgInfo.Channels != 4)
                        throw new NotSupportedException ("Only PNG files with 4 channels are currently supported.");

                    Int32 pixelARGB = ImageLineHelper.GetPixelToARGB8 (line, col);

                    Byte b = (Byte)((pixelARGB) & 0xff);
                    Byte g = (Byte)((pixelARGB >> 8) & 0xff);
                    Byte r = (Byte)((pixelARGB >> 16) & 0xff);
                    Byte a = (Byte)((pixelARGB >> 24) & 0xff);

                    pixmap [col, row] = new Rgba32(r, g, b, a);
                }
            }

            outputResource.Data = pixmap;

            //Debug.DumpToPPM (output.Resource, input.Files[0] + "test.ppm");

            return output;
        }

    }
}
