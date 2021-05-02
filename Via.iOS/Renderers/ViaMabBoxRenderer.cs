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
using Naxam.Controls.Mapbox.Platform.iOS;
using Naxam.Controls.Mapbox.Forms;
using Mapbox;

[assembly: ExportRenderer(typeof(ViaMapBox), typeof(ViaMabBoxRenderer))]
namespace Via.iOS.Renderers
{
    public class ViaMabBoxRenderer : MapViewRenderer
    {
        private MGLMapView Map => Control as MGLMapView;
        private readonly UITapGestureRecognizer _tapRecogniser;
        protected override void OnElementChanged(ElementChangedEventArgs<MapView> e)
        {
            if (e.OldElement != null && Map != null)
            {
                Control?.RemoveGestureRecognizer(_tapRecogniser);
            }

            if (e.NewElement != null)
            {
                Control?.AddGestureRecognizer(_tapRecogniser);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
        }

        private void OnTap(UITapGestureRecognizer recognizer)
        {
            var cgPoint = recognizer.LocationInView(Control);

            var location = Control.ConvertPoint(cgPoint, Control);

            ((ViaMapBox)Element).OnTap(new Position(location.Latitude, location.Longitude));
        }
    }
}