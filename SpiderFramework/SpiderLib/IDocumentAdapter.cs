using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpiderFramework.Storage;

namespace SpiderFramework
{
	public interface IDocumentAdapter
	{
		IDocument GetDocument();
	}
}
