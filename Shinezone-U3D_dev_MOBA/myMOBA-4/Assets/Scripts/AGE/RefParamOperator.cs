using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AGE
{
	public class RefParamOperator
	{
		public DictionaryView<string, SRefParam> refParamList = new DictionaryView<string, SRefParam>();

		public DictionaryView<string, ListView<RefData>> refDataList;

		public Action owner;

		public void ClearParams()
		{
			DictionaryView<string, SRefParam>.Enumerator enumerator = this.refParamList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, SRefParam> current = enumerator.Current;
				SRefParam value = current.Value;
				value.Destroy();
			}
			this.refParamList.Clear();
		}

		public void Reset()
		{
			this.owner = null;
			this.ClearParams();
		}

		public void AddRefParam(string name, bool value)
		{
			if (!this.refParamList.ContainsKey(name))
			{
				SRefParam sRefParam = SObjPool<SRefParam>.New();
				sRefParam.type = SRefParam.ValType.Bool;
				sRefParam.union._bool = value;
				this.refParamList.Add(name, sRefParam);
			}
		}

		public void AddRefParam(string name, int value)
		{
			if (!this.refParamList.ContainsKey(name))
			{
				SRefParam sRefParam = SObjPool<SRefParam>.New();
				sRefParam.type = SRefParam.ValType.Int;
				sRefParam.union._int = value;
				this.refParamList.Add(name, sRefParam);
			}
		}

		public void AddRefParam(string name, uint value)
		{
			if (!this.refParamList.ContainsKey(name))
			{
				SRefParam sRefParam = SObjPool<SRefParam>.New();
				sRefParam.type = SRefParam.ValType.UInt;
				sRefParam.union._uint = value;
				this.refParamList.Add(name, sRefParam);
			}
		}

		public void AddRefParam(string name, float value)
		{
			if (!this.refParamList.ContainsKey(name))
			{
				SRefParam sRefParam = SObjPool<SRefParam>.New();
				sRefParam.type = SRefParam.ValType.Float;
				sRefParam.union._float = value;
				this.refParamList.Add(name, sRefParam);
			}
		}

		public void AddRefParam(string name, VInt3 value)
		{
			if (!this.refParamList.ContainsKey(name))
			{
				SRefParam sRefParam = SObjPool<SRefParam>.New();
				sRefParam.type = SRefParam.ValType.VInt3;
				sRefParam.union._vint3 = value;
				this.refParamList.Add(name, sRefParam);
			}
		}

		public void AddRefParam(string name, Vector3 value)
		{
			if (!this.refParamList.ContainsKey(name))
			{
				SRefParam sRefParam = SObjPool<SRefParam>.New();
				sRefParam.type = SRefParam.ValType.Vector3;
				sRefParam.union._vec3 = value;
				this.refParamList.Add(name, sRefParam);
			}
		}

		public void AddRefParam(string name, Quaternion value)
		{
			if (!this.refParamList.ContainsKey(name))
			{
				SRefParam sRefParam = SObjPool<SRefParam>.New();
				sRefParam.type = SRefParam.ValType.Quaternion;
				sRefParam.union._quat = value;
				this.refParamList.Add(name, sRefParam);
			}
		}

		public void AddRefParam(string name, object value)
		{
			if (!this.refParamList.ContainsKey(name))
			{
				SRefParam sRefParam = SObjPool<SRefParam>.New();
				sRefParam.obj = value;
				sRefParam.type = SRefParam.ValType.Object;
				this.refParamList.Add(name, sRefParam);
			}
		}

		public void AddRefParam(string name, PoolObjHandle<ActorRoot> value)
		{
			if (!this.refParamList.ContainsKey(name))
			{
				SRefParam sRefParam = SObjPool<SRefParam>.New();
				sRefParam.handle = value;
				sRefParam.type = SRefParam.ValType.ActorRoot;
				this.refParamList.Add(name, sRefParam);
			}
		}

		public void SetRefParam(string name, bool value)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam))
			{
				sRefParam.union._bool = value;
				ListView<RefData> srcRefDataList = this.GetSrcRefDataList(name);
				if (srcRefDataList != null)
				{
					this.SetRefData(value, srcRefDataList);
				}
			}
		}

		public void SetRefParam(string name, int value)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam))
			{
				sRefParam.union._int = value;
				ListView<RefData> srcRefDataList = this.GetSrcRefDataList(name);
				if (srcRefDataList != null)
				{
					this.SetRefData(value, srcRefDataList);
				}
			}
		}

		public void SetRefParam(string name, uint value)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam))
			{
				sRefParam.union._uint = value;
				ListView<RefData> srcRefDataList = this.GetSrcRefDataList(name);
				if (srcRefDataList != null)
				{
					this.SetRefData(value, srcRefDataList);
				}
			}
		}

		public void SetRefParam(string name, float value)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam))
			{
				sRefParam.union._float = value;
				ListView<RefData> srcRefDataList = this.GetSrcRefDataList(name);
				if (srcRefDataList != null)
				{
					this.SetRefData(value, srcRefDataList);
				}
			}
		}

		public void SetRefParam(string name, VInt3 value)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam))
			{
				sRefParam.union._vint3 = value;
				ListView<RefData> srcRefDataList = this.GetSrcRefDataList(name);
				if (srcRefDataList != null)
				{
					this.SetRefData(value, srcRefDataList);
				}
			}
		}

		public void SetRefParam(string name, Vector3 value)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam))
			{
				sRefParam.union._vec3 = value;
				ListView<RefData> srcRefDataList = this.GetSrcRefDataList(name);
				if (srcRefDataList != null)
				{
					this.SetRefData(value, srcRefDataList);
				}
			}
		}

		public void SetRefParam(string name, Quaternion value)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam))
			{
				sRefParam.union._quat = value;
				ListView<RefData> srcRefDataList = this.GetSrcRefDataList(name);
				if (srcRefDataList != null)
				{
					this.SetRefData(value, srcRefDataList);
				}
			}
		}

		public void SetRefParam(string name, object value)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam))
			{
				sRefParam.obj = value;
				ListView<RefData> srcRefDataList = this.GetSrcRefDataList(name);
				if (srcRefDataList != null)
				{
					this.SetRefData(value, srcRefDataList);
				}
			}
		}

		public void SetRefParam(string name, PoolObjHandle<ActorRoot> value)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam))
			{
				sRefParam.handle = value;
				ListView<RefData> srcRefDataList = this.GetSrcRefDataList(name);
				if (srcRefDataList != null)
				{
					this.SetRefData(value, srcRefDataList);
				}
			}
		}

		public ListView<RefData> GetSrcRefDataList(string name)
		{
			ListView<RefData> result = null;
			if (this.owner != null && this.owner.refParamsSrc != null && this.owner.refParamsSrc.refDataList != null)
			{
				this.owner.refParamsSrc.refDataList.TryGetValue(name, out result);
			}
			return result;
		}

		public void SetRefData(object value, ListView<RefData> srcRefDataList)
		{
			for (int i = 0; i < srcRefDataList.Count; i++)
			{
				RefData refData = srcRefDataList[i];
				if (refData.eventIdx > -1)
				{
					Track track = this.owner.GetTrack(refData.trackIndex);
					BaseEvent obj = track.trackEvents[refData.eventIdx];
					refData.fieldInfo.SetValue(obj, value);
				}
				else
				{
					Track track2 = this.owner.GetTrack(refData.trackIndex);
					refData.fieldInfo.SetValue(track2, value);
				}
			}
		}

		public bool GetRefParam(string name, ref bool value)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam) && sRefParam.type == SRefParam.ValType.Bool)
			{
				value = sRefParam.union._bool;
				return true;
			}
			return false;
		}

		public bool GetRefParam(string name, ref int value)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam) && sRefParam.type == SRefParam.ValType.Int)
			{
				value = sRefParam.union._int;
				return true;
			}
			return false;
		}

		public bool GetRefParam(string name, ref uint value)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam) && sRefParam.type == SRefParam.ValType.UInt)
			{
				value = sRefParam.union._uint;
				return true;
			}
			return false;
		}

		public bool GetRefParam(string name, ref float value)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam) && sRefParam.type == SRefParam.ValType.Float)
			{
				value = sRefParam.union._float;
				return true;
			}
			return false;
		}

		public bool GetRefParam(string name, ref VInt3 value)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam) && sRefParam.type == SRefParam.ValType.VInt3)
			{
				value = sRefParam.union._vint3;
				return true;
			}
			return false;
		}

		public bool GetRefParam(string name, ref Vector3 value)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam) && sRefParam.type == SRefParam.ValType.Vector3)
			{
				value = sRefParam.union._vec3;
				return true;
			}
			return false;
		}

		public bool GetRefParam(string name, ref Quaternion value)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam) && sRefParam.type == SRefParam.ValType.Quaternion)
			{
				value = sRefParam.union._quat;
				return true;
			}
			return false;
		}

		public bool GetRefParam(string name, ref object value)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam) && sRefParam.type == SRefParam.ValType.Object)
			{
				value = sRefParam.obj;
				return true;
			}
			return false;
		}

		public bool GetRefParam(string name, ref PoolObjHandle<ActorRoot> value)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam) && sRefParam.type == SRefParam.ValType.ActorRoot)
			{
				value = sRefParam.handle;
				return true;
			}
			return false;
		}

		public void SetOrAddRefParam(string name, SRefParam param)
		{
			SRefParam sRefParam = null;
			if (this.refParamList.TryGetValue(name, out sRefParam))
			{
				if (sRefParam.type == param.type)
				{
					if (sRefParam.type < SRefParam.ValType.Object)
					{
						sRefParam.union._quat = param.union._quat;
					}
					else
					{
						sRefParam.obj = param.obj;
						if (sRefParam.type == SRefParam.ValType.ActorRoot)
						{
							sRefParam.union._uint = param.union._uint;
						}
					}
				}
			}
			else
			{
				this.refParamList.Add(name, param.Clone());
			}
		}

		public RefData AddRefData(string name, FieldInfo field, object data)
		{
			if (this.refDataList == null)
			{
				this.refDataList = new DictionaryView<string, ListView<RefData>>();
			}
			ListView<RefData> listView;
			if (!this.refDataList.TryGetValue(name, out listView))
			{
				listView = new ListView<RefData>();
				this.refDataList.Add(name, listView);
			}
			RefData refData = new RefData(field, data);
			listView.Add(refData);
			return refData;
		}

		public T GetRefParamObject<T>(string name) where T : class
		{
			object obj = null;
			if (this.GetRefParam(name, ref obj))
			{
				return obj as T;
			}
			return (T)((object)null);
		}
	}
}
