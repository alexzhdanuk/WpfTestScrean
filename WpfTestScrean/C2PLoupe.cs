using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Xml.Linq;
using System.Windows;

namespace WpfTestScrean
{
    class C2PLoupe
    {
        
        public C2PLoupe()
        {
            
        }

        public void RenderLoup(Rect rectangle,BitmapSource screan,DrawingContext drawingContext)
        {
            Brush brushBackground = new SolidColorBrush(Colors.Black);
            Pen circlePen = new Pen(brushBackground, 1.0d);
            BitmapSource bitmap = CreateLoupeImage(rectangle,screan);
            drawingContext.DrawImage(bitmap, new Rect(rectangle.X + 58, rectangle.Y + 58, 110, 110));
            drawingContext.DrawEllipse(null, circlePen, new Point(rectangle.X + 113, rectangle.Y + 113), 55, 55);

            drawingContext.DrawLine(circlePen, new Point(rectangle.X + 92, rectangle.Y + 113), new Point(rectangle.X + 134, rectangle.Y + 113));
            drawingContext.DrawLine(circlePen, new Point(rectangle.X + 113, rectangle.Y + 92), new Point(rectangle.X + 113, rectangle.Y + 134));
        }

        private BitmapSource CreateLoupeImage(Rect rectangle, BitmapSource screan)
        {
            int width = screan.PixelWidth;
            int height = screan.PixelHeight;
            int stride = (((int)rectangle.Width * 32 + 31) & ~31) / 8;

            Byte[] screen = null;
            byte[] newImage = new byte[(int)(rectangle.Height * rectangle.Width * 4)];

            screen = C2PImageByteOperations.BufferFromImage(C2PImageByteOperations.BitmapSourceToBitmapImage(screan));

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
                        if (C2PDrawingMath.DistanceFromPoints(new Point(X, Y), new Point(rectangle.Width / 2, rectangle.Height / 2)) < rectangle.Width / 2)
                        {
                            newImage[4 * ((int)rectangle.Width * Y + X) + 3] = screen[4 * (width * (height - y) + x) + 3];
                        }
                        else
                        {
                            newImage[4 * ((int)rectangle.Width * Y + X) + 3] = 0;
                        }
                    }
                }
            }

            BitmapSource bitmap = BitmapSource.Create((int)(rectangle.Width), (int)(rectangle.Height), 96, 96, screan.Format, null, newImage, stride);

            return bitmap;
        }
        
    }
}
