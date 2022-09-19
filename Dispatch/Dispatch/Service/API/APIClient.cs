using Dispatch.Helpers;
using Dispatch.Service.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch.Service.API
{
    public class APIClient
    {
        public readonly string Platform = Environment.OSVersion.ToString();

        public string HardwareIdentifier { get; }

        public APIClient(string hardwareIdentifier)
        {
            HardwareIdentifier = hardwareIdentifier;
        }

        private async Task<T> Request<T>(HttpMethod method, string url, object body = null)
        {
            var client = new HttpClient();

            var message = new HttpRequestMessage(method, new Uri(url));
            message.Headers.Add("Accept", "application /json");
            message.Headers.Add("User-Agent", Constants.APP_NAME);

            if (body != null)
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            }

            var response = await client.SendAsync(message);

            if (!response.IsSuccessStatusCode)
            {
                var errorJson = await response.Content.ReadAsStringAsync();
                var error = JObject.Parse(errorJson); // Try

                if (error.TryGetValue("message", out JToken value))
                {
                    throw new Exception(value.Value<string>());
                }

                throw new Exception("Unknown error message");
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public Task<Device> InstallTrial()
        {
            var body = new Dictionary<string, string>() { { "platform", Platform } };
            return Request<Device>(HttpMethod.Post, $"{Constants.API_URL}/device/{HardwareIdentifier}/trial", body);
        }

        public Task<Device> InstallLicense(string key)
        {
            var body = new Dictionary<string, string>() { { "platform", Platform }, { "key", key } };
            return Request<Device>(HttpMethod.Post, $"{Constants.API_URL}/device/{HardwareIdentifier}/license", body);
        }

        public Task<Device> RemoveLicense(string key)
        {
            var body = new Dictionary<string, string>() { { "key", key } };
            return Request<Device>(HttpMethod.Delete, $"{Constants.API_URL}/device/{HardwareIdentifier}/license", body);
        }

        public Task<DeviceStatus> GetDeviceStatus()
        {
            return Request<DeviceStatus>(HttpMethod.Get, $"{Constants.API_URL}/device/{HardwareIdentifier}/{Constants.VERSION}");
        }
    }
}
