package mono.com.mapbox.mapboxsdk.maps;


public class MapboxMap_OnStyleLoadedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.mapbox.mapboxsdk.maps.MapboxMap.OnStyleLoadedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onStyleLoaded:(Ljava/lang/String;)V:GetOnStyleLoaded_Ljava_lang_String_Handler:Com.Mapbox.Mapboxsdk.Maps.MapboxMap/IOnStyleLoadedListenerInvoker, Naxam.Mapbox.Droid\n" +
			"";
		mono.android.Runtime.register ("Com.Mapbox.Mapboxsdk.Maps.MapboxMap+IOnStyleLoadedListenerImplementor, Naxam.Mapbox.Droid", MapboxMap_OnStyleLoadedListenerImplementor.class, __md_methods);
	}


	public MapboxMap_OnStyleLoadedListenerImplementor ()
	{
		super ();
		if (getClass () == MapboxMap_OnStyleLoadedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Mapbox.Mapboxsdk.Maps.MapboxMap+IOnStyleLoadedListenerImplementor, Naxam.Mapbox.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onStyleLoaded (java.lang.String p0)
	{
		n_onStyleLoaded (p0);
	}

	private native void n_onStyleLoaded (java.lang.String p0);

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
