using System;
using System.Net;
using System.IO;
using System.Text;

using HtmlAgilityPack;

namespace UNBGradeScraper_v2
{
	public class HTMLPageProcessor
	{
		public static Stream GetMarksStream()
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://es.unb.ca/apps/mobile/grades/");
			request.Method = "GET";
			request.CookieContainer = new CookieContainer();
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			return response.GetResponseStream ();
		}

		/*
		public static Stream ApplyLogin(Stream stream, Student student)
		{
			using (stream)
			{
				var doc = new HtmlDocument();
				doc.Load(stream);
				while (doc.DocumentNode.SelectSingleNode("//title").InnerText.Contains("Secure Services Login"))
				{
					doc = Login(response.Cookies["JSESSIONID"], doc.DocumentNode.SelectNodes("//input[@type=\"hidden\"]"), doc.DocumentNode.SelectNodes("//input[@type=\"submit\"]"), student);
				}
				return doc;
			}
		}*/

		public static HtmlDocument getMarksPage(Student student)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://es.unb.ca/apps/mobile/grades/");
			request.Method = "GET";
			request.CookieContainer = new CookieContainer();
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			using (Stream stream = response.GetResponseStream())
			{
				var doc = new HtmlDocument();
				doc.Load(stream);
				while (doc.DocumentNode.SelectSingleNode("//title").InnerText.Contains("Secure Services Login"))
				{
					doc = Login(response.Cookies["JSESSIONID"], doc.DocumentNode.SelectNodes("//input[@type=\"hidden\"]"), doc.DocumentNode.SelectNodes("//input[@type=\"submit\"]"), student);
				}
				return doc;
			}
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

