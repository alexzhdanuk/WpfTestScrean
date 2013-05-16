using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;

namespace WpfTestScrean
{
    public class C2PDrawingLayer : Adorner
    {
        private Canvas m_canvas;
        private Ellipse m_prCropMask;
        private VisualCollection m_visualColection;

        public C2PDrawingLayer(UIElement adornedElement)
            : base(adornedElement)
        {
            m_visualColection = new VisualCollection(this);
            m_prCropMask = new Ellipse();
            m_prCropMask.RenderSize = new Size(100, 100);
            m_prCropMask.IsHitTestVisible = false;
            m_prCropMask.RenderSize = new Size(10, 20);
            m_visualColection.Add(m_prCropMask);

            m_canvas = new Canvas();
            m_canvas.HorizontalAlignment = HorizontalAlignment.Stretch;
            m_canvas.VerticalAlignment = VerticalAlignment.Stretch;

            m_visualColection.Add(m_canvas);
        
        }
    }
}
