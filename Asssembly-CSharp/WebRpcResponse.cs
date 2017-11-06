using ExitGames.Client.Photon;
using System;
using System.Collections.Generic;

public class WebRpcResponse
{
	public string Name
	{
		get;
		private set;
	}

	public int ReturnCode
	{
		get;
		private set;
	}

	public string DebugMessage
	{
		get;
		private set;
	}

	public Dictionary<string, object> Parameters
	{
		get;
		private set;
	}

	public WebRpcResponse(OperationResponse response)
	{
		object obj;
		response.Parameters.TryGetValue(209, ref obj);
		this.Name = (obj as string);
		response.Parameters.TryGetValue(207, ref obj);
		this.ReturnCode = ((obj == null) ? -1 : ((int)((byte)obj)));
		response.Parameters.TryGetValue(208, ref obj);
		this.Parameters = (obj as Dictionary<string, object>);
		response.Parameters.TryGetValue(206, ref obj);
		this.DebugMessage = (obj as string);
	}

	public string ToStringFull()
	{
		return string.Format("{0}={2}: {1} \"{3}\"", new object[]
		{
			this.Name,
			SupportClass.DictionaryToString(this.Parameters),
			this.ReturnCode,
			this.DebugMessage
		});
	}
}
