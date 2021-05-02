package md51a368c209211627bd83fcffc52e42025;


public class CreateOfflineRegionCallback
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.mapbox.mapboxsdk.offline.OfflineManager.CreateOfflineRegionCallback
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Lcom/mapbox/mapboxsdk/offline/OfflineRegion;)V:GetOnCreate_Lcom_mapbox_mapboxsdk_offline_OfflineRegion_Handler:Com.Mapbox.Mapboxsdk.Offline.OfflineManager/ICreateOfflineRegionCallbackInvoker, Naxam.Mapbox.Droid\n" +
			"n_onError:(Ljava/lang/String;)V:GetOnError_Ljava_lang_String_Handler:Com.Mapbox.Mapboxsdk.Offline.OfflineManager/ICreateOfflineRegionCallbackInvoker, Naxam.Mapbox.Droid\n" +
			"";
		mono.android.Runtime.register ("Naxam.Mapbox.Platform.Droid.Offline.CreateOfflineRegionCallback, Naxam.Mapbox.Platform.Droid", CreateOfflineRegionCallback.class, __md_methods);
	}


	public CreateOfflineRegionCallback ()
	{
		super ();
		if (getClass () == CreateOfflineRegionCallback.class)
			mono.android.TypeManager.Activate ("Naxam.Mapbox.Platform.Droid.Offline.CreateOfflineRegionCallback, Naxam.Mapbox.Platform.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (com.mapbox.mapboxsdk.offline.OfflineRegion p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (com.mapbox.mapboxsdk.offline.OfflineRegion p0);


	public void onError (java.lang.String p0)
	{
		n_onError (p0);
	}

	private native void n_onError (java.lang.String p0);

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
