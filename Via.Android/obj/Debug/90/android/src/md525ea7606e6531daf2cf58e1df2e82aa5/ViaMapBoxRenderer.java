package md525ea7606e6531daf2cf58e1df2e82aa5;


public class ViaMapBoxRenderer
	extends md5007796e86d33eab93aea4d48c2478735.MapViewRenderer
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("Via.Droid.Renderers.ViaMapBoxRenderer, Via.Android", ViaMapBoxRenderer.class, __md_methods);
	}


	public ViaMapBoxRenderer (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == ViaMapBoxRenderer.class)
			mono.android.TypeManager.Activate ("Via.Droid.Renderers.ViaMapBoxRenderer, Via.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public ViaMapBoxRenderer (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == ViaMapBoxRenderer.class)
			mono.android.TypeManager.Activate ("Via.Droid.Renderers.ViaMapBoxRenderer, Via.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public ViaMapBoxRenderer (android.content.Context p0)
	{
		super (p0);
		if (getClass () == ViaMapBoxRenderer.class)
			mono.android.TypeManager.Activate ("Via.Droid.Renderers.ViaMapBoxRenderer, Via.Android", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
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
