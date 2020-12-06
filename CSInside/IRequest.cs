using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSInside
{
    /// <summary>
    /// API 요청을 정의합니다.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IRequest<TResult>
    {
        /// <summary>
        /// API 요청을 실행합니다.
        /// </summary>
        /// <returns></returns>
        public Task<TResult> ExecuteAsync();
    }
}
