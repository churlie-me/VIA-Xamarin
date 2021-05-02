using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using CoreGraphics;
using System.ComponentModel;
using Via.iOS.Renderers;
using Via.Controls;
using CoreAnimation;

[assembly: ExportRenderer(typeof(ViaFrame), typeof(ViaFrameRenderer))]
namespace Via.iOS.Renderers
{
    public class ViaFrameRenderer : FrameRenderer
    {
        public ViaFrameRenderer()
        {
            Layer.BorderColor = UIColor.Black.CGColor;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            CAShapeLayer viewBorder = new CAShapeLayer();
            viewBorder.StrokeColor = UIColor.Black.CGColor;
            viewBorder.FillColor = null;
            viewBorder.LineDashPattern = new NSNumber[] { new NSNumber(2), new NSNumber(2) };
            viewBorder.Frame = NativeView.Bounds;
            viewBorder.Path = UIBezierPath.FromRect(NativeView.Bounds).CGPath;

            Layer.AddSublayer(viewBorder);

            // If you don't want the shadow effect
            Element.HasShadow = false;
        }
    }
}