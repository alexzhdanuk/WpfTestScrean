using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace WpfTestScrean
{
    class C2PImageByteOperations
    {

        public C2PImageByteOperations()
        { 
        
        }

        public static bool isPointInElips(Point pointFromPicture, Point centerElips, double radiusA, double radiusB)
        {
            bool isInArea = false;
            if ((pointFromPicture.X - centerElips.X) * (pointFromPicture.X - centerElips.X) / (radiusA * radiusA)
                + (pointFromPicture.Y - centerElips.Y) * (pointFromPicture.Y - centerElips.Y) / (radiusB * radiusB) <= 1)
            {
                isInArea = true;
            }
            return isInArea;
        }


        public static ImageBrush CreateOpasityMaskForSelect(BitmapSource bitmap, Point StartPosition, Point EndPosition)
        {
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int stride = ((width * 32 + 31) & ~31) / 8;
            byte[] pixels = new byte[width * height * 4];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pixels[4 * (width * y + x) + 0] = 0;
                    pixels[4 * (width * y + x) + 1] = 0;
                    pixels[4 * (width * y + x) + 2] = 0;

                    if ((x > StartPosition.X && x < EndPosition.X) && (y > StartPosition.Y && y < EndPosition.Y))
                    {
                        pixels[4 * (width * y + x) + 3] = 0;
                    }
                    else
                    {
                        pixels[4 * (width * y + x) + 3] = 77;
                    }
                }
            }

            BitmapSource image = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, pixels, stride);

            ImageBrush Mask = new ImageBrush();
            Mask.ImageSource = image;

            return Mask;
        }

        

        public static ImageBrush CreateOpasityMaskForElips(BitmapSource bitmap, Point StartPosition, Point EndPosition, Point centerElips)
        {
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int stride = ((width * 32 + 31) & ~31) / 8;
            byte[] pixels = new byte[width * height * 4];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    pixels[4 * (width * y + x) + 0] = 0;
                    pixels[4 * (width * y + x) + 1] = 0;
                    pixels[4 * (width * y + x) + 2] = 0;


                    if (isPointInElips(new Point(x, y), centerElips, (EndPosition.X - StartPosition.X) / 2, (EndPosition.Y - StartPosition.Y) / 2))
                    {
                        pixels[4 * (width * y + x) + 3] = 0;
                    }
                    else
                    {
                        pixels[4 * (width * y + x) + 3] = 77;
                    }
                }
            }

            BitmapSource image = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, pixels, stride);

            ImageBrush Mask = new ImageBrush();
            Mask.ImageSource = image;

            return Mask;
        }



        public static ImageSource CutImageFromImage(Rect rectangle, BitmapSource bitmapSource, bool isElips)
        {
            int width = bitmapSource.PixelWidth;
            int height = bitmapSource.PixelHeight;
            int stride = (((int)rectangle.Width * 32 + 31) & ~31) / 8;


            Byte[] screen = null;
            byte[] newImage = new byte[(int)(rectangle.Height * rectangle.Width * 4)];


            screen = BufferFromImage(BitmapSourceToBitmapImage(bitmapSource));

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if ((x >= rectangle.X && x < (rectangle.X + rectangle.Width)) && (y >= rectangle.Y && y < (rectangle.Y + rectangle.Height)))
                    {
                        int X = x - (int)rectangle.X;
                        int Y = y - (int)rectangle.Y;

                        newImage[4 * ((int)rectangle.Width * Y + X) + 0] = screen[4 * (width * (height - y) + x) + 0];
                        newImage[4 * ((int)rectangle.Width * Y + X) + 1] = screen[4 * (width * (height - y) + x) + 1];
                        newImage[4 * ((int)rectangle.Width * Y + X) + 2] = screen[4 * (width * (height - y) + x) + 2];

                        if (isElips && !isPointInElips(new Point(x, y), new Point(rectangle.X + rectangle.Width / 2, rectangle.Y + rectangle.Height / 2), rectangle.Width / 2, rectangle.Height / 2))
                        {
                            newImage[4 * ((int)rectangle.Width * Y + X) + 3] = 0;
                        }
                        else
                        {
                            newImage[4 * ((int)rectangle.Width * Y + X) + 3] = screen[4 * (width * (height - y) + x) + 3];
                        }
                    }
                }
            }

            BitmapSource bitmap = BitmapSource.Create((int)(rectangle.Width), (int)(rectangle.Height), 96, 96, bitmapSource.Format, null, newImage, stride);

            return bitmap;
        }

        public static Byte[] BufferFromImage(BitmapSource imageSource)
        {

            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            //encoder.Palette = imageSource.Palette;

            encoder.Frames.Add(BitmapFrame.Create(imageSource));
            //encoder.QualityLevel = 100;
            byte[] bit = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                //encoder.Frames.Add(BitmapFrame.Create(imageSource));
                encoder.Save(stream);
                bit = stream.ToArray();
                stream.Close();
            }

            return bit;

        }


        public static Byte[] BufferFromImage(BitmapImage imageSource)
        {
            Stream stream = imageSource.StreamSource;
            Byte[] buffer = null;
            if (stream != null && stream.Length > 0)
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    buffer = br.ReadBytes((Int32)stream.Length);
                }
            }

            return buffer;
        }

        private double Cos(double angel)
        {
            double  q, s = 0;
            q = angel;
            for (int i = 1; i <= 100; i++)
            {
                s += q;
                q *= (-1) * angel * angel / (2 * i - 1) / (2 * i);
            }
            return s;
        }



        public static BitmapImage BitmapSourceToBitmapImage(BitmapSource source)
        {
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            MemoryStream memoryStream = new MemoryStream();
            BitmapImage bImg = new BitmapImage();
            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(memoryStream);

            bImg.BeginInit();
            bImg.StreamSource = new MemoryStream(memoryStream.ToArray());
            bImg.EndInit();

            memoryStream.Close();

            return bImg;

        }


    }
}
