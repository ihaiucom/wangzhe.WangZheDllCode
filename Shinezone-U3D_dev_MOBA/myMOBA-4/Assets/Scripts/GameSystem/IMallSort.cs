using System;

namespace Assets.Scripts.GameSystem
{
	public interface IMallSort<T>
	{
		string GetSortTypeName(T sortType);

		void SetSortType(T sortType);

		T GetCurSortType();

		bool IsDesc();
	}
}
