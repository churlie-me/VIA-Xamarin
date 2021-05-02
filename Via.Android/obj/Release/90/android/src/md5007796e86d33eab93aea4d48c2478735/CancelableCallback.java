package md5007796e86d33eab93aea4d48c2478735;


public class CancelableCallback
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.mapbox.mapboxsdk.maps.MapboxMap.CancelableCallback
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCancel:()V:GetOnCancelHandler:Com.Mapbox.Mapboxsdk.Maps.MapboxMap/ICancelableCallbackInvoker, Naxam.Mapbox.Droid\n" +
			"n_onFinish:()V:GetOnFinishHandler:Com.Mapbox.Mapboxsdk.Maps.MapboxMap/ICancelableCallbackInvoker, Naxam.Mapbox.Droid\n" +
			"";
		mono.android.Runtime.register ("Naxam.Controls.Mapbox.Platform.Droid.CancelableCallback, Naxam.Mapbox.Platform.Droid", CancelableCallback.class, __md_methods);
	}


	public CancelableCallback ()
	{
		super ();
		if (getClass () == CancelableCallback.class)
			mono.android.TypeManager.Activate ("Naxam.Controls.Mapbox.Platform.Droid.CancelableCallback, Naxam.Mapbox.Platform.Droid", "", this, new java.lang.Object[] {  });
	}


	public void onCancel ()
	{
		n_onCancel ();
	}

	private native void n_onCancel ();


	public void onFinish ()
	{
		n_onFinish ();
	}

	private native void n_onFinish ();

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
