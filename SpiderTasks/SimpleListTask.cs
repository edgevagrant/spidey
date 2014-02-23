using log4net;
using SpiderFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using SpiderFramework.Storage;

namespace SpiderTasks
{
	public abstract class SimpleListTask : Task, IDatabase
    {
        ILog logger = LogManager.GetLogger(typeof(SimpleListTask));
		public abstract string MainContainerClassName { get; }
		public abstract string ListItemContainerClassname { get; }
		public string Url { get; set; }
		public string Referer { get; set; }
		public string DefaultEncodingName { get; set; }

		public string ConnectionString
		{
			get;
			set;
		}

		public DatabaseType DBType
		{
			get;
			set;
		}
        public sealed override void DoStuff()
		{
			HttpRequestContentExtractor hqExtractor = new HttpRequestContentExtractor(HttpMethod.GET, null);
			if (!(hqExtractor.Goto(this.Url, this.Referer, this.DefaultEncodingName)))
			{
				return;
			}
			var classnames = MainContainerClassName.Split(new char[]{'|'});
			HtmlContent htmldesc = null;
			foreach (string classname in classnames)
			{
				htmldesc = hqExtractor.GetContent(classname);
				if (htmldesc != null)
					break;
			}
			if (htmldesc == null)
				logger.Error("class="+MainContainerClassName+" div not found");
			var htmldoc = htmldesc.GetHtmlDocumentObject();
			if (htmldoc != null)
			{
				var divSet = htmldoc.DocumentNode.SelectNodes("//li[@class='"+ListItemContainerClassname+"']");
				foreach (HtmlNode node in divSet)
				{
					HandleListItem(node);
				}
			}
        }

		public abstract void HandleListItem(HtmlNode itemNode);
    }
}
