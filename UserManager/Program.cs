using System;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace UserManager
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			new XDocument(new XElement("students","test")).Save("test.xml");
		}

		public static void AddStudent()
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
