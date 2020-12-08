using System;
using System.Collections.Generic;
using System.Text;

namespace CSInside
{
    public class StringParagraph : Paragraph
    {
        public override object Content { get; set; }

        public StringParagraph(string text)
        {
            if (text == null)
                throw new ArgumentNullException();
            Content = text;
        }

    }
}
