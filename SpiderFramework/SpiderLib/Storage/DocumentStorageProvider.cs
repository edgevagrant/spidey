using SpiderFramework.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpiderFramework.Storage
{
    public class DocumentStorageProvider
    {
        private DocumentStorageProvider() { }

        public static IDocumentStorage GetInstance(IDatabase database)
        {
            return GetInstance(database.DBType, database.ConnectionString);
        }

        public static IDocumentStorage GetInstance(DatabaseType databaseType, string connectionString)
        {
            if (databaseType == DatabaseType.MongoDB)
            {
                return new MongoDBDocumentStorage(connectionString, "docs");
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
