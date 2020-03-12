﻿using DFC.Composite.Shell.HttpResponseMessageHandlers;
using DFC.Composite.Shell.Models.Common;
using DFC.Composite.Shell.Services.CookieParsers;
using DFC.Composite.Shell.Services.HeaderRenamer;
using DFC.Composite.Shell.Services.PathLocator;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using Xunit;

namespace DFC.Composite.Shell.Test.HttpResponseMessageHandlers
{
    public class CookieHttpResponseMessageHandlerTests
    {
        private IPathLocator pathLocator;
        private IHttpContextAccessor httpContextAccessor;
        private ISetCookieParser setCookieParser;
        private CookieHttpResponseMessageHandler cookieHttpResponseMessageHandler;
        private IHeaderRenamerService headerRenamerService;

        public CookieHttpResponseMessageHandlerTests()
        {
            pathLocator = A.Fake<IPathLocator>();
            httpContextAccessor = A.Fake<IHttpContextAccessor>();
            setCookieParser = new SetCookieParser();
            httpContextAccessor.HttpContext = new DefaultHttpContext();
            headerRenamerService = new HeaderRenamerService();

            cookieHttpResponseMessageHandler = new CookieHttpResponseMessageHandler(httpContextAccessor, pathLocator, setCookieParser, headerRenamerService);
        }

        [Fact]
        public void CanCopyCookieValuesFromChildAppToShellWhenCookieHasSingleValue()
        {
            using (var childHttpResponseMessage = new HttpResponseMessage())
            {
                //Arrange
                var path = "path1";
                A.CallTo(() => pathLocator.GetPath()).Returns(path);
                childHttpResponseMessage.Headers.Add(HeaderNames.SetCookie, "v1=v1value");
                childHttpResponseMessage.Headers.Add(HeaderNames.Referer, "Referer1=Referer1Value");

                //Act
                cookieHttpResponseMessageHandler.Process(childHttpResponseMessage);

                //Assert
                var shellResponseHeaders = httpContextAccessor.HttpContext.Response.Headers;
                var setCookieHeader = shellResponseHeaders[HeaderNames.SetCookie];
                Assert.Single(setCookieHeader);
                Assert.StartsWith(path, setCookieHeader[0], StringComparison.OrdinalIgnoreCase);
            }
        }

        [Fact]
        public void CanCopyCookieValuesFromChildAppToShellWhenCookieHasMultipleValues()
        {
            using (var childHttpResponseMessage = new HttpResponseMessage())
            {
                //Arrange
                var path = "path1";
                A.CallTo(() => pathLocator.GetPath()).Returns(path);
                childHttpResponseMessage.Headers.Add(HeaderNames.SetCookie, new List<string>() { "v1=value1", "v2=value2" });
                childHttpResponseMessage.Headers.Add(HeaderNames.Referer, "Referer1=Referer1Value");

                //Act
                cookieHttpResponseMessageHandler.Process(childHttpResponseMessage);

                //Assert
                var shellResponseHeaders = httpContextAccessor.HttpContext.Response.Headers;
                var setCookieHeader = shellResponseHeaders[HeaderNames.SetCookie];
                Assert.Equal(2, setCookieHeader.Count);
                Assert.StartsWith(path, setCookieHeader[0], StringComparison.OrdinalIgnoreCase);
            }
        }

        [Fact]
        public void CanCopyCookieValuesCorrectly()
        {
            using (var childHttpResponseMessage = new HttpResponseMessage())
            {
                //Arrange
                var path = "path1";
                A.CallTo(() => pathLocator.GetPath()).Returns(path);
                childHttpResponseMessage.Headers.Add(HeaderNames.SetCookie, new List<string>() { "v1=value1", "v2=value2" });
                childHttpResponseMessage.Headers.Add(HeaderNames.Referer, "Referer1=Referer1Value");

                //Act
                cookieHttpResponseMessageHandler.Process(childHttpResponseMessage);

                //Assert
                var shellResponseHeaders = httpContextAccessor.HttpContext.Response.Headers;
                var setCookieHeader = shellResponseHeaders[HeaderNames.SetCookie];
                Assert.Equal(2, setCookieHeader.Count);
                Assert.Contains("path1v1=value1", setCookieHeader[0], StringComparison.OrdinalIgnoreCase);
                Assert.Contains("path1v2=value2", setCookieHeader[1], StringComparison.OrdinalIgnoreCase);
            }
        }

        [Fact]
        public void CookieValuesAreCopiedToHttpContextItems()
        {
            using (var childHttpResponseMessage = new HttpResponseMessage())
            {
                //Arrange
                var path = "path1";
                A.CallTo(() => pathLocator.GetPath()).Returns(path);
                childHttpResponseMessage.Headers.Add(HeaderNames.SetCookie, new List<string>() { "v1=value1", "v2=value2" });
                childHttpResponseMessage.Headers.Add(HeaderNames.Referer, "Referer1=Referer1Value");

                //Act
                cookieHttpResponseMessageHandler.Process(childHttpResponseMessage);

                //Assert
                var shellContextItems = httpContextAccessor.HttpContext.Items;
                Assert.Equal(2, shellContextItems.Count);
                Assert.Contains(shellContextItems, x => x.Key.ToString() == "path1v1");
                Assert.Contains(shellContextItems, x => x.Key.ToString() == "path1v2");
            }
        }

        [Fact]
        public void HeaderWithNameDfcSessionIsNotPrefixedWithPath()
        {
            using (var childHttpResponseMessage = new HttpResponseMessage())
            {
                //Arrange
                var path = "path1";
                A.CallTo(() => pathLocator.GetPath()).Returns(path);
                childHttpResponseMessage.Headers.Add(HeaderNames.SetCookie, new List<string>() { $"{Constants.DfcSession}=value1", "v2=value2" });
                childHttpResponseMessage.Headers.Add(HeaderNames.Referer, "Referer1=Referer1Value");

                //Act
                cookieHttpResponseMessageHandler.Process(childHttpResponseMessage);

                //Assert
                var shellResponseHeaders = httpContextAccessor.HttpContext.Response.Headers;
                var setCookieHeader = shellResponseHeaders[HeaderNames.SetCookie];
                Assert.Equal(2, setCookieHeader.Count);
                Assert.StartsWith($"{Constants.DfcSession}=value1", setCookieHeader[0], StringComparison.OrdinalIgnoreCase);
                Assert.StartsWith($"{path}v2=value2", setCookieHeader[1], StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
