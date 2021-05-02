using System;
using System.Collections.Generic;
using Plugin.InputKit.Shared.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Via.Models;
using ViaDropdown = Via.Controls.Dropdown;
using ViaDatePicker = Via.Controls.ViaDatePicker;
using System.Linq;
using Xamarin.Forms.Internals;
using Via.Helpers;
using FFImageLoading.Svg.Forms;
using FFImageLoading.Forms;
using FFImageLoading.Transformations;
using Via.Controls;

namespace Via.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Parties : ContentView, PartyPageState
    {
        private Button button;
        private readonly Control controls;
        
        private Feature feature = new Feature();
        private Xamarin.Forms.Image image;
        private List<string> options;

        private int passenger;
        private RelativeLayout relativeLayout;
        private ReportData report;
        private StackLayout driversStackLayout, passengersStackLayout, dropStack;
        private Xamarin.Forms.Image dropImage;
        private ViaDropdown viaDropdown;
        private ViaDatePicker viaDatePicker;
        public static List<PartyItem> availableParties;
        private Party party;
        private Person person;
        private char[] _alphabet = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P' };


        private RadioButton radioButton;
        private RadioButtonGroupView groupRadioButton;
        public static List<Feature> partyFeatures, driverFeatures, passengerFeatures;
        private Frame frame;
        private ViaEntry viaEntry;
        private StackLayout stackLayout, switchStackLayout, partyStack, personsStackLayout, passengersStack;
        private Label label;
        private Grid grid;
        private CheckBox checkBox;
        private DatePicker datePicker;
        private Button buttonOn, buttonOff, buttonDriver, buttonPassenger;
        private PartyView partyView;
        public List<PartyView> partyViews = new List<PartyView>();
        public static int partyViewIndex = 0;
        public PartyPageState partyPageState;
        private SvgCachedImage svgImage;
        public Parties()
        {
            availableParties = null;
            availableParties = new List<PartyItem>();

            partyViewIndex = 0;

            partyPageState = this;
            InitializeComponent();
            controls = CreateReport._controls;
            report = CreateReport.report;

            LoadParties();
        }

        private void LoadParties()
        {
            try
            {
                if (CreateReport.selectedParties.Count > 0)
                {
                    foreach (Item party in CreateReport.selectedParties)
                    {
                        var availableParty = Accident.availableParties.Find(x => x.ID == party.ID);

                        var partyItem = new PartyItem
                        {
                            ID = availableParty.ID,
                            Icon = availableParty.Icon,
                            InvolvedTypes = availableParty.InvolvedTypes,
                            Grid = availableParty.Grid,
                            Tags = availableParty.Tags,
                            ExcludeFeatureIds = availableParty.ExcludeFeatureIds,
                            Title = availableParty.Title,
                            BackgroundColor = (partyViewIndex == 0) ? Color.FromHex("#39b835") : Color.FromHex("#6a6a77")
                        };

                        availableParties.Add(partyItem);
                        LoadParty(party);
                        partyViewIndex++;
                    }

                    //Reset partyViewIndex to 0 so as to start with the first instances 
                    partyViewIndex = 0;

                    //Load Parties list
                    selectedParties.ItemsSource = null;
                    selectedParties.ItemsSource = availableParties;

                    selectedParties.SelectedItem = availableParties[partyViewIndex];

                    PartiesStackControl.Children.Add(partyViews[partyViewIndex].ContentView); //Set default party contentview
                }
            }
            catch(Exception ex)
            {

            }
        }

        Party reportParty; Person reportPerson;
        private void LoadParty(Item party)
        {
            try
            {
                partyView = new PartyView();
                partyView.ContentView = new ContentView();
                partyStack = new StackLayout();
                partyViews.Add(partyView);

                stackLayout = new StackLayout
                {
                    HorizontalOptions = LayoutOptions.Fill,
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(20, 10, 20, 10),
                    BackgroundColor = Color.LightGray
                };

            label = new Label
            {
                Text = party.Title,
                FontSize = 18,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"
            };

                stackLayout.Children.Add(label);

            button = new Button
            {
                BackgroundColor = Color.Red,
                Text = "Remove Party -",
                WidthRequest = 120,
                FontSize = 12,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                TextColor = Color.White,
                HeightRequest = 30,
                CornerRadius = 30,
                Margin = new Thickness(0, 0, 0, 0),
                Padding = 1,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
            };

                button.Clicked += (sender, args) =>
                {
                    RemoveCurrentParty();
                };

                stackLayout.Children.Add(button);
                partyStack.Children.Add(stackLayout);

                //Load initial party controls
                LoadPartyControls(party);

                //Actions to load drivers and passengers
                if (party.InvolvedTypes.Count() > 0)
                {
                    personsStackLayout = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical
                    };

                    stackLayout = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.EndAndExpand
                    };

                buttonDriver = new Button
                {
                    Text = "Add Driver",
                    HeightRequest = 30,
                    Margin = new Thickness(0, 0, 0, 0),
                    WidthRequest = 120,
                    TextColor = Color.White,
                    Padding = 1,
                    BackgroundColor = Color.LightSlateGray,
                    FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
                };

                    buttonDriver.Clicked += (sender, args) =>
                    {
                        (sender as Button).BackgroundColor = Color.LightSlateGray;
                        (sender as Button).TextColor = Color.White;
                        (((sender as Button).Parent as StackLayout).Children[1] as Button).BackgroundColor = Color.LightGray;
                        (((sender as Button).Parent as StackLayout).Children[1] as Button).TextColor = Color.Black;

                        (((sender as Button).Parent as StackLayout).Parent as StackLayout).Children.Add(LoadDriver(CreateReport.selectedParties[partyViewIndex]));
                        (sender as Button).IsEnabled = false;
                    };

                    stackLayout.Children.Add(buttonDriver);

                buttonPassenger = new Button
                {
                    Text = "Add Passenger",
                    HeightRequest = 30,
                    TextColor = Color.Black,
                    Margin = new Thickness(-15, 0, 0, 0),
                    WidthRequest = 120,
                    Padding = 1,
                    BackgroundColor = Color.LightGray,
                    FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
                };

                    buttonPassenger.Clicked += (sender, args) =>
                    {
                        (sender as Button).BackgroundColor = Color.LightSlateGray;
                        (sender as Button).TextColor = Color.White;
                        (((sender as Button).Parent as StackLayout).Children[0] as Button).BackgroundColor = Color.LightGray;
                        (((sender as Button).Parent as StackLayout).Children[0] as Button).TextColor = Color.Black;

                        (((sender as Button).Parent as StackLayout).Parent as StackLayout).Children.Add(LoadPassengerControls(CreateReport.selectedParties[partyViewIndex]));
                    };

                    stackLayout.Children.Add(buttonPassenger);
                    personsStackLayout.Children.Add(stackLayout);

                    //On edit report
                    if (CreateReport._reportStatus == ReportStatus.Edit)
                    {
                        foreach (var _person in reportParty.Persons)
                        {
                            reportPerson = _person;
                            if (party.InvolvedTypes.Contains(InvolvedTypes.Driver.ToString()))
                            {
                                if (reportPerson.Features.Count > 0) //If there any values that were entered for driver
                                {
                                    var driverStack = LoadDriver(party);
                                    personsStackLayout.Children.Add(driverStack);
                                    SetVictimValue(reportPerson, driverStack);
                                }
                                buttonDriver.IsEnabled = false;
                            }
                            else if (party.InvolvedTypes.Contains(InvolvedTypes.Passenger.ToString()) && party.InvolvedTypes.Count == 1)
                            {
                                if (reportPerson.Features.Count > 0) //If there any values that were entered for passenger
                                {
                                    var _passengerStack = LoadPassengerControls(party);
                                    personsStackLayout.Children.Add(_passengerStack);
                                    SetVictimValue(reportPerson, _passengerStack);
                                }
                                buttonDriver.IsEnabled = false;
                            }
                        }
                    }
                    else
                    {
                        //For persons involved
                        if (party.InvolvedTypes.Contains(InvolvedTypes.Driver.ToString()))
                        {
                            personsStackLayout.Children.Add(LoadDriver(party));
                            buttonDriver.IsEnabled = false;
                        }
                        else if (party.InvolvedTypes.Contains(InvolvedTypes.Passenger.ToString()) && party.InvolvedTypes.Count == 1)
                        {
                            //personsStackLayout.Children.Add(LoadPassengerControls()); 
                            buttonDriver.IsEnabled = false;
                        }
                    }

                }

                partyStack.Children.Add(personsStackLayout);
                partyViews[partyViewIndex].ContentView.Content = partyStack;
            }
            catch(Exception ex)
            {

            }
        }

        private void SetVictimValue(Person _person, StackLayout stackLayout)
        {
            try
            {
                RadioButtonGroupView radioGroupView = null;
                if (_person.Type == FeatureType.Passenger.ToString())
                    radioGroupView = ((stackLayout.Children[1] as StackLayout).Children[0] as StackLayout).Children[1] as RadioButtonGroupView;
                else
                    radioGroupView = (stackLayout.Children[1] as StackLayout).Children[1] as RadioButtonGroupView;

                radioGroupView.SelectedItem = _person.VictimType;
            }
            catch(Exception ex)
            {

            }
        }

        private void RemoveCurrentParty()
        {
            try
            {
                partyViews.RemoveAt(partyViewIndex);

                //Remove Item from selectedParties
                var itemRemoved = CreateReport.selectedParties.Find(x => x.ID == availableParties[partyViewIndex].ID);

                //Remove item from selectedParties list
                CreateReport.selectedParties.Remove(itemRemoved);

                //Remove item from Accident modes list
                var itemIndex = Accident.availableParties.IndexOf(itemRemoved);

                Accident.availableParties[itemIndex].IsSelected = false;
                Accident.availableParties[itemIndex].TextColor = Color.Gray;
                Accident.availableParties[itemIndex].BackgroundColor = Color.Transparent;
                Accident.availableParties[itemIndex].OnPropertyChanged();

                //Remove from horizontal listview
                availableParties.RemoveAt(partyViewIndex);

                partyViewIndex = (partyViewIndex == 0 && availableParties.Count > 0) ? 0 : --partyViewIndex;

                if (availableParties.Count > 0)
                {
                    availableParties[partyViewIndex].IsSelected = true;
                    availableParties[partyViewIndex].TextColor = Color.White;
                    availableParties[partyViewIndex].BackgroundColor = Color.FromHex("#39b835");
                    availableParties[partyViewIndex].OnPropertyChanged();
                }

            (selectedParties.Content as StackLayout).Children.Clear();
                selectedParties.Render();

                selectedParties.ItemsSource = null;
                selectedParties.ItemsSource = availableParties;

                PartiesStackControl.Children.Clear(); //Change/Clear Layout

                if (partyViewIndex == -1) ++partyViewIndex;

                if (CreateReport.selectedParties.Count() > 0)
                {
                    selectedParties.SelectedItem = availableParties[partyViewIndex];
                    PartiesStackControl.Children.Add(partyViews[partyViewIndex].ContentView);
                }
                else
                {
                    var nopartiesLabel = new Label
                    {
                        Text = "You have no selected parties",
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        HorizontalOptions = LayoutOptions.CenterAndExpand
                    };

                    PartiesStackControl.Children.Add(nopartiesLabel);
                }
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Loads Controls based on the selection mode
        /// </summary>
        /// <param name="party"></param>
        private void LoadPartyControls(Item party)
        {
            try
            {
                partyFeatures = CreateReport._controls.Libary.Features.Where(x => x.FeatureTypes.FirstOrDefault() == FeatureType.Party.ToString()).ToList();

                if (CreateReport._reportStatus == ReportStatus.Edit)
                    if (CreateReport.report.Data.Parties.Count > 0)
                        reportParty = CreateReport.report.Data.Parties.Find(x => x.Id.Substring(0, x.Id.IndexOf("_")) == party.ID);

                foreach (Feature feature in partyFeatures)
                    if (party.ExcludeFeatureIds.Where(y => y.FeatureId == feature.Id).Count() == 1) //check if feature is excluded from party
                        continue; //Feature is to be excluded from party
                    else
                    {
                        if (Convert.ToInt32(feature.Id) == CreateReport._controls.Libary.ImpactPointId)
                        {
                            var ImpactPoint = CreateReport._controls.Libary.ImpactPoints.Find(x => x.Grid == party.Grid);
                            if (ImpactPoint != null && !string.IsNullOrEmpty(ImpactPoint.Grid))
                                GridControl(feature, partyStack, ImpactPoint.Grid);
                            else
                                LoadControl(feature, partyStack);
                        }
                        else
                        {
                            LoadControl(feature, partyStack);
                        }
                    }
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Loads Driver controls
        /// </summary>
        /// <returns>StackLayout</returns>
        private StackLayout LoadDriver(Item party)
        {
            try
            {
                driversStackLayout = new StackLayout { Orientation = StackOrientation.Vertical, ClassId = InvolvedTypes.Driver.ToString() };
                //Driver Dashboard
                stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Margin = new Thickness(0, 15, 0, 8)
                };

            label = new Label
            {
                Text = "Driver ",
                FontSize = 18,
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"
            };
            stackLayout.Children.Add(label);

            button = new Button
            {
                Text = "Remove -",
                BackgroundColor = Color.Red,
                TextColor = Color.White,
                FontSize = 12,
                WidthRequest = 120,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                HeightRequest = 30,
                CornerRadius = 30,
                Margin = new Thickness(0, 0, 0, 0),
                Padding = 1,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
            };

                button.Clicked += (sender, e) =>
                {
                    var lastStackIndex = (partyViews[partyViewIndex].ContentView.Content as StackLayout).Children.Count - 1;
                    var personsStackLayout = ((partyViews[partyViewIndex].ContentView.Content as StackLayout).Children[lastStackIndex] as StackLayout);

                    personsStackLayout.Children.Remove((((sender as Button).Parent as StackLayout).Parent as StackLayout));

                //Activate Add Driver button
                ((personsStackLayout.Children[0] as StackLayout).Children[0] as Button).IsEnabled = true;
                };
                stackLayout.Children.Add(button);
                driversStackLayout.Children.Add(stackLayout);
                LoadVictimType(driversStackLayout);

                driverFeatures = CreateReport._controls.Libary.Features.Where(x => x.FeatureTypes.Contains(FeatureType.Driver.ToString())).ToList();

                foreach (Feature feature in driverFeatures)
                    if (party.ExcludeFeatureIds.Where(y => y.FeatureId == feature.Id).Count() == 1) //check if feature is excluded from party
                        continue; //Feature is to be excluded from party
                    else
                        LoadControl(feature, driversStackLayout);

                return driversStackLayout;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Victim Type is hard coded, not part of the library
        /// </summary>
        string[] victimType = { "NotWounded", "Wounded", "FirstAid", "Hospitalized", "Killed" };
        private void LoadVictimType(StackLayout personsStackLayout)
        {
            try
            {
                stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    Margin = new Thickness(0, 0, 0, 10)
                };

            //Control Lable/Title
            label = new Label
            {
                Text = "Type of severity",
                TextColor = Color.FromHex("#00314b"),
                FontSize = 17,
                Margin = new Thickness(0, 0, 0, 3),
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"
                // FontAttributes = FontAttributes.Bold
            };

                int i = 1, gridRow = 0;
                var grid = new Grid
                {
                    VerticalOptions = LayoutOptions.Center,
                    ColumnDefinitions =
                {
                    new ColumnDefinition {Width = new GridLength(.5, GridUnitType.Star)},
                    new ColumnDefinition {Width = new GridLength(.5, GridUnitType.Star)}
                }
            };
            string[] victimType = { "NotWounded", "Wounded", "FirstAid", "Hospitalized", "Killed"};
            for (var _type = 0; _type < victimType.Length; _type ++)
            {
                radioButton = new RadioButton();
                radioButton.HorizontalOptions = LayoutOptions.Start;
                radioButton.VerticalOptions = LayoutOptions.Center;
                radioButton.TextFontSize = 15;
                radioButton.Text = victimType[_type];
                radioButton.Value = victimType[_type];
                radioButton.FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf";

                    if ((i % 2).Equals(1))
                    {
                        grid.Children.Add(radioButton, 0, gridRow);
                    }
                    else
                    {
                        grid.Children.Add(radioButton, 1, gridRow);
                        gridRow++;
                    }
                    i++;
                }

                groupRadioButton = new RadioButtonGroupView();

                groupRadioButton.SelectedItemChanged += (sender, args) =>
                {
                    (((sender as RadioButtonGroupView).Parent as StackLayout).Children[0] as Label).TextColor = Color.FromHex("#00314b");
                    if ((sender as RadioButtonGroupView).SelectedItem.ToString() == "Killed")
                        DateOfDeath(((sender as RadioButtonGroupView).Parent as StackLayout).Parent as StackLayout); //Adds Dead of Dead to Person's StackLayout
                else
                        DateOfDeath(((sender as RadioButtonGroupView).Parent as StackLayout).Parent as StackLayout, true); //Removes Dead of Dead to Person's StackLayout
            };

                groupRadioButton.Children.Add(grid);

                stackLayout.Children.Add(label);
                stackLayout.Children.Add(groupRadioButton);

                personsStackLayout.Children.Add(stackLayout);
            }
            catch(Exception ex)
            {

            }
        }

        public void DateOfDeath(StackLayout _stack, bool isRemoved = false)
        {
            try
            {
                if (isRemoved)
                {
                    if (((_stack.Children[_stack.Children.Count - 1] as StackLayout).ClassId == "DOD"))
                        _stack.Children.RemoveAt(_stack.Children.Count - 1);
                    return;
                }

                stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    Margin = new Thickness(0, 0, 0, 10),
                    ClassId = "DOD"
                };

            //Control Lable/Title
            label = new Label
            {
                Text = "Dead Of Death",
                TextColor = Color.FromHex("#00314b"),
                FontSize = 17,
                Margin = new Thickness(0, 0, 0, 3),
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"
                //FontAttributes = FontAttributes.Bold
            };

            //Control Entry
            viaDatePicker = new ViaDatePicker
            {
                Date = DateTime.Now,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.White,
                Margin = new Thickness(2.5, 0, 2.5, 0),
                FontSize = 17,
                HeightRequest = 38,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
            };

                //Frame to give the Entry control shape and border color
                frame = new Frame
                {
                    CornerRadius = 8,
                    HasShadow = false,
                    BorderColor = Color.LightSlateGray,
                    Margin = new Thickness(0, 0, 0, 10),
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    Padding = new Thickness(2, 2, 2, 2)
                };
                frame.Content = viaDatePicker; //Enclose the entry control within the frame

                stackLayout.Children.Add(label);
                stackLayout.Children.Add(frame);

                //Add stacklayout to the page
                _stack.Children.Add(stackLayout);
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Loads Passenger Head Layout(stacklayout) and Main Passenger Stacklayout
        /// </summary>
        /// <returns>StackLayout</returns>
        private StackLayout LoadPassengerControls(Item party)
        {
            try
            {
                passengersStackLayout = new StackLayout { Orientation = StackOrientation.Vertical, ClassId = InvolvedTypes.Passenger.ToString() };
                stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Margin = new Thickness(0, 15, 0, 8)
                };
                label = new Label
                {
                    Text = "Passenger " + ++partyViews[partyViewIndex].passengers,
                    FontSize = 18,

                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"
            };
            stackLayout.Children.Add(label);

            //Button to remove passenger at this position
            button = new Button
            {
                Text = "Remove -",
                WidthRequest = 120,
                FontSize = 12,
                BackgroundColor = Color.Red,
                TextColor = Color.White,
                HeightRequest = 30,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                CornerRadius = 30,
                Margin = new Thickness(0, 0, 0, 0),
                Padding = 1,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
            };

                button.Clicked += (sender, e) =>
                {
                    var lastStackIndex = (partyViews[partyViewIndex].ContentView.Content as StackLayout).Children.Count - 1;
                    var personsStackLayout = ((partyViews[partyViewIndex].ContentView.Content as StackLayout).Children[lastStackIndex] as StackLayout);

                    personsStackLayout.Children.Remove((((sender as Button).Parent as StackLayout).Parent as StackLayout));
                    partyViews[partyViewIndex].passengers = (partyViews[partyViewIndex].passengers == 0) ? 0 : partyViews[partyViewIndex].passengers - 1; //Reduce the number of exisiting passengers
            };
                stackLayout.Children.Add(button);
                passengersStackLayout.Children.Add(stackLayout);

                passengersStackLayout.Children.Add(LoadPassenger(party));

                return passengersStackLayout;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Loads passenger controls
        /// </summary>
        /// <returns>Stacklayout</returns>
        private StackLayout LoadPassenger(Item party)
        {
            try
            {
                passengersStack = new StackLayout { Orientation = StackOrientation.Vertical };

                //Add Victim type options first
                LoadVictimType(passengersStack);

                passengerFeatures = CreateReport._controls.Libary.Features.Where(x => x.FeatureTypes.Contains(FeatureType.Passenger.ToString())).ToList();

                foreach (Feature feature in passengerFeatures)
                    if (party.ExcludeFeatureIds.Where(y => y.FeatureId == feature.Id).Count() == 1) //check if feature is excluded from party
                        continue; //Feature is to be excluded from party
                    else
                        LoadControl(feature, passengersStack);

                return passengersStack;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Loads Controls by feature input type
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="posessionStackLayout"></param>
        private void LoadControl(Feature feature, StackLayout posessionStackLayout)
        {
            try
            {
                switch (feature.FeatureInputType)
                {
                    case "Text":
                        EntryControl(feature, posessionStackLayout);
                        break;
                    case "Number":
                        NumericControl(feature, posessionStackLayout);
                        break;
                    case "SingleChoice":
                        if (feature.Items.Count == 2)
                            SwitchControl(feature, posessionStackLayout);
                        else if (feature.Items.Count > 15)
                            DropDownControl(feature, posessionStackLayout);
                        else
                            SingleSelectControl(feature, posessionStackLayout);
                        break;
                    case "MultiChoice":
                        MultiSelectControl(feature, posessionStackLayout);
                        break;
                    case "Date":
                        DateTimeControl(feature, posessionStackLayout);
                        break;
                }
            }
            catch(Exception ex)
            {

            }
        }

        /*
         * Beginning of dynamic controls
         */

        //Single Select Control
        private void SingleSelectControl(Feature feature, StackLayout posessionStackLayout)
        {
            try
            {
                var _value = "";
                if (feature.FeatureTypes.Contains(InvolvedTypes.Driver.ToString()) || feature.FeatureTypes.Contains(InvolvedTypes.Passenger.ToString()))
                {
                    if (reportPerson != null)
                        _value = reportPerson.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                }
                else
                {
                    if (reportParty != null)
                        _value = reportParty.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                }

                stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    Margin = new Thickness(0, 0, 0, 10)
                };

            //Control Lable/Title
            label = new Label
            {
                Text = feature.Title,
                TextColor = Color.FromHex("#00314b"),
                FontSize = 17,
                Margin = new Thickness(0, 0, 0, 3),
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"
                // FontAttributes = FontAttributes.Bold
            };

                //Create a stacklayout to hold control label and feature state button if any
                var headerStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill };
                headerStack.Children.Add(label);

                //Load Feature States if any
                var _frame = AddFeatureStateOption(feature);
                if (_frame != null)
                    headerStack.Children.Add(_frame);

                int i = 1, gridRow = 0;
                var grid = new Grid
                {
                    VerticalOptions = LayoutOptions.Center,
                    ColumnDefinitions =
                {
                    new ColumnDefinition {Width = new GridLength(.5, GridUnitType.Star)},
                    new ColumnDefinition {Width = new GridLength(.5, GridUnitType.Star)}
                }
                };

            foreach (var classification in feature.Items)
            {
                radioButton = new RadioButton();
                radioButton.HorizontalOptions = LayoutOptions.Start;
                radioButton.VerticalOptions = LayoutOptions.Center;
                radioButton.TextFontSize = 15;
                radioButton.Text = classification.Title;
                radioButton.Value = classification.Id;
                radioButton.IsChecked = (classification.Title == _value);
                radioButton.FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf";

                    if (CreateReport._reportStatus == ReportStatus.Edit)
                        if (!string.IsNullOrEmpty(_value))
                            if (_value == classification.Id && !_value.Contains("-"))
                                radioButton.IsChecked = true;

                    if ((i % 2).Equals(1))
                    {
                        grid.Children.Add(radioButton, 0, gridRow);
                    }
                    else
                    {
                        grid.Children.Add(radioButton, 1, gridRow);
                        gridRow++;
                    }
                    i++;
                }

                if (CreateReport._reportStatus == ReportStatus.Edit)
                    if (!string.IsNullOrEmpty(_value))
                        if (_value.Contains("-"))
                            SetFeatureStateOnEdit(_frame, _value);

                groupRadioButton = new RadioButtonGroupView();
                groupRadioButton.ClassId = feature.Id;

                groupRadioButton.SelectedItemChanged += (sender, args) =>
                {
                    ((((sender as RadioButtonGroupView).Parent as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor = Color.FromHex("#00314b");

                    var _feature = CreateReport._controls.Libary.Features.Find(x => x.Id == (sender as RadioButtonGroupView).ClassId);

                //check if feature has feature states
                if (_feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || _feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                    {
                    //lets find any clicked button (to be identified by background color)
                    var _featureStateStack = ((((sender as RadioButtonGroupView).Parent as StackLayout).Children[0] as StackLayout).Children[1] as Frame).Content as StackLayout;
                        var _stateBtns = _featureStateStack.Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736"));

                        if (_stateBtns.Count() > 0)
                            ToggleExtraOptions(_stateBtns.FirstOrDefault(), false);
                    }
                };

                groupRadioButton.Children.Add(grid);


                stackLayout.Children.Add(headerStack);
                stackLayout.Children.Add(groupRadioButton);

                //Add stacklayout to the page
                posessionStackLayout.Children.Add(stackLayout);
            }
            catch(Exception ex)
            {

            }
        }

        void ToggleExtraOptions(object sender, bool isSeleted)
        {
            try
            {
                if (isSeleted)
                {
                    (((((sender as Button).Parent as StackLayout).Parent as Frame).Parent as StackLayout).Children[0] as Label).TextColor = Color.FromHex("#00314b");
                    (sender as Button).BackgroundColor = Color.FromHex("#eea736");
                    (sender as Button).TextColor = Color.White;

                    if (((sender as Button).Parent as StackLayout).Children.Count > 1)
                    {
                        var secBtn = (Button)((sender as Button).Parent as StackLayout).Children.Where(x => x != (sender as Button)).FirstOrDefault();
                        secBtn.BackgroundColor = Color.White;
                        secBtn.TextColor = Color.Default;
                    }
                }
                else
                {
                    (sender as Button).BackgroundColor = Color.White;
                    (sender as Button).TextColor = Color.Default;
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void SetFeatureStateOnEdit(Frame _frame, string featureStateValue)
        {
            try
            {
                foreach (var child in (_frame.Content as StackLayout).Children)
                {
                    if (child.ClassId == featureStateValue)
                    {
                        (child as Button).BackgroundColor = Color.FromHex("#eea736");
                        (child as Button).TextColor = Color.White;
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        //Multi Select Control
        private void MultiSelectControl(Feature feature, StackLayout posessionStackLayout)
        {
            try
            {
                List<string> values = new List<string>();
                if (feature.FeatureTypes.Contains(InvolvedTypes.Driver.ToString()) || feature.FeatureTypes.Contains(InvolvedTypes.Passenger.ToString()))
                {
                    if (reportPerson != null)
                        values = reportPerson.Features.Find(x => x.Id == feature.Id).Values;
                }
                else
                {
                    if (reportParty != null)
                        values = reportParty.Features.Find(x => x.Id == feature.Id).Values;
                }

                stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    Margin = new Thickness(0, 0, 0, 10)
                };

            //Control label
            label = new Label
            {
                Text = feature.Title,
                TextColor = Color.FromHex("#00314b"),
                FontSize = 17,
                Margin = new Thickness(0, 0, 0, 3),
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"
                //FontAttributes = FontAttributes.Bold
            };

                //Create a stacklayout to hold control label and feature state button if any
                var headerStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill };
                headerStack.Children.Add(label);

                //Load Feature States if any
                var _frame = AddFeatureStateOption(feature);
                if (_frame != null)
                    headerStack.Children.Add(_frame);

                int i = 1, gridRow = 0;
                grid = new Grid
                {
                    ClassId = feature.Id,
                    VerticalOptions = LayoutOptions.Center,
                    ColumnDefinitions =
                {
                    new ColumnDefinition {Width = new GridLength(.5, GridUnitType.Star)},
                    new ColumnDefinition {Width = new GridLength(.5, GridUnitType.Star)}
                }
                };

            foreach (var option in feature.Items)
            {
                checkBox = new CheckBox
                {
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    TextFontSize = 15,
                    FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf",
                    Text = option.Title,
                    ClassId = option.Id
                };

                    //Add values if an existing report is being edited
                    if (CreateReport._reportStatus == ReportStatus.Edit)
                        if (values.Count > 0)
                        {
                            string _value = values.Find(x => x == option.Id);
                            if (!string.IsNullOrEmpty(_value) && !_value.Contains("-"))
                                checkBox.IsChecked = true;
                        }


                    string _ID = feature.Id;
                    checkBox.CheckChanged += (sender, args) =>
                    {
                    //Changing the Permitted Category Label
                    if ((((((sender as CheckBox).Parent as Grid).Parent as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor.Equals(Color.Red))
                            (((((sender as CheckBox).Parent as Grid).Parent as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor = Color.Default;

                        var _feature = CreateReport._controls.Libary.Features.Find(x => x.Id == ((sender as CheckBox).Parent as Grid).ClassId);
                    //check if feature has feature states
                    if (_feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || _feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                        {
                        //lets find any clicked button
                        var _featureStateStack = (((((sender as CheckBox).Parent as Grid).Parent as StackLayout)
                                                     .Children[0] as StackLayout).Children[1] as Frame).Content as StackLayout;
                            var _stateBtns = _featureStateStack.Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736"));

                            if (_stateBtns.Count() > 0)
                                ToggleExtraOptions(_stateBtns.FirstOrDefault(), false);
                        }
                    };

                    if ((i % 2).Equals(1))
                    {
                        grid.Children.Add(checkBox, 0, gridRow);
                    }
                    else
                    {
                        grid.Children.Add(checkBox, 1, gridRow);
                        gridRow++;
                    }

                    i++;
                }

                if (CreateReport._reportStatus == ReportStatus.Edit)
                    //Feature state value selected
                    if (values.Count == 1)
                        if (values.FirstOrDefault().Contains("-"))
                            SetFeatureStateOnEdit(_frame, values.FirstOrDefault());

                stackLayout.Children.Add(headerStack);
                stackLayout.Children.Add(grid);

                //Add stacklayout to accident page
                posessionStackLayout.Children.Add(stackLayout);
            }
            catch(Exception ex)
            {

            }
        }

        //Text Control
        public void EntryControl(Feature feature, StackLayout posessionStackLayout)
        {
            try
            {
                var _value = "";
                if (CreateReport._reportStatus == ReportStatus.Edit)
                {
                    if (feature.FeatureTypes.Contains(InvolvedTypes.Driver.ToString()) || feature.FeatureTypes.Contains(InvolvedTypes.Passenger.ToString()))
                    {
                        if (reportPerson != null)
                            _value = reportPerson.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                    }
                    else
                    {
                        if (reportParty != null)
                            _value = reportParty.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                    }
                }

                stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    Margin = new Thickness(0, 0, 0, 10)
                };

            //Control Lable/Title
            label = new Label
            {
                Text = feature.Title,
                TextColor = Color.FromHex("#00314b"),
                FontSize = 17,
                Margin = new Thickness(0, 0, 0, 3),
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"
                //FontAttributes = FontAttributes.Bold
            };

                //Create a stacklayout to hold control label and feature state button if any
                var headerStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill };
                headerStack.Children.Add(label);

                //Load Feature States if any
                var _frame = AddFeatureStateOption(feature);
                if (_frame != null)
                    headerStack.Children.Add(_frame);

            //Control Entry
            viaEntry = new ViaEntry
            {
                ClassId = feature.Id,
                BorderColor = Color.White,
                BackgroundColor = Color.White,
                Keyboard = Keyboard.Text,
                FontSize = 17,
                Margin = new Thickness(2.5, 0, 2.5, 0),
                HeightRequest = 38,
                IsCurvedCornersEnabled = true,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
            };

                if (CreateReport._reportStatus == ReportStatus.Edit)
                {
                    if (!string.IsNullOrEmpty(_value))
                    {
                        if (_value.Contains("-"))
                            SetFeatureStateOnEdit(_frame, _value);
                        else
                            viaEntry.Text = _value;
                    }
                }

                viaEntry.TextChanged += viaEntry_TextChanged;
                //Frame to give the Entry control shape and border color
                frame = new Frame
                {
                    CornerRadius = 8,
                    HasShadow = false,
                    BorderColor = Color.LightSlateGray,
                    Margin = new Thickness(0, 0, 0, 10),
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    Padding = new Thickness(2, 2, 2, 2)
                };
                frame.Content = viaEntry; //Enclose the entry control within the frame

                stackLayout.Children.Add(headerStack);
                stackLayout.Children.Add(frame);

                //Add stacklayout to the page
                posessionStackLayout.Children.Add(stackLayout);
            }
            catch(Exception ex)
            {

            }
        }

        //Text Changed Event for Entry Controls
        private void viaEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (((sender as ViaEntry).Parent as Frame).BorderColor.Equals(Color.Red))
                    ((sender as ViaEntry).Parent as Frame).BorderColor = Color.LightSlateGray;

                var _feature = CreateReport._controls.Libary.Features.Find(x => x.Id == (sender as ViaEntry).ClassId);

                if (_feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || _feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                {
                    //lets find any clicked button (to be identified by its background color)
                    var _featureStateStack = (((((sender as ViaEntry).Parent as Frame).Parent as StackLayout)
                                            .Children[0] as StackLayout).Children[1] as Frame).Content as StackLayout;

                    var _stateBtns = _featureStateStack.Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736"));

                    if (_stateBtns.Count() > 0)
                        ToggleExtraOptions(_stateBtns.FirstOrDefault(), false);
                }
            }
            catch(Exception ex)
            {

            }
        }

        //Feature State Control
        private Frame AddFeatureStateOption(Feature feature)
        {
            try
            {
                var featureStates = feature.FeatureStates.Where(x => x != FeatureStates.AllowOther.ToString() && x != FeatureStates.Required.ToString()).ToList<string>();

                //return null if there are no feature state values left
                if (featureStates.Count() == 0)
                    return null;

            var _featureStack = new StackLayout { Orientation = StackOrientation.Horizontal };
            foreach (var featureState in featureStates)
            {
                //Add a button to as feature state option
                Button featureStateBtn = new Button
                {
                    Text = featureState,
                    HeightRequest = 30,
                    CornerRadius = 30,
                    FontSize = 13,
                    TextColor = Color.Default,
                    //Get feature state value from loaded object _controls
                    ClassId = CreateReport._controls.Libary.FeatureStateValues.Find(x => x.FeatureState == featureState).Value,
                    WidthRequest = 120,
                    Padding = 1,
                    BackgroundColor = Color.White,
                    FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
                };

                    //Remove values selected or entered from the corresponding control
                    featureStateBtn.Clicked += (sender, args) =>
                    {
                        if ((sender as Button).BackgroundColor == Color.FromHex("#eea736"))
                            ToggleExtraOptions(sender, false);
                        else
                            ToggleExtraOptions(sender, true);
                    };
                    _featureStack.Children.Add(featureStateBtn);
                }

                var featureStateFrame = new Frame
                {
                    BackgroundColor = Color.FromHex("#eea736"),
                    Padding = 2,
                    HasShadow = false,
                    CornerRadius = 30,
                    HeightRequest = 32,
                    IsClippedToBounds = false,
                    HorizontalOptions = LayoutOptions.EndAndExpand
                };

                featureStateFrame.Content = _featureStack;
                return featureStateFrame;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        //Numeric Control
        public void NumericControl(Feature feature, StackLayout posessionStackLayout)
        {
            try
            {
                var _value = "";
                if (CreateReport._reportStatus == ReportStatus.Edit)
                {
                    if (feature.FeatureTypes.Contains(InvolvedTypes.Driver.ToString()) || feature.FeatureTypes.Contains(InvolvedTypes.Passenger.ToString()))
                    {
                        if (reportPerson != null)
                            _value = reportPerson.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                    }
                    else
                    {
                        if (reportParty != null)
                            _value = reportParty.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                    }
                }

                stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    Margin = new Thickness(0, 0, 0, 10)
                };

            //Control Lable/Title
            label = new Label
            {
                Text = feature.Title,
                TextColor = Color.FromHex("#00314b"),
                FontSize = 17,
                Margin = new Thickness(0, 0, 0, 3),
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"

                //FontAttributes = FontAttributes.Bold
            };

                //Create a stacklayout to hold control label and feature state button if any
                var headerStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill };
                headerStack.Children.Add(label);

                //Load Feature States if any
                var _frame = AddFeatureStateOption(feature);
                if (_frame != null)
                    headerStack.Children.Add(_frame);

            //Control Entry
            viaEntry = new ViaEntry
            {
                ClassId = feature.Id,
                BorderColor = Color.White,
                BackgroundColor = Color.White,
                Keyboard = Keyboard.Numeric,
                FontSize = 17,
                Margin = new Thickness(2.5, 0, 2.5, 0),
                HeightRequest = 38,
                IsCurvedCornersEnabled = true,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
            };

                if (CreateReport._reportStatus == ReportStatus.Edit)
                {
                    if (!string.IsNullOrEmpty(_value))
                    {
                        if (_value.Contains("-"))
                            SetFeatureStateOnEdit(_frame, _value);
                        else
                            viaEntry.Text = _value;
                    }
                }

                viaEntry.TextChanged += viaEntry_TextChanged;

                //Frame to give the Entry control shape and border color
                frame = new Frame
                {
                    CornerRadius = 8,
                    HasShadow = false,
                    BorderColor = Color.LightSlateGray,
                    Margin = new Thickness(0, 0, 0, 10),
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    Padding = new Thickness(2, 2, 2, 2)
                };
                frame.Content = viaEntry; //Enclose the entry control within the frame

                stackLayout.Children.Add(headerStack);
                stackLayout.Children.Add(frame);

                //Add stacklayout to the page
                posessionStackLayout.Children.Add(stackLayout);
            }
            catch(Exception ex)
            {

            }
        }

        //Switch Control
        private void SwitchControl(Feature feature, StackLayout posessionStackLayout)
        {
            try
            {
                stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    Margin = new Thickness(0, 0, 0, 10)
                };

            //Control Lable/Title
            label = new Label
            {
                Text = feature.Title,
                TextColor = Color.FromHex("#00314b"),
                FontSize = 17,
                Margin = new Thickness(0, 0, 0, 3),
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"
                //FontAttributes = FontAttributes.Bold
            };

                //Create a stacklayout to hold control label and feature state button if any
                var headerStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill };
                headerStack.Children.Add(label);

                //Load Feature States if any
                var _frame = AddFeatureStateOption(feature);
                if (_frame != null)
                    headerStack.Children.Add(_frame);

                switchStackLayout = new StackLayout
                {
                    ClassId = feature.Id,
                    Orientation = StackOrientation.Horizontal
                };

            buttonOn = new Button
            {
                Text = feature.Items[0].Title,
                HeightRequest = 30,
                TextColor = Color.Default,
                CornerRadius = 30,
                Margin = new Thickness(0, 0, 0, 0),
                WidthRequest = 120,
                Padding = 1,
                ClassId = feature.Items[0].Id,
                BackgroundColor = Color.White,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
            };

                buttonOn.Clicked += (sender, args) =>
                {
                    (sender as Button).BackgroundColor = Color.White;
                    (sender as Button).TextColor = Color.Default;
                    (((sender as Button).Parent as StackLayout).Children[1] as Button).BackgroundColor = Color.FromHex("#39b835");
                    (((sender as Button).Parent as StackLayout).Children[1] as Button).TextColor = Color.White;

                //Change text color incase it is red
                ((((((sender as Button).Parent as StackLayout).Parent as Frame).Parent as StackLayout)
                                                 .Children[0] as StackLayout).Children[0] as Label).TextColor = Color.FromHex("#00314b");

                    var _feature = CreateReport._controls.Libary.Features.Find(x => x.Id == ((sender as Button).Parent as StackLayout).ClassId);

                    if (_feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || _feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                    {
                    //lets find any clicked button (to be identified by its background color)
                    var _featureStateStack = ((((((sender as Button).Parent as StackLayout).Parent as Frame).Parent as StackLayout)
                                                 .Children[0] as StackLayout).Children[1] as Frame).Content as StackLayout;

                        var _stateBtns = _featureStateStack.Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736"));

                        if (_stateBtns.Count() > 0)
                            ToggleExtraOptions(_stateBtns.FirstOrDefault(), false);
                    }
                };

                switchStackLayout.Children.Add(buttonOn);

            buttonOff = new Button
            {
                Text = feature.Items[1].Title,
                HeightRequest = 30,
                CornerRadius = 30,
                FontSize = 13,
                TextColor = Color.Default,
                Margin = new Thickness(-5, 0, 0, 0),
                WidthRequest = 120,
                Padding = 1,
                BackgroundColor = Color.White,
                ClassId = feature.Items[1].Id,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
            };
            buttonOff.Clicked += (sender, args) =>
            {
                (sender as Button).BackgroundColor = Color.White;
                (sender as Button).TextColor = Color.Default;
                (((sender as Button).Parent as StackLayout).Children[0] as Button).BackgroundColor = Color.FromHex("#39b835");
                (((sender as Button).Parent as StackLayout).Children[0] as Button).TextColor = Color.White;

                //Change text color incase it is red
                ((((((sender as Button).Parent as StackLayout).Parent as Frame).Parent as StackLayout)
                                                 .Children[0] as StackLayout).Children[0] as Label).TextColor = Color.FromHex("#00314b");

                    var _feature = CreateReport._controls.Libary.Features.Find(x => x.Id == ((sender as Button).Parent as StackLayout).ClassId);

                    if (_feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || _feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                    {
                    //lets find any clicked button (to be identified by its background color)
                    var _featureStateStack = ((((((sender as Button).Parent as StackLayout).Parent as Frame).Parent as StackLayout)
                                                 .Children[0] as StackLayout).Children[1] as Frame).Content as StackLayout;

                        var _stateBtns = _featureStateStack.Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736"));

                        if (_stateBtns.Count() > 0)
                            ToggleExtraOptions(_stateBtns.FirstOrDefault(), false);
                    }
                };

                //Incase of report edit
                var _value = "";
                if (CreateReport._reportStatus == ReportStatus.Edit)
                {
                    if (feature.FeatureTypes.Contains(InvolvedTypes.Driver.ToString()) || feature.FeatureTypes.Contains(InvolvedTypes.Passenger.ToString()))
                    {
                        if (reportPerson != null)
                            _value = reportPerson.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                    }
                    else
                    {
                        if (reportParty != null)
                            _value = reportParty.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                    }

                    if (_value.Contains("-"))
                        SetFeatureStateOnEdit(_frame, _value);
                    else
                    {
                        if (feature.Items[1].Id == _value)
                        {
                            buttonOff.BackgroundColor = Color.FromHex("#39b835");
                            buttonOff.TextColor = Color.White;
                        }
                        else
                        {
                            buttonOn.BackgroundColor = Color.FromHex("#39b835");
                            buttonOn.TextColor = Color.White;
                        }
                    }
                }

                switchStackLayout.Children.Add(buttonOff);

                var switchFrame = new Frame { BackgroundColor = Color.FromHex("#39b835"), Padding = 2, HasShadow = false, CornerRadius = 30, IsClippedToBounds = true, HorizontalOptions = LayoutOptions.Start };
                switchFrame.Content = switchStackLayout;

                stackLayout.Children.Add(headerStack);
                stackLayout.Children.Add(switchFrame);

                //Add stacklayout to the page
                posessionStackLayout.Children.Add(stackLayout);
            }
            catch(Exception ex)
            {

            }
        }

        //DateTime Control
        public void DateTimeControl(Feature feature, StackLayout posessionStackLayout)
        {
            try
            {
                var _value = "";
                if (CreateReport._reportStatus == ReportStatus.Edit)
                {
                    if (feature.FeatureTypes.Contains(InvolvedTypes.Driver.ToString()) || feature.FeatureTypes.Contains(InvolvedTypes.Passenger.ToString()))
                    {
                        if (reportPerson != null)
                            _value = reportPerson.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                    }
                    else
                    {
                        if (reportParty != null)
                            _value = reportParty.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                    }
                }

                stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    Margin = new Thickness(0, 0, 0, 10)
                };

            //Control Lable/Title
            label = new Label
            {
                Text = feature.Title,
                TextColor = Color.FromHex("#00314b"),
                FontSize = 17,
                Margin = new Thickness(0, 0, 0, 3),
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"
                //FontAttributes = FontAttributes.Bold
            };

                //Create a stacklayout to hold control label and feature state button if any
                var headerStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill };
                headerStack.Children.Add(label);

                //Load Feature States if any
                var _frame = AddFeatureStateOption(feature);
                if (_frame != null)
                    headerStack.Children.Add(_frame);

            //Control Entry
            viaDatePicker = new ViaDatePicker
            {
                ClassId = feature.Id,
                Date = DateTime.Now,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.White,
                Margin = new Thickness(2.5, 0, 2.5, 0),
                FontSize = 17,
                HeightRequest = 38,
                MaximumDate = DateTime.Now,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
            };

                if (CreateReport._reportStatus == ReportStatus.Edit)
                {
                    if (_value.Contains("-"))
                        SetFeatureStateOnEdit(_frame, _value);
                    else
                        viaDatePicker.Date = Convert.ToDateTime(_value);
                }

                viaDatePicker.DateSelected += (object sender, DateChangedEventArgs e) =>
                {
                    var _feature = CreateReport._controls.Libary.Features.Find(x => x.Id == (sender as ViaDatePicker).ClassId);

                    if (_feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || _feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                    {
                        //lets find any clicked button (to be identified by its background color)
                        var _featureStateStack = (((((sender as ViaDatePicker).Parent as Frame).Parent as StackLayout).Children[0] as StackLayout)
                                                 .Children[1] as Frame).Content as StackLayout;
                        var _stateBtns = _featureStateStack.Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736"));

                        if (_stateBtns.Count() > 0)
                            ToggleExtraOptions(_stateBtns.FirstOrDefault(), false);
                    }
                };

                //Frame to give the Entry control shape and border color
                frame = new Frame
                {
                    CornerRadius = 8,
                    HasShadow = false,
                    BorderColor = Color.LightSlateGray,
                    Margin = new Thickness(0, 0, 0, 10),
                    VerticalOptions = LayoutOptions.CenterAndExpand,
                    Padding = new Thickness(2, 2, 2, 2)
                };
                frame.Content = viaDatePicker; //Enclose the entry control within the frame

                stackLayout.Children.Add(headerStack);
                stackLayout.Children.Add(frame);

                //Add stacklayout to the page
                posessionStackLayout.Children.Add(stackLayout);
            }
            catch(Exception ex)
            {

            }
        }

        private void DropDownControl(Feature feature, StackLayout posessionStackLayout)
        {
            try
            {
                var _value = "";
                if (CreateReport._reportStatus == ReportStatus.Edit)
                {
                    if (feature.FeatureTypes.Contains(InvolvedTypes.Driver.ToString()) || feature.FeatureTypes.Contains(InvolvedTypes.Passenger.ToString()))
                    {
                        if (reportPerson != null)
                            _value = reportPerson.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                    }
                    else
                    {
                        if (reportParty != null)
                            _value = reportParty.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                    }
                }

            stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Margin = new Thickness(0, 0, 0, 10)
            };
            //Control label
            label = new Label
            {
                Text = feature.Title,
                FontSize = 16,
                FontAttributes = FontAttributes.Bold,
                Margin = new Thickness(0, 0, 0, 5),
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"
            };
            //Create a stacklayout to hold control label and feature state button if any
            var headerStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill };
            headerStack.Children.Add(label);

                //Load Feature States if any
                var _frame = AddFeatureStateOption(feature);
                if (_frame != null)
                    headerStack.Children.Add(_frame);

                stackLayout.Children.Add(headerStack);

            viaDropdown = new ViaDropdown
            {
                ClassId = feature.Id,
                ItemsSource = feature.Items.Select(o => o.Title).ToList(),
                Title = "Choose " + feature.Title,
                HeightRequest = 38,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
            };

                if (CreateReport._reportStatus == ReportStatus.Edit)
                {
                    if (_value.Contains("-"))
                        SetFeatureStateOnEdit(_frame, _value);
                    else
                        viaDropdown.SelectedItem = _value;
                }

                viaDropdown.SelectedIndexChanged += (sender, args) =>
                {
                    if (((((((sender as ViaDropdown).Parent as StackLayout).Parent as Frame).Parent as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor.Equals(Color.Red))
                        ((((((sender as ViaDropdown).Parent as StackLayout).Parent as Frame).Parent as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor = Color.FromHex("#00314b");

                    var _feature = CreateReport._controls.Libary.Features.Find(x => x.Id == (sender as ViaDropdown).ClassId);

                    if (_feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || _feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                    {
                    //lets find any clicked button (to be identified by its background color)
                    var _featureStateStack = (((((sender as ViaDropdown).Parent as StackLayout).Parent as Frame).Parent as StackLayout).Children[1] as Frame).Content as StackLayout;
                        var _stateBtns = _featureStateStack.Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736"));

                        if (_stateBtns.Count() > 0)
                            ToggleExtraOptions(_stateBtns.FirstOrDefault(), false);
                    }
                };

                dropStack = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(10, 0, 10, 0),
                    VerticalOptions = LayoutOptions.CenterAndExpand
                };

                dropImage = new Xamarin.Forms.Image
                {
                    Source = "ic_dropdown",
                    HeightRequest = 16,
                    WidthRequest = 16,
                    HorizontalOptions = LayoutOptions.EndAndExpand
                };
                dropStack.Children.Add(viaDropdown);
                dropStack.Children.Add(dropImage);

                frame = new Frame
                {
                    HasShadow = false,
                    CornerRadius = 8,
                    Padding = 2,
                    BorderColor = Color.LightSlateGray
                };
                frame.Content = dropStack;
                stackLayout.Children.Add(frame);

                posessionStackLayout.Children.Add(stackLayout);
            }
            catch(Exception ex)
            {

            }
        }

        PointOfImpact pointOfImpact;
        private void GridControl (Feature feature, StackLayout posessionStackLayout, string gridID)
        {
            try
            { 
                stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    Margin = new Thickness(0, 0, 0, 10)
                };

            //Control Lable/Title
            label = new Label
            {
                Text = feature.Title,
                TextColor = Color.FromHex("#00314b"),
                FontSize = 17,
                Margin = new Thickness(0, 0, 0, 3),
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"
                //FontAttributes = FontAttributes.Bold
            };

                //Create a stacklayout to hold control label and feature state button if any
                var headerStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill };
                headerStack.Children.Add(label);

                //Load Feature States if any
                var _frame = AddFeatureStateOption(feature);
                if (_frame != null)
                    headerStack.Children.Add(_frame);

                stackLayout.Children.Add(headerStack);

                var _value = "";
                if (CreateReport._reportStatus == ReportStatus.Edit)
                {
                    if (feature.FeatureTypes.Contains(InvolvedTypes.Driver.ToString()) || feature.FeatureTypes.Contains(InvolvedTypes.Passenger.ToString()))
                    {
                        if (reportPerson != null)
                            _value = reportPerson.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                    }
                    else
                    {
                        if (reportParty != null)
                            _value = reportParty.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                    }

                    if(!string.IsNullOrEmpty(_value))
                        if (_value.Contains("-")) //for feature state
                            SetFeatureStateOnEdit(_frame, _value);
                }

                var imgUrl = string.Format("http://cdn.via.nl/img/mainmot/{0}.svg", gridID);
                svgImage = new SvgCachedImage()
                {
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    HeightRequest = 100,
                    DownsampleToViewSize = true,
                    DownsampleWidth = Width,
                    RetryCount = 0,
                    RetryDelay = 250,
                    Source = SvgImageSource.FromUri(new Uri(imgUrl)),
                    Margin = new Thickness(0, 0, 0, 10)
                };

                svgImage.GestureRecognizers.Add(
                    new TapGestureRecognizer()
                    {
                        Command = new Command(async () =>
                        {
                            pointOfImpact = new PointOfImpact(partyPageState, availableParties[partyViewIndex], partyViews[partyViewIndex].selectedGrid);
                            await Navigation.PushAsync(pointOfImpact);
                        })
                    });
                stackLayout.Children.Add(svgImage);
                posessionStackLayout.Children.Add(stackLayout);
            }
            catch (Exception ex)
            {

            }
        }
        /*
         * End of dynamic controls
         */

        private void SelectedParties_ItemSelected(object sender, ItemTappedEventArgs args)
        {
            try
            {
                partyViewIndex = (selectedParties.ItemsSource as List<PartyItem>).IndexOf(args.Item as PartyItem);

                var _listView = sender as HorizontalListview;
                var selectedItem = args.Item as PartyItem;
                selectedItem.IsSelected = true;
                selectedItem.OnPropertyChanged();

                // set the text color of the selected item
                foreach (PartyItem item in _listView.ItemsSource)
                {
                    item.IsSelected = (item.Equals(selectedItem)) ? true : false;

                    item.BackgroundColor = (item.IsSelected) ? Color.FromHex("#39b835") : Color.FromHex("#6a6a77");
                    item.OnPropertyChanged();
                }

                PartiesStackControl.Children.Clear();
                PartiesStackControl.Children.Add(partyViews[partyViewIndex].ContentView);
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Validate Parties Added
        /// </summary>
        /// <returns>Boolean</returns>
        internal bool ArePartiesValid()
        {
            bool isPartiesValid = true;

            try
            {
                if (CreateReport.selectedParties.Count > 0) //Validate if there are any selected parties
                {
                    Validation();

                    //Display the first invalid page if any
                    for (var party = 0; party < partyViews.Count; party++)
                    {
                        if (!partyViews[party].isValid)
                        {
                            PartiesStackControl.Children.Clear();
                            PartiesStackControl.Children.Add(partyViews[party].ContentView);

                            isPartiesValid = partyViews[party].isValid;
                            partyViewIndex = party;

                            //Change selected partyview
                            if (availableParties.Count > 1)
                            {
                                var selectedItem = availableParties[party];
                                selectedItem.IsSelected = true;
                                selectedItem.OnPropertyChanged();
                                // set the text color of the selected item
                                foreach (PartyItem item in selectedParties.ItemsSource)
                                {
                                    item.IsSelected = (item.Equals(selectedItem)) ? true : false;

                                    item.BackgroundColor = (item.IsSelected) ? Color.FromHex("#39b835") : Color.FromHex("#6a6a77");
                                    item.OnPropertyChanged();
                                }
                            }

                            break;
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }

            return isPartiesValid;
        }


        /// <summary>
        /// Validation for all controls at party and persons involved levels
        /// </summary>
        private void Validation()
        {
            CreateReport.report.Data.Parties = new List<Party>();
            try
            {
                bool isValid = true;
                partyViewIndex = 0;

                //Validate controls for each party added
                foreach (Item _party in CreateReport.selectedParties)
                {
                    party = new Party { Id = string.Format("{0}_{1}", _party.ID, _alphabet[partyViewIndex]) };
                    party.Features = new List<ReportFeature>();
                    party.Persons = new List<Person>();

                    isValid = ValidatePartyControls(_party); //Validate parties
                    var personsValid = ValidatePersonsControls(_party); //Validate persons involved

                    isValid = (!isValid) ? false : personsValid; //Only change if parties controls are valid otherwise it sould remain false

                    partyViews[partyViewIndex].isValid = isValid;
                    if(isValid)
                        CreateReport.report.Data.Parties.Add(party);
                    partyViewIndex++;
                }
                partyViewIndex = 0;
            }
            catch (Exception ex)
            {
                Log.Warning("Validation Exception : ", ex.Message);
            }
        }

        /// <summary>
        /// Validation of persons involved 
        /// </summary>
        private bool ValidatePersonsControls(Item _party)
        {
            bool isValid = true;
            try
            {
                var lastStackIndex = (partyViews[partyViewIndex].ContentView.Content as StackLayout).Children.Count - 1;
                var personsStackLayout = ((partyViews[partyViewIndex].ContentView.Content as StackLayout).Children[lastStackIndex] as StackLayout);
                int i = 0;
                foreach (StackLayout _stackLayout in personsStackLayout.Children)
                {
                    int featurePos = 0;
                    if (_stackLayout.ClassId == InvolvedTypes.Driver.ToString())
                    {
                        featurePos = 2; //Positions of dynamically added elements start from 2 cuz of the header and victim type control
                        person = new Person { Id = string.Format("{0}_{1}_{2}", _party.ID, _alphabet[partyViewIndex], i++) };
                        person.Features = new List<ReportFeature>();
                        person.Type = FeatureType.Driver.ToString();
                        isValid = ValidateVictimType(person, _stackLayout);

                        driverFeatures = CreateReport._controls.Libary.Features.Where(x => x.FeatureTypes.Contains(FeatureType.Driver.ToString())).ToList();
                        foreach (Feature feature in driverFeatures)
                        {
                            if (_party.ExcludeFeatureIds.Where(y => y.FeatureId == feature.Id).Count() == 1) //check if feature is excluded from party
                                continue;
                            else
                            {
                                var controlValid = ValidateControl(feature, featurePos, _stackLayout);
                                isValid = (!isValid) ? false : controlValid;
                                featurePos++;
                            }
                        }

                        party.Persons.Add(person);
                    }
                    else if (_stackLayout.ClassId == InvolvedTypes.Passenger.ToString())
                    {
                        featurePos = 1; //Positions of dynamically added elements start from 1 cuz of the victim type control

                        //Create new person
                        person = new Person { Id = string.Format("{0}_{1}_{2}", _party.ID, _alphabet[partyViewIndex], i++) };
                        person.Features = new List<ReportFeature>();
                        passengerFeatures = CreateReport._controls.Libary.Features.Where(x => x.FeatureTypes.Contains(FeatureType.Passenger.ToString())).ToList();
                        person.Type = FeatureType.Passenger.ToString();
                        isValid = ValidateVictimType(person, _stackLayout);

                        foreach (Feature feature in passengerFeatures)
                        {
                            if (_party.ExcludeFeatureIds.Where(y => y.FeatureId == feature.Id).Count() == 1) //check if feature is excluded from party
                                continue;
                            else
                            {
                                var controlValid = ValidateControl(feature, featurePos, (_stackLayout.Children[1] as StackLayout));
                                isValid = (!isValid) ? false : controlValid;
                                featurePos++;
                            }
                        }

                        party.Persons.Add(person);
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return isValid;
        }

        private bool ValidateVictimType(Person _person, StackLayout _stack)
        {
            RadioButtonGroupView radioGroupView = null;
            if(_person.Type == FeatureType.Passenger.ToString())
                radioGroupView = ((_stack.Children[1] as StackLayout).Children[0] as StackLayout).Children[1] as RadioButtonGroupView;
            else
                radioGroupView = (_stack.Children[1] as StackLayout).Children[1] as RadioButtonGroupView;

            //Atleast an item should be selected
            if (radioGroupView.SelectedIndex == -1)
            {
                ((radioGroupView.Parent as StackLayout).Children[0] as Label).TextColor = Color.Red;
                return false;
            }

            //Assign victimtype the selected option
            _person.VictimType = victimType[radioGroupView.SelectedIndex];

            //Collect date of death if selected option is killed
            if(_person.VictimType == VictimType.Killed.ToString())
                ValidateDateOfDeath(_person, _stack);

            return true;
        }

        private bool ValidateDateOfDeath(Person _person, StackLayout _stack)
        {
            if(_person.Type == FeatureType.Passenger.ToString())
                _person.DateOfDeath = ((((_stack.Children[1] as StackLayout).Children[_stack.Children.Count - 1] as StackLayout).Children[1] as Frame).Content as ViaDatePicker).Date.ToString("yyyyMMdd");
            else
                _person.DateOfDeath = (((_stack.Children[_stack.Children.Count - 1] as StackLayout).Children[1] as Frame).Content as ViaDatePicker).Date.ToString("yyyyMMdd");

            return true;
        }

        /// <summary>
        /// Validation for party controls
        /// Returns a boolean value if all party controls are valid
        /// </summary>
        private bool ValidatePartyControls(Item party)
        {
            int featurePos = 1;
            bool isControlValid = true;
            partyFeatures = CreateReport._controls.Libary.Features.Where(x => x.FeatureTypes.FirstOrDefault() == FeatureType.Party.ToString()).ToList();

            foreach (Feature feature in partyFeatures) {
                if (party.ExcludeFeatureIds.Where(y => y.FeatureId == feature.Id).Count() == 1) //check if feature is excluded from party
                    continue;
                else
                {
                    if (Convert.ToInt32(feature.Id) == CreateReport._controls.Libary.ImpactPointId)
                    {
                        var ImpactPoint = CreateReport._controls.Libary.ImpactPoints.Find(x => x.Grid == party.Grid);
                        if (ImpactPoint != null && !string.IsNullOrEmpty(ImpactPoint.Grid))
                        {
                            ValidateGridControl(feature, featurePos, (partyViews[partyViewIndex].ContentView.Content as StackLayout));
                            featurePos++;
                            continue;
                        }
                        else
                        {
                            var controlValid = ValidateControl(feature, featurePos, (partyViews[partyViewIndex].ContentView.Content as StackLayout));
                            isControlValid = (!isControlValid) ? false : controlValid;
                        }
                        featurePos++;
                    }
                    else
                    {
                        {
                            var controlValid = ValidateControl(feature, featurePos, (partyViews[partyViewIndex].ContentView.Content as StackLayout));
                            isControlValid = (!isControlValid) ? false : controlValid;
                        }
                        featurePos++;
                    }
                }
            }

            return isControlValid;
        }

        /// <summary>
        /// Validate controls according to the feature, position of that control in the StackLayout
        /// And the StackLayout in which it is contained
        /// </summary>
        private bool ValidateControl(Feature feature, int featurePos, StackLayout stackLayout)
        {
            try
            {
                bool isAccidentValid = true;
                //if (feature.FeatureStates.Contains(FeatureStates.Required)) //Check to see if feature state is required
                switch (feature.FeatureInputType)
                {
                    case "Text":
                        isAccidentValid = ValidateEntryControl(feature, featurePos, stackLayout);
                        break;
                    case "Number":
                        isAccidentValid = ValidateNumericControl(feature, featurePos, stackLayout);
                        break;
                    case "SingleChoice":
                        if (feature.Items.Count == 2)
                            isAccidentValid = ValidateSwitchControl(feature, featurePos, stackLayout);
                        else if (feature.Items.Count > 15)
                            isAccidentValid = ValidateDropdownControl(feature, featurePos, stackLayout);
                        else
                            isAccidentValid = ValidateSingleSelectControl(feature, featurePos, stackLayout);
                        break;
                    case "MultiChoice":
                        isAccidentValid = ValidateMultiSelectControl(feature, featurePos, stackLayout);
                        break;
                    case "Date":
                        isAccidentValid = ValidateDateControl(feature, featurePos, stackLayout);
                        break;
                }

                return isAccidentValid;
            }
            catch(Exception ex)
            {

            }
            return false;
        }

        /// <summary>
        /// Validation for MultiChoice (Checkboxes)
        /// </summary>
        private bool ValidateMultiSelectControl(Feature feature, int featurePos, StackLayout stackLayout)
        {
            try
            {
                var _featureStateValue = "";

                //Check if control has feature states
                if (feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                {
                    //find any clicked buttons
                    var featureStateBtn = ((((stackLayout.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[1] as Frame)
                                      .Content as StackLayout).Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736"));

                    _featureStateValue = GetSelectedFeatureState((featureStateBtn.FirstOrDefault() as Button), feature);
                }

                var checkedCheckBoxes = ((stackLayout.Children[featurePos] as StackLayout).Children[1] as Grid).Children.Where(x => (x as CheckBox).IsChecked).ToList();

                if (feature.FeatureStates.Contains(FeatureStates.Required.ToString()))
                    if (checkedCheckBoxes.Count() == 0 && string.IsNullOrEmpty(_featureStateValue))
                    {
                        (((stackLayout.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor = Color.Red;
                        return false;
                    }

                List<string> checkedValues = new List<string>();
                if (checkedCheckBoxes.Count > 0)
                {
                    foreach (View _checKView in checkedCheckBoxes)
                        checkedValues.Add((_checKView as CheckBox).ClassId);
                }
                else
                {
                    checkedValues.Add(_featureStateValue);
                }

                if (feature.FeatureTypes.Contains(InvolvedTypes.Driver.ToString()) || feature.FeatureTypes.Contains(InvolvedTypes.Passenger.ToString()))
                {
                    person.Features.Add(new ReportFeature { Id = feature.Id, Values = checkedValues });
                }
                else
                {
                    party.Features.Add(new ReportFeature { Id = feature.Id, Values = checkedValues });
                }
            }
            catch(Exception ex)
            {

            }

            return true;
        }

        /// <summary>
        /// Validation for a Radio Buttons(SingleChoice)
        /// </summary>
        private bool ValidateSingleSelectControl(Feature feature, int featurePos, StackLayout stackLayout)
        {
            try
            {
                bool _isValid = true;
                var _value = ""; var _featureStateValue = "";

                //Check if control has feature states
                if (feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                {
                    //find any clicked buttons
                    var featureStateBtns = ((((stackLayout.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[1] as Frame)
                                      .Content as StackLayout).Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736")).ToList();

                    if (featureStateBtns.Count > 0) //No button has been pressed/selected
                        _featureStateValue = GetSelectedFeatureState((featureStateBtns.FirstOrDefault() as Button), feature);
                }

                if ((stackLayout.Children[featurePos] as StackLayout).IsVisible) //Check for controls visibility
                {
                    var radioGroupView = ((stackLayout.Children[featurePos] as StackLayout).Children[1] as RadioButtonGroupView);

                    if (radioGroupView.SelectedIndex == -1 && string.IsNullOrEmpty(_featureStateValue))
                    {
                        _isValid = false;
                    }

                    if (feature.FeatureStates.Contains(FeatureStates.AllowOther.ToString()))
                    {
                        var otherEntry = ((radioGroupView.Children[0] as Grid).Children[(radioGroupView.Children[0] as Grid).Children.Count - 1] as StackLayout).Children[1] as ViaEntry;
                        //Other radio is the last, therefore it's position is the number of items on the feature
                        if (radioGroupView.SelectedIndex == feature.Items.Count)
                            if (string.IsNullOrEmpty(otherEntry.Text))
                            {
                                otherEntry.BorderColor = Color.Red;
                                _isValid = false;
                            }
                            else
                            {
                                _value = otherEntry.Text;
                            }
                    }

                    if (feature.FeatureStates.Contains(FeatureStates.Required.ToString()))
                        if (!_isValid && string.IsNullOrEmpty(_featureStateValue))
                        {
                            (((stackLayout.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor = Color.Red;
                            return false;
                        }

                    if (radioGroupView.SelectedIndex >= 0) //Radiobutton selected
                    {
                        if (string.IsNullOrEmpty(_value))
                        {
                            var selectedRadio = radioGroupView.SelectedItem.ToString();
                            _value = feature.Items.Find(x => x.Id == selectedRadio).Id;
                        }
                    }
                    else if (!string.IsNullOrEmpty(_featureStateValue)) //feature state is selected
                        _value = _featureStateValue;


                    if (feature.FeatureTypes.Contains(InvolvedTypes.Driver.ToString()) || feature.FeatureTypes.Contains(InvolvedTypes.Passenger.ToString()))
                    {
                        person.Features.Add(
                            new ReportFeature
                            {
                                Id = feature.Id,
                                Values = new List<string>() { _value }
                            });
                    }
                    else
                    {
                        party.Features.Add(
                            new ReportFeature
                            {
                                Id = feature.Id,
                                Values = new List<string>() { _value }
                            });
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return true;
        }

        //Validate feature states if any
        private string GetSelectedFeatureState(Button featureState, Feature feature)
        {            //Check if button is selected
            if (featureState.BackgroundColor.Equals(Color.FromHex("#eea736")))
                return featureState.ClassId; //selected value

            return null;
        }

        /// <summary>
        /// Validation for a Switch Control (Composed of 2 buttons with the selected button being colored)
        /// </summary>
        private bool ValidateSwitchControl(Feature feature, int featurePos, StackLayout stackLayout)
        {
            try
            {
                var button = ((((stackLayout.Children[featurePos] as StackLayout).Children[1] as Frame).Content as StackLayout).Children[0] as Button);
                var button2 = ((((stackLayout.Children[featurePos] as StackLayout).Children[1] as Frame).Content as StackLayout).Children[1] as Button);

                var _selectedValue = ""; var _featureStateValue = "";

                //Check if control has featurestates
                if (feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                {
                    //find any clicked buttons
                    var featureStateBtns = ((((stackLayout.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[1] as Frame)
                                     .Content as StackLayout).Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736")).ToList();

                    if (featureStateBtns.Count > 0) //No button has been pressed/selected
                        _featureStateValue = GetSelectedFeatureState((featureStateBtns.FirstOrDefault() as Button), feature);
                }

                //Check for selected button
                if (button.BackgroundColor != Color.White)
                {
                    _selectedValue = feature.Items[0].Id;
                }
                else if (button2.BackgroundColor != Color.White)
                {
                    _selectedValue = feature.Items[1].Id;
                }

                if (feature.FeatureStates.Contains(FeatureStates.Required.ToString()))
                    if (string.IsNullOrEmpty(_selectedValue) && string.IsNullOrEmpty(_featureStateValue))
                    {
                        (((stackLayout.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor = Color.Red;
                        return false;
                    }

                if (string.IsNullOrEmpty(_selectedValue))
                    _selectedValue = _featureStateValue;

                if (feature.FeatureTypes.Contains(InvolvedTypes.Driver.ToString()) || feature.FeatureTypes.Contains(InvolvedTypes.Passenger.ToString()))
                {
                    person.Features.Add(new ReportFeature { Id = feature.Id, Values = new List<string>() { _selectedValue } });
                }
                else
                {
                    party.Features.Add(new ReportFeature { Id = feature.Id, Values = new List<string>() { _selectedValue } });
                }
            }
            catch(Exception ex)
            {

            }

            return true;
        }

        /// <summary>
        /// Validation for a Number Entry Control
        /// </summary>
        private bool ValidateNumericControl(Feature feature, int featurePos, StackLayout stackLayout)
        {
            try
            {
                var _featureStateValue = ""; string _itemValue = "";
                //Check if control has featurestates
                if (feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                {
                    //find any clicked buttons
                    var featureStateBtns = ((((stackLayout.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[1] as Frame)
                                      .Content as StackLayout).Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736")).ToList();

                    if (featureStateBtns.Count > 0) //No button has been pressed/selected
                        _featureStateValue = GetSelectedFeatureState((featureStateBtns.FirstOrDefault() as Button), feature);
                }

                var _entryValue = (((stackLayout.Children[featurePos] as StackLayout).Children[1] as Frame).Content as ViaEntry).Text;

                //A feature's validation can be required or not. Only those that are required shall be validated
                if (feature.FeatureStates.Contains(FeatureStates.Required.ToString()))
                    //check if nothing was entered into the entry control
                    if (string.IsNullOrEmpty(_entryValue) && string.IsNullOrEmpty(_featureStateValue))
                    {
                        //change the border color if not valid
                        ((stackLayout.Children[featurePos] as StackLayout).Children[1] as Frame).BorderColor = Color.Red;
                        return false;
                    }

                _itemValue = (!string.IsNullOrEmpty(_entryValue)) ? _entryValue : _featureStateValue;

                if (feature.FeatureTypes.Contains(InvolvedTypes.Driver.ToString()) || feature.FeatureTypes.Contains(InvolvedTypes.Passenger.ToString()))
                {
                    person.Features.Add(new ReportFeature { Id = feature.Id, Values = new List<string>() { _itemValue } });
                }
                else
                {
                    party.Features.Add(new ReportFeature { Id = feature.Id, Values = new List<string>() { _itemValue } });
                }
            }
            catch(Exception ex)
            {

            }

            return true;
        }

        /// <summary>
        /// Validation for a Text Entry Control
        /// </summary>
        private bool ValidateEntryControl(Feature feature, int featurePos, StackLayout stackLayout)
        {
            try
            {
                var _featureStateValue = ""; string _itemValue = "";
                //Check if control has featurestates
                if (feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                {
                    //find any clicked buttons
                    var featureStateBtns = ((((stackLayout.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[1] as Frame)
                                      .Content as StackLayout).Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736")).ToList();

                    if (featureStateBtns.Count > 0) //No button has been pressed/selected
                        _featureStateValue = GetSelectedFeatureState((featureStateBtns.FirstOrDefault() as Button), feature);
                }

                var _entryValue = (((stackLayout.Children[featurePos] as StackLayout).Children[1] as Frame).Content as ViaEntry).Text;

                //A feature's validation can be required or not. Only those that are required shall be validated
                if (feature.FeatureStates.Contains(FeatureStates.Required.ToString()))
                    //check if nothing was entered into the entry control
                    if (string.IsNullOrEmpty(_entryValue) && string.IsNullOrEmpty(_featureStateValue))
                    {
                        //change the border color if not valid
                        ((stackLayout.Children[featurePos] as StackLayout).Children[1] as Frame).BorderColor = Color.Red;
                        return false;
                    }

                _itemValue = (!string.IsNullOrEmpty(_entryValue)) ? _entryValue : _featureStateValue;

                if (feature.FeatureTypes.Contains(InvolvedTypes.Driver.ToString()) || feature.FeatureTypes.Contains(InvolvedTypes.Passenger.ToString()))
                {
                    person.Features.Add(new ReportFeature { Id = feature.Id, Values = new List<string>() { _itemValue } });
                }
                else
                {
                    party.Features.Add(new ReportFeature { Id = feature.Id, Values = new List<string>() { _itemValue } });
                }
            }
            catch(Exception ex)
            {

            }

            return true;
        }

        /// <summary>
        /// Validate Dropdown (Used for SingleChoice with Items.Count > 15)
        /// </summary>
        private bool ValidateDropdownControl(Feature feature, int featurePos, StackLayout stackLayout)
        {
            try
            {
                var _featureStateValue = ""; string _Value = "";

                //Check if control has featurestates
                if (feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                {
                    //find any clicked buttons
                    var featureStateBtns = ((((stackLayout.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[1] as Frame)
                                      .Content as StackLayout).Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736")).ToList();

                    if (featureStateBtns.Count > 0) //No button has been pressed/selected
                        _featureStateValue = GetSelectedFeatureState((featureStateBtns.FirstOrDefault() as Button), feature);
                }


                //A feature's validation can be required or not. Only those that are required shall be validated
                if (feature.FeatureStates.Contains(FeatureStates.Required.ToString()))
                    //check for any radio button selected, At this point neither control nor feature state option is selected
                    if (((((stackLayout.Children[featurePos] as StackLayout).Children[1] as Frame).Content as StackLayout).Children[0] as ViaDropdown).SelectedIndex == -1
                       && string.IsNullOrEmpty(_featureStateValue))
                    {
                        //change control label text color if none is selected
                        (((stackLayout.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor = Color.Red;
                        return false;
                    }

                //get selected radio button
                int selectedIndex = ((((stackLayout.Children[featurePos] as StackLayout).Children[1] as Frame).Content as StackLayout).Children[0] as ViaDropdown).SelectedIndex;

                //get value using selectedindex or
                //value takes on feature state value
                _Value = (selectedIndex >= 0) ? feature.Items[selectedIndex].Id : _featureStateValue;

                if (feature.FeatureTypes.Contains(InvolvedTypes.Driver.ToString()) || feature.FeatureTypes.Contains(InvolvedTypes.Passenger.ToString()))
                {
                    person.Features.Add(new ReportFeature { Id = feature.Id, Values = new List<string>() { _Value } });
                }
                else
                {
                    party.Features.Add(new ReportFeature { Id = feature.Id, Values = new List<string>() { _Value } });
                }
            }
            catch(Exception ex)
            {

            }
            return true;
        }

        /// <summary>
        /// Validate Date Control
        /// </summary>
        private bool ValidateDateControl(Feature feature, int featurePos, StackLayout stackLayout)
        {
            try
            {
                string _featureStateValue = "", _value = "";
                //Check if control has featurestates
                if (feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                {
                    //find any clicked buttons
                    var featureStateBtns = ((((stackLayout.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[1] as Frame)
                                     .Content as StackLayout).Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736")).ToList();

                    if (featureStateBtns.Count > 0) //No button has been pressed/selected
                        _featureStateValue = GetSelectedFeatureState((featureStateBtns.FirstOrDefault() as Button), feature);
                }

                var dateSelected = (((stackLayout.Children[featurePos] as StackLayout).Children[1] as Frame).Content as ViaDatePicker).Date;
                _value = (!string.IsNullOrEmpty(_featureStateValue)) ? _featureStateValue : dateSelected.ToString("yyyyMMdd");

                if (feature.FeatureTypes.Contains(InvolvedTypes.Driver.ToString()) || feature.FeatureTypes.Contains(InvolvedTypes.Passenger.ToString()))
                {
                    person.Features.Add(new ReportFeature { Id = feature.Id, Values = new List<string>() { _value } });
                }
                else
                {
                    party.Features.Add(new ReportFeature { Id = feature.Id, Values = new List<string>() { _value } });
                }
            }
            catch(Exception ex)
            {

            }
            return true;
        }

        /// <summary>
        /// Validation for a Point Of Impact Control
        /// </summary>
        private bool ValidateGridControl(Feature feature, int featurePos, StackLayout stackLayout)
        {
            try
            {
                string _featureStateValue = "", _value = "";
                //Check if control has featurestates
                if (feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                {
                    //find any clicked buttons
                    var featureStateBtns = ((((stackLayout.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[1] as Frame)
                                     .Content as StackLayout).Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736")).ToList();

                    if (featureStateBtns.Count > 0) //No button has been pressed/selected
                        _featureStateValue = GetSelectedFeatureState((featureStateBtns.FirstOrDefault() as Button), feature);
                }

                if (string.IsNullOrEmpty(partyViews[partyViewIndex].selectedGrid) && string.IsNullOrEmpty(_featureStateValue))
                    (((stackLayout.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor = Color.Red;
                else
                    _value = (!string.IsNullOrEmpty(partyViews[partyViewIndex].selectedGrid)) ? partyViews[partyViewIndex].selectedGrid : _featureStateValue;

                party.Features.Add(new ReportFeature { Id = feature.Id, Values = new List<string>() { _value } });
            }
            catch(Exception ex)
            {

            }
            return true;
        }

        private AccidentModes accidentModes;
        private async void AddNewParty(object sender, EventArgs args)
        {
            accidentModes = new AccidentModes(ReportStatus.Create);
            accidentModes.partyPageState = this;
            await Navigation.PushAsync(accidentModes);
        }

        /// <summary>
        /// On return form point of impact page
        /// <summary>
        public void OnResumePage(List<Item> newlyAddedParties)
        {
            try
            {
                Navigation.RemovePage(accidentModes);

                CreateAddedPartyViews();
            }
            catch(Exception ex)
            {

            }
        }

        public void CreateAddedPartyViews()
        {
            try
            {
                List<Item> newSelectedParties = CreateReport.selectedParties.Where(p => !availableParties.Any(p2 => p2.ID == p.ID)).ToList<Item>();


                if (newSelectedParties.Count > 0)
                {
                    var currentIndex = partyViewIndex;

                    int i = 0;
                    foreach (Item party in newSelectedParties)
                    {
                        if (partyViews.Count > 0) //do we have existing party views
                            partyViewIndex++;

                        var partyItem = new PartyItem
                        {
                            ID = party.ID,
                            Icon = party.Icon,
                            InvolvedTypes = party.InvolvedTypes,
                            Grid = party.Grid,
                            Tags = party.Tags,
                            ExcludeFeatureIds = party.ExcludeFeatureIds,
                            Title = party.Title,
                            BackgroundColor = (availableParties.Count == 0 && i == 0) ? Color.FromHex("#39b835") : Color.FromHex("#6a6a77")
                        };

                        availableParties.Add(partyItem);
                        LoadParty(party);
                        i++;
                    }

                    //Load Parties list
                    if (selectedParties.Content != null)
                    {
                        (selectedParties.Content as StackLayout).Children.Clear();
                        selectedParties.Render();
                    }

                    selectedParties.ItemsSource = null;
                    selectedParties.ItemsSource = availableParties;

                    partyViewIndex = currentIndex;
                    if (availableParties.Count > 0)
                    {
                        if (partyViewIndex == -1)
                            partyViewIndex++;
                        selectedParties.SelectedItem = availableParties[partyViewIndex];
                    }

                    if (availableParties.Count == 1)
                    {
                        PartiesStackControl.Children.Clear();
                        PartiesStackControl.Children.Add(partyViews[partyViewIndex].ContentView);
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        public void OnPartyRemoved(int partyPageIndex)
        {
            try
            {
                if (partyViews.Count > partyPageIndex)
                {
                    partyViews.RemoveAt(partyPageIndex);
                    availableParties.RemoveAt(partyPageIndex);

                    //Check if page currently being viewed is being deleted
                    if (partyPageIndex == partyViewIndex)
                        RemoveCurrentParty();

                    //Load Parties list
                    selectedParties.Content = null;
                    selectedParties.Render();

                    selectedParties.ItemsSource = null;
                    selectedParties.ItemsSource = availableParties;
                    selectedParties.SelectedItem = availableParties[partyViewIndex];
                }
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Use on sleep to remove an existing party
        /// </summary>
        public void OnSleepPage()
        {
            
        }

        public void OnStartPage()
        {
            
        }

        //Load party to be edited
        public void OnPartyEdit(int partyIndex)
        {
            try
            {
                //Return to parties page
                CreateReport.reportPageState.OnNavigateToPage(ReportStage.Parties);

                selectedParties.SelectedItem = availableParties[partyIndex];
                PartiesStackControl.Children.Add(partyViews[partyIndex].ContentView);
            }
            catch(Exception ex)
            {

            }
        }

        public void OnGridSelected(string selectedGrid)
        {
            Navigation.RemovePage(pointOfImpact);
            partyViews[partyViewIndex].selectedGrid = selectedGrid;
        }
    }
}