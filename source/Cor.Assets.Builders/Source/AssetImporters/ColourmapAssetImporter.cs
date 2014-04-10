using System;
using MonoMac.AppKit;
using MonoMac.CoreGraphics;
using System.Drawing;
using System.Runtime.InteropServices;
using Abacus.Packed;

#if WINDOWS
using System.Drawing;
using System.Drawing.Imaging;
#endif

using System.IO;

namespace Cor
{
    public class ColourmapAssetImporter
        : AssetImporter <ColourmapAsset>
    {
        public override String [] SupportedSourceFileExtensions
        {
            get { return new [] { "png", "jpg", "bmp" }; }
        }

        #if OSX
        //Store pixel data as an RGBA Bitmap
        static IntPtr RequestImagePixelData (NSImage inImage)
        {
            var imageSize = inImage.Size;

            CGBitmapContext ctxt = CreateRgbaBitmapContext (inImage.CGImage);

            var rect = new RectangleF (0, 0, imageSize.Width, imageSize.Height);

            ctxt.DrawImage (rect, inImage.CGImage);
            var data = ctxt.Data;

            if (ctxt.BitsPerPixel != 32) throw new Exception ();

            return data;
        }

        static CGBitmapContext CreateRgbaBitmapContext (CGImage inImage)
        {
            var pixelsWide = inImage.Width;
            var pixelsHigh = inImage.Height;

            using (var colorSpace = CGColorSpace.CreateDeviceRGB())
            {
                var bitmapBytesPerRow = pixelsWide * 4;
                var bitmapByteCount = bitmapBytesPerRow * pixelsHigh;
                var bitmapData = Marshal.AllocHGlobal (bitmapByteCount);

                if (bitmapData == IntPtr.Zero)
                {
                    throw new Exception ("Memory not allocated.");
                }

                var context = new CGBitmapContext (
                    bitmapData,
                    pixelsWide,
                    pixelsHigh,
                    8,
                    bitmapBytesPerRow,
                    colorSpace,
                    CGImageAlphaInfo.PremultipliedLast);

                if (context == null)
                {
                    throw new Exception ("Context not created");
                }

                return context;
            }
        }
        #endif


        public override AssetImporterOutput <ColourmapAsset> Import (AssetImporterInput input)
        {
            var output = new AssetImporterOutput <ColourmapAsset> ();
            var outputResource = new ColourmapAsset ();
            output.OutputAsset = outputResource;

            if (input.Files.Count != 1)
                throw new Exception ("BitmapResourceBuilder only supports one input file.");

            if (!File.Exists (input.Files[0]))
                throw new Exception ("BitmapResourceBuilder cannot find input file.");


            #if WINDOWS

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


            #elif OSX

            using(var fStream = new FileStream (input.Files[0], FileMode.Open))
            {
                var nsImage = NSImage.FromStream (fStream);
                IntPtr dataPointer = RequestImagePixelData (nsImage);


                int width = (int) nsImage.Size.Width;
                int height = (int) nsImage.Size.Height;

                int bytecount = width * height * 4;

                byte[] pixdatabytes = new byte [bytecount];
                Marshal.Copy(dataPointer, pixdatabytes, 0, bytecount);

                var pixmap = new Rgba32[width, height];

                int offset = 0;

                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        pixmap [x, y] = new Rgba32 ();
                        pixmap [x, y].R = pixdatabytes [offset++];
                        pixmap [x, y].G = pixdatabytes [offset++];
                        pixmap [x, y].B = pixdatabytes [offset++];
                        pixmap [x, y].A = pixdatabytes [offset++];
                    }
                }

                outputResource.Data = pixmap;
            }

            #else
            throw new NotSupportedException ();
            #endif

            //Debug.DumpToPPM (output.Resource, input.Files[0] + "test.ppm");

            return output;
        }

    }
}
