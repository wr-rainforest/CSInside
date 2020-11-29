using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace CSInside.Test
{
    public class ImageRequestTests
    {
        static ApiService service;
        static ApiService Service
        {
            get
            {
                if (service == null)
                    service = new ApiService(new AuthTokenProvider());
                return service;
            }
        }

        // 테스트용 이미지 출처: https://www.mbcsportsplus.com/data/board/attach/2019/10/20191016083311_xzepkrir.png
        // origin: "https://dcimg2.dcinside.co.kr/viewimage.php?id=3dafdf21f7d335ab67b1d1&no=24b0d769e1d32ca73dec84fa11d0283195504478ca9b7677dc322d30cb3c9b4564dec257822f7455f3bb80f28454b4cd8c1e0c7ae0c4c6b369be967302f6122357b6"
        // _s: "https://dcimg2.dcinside.co.kr/viewimage.php?id=3dafdf21f7d335ab67b1d1&no=24b0d769e1d32ca73dec84fa11d0283195504478ca9b7677dc322d30cb3c9b4564dec257822f7455f3bb80f28454b4cd8c1e0c7ae0c4c6b369be967302f6122357b62f2d"
        // _s2: "https://dcimg2.dcinside.co.kr/viewimage.php?id=3dafdf21f7d335ab67b1d1&no=24b0d769e1d32ca73dec84fa11d0283195504478ca9b7677dc322d30cb3c9b4564dec257822f7455f3bb80f28454b4cd8c1e0c7ae0c4c6b369be967302f6122357b62f2db7"
        // web2_: "https://dcimg2.dcinside.co.kr/viewimage.php?id=3dafdf21f7d335ab67b1d1&no=24b0d769e1d32ca73dec84fa11d0283195504478ca9b7677dc322d30cb3c9b4564dec257822f7455f3bb80b4d90fb5a7bc7aa968d535bf27209a91893a24cc9b98bdd5e60c0c4f"
        // filename: 1893501027_d6bcb293.png

        [Fact]
        public async void OriginImageRequestTest()
        {
            ImageRequest request = Service.CreateImageRequest("https://dcimg2.dcinside.co.kr/viewimage.php?id=3dafdf21f7d335ab67b1d1&no=24b0d769e1d32ca73dec84fa11d0283195504478ca9b7677dc322d30cb3c9b4564dec257822f7455f3bb80f28454b4cd8c1e0c7ae0c4c6b369be967302f6122357b6");
            byte[] image = await request.ExecuteAsync(ImageType.Origin);
            Assert.NotNull(image);
            Assert.True(image.Length == 558645);
        }

        [Fact]
        public async void PreviewImageRequestTest()
        {
            ImageRequest request = Service.CreateImageRequest("https://dcimg2.dcinside.co.kr/viewimage.php?id=3dafdf21f7d335ab67b1d1&no=24b0d769e1d32ca73dec84fa11d0283195504478ca9b7677dc322d30cb3c9b4564dec257822f7455f3bb80f28454b4cd8c1e0c7ae0c4c6b369be967302f6122357b6");
            byte[] image = await request.ExecuteAsync(ImageType.Preview);
            Assert.NotNull(image);
            Assert.True(image.Length == 146351);
        }

        [Fact]
        public async void WebImageRequestTest()
        {
            ImageRequest request = Service.CreateImageRequest("https://dcimg2.dcinside.co.kr/viewimage.php?id=3dafdf21f7d335ab67b1d1&no=24b0d769e1d32ca73dec84fa11d0283195504478ca9b7677dc322d30cb3c9b4564dec257822f7455f3bb80f28454b4cd8c1e0c7ae0c4c6b369be967302f6122357b6");
            byte[] image = await request.ExecuteAsync(ImageType.Web);
            Assert.NotNull(image);
            Assert.True(image.Length == 350421);
        }

        [Fact]
        public void GetImageFileNameTest()
        {
            ImageRequest request = Service.CreateImageRequest("https://dcimg2.dcinside.co.kr/viewimage.php?id=3dafdf21f7d335ab67b1d1&no=24b0d769e1d32ca73dec84fa11d0283195504478ca9b7677dc322d30cb3c9b4564dec257822f7455f3bb80f28454b4cd8c1e0c7ae0c4c6b369be967302f6122357b6");
            Assert.True(request.GetImageFileName() == "1893501027_d6bcb293.png");
        }

        [Fact]
        public void GetImageExtensionTest()
        {
            ImageRequest request = Service.CreateImageRequest("https://dcimg2.dcinside.co.kr/viewimage.php?id=3dafdf21f7d335ab67b1d1&no=24b0d769e1d32ca73dec84fa11d0283195504478ca9b7677dc322d30cb3c9b4564dec257822f7455f3bb80f28454b4cd8c1e0c7ae0c4c6b369be967302f6122357b6");
            Assert.True(request.GetImageExtension() == ".png");
        }
    }
}
