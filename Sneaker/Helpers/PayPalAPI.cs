using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Sneaker.Helpers
{
    public class PayPalAPI
    {
        private readonly IConfiguration _configuration;
        public PayPalAPI(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> getRedirectURLToPayPal(double total, string currency)
        {
            try
            {
                return Task.Run(async () =>
                {
                    HttpClient httpClient = GetPaypalHttpClient();
                    PayPalAccessToken accessToken = await GetPayPalAccessTokenAsync(httpClient);
                    PayPalPaymentCreatedResponse createdResponse =
                        await CreatePayPalPaymentAsync(httpClient, accessToken, total, currency);
                    return createdResponse.links.First(x => x.rel == "approval_url").href;
                }).Result;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e, "Failed to login to Paypal");
                return null;
            }
        }

        private HttpClient GetPaypalHttpClient()
        {
            string sandbox = _configuration["PayPal:urlAPI"];
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(sandbox),
                Timeout = TimeSpan.FromSeconds(30),
            };
            return httpClient;
        }

        private async Task<PayPalAccessToken> GetPayPalAccessTokenAsync(HttpClient httpClient)
        {
            byte[] bytes = Encoding.GetEncoding("iso-8859-1")
                .GetBytes($"{_configuration["PayPal:clientId"]}:{_configuration["PayPal:secret"]}");

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "/v1/oauth2/token");
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(bytes));

            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "client_credentials"
            };

            requestMessage.Content = new FormUrlEncodedContent(form);

            HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);

            string content = await responseMessage.Content.ReadAsStringAsync();
            PayPalAccessToken accessToken = JsonConvert.DeserializeObject<PayPalAccessToken>(content);
            return accessToken;
        }

        private async Task<PayPalPaymentCreatedResponse> CreatePayPalPaymentAsync(HttpClient httpClient, PayPalAccessToken accessToken, double total, string currency)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, "v1/payments/payment");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.access_token);
            var payment = JObject.FromObject(new
            {
                intent = "sale",
                redirect_urls = new
                {
                    return_url = _configuration["PayPal:returnUrl"],
                    cancel_url = _configuration["PayPal:cancelUrl"]
                },
                payer = new { payment_method = "paypal" },
                transactions = JArray.FromObject(new[]
                {
                    new
                    {
                        amount = new
                        {
                            total = total,
                            currency = "USD"
                        }
                    }
                })
            });

            requestMessage.Content =
                new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);

            string content = await responseMessage.Content.ReadAsStringAsync();
            PayPalPaymentCreatedResponse paymentCreatedResponse =
                JsonConvert.DeserializeObject<PayPalPaymentCreatedResponse>(content);
            return paymentCreatedResponse;
        }


        public async Task<PayPalPaymentExecutedResponse> ExecutedPayment(string paymentId, string payerId)
        {
            try
            {
                HttpClient httpClient = GetPaypalHttpClient();
                PayPalAccessToken accessToken = await GetPayPalAccessTokenAsync(httpClient);
                return await ExecutedPaypalPaymentAsync(httpClient, accessToken, payerId, paymentId);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e, "Failed to login to Paypal");
                return null;
            }
        }

        private async Task<PayPalPaymentExecutedResponse> ExecutedPaypalPaymentAsync(HttpClient httpClient, PayPalAccessToken accessToken, string payerId, string paymentId)
        {
            HttpRequestMessage requestMessage =
                new HttpRequestMessage(HttpMethod.Post, $"v1/payments/payment/{paymentId}/execute");
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.access_token);

            var payment = JObject.FromObject(new
            {
                payer_id = payerId
            });

            requestMessage.Content =
                new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);
            string content = await responseMessage.Content.ReadAsStringAsync();
            PayPalPaymentExecutedResponse paymentExecutedResponse =
                JsonConvert.DeserializeObject<PayPalPaymentExecutedResponse>(content);
            return paymentExecutedResponse;
        }
    }
    
}