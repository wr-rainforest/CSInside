using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSInside
{
    public class Paragraph
    {
        public ParagraphType Type { get; set; }

        public object Content { get; set; }

        private readonly byte[] jpeg = new byte[] { 0xFF, 0xD8 };

        private readonly byte[] png = new byte[] { 0x89, 0x50, 0x4E, 0x47 };

        private readonly byte[] gif = new byte[] { 0x47, 0x49, 0x46 };

        public Paragraph(byte[] image)
        {
            if (image.Take(2).SequenceEqual(jpeg) || image.Take(4).SequenceEqual(png) || image.Take(3).SequenceEqual(gif)) { }
            else
                throw new ArgumentException("이미지 파일이 아닙니다. jpeg, png, gif 파일만 인식 가능합니다.");
            Type = ParagraphType.Image;
            Content = image;
        }

        public Paragraph(string text)
        {
            if (text == null)
                throw new ArgumentNullException();
            Type = ParagraphType.Text;
            Content = text;
        }


        public Paragraph(DCCon dccon)
        {
            Type = ParagraphType.DCCon;
            Content = dccon;
        }
    }
}
