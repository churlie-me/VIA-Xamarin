package md5007796e86d33eab93aea4d48c2478735;


public class SnapshotReadyCallback
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.mapbox.mapboxsdk.maps.MapboxMap.SnapshotReadyCallback
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onSnapshotReady:(Landroid/graphics/Bitmap;)V:GetOnSnapshotReady_Landroid_graphics_Bitmap_Handler:Com.Mapbox.Mapboxsdk.Maps.MapboxMap/ISnapshotReadyCallbackInvoker, Naxam.Mapbox.Droid\n" +
			"";
		mono.android.Runtime.register ("Naxam.Controls.Mapbox.Platform.Droid.SnapshotReadyCallback, Naxam.Mapbox.Platform.Droid", SnapshotReadyCallback.class, __md_methods);
	}


	public SnapshotReadyCallback ()
	{
		super ();
		if (getClass () == SnapshotReadyCallback.class)
			mono.android.TypeManager.Activate ("Naxam.Controls.Mapbox.Platform.Droid.SnapshotReadyCallback, Naxam.Mapbox.Platform.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onSnapshotReady (android.graphics.Bitmap p0)
	{
		n_onSnapshotReady (p0);
	}

	private native void n_onSnapshotReady (android.graphics.Bitmap p0);

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
