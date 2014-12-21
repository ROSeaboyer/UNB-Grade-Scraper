using HtmlAgilityPack;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace UNBGradeScraper_v2
{
    class Program
    {
        static string username = "", password = "";
        static void Main(string[] args)
        {
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
            int currentCount, previousCount = 0, randTime;
            DateTime newTime;
            Random rand = new Random();
            Console.Title = "Grade Scraper";
            while (true)
            {
                var document = getMarksPage();
                var courseTags = document.DocumentNode.SelectNodes("//li/a/strong");
                var markTags = document.DocumentNode.SelectNodes("//li/a/span/strong");
                currentCount = 0;
                for (int i = 0; i < courseTags.Count; i++)
                {
                    if (courseTags[i].InnerText.Contains("COOP")) continue;
                    Console.Write(courseTags[i].InnerText.Substring(0, courseTags[i].InnerText.LastIndexOf("*")).Replace("*", " "));
                    Console.Write(":\t");
                    if (string.IsNullOrWhiteSpace(markTags[i].InnerText))
                    {
                        Console.WriteLine("N/A");
                        currentCount++;
                    }
                    else
                    {
                        Console.WriteLine(markTags[i].InnerText);
                    }
                }
                if (currentCount < previousCount || currentCount == 0)
                {
                    FlashWindow.Flash(FindWindow.FindWindowByCaption(IntPtr.Zero, Console.Title));
                    if (currentCount == 0)
                    {
                        break;
                    }
                }
                previousCount = currentCount;

                randTime = rand.Next(15, 45);
                newTime = DateTime.Now.AddMinutes(randTime);
                Console.WriteLine("Next check is at " + newTime.ToString("t") + " (" + randTime + " minutes)");
                Thread.Sleep(randTime * 60000);
            }
            Console.WriteLine("Check complete. Press any key to exit...");
            Console.ReadKey();
        }

        static void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode != PowerModes.StatusChange)
            {
                Console.WriteLine("Computer entered " + e.Mode + " at " + DateTime.Now.ToString("t"));
            }
        }

        static void checkUsername()
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.Write("Please enter your username: ");
                username = Console.ReadLine();
                Console.Write("Please enter your password: ");
                password = getPassword();
                Console.WriteLine();
            }
        }

        static HtmlDocument getMarksPage()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://es.unb.ca/apps/mobile/grades/");
            request.Method = "GET";
            request.CookieContainer = new CookieContainer();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                var doc = new HtmlDocument();
                doc.Load(stream);
                if (doc.DocumentNode.SelectSingleNode("//title").InnerText.Contains("Secure Services Login"))
                {
                    doc = Login(response.Cookies["JSESSIONID"], doc.DocumentNode.SelectNodes("//input[@type=\"hidden\"]"), doc.DocumentNode.SelectNodes("//input[@type=\"submit\"]"));
                }
                return doc;
            }
        }

        static HtmlDocument Login(Cookie jSessionID, HtmlNodeCollection hidden, HtmlNodeCollection submit)
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
            checkUsername();
            postData.Append("username=" + username);
            postData.Append("&password=" + password);
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

        static string getPassword()
        {
            StringBuilder pwd = new StringBuilder();
            ConsoleKeyInfo i;
            while (true)
            {
                i = Console.ReadKey(true);
                if (i.Key == ConsoleKey.Enter)
                {
                    break;
                }
                else if (i.Key == ConsoleKey.Backspace)
                {
                    if (pwd.Length > 0)
                    {
                        pwd.Remove(pwd.Length - 1, 1);
                        Console.Write("\b \b");
                    }
                }
                else
                {
                    pwd.Append(i.KeyChar);
                    Console.Write("*");
                }
            }
            return pwd.ToString();
        }
    }
}