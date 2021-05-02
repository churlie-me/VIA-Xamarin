using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Via.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImageEnlarge : ContentPage
    {
        private int selectedIndex;
        private List<MediaFile> accidentPhotos;

        public event EventHandler<NavigationEventArgs> Popped;

        public ImageEnlarge(int _selectedIndex)
        {
            InitializeComponent();

            accidentPhotos = Location._accidentPhotos;
            selectedIndex = _selectedIndex;

            accidentPhoto.Source = ImageSource.FromStream(() =>
            {
                var stream = accidentPhotos[selectedIndex].GetStream();
                return stream;
            }); 
        }

        private void DeleteImage(object sender, EventArgs e)
        {

            if (accidentPhotos.ElementAtOrDefault(selectedIndex) != null)
            {
                    accidentPhotos.RemoveAt(selectedIndex);

                // show next Image

                if ((selectedIndex + 1) < accidentPhotos.Count)
                {
                    //show next image
                    accidentPhoto.Source = ImageSource.FromStream(() =>
                    {
                        var stream = accidentPhotos[++selectedIndex].GetStream();
                        return stream;
                    });

                    ++selectedIndex;

                }
                else if ((selectedIndex - 1) > -1)
                {
                    //show previous image
                    accidentPhoto.Source = ImageSource.FromStream(() =>
                    {
                        var stream = accidentPhotos[(selectedIndex - 1)].GetStream();
                        return stream;
                    });

                    --selectedIndex;
                }
                else
                {
                    MessagingCenter.Send(this, "ReloadAccidentPhotoView", accidentPhotos);
                    Navigation.PopAsync();

                }
            }

        }

        private void CloseReport(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "ReloadAccidentPhotoView", accidentPhotos);
            Navigation.PopAsync();
        }

        private void Next(object sender, EventArgs args)
        {
            selectedIndex = ((selectedIndex + 1) < accidentPhotos.Count) ? ++selectedIndex : selectedIndex;
            accidentPhoto.Source = ImageSource.FromStream(() =>
            {
                var stream = accidentPhotos[selectedIndex].GetStream();
                return stream;
            });
        }

        private void Previous(object sender, EventArgs args)
        {
            selectedIndex = ((selectedIndex - 1) == -1) ? selectedIndex : (selectedIndex - 1);
            accidentPhoto.Source = ImageSource.FromStream(() =>
            {
                var stream = accidentPhotos[selectedIndex].GetStream();
                return stream;
            });
        }

        protected override void OnAppearing()
        {
            try
            {


                base.OnAppearing();


                if (Application.Current.MainPage is MasterDetailPage masterDetailPage)
                {
                    masterDetailPage.IsGestureEnabled = false;
                }
                else if (Application.Current.MainPage is NavigationPage navigationPage && navigationPage.CurrentPage is MasterDetailPage nestedMasterDetail)
                {
                    nestedMasterDetail.IsGestureEnabled = false;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("ex" + ex.Message + ", " + ex.InnerException);
            }

        }

        protected override void OnDisappearing()
        {
            try
            {


                base.OnDisappearing();


                if (Xamarin.Forms.Application.Current.MainPage is MasterDetailPage masterDetailPage)
                {
                    masterDetailPage.IsGestureEnabled = true;
                }
                else if (Xamarin.Forms.Application.Current.MainPage is NavigationPage navigationPage && navigationPage.CurrentPage is MasterDetailPage nestedMasterDetail)
                {
                    nestedMasterDetail.IsGestureEnabled = true;
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("ex" + ex.Message + ", " + ex.InnerException);
            }

        }
    }
}
