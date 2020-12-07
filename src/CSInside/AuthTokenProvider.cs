using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CSInside.Extensions;
using Newtonsoft.Json.Linq;

namespace CSInside
{
    public class AuthTokenProvider : IAuthTokenProvider, IDisposable
    {
        private readonly HttpClient client;

        private readonly SHA256Managed sha256 = new SHA256Managed();

        private readonly Random random = new Random();

        private string accessToken;

        private string clientToken;
#nullable enable
        private string? userToken = null;
#nullable restore
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

        public AuthTokenProvider(string accessToken) : this()
        {
            this.accessToken = accessToken;
            dateFetchTime = DateTime.Now.Date;
        }

        public AuthTokenProvider(string accessToken, string userToken) : this(accessToken)
        {
            this.userToken = userToken;
        }

        public string GetAccessToken()
        {
            if (string.IsNullOrEmpty(accessToken) || DateTime.Now.Date.CompareTo(dateFetchTime) == 1)
            {
                accessToken = FetchAccessTokenAsync().Result;
            }
            return accessToken;
        }

        public string GetClientToken()
        {
            clientToken = $"{RandomString(11)}:APA91bFMI-0d1b0wJmlIWoDPVa_V5Nv0OWnAefN7fGLegy6D76TN_CRo5RSUO-6V7Wnq44t7Rzx0A4kICVZ7wX-hJd3mrczE5NnLud722k5c-XRjIxYGVM9yZBScqE3oh4xbJOe2AvDe";
            return clientToken;
        }

        public string GetUserToken()
        {
            if (userToken == null)
                throw new CSInsideException("인증이 필요합니다.");
            return userToken;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="CSInsideException"></exception>
        public async Task<bool> LoginAsync(string id, string password)
        {
            // HTTP 요청
            string uri = "https://dcid.dcinside.com/join/mobile_app_login.php";
            string jsonString;
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, uri);
                var keyValuePairs = new Dictionary<string, string>();
                keyValuePairs.Add("user_id", id);
                keyValuePairs.Add("user_pw", password);
                request.Content = new FormUrlEncodedContent(keyValuePairs);
                var response = await client.SendAsync(request);
                //response.EnsureSuccessStatusCode();
                jsonString = await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(CSInsideException))
                    throw;
                throw new CSInsideException($"예기치 않은 예외가 발생하였습니다.", e);
            }
            if (string.IsNullOrEmpty(jsonString))
            {
                throw new CSInsideException($"예기치 않은 오류: 서버에서 빈 문자열을 반환하였습니다.");
            }

            // 반환값 처리
            JObject jObject = JToken.Parse(jsonString) is JObject ? JToken.Parse(jsonString) as JObject : (JToken.Parse(jsonString) as JArray)[0] as JObject;
            if ((bool)jObject["result"])
            {
                userToken = (string)jObject["user_id"];
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<string> FetchDateAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "http://json2.dcinside.com/json0/app_check_A_rina.php");
            var response = await client.SendAsync(request);
            string jsonString = await response.Content.ReadAsStringAsync();
            JObject jObject = JToken.Parse(jsonString) is JObject ? JToken.Parse(jsonString) as JObject : (JToken.Parse(jsonString) as JArray)[0] as JObject;
            if (!(bool)jObject["result"])
            {
                throw new Exception((string)jObject["cause"]);
            }
            dateFetchTime = DateTime.Now.Date;
            return (string)jObject["date"];
        }

        private async Task<string> FetchAccessTokenAsync()
        {
            string date = await FetchDateAsync();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://dcid.dcinside.com/join/mobile_app_key_verification_3rd.php");
            var keyValuePairs = new Dictionary<string, string>();
            var value_token = sha256.ComputeHash($"dcArdchk_{date}".ToByteArray(Encoding.ASCII)).ToHexString();
            keyValuePairs.Add("value_token", value_token);
            keyValuePairs.Add("signature", "ReOo4u96nnv8Njd7707KpYiIVYQ3FlcKHDJE046Pg6s=");
            keyValuePairs.Add("vCode", "30350");
            keyValuePairs.Add("vName", "3.9.4");
            keyValuePairs.Add("client_token", GetClientToken());
            request.Content = new FormUrlEncodedContent(keyValuePairs);
            var response = await client.SendAsync(request);
            string jsonString = await response.Content.ReadAsStringAsync();
            JObject jObject = JToken.Parse(jsonString) is JObject ? JToken.Parse(jsonString) as JObject : (JToken.Parse(jsonString) as JArray)[0] as JObject;
            if (!(bool)jObject["result"])
            {
                throw new Exception((string)jObject["cause"]);
            }
            return (string)jObject["app_id"];
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
    }
}
