using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Timers;
using SpiderFramework;
using SpiderFramework.Queue;
using System.Configuration;
using log4net;

namespace CrawlingService
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			log4net.Config.XmlConfigurator.Configure();
			ThreadPool.SetMaxThreads(Int32.Parse(ConfigurationManager.AppSettings["MaxThreads"]), 50);
			if (!Environment.UserInteractive)
			{
				ServiceBase[] ServicesToRun;
				ServicesToRun = new ServiceBase[] 
				{ 
					new Service1() 
				};
				ServiceBase.Run(ServicesToRun);
			}
			else
			{
				System.Timers.Timer aTimer = new System.Timers.Timer();
				aTimer.Elapsed += new ElapsedEventHandler(PerformTimerOperation);
				// Set the Interval to 3 second.
				aTimer.Interval = Double.Parse(ConfigurationManager.AppSettings["ScanInterval"]);
				aTimer.Enabled = true;
				// Let the timer run for 10 seconds before the main
				// thread exits and the process terminates
				//--list task

				Console.WriteLine("Task Engine started");
				Console.Read();
			}
		}
		static ILog logger =LogManager.GetLogger("main thread");
		static void PerformTimerOperation(Object stateInfo, ElapsedEventArgs args)
		{
			int workThreads=0, completionThreads=0;

			ThreadPool.GetAvailableThreads(out workThreads, out completionThreads);
			if (workThreads == 0)
				return;
			using (MongoQueue<TaskMessage> queue = new MongoQueue<TaskMessage>(ConfigurationManager.AppSettings["mongodbqueue"], 100000000))
			{
				var msg = queue.Receive();
				ThreadPool.QueueUserWorkItem(new WaitCallback(x =>
				{
					if (msg != null)
					{
						logger.Info("initialize " + msg.ToString());
						var task = TaskCreator.CreateInstance(msg);
						task.Run();
					}

				}));
			}
		}

	}
}
