using System;
using System.Diagnostics;
using System.Net;
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
    public partial class EditPersonalInformation : ContentPage
    {
        private ViaUser user;
        public EditPersonalInformation()
        {
            InitializeComponent();
            try
            {
                user = ViaSessions.GetUser();

                SetProfilePic();

                SetUserInformation();

            }
            catch(Exception ex)
            {

            }
        }

        private void SetUserInformation()
        {
            //Set data to texts.
            userNames.Text = user.profile.firstname + " " + user.profile.lastname;

            firstname_entry.Text = user.profile.firstname;
            middlename_entry.Text = user.profile.middlename;
            lastname_entry.Text = user.profile.lastname;


            //Select Gender
            if (user.profile.gender == "Male")
                male_rbtn.IsChecked = true;
            else if (user.profile.gender == "Female")
                female_rbtn.IsChecked = true;
            else
                neutral_rbtn.IsChecked = true;
        }


        /***
         * This method is responsible for changing the personal information.
         */
        private async void ChangePersonalInformation()
        {
            try
            {
                ActivityIndicator.IsRunning = true;
                ActivityIndicator.IsVisible = true;

                var firstname = firstname_entry.Text;
                var middlename = middlename_entry.Text;
                var lastname = lastname_entry.Text;
                var gender = SelectedVale();

                var profile = new Profile
                {
                    fullname = firstname + " " + middlename + " " + lastname,
                    firstname = firstname,
                    middlename = middlename,
                    lastname = lastname,
                    function = "trial",
                    gender = gender
                };

                var data = JsonConvert.SerializeObject(profile);
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var client = new HttpClient();

                UserProfile.CheckUserValidity();
                user = ViaSessions.GetUser();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token.token);
                var response = await client.PutAsync(Utils.Settings.BaseUrl + "/auth/Users/ChangeProfile", content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    await DisplayAlert("", "You have successfully edited your profile.", "OK");

                    //Reset user information
                    await ViaSessions.RegenerateNewToken();

                    ActivityIndicator.IsRunning = false;
                    ActivityIndicator.IsVisible = false;

                    // Go back to the previous page.    
                    await Navigation.PopAsync();
                }
                else
                {
                    ActivityIndicator.IsRunning = false;
                    ActivityIndicator.IsVisible = false;
                    await DisplayAlert("", "No changes saved", "OK");
                }
            }
            catch(Exception ex)
            {

            }
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

        /**
         * This method handles a click event after changing.
         */
        private async void Button_OnClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(firstname_entry.Text) && !string.IsNullOrEmpty(lastname_entry.Text))
                ChangePersonalInformation();
            else
                await DisplayAlert(null, "Firstname and Lastname cannot be empty", "Ok");
        }

        /**
         * This method handles the Radio Buttons.
         */
        private string SelectedVale()
        {
            if (male_rbtn.IsChecked || male_rbtn.IsPressed)
                return "Male";

            if (female_rbtn.IsPressed || female_rbtn.IsChecked)
                return "Female";

            return "Neutral";
        }
    }
}