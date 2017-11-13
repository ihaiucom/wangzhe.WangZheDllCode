using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Pathfinding
{
	public class ObjImporter
	{
		private struct meshStruct
		{
			public Vector3[] vertices;

			public Vector3[] normals;

			public Vector2[] uv;

			public Vector2[] uv1;

			public Vector2[] uv2;

			public int[] triangles;

			public int[] faceVerts;

			public int[] faceUVs;

			public Vector3[] faceData;

			public string name;

			public string fileName;
		}

		public static Mesh ImportFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				Debug.LogError("No file was found at '" + filePath + "'");
				return null;
			}
			ObjImporter.meshStruct meshStruct = ObjImporter.createMeshStruct(filePath);
			ObjImporter.populateMeshStruct(ref meshStruct);
			Vector3[] array = new Vector3[meshStruct.faceData.Length];
			Vector2[] array2 = new Vector2[meshStruct.faceData.Length];
			Vector3[] array3 = new Vector3[meshStruct.faceData.Length];
			int num = 0;
			Vector3[] faceData = meshStruct.faceData;
			for (int i = 0; i < faceData.Length; i++)
			{
				Vector3 vector = faceData[i];
				array[num] = meshStruct.vertices[(int)vector.x - 1];
				if (vector.y >= 1f)
				{
					array2[num] = meshStruct.uv[(int)vector.y - 1];
				}
				if (vector.z >= 1f)
				{
					array3[num] = meshStruct.normals[(int)vector.z - 1];
				}
				num++;
			}
			Mesh mesh = new Mesh();
			mesh.vertices = array;
			mesh.uv = array2;
			mesh.normals = array3;
			mesh.triangles = meshStruct.triangles;
			mesh.RecalculateBounds();
			return mesh;
		}

		private static ObjImporter.meshStruct createMeshStruct(string filename)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			ObjImporter.meshStruct result = default(ObjImporter.meshStruct);
			result.fileName = filename;
			StreamReader streamReader = File.OpenText(filename);
			string text = streamReader.ReadToEnd();
			streamReader.Dispose();
			using (StringReader stringReader = new StringReader(text))
			{
				string text2 = stringReader.ReadLine();
				char[] array = new char[]
				{
					' '
				};
				while (text2 != null)
				{
					if (!text2.StartsWith("f ") && !text2.StartsWith("v ") && !text2.StartsWith("vt ") && !text2.StartsWith("vn "))
					{
						text2 = stringReader.ReadLine();
						if (text2 != null)
						{
							text2 = text2.Replace("  ", " ");
						}
					}
					else
					{
						text2 = text2.Trim();
						string[] array2 = text2.Split(array, 50);
						string text3 = array2[0];
						string text4 = text3;
						if (text4 != null)
						{
							if (ObjImporter.<>f__switch$map1 == null)
							{
								Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
								dictionary.Add("v", 0);
								dictionary.Add("vt", 1);
								dictionary.Add("vn", 2);
								dictionary.Add("f", 3);
								ObjImporter.<>f__switch$map1 = dictionary;
							}
							int num6;
							if (ObjImporter.<>f__switch$map1.TryGetValue(text4, ref num6))
							{
								switch (num6)
								{
								case 0:
									num2++;
									break;
								case 1:
									num3++;
									break;
								case 2:
									num4++;
									break;
								case 3:
									num5 = num5 + array2.Length - 1;
									num += 3 * (array2.Length - 2);
									break;
								}
							}
						}
						text2 = stringReader.ReadLine();
						if (text2 != null)
						{
							text2 = text2.Replace("  ", " ");
						}
					}
				}
			}
			result.triangles = new int[num];
			result.vertices = new Vector3[num2];
			result.uv = new Vector2[num3];
			result.normals = new Vector3[num4];
			result.faceData = new Vector3[num5];
			return result;
		}

		private static void populateMeshStruct(ref ObjImporter.meshStruct mesh)
		{
			StreamReader streamReader = File.OpenText(mesh.fileName);
			string text = streamReader.ReadToEnd();
			streamReader.Close();
			using (StringReader stringReader = new StringReader(text))
			{
				string text2 = stringReader.ReadLine();
				char[] array = new char[]
				{
					' '
				};
				char[] array2 = new char[]
				{
					'/'
				};
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				int num6 = 0;
				int num7 = 0;
				while (text2 != null)
				{
					if (!text2.StartsWith("f ") && !text2.StartsWith("v ") && !text2.StartsWith("vt ") && !text2.StartsWith("vn ") && !text2.StartsWith("g ") && !text2.StartsWith("usemtl ") && !text2.StartsWith("mtllib ") && !text2.StartsWith("vt1 ") && !text2.StartsWith("vt2 ") && !text2.StartsWith("vc ") && !text2.StartsWith("usemap "))
					{
						text2 = stringReader.ReadLine();
						if (text2 != null)
						{
							text2 = text2.Replace("  ", " ");
						}
					}
					else
					{
						text2 = text2.Trim();
						string[] array3 = text2.Split(array, 50);
						string text3 = array3[0];
						string text4 = text3;
						if (text4 != null)
						{
							if (ObjImporter.<>f__switch$map2 == null)
							{
								Dictionary<string, int> dictionary = new Dictionary<string, int>(6);
								dictionary.Add("v", 0);
								dictionary.Add("vt", 1);
								dictionary.Add("vt1", 2);
								dictionary.Add("vt2", 3);
								dictionary.Add("vn", 4);
								dictionary.Add("f", 5);
								ObjImporter.<>f__switch$map2 = dictionary;
							}
							int num8;
							if (ObjImporter.<>f__switch$map2.TryGetValue(text4, ref num8))
							{
								switch (num8)
								{
								case 0:
									mesh.vertices[num3] = new Vector3(Convert.ToSingle(array3[1]), Convert.ToSingle(array3[2]), Convert.ToSingle(array3[3]));
									num3++;
									break;
								case 1:
									mesh.uv[num5] = new Vector2(Convert.ToSingle(array3[1]), Convert.ToSingle(array3[2]));
									num5++;
									break;
								case 2:
									mesh.uv[num6] = new Vector2(Convert.ToSingle(array3[1]), Convert.ToSingle(array3[2]));
									num6++;
									break;
								case 3:
									mesh.uv[num7] = new Vector2(Convert.ToSingle(array3[1]), Convert.ToSingle(array3[2]));
									num7++;
									break;
								case 4:
									mesh.normals[num4] = new Vector3(Convert.ToSingle(array3[1]), Convert.ToSingle(array3[2]), Convert.ToSingle(array3[3]));
									num4++;
									break;
								case 5:
								{
									int num9 = 1;
									List<int> list = new List<int>();
									while (num9 < array3.Length && (string.Empty + array3[num9]).get_Length() > 0)
									{
										Vector3 vector = default(Vector3);
										string[] array4 = array3[num9].Split(array2, 3);
										vector.x = (float)Convert.ToInt32(array4[0]);
										if (array4.Length > 1)
										{
											if (array4[1] != string.Empty)
											{
												vector.y = (float)Convert.ToInt32(array4[1]);
											}
											vector.z = (float)Convert.ToInt32(array4[2]);
										}
										num9++;
										mesh.faceData[num2] = vector;
										list.Add(num2);
										num2++;
									}
									num9 = 1;
									while (num9 + 2 < array3.Length)
									{
										mesh.triangles[num] = list.get_Item(0);
										num++;
										mesh.triangles[num] = list.get_Item(num9);
										num++;
										mesh.triangles[num] = list.get_Item(num9 + 1);
										num++;
										num9++;
									}
									break;
								}
								}
							}
						}
						text2 = stringReader.ReadLine();
						if (text2 != null)
						{
							text2 = text2.Replace("  ", " ");
						}
					}
				}
			}
		}
	}
}
