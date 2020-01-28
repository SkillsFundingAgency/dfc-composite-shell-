using HtmlAgilityPack;
using System;

namespace DFC.Composite.Shell.Services.UrlRewriter
{
    public class RelativeUrlRewriterService : IUrlRewriterService
    {
        public string Rewrite(string content, string requestBaseUrl, string applicationRootUrl, string path)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            var links = doc.DocumentNode.SelectNodes("//a");
            if (links != null)
            {
                foreach (var link in links)
                {
                    var hrefValue = link.GetAttributeValue("href", string.Empty);
                    if (IsRelativeUrl(hrefValue))
                    {
                        link.SetAttributeValue("href", GetUrl(path, hrefValue));
                    }
                }
            }

            var forms = doc.DocumentNode.SelectNodes("//form");
            if (forms != null)
            {
                foreach (var link in forms)
                {
                    var actionValue = link.GetAttributeValue("action", string.Empty);
                    if (IsRelativeUrl(actionValue))
                    {
                        link.SetAttributeValue("action", GetUrl(path, actionValue));
                    }
                }
            }

            content = doc.DocumentNode.OuterHtml;

            return content;
        }

        private string GetUrl(string prefix, string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                if (url.StartsWith("/", StringComparison.InvariantCultureIgnoreCase))
                {
                    prefix = "/" + prefix;
                }
                else
                {
                    prefix = prefix + "/";
                }
            }

            var result = $"{prefix}{url}";
            return result;
        }

        private bool IsRelativeUrl(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.Relative);
        }
    }
}