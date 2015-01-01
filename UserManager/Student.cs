using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;


namespace StudentManager
{
	public class Student
	{
		public string username { get; private set; }
		public string password { get; private set; }

		public Student()
		{
			string username;
			string password;

			Console.Write("Please enter your username: ");
			username = Console.ReadLine();
			Console.Write("Please enter your password: ");
			password = getPassword();
			Console.WriteLine();

			this.username = username;
			this.password = password;
		}

		public Student (string username, string password)
		{
			this.username = username;
			this.password = password;
		}

		public static Student GetStudent()
		{
			XDocument doc;
			if (File.Exists (MainClass.STUDENT_FILE)) {
				doc = XDocument.Load (MainClass.STUDENT_FILE);
			} else {
				return new Student ();
			}

			var stu = (from student in doc.Descendants ("student")
				where student.Element ("username").Value == "dschroer"
				select new Student(student.Element("username").Value,student.Element("password").Value)).FirstOrDefault ();

			return stu;
		}

		private static string getPassword()
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

