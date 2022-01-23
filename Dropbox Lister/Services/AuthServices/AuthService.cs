using Dropbox_Lister.DTOs.Exceptions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Dropbox_Lister.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private HttpClient _client;
        private readonly IConfigurationRoot _config;
        public AuthService(HttpClient client, IConfigurationRoot config)
        {
            _client = client;
            _config = config;
        }

        public async void AddSelectUserHeader()
        {
            var response = _client.PostAsJsonAsync("https://content.dropboxapi.com/2/team/members/list_v2", new { limit = 100 }).Result;
            var result = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());
            if (result != null)
                _client.DefaultRequestHeaders.Add("Dropbox-API-Select-User", result["members"][0]["profile"]["team_member_id"].ToString());

            Console.WriteLine("User Header Set");
        }

        public async Task GetCode(string appkey)
        {
            if (string.IsNullOrEmpty(appkey))
                throw new ArgumentNullException();

            if (_config["openBrowserLink"] == "" || !_config["openBrowserLink"].StartsWith("https://www.dropbox.com/"))
                throw new ArgumentNullException();
            
            string url = _config["openBrowserLink"].Replace("{appkey}", appkey);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
            }

            await SetBearerToken(EnterAppCode());

        }

        public async Task SetBearerToken(string appCode)
        {
            if (_client.DefaultRequestHeaders.Authorization.Scheme == "Basic" && !string.IsNullOrEmpty(appCode))
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();

                parameters.Add("code", appCode.Trim());
                parameters.Add("grant_type", "authorization_code");

                var response = _client.PostAsync(_config["OAuth2Endpoint"], new FormUrlEncodedContent(parameters)).Result;
                if (response.IsSuccessStatusCode)
                {
                    var result = JsonConvert.DeserializeObject<JToken>(await response.Content.ReadAsStringAsync());
                    if (result != null)
                    {
                        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result["access_token"].ToString());
                        Console.WriteLine("Code Granted !");
                        AddSelectUserHeader();
                    }
                }
                else
                {
                    var error = JsonConvert.DeserializeObject<GenericError>(await response.Content.ReadAsStringAsync());
                    Console.Error.WriteLine("An Error has occured during authentication: " + error.ErrorSummary);

                    await SetBearerToken(EnterAppCode());
                }
            }
            else if (string.IsNullOrEmpty(appCode))
            {
                throw new Exception("appCode Cannot be empty");
            }
        }

        public string EnterAppCode()
        {
            Console.WriteLine("Please enter the code: ");
            var code = Console.ReadLine(); // TODO: Decide if should I wrap ReadLine to better test it
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException();

            Console.WriteLine("Please wait until your authentication");
            return code;
        }
    }
}
