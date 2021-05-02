using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Via.Data;
using Via.Models;
using Via.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Via.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangePassword : ContentPage
    {
        private ViaUser user;
        public ChangePassword()
        {
            InitializeComponent();

            ActivityIndicator.IsVisible = false;
            ActivityIndicator.IsRunning = false;

            user = ViaSessions.GetUser();
            SetProfilePicAndUsername();
        }

        private void SetProfilePicAndUsername()
        {
            try
            {
                userNames.Text = user.profile.firstname + " " + user.profile.lastname;
                profile_img.Source = user.profile.avatar;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async void ChangingPassword()
        {
            try
            {
                ActivityIndicator.IsVisible = true;
                ActivityIndicator.IsRunning = true;

                var changePassword = new Models.ChangePassword
                {
                    newPassword = new_password_entry.Text,
                    oldPassword = old_password_entry.Text,
                    newPasswordCheck = confirm_password_entry.Text
                };

                UserProfile.CheckUserValidity();
                user = ViaSessions.GetUser();

                var data = JsonConvert.SerializeObject(changePassword);
                var content = new StringContent(data, Encoding.UTF8, "application/json");
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token.token);
                var response = await client.PostAsync(Utils.Settings.BaseUrl + "/auth/Users/ChangePassword", content);


                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ActivityIndicator.IsVisible = false;
                    ActivityIndicator.IsRunning = false;
                    await DisplayAlert("", "You have successfully changed your password.", "OK");

                    ViaSessions.SavePassword(new_password_entry.Text);
                    Navigation.RemovePage(this);
                }

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    ActivityIndicator.IsVisible = false;
                    ActivityIndicator.IsRunning = false;
                    await DisplayAlert("Attention!!!",
                        "The password did not pass the rule check. It needs to be minimal 10 characters long, " +
                        "and contain a uppercase, lowercase and a digit.", "OK");
                }
            }
            catch (Exception n)
            {

            }
        }

        private async void ChangingPasswordBtn(object sender, EventArgs e)
        {
            if (new_password_entry.Text != confirm_password_entry.Text)
            {
                ActivityIndicator.IsVisible = false;
                ActivityIndicator.IsRunning = false;
                await DisplayAlert("", "Password Mismatch!", "OK");
            }
            else
            {
                ChangingPassword();
            }
        }
    }
}