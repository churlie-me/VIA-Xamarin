using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Via.Data;
using Via.Models;
using Xamarin.Forms;

namespace Via.ViewModels
{
    class ImageSourceViewModel
    {
        private static readonly HttpClient _client = new HttpClient();
        private static ViaUser user = ViaSessions.GetUser();

        public ImageSource DownloadedSource { get; set; }

        //public void DownloadImageData()
        //{
        //    WebClient webClient = new WebClient();
        //    webClient.Credentials = new NetworkCredential(username, passwort, domain);
        //    ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //    byte[] imageBytes = webClient.DownloadData(url);
        //    byte[] imageBytes = _client.GetByteArrayAsync()
        //    DownloadedSource = ImageSource.FromStream(() => new MemoryStream(imageBytes));

        //    NotifyPropertyChanged(nameof(DownloadedSource));
        //}
    }
}
