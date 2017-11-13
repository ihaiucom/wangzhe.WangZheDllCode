using System;

public struct PhotonMessageInfo
{
	private readonly int timeInt;

	public readonly PhotonPlayer sender;

	public readonly PhotonView photonView;

	public double timestamp
	{
		get
		{
			uint num = (uint)this.timeInt;
			double num2 = num;
			return num2 / 1000.0;
		}
	}

	public PhotonMessageInfo(PhotonPlayer player, int timestamp, PhotonView view)
	{
		this.sender = player;
		this.timeInt = timestamp;
		this.photonView = view;
	}

	public override string ToString()
	{
		return string.Format("[PhotonMessageInfo: Sender='{1}' Senttime={0}]", this.timestamp, this.sender);
	}
}
