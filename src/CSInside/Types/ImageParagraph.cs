using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSInside
{
    public class ImageParagraph : Paragraph
    {
        public string Extension { get => GetImageExtension((byte[])Content); }

        private byte[] image;

        public override object Content
        { 
            get => image;
            set 
            {
                byte[] arr = (byte[])value;
                if (!(arr.Take(2).SequenceEqual(jpeg) || arr.Take(4).SequenceEqual(png) || arr.Take(3).SequenceEqual(gif)))
                    throw new ArgumentException("이미지 파일이 아닙니다. jpeg, png, gif 파일만 인식 가능합니다.");
                image = arr;
            }
        }

        public ImageParagraph(byte[] image)
        {
            if (image.Take(2).SequenceEqual(jpeg) || image.Take(4).SequenceEqual(png) || image.Take(3).SequenceEqual(gif)) { }
            else
                throw new ArgumentException("이미지 파일이 아닙니다. jpeg, png, gif 파일만 인식 가능합니다.");
            this.image = image;
        }

        #region private string GetImageExtension(byte[] image)
        private readonly byte[] jpeg = new byte[] { 0xFF, 0xD8 };

        private readonly byte[] png = new byte[] { 0x89, 0x50, 0x4E, 0x47 };

        private readonly byte[] gif = new byte[] { 0x47, 0x49, 0x46 };

        private string GetImageExtension(byte[] image)
        {
            if (image.Take(2).SequenceEqual(jpeg))
                return ".jpg";
            else if (image.Take(4).SequenceEqual(png))
                return ".png";
            else if (image.Take(3).SequenceEqual(gif))
                return ".gif";
            throw new Exception("err");
        }
        #endregion
    }
}
