using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Via.Models;
using static Via.Data.Constants;
using System.Net;
using System.Net.Http.Headers;
using Via.Views;
using System.IO;
using Plugin.Media.Abstractions;
using System.Diagnostics;
using Xamarin.Forms.Internals;

namespace Via.Data
{
    public static class ViaAsyncTasks
    {
        private static readonly HttpClient _client = new HttpClient();
        private static ViaUser user = ViaSessions.GetUser();

        public static async Task<HttpResponseMessage> SignInAsync(string username, string password)
        {
            var uri = new Uri(string.Format(Constants.AuthorizationUrl + "/Auth/Users/Authorize", string.Empty));

            try
            {
                var json = JsonConvert.SerializeObject(new Credentials()
                {
                    username = username,
                    password = password,
                    grantType = "password"
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(uri, content);

               
                return response;

                
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            }
        }

        public static async Task<HttpResponseMessage> SendReport(ReportData report, ViaUser user)
        {
            Debug.WriteLine("Am here trying to send report {0}", report.Data.LND);
            try
            {
                Debug.WriteLine("Am here trying to send report");
                var Url = new Uri(string.Format("https://developapi.via.nl/report/{0}/StoreAccidentData", report.Data.LND));

               
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token.token);

                var json = JsonConvert.SerializeObject(report.Data);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(Url, content);

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Successfukle {response.Content.ReadAsStringAsync()}");
                    return response;
                }

                Debug.WriteLine($"Status Code : {response.StatusCode}");
                Debug.WriteLine($"Reaseon for the code : {response.ReasonPhrase}");

                return null;

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception: {ex.Message}");

                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            }
        }

        public static async Task<HttpResponseMessage> UploadAccidentImage(byte[] _accidentPhoto, string accidentID, string countryCode, ViaUser user)
        {
            var Url = new Uri(string.Format("https://developapi.via.nl/report/{0}/UploadImage/{1}", countryCode, accidentID));
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token.token);

                var fileContent = new ByteArrayContent(_accidentPhoto);

                var response = await _client.PostAsync(Url, fileContent);

                if (response.IsSuccessStatusCode)
                {
                    return response;
                }
            
                return null;
 
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            }
        }

        public static async Task<HttpResponseMessage> SendMail(string email)
        {
            var uri = new Uri(string.Format(Constants.AuthorizationUrl + "/Auth/Users/ForgotPassword", string.Empty));

            try
            {
                var json = JsonConvert.SerializeObject(new ForgotCredentials()
                {
                    emailaddress = email,
                    callbackUrl = ""
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(uri, content);

                return response;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            }
        }

        public static async Task<HttpResponseMessage> GetReports(string countryCode, AccidentRequest accRequest)
        {
            var uri = new Uri(string.Format(Constants.AuthorizationUrl + "/{0}/AccidentTable", countryCode));

            try
            {
                var json = JsonConvert.SerializeObject(accRequest);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccidentReports.user.token.token);
                var response = await _client.PostAsync(uri, content);

                return response;
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            }
        }

        public static async Task<HttpResponseMessage> UploadImage(FileStream _fileStream)
        {
            var Url = new Uri(string.Format("https://developapi.via.nl/report/{0}/UploadImage/{1}", CreateReport._controls.Data.LND, CreateReport._controls.Data.Id));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token.token);
            try
            {
                var json = JsonConvert.SerializeObject(_fileStream);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(Url, content);

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            }
        }

        public static string GetImage(string Url)
        {
            try
            {
                CheckUserValidity();
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token.token);
                var response = _client.GetAsync(Url).Result;
                var _image = JsonConvert.DeserializeObject<string>(response.Content.ReadAsStringAsync().Result);

                return _image;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static async void CheckUserValidity()
        {
            try
            {
                if (DateTime.Now > Convert.ToDateTime(user.token.validTo))
                {
                    await ViaSessions.RegenerateNewToken();
                    user = null;
                    user = ViaSessions.GetUser();
                }
            }
            catch (Exception ex)
            {
                Log.Warning("App Crash => ", ex.Message);
            }
        }
    }
}
