// Distrubuted under the MIT license
// ===================================================
// SharpMC uses the permissive MIT license.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the “Software”), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software
// 
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// 
// ©Copyright Kenny van Vulpen - 2015

using SharpMC.Core.Entity;
using SharpMC.Core.Utils;
using System;
using System.Linq;
using System.Text;

namespace SharpMC.Core.Networking.Packages
{
	internal class JoinGame : Package<JoinGame>
	{
		public Player Player;

		public JoinGame(ClientWrapper client) : base(client)
		{
			SendId = 0x26;
		}

		public JoinGame(ClientWrapper client, DataBuffer buffer) : base(client, buffer)
		{
			SendId = 0x26;
		}

		public override void Write()
		{
			if (Buffer != null)
			{
				Buffer.WriteVarInt(SendId); 

				Buffer.WriteInt(Player.EntityId); //	Entity ID	Int
				Buffer.WriteByte((byte) Player.Gamemode); // Gamemode	Unsigned Byte
				Buffer.WriteInt(Player.Dimension); // Dimension	Int Enum
				Buffer.WriteLong(ServerSettings.SeedHash); // Hashed seed	Long
				Buffer.WriteByte((byte) ServerSettings.MaxPlayers);
				var levelType = Client.Player.Level.LevelType.ToString();
				Buffer.WriteString(char.ToLower(levelType[0]) + levelType.Substring(1)); // pad to 16 bytes?
				Buffer.WriteVarInt(6); // View distance varint
				Buffer.WriteBool(false); // Reduced debug info
				//Buffer.WriteByte((byte) Client.Player.Level.Difficulty);
				Buffer.WriteBool(true); // Enable respawn screen
				Buffer.FlushData();
			}
		}
	}
}