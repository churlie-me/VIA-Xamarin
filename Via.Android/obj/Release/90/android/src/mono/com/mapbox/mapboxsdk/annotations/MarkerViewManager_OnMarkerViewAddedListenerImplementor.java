package mono.com.mapbox.mapboxsdk.annotations;


public class MarkerViewManager_OnMarkerViewAddedListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.mapbox.mapboxsdk.annotations.MarkerViewManager.OnMarkerViewAddedListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onViewAdded:(Lcom/mapbox/mapboxsdk/annotations/MarkerView;)V:GetOnViewAdded_Lcom_mapbox_mapboxsdk_annotations_MarkerView_Handler:Com.Mapbox.Mapboxsdk.Annotations.MarkerViewManager/IOnMarkerViewAddedListenerInvoker, Naxam.Mapbox.Droid\n" +
			"";
		mono.android.Runtime.register ("Com.Mapbox.Mapboxsdk.Annotations.MarkerViewManager+IOnMarkerViewAddedListenerImplementor, Naxam.Mapbox.Droid", MarkerViewManager_OnMarkerViewAddedListenerImplementor.class, __md_methods);
	}


	public MarkerViewManager_OnMarkerViewAddedListenerImplementor ()
	{
		super ();
		if (getClass () == MarkerViewManager_OnMarkerViewAddedListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Mapbox.Mapboxsdk.Annotations.MarkerViewManager+IOnMarkerViewAddedListenerImplementor, Naxam.Mapbox.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onViewAdded (com.mapbox.mapboxsdk.annotations.MarkerView p0)
	{
		n_onViewAdded (p0);
	}

	private native void n_onViewAdded (com.mapbox.mapboxsdk.annotations.MarkerView p0);

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
