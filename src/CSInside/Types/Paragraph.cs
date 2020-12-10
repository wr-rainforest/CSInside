using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace CSInside
{
    /// <summary>
    /// 게시글 단락을 나타냅니다.
    /// </summary>
    public abstract class Paragraph
    {
        internal abstract HttpContent GetHttpContent();
    }
}
