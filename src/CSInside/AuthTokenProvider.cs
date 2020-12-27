using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CSInside.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CSInside
{
    public class AuthTokenProvider : IAuthTokenProvider, IDisposable
    {
        private readonly HttpClient _client;

        private readonly SHA256Managed _sha256 = new SHA256Managed();

        private readonly Random _random = new Random();

        private string _accessToken;

        private string _clientToken;
#nullable enable
        private string? _userToken = null;
#nullable restore
        [JsonProperty("app_id")]
        private string _app_id { get => GetAccessToken(); set => _accessToken = value; }

        private DateTime _appid_fetched;
        [JsonProperty("app_id_fetched")]
        private DateTime AppIdFetchedTime { get { GetAccessToken(); return _appid_fetched; } set => _appid_fetched = value; }

        [JsonProperty("client_token")]
        private string ClientToken { get => GetClientToken(); set => _clientToken = value; }

        [JsonProperty("user_id_fetched")]
        private DateTime UserIdFetchedTime { get; set; }
        [JsonProperty("user_id")]
        private string UserToken { get => _userToken; set => _userToken = value; }

        #region ctor
        public AuthTokenProvider()
        {
            var handler = new SocketsHttpHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.GZip
            };
            _client = new HttpClient(handler);
            _client.DefaultRequestHeaders.Add("User-Agent", "dcinside.app");
            _client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
        }
        #endregion

        #region IAuthTokenProvider
        public string GetAccessToken()
        {
            if (string.IsNullOrEmpty(_accessToken) || (DateTime.Now - _appid_fetched).TotalSeconds >= 43200)
            {
                _accessToken = FetchAccessTokenAsync().Result;
            }
            return _accessToken;
        }

        public string GetClientToken()
        {
            if(_clientToken == null)
                _clientToken = $"{RandomString(22)}:APA91bFMI-0d1b0wJmlIWoDPVa_V5Nv0OWnAefN7fGLegy6D76TN_CRo5RSUO-6V7Wnq44t7Rzx0A4kICVZ7wX-hJd3mrczE5NnLud722k5c-XRjIxYGVM9yZBScqE3oh4xbJOe2AvDe";
            return _clientToken;
        }

        public string GetUserToken()
        {
            if (_userToken == null)
                throw new CSInsideException("인증이 필요합니다.");
            return _userToken;
        }
        #endregion

        public async Task LoginAsync(string id, string password)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://dcid.dcinside.com/join/mobile_app_login.php");
            var keyValuePairs = new Dictionary<string, string>()
            {
                {"user_id", id },
                {"user_pw", password }
            };
            request.Content = new FormUrlEncodedContent(keyValuePairs);
            var response = await _client.SendAsync(request);
            JToken jToken = JToken.Parse(await response.Content.ReadAsStringAsync());
            JObject jObject = jToken is JObject ? jToken as JObject : (jToken as JArray)[0] as JObject;
            if ((bool)jObject["result"])
            {
                _userToken = (string)jObject["user_id"];
                return;
            }
            throw new CSInsideException(jObject.ToString(Formatting.None));
        }

        private async Task<string> FetchAccessTokenAsync()
        {
            JToken dateJToken = JToken.Parse(await _client.GetStringAsync("http://json2.dcinside.com/json0/app_check_A_rina.php"));
            JObject dateJObject = dateJToken is JObject ? dateJToken as JObject : (dateJToken as JArray)[0] as JObject;
            if (!(bool)dateJObject["result"])
                throw new CSInsideException(dateJObject.ToString(Formatting.None));
            _appid_fetched = DateTime.Now;

            var request = new HttpRequestMessage(HttpMethod.Post, "https://dcid.dcinside.com/join/mobile_app_key_verification_3rd.php");
            var keyValuePairs = new Dictionary<string, string>()
            {
                {"value_token", _sha256.ComputeHash($"dcArdchk_{(string)dateJObject["date"]}".ToByteArray(Encoding.ASCII)).ToHexString() },
                {"signature", "ReOo4u96nnv8Njd7707KpYiIVYQ3FlcKHDJE046Pg6s=" },
                {"vCode", "30403"},
                {"vName", "4.0.6"},
                {"client_token", GetClientToken()}
            };
            request.Content = new FormUrlEncodedContent(keyValuePairs);
            var response = await _client.SendAsync(request);
            JToken jToken = JToken.Parse(await response.Content.ReadAsStringAsync());
            JObject jObject = jToken is JObject ? jToken as JObject : (jToken as JArray)[0] as JObject;
            if (!(bool)jObject["result"])
                throw new CSInsideException(jObject.ToString(Formatting.None));
            return (string)jObject["app_id"];
        }

        private string RandomString(int length)
        {
            const string chars = "-0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxy";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        #region IDisposable
        public void Dispose()
        {
            _client.Dispose();
            _sha256.Dispose();
        }
        #endregion
    }
}
