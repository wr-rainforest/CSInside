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
        /// <summary>
        /// 디시콘 정보를 가져오거나 설정합니다.
        /// </summary>
        public DCCon DCCon { get; set; }

        #region ctor
        /// <summary>
        /// <seealso cref="DCConParagraph"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        public DCConParagraph()
        {

        }

        /// <summary>
        /// <paramref name="dccon"/>을 사용하여 <seealso cref="DCConParagraph"/> 클래스의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="dccon"></param>
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
