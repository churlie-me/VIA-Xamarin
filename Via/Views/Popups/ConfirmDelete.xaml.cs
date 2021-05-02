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
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace Via.Views.Popups
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ConfirmDelete : PopupPage
    {
        private ReportState state;
        public ConfirmDelete(ReportState state)
		{
			InitializeComponent();
            this.state = state;
            Init();
        }

        private void Init()
        {
            //this.BackgroundColor = Color.Accent;
            //if (Device.RuntimePlatform == Device.iOS)
            //{
            //    this.On<iOS>().UseBlurEffect(BlurEffectStyle.ExtraLight);
            //}
            //else
            //{
            this.BackgroundInputTransparent = true;
            //}
        }

        protected override bool OnBackgroundClicked()
        {
            return false;
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }

        private void OnConfirm(object sender, EventArgs args)
        {
            try
            {
                state.OnReportDeleted();
            }
            catch (Exception ex)
            {
                //DisplayAlert("Error", ex.Message);
                Debug.WriteLine($"Exception after Selection Complete:  {ex}");
            }
        }

        private void OnCancel(object sender, EventArgs args)
        {
            try
            {
                Task.Run(async () => await PopupNavigation.Instance.RemovePageAsync(this));
            }
            catch (Exception ex)
            {
                //DisplayAlert("Error", ex.Message);
                Debug.WriteLine($"Exception after Selection Complete:  {ex}");
            }
        }
    }
}