using Newtonsoft.Json;
using RestSharp;
using System.Configuration;
using System.Text.RegularExpressions;
using VC.Res.Core.Settings;

namespace VC.Res.Core.Integrations.Zoho
{
    internal class ZConfiguration
    {
        Configuration config;
        

        private string GetToken()
        {
            var client = new HttpClient();
            //{ Global.Fetch.Zoho_Token_Url}
            //{Global.Fetch.Zoho_Refresh_Token}
            //{ Global.Fetch.Zoho_Client_Id}
            // {Global.Fetch.Zoho_Client_Secret}
            string url = $@"{ZConfigurationOption.ZConfig.TokenUrl}?refresh_token={ZConfigurationOption.ZConfig.RefereshToken}&client_id={ZConfigurationOption.ZConfig.ClientId}&client_secret={ZConfigurationOption.ZConfig.ClientSecret}&grant_type=refresh_token";
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var res = client.Send(request);
            Task<string> _t = res.Content.ReadAsStringAsync();
            string data = _t.Result;
            ZohoToken tokenObj = JsonConvert.DeserializeObject<ZohoToken>(data) ?? new ZohoToken();
            return tokenObj.access_token ?? "";
        }

        public async Task<T> ZClientRequest<T>(string method, object data, string Module = "Contacts")
        {
            try
            {
                int stepcount = 0;
                string accessToken = (string)LocalCache.Get(LocalCache.Key.Zoho_Token, "", false) ?? "";

                REPEATE_GENERATE_NEW_TOKEN:
                if (string.IsNullOrEmpty(accessToken))
                {
                    accessToken = GetToken();
                    LocalCache.Set(LocalCache.Key.Zoho_Token, accessToken);
                }

                var client = new RestClient();
                var request = new RestRequest($@"{ZConfigurationOption.ZConfig.ApiUrl}{Module}");
                request.Method = method.Equals("POST") ? Method.Post : method.Equals("PUT") ? Method.Put : Method.Delete;
                if(!method.Equals("DELETE"))
                    request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", $@"Bearer {accessToken}");
               if(!method.Equals("DELETE"))
                    request.AddStringBody(JsonConvert.SerializeObject(data), DataFormat.Json);
                RestResponse response = await client.ExecuteAsync(request);
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    if (stepcount < 2)
                    {
                        stepcount++;
                        accessToken = string.Empty;
                        goto REPEATE_GENERATE_NEW_TOKEN;
                    }
                }
                return JsonConvert.DeserializeObject<T>(response.Content.ToString());
            }
            catch (Exception ex)
            {
                return default(T);
            }

        }
    }

    public class ZohoToken
    {
        public string? access_token { get; set; }
        public string? scope { get; set; }
        public string? api_domain { get; set; }
        public string? token_type { get; set; }
        public int expires_in { get; set; }
    }

    public class ZConfigurationOption
    {
        public string? ApiUrl { get; set; }
        public string? RefereshToken { get; set; }
        public string? TokenUrl { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }

        public static ZConfigurationOption ZConfig { get; set; }
    }
}
