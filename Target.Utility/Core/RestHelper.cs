using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Target.Utility.Dtos;

namespace Target.Utility.Core
{
    /// <summary>
    /// Cette classe contient des méthodes d'extention sur HttpContent (System.Net.Http.Formatting.Extension).
    /// Si les lignes avec response.Content.ReadAsAsync et _client.PostAsJsonAsync ne compile pas,
    /// Il faut se les procurer avec Nuget : Install-Package System.Net.Http.Formatting.Extension -ProjectName ""
    /// </summary>
    public class RestHelper
    {
        #region Fields

        private readonly HttpClient _client;

        #endregion

        #region Constructors

        public RestHelper()
        {
            _client = new HttpClient { BaseAddress = new Uri("http://dronolab-dashboard.azurewebsites.net/") };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            // todo REST token
        }

        #endregion

        #region Publics Methods

        public async Task<List<SlackUserDto>> GetAsync()
        {
            List<SlackUserDto> users = null;
            var response = await _client.GetAsync("api/SlackUsersApi").ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                users = await response.Content.ReadAsAsync<List<SlackUserDto>>();
            }

            return users;
        }

        public async Task<SlackUserDto> UpdateAsync(SlackUserDto slackUser)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync($"api/SlackUsersApi/{slackUser.Id}", slackUser);

            // Lance une exception si nous n'avons pas un HTTP 2xx
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            slackUser = await response.Content.ReadAsAsync<SlackUserDto>();
            return slackUser;
        }

        #endregion

    }
}
