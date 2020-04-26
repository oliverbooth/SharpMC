namespace SharpMC.Core
{
	/// <summary>
	/// Server settings
	/// </summary>
	public class ServerSettings
	{
		internal static string Seed = "default";
		internal static byte[] SeedHash = new byte[256];
		internal static bool UseCompression = false;
		internal static int CompressionThreshold = 999999999;
		internal static bool OnlineMode = true;
		internal static bool EncryptionEnabled = true;
		internal static int MaxPlayers = 10;

		public static bool DisplayPacketErrors = true;
		public static bool Debug = true;
		public static string Motd = "";

		public static bool ReportExceptionsToClient = true;
	}
}
