using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using static Via.Data.Constants;
using System.Net.Http;
using System.Threading.Tasks;
using Via.Data;
using AvailableParty = Via.Models.Item;
using System.IO;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace Via.Models
{
    public class SignInViewModel : INotifyPropertyChanged
    {
        public Action DisplayFieldsPrompt, DisplayInvalidSignInPrompt;
        private static readonly HttpClient _client = new HttpClient();
        private ViaUser user =  new ViaUser();
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        private string username;
        public string Email
        {
            get { return username; }
            set
            {
                username = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Mail"));
            }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Password"));
            }
        }
        public ICommand SubmitCommand { protected set; get; }
        public SignInViewModel()
        {
            SubmitCommand = new Command(OnSubmit);
        }
        public void OnSubmit()
        {
            if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
            {
                //Login Connection to api
                user = SignInUser(username, password);
            }
            else
            {
                DisplayFieldsPrompt();
            }
        }

        public ViaUser SignInUser(string username, string password)
        {
            ViaUser user = new ViaUser();
            Task.Factory.StartNew(async () => {
                // Do some work on a background thread, allowing the UI to remain responsive
                var response  = ViaAsyncTasks.SignInAsync(username, password).Result;
                var responseContent = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<ViaUser>(responseContent);
                // When the background work is done, continue with this code block
            }).ContinueWith(task => {
                //DoSomethingOnTheUIThread();
                // the following forces the code in the ContinueWith block to be run on the
                // calling thread, often the Main/UI thread.
            }, TaskScheduler.FromCurrentSynchronizationContext());
            return user;
        }

        private static async void SignInUser()
        {
            ViaUser user = new ViaUser();
            try
            {
                var response = await _client.PostAsync(AuthorizationUrl, null);

                var content = await response.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<ViaUser>(content);
            }
            catch (Exception ex)
            {

            }
        }
    }

    public class AvailablePartiesViewModel
    {
        public List<AvailableParty> availableParties;
        public int SelectedItemsCounter { get; set; } = 0;

        public AvailablePartiesViewModel(List<AvailableParty> availableParties)
        {
            this.availableParties = availableParties;
            ItemTappedCommand = new Command((object model) => {

                if (model != null && model is ItemTappedEventArgs)
                {
                    if (!((AvailableParty)((ItemTappedEventArgs)model).Item).IsSelected)
                        SelectedItemsCounter++;
                    else
                        SelectedItemsCounter--;

                    ((AvailableParty)((ItemTappedEventArgs)model).Item).IsSelected = !((AvailableParty)((ItemTappedEventArgs)model).Item).IsSelected;
                }
            });
        }

        public ICommand ItemTappedCommand { get; protected set; }

    }

    public class ReportImageViewModel : INotifyPropertyChanged
    {
        private static readonly HttpClient _client = new HttpClient();
        private static ViaUser user = ViaSessions.GetUser();

        //public string Url { get; set; }

        private ImageSource imageSource;
        public ImageSource Url
        {
            get
            {
                return imageSource;
            }
            set
            {
                if(imageSource != value)
                {
                    imageSource = DownloadImageData(value);
                    NotifyPropertyChanged("Url");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ImageSource DownloadImageData(ImageSource image)
        {
             var url = image.GetValue(UriImageSource.UriProperty).ToString();

            Debug.WriteLine($"Image Url: {url}");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token.token);
            Debug.WriteLine($"URL : {url}");
            byte[] imageBytes = _client.GetByteArrayAsync(url).Result;
            var retSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));

            return retSource;
        }
    }


}
