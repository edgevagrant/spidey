using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace SpiderFramework.Storage
{

    public interface IDocumentStorage
    {
        string DefaultPath { get; set; }
        bool DocumentExists(string title);
        bool SaveDocument(IDocument document);
		bool SaveDocument(IDocument document, bool rewrite);
		IDocument LoadDocument(string title);
		List<IDocument> LoadDocuments();
    }
}
