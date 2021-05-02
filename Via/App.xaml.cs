using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Plugin.LocalNotifications;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Via.Data;
using Via.Models;
using Via.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

 [assembly: XamlCompilation(XamlCompilationOptions.Compile)]


namespace Via
{
    
    public partial class App : Application
    {
        public static NavigationPage NavigationPage { get; private set; }
        public App()
        {
            InitializeComponent();

            //Page MenuPage = new MenuPage();
            NavigationPage = new NavigationPage(new SplashScreen());

            //Page RootPage = new RootPage
            //{
            //    Master = MenuPage,
            //    Detail = NavigationPage
            //};
            MainPage = NavigationPage;
        
        }

        protected override void OnStart()
        {
            try
            {
                Task.Run(async () =>
                { 
                    await DisplayNotifications();
                }
                );
                // Handle when your app resumes
            }
            catch(Exception ex)
            {

            }
            
        }

        protected override void OnSleep()
        {
            try
            {
                Task.Run(async () =>
                { 
                    await DisplayNotifications();
                }
               );
            }
            catch(Exception ex)
            {

            }
        }

        protected  override void OnResume()
        {
            try
            {
                Task.Run(async () =>
                { 
                    await DisplayNotifications();
                }
               );
            }
            catch(Exception ex)
            {

            }
        }

        
        private async Task DisplayNotifications()
        {
            try
            {
                await Task.Factory.StartNew(() =>
                 {
                 //Declare a local database manager.     
                 var user = new ViaUser();
                     var databaseManager = new DatabaseManager();



                 //Check if there's data in Sqlite.
                 if (databaseManager.GetReports().Count > 0)
                     {
                     //Setting time to 6:00 am (Morning hours).
                     var strDate = "06:00:00";
                         var date = DateTime.ParseExact(strDate, "HH:mm:ss", CultureInfo.InvariantCulture);

                         var rnd = new Random();
                         var randomGenerator = rnd.Next(1, 1000);
                         Debug.WriteLine("Creating Push notification");

                     //To be fixed later

                     //CrossLocalNotifications.Current.Show("Via Accident Reporting",
                     //    "Hey " + user.profile.fullname + " you have an unsent incident report. \n Send now.",
                     //    randomGenerator, date);
                 }
                 }).ContinueWith(task => { Device.BeginInvokeOnMainThread(() => { }); },
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch(Exception ex)
            {

            }
        }
    }
}