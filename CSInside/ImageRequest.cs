using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CSInside
{
    public class ImageRequest : RequestBase<ImageType, byte[]>
    {
        private readonly static MD5 md5 = MD5.Create();

        private readonly string baseUri = "http://image.dcinside.com";

        private string originUri;

        private string directory;

        private string fileName;

        private string extension;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageUri"></param>
        /// <param name="client"></param>
        /// <param name="authTokenProvider"></param>
        /// <exception cref="ArgumentException"></exception>
        internal ImageRequest(string imageUri, ApiService service) : base(service)
        {
            if(!Uri.TryCreate(imageUri, UriKind.Absolute, out Uri uri))
            {
                throw new ArgumentException("Uri 파싱에 실패하였습니다.");
            }
            string param = HttpUtility.ParseQueryString(uri.Query).Get("no");
            if(string.IsNullOrEmpty(param))
            {
                throw new ArgumentException("Uri 분석에 실패하였습니다. ('no' 매개변수가 Uri에 존재하지 않습니다.)");
            }
            string plainText = DecryptQueryString(param);
            fileName = Path.GetFileName(plainText);
            directory = plainText.Replace("/" + fileName, string.Empty);
            extension = Path.GetExtension(plainText);
            if (fileName.Contains("web_"))
            {
                fileName = fileName.Replace("web_", "");
            }
            else if (fileName.Contains("web2_"))
            {
                fileName = fileName.Replace("web2_", "");
            }
            else if (extension.Contains("_s"))
            {
                extension = extension.Replace("_s", string.Empty);
                fileName = Path.ChangeExtension(fileName, extension);
            }
            else if (extension.Contains("_s2"))
            {
                extension = extension.Replace("_s2", string.Empty);
                fileName = Path.ChangeExtension(fileName, extension);
            }
            originUri = imageUri;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApiRequestException"></exception>
        public override async Task<byte[]> Execute(ImageType imageType = ImageType.Origin)
        {
            string imageUri;
            switch (imageType)
            {
                case ImageType.Origin:
                    imageUri = $"{baseUri}/{directory}/{fileName}";
                    break;
                case ImageType.S:
                    imageUri = $"{baseUri}/{directory}/{fileName}_s";
                    break;
                case ImageType.Preview:
                    imageUri = $"{baseUri}/{directory}/{fileName}_s2";
                    break;
                case ImageType.Web:
                    imageUri = $"{baseUri}/{directory}/web2_{fileName}";
                    break;
                default:
                    throw new Exception();
            }
            var request = new HttpRequestMessage(HttpMethod.Get, imageUri);
            var response =await Client.SendAsync(request);
            if(response.StatusCode == HttpStatusCode.NotFound && imageType == ImageType.Web)
            {
                imageUri = $"{baseUri}/{directory}/web_{fileName}";
                request = new HttpRequestMessage(HttpMethod.Get, imageUri);
                request.Headers.Add("Referer", "https://dcinside.com");
                response = await Client.SendAsync(request);
            }
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new ApiRequestException(404, $"서버에서 이미지를 찾을 수 없습니다. ({imageUri})");
            }
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                throw new ApiRequestException((int)response.StatusCode, "알 수 없는 오류", e);
            }
            return await response.Content.ReadAsByteArrayAsync();
            
        }

        public string GetImageExtension()
        {
            return extension;
        }

        private static string DecryptQueryString(string hexString)
        {
            byte[] encrypted = Enumerable.Range(0, hexString.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                .ToArray();
            byte[] iv = { 0x64, 0x63, 0x69, 0x6e, 0x73, 0x69, 0x64, 0x65 };
            byte[] key = { 0x4d, 0xdd, 0xb0, 0x46, 0x85, 0xb2, 0x58, 0xc6, 0x0e, 0xdf, 0xb6, 0xd5, 0x76, 0xb1, 0x44, 0x5d };
            int blockSize = 16;
            var list = new List<byte>();
            encrypted.Select((value, index) => new { value, index })
                .GroupBy(item => item.index / blockSize)
                .Select(block => block.Select(item => item.value).ToList())
                .ToList()
                .ForEach(bytes =>
                {
                    var block = bytes.Select((b, j) => (byte)(b ^ key[j]));
                    list.AddRange(block);
                    key = md5.ComputeHash(iv.Concat(key).ToArray().Concat(block).ToArray());
                });
            return Encoding.ASCII.GetString(list.ToArray());
        }
    }

    public enum ImageType
    {
        Origin = 0,
        S = 1,
        Preview = 2,
        Web = 3
    }
}
