using Foundation;
using ImageCircle.Forms.Plugin.iOS;
using Mapbox;
using Naxam.Controls.Mapbox.Platform.iOS;
using Plugin.InputKit.Platforms.iOS;
using Rg.Plugins.Popup;
using UIKit;
using UserNotifications;
using Via.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Platform = Stormlion.ImageCropper.iOS.Platform;
using FabricSdk;
using CrashlyticsKit;
using System;

namespace Via.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Popup.Init();

            MGLAccountManager.AccessToken = MapBoxService.AccessToken;

            Forms.Init();
            
            Platform.Init();

            //Initialize Image Circle Plugin
            ImageCircleRenderer.Init();

            LoadApplication(new App());

            // Initialize input kit for IOS.
            Config.Init();

            

            FFImageLoading.Forms.Platform.CachedImageRenderer.Init();


            // Initialize push notifications.
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                // Ask the user for permission to get notifications on iOS 10.0+
                UNUserNotificationCenter.Current.RequestAuthorization(
                    UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
                    (approved, error) => { });
                // Watch for notifications while app is active
                UNUserNotificationCenter.Current.Delegate = new UserNotificationCenterDelegate();
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                // Ask the user for permission to get notifications on iOS 8.0+
                var settings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                    new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            }

            Crashlytics.Instance.Initialize();
            FabricSdk.Fabric.Instance.Initialize();


            foreach (var font in UIFont.FamilyNames)
            {
                foreach (var item in UIFont.FontNamesForFamilyName(font))
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("-----------");
            }

            return base.FinishedLaunching(app, options);
        }

        
    }
}