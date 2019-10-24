﻿using DFC.Composite.Shell.Integration.Test.Framework;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DFC.Composite.Shell.Integration.Test
{
    public class ApplicationRegionOfflineTests : IClassFixture<ShellTestWebApplicationFactory<Startup>>
    {
        private readonly ShellTestWebApplicationFactory<Startup> factory;

        public ApplicationRegionOfflineTests(ShellTestWebApplicationFactory<Startup> shellTestWebApplicationFactory)
        {
            factory = shellTestWebApplicationFactory;
        }

        [Fact]
        public async Task WhenAnRegionIsOfflineContentIncludesTheRegionsOfflineHtml()
        {
            var shellUrl = "path1";
            var client = factory.CreateClientWithWebHostBuilder();

            var response = await client.GetAsync(shellUrl).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            Assert.Contains("path1 region bodyfooter is offline", responseHtml, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task WhenAnRegionIsOfflineAndOtherRegionsAreOnlineContentIncludesOfflineRegionHtmlAndContentFromOnlineRegions()
        {
            var shellUrl = "path1";
            var client = factory.CreateClientWithWebHostBuilder();

            var response = await client.GetAsync(shellUrl).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            Assert.Contains("path1 region bodyfooter is offline", responseHtml, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("GET, http://www.path1.com/path1/head, path1, Head", responseHtml, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("GET, http://www.path1.com/path1/breadcrumb, path1, Breadcrumb", responseHtml, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("GET, http://www.path1.com/path1/body, path1, Body", responseHtml, StringComparison.OrdinalIgnoreCase);
        }
    }
}