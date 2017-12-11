using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using SESM;

namespace SESMConsole
{
	class SESMConsole
	{
		static ServerInfo si;
		static Process srcds, steamcmd;

		static void Main(string[] args)
		{
			while (si == null)
			{
				ArgHandler(args);
			}
			ProcessInit();
			while (!File.Exists(si.ServerExe))
			{
				Update();
			}
		}

		static void Update()
		{
			steamcmd.Start();
			steamcmd.WaitForExit();
		}

		static void ProcessInit()
		{
			srcds.StartInfo.FileName = si.ServerExe;
			srcds.StartInfo.Arguments = si.ServerArgs;
			srcds.StartInfo.RedirectStandardOutput = true;
			srcds.StartInfo.StandardOutputEncoding=Encoding.UTF8;
			srcds.OutputDataReceived += Srcds_OutputDataReceived;
			steamcmd.StartInfo.FileName = Environment.CurrentDirectory + "\\SteamCMD\\steamcmd.exe";
			steamcmd.StartInfo.Arguments = si.UpdateArgs;
			steamcmd.StartInfo.RedirectStandardOutput = true;
			steamcmd.OutputDataReceived += SteamCMD_OutputDataReceived;
		}

		static void Srcds_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Data))
			{
				Logger.Output(e.Data);
				Console.WriteLine(e.Data);
			}
		}
		static void SteamCMD_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (!string.IsNullOrEmpty(e.Data))
			{
				string output = "[SteamCMD]" + e.Data;
				Logger.Output(output);
				Console.WriteLine(output);
			}
		}

		static void ArgHandler(string[] args)
		{
			if (args.Length == 0)
			{
				si = new ServerInfo(ServerChooser());
			}
			else if (args.Length == 1)
			{
				si = new ServerInfo(args[0]);
			}
			else
			{
				Console.WriteLine("To be added in future.");
				Console.ReadLine();
				Environment.Exit(0);
			}
		}

		static string ServerChooser()
		{
			int ID;
			List<String> list = Config.QueryList();
			if (list!= null)
			{
				for (int i = 1; i <= list.Count; ++i)
				{
					Console.WriteLine(i + ". " + list[(i - 1)]);
				}
				Console.WriteLine("Please type the server ID that you want to start.");
				try
				{
					ID = Convert.ToInt32(Console.ReadLine());
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					throw;
				}
				if (ID <= list.Count)
				{
					return list[(ID - 1)];
				}
				else
				{
					return null;
				}
			}
			else
			{
				Console.WriteLine("Please check you config!");
				Console.ReadLine();
				return null;
			}
		}

		static void InputHandler(string input)
		{
			Logger.Output("] " + input);
			switch (si.InputMethod)
			{
				case "rcon":
					RconSender(input);
					break;
				case "key":
					break;
			}
		}

		static void RconSender(string msg)
		{
			
		}
	}

	class ServerInfo
	{
		public string ServerPath;

		public string ServerExe => ServerPath + "\\srcds.exe";

		public string ServerArgs;

		public string InputMethod;

		public string UpdateArgs;

		public ServerInfo(string name)
		{
			ServerPath = Config.QuerySingleItem(name, "ServerPath");
			ServerArgs = Config.QuerySingleItem(name, "ServerArgs");
			InputMethod = Config.QuerySingleItem(name, "InputMethod");
			UpdateArgs = Config.QuerySingleItem(name, "UpdateArgs");
		}
	}
}