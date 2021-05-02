using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Via.Helpers;
using Via.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Naxam.Controls.Mapbox.Forms;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Via.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ReportOverview : ContentPage
	{
        private ReportData report;
        private Label label;
        private StackLayout stackLayout;
        private Button button;
        private Frame partyFrame;
        private StackLayout partyStackLayout;
        public PartyPageState partyPageState;
        private double _lat, _long;

        public ReportOverview (ReportData report)
		{
            Title = "Report Overview";

			InitializeComponent ();
            this.report = report;

            _lat = Convert.ToDouble(report.Data.LocationAttributes.Find(x => x.Field == "LAT").Value);
            _long = Convert.ToDouble(report.Data.LocationAttributes.Find(x => x.Field == "LON").Value);

            LocationOverview();
            AccidentOverview();
            PartiesOverview();
        }

        /// <summary>
        /// Overview on Location Details
        /// </summary>
        public void LocationOverview()
        {
            try
            {
                var location = report.Data.LocationAttributes.Find(x => x.Field == "NAME");
                //selectedLocation.Text = (location.Value.Length > 20) ? location.Value.Substring(0, 20) + "..." : location.Value;
                selectedLocation.Text = location.Value;
                map.MapStyle = new MapStyle("mapbox://styles/wamos/ck3idx14f1uie1cmjhqw91613");

                map.HeightRequest = 100.0;
                map.ZoomLevel = 13;
                map.Scale = 1;
                map.Center = new Position { Lat = _lat, Long = _long };

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
            //map.Annotations = annotations;
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
                var control = Report._controls.Libary.Features.Find(x => x.Id == report.Data.Features[feature].Id);
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
                    //FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 0, 0, 1),
                    FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"
                };
                grid.Children.Add(label, 0, feature);

                label = new Label
                {
                    Text = selectedItems,
                    FontSize = 17,
                    FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf",
                    Margin = new Thickness(0, 0, 0, 1)
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
        /// Listing for parties involved in the accident
        /// </summary>
        private void PartiesOverview()
        {
            foreach (var party in report.Data.Parties)
            {
                LoadParty(party);
            }
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
            partyFrame = new Frame
            {
                CornerRadius = 8,
                Margin = new Thickness(0, 0, 0, 10),
                BorderColor = Color.LightGray,
                BackgroundColor = Color.LightGray
            };
            partyFrame.Content = partyStackLayout;
            partiesOverview.Children.Add(partyFrame);

            stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal
            };

            label = new Label
            {
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf",
                Text = Report._controls.Libary.AvailableParties.Items.Find(x => x.ID == party.Id.Substring(0, party.Id.IndexOf("_"))).Title
            };
            stackLayout.Children.Add(label);

            label = new Label
            {
                Text = "Party (" + party.Id + ")",
                FontAttributes = FontAttributes.Italic,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
            };
            stackLayout.Children.Add(label);
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
                var control = Report._controls.Libary.Features.Find(x => x.Id == party.Features[feature].Id);
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
                    FontAttributes = FontAttributes.Bold,
                    Margin = new Thickness(0, 0, 0, 1)
                };
                grid.Children.Add(label, 0, feature);

                label = new Label
                {
                    Text = selectedItems,
                    FontSize = 17,
                    Margin = new Thickness(0, 0, 0, 1)
                };
                grid.Children.Add(label, 1, feature);
            }
            partyStackLayout.Children.Add(grid);

            LoadPartyPersons(party);
        }

        /// <summary>
        /// Load persons attached to party
        /// </summary>
        /// <param name="party"></param>
        private void LoadPartyPersons(Party party)
        {
            
            for (var person = 0; person < party.Persons.Count; person++)
            {
                stackLayout = new StackLayout { Orientation = StackOrientation.Vertical, Margin = new Thickness(0, 0, 0, 10) };
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

                    var control = Report._controls.Libary.Features.Find(x => x.Id == party.Persons[person].Features[feature].Id);
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
                        FontAttributes = FontAttributes.Bold,
                        Margin = new Thickness(0, 0, 0, 1)
                    };
                    personsGrid.Children.Add(label, 0, feature);

                    var selectedValues = "";
                    foreach (var value in party.Persons[person].Features[feature].Values)
                    {
                        selectedValues += value;
                    }

                    label = new Label
                    {
                        Text = selectedValues,
                        FontSize = 17,
                        Margin = new Thickness(0, 0, 0, 1)
                    };
                    personsGrid.Children.Add(label, 1, feature);
                }

                partyStackLayout.Children.Add(personsGrid);
            }
        }

        private async void SubmitReportAsync(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new SaveReport(this.report, ReportStatus.Edit));
        }
    }
}