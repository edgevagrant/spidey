using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SpiderFramework.Storage
{
    public interface IFileStorage
    {
        string Add(string path, Stream stream);
        string Add(string path, Stream stream, string contentType);
        void Delete(string path);
        bool Exists(string path);
    }
}
