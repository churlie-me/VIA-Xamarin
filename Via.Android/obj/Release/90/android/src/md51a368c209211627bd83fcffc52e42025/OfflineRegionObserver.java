package md51a368c209211627bd83fcffc52e42025;


public class OfflineRegionObserver
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.mapbox.mapboxsdk.offline.OfflineRegion.OfflineRegionObserver
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_mapboxTileCountLimitExceeded:(J)V:GetMapboxTileCountLimitExceeded_JHandler:Com.Mapbox.Mapboxsdk.Offline.OfflineRegion/IOfflineRegionObserverInvoker, Naxam.Mapbox.Droid\n" +
			"n_onError:(Lcom/mapbox/mapboxsdk/offline/OfflineRegionError;)V:GetOnError_Lcom_mapbox_mapboxsdk_offline_OfflineRegionError_Handler:Com.Mapbox.Mapboxsdk.Offline.OfflineRegion/IOfflineRegionObserverInvoker, Naxam.Mapbox.Droid\n" +
			"n_onStatusChanged:(Lcom/mapbox/mapboxsdk/offline/OfflineRegionStatus;)V:GetOnStatusChanged_Lcom_mapbox_mapboxsdk_offline_OfflineRegionStatus_Handler:Com.Mapbox.Mapboxsdk.Offline.OfflineRegion/IOfflineRegionObserverInvoker, Naxam.Mapbox.Droid\n" +
			"";
		mono.android.Runtime.register ("Naxam.Mapbox.Platform.Droid.Offline.OfflineRegionObserver, Naxam.Mapbox.Platform.Droid", OfflineRegionObserver.class, __md_methods);
	}


	public OfflineRegionObserver ()
	{
		super ();
		if (getClass () == OfflineRegionObserver.class)
			mono.android.TypeManager.Activate ("Naxam.Mapbox.Platform.Droid.Offline.OfflineRegionObserver, Naxam.Mapbox.Platform.Droid", "", this, new java.lang.Object[] {  });
	}


	public void mapboxTileCountLimitExceeded (long p0)
	{
		n_mapboxTileCountLimitExceeded (p0);
	}

	private native void n_mapboxTileCountLimitExceeded (long p0);


	public void onError (com.mapbox.mapboxsdk.offline.OfflineRegionError p0)
	{
		n_onError (p0);
	}

	private native void n_onError (com.mapbox.mapboxsdk.offline.OfflineRegionError p0);


	public void onStatusChanged (com.mapbox.mapboxsdk.offline.OfflineRegionStatus p0)
	{
		n_onStatusChanged (p0);
	}

	private native void n_onStatusChanged (com.mapbox.mapboxsdk.offline.OfflineRegionStatus p0);

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
