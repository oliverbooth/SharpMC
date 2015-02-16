﻿using SharpMCRewrite.NET;

namespace SharpMCRewrite.Networking.Packages
{
	class HeldItemChange : Package<HeldItemChange>
	{
		public HeldItemChange(ClientWrapper client) : base(client)
		{
			SendId = 0x09;
			ReadId = 0x09;
		}

		public HeldItemChange(ClientWrapper client, MSGBuffer buffer) : base(client, buffer)
		{
			SendId = 0x09;
			ReadId = 0x09;
		}

		public override void Read()
		{
			byte slot = (byte)Buffer.ReadByte();
			Client.Player.CurrentSlot = slot;
		}

		public override void Write()
		{
			Buffer.WriteByte(Client.Player.CurrentSlot);
			Buffer.FlushData();
		}
	}
}