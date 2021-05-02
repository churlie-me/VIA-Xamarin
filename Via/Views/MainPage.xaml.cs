using Via.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Via.Data;
using Settings = Via.Utils.Settings;

namespace Via.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;

            MenuPages.Add((int)MenuItemType.Reports, (NavigationPage)Detail);
            MenuPages.Add((int)MenuItemType.Settings, null);
            MenuPages.Add((int)MenuItemType.Logout, null);
        }

        public async Task NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.Reports:
                        MenuPages.Add(id, new NavigationPage(new AccidentReports()));
                        break;
                    case (int)MenuItemType.Settings:
                        MenuPages.Add(id, new NavigationPage(new UserProfile()));
                        //await Navigation.PushAsync(new UserProfile());
                        break;
                    case (int)MenuItemType.Logout:
                        ViaSessions.Logout();
                        break;
                }
            }

            var newPage = MenuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }

            if (id == (MenuPages.Count - 1))
            {
                ViaSessions.Logout();
                Application.Current.MainPage = new SignIn();
            }
        }
    }
}