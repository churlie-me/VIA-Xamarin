using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Via.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Via.Controls;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(HorizontalListview), typeof(HorizontalListviewRenderer))]
namespace Via.Droid.Renderers
{
    public class HorizontalListviewRenderer : ScrollViewRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            var element = e.NewElement as HorizontalListview;
            element?.Render();

            if (e.OldElement != null)
                e.OldElement.PropertyChanged -= OnElementPropertyChanged;

            e.NewElement.PropertyChanged += OnElementPropertyChanged;

        }

        protected void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }
    }
}