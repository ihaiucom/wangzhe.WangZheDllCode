using System;

namespace ExitGames.Client.Photon.Chat
{
	public enum CustomAuthenticationType : byte
	{
		Custom,
		Steam,
		Facebook,
		Oculus,
		PlayStation,
		Xbox,
		None = 255
	}
}
