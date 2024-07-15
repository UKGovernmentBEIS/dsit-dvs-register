﻿using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace DVSRegister.CommonUtility
{
    public static class HttpRequestHelper
    {
        public static HttpMessageHandler handler { get; set; }

        public static Task<T> SendGetRequestAsync<T>(RequestParameters parameters)
        {
            return SendRequestAsync<T>(RequestType.Get, parameters);
        }

        public static Task<T> SendPostRequestAsync<T>(RequestParameters parameters)
        {
            return SendRequestAsync<T>(RequestType.Post, parameters);
        }

        public static string ConvertToBase64(string username, string password)
        {
            return Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{username}:{password}"));
        }

        private static async Task<T> SendRequestAsync<T>(RequestType requestType, RequestParameters parameters)
        {
            var httpClient = SetupHttpClient(parameters);
            var response = await (requestType switch
            {
                RequestType.Get => httpClient.GetAsync(parameters.Path),
                RequestType.Post => httpClient.PostAsync(parameters.Path, parameters.Body),
                _ => throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null)
            });
            return ConvertResponseToObject<T>(response);
        }

        private static HttpClient SetupHttpClient(RequestParameters parameters)
        {
            var httpClient = new HttpClient(handler ?? new HttpClientHandler());
            httpClient.BaseAddress = new Uri(parameters.BaseAddress);
            httpClient.DefaultRequestHeaders.Authorization = parameters.Auth;
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            foreach (var headerDetails in parameters.Headers)
            {
                httpClient.DefaultRequestHeaders.Add(headerDetails.Key, headerDetails.Value);
            }
            return httpClient;
        }

        private static T ConvertResponseToObject<T>(HttpResponseMessage response)
        {
            var bodyString = response.Content.ReadAsStringAsync().Result;

            if (!response.IsSuccessStatusCode)
            {
                //throw new ApiException($"Request to {response.RequestMessage?.RequestUri} failed. " +
                //                       $"Error message: {response.StatusCode}; {response.ReasonPhrase}; {bodyString}", response.StatusCode, bodyString);
            }

            return JsonConvert.DeserializeObject<T>(bodyString);
        }
    }

    public class RequestParameters
    {
        public string BaseAddress { get; set; }
        public string Path { get; set; }
        public AuthenticationHeaderValue Auth { get; set; }
        public HttpContent Body { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new();
    }

    internal enum RequestType
    {
        Get,
        Post,
    }
}
