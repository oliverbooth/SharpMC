﻿using System;
using System.IO;
using System.Threading;
using SharpMC.Core.API;
using SharpMC.Core.Entity;
using SharpMC.Core.Enums;
using SharpMC.Core.Networking;
using SharpMC.Core.PluginChannel;
using SharpMC.Core.Utils;
using SharpMC.Core.Worlds;
using SharpMC.Core.Worlds.Anvil;
using SharpMC.Core.Worlds.Flatland;
using SharpMC.Core.Worlds.Nether;
using SharpMC.Core.Worlds.Standard;

namespace SharpMC.Core
{
	public class SharpMcServer
	{
		private bool _initiated = false;
		public SharpMcServer()
		{
			ConsoleFunctions.WriteInfoLine(string.Format("Initiating {0}", Globals.ProtocolName));

			ConsoleFunctions.WriteInfoLine("Enabling global error handling...");
			var currentDomain = AppDomain.CurrentDomain;
			currentDomain.UnhandledException += UnhandledException;

			ConsoleFunctions.WriteInfoLine("Loading settings...");
			LoadSettings();

			ConsoleFunctions.WriteInfoLine("Loading variables...");
			InitiateVariables();

			ConsoleFunctions.WriteInfoLine("Checking files and directories...");
			CheckDirectoriesAndFiles();

			ConsoleFunctions.WriteInfoLine("Loading plugins...");
			LoadPlugins();

			_initiated = true;
		}

		public void StartServer()
		{
			if (!_initiated) throw new Exception("Server not initated!");

			ConsoleFunctions.WriteInfoLine("Enabling plugins...");
			EnablePlugins();

			Console.CancelKeyPress += ConsoleOnCancelKeyPress;

			try
			{
				new Thread(() => Globals.ServerListener.ListenForClients()).Start();
				new Thread(() => new ConsoleCommandHandler().WaitForCommand()).Start();
			}
			catch (Exception ex)
			{
				UnhandledException(this, new UnhandledExceptionEventArgs(ex, false));
			}
		}

		private void LoadSettings()
		{
			Config.ConfigFile = "server.properties";
			Config.InitialValue = new[]
			{
				"#DO NOT REMOVE THIS LINE - SharpMC Config",
				"Port=25565",
				"MaxPlayers=10",
				"LevelType=standard",
				"WorldName=world",
				"Online-mode=false",
				"Seed=SharpMC",
				"Motd=The hell is this crap?"
			};
			Config.Check();
		}

		private void InitiateVariables()
		{
			Globals.Rand = new Random();
			Console.Title = Globals.ProtocolName;
			ServerSettings.Debug = Config.GetProperty("debug", false);
			ServerSettings.DisplayPacketErrors = Config.GetProperty("ShowNetworkErrors", false);
#if DEBUG
			ServerSettings.Debug = true;
#endif
			ServerSettings.MaxPlayers = Config.GetProperty("MaxPlayers", 10);
			ServerSettings.Seed = Config.GetProperty("Seed", "SharpMC");
			ServerSettings.Motd = Config.GetProperty("motd", "The hell is this crap?");

			Globals.LevelManager = new LevelManager(LoadLevel());
			Globals.LevelManager.AddLevel("nether", new NetherLevel("nether"));
			ServerSettings.OnlineMode = Config.GetProperty("Online-mode", false);
			Globals.ChatHandler = new Synchronized<ChatHandler>(new ChatHandler());
			
			Globals.ServerKey = PacketCryptography.GenerateKeyPair();

			Globals.ClientManager = new ClientManager();

			Globals.ConsolePlayer = new Player(Globals.LevelManager.MainLevel)
			{
				Username = "Console",
				Wrapper = new ClientWrapper(null),
				Uuid = Guid.NewGuid().ToString(),
				Gamemode = Gamemode.Spectator,
			};
			Globals.ConsolePlayer.Wrapper.Player = Globals.ConsolePlayer;
			Globals.ConsolePlayer.IsOperator = true;

			Globals.MessageFactory = new MessageFactory();

			Globals.PluginManager = new PluginManager();

			Globals.ServerListener = new BasicListener();

			OperatorLoader.LoadOperators();
		}

		private Level LoadLevel()
		{
			var lvltype = Config.GetProperty("LevelType", "standard");
			Level lvl;
			switch (lvltype.ToLower())
			{
				case "flatland":
					lvl = new FlatLandLevel(Config.GetProperty("WorldName", "world"));
					break;
				case "standard":
					lvl = new StandardLevel(Config.GetProperty("WorldName", "world"));
					break;
				case "anvil":
					lvl = new AnvilLevel(Config.GetProperty("WorldName", "world"));
					break;
				default:
					lvl = new StandardLevel(Config.GetProperty("WorldName", "world"));
					break;
			}
			return lvl;
		}

		private void CheckDirectoriesAndFiles()
		{
			if (!Directory.Exists(Globals.LevelManager.MainLevel.LvlName))
				Directory.CreateDirectory(Globals.LevelManager.MainLevel.LvlName);

			if (!Directory.Exists("Players"))
				Directory.CreateDirectory("Players");
		}

		private void LoadPlugins()
		{
			Globals.PluginManager.LoadPlugins();
		}

		private void EnablePlugins()
		{
			Globals.PluginManager.EnablePlugins(Globals.LevelManager);
		}

		private static void UnhandledException(object sender, UnhandledExceptionEventArgs args)
		{
			var e = (Exception)args.ExceptionObject;
			ConsoleFunctions.WriteErrorLine("An unhandled exception occured! Error message: " + e.Message);
		}

		private void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs consoleCancelEventArgs)
		{
			Globals.StopServer();
		}
	}
}
