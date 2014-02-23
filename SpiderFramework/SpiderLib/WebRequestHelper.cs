using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.IO.Compression;

namespace SpiderFramework
{
    public class WebRequestHelper
    {
        const string FireFoxAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.2.23) Gecko/20110920 Firefox/3.6.23";
        const string IE7 = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
        const string IE6 = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
        const string IE9 = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
        const string IE10 = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; Trident/6.0)";
        const string Chrome20 = "Mozilla/5.0 (X11; CrOS i686 2268.111.0) AppleWebKit/536.11 (KHTML, like Gecko) Chrome/20.0.1132.57 Safari/536.11";

        private static HttpWebRequest CreateWebRequest(string url, string acceptTypeText)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.ServicePoint.Expect100Continue = false;
            req.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");
            req.KeepAlive = true;
            req.AllowAutoRedirect = true;
            req.Accept = acceptTypeText; 
            req.Timeout = 50000;
            return req;
        }
        public static HttpWebResponse SendPostRequestAndGetResponse(string url, string referer, CookieContainer cookies, string postData)
        {
            HttpWebResponse res = null;
            HttpWebRequest req = CreateWebRequest(url, "text/html, application/xhtml+xml, */*");
            req.Method = "POST";
            req.ServicePoint.Expect100Continue = false;
            req.CookieContainer = cookies;
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = 0;
            req.Referer = referer;
            req.UserAgent = IE9;

            if (!string.IsNullOrEmpty(postData))
            {
                byte[] lbPostBuffer = Encoding.UTF8.GetBytes(postData);
                req.ContentLength = lbPostBuffer.Length;

                Stream PostStream = req.GetRequestStream();
                PostStream.Write(lbPostBuffer, 0, lbPostBuffer.Length);
                PostStream.Close();  
            }
            res = (HttpWebResponse)req.GetResponse();
            return res;
        }
        public static string SendPostRequest(string url, string referer, CookieContainer cookies, string postData, string encodingName)
        {
            HttpWebResponse res = SendPostRequestAndGetResponse(url, referer, cookies, postData);
            return GetResponseString(res, encodingName);
        }
        public static string SendPostRequest(string url, CookieContainer cookies, string postData)
        {
            return SendPostRequest(url, null, cookies, postData, null);
        }
        public static string SendPostRequest(string url, string postData)
        {
            return SendPostRequest(url, null, null, postData, null);
        }
        public static string SendPostRequest(string url, string referer, string postData)
        {
            return SendPostRequest(url, referer, null, postData, null);
        }

        public static string GetResponseString(HttpWebResponse res,string encodingName)
        {
			if (encodingName == null)
				encodingName = "ISO-8859-1";
            MemoryStream memory = null;
            Stream responseStream = null;
            string content;
            try
            {
                //handle chunked data
                int length = 0;

                if (res.ContentLength > 0)
                {
                    length = (int)res.ContentLength;
                }
                else
                {
                    length = 8000;
                }
                memory = new MemoryStream(length);
                //if (res.Headers["Transfer-Encoding"] != null && res.Headers["Transfer-Encoding"].ToLower() == "chunked")
                Byte[] buf = new byte[4096];
                Stream resStream = res.GetResponseStream();
                int count = 0;


                do
                {
                    count = resStream.Read(buf, 0, buf.Length);
                    if (count != 0)
                    {
                        memory.Write(buf, 0, count);
                    }
                } while (count > 0);

                memory.Position = 0;
                //handle gzip or deflate stream
                if (res.ContentEncoding.ToLower().Contains("gzip"))
                {
                    responseStream = new GZipStream(memory, CompressionMode.Decompress);
                }
                else if (res.ContentEncoding.ToLower().Contains("deflate"))
                {
                    responseStream = new DeflateStream(memory, CompressionMode.Decompress);
                }
                else
                {
                    responseStream = memory;
                }
                int charsetPos = res.ContentType.IndexOf("charset=");
				string encoding = encodingName;

                if (charsetPos >= 0)
                {
                    int semicolonPos = res.ContentType.IndexOf(";", charsetPos);
                    encoding = res.ContentType.Substring(charsetPos + 8, semicolonPos > 0 ? semicolonPos - charsetPos - 8 : res.ContentType.Length - charsetPos - 8);
                }
                var oEncoding = Encoding.GetEncoding(encoding);
                using (StreamReader sr = new StreamReader(responseStream, oEncoding))
                {
                    content = sr.ReadToEnd();
                }
                return content;
            }
            finally
            {
                if (memory != null)
                    memory.Close();
                if (responseStream != null)
                    responseStream.Close();
            }

        }
        public static Stream GetRtfResponse(string url, string referer, CookieContainer cookies)
        {
            return SendGetRequestWithContentTypeResponse(url, "application/rtf", referer, cookies);
        }
        public static Stream GetHtmlResponse(string url, string referer, CookieContainer cookies)
        {
            return SendGetRequestWithContentTypeResponse(url, "text/html", referer, cookies);
        }
        public static Stream GetXmlResponse(string url, string referer, CookieContainer cookies)
        {
            return SendGetRequestWithContentTypeResponse(url, "text/xml", referer, cookies);
        }
        public static Stream GetTextResponse(string url, string referer, CookieContainer cookies)
        {
            return SendGetRequestWithContentTypeResponse(url, "text/plain", referer, cookies);
        }
        public static Stream GetWordResponse(string url, string referer, CookieContainer cookies)
        {
            return SendGetRequestWithContentTypeResponse(url, "application/msword", referer, cookies);
        }
        public static Stream GetExcelResponse(string url, string referer, CookieContainer cookies)
        {
            return SendGetRequestWithContentTypeResponse(url, "application/x-excel", referer, cookies);
        }
        public static Stream GetPDFResponse(string url, string referer, CookieContainer cookies)
        {
            return SendGetRequestWithContentTypeResponse(url, "application/pdf", referer, cookies);
        }
        public static Stream GetImageResponse(string url, string referer, CookieContainer cookies)
        {
            return SendGetRequestWithContentTypeResponse(url, "image/png, image/svg+xml, image/*;q=0.8, */*;q=0.5", referer, cookies);
        }
        public static Stream SendGetRequestAndAutoDetectContentType(string url, string referer, CookieContainer cookies)
        {
            Uri uri = new Uri(url, UriKind.Absolute);
            FileInfo fi = new FileInfo(uri.LocalPath);
            switch (fi.Extension)
            { 
                case ".pdf":
                    return GetPDFResponse(url, referer, cookies);
                case ".xls":
                    return GetExcelResponse(url, referer, cookies);
                case ".doc":
                    return GetWordResponse(url, referer, cookies);
                case ".xml":
                    return GetXmlResponse(url, referer, cookies);
                case ".rtf":
                    return GetRtfResponse(url, referer, cookies); 
                case "xlsx":
                    return SendGetRequestWithContentTypeResponse(url, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", referer, cookies);
                case "docx":
                    return SendGetRequestWithContentTypeResponse(url, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", referer, cookies);
                case "pptx":
                    return SendGetRequestWithContentTypeResponse(url, "application/vnd.openxmlformats-officedocument.presentationml.presentation", referer, cookies);
                case ".jpg":
                case ".png":
                case ".gif":
                case ".jpeg":
                case ".bmp":
                    return GetImageResponse(url, referer, cookies);
                default:
                    return GetTextResponse(url, referer, cookies);
            }
        }
        public static Stream SendGetRequestWithContentTypeResponse(string url, string contentType, string referer, CookieContainer cookies)
        { 
            HttpWebResponse res = null;
            //byte[] buffer =new byte[1024];
            Stream responseStream = null;
            try
            {
                HttpWebRequest req = CreateWebRequest(url, contentType);
                req.Method = "GET";
                req.ServicePoint.Expect100Continue = false;
                req.Referer = referer;
                req.CookieContainer = cookies;
                req.UserAgent = IE9;
                res = (HttpWebResponse)req.GetResponse();

                //handle chunked data
                int length = 0;

                if (res.ContentLength > 0)
                {
                    length = (int)res.ContentLength;
                }
                else
                {
                    length = 8000;
                }
                MemoryStream memory = new MemoryStream(length);
                //if (res.Headers["Transfer-Encoding"] != null && res.Headers["Transfer-Encoding"].ToLower() == "chunked")
                //{

                Byte[] buf = new byte[4096];
                Stream resStream = res.GetResponseStream();
                int count = 0;
                do
                {
                    count = resStream.Read(buf, 0, buf.Length);
                    if (count != 0)
                    {
                        memory.Write(buf, 0, count);
                    }
                } while (count > 0);

                //}
                memory.Position = 0;
                //handle gzip or deflate stream
                if (res.ContentEncoding.ToLower().Contains("gzip"))
                {
                    responseStream = new GZipStream(memory, CompressionMode.Decompress);
                }
                else if (res.ContentEncoding.ToLower().Contains("deflate"))
                {
                    responseStream = new DeflateStream(memory, CompressionMode.Decompress);
                }
                else
                {
                    responseStream = memory;
                }
                return responseStream;
                //using (BinaryReader br = new BinaryReader(responseStream))
                //{
                //    int x = br.Read(buffer,0,buffer.Length);
                //    while(x>0)
                //    {
                //        ms.Write(buffer, 0, x);
                //        x = br.Read(buffer, 0, buffer.Length);
                //    }
                //}
                //return ms;
            }
            finally
            {
                if (res != null)
                    res.Close();
            }
        }
        public static HttpWebResponse SendGetRequestWithHttpResponse(string url, string referer, CookieContainer cookies)
        {
            HttpWebRequest req = CreateWebRequest(url, "text/html, application/xhtml+xml, */*");
            req.ServicePoint.Expect100Continue = false;
            req.Method = "GET";
            req.Referer = referer;
            req.CookieContainer = cookies;
            //req.Headers.Add(HttpRequestHeader.Cookie)
            //req.AutomaticDecompression = DecompressionMethods.GZip;
            req.UserAgent = IE9;
            return (HttpWebResponse)req.GetResponse();
        }
        public static string SendGetRequest(string url, string referer, CookieContainer cookies, string encodingName)
        {
            HttpWebResponse res = null;
            try
            {
				res = SendGetRequestWithHttpResponse(url, referer, cookies);
                return GetResponseString(res, encodingName);
            }
            finally
            {
                if (res != null)
                    res.Close();
            }
        }
        public static string SendGetRequest(string url)
        {
            return SendGetRequest(url, null,null, null);
        }
        public static string SendGetRequest(string url, CookieContainer cookies)
        {
            return SendGetRequest(url, null, cookies, null);
        }
    }
}
