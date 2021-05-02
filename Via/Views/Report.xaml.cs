using Naxam.Controls.Mapbox.Forms;
using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Via.Data;
using Via.Helpers;
using Via.Models;
using Via.Views.Popups;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Via.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Report : ContentPage, ReportState
    {
        private ReportData report;
        private Label label;
        private int sqlLiteID;
        public static Control _controls = new Control();
        private ViaUser user;
        private readonly HttpClient _client = new HttpClient();
        private double _lat, _long;

        public Report (int sqlLiteID, ReportData report)
		{
            this.report = report;
            this.sqlLiteID = sqlLiteID;
            this.user = ViaSessions.GetUser();

            _controls = null;
            _lat = Convert.ToDouble(report.Data.LocationAttributes.Find(x => x.Field == "LAT").Value);
            _long = Convert.ToDouble(report.Data.LocationAttributes.Find(x => x.Field == "LON").Value);

            InitializeComponent();
            LocationOverview();
            GetControls();
        }

        private async void GetControls()
        {
            reportActivityIndicator.IsVisible = true;
            reportActivityIndicator.IsRunning = true;
            reportDetails.IsVisible = false;
            CheckUserValidity();

            try
            {
                var Url = string.Format("https://developapi.via.nl/report/{0}/AccidentLocation/{1}/{2}", report.Data.LND, _lat, _long);
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token.token);

                var response = await _client.PostAsync(Url, null);

                var content = await response.Content.ReadAsStringAsync();
                _controls = JsonConvert.DeserializeObject<Control>(content);
                
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        reportActivityIndicator.IsRunning = false;
                        reportActivityIndicator.IsVisible = false;
                        reportDetails.IsVisible = true;
                        if (_controls != null)
                            AccidentOverview();
                    });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("debug", ex.Message));
                GetControls();
            }
        }

        private async void CheckUserValidity()
        {
            if (DateTime.Now > Convert.ToDateTime(user.token.validTo))
            {
                await ViaSessions.RegenerateNewToken();
                user = null;
                user = ViaSessions.GetUser();
            }
        }

        private ActivityIndicator activityIndicator;
        private void ShowProgressLoader()
        {
            activityIndicator = new ActivityIndicator
            {
                IsRunning = true,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
        }

        /// <summary>
        /// Overview on Location Details
        /// </summary>
        public void LocationOverview()
        {
            var location = report.Data.LocationAttributes.Find(x => x.Field == "NAME");
            selectedLocation.Text = (location.Value.Length > 10) ? location.Value.Substring(0, 10) + "..." : location.Value;

            reportID.Text = report.Data.Id.ToString();
            
            DateTime accDateTime = DateTimeOffset.FromUnixTimeMilliseconds(report.Data.DateTime).DateTime;

            accidentDateTime.Text = accDateTime.Day + " " + accDateTime.ToString("MMM") + " " + accDateTime.ToString("HH:mm tt");

            try
            {
                map.HeightRequest = 100.0;
                map.ZoomLevel = 13;
                map.Scale = 1;
                map.MapStyle = new MapStyle("mapbox://styles/wamos/ck3idx14f1uie1cmjhqw91613");
                map.Center = new Position { Lat = _lat, Long = _long};

                var annotations = new ObservableCollection<Annotation>
                {
                    new PointAnnotation
                    {
                        Title = location.Value,
                        Coordinate = new Position { Lat = _lat, Long = _long}
                    }
                };
                map.Annotations = annotations;
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Overview on Accident Details as selected by the reporter
        /// </summary>
        private void AccidentOverview()
        {
            try
            {
                Grid grid = new Grid
                {
                    VerticalOptions = LayoutOptions.Center,
                    ColumnDefinitions =
                {
                    new ColumnDefinition {Width = new GridLength(.5, GridUnitType.Star)},
                    new ColumnDefinition {Width = new GridLength(.5, GridUnitType.Star)}
                }
                };
                for (var feature = 0; feature < report.Data.Features.Count; feature++)
                {
                    //Get actual texts of selected values
                    var control = _controls.Libary.Features.Find(x => x.Id == report.Data.Features[feature].Id);
                    var selectedItems = "";
                    foreach (var value in report.Data.Features[feature].Values)
                    {
                        if (control.Items != null && !string.IsNullOrEmpty(value) && !value.Contains("-")) //Neither unknowm nor inapplicable was selected
                        {
                            selectedItems += (selectedItems == "") ? control.Items.Find(x => x.Id == value).Title : ", " + control.Items.Find(x => x.Id == value).Title;
                        }
                        else
                            selectedItems = value;
                    }


                    label = new Label
                    {
                        Text = control.Title,
                        TextColor = Color.FromHex("#00314b"),
                        FontSize = 17,
                        FontAttributes = FontAttributes.Bold,
                        Margin = new Thickness(0, 0, 0, 1),
                        FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
                    };
                    grid.Children.Add(label, 0, feature);


                    label = new Label
                    {
                        Text = selectedItems,
                        FontSize = 17,
                        Margin = new Thickness(0, 0, 0, 1),
                        FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-NormalBold" : "TitilliumWeb-NormalBold.ttf"
                    };
                    grid.Children.Add(label, 1, feature);
                }

                accidentOverview.Children.Add(grid);

                accidentPhotos.ItemsSource = report.Data.Images;
                //Load attached photos
                //LoadAccidentPhotos();
            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// Load accident photos attached if any
        /// </summary>
        //public void LoadAccidentPhotos()
        //{
        //    accidentImagesContainer.Children.Clear(); //Remove all existing images from the images container
        //    if (Location._accidentPhotos.Count > 0)
        //        foreach (MediaFile mediaFile in Location._accidentPhotos)
        //            AddImageToCollection(mediaFile);
        //}

        //public void AddImageToCollection(MediaFile _accidentPhoto)
        //{
        //    if (accidentImagesContainer.Children.Count > 3)
        //    {
        //        extraAccidentPhotosFrame.IsVisible = true;
        //        extraAccidentPhotosLabel.Text = string.Format("+{0}", (Location._accidentPhotos.Count - 4));
        //    }
        //    else
        //    {
        //        AbsoluteLayout _absoluteLayout = new AbsoluteLayout();
        //        _absoluteLayout.GestureRecognizers.Add(
        //                        new TapGestureRecognizer()
        //                        {
        //                            Command = new Command(async () => {
        //                                await Navigation.PushAsync(new ImageEnlarge(Location._accidentPhotos.IndexOf(_accidentPhoto)));
        //                            })
        //                        });
        //        Image _accidentImage = new Image
        //        {
        //            Source = ImageSource.FromStream(() =>
        //            {
        //                var stream = _accidentPhoto.GetStream();
        //                return stream;
        //            }),
        //            HeightRequest = 55,
        //            WidthRequest = 55,
        //            Aspect = Aspect.Fill
        //        };

        //        Frame _frame = new Frame
        //        {
        //            CornerRadius = 8,
        //            Padding = 0,
        //            HasShadow = false,
        //            Margin = new Thickness(0, 5, 5, 0)
        //        };

        //        //Add Deletion button
        //        ContentView _deleteContentView = new ContentView();
        //        Image _deleteImage = new Image
        //        {
        //            Source = "ic_remove.png",
        //            HeightRequest = 20,
        //            WidthRequest = 20
        //        };
        //        _deleteContentView.Content = _accidentImage;

        //        _frame.Content = _accidentImage;
        //        _absoluteLayout.Children.Add(_frame, new Rectangle(0f, 0f, 1, 1), AbsoluteLayoutFlags.All);
        //        _absoluteLayout.Children.Add(_deleteContentView, new Rectangle(1f, 0f, -1, -1), AbsoluteLayoutFlags.PositionProportional);
        //        accidentImagesContainer.Children.Add(_absoluteLayout);
        //    }
        //}

        private ConfirmDelete confirmDelete;
        private async void DeleteReport(object sender, EventArgs args)
        {
            try
            {
                confirmDelete = new ConfirmDelete(this);
                await Task.Run(async () => await PopupNavigation.Instance.PushAsync(confirmDelete, true));
            }
            catch(Exception ex)
            {

            }
        }

        private void CloseReport(object sender, EventArgs args)
        {
            Navigation.RemovePage(this);
        }

        private async void ViewReportOverview(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new ReportOverview(report));
        }

        private async void EditReport(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new CreateReport(report));
        }

        public void OnReportDeleted()
        {
            try
            {
                var result = new DatabaseManager().DeleteReport(new SqlLiteReport { reportID = sqlLiteID, reportData = JsonConvert.SerializeObject(report) });
                if (result > 0)
                {
                    Task.Run(async () => await PopupNavigation.Instance.RemovePageAsync(confirmDelete));
                    Navigation.RemovePage(this);
                }
            }
            catch(Exception ex)
            {

            }
        }

        public void OnResumePage(bool o)
        {
            throw new NotImplementedException();
        }

        public void OnSleepPage()
        {
            throw new NotImplementedException();
        }

        public void OnStartPage()
        {
            throw new NotImplementedException();
        }
    }
}