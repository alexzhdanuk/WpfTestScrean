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
using System.Xml.Linq;
using System.Globalization;




namespace WpfTestScrean
{
    public enum TernaryRasterOperations : uint
    {
        /// <summary>dest = source</summary>
        SRCCOPY = 0x00CC0020,
        /// <summary>dest = source OR dest</summary>
        SRCPAINT = 0x00EE0086,
        /// <summary>dest = source AND dest</summary>
        SRCAND = 0x008800C6,
        /// <summary>dest = source XOR dest</summary>
        SRCINVERT = 0x00660046,
        /// <summary>dest = source AND (NOT dest)</summary>
        SRCERASE = 0x00440328,
        /// <summary>dest = (NOT source)</summary>
        NOTSRCCOPY = 0x00330008,
        /// <summary>dest = (NOT src) AND (NOT dest)</summary>
        NOTSRCERASE = 0x001100A6,
        /// <summary>dest = (source AND pattern)</summary>
        MERGECOPY = 0x00C000CA,
        /// <summary>dest = (NOT source) OR dest</summary>
        MERGEPAINT = 0x00BB0226,
        /// <summary>dest = pattern</summary>
        PATCOPY = 0x00F00021,
        /// <summary>dest = DPSnoo</summary>
        PATPAINT = 0x00FB0A09,
        /// <summary>dest = pattern XOR dest</summary>
        PATINVERT = 0x005A0049,
        /// <summary>dest = (NOT dest)</summary>
        DSTINVERT = 0x00550009,
        /// <summary>dest = BLACK</summary>
        BLACKNESS = 0x00000042,
        /// <summary>dest = WHITE</summary>
        WHITENESS = 0x00FF0062
    }

    class C2PScreenshotDrawing
    {

        #region Class variable
        private System.Windows.Controls.Image m_imageContanier;
        private DrawingVisual                 m_drawingVisual; 
        private DrawingContext                m_drawingContext;
        private BitmapSource                  m_screenshot;
        private bool                          m_isDraw;
        private bool                          m_isSelect;
        private Point                         m_startPosition;
        private Point                         m_endPosition;
        private C2PLoupe                      m_loupe;
        private bool                          m_isElipsDraw;
        #endregion

        #region WINAPI DLL Imports

        [DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true, SetLastError = true)]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);
                
        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        #endregion

        #region Constructor

        public C2PScreenshotDrawing(System.Windows.Controls.Image contanier)
        {
            m_imageContanier = contanier;
            m_isDraw = false;
            m_drawingVisual = new DrawingVisual();
            m_imageContanier.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(OnMouseLeftButtonDown);
            m_imageContanier.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(OnMouseLeftButtonUp);
            m_imageContanier.MouseMove += new System.Windows.Input.MouseEventHandler(OnMouseMove);
            m_imageContanier.MouseEnter += new MouseEventHandler(OnMouseEnter);
            m_loupe = new C2PLoupe();
            m_isSelect = false;
            m_isElipsDraw = false;
         }
    
        #endregion

        #region Property
        public System.Windows.Controls.Image ImageContanier
        { 
            get
            { return m_imageContanier;}
            set
            {   m_imageContanier = value; }
        }

        public bool isElips
        {
            get
            { return m_isElipsDraw; }
            set
            { m_isElipsDraw = value; }
        
        }
        
        #endregion

        #region Signals

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_isDraw = true;
            m_startPosition = e.GetPosition(null);
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            m_isDraw = false;
            m_imageContanier.Source = C2PImageByteOperations.CutImageFromImage(new Rect(GetStartPositionCursorForSelect(), GetEndPositionCursorForSelect()), m_screenshot, m_isElipsDraw);
            m_isSelect = true;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (m_isDraw)
            {
                m_endPosition = e.GetPosition(null);
                Render(m_isDraw);
            }
            if (!m_isDraw && !m_isSelect)
            {
                m_endPosition = e.GetPosition(null);
                Render(m_isDraw);
            }
        
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if(m_imageContanier != null )
            {
                m_imageContanier.Cursor = Cursors.None;
            }
        }

        #endregion

        #region Draw shape
        private void DrawRuler(bool isSelectArea,bool isElips)
        {
            Color col = new Color();
            col.R = 0;
            col.G = 89;
            col.B = 255;
            col.A = 255;
            Brush brush = new SolidColorBrush(col);
            Brush circleBrush = new SolidColorBrush(Colors.Black);
            Brush brushRectangle = new SolidColorBrush(Colors.Red);
            Pen circlePen = new Pen(circleBrush, 1.0d);
            Pen penDash = new Pen(brush, 1.0d);
            Pen penCursor = new Pen(brush, 3.0d);
            Pen penRectangle = new Pen(brushRectangle, 2.0d);
            penDash.DashStyle = DashStyles.Dash;


            if (isSelectArea)
            {
                String str = "";
                str += (GetEndPositionCursorForSelect().X - GetStartPositionCursorForSelect().X).ToString();
                str += "x";
                str += (GetEndPositionCursorForSelect().Y - GetStartPositionCursorForSelect().Y).ToString();
                str += "px";

                FormattedText formattedText = new FormattedText(str, CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight, new Typeface("Verdana"), 12, System.Windows.Media.Brushes.White);



                if (m_isElipsDraw)
                {
                    m_drawingContext.PushOpacityMask(C2PImageByteOperations.CreateOpasityMaskForElips(m_screenshot, GetStartPositionCursorForSelect(), GetEndPositionCursorForSelect(), GetCenterForElips()));
                    m_drawingContext.DrawRectangle(circleBrush, null, new Rect(0, 0, (int)SystemParameters.VirtualScreenWidth, (int)SystemParameters.VirtualScreenHeight));
                    m_drawingContext.Pop();
                    m_drawingContext.DrawEllipse(null, penRectangle, GetCenterForElips(), GetWidthForElips(), GetHeigthForElips());
                }
                else
                {
                    m_drawingContext.PushOpacityMask(C2PImageByteOperations.CreateOpasityMaskForSelect(m_screenshot, GetStartPositionCursorForSelect(), GetEndPositionCursorForSelect()));
                    m_drawingContext.DrawRectangle(circleBrush, null, new Rect(0, 0, (int)SystemParameters.VirtualScreenWidth, (int)SystemParameters.VirtualScreenHeight));
                    m_drawingContext.Pop();
                    m_drawingContext.DrawRectangle(null, penRectangle, new Rect(GetStartPositionCursorForSelect(), GetEndPositionCursorForSelect()));
                }
                
                m_drawingContext.DrawLine(penDash, new Point(0, GetStartPositionCursorForSelect().Y), new Point(m_screenshot.Width, GetStartPositionCursorForSelect().Y));
                m_drawingContext.DrawLine(penDash, new Point(GetStartPositionCursorForSelect().X, 0), new Point(GetStartPositionCursorForSelect().X, m_screenshot.Height));
                m_drawingContext.DrawLine(penDash, new Point(0, GetEndPositionCursorForSelect().Y), new Point(m_screenshot.Width, GetEndPositionCursorForSelect().Y));
                m_drawingContext.DrawLine(penDash, new Point(GetEndPositionCursorForSelect().X, 0), new Point(GetEndPositionCursorForSelect().X, m_screenshot.Height));
                m_drawingContext.DrawLine(penCursor, new Point(m_endPosition.X - 15, m_endPosition.Y), new Point(m_endPosition.X + 15, m_endPosition.Y));
                m_drawingContext.DrawLine(penCursor, new Point(m_endPosition.X, m_endPosition.Y - 15), new Point(m_endPosition.X, m_endPosition.Y + 15));
                m_drawingContext.DrawRectangle(brush, null, new Rect(m_endPosition.X, m_endPosition.Y - 15, formattedText.Width, formattedText.Height));
                m_drawingContext.DrawText(formattedText, new Point(m_endPosition.X, m_endPosition.Y - 15));
                
            }
            else
            {
                m_drawingContext.DrawLine(penDash, new Point(0, m_endPosition.Y), new Point(m_screenshot.Width, m_endPosition.Y));
                m_drawingContext.DrawLine(penDash, new Point(m_endPosition.X, 0), new Point(m_endPosition.X, m_screenshot.Height));
                m_drawingContext.DrawLine(penCursor, new Point(m_endPosition.X - 15, m_endPosition.Y), new Point(m_endPosition.X + 15, m_endPosition.Y));
                m_drawingContext.DrawLine(penCursor, new Point(m_endPosition.X, m_endPosition.Y - 15), new Point(m_endPosition.X, m_endPosition.Y + 15));
            }

        
        }

        private void DrawLoupe()
        {
            
            if (m_endPosition.X > 55 && m_endPosition.Y > 55)
            {
                m_loupe.RenderLoup(new Rect(m_endPosition.X - 55, m_endPosition.Y - 55, 110, 110), m_screenshot,m_drawingContext);
            }
        }

        private void Render(bool isSelectArea)
        {
            Image img = new Image();

            m_drawingContext = m_drawingVisual.RenderOpen();
            m_drawingContext.DrawImage(m_screenshot, new Rect(0, 0, (int)SystemParameters.VirtualScreenWidth, (int)SystemParameters.VirtualScreenHeight));
            DrawRuler(isSelectArea,true);
            //DrawLoupe();
            m_drawingContext.Close();
            RenderTargetBitmap bmp = new RenderTargetBitmap((int)SystemParameters.VirtualScreenWidth, (int)SystemParameters.VirtualScreenHeight, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(m_drawingVisual);

            img.BeginInit(); //вроде это даже и не нужно
            img.Source = bmp;
            img.EndInit();   //как и это
            m_imageContanier.Source = img.Source;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        #endregion

        #region Servise function
        private Point GetStartPositionCursorForSelect()
        {
            if (m_startPosition.X < m_endPosition.X && m_startPosition.Y < m_endPosition.Y)
            {
                return m_startPosition;
            }
            if (m_startPosition.X > m_endPosition.X && m_startPosition.Y > m_endPosition.Y)
            {
                return m_endPosition;
            }
            if (m_startPosition.X < m_endPosition.X && m_startPosition.Y > m_endPosition.Y)
            {
                return new Point(m_startPosition.X, m_endPosition.Y);
            }
            if (m_startPosition.X > m_endPosition.X && m_startPosition.Y < m_endPosition.Y)
            {
                return new Point(m_endPosition.X, m_startPosition.Y);
            }
            return new Point(0, 0);
        }

        private Point GetEndPositionCursorForSelect()
        {
            if (m_startPosition.X < m_endPosition.X && m_startPosition.Y < m_endPosition.Y)
            {
                return m_endPosition;
            }
            if (m_startPosition.X > m_endPosition.X && m_startPosition.Y > m_endPosition.Y)
            {
                return m_startPosition;
            }
            if (m_startPosition.X < m_endPosition.X && m_startPosition.Y > m_endPosition.Y)
            {
                return new Point(m_endPosition.X, m_startPosition.Y);
            }
            if (m_startPosition.X > m_endPosition.X && m_startPosition.Y < m_endPosition.Y)
            {
                return new Point(m_startPosition.X, m_endPosition.Y);
            }
            return new Point(0, 0);
        }


        private double GetWidthForElips()
        {
            double radius;
            radius = GetStartPositionCursorForSelect().X - GetEndPositionCursorForSelect().X;
            return radius/2;
        }

        private double GetHeigthForElips()
        {
            double radius;
            radius = GetStartPositionCursorForSelect().Y - GetEndPositionCursorForSelect().Y;
            return radius/2;
        }


        private Point GetCenterForElips()
        {
            Point center = new Point(0, 0);
            center.X = GetEndPositionCursorForSelect().X + GetWidthForElips();
            center.Y = GetEndPositionCursorForSelect().Y + GetHeigthForElips();
            return center;
        }
        #endregion

        #region Create Image
        public void  CreateScreaneshot(Rect area)
        {
            IntPtr screenDC = GetDC(IntPtr.Zero);
            IntPtr memDC = CreateCompatibleDC(screenDC);
            IntPtr hBitmap = CreateCompatibleBitmap(screenDC, (int)SystemParameters.VirtualScreenWidth, (int)SystemParameters.VirtualScreenHeight);
            SelectObject(memDC, hBitmap); // Select bitmap from compatible bitmap to memDC

            // TODO: BitBlt may fail horribly
            BitBlt(memDC, 0, 0, (int)area.Width, (int)area.Height, screenDC, (int)area.X, (int)area.Y, TernaryRasterOperations.SRCCOPY);
            m_screenshot = Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            m_imageContanier.Source = m_screenshot;

            DeleteObject(hBitmap);
            ReleaseDC(IntPtr.Zero, screenDC);
            ReleaseDC(IntPtr.Zero, memDC);
        }
        
        private Image CreateResizedImage(ImageSource source, int width, int height, int margin)
        {
            Rect rect = new Rect(margin, margin, width - margin * 2, height - margin * 2);
            Image bitmap = new Image();
            DrawingGroup group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext;
            group.Children.Add(new ImageDrawing(source, rect));

            drawingContext = drawingVisual.RenderOpen();
            drawingContext.DrawDrawing(group);
            drawingContext.Close();
            RenderTargetBitmap resizedImage = new RenderTargetBitmap(
                width, height,         // Resized dimensions
                96, 96,                // Default DPI values
                PixelFormats.Default); // Default pixel format

            bitmap.BeginInit();
            resizedImage.Render(m_drawingVisual);
            bitmap.Source = resizedImage;
            bitmap.EndInit();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            return bitmap;
        }
        #endregion

    }
}
