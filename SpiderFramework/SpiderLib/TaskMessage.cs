using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderFramework.Queue
{
	public class TaskMessage
	{
		public TaskMessage()
		{
			this.Parameters = new Dictionary<string, string>();
		}
		public string AssemblyName
		{
			get;
			set;		
		}
		public string ClassName
		{ 
			get;set;
		}
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
		public Dictionary<string, string> Parameters 
		{ 
			get; set; 
		}
		public override string ToString()
		{
			return string.Format("{0}, {1}", ClassName, Name);
		}
	}
}
