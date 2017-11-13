using System;
using System.Collections.Generic;
using System.Reflection;

namespace AGE
{
	public class ActionMetaRepository : Singleton<ActionMetaRepository>
	{
		public DictionaryView<Type, List<AssetReferenceMeta>> Repositories = new DictionaryView<Type, List<AssetReferenceMeta>>();

		public List<AssetReferenceMeta> GetAssociatedResourcesMeta(Type InType)
		{
			if (InType != typeof(BaseEvent) && !InType.IsSubclassOf(typeof(BaseEvent)))
			{
				return null;
			}
			List<AssetReferenceMeta> list = null;
			if (this.Repositories.TryGetValue(InType, out list))
			{
				return list;
			}
			list = new List<AssetReferenceMeta>();
			this.Repositories.Add(InType, list);
			Type type = InType;
			while (type == typeof(BaseEvent) || type.IsSubclassOf(typeof(BaseEvent)))
			{
				FieldInfo[] fields = type.GetFields(20);
				if (fields != null)
				{
					for (int i = 0; i < fields.Length; i++)
					{
						FieldInfo fieldInfo = fields[i];
						AssetReference assetReference = Attribute.GetCustomAttribute(fieldInfo, typeof(AssetReference)) as AssetReference;
						if (assetReference != null)
						{
							AssetReferenceMeta assetReferenceMeta = default(AssetReferenceMeta);
							assetReferenceMeta.MetaFieldInfo = fieldInfo;
							assetReferenceMeta.Reference = assetReference;
							list.Add(assetReferenceMeta);
						}
					}
				}
				type = type.get_BaseType();
			}
			return list;
		}
	}
}
