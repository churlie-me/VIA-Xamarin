using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using Naxam.Controls.Mapbox.Forms;
using Newtonsoft.Json;
using Plugin.Media.Abstractions;
using Rg.Plugins.Popup.Services;
using Via.Controls;
using Via.Models;
using Via.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Via.Views.Popups;
using Via.Helpers;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;
using Plugin.Permissions.Abstractions;
using System.ComponentModel;
using Plugin.Geolocator;

namespace Via.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class Location : ContentView, LocationPageState
    {
        private readonly HttpClient _client = new HttpClient();
        private ObservableCollection<Result> _listOfLocations;
        //private ReportData report;
        private double _counter = 13;
        //private ObservableCollection<Result> _saveLocations;
        private Result item;
        public DateTime selectedDate, selectedTime;
        private List<ScrollableDate> scrollableDates = new List<ScrollableDate>();
        private List<DateTime> selectedDateRange = new List<DateTime>();
        public List<ScrollableTime> scrollableTime = new List<ScrollableTime>();
        //private string DateOfAccident;
        //private DateTime reportDraftCreated = DateTime.Now;
        public static List<MediaFile> _accidentPhotos = new List<MediaFile>();
        public static List<MediaFile> _removedAccidentPhotosTemp = new List<MediaFile>();
        public double Lat, Lng;
        public double zoomLevel;
        private ReportPageState reportPageState = CreateReport.reportPageState;
        public bool isRequestPending = false;
        private bool isMapLoaded = false;
        public bool hasMapCenterPropertyChanged = false;

        public Location()
        {
            InitializeComponent();
            try
            {
                BindingContext = this;
                accidentDatePicker.MaximumDate = DateTime.Now;


                map.GestureRecognizers.Add(new TapGestureRecognizer()
                {
                    Command = new Command(() =>
                    {
                        MapTapped();
                    })
                });
                map.MapStyle = new MapStyle("mapbox://styles/wamos/ck3idx14f1uie1cmjhqw91613");
                
                map.PropertyChanged += MapBoxPropertyChanged;

                map.RotateEnabled = false;



                map.DragFinishedCommand = new Command(() =>
                {
                    VerifyLocation();
                });

                map.DidTapOnMapCommand = new Command<object>((mapTapped) =>
                {
                    VerifyLocation();
                    //hasMapCenterPropertyChanged = false;
                });

                map.RegionDidChangeCommand = new Command<object>(async(regionChanged) =>
                {
                    

                    Debug.WriteLine("regionChanged: " + regionChanged);

                    hasMapCenterPropertyChanged = ((bool)regionChanged);

                   //if(Device.RuntimePlatform == Device.Android && regionChanged.Equals(false))
                   // {
                         
                   //         await Task.Delay(1000);
                   //         VerifyLocation();
                        
                   // }

                });

                map.DragFinishedCommand = new Command<object>(ExecuteDragFinishedCommand, CanExecuteDragFinishedCommand);

                map.DidFinishLoadingStyleCommand = new Command<MapStyle>((obj) =>
                {
                    isMapLoaded = true;
                });

                MessagingCenter.Subscribe<ImageEnlarge, List<MediaFile>>(this, "ReloadAccidentPhotoView", (sender, arg) => { ReloadAttachedAccidentImages(arg); });
            }
            catch (Exception ex)
            {
                Log.Warning("Maps Exception : {0}", ex.Message);
            }

            _accidentPhotos = null;
            _accidentPhotos = new List<MediaFile>();
        }


        private void MapTapped()
        {

        }

        private void MapBoxPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                var mapView = (Naxam.Controls.Mapbox.Forms.MapView)sender;

                Debug.WriteLine("Property has changed" + e.PropertyName);

                if (isMapLoaded && !hasMapCenterPropertyChanged && !isRequestPending && !e.PropertyName.Equals("RotatedDegree"))
                {
                    Debug.WriteLine("Property has changed" + e.PropertyName + " isMapLoaded" + isMapLoaded + "ZoomLevel " + map.ZoomLevel);

                    if (e.PropertyName.Equals("Center"))
                    {

                        if (map.ZoomLevel < 14)
                            map.ZoomLevel += 1;
                        else
                            VerifyLocation();
                    }

                    if (Device.RuntimePlatform == Device.Android && e.PropertyName.Equals("ZoomLevel"))
                    {
                        if (map.ZoomLevel > 14)
                            VerifyLocation();
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        bool CanExecuteDragFinishedCommand(object parameter)
        {
            return true;
        }

        void ExecuteDragFinishedCommand(object parameter)
        {
            VerifyLocation();
        }

        private void VerifyLocation()
        {
            try
            {
                if (map.ZoomLevel > 14)
                {
                    mapMarker.IsVisible = true;
                    //labelFrame.IsVisible = false;

                    if (!isRequestPending)
                    {
                        if (!(Lat.Equals(map.Center.Lat) && Lng.Equals(map.Center.Long)))
                        {
                            isRequestPending = true;
                            Lat = map.Center.Lat;
                            Lng = map.Center.Long;
                            zoomLevel = map.ZoomLevel;

                            Device.BeginInvokeOnMainThread(() =>
                            {
                                reportPageState.OnValidateAccidentLocation();
                            });
                        }
                    }

                }
                else
                {
                    mapMarker.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Fire due to events", ex.Message);
            }
        }

        public void SetMapCoordinates(bool isVisible)
        {
            mapMarker.IsVisible = true;
            map.ZoomLevel = zoomLevel;
            map.Center = new Position { Lat = Lat, Long = Lng };

            uploadPhotos.IsVisible = IsVisible;
        }

        public void LoadAccidentPhotos()
        {
            accidentImagesContainer.Children.Clear(); //Remove all existing images from the images container
            if (_accidentPhotos.Count > 0)
                foreach (MediaFile mediaFile in _accidentPhotos)
                    AddImageToCollection(mediaFile);
        }

        public void SetDateTime(DateTime dateTime)
        {
            try
            {

                selectedDate = selectedTime = dateTime;
                GetDates(selectedDate);
                accidentDates.ItemsSource = null;
                accidentDates.ItemsSource = scrollableDates;

                accidentDates.SelectedItem = selectedDate.Date;

                var dateIndex = selectedDateRange.IndexOf(selectedDate.Date);

                SetSelectedDate(new ScrollableDate
                {
                    IsSelected = true,
                    Day = selectedDate.Date.DayOfWeek.ToString(),
                    Date = selectedDate.Date.Day,
                    Month = selectedDate.ToString("MMM")
                });

                accidentTimePicker.Time = selectedTime.TimeOfDay;
                GetTimeIntervals(selectedDate);

                accidentTimes.ItemsSource = null;
                accidentTimes.ItemsSource = scrollableTime;
                var _sTime = scrollableTime.Find(x => x.Time == selectedDate.ToString("HH:mm tt").ToLower());

                SetSelectedTime(_sTime);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception on SetDateTime:  {ex.Message}");
            }

        }

        public void SetSelectedDate(ScrollableDate selectedItem)
        {
            try
            {

                if (accidentDates.ItemsSource == null)
                    return;

                foreach (ScrollableDate item in accidentDates.ItemsSource)
                {
                    if (item.Day.Equals(selectedItem.Day) && item.Date.Equals(selectedItem.Date) && item.Month.Equals(selectedItem.Month))
                    {

                        Debug.WriteLine("Am here {0}", selectedItem.Date);

                        selectedItem.IsSelected = true;
                        selectedItem.ItemTextColor = Color.White;
                        selectedItem.ItemTopBackgroundColor = Color.FromHex("#1f536c");
                        selectedItem.ItemBottomBackgroundColor = Color.FromHex("#003552");
                        selectedItem.ItemSeparatorColor = Color.FromHex("#1f536c");
                        selectedItem.clickCount += 1;
                        selectedItem.OnPropertyChanged();

                        var deviceWidth = Application.Current.MainPage.Width;
                        var dateElCount = deviceWidth / 100;
                        var hListViewY = accidentDates.Y;
                        var scrollEnabledCount = dateElCount / 3;

                        if (scrollableDates.IndexOf(item) > scrollEnabledCount)
                        {
                            accidentDates.ScrollToAsync((scrollableDates.IndexOf(item) - scrollEnabledCount) * 100, hListViewY, true);
                        }

                    }

                    //All unselected items should have a click count of 0
                    if (!item.Equals(selectedItem))
                        item.clickCount = 0;

                    //An item should be clicked more than once to show the date picker popup
                    if (item.clickCount > 1)
                    {
                        accidentDatePicker.Focus();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception on SetSelectedDate: {ex.Message}, {ex.InnerException}, {ex.StackTrace}");
            }
        }


        public void SetSelectedTime(ScrollableTime selectedItem)
        {
            try
            {


                if (accidentTimes.ItemsSource == null)
                    return;

                if (selectedItem == null)
                    return;

                var itemIndex = scrollableTime.IndexOf(selectedItem);

                foreach (ScrollableTime item in accidentTimes.ItemsSource)
                {

                    if (item.Time.Equals(selectedItem.Time))
                    {
                        selectedItem.IsSelected = true;
                        selectedItem.ItemTextColor = Color.White;
                        selectedItem.ItemIcon = "ic_clock_white.png";
                        selectedItem.ItemTopBackgroundColor = Color.FromHex("#1f536c");
                        selectedItem.ItemBottomBackgroundColor = Color.FromHex("#003552");
                        selectedItem.ItemSeparatorColor = Color.FromHex("#1f536c");
                        selectedItem.clickCount = 1;
                        selectedItem.OnPropertyChanged();


                        var deviceWidth = Application.Current.MainPage.Width;
                        var dateElCount = deviceWidth / 180;

                        var hListViewY = accidentTimes.Y;
                        var scrollEnabledCount = dateElCount / 2;

                        Debug.WriteLine($"dateElCount: {dateElCount}, selectedItemIndex: {itemIndex}");

                        if (itemIndex > scrollEnabledCount)
                        {
                            accidentTimes.ScrollToAsync((itemIndex - scrollEnabledCount) * 200, hListViewY, true);
                        }
                    }
                    else
                    {
                        item.IsSelected = false;
                        item.ItemTextColor = Color.Default;
                        item.ItemTopBackgroundColor = Color.White;
                        item.ItemIcon = "ic_clock.png";
                        item.ItemBottomBackgroundColor = Color.White;
                        item.ItemSeparatorColor = Color.Default;
                        item.OnPropertyChanged();
                    }



                    //All unselected items should have a click count of 0
                    if (!item.Equals(selectedItem))
                        item.clickCount = 0;

                    //An item should be clicked more than once to show the date picker popup
                    if (item.clickCount > 1)
                    {
                        accidentTimePicker.Focus();
                        break;
                    }


                }

                selectedTime = selectedDate.Date.Add(accidentTimePicker.Time);

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception on SetSelectedTime:  {ex.Message}, {ex.InnerException}, {ex.StackTrace}");
            }
        }

        private void Handle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(e.NewTextValue))
                    LocationSearchQuery(e.NewTextValue);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private SearchLocation searchLocation;
        private void LocationSearchQuery(string searchname)
        {
            try
            {
                var Url = string.Format(MapBoxService.GeocoderApi + "{0}.js?key=" + MapBoxService.Key, searchname);
                var content = _client.GetStringAsync(Url).Result;

                searchLocation = JsonConvert.DeserializeObject<SearchLocation>(content);

                if (searchLocation.results.Count > 0)
                {
                    places_lv.IsVisible = true;

                    places_lv.ItemsSource = searchLocation.results.Take(8);

                    // wrap the listView content.
                    var i = searchLocation.results.Count;
                    var heightRowsList = 90;
                    i = i * heightRowsList;
                    places_lv.HeightRequest = i;
                }
                else
                {
                    places_lv.IsVisible = false;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        //private async void ViewLarge(object sender, EventArgs args)
        //{
        //    await Navigation.PushAsync(new ImageEnlarge());
        //}

        private void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                item = (Result)((ListView)sender).SelectedItem;
                ((ListView)sender).SelectedItem = null;

                map.Center = new Position { Lat = item.lat, Long = item.lon };

                search_entry.Text = "";

                places_lv.IsVisible = false;


            }
            catch (Exception em)
            {
                Debug.WriteLine(em.Message);
            }
        }

        private void ZoomIn(object sender, EventArgs e)
        {

            _counter += 2;
            map.ZoomLevel = _counter;
            map.Scale = 1;

        }

        private void ZoomOut(object sender, EventArgs e)
        {
            _counter -= 2;
            map.ZoomLevel = _counter;
            map.Scale = 1;
        }

        public bool IsLocationValid()
        {
            DateTime accidentTimeStamp = selectedDate.Date.Add(selectedTime.TimeOfDay);
            var accidentUnixTimestamp = ((DateTimeOffset)accidentTimeStamp).ToUnixTimeMilliseconds();

            CreateReport.report.Data.LND = CreateReport.LND;
            CreateReport.report.Data.DateTime = accidentUnixTimestamp;
            return true;
        }

        private void ShowDatePicker(object sender, EventArgs args)
        {
            accidentDatePicker.Focus();
        }

        private void RefreshDateScroll(object sender, DateChangedEventArgs args)
        {


            Debug.WriteLine($"SelectedDate inside {args.NewDate.Date}");
            scrollableDates.Clear();
            GetDates(args.NewDate);


            accidentDates.Render();
            Task.Delay(1000);
            Debug.WriteLine($"SelectedDate outside {args.NewDate.Date}");
            selectedDate = args.NewDate;
            SetSelectedDate(new ScrollableDate
            {
                Day = selectedDate.Date.DayOfWeek.ToString(),
                Date = selectedDate.Date.Day,
                Month = selectedDate.ToString("MMM"),
            });



            //SetSelectedTime(scrollableTime.Find(x => x.Time ==  selectedTime.ToString("HH:mm tt")));
        }

        private void OnTimePickerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                var timePicker = sender as TimePicker;
                if (e.PropertyName == "Time")
                {
                    //Will only execute on selection from the time picker once the scrollview is already populated
                    if (scrollableTime.Count > 0)
                    {
                        var selectedDateTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day).Add(timePicker.Time);
                        //var selectedDateTime = selectedDate.Date.Add(timePicker.Time); // Convert.ToDateTime(timePicker.Time.ToString());

                        Debug.WriteLine($"selectedDateTime {selectedDateTime}");
                        //check if time is not in the future

                        if (selectedDateTime < DateTime.Now)
                        {
                            scrollableTime.Clear();
                            GetTimeIntervals(selectedDateTime);

                            accidentTimes.Content = null;
                            accidentTimes.Render();
                            accidentTimes.ItemsSource = null;
                            accidentTimes.ItemsSource = scrollableTime;
                            accidentTimePicker.Time = selectedDateTime.TimeOfDay;
                            Task.Delay(1000);
                            SetSelectedTime(scrollableTime.Find(x => x.Time == selectedDateTime.TimeOfDay.ToString()));
                        }
                        else
                        {
                            accidentTimePicker.Time = selectedDate.TimeOfDay;
                        }


                    }
                }



            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception {ex.Message}");
            }

        }

        public void GetDates(DateTime selectedDate)
        {
            selectedDateRange.Clear();
            scrollableDates.Clear();
            for (var date = new DateTime(selectedDate.Year, selectedDate.Month, 1); date.Month == selectedDate.Month; date = date.AddDays(1))
            {
                selectedDateRange.Add(date);

                if (date.Day == selectedDate.Day)
                    scrollableDates.Add(new ScrollableDate
                    {
                        Day = date.DayOfWeek.ToString(),
                        Date = date.Day,
                        Month = selectedDate.ToString("MMM"),
                        IsSelected = true,
                        ItemTextColor = Color.White,
                        ItemTopBackgroundColor = Color.FromHex("#1f536c"),
                        ItemBottomBackgroundColor = Color.FromHex("#003552"),
                        ItemSeparatorColor = Color.FromHex("#1f536c"),
                        clickCount = 1
                    });
                else
                    scrollableDates.Add(new ScrollableDate { Day = date.DayOfWeek.ToString(), Date = date.Day, Month = selectedDate.ToString("MMM") });
            }
        }

        private string splitTime;
        public void GetTimeIntervals(DateTime time)
        {
            var startFrom = time.AddMinutes(-30);
            for (int i = 0; i < 12; i++)
            {
                splitTime = startFrom.AddMinutes(5 * i).ToString("HH:mm tt");
                scrollableTime.Add(new ScrollableTime { Time = splitTime.ToLower() });
                if (time.ToString("HH:mm tt") == splitTime)
                {
                    var scrollableTimeItem = scrollableTime.FirstOrDefault(item => item.Time == splitTime.ToLower());
                    if (scrollableTimeItem != null)
                    {
                        scrollableTimeItem.IsSelected = true;
                        scrollableTimeItem.ItemIcon = "ic_clock_white.png";
                        scrollableTimeItem.ItemTextColor = Color.White;
                        scrollableTimeItem.ItemTopBackgroundColor = Color.FromHex("#1f536c");
                        scrollableTimeItem.ItemBottomBackgroundColor = Color.FromHex("#003552");
                        scrollableTimeItem.ItemSeparatorColor = Color.FromHex("#1f536c");
                        scrollableTimeItem.clickCount = 1;
                    }


                }


            }
        }

        private void AccidentDates_ItemSelected(object sender, ItemTappedEventArgs e)
        {

            var hListView = sender as HorizontalListview;
            var selectedItem = e.Item as ScrollableDate;


            if (selectedDateRange[scrollableDates.IndexOf(selectedItem)] <= DateTime.Now.Date)
            {
                selectedItem.IsSelected = true;
                selectedItem.ItemTextColor = Color.White;
                selectedItem.ItemTopBackgroundColor = Color.FromHex("#1f536c");
                selectedItem.ItemBottomBackgroundColor = Color.FromHex("#003552");
                selectedItem.ItemSeparatorColor = Color.FromHex("#1f536c");
                selectedItem.clickCount += 1;
                selectedItem.OnPropertyChanged();

                var dateIndex = scrollableDates.IndexOf(selectedItem);
                selectedDate = selectedDateRange[dateIndex]; //Set selected date to currently tapped item

                // set the text color of the selected item
                foreach (ScrollableDate item in hListView.ItemsSource)
                {
                    if (item.IsSelected && !item.Equals(selectedItem))
                    {
                        item.IsSelected = false;
                        item.ItemTextColor = Color.Default;
                        item.ItemTopBackgroundColor = Color.White;
                        item.ItemBottomBackgroundColor = Color.White;
                        item.ItemSeparatorColor = Color.Default;
                        item.OnPropertyChanged();

                        var deviceWidth = Application.Current.MainPage.Width;
                        var dateElCount = deviceWidth / 100;
                        var hListViewY = hListView.Y;
                        var scrollEnabledCount = dateElCount / 3;

                        if (dateIndex > scrollEnabledCount)
                        {
                            hListView.ScrollToAsync((dateIndex - scrollEnabledCount) * 100, hListViewY, true);
                        }

                    }



                    //All unselected items should have a click count of 0
                    if (!item.Equals(selectedItem))
                        item.clickCount = 0;

                    //An item should be clicked more than once to show the date picker popup
                    if (item.clickCount > 1)
                    {
                        accidentDatePicker.Focus();
                        break;
                    }
                }
            }
        }



        private void HListView_ItemSelected(object sender, ItemTappedEventArgs e)
        {
            throw new NotImplementedException();
        }

        //DateTime _datetime;
        private void AccidentTime_ItemSelected(object sender, ItemTappedEventArgs e)
        {

            try
            {
                var hListView = sender as HorizontalListview;

                if (hListView.ItemsSource == null)
                    return;


                if (!(e.Item is ScrollableTime selectedItem))
                    return;



                var time = selectedItem.Time.Replace("pm", "").Replace("am", "");

                var selectedDataTime = new DateTime(selectedDate.Year, selectedDate.Month, selectedDate.Day).Add(Convert.ToDateTime(time).TimeOfDay);

                //DateTime _selectedTime = DateTime.ParseExact(time, "HH:mm:ss", CultureInfo.InvariantCulture);

                if (selectedDataTime.Ticks <= DateTime.Now.Ticks)
                {
                    foreach (ScrollableTime item in hListView.ItemsSource)
                    {
                        if (item.Time.Equals(selectedItem.Time))
                        {

                            selectedItem.IsSelected = true;
                            selectedItem.ItemTextColor = Color.White;
                            selectedItem.ItemIcon = "ic_clock_white.png";
                            selectedItem.ItemTopBackgroundColor = Color.FromHex("#1f536c");
                            selectedItem.ItemBottomBackgroundColor = Color.FromHex("#003552");
                            selectedItem.ItemSeparatorColor = Color.FromHex("#1f536c");
                            selectedItem.clickCount += 1;
                            selectedItem.OnPropertyChanged();

                            //var itemIndex = scrollableTime.IndexOf(selectedItem);

                            //var deviceWidth = Application.Current.MainPage.Width;
                            //var dateElCount = deviceWidth / 180;

                            //var hListViewY = hListView.Y;
                            //var scrollEnabledCount = dateElCount / 2;

                            //if (itemIndex > scrollEnabledCount)
                            //{
                            //    hListView.ScrollToAsync((itemIndex - scrollEnabledCount) * 200, hListViewY, true).Wait();
                            //}
                        }
                        else
                        {
                            item.IsSelected = false;
                            item.ItemTextColor = Color.Default;
                            item.ItemTopBackgroundColor = Color.White;
                            item.ItemIcon = "ic_clock.png";
                            item.ItemBottomBackgroundColor = Color.White;
                            item.ItemSeparatorColor = Color.Default;
                            item.OnPropertyChanged();
                        }

                        //All unselected items should have a click count of 0
                        if (!item.Equals(selectedItem))
                            item.clickCount = 0;

                        //An item should be clicked more than once to show the date picker popup
                        if (item.clickCount > 1)
                        {
                            accidentTimePicker.Focus();
                            break;
                        }


                    }

                    Debug.WriteLine($"Time {time}");

                    selectedTime = selectedDate.Date.Add(TimeSpan.Parse(time));

                    Debug.WriteLine($"SelectedTime {selectedTime}");

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception Messages {ex.Message}");
            }


        }

        private SelectUploadOption selectUploadOption;
        private async void SelectUploadOptions(object sender, EventArgs args)
        {
            try
            {
                var hasCameraPermision = await Utils.Utils.CheckPermissions(Permission.Camera);
                var hasPhotosPermission = await Utils.Utils.CheckPermissions(Permission.Photos);
              
                if (hasCameraPermision && hasPhotosPermission)
                {
                    // We have permission, go ahead and use the camera.
                    selectUploadOption = new SelectUploadOption(this);
                    await PopupNavigation.Instance.PushAsync(selectUploadOption);
                }
                else
                {
                    Debug.WriteLine($"NOte Allowed");
                    // Camera permission is not granted. If necessary display rationale & request.
                }

            } catch(Exception ex)
            {
                Debug.WriteLine($"Exec {ex.Message}");
            }
           
            
        }


        private void ShowExtraAccidentPhotoslabel(object sender, EventArgs args)
        {
            try
            {

                Navigation.PushAsync(new ImageEnlarge(4));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception {ex.Message}");
            }

            
        }

        public void AddImageToCollection(MediaFile _accidentPhoto)
        {
            if (accidentImagesContainer.Children.Count > 3)
            {
                extraAccidentPhotosFrame.IsVisible = true;
                extraAccidentPhotosLabel.Text = string.Format("+{0}", (_accidentPhotos.Count - 4));
            }
            else
            {
                AbsoluteLayout _absoluteLayout = new AbsoluteLayout();

                Xamarin.Forms.Image _accidentImage = new Xamarin.Forms.Image
                {
                    Source = ImageSource.FromStream(() =>
                    {
                        var stream = _accidentPhoto.GetStream();
                        return stream;
                    }),
                    HeightRequest = 55,
                    WidthRequest = 55,
                    Aspect = Aspect.Fill
                };
                _accidentImage.GestureRecognizers.Add(
                    new TapGestureRecognizer()
                    {
                        Command = new Command(async () =>
                        {
                            //_absoluteLayout.Children.s

                            await Navigation.PushAsync(new ImageEnlarge(_accidentPhotos.IndexOf(_accidentPhoto)));
                        })
                    });

                StackLayout stackLayout = new StackLayout
                {
                    HeightRequest = 55,
                    WidthRequest = 55
                };

                stackLayout.Children.Add(_accidentImage);

                Frame _frame = new Frame
                {
                    IsClippedToBounds = true,
                    BorderColor = Color.White,
                    CornerRadius = 10,
                    Padding = 0,
                    HasShadow = false,
                    Margin = new Thickness(0, 7, 7, 0),
                    HeightRequest = 55,
                    WidthRequest = 55
                };

                //Add Deletion button
                Xamarin.Forms.Image _deleteImage = new Xamarin.Forms.Image
                {
                    Source = "ic_remove.png",
                    HeightRequest = 20,
                    WidthRequest = 20
                };
                _deleteImage.GestureRecognizers.Add(
                    new TapGestureRecognizer()
                    {
                        Command = new Command(() =>
                        {
                            //Remove image from collection                                                    
                            _accidentPhotos.Remove(_accidentPhoto);

                            //Image deleted from Temp folder
                            if (File.Exists(_accidentPhoto.Path))
                            {

                                Debug.WriteLine($"Image @Path   {_accidentPhoto.Path} deleted");
                                File.Delete(_accidentPhoto.Path);
                            }

                            //image view removed also
                            accidentImagesContainer.Children.Remove(_absoluteLayout);
                            ReloadAttachedAccidentImages(_accidentPhotos);

                        })
                    });

                _frame.Content = stackLayout;
                _absoluteLayout.Children.Add(_frame, new Rectangle(0f, 0f, 1, 1), AbsoluteLayoutFlags.All);
                _absoluteLayout.Children.Add(_deleteImage, new Rectangle(0.95f, 0f, 20, 20), AbsoluteLayoutFlags.PositionProportional);
                accidentImagesContainer.Children.Add(_absoluteLayout);
            }

            //Close Image Uploading Options Window
            CloseSelectUploadOption();
            ScrollToBottom();
        }


        public void ReloadAttachedAccidentImages(List<MediaFile> accidentImages)
        {
            accidentImagesContainer.Children.Clear();
            foreach (var item in accidentImages)
            {
                AddImageToCollection(item);
            }

            if (_accidentPhotos.Count < 5)
            {
                extraAccidentPhotosFrame.IsVisible = false;
            }

            ScrollToBottom();
        }


        public void ScrollToBottom()
        {
            var lastChild = (locationScroll.Content as StackLayout).Children.LastOrDefault();
            locationScroll.ScrollToAsync(lastChild, ScrollToPosition.MakeVisible, true);
        }

        //Close Options for uploading images
        private async void CloseSelectUploadOption()
        {
            await PopupNavigation.Instance.RemovePageAsync(selectUploadOption);
        }

        public void AddImageToCollectionThroughVia(string _accidentPhoto)
        {
            try
            {
                if (accidentImagesContainer.Children.Count > 3)
                {
                    extraAccidentPhotosFrame.IsVisible = true;
                    extraAccidentPhotosLabel.Text = string.Format("+{0}", (_accidentPhotos.Count - 4));
                }
                else
                {
                    AbsoluteLayout _absoluteLayout = new AbsoluteLayout();

                    Xamarin.Forms.Image _accidentImage = new Xamarin.Forms.Image
                    {
                        Source = ImageSource.FromUri(new Uri(_accidentPhoto, UriKind.Absolute)),
                        HeightRequest = 55,
                        WidthRequest = 55,
                        Aspect = Aspect.Fill
                    };

                    Frame _frame = new Frame
                    {
                        CornerRadius = 8,
                        Padding = 0,
                        HasShadow = false,
                        Margin = new Thickness(0, 5, 5, 0)
                    };
                    _frame.Content = _accidentImage;

                    //Add Deletion button
                    ContentView _deleteContentView = new ContentView();

                    Xamarin.Forms.Image _deleteImage = new Xamarin.Forms.Image
                    {
                        Source = "ic_remove.png",
                        HeightRequest = 20,
                        WidthRequest = 20
                    };
                    var tapGestureRecognizer = new TapGestureRecognizer();
                    tapGestureRecognizer.Tapped += (sender, args) =>
                    {
                    // handle the tap
                    accidentImagesContainer.Children.Remove((sender as ContentView).Parent as AbsoluteLayout);
                        var imgPosition = accidentImagesContainer.Children.IndexOf((sender as ContentView).Parent as AbsoluteLayout);
                        _accidentPhotos.RemoveAt(imgPosition);

                    };
                    _deleteImage.GestureRecognizers.Add(tapGestureRecognizer);
                    _deleteContentView.Content = _accidentImage;

                    _absoluteLayout.Children.Add(_frame);
                    _absoluteLayout.Children.Add(_deleteContentView);
                    accidentImagesContainer.Children.Add(_absoluteLayout);
                }
            }
            catch(Exception ex)
            {

            }
        }

        public void OnResumePage(MediaFile _mediaFile)
        {
            AddImageToCollection(_mediaFile);
        }

        public void LoadUploadedAccidentPhotos()
        {
            try
            {
                if (CreateReport.report.Data.Images.Count > 0)
                {
                    int _count = 0;
                    if (accidentImagesContainer.Children.Count > 0)
                        foreach (AbsoluteLayout absoluteLayout in accidentImagesContainer.Children)
                        {
                            var _image = ((absoluteLayout.Children[0] as Frame).Content as StackLayout).Children[0] as Xamarin.Forms.Image;
                            _image.Source = new UriImageSource { Uri = new Uri(CreateReport.report.Data.Images[_count++].Url) };
                        }
                    else
                        foreach (ReportImage image in CreateReport.report.Data.Images)
                            AddImageToCollectionThroughVia(image.Url);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void ViaMapBox_Tapped(object sender, MapTapEventArgs e)
        {

        }



        public void OnSleepPage()
        {

        }

        public void OnStartPage()
        {
            
        }

        private async void UserLocation(object sender, EventArgs args)
        {
            try
            {
                var hasPermission = await Utils.Utils.CheckPermissions(Permission.Location);
                if (!hasPermission)
                    return;

                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;

                var position = await locator.GetPositionAsync(timeout: new TimeSpan(0, 0, 10));

                if (position == null)
                {
                    throw new ArgumentNullException("Gps Error", "Gps Position is null");
                    //return;
                }
                else
                {
                    map.Center = new Position { Lat = position.Latitude, Long = position.Longitude };
                }
            }
            catch (Exception ex)
            {
                Log.Warning("Gps Exception", ex.Message);
            }
        }
    }
}