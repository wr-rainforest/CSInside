using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web;

namespace CSInside
{
    /// <summary>
    /// 게시글의 문자열 단락을 나타냅니다.
    /// </summary>
    public class StringParagraph : Paragraph
    {
        public string Text { get; set; }

        #region ctor
        public StringParagraph()
        {

        }

        /// <summary>
        /// <seealso cref="StringParagraph"/>의 새 인스턴스를 초기화합니다.
        /// </summary>
        /// <param name="text"></param>
        public StringParagraph(string text)
        {
            if (text == null)
                throw new ArgumentNullException();
            Text = text;
        }
        #endregion

        internal override HttpContent GetHttpContent()
        {
            return new StringContent($"<div>{HttpUtility.HtmlEncode(Text)}</div>");
        }
    }
}
