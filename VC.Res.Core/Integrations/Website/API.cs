using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace VC.Res.Core.Integrations.Website
{
    public static class API
    {
        private static readonly HttpClient s_httpClient = new() { Timeout = new TimeSpan(0, 1, 0) };

        private static string s_premisesEndpointURL = "/umbraco/api/premises/";
        private static string s_countryEndpointURL = "/umbraco/api/country/";
        private static string s_regionEndpointURL = "/umbraco/api/region/";

        #region Premises Endpoints

        public static async Task<APIResponse<bool>> Premises_CreateAsync(Premises.Premise premise)
        {
            var objReturn = new APIResponse<bool>();

            if (!Settings.Global.Fetch.Website_APIEnabled) { return objReturn; }

            try
            {
                // set content
                var requestContent = new StringContent(JsonConvert.SerializeObject(await ModelConverter.To_PremiseAsync(premise)));
                requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // add auth header
                requestContent.Headers.Add("ApiKey", Settings.Global.Fetch.Website_APIKey);

                // send request
                var response = await s_httpClient.PostAsync(Settings.Global.Fetch.Website_URL.TrimEnd('/') + s_premisesEndpointURL + "create", requestContent);
                requestContent.Dispose();

                // successfully got a response at this point
                objReturn.Success = true;

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var objReturnedPremise = JsonConvert.DeserializeObject<Shared.Models.Premise>(content);
                    content = null;

                    if (objReturnedPremise.Loaded)
                    {
                        _ = await Premises.Premise.Update_WebsiteIntegration(premise.Id, objReturnedPremise, "Website API");

                        objReturn.Result = true;
                    }

                    objReturnedPremise = null;
                }
                else
                {
                    await objReturn.ProcessError(response);

                    _ = Error.Generic(typeof(API).ToString(), "Premises_CreateAsync(Premises.Premise)", "premise: " + JsonConvert.SerializeObject(premise), objReturn.Error_Message);
                }

                response.Dispose();
                requestContent.Dispose();
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(API).ToString(), "Premises_CreateAsync(Premises.Premise)", ex,
                    "premise: " + JsonConvert.SerializeObject(premise));
                return objReturn;
            }

            return objReturn;
        }

        public static async Task<APIResponse<bool>> Premises_UpdateAsync(Premises.Premise premise)
        {
            var objReturn = new APIResponse<bool>();

            if (!Settings.Global.Fetch.Website_APIEnabled) { return objReturn; }

            try
            {
                // set content
                var requestContent = new StringContent(JsonConvert.SerializeObject(await ModelConverter.To_PremiseAsync(premise)));
                requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // add auth header
                requestContent.Headers.Add("ApiKey", Settings.Global.Fetch.Website_APIKey);

                // send request
                var response = await s_httpClient.PostAsync(Settings.Global.Fetch.Website_URL.TrimEnd('/') + s_premisesEndpointURL + "update", requestContent);
                requestContent.Dispose();

                // successfully got a response at this point
                objReturn.Success = true;

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var objReturnedPremise = JsonConvert.DeserializeObject<Shared.Models.Premise>(content);
                    content = null;

                    if (objReturnedPremise.Loaded)
                    {
                        _ = await Premises.Premise.Update_WebsiteIntegration(premise.Id, objReturnedPremise, "Website API");

                        objReturn.Result = true;
                    }

                    objReturnedPremise = null;
                }
                else
                {
                    await objReturn.ProcessError(response);

                    _ = Error.Generic(typeof(API).ToString(), "Premises_UpdateAsync(Premises.Premise)", "premise: " + JsonConvert.SerializeObject(premise), objReturn.Error_Message);
                }

                response.Dispose();
                requestContent.Dispose();
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(API).ToString(), "Premises_UpdateAsync(Premises.Premise)", ex,
                    "premise: " + JsonConvert.SerializeObject(premise));
                return objReturn;
            }

            return objReturn;
        }

        public static async Task<APIResponse<bool>> Premises_DeleteAsync(int iId)
        {
            var objReturn = new APIResponse<bool>();

            if (!Settings.Global.Fetch.Website_APIEnabled) { return objReturn; }

            try
            {
                // set content
                var requestContent = new StringContent(iId.ToString());
                requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // add auth header
                requestContent.Headers.Add("ApiKey", Settings.Global.Fetch.Website_APIKey);

                // send request
                var response = await s_httpClient.PostAsync(Settings.Global.Fetch.Website_URL.TrimEnd('/') + s_premisesEndpointURL + "delete", requestContent);
                requestContent.Dispose();

                // successfully got a response at this point
                objReturn.Success = true;

                if (response.IsSuccessStatusCode)
                {
                    objReturn.Result = true;
                }
                else
                {
                    await objReturn.ProcessError(response);

                    _ = Error.Generic(typeof(API).ToString(), "Premises_DeleteAsync(int)", "iId: " + iId.ToString(), objReturn.Error_Message);
                }

                response.Dispose();
                requestContent.Dispose();
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(API).ToString(), "Premises_DeleteAsync(int)", ex,
                    "iId: " + iId.ToString());
                return objReturn;
            }

            return objReturn;
        }

        #endregion premises endpoints


        #region Country Endpoints

        public static async Task<APIResponse<bool>> Country_CreateAsync(Common.Country country)
        {
            var objReturn = new APIResponse<bool>();

            if (!Settings.Global.Fetch.Website_APIEnabled) { return objReturn; }

            try
            {
                // set content
                var requestContent = new StringContent(JsonConvert.SerializeObject(ModelConverter.To_Country(country)));
                requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // add auth header
                requestContent.Headers.Add("ApiKey", Settings.Global.Fetch.Website_APIKey);

                // send request
                var response = await s_httpClient.PostAsync(Settings.Global.Fetch.Website_URL.TrimEnd('/') + s_countryEndpointURL + "create", requestContent);
                requestContent.Dispose();

                // successfully got a response at this point
                objReturn.Success = true;

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var objReturnedCountry = JsonConvert.DeserializeObject<Shared.Models.Country>(content);
                    content = null;

                    if (objReturnedCountry.Loaded)
                    {
                        _ = await Common.Country.Update_WebsiteIntegration(country.Id, objReturnedCountry.Umb_Id, "Website API");

                        objReturn.Result = true;
                    }

                    objReturnedCountry = null;
                }
                else
                {
                    await objReturn.ProcessError(response);

                    _ = Error.Generic(typeof(API).ToString(), "Country_CreateAsync(Common.Country)", "country: " + JsonConvert.SerializeObject(country), objReturn.Error_Message);
                }

                response.Dispose();
                requestContent.Dispose();
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(API).ToString(), "Country_CreateAsync(Common.Country)", ex,
                    "country: " + JsonConvert.SerializeObject(country));
                return objReturn;
            }

            return objReturn;
        }

        #endregion country endpoints


        #region Region Endpoints

        public static async Task<APIResponse<bool>> Region_CreateAsync(Common.Region region)
        {
            var objReturn = new APIResponse<bool>();

            if (!Settings.Global.Fetch.Website_APIEnabled) { return objReturn; }

            try
            {
                // set content
                var requestContent = new StringContent(JsonConvert.SerializeObject(ModelConverter.To_Region(region)));
                requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // add auth header
                requestContent.Headers.Add("ApiKey", Settings.Global.Fetch.Website_APIKey);

                // send request
                var response = await s_httpClient.PostAsync(Settings.Global.Fetch.Website_URL.TrimEnd('/') + s_regionEndpointURL + "create", requestContent);
                requestContent.Dispose();

                // successfully got a response at this point
                objReturn.Success = true;

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var objReturnedRegion = JsonConvert.DeserializeObject<Shared.Models.Region>(content);
                    content = null;

                    if (objReturnedRegion.Loaded)
                    {
                        _ = await Common.Region.Update_WebsiteIntegration(region.Id, objReturnedRegion.Umb_Id, "Website API");

                        objReturn.Result = true;
                    }

                    objReturnedRegion = null;
                }
                else
                {
                    await objReturn.ProcessError(response);

                    _ = Error.Generic(typeof(API).ToString(), "Region_CreateAsync(Common.Region)", "region: " + JsonConvert.SerializeObject(region), objReturn.Error_Message);
                }

                response.Dispose();
                requestContent.Dispose();
            }
            catch (Exception ex)
            {
                _ = Error.Exception(typeof(API).ToString(), "Region_CreateAsync(Common.Region)", ex,
                    "region: " + JsonConvert.SerializeObject(region));
                return objReturn;
            }

            return objReturn;
        }

        #endregion region endpoints
    }
}
