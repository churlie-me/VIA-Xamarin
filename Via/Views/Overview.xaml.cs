using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using Naxam.Controls.Mapbox.Forms;
using Via.Models;
using Plugin.Media.Abstractions;
using Via.Helpers;
using Xamarin.Forms.Internals;

namespace Via.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Overview : ContentView
    {
        private ReportData report;
        private Label label;
        private StackLayout stackLayout;
        private Button button;
        private Frame partyFrame;
        private StackLayout partyStackLayout;
        public PartyPageState partyPageState;
        private double _lat, _long;

        public Overview()
        {
            InitializeComponent();
            report = CreateReport.report;

            _lat = Convert.ToDouble(report.Data.LocationAttributes.Find(x => x.Field == "LAT").Value);
            _long = Convert.ToDouble(report.Data.LocationAttributes.Find(x => x.Field == "LON").Value);

            try
            {
                LocationOverview();
                AccidentOverview();
                PartiesOverview();
            }
            catch(Exception ex)
            {
                Log.Warning("Overview Error" , ex.Message);
            }
        }

        /// <summary>
        /// Overview on Location Details
        /// </summary>
        public void LocationOverview()
        {
            var location = CreateReport.report.Data.LocationAttributes.Find(x => x.Field == "NAME");
            selectedLocation.Text = (location == null)? "Unknown" : location.Value;

            try
            {
                map.HeightRequest = 100.0;
                map.MapStyle = new MapStyle("mapbox://styles/wamos/ck3idx14f1uie1cmjhqw91613");
                map.ZoomLevel = 13;
                map.Scale = 1;
                map.Center = new Position { Lat = _lat, Long = _long };

                var annotations = new ObservableCollection<Annotation>
                {
                    new PointAnnotation
                    {
                        Title = (location == null)? "Unknown" : location.Value,
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
                var control = CreateReport._controls.Libary.Features.Find(x => x.Id == report.Data.Features[feature].Id);
                var selectedValue = "";
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
                    FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf",
                   // FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 0, 0, 1)
                };
                grid.Children.Add(label, 0, feature);

                label = new Label
                {
                    Text = (selectedItems == CreateReport._controls.Libary.FeatureStateValues
                                                .Find(x => x.FeatureState == FeatureStates.AllowUnknown.ToString()).Value)? "Unknown" :  selectedItems,
                    FontSize = 17,
                    Margin = new Thickness(0, 0, 0, 1),
                    FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
                };
                grid.Children.Add(label, 1, feature);
            }

            accidentOverview.Children.Add(grid);

            //Display photos incase of any were attached
            if (report.Data.Images.Count > 0)
            {
                accidentPhotoStack.IsVisible = true;
                accidentPhotos.IsVisible = true;
                accidentPhotos.ItemsSource = report.Data.Images;
            }
            else
            { 
                accidentPhotos.IsVisible = false;
                accidentPhotos.IsEnabled = false;
                accidentPhotoStack.IsVisible = false;
                accidentPhotoStack.IsEnabled = false;
            }
        }

        /// <summary>
        /// Return to Accident Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnEditAccident(object sender, EventArgs args)
        {
            CreateReport.reportPageState.OnNavigateToPage(ReportStage.Accident);
        }

        /// <summary>
        /// Return to Location Page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnEditLocation(object sender, EventArgs args)
        {
            CreateReport.reportPageState.OnNavigateToPage(ReportStage.Location);
        }

        /// <summary>
        /// Listing for parties involved in the accident
        /// </summary>
        private void PartiesOverview()
        {
            if (report.Data.Parties.Count > 0)
            {
                parties.IsVisible = true;
                foreach (var party in report.Data.Parties)
                {
                    LoadParty(party);
                }
            }
            else
                parties.IsVisible = false;
        }

        /// <summary>
        /// Load parties values
        /// </summary>
        /// <param name="party"></param>
        private void LoadParty(Party party)
        {
            partyStackLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical
            }; 

            stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };

            label = new Label
            {
                Text = Parties.availableParties.Find(x => x.ID == party.Id.Substring(0, party.Id.IndexOf("_"))).Title,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"
            };
            stackLayout.Children.Add(label);

            label = new Label
            {
                Text = "Party (" + party.Id + ")",
                FontAttributes = FontAttributes.Italic,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-NormalBold" : "TitilliumWeb-NormalBold.ttf"
            };
            stackLayout.Children.Add(label);

            button = new Button
            {
                FontSize = 12,
                Text = "Edit",
                Padding = new Thickness(1),
                HeightRequest = 30,
                CornerRadius = 30,
                WidthRequest = 100,
                BackgroundColor = Color.DeepSkyBlue,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                TextColor = Color.White,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-NormalBold" : "TitilliumWeb-NormalBold.ttf"
            };
            button.Clicked += (sender, args) =>
            {
                var partyIndex = partiesOverview.Children.IndexOf(((((sender as Button).Parent as StackLayout).Parent as StackLayout).Parent as Frame));
                partyPageState.OnPartyEdit(partyIndex);
            };

            stackLayout.Children.Add(button);
            partyStackLayout.Children.Add(stackLayout);

            Grid grid = new Grid
            {
                VerticalOptions = LayoutOptions.Center,
                ColumnDefinitions =
                {
                    new ColumnDefinition {Width = new GridLength(.5, GridUnitType.Star)},
                    new ColumnDefinition {Width = new GridLength(.5, GridUnitType.Star)}
                }
            };

            for (var feature = 0; feature < party.Features.Count; feature++)
            {
                var control = CreateReport._controls.Libary.Features.Find(x => x.Id == party.Features[feature].Id);
                var selectedItems = "";
                foreach (var value in party.Features[feature].Values)
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
                    FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf",
                    //FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 0, 0, 1)
                };
                grid.Children.Add(label, 0, feature);

                label = new Label
                {
                    Text = (selectedItems == CreateReport._controls.Libary.FeatureStateValues
                                                .Find(x => x.FeatureState == FeatureStates.AllowUnknown.ToString()).Value) ? "Unknown" : selectedItems,
                    FontSize = 17,
                    Margin = new Thickness(0, 0, 0, 1),
                    FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-NormalBold" : "TitilliumWeb-NormalBold.ttf"
                };
                grid.Children.Add(label, 1, feature);
            }
            partyStackLayout.Children.Add(grid);

            LoadPartyPersons(party);

            partyFrame = new Frame
            {
                CornerRadius = 8,
                Margin = new Thickness(0, 0, 0, 10),
                BorderColor = Color.LightGray,
                HasShadow = (Device.RuntimePlatform == Device.iOS) ? false : true,
                BackgroundColor = Color.LightGray
            };
            partyFrame.Content = partyStackLayout;
            partiesOverview.Children.Add(partyFrame);
        }

        /// <summary>
        /// Load persons attached to party
        /// </summary>
        /// <param name="party"></param>
        private void LoadPartyPersons(Party party)
        {
            for (var person = 0; person < party.Persons.Count; person++)
            {
                stackLayout = new StackLayout { Orientation = StackOrientation.Vertical, Margin = new Thickness(0,0,0,10) };
                label = new Label { Text = string.Format("Person {0}", person + 1), FontAttributes = FontAttributes.Italic | FontAttributes.Bold };
                stackLayout.Children.Add(label);
                partyStackLayout.Children.Add(stackLayout);

                Grid personsGrid = new Grid
                {
                    VerticalOptions = LayoutOptions.Center,
                    ColumnDefinitions =
                {
                    new ColumnDefinition {Width = new GridLength(.5, GridUnitType.Star)},
                    new ColumnDefinition {Width = new GridLength(.5, GridUnitType.Star)}
                }
                };

                for (var feature = 0; feature < party.Persons[person].Features.Count; feature++)
                {
                    var control = CreateReport._controls.Libary.Features.Find(x => x.Id == party.Persons[person].Features[feature].Id);
                    var selectedItems = "";
                    foreach (var value in party.Persons[person].Features[feature].Values)
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
                        FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf",
                       // FontAttributes = FontAttributes.Bold,
                        Margin = new Thickness(0, 0, 0, 1)
                    };
                    personsGrid.Children.Add(label, 0, feature);

                    var selectedValues = "";
                    foreach (var value in party.Persons[person].Features[feature].Values)
                    {
                        var _feature = CreateReport._controls.Libary.Features.Find(x => x.Id == value);

                        if (_feature != null)
                            selectedValues += _feature.Title;
                        else
                            selectedValues += value;
                    }

                    label = new Label
                    {
                        Text = (selectedItems == CreateReport._controls.Libary.FeatureStateValues
                                                .Find(x => x.FeatureState == FeatureStates.AllowUnknown.ToString()).Value) ? "Unknown" : selectedItems,
                        FontSize = 17,
                        Margin = new Thickness(0, 0, 0, 1),
                        FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-NormalBold" : "TitilliumWeb-NormalBold.ttf"
                    };
                    personsGrid.Children.Add(label, 1, feature);
                }

                partyStackLayout.Children.Add(personsGrid);
            }
        }

        private async void SubmitReportAsync(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new SaveReport(report, CreateReport._reportStatus));
        }
    }
}