using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using MongoDB.Bson.Serialization.Attributes;

namespace SpiderFramework.Storage
{
    public interface IHtmlContent
    {
		HtmlDocument GetHtmlDocumentObject();
        string Content { get; set; }
        string Title{ get; set; }
        string Url { get; set; }
		DateTime LastUpdatedDate { get; set; }
    }
}
