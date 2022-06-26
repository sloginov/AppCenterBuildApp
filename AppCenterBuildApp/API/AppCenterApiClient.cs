using AppCenterBuildApp.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json;
using System.Text;

namespace AppCenterBuildApp.API
{
    internal class AppCenterApiClient
    {
        private const string APP_CENTER_BASE_URL = "https://api.appcenter.ms/v0.1/";
        private string apiToken;
        private JsonSerializerOptions ResponseJsonSerializerOptions => new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="apiToken">Token for access to API</param>
        public AppCenterApiClient(string apiToken)
        {
            this.apiToken = apiToken;
        }

        private HttpClient CreateHttpClient()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //Put api token into header according documentation (https://docs.microsoft.com/en-us/appcenter/api-docs/#using-an-api-token-in-an-api-request)
            client.DefaultRequestHeaders.Add("X-API-Token", apiToken);

            return client;
        }

        /// <summary>
        /// Send get request to App Center API
        /// </summary>
        /// <param name="endpoint">API endpoint (see list of endpoints: https://openapi.appcenter.ms/)</param>
        /// <returns>Text of response (JSON)</returns>
        private async Task<string> SendApiGetRequestAsync(string endpoint)
        {
            using HttpClient client = CreateHttpClient();

            var requestUrl = string.Concat(APP_CENTER_BASE_URL, endpoint.TrimStart('/'));

            HttpResponseMessage response = await client.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
                throw new Exception($"Failed to execute get request '{endpoint}'. StatusCode: {response.StatusCode}, Reason: {response.ReasonPhrase}");
        }
        /// <summary>
        /// Send post request to App Center API
        /// </summary>
        /// <param name="endpoint">API endpoint (see list of endpoints: https://openapi.appcenter.ms/)</param>
        /// <param name="content"></param>
        /// <returns>Text of response (JSON)</returns>
        private async Task<string> SendApiPostRequestAsync(string endpoint, HttpContent content = null)
        {
            using HttpClient client = CreateHttpClient();

            var requestUrl = string.Concat(APP_CENTER_BASE_URL, endpoint.TrimStart('/'));

            HttpResponseMessage response = await client.PostAsync(requestUrl, content ?? new StringContent(string.Empty));
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
                throw new Exception($"Failed to execute post request '{endpoint}'. StatusCode: {response.StatusCode}, Reason: {response.ReasonPhrase}");
        }
        /// <summary>
        /// Send patch request to App Center API
        /// </summary>
        /// <param name="endpoint">API endpoint (see list of endpoints: https://openapi.appcenter.ms/)</param>
        /// <param name="content"></param>
        /// <returns>Text of response (JSON)</returns>
        private async Task<string> SendApiPatchRequestAsync(string endpoint, HttpContent content = null)
        {
            using HttpClient client = CreateHttpClient();

            var requestUrl = string.Concat(APP_CENTER_BASE_URL, endpoint.TrimStart('/'));

            HttpResponseMessage response = await client.PatchAsync(requestUrl, content ?? new StringContent(string.Empty));
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
                throw new Exception($"Failed to execute patch request '{endpoint}'. StatusCode: {response.StatusCode}, Reason: {response.ReasonPhrase}");
        }

        /// <summary>
        /// Retrieve app list
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<AppInfo>> GetAppsAsync()
        {
            var response = await SendApiGetRequestAsync("/apps");
            return JsonSerializer.Deserialize<IEnumerable<AppInfo>>(response, ResponseJsonSerializerOptions);
        }


        /// <summary>
        /// Retrieve the list of app branches
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<BranchStatus>> GetBranchListAsync(AppInfo appInfo)
        {
            var response = await SendApiGetRequestAsync($"/apps/{appInfo.Owner.Name}/{appInfo.Name}/branches");
            return JsonSerializer.Deserialize<IEnumerable<BranchStatus>>(response, ResponseJsonSerializerOptions);
        }

        /// <summary>
        /// Create a new build
        /// </summary>
        /// <param name="appInfo">App info</param>
        /// <param name="branchName">Branch name</param>
        /// <returns></returns>
        public async Task<BuildInfo> CreateBuildAsync(AppInfo appInfo, string branchName)
        {
            //TODO: Consider cancellation of last build if it's running
            var response = await SendApiPostRequestAsync($"/apps/{appInfo.Owner.Name}/{appInfo.Name}/branches/{branchName}/builds");
            return JsonSerializer.Deserialize<BuildInfo>(response, ResponseJsonSerializerOptions);
        }

        /// <summary>
        /// Get build info
        /// </summary>
        /// <param name="appInfo">App info</param>
        /// <param name="buildId">Build id</param>
        /// <returns></returns>
        public async Task<BuildInfo> GetBuildInfoAsync(AppInfo appInfo, int buildId)
        {
            var response = await SendApiGetRequestAsync($"/apps/{appInfo.Owner.Name}/{appInfo.Name}/builds/{buildId}");
            return JsonSerializer.Deserialize<BuildInfo>(response, ResponseJsonSerializerOptions);
        }

        /// <summary>
        /// Cancel build
        /// </summary>
        /// <param name="appInfo">App info</param>
        /// <param name="buildId">Build id</param>
        /// <returns></returns>
        public async Task CancelBuildAsync(AppInfo appInfo, int buildId)
        {
            await SendApiPatchRequestAsync(
                $"/apps/{appInfo.Owner.Name}/{appInfo.Name}/builds/{buildId}",
                new StringContent("{ \"status\": \"cancelling\" }", Encoding.UTF8, "application/json"));
        }
    }
}
