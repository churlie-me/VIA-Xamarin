using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Via.Data;
using Via.Models;
using Via.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Via
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ForgotPassword : ContentPage
	{
        private ResponseMessage forgotPasswordResponse;
		public ForgotPassword ()
		{
			InitializeComponent ();
		}

        private void SignIn(object sender, EventArgs args)
        {
            App.Current.MainPage = new SignIn();
        }

        private void CredentialsChanged(object sender, EventArgs args)
        {
            if (((sender as ViaEntry).Parent as Frame).BorderColor == Color.Red)
            {
                ((sender as ViaEntry).Parent as Frame).BorderColor = Color.White;
                loginError.IsVisible = false;
            }
        }

        private void SubmitForgotPassword(object sender, EventArgs args)
        {
            if (!string.IsNullOrEmpty(Mail.Text))
            {
                if(IsEmailValid())
                    SendMail();
                else
                    DisplayAlert("Attention!!!", "Wrong email format", "OK");
            }
            else
            {
                DisplayAlert("Attention!!!", "Enter your email", "OK");
            }
        }

        private bool IsEmailValid()
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(Mail.Text);
                return address.Address == Mail.Text;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public void SendMail()
        {
            //Show progress indicator
            processInIndicator.IsVisible = true;
            processInIndicator.IsRunning = true;
            forgotPassword.Text = "Please wait...";
            
            Task.Factory.StartNew(async () =>
            {
                // Do some work on a background thread, allowing the UI to remain responsive
                var response = ViaAsyncTasks.SendMail(Mail.Text).Result;
                var responseContent = await response.Content.ReadAsStringAsync();
                forgotPasswordResponse = JsonConvert.DeserializeObject<ResponseMessage>(responseContent);
            }).ContinueWith(task =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    try
                    {
                        //Hide progress indicator
                        processInIndicator.IsRunning = false;
                        processInIndicator.IsVisible = false;
                        forgotPassword.Text = "SEND";

                        if(forgotPasswordResponse != null)
                            if (forgotPasswordResponse.message != null)
                                DisplayAlert(null, forgotPasswordResponse.message, "OK");
                            else
                                DisplayAlert(null, "Please check your internet connection and try again", "OK");
                        else
                            DisplayAlert(null, "Wrong email address", "OK");
                    }
                    catch (Exception ex)
                    {

                    }
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}