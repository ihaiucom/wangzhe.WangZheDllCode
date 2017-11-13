using System;

public class Region
{
	public CloudRegionCode Code;

	public string Cluster;

	public string HostAndPort;

	public int Ping;

	public Region(CloudRegionCode code)
	{
		this.Code = code;
		this.Cluster = code.ToString();
	}

	public Region(CloudRegionCode code, string regionCodeString, string address)
	{
		this.Code = code;
		this.Cluster = regionCodeString;
		this.HostAndPort = address;
	}

	public static CloudRegionCode Parse(string codeAsString)
	{
		if (codeAsString == null)
		{
			return CloudRegionCode.none;
		}
		int num = codeAsString.IndexOf('/');
		if (num > 0)
		{
			codeAsString = codeAsString.Substring(0, num);
		}
		codeAsString = codeAsString.ToLower();
		if (Enum.IsDefined(typeof(CloudRegionCode), codeAsString))
		{
			return (CloudRegionCode)((int)Enum.Parse(typeof(CloudRegionCode), codeAsString));
		}
		return CloudRegionCode.none;
	}

	internal static CloudRegionFlag ParseFlag(CloudRegionCode region)
	{
		if (Enum.IsDefined(typeof(CloudRegionFlag), region.ToString()))
		{
			return (CloudRegionFlag)((int)Enum.Parse(typeof(CloudRegionFlag), region.ToString()));
		}
		return (CloudRegionFlag)0;
	}

	[Obsolete]
	internal static CloudRegionFlag ParseFlag(string codeAsString)
	{
		codeAsString = codeAsString.ToLower();
		CloudRegionFlag result = (CloudRegionFlag)0;
		if (Enum.IsDefined(typeof(CloudRegionFlag), codeAsString))
		{
			result = (CloudRegionFlag)((int)Enum.Parse(typeof(CloudRegionFlag), codeAsString));
		}
		return result;
	}

	public override string ToString()
	{
		return string.Format("'{0}' \t{1}ms \t{2}", this.Cluster, this.Ping, this.HostAndPort);
	}
}
