using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CSInside.XUnit
{
    public class PostWriteRequestTests
    {
        static ApiService service;
        static ApiService Service
        {
            get
            {
                if (service == null)
                {
                    AuthTokenProvider authTokenProvider = new AuthTokenProvider();
                    authTokenProvider.GetAccessToken();
                    Task.Delay(5000).Wait();
                    service = new ApiService(authTokenProvider);
                }
                return service;
            }
        }

        [Fact]
        public async void PostWriteRequestTest()
        {
            ParagraphCollection paragraphs = new ParagraphCollection();
            paragraphs.Add(new StringParagraph("1문단 첫번째 줄\r\n1문단 두번째 줄"));
            paragraphs.Add(new StringParagraph("2문단 첫번째 줄\r\n2문단 두번째 줄"));
            PostWriteRequest request = Service.CreatePostWriteRequest(
                "programming",
                DateTime.Now.ToString(),
                "ㅇㅇ",
                "password",
                paragraphs);
            int postNo = await request.ExecuteAsync();
            Assert.True(postNo > 0);
            try
            {
                await Service.CreatePostDeleteRequest("programming", postNo, "password").ExecuteAsync();
            }
            catch
            {

            }
        }

        [Fact]
        public async void DCConPostWriteRequestTest()
        {
            ParagraphCollection paragraphs = new ParagraphCollection();
            DCCon dccon = new DCCon()
            {
                DetailIndex = "1613979",
                Title = "헐",
                ImageUri = "https://dcimg5.dcinside.com/dccon.php?no=62b5df2be09d3ca567b1c5bc12d46b394aa3b1058c6e4d0ca41648b65ceb246e13df9546348593b9b03553cb2b363e94da0bda2f33af133d69a3e3bd02836bc703d2a6",
                PackageIndex = 63214
            };
            paragraphs.Add(new DCConParagraph(dccon));
            paragraphs.Add(new StringParagraph("2문단 첫번째 줄\r\n2문단 두번째 줄"));
            paragraphs.Add(new DCConParagraph(dccon));
            paragraphs.Add(new StringParagraph("4문단 첫번째 줄\r\n4문단 두번째 줄"));
            paragraphs.Add(new DCConParagraph(dccon));
            paragraphs.Add(new StringParagraph("6문단 첫번째 줄\r\n4문단 두번째 줄"));
            PostWriteRequest request = Service.CreatePostWriteRequest(
                "programming",
                DateTime.Now.ToString(),
                "ㅇㅇ",
                "password",
                paragraphs);
            int postNo = await request.ExecuteAsync();
            Assert.True(postNo > 0);

            try
            {
                await Service.CreatePostDeleteRequest("programming", postNo, "password").ExecuteAsync();
            }
            catch
            {

            }
        }
    }
}
