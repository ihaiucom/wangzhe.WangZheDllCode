using Assets.Scripts.Common;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AGE
{
	public abstract class BaseEvent : PooledClassObject
	{
		public int time;

		public Dictionary<int, bool> waitForConditions = new Dictionary<int, bool>();

		public Track track;

		public virtual bool bScaleStart
		{
			get
			{
				return true;
			}
		}

		public float timeSec
		{
			get
			{
				return ActionUtility.MsToSec(this.time);
			}
		}

		public BaseEvent()
		{
			this.bChkReset = false;
		}

		public override void OnUse()
		{
			base.OnUse();
			this.time = 0;
			this.waitForConditions = null;
			this.track = null;
		}

		public virtual bool SupportEditMode()
		{
			return false;
		}

		public virtual Dictionary<string, bool> GetAssociatedResources()
		{
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			Type type = base.GetType();
			Type typeFromHandle = typeof(string);
			List<AssetReferenceMeta> associatedResourcesMeta = Singleton<ActionMetaRepository>.instance.GetAssociatedResourcesMeta(type);
			if (associatedResourcesMeta != null)
			{
				for (int i = 0; i < associatedResourcesMeta.Count; i++)
				{
					AssetReferenceMeta assetReferenceMeta = associatedResourcesMeta[i];
					if (assetReferenceMeta.MetaFieldInfo.FieldType == typeFromHandle)
					{
						dictionary.Add(assetReferenceMeta.MetaFieldInfo.GetValue(this) as string, true);
					}
				}
			}
			return dictionary;
		}

		public virtual void GetAssociatedResources(Dictionary<object, AssetRefType> results, int markID)
		{
			Type type = base.GetType();
			Type typeFromHandle = typeof(string);
			Type typeFromHandle2 = typeof(int);
			Type typeFromHandle3 = typeof(uint);
			List<AssetReferenceMeta> associatedResourcesMeta = Singleton<ActionMetaRepository>.instance.GetAssociatedResourcesMeta(type);
			if (associatedResourcesMeta != null)
			{
				for (int i = 0; i < associatedResourcesMeta.Count; i++)
				{
					AssetReferenceMeta assetReferenceMeta = associatedResourcesMeta[i];
					switch (assetReferenceMeta.Reference.RefType)
					{
					case AssetRefType.Action:
					case AssetRefType.Prefab:
					case AssetRefType.Particle:
					case AssetRefType.Sound:
						if (assetReferenceMeta.MetaFieldInfo.FieldType == typeFromHandle)
						{
							string text = assetReferenceMeta.MetaFieldInfo.GetValue(this) as string;
							if (text != null && text.Length > 0 && !results.ContainsKey(text))
							{
								results.Add(text, assetReferenceMeta.Reference.RefType);
							}
						}
						break;
					case AssetRefType.SkillID:
					case AssetRefType.SkillCombine:
					case AssetRefType.MonsterConfigId:
					case AssetRefType.CallActorConfigId:
						if (assetReferenceMeta.MetaFieldInfo.FieldType == typeFromHandle2 || assetReferenceMeta.MetaFieldInfo.FieldType == typeFromHandle3)
						{
							object value = assetReferenceMeta.MetaFieldInfo.GetValue(this);
							int num = (int)value;
							if (num != 0)
							{
								ulong num2 = (ulong)((ulong)((long)assetReferenceMeta.Reference.RefType) << 32);
								ulong num3 = (ulong)num | num2;
								if (!results.ContainsKey(num3))
								{
									results.Add(num3, assetReferenceMeta.Reference.RefType);
								}
							}
						}
						break;
					}
				}
			}
		}

		public virtual List<string> GetAssociatedAction()
		{
			List<string> list = new List<string>();
			Type type = base.GetType();
			while (type == typeof(BaseEvent) || type.IsSubclassOf(typeof(BaseEvent)))
			{
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
				for (int i = 0; i < fields.Length; i++)
				{
					FieldInfo fieldInfo = fields[i];
					if (fieldInfo.FieldType == typeof(string) && Attribute.IsDefined(fieldInfo, typeof(ActionReference)))
					{
						string text = fieldInfo.GetValue(this) as string;
						if (text.Length > 0 && !list.Contains(text))
						{
							list.Add(text);
						}
					}
				}
				type = type.BaseType;
			}
			return list;
		}

		public abstract BaseEvent Clone();

		public virtual void OnLoaded()
		{
		}

		protected virtual void CopyData(BaseEvent src)
		{
			this.time = src.time;
			this.waitForConditions = src.waitForConditions;
		}

		public bool CheckConditions(Action _action)
		{
			if (this.waitForConditions != null)
			{
				Dictionary<int, bool>.Enumerator enumerator = this.waitForConditions.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, bool> current = enumerator.Current;
					int key = current.Key;
					if (key >= 0 && key < _action.GetConditionCount())
					{
						bool arg_5B_0 = _action.GetCondition(_action.GetTrack(key));
						KeyValuePair<int, bool> current2 = enumerator.Current;
						if (arg_5B_0 != current2.Value)
						{
							return false;
						}
					}
				}
			}
			return true;
		}
	}
}
