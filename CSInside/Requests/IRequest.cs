using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSInside
{
    /// <summary>
    /// API 요청을 정의합니다.
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// API 요청을 실행합니다.
        /// </summary>
        public Task ExecuteAsync();
    }
}
