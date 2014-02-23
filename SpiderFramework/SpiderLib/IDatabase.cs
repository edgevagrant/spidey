using SpiderFramework.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderFramework
{
	public enum DatabaseType 
	{
		SqlServer,
		MongoDB
	}

	public interface IDatabase
	{
		string ConnectionString { get; set; }
		DatabaseType DBType { get; set; }
	}
}
