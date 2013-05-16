using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
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

namespace WpfTestScrean
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isDrawLine;
        //BitmapSource screanshot;
        //Point StartPosition;
        //Point EndPosition;
        Image ImageGrafics;
        C2PScreenshotDrawing m_screenshotDrawing;
        CroppingAdorner _clp;
        //FrameworkElement _felCur = null;
        //Brush _brOriginal;


        public MainWindow()
        {
            InitializeComponent();
            isDrawLine = false;
            ImageGrafics = new Image();
            ImageForm.Width = SystemParameters.VirtualScreenWidth;
            ImageForm.Height = SystemParameters.VirtualScreenHeight;
            m_screenshotDrawing = new C2PScreenshotDrawing(ImageForm);
            m_screenshotDrawing.isElips = true;
        }


        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            /*
            if (e.Key == System.Windows.Input.Key.F3)
            {
                m_screenshotDrawing.CreateScreaneshot(new System.Windows.Rect(0, 0, (int)System.Windows.SystemParameters.PrimaryScreenWidth, (int)System.Windows.SystemParameters.PrimaryScreenHeight));
                this.WindowState = System.Windows.WindowState.Maximized;
                this.WindowStyle = System.Windows.WindowStyle.None;
                
                
            }
            if (e.Key == System.Windows.Input.Key.F4)
            {
                this.WindowState = System.Windows.WindowState.Normal;
                this.WindowStyle = System.Windows.WindowStyle.SingleBorderWindow;
                
            }
            
            if (e.Key == System.Windows.Input.Key.F5)
            {
                Image img = new Image();
                DrawingVisual m_drawingVisual = new DrawingVisual();
                DrawingContext m_drawingContext;


                Brush circleBrush = new SolidColorBrush(Color.FromArgb(10, 0, 255, 0));
                Brush brushRectangle = new SolidColorBrush(Color.FromArgb(128, 255, 0, 0));
                Brush brush = new SolidColorBrush(Colors.Blue);

                Pen circlePen = new Pen(circleBrush, 1.0d);
                Pen penDash = new Pen(brush, 1.0d);
                Pen penCursor = new Pen(brush, 3.0d);
                Pen penRectangle = new Pen(brushRectangle, 2.0d);


                m_drawingContext = m_drawingVisual.RenderOpen();
                m_drawingContext.DrawRectangle(brush, null, new Rect(0, 0, 350, 350));
                m_drawingContext.DrawRectangle(brushRectangle, null, new Rect(10, 10, 250, 250));
                m_drawingContext.DrawRectangle(circleBrush, null, new Rect(20, 20, 130, 130));

                m_drawingContext.Close();
                RenderTargetBitmap bmp = new RenderTargetBitmap((int)SystemParameters.VirtualScreenWidth, (int)SystemParameters.VirtualScreenHeight, 96, 96, PixelFormats.Pbgra32);
                bmp.Render(m_drawingVisual);

                img.BeginInit(); //вроде это даже и не нужно
                img.Source = bmp;
                img.EndInit();   //как и это
                ImageForm.Source = img.Source;
                GC.Collect();
                GC.WaitForPendingFinalizers();

            }
            if (e.Key == System.Windows.Input.Key.F6)
            {
                /*
                Line myLine = new Line();
                myLine.Stretch = Stretch.Fill;
                myLine.Stroke = System.Windows.Media.Brushes.LightSteelBlue;
                myLine.X1 = 1;
                myLine.X2 = 50;
                myLine.Y1 = 1;
                myLine.Y2 = 50;
                myLine.HorizontalAlignment = HorizontalAlignment.Left;
                myLine.VerticalAlignment = VerticalAlignment.Center;
                myLine.StrokeThickness = 5;
                grid1.Children.Add(myLine);
                

            }
             
            if (e.Key == System.Windows.Input.Key.F7)
            {
                // Create a StackPanel to contain the shape.
                StackPanel myStackPanel = new StackPanel();

                // Create a red Ellipse.
                Ellipse myEllipse = new Ellipse();

                // Create a SolidColorBrush with a red color to fill the 
                // Ellipse with.
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();

                // Describes the brush's color using RGB values. 
                // Each value has a range of 0-255.
                mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
                myEllipse.Fill = mySolidColorBrush;
                myEllipse.StrokeThickness = 2;
                myEllipse.Stroke = Brushes.Black;

                // Set the width and height of the Ellipse.
                myEllipse.Width = 200;
                myEllipse.Height = 100;

                // Add the Ellipse to the StackPanel.
                myStackPanel.Children.Add(myEllipse);

                this.Content = myStackPanel;
            }
             
            if (e.Key == System.Windows.Input.Key.F4)
            {
                Rect rcInterior = new Rect(
                ImageForm.ActualWidth * 0.2,
                ImageForm.ActualHeight * 0.2,
                ImageForm.ActualWidth * 0.6,
                ImageForm.ActualHeight * 0.6);

                AdornerLayer aly = AdornerLayer.GetAdornerLayer(ImageForm);

                TestAdorner clp = new TestAdorner(ImageForm);//ImageForm, rcInterior);

                aly.Add(clp);

                ImageForm.Source = clp.BpsCrop();
                //clp.CropChanged += CropChanged;


            }
             */
            if (e.Key == System.Windows.Input.Key.F3)
            {
                Rect rcInterior = new Rect(
                ImageForm.Width * 0.2,
                ImageForm.Height * 0.2,
                ImageForm.Width * 0.6,
                ImageForm.Height * 0.6);

                AdornerLayer aly = AdornerLayer.GetAdornerLayer(ImageForm);

                CroppingAdorner _clp = new CroppingAdorner(ImageForm, rcInterior);

                aly.Add(_clp);

                ImageForm.Source = _clp.BpsCrop();
                _clp.CropChanged += CropChanged;
                SetClipColorGrey();
            }

            

        }

        private void CropChanged(Object sender, RoutedEventArgs rea)
        {
            RefreshCropImage();
        }

        private void RefreshCropImage()
        {
            if (_clp != null)
            {
                Rect rc = _clp.ClippingRectangle;

                
                ImageForm.Source = _clp.BpsCrop();
            }
        }

        private void SetClipColorGrey()
        {
            if (_clp != null)
            {
                Color clr = Colors.Black;
                clr.A = 110;
                _clp.Fill = new SolidColorBrush(clr);
            }
        }


        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ImageForm.Width = this.Width;
            ImageForm.Height = this.Height;
        }


    }
}
