using System;

namespace Apollo
{
	public interface IQMi : IApolloServiceBase
	{
		void ShowQMi();

		void HideQMi();

		void SetGameEngineType(string gameEngineInfo);
	}
}
