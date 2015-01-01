using HtmlAgilityPack;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace UNBGradeScraper_v2
{
    class Program
    {
		static Student student;
        static void Main(string[] args)
        {
            int currentCount, previousCount = 0, randTime;
            DateTime newTime;
            Random rand = new Random();
            Console.Title = "Grade Scraper";

			//TODO:Add support for multiple students
			student = Student.GetStudent ();

            while (true)
            {
				//TODO:Split into multiple stages
				var document = HTMLPageProcessor.getMarksPage(student);


                var courseTags = document.DocumentNode.SelectNodes("//li/a/strong");
                var markTags = document.DocumentNode.SelectNodes("//li/a/span/strong");
                currentCount = 0;

				//TODO:Separate into new method
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
					if (System.Environment.OSVersion.Platform != PlatformID.MacOSX && System.Environment.OSVersion.Platform != PlatformID.Unix && (int)System.Environment.OSVersion.Platform != 128)
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
    }
}