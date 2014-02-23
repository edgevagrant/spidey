using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace SpiderFramework.Storage
{
    public class MySqlDocumentStorage : IDocumentStorage
    {
        MySqlConnection connection = null;
        string tableName = null;
        
        public MySqlDocumentStorage(string tableName)
        {
            this.tableName = tableName;
        }
        public void Connect(string connectionString)
        {
            this.connection = new MySqlConnection(connectionString);
        }
        public void Disconnect()
        {
            this.connection.Close();
        }

        public string DefaultPath
        {
            get;
            set;
        }

        public bool DocumentExists(string url)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = string.Format("select count(*) from {0} where url = '{1}'", this.tableName, url);
            int count = (int)cmd.ExecuteScalar();
            return count>0 ?  true:false;
        }

        public bool SaveDocument(IDocumentDescriptor document)
        {
            if (DocumentExists(document.Url))
            {
                //update
                var cmd = connection.CreateCommand();
                cmd.CommandText = string.Format("update {0} set content='{1}',title = '{3}' where url = '{2}'", this.tableName,document.Content,document.Url,document.Title);
                int i = cmd.ExecuteNonQuery();
                if (i <= 0)
                {
                    return false;
                }
            }
            else
            { 
                //insert
                var cmd = connection.CreateCommand();
                cmd.CommandText = string.Format("insert into {0}(url,content,title)values('{1}','{2}','{3}')", this.tableName,document.Url, document.Content,document.Title);
                int i = cmd.ExecuteNonQuery();
                if (i <= 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool SaveDocument(string title, string document)
        {
            IDocumentDescriptor doc = new HtmlContentDescriptor();
            return this.SaveDocument(doc.Title, doc.Content);
        }

        public IDocumentDescriptor LoadDocument(string url)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = string.Format("select content from {0} where url='{1}'",this.tableName,url);
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                HtmlContentDescriptor content = new HtmlContentDescriptor();
                content.Content = reader["content"] as string;
                content.Title = reader["title"] as string;
                content.Url = url;
                return content;
            }
            return null;
        }

        public List<IDocumentDescriptor> LoadDocuments()
        {
            throw new NotImplementedException();
        }
    }
}
