using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.CurrentActivity;

namespace Via.Droid
{
    [Application]
    public class MainApplication : Application, Application.IActivityLifecycleCallbacks
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {

        }

        public override void OnCreate()
        {
            base.OnCreate();
            try
            {
                RegisterActivityLifecycleCallbacks(this);
            }
            catch(Exception ex)
            {

            }
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            try
            {
                UnregisterActivityLifecycleCallbacks(this);
            }
            catch(Exception ex)
            {

            }
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            try
            {
                CrossCurrentActivity.Current.Activity = activity;
            }
            catch(Exception ex)
            {

            }
        }

        public void OnActivityDestroyed(Activity activity)
        {
            
        }

        public void OnActivityPaused(Activity activity)
        {
            
        }

        public void OnActivityResumed(Activity activity)
        {
            try
            {
                CrossCurrentActivity.Current.Activity = activity;
            }
            catch(Exception ex)
            {

            }
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            
        }

        public void OnActivityStarted(Activity activity)
        {
            try
            {
                CrossCurrentActivity.Current.Activity = activity;
            }
            catch(Exception ex)
            {

            }
        }

        public void OnActivityStopped(Activity activity)
        {
            
        }
    }
}