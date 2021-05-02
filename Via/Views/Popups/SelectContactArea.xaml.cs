using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Via.Helpers;
using Via.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Via.Views.Popups
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SelectContactArea : PopupPage
    {
        ReportPageState pageState;
        private List<ContactAreaItem> contactAreas = new List<ContactAreaItem>();
		public SelectContactArea(ReportPageState _pageState)
		{
            this.pageState = _pageState;
			InitializeComponent();
            LoadCountries();

        }

        private void LoadCountries()
        {
            foreach (var country in CreateReport.contactArea.LND)
                contactAreas.Add(new ContactAreaItem { Country = country});

            selectContactAreaListView.ItemsSource = contactAreas;
        }

        private ContactAreaItem _selectedContactAreaItem;
        private void OnContactAreaItemTapped(object sender, ItemTappedEventArgs args)
        {
            var contactAreaListView = sender as ListView;
            _selectedContactAreaItem = (args.Item as ContactAreaItem);
            _selectedContactAreaItem.BackgroundColor = Color.FromHex("#dfe5e8");
            _selectedContactAreaItem.BorderColor = Color.FromHex("#b1c0c8");
            _selectedContactAreaItem.IsSelected = true;
            _selectedContactAreaItem.OnPropertyChanged();
            
            CreateReport.LND = (args.Item as ContactAreaItem).Country;
            foreach (ContactAreaItem item in contactAreaListView.ItemsSource)
            {
                if (item.IsSelected && !item.Equals(_selectedContactAreaItem))
                {
                    item.IsSelected = false;
                    item.BackgroundColor = Color.Transparent;
                    item.BorderColor = Color.FromHex("#d5dde1");
                    item.OnPropertyChanged();
                }
            }

            if (selectCountry.TextColor == Color.Red)
                selectCountry.TextColor = Color.Default;
        }

        protected override bool OnBackgroundClicked()
        {
            return false;
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }

        private void SelectionComplete(object sender, EventArgs args)
        {
            try
            {
                if (_selectedContactAreaItem == null)
                    selectCountry.TextColor = Color.Red;
                else
                    pageState.OnResumePage(_selectedContactAreaItem.Country);
            }
            catch (Exception ex)
            {
                //DisplayAlert("Error", ex.Message);
                Debug.WriteLine($"Exception after Selection Complete:  {ex}");
            }
        }
    }
}