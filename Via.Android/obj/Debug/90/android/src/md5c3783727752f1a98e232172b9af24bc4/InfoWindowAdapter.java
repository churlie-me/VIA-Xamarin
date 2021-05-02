package md5c3783727752f1a98e232172b9af24bc4;


public class InfoWindowAdapter
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
		mono.android.Runtime.register ("Via.Droid.InfoWindowAdapter, Via.Android", InfoWindowAdapter.class, __md_methods);
	}


	public InfoWindowAdapter ()
	{
		super ();
		if (getClass () == InfoWindowAdapter.class)
			mono.android.TypeManager.Activate ("Via.Droid.InfoWindowAdapter, Via.Android", "", this, new java.lang.Object[] {  });
	}

	public InfoWindowAdapter (android.content.Context p0)
	{
		super ();
		if (getClass () == InfoWindowAdapter.class)
			mono.android.TypeManager.Activate ("Via.Droid.InfoWindowAdapter, Via.Android", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
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
