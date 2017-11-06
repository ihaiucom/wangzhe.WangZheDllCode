using Pathfinding.Serialization.Zip;
using Pathfinding.Util;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Pathfinding.Serialization
{
	public class AstarSerializer
	{
		private sealed class _SerializeExtraInfoBytes_c__AnonStorey20
		{
			internal int totCount;

			internal BinaryWriter wr;

			internal bool __m__15(GraphNode node)
			{
				this.totCount = Math.Max(node.NodeIndex, this.totCount);
				if (node.NodeIndex == -1)
				{
					Debug.LogError("Graph contains destroyed nodes. This is a bug.");
				}
				return true;
			}
		}

		private const string binaryExt = ".binary";

		private const string jsonExt = ".binary";

		private AstarData data;

		private ZipFile zip;

		private MemoryStream str;

		private GraphMeta meta;

		private SerializeSettings settings;

		public NavGraph[] graphs;

		private uint checksum = 4294967295u;

		private static StringBuilder _stringBuilder = new StringBuilder();

		public AstarSerializer(AstarData data)
		{
			this.data = data;
			this.settings = SerializeSettings.Settings;
		}

		public AstarSerializer(AstarData data, SerializeSettings settings)
		{
			this.data = data;
			this.settings = settings;
		}

		private static StringBuilder GetStringBuilder()
		{
			AstarSerializer._stringBuilder.set_Length(0);
			return AstarSerializer._stringBuilder;
		}

		public void AddChecksum(byte[] bytes)
		{
			this.checksum = Checksum.GetChecksum(bytes, this.checksum);
		}

		public uint GetChecksum()
		{
			return this.checksum;
		}

		public void OpenSerialize()
		{
			this.zip = new ZipFile();
			this.zip.AlternateEncoding = Encoding.get_UTF8();
			this.zip.AlternateEncodingUsage = ZipOption.Always;
			this.meta = new GraphMeta();
		}

		public byte[] CloseSerialize()
		{
			byte[] array = this.SerializeMeta();
			this.AddChecksum(array);
			this.zip.AddEntry("meta.binary", array);
			MemoryStream memoryStream = new MemoryStream();
			this.zip.Save(memoryStream);
			array = memoryStream.ToArray();
			memoryStream.Dispose();
			this.zip.Dispose();
			this.zip = null;
			return array;
		}

		public void SerializeGraphs(NavGraph[] _graphs)
		{
			if (this.graphs != null)
			{
				throw new InvalidOperationException("Cannot serialize graphs multiple times.");
			}
			this.graphs = _graphs;
			if (this.zip == null)
			{
				throw new NullReferenceException("You must not call CloseSerialize before a call to this function");
			}
			if (this.graphs == null)
			{
				this.graphs = new NavGraph[0];
			}
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null)
				{
					byte[] bytes = this.Serialize(this.graphs[i]);
					this.AddChecksum(bytes);
					this.zip.AddEntry("graph" + i + ".binary", bytes);
				}
			}
		}

		public void SerializeUserConnections(UserConnection[] conns)
		{
		}

		private byte[] SerializeMeta()
		{
			this.meta.version = AstarPath.Version;
			this.meta.graphs = this.data.graphs.Length;
			this.meta.guids = new string[this.data.graphs.Length];
			this.meta.typeNames = new string[this.data.graphs.Length];
			this.meta.nodeCounts = new int[this.data.graphs.Length];
			for (int i = 0; i < this.data.graphs.Length; i++)
			{
				if (this.data.graphs[i] != null)
				{
					this.meta.guids[i] = this.data.graphs[i].guid.ToString();
					this.meta.typeNames[i] = this.data.graphs[i].GetType().get_FullName();
				}
			}
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
			binaryWriter.Write("A*");
			binaryWriter.Write(this.meta.version.get_Major());
			binaryWriter.Write(this.meta.version.get_Minor());
			binaryWriter.Write(this.meta.version.get_Build());
			binaryWriter.Write(this.meta.version.get_Revision());
			binaryWriter.Write(this.meta.graphs);
			binaryWriter.Write(this.meta.guids.Length);
			for (int j = 0; j < this.meta.guids.Length; j++)
			{
				binaryWriter.Write(this.meta.guids[j]);
			}
			binaryWriter.Write(this.meta.typeNames.Length);
			for (int k = 0; k < this.meta.typeNames.Length; k++)
			{
				binaryWriter.Write(this.meta.typeNames[k]);
			}
			binaryWriter.Write(this.meta.nodeCounts.Length);
			for (int l = 0; l < this.meta.nodeCounts.Length; l++)
			{
				binaryWriter.Write(this.meta.nodeCounts[l]);
			}
			return memoryStream.ToArray();
		}

		public byte[] Serialize(NavGraph graph)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(memoryStream);
			GraphSerializationContext ctx = new GraphSerializationContext(writer);
			graph.SerializeSettings(ctx);
			return memoryStream.ToArray();
		}

		public void SerializeNodes()
		{
			if (!this.settings.nodes)
			{
				return;
			}
			if (this.graphs == null)
			{
				throw new InvalidOperationException("Cannot serialize nodes with no serialized graphs (call SerializeGraphs first)");
			}
			for (int i = 0; i < this.graphs.Length; i++)
			{
				byte[] bytes = this.SerializeNodes(i);
				this.AddChecksum(bytes);
				this.zip.AddEntry("graph" + i + "_nodes.binary", bytes);
			}
			for (int j = 0; j < this.graphs.Length; j++)
			{
				byte[] bytes2 = this.SerializeNodeConnections(j);
				this.AddChecksum(bytes2);
				this.zip.AddEntry("graph" + j + "_conns.binary", bytes2);
			}
		}

		private byte[] SerializeNodes(int index)
		{
			return new byte[0];
		}

		public byte[] SerializeExtraInfoBytes()
		{
			AstarSerializer._SerializeExtraInfoBytes_c__AnonStorey20 _SerializeExtraInfoBytes_c__AnonStorey = new AstarSerializer._SerializeExtraInfoBytes_c__AnonStorey20();
			if (!this.settings.nodes)
			{
				return null;
			}
			_SerializeExtraInfoBytes_c__AnonStorey.totCount = 0;
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null)
				{
					this.graphs[i].GetNodes(delegate(GraphNode node)
					{
						_SerializeExtraInfoBytes_c__AnonStorey.totCount = Math.Max(node.NodeIndex, _SerializeExtraInfoBytes_c__AnonStorey.totCount);
						if (node.NodeIndex == -1)
						{
							Debug.LogError("Graph contains destroyed nodes. This is a bug.");
						}
						return true;
					});
				}
			}
			MemoryStream memoryStream = new MemoryStream();
			_SerializeExtraInfoBytes_c__AnonStorey.wr = new BinaryWriter(memoryStream);
			_SerializeExtraInfoBytes_c__AnonStorey.wr.Write(_SerializeExtraInfoBytes_c__AnonStorey.totCount);
			int c = 0;
			for (int j = 0; j < this.graphs.Length; j++)
			{
				if (this.graphs[j] != null)
				{
					this.graphs[j].GetNodes(delegate(GraphNode node)
					{
						c = Math.Max(node.NodeIndex, c);
						_SerializeExtraInfoBytes_c__AnonStorey.wr.Write(node.NodeIndex);
						return true;
					});
				}
			}
			if (c != _SerializeExtraInfoBytes_c__AnonStorey.totCount)
			{
				throw new Exception("Some graphs are not consistent in their GetNodes calls, sequential calls give different results.");
			}
			for (int k = 0; k < this.graphs.Length; k++)
			{
				if (this.graphs[k] != null)
				{
					GraphSerializationContext ctx = new GraphSerializationContext(_SerializeExtraInfoBytes_c__AnonStorey.wr);
					this.graphs[k].SerializeExtraInfo(ctx);
					ctx = new GraphSerializationContext(_SerializeExtraInfoBytes_c__AnonStorey.wr);
					this.graphs[k].GetNodes(delegate(GraphNode node)
					{
						node.SerializeReferences(ctx);
						return true;
					});
				}
			}
			_SerializeExtraInfoBytes_c__AnonStorey.wr.Close();
			return memoryStream.ToArray();
		}

		public void SerializeExtraInfo()
		{
			if (!this.settings.nodes)
			{
				return;
			}
			int totCount = 0;
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null)
				{
					this.graphs[i].GetNodes(delegate(GraphNode node)
					{
						totCount = Math.Max(node.NodeIndex, totCount);
						if (node.NodeIndex == -1)
						{
							Debug.LogError("Graph contains destroyed nodes. This is a bug.");
						}
						return true;
					});
				}
			}
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter wr = new BinaryWriter(memoryStream);
			wr.Write(totCount);
			int c = 0;
			for (int j = 0; j < this.graphs.Length; j++)
			{
				if (this.graphs[j] != null)
				{
					this.graphs[j].GetNodes(delegate(GraphNode node)
					{
						c = Math.Max(node.NodeIndex, c);
						wr.Write(node.NodeIndex);
						return true;
					});
				}
			}
			if (c != totCount)
			{
				throw new Exception("Some graphs are not consistent in their GetNodes calls, sequential calls give different results.");
			}
			byte[] bytes = memoryStream.ToArray();
			wr.Close();
			this.AddChecksum(bytes);
			this.zip.AddEntry("graph_references.binary", bytes);
			for (int k = 0; k < this.graphs.Length; k++)
			{
				if (this.graphs[k] != null)
				{
					MemoryStream memoryStream2 = new MemoryStream();
					BinaryWriter binaryWriter = new BinaryWriter(memoryStream2);
					GraphSerializationContext ctx = new GraphSerializationContext(binaryWriter);
					this.graphs[k].SerializeExtraInfo(ctx);
					byte[] bytes2 = memoryStream2.ToArray();
					binaryWriter.Close();
					this.AddChecksum(bytes2);
					this.zip.AddEntry("graph" + k + "_extra.binary", bytes2);
					memoryStream2 = new MemoryStream();
					binaryWriter = new BinaryWriter(memoryStream2);
					ctx = new GraphSerializationContext(binaryWriter);
					this.graphs[k].GetNodes(delegate(GraphNode node)
					{
						node.SerializeReferences(ctx);
						return true;
					});
					binaryWriter.Close();
					bytes2 = memoryStream2.ToArray();
					this.AddChecksum(bytes2);
					this.zip.AddEntry("graph" + k + "_references.binary", bytes2);
				}
			}
		}

		private byte[] SerializeNodeConnections(int index)
		{
			return new byte[0];
		}

		public void SerializeEditorSettings(GraphEditorBase[] editors)
		{
			if (editors == null || !this.settings.editorSettings)
			{
				return;
			}
		}

		public bool OpenDeserialize(byte[] bytes)
		{
			this.str = new MemoryStream();
			this.str.Write(bytes, 0, bytes.Length);
			this.str.set_Position(0L);
			try
			{
				this.zip = ZipFile.Read(this.str);
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Caught exception when loading from zip\n" + ex);
				this.str.Dispose();
				return false;
			}
			this.meta = this.DeserializeMeta(this.zip["meta.binary"]);
			if (this.meta.version > AstarPath.Version)
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Trying to load data from a newer version of the A* Pathfinding Project\nCurrent version: ",
					AstarPath.Version,
					" Data version: ",
					this.meta.version
				}));
			}
			else if (this.meta.version < AstarPath.Version)
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Trying to load data from an older version of the A* Pathfinding Project\nCurrent version: ",
					AstarPath.Version,
					" Data version: ",
					this.meta.version,
					"\nThis is usually fine, it just means you have upgraded to a new version.\nHowever node data (not settings) can get corrupted between versions, so it is recommendedto recalculate any caches (those for faster startup) and resave any files. Even if it seems to load fine, it might cause subtle bugs.\n"
				}));
			}
			return true;
		}

		public void CloseDeserialize()
		{
			this.str.Dispose();
			this.zip.Dispose();
			this.zip = null;
			this.str = null;
		}

		public NavGraph[] DeserializeGraphs()
		{
			this.graphs = new NavGraph[this.meta.graphs];
			int num = 0;
			for (int i = 0; i < this.meta.graphs; i++)
			{
				Type graphType = this.meta.GetGraphType(i);
				if (!object.Equals(graphType, null))
				{
					num++;
					ZipEntry zipEntry = this.zip["graph" + i + ".binary"];
					if (zipEntry == null)
					{
						throw new FileNotFoundException(string.Concat(new object[]
						{
							"Could not find data for graph ",
							i,
							" in zip. Entry 'graph+",
							i,
							".binary' does not exist"
						}));
					}
					NavGraph navGraph = this.data.CreateGraph(graphType);
					MemoryStream memoryStream = new MemoryStream();
					zipEntry.Extract(memoryStream);
					memoryStream.set_Position(0L);
					BinaryReader reader = new BinaryReader(memoryStream);
					GraphSerializationContext ctx = new GraphSerializationContext(reader, null, i);
					navGraph.DeserializeSettings(ctx);
					this.graphs[i] = navGraph;
					if (this.graphs[i].guid.ToString() != this.meta.guids[i])
					{
						throw new Exception("Guid in graph file not equal to guid defined in meta file. Have you edited the data manually?\n" + this.graphs[i].guid.ToString() + " != " + this.meta.guids[i]);
					}
				}
			}
			NavGraph[] array = new NavGraph[num];
			num = 0;
			for (int j = 0; j < this.graphs.Length; j++)
			{
				if (this.graphs[j] != null)
				{
					array[num] = this.graphs[j];
					num++;
				}
			}
			this.graphs = array;
			return this.graphs;
		}

		public UserConnection[] DeserializeUserConnections()
		{
			return new UserConnection[0];
		}

		public void DeserializeNodes()
		{
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] == null || this.zip.ContainsEntry("graph" + i + "_nodes.binary"))
				{
				}
			}
			for (int j = 0; j < this.graphs.Length; j++)
			{
				if (this.graphs[j] != null)
				{
					ZipEntry zipEntry = this.zip["graph" + j + "_nodes.binary"];
					if (zipEntry != null)
					{
						MemoryStream memoryStream = new MemoryStream();
						zipEntry.Extract(memoryStream);
						memoryStream.set_Position(0L);
						BinaryReader reader = new BinaryReader(memoryStream);
						this.DeserializeNodes(j, reader);
					}
				}
			}
			for (int k = 0; k < this.graphs.Length; k++)
			{
				if (this.graphs[k] != null)
				{
					ZipEntry zipEntry2 = this.zip["graph" + k + "_conns.binary"];
					if (zipEntry2 != null)
					{
						MemoryStream memoryStream2 = new MemoryStream();
						zipEntry2.Extract(memoryStream2);
						memoryStream2.set_Position(0L);
						BinaryReader reader2 = new BinaryReader(memoryStream2);
						this.DeserializeNodeConnections(k, reader2);
					}
				}
			}
		}

		public void DeserializeExtraInfo()
		{
			bool flag = false;
			for (int i = 0; i < this.graphs.Length; i++)
			{
				ZipEntry zipEntry = this.zip["graph" + i + "_extra.binary"];
				if (zipEntry != null)
				{
					flag = true;
					MemoryStream memoryStream = new MemoryStream();
					zipEntry.Extract(memoryStream);
					memoryStream.Seek(0L, 0);
					BinaryReader reader2 = new BinaryReader(memoryStream);
					GraphSerializationContext ctx2 = new GraphSerializationContext(reader2, null, i);
					this.graphs[i].DeserializeExtraInfo(ctx2);
				}
			}
			if (!flag)
			{
				return;
			}
			int totCount = 0;
			for (int j = 0; j < this.graphs.Length; j++)
			{
				if (this.graphs[j] != null)
				{
					this.graphs[j].GetNodes(delegate(GraphNode node)
					{
						totCount = Math.Max(node.NodeIndex, totCount);
						if (node.NodeIndex == -1)
						{
							Debug.LogError("Graph contains destroyed nodes. This is a bug.");
						}
						return true;
					});
				}
			}
			ZipEntry zipEntry2 = this.zip["graph_references.binary"];
			if (zipEntry2 == null)
			{
				throw new Exception("Node references not found in the data. Was this loaded from an older version of the A* Pathfinding Project?");
			}
			MemoryStream memoryStream2 = new MemoryStream();
			zipEntry2.Extract(memoryStream2);
			memoryStream2.Seek(0L, 0);
			BinaryReader reader = new BinaryReader(memoryStream2);
			int num = reader.ReadInt32();
			GraphNode[] int2Node = new GraphNode[num + 1];
			try
			{
				for (int k = 0; k < this.graphs.Length; k++)
				{
					if (this.graphs[k] != null)
					{
						this.graphs[k].GetNodes(delegate(GraphNode node)
						{
							int2Node[reader.ReadInt32()] = node;
							return true;
						});
					}
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Some graph(s) has thrown an exception during GetNodes, or some graph(s) have deserialized more or fewer nodes than were serialized", ex);
			}
			reader.Close();
			for (int l = 0; l < this.graphs.Length; l++)
			{
				if (this.graphs[l] != null)
				{
					zipEntry2 = this.zip["graph" + l + "_references.binary"];
					if (zipEntry2 == null)
					{
						throw new Exception("Node references for graph " + l + " not found in the data. Was this loaded from an older version of the A* Pathfinding Project?");
					}
					memoryStream2 = new MemoryStream();
					zipEntry2.Extract(memoryStream2);
					memoryStream2.Seek(0L, 0);
					reader = new BinaryReader(memoryStream2);
					GraphSerializationContext ctx = new GraphSerializationContext(reader, int2Node, l);
					this.graphs[l].GetNodes(delegate(GraphNode node)
					{
						node.DeserializeReferences(ctx);
						return true;
					});
				}
			}
		}

		public void PostDeserialization()
		{
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null)
				{
					this.graphs[i].PostDeserialization();
				}
			}
		}

		private void DeserializeNodes(int index, BinaryReader reader)
		{
		}

		private void DeserializeNodeConnections(int index, BinaryReader reader)
		{
		}

		public void DeserializeEditorSettings(GraphEditorBase[] graphEditors)
		{
		}

		private string GetString(ZipEntry entry)
		{
			MemoryStream memoryStream = new MemoryStream();
			entry.Extract(memoryStream);
			memoryStream.set_Position(0L);
			StreamReader streamReader = new StreamReader(memoryStream);
			string result = streamReader.ReadToEnd();
			memoryStream.set_Position(0L);
			streamReader.Dispose();
			return result;
		}

		private GraphMeta DeserializeMeta(ZipEntry entry)
		{
			if (entry == null)
			{
				throw new Exception("No metadata found in serialized data.");
			}
			GraphMeta graphMeta = new GraphMeta();
			MemoryStream memoryStream = new MemoryStream();
			entry.Extract(memoryStream);
			memoryStream.set_Position(0L);
			BinaryReader binaryReader = new BinaryReader(memoryStream);
			if (binaryReader.ReadString() != "A*")
			{
				throw new Exception("Invalid magic number in saved data");
			}
			int num = binaryReader.ReadInt32();
			int num2 = binaryReader.ReadInt32();
			int num3 = binaryReader.ReadInt32();
			int num4 = binaryReader.ReadInt32();
			if (num < 0)
			{
				graphMeta.version = new Version(0, 0);
			}
			else if (num2 < 0)
			{
				graphMeta.version = new Version(num, 0);
			}
			else if (num3 < 0)
			{
				graphMeta.version = new Version(num, num2);
			}
			else if (num4 < 0)
			{
				graphMeta.version = new Version(num, num2, num3);
			}
			else
			{
				graphMeta.version = new Version(num, num2, num3, num4);
			}
			graphMeta.graphs = binaryReader.ReadInt32();
			graphMeta.guids = new string[binaryReader.ReadInt32()];
			for (int i = 0; i < graphMeta.guids.Length; i++)
			{
				graphMeta.guids[i] = binaryReader.ReadString();
			}
			graphMeta.typeNames = new string[binaryReader.ReadInt32()];
			for (int j = 0; j < graphMeta.typeNames.Length; j++)
			{
				graphMeta.typeNames[j] = binaryReader.ReadString();
			}
			graphMeta.nodeCounts = new int[binaryReader.ReadInt32()];
			for (int k = 0; k < graphMeta.nodeCounts.Length; k++)
			{
				graphMeta.nodeCounts[k] = binaryReader.ReadInt32();
			}
			return graphMeta;
		}

		public static void SaveToFile(string path, byte[] data)
		{
			using (FileStream fileStream = new FileStream(path, 2))
			{
				fileStream.Write(data, 0, data.Length);
			}
		}

		public static byte[] LoadFromFile(string path)
		{
			byte[] result;
			using (FileStream fileStream = new FileStream(path, 3))
			{
				byte[] array = new byte[(int)fileStream.get_Length()];
				fileStream.Read(array, 0, (int)fileStream.get_Length());
				result = array;
			}
			return result;
		}
	}
}
