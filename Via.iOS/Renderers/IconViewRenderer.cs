using System;
using System.ComponentModel;
using Xamarin.Forms;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using Via.Controls;
using Via.iOS.Renderers;

[assembly: ExportRenderer(typeof(Via.Controls.Image), typeof(IconViewRenderer))]

namespace Via.iOS.Renderers
{
    public class IconViewRenderer : ImageRenderer
    {

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Image> e)
        {
            base.OnElementChanged(e);

            SetTint();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Controls.Image.TintColorProperty.PropertyName || e.PropertyName == Xamarin.Forms.Image.SourceProperty.PropertyName)
                SetTint();
        }

        void SetTint()
        {
            if (Control?.Image == null || Element == null)
                return;

            if (((Controls.Image)Element).TintColor == Color.Transparent)
            {
                //Turn off tinting
                Control.Image = Control.Image.ImageWithRenderingMode(UIImageRenderingMode.Automatic);
                Control.TintColor = null;
            }
            else
            {
                //Apply tint color
                Control.Image = Control.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
                Control.TintColor = ((Controls.Image)Element).TintColor.ToUIColor();
            }
        }
    }
}

