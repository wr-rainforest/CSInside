using System;
using System.Collections.Generic;
using System.Text;

namespace CSInside
{
    /// <summary>
    /// 게시글의 디시콘 단락을 나타냅니다.
    /// </summary>
    public class DCConParagraph : Paragraph
    {
        public override object Content { get; set; }

        public DCConParagraph(DCCon dccon)
        {
            Content = dccon;
        }
    }
}
