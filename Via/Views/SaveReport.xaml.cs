using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Via.Data;
using Via.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Via.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SaveReport : ContentPage
    {
        private ReportData _report;
        private readonly HttpClient _client = new HttpClient();
        private ViaUser user;
        private StoredReport storedReport;
        private ReportImage reportImage;
        private ReportStatus _reportStatus;
        private bool isSubmitted = false;
        public SaveReport(ReportData report, ReportStatus status)
        {
            InitializeComponent();
            _reportStatus = status;
            user = ViaSessions.GetUser();
            _report = report;
        }

        private ReportImage returnedImage;
        private async void UploadReportImages()
        {
            foreach (MediaFile mediaFile in Location._accidentPhotos)
            {
                FileStream _fileStream = new FileStream(mediaFile.Path, FileMode.Open, FileAccess.Read);
                var response = ViaAsyncTasks.UploadImage(_fileStream).Result;
                var responseContent = await response.Content.ReadAsStringAsync();
                returnedImage = JsonConvert.DeserializeObject<ReportImage>(responseContent);
                //Hide progress indicator and show the signin stack
                if (returnedImage == null)
                    return;
            }
        }

        /// <summary>
        /// Send Report via Api
        /// </summary>
        private void SaveReportData()
        {
            ShowProgress();
            //UploadReportImages();
            Task.Factory.StartNew(async () =>
            {
                var _response = ViaAsyncTasks.SendReport(_report, user).Result;
                var responseContent = await _response.Content.ReadAsStringAsync();
                storedReport = JsonConvert.DeserializeObject<StoredReport>(responseContent);
            }).ContinueWith(task =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    //Hide progress indicator
                    HideProgress();

                    reportStatus.IsVisible = true;
                    returnPrevious.IsVisible = true;
                    reportIcon.IsVisible = true;
                    try
                    {
                        if (storedReport != null) {
                            if (storedReport.QRCodeBase64 == null || storedReport.ID == "0")
                            {
                                reportIcon.Source = "ic_report_failed.png";
                                reportStatus.Text = "Report Not Sent";
                                reportStatus.TextColor = Color.Red;
                                failedReason.IsVisible = true;
                                failedReason.Text = storedReport.Message;
                                

                                tryAgain.IsVisible = true;
                                saveLocal.IsVisible = true;
                            }
                            else
                            {
                                isSubmitted = true;
                                reportIcon.Source = "ic_report_success.png";
                                reportStatus.Text = "Report Sent Successfully";
                                reportStatus.TextColor = Color.Green;
                                failedReason.IsVisible = false;

                                //Hide Buttons
                                tryAgain.IsVisible = false;
                                saveLocal.IsVisible = false;
                            }
                        }
                        else
                        {
                            reportIcon.Source = "ic_report_failed.png";
                            reportStatus.Text = "Report Not Sent";
                            reportStatus.TextColor = Color.Red;
                            failedReason.IsVisible = true;

                            tryAgain.IsVisible = true;
                            saveLocal.IsVisible = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("Exception {0}", ex.Message);
                    }
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SaveReportData();
        }

        private void GoBack(object sender, EventArgs e)
        {
            if (isSubmitted)
            {
                Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 2]);
                Navigation.RemovePage(this);
            }
            else
                Navigation.RemovePage(this);
        }

        private void ResendReport(object sender, EventArgs args)
        {
            SaveReportData();
        }

        /// <summary>
        /// Saves the report locally on phone using SqlLite
        /// </summary>
        private DatabaseManager _dbManager;
        private async void SaveLocal(object sender, EventArgs args)
        {
            try
            {
                _dbManager = new DatabaseManager();
                var status = -1;
                if(_reportStatus.Equals(ReportStatus.Create))
                    status = _dbManager.SaveReport(new SqlLiteReport { reportData = JsonConvert.SerializeObject(_report) });
                else
                    status = _dbManager.UpdateReport(_report);

                if (status > 0)
                {
                    var message = string.Format("This report has been {0} locally on your device", (_reportStatus.Equals(ReportStatus.Create)? "saved" : "updated"));
                    await DisplayAlert(null, message, "OK");
                    
                    this.Navigation.RemovePage(this.Navigation.NavigationStack[this.Navigation.NavigationStack.Count - 1]);
                    Navigation.RemovePage(this);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Demostrates the progress bar
        /// </summary>
        private void ShowProgress()
        {
            if (!sendingInProgress.IsVisible)
            {
                reportStatus.IsVisible = false;
                returnPrevious.IsVisible = false;
                reportIcon.IsVisible = false;
                tryAgain.IsVisible = false;
                saveLocal.IsVisible = false;
                failedReason.IsVisible = false;

                sendingInProgress.IsVisible = true;
                reportSaveIndicator.IsVisible = true;
                reportSaveIndicator.IsRunning = true;
            }
        }

        private void HideProgress()
        {
            reportSaveIndicator.IsRunning = false;
            sendingInProgress.IsVisible = false;
            reportSaveIndicator.IsVisible = false;
        }
    }
}