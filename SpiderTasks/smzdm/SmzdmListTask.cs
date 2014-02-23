using HtmlAgilityPack;
using MongoDB.Bson;
using SpiderFramework;
using SpiderFramework.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderTasks.smzdm
{
    public class SmzdmListTask : SimpleListTask
    {
		public SmzdmListTask()
		{
			this.Referer = "http://www.smzdm.com/";
		}


		public override string MainContainerClassName
		{
			get { return "all"; }
		}

		public override string ListItemContainerClassname
		{
			get { return "perContentBox"; }
		}

		public override void HandleListItem(HtmlNode node)
		{
            HtmlNode titleNode = node.SelectSingleNode("h2[@class='con_title noBg']");
            string title = titleNode.InnerText.Replace("&nbsp;", " ").Trim();
            HtmlNode priceNode = titleNode.SelectSingleNode("a/span");
            string price = null;
            if (priceNode != null)
            {
                price = priceNode.InnerText.Replace("&nbsp;", " ").Trim();
            }
            HtmlNode timeNode = node.SelectSingleNode("div[@class='dateTime']");
            string time = timeNode.InnerText;
            HtmlNode contentNode = node.SelectSingleNode("div[@class='conCenterBlock']");
            string content = contentNode.InnerText.Replace("展开全文", "").Replace("向上收起", "").Replace("&nbsp;", " ").Trim();
            HtmlNode zhidaNode = node.SelectSingleNode(".//div[@class='bugBlock' or @class='bugBlock_more']/a");
            string href = null;
            if (zhidaNode == null)
                return;
            href = zhidaNode.Attributes["href"].Value;

            logger.InfoFormat("{0}-{1}-{2}", time, title, href);

            CouponEntity ce = new CouponEntity();
            ce.Id = ObjectId.GenerateNewId();
            if (time.Contains("2013-") || time.Contains("2012-") || time.Contains("2011-"))
            {
                ce.Time = DateTime.Parse(time);
            }
            else
            {
                ce.Time = DateTime.Parse(DateTime.Now.Year+"-"+ time);
            }
            ce.Title = title;
            ce.Price = price;
            ce.Content = content;
            ce.TargetUrl = href;
            ce.Url = this.Url;
            using (MongoDBEntityStorage<CouponEntity> store = new MongoDBEntityStorage<CouponEntity>(this.ConnectionString, "smzdm", "Title"))
            {
                if (!store.Save(ce))
                {
                    logger.WarnFormat("[Data Exists] {0}", title as string);
                }
                else
                {
                    logger.InfoFormat("[Mine Success] {0}", title as string);
                }
            }
		}
	}
}
