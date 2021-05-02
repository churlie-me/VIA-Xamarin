package md51a368c209211627bd83fcffc52e42025;


public class ListOfflineRegionsCallback
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.mapbox.mapboxsdk.offline.OfflineManager.ListOfflineRegionsCallback
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onError:(Ljava/lang/String;)V:GetOnError_Ljava_lang_String_Handler:Com.Mapbox.Mapboxsdk.Offline.OfflineManager/IListOfflineRegionsCallbackInvoker, Naxam.Mapbox.Droid\n" +
			"n_onList:([Lcom/mapbox/mapboxsdk/offline/OfflineRegion;)V:GetOnList_arrayLcom_mapbox_mapboxsdk_offline_OfflineRegion_Handler:Com.Mapbox.Mapboxsdk.Offline.OfflineManager/IListOfflineRegionsCallbackInvoker, Naxam.Mapbox.Droid\n" +
			"";
		mono.android.Runtime.register ("Naxam.Mapbox.Platform.Droid.Offline.ListOfflineRegionsCallback, Naxam.Mapbox.Platform.Droid", ListOfflineRegionsCallback.class, __md_methods);
	}


	public ListOfflineRegionsCallback ()
	{
		super ();
		if (getClass () == ListOfflineRegionsCallback.class)
			mono.android.TypeManager.Activate ("Naxam.Mapbox.Platform.Droid.Offline.ListOfflineRegionsCallback, Naxam.Mapbox.Platform.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onError (java.lang.String p0)
	{
		n_onError (p0);
	}

	private native void n_onError (java.lang.String p0);


	public void onList (com.mapbox.mapboxsdk.offline.OfflineRegion[] p0)
	{
		n_onList (p0);
	}

	private native void n_onList (com.mapbox.mapboxsdk.offline.OfflineRegion[] p0);

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
