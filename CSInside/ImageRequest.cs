using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace CSInside
{
    public class ImageRequest : RequestBase<ImageType, byte[]>
    {
        private readonly static MD5 md5 = MD5.Create();

        private readonly static Regex regex = new Regex(@"^[A-Fa-f0-9]*$");

        private readonly static string baseUri = "http://image.dcinside.com";

        private readonly string initUri;

        private readonly string directory;

        private readonly string fileName;
#nullable enable
        private readonly string? prefix;
#nullable restore
        private readonly string extension;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageUri"></param>
        /// <param name="service"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        internal ImageRequest(string imageUri, ApiService service) : base(service)
        {
            //매개변수 검사
            if (imageUri is null)
                throw new ArgumentNullException(nameof(imageUri));
            if (!Uri.TryCreate(imageUri, UriKind.Absolute, out Uri uri))
                throw new ArgumentException("imageUri 파싱에 실패하였습니다.", nameof(imageUri));
            string param = HttpUtility.ParseQueryString(uri.Query).Get("no");
            if (string.IsNullOrEmpty(param))
                throw new ArgumentException("'no' 매개변수가 imageUri에 존재하지 않습니다.", nameof(imageUri));
            if (!regex.IsMatch(param))
                throw new ArgumentException("imageUri의 'no' 매개변수는 Hex String이어야 합니다.", nameof(imageUri));

            //필드 초기화
            string plainText = DecryptQueryString(param);
            fileName = Path.GetFileName(plainText);
            directory = plainText.Replace("/" + fileName, string.Empty);
            extension = Path.GetExtension(plainText);
            if (fileName.StartsWith("web_"))
            {
                prefix = "web_";
                fileName = fileName.Replace("web_", string.Empty);
            }
            else if (fileName.StartsWith("web2_"))
            {
                prefix = "web2_";
                fileName = fileName.Replace("web2_", string.Empty);
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
            initUri = imageUri;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="CSInsideException"></exception>
#nullable enable
        public override async Task<byte[]?> ExecuteAsync(ImageType imageType = ImageType.Origin)
#nullable restore
        {
            //imageUri 조합
#pragma warning disable CS8509 // switch 식에서 입력 형식의 가능한 값을 모두 처리하지는 않습니다(전체 아님).
            string imageUri = imageType switch
#pragma warning restore CS8509 // switch 식에서 입력 형식의 가능한 값을 모두 처리하지는 않습니다(전체 아님).
            {
                ImageType.Origin => $"{baseUri}/{directory}/{fileName}",
                ImageType.S => $"{baseUri}/{directory}/{fileName}_s",
                ImageType.Preview => $"{baseUri}/{directory}/{fileName}_s2",
                ImageType.Web => $"{baseUri}/{directory}/{(string.IsNullOrEmpty(prefix) ? "web2_" : prefix)}{fileName}",
            };

            //HTTP 요청
            byte[] image = null;
            //- imageType == ImageType.Web && prefix == null
            //  - 1st. $"{baseUri}/{directory}/web2_{fileName}
            //  - 2nd. $"{baseUri}/{directory}/web_{fileName}
            //  - 3rd. $"{baseUri}/{directory}/{fileName}
            //  - 4th  return null

            //- imageType == ImageType.Web && prefix == web_
            //  - 1st. $"{baseUri}/{directory}/web_{fileName}
            //  - 2nd. 
            //  - 3rd.
            //  - 4th.  return null

            //- imageType == ImageType.Web && prefix == web2_
            //  - 1st. $"{baseUri}/{directory}/web2_{fileName}
            //  - 2nd.
            //  - 3rd.
            //  - 4th  return null
            try
            {
                //1st
                var response = await Client.GetAsync(imageUri);
                //2nd
                if (response.StatusCode == HttpStatusCode.NotFound && imageType == ImageType.Web && string.IsNullOrEmpty(prefix))
                {
                    imageUri = $"{baseUri}/{directory}/web_{fileName}";
                    response = await Client.GetAsync(imageUri);
                }
                //3rd
                if (response.StatusCode == HttpStatusCode.NotFound && imageType == ImageType.Web && string.IsNullOrEmpty(prefix))
                {
                    imageUri = $"{baseUri}/{directory}/{fileName}";
                    response = await Client.GetAsync(imageUri);
                }
                //4th
                if (response.StatusCode != HttpStatusCode.NotFound)
                {
                    //404에는 null값 반환, 다른 상태코드는 throw
                    response.EnsureSuccessStatusCode();
                }
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(CSInsideException))
                    throw;
                throw new CSInsideException($"예기치 않은 예외가 발생하였습니다.", e);
            }
            return image;
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
