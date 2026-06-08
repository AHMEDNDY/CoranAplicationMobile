package crc644a2a1913f48e06d8;


public class CusotmOnEditorActionListiner
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		android.widget.TextView.OnEditorActionListener
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onEditorAction:(Landroid/widget/TextView;ILandroid/view/KeyEvent;)Z:GetOnEditorAction_Landroid_widget_TextView_ILandroid_view_KeyEvent_Handler:Android.Widget.TextView/IOnEditorActionListenerInvoker, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null\n" +
			"";
		mono.android.Runtime.register ("Syncfusion.Maui.PdfViewer.CusotmOnEditorActionListiner, Syncfusion.Maui.PdfViewer", CusotmOnEditorActionListiner.class, __md_methods);
	}

	public CusotmOnEditorActionListiner ()
	{
		super ();
		if (getClass () == CusotmOnEditorActionListiner.class) {
			mono.android.TypeManager.Activate ("Syncfusion.Maui.PdfViewer.CusotmOnEditorActionListiner, Syncfusion.Maui.PdfViewer", "", this, new java.lang.Object[] {  });
		}
	}

	public boolean onEditorAction (android.widget.TextView p0, int p1, android.view.KeyEvent p2)
	{
		return n_onEditorAction (p0, p1, p2);
	}

	private native boolean n_onEditorAction (android.widget.TextView p0, int p1, android.view.KeyEvent p2);

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
