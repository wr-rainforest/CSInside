using System;
using System.Collections.Generic;
using System.Text;

namespace CSInside
{
    /// <summary>
    /// 디시인사이드 API 인증 토큰 공급자를 정의합니다.
    /// </summary>
    public interface IAuthTokenProvider
    {
        public string GetAccessToken();

        public string GetClientToken();

        public string GetUserToken();
    }
}
