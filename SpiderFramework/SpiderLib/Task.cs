using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;

namespace SpiderFramework
{
	public abstract class Task:ITask
	{
		protected ILog logger = LogManager.GetLogger(typeof(Task));

		#region ITask Members

		public string Name
		{
			get;
			set;
		}

		public string Category
		{
			get;
			set;
		}

		public TaskStatus Status
		{
			get;
			set;
		}

		public DateTime? StartTime
		{
			get;
			set;
		}

		public DateTime? EndTime
		{
			get;
			set;
		}

		public event EventHandler TaskCompleted;

		public event EventHandler TaskStart;

		public void Run()
		{
			if (TaskStart != null)
			{
				TaskStart(this, null);
			}
			try
			{
				DoStuff();
			}
			catch (Exception ex)
			{
				logger.Error("fatal error", ex);
			}

			if (TaskCompleted != null)
			{
				TaskCompleted(this, null);
			}
		}

		public abstract void DoStuff();

		#endregion
	}
}
