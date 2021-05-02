using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using Via.Data;
using Via.Models;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;
using Rg.Plugins.Popup.Services;
using Via.Views.Popups;
using Via.Helpers;
using System.Threading.Tasks;
using System.IO;
using Plugin.Media.Abstractions;

namespace Via.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class CreateReport : CustomContentPage, ReportPageState
    {
        public static double lat;// = -17.467686;
        public static double lng;// = 14.716677;
        public static string LND;
        private readonly HttpClient _client = new HttpClient();
        public static Control _controls = new Control();
        public static ReportData report;
       
        public static ContactArea contactArea;
        private Location _location;
        private Accident _accident;
        private Overview _overview;
        private Parties _parties;
        public static List<Item> selectedParties;
        public int draftCreated;
        public static ViaUser user;
        private ReportStage stage; //keeps track of the report stage the user is currently on
        public static ReportPageState reportPageState;
        private bool isLocationValid = false, isAccidentValid = false, ispartiesValid = false;
        public static string isLocationValidText;
        private double zoomLevel;
        public static ReportStatus _reportStatus = ReportStatus.Create;


        public CreateReport(ReportData _report = null)
        {
            InitializeComponent();

            Debug.WriteLine("Initialising createreport");
            user = null;
            user = ViaSessions.GetUser();

            selectedParties = null;
            selectedParties = new List<Item>();

            reportPageState = this;
            isLocationValidText = "";



            if (user.profile != null)
                profileAvatar.Source = new UriImageSource
                {
                    Uri = new Uri(user.profile.avatar),
                    CachingEnabled = false
                };


            if (EnableBackButtonOverride)
            {
                this.CustomBackButtonAction = () => ShowWarning();
            }

            if (_report == null)
            {
                report = new ReportData();
                report.Data = new Models.Data();
                report.Data.LocationAttributes = new List<LocationAttribute>();
                report.Data.Features = new List<ReportFeature>();
                report.Data.Parties = new List<Party>();
                report.Data.LND = LND;
                report.Data.Images = new List<ReportImage>();
                //report.Data.DateTime = DateTime.Now.Ticks;

                 DetermineCountry();
            }
            else
            {
                _reportStatus = ReportStatus.Edit;

                //Get library controls
                _controls = Report._controls;
                report = _report;

                //Get selected parties
                foreach (var party in report.Data.Parties)
                {
                    var _item = _controls.Libary.AvailableParties.Items.Find(x => x.ID == party.Id.Substring(0, party.Id.IndexOf("_", StringComparison.CurrentCulture)));
                    if (_item != null)
                    {
                        _item.IsSelected = true;
                        _item.TextColor = Color.White;
                        _item.BackgroundColor = Color.FromHex("#39b835");
                    }
                    selectedParties.Add(_item);
                }

                this.Title = "Edit Report";

                //Load location by default
                LoadLocation();
            }
        }

        private async void DetermineCountry()
        {
            if (user.status == "Expired")
            {
                ViaSessions.Logout();
                var signIn = new SignIn();
                signIn.DisplaySignInError("Your account has expired");
                Application.Current.MainPage = signIn;
                return;
            }

            CheckUserValidityAsync();

            try
            {
                var Url = string.Format("https://developapi.via.nl/report/init/Extent");
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token.token);

                var response =  await _client.GetAsync(Url);

                contactArea = JsonConvert.DeserializeObject<ContactArea>(response.Content.ReadAsStringAsync().Result);

                SelectCountry();
            }
            catch (Exception ex)
            {
                Log.Warning("Connection Exception", ex.Message);
                DetermineCountry();
            }
        }

        private void CheckUserValidityAsync()
        {
            try
            {
                Debug.WriteLine(user.token.validTo);
               
                if (DateTime.Now > Convert.ToDateTime(user.token.validTo))
                {
                    Debug.WriteLine("validating token createreport");
                    Task.Run(async () => await ViaSessions.RegenerateNewToken());
                    user = null;
                    user = ViaSessions.GetUser();
                }
            }
            catch (Exception ex)
            {
                Log.Warning("App Crash => ", ex.Message);
            }
        }

        private SelectContactArea selectContactArea;
        private void  SelectCountry()
        {
            selectContactArea = new SelectContactArea(this);
           // PopupNavigation.Instance.PushAsync(selectContactArea, true);
            Task.Run(async () => await PopupNavigation.Instance.PushAsync(selectContactArea, true));
        }

        private ActivityIndicator activityIndicator;

        private async void LoadControls()
        {
            var mapActivityIndicator = (ActivityIndicator)_location.FindByName("mapActivityIndicator");
            var isValidLocationLabel = (Label)_location.FindByName("isValidLocationLabel");
            var locationDetailLabel = (Label)_location.FindByName("locationDetailLabel");
            var mapMarker = (Image)_location.FindByName("mapMarker");
            var labelFrame = (Frame)_location.FindByName("labelFrame");
            locationDetailLabel.Text = "";
            nextPageFAB.IsVisible = false; //Hide Floating Action Button
            labelFrame.IsVisible = false;

            CheckUserValidityAsync();

            try
            {
                mapActivityIndicator.IsRunning = true;
                mapActivityIndicator.IsEnabled = true;
                isValidLocationLabel.Text = "Searching ...";
                isValidLocationLabel.TextColor = Color.FromHex("#00314b");
                var Url = string.Format("https://developapi.via.nl/report/{0}/AccidentLocation/{1}/{2}", LND, lng, lat);
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token.token);

                var response = await _client.PostAsync(Url, null);


                var content = await response.Content.ReadAsStringAsync();
                _controls = null;
                _controls = new Control();
                _controls = JsonConvert.DeserializeObject<Control>(content);

                _location.zoomLevel = zoomLevel;
                if (_controls.ErrorMessage != null)
                {
                    //_location.Lat = lat;
                    //_location.Lng = lng;
                   // _location.SetMapCoordinates(false);

                    //await DisplayAlert(null, _controls.ErrorMessage.ToString(), "OK");
                    mapActivityIndicator.IsRunning = false;
                    mapActivityIndicator.IsEnabled = false;
                    mapMarker.IsVisible = true;
                    labelFrame.IsVisible = false;
                    isValidLocationLabel.Text = $"Location is {_controls.ErrorMessage.ToString()}";
                    isValidLocationLabel.TextColor = Color.FromHex("#ed5249");

                     _location.hasMapCenterPropertyChanged = false;
                    var uploadPhotostackLayout = (StackLayout)_location.FindByName("uploadPhotos");
                    uploadPhotostackLayout.IsVisible = true;



                    Debug.WriteLine(string.Format("debug 1 {0}", _controls.ErrorMessage.ToString()));
                }
                else
                {
                    report.Data.LocationAttributes = _controls.Data.LocationAttributes;
                    _location.Lat = Convert.ToDouble(report.Data.LocationAttributes.Find(x => x.Field == "LAT").Value);
                    _location.Lng = Convert.ToDouble(report.Data.LocationAttributes.Find(x => x.Field == "LON").Value);

                    mapActivityIndicator.IsRunning = false;
                    mapActivityIndicator.IsEnabled = false;

                    _location.SetMapCoordinates(true);
                    isValidLocationLabel.Text = "";

                    if(locationDetailLabel != null)
                         locationDetailLabel.Text = Convert.ToString(report.Data.LocationAttributes.Find(x => x.Field == "NAME").Value);
                    
                    mapMarker.IsVisible = false;
                    labelFrame.IsVisible = true;
                    ShowNextPageFAB();

                    _location.ScrollToBottom();

                    _location.hasMapCenterPropertyChanged = true;
                }


                _location.isRequestPending = false;
               
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("debug 2 {0}, {1}, {2}", ex.Message, ex.StackTrace, ex.InnerException));
                _location.hasMapCenterPropertyChanged = true;
                _location.isRequestPending = false;

                //Only three requests allowed
                if(draftCreated++ < 3)
                    LoadControls();
            }
        }

        //Accident and parties Pages will be loaded after report instance has been created
        private void StartAccidentImagesUpload()
        {
            //NavigatePage(stage); //Navigate to accident page

            Task.Run(() =>
            {
                UploadImages(); //Start uploading photos if any
            });
        }

        private void LoadLocation()
        {
            try
            {
                _location = new Location();

                stage = ReportStage.Location;
                Title = pageTitle.Text = "Location";

                (ReportNav.Children[0] as Frame).BackgroundColor = Color.FromHex("#FF2ED573");
                (((ReportNav.Children[0] as Frame).Content as StackLayout).Children[1] as Image).Source = ImageSource.FromFile("ic_map_marker_white.png");
                (((ReportNav.Children[0] as Frame).Content as StackLayout).Children[2] as Label).TextColor = Color.White;

                AccidentControls.Children.Clear();
                AccidentControls.Children.Add(_location);


                var accidentDate = DateTime.Now;

                if (_reportStatus == ReportStatus.Edit) //For the report edit
                {
                    accidentDate = DateTimeOffset.FromUnixTimeMilliseconds(report.Data.DateTime).DateTime;

                    _location.Lat = Convert.ToDouble(report.Data.LocationAttributes.Find(x => x.Field == "LAT").Value);
                    _location.Lng = Convert.ToDouble(report.Data.LocationAttributes.Find(x => x.Field == "LON").Value);
                    _location.zoomLevel = 16;
                    _location.SetMapCoordinates(true);
                    nextPageFAB.IsVisible = true; //Show Floating Action Button

                    _location.ScrollToBottom();
                }

                Debug.WriteLine($"accidentDate {accidentDate}");

                _location.SetDateTime(accidentDate);

                Debug.WriteLine("Done loading");

                //Load accident attached photos if any
                _location.LoadUploadedAccidentPhotos();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception {ex}");
            }

        }

        private void OnLocationTapped(object sender, EventArgs args)
        {
            try
            {
                stage = ReportStage.Location;
                NavigatePage(ReportStage.Location);


                Debug.WriteLine("Updating Date {0} and Time {1}", _location.selectedDate, _location.selectedTime.ToString("HH:mm tt"));
                _location.SetSelectedDate(new ScrollableDate
                {
                    Day = _location.selectedDate.Date.DayOfWeek.ToString(),
                    Date = _location.selectedDate.Date.Day,
                    Month = _location.selectedDate.ToString("MMM")
                });

                if(_location.scrollableTime != null && _location.scrollableTime.Count > 0)
                     _location.SetSelectedTime(_location.scrollableTime.Find(x => x.Time == _location.selectedTime.ToString("HH:mm tt")));


                //_location.
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception {ex}");
            }





        }

        private void OnAccidentTapped(object sender, EventArgs args)
        {
            if (!_location.IsLocationValid())
                return;

            stage = ReportStage.Accident;
            NavigatePage(ReportStage.Accident);
        }

        private void OnPartiesTapped(object sender, EventArgs args)
        {
            if (!_accident.IsAccidentValid())
                return;

            if (_parties != null)
                _parties.CreateAddedPartyViews();

            stage = ReportStage.Parties;
            NavigatePage(ReportStage.Parties);
        }

        private void OnOverviewTapped(object sender, EventArgs args)
        {
            if (!_parties.ArePartiesValid())
                return;

            stage = ReportStage.Overview;
            NavigatePage(ReportStage.Overview);
        }

        public void NavigatePage(ReportStage stage)
        {
            switch (stage)
            {
                case ReportStage.Location:
                    if (isLocationValid)
                        (((ReportNav.Children[0] as Frame).Content as StackLayout).Children[0] as Image).Source = "ic_done_white.png";
                    else
                        (((ReportNav.Children[0] as Frame).Content as StackLayout).Children[0] as Image).Source = "";

                    Title = pageTitle.Text = "Location";
                    (ReportNav.Children[0] as Frame).BackgroundColor = Color.FromHex("#FF2ED573");
                    (ReportNav.Children[3] as Frame).BackgroundColor = Color.White;
                    (ReportNav.Children[1] as Frame).BackgroundColor = Color.White;
                    (ReportNav.Children[2] as Frame).BackgroundColor = Color.White;

                    (((ReportNav.Children[0] as Frame).Content as StackLayout).Children[1] as Image).Source = ImageSource.FromFile("ic_map_marker_white.png");
                    (((ReportNav.Children[3] as Frame).Content as StackLayout).Children[1] as Image).Source = ImageSource.FromFile("ic_overview.png");
                    (((ReportNav.Children[1] as Frame).Content as StackLayout).Children[1] as Image).Source = ImageSource.FromFile("ic_accident.png");
                    (((ReportNav.Children[2] as Frame).Content as StackLayout).Children[1] as Image).Source = ImageSource.FromFile("ic_parties.png");

                    (((ReportNav.Children[0] as Frame).Content as StackLayout).Children[2] as Label).TextColor = Color.White;
                    (((ReportNav.Children[3] as Frame).Content as StackLayout).Children[2] as Label).TextColor = Color.Black;
                    (((ReportNav.Children[1] as Frame).Content as StackLayout).Children[2] as Label).TextColor = Color.Black;
                    (((ReportNav.Children[2] as Frame).Content as StackLayout).Children[2] as Label).TextColor = Color.Black;
                    AccidentControls.Children.Clear();
                    AccidentControls.Children.Add(_location);
 
                    _location.LoadUploadedAccidentPhotos();
                    break;

                case ReportStage.Accident:

                    if (isAccidentValid)
                        (((ReportNav.Children[1] as Frame).Content as StackLayout).Children[0] as Image).Source = "ic_done_white.png";
                    else
                    {
                        (((ReportNav.Children[0] as Frame).Content as StackLayout).Children[0] as Image).Source = "ic_done.png";
                        (((ReportNav.Children[1] as Frame).Content as StackLayout).Children[0] as Image).Source = "";
                    }

                    Title = pageTitle.Text = "Accident";
                    (ReportNav.Children[1] as Frame).BackgroundColor = Color.FromHex("#FF2ED573");
                    (ReportNav.Children[0] as Frame).BackgroundColor = Color.White;
                    (ReportNav.Children[3] as Frame).BackgroundColor = Color.White;
                    (ReportNav.Children[2] as Frame).BackgroundColor = Color.White;

                    (((ReportNav.Children[1] as Frame).Content as StackLayout).Children[1] as Image).Source = ImageSource.FromFile("ic_accident_white.png");
                    (((ReportNav.Children[0] as Frame).Content as StackLayout).Children[1] as Image).Source = ImageSource.FromFile("ic_map_marker.png");
                    (((ReportNav.Children[3] as Frame).Content as StackLayout).Children[1] as Image).Source = ImageSource.FromFile("ic_overview.png");
                    (((ReportNav.Children[2] as Frame).Content as StackLayout).Children[1] as Image).Source = ImageSource.FromFile("ic_parties.png");

                    (((ReportNav.Children[1] as Frame).Content as StackLayout).Children[2] as Label).TextColor = Color.White;
                    (((ReportNav.Children[0] as Frame).Content as StackLayout).Children[2] as Label).TextColor = Color.Black;
                    (((ReportNav.Children[3] as Frame).Content as StackLayout).Children[2] as Label).TextColor = Color.Black;
                    (((ReportNav.Children[2] as Frame).Content as StackLayout).Children[2] as Label).TextColor = Color.Black;

                    AccidentControls.Children.Clear();
                    AccidentControls.Children.Add(_accident);

                    //responsible for uploading images in the background
                    StartAccidentImagesUpload();
                    break;
                case ReportStage.Parties:

                    if (ispartiesValid)
                        (((ReportNav.Children[2] as Frame).Content as StackLayout).Children[0] as Image).Source = "ic_done_white.png";
                    else
                    {
                        (((ReportNav.Children[1] as Frame).Content as StackLayout).Children[0] as Image).Source = "ic_done.png";
                        (((ReportNav.Children[2] as Frame).Content as StackLayout).Children[0] as Image).Source = "";
                    }

                    Title = pageTitle.Text = "Parties Involved";
                    (ReportNav.Children[2] as Frame).BackgroundColor = Color.FromHex("#FF2ED573");
                    (ReportNav.Children[0] as Frame).BackgroundColor = Color.White;
                    (ReportNav.Children[1] as Frame).BackgroundColor = Color.White;
                    (ReportNav.Children[3] as Frame).BackgroundColor = Color.White;


                    (((ReportNav.Children[2] as Frame).Content as StackLayout).Children[1] as Image).Source = ImageSource.FromFile("ic_parties_white.png");
                    (((ReportNav.Children[0] as Frame).Content as StackLayout).Children[1] as Image).Source = ImageSource.FromFile("ic_map_marker.png");
                    (((ReportNav.Children[1] as Frame).Content as StackLayout).Children[1] as Image).Source = ImageSource.FromFile("ic_accident.png");
                    (((ReportNav.Children[3] as Frame).Content as StackLayout).Children[1] as Image).Source = ImageSource.FromFile("ic_overview.png");

                    (((ReportNav.Children[2] as Frame).Content as StackLayout).Children[2] as Label).TextColor = Color.White;
                    (((ReportNav.Children[0] as Frame).Content as StackLayout).Children[2] as Label).TextColor = Color.Black;
                    (((ReportNav.Children[1] as Frame).Content as StackLayout).Children[2] as Label).TextColor = Color.Black;
                    (((ReportNav.Children[3] as Frame).Content as StackLayout).Children[2] as Label).TextColor = Color.Black;

                    AccidentControls.Children.Clear();
                    AccidentControls.Children.Add(_parties);
                    break;
                case ReportStage.Overview:
                    if (ispartiesValid)
                        (((ReportNav.Children[2] as Frame).Content as StackLayout).Children[0] as Image).Source = "ic_done.png";

                    Title = pageTitle.Text = "Overview";
                    (ReportNav.Children[3] as Frame).BackgroundColor = Color.FromHex("#FF2ED573");
                    (ReportNav.Children[0] as Frame).BackgroundColor = Color.White;
                    (ReportNav.Children[1] as Frame).BackgroundColor = Color.White;
                    (ReportNav.Children[2] as Frame).BackgroundColor = Color.White;

                    (((ReportNav.Children[3] as Frame).Content as StackLayout).Children[1] as Image).Source = ImageSource.FromFile("ic_overview_white.png");
                    (((ReportNav.Children[0] as Frame).Content as StackLayout).Children[1] as Image).Source = ImageSource.FromFile("ic_map_marker.png");
                    (((ReportNav.Children[1] as Frame).Content as StackLayout).Children[1] as Image).Source = ImageSource.FromFile("ic_accident.png");
                    (((ReportNav.Children[2] as Frame).Content as StackLayout).Children[1] as Image).Source = ImageSource.FromFile("ic_parties.png");

                    (((ReportNav.Children[3] as Frame).Content as StackLayout).Children[2] as Label).TextColor = Color.White;
                    (((ReportNav.Children[0] as Frame).Content as StackLayout).Children[2] as Label).TextColor = Color.Black;
                    (((ReportNav.Children[1] as Frame).Content as StackLayout).Children[2] as Label).TextColor = Color.Black;
                    (((ReportNav.Children[2] as Frame).Content as StackLayout).Children[2] as Label).TextColor = Color.Black;

                    _overview.partyPageState = _parties.partyPageState;
                    AccidentControls.Children.Clear();
                    AccidentControls.Children.Add(_overview);
                    break;
            }
            ShowNextPageFAB();
        }


        //When the floating action button is clicked
        private void NextStage_Tapped(object sender, EventArgs args)
        {
            try
            {
                if (stage == ReportStage.Location)
                {
                    if (_location.IsLocationValid())
                    {
                        isLocationValid = true;

                        (((ReportNav.Children[0] as Frame).Content as StackLayout).Children[0] as Image).Source = "ic_done.png";

                        _accident = _accident ?? new Accident();

                        stage = ReportStage.Accident;
                        accidentFrame.IsEnabled = true;
                    }
                }
                else if (stage == ReportStage.Accident)
                {
                    if (_accident.IsAccidentValid())
                    {
                        isAccidentValid = true;

                        (((ReportNav.Children[1] as Frame).Content as StackLayout).Children[0] as Image).Source = "ic_done.png";
                        stage = ReportStage.Parties;

                        partiesFrame.IsEnabled = true;

                        if (_parties == null)
                        {
                            _parties = new Parties();
                        }
                        else
                            _parties.CreateAddedPartyViews();
                    }
                    else
                    {
                        isAccidentValid = false;
                        return;
                    }
                }
                else if (stage == ReportStage.Parties)
                {
                    if (_parties.ArePartiesValid())
                    {
                        ispartiesValid = true;

                        (((ReportNav.Children[2] as Frame).Content as StackLayout).Children[0] as Image).Source = "ic_done.png";

                        stage = ReportStage.Overview;

                        _overview = null;
                        _overview = new Overview();
                        overviewFrame.IsEnabled = true;
                    }
                    else
                    {
                        ispartiesValid = true;
                        return;
                    }
                }

                //Navigate to the next page
                NavigatePage(stage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            ShowWarning();

            return true;
        }

        private void OnTapImageGestureRecognizerTapped(object sender, EventArgs args)
        {
            Debug.WriteLine("testsj button tap");
            ShowWarning();

        }

        //Warn user 
        private void ShowWarning()
        {
            Device.BeginInvokeOnMainThread(async () => {
                var result = await this.DisplayAlert(null, "Your report has not been saved, Do you want to exit?", "Yes", "No");
                if (result) await this.Navigation.PopAsync();
            });
        }
        /* 
         * Make NextPage Floating Action Button visible on this page/stage if it's not visible
         */
        void ShowNextPageFAB()
        {
            if (stage.Equals(ReportStage.Location) || stage.Equals(ReportStage.Accident) || stage.Equals(ReportStage.Parties))
            {
                isLocationValidText = "";
                nextPageFAB.IsVisible = true;
            }

            else
                nextPageFAB.IsVisible = false;
        }

        public void OnResumePage(string _selectedContactArea)
        {
            LND = _selectedContactArea;
            Task.Run(async () => await PopupNavigation.Instance.RemovePageAsync(selectContactArea)); //Remove popUp
            try
            {
                if (!string.IsNullOrEmpty(LND))
                    LoadLocation();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void OnSleepPage()
        {

        }

        public void OnStartPage()
        {

        }

        public void OnNavigateToPage(ReportStage stage)
        {
            this.stage = stage;
            ShowNextPageFAB();
            NavigatePage(stage);
        }

        private void UploadImages()
        {
            if (Location._accidentPhotos.Count > 0)
                foreach (MediaFile mediaFile in Location._accidentPhotos)
                    Task.Run(async () => await UploadImage(mediaFile));
        }

        //ReportImage reportImage;
        string pathFile, pathName, folderPath;
        //byte[] imageBytes;
        private async Task UploadImage(MediaFile _accidentPhoto)
        {
            var imageBytes = ConvertImage(_accidentPhoto);

            if (imageBytes.Length > 0)
                try
                {
                    var _response = ViaAsyncTasks.UploadAccidentImage(imageBytes, _controls.Data.Id.ToString(), LND, user).Result;
                    var responseContent = await _response.Content.ReadAsStringAsync();

                    var reportImage = JsonConvert.DeserializeObject<ReportImage>(responseContent);

                    if (reportImage != null)
                    {
                        report.Data.Images.Add(reportImage);


                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Exception : {ex.Message}");
                }
        }



        //byte[] readAllBytes;
        private byte[] ConvertImage(MediaFile _accidentPhoto)
        {
            try
            {
                pathFile = _accidentPhoto.Path;
                pathName = Path.GetFileName(pathFile);

                //Convert photo to binary stream
                folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                using (MemoryStream stream = new MemoryStream())
                {
                    _accidentPhoto.GetStream().CopyTo(stream);
                    _accidentPhoto.Dispose();
                    return stream.ToArray();
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exceptions {ex.Message}");
                return null;
            }


        }

        public void OnRefreshPage(ContentView view)
        {
            AccidentControls.Children.Clear();
            AccidentControls.Children.Add(view);
        }

        public void OnValidateAccidentLocation()
        {
            lat = _location.Lat;
            lng = _location.Lng;
            zoomLevel = _location.zoomLevel;
            LoadControls();
        }

        public string isValidLocationResponse()
        {
            Debug.WriteLine($"isLocationValidText: {isLocationValidText}");
            return isLocationValidText;
        }



        protected override void OnAppearing()
        {
            try
            {

            
            base.OnAppearing();

           
            if(Application.Current.MainPage is MasterDetailPage masterDetailPage)
            {
                masterDetailPage.IsGestureEnabled = false;
            }
            else if(Application.Current.MainPage is NavigationPage navigationPage && navigationPage.CurrentPage is MasterDetailPage nestedMasterDetail)
            {
                nestedMasterDetail.IsGestureEnabled = false;
            }
          
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ex" + ex.Message + ", " + ex.InnerException);
            }

        }

        protected override void OnDisappearing()
        {
            try
            {

            
            base.OnDisappearing();

            
                if(Xamarin.Forms.Application.Current.MainPage is MasterDetailPage masterDetailPage)
                {
                    masterDetailPage.IsGestureEnabled = true;
                }
                else if(Xamarin.Forms.Application.Current.MainPage is NavigationPage navigationPage && navigationPage.CurrentPage is MasterDetailPage nestedMasterDetail)
                {
                    nestedMasterDetail.IsGestureEnabled = true;
                }
            
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ex" + ex.Message + ", " + ex.InnerException);
            }

        }



    }
}