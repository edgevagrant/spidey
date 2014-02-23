using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderFramework.Storage
{
    public interface IMongoDBEntity
    {
        string Key { get; }
    }
}
