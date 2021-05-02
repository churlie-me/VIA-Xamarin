package mono.com.mapbox.mapboxsdk.maps;


public class MapboxMap_OnCameraChangeListenerImplementor
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.mapbox.mapboxsdk.maps.MapboxMap.OnCameraChangeListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCameraChange:(Lcom/mapbox/mapboxsdk/camera/CameraPosition;)V:GetOnCameraChange_Lcom_mapbox_mapboxsdk_camera_CameraPosition_Handler:Com.Mapbox.Mapboxsdk.Maps.MapboxMap/IOnCameraChangeListenerInvoker, Naxam.Mapbox.Droid\n" +
			"";
		mono.android.Runtime.register ("Com.Mapbox.Mapboxsdk.Maps.MapboxMap+IOnCameraChangeListenerImplementor, Naxam.Mapbox.Droid", MapboxMap_OnCameraChangeListenerImplementor.class, __md_methods);
	}


	public MapboxMap_OnCameraChangeListenerImplementor ()
	{
		super ();
		if (getClass () == MapboxMap_OnCameraChangeListenerImplementor.class)
			mono.android.TypeManager.Activate ("Com.Mapbox.Mapboxsdk.Maps.MapboxMap+IOnCameraChangeListenerImplementor, Naxam.Mapbox.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onCameraChange (com.mapbox.mapboxsdk.camera.CameraPosition p0)
	{
		n_onCameraChange (p0);
	}

	private native void n_onCameraChange (com.mapbox.mapboxsdk.camera.CameraPosition p0);

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
