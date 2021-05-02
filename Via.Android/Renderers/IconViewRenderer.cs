using System;
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using System.ComponentModel;
using Via.Controls;
using Xamarin.Forms;
using Via.Droid.Renderers;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Content;

[assembly: ExportRenderer(typeof(Via.Controls.Image), typeof(IconViewRenderer))]

namespace Via.Droid.Renderers
{
    public class IconViewRenderer : ImageRenderer
    {
        public IconViewRenderer(Context context) : base(context)
        {
        }

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
            if (Control == null || Element == null)
                return;

            if (((Controls.Image)Element).TintColor.Equals(Xamarin.Forms.Color.Transparent))
            {
                //Turn off tintingy

                if (Control.ColorFilter != null)
                    Control.ClearColorFilter();

                return;
            }

            //Apply tint color
            var colorFilter = new PorterDuffColorFilter(((Controls.Image)Element).TintColor.ToAndroid(), PorterDuff.Mode.SrcIn);
            Control.SetColorFilter(colorFilter);
            Console.WriteLine($"colorFilter {colorFilter}");
        }
    }
}

