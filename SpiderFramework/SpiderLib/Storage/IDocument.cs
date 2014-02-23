using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderFramework.Storage
{
    public interface IDocument
    {
        string Content { get; set; }
        string Title { get; set; }
        string Url { get; set; }
        DateTime? PublishDate { get; set; }
        DateTime LastUpdatedDate { get; set; }
        List<string> Keywords { get; set; }
        string Author { get; set; }
        List<string> Attachments { get; set; }
        string MD5 { get; set; }
        string SourceUrl { get; set; }
        string SourceName { get; set; }
    }
    public interface IDocumentSource
    {
        string Url { get; set; }
        string Name { get; set; }
    }
}
