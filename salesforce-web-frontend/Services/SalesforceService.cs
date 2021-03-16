using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using salesforce_web_frontend.Models;

namespace salesforce_web_frontend.Services
{
    public interface ISalesforceService
    {
        Task<string> CreateLeadAsync(Lead lead);
    }


    public class SalesforceService : ISalesforceService
    {
        public SalesforceService(IConfiguration configuration)
        {
            Username = configuration.GetSection("Salesforce")["Username"];
            Password = configuration.GetSection("Salesforce")["Password"];
            Token = configuration.GetSection("Salesforce")["Token"];
            ClientSecret = configuration.GetSection("Salesforce")["ClientSecret"];
            ClientId = configuration.GetSection("Salesforce")["ClientId"];
            LoginEndpoint = configuration.GetSection("Salesforce")["LoginEndpoint"];
            ApiEndpoint = configuration.GetSection("Salesforce")["ApiEndpoint"];
        }

        private string Username { get; }
        private string Password { get; }

        private string Token { get; }

        private string ClientId { get; }
        private string ClientSecret { get; }


        private string LoginEndpoint { get; }
        private string ApiEndpoint { get; }


        public async Task<string> CreateLeadAsync(Lead lead)
        {
            //Auth
            var salesforceAuthentificationResponse = await Auth();

            using var client = new HttpClient();
            var json = JsonSerializer.Serialize(lead);

            //Arrage 
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post,
                $"{salesforceAuthentificationResponse.instance_url}{ApiEndpoint}sobjects/Lead"); //Build up POST url
            request.Headers.Add("Authorization", $"Bearer {salesforceAuthentificationResponse.access_token}");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            request.Content = content;

            //Send request with Auth Token 
            var response = await client.SendAsync(request);

            //Get results back. SF will response with a HTTP Status Code and a message
            return await response.Content.ReadAsStringAsync();
        }


        /// <summary>
        ///     Auth to Salesforce and return the login object that contains the access token and instance url
        /// </summary>
        /// <returns></returns>
        public async Task<SalesforceAuthentificationResponse> Auth()
        {
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("username", Username),
                new KeyValuePair<string, string>("password", Password + Token)
            });
            var httpClient = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(LoginEndpoint),
                Content = content
            };
            var responseMessage = await httpClient.SendAsync(request);
            var response = await responseMessage.Content.ReadAsStringAsync();
            var salesforceAuthentificationResponse =
                JsonSerializer.Deserialize<SalesforceAuthentificationResponse>(response);

            return salesforceAuthentificationResponse;
        }
    }
}