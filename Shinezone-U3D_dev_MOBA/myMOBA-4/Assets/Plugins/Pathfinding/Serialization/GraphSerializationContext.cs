using System;
using System.IO;
using UnityEngine;

namespace Pathfinding.Serialization
{
	public class GraphSerializationContext
	{
		private readonly GraphNode[] id2NodeMapping;

		public readonly BinaryReader reader;

		public readonly BinaryWriter writer;

		public readonly int graphIndex;

		public GraphSerializationContext(BinaryReader reader, GraphNode[] id2NodeMapping, int graphIndex)
		{
			this.reader = reader;
			this.id2NodeMapping = id2NodeMapping;
			this.graphIndex = graphIndex;
		}

		public GraphSerializationContext(BinaryWriter writer)
		{
			this.writer = writer;
		}

		public int GetNodeIdentifier(GraphNode node)
		{
			return (node != null) ? node.NodeIndex : -1;
		}

		public GraphNode GetNodeFromIdentifier(int id)
		{
			if (this.id2NodeMapping == null)
			{
				throw new Exception("Calling GetNodeFromIdentifier when serializing");
			}
			if (id == -1)
			{
				return null;
			}
			GraphNode graphNode = this.id2NodeMapping[id];
			if (graphNode == null)
			{
				throw new Exception("Invalid id");
			}
			return graphNode;
		}

		public void SerializeVector3(Vector3 v)
		{
			this.writer.Write(v.x);
			this.writer.Write(v.y);
			this.writer.Write(v.z);
		}

		public Vector3 DeserializeVector3()
		{
			return new Vector3(this.reader.ReadSingle(), this.reader.ReadSingle(), this.reader.ReadSingle());
		}

		public void SerializeUnityObject(UnityEngine.Object ob)
		{
			if (ob == null)
			{
				this.writer.Write(2147483647);
				return;
			}
			int instanceID = ob.GetInstanceID();
			string name = ob.name;
			string assemblyQualifiedName = ob.GetType().AssemblyQualifiedName;
			string value = string.Empty;
			Component component = ob as Component;
			GameObject gameObject = ob as GameObject;
			if (component != null || gameObject != null)
			{
				if (component != null && gameObject == null)
				{
					gameObject = component.gameObject;
				}
				UnityReferenceHelper unityReferenceHelper = gameObject.GetComponent<UnityReferenceHelper>();
				if (unityReferenceHelper == null)
				{
					Debug.Log("Adding UnityReferenceHelper to Unity Reference '" + ob.name + "'");
					unityReferenceHelper = gameObject.AddComponent<UnityReferenceHelper>();
				}
				unityReferenceHelper.Reset();
				value = unityReferenceHelper.GetGUID();
			}
			this.writer.Write(instanceID);
			this.writer.Write(name);
			this.writer.Write(assemblyQualifiedName);
			this.writer.Write(value);
		}

		public UnityEngine.Object DeserializeUnityObject()
		{
			int num = this.reader.ReadInt32();
			if (num == 2147483647)
			{
				return null;
			}
			string text = this.reader.ReadString();
			string text2 = this.reader.ReadString();
			string text3 = this.reader.ReadString();
			Type type = UtilityPlugin.GetType(text2);
			if (type == null)
			{
				Debug.LogError("Could not find type '" + text2 + "'. Cannot deserialize Unity reference");
				return null;
			}
			if (!string.IsNullOrEmpty(text3))
			{
				UnityReferenceHelper[] array = UnityEngine.Object.FindObjectsOfType(typeof(UnityReferenceHelper)) as UnityReferenceHelper[];
				int i = 0;
				while (i < array.Length)
				{
					if (array[i].GetGUID() == text3)
					{
						if (type == typeof(GameObject))
						{
							return array[i].gameObject;
						}
						return array[i].GetComponent(type);
					}
					else
					{
						i++;
					}
				}
			}
			UnityEngine.Object[] array2 = Resources.LoadAll(text, type);
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j].name == text || array2.Length == 1)
				{
					return array2[j];
				}
			}
			return null;
		}
	}
}
