using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderLib
{
	public class News
	{
		string Title { get; set; }
		string Body { get; set; }
		string Abstract { get; set; }
		List<string> Keywords { get; set; }
		string SourceUrl { get; set; }
		string Author { get; set; }
		List<string> Attachments { get; set; }
	}
}
