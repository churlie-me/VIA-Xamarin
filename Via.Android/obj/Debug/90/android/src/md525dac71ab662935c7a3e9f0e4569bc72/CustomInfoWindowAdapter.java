package md525dac71ab662935c7a3e9f0e4569bc72;


public class CustomInfoWindowAdapter
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.mapbox.mapboxsdk.maps.MapboxMap.InfoWindowAdapter
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_getInfoWindow:(Lcom/mapbox/mapboxsdk/annotations/Marker;)Landroid/view/View;:GetGetInfoWindow_Lcom_mapbox_mapboxsdk_annotations_Marker_Handler:Com.Mapbox.Mapboxsdk.Maps.MapboxMap/IInfoWindowAdapterInvoker, Naxam.Mapbox.Droid\n" +
			"";
		mono.android.Runtime.register ("Naxam.Mapbox.Platform.Droid.CustomInfoWindowAdapter, Naxam.Mapbox.Platform.Droid", CustomInfoWindowAdapter.class, __md_methods);
	}


	public CustomInfoWindowAdapter ()
	{
		super ();
		if (getClass () == CustomInfoWindowAdapter.class)
			mono.android.TypeManager.Activate ("Naxam.Mapbox.Platform.Droid.CustomInfoWindowAdapter, Naxam.Mapbox.Platform.Droid", "", this, new java.lang.Object[] {  });
	}


	public android.view.View getInfoWindow (com.mapbox.mapboxsdk.annotations.Marker p0)
	{
		return n_getInfoWindow (p0);
	}

	private native android.view.View n_getInfoWindow (com.mapbox.mapboxsdk.annotations.Marker p0);

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
