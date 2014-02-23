using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpiderFramework.Queue;
using CommandLine;
using System.Configuration;

namespace TaskCreator
{
	class Program
	{
        class Options
        {
            [Option('a', "assembly", Required=true, HelpText="Where the spider implemention is")]
            public string AssemblyName { get; set; }
            [Option('c', "class", Required = true, HelpText = "Class name of the spider")]
            public string ClassName { get; set; }
            [Option('n', "name", Required = true, HelpText = "Spider name for display purpose")]
            public string Name { get; set; }
            [Option('d', "category", Required = true, HelpText = "Category of the spider")]
            public string Category { get; set; }
            [Option("dbtype", HelpText = "Database type", DefaultValue="MongoDB")]
            public string DBType { get; set; }
            [Option("s", HelpText = "")]
            public string ConnectionString { get; set; }
            public string Parameters { get; set; }
        }

		static TaskMessage CreateMsg(Options options)
		{
			SpiderFramework.Queue.TaskMessage msg = new SpiderFramework.Queue.TaskMessage();
            msg.AssemblyName = options.AssemblyName;
            msg.ClassName = options.ClassName;
            msg.Name = options.Name;
            msg.Category = options.Category;
            msg.Parameters.Add("DBType", options.DBType);
            msg.Parameters.Add("ConnectionString", options.ConnectionString);
            //msg.AssemblyName = "SpiderFramework";
            //msg.ClassName = "SpiderFramework.Customization.NhfpcFoodSafeListTask";
            //msg.Name = "NhfpcFoodSafe list mining";
            //msg.Category = "list task";
            //msg.Parameters.Add("DBType", "MongoDB");
            //msg.Parameters.Add("ConnectionString", "mongodb://localhost:27017/test");
			return msg;
		}

		static void Main(string[] args)
		{
            var options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
            {
                return;
            }
            using (MongoQueue<TaskMessage> queue = new MongoQueue<TaskMessage>(ConfigurationManager.AppSettings["mongodbqueue"], 100000))
            {
                queue.Send(CreateMsg(options));
            }
		}
	}
}
