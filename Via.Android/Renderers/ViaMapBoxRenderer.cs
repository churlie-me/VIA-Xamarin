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
using Naxam.Controls.Mapbox.Platform.Droid;
using Via.Controls;
using Via.Droid.Renderers;
using Xamarin.Forms;
using Naxam.Controls.Mapbox.Forms;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;

[assembly: ExportRenderer(typeof(ViaMapBox), typeof(ViaMapBoxRenderer))]
namespace Via.Droid.Renderers
{
    public class ViaMapBoxRenderer : MapViewRenderer
    {
        public ViaMapBoxRenderer(Context context) : base(context)
        {

        }

        private readonly TapGestureRecognizer _tapRecogniser;
        protected override void OnElementChanged(ElementChangedEventArgs<MapView> e)
        {
            if (e.OldElement != null && e.NewElement != null)
            {
                (e.OldElement)?.GestureRecognizers.Add(_tapRecogniser);
            }

            var element = e.NewElement as ViaMapBox;
            if (e.NewElement != null)
                element?.GestureRecognizers.Add(_tapRecogniser);
        }

        private void OnTap(object sender, TapGestureRecognizer recognizer)
        {

        }
    }
}