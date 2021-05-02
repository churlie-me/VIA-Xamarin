using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Permissions.Abstractions;
using Via.Data;
using Via.Models;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using Settings = Via.Utils.Settings;

namespace Via.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccidentReports : CustomContentPage
    {
        public static ViaUser user;
        public DatabaseManager _dbManager = new DatabaseManager();
        private List<Models.Report> reports;
        private List<ReportData> reportsData;
        private List<SqlLiteReport> savedreports;
        public AccidentReports()
        {
            InitializeComponent();
        }

       private void AccidentReportsInit()
        {
            try
            {
                user = null;
                user = ViaSessions.GetUser();
                reportsData = new List<ReportData>();

                GetLocallySavedReports();

                if (user.profile != null)
                    profileAvatar.Source = new UriImageSource
                    {
                        Uri = new Uri(user.profile.avatar),
                        CachingEnabled = true,
                        CacheValidity = new TimeSpan(0, 0, 1, 0)

                    };

                currentDay.Text = DateTime.Now.DayOfWeek + " ";
                currentDate.Text = DateTime.Now.Day + " " + DateTime.Now.ToString("MMM");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ex" + ex.Message + ", " + ex.InnerException);
            }
        }

        private void ShowNavigationDrawer(object sender, EventArgs args)
        {
            try
            {
                if (Application.Current.MainPage is MasterDetailPage masterDetailPage)
                {
                    masterDetailPage.IsPresented = true;
                }
                else if (Application.Current.MainPage is NavigationPage navigationPage && navigationPage.CurrentPage is MasterDetailPage nestedMasterDetail)
                {
                    nestedMasterDetail.IsPresented = true;
                }

                Debug.WriteLine("gere");
            }
            catch(Exception ex)
            {
                Debug.WriteLine("ex" + ex.Message + ", " + ex.InnerException);
            }
                
           
           
        }

        private async void CreateReport(object sender, EventArgs args)
        {
            try
            {
                Debug.WriteLine("Am here waiting");
                var hasLocationPermission = await Utils.Utils.CheckPermissions(Permission.Location);

                if (hasLocationPermission)
                {
                    Debug.WriteLine("Am here done");
                    //App.NavigationPage.BarBackgroundColor = Color.Default;
                    await Navigation.PushAsync(new CreateReport());
                }
                else
                {
                    Debug.WriteLine($"No location Permissions");
                }

              
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ex" + ex.Message + ", " + ex.InnerException);
            }
        }

        private async void ViewReport(object sender, ItemTappedEventArgs args)
        {
            try
            { 
                Models.Report report = args.Item as Models.Report;
                App.NavigationPage.BarBackgroundColor = Color.Default;
                await Navigation.PushAsync(new Report(savedreports[reports.IndexOf(report)].reportID, reportsData.Find(x => x.Data.Id.ToString() == report.ID)));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ex" + ex.Message + ", " + ex.InnerException);
            }
        }

        public void GetLocallySavedReports()
        {
            //Hide list
            accidentReports.IsVisible = false;
            savedReportsComment.IsVisible = false;
            processIndicator.IsVisible = true;
            processIndicator.IsRunning = true;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    savedreports = _dbManager.GetReports();
                    reports = new List<Models.Report>();
                    foreach (var report in savedreports)
                    {
                        ReportData reportData = JsonConvert.DeserializeObject<ReportData>(report.reportData);
                        DateTime dateResult = DateTimeOffset.FromUnixTimeMilliseconds(reportData.Data.DateTime).DateTime;

                        var location = reportData.Data.LocationAttributes.Find(x => x.Field == "NAME");

                        //Console.WriteLine($"Image Count: {reportData.Data.Images.Count}");

                        reports.Add(new Models.Report
                        {
                            ID = reportData.Data.Id.ToString(),
                            DateTime = dateResult,
                            Images = reportData.Data.Images,
                            Location =  (location.Value.Length > 14)? location.Value.Substring(0,13) + "..." : location.Value
                        });

                        reportsData.Add(reportData);
                    }
                }
                catch (Exception ex)
                {
                    Log.Warning("SqlLite Exception ", ex.Message);
                }
            }).ContinueWith(task =>
            {
                //DoSomethingOnTheUIThread();
                // the following forces the code in the ContinueWith block to be run on the
                //Display list


                
                processIndicator.IsVisible = false;
                processIndicator.IsRunning = false;

                if (accidentReports.IsRefreshing == true)
                    accidentReports.IsRefreshing = false;

                if (reports.Count > 0)
                {
                    accidentReports.IsVisible = true;
                    savedReportsComment.IsVisible = false;
                    accidentReports.ItemsSource = reports;

                    
                }
                else
                {
                    accidentReports.IsVisible = false;
                    savedReportsComment.IsVisible = true;
                }
                // calling thread, often the Main/UI thread.
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                AccidentReportsInit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ex" + ex.Message + ", " + ex.InnerException);
            }
}
    }
}