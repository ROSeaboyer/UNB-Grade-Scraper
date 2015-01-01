using System;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace StudentManager
{
	class MainClass
	{
		public const string STUDENT_FILE = "students.xml";

		public static XDocument studentInfo;

		public static void Main (string[] args)
		{
			if (File.Exists (STUDENT_FILE)) {
				XDocument.Load (STUDENT_FILE);
			} else {
				studentInfo = new XDocument ();
			}



		}

		public static void AddStudent(Student student)
		{
			throw new NotImplementedException ();
		}

		public static void EditStudent()
		{
			throw new NotImplementedException ();
		}

		public static void RemoveStudent()
		{
			throw new NotImplementedException ();
		}
	}
}
