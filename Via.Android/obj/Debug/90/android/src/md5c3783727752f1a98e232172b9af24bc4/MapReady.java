package md5c3783727752f1a98e232172b9af24bc4;


public class MapReady
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.mapbox.mapboxsdk.maps.OnMapReadyCallback
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onMapReady:(Lcom/mapbox/mapboxsdk/maps/MapboxMap;)V:GetOnMapReady_Lcom_mapbox_mapboxsdk_maps_MapboxMap_Handler:Com.Mapbox.Mapboxsdk.Maps.IOnMapReadyCallbackInvoker, Naxam.Mapbox.Droid\n" +
			"";
		mono.android.Runtime.register ("Via.Droid.MapReady, Via.Android", MapReady.class, __md_methods);
	}


	public MapReady ()
	{
		super ();
		if (getClass () == MapReady.class)
			mono.android.TypeManager.Activate ("Via.Droid.MapReady, Via.Android", "", this, new java.lang.Object[] {  });
	}

	public MapReady (android.content.Context p0)
	{
		super ();
		if (getClass () == MapReady.class)
			mono.android.TypeManager.Activate ("Via.Droid.MapReady, Via.Android", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}


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
