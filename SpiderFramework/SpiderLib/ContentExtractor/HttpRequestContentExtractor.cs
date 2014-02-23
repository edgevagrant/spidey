using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Net;
using SpiderFramework.Storage;
using log4net;

namespace SpiderFramework
{
    public class HttpRequestContentExtractor : IContentExtractor
    {
        HttpMethod httpMethod = HttpMethod.GET;
        string postData = null;
        String url;
        public HttpRequestContentExtractor(HttpMethod httpMethod, string postData)
        {
            this.httpMethod = httpMethod;
            this.postData = postData;
        }

        public ILog Logger
        {
            get;
            set;
        }

        string content;
        public HtmlContent GetContent(string xpath)
        {
            if (this.url == null || content == null)
                throw new InvalidOperationException("call IContentExtractor.GotoPage first");
            HtmlContent doc = new HtmlContent();
            doc.LastUpdatedDate = DateTime.Now;
            HtmlDocument htmldoc = new HtmlDocument();
            htmldoc.LoadHtml(content);
            HtmlAgilityPack.HtmlNode titleNode = htmldoc.DocumentNode.SelectSingleNode("//title");
            if(titleNode!=null)
                doc.Title = titleNode.InnerText;
            if (xpath.ToLower() == "all")
            {
                doc.Content = htmldoc.DocumentNode.OuterHtml;
            }
            else if (!xpath.StartsWith("/"))
            {
                HtmlAgilityPack.HtmlNodeCollection divs = htmldoc.DocumentNode.SelectNodes("//div");
				if (divs == null)
					return null;
                HtmlNode targetNode = divs.FirstOrDefault(a => a.Id == xpath || (a.Attributes["class"] != null && a.Attributes["class"].Value == xpath));
                if (targetNode == null)
                {
                    if (Logger != null)
                        Logger.WarnFormat("[Mine Fails] '{1}' container not found - {0}", url, xpath);
                    return null;
                }
                doc.Content = targetNode.OuterHtml;
            }
            else //starts with '//'
            {
                var targetNode = htmldoc.DocumentNode.SelectSingleNode(xpath);
                if (targetNode == null)
                {
                    if (Logger != null)
                        Logger.WarnFormat("[Mine Fails] '{1}' container not found - {0}", url, xpath);
                    return null;
                }
                doc.Content = targetNode.OuterHtml;
            }
            doc.Url = url;
            return doc;
        }

        public CookieContainer Cookies
        {
            get;
            set;
        }

        public bool Goto(string url)
        {

            try
            {
                this.url = url;
                if (httpMethod == HttpMethod.GET)
                    this.content = WebRequestHelper.SendGetRequest(url, Cookies);
                else
                    this.content = WebRequestHelper.SendPostRequest(url, Cookies, postData);
                return true;
            }
            catch (Exception e)
            {
                if (Logger != null)
                {
                    if (e.Message.Contains("404"))
                        Logger.ErrorFormat("[Mine Fail][Error404] {0}页面找不到", url);
                    else if(e.Message.Contains("403"))
                        Logger.ErrorFormat("[Mine Fail][Error403] {0}页面禁止访问或spider被屏蔽", url);
                    else
                        Logger.ErrorFormat("[Mine Fail][Unknown] {0}", url);
                }
                return false;
            }

        }
		public bool Goto(string url, string referer)
		{
			return this.Goto(url, referer, null);
		}
		public bool Goto(string url, string referer, string defaultEncodingName)
        {
            try
            {
                this.url = url;
                if (httpMethod == HttpMethod.GET)
					this.content = WebRequestHelper.SendGetRequest(url, referer, Cookies, defaultEncodingName);
                else
					this.content = WebRequestHelper.SendPostRequest(url, referer, Cookies, postData, defaultEncodingName);
                return true;
            }
            catch (Exception e)
            {
                if (Logger != null)
                {
                    if (e.Message.Contains("404"))
                        Logger.ErrorFormat("[Mine Fail][Error404] {0}", url);
                    else if (e.Message.Contains("403"))
                        Logger.ErrorFormat("[Mine Fail][Error403] {0}页面禁止访问或spider被屏蔽", url);
                    else
                        Logger.ErrorFormat("[Mine Fail][Unknown] {0}", url);
                }
                return false;
            }
        }

        public object CreateInstance()
        {
            return null;
        }


        public void DestroyInstance()
        {

        }


        public object Instance
        {
            get { throw new NotImplementedException(); }
        }
    }
}
