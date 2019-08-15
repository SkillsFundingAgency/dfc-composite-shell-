﻿using DFC.Composite.Shell.Integration.Test.Framework;
using System.Threading.Tasks;
using Xunit;

namespace DFC.Composite.Shell.Integration.Test.Tests
{
    public class RobotsTests : IClassFixture<ShellTestWebApplicationFactory<Startup>>
    {
        private readonly ShellTestWebApplicationFactory<Startup> _factory;

        public RobotsTests(ShellTestWebApplicationFactory<Startup> shellTestWebApplicationFactory)
        {
            _factory = shellTestWebApplicationFactory;
        }

        [Fact]
        public async Task Robots_ReturnsContent()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/robots.txt");

            response.EnsureSuccessStatusCode();
            var responseHtml = await response.Content.ReadAsStringAsync();
            Assert.True(responseHtml.Contains("User-agent:") || responseHtml.Contains("Disallow:"));
        }
    }
}
