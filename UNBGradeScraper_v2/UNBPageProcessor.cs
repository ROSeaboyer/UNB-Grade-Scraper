using System;
using System.Net;
using System.IO;
using System.Text;

using StudentManager;

using HtmlAgilityPack;

namespace UNBGradeScraper_v2
{
	public class UNBPageProcessor
	{
		public static HttpWebResponse GetMarksWebResponse()
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://es.unb.ca/apps/mobile/grades/");
			request.Method = "GET";
			request.CookieContainer = new CookieContainer();
			return (HttpWebResponse)request.GetResponse();
		}

		public static Cookie GetSessionId(HttpWebResponse response)
		{
			return response.Cookies ["JSESSIONID"];
		}

		public static HtmlDocument LoginAsStudent(Stream stream, Cookie sessionId, Student student)
		{
			var doc = new HtmlDocument();

			using (stream)
			{
				doc.Load(stream);
				while (doc.DocumentNode.SelectSingleNode("//title").InnerText.Contains("Secure Services Login"))
				{
					doc = Login(sessionId, doc.DocumentNode.SelectNodes("//input[@type=\"hidden\"]"), doc.DocumentNode.SelectNodes("//input[@type=\"submit\"]"), student);
				}

			}

			return doc;
		}
	
		public static HtmlDocument Login(Cookie jSessionID, HtmlNodeCollection hidden, HtmlNodeCollection submit, Student student)
		{
			HtmlDocument loginDoc = new HtmlDocument();
			HttpWebRequest loginRequest = (HttpWebRequest)WebRequest.Create("https://idp.unb.ca/cas/login;jsessionid=" + jSessionID.Value + "?service=https%3a%2f%2fes.unb.ca%2fapps%2fmobile%2fgrades%2f");
			loginRequest.CookieContainer = new CookieContainer();
			loginRequest.CookieContainer.Add(jSessionID);
			loginRequest.Referer = "https://idp.unb.ca/cas/login?service=https%3a%2f%2fes.unb.ca%2fapps%2fmobile%2fgrades%2f";
			loginRequest.Method = "POST";
			loginRequest.ContentType = "application/x-www-form-urlencoded";
			StringBuilder postData = new StringBuilder();
			foreach (HtmlNode node in hidden)
			{
				postData.Append(node.GetAttributeValue("name", "") + "=" + node.GetAttributeValue("value", "") + "&");
			}
			postData.Append("username=" + student.username);
			postData.Append("&password=" + student.password);
			foreach (HtmlNode node in submit)
			{
				postData.Append("&" + node.GetAttributeValue("name", "") + "=" + node.GetAttributeValue("value", "").Replace(" ", "+"));
			}
			string postDataStr = postData.ToString();
			byte[] postDataByteArr = Encoding.UTF8.GetBytes(postDataStr);
			loginRequest.ContentLength = postDataByteArr.Length;
			using (Stream loginStream = loginRequest.GetRequestStream())
			{
				loginStream.Write(postDataByteArr, 0, postDataByteArr.Length);
			}
			HttpWebResponse loginResponse = (HttpWebResponse)loginRequest.GetResponse();
			using (Stream loginStream = loginResponse.GetResponseStream())
			{
				loginDoc.Load(loginStream);
			}
			return loginDoc;
		}
	}
}

