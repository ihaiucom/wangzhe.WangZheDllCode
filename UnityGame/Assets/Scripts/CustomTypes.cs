using ExitGames.Client.Photon;
using System;
using UnityEngine;

internal static class CustomTypes
{
	public static readonly byte[] memVector3 = new byte[12];

	public static readonly byte[] memVector2 = new byte[8];

	public static readonly byte[] memQuarternion = new byte[16];

	public static readonly byte[] memPlayer = new byte[4];

	internal static void Register()
	{
		PhotonPeer.RegisterType(typeof(Vector2), 87, new SerializeStreamMethod(CustomTypes.SerializeVector2), new DeserializeStreamMethod(CustomTypes.DeserializeVector2));
		PhotonPeer.RegisterType(typeof(Vector3), 86, new SerializeStreamMethod(CustomTypes.SerializeVector3), new DeserializeStreamMethod(CustomTypes.DeserializeVector3));
		PhotonPeer.RegisterType(typeof(Quaternion), 81, new SerializeStreamMethod(CustomTypes.SerializeQuaternion), new DeserializeStreamMethod(CustomTypes.DeserializeQuaternion));
		PhotonPeer.RegisterType(typeof(PhotonPlayer), 80, new SerializeStreamMethod(CustomTypes.SerializePhotonPlayer), new DeserializeStreamMethod(CustomTypes.DeserializePhotonPlayer));
	}

	private static short SerializeVector3(StreamBuffer outStream, object customobject)
	{
		Vector3 vector = (Vector3)customobject;
		int num = 0;
		byte[] array = CustomTypes.memVector3;
		lock (array)
		{
			byte[] array2 = CustomTypes.memVector3;
			Protocol.Serialize(vector.x, array2, ref num);
			Protocol.Serialize(vector.y, array2, ref num);
			Protocol.Serialize(vector.z, array2, ref num);
			outStream.Write(array2, 0, 12);
		}
		return 12;
	}

	private static object DeserializeVector3(StreamBuffer inStream, short length)
	{
		Vector3 vector = default(Vector3);
		byte[] array = CustomTypes.memVector3;
		lock (array)
		{
			inStream.Read(CustomTypes.memVector3, 0, 12);
			int num = 0;
			Protocol.Deserialize(ref vector.x, CustomTypes.memVector3, ref num);
			Protocol.Deserialize(ref vector.y, CustomTypes.memVector3, ref num);
			Protocol.Deserialize(ref vector.z, CustomTypes.memVector3, ref num);
		}
		return vector;
	}

	private static short SerializeVector2(StreamBuffer outStream, object customobject)
	{
		Vector2 vector = (Vector2)customobject;
		byte[] array = CustomTypes.memVector2;
		lock (array)
		{
			byte[] array2 = CustomTypes.memVector2;
			int num = 0;
			Protocol.Serialize(vector.x, array2, ref num);
			Protocol.Serialize(vector.y, array2, ref num);
			outStream.Write(array2, 0, 8);
		}
		return 8;
	}

	private static object DeserializeVector2(StreamBuffer inStream, short length)
	{
		Vector2 vector = default(Vector2);
		byte[] array = CustomTypes.memVector2;
		lock (array)
		{
			inStream.Read(CustomTypes.memVector2, 0, 8);
			int num = 0;
			Protocol.Deserialize(ref vector.x, CustomTypes.memVector2, ref num);
			Protocol.Deserialize(ref vector.y, CustomTypes.memVector2, ref num);
		}
		return vector;
	}

	private static short SerializeQuaternion(StreamBuffer outStream, object customobject)
	{
		Quaternion quaternion = (Quaternion)customobject;
		byte[] array = CustomTypes.memQuarternion;
		lock (array)
		{
			byte[] array2 = CustomTypes.memQuarternion;
			int num = 0;
			Protocol.Serialize(quaternion.w, array2, ref num);
			Protocol.Serialize(quaternion.x, array2, ref num);
			Protocol.Serialize(quaternion.y, array2, ref num);
			Protocol.Serialize(quaternion.z, array2, ref num);
			outStream.Write(array2, 0, 16);
		}
		return 16;
	}

	private static object DeserializeQuaternion(StreamBuffer inStream, short length)
	{
		Quaternion quaternion = default(Quaternion);
		byte[] array = CustomTypes.memQuarternion;
		lock (array)
		{
			inStream.Read(CustomTypes.memQuarternion, 0, 16);
			int num = 0;
			Protocol.Deserialize(ref quaternion.w, CustomTypes.memQuarternion, ref num);
			Protocol.Deserialize(ref quaternion.x, CustomTypes.memQuarternion, ref num);
			Protocol.Deserialize(ref quaternion.y, CustomTypes.memQuarternion, ref num);
			Protocol.Deserialize(ref quaternion.z, CustomTypes.memQuarternion, ref num);
		}
		return quaternion;
	}

	private static short SerializePhotonPlayer(StreamBuffer outStream, object customobject)
	{
		int iD = ((PhotonPlayer)customobject).ID;
		byte[] array = CustomTypes.memPlayer;
		short result;
		lock (array)
		{
			byte[] array2 = CustomTypes.memPlayer;
			int num = 0;
			Protocol.Serialize(iD, array2, ref num);
			outStream.Write(array2, 0, 4);
			result = 4;
		}
		return result;
	}

	private static object DeserializePhotonPlayer(StreamBuffer inStream, short length)
	{
		byte[] array = CustomTypes.memPlayer;
		int num2;
		lock (array)
		{
			inStream.Read(CustomTypes.memPlayer, 0, (int)length);
			int num = 0;
			Protocol.Deserialize(ref num2, CustomTypes.memPlayer, ref num);
		}
		if (PhotonNetwork.networkingPeer.mActors.ContainsKey(num2))
		{
			return PhotonNetwork.networkingPeer.mActors.get_Item(num2);
		}
		return null;
	}
}
