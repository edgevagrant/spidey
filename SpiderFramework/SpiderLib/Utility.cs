using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading;
using log4net;
using WatiN.Core;
using System.Reflection;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace SpiderFramework
{
	public static class Utility
	{
		public static bool IsValidAttachmentLink(string hrefValue)
		{
			if(hrefValue.StartsWith("javascript")||hrefValue.StartsWith("#"))
				return false;

			if (hrefValue.EndsWith(".doc") 
				|| hrefValue.EndsWith(".pdf") 
				|| hrefValue.EndsWith(".docx")
				|| hrefValue.EndsWith(".xls")
				|| hrefValue.EndsWith(".xlsx")
				)
				return true;

			return false;
		}
		public static string CalculateMD5Hash(string input)
		{
			if (input == null)
				return null;
			// step 1, calculate MD5 hash from input
			MD5 md5 = MD5.Create();
			byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
			byte[] hash = md5.ComputeHash(inputBytes);

			// step 2, convert byte array to hex string
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < hash.Length; i++)
			{
				sb.Append(hash[i].ToString("X2"));
			}
			return sb.ToString();
		}
		public static string GetHostAddr(string uriString)
		{
			Uri uri = new Uri(uriString, UriKind.Absolute);
			return uri.Host;
		}
		public static bool IsSameSite(Uri uri1, Uri uri2)
		{
			if (string.Compare(uri1.Host, uri2.Host, true) == 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		static void GetInnerExceptionMessages(Exception ex, StringBuilder sb)
		{
			if (ex.InnerException == null)
			{
				if (sb.Length == 0)
					sb.AppendLine(ex.Message);
				else
					sb.AppendLine("|" + ex.Message);
			}
			else
			{
				GetInnerExceptionMessages(ex.InnerException, sb);
				if (sb.Length == 0)
					sb.AppendLine(ex.Message);
				else
					sb.AppendLine("|" + ex.Message);
			}
		}
		public static string GetInnerExceptionMessages(Exception ex)
		{
			StringBuilder sb = new StringBuilder();
			GetInnerExceptionMessages(ex, sb);
			return sb.ToString();
		}
		public static Uri MakeAbsoluteUri(
			Uri baseUri,
			Uri uri)
		{
			if (Uri.IsWellFormedUriString(uri.OriginalString, UriKind.Absolute))
			{
				return uri;
			}
			else
			{
				if (baseUri == null)
				{
					return uri;
				}
				else
				{
					string[] up = uri.OriginalString.Split('?');

					UriBuilder ub = new UriBuilder(baseUri.AbsoluteUri);
					ub.Path = CombineVPath(ub.Path, up[0]);

					if (up.Length > 1)
					{
						ub.Query = up[1];
					}

					return ub.Uri;
				}
			}
		}
		public static string CleanupUrl(
			string url)
		{
			// Remove accidentially contained ASP-tags.
			url = Regex.Replace(
				url,
				@"<%.*?%>",
				string.Empty,
				RegexOptions.Singleline);

			// Remove anchors.
			url = Regex.Replace(
				url,
				@"#.*?$",
				string.Empty,
				RegexOptions.Singleline);

			return url;
		}
		private static string CombineVPath(
			string s1,
			string s2)
		{
			if (string.IsNullOrEmpty(s1))
			{
				return s2;
			}
			else if (string.IsNullOrEmpty(s2))
			{
				return s1;
			}
			else
			{
				s1 = s1.TrimEnd('/');
				s2 = s2.TrimStart('/');

				return s1 + @"/" + s2;
			}
		}
	}
}
