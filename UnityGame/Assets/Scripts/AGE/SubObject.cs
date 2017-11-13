using System;
using UnityEngine;

namespace AGE
{
	public sealed class SubObject : Attribute
	{
		public static GameObject FindSubObject(GameObject _targetObject, string _subObjectNamePath)
		{
			if (_subObjectNamePath.IndexOf('/') >= 0)
			{
				Transform transform = _targetObject.transform.Find(_subObjectNamePath);
				if (transform)
				{
					return transform.gameObject;
				}
				return null;
			}
			else
			{
				Transform transform2 = _targetObject.transform.Find(_subObjectNamePath);
				if (transform2 == null)
				{
					for (int i = 0; i < _targetObject.transform.childCount; i++)
					{
						GameObject gameObject = SubObject.FindSubObject(_targetObject.transform.GetChild(i).gameObject, _subObjectNamePath);
						if (gameObject != null)
						{
							return gameObject;
						}
					}
					return null;
				}
				return transform2.gameObject;
			}
		}
	}
}
