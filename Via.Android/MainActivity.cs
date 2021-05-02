using System;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Com.Mapbox.Mapboxsdk;
using Com.Mapbox.Mapboxsdk.Annotations;
using Com.Mapbox.Mapboxsdk.Geometry;
using Com.Mapbox.Mapboxsdk.Maps;
using FFImageLoading;
using FFImageLoading.Forms.Platform;
using ImageCircle.Forms.Plugin.Droid;
using Plugin.CurrentActivity;
using Plugin.InputKit.Platforms.Droid;
using Plugin.LocalNotifications;
using Plugin.Permissions;
using Rg.Plugins.Popup;
using Via.Views;
using Via.Utils;
using Via.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Com.Mapbox.Mapboxsdk.Maps.MapboxMap;
using Color = Android.Graphics.Color;
using Object = Java.Lang.Object;
using Platform = Stormlion.ImageCropper.Droid.Platform;
using View = Android.Views.View;

namespace Via.Droid
{
    [Activity(Label = "Via", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                TabLayoutResource = Resource.Layout.Tabbar;
                ToolbarResource = Resource.Layout.Toolbar;

                // Initailize the cropper tool.
                Platform.Init();

                base.OnCreate(savedInstanceState);

                Config.Init(this, savedInstanceState);

                Popup.Init(this, savedInstanceState);

                Forms.Init(this, savedInstanceState);

                //Initialize Image Circle Plugin
                ImageCircleRenderer.Init();

                //NotificationCenter

                LoadApplication(new App());

                Android.Support.V7.Widget.Toolbar toolbar = this.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    SetSupportActionBar(toolbar);


                }

                //Initialize image cached renderer.
                CachedImageRenderer.Init(true);

                // Initailize the camera plugin.
                CrossCurrentActivity.Current.Init(this, savedInstanceState);

                //Initialize mapbox in Android.
                Mapbox.GetInstance(this, MapBoxService.AccessToken);

                // Initialize local notifications.
                LocalNotificationsImplementation.NotificationIconId = Resource.Mipmap.icon;
            }
            catch(Exception ex)
            {

            }
        }

        //protected override void OnResume()
        //{
        //    Android.Support.V7.Widget.Toolbar toolbar = this.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
        //    if (toolbar != null)
        //        SetSupportActionBar(toolbar);
        //}


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            Permission[] grantResults)
        {
            try
            {
                PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
            catch(Exception ex)
            {

            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            try
            {
                Platform.OnActivityResult(requestCode, resultCode, data);
            }
            catch(Exception ex)
            {

            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            // check if the current item id 
            // is equals to the back button id
            try
            {
                Console.WriteLine($"item.ItemId : {item.ItemId}");
                if (item.ItemId == 16908332)
                {
                    // retrieve the current xamarin forms page instance
                    var currentpage = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault() as CustomContentPage;

                    // check if the page has subscribed to 
                    // the custom back button event
                    if (currentpage?.CustomBackButtonAction != null)
                    {
                        // invoke the Custom back button action
                        currentpage?.CustomBackButtonAction.Invoke();
                        // and disable the default back button action
                        return false;
                    }

                    // if its not subscribed then go ahead 
                    // with the default back button action
                    return base.OnOptionsItemSelected(item);
                }
                else
                {
                    // since its not the back button 
                    //click, pass the event to the base
                    return base.OnOptionsItemSelected(item);
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public override void OnBackPressed()
        {
            // this is not necessary, but in Android user 
            // has both Nav bar back button and
            // physical back button its safe 
            // to cover the both events

            // retrieve the current xamarin forms page instance
            try
            {
                var currentpage = Xamarin.Forms.Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault() as CustomContentPage;


                // check if the page has subscribed to 
                // the custom back button event
                if (currentpage?.CustomBackButtonAction != null)
                {
                    currentpage?.CustomBackButtonAction.Invoke();
                }
                else
                {
                    base.OnBackPressed();
                }
            }
            catch(Exception ex)
            {

            }
        }
    }

    public class MapReady : Object, IOnMapReadyCallback
    {
        private readonly Context _context;

        public MapReady(Context context)
        {
            _context = context;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void OnMapReady(MapboxMap mapboxMap)
        {
            mapboxMap.AddMarker(new MarkerOptions().SetPosition(new LatLng(40.416717, -3.703771))
                .SetTitle("xxxxspain"));
            mapboxMap.InfoWindowAdapter = new InfoWindowAdapter(_context);
        }
    }

    public class InfoWindowAdapter : Object, IInfoWindowAdapter
    {
        private readonly Context _context;

        public InfoWindowAdapter(Context c)
        {
            _context = c;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public View GetInfoWindow(Marker marker)
        {
            var parent = new LinearLayout(_context);
            parent.LayoutParameters =
                new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
            parent.Orientation = Orientation.Vertical;
            parent.SetBackgroundColor(Color.Red);
            var txtTittle = new TextView(_context);
            var countryFlagImage = new ImageView(_context);
            switch (marker.Title)
            {
                case "spain":
                    txtTittle.SetText(marker.Title, TextView.BufferType.Normal);
                    countryFlagImage.SetImageDrawable(ContextCompat.GetDrawable(
                        _context, Resource.Drawable.mapbox_logo_icon));
                    break;
                case "egypt":
                    txtTittle.SetText(marker.Title, TextView.BufferType.Normal);
                    countryFlagImage.SetImageDrawable(ContextCompat.GetDrawable(
                        _context, Resource.Drawable.mapbox_logo_icon));
                    break;
                default:
                    txtTittle.SetText(marker.Title, TextView.BufferType.Normal);
                    countryFlagImage.SetImageDrawable(ContextCompat.GetDrawable(
                        _context, Resource.Drawable.mapbox_logo_icon));
                    break;
            }

            countryFlagImage.LayoutParameters = new ViewGroup.LayoutParams(150, 100);
            txtTittle.LayoutParameters = new ViewGroup.LayoutParams(150, 100);

            parent.AddView(countryFlagImage);
            parent.AddView(txtTittle);

            return parent;
        }
    }

    
}