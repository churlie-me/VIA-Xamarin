using System;
using System.Collections.Generic;
using Plugin.InputKit.Shared.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Linq;
using Via.Models;
using ViaDropdown = Via.Controls.Dropdown;
using Via.Controls;
using Via.Helpers;
using System.Diagnostics;

namespace Via.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Accident : ContentView, AccidentPageState
    {
        public static List<Item> availableParties = new List<Item>();
        private readonly Control controls;
        public static ReportData report;
        private RadioButton radioButton;
        private RadioButtonGroupView groupRadioButton;
        private List<Feature> accidentFeatures;
        private Frame frame;
        private ViaEntry viaEntry;
        private StackLayout stackLayout, switchStackLayout, dropStack;
        private ViaDropdown viaDropdown;
        private Xamarin.Forms.Image dropImage;
        private Label label;
        private Grid grid;
        private CheckBox checkBox;
        private Button buttonOn, buttonOff;
        public static AccidentPageState accidentPageState;

        public Accident()
        {
            accidentPageState = this;
            InitializeComponent();
            report = CreateReport.report;
            availableParties = CreateReport._controls.Libary.AvailableParties.Items;

            LoadControls();
        }

        //Load Controls based on the feature selection mode
        private void LoadControls()
        {
            accidentFeatures = CreateReport._controls.Libary.Features.Where(x => x.FeatureTypes.FirstOrDefault() == FeatureType.Accident.ToString()).ToList();

            foreach (Feature feature in accidentFeatures)
                switch (feature.FeatureInputType)
                {
                    case "Text":
                        EntryControl(feature);
                        break;
                    case "Number":
                        NumericControl(feature);
                        break;
                    case "SingleChoice":
                        if (feature.Items.Count == 2)
                            SwitchControl(feature);
                        else if (feature.Items.Count > 15)
                            DropDownControl(feature);
                        else
                            SingleSelectControl(feature);
                        break;
                    case "MultiChoice":
                        MultiSelectControl(feature);
                        break;
                    case "Date":
                        DateTimeControl(feature);
                        break;
                }
        }

        //SingleSelect Control with not more than 15 items
        private void SingleSelectControl(Feature feature)
        {
            try
            {
                //control stacklayout
                stackLayout = new StackLayout
                {
                    IsVisible = (feature.Id == "154" || feature.Id == "166") ? false : true,
                    Orientation = StackOrientation.Vertical,
                    Margin = new Thickness(0, 0, 0, 10)
                };

            //Control Lable/Title
            label = new Label
            {
                Text = feature.Title,
                TextColor = Color.FromHex("#FF00314b"),
                FontSize = 16,
                Margin = new Thickness(0, 0, 0, 3),
                FontAttributes = FontAttributes.Bold,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"

            };

                //Create a stacklayout to hold control label and feature state button if any
                var headerStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill };
                headerStack.Children.Add(label);

                //Load Feature States if any
                var _frame = AddFeatureStateOption(feature);
                if (_frame != null)
                    headerStack.Children.Add(_frame);


                //GroupButtonView in a grid
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
                var value = "";
                if (CreateReport.report.Data.Features.Count > 0 && CreateReport._reportStatus == ReportStatus.Edit)
                {
                    ReportFeature rFeature = CreateReport.report.Data.Features.Find(x => x.Id == feature.Id);
                    if (rFeature != null)
                        if (rFeature.Values.Count > 0)
                            value = rFeature.Values.FirstOrDefault();
                }

                foreach (var classification in feature.Items)
                {
                    radioButton = new RadioButton();
                    radioButton.HorizontalOptions = LayoutOptions.Start;
                    radioButton.VerticalOptions = LayoutOptions.Center;
                    radioButton.TextFontSize = 15;
                    radioButton.Text = classification.Title;
                    radioButton.Value = classification.Id;

                    if (CreateReport._reportStatus == ReportStatus.Edit)
                        if (!string.IsNullOrEmpty(value))
                            if (value == classification.Id && !value.Contains("-"))
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
                    if (!string.IsNullOrEmpty(value))
                        if (value.Contains("-"))
                            SetFeatureStateOnEdit(_frame, value);

                //Manually add an extra option for features with feature states Allow other
                if (feature.FeatureStates.Contains(FeatureStates.AllowOther.ToString()))
                {
                    radioButton = new RadioButton();
                    radioButton.HorizontalOptions = LayoutOptions.Start;
                    radioButton.VerticalOptions = LayoutOptions.Center;
                    radioButton.TextFontSize = 15;
                    radioButton.Text = "Other";
                    radioButton.Value = 0;

                    //Add entry on other selection
                    var other = new ViaEntry
                    {
                        BorderColor = Color.LightSlateGray,
                        BackgroundColor = Color.White,
                        Keyboard = Keyboard.Numeric,
                        FontSize = 17,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Margin = new Thickness(5, 0, 0, 0),
                        HeightRequest = 25,
                        IsCurvedCornersEnabled = true
                    };

                    var _nestedStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand };

                    _nestedStack.Children.Add(radioButton);
                    _nestedStack.Children.Add(other);

                    if ((i % 2).Equals(1))
                    {
                        grid.Children.Add(_nestedStack, 0, gridRow);
                    }
                    else
                    {
                        grid.Children.Add(_nestedStack, 1, gridRow);
                    }
                    gridRow++; i++;
                }

                groupRadioButton = new RadioButtonGroupView();
                groupRadioButton.ClassId = feature.Id;
                groupRadioButton.Children.Add(grid);

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

                stackLayout.Children.Add(headerStack);
                stackLayout.Children.Add(groupRadioButton);

                //Add stacklayout to the page
                accidentStack.Children.Add(stackLayout);
            }
            catch(Exception ex)
            {

            }
        }

        void ToggleExtraOptions(object sender, bool isSeleted)
        {
            if (isSeleted)
            {
                (((((sender as Button).Parent as StackLayout).Parent as Frame).Parent as StackLayout).Children[0] as Label).TextColor = Color.FromHex("#00314b");
                (sender as Button).BackgroundColor = Color.FromHex("#eea736");
                (sender as Button).TextColor = Color.White;
                (sender as Button).FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf";
            }
            else
            {
                (sender as Button).BackgroundColor = Color.White;
                (sender as Button).TextColor = Color.Default;
                (sender as Button).FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf";
            }
        }

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
                        if((sender as Button).BackgroundColor == Color.FromHex("#eea736"))
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
                    HeightRequest = 30,
                    IsClippedToBounds = true,
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

        //Multi Select Control
        private void MultiSelectControl(Feature feature)
        {
            try
            {
                //feature stack
                stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    Margin = new Thickness(0, 0, 0, 10)
                };

            //control label
            label = new Label
            {
                Text = feature.Title,
                TextColor = Color.FromHex("#00314b"),
                FontSize = 17,
                Margin = new Thickness(0, 0, 0, 3),
                //FontAttributes = FontAttributes.Bold,
               FontFamily= Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"
        };

                //Create a stacklayout to hold control label and feature state button if any
                var headerStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill };
                headerStack.Children.Add(label);

                //Load Feature States if any
                var _frame = AddFeatureStateOption(feature);
                if (_frame != null)
                    headerStack.Children.Add(_frame);

                int i = 1, gridRow = 0;
                //checkboxes are arranged in a grid
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

                var values = new List<string>();
                if (CreateReport._reportStatus == ReportStatus.Edit)
                {
                    if (CreateReport.report.Data.Features.Count > 0)
                        values = CreateReport.report.Data.Features.Find(x => x.Id == feature.Id).Values;

                    //Feature state value selected
                    if (values.Count == 1)
                        if (values.FirstOrDefault().Contains("-"))
                            SetFeatureStateOnEdit(_frame, values.FirstOrDefault());
                }

            //create checkboxes for each feature item
            foreach (var option in feature.Items)
            {
                checkBox = new CheckBox
                {
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.Center,
                    TextFontSize = 15,
                    Text = option.Title,
                    FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf",
                ClassId = option.Id
                };

                    //Add values if an existing report is being edited
                    if (CreateReport._reportStatus == ReportStatus.Edit)
                    {
                        if (values.Count > 0)
                        {
                            var value = values.Find(x => x == option.Id);
                            if (!string.IsNullOrEmpty(value) && !value.Contains("-"))
                                checkBox.IsChecked = true;
                        }
                    }

                    string _ID = feature.Id;
                    checkBox.CheckChanged += (sender, args) =>
                    {
                    //Changing the Permitted Category Label
                    if ((((((sender as CheckBox).Parent as Grid).Parent as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor.Equals(Color.Red))
                            (((((sender as CheckBox).Parent as Grid).Parent as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor = Color.FromHex("#00314b");

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

                stackLayout.Children.Add(headerStack);
                stackLayout.Children.Add(grid);

                //Add stacklayout to accident page
                accidentStack.Children.Add(stackLayout);
            }
            catch(Exception ex)
            {

            }
        }

        //Text Control
        public void EntryControl(Feature feature)
        {
            try
            {
                //feature control
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
                Margin = new Thickness(0,0,0,3),
                // FontAttributes = FontAttributes.Bold
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"
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
                BorderColor = Color.White,
                BackgroundColor = Color.White,
                Keyboard = Keyboard.Text,
                Margin = new Thickness(2.5,0,2.5,0),
                CornerRadius = 8,
                FontSize = 17,
                HeightRequest = 38,
                IsCurvedCornersEnabled = true,
                ClassId = feature.Id,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
        };

                if (CreateReport._reportStatus == ReportStatus.Edit)
                {
                    string value = CreateReport.report.Data.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (value.Contains("-"))
                            SetFeatureStateOnEdit(_frame, value);
                        else
                            if (!string.IsNullOrEmpty(value))
                            viaEntry.Text = value.ToString();
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
                    Padding = new Thickness(2)
                };
                frame.Content = viaEntry; //Enclose the entry control within the frame

                stackLayout.Children.Add(headerStack);
                stackLayout.Children.Add(frame);

                //Add stacklayout to the page
                accidentStack.Children.Add(stackLayout);
            }
            catch(Exception ex)
            {

            }
        }

        private void SetFeatureStateOnEdit(Frame _frame ,string featureStateValue)
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

        //Text Changed Event for Entry Controls
        private void viaEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (((sender as ViaEntry).Parent as Frame).BorderColor.Equals(Color.Red))
                    ((sender as ViaEntry).Parent as Frame).BorderColor = Color.FromHex("#00314b");

                //Check for any featurhe states options, if selected, then only one q
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

        //Numeric Control
        public void NumericControl(Feature feature)
        {
            try
            {
                //feature stack
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
                //FontAttributes = FontAttributes.Bold
               FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"
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
                BorderColor = Color.White,
                BackgroundColor = Color.White,
                Keyboard = Keyboard.Numeric,
                FontSize = 17,
                Margin = new Thickness(2.5, 0, 2.5, 0),
                HeightRequest = 38,
                IsCurvedCornersEnabled = true,
                ClassId = feature.Id,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
        };

                //On edit report
                if (CreateReport._reportStatus == ReportStatus.Edit)
                {
                    var value = CreateReport.report.Data.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (value.Contains("-"))
                            SetFeatureStateOnEdit(_frame, value);
                        else
                            viaEntry.Text = value;
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
                accidentStack.Children.Add(stackLayout);
            }
            catch(Exception ex)
            {

            }
        }

        //Switch Control
        private void SwitchControl(Feature feature)
        {
            try
            {
                //feature stack
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
                //FontAttributes = FontAttributes.Bold
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-Bold" : "TitilliumWeb-Bold.ttf"
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
                    Orientation = StackOrientation.Horizontal,
                    ClassId = feature.Id
                };

            buttonOn = new Button
            {
                Text = feature.Items[0].Title,
                HeightRequest = 30,
                CornerRadius = 30,
                ClassId = feature.Items[0].Id,
                Margin = new Thickness(0, 0, 0, 0),
                WidthRequest = 120,
                TextColor = Color.Default,
                Padding = 1,
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

                    ToggleControls(sender, true);

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
                ClassId = feature.Items[1].Id,
                Margin = new Thickness(-5, 0, 0, 0),
                WidthRequest = 120,
                Padding = 1,
                BackgroundColor = Color.White,
               FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
        };

                if (CreateReport._reportStatus == ReportStatus.Edit)
                {
                    var value = CreateReport.report.Data.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                    if (value.Contains("-"))
                        SetFeatureStateOnEdit(_frame, value);
                    else
                    {
                        if (feature.Items[1].Id == value)
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

                buttonOff.Clicked += (sender, args) =>
                {
                    (sender as Button).BackgroundColor = Color.White;
                    (sender as Button).TextColor = Color.Default;
                    (((sender as Button).Parent as StackLayout).Children[0] as Button).BackgroundColor = Color.FromHex("#39b835");
                    (((sender as Button).Parent as StackLayout).Children[0] as Button).TextColor = Color.White;

                //Change text color incase it is red
                ((((((sender as Button).Parent as StackLayout).Parent as Frame).Parent as StackLayout)
                                                 .Children[0] as StackLayout).Children[0] as Label).TextColor = Color.FromHex("#00314b");

                    ToggleControls(sender, false);

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

                switchStackLayout.Children.Add(buttonOff);
                var switchFrame = new Frame { BackgroundColor = Color.FromHex("#39b835"), Padding = 2, HasShadow = false, CornerRadius = 30, IsClippedToBounds = true, HorizontalOptions = LayoutOptions.Start };
                switchFrame.Content = switchStackLayout;
                stackLayout.Children.Add(headerStack);
                stackLayout.Children.Add(switchFrame);

                //Add stacklayout to the page
                accidentStack.Children.Add(stackLayout);
            }
            catch(Exception ex)
            {

            }
        }

        //Some features determine the necessity of other features
        private void ToggleControls(object sender, bool _switch)
        {
            try
            {
                if ((sender as Button).ClassId == "6")
                {
                    Feature _feature = CreateReport._controls.Libary.Features.Find(x => x.Id == "6");
                    foreach (ExcludeFeature _excludedFeature in _feature.Items[1].ExcludeFeatureIds)
                    {
                        int _featurePosition = CreateReport._controls.Libary.Features.IndexOf(CreateReport._controls.Libary.Features.Find(x => x.Id == _excludedFeature.FeatureId));
                        (accidentStack.Children[_featurePosition] as StackLayout).IsVisible = _switch;
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        AccidentModes accidentModes;
        //Go to page with accident modes
        private async void OnSelectModes(object sender, EventArgs e)
        {
            try
            {
                accidentModes = new AccidentModes(ReportStatus.Create);
                await Navigation.PushAsync(accidentModes);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Exception {0}", ex.Message);
            }
            
        }

        //DateTime Control
        public void DateTimeControl(Feature feature)
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
                    FontSize = 14,
                    Margin = new Thickness(0, 0, 0, 3),
                    FontAttributes = FontAttributes.Bold
                };

                //Create a stacklayout to hold control label and feature state button if any
                var headerStack = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Fill };
                headerStack.Children.Add(label);

                //Load Feature States if any
                var _frame = AddFeatureStateOption(feature);
                if (_frame != null)
                    headerStack.Children.Add(_frame);

                //Control Entry
                var datePicker = new DatePicker
                {
                    BackgroundColor = Color.White,
                    FontSize = 17,
                    Margin = new Thickness(2.5, 0, 2.5, 0),
                    HeightRequest = 38,
                    ClassId = feature.Id
                };

                if (CreateReport._reportStatus == ReportStatus.Edit)
                {
                    var value = CreateReport.report.Data.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                    if (value.Contains("-"))
                        SetFeatureStateOnEdit(_frame, value);
                    else
                        datePicker.Date = Convert.ToDateTime(value);
                }

                datePicker.DateSelected += (object sender, DateChangedEventArgs e) =>
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
                frame.Content = datePicker; //Enclose the entry control within the frame

                stackLayout.Children.Add(headerStack);
                stackLayout.Children.Add(frame);

                //Add stacklayout to the page
                accidentStack.Children.Add(stackLayout);
            }
            catch(Exception ex)
            {

            }
        }

        private void DropDownControl(Feature feature)
        {
            try
            {
                //var reportValue = CreateReport.report.Data.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();

            //feature stack
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

            //dropdown custom renderer
            viaDropdown = new ViaDropdown
            {
                ClassId = feature.Id,
                ItemsSource = feature.Items.Select(o => o.Title).ToList(),
                Title = "Choose " + feature.Title,
                HeightRequest = 38,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                FontFamily = Device.RuntimePlatform == Device.iOS ? "TitilliumWeb-SemiBold" : "TitilliumWeb-SemiBold.ttf"
            //SelectedItem = (!string.IsNullOrEmpty(reportValue))? reportValue : null
        };

                if (CreateReport._reportStatus == ReportStatus.Edit)
                {
                    var value = CreateReport.report.Data.Features.Find(x => x.Id == feature.Id).Values.FirstOrDefault();
                    if (value.Contains("-"))
                        SetFeatureStateOnEdit(_frame, value);
                    else
                        viaDropdown.SelectedItem = value;
                }

                viaDropdown.SelectedIndexChanged += (sender, args) =>
                {
                //remove validation colors if any
                if ((((((sender as ViaDropdown).Parent as StackLayout).Parent as Frame).Parent as StackLayout).Children[0] as Label).TextColor.Equals(Color.Red))
                        (((((sender as ViaDropdown).Parent as StackLayout).Parent as Frame).Parent as StackLayout).Children[0] as Label).TextColor = Color.FromHex("#00314b");

                //unselect feature state options if any
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

                //dropsown container
                dropStack = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(10, 0, 10, 0),
                    VerticalOptions = LayoutOptions.CenterAndExpand
                };

                //dropsownimage chevrolet-down
                dropImage = new Xamarin.Forms.Image
                {
                    Source = "ic_dropdown.png",
                    HeightRequest = 16,
                    WidthRequest = 16,
                    HorizontalOptions = LayoutOptions.EndAndExpand
                };
                dropStack.Children.Add(viaDropdown);
                dropStack.Children.Add(dropImage);

                //add dropdown stack in a frame to add shape to it
                frame = new Frame
                {
                    HasShadow = false,
                    CornerRadius = 8,
                    Padding = 2,
                    IsClippedToBounds = true,
                    BorderColor = Color.LightSlateGray
                };
                frame.Content = dropStack;
                stackLayout.Children.Add(frame);

                //add cotnrol to page
                accidentStack.Children.Add(stackLayout);
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Validate Accident Controls
        /// </summary>
        public bool IsAccidentValid()
        {
            try
            {
                bool isValid = true;

                //Validate Parties
                //if(CreateReport.selectedParties.Count == 0)
                //{
                //    modeOfAccident.BorderColor = Color.Red;
                //    isValid = false;
                //}

                bool _areControlsValid = ControlsValidation();
                isValid = (!isValid) ? false : _areControlsValid;

                if (isValid)
                {
                    CreateReport.report.Data.Id = CreateReport._controls.Data.Id;
                    CreateReport.report.Data.GenerationDateTime = CreateReport._controls.Data.GenerationDateTime;
                    CreateReport.report.Data.MutationDateTime = CreateReport._controls.Data.MutationDateTime;
                    CreateReport.report.Data.ChangedLocation = CreateReport._controls.Data.ChangedLocation;
                }

                return isValid;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Evey control is validated following it's correspondednt feature
        /// Once all controls have been validated
        /// </summary>
        /// <returns>bool</returns>
        private bool ControlsValidation()
        {
            try
            {
                //create report features to assign selected values
                CreateReport.report.Data.Features = new List<ReportFeature>();

                bool isAccidentValid = true, isControlValid = true; int featurePos = 0;
                foreach (Feature feature in accidentFeatures)
                {
                    switch (feature.FeatureInputType)
                    {
                        case "Text":
                            isControlValid = ValidateEntryControl(feature, featurePos);
                            isAccidentValid = (!isAccidentValid) ? false : isControlValid;
                            break;
                        case "Number":
                            isControlValid = ValidateNumericControl(feature, featurePos);
                            isAccidentValid = (!isAccidentValid) ? false : isControlValid;
                            break;
                        case "SingleChoice":
                            if (feature.Items.Count == 2)
                            {
                                isControlValid = ValidateSwitchControl(feature, featurePos);
                                isAccidentValid = (!isAccidentValid) ? false : isControlValid;
                            }
                            else if (feature.Items.Count > 15)
                            {
                                isControlValid = ValidateDropdownControl(feature, featurePos);
                                isAccidentValid = (!isAccidentValid) ? false : isControlValid;
                            }
                            else
                            {
                                isControlValid = ValidateSingleSelectControl(feature, featurePos);
                                isAccidentValid = (!isAccidentValid) ? false : isControlValid;
                            }
                            break;
                        case "MultiChoice":
                            isControlValid = ValidateMultiSelectControl(feature, featurePos);
                            isAccidentValid = (!isAccidentValid) ? false : isControlValid;
                            break;
                        case "Date":
                            isControlValid = ValidateDateControl(feature, featurePos);
                            isAccidentValid = (!isAccidentValid) ? false : isControlValid;
                            break;
                    }
                    featurePos++;
                }
                return isAccidentValid;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Validation for MultiChoice (Checkboxes)
        /// </summary>
        private bool ValidateMultiSelectControl(Feature feature, int featurePos)
        {
            try
            {
                var _featureStateValue = "";

                //Check if control has feature states
                if (feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                {
                    //find any clicked buttons
                    var featureStateBtns = ((((accidentStack.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[1] as Frame)
                                      .Content as StackLayout).Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736")).ToList();

                    if (featureStateBtns.Count > 0) //No button has been pressed/selected
                        _featureStateValue = GetSelectedFeatureState((featureStateBtns.FirstOrDefault() as Button), feature);
                }

                var checkedCheckBoxes = ((accidentStack.Children[featurePos] as StackLayout).Children[1] as Grid).Children.Where(x => (x as CheckBox).IsChecked == true).ToList();

                if (feature.FeatureStates.Contains(FeatureStates.Required.ToString()))
                    if (checkedCheckBoxes.Count() == 0 && string.IsNullOrEmpty(_featureStateValue))
                    {
                        (((accidentStack.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor = Color.Red;
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

                CreateReport.report.Data.Features.Add(new ReportFeature { Id = feature.Id, Values = checkedValues });

            }
            catch(Exception ex)
            {

            }
            return true;
        }

        /// <summary>
        /// Validation for a Radio Buttons(SingleChoice)
        /// </summary>
        private bool ValidateSingleSelectControl(Feature feature, int featurePos)
        {
            try
            {
                bool _isValid = true;
                var _value = ""; var _featureStateValue = "";

                //Check if control has feature states
                if (feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                {
                    //find any clicked buttons
                    var featureStateBtns = ((((accidentStack.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[1] as Frame)
                                      .Content as StackLayout).Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736")).ToList();

                    if (featureStateBtns.Count > 0) //No button has been pressed/selected
                        _featureStateValue = GetSelectedFeatureState((featureStateBtns.FirstOrDefault() as Button), feature);
                }

                if ((accidentStack.Children[featurePos] as StackLayout).IsVisible) //Check for controls visibility
                {
                    var radioGroupView = ((accidentStack.Children[featurePos] as StackLayout).Children[1] as RadioButtonGroupView);

                    if (radioGroupView.SelectedIndex == -1 && string.IsNullOrEmpty(_featureStateValue))
                        _isValid = false;

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
                            (((accidentStack.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor = Color.Red;
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

                    CreateReport.report.Data.Features.Add(
                            new ReportFeature
                            {
                                Id = feature.Id,
                                Values = new List<string>() { _value }
                            });
                }
            }
            catch(Exception ex)
            {

            }
            return true;
        }

        /// <summary>
        /// Validation for a Switch Control (Composed of 2 buttons with the selected button being colored)
        /// </summary>
        private bool ValidateSwitchControl(Feature feature, int featurePos)
        {
            try
            {
                var button = ((((accidentStack.Children[featurePos] as StackLayout).Children[1] as Frame).Content as StackLayout).Children[0] as Button);
                var button2 = ((((accidentStack.Children[featurePos] as StackLayout).Children[1] as Frame).Content as StackLayout).Children[1] as Button);

                var _selectedValue = ""; var _featureStateValue = "";

                //Check if control has featurestates
                if (feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                {
                    //find any clicked buttons
                    var featureStateBtns = ((((accidentStack.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[1] as Frame)
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
                        (((accidentStack.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor = Color.Red;
                        return false;
                    }

                if (string.IsNullOrEmpty(_selectedValue))
                    _selectedValue = _featureStateValue;

                CreateReport.report.Data.Features.Add(new ReportFeature { Id = feature.Id, Values = new List<string> { _selectedValue } });

            }
            catch(Exception ex)
            {

            }
            return true;
        }

        /// <summary>
        /// Validation for a Number Entry Control
        /// </summary>
        private bool ValidateNumericControl(Feature feature, int featurePosition)
        {
            try
            {
                var _featureStateValue = ""; string _itemValue = "";
                //Check if control has featurestates
                if (feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                {
                    //find any clicked buttons
                    var featureStateBtns = ((((accidentStack.Children[featurePosition] as StackLayout).Children[0] as StackLayout).Children[1] as Frame)
                                      .Content as StackLayout).Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736")).ToList();

                    if (featureStateBtns.Count > 0) //No button has been pressed/selected
                        _featureStateValue = GetSelectedFeatureState((featureStateBtns.FirstOrDefault() as Button), feature);
                }

                var _entryValue = (((accidentStack.Children[featurePosition] as StackLayout).Children[1] as Frame).Content as ViaEntry).Text;

                //A feature's validation can be required or not. Only those that are required shall be validated
                if (feature.FeatureStates.Contains(FeatureStates.Required.ToString()))
                    //check if nothing was entered into the entry control
                    if (string.IsNullOrEmpty(_entryValue) && string.IsNullOrEmpty(_featureStateValue))
                    {
                        //change the border color if not valid
                        ((accidentStack.Children[featurePosition] as StackLayout).Children[1] as Frame).BorderColor = Color.Red;
                        return false;
                    }

                _itemValue = (!string.IsNullOrEmpty(_entryValue)) ? _entryValue : _featureStateValue;


                CreateReport.report.Data.Features.Add(new ReportFeature { Id = feature.Id, Values = new List<string> { _itemValue } });
            }
            catch(Exception ex)
            {

            }
            return true;
        }

        /// <summary>
        /// Validation for a Text Entry Control
        /// </summary>
        private bool ValidateEntryControl(Feature feature, int featurePosition)
        {
            try
            {
                var _featureStateValue = ""; string _itemValue = "";
                //Check if control has featurestates
                if (feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                {
                    //find any clicked buttons
                    var featureStateBtns = ((((accidentStack.Children[featurePosition] as StackLayout).Children[0] as StackLayout).Children[1] as Frame)
                                      .Content as StackLayout).Children.Where(x => (x as Button).BackgroundColor == Color.FromHex("#eea736")).ToList();

                    if (featureStateBtns.Count > 0) //No button has been pressed/selected
                        _featureStateValue = GetSelectedFeatureState((featureStateBtns.FirstOrDefault() as Button), feature);
                }

                var _entryValue = (((accidentStack.Children[featurePosition] as StackLayout).Children[1] as Frame).Content as ViaEntry).Text;

                //A feature's validation can be required or not. Only those that are required shall be validated
                if (feature.FeatureStates.Contains(FeatureStates.Required.ToString()))
                    //check if nothing was entered into the entry control
                    if (string.IsNullOrEmpty(_entryValue) && string.IsNullOrEmpty(_featureStateValue))
                    {
                        //change the border color if not valid
                        ((accidentStack.Children[featurePosition] as StackLayout).Children[1] as Frame).BorderColor = Color.Red;
                        return false;
                    }

                _itemValue = (!string.IsNullOrEmpty(_entryValue)) ? _entryValue : _featureStateValue;


                CreateReport.report.Data.Features.Add(new ReportFeature { Id = feature.Id, Values = new List<string> { _itemValue } });
            }
            catch(Exception ex)
            {

            }
            return true;
        }

        /// <summary>
        /// Validate Dropdown (Used for SingleChoice with Items.Count > 15)
        /// </summary>
        private bool ValidateDropdownControl(Feature feature, int featurePos)
        {
            try
            {
                var _featureStateValue = ""; string _itemValue = "";

                //Check if control has featurestates
                if (feature.FeatureStates.Contains(FeatureStates.AllowInapplicable.ToString()) || feature.FeatureStates.Contains(FeatureStates.AllowUnknown.ToString()))
                {
                    //find any clicked buttons
                    var featureStateBtns = ((((accidentStack.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[1] as Frame)
                                      .Content as StackLayout).Children.Where(x => x.BackgroundColor == Color.FromHex("#eea736")).ToList();

                    if (featureStateBtns.Count > 0) //No button has been pressed/selected
                        _featureStateValue = GetSelectedFeatureState((featureStateBtns.FirstOrDefault() as Button), feature);
                }


                //A feature's validation can be required or not. Only those that are required shall be validated
                if (feature.FeatureStates.Contains(FeatureStates.Required.ToString()))
                    //check for any radio button selected, At this point neither control nor feature state option is selected
                    if (((((accidentStack.Children[featurePos] as StackLayout).Children[1] as Frame).Content as StackLayout).Children[0] as ViaDropdown).SelectedIndex == -1
                       && string.IsNullOrEmpty(_featureStateValue))
                    {
                        //change control label text color if none is selected
                        (((accidentStack.Children[featurePos] as StackLayout).Children[0] as StackLayout).Children[0] as Label).TextColor = Color.Red;
                        return false;
                    }

                //get selected radio button
                int selectedIndex = ((((accidentStack.Children[featurePos] as StackLayout).Children[1] as Frame).Content as StackLayout).Children[0] as ViaDropdown).SelectedIndex;

                if (selectedIndex >= 0)
                    //get itemvalue using selectedindex
                    _itemValue = feature.Items[selectedIndex].Id;
                else
                    //itemvalue takes on feature state value
                    _itemValue = _featureStateValue;

                //add value to report feature
                CreateReport.report.Data.Features.Add(new ReportFeature { Id = feature.Id, Values = new List<string> { _itemValue } });
            }
            catch(Exception ex)
            {

            }
            return true;
        }

        //Validate feature states if any
        private string GetSelectedFeatureState(Button featureState, Feature feature)
        {
            try
            {
                //Check if button is selected
                if (featureState.BackgroundColor.Equals(Color.FromHex("#eea736")))
                    return featureState.ClassId; //selected value
            }
            catch(Exception ex)
            {

            }
            return null;
        }

        /// <summary>
        /// Validate Date Control
        /// </summary>
        private bool ValidateDateControl(Feature feature, int featurePos)
        {
            try
            {
                var dateSelected = (((accidentStack.Children[featurePos] as StackLayout).Children[1] as Frame).Content as ViaDatePicker).Date;

                CreateReport.report.Data.Features.Add(new ReportFeature { Id = feature.Id, Values = new List<string> { dateSelected.ToString("yyyyMMdd") } });

            }
            catch (Exception ex)
            {
            }
            return true;
        }

        public void OnResumePage(bool o)
        {
            Navigation.RemovePage(accidentModes);
            modeOfAccident.BorderColor = Color.LightSlateGray;
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