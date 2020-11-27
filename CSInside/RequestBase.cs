using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CSInside
{
    public abstract class RequestBase<TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        protected HttpClient Client { get; }

        /// <summary>
        /// 
        /// </summary>
        protected IAuthTokenProvider AuthTokenProvider { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="authTokenProvider"></param>
        internal RequestBase(HttpClient client, IAuthTokenProvider authTokenProvider)
        {
            Client = client;
            AuthTokenProvider = authTokenProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Task<TResult> Execute();
    }

    public abstract class RequestBase<T, TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        protected HttpClient Client { get; }

        /// <summary>
        /// 
        /// </summary>
        protected IAuthTokenProvider AuthTokenProvider { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="authTokenProvider"></param>
        internal RequestBase(HttpClient client, IAuthTokenProvider authTokenProvider)
        {
            Client = client;
            AuthTokenProvider = authTokenProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Task<TResult> Execute(T arg);
    }

    public abstract class RequestBase<T1, T2, TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        protected HttpClient Client { get; }

        /// <summary>
        /// 
        /// </summary>
        protected IAuthTokenProvider AuthTokenProvider { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="authTokenProvider"></param>
        internal RequestBase(HttpClient client, IAuthTokenProvider authTokenProvider)
        {
            Client = client;
            AuthTokenProvider = authTokenProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Task<TResult> Execute(T1 arg1, T2 arg2);
    }
}
