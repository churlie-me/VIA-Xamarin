package mono.com.mapbox.mapboxsdk.maps;


public class MapboxMap_OnScrollListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.mapbox.mapboxsdk.maps.MapboxMap.OnScrollListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onScroll:()V:GetOnScrollHandler:Com.Mapbox.Mapboxsdk.Maps.MapboxMap/IOnScrollListenerInvoker, Naxam.Mapbox.Droid\n" +
			"";
		mono.android.Runtime.register ("Com.Mapbox.Mapboxsdk.Maps.MapboxMap+IOnScrollListenerImplementor, Naxam.Mapbox.Droid", MapboxMap_OnScrollListenerImplementor.class, __md_methods);
	}


	public MapboxMap_OnScrollListenerImplementor ()
	{
		super ();
		if (getClass () == MapboxMap_OnScrollListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Mapbox.Mapboxsdk.Maps.MapboxMap+IOnScrollListenerImplementor, Naxam.Mapbox.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onScroll ()
	{
		n_onScroll ();
	}

	private native void n_onScroll ();

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
