// ┌────────────────────────────────────────────────────────────────────────┐ \\
// │ Cor! - Low Level 3D App Engine                                         │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Brought to you by:                                                     │ \\
// │          _________                    .__               __             │ \\
// │         /   _____/__ __  ____    ____ |__|____    _____/  |_           │ \\
// │         \_____  \|  |  \/    \  / ___\|  \__  \  /    \   __\          │ \\
// │         /        \  |  /   |  \/ /_/  >  |/ __ \|   |  \  |            │ \\
// │        /_______  /____/|___|  /\___  /|__(____  /___|  /__|            │ \\
// │                \/           \//_____/         \/     \/                │ \\
// │                                                                        │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Copyright © 2013 A.J.Pook (http://sungiant.github.com)                 │ \\
// ├────────────────────────────────────────────────────────────────────────┤ \\
// │ Permission is hereby granted, free of charge, to any person obtaining  │ \\
// │ a copy of this software and associated documentation files (the        │ \\
// │ "Software"), to deal in the Software without restriction, including    │ \\
// │ without limitation the rights to use, copy, modify, merge, publish,    │ \\
// │ distribute, sublicense, and/or sellcopies of the Software, and to      │ \\
// │ permit persons to whom the Software is furnished to do so, subject to  │ \\
// │ the following conditions:                                              │ \\
// │                                                                        │ \\
// │ The above copyright notice and this permission notice shall be         │ \\
// │ included in all copies or substantial portions of the Software.        │ \\
// │                                                                        │ \\
// │ THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,        │ \\
// │ EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF     │ \\
// │ MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. │ \\
// │ IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY   │ \\
// │ CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,   │ \\
// │ TORT OR OTHERWISE, ARISING FROM,OUT OF OR IN CONNECTION WITH THE       │ \\
// │ SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.                 │ \\
// └────────────────────────────────────────────────────────────────────────┘ \\

using System;
using Sungiant.Abacus.Packed;
using Sungiant.Abacus.SinglePrecision;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;


namespace Sungiant.Cor.MonoTouchRuntime
{
    internal class OpenGLTextureWrapper
        : Texture2D
        //, IDisposable todo: IResource pattern for destroying stuff
    {
        public int glTextureId {get; private set;}

        UIImage uiImage;

        int pixelsWide;
        int pixelsHigh;  

        internal static OpenGLTextureWrapper CreateFromFile(string path)
        {   
            var uiImage = UIImage.FromFile(path);

			var texture = new OpenGLTextureWrapper(uiImage);

            return texture;

        }

        private OpenGLTextureWrapper(UIImage uiImage)
        {
            this.uiImage = uiImage;
            IntPtr dataPointer = RequestImagePixelData(uiImage);

            CreateTexture2D((int)uiImage.Size.Width, (int)uiImage.Size.Height, dataPointer);
        }


        //Store pixel data as an ARGB Bitmap
        IntPtr RequestImagePixelData (UIImage inImage)
        {
            var imageSize = inImage.Size;
            
            CGBitmapContext ctxt = CreateRgbaBitmapContext (inImage.CGImage);
            
			var rect = new RectangleF (0, 0, imageSize.Width, imageSize.Height);
            
            ctxt.DrawImage (rect, inImage.CGImage);
            var data = ctxt.Data;
            
            return data;
        }

        CGBitmapContext CreateRgbaBitmapContext (CGImage inImage)
        {
            pixelsWide = inImage.Width;
            pixelsHigh = inImage.Height;

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
        


        public override int Width
        {
            get
            {
                return pixelsWide;
            }
        }
		public override int Height
        {
            get
            {
                return pixelsHigh;
            }
        }

        
        void CreateTexture2D(int width, int height, IntPtr pixelDataRgba32)
        {
            int textureId = -1;
            
            
            // this sets the unpack alignment.  which is used when reading pixels
            // in the fragment shader.  when the textue data is uploaded via glTexImage2d,
            // the rows of pixels are assumed to be aligned to the value set for GL_UNPACK_ALIGNMENT.
            // By default, the value is 4, meaning that rows of pixels are assumed to begin
            // on 4-byte boundaries.  this is a global STATE.
            OpenTK.Graphics.ES20.GL.PixelStore(OpenTK.Graphics.ES20.All.UnpackAlignment, 4);
            OpenTKHelper.CheckError();

            // the first sept in the application of texture is to create the
            // texture object.  this is a container object that holds the 
            // texture data.  this function returns a handle to a texture
            // object.
            OpenTK.Graphics.ES20.GL.GenTextures(1, ref textureId);
            OpenTKHelper.CheckError();

            this.glTextureId = textureId;

            
            var textureTarget = OpenTK.Graphics.ES20.All.Texture2D;
            
            
            // we need to bind the texture object so that we can opperate on it.
            OpenTK.Graphics.ES20.GL.BindTexture(textureTarget, textureId);
            OpenTKHelper.CheckError();

            var internalFormat = (OpenTK.Graphics.ES20.All) OpenTK.Graphics.ES20.PixelFormat.Rgba;
            
            
            var textureDataFormat = (OpenTK.Graphics.ES20.All) OpenTK.Graphics.ES20.All.UnsignedByte;
            
            
            
            // now use the bound texture object to load the image data.
            OpenTK.Graphics.ES20.GL.TexImage2D(
                
                // specifies the texture target, either GL_TEXTURE_2D or one of the cubemap face targets.
                textureTarget,
                
                // specifies which mip level to load.  the base level is
                // specified by 0 following by an increasing level for each
                // successive mipmap.
                0,
                
                // internal format for the texture storage, can be:
                // - GL_RGBA
                // - GL_RGB
                // - GL_LUMINANCE_ALPHA
                // - GL_LUMINANCE
                // - GL_ALPHA
                (int) internalFormat,
                
                // the width of the image in pixels
                width,
                
                // the height of the image in pixels
                height,
                
                // boarder - set to zero, only here for compatibility with OpenGL desktop
                0,
                
                // the format of the incoming texture data, in opengl es this 
                // has to be the same as the internal format
                internalFormat,
                
                // the type of the incoming pixel data, can be:
                // - unsigned byte
                // - unsigned short 4444
                // - unsigned short 5551
                // - unsigned short 565
                textureDataFormat, // this refers to each individual channel
                
                
                pixelDataRgba32
                
                );

            OpenTKHelper.CheckError();

            // sets the minification and maginfication filtering modes.  required
            // because we have not loaded a complete mipmap chain for the texture
            // so we must select a non mipmapped minification filter.
            OpenTK.Graphics.ES20.GL.TexParameter(textureTarget, OpenTK.Graphics.ES20.All.TextureMinFilter, (int) OpenTK.Graphics.ES20.All.Nearest );

            OpenTKHelper.CheckError();

            OpenTK.Graphics.ES20.GL.TexParameter(textureTarget, OpenTK.Graphics.ES20.All.TextureMagFilter, (int) OpenTK.Graphics.ES20.All.Nearest );

            OpenTKHelper.CheckError();
        }
        
        
        
        void DeleteTexture(Texture2D texture)
        {
            int textureId = (texture as OpenGLTextureWrapper).glTextureId;
            
            OpenTK.Graphics.ES20.GL.DeleteTextures(1, ref textureId);
        }
    }
}

