using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Via.Data;
using Via.Models;
 
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Settings = Via.Utils.Settings;

namespace Via.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        private readonly List<MainMenuItem> items;
        private ViaUser user = new ViaUser();

        public MenuPage()
        {
            try
            {
                InitializeComponent();

                items = new List<MainMenuItem>
            {
                new MainMenuItem {Id = MenuItemType.Reports, Title = "Reports" },
                new MainMenuItem {Id = MenuItemType.Settings, Title = "Settings"},
                new MainMenuItem { Id = MenuItemType.Logout, Title = "Logout", Color = "#ff1b45", ShowBorder = true}

            };

                //ListViewMenu.ItemsSource = items;
                //ListViewMenu.ItemTapped += async (sender, e) =>
                //{
                //    var tappedItem = e.Item;
                //    if (tappedItem == null)
                //        return;

                //    var id = (int)((MainMenuItem)e.Item).Id;

                //    if (id == (int)MenuItemType.Settings)
                //    {
                //        //Hide side menu
                        
                //        //(Application.Current.MainPage as MasterDetailPage).IsPresented = false;

                //        App.NavigationPage.BarBackgroundColor = Color.White;
                //        await Navigation.PushAsync(new UserProfile());                        
                //    }
                //    else
                //        await RootPage.NavigateFromMenu(id);
                //};
            }
            catch(Exception ex)
            {

            }
        }

        private async void onTapSettingGestureRecognizerTapped(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new UserProfile());
        }

        private void onTapReportGestureRecognizerTapped(object sender, EventArgs args)
        {
          //  await Navigation.PopAsync();//RootPage.NavigateFromMenu((int)(MenuItemType.Reports));
            if (Application.Current.MainPage is MasterDetailPage masterDetailPage)
            {
                masterDetailPage.IsPresented = false;
            }
            else if (Application.Current.MainPage is NavigationPage navigationPage && navigationPage.CurrentPage is MasterDetailPage nestedMasterDetail)
            {
                nestedMasterDetail.IsPresented = false;
            }
        }

        private void onTapLogoutGestureRecognizerTapped(object sender, EventArgs args)
        {
            //await RootPage.NavigateFromMenu((int)(MenuItemType.Logout));
            ViaSessions.Logout();
            Application.Current.MainPage = new SignIn();
        }

        private MainPage RootPage => Application.Current.MainPage as MainPage;
        
        public void DisplayUser()
        {
            try
            {
                if (user != null)
                {
                    var avatarUri = new UriImageSource { Uri = new Uri(user.profile.avatar), CachingEnabled = false };
                    userAvatar.Source = avatarUri;

                    userName.Text = user.profile.firstname + " " + user.profile.lastname;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        protected override void OnAppearing()
        {
            try
            {
                user = ViaSessions.GetUser();
                DisplayUser();
            }
            catch(Exception ex)
            {

            }
        }
    }
}