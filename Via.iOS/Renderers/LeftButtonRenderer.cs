using CoreAnimation;
using CoreGraphics;
using System;
using System.ComponentModel;
using UIKit;
using Via.Controls;
using Via.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(LeftButton), typeof(LeftButtonRenderer))]
namespace Via.iOS.Renderers
{
    public class LeftButtonRenderer : ButtonRenderer
    {
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var maskingShapeLayer = new CAShapeLayer()
            {
                Path = UIBezierPath.FromRoundedRect(Bounds, UIRectCorner.BottomLeft | UIRectCorner.TopLeft, new CGSize(10, 10)).CGPath,
                BackgroundColor = new CGColor(0.00f, 0.19f, 0.29f, 1.0f)
            };
            Layer.Mask = maskingShapeLayer;
        }

        public override void LayoutSubviews()
        {
            var maskingShapeLayer = new CAShapeLayer()
            {
                Path = UIBezierPath.FromRoundedRect(Bounds, UIRectCorner.BottomLeft | UIRectCorner.TopLeft, new CGSize(10, 10)).CGPath,
                BackgroundColor = new CGColor(0.00f, 0.19f, 0.29f, 1.0f)
            };
            Layer.Mask = maskingShapeLayer;
            base.LayoutSubviews();
        }
    }
}
