using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Via.Data;
using Via.Models;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Via.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditAccountSettings : ContentPage
    {
        private string language;
        private string timeZone;
        private string speed;
        private string mailInterval;
        private ViaUser user;
        public EditAccountSettings()
        {
            InitializeComponent();

            try
            {
                user = ViaSessions.GetUser();

                ActivityIndicator.IsRunning = false;
                ActivityIndicator.IsVisible = false;
                status_txt.IsVisible = false;

                userNames.Text = user.profile.firstname + " " + user.profile.lastname;
                SetProfilePic();

                AccountSettingsInit();
                GenerateViews();
            }
            catch(Exception ex)
            {

            }
        }

        private void AccountSettingsInit()
        {
            this.language = user.settings.culture;
            this.timeZone = user.settings.timeZone;
            this.speed = user.settings.speedUnit;
            this.mailInterval = user.settings.mailInterval;
        }

        private void SetProfilePic()
        {
            try
            {
                profile_img.Source = ViaSessions.GetUser().profile.avatar;
            }
            catch (Exception m)
            {
                Debug.WriteLine(m.ToString());
            }
        }

        private void GenerateViews()
        {
            try
            {

                // Populating the speed units and mailIntervals
                user.InitialSettings = ViaSessions.InitialSettings();
                speedunits_dd.ItemsSource = user.InitialSettings.speedUnits;
                mailintervals_dd.ItemsSource = user.InitialSettings.mailIntervals;

                // Populating the language values.
                Cultures bsObj = new Cultures()
                {
                    en_GB = "en-GB",
                    fr_FR = "fr-FR",
                    nl_NL = "nl-NL"
                };

                var newList = new List<string>();

                // Storing the values in the list.
                newList.Add(bsObj.en_GB);
                newList.Add(bsObj.fr_FR);
                newList.Add(bsObj.nl_NL);
                language_dd.ItemsSource = newList;
                Via.Utils.Settings settings = new Via.Utils.Settings();

                timezone_dd.ItemsSource = Utils.Settings.Timezones();

                // Initialize views with data.
                language_dd.SelectedItem = language;
                timezone_dd.SelectedItem = timeZone;
                speedunits_dd.SelectedItem = speed;
                mailintervals_dd.SelectedItem = mailInterval;
            }
            catch(Exception ex)
            {

            }
        }

        private void SetAccountSettings(object sender, EventArgs e)
        {
            try
            {
                SettingData();

            }
            catch(Exception ex)
            {

            }
        }

        private async void SettingData()
        {
            try
            {
                ActivityIndicator.IsRunning = true;
                ActivityIndicator.IsVisible = true;

                var speedunit = speedunits_dd.Items[speedunits_dd.SelectedIndex].Trim();
                var mailinterval = mailintervals_dd.Items[mailintervals_dd.SelectedIndex].Trim();
                var language = language_dd.Items[language_dd.SelectedIndex].Trim();
                var timezone = timezone_dd.Items[timezone_dd.SelectedIndex].Trim();

                var settings = new Settings
                {
                    speedUnit = speedunit,
                    mailInterval = mailinterval,
                    mainAreaID = user.settings.mainAreaID,
                    culture = language,
                    mailDayOfWeek = "Sunday",
                    timeZone = timezone
                };
                var data = JsonConvert.SerializeObject(settings);
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var client = new HttpClient();

                UserProfile.CheckUserValidity();
                user = ViaSessions.GetUser();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token.token);

                var response = await client.PutAsync(Utils.Settings.BaseUrl + "/auth/Users/ChangeSettings", content);
            
            
                if (!speedunit.Equals(
                        "") || !mailinterval.Equals(""))
                {
                    if (response.StatusCode.ToString().Trim() != "OK")
                    {
                   
                        await DisplayAlert("","There's a problem while setting,check your network connection","OK");
                        ActivityIndicator.IsRunning = false;
                        ActivityIndicator.IsVisible = false;
                    }

                    else
                    {
                        await DisplayAlert("","You have successfully edited the account settings.","OK");

                        await ViaSessions.RegenerateNewToken();

                        //userprofile.SpeedTxt.Text = speedunits_dd.Title;
                        ActivityIndicator.IsRunning = false;
                        ActivityIndicator.IsVisible = false;

                        // Go back to the previous page.    
                        await Navigation.PopAsync();
                    }
                }

                else
                {

                    await DisplayAlert("","You missed selecting a choice.","OK");
             
                    ActivityIndicator.IsRunning = false;
                    ActivityIndicator.IsVisible = false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}