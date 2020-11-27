using CSInside.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Globalization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace CSInside
{
    public class AuthTokenProvider : IAuthTokenProvider, IDisposable, IEquatable<AuthTokenProvider>
    {
        private readonly HttpClient client;

        private readonly SHA256Managed sha256 = new SHA256Managed();

        private readonly Random random = new Random();

        private string appId;

        private DateTime dateFetchTime;

        public AuthTokenProvider()
        {
            var handler = new SocketsHttpHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.GZip
            };
            client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("User-Agent", "dcinside.app");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
        }

        public AuthTokenProvider(string appId) : this()
        {
            this.appId = appId;
            dateFetchTime = DateTime.Now.Date;
        }

        public string GetAppId()
        {
            if (string.IsNullOrEmpty(appId) || DateTime.Now.Date.CompareTo(dateFetchTime) == 1)
            {
                string date = FetchDate();
                appId = FetchAppId(date);
            }
            return appId;
        }

        public string GetClientToken()
        {
            return $"{RandomString(11)}:APA91bFMI-0d1b0wJmlIWoDPVa_V5Nv0OWnAefN7fGLegy6D76TN_CRo5RSUO-6V7Wnq44t7Rzx0A4kICVZ7wX-hJd3mrczE5NnLud722k5c-XRjIxYGVM9yZBScqE3oh4xbJOe2AvDe";
        }

        private string FetchDate()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://json2.dcinside.com/json0/app_check_A_rina.php");
            var response = client.SendAsync(request).Result;
            JArray jArray = JsonConvert.DeserializeObject<JArray>(response.Content.ReadAsStringAsync().Result);
            if (!(bool)jArray[0]["result"])
            {
                throw new Exception((string)jArray[0]["cause"]);
            }
            dateFetchTime = DateTime.Now.Date;
            return (string)jArray[0]["date"];
        }

        private string FetchAppId(string date)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://dcid.dcinside.com/join/mobile_app_key_verification_3rd.php");
            var keyValuePairs = new Dictionary<string, string>();
            var value_token = sha256.ComputeHash($"dcArdchk_{date}".ToByteArray(Encoding.ASCII)).ToHexString();
            keyValuePairs.Add("value_token", value_token);
            keyValuePairs.Add("signature", "ReOo4u96nnv8Njd7707KpYiIVYQ3FlcKHDJE046Pg6s=");
            keyValuePairs.Add("vCode", "30350");
            keyValuePairs.Add("vName", "3.9.4");
            keyValuePairs.Add("client_token", GetClientToken());
            request.Content = new FormUrlEncodedContent(keyValuePairs);
            var response = client.SendAsync(request).Result;
            JArray jArray = JsonConvert.DeserializeObject<JArray>(response.Content.ReadAsStringAsync().Result);
            if(!(bool)jArray[0]["result"])
            {
                throw new Exception((string)jArray[0]["cause"]);
            }
            return (string)jArray[0]["app_id"];
        }

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public void Dispose()
        {
            client.Dispose();
            sha256.Dispose();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as AuthTokenProvider);
        }

        public bool Equals([AllowNull] AuthTokenProvider other)
        {
            return other != null &&
                client == other.client;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(client);
        }
    }
}
