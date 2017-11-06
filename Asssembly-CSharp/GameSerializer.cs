using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;

public class GameSerializer
{
	public const string PREFABREF_ASSET_DIR = "PrefabAssets";

	private const string RESOURCES_DIR = "Assets/Resources";

	public const string DOM_ATTR_NAME_PREFAB = "PFB";

	public const string DOM_NODE_NAME_OBJNAME = "ON";

	public const string DOM_NODE_NAME_TRANSFORM = "T";

	public const string DOM_NODE_NAME_COMPONENT = "Cop";

	public const string DOM_NODE_NAME_CHILDNODE = "CHD";

	public const string DOM_ATTR_NAME_TRANSFORM_LOC_POS = "P";

	public const string DOM_ATTR_NAME_TRANSFORM_LOC_ROT = "R";

	public const string DOM_ATTR_NAME_TRANSFORM_LOC_SCL = "S";

	public const string DOM_ATTR_NAME_TRANSFORM_LAYER = "L";

	public const string DOM_ATTR_NAME_TRANSFORM_TAG = "Tag";

	public const string DOM_ATTR_NAME_TRANSFORM_ACTIVE = "A";

	public const string DOM_ATTR_NAME_OBJECT_TYPE = "Type";

	public const string DOM_ATTR_NAME_ARRAY_SIZE = "Size";

	public const string DOM_ATTR_NAME_VALUE = "V";

	public const string DOM_ATTR_NAME_JUDGETYPE = "JT";

	public const string DOM_ROOT_TAG = "root";

	public const string DOM_ATTR_IS_NULL = "NULL";

	public const string DOM_ATTR_DISABLE = "DIS";

	public const string DOM_LIGHTMAP_INFO = "LMI";

	public const string DOM_ATTR_LIGHTMAP_IDX = "I";

	public const string DOM_ATTR_LIGHTMAP_TILEOFFSET = "TO";

	private const string DOM_ATTR_JUDGETYPE_REF = "Ref";

	private const string DOM_ATTR_JUDGETYPE_ARRAY = "Arr";

	private const string DOM_ATTR_JUDGETYPE_PRIMITIVE = "Pri";

	private const string DOM_ATTR_JUDGETYPE_ENUM = "Enum";

	private const string DOM_ATTR_JUDGETYPE_CUSTOM = "Cus";

	private const string DOM_ATTR_JUDGETYPE_COMMON = "Com";

	public const string K_EXPORT_PATH_IN_RESOURCES_FOLDER = "SceneExport";

	public const string K_EXPORT_PATH = "/Resources/SceneExport";

	private const BindingFlags s_bindFlag = 62;

	public string m_domPath = Application.dataPath;

	private static GameObject s_gameObjectRoot4Read = null;

	private static int s_saveRecurTimes = 0;

	private ArrayList m_storedObjs = new ArrayList();

	private static string s_saveFailStr = string.Empty;

	private static Type[] s_ComponentsCannotSerialize = new Type[]
	{
		typeof(Rigidbody),
		typeof(Rigidbody2D),
		typeof(MeshFilter),
		typeof(Renderer),
		typeof(PhysicMaterial),
		typeof(Avatar),
		typeof(Material),
		typeof(Animator),
		typeof(MeshCollider)
	};

	private static Type[] s_FieldsCannotSerialize = new Type[]
	{
		typeof(Mesh)
	};

	private static DictionaryView<Type, ICustomizedComponentSerializer> s_componentSerializerTypeCache = null;

	private static DictionaryView<Type, ICustomizedObjectSerializer> s_objectSerializerTypeCache = null;

	private static Dictionary<string, Type> s_typeCache = new Dictionary<string, Type>();

	private static DictionaryView<Type, ICustomizedComponentSerializer> componentSerializerTypeCache
	{
		get
		{
			if (GameSerializer.s_componentSerializerTypeCache == null)
			{
				GameSerializer.s_componentSerializerTypeCache = new DictionaryView<Type, ICustomizedComponentSerializer>();
				ClassEnumerator classEnumerator = new ClassEnumerator(typeof(ComponentTypeSerializerAttribute), typeof(ICustomizedComponentSerializer), typeof(ComponentTypeSerializerAttribute).get_Assembly(), true, false, false);
				foreach (Type current in classEnumerator.results)
				{
					object[] customAttributes = current.GetCustomAttributes(typeof(ComponentTypeSerializerAttribute), true);
					for (int i = 0; i < customAttributes.Length; i++)
					{
						GameSerializer.s_componentSerializerTypeCache.Add((customAttributes[0] as ComponentTypeSerializerAttribute).type, Activator.CreateInstance(current) as ICustomizedComponentSerializer);
					}
				}
			}
			return GameSerializer.s_componentSerializerTypeCache;
		}
	}

	private static DictionaryView<Type, ICustomizedObjectSerializer> objectSerializerTypeCache
	{
		get
		{
			if (GameSerializer.s_objectSerializerTypeCache == null)
			{
				GameSerializer.s_objectSerializerTypeCache = new DictionaryView<Type, ICustomizedObjectSerializer>();
				ClassEnumerator classEnumerator = new ClassEnumerator(typeof(ObjectTypeSerializerAttribute), typeof(ICustomizedObjectSerializer), typeof(ObjectTypeSerializerAttribute).get_Assembly(), true, false, false);
				foreach (Type current in classEnumerator.results)
				{
					object[] customAttributes = current.GetCustomAttributes(typeof(ObjectTypeSerializerAttribute), true);
					for (int i = 0; i < customAttributes.Length; i++)
					{
						Type type = (customAttributes[0] as ObjectTypeSerializerAttribute).type;
						if (type.get_IsGenericType())
						{
							type = type.GetGenericTypeDefinition();
						}
						GameSerializer.s_objectSerializerTypeCache.Add(type, Activator.CreateInstance(current) as ICustomizedObjectSerializer);
					}
				}
			}
			return GameSerializer.s_objectSerializerTypeCache;
		}
	}

	private static ICustomizedComponentSerializer GetComponentSerlizer(Type type)
	{
		ICustomizedComponentSerializer result = null;
		if (GameSerializer.componentSerializerTypeCache.TryGetValue(type, out result))
		{
			return result;
		}
		return null;
	}

	private static ICustomizedObjectSerializer GetObjectSerlizer(Type type)
	{
		ICustomizedObjectSerializer result = null;
		if (type.get_IsGenericType())
		{
			type = type.GetGenericTypeDefinition();
		}
		if (GameSerializer.objectSerializerTypeCache.TryGetValue(type, out result))
		{
			return result;
		}
		return null;
	}

	private static bool isNull(object obj)
	{
		return object.ReferenceEquals(obj, null) || obj.Equals(null);
	}

	private static GameObject Load(BinaryDomDocument document)
	{
		GameObject gameObject = null;
		GameSerializer.s_gameObjectRoot4Read = null;
		BinaryNode root = document.Root;
		if (root != null)
		{
			gameObject = GameSerializer.LoadRecursionOnce(null, root);
			GameSerializer.s_gameObjectRoot4Read = gameObject;
			GameSerializer.LoadRecursionTwice(null, root, gameObject);
		}
		if (Camera.main != null)
		{
			Camera.SetupCurrent(Camera.main);
		}
		GameSerializer.s_gameObjectRoot4Read = null;
		return gameObject;
	}

	public GameObject Load(LevelResAsset lightmapAsset)
	{
		GameObject result = null;
		if (lightmapAsset != null)
		{
			if (!(lightmapAsset.levelDom != null))
			{
				return null;
			}
			TextAsset levelDom = lightmapAsset.levelDom;
			BinaryDomDocument document = new BinaryDomDocument(levelDom.bytes);
			result = GameSerializer.Load(document);
			int num = lightmapAsset.lightmapFar.Length;
			LightmapData[] array = new LightmapData[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = new LightmapData
				{
					lightmapFar = lightmapAsset.lightmapFar[i],
					lightmapNear = lightmapAsset.lightmapNear[i]
				};
			}
			LightmapSettings.lightmaps = array;
		}
		return result;
	}

	public GameObject Load(byte[] data)
	{
		BinaryDomDocument document = new BinaryDomDocument(data);
		return GameSerializer.Load(document);
	}

	private static GameObject LoadRecursionOnce(GameObject parentGo, BinaryNode domNode)
	{
		GameObject gameObject = null;
		string nodeAttr = GameSerializer.GetNodeAttr(domNode, "ON");
		if (parentGo != null)
		{
			for (int i = 0; i < parentGo.transform.childCount; i++)
			{
				if (parentGo.transform.GetChild(i).name.Equals(nodeAttr))
				{
					gameObject = parentGo.transform.GetChild(i).gameObject;
					break;
				}
			}
		}
		if (gameObject == null)
		{
			string nodeAttr2 = GameSerializer.GetNodeAttr(domNode, "PFB");
			if (nodeAttr2 != null && nodeAttr2.get_Length() != 0)
			{
				object resource = GameSerializer.GetResource(nodeAttr2, typeof(GameObject));
				if (resource == null || !(resource is GameObject))
				{
					Debug.LogError(nodeAttr2 + " 不存在或者类型错误，请重新导出该场景");
					gameObject = new GameObject();
				}
				else
				{
					GameObject gameObject2 = resource as GameObject;
					bool activeSelf = gameObject2.activeSelf;
					gameObject2.SetActive(false);
					gameObject = (GameObject)Object.Instantiate(gameObject2);
					gameObject2.SetActive(activeSelf);
				}
			}
			else
			{
				gameObject = new GameObject();
			}
		}
		Vector3 localScale = gameObject.transform.localScale;
		gameObject.name = GameSerializer.GetNodeAttr(domNode, "ON");
		if (parentGo != null)
		{
			gameObject.transform.parent = parentGo.transform;
		}
		gameObject.transform.localScale = localScale;
		GameSerializer.DeserializeObject(domNode, gameObject);
		gameObject.SetActive(false);
		GameSerializer.InitComponets(domNode, gameObject);
		for (int j = 0; j < domNode.GetChildNum(); j++)
		{
			BinaryNode child = domNode.GetChild(j);
			if (child.GetName() == "CHD")
			{
				GameSerializer.LoadRecursionOnce(gameObject, child);
			}
		}
		return gameObject;
	}

	private static void LoadRecursionTwice(GameObject parentGo, BinaryNode domNode, GameObject go)
	{
		if (domNode == domNode.OwnerDocument.Root)
		{
			GameSerializer.LoadComponets(domNode, go);
			int num = -1;
			for (int i = 0; i < domNode.GetChildNum(); i++)
			{
				BinaryNode child = domNode.GetChild(i);
				if (child.GetName() == "CHD")
				{
					num++;
					GameObject gameObject = go.transform.GetChild(num).gameObject;
					if (!(gameObject == null))
					{
						GameSerializer.LoadRecursionTwice(null, child, gameObject);
					}
				}
			}
		}
		else
		{
			BinaryNode parentNode = domNode.ParentNode;
			for (int j = 0; j < parentNode.GetChildNum(); j++)
			{
				BinaryNode child2 = parentNode.GetChild(j);
				if (!(child2.GetName() != "CHD"))
				{
					string attribute = GameSerializer.GetAttribute(child2, "ON");
					if (!(attribute != go.name))
					{
						GameSerializer.LoadComponets(child2, go);
						if (child2.GetChildNum() > 0 && go.transform.childCount > 0)
						{
							BinaryNode child3 = child2.GetChild(0);
							for (int k = 0; k < go.transform.childCount; k++)
							{
								GameObject gameObject2 = go.transform.GetChild(k).gameObject;
								GameSerializer.LoadRecursionTwice(null, child3, gameObject2);
							}
						}
						domNode = child2;
					}
				}
			}
		}
		string nodeAttr = GameSerializer.GetNodeAttr(domNode, "DIS");
		if (nodeAttr != null)
		{
			go.SetActive(false);
		}
		else
		{
			go.SetActive(true);
		}
	}

	private static void InitComponets(BinaryNode domNode, GameObject go)
	{
		Component[] components = go.GetComponents(typeof(Component));
		for (int i = 0; i < domNode.GetChildNum(); i++)
		{
			BinaryNode child = domNode.GetChild(i);
			if (!(child.GetName() != "Cop"))
			{
				string nodeAttr = GameSerializer.GetNodeAttr(child, "Type");
				Component x = null;
				for (int j = 0; j < components.Length; j++)
				{
					if (!GameSerializer.isNull(components[j]))
					{
						string pureType = GameSerializer.GetPureType(components[j].GetType().ToString());
						if (pureType.Equals(nodeAttr))
						{
							x = components[j];
							break;
						}
					}
				}
				if (x == null)
				{
					x = go.AddComponent(GameSerializer.GetType(nodeAttr));
				}
			}
		}
	}

	private static void LoadComponets(BinaryNode domNode, GameObject go)
	{
		for (int i = 0; i < domNode.GetChildNum(); i++)
		{
			BinaryNode child = domNode.GetChild(i);
			if (!(child.GetName() != "Cop"))
			{
				string nodeAttr = GameSerializer.GetNodeAttr(child, "Type");
				Component component = go.GetComponent(nodeAttr);
				if (!(component == null))
				{
					string nodeAttr2 = GameSerializer.GetNodeAttr(child, "DIS");
					if (nodeAttr2 != null && component is Behaviour)
					{
						Behaviour behaviour = (Behaviour)component;
						behaviour.enabled = false;
					}
					ICustomizedComponentSerializer componentSerlizer = GameSerializer.GetComponentSerlizer(component.GetType());
					if (componentSerlizer != null)
					{
						componentSerlizer.ComponentDeserialize(component, child);
					}
					else
					{
						MemberInfo[] members = component.GetType().GetMembers();
						for (int j = 0; j < members.Length; j++)
						{
							if (GameSerializer.IsMINeedExport(members[j]))
							{
								BinaryNode binaryNode = child.SelectSingleNode(members[j].get_Name());
								if (binaryNode != null)
								{
									object @object = GameSerializer.GetObject(binaryNode);
									try
									{
										if (@object != null)
										{
											GameSerializer.SetMIValue(members[j], component, @object);
										}
									}
									catch (Exception var_11_F4)
									{
									}
								}
							}
						}
					}
				}
			}
		}
	}

	public static object GetObject(BinaryNode currNode)
	{
		string nodeAttr = GameSerializer.GetNodeAttr(currNode, "NULL");
		object obj = null;
		if (nodeAttr != null)
		{
			return obj;
		}
		string nodeAttr2 = GameSerializer.GetNodeAttr(currNode, "Type");
		string nodeAttr3 = GameSerializer.GetNodeAttr(currNode, "V");
		string nodeAttr4 = GameSerializer.GetNodeAttr(currNode, "JT");
		if ("Arr".Equals(nodeAttr4))
		{
			if (nodeAttr2 != null)
			{
				string typeStr = nodeAttr2.Replace("[]", string.Empty);
				Type type = GameSerializer.GetType(typeStr);
				if (type == null)
				{
					Debug.LogError("Array type " + nodeAttr2 + " create failed!");
					return null;
				}
				Array array = Array.CreateInstance(type, currNode.GetChildNum());
				for (int i = 0; i < array.get_Length(); i++)
				{
					array.SetValue(GameSerializer.GetObject(currNode.GetChild(i)), i);
				}
				obj = array;
			}
		}
		else if ("Cus".Equals(nodeAttr4))
		{
			if (nodeAttr2 != null)
			{
				Type type2 = GameSerializer.GetType(nodeAttr2);
				ICustomizedObjectSerializer objectSerlizer = GameSerializer.GetObjectSerlizer(type2);
				if (objectSerlizer != null && objectSerlizer is ICustomInstantiate)
				{
					obj = ((ICustomInstantiate)objectSerlizer).Instantiate(currNode);
				}
				else
				{
					obj = GameSerializer.CreateInstance(type2);
				}
				if (obj == null)
				{
					return null;
				}
				if (objectSerlizer != null)
				{
					objectSerlizer.ObjectDeserialize(ref obj, currNode);
				}
			}
		}
		else if ("Enum".Equals(nodeAttr4))
		{
			if (nodeAttr2 != null)
			{
				Type type3 = GameSerializer.GetType(nodeAttr2);
				obj = Enum.ToObject(type3, int.Parse(nodeAttr3));
			}
		}
		else if ("Pri".Equals(nodeAttr4))
		{
			if (nodeAttr2 != null)
			{
				obj = Convert.ChangeType(nodeAttr3, GameSerializer.GetType(nodeAttr2));
			}
		}
		else if ("Ref".Equals(nodeAttr4))
		{
			Object gameObjectFromPath = GameSerializer.GetGameObjectFromPath(nodeAttr3, nodeAttr2);
			if (gameObjectFromPath != null)
			{
				if (gameObjectFromPath is GameObject)
				{
					if (nodeAttr2 != null)
					{
						string pureType = GameSerializer.GetPureType(nodeAttr2);
						if (!"GameObject".Equals(pureType))
						{
							obj = (gameObjectFromPath as GameObject).GetComponent(pureType);
							if (obj == null)
							{
								Debug.LogError("No " + pureType + " component found in " + nodeAttr3);
							}
						}
						else
						{
							obj = gameObjectFromPath;
						}
					}
				}
				else
				{
					obj = gameObjectFromPath;
				}
			}
			else
			{
				obj = null;
				Debug.LogError("Load gameobject " + nodeAttr3 + " failed!");
			}
		}
		else if ("Com".Equals(nodeAttr4))
		{
			obj = GameSerializer.CreateInstance(nodeAttr2);
			if (obj == null)
			{
				return null;
			}
			MemberInfo[] members = obj.GetType().GetMembers();
			for (int j = 0; j < members.Length; j++)
			{
				if (GameSerializer.IsMINeedExport(members[j]))
				{
					BinaryNode binaryNode = currNode.SelectSingleNode(members[j].get_Name());
					if (binaryNode != null)
					{
						try
						{
							object @object = GameSerializer.GetObject(binaryNode);
							if (binaryNode != null && @object != null)
							{
								GameSerializer.SetMIValue(members[j], obj, @object);
							}
						}
						catch (Exception ex)
						{
							Debug.LogError(string.Concat(new object[]
							{
								"Set field value failed! Field ",
								members[j].get_Name(),
								" in ",
								obj.GetType(),
								"e:",
								ex
							}));
						}
					}
				}
			}
		}
		return obj;
	}

	[DebuggerHidden]
	public static IEnumerator GetObjectAsync(BinaryNode currNode, ObjectHolder holder)
	{
		GameSerializer.<GetObjectAsync>c__Iterator38 <GetObjectAsync>c__Iterator = new GameSerializer.<GetObjectAsync>c__Iterator38();
		<GetObjectAsync>c__Iterator.currNode = currNode;
		<GetObjectAsync>c__Iterator.holder = holder;
		<GetObjectAsync>c__Iterator.<$>currNode = currNode;
		<GetObjectAsync>c__Iterator.<$>holder = holder;
		return <GetObjectAsync>c__Iterator;
	}

	[DebuggerHidden]
	private static IEnumerator LoadAsync(BinaryDomDocument document, ObjectHolder holder)
	{
		GameSerializer.<LoadAsync>c__Iterator39 <LoadAsync>c__Iterator = new GameSerializer.<LoadAsync>c__Iterator39();
		<LoadAsync>c__Iterator.document = document;
		<LoadAsync>c__Iterator.holder = holder;
		<LoadAsync>c__Iterator.<$>document = document;
		<LoadAsync>c__Iterator.<$>holder = holder;
		return <LoadAsync>c__Iterator;
	}

	[DebuggerHidden]
	public IEnumerator LoadAsync(LevelResAsset lightmapAsset, ObjectHolder holder)
	{
		GameSerializer.<LoadAsync>c__Iterator3A <LoadAsync>c__Iterator3A = new GameSerializer.<LoadAsync>c__Iterator3A();
		<LoadAsync>c__Iterator3A.lightmapAsset = lightmapAsset;
		<LoadAsync>c__Iterator3A.holder = holder;
		<LoadAsync>c__Iterator3A.<$>lightmapAsset = lightmapAsset;
		<LoadAsync>c__Iterator3A.<$>holder = holder;
		return <LoadAsync>c__Iterator3A;
	}

	[DebuggerHidden]
	public IEnumerator LoadAsync(byte[] data, ObjectHolder holder)
	{
		GameSerializer.<LoadAsync>c__Iterator3B <LoadAsync>c__Iterator3B = new GameSerializer.<LoadAsync>c__Iterator3B();
		<LoadAsync>c__Iterator3B.data = data;
		<LoadAsync>c__Iterator3B.holder = holder;
		<LoadAsync>c__Iterator3B.<$>data = data;
		<LoadAsync>c__Iterator3B.<$>holder = holder;
		return <LoadAsync>c__Iterator3B;
	}

	[DebuggerHidden]
	private static IEnumerator LoadComponetsAsync(BinaryNode domNode, GameObject go)
	{
		GameSerializer.<LoadComponetsAsync>c__Iterator3C <LoadComponetsAsync>c__Iterator3C = new GameSerializer.<LoadComponetsAsync>c__Iterator3C();
		<LoadComponetsAsync>c__Iterator3C.domNode = domNode;
		<LoadComponetsAsync>c__Iterator3C.go = go;
		<LoadComponetsAsync>c__Iterator3C.<$>domNode = domNode;
		<LoadComponetsAsync>c__Iterator3C.<$>go = go;
		return <LoadComponetsAsync>c__Iterator3C;
	}

	[DebuggerHidden]
	private static IEnumerator LoadRecursionOnceAsync(GameObject parentGo, BinaryNode domNode, ObjectHolder result)
	{
		GameSerializer.<LoadRecursionOnceAsync>c__Iterator3D <LoadRecursionOnceAsync>c__Iterator3D = new GameSerializer.<LoadRecursionOnceAsync>c__Iterator3D();
		<LoadRecursionOnceAsync>c__Iterator3D.domNode = domNode;
		<LoadRecursionOnceAsync>c__Iterator3D.parentGo = parentGo;
		<LoadRecursionOnceAsync>c__Iterator3D.result = result;
		<LoadRecursionOnceAsync>c__Iterator3D.<$>domNode = domNode;
		<LoadRecursionOnceAsync>c__Iterator3D.<$>parentGo = parentGo;
		<LoadRecursionOnceAsync>c__Iterator3D.<$>result = result;
		return <LoadRecursionOnceAsync>c__Iterator3D;
	}

	[DebuggerHidden]
	private static IEnumerator LoadRecursionTwiceAsync(GameObject parentGo, BinaryNode domNode, GameObject go)
	{
		GameSerializer.<LoadRecursionTwiceAsync>c__Iterator3E <LoadRecursionTwiceAsync>c__Iterator3E = new GameSerializer.<LoadRecursionTwiceAsync>c__Iterator3E();
		<LoadRecursionTwiceAsync>c__Iterator3E.domNode = domNode;
		<LoadRecursionTwiceAsync>c__Iterator3E.go = go;
		<LoadRecursionTwiceAsync>c__Iterator3E.<$>domNode = domNode;
		<LoadRecursionTwiceAsync>c__Iterator3E.<$>go = go;
		return <LoadRecursionTwiceAsync>c__Iterator3E;
	}

	public static object GetResource(string pathName, Type type)
	{
		object result = null;
		string text = pathName.Replace("\\", "/");
		if (text.Contains("PrefabAssets/"))
		{
			PrefabRefAsset prefabRefAsset = (PrefabRefAsset)Singleton<CResourceManager>.GetInstance().GetResource(text, typeof(PrefabRefAsset), enResourceType.BattleScene, false, false).m_content;
			if (prefabRefAsset != null)
			{
				result = prefabRefAsset.m_prefabObject;
			}
		}
		else
		{
			string fullPathInResources = pathName;
			string text2 = "Assets/Resources";
			int num = pathName.IndexOf(text2);
			if (num >= 0)
			{
				fullPathInResources = pathName.Substring(num + text2.get_Length() + 1);
			}
			result = Singleton<CResourceManager>.GetInstance().GetResource(fullPathInResources, type, enResourceType.BattleScene, false, false).m_content;
		}
		return result;
	}

	public static Object GetGameObjectFromPath(string pathname, string typeName = null)
	{
		if (string.IsNullOrEmpty(pathname))
		{
			return null;
		}
		Type type = typeof(GameObject);
		if (typeName != null)
		{
			type = GameSerializer.GetType(typeName);
		}
		Object @object = (Object)GameSerializer.GetResource(pathname, type);
		if (@object == null)
		{
			if (GameSerializer.s_gameObjectRoot4Read != null)
			{
				if (pathname == string.Format("/{0}", GameSerializer.s_gameObjectRoot4Read.name))
				{
					@object = GameSerializer.s_gameObjectRoot4Read;
				}
				else
				{
					string name = pathname.Replace(string.Format("/{0}/", GameSerializer.s_gameObjectRoot4Read.name), string.Empty);
					Transform transform = GameSerializer.s_gameObjectRoot4Read.transform.Find(name);
					if (transform != null)
					{
						@object = transform.gameObject;
					}
				}
			}
			else
			{
				@object = GameObject.Find(pathname);
			}
		}
		return @object;
	}

	private static void DeserializeObject(BinaryNode objInfoNode, GameObject go)
	{
		BinaryNode binaryNode = objInfoNode.SelectSingleNode("T");
		if (binaryNode != null)
		{
			try
			{
				byte[] binaryAttribute = GameSerializer.GetBinaryAttribute(binaryNode, "P");
				if (binaryAttribute != null)
				{
					go.transform.position = UnityBasetypeSerializer.BytesToVector3(binaryAttribute);
				}
				byte[] binaryAttribute2 = GameSerializer.GetBinaryAttribute(binaryNode, "R");
				if (binaryAttribute2 != null)
				{
					go.transform.rotation = UnityBasetypeSerializer.BytesToQuaternion(binaryAttribute2);
				}
				byte[] binaryAttribute3 = GameSerializer.GetBinaryAttribute(binaryNode, "S");
				if (binaryAttribute3 != null)
				{
					go.transform.localScale = UnityBasetypeSerializer.BytesToVector3(binaryAttribute3);
				}
				string attribute = GameSerializer.GetAttribute(binaryNode, "L");
				if (attribute != null)
				{
					go.layer = Convert.ToInt32(attribute);
				}
				string attribute2 = GameSerializer.GetAttribute(binaryNode, "Tag");
				if (attribute2 != null)
				{
					go.tag = attribute2;
				}
				string attribute3 = GameSerializer.GetAttribute(binaryNode, "A");
				if (attribute3 != null)
				{
					go.SetActive(attribute3.Equals("True"));
				}
			}
			catch (Exception)
			{
				DebugHelper.Assert(false, "Gameobject {0} transform load failed!", new object[]
				{
					go.name
				});
			}
		}
		BinaryNode binaryNode2 = objInfoNode.SelectSingleNode("LMI");
		if (binaryNode2 != null)
		{
			Renderer component = go.GetComponent<Renderer>();
			if (component != null)
			{
				string attribute4 = GameSerializer.GetAttribute(binaryNode2, "I");
				if (attribute4 != null)
				{
					component.lightmapIndex = Convert.ToInt32(attribute4);
				}
				else
				{
					component.lightmapIndex = -1;
				}
				byte[] binaryAttribute4 = GameSerializer.GetBinaryAttribute(binaryNode2, "TO");
				if (binaryAttribute4 != null)
				{
					component.lightmapTilingOffset = UnityBasetypeSerializer.BytesToVector4(binaryAttribute4);
				}
			}
		}
	}

	private string GetPathFromGameObj(GameObject obj, string org_path)
	{
		string text = org_path;
		if (text.Contains(obj.transform.name))
		{
			int num = text.IndexOf(obj.name) + obj.name.get_Length() + 1;
			text = text.Substring(num, text.get_Length() - num - 1);
		}
		return text;
	}

	public static GameObject FindRootGameObject(GameObject anyGo)
	{
		GameObject gameObject = anyGo;
		while (gameObject.transform.parent != null)
		{
			gameObject = gameObject.transform.parent.gameObject;
		}
		return gameObject;
	}

	public static string GetGameObjectPathName(GameObject go)
	{
		string text = string.Empty;
		if (go == null)
		{
			return "null";
		}
		if (text != null && text.get_Length() != 0)
		{
			return text;
		}
		try
		{
			text = "/" + go.name;
			GameObject gameObject = go;
			while (gameObject.transform.parent != null)
			{
				text = "/" + gameObject.transform.parent.name + text;
				gameObject = gameObject.transform.parent.gameObject;
			}
		}
		catch (Exception message)
		{
			Debug.Log("Get gameobject " + go.name + " path failed!");
			Debug.LogError(message);
			text = string.Empty;
		}
		return text;
	}

	private static bool IsPrimitive(Type type)
	{
		return type.get_IsPrimitive() || type == typeof(string) || type == typeof(decimal);
	}

	private static bool IsUnityReferenceType(Type type)
	{
		return GameSerializer.IsInherit(type, typeof(GameObject)) || GameSerializer.IsInherit(type, typeof(Component)) || GameSerializer.IsInherit(type, typeof(ScriptableObject)) || GameSerializer.IsInherit(type, typeof(Mesh)) || GameSerializer.IsInherit(type, typeof(Material));
	}

	private static bool IsComplexObject(object o)
	{
		return o != null && !(o is GameObject) && !(o is Component) && !o.GetType().get_IsValueType() && !(o is string) && o.GetType() != typeof(decimal);
	}

	private static bool IsGameObject(object obj)
	{
		return obj is GameObject;
	}

	public static string GetNodeAttr(BinaryNode node, string attrName)
	{
		string result = null;
		for (int i = 0; i < node.GetAttrNum(); i++)
		{
			BinaryAttr attr = node.GetAttr(i);
			if (attr.GetName() == attrName)
			{
				return attr.GetValueString();
			}
		}
		return result;
	}

	public static string GetPureType(object o)
	{
		if (o is Component)
		{
			return GameSerializer.GetPureType(o.GetType().ToString());
		}
		return o.GetType().ToString();
	}

	public static string GetPureType(string str)
	{
		string text = str;
		if (text.Contains("."))
		{
			text = text.Substring(text.LastIndexOf(".") + 1, text.get_Length() - text.LastIndexOf(".") - 1);
		}
		return text;
	}

	public static bool isStringEqual(string s1, string s2)
	{
		return s1 != null && s2 != null && s1.Equals(s2);
	}

	public static bool IsEqual(object o1, object o2)
	{
		if (o1 == null && o2 == null)
		{
			return true;
		}
		if (o1 == null || o2 == null)
		{
			return false;
		}
		if (o1.GetType() != o2.GetType())
		{
			return false;
		}
		if (GameSerializer.IsComplexObject(o1) || GameSerializer.IsComplexObject(o2))
		{
			return false;
		}
		if (GameSerializer.IsGameObject(o1) && GameSerializer.IsGameObject(o2))
		{
			GameObject go = (GameObject)o1;
			GameObject go2 = (GameObject)o2;
			string gameObjectPathName = GameSerializer.GetGameObjectPathName(go);
			string gameObjectPathName2 = GameSerializer.GetGameObjectPathName(go2);
			return gameObjectPathName != null && gameObjectPathName2 != null && gameObjectPathName.Equals(gameObjectPathName2);
		}
		if (!object.ReferenceEquals(o1.GetType(), o2.GetType()))
		{
			return false;
		}
		if (!o1.GetType().ToString().EndsWith("[]"))
		{
			return o1.Equals(o2);
		}
		Array array = (Array)o1;
		Array array2 = (Array)o2;
		if (array.GetLength(0) != array2.GetLength(0))
		{
			return false;
		}
		for (int i = 0; i < array.GetLength(0); i++)
		{
			if (array.GetValue(i) != array2.GetValue(i))
			{
				return false;
			}
		}
		return true;
	}

	public static string GetAttribute(BinaryNode node, string attName)
	{
		for (int i = 0; i < node.GetAttrNum(); i++)
		{
			BinaryAttr attr = node.GetAttr(i);
			if (attr.GetName() == attName)
			{
				return attr.GetValueString();
			}
		}
		return null;
	}

	public static byte[] GetBinaryAttribute(BinaryNode node, string attName)
	{
		for (int i = 0; i < node.GetAttrNum(); i++)
		{
			BinaryAttr attr = node.GetAttr(i);
			if (attr.GetName() == attName)
			{
				return attr.GetValue();
			}
		}
		return null;
	}

	private static bool IsMINeedExport(MemberInfo mi)
	{
		if (mi.get_MemberType() == 4)
		{
			FieldInfo fieldInfo = (FieldInfo)mi;
			return (!fieldInfo.get_IsLiteral() || fieldInfo.get_IsInitOnly()) && !fieldInfo.get_IsStatic();
		}
		if (mi.get_MemberType() == 16)
		{
			PropertyInfo propertyInfo = (PropertyInfo)mi;
			return propertyInfo.GetSetMethod() != null && propertyInfo.GetSetMethod().get_IsPublic();
		}
		return false;
	}

	private static void SetMIValue(MemberInfo mi, object owner, object value)
	{
		if (owner != null && mi != null && !owner.ToString().Equals("null"))
		{
			MemberTypes memberType = mi.get_MemberType();
			if (memberType != 4)
			{
				if (memberType == 16)
				{
					PropertyInfo propertyInfo = (PropertyInfo)mi;
					if (GameSerializer.GetMIType(mi).ToString().EndsWith("[]"))
					{
						object[] array = (object[])value;
						IList list = (IList)propertyInfo.GetValue(owner, null);
						for (int i = 0; i < array.Length; i++)
						{
							list.set_Item(i, array[i]);
						}
					}
					else
					{
						try
						{
							propertyInfo.SetValue(owner, value, null);
						}
						catch (Exception var_5_A4)
						{
						}
					}
				}
			}
			else
			{
				FieldInfo fieldInfo = (FieldInfo)mi;
				fieldInfo.SetValue(owner, value);
			}
		}
	}

	private static object GetMIValue(MemberInfo mi, object owner)
	{
		try
		{
			if (owner != null && mi != null)
			{
				MemberTypes memberType = mi.get_MemberType();
				if (memberType == 4)
				{
					FieldInfo fieldInfo = (FieldInfo)mi;
					object value = fieldInfo.GetValue(owner);
					object result = value;
					return result;
				}
				if (memberType == 16)
				{
					PropertyInfo propertyInfo = (PropertyInfo)mi;
					object value2 = propertyInfo.GetValue(owner, null);
					object result = value2;
					return result;
				}
			}
		}
		catch (Exception ex)
		{
			DebugHelper.Assert(mi != null);
			if (mi != null)
			{
				Debug.Log(string.Concat(new object[]
				{
					GameSerializer.GetMITypeStr(mi),
					mi.get_MemberType(),
					" ",
					mi.get_ReflectedType().ToString(),
					" ",
					mi.get_Name(),
					" seems not support !!!",
					ex
				}));
			}
		}
		return null;
	}

	private static string GetMITypeStr(MemberInfo mi)
	{
		if (mi != null)
		{
			MemberTypes memberType = mi.get_MemberType();
			if (memberType == 4)
			{
				FieldInfo fieldInfo = (FieldInfo)mi;
				return fieldInfo.get_FieldType().ToString();
			}
			if (memberType == 16)
			{
				PropertyInfo propertyInfo = (PropertyInfo)mi;
				return propertyInfo.get_PropertyType().ToString();
			}
		}
		return string.Empty;
	}

	private static Type GetMIType(MemberInfo mi)
	{
		if (mi != null)
		{
			MemberTypes memberType = mi.get_MemberType();
			if (memberType == 4)
			{
				FieldInfo fieldInfo = (FieldInfo)mi;
				return fieldInfo.get_FieldType();
			}
			if (memberType == 16)
			{
				PropertyInfo propertyInfo = (PropertyInfo)mi;
				return propertyInfo.get_PropertyType();
			}
		}
		return null;
	}

	public static string GetObjectHierachy(GameObject go)
	{
		string text = "/" + go.name;
		if (go.transform.parent != null)
		{
			text = GameSerializer.GetObjectHierachy(go.transform.parent.gameObject) + text;
		}
		return text;
	}

	private static bool CheckObjectLegal(GameObject go, Component[] cpnt, GameObject prefab)
	{
		if (prefab != null)
		{
			return true;
		}
		for (int i = 0; i < cpnt.Length; i++)
		{
			for (int j = 0; j < GameSerializer.s_ComponentsCannotSerialize.Length; j++)
			{
				if (cpnt[i] != null && cpnt[i].GetType() == GameSerializer.s_ComponentsCannotSerialize[j])
				{
					Debug.LogWarning(string.Concat(new string[]
					{
						"忽略保存对象:",
						GameSerializer.GetObjectHierachy(go),
						"，因为其有",
						GameSerializer.GetPureType(GameSerializer.s_ComponentsCannotSerialize[j].ToString()),
						"组件却不是Prefab"
					}));
					return false;
				}
			}
		}
		return true;
	}

	private static bool IsNeedNotSaveByType(Type tp, bool storeAsObject)
	{
		if (storeAsObject)
		{
			for (int i = 0; i < GameSerializer.s_FieldsCannotSerialize.Length; i++)
			{
				if (GameSerializer.IsInherit(tp, GameSerializer.s_FieldsCannotSerialize[i]))
				{
					return true;
				}
			}
			return false;
		}
		for (int j = 0; j < GameSerializer.s_ComponentsCannotSerialize.Length; j++)
		{
			if (GameSerializer.IsInherit(tp, GameSerializer.s_ComponentsCannotSerialize[j]))
			{
				return true;
			}
		}
		return false;
	}

	private static bool IsNeedNotSave(object o, bool storeAsObject)
	{
		return GameSerializer.IsNeedNotSaveByType(o.GetType(), storeAsObject);
	}

	private static bool IsNeedNotSaveMemberInfo(object o, MemberInfo mi, bool storeAsObject)
	{
		if (GameSerializer.IsNeedNotSaveByType(GameSerializer.GetMIType(mi), storeAsObject))
		{
			return true;
		}
		if (mi is PropertyInfo)
		{
			PropertyInfo propertyInfo = (PropertyInfo)mi;
			if (!propertyInfo.get_CanWrite())
			{
				return true;
			}
			if (propertyInfo.GetIndexParameters().Length != 0)
			{
				return true;
			}
		}
		return (o != null && !GameSerializer.IsUnityReferenceType(o.GetType()) && !GameSerializer.GetMITypeStr(mi).EndsWith("[]") && GameSerializer.GetMIValue(mi, o) == o) || (o != null && o is Component && (mi.get_Name().Equals("tag") || mi.get_Name().Equals("name") || mi.get_Name().Equals("active")));
	}

	private static bool IsInherit(Type type, Type BaseType)
	{
		if (type == BaseType)
		{
			return true;
		}
		if (BaseType.get_IsInterface())
		{
			return BaseType.IsAssignableFrom(type);
		}
		if (BaseType.get_IsValueType())
		{
			return false;
		}
		if (BaseType == typeof(Enum))
		{
			return type.get_IsEnum();
		}
		return type.IsSubclassOf(BaseType);
	}

	public static Type GetType(string typeStr)
	{
		Type result = null;
		if (GameSerializer.s_typeCache.TryGetValue(typeStr, ref result))
		{
			return result;
		}
		if (typeStr.Contains("`") && typeStr.Contains("[") && typeStr.Contains("]"))
		{
			string typeStr2 = typeStr.Substring(0, typeStr.IndexOf("["));
			int num = typeStr.IndexOf("[");
			int num2 = typeStr.LastIndexOf("]");
			string text = typeStr.Substring(num + 1, num2 - num - 1);
			string[] array = text.Split(new char[]
			{
				','
			});
			Type type = GameSerializer.GetType(typeStr2);
			Type[] array2 = new Type[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = GameSerializer.GetType(array[i]);
			}
			Type type2 = type.MakeGenericType(array2);
			GameSerializer.s_typeCache.Add(typeStr, type2);
			return type2;
		}
		Type type3 = Utility.GetType(typeStr);
		if (type3 == null)
		{
			Assembly[] assemblies = AppDomain.get_CurrentDomain().GetAssemblies();
			for (int j = 0; j < assemblies.Length; j++)
			{
				if (assemblies[j] != null)
				{
					Assembly assembly = assemblies[j];
					Type[] types = assembly.GetTypes();
					for (int k = 0; k < types.Length; k++)
					{
						if (typeStr == types[k].get_Name())
						{
							type3 = types[k];
							GameSerializer.s_typeCache.Add(typeStr, type3);
							return type3;
						}
					}
				}
			}
		}
		GameSerializer.s_typeCache.Add(typeStr, type3);
		return type3;
	}

	public static object CreateInstance(Type type)
	{
		object obj = null;
		try
		{
			obj = Activator.CreateInstance(type);
		}
		catch (Exception ex)
		{
			DebugHelper.Assert(obj != null, "{0} create failed. due to exception: {1}", new object[]
			{
				(type != null) ? type.ToString() : "UnkownType",
				ex
			});
		}
		return obj;
	}

	public static object CreateInstance(string typeStr)
	{
		return GameSerializer.CreateInstance(GameSerializer.GetType(typeStr));
	}
}
