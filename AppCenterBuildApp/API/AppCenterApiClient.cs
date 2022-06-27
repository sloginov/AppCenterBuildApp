using AppCenterBuildApp.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text;

namespace AppCenterBuildApp.API
{
    internal class AppCenterApiClient
    {
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

        private string GetApiRequestUrl(string endpoint) => string.Concat(Constants.APP_CENTER_BASE_URL, endpoint.TrimStart('/'));

        private async Task<string> HandleResponse(string endpoint, HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            else
                throw new AppCenterApiResponseError(endpoint, response);
        }

        /// <summary>
        /// Send get request to App Center API
        /// </summary>
        /// <param name="endpoint">API endpoint (see list of endpoints: https://openapi.appcenter.ms/)</param>
        /// <returns>Text of response (JSON)</returns>
        private async Task<string> SendApiGetRequestAsync(string endpoint)
        {
            using HttpClient client = CreateHttpClient();
            HttpResponseMessage response = await client.GetAsync(GetApiRequestUrl(endpoint));
            var result = await HandleResponse(endpoint, response);
            return result;
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
            HttpResponseMessage response = await client.PostAsync(GetApiRequestUrl(endpoint), content ?? new StringContent(string.Empty));
            var result = await HandleResponse(endpoint, response);
            return result;
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
            HttpResponseMessage response = await client.PatchAsync(GetApiRequestUrl(endpoint), content ?? new StringContent(string.Empty));
            var result = await HandleResponse(endpoint, response);
            return result;
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
        /// <param name="appOwnerName">App owner name</param>
        /// <param name="appName">App name</param>
        /// <param name="branchName">Branch name</param>
        /// <returns></returns>
        public async Task<BuildInfo> CreateBuildAsync(string appOwnerName, string appName, string branchName)
        {
            //TODO: Consider cancellation of last build if it's running
            var response = await SendApiPostRequestAsync($"/apps/{appOwnerName}/{appName}/branches/{branchName}/builds");
            return JsonSerializer.Deserialize<BuildInfo>(response, ResponseJsonSerializerOptions);
        }

        /// <summary>
        /// Get build info
        /// </summary>
        /// <param name="appOwnerName">App owner name</param>
        /// <param name="appName">App name</param>
        /// <param name="buildId">Build id</param>
        /// <returns></returns>
        public async Task<BuildInfo> GetBuildInfoAsync(string appOwnerName, string appName, int buildId)
        {
            var response = await SendApiGetRequestAsync($"/apps/{appOwnerName}/{appName}/builds/{buildId}");
            return JsonSerializer.Deserialize<BuildInfo>(response, ResponseJsonSerializerOptions);
        }

        /// <summary>
        /// Cancel build
        /// </summary>
        /// <param name="appOwnerName">App owner name</param>
        /// <param name="appName">App name</param>
        /// <param name="buildId">Build id</param>
        /// <returns></returns>
        public async Task CancelBuildAsync(string appOwnerName, string appName, int buildId)
        {
            await SendApiPatchRequestAsync(
                $"/apps/{appOwnerName}/{appName}/builds/{buildId}",
                new StringContent("{ \"status\": \"cancelling\" }", Encoding.UTF8, "application/json"));
        }
    }
}
