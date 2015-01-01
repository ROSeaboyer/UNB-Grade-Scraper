using System;
using System.Text;

namespace UNBGradeScraper_v2
{
	public class Student
	{
		public string username { get; private set; }
		public string password { get; private set; }

		public Student (string username, string password)
		{
			this.username = username;
			this.password = password;
		}

		public static Student GetStudent()
		{
			string username;
			string password;

			Console.Write("Please enter your username: ");
			username = Console.ReadLine();
			Console.Write("Please enter your password: ");
			password = getPassword();
			Console.WriteLine();

			return new Student (username, password);
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

