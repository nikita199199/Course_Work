package crc64eeff1b3d54fe8840;


public class MyInfoWindowAdapter
	extends java.lang.Object
	implements
		mono.android.IGCUserPeer,
		com.google.android.gms.maps.GoogleMap.InfoWindowAdapter
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_getInfoContents:(Lcom/google/android/gms/maps/model/Marker;)Landroid/view/View;:GetGetInfoContents_Lcom_google_android_gms_maps_model_Marker_Handler:Android.Gms.Maps.GoogleMap/IInfoWindowAdapterInvoker, Xamarin.GooglePlayServices.Maps\n" +
			"n_getInfoWindow:(Lcom/google/android/gms/maps/model/Marker;)Landroid/view/View;:GetGetInfoWindow_Lcom_google_android_gms_maps_model_Marker_Handler:Android.Gms.Maps.GoogleMap/IInfoWindowAdapterInvoker, Xamarin.GooglePlayServices.Maps\n" +
			"";
		mono.android.Runtime.register ("Proba.MyInfoWindowAdapter, Proba", MyInfoWindowAdapter.class, __md_methods);
	}


	public MyInfoWindowAdapter ()
	{
		super ();
		if (getClass () == MyInfoWindowAdapter.class)
			mono.android.TypeManager.Activate ("Proba.MyInfoWindowAdapter, Proba", "", this, new java.lang.Object[] {  });
	}

	public MyInfoWindowAdapter (crc64eeff1b3d54fe8840.MainActivity p0, java.lang.String p1)
	{
		super ();
		if (getClass () == MyInfoWindowAdapter.class)
			mono.android.TypeManager.Activate ("Proba.MyInfoWindowAdapter, Proba", "Proba.MainActivity, Proba:System.String, mscorlib", this, new java.lang.Object[] { p0, p1 });
	}


	public android.view.View getInfoContents (com.google.android.gms.maps.model.Marker p0)
	{
		return n_getInfoContents (p0);
	}

	private native android.view.View n_getInfoContents (com.google.android.gms.maps.model.Marker p0);


	public android.view.View getInfoWindow (com.google.android.gms.maps.model.Marker p0)
	{
		return n_getInfoWindow (p0);
	}

	private native android.view.View n_getInfoWindow (com.google.android.gms.maps.model.Marker p0);

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
