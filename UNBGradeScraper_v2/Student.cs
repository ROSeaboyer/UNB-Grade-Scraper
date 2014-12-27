using System;

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
			password = Program.getPassword();
			Console.WriteLine();

			return new Student (username, password);
		}
	}
}

