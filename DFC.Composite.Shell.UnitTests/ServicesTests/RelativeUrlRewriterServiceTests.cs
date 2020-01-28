using DFC.Composite.Shell.Services.UrlRewriter;
using Xunit;

namespace DFC.Composite.Shell.Test.ServicesTests
{
    public class RelativeUrlRewriterServiceTests
    {
        private readonly IUrlRewriterService urlRewriterService;

        public RelativeUrlRewriterServiceTests()
        {
            urlRewriterService = new RelativeUrlRewriterService();
        }

        [Fact]
        public void ShouldRewriteChildApplicationUrls()
        {
            const string shellAppUrl = "http://shell";
            const string childAppUrl = "http://child1";
            var contentPathName = "child1";

            var content = $"<a href='edit/1'></a>";
            var processedContentExpected = $"<a href='{contentPathName}/edit/1'></a>";

            var result = urlRewriterService.Rewrite(content, shellAppUrl, childAppUrl, contentPathName);

            Assert.Equal(processedContentExpected, result);
        }

        [Fact]
        public void ShouldRewriteOnlyRelativeUrlsInChildApplicationUrls()
        {
            const string shellAppUrl = "http://shell";
            const string childAppUrl = "http://child1";
            var contentPathName = "child1";

            var content = $"<a href='/edit/1'></a>, <a href='http://www.google.com'>google</a>";
            var processedContentExpected = $"<a href='/{contentPathName}/edit/1'></a>, <a href='http://www.google.com'>google</a>";

            var result = urlRewriterService.Rewrite(content, shellAppUrl, childAppUrl, contentPathName);

            Assert.Equal(processedContentExpected, result);
        }

        [Fact]
        public void EmptyUrls()
        {
            const string shellAppUrl = "http://shell";
            const string childAppUrl = "http://child1";
            var contentPathName = "child1";

            var content = $"<a href=''>link1</a>";
            var processedContentExpected = $"<a href='{contentPathName}'>link1</a>";

            var result = urlRewriterService.Rewrite(content, shellAppUrl, childAppUrl, contentPathName);

            Assert.Equal(processedContentExpected, result);
        }
    }
}