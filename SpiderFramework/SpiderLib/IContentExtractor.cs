using SpiderFramework.Storage;
using log4net;


namespace SpiderFramework
{
    public interface IContentExtractor
    {
        bool Goto(string url);
        //bool Goto(string url, string referer);
        HtmlContent GetContent(string xpath);
        ILog Logger { get; set; }
        object CreateInstance();
        object Instance { get; }
        void DestroyInstance();
    }


    public enum HttpMethod
    { 
        POST,
        GET
    }

}
