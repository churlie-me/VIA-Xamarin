using System;
using Naxam.Controls.Mapbox.Forms;
using Xamarin.Forms;

namespace Via
{
    public class MapViewRenderer : ViewRenderer<MapView, View>, IOnMapReadyCallback
    {
        public MapViewRenderer()
        {
        }

        public void OnMapReady(MapboxMap mapBox)
        {
            map = mapBox;
            map.SetStyle("mapbox://styles/mapbox/streets-v9");
            mapReady = true;
            OnMapRegionChanged();
            map.UiSettings.RotateGesturesEnabled = Element.RotateEnabled;
            map.UiSettings.TiltGesturesEnabled = Element.PitchEnabled;
            RemoveAllAnnotations();
            OnMapRegionChanged();
            if (Element.Center != null)
            {
                FocustoLocation(Element.Center.ToLatLng());
            }
            else
            {
                FocustoLocation(new LatLng(21.0278, 105.8342));
            }

            AddMapEvents();
            SetupFunctions();
            if (Element.MapStyle == null)
            {
                if (map.StyleUrl != null)
                {
                    Element.MapStyle = new MapStyle(map.StyleUrl);
                }
            }
            else
            {
                UpdateMapStyle();
            }

            if (Element.InfoWindowTemplate != null)
            {
                var info = new CustomInfoWindowAdapter(Context, Element);
                map.InfoWindowAdapter = info;
            }

            if (Element.Annotations != null)
            {
                AddAnnotations(Element.Annotations.ToArray());
                if (Element.Annotations is INotifyCollectionChanged notifyCollection)
                {
                    notifyCollection.CollectionChanged -= OnAnnotationsCollectionChanged;
                    notifyCollection.CollectionChanged += OnAnnotationsCollectionChanged;
                }
            }
        }
    }
}
