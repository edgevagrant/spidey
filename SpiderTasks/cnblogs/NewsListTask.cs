﻿using SpiderFramework.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderTasks.cnblogs
{
    public class NewsListTask:AutoGeneratedListTask
    {
        public override string UrlTemplate
        {
            get { return "http://news.cnblogs.com/n/{0}/"; }
        }

        public override TaskMessage CreatePageTask(string name, string url)
        {
            logger.Info(url);
            TaskMessage msg = new TaskMessage();
            msg.AssemblyName = "SpiderTasks";
            msg.ClassName = "SpiderTasks.cnblogs.NewsContentTask";
            msg.Name = name;
            msg.Category = "cnblogs";

            msg.Parameters.Add("DBType", this.DBType.ToString());
            msg.Parameters.Add("ConnectionString", ConnectionString);

            msg.Parameters.Add("Url", url);
            return msg;
        }
    }
}
