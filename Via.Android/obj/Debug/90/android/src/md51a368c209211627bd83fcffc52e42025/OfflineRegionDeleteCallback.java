package md51a368c209211627bd83fcffc52e42025;


public class OfflineRegionDeleteCallback
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.mapbox.mapboxsdk.offline.OfflineRegion.OfflineRegionDeleteCallback
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onDelete:()V:GetOnDeleteHandler:Com.Mapbox.Mapboxsdk.Offline.OfflineRegion/IOfflineRegionDeleteCallbackInvoker, Naxam.Mapbox.Droid\n" +
			"n_onError:(Ljava/lang/String;)V:GetOnError_Ljava_lang_String_Handler:Com.Mapbox.Mapboxsdk.Offline.OfflineRegion/IOfflineRegionDeleteCallbackInvoker, Naxam.Mapbox.Droid\n" +
			"";
		mono.android.Runtime.register ("Naxam.Mapbox.Platform.Droid.Offline.OfflineRegionDeleteCallback, Naxam.Mapbox.Platform.Droid", OfflineRegionDeleteCallback.class, __md_methods);
	}


	public OfflineRegionDeleteCallback ()
	{
		super ();
		if (getClass () == OfflineRegionDeleteCallback.class)
			mono.android.TypeManager.Activate ("Naxam.Mapbox.Platform.Droid.Offline.OfflineRegionDeleteCallback, Naxam.Mapbox.Platform.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onDelete ()
	{
		n_onDelete ();
	}

	private native void n_onDelete ();


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
