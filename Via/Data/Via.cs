using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Via.Models;
using Xamarin.Forms;
using static Xamarin.Forms.Application;

namespace Via.Data
{
    public static class ViaSessions
    {
        // Save data Values
        public static void SaveUser(ViaUser _user)
        {
            var userJson = JsonConvert.SerializeObject(_user);
            if (Current.Properties.ContainsKey("user"))
                Current.Properties.Remove("user");

            Current.Properties.Add("user", userJson);
            Current.SavePropertiesAsync();
        }

        public static void SaveSettings(InitialSettings _settings)
        {
            var userJson = JsonConvert.SerializeObject(_settings);
            if (Current.Properties.ContainsKey("settings"))
                Current.Properties.Remove("settings");

            Current.Properties.Add("settings", userJson);
            Current.SavePropertiesAsync();
        }

        /// <summary>
        ///     This method is responsible for regenerating a new token when it expires.
        /// </summary>
        public static async Task RegenerateNewToken()
        {
            try
            {
                var user = GetUser();

                // Do some work on a background thread, allowing the UI to remain responsive
                var password = Current.Properties["password"].ToString();
                await Task.Factory.StartNew(async () =>
                {
                    var response = ViaAsyncTasks.SignInAsync(user.username, password).Result;
                    var responseContent = await response.Content.ReadAsStringAsync();

                    user = new ViaUser();
                    user = JsonConvert.DeserializeObject<ViaUser>(responseContent);
                }).ContinueWith(task =>
                {
                    SaveUser(user);
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// This method downloads the settings object.
        /// </summary>
        public static async void DownloadSettings()
        {
            ViaUser user = GetUser();
            try
            {
                var token = GetUser().token.token;

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var content = await client.GetStringAsync(Via.Utils.Settings.BaseUrl + "/auth/Settings/");
                var InitialSettings = JsonConvert.DeserializeObject<InitialSettings>(content);

                SaveSettings(InitialSettings);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Settings Exception : ", ex.Message);
            }
        }

        /// <summary>
        ///Gets initial settings object.
        /// </summary>
        public static InitialSettings InitialSettings()
        {
            var userJson = "";
            if (Current.Properties.ContainsKey("settings")) userJson = Current.Properties["settings"] as string;

            // deserialize the json into the object
            var settings = new JsonSerializerSettings
            { NullValueHandling = NullValueHandling.Ignore, DateFormatHandling = DateFormatHandling.IsoDateFormat };
            var _settings = JsonConvert.DeserializeObject<InitialSettings>(userJson, settings);

            return _settings;
        }


        /// <summary>
        ///     Method responsible for Getting a full User JSON object.
        /// </summary>
        /// <returns>user object</returns>
        public static ViaUser GetUser()
        {
            var userJson = "";
            if (Current.Properties.ContainsKey("user")) userJson = Current.Properties["user"] as string;

            // deserialize the json into the object
            var settings = new JsonSerializerSettings
            { NullValueHandling = NullValueHandling.Ignore, DateFormatHandling = DateFormatHandling.IsoDateFormat };
            var _user = JsonConvert.DeserializeObject<ViaUser>(userJson, settings);

            return _user;
        }

        /// <summary>
        ///     Method Responsible for Logging out.
        /// </summary>
        public static async void Logout()
        {
            if (Current.Properties.ContainsKey("user"))
            {
                Current.Properties.Remove("user");
                Current.Properties.Clear();
                await Current.SavePropertiesAsync();
            }
        }

        public static void SavePassword(string password)
        {
            if (Current.Properties.ContainsKey("password"))
                Current.Properties.Remove("password");

            Current.Properties.Add("password", password);
            Current.SavePropertiesAsync();
        }
    }
}