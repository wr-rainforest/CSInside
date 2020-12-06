using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CSInside.Readers
{
    internal abstract class Reader<TResult> : IReader<TResult>
    {
        public abstract int Position { get; set; }

        public abstract int Count { get; }

        /// <summary>
        /// 
        /// </summary>
        protected IAuthTokenProvider AuthTokenProvider
        {
            get
            {
                //if (Service == null)
                //    throw new NullReferenceException();
                return Service.AuthTokenProvider;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private ApiService Service { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        internal Reader(ApiService service)
        {
            Service = service;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Task<TResult> ReadAsync();

        protected async Task<JObject> GetResponseAsync(HttpRequestMessage request)
        {
            string responseString;
            try
            {
                var response = await Service.Client.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                    throw new CSInsideException($"API 서버에서 Internal Server Error를 반환하였습니다. 인증 토큰이 만료되었거나 올바르지 않은 인증 토큰일 수 있습니다.");
                responseString = await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(CSInsideException))
                    throw;
                throw new CSInsideException($"예기치 않은 예외가 발생하였습니다.", e);
            }
            if (string.IsNullOrEmpty(responseString))
            {
                throw new CSInsideException($"예기치 않은 오류: 서버에서 빈 문자열을 반환하였습니다.");
            }

            // Json 파싱
            JToken jToken;
            try
            {
                jToken = JToken.Parse(responseString);
            }
            catch (Exception e)
            {
                throw new CSInsideException($"Json 파싱에 실패하였습니다.", e);
            }
            JObject jObject = jToken is JObject ? jToken as JObject : (jToken as JArray)[0] as JObject;

            // 반환값 처리
            return jObject;
        }
    }
}
