package md5007796e86d33eab93aea4d48c2478735;


public class MapViewRenderer
	extends md51558244f76c53b6aeda52c8a337f2c37.ViewRenderer_2
	implements
		mono.android.IGCUserPeer,
		com.mapbox.mapboxsdk.maps.MapView.OnMapChangedListener,
		com.mapbox.mapboxsdk.maps.OnMapReadyCallback
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onMapChanged:(I)V:GetOnMapChanged_IHandler:Com.Mapbox.Mapboxsdk.Maps.MapView/IOnMapChangedListenerInvoker, Naxam.Mapbox.Droid\n" +
			"n_onMapReady:(Lcom/mapbox/mapboxsdk/maps/MapboxMap;)V:GetOnMapReady_Lcom_mapbox_mapboxsdk_maps_MapboxMap_Handler:Com.Mapbox.Mapboxsdk.Maps.IOnMapReadyCallbackInvoker, Naxam.Mapbox.Droid\n" +
			"";
		mono.android.Runtime.register ("Naxam.Controls.Mapbox.Platform.Droid.MapViewRenderer, Naxam.Mapbox.Platform.Droid", MapViewRenderer.class, __md_methods);
	}


	public MapViewRenderer (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == MapViewRenderer.class)
			mono.android.TypeManager.Activate ("Naxam.Controls.Mapbox.Platform.Droid.MapViewRenderer, Naxam.Mapbox.Platform.Droid", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public MapViewRenderer (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == MapViewRenderer.class)
			mono.android.TypeManager.Activate ("Naxam.Controls.Mapbox.Platform.Droid.MapViewRenderer, Naxam.Mapbox.Platform.Droid", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public MapViewRenderer (android.content.Context p0)
	{
		super (p0);
		if (getClass () == MapViewRenderer.class)
			mono.android.TypeManager.Activate ("Naxam.Controls.Mapbox.Platform.Droid.MapViewRenderer, Naxam.Mapbox.Platform.Droid", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


	public void onMapChanged (int p0)
	{
		n_onMapChanged (p0);
	}

	private native void n_onMapChanged (int p0);


	public void onMapReady (com.mapbox.mapboxsdk.maps.MapboxMap p0)
	{
		n_onMapReady (p0);
	}

	private native void n_onMapReady (com.mapbox.mapboxsdk.maps.MapboxMap p0);

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
