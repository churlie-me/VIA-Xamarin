using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Via.Data;
using Via.Models;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Via.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserProfile : ContentPage
    {
        private ViaUser _user;
        public UserProfile()
        {
            InitializeComponent();
        }

        public async static void CheckUserValidity()
        {
            try
            {
                if (DateTime.Now > Convert.ToDateTime(ViaSessions.GetUser().token.validTo))
                {
                    await ViaSessions.RegenerateNewToken();
                }
            }
            catch (Exception ex)
            {
                Log.Warning("App Crash => ", ex.Message);
            }
        }

        private void ShowProfileInfo(object sender, EventArgs e)
        {
            try
            {
                Navigation.PushAsync(new EditPersonalInformation());
            }
            catch (Exception ex)
            {

            }
        }

        private void AccountSettings(object sender, EventArgs e)
        {
            try
            {
                Navigation.PushAsync(new EditAccountSettings());
            }
            catch (Exception ex)
            {

            }
        }


        protected override void OnAppearing()
        {
            try
            {
                _user = null;
                _user = ViaSessions.GetUser();
                if (_user.profile != null)
                {
                    LoadUserProfile();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void LoadUserProfile()
        {
            userNames.Text = _user.profile.firstname + " " + _user.profile.lastname;
            SetProfilePic();
        }

        private void SetProfilePic()
        {
            try
            {
                var avatarUri = new UriImageSource { Uri = new Uri(_user.profile.avatar), CachingEnabled = false };

                //if (profile_img.Source != null)
                //{
                //    if (profile_img.Source.ToString().Trim() != avatarUri.ToString().Trim())
                //    {
                //        profile_img.Source = null;
                //        profile_img.Source = avatarUri;
                //    }
                //}
                //else
                //{
                    profile_img.Source = avatarUri;
                //}
            }
            catch (Exception m)
            {
                Debug.WriteLine("Set Profile Pic Issue: {0}",m.ToString());
            }
        }

        private async void ChangePassword(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new ChangePassword());
            }
            catch (Exception ex)
            {

            }
        }

        private async void ChangeAvatar(object sender, EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new EditProfilePicture());
            }
            catch (Exception ex)
            {

            }
        }
    }
}