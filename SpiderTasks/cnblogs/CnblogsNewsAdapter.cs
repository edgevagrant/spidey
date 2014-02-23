using SpiderFramework;
using SpiderFramework.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderTasks.cnblogs
{
    public class CnblogsNewsAdapter : IDocumentAdapter
    {
        IHtmlContent htmlContent = null;
        string url = null;
        public CnblogsNewsAdapter(IHtmlContent content, string url)
        {
            this.htmlContent = content;
            this.url = url;

        }



        public IDocument GetDocument()
        {

            if (this.htmlContent == null)
                return null;

            Document doc = new Document();
            doc.Url = htmlContent.Url;

            var htmldoc = htmlContent.GetHtmlDocumentObject();
            doc.Title = htmldoc.GetElementbyId("news_title").InnerText;
            doc.Content = htmldoc.GetElementbyId("news_body").InnerHtml;

            var n1= htmldoc.GetElementbyId("come_from");
            if(n1.ChildNodes.Count>=2)
            {
                doc.SourceUrl=n1.ChildNodes[1].Attributes["href"].Value;
                doc.SourceName = n1.ChildNodes[1].InnerText;
            }
            var n2 = htmldoc.GetElementbyId("news_info");
            var nPoster= n2.SelectSingleNode("span[@class='news_poster']/a");
            doc.Author = nPoster.InnerText;
            var nPublishDate = n2.SelectSingleNode("span[@class='time']");
            doc.PublishDate = DateTime.Parse(nPublishDate.InnerText.Replace("发布于","").Trim());

            doc.Url = url;
            
            return doc;
        }
    }
}
