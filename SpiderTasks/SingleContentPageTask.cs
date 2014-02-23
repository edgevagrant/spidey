using SpiderFramework;
using SpiderFramework.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderTasks
{
    public abstract class SingleContentPageTask : Task, IDatabase
    {
        public virtual string Url { get; set; }
        public virtual string DivClassName { get; set; }

        public SingleContentPageTask()
		{
			this.TaskStart += BasicSpiderTask_TaskStart;
			this.TaskCompleted += BasicSpiderTask_TaskCompleted;
		}

		void BasicSpiderTask_TaskCompleted(object sender, EventArgs e)
		{
			logger.Info("Task completed");
		}

		void BasicSpiderTask_TaskStart(object sender, EventArgs e)
		{
			logger.Info("Task started on "+ DateTime.Now.ToString());
		}

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


        public override void DoStuff()
        {
            HttpRequestContentExtractor ce = new HttpRequestContentExtractor(HttpMethod.GET, null);
            if (ce.Goto(this.Url))
            {
                var doc = ce.GetContent(DivClassName);
                logger.Info(this.Url);
                if (!Save(doc))
                {
                    Console.Write("file exists");
                }
            }
        }

        public abstract bool Save(IHtmlContent htmldoc);
    }
}
