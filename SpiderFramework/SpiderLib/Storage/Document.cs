using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson;
using SpiderFramework;

namespace SpiderFramework.Storage
{
	public class Document:IDocument
	{
		public Document()
		{
			this.Attachments = new List<string>();
		}
		public ObjectId Id
		{
			get;
			set;
		}
		public string Content
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public string Url
		{
			get;
			set;
		}
		public DateTime? PublishDate
		{
			get;
			set;
		}
		public DateTime LastUpdatedDate
		{
			get;
			set;
		}

		public List<string> Keywords
		{
			get;
			set;
		}

		public string Author
		{
			get;
			set;
		}

		public List<string> Attachments
		{
			get;
			set;
		}

		public string MD5
		{
			get { return Utility.CalculateMD5Hash(this.Content); }
			set { }
		}

        public string SourceName
        {
            get;
            set;
        }
        public string SourceUrl
        {
            get;
            set;
        }
	}
}
