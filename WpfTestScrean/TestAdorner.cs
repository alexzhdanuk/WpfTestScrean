using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Globalization;

namespace WpfTestScrean
{
    public class TestAdorner : Adorner
    {
        private Canvas m_canvas;
        private PuncturedRect _prCropMask;
        private VisualCollection m_visualColection;

        public TestAdorner(UIElement adornedElement, Rect rcInit)
            : base(adornedElement)
        {
            m_visualColection = new VisualCollection(this);
            _prCropMask = new PuncturedRect();
            _prCropMask.RenderSize = new Size(100, 100);
            _prCropMask.IsHitTestVisible = false;
            _prCropMask.RenderSize = new Size(10, 20);
            _prCropMask.RectInterior = rcInit;
            //_prCropMask.Fill = Fill;
            m_visualColection.Add(_prCropMask);

            m_canvas = new Canvas();
            m_canvas.HorizontalAlignment = HorizontalAlignment.Stretch;
            m_canvas.VerticalAlignment = VerticalAlignment.Stretch;

            m_visualColection.Add(m_canvas);
        
        }

        public BitmapSource BpsCrop()
        {
            Thickness margin = AdornerMargin();
            Rect rcInterior = _prCropMask.RectInteriorProperty;

            Point pxFromSize = UnitsToPx(rcInterior.Width, rcInterior.Height);

            // It appears that CroppedBitmap indexes from the upper left of the margin whereas RenderTargetBitmap renders the
            // control exclusive of the margin.  Hence our need to take the margins into account here...

            Point pxFromPos = UnitsToPx(rcInterior.Left + margin.Left, rcInterior.Top + margin.Top);
            Point pxWhole = UnitsToPx(AdornedElement.RenderSize.Width + margin.Left, AdornedElement.RenderSize.Height + margin.Left);
            pxFromSize.X = Math.Max(Math.Min(pxWhole.X - pxFromPos.X, pxFromSize.X), 0);
            pxFromSize.Y = Math.Max(Math.Min(pxWhole.Y - pxFromPos.Y, pxFromSize.Y), 0);
            if (pxFromSize.X == 0 || pxFromSize.Y == 0)
            {
                return null;
            }
            System.Windows.Int32Rect rcFrom = new System.Windows.Int32Rect((int)152, (int)111, (int)200, (int)200);

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)pxWhole.X, (int)pxWhole.Y, (double)96.0, (double)96.0, PixelFormats.Default);
            rtb.Render(AdornedElement);
            return new CroppedBitmap(rtb, rcFrom);
        }

        private Thickness AdornerMargin()
        {
            Thickness thick = new Thickness(0);
            if (AdornedElement is FrameworkElement)
            {
                thick = ((FrameworkElement)AdornedElement).Margin;
            }
            return thick;
        }

        private Point UnitsToPx(double x, double y)
        {
            return new Point((int)(x * 96 / 96), (int)(y * 96 / 96));
        }
    }

    public class MyAdorner : Adorner
    {
        public MyAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            SolidColorBrush renderBrush = new SolidColorBrush(Colors.LightCoral);
            Pen renderPen = new Pen(new SolidColorBrush(Colors.DarkBlue), 1.0);

            drawingContext.DrawRectangle(renderBrush, renderPen,
                new Rect(new Point(0, 0), AdornedElement.DesiredSize));

            drawingContext.DrawText(new FormattedText("Help Text!", new CultureInfo("en-US"), FlowDirection.LeftToRight, new Typeface("Arial"), 18, Brushes.Black),
               new Point(10, 10));
        }
    }


}
