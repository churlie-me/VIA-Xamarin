using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using FFImageLoading;
using FFImageLoading.Forms;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Stormlion.ImageCropper;
using Via.Data;
using Via.Models;
using Via.Views;
using Via.Utils;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Settings = Via.Utils.Settings;
using FFImageLoading.Cache;

namespace Via.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditProfilePicture : ContentPage
    {
        private new static readonly string X = "x";
        private new static readonly string Y = "y";
        private static readonly int XValue = 0;
        private static readonly int YValue = 0;
        private double WidthValue { get; set; } = 2000;
        private double HeightValue { get; set; } = 3000;

        private new static readonly string Width = "width";
        private new static readonly string Height = "height";
        private ViaUser user;
        private ImageCropper imageCropper;


        private MediaFile _mediaFile;

        public EditProfilePicture()
        {
            InitializeComponent();

            CrossMedia.Current.Initialize();


            user = ViaSessions.GetUser();

            SetProfilePic();

            //Set data to texts.
            userNames.Text = user.profile.firstname + " " + user.profile.lastname;
        }

        private void SetProfilePic()
        {
            try
            {
                // profile_img.Source = ViaSessions.GetUser().profile.avatar;

                profile_img.Source = new UriImageSource
                {
                    Uri = new Uri(ViaSessions.GetUser().profile.avatar),
                    CachingEnabled = true,
                    CacheValidity = new TimeSpan(0, 0, 1, 0)
                };
            }
            catch (Exception m)
            {
                Debug.WriteLine(m.ToString());
            }
        }

        //private void Refresh()
        //{
        //    try
        //    {
        //        if (App.CroppedImage != null)
        //        {
        //            Stream stream = new MemoryStream(App.CroppedImage);
        //            image.Source = ImageSource.FromStream(() => stream);

        //            Content = image;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //    }
        //}

        /// <summary>
        ///     This uploads a profile picture.
        /// </summary>
        private async void UploadImage()
        {
            try
            {
                ai.IsRunning = true;
                ai.IsVisible = true;

                var multiForm = new MultipartFormDataContent();
                Debug.WriteLine($"Profile Image Source: {profile_img.Source.ToString()}");


                var pathFile = profile_img.Source
                    .ToString().Replace("/data/user/0", "/data/data");
                var getPathName = Path.GetFileName(pathFile);
                Debug.WriteLine($"getPathName : {getPathName}");
                //Debug.WriteLine($"imageFile Height: { DependencyService.Get<IImageResource>().GetSize(pathFile).Height} Width: {DependencyService.Get<IImageResource>().GetSize(pathFile).Width}");
                // folder path.
                var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                Debug.WriteLine($"folderPath : {folderPath} pathFile:{pathFile}");

                // Convert it into bytes.


                var readAllBytes = File.ReadAllBytes(Path.Combine(folderPath, getPathName));
                Debug.WriteLine($"readAllBytes : {readAllBytes.Length}");

                Debug.WriteLine($"WidthValue : {WidthValue} HeightValue: {HeightValue}");
                //var baContent = new ByteArrayContent(readAllBytes);
                Debug.WriteLine($"XValue : {XValue} YValue: {YValue}");

                multiForm.Add(new StringContent(XValue.ToString()), X);
                multiForm.Add(new StringContent(YValue.ToString()), Y);
                multiForm.Add(new StringContent(WidthValue.ToString()), Width);
                multiForm.Add(new StringContent(WidthValue.ToString()), Height);
                multiForm.Add(new StreamContent(new MemoryStream(readAllBytes)), "\"image\"", $"\"{Path.GetFileName(pathFile)}\"");

                UserProfile.CheckUserValidity();
                user = ViaSessions.GetUser();

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token.token);
                var response = await client.PostAsync(Settings.BaseUrl + "/auth/Users/ChangeAvatar", multiForm);



                Debug.WriteLine($"response  : {await response.Content.ReadAsStringAsync()}");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Debug.WriteLine("Response {0}", response.Content.ToString());
                    ai.IsRunning = false;
                    ai.IsVisible = false;

                    var avatar = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Profile1: {avatar}");

                    await ViaSessions.RegenerateNewToken();

                    Debug.WriteLine($"Profile2: {ViaSessions.GetUser().profile.avatar}");

                    await DisplayAlert("", "You have successfully changed your profile picture.", "OK");
                    // Go back to the previous page.    
                    await Navigation.PopAsync();
                }

                else
                {
                    ai.IsRunning = false;
                    ai.IsVisible = false;

                    await DisplayAlert("", "There was a problem while changing your profile picture.", "OK");
                    // Go back to the previous page.    
                    //await Navigation.PopAsync();
                    Debug.WriteLine($"response  : {response.RequestMessage.ToString()}");

                    Debug.WriteLine($"responseStatus : {response.StatusCode}");
                    //throw new Exception(response.RequestMessage.ToString());
                }
            }
            catch (Exception e)
            {
                ai.IsRunning = false;
                ai.IsVisible = false;
            }
        }

        /// <summary>
        ///     This selects an Image from the Gallery & Camera.
        /// </summary>
        private void SelectImageFromGallery(object sender, EventArgs e)
        {
            new ImageCropper
            {
                CropShape = ImageCropper.CropShapeType.Rectangle,
                AspectRatioX = 1,
                AspectRatioY = 1,
                Success = imageFile =>
                {
                    Debug.WriteLine($"imageFile: {imageFile}");

                    Debug.WriteLine($"imageFile Height: { DependencyService.Get<IImageResource>().GetSize(imageFile).Height} Width: {DependencyService.Get<IImageResource>().GetSize(imageFile).Width}");
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        profile_img.Source = ImageSource.FromFile(imageFile);
                        HeightValue = DependencyService.Get<IImageResource>().GetSize(imageFile).Height;
                        WidthValue = DependencyService.Get<IImageResource>().GetSize(imageFile).Width;

                        if (Device.RuntimePlatform.Equals(Device.Android))
                        {


                            //Debug.WriteLine($"profile_img.Source  : {profile_img.Source.ToString()}");

                            string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                            //Debug.WriteLine($"folderPath  : {folderPath}");
                            var pathFile = profile_img.Source
                                .ToString().Replace("/data/user/0", "/data/data");
                            //Debug.WriteLine($"pathFile  : {pathFile}");
                            File.Copy(imageFile, Path.Combine(folderPath, Path.GetFileName(pathFile)));
                        }
                        //else
                        //{
                        //    profile_img.Source = ImageSource.FromFile(imageFile);
                        //}
                    });
                }
            }.Show(this);

            //imageCropper;
            //Debug.WriteLine($"Aspect Ratio X: {imageCropper.AspectRatioX}, Y: {imageCropper.AspectRatioY}");
        }



        /// <summary>
        ///     This saves a profile picture by uploading it.
        /// </summary>
        private void SaveProfilePicture(object sender, EventArgs e)
        {
            UploadImage();
        }
    }
}