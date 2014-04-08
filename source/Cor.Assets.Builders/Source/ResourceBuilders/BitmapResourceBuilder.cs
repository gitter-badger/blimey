using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Cor
{
    public class BitmapResourceBuilder
        : ResourceBuilder <BitmapResource>
    {
        public override String [] SupportedSourceFileExtensions
        {
            get { return new [] { "png", "jpg", "bmp" }; }
        }

        public override ResourceBuilderOutput <BitmapResource> Import (ResourceBuilderInput input)
        {
            var outputResource = new BitmapResource ();
            
            if (input.Files.Count != 1)
                throw new Exception ("BitmapResourceBuilder only supports one input file.");
            
            if (!File.Exists (input.Files[0]))
                throw new Exception ("BitmapResourceBuilder cannot find input file.");
            
            var bmp = new Bitmap (input.Files[0]);
            
            outputResource.Bitmap = bmp;
            
            var width = outputResource.Bitmap.Width;
            var height = outputResource.Bitmap.Height;

            // Force the input's pixelformat to ARGB32, so we can have a
            // common pixel format to deal with.
            if (outputResource.Bitmap.PixelFormat != PixelFormat.Format32bppArgb)
            {
                var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);

                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.DrawImage(outputResource.Bitmap, 0,0, width, height);
                }

                outputResource.Bitmap = bitmap;
            }

            var output = new ResourceBuilderOutput <BitmapResource> ();
            output.Resource = outputResource;
            return output;
        }

    }
}
