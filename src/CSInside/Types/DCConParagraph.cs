using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace CSInside
{
    /// <summary>
    /// 게시글의 디시콘 단락을 나타냅니다.
    /// </summary>
    public class DCConParagraph : Paragraph
    {
        public DCCon DCCon { get; set; }

        #region ctor
        public DCConParagraph()
        {

        }

        public DCConParagraph(DCCon dccon)
        {
            DCCon = dccon;
        }
        #endregion

        internal override HttpContent GetHttpContent()
        {
            string imgTag =
                $"<img src='{DCCon.ImageUri}'" +
                $" class='written_dccon'" +
                $" alt='{DCCon.Title}'" +
                $" conalt='{DCCon.Title}'" +
                $" title='{DCCon.Title}'>";
            return new StringContent(imgTag);
        }

    }
}
