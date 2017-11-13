using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using UnityEngine;

public class PhotonPingManager
{
	private const string wssProtocolString = "wss://";

	public bool UseNative;

	public static int Attempts = 5;

	public static bool IgnoreInitialAttempt = true;

	public static int MaxMilliseconsPerPing = 800;

	private int PingsRunning;

	public Region BestRegion
	{
		get
		{
			Region result = null;
			int num = 2147483647;
			using (List<Region>.Enumerator enumerator = PhotonNetwork.networkingPeer.AvailableRegions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Region current = enumerator.get_Current();
					Debug.Log("BestRegion checks region: " + current);
					if (current.Ping != 0 && current.Ping < num)
					{
						num = current.Ping;
						result = current;
					}
				}
			}
			return result;
		}
	}

	public bool Done
	{
		get
		{
			return this.PingsRunning == 0;
		}
	}

	[DebuggerHidden]
	public IEnumerator PingSocket(Region region)
	{
		PhotonPingManager.<PingSocket>c__Iterator6 <PingSocket>c__Iterator = new PhotonPingManager.<PingSocket>c__Iterator6();
		<PingSocket>c__Iterator.region = region;
		<PingSocket>c__Iterator.<$>region = region;
		<PingSocket>c__Iterator.<>f__this = this;
		return <PingSocket>c__Iterator;
	}

	public static string ResolveHost(string hostName)
	{
		string text = string.Empty;
		try
		{
			IPAddress[] hostAddresses = Dns.GetHostAddresses(hostName);
			if (hostAddresses.Length == 1)
			{
				string result = hostAddresses[0].ToString();
				return result;
			}
			for (int i = 0; i < hostAddresses.Length; i++)
			{
				IPAddress iPAddress = hostAddresses[i];
				if (iPAddress != null)
				{
					if (iPAddress.ToString().Contains(":"))
					{
						string result = iPAddress.ToString();
						return result;
					}
					if (string.IsNullOrEmpty(text))
					{
						text = hostAddresses.ToString();
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.Log("Exception caught! " + ex.get_Source() + " Message: " + ex.get_Message());
		}
		return text;
	}
}
