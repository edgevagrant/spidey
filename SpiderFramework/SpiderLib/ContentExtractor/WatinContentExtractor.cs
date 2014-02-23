using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WatiN.Core;
using log4net;
using SpiderFramework.Storage;

namespace SpiderFramework
{
    public class WatinContentExtractor : IContentExtractor
    {
        Browser browser;
        String url;
        public WatinContentExtractor(Browser browser)
        {
            this.browser = browser;
        }
        public WatinContentExtractor()
        {

        }
        public ILog Logger
        {
            get;
            set;
        }
        public HtmlContent GetContent(string idOrClassName)
        {
            Div targetDiv = browser.Div(Find.ById(idOrClassName));
            if (!targetDiv.Exists)
            {
                targetDiv = browser.Div(Find.ByClass(idOrClassName));
                if (!targetDiv.Exists)
                    return null;
            }
            HtmlContent doc = new HtmlContent();
            doc.Content = targetDiv.OuterHtml;
            doc.Title = browser.Title;
            doc.Url = url;
            return doc;
        }

        public bool Goto(string url)
        {
            this.url = url;
            browser.GoTo(url);
            browser.WaitForComplete(30);
            return true;
        }


        public object CreateInstance()
        {
            if (this.browser == null)
                this.browser = new IE();
            return this.browser;
        }


        public void DestroyInstance()
        {
            if (this.browser != null)
                this.browser.Close();
        }


        public object Instance
        {
            get { return browser; }
        }
    }
}
