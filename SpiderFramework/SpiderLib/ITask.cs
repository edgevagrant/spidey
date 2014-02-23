using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderFramework
{
	public enum TaskStatus
	{ 
		New,
		InProgress,
		Completed,
		Failed
	}

    public interface ITask
    {
		string Name { get; set; }
		string Category { get; set; }
		TaskStatus Status { get; set; }
		DateTime? StartTime { get; set; }
		DateTime? EndTime { get; set; }

		event EventHandler TaskCompleted;
		event EventHandler TaskStart;

		void Run();
    }
}
