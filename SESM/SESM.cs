using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;

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
					new XElement("SESMConfig",
						new XElement("SteamCMD",
							new XAttribute("path", "steamcmd\\steamcmd.exe"),
							new XAttribute("login", "anonymous")
						)
					)
				);
				xmlWriter = XmlWriter.Create(cfgxml, settings);
			}
			else if (type == "srv")
			{
				xElement = new XElement(
					new XElement("ServerConfig"));
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
			XmlNodeList nodeList = srvDoc.SelectNodes("ServerConfig/*");
			if (nodeList.Count > 0)
			{
				return true;
			}
			return false;
		}

		public static List<string> QueryList()
		{
			List<string> list = new List<string>();

			XmlNode servers = srvDoc.DocumentElement;
			if (servers != null)
			{
				foreach (XmlNode node in servers)
				{
					if (node.Attributes != null)
						list.Add(node.Attributes.GetNamedItem("name").OuterXml);
					else
						return null;
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
			XmlNode servers = srvDoc.DocumentElement;
			string result = string.Empty;

			try
			{
				foreach (XmlNode node in servers.ChildNodes)
				{
					foreach (XmlAttribute att in node.Attributes)
					{
						if (att.Value == qservername)
						{
							result = node.Attributes.GetNamedItem(item).Value;
						}
					}
				}
				return result;
			}
			catch (NullReferenceException e)
			{
				Logger.Log(e.Message);
				return null;
			}
		}

		public static List<string> Query(string qservername)
		{
			List<string> list = new List<string>();
			XmlNode servers = srvDoc.DocumentElement;

			try
			{
				foreach (XmlNode node in servers.ChildNodes)
				{
					foreach (XmlAttribute att in node.Attributes)
					{
						if (att.Value == qservername)
						{
							foreach (XmlAttribute valve in node.Attributes)
							{
								list.Add(valve.Value);
//								list.Add(valve.OuterXml);
							}
						}
					}
				}
				return list;
			}
			catch (NullReferenceException e)
			{
				Logger.Log(e.Message);
				return null;
			}
		}

		public static void EditValve(string eservername, string item, string valve)
		{
			XmlNode servers = srvDoc.DocumentElement;

			try
			{
				foreach (XmlNode node in servers.ChildNodes)
				{
					foreach (XmlAttribute att in node.Attributes)
					{
						if (att.Value == eservername)
						{
							foreach (XmlAttribute name in node.Attributes)
							{
								if (name.Name == item)
								{
									name.Value = valve;
								}
							}
						}
					}
				}
				srvDoc.Save(srvxml);
			}
			catch (NullReferenceException e)
			{
				Logger.Log(e.Message);
			}
		}
	}

	public class Logger
	{
		static StreamWriter sw;
		static string text = string.Empty;
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