using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Via.Controls;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Via.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ViaFrame), typeof(ViaFrameRenderer))]
namespace Via.Droid.Renderers
{
    public class ViaFrameRenderer : VisualElementRenderer<Frame>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var drawable = new GradientDrawable();
                var drawableResource = Resource.Drawable.DashedBorder;
                ViewGroup.SetBackgroundResource(Resource.Drawable.DashedBorder);
            }
        }
    }
}