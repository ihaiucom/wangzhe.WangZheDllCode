using System;

namespace Apollo
{
	public interface IApolloHttpRequest
	{
		event OnRespondHandler ResponseEvent;

		ApolloResult SendRequest();

		void EnableAutoUpdate(bool enable);

		ApolloResult SetHttpVersion(string version);

		void SetTimeout(float timeout);

		void Update();

		string GetHttpVersion();

		ApolloResult SetURL(string URL);

		string GetURL();

		ApolloResult SetMethod(string method);

		string GetMethod();

		ApolloResult SetHeader(string name, string value);

		string GetHeader(string name);

		ApolloResult SetData(byte[] data);

		byte[] GetData();

		string ToString();

		IApolloTalker GetTalker();

		IApolloHttpResponse GetResponse();
	}
}
