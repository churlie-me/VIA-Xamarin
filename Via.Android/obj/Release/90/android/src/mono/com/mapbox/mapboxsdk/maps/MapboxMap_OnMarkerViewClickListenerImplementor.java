package mono.com.mapbox.mapboxsdk.maps;


public class MapboxMap_OnMarkerViewClickListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.mapbox.mapboxsdk.maps.MapboxMap.OnMarkerViewClickListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onMarkerClick:(Lcom/mapbox/mapboxsdk/annotations/Marker;Landroid/view/View;Lcom/mapbox/mapboxsdk/maps/MapboxMap$MarkerViewAdapter;)Z:GetOnMarkerClick_Lcom_mapbox_mapboxsdk_annotations_Marker_Landroid_view_View_Lcom_mapbox_mapboxsdk_maps_MapboxMap_MarkerViewAdapter_Handler:Com.Mapbox.Mapboxsdk.Maps.MapboxMap/IOnMarkerViewClickListenerInvoker, Naxam.Mapbox.Droid\n" +
			"";
		mono.android.Runtime.register ("Com.Mapbox.Mapboxsdk.Maps.MapboxMap+IOnMarkerViewClickListenerImplementor, Naxam.Mapbox.Droid", MapboxMap_OnMarkerViewClickListenerImplementor.class, __md_methods);
	}


	public MapboxMap_OnMarkerViewClickListenerImplementor ()
	{
		super ();
		if (getClass () == MapboxMap_OnMarkerViewClickListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Mapbox.Mapboxsdk.Maps.MapboxMap+IOnMarkerViewClickListenerImplementor, Naxam.Mapbox.Droid", "", this, new java.lang.Object[] {  });
	}


	public boolean onMarkerClick (com.mapbox.mapboxsdk.annotations.Marker p0, android.view.View p1, com.mapbox.mapboxsdk.maps.MapboxMap.MarkerViewAdapter p2)
	{
		return n_onMarkerClick (p0, p1, p2);
	}

	private native boolean n_onMarkerClick (com.mapbox.mapboxsdk.annotations.Marker p0, android.view.View p1, com.mapbox.mapboxsdk.maps.MapboxMap.MarkerViewAdapter p2);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
