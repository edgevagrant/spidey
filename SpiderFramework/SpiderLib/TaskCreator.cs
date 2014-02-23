using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SpiderFramework.Queue;

namespace SpiderFramework
{
	public class TaskCreator
	{
		public static ITask CreateInstance(TaskMessage msg)
		{
			var objHandler= Activator.CreateInstance(msg.AssemblyName, msg.ClassName);
			var task = (ITask)objHandler.Unwrap();
			task.Name = msg.Name;
			task.Category = msg.Category;
			foreach (var pair in msg.Parameters)
			{
				PropertyInfo prop = task.GetType().GetProperty(pair.Key, BindingFlags.Public | BindingFlags.Instance);
				if (prop != null&& prop.CanWrite)
				{
					if (!prop.PropertyType.IsEnum)
					{
						prop.SetValue(task, pair.Value, null);
					}
					else
					{
						prop.SetValue(task, prop.PropertyType.GetField(pair.Value).GetValue(prop.PropertyType), null);
					}
				}
			}
			return task;
		}
	}
}
