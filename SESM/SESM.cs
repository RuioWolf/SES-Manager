using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace SESM
{
	public class Config
	{
		private static string cfgxml, srvxml;
		private static XmlDocument cfgDoc = new XmlDocument();
		private static XmlDocument srvDoc = new XmlDocument();

		public static void Init()
		{
			cfgxml = Environment.CurrentDirectory + "\\Config.xml";
			srvxml = Environment.CurrentDirectory + "\\Server.xml";
			LoadConfig();
		}

		public static void LoadConfig()
		{
			try
			{
				cfgDoc.Load(cfgxml);
			}
			catch (FileNotFoundException e)
			{
				Logger.Log(e.Message);
				CreatConfig("cfg");
			}
			try
			{
				srvDoc.Load(srvxml);
			}
			catch (FileNotFoundException e)
			{
				Logger.Log(e.Message);
				CreatConfig("srv");
			}
		}

		public static void CreatConfig(string type)
		{
			//			XmlDeclaration xmlDec = cfgDoc.CreateXmlDeclaration("1.0", "utf-8", null);
			//			XmlElement sesmConfig = cfgDoc.CreateElement("SESMConfig");
			//			
			//			cfgDoc.AppendChild(xmlDec);
			//			cfgDoc.AppendChild(sesmConfig);

			XElement xElement;
			XmlWriter xmlWriter;
			XmlWriterSettings settings = new XmlWriterSettings
			{
				Encoding = new UTF8Encoding(true),
				Indent = true
			};
			if (type == "cfg")
			{
				xElement = new XElement(
					new XElement("SESMConfig")
				);
				xmlWriter = XmlWriter.Create(cfgxml, settings);
			}
			else if (type == "srv")
			{
				xElement = new XElement(
					new XElement("ServerConfig",
						new XElement("Server", new XAttribute("name", "Server1"), new XAttribute("dir", "c:\\"),
							new XElement("IP", "1.2.3.4"),
							new XElement("Port", "27015")
						)
					)
				);
				xmlWriter = XmlWriter.Create(srvxml, settings);
			}
			else
			{
				return;
			}
			xElement.Save(xmlWriter);
			xmlWriter.Flush();
			xmlWriter.Close();
		}

		public static bool CheckXml()
		{
			//			try
			//			{
			//				cfgDoc.Load(cfgxml);
			//			}
			//			catch (Exception e)
			//			{
			//				Logger.Log(e.Message);
			//				return false;
			//			}
			//XmlNodeList nodeList = cfgDoc.GetElementsByTagName("IP");
			XmlNodeList nodeList = cfgDoc.SelectNodes("SESMConfig/*");
			if (nodeList.Count > 0)
			{
				return true;
			}
			return false;
		}

		public static List<string> QueryList()
		{
			List<string> list = new List<string>();
			StringBuilder stringBuilder = new StringBuilder();

			XmlNode servername = cfgDoc.DocumentElement;
			if (servername != null)
			{
				foreach (XmlNode node in servername)
				{
					foreach (XmlAttribute xa in node.Attributes)
					{
						list.Add(xa.Value);
					}
				}
				return list;
			}
			else
			{
				return null;
			}
		}

		public static string QuerySingleItem(string qservername, string item)
		{
			//			XmlNode node = cfgDoc.SelectSingleNode("ServerConfig/" + servername + "/" + item);
			//			if (node != null)
			//			{
			//				return node.InnerText;
			//			}
			//			else
			//			{
			//				return null;
			//			}
			XmlNode servername = cfgDoc.DocumentElement;
			if (servername != null)
			{
				string result = string.Empty;
				foreach (XmlNode node in servername)
				{
					foreach (XmlAttribute xa in node.Attributes)
					{
						if (xa.Value == qservername)
						{
							foreach (XmlNode res in node.ChildNodes)
							{
								if (res.Name == item)
								{
									result = res.InnerText;
								}
							}
						}
					}
				}
				return result;
			}
			else
			{
				return null;
			}
		}

		public static List<string> Query(string qservername)
		{
			List<string> list = new List<string>();
			XmlNodeList lnode = cfgDoc.SelectNodes("ServerConfig/*");

			if (lnode != null)
			{
				foreach (XmlElement sname in lnode)
				{
					if (sname.GetAttribute("name") == qservername)
					{
						foreach (XmlNode item in sname.ChildNodes)
						{
							list.Add(item.InnerText);
						}
					}
				}
				return list;
			}
			else
			{
				return null;
			}
		}

		public static void EditValve(string servername, string item, string valve)
		{
			XmlNode node = cfgDoc.SelectSingleNode("Server/" + servername + "/" + item);
			node.InnerText = valve;
			cfgDoc.Save(cfgxml);
		}

		public static void EditName(string oldname, string newname)
		{
			XmlNode node = cfgDoc.DocumentElement;
			foreach (XmlNode oldNode in node.ChildNodes)
			{
				if (oldNode.Name == oldname)
				{
					XmlNode n = oldNode;
				}
			}
		}

		private static void ErrorHandler(string arg)
		{
			Logger.Log(arg);
		}
	}

	public class Logger
	{
		static StreamWriter sw;
		static string text;
		static string logDir = Environment.CurrentDirectory + "\\Log";
		static string logFile = logDir + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
		static string cOutput = logDir + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

		private static void DirHandler()
		{
			if (!File.Exists(Environment.CurrentDirectory + "\\Log"))
			{
				Directory.CreateDirectory(logDir);
			}
		}

		public static void Log(string arg)
		{
			DirHandler();
			text = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd hh:mm:ss") + ": " + arg;
			sw = File.AppendText(logFile);
			sw.WriteLine(text);
			sw.Flush();
			sw.Close();
		}

		public static void Output(string arg)
		{
			DirHandler();
			text = DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd hh:mm:ss") + ": " + arg;
			sw = File.AppendText(cOutput);
			sw.WriteLine(text);
			sw.Flush();
			sw.Close();
		}
	}
}