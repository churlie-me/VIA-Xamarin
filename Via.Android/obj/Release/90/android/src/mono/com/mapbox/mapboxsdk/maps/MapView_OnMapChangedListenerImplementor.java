package mono.com.mapbox.mapboxsdk.maps;


public class MapView_OnMapChangedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.mapbox.mapboxsdk.maps.MapView.OnMapChangedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onMapChanged:(I)V:GetOnMapChanged_IHandler:Com.Mapbox.Mapboxsdk.Maps.MapView/IOnMapChangedListenerInvoker, Naxam.Mapbox.Droid\n" +
			"";
		mono.android.Runtime.register ("Com.Mapbox.Mapboxsdk.Maps.MapView+IOnMapChangedListenerImplementor, Naxam.Mapbox.Droid", MapView_OnMapChangedListenerImplementor.class, __md_methods);
	}


	public MapView_OnMapChangedListenerImplementor ()
	{
		super ();
		if (getClass () == MapView_OnMapChangedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Mapbox.Mapboxsdk.Maps.MapView+IOnMapChangedListenerImplementor, Naxam.Mapbox.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onMapChanged (int p0)
	{
		n_onMapChanged (p0);
	}

	private native void n_onMapChanged (int p0);

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
