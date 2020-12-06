using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CSInside
{
    /// <summary>
    /// 생성, 수정, 삭제와 관련된 API 요청을 정의합니다.
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// API 요청을 실행합니다.
        /// </summary>
        public Task ExecuteAsync();
    }
}
