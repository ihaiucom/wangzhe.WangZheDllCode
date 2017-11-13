using System;
using UnityEngine;

namespace AGE
{
	public sealed class ObjectTemplate : Attribute
	{
		public Type[] dependencies;

		public bool dynamicObject;

		public ObjectTemplate(bool _dynamicObject)
		{
			this.dynamicObject = _dynamicObject;
		}

		public ObjectTemplate(params Type[] _dependencies)
		{
			this.dependencies = _dependencies;
		}

		public bool CheckForDependencies(GameObject _gameObject)
		{
			Type[] array = this.dependencies;
			for (int i = 0; i < array.Length; i++)
			{
				Type type = array[i];
				if (!_gameObject.GetComponent(type))
				{
					return false;
				}
			}
			return true;
		}
	}
}
