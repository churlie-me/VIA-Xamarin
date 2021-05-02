package md57d2206917442bea2f34d97a29525b088;


public class ImageResource
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Via.Droid.Utils.ImageResource, Via.Android", ImageResource.class, __md_methods);
	}


	public ImageResource ()
	{
		super ();
		if (getClass () == ImageResource.class)
			mono.android.TypeManager.Activate ("Via.Droid.Utils.ImageResource, Via.Android", "", this, new java.lang.Object[] {  });
	}

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
