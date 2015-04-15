/*
Decoder for Targa (.TGA) images.
Supports pretty much the full Targa specification (all bit
depths, etc).  At the very least, it decodes all TGA images that
I've found in the wild.  If you find one that it fails to decode,
let me know!
Copyright 2013 by Dmitry Brant.
You may use this source code in your application(s) free of charge,
as long as attribution is given to me (Dmitry Brant) and my URL
(http://dmitrybrant.com) in your application's "about" box and/or
documentation. Of course, donations are always welcome:
http://dmitrybrant.com/donate
If you would like to use this source code without attribution, please
contact me through http://dmitrybrant.com, or visit this page:
http://dmitrybrant.com/noattributionlicense
-----------------------------------------------------------
Full License Agreement for this source code module:
"Author" herein shall refer to Dmitry Brant. "Software" shall refer
to this source code module.
This software is supplied to you by the Author in consideration of
your agreement to the following terms, and your use, installation,
modification or redistribution of this software constitutes acceptance
of these terms. If you do not agree with these terms, please do not use,
install, modify or redistribute this software.
In consideration of your agreement to abide by the following terms,
and subject to these terms, the Author grants you a personal,
non-exclusive license, to use, reproduce, modify and redistribute
the software, with or without modifications, in source and/or binary
forms; provided that if you redistribute the software in its entirety
and without modifications, you must retain this notice and the following
text and disclaimers in all such redistributions of the software, and
that in all cases attribution of the Author as the original author
of the source code shall be included in all such resulting software
products or distributions. Neither the name, trademarks, service marks
or logos of the Author may be used to endorse or promote products
derived from the software without specific prior written permission
from the Author. Except as expressly stated in this notice, no other
rights or licenses, express or implied, are granted by the Author
herein, including but not limited to any patent rights that may be
infringed by your derivative works or by other works in which the 
oftware may be incorporated.
The software is provided by the Author on an "AS IS" basis. THE AUTHOR
MAKES NO WARRANTIES, EXPRESS OR IMPLIED, INCLUDING WITHOUT
LIMITATION THE IMPLIED WARRANTIES OF NON-INFRINGEMENT, MERCHANTABILITY
AND FITNESS FOR A PARTICULAR PURPOSE, REGARDING THE SOFTWARE OR ITS USE
AND OPERATION ALONE OR IN COMBINATION WITH YOUR PRODUCTS.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, INDIRECT,
INCIDENTAL OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
PROFITS; OR BUSINESS INTERRUPTION) ARISING IN ANY WAY OUT OF THE USE,
REPRODUCTION, MODIFICATION AND/OR DISTRIBUTION OF THE SOFTWARE, HOWEVER
CAUSED AND WHETHER UNDER THEORY OF CONTRACT, TORT (INCLUDING NEGLIGENCE),
STRICT LIABILITY OR OTHERWISE, EVEN IF THE AUTHOR HAS BEEN ADVISED
OF THE POSSIBILITY OF SUCH DAMAGE.
*/

#pragma warning disable 219


using System;
using System.IO;

namespace DmitryBrant.ImageFormats
{
    /// <summary>
    /// Handles reading Targa (.TGA) images.
    /// </summary>
    public static class TgaReader
    {
        /// <summary>
        /// Reads a Targa (.TGA) image from a zStream.
        /// </summary>
        /// <param name="zStream">Stream from which to read the image.</param>
        /// <returns>Bitmap that contains the image that was read.</returns>
        public static void Load (Stream zStream, out Int32 zWidth, out Int32 zHeight, out Byte[] zData)
        {
            BinaryReader reader = new BinaryReader(zStream);

            UInt32[] palette = null;
            byte[] scanline = null;

            byte idFieldLength = (byte)zStream.ReadByte();
            byte colorMap = (byte)zStream.ReadByte();
            byte imageType = (byte)zStream.ReadByte();
            UInt16 colorMapOffset = LittleEndian(reader.ReadUInt16());
            UInt16 colorsUsed = LittleEndian(reader.ReadUInt16());
            byte bitsPerColorMap = (byte)zStream.ReadByte();
            UInt16 xCoord = LittleEndian(reader.ReadUInt16());
            UInt16 yCoord = LittleEndian(reader.ReadUInt16());
            UInt16 imgWidth = LittleEndian(reader.ReadUInt16());
            UInt16 imgHeight = LittleEndian(reader.ReadUInt16());
            byte bitsPerPixel = (byte)zStream.ReadByte();
            byte imgFlags = (byte)zStream.ReadByte();

            if (colorMap > 1)
                throw new ApplicationException("This is not a valid TGA file.");

            if (idFieldLength > 0)
            {
                byte[] idBytes = new byte[idFieldLength];
                zStream.Read(idBytes, 0, idFieldLength);
                string idStr = System.Text.Encoding.ASCII.GetString(idBytes);

                //do something with the ID string...
                System.Diagnostics.Debug.WriteLine("Targa image ID: " + idStr);
            }

            //image types:
            //0 - No Image Data Available
            //1 - Uncompressed Color Image
            //2 - Uncompressed RGB Image
            //3 - Uncompressed Black & White Image
            //9 - Compressed Color Image
            //10 - Compressed RGB Image
            //11 - Compressed Black & White Image

            if ((imageType > 11) || ((imageType > 3) && (imageType < 9)))
            {
                throw new ApplicationException("This image type (" + imageType + ") is not supported.");
            }
            else if (bitsPerPixel != 8 && bitsPerPixel != 15 && bitsPerPixel != 16 && bitsPerPixel != 24 && bitsPerPixel != 32)
            {
                throw new ApplicationException("Number of bits per pixel (" + bitsPerPixel + ") is not supported.");
            }
            if (colorMap > 0)
            {
                if (bitsPerColorMap != 15 && bitsPerColorMap != 16 && bitsPerColorMap != 24 && bitsPerColorMap != 32)
                {
                    throw new ApplicationException("Number of bits per color map (" + bitsPerPixel + ") is not supported.");
                }
            }

            byte[] data = new byte[imgWidth * 4 * imgHeight];

            try
            {

                if (colorMap > 0)
                {
                    int paletteEntries = colorMapOffset + colorsUsed;
                    palette = new UInt32[paletteEntries];

                    if (bitsPerColorMap == 24)
                    {
                        for (int i = colorMapOffset; i < paletteEntries; i++)
                        {
                            palette[i] = 0xFF000000;
                            palette[i] |= (UInt32)(zStream.ReadByte() << 16);
                            palette[i] |= (UInt32)(zStream.ReadByte() << 8);
                            palette[i] |= (UInt32)(zStream.ReadByte());
                        }
                    }
                    else if (bitsPerColorMap == 32)
                    {
                        for (int i = colorMapOffset; i < paletteEntries; i++)
                        {
                            palette[i] = 0xFF000000;
                            palette[i] |= (UInt32)(zStream.ReadByte() << 16);
                            palette[i] |= (UInt32)(zStream.ReadByte() << 8);
                            palette[i] |= (UInt32)(zStream.ReadByte());
                            palette[i] |= (UInt32)(zStream.ReadByte() << 24);
                        }
                    }
                    else if ((bitsPerColorMap == 15) || (bitsPerColorMap == 16))
                    {
                        int hi, lo;
                        for (int i = colorMapOffset; i < paletteEntries; i++)
                        {
                            hi = zStream.ReadByte();
                            lo = zStream.ReadByte();
                            palette[i] = 0xFF000000;
                            palette[i] |= (UInt32)((hi & 0x1F) << 3) << 16;
                            palette[i] |= (UInt32)((((lo & 0x3) << 3) + ((hi & 0xE0) >> 5)) << 3) << 8;
                            palette[i] |= (UInt32)(((lo & 0x7F) >> 2) << 3);
                        }
                    }
                }

                if (imageType == 1 || imageType == 2 || imageType == 3)
                {
                    scanline = new byte[imgWidth * (bitsPerPixel / 8)];
                    for (int y = imgHeight - 1; y >= 0; y--)
                    {
                        switch (bitsPerPixel)
                        {
                            case 8:
                                zStream.Read(scanline, 0, scanline.Length);
                                if (imageType == 1)
                                {
                                    for (int x = 0; x < imgWidth; x++)
                                    {
                                        data[4 * (y * imgWidth + x)] = (byte)((palette[scanline[x]] >> 16) & 0XFF);
                                        data[4 * (y * imgWidth + x) + 1] = (byte)((palette[scanline[x]] >> 8) & 0XFF);
                                        data[4 * (y * imgWidth + x) + 2] = (byte)((palette[scanline[x]]) & 0XFF);
                                        data[4 * (y * imgWidth + x) + 3] = 0xFF;
                                    }
                                }
                                else if (imageType == 3)
                                {
                                    for (int x = 0; x < imgWidth; x++)
                                    {
                                        data[4 * (y * imgWidth + x)] = scanline[x];
                                        data[4 * (y * imgWidth + x) + 1] = scanline[x];
                                        data[4 * (y * imgWidth + x) + 2] = scanline[x];
                                        data[4 * (y * imgWidth + x) + 3] = 0xFF;
                                    }
                                }
                                break;
                            case 15:
                            case 16:
                                int hi, lo;
                                for (int x = 0; x < imgWidth; x++)
                                {
                                    hi = zStream.ReadByte();
                                    lo = zStream.ReadByte();

                                    data[4 * (y * imgWidth + x)] = (byte)((hi & 0x1F) << 3);
                                    data[4 * (y * imgWidth + x) + 1] = (byte)((((lo & 0x3) << 3) + ((hi & 0xE0) >> 5)) << 3);
                                    data[4 * (y * imgWidth + x) + 2] = (byte)(((lo & 0x7F) >> 2) << 3);
                                    data[4 * (y * imgWidth + x) + 3] = 0xFF;
                                }
                                break;
                            case 24:
                                zStream.Read(scanline, 0, scanline.Length);
                                for (int x = 0; x < imgWidth; x++)
                                {
                                    data[4 * (y * imgWidth + x)] = scanline[x * 3];
                                    data[4 * (y * imgWidth + x) + 1] = scanline[x * 3 + 1];
                                    data[4 * (y * imgWidth + x) + 2] = scanline[x * 3 + 2];
                                    data[4 * (y * imgWidth + x) + 3] = 0xFF;
                                }
                                break;
                            case 32:
                                zStream.Read(scanline, 0, scanline.Length);
                                for (int x = 0; x < imgWidth; x++)
                                {
                                    data[4 * (y * imgWidth + x)] = scanline[x * 4];
                                    data[4 * (y * imgWidth + x) + 1] = scanline[x * 4 + 1];
                                    data[4 * (y * imgWidth + x) + 2] = scanline[x * 4 + 2];
                                    data[4 * (y * imgWidth + x) + 3] = 0xFF; // scanline[x * 4 + 3];
                                }
                                break;
                        }
                    }

                }
                else if (imageType == 9 || imageType == 10 || imageType == 11)
                {
                    int y = imgHeight - 1, x = 0, i;
                    int bytesPerPixel = bitsPerPixel / 8;
                    scanline = new byte[imgWidth * 4];

                    while (y >= 0 && zStream.Position < zStream.Length)
                    {
                        i = zStream.ReadByte();
                        if (i < 128)
                        {
                            i++;
                            switch (bitsPerPixel)
                            {
                                case 8:
                                    zStream.Read(scanline, 0, i * bytesPerPixel);
                                    if (imageType == 9)
                                    {
                                        for (int j = 0; j < i; j++)
                                        {
                                            data[4 * (y * imgWidth + x)] = (byte)((palette[scanline[j]] >> 16) & 0XFF);
                                            data[4 * (y * imgWidth + x) + 1] = (byte)((palette[scanline[j]] >> 8) & 0XFF);
                                            data[4 * (y * imgWidth + x) + 2] = (byte)((palette[scanline[j]]) & 0XFF);
                                            data[4 * (y * imgWidth + x) + 3] = 0xFF;
                                            x++;
                                            if (x >= imgWidth) { x = 0; y--; }
                                        }
                                    }
                                    else if (imageType == 11)
                                    {
                                        for (int j = 0; j < i; j++)
                                        {
                                            data[4 * (y * imgWidth + x)] = scanline[j];
                                            data[4 * (y * imgWidth + x) + 1] = scanline[j];
                                            data[4 * (y * imgWidth + x) + 2] = scanline[j];
                                            data[4 * (y * imgWidth + x) + 3] = 0xFF;
                                            x++;
                                            if (x >= imgWidth) { x = 0; y--; }
                                        }
                                    }
                                    break;
                                case 15:
                                case 16:
                                    int hi, lo;
                                    for (int j = 0; j < i; j++)
                                    {
                                        hi = zStream.ReadByte();
                                        lo = zStream.ReadByte();

                                        data[4 * (y * imgWidth + x)] = (byte)((hi & 0x1F) << 3);
                                        data[4 * (y * imgWidth + x) + 1] = (byte)((((lo & 0x3) << 3) + ((hi & 0xE0) >> 5)) << 3);
                                        data[4 * (y * imgWidth + x) + 2] = (byte)(((lo & 0x7F) >> 2) << 3);
                                        data[4 * (y * imgWidth + x) + 3] = 0xFF;
                                        x++;
                                        if (x >= imgWidth) { x = 0; y--; }
                                    }
                                    break;
                                case 24:
                                    zStream.Read(scanline, 0, i * bytesPerPixel);
                                    for (int j = 0; j < i; j++)
                                    {
                                        data[4 * (y * imgWidth + x)] = scanline[j * 3];
                                        data[4 * (y * imgWidth + x) + 1] = scanline[j * 3 + 1];
                                        data[4 * (y * imgWidth + x) + 2] = scanline[j * 3 + 2];
                                        data[4 * (y * imgWidth + x) + 3] = 0xFF;
                                        x++;
                                        if (x >= imgWidth) { x = 0; y--; }
                                    }
                                    break;
                                case 32:
                                    zStream.Read(scanline, 0, i * bytesPerPixel);
                                    for (int j = 0; j < i; j++)
                                    {
                                        data[4 * (y * imgWidth + x)] = scanline[j * 4];
                                        data[4 * (y * imgWidth + x) + 1] = scanline[j * 4 + 1];
                                        data[4 * (y * imgWidth + x) + 2] = scanline[j * 4 + 2];
                                        data[4 * (y * imgWidth + x) + 3] = 0xFF; // scanline[j * 4 + 3];
                                        x++;
                                        if (x >= imgWidth) { x = 0; y--; }
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            i = (i & 0x7F) + 1;
                            int r, g, b, a;

                            switch (bitsPerPixel)
                            {
                                case 8:
                                    int p = zStream.ReadByte();
                                    if (imageType == 9)
                                    {
                                        for (int j = 0; j < i; j++)
                                        {
                                            data[4 * (y * imgWidth + x)] = (byte)((palette[p] >> 16) & 0XFF);
                                            data[4 * (y * imgWidth + x) + 1] = (byte)((palette[p] >> 8) & 0XFF);
                                            data[4 * (y * imgWidth + x) + 2] = (byte)((palette[p]) & 0XFF);
                                            data[4 * (y * imgWidth + x) + 3] = 0xFF;
                                            x++;
                                            if (x >= imgWidth) { x = 0; y--; }
                                        }
                                    }
                                    else if (imageType == 11)
                                    {
                                        for (int j = 0; j < i; j++)
                                        {
                                            data[4 * (y * imgWidth + x)] = (byte)p;
                                            data[4 * (y * imgWidth + x) + 1] = (byte)p;
                                            data[4 * (y * imgWidth + x) + 2] = (byte)p;
                                            data[4 * (y * imgWidth + x) + 3] = 0xFF;
                                            x++;
                                            if (x >= imgWidth) { x = 0; y--; }
                                        }
                                    }
                                    break;
                                case 15:
                                case 16:
                                    int hi = zStream.ReadByte();
                                    int lo = zStream.ReadByte();
                                    for (int j = 0; j < i; j++)
                                    {
                                        data[4 * (y * imgWidth + x)] = (byte)((hi & 0x1F) << 3);
                                        data[4 * (y * imgWidth + x) + 1] = (byte)((((lo & 0x3) << 3) + ((hi & 0xE0) >> 5)) << 3);
                                        data[4 * (y * imgWidth + x) + 2] = (byte)(((lo & 0x7F) >> 2) << 3);
                                        data[4 * (y * imgWidth + x) + 3] = 0xFF;
                                        x++;
                                        if (x >= imgWidth) { x = 0; y--; }
                                    }
                                    break;
                                case 24:
                                    r = zStream.ReadByte();
                                    g = zStream.ReadByte();
                                    b = zStream.ReadByte();
                                    for (int j = 0; j < i; j++)
                                    {
                                        data[4 * (y * imgWidth + x)] = (byte)r;
                                        data[4 * (y * imgWidth + x) + 1] = (byte)g;
                                        data[4 * (y * imgWidth + x) + 2] = (byte)b;
                                        data[4 * (y * imgWidth + x) + 3] = 0xFF;
                                        x++;
                                        if (x >= imgWidth) { x = 0; y--; }
                                    }
                                    break;
                                case 32:
                                    r = zStream.ReadByte();
                                    g = zStream.ReadByte();
                                    b = zStream.ReadByte();
                                    a = zStream.ReadByte();
                                    for (int j = 0; j < i; j++)
                                    {
                                        data[4 * (y * imgWidth + x)] = (byte)r;
                                        data[4 * (y * imgWidth + x) + 1] = (byte)g;
                                        data[4 * (y * imgWidth + x) + 2] = (byte)b;
                                        data[4 * (y * imgWidth + x) + 3] = 0xFF; // (byte)a;
                                        x++;
                                        if (x >= imgWidth) { x = 0; y--; }
                                    }
                                    break;
                            }
                        }

                    }
                }

            }
            catch (Exception e)
            {
                //give a partial image in case of unexpected end-of-file
                System.Diagnostics.Debug.WriteLine("Error while processing TGA file: " + e.Message);
            }

            bool flipX = false;
            bool flipY = false;
            int imgOrientation = (imgFlags >> 4) & 0x3;

            if (imgOrientation == 1) flipX = true;
            else if (imgOrientation == 2) flipY = true;
            else if (imgOrientation == 3)
            {
                flipX = true;
                flipY = true;
            }

            UInt32 W_LIM = imgWidth;
            UInt32 H_LIM = imgHeight;

            if (flipX && flipY) W_LIM /= 2;
            else if (flipX) W_LIM /= 2;
            else if (flipY) H_LIM /= 2;
            
            if (flipX || flipY) // copy in place
            {
                for (UInt32 sourceX = 0; sourceX < W_LIM; ++sourceX)
                {
                    UInt32 targetX = flipX ? imgWidth - sourceX - 1 : sourceX;

                    for (UInt32 sourceY = 0; sourceY < H_LIM; ++sourceY)
                    {
                        UInt32 sourceI = ((sourceX + (sourceY * imgWidth)) * 4);

                        UInt32 targetY = flipY ? imgHeight - sourceY - 1 : sourceY;
                        UInt32 targetI = ((targetX + (targetY * imgHeight)) * 4);

                        // Copy
                        Byte sourceB = data[sourceI + 0];
                        Byte sourceG = data[sourceI + 1];
                        Byte sourceR = data[sourceI + 2];
                        Byte sourceA = data[sourceI + 3];

                        // Overwrite
                        data[sourceI + 0] = data[targetI + 0];
                        data[sourceI + 1] = data[targetI + 1];
                        data[sourceI + 2] = data[targetI + 2];
                        data[sourceI + 3] = data[targetI + 3];

                        // Paste
                        data[targetI + 0] = sourceB;
                        data[targetI + 1] = sourceG;
                        data[targetI + 2] = sourceR;
                        data[targetI + 3] = sourceA;

                    }
                }
            }

            // assign output
            zWidth = (int)imgWidth;
            zHeight = (int)imgHeight;
            zData = data;

            return;
        }


        static UInt16 LittleEndian(UInt16 val)
        {
            if (BitConverter.IsLittleEndian) return val;
            return conv_endian(val);
        }

        static UInt32 LittleEndian(UInt32 val)
        {
            if (BitConverter.IsLittleEndian) return val;
            return conv_endian(val);
        }

        static UInt16 conv_endian(UInt16 val)
        {
            UInt16 temp;
            temp = (UInt16)(val << 8); temp &= 0xFF00; temp |= (UInt16)((val >> 8) & 0xFF);
            return temp;
        }
        static UInt32 conv_endian(UInt32 val)
        {
            UInt32 temp = (val & 0x000000FF) << 24;
            temp |= (val & 0x0000FF00) << 8;
            temp |= (val & 0x00FF0000) >> 8;
            temp |= (val & 0xFF000000) >> 24;
            return (temp);
        }
    }
}