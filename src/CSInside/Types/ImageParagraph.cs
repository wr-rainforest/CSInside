using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace CSInside
{
    /// <summary>
    /// 게시글의 이미지 단락을 나타냅니다.
    /// </summary>
    public class ImageParagraph : Paragraph
    {
        #region Property
        public string Extension { get => GetImageExtension(image); }

        private byte[] image;
        public byte[] Image 
        {
            get => image;
            set
            {
                byte[] arr = value;
                if (!(arr.Take(2).SequenceEqual(jpeg) || arr.Take(4).SequenceEqual(png) || arr.Take(3).SequenceEqual(gif)))
                    throw new ArgumentException("이미지 파일이 아닙니다. jpeg, png, gif 파일만 인식 가능합니다.");
                image = arr;
            }
        }
        #endregion

        #region ctor
        public ImageParagraph()
        {

        }

        /// <summary>
        /// <paramref name="image"/>를 사용하여 <seealso cref="ImageParagraph"/>의 새 인스턴스를 초기화합니다. 
        /// </summary>
        /// <param name="image">jpg, png, gif</param>
        public ImageParagraph(byte[] image)
        {
            if (image.Take(2).SequenceEqual(jpeg) || image.Take(4).SequenceEqual(png) || image.Take(3).SequenceEqual(gif)) { }
            else
                throw new ArgumentException("이미지 파일이 아닙니다. jpeg, png, gif 파일만 인식 가능합니다.");
            this.image = image;
        }
        #endregion

        internal override HttpContent GetHttpContent() 
        {
            return new ByteArrayContent(image);
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
