using SpiderFramework.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderFramework.Storage
{
    public class FileStorageProvider
    {
        private FileStorageProvider() { }



        public static IFileStorage GetInstance(DatabaseType databaseType, string connectionString)
        {
            if (databaseType == DatabaseType.MongoDB)
            {
                return new MongoDBFileStorage(connectionString, "docs");
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
