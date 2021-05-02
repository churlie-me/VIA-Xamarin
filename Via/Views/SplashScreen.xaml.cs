using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Via.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SplashScreen : ContentPage
    
    {
        public SplashScreen()
        {
            InitializeComponent();
            SplashNavigationAsync();

        }

        private async void SplashNavigationAsync()
        {
            try
            {
                //Delay by 5 seconds
                await Task.Delay(1000);
                App.Current.MainPage = new SignIn();
            }
            catch(Exception ex)
            {
               
            }
        }
    }
}