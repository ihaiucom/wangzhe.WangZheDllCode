using System;

namespace Apollo
{
	public interface IApolloHttpResponse
	{
		string GetHttpVersion();

		string GetStatus();

		string GetStatusMessage();

		string GetHeader(string name);

		byte[] GetData();

		string ToString();
	}
}
