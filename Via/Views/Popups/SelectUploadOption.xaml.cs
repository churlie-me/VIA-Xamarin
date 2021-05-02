using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Via.Data;
using Via.Helpers;
using Via.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Via.Views.Popups
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SelectUploadOption : PopupPage
    {
        private LocationPageState _pageState;
		public SelectUploadOption (LocationPageState pageState)
		{
            this._pageState = pageState;
			InitializeComponent ();
		}

        private async void OpenCamera(object sender, EventArgs args)
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsTakePhotoSupported && CrossMedia.Current.IsPickPhotoSupported)
                return;
            else
            {
                MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    SaveToAlbum = true,
                    Directory = "Images",
                    Name = "Via" + DateTime.Now.ToString("yyyymmdd_hhmmss") + ".jpg"
                });

                if (file == null)
                    return;

                Location._accidentPhotos.Add(file);
                _pageState.OnResumePage(file);
            }
        }

        private async void OpenGallery(object sender, EventArgs args)
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsPickPhotoSupported)
                return;

            var file = await CrossMedia.Current.PickPhotoAsync();
            if (file == null)
                return;

            Location._accidentPhotos.Add(file);
            _pageState.OnResumePage(file);
        }
    }
}