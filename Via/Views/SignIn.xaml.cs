using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Via.Data;
using Via.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Settings = Via.Utils.Settings;

namespace Via.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignIn : ContentPage
    {
        private ViaUser user = new ViaUser();
        public SignIn()
        {
            user = ViaSessions.GetUser();
            if (user != null)
            {
                //Navigate to the main page
                Device.BeginInvokeOnMainThread(() => {
                    StartApplication();
                });
            }
            InitializeComponent();

            Mail.Completed += (object sender, EventArgs e) =>
            {
                Password.Focus();
            };

            Password.Completed += async (object sender, EventArgs e) =>
            {
                AuthenticateUser();
            };
        }

        private void SignInUser(object sender, EventArgs args)
        {
            AuthenticateUser();
        }

        private void AuthenticateUser()
        {
            if (string.IsNullOrEmpty(Mail.Text) && !string.IsNullOrEmpty(Password.Text))
            {
                DisplaySignInError("Email cannot be empty!");
            }
            else if (!string.IsNullOrEmpty(Mail.Text) && string.IsNullOrEmpty(Password.Text))
            {
                DisplaySignInError("Password cannot be empty!");
            }
            else if(!string.IsNullOrEmpty(Mail.Text) && !string.IsNullOrEmpty(Password.Text))
            {
                GetUser();
            }
            else
            {
                DisplaySignInError("Missing email or password");
               /// DisplayAlert("Attention!!!", "Missing email or password", "OK");
            }
        }

        private void CredentialsChanged(object sender, EventArgs args)
        {
            if (((sender as ViaEntry).Parent as Frame).BorderColor == Color.Red)
            {
                ((sender as ViaEntry).Parent as Frame).BorderColor = Color.White;
                loginError.IsVisible = false;
            }
        }

        // This stores credentials for other endpoints to use.
        private void StoreCredentials()
        {
            if (!Application.Current.Properties.ContainsKey("username") || 
                !Application.Current.Properties.ContainsKey("password"))
            {
                Application.Current.Properties.Add("username", Mail.Text);
                Application.Current.Properties.Add("password", Password.Text);
                Application.Current.SavePropertiesAsync();
            }
            
        }

        /// <summary>
        /// Request for User using a user's credentials
        /// </summary>
        public void GetUser()
        {
            //Hide progress indicator and show the signin stack
            splashScreenBg.IsVisible = false;
            signInStack.IsVisible = false;
            signInProgress.IsVisible = true;
            signInIndicator.IsRunning = true;

            Task.Factory.StartNew(async () => {
                // Do some work on a background thread, allowing the UI to remain responsive
                var response = ViaAsyncTasks.SignInAsync(Mail.Text, Password.Text).Result;
                var responseContent = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<ViaUser>(responseContent);
            }).ContinueWith(task => {
                Device.BeginInvokeOnMainThread(() =>
                {
                    if (user == null)
                    {
                        //Return signIn page to for user to re-enter their credentials
                        DisplaySignInError("Incorrect login details. Try again");
                        (Mail.Parent as Frame).BorderColor = Color.Red;
                        (Password.Parent as Frame).BorderColor = Color.Red;
                    }
                    else
                    {
                        if(user.profile == null)
                        {
                            DisplaySignInError("Incorrect login details. Try again");
                            (Mail.Parent as Frame).BorderColor = Color.Red;
                            (Mail.Parent as Frame).Padding = 4;
                            (Password.Parent as Frame).BorderColor = Color.Red;
                            (Password.Parent as Frame).Padding = 4;
                            return;
                        }

                        if (!string.IsNullOrEmpty(user.status) && user.status == "Expired")
                        {
                            DisplaySignInError("Your account has expired");
                            return;
                        }

                        ViaSessions.SaveUser(user);
                        ViaSessions.DownloadSettings();
                        StartApplication();
                        StoreCredentials();
                    }
                });
            },TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void DisplaySignInError(string message)
        {
            //Hide progress indicator and show the signin stack
            signInIndicator.IsRunning = false;
            signInProgress.IsVisible = false;
            splashScreenBg.IsVisible = true;
            signInStack.IsVisible = true;

            loginError.IsVisible = true;
            ((loginError.Content as StackLayout).Children[1] as Label).Text = message;
        }

        public void StartApplication()
        {
            //Navigate to the main page
            Application.Current.MainPage = new NavigationPage(new MainPage())
                                               {
                                                BarBackgroundColor = Color.White,
                                                BarTextColor = Color.FromHex("#00314b"),
                                                Tint = Color.FromHex("#00314b")
                                               };
        }

        private void ForgotPassword(object sender, EventArgs args)
        {
            Application.Current.MainPage = new ForgotPassword();
        }
    }
}