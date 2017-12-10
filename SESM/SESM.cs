#define mB

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace SESM
{
	public class Config
	{
		private static string cfgxml, srvxml;

//		private static XmlDocument cfgDoc = new XmlDocument();
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
//				cfgDoc.Load(cfgxml);
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
							new XAttribute("path", "SteamCMD\\steamcmd.exe"),
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
			if (nodeList != null && nodeList.Count > 0)
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
			string result = string.Empty;

#if !mB
			XmlNode servers = srvDoc.DocumentElement;
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
#else
			try
			{
				XmlAttributeCollection xmlAttributeCollection = GetSrvNodeByName(qservername).Attributes;
				result = xmlAttributeCollection.GetNamedItem(item).Value;
				return result;
			}
			catch (Exception)
			{

				return null;
			}
#endif
		}

		public static List<string> Query(string qservername)
		{
			List<string> list = new List<string>();
#if !mB
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
								list.Add(valve.OuterXml);
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
			
#else
			var xmlAttributeCollection = GetSrvNodeByName(qservername).Attributes;
			if (xmlAttributeCollection != null)
				foreach (XmlAttribute att in xmlAttributeCollection)
				{
					list.Add(att.Value);
				}
			return list;
#endif
		}

		public static void EditValve(string eservername, string item, string valve)
		{
			if (string.IsNullOrEmpty(eservername) ||
				string.IsNullOrEmpty(item) ||
				string.IsNullOrEmpty(valve))
			{
				return;
			}
#if !mB
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
#else
			XmlElement xe = (XmlElement) GetSrvNodeByName(eservername);

			xe.SetAttribute(item, valve);
			srvDoc.Save(srvxml);
#endif
		}

		public static void EditValve(string eservername, string item)
		{
			if (string.IsNullOrEmpty(eservername) ||
				string.IsNullOrEmpty(item))
			{
				return;
			}

			XmlElement xe = (XmlElement) GetSrvNodeByName(eservername);

			if (item != "name")
			{
				xe.RemoveAttribute(item);
			}
			else
			{
				DeleteServer(eservername);
			}
			srvDoc.Save(srvxml);
		}

		public static void DeleteServer(string dservername)
		{
#if !mB
			XmlNode servers = srvDoc.DocumentElement;
			try
			{
				if (servers != null)
					foreach (XmlNode node in servers.ChildNodes)
					{
						if (node.Attributes != null)
							foreach (XmlAttribute att in node.Attributes)
							{
								if (att.Value == dservername)
								{
									srvDoc.RemoveChild(node);
								}
							}
					}
				srvDoc.Save(srvxml);
			}
			catch (NullReferenceException e)
			{
				Logger.Log(e.Message);
			}
#else
			XmlNode srvroot = srvDoc.SelectSingleNode("ServerConfig");
			try
			{
				XmlNode selectSingleNode = srvDoc.SelectSingleNode("ServerConfig");
				if (selectSingleNode != null)
				{
					XmlNodeList srv = selectSingleNode.ChildNodes;
				}
				if (srvroot != null)
					srvroot.RemoveChild(GetSrvNodeByName(dservername));
			}
			catch (NullReferenceException e)
			{
				Logger.Log(e.Message);
			}
			srvDoc.Save(srvxml);
#endif
		}

		public static XmlNode AddServer(string aservername)
		{
			XmlNode srvroot = srvDoc.SelectSingleNode("ServerConfig");
			XmlElement xe = srvDoc.CreateElement("Server");
			xe.SetAttribute("name", aservername);
			if (srvroot != null)
				srvroot.AppendChild(xe);
			srvDoc.Save(srvxml);
			XmlNode result = (XmlNode) xe;
			return result;
		}

		public static XmlNode GetSrvNodeByName(string servername)
		{
			XmlNode result = null;
			XmlNode servers = srvDoc.DocumentElement;
			bool exist = false;

			try
			{
				if (servers != null)
					foreach (XmlNode node in servers.ChildNodes)
					{
						if (node.Attributes != null)
							foreach (XmlAttribute att in node.Attributes)
							{
								if (att.Value == servername)
								{
									result = node;
									exist = true;
								}
							}
					}
				if (!exist)
				{
					result = AddServer(servername);
				}
				return result;
			}
			catch (Exception e)
			{
				Logger.Log(e.Message);
				return null;
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