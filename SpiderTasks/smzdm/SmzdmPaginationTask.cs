using SpiderFramework;
using SpiderFramework.Queue;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace SpiderTasks.smzdm
{
    public class SmzdmPaginationTask : SimplePaginationTask
    {
        protected override string ListPageUrl
        {
            get { return "http://www.smzdm.com/page/{0}"; }
        }

        //public override string Name
        //{
        //    get { return "什么值得买 挖掘器"; }
        //}

        protected override int LastPageNum
        {
            get { return 20; }
        }

        protected override SimplePaginationTask.MiningStrategy Strategy
        {
            get { return MiningStrategy.UntilLastPageNum; }
        }

        public override TaskMessage CreatePageTask(string name, string url)
        {
            logger.Info(url);
            TaskMessage msg = new TaskMessage();
            msg.AssemblyName = "SpiderTasks";
            msg.ClassName = "SpiderTasks.smzdm.SmzdmListTask";
            msg.Name = name;
            msg.Category = "Smzdm";
            msg.Parameters.Add("DBType", this.DBType.ToString());
            msg.Parameters.Add("Url", url);
            msg.Parameters.Add("ConnectionString", ConnectionString);
            return msg;
        }
    }
}
