using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using MongoDB.Bson;

namespace SpiderFramework.Storage
{
    public class HtmlContent:IHtmlContent
    {
        public HtmlDocument GetHtmlDocumentObject()
        {
            HtmlDocument doc = new HtmlDocument();
            if (this.Content != null)
                doc.LoadHtml(this.Content);
            return doc;
        }

		public DateTime LastUpdatedDate
		{
			get;
			set;
		}

        public string Content
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }
    }
}
