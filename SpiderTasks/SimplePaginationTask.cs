using SpiderFramework;
using SpiderFramework.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SpiderTasks
{
    public abstract class SimplePaginationTask:Task, IDatabase
    {
        protected enum MiningStrategy
        {
            UntilLastPageNum,
            UntilNotAvailable
        }

        protected abstract string ListPageUrl{ get; }
        protected abstract int LastPageNum{get;}
        protected virtual int Step
        {
            get { return 1; }
        }
        protected abstract MiningStrategy Strategy { get; }


        public sealed override void DoStuff()
        {
            using (MongoQueue<TaskMessage> queue = new MongoQueue<TaskMessage>(ConfigurationManager.AppSettings["mongodbqueue"], 1000000))
            {
                if (Strategy == MiningStrategy.UntilLastPageNum)
                {
                    for (int i = 1; i <= LastPageNum; i+=Step)
                    {
                        string url = string.Format(ListPageUrl, i);
                        var msg = CreatePageTask(this.Name, url);
                        queue.Send(msg);
                    }
                }
                else
                {
                    int i = 1;
                    string url = string.Format(ListPageUrl, i);
                    HttpRequestContentExtractor ce = new HttpRequestContentExtractor(HttpMethod.GET, null);
                    while (ce.Goto(url))
                    {
                        var msg = CreatePageTask(this.Name, url);
                        queue.Send(msg);
                        i += Step;
                        url = string.Format(ListPageUrl, i);
                    }
                }
            }
        }

        public abstract TaskMessage CreatePageTask(string name, string url);

		#region IDatabase Members

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

		#endregion
	}
}
