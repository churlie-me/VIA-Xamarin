using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Via.Data;
using Via.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Via.Helpers
{
    class ImageConverter : IValueConverter
    {
        private static readonly HttpClient _client = new HttpClient();
        private static ViaUser user = ViaSessions.GetUser();


        public  object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ImageSource retSource = null;
            try
            {
                
                if (value != null)
                {
                    var imageSource = (string)value;

                    Debug.WriteLine($"ImageSource {imageSource}");
                    _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token.token);


                    byte[] imageBytes = _client.GetByteArrayAsync(imageSource).Result;

                    if(imageBytes.Length > 0)
                    {
                        retSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));                        
                    }
                    else
                    {
                        throw new Exception("Error: Unable to download file");
                    }

                }
               
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception {ex.Message}");

            }

            return retSource;

        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
