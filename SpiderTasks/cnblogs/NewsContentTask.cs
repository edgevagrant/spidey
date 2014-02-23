using SpiderFramework;
using SpiderFramework.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderTasks.cnblogs
{
    public class NewsContentTask:SingleContentPageTask
    {

        public override bool Save(IHtmlContent htmldoc)
        {
            var docprovider = DocumentStorageProvider.GetInstance(this.DBType, this.ConnectionString);

            CnblogsNewsAdapter adpt = new CnblogsNewsAdapter(htmldoc, this.Url);
            IDocument doc = adpt.GetDocument();

            IFileStorage fs = FileStorageProvider.GetInstance(this.DBType, this.ConnectionString);
            foreach (string url in doc.Attachments)
            {
                if (!fs.Exists(url))
                {
                    logger.InfoFormat("Downloading document: {0}", url);
                    var webStream = WebRequestHelper.SendGetRequestAndAutoDetectContentType(url, null, null);
                    fs.Add(url, webStream);
                }
            }
            doc.LastUpdatedDate = DateTime.Now;
            return docprovider.SaveDocument(doc, true);

        }
    }
}
