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
        protected HttpClient Client
        {
            get
            {
                //if (Service == null)
                //    throw new NullReferenceException();
                return Service.Client;
            }
        }

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
        public ApiService Service { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        internal RequestBase(ApiService service)
        {
            Service = service;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Task<TResult> ExecuteAsync();
    }

    public abstract class RequestBase<T, TResult>
    {
        /// <summary>
        /// 
        /// </summary>
        protected HttpClient Client
        {
            get
            {
                //if (Service == null)
                //    throw new NullReferenceException();
                return Service.Client;
            }
        }

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
        public ApiService Service { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        internal RequestBase(ApiService service)
        {
            Service = service;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract Task<TResult> ExecuteAsync(T arg);
    }
}
