using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Apollo
{
	public class ApolloBufferReader
	{
		private byte[] buffer;

		private int position;

		public ApolloBufferReader()
		{
		}

		public ApolloBufferReader(byte[] bs)
		{
			this.buffer = bs;
		}

		public void Reset()
		{
			this.buffer = null;
			this.position = 0;
		}

		public bool Read(ref bool b)
		{
			byte b2 = (!b) ? (byte)0 : (byte)1;
			return this.Read(ref b2) != 0;
		}

		public byte Read(ref byte c)
		{
			if (this.buffer == null || this.position >= this.buffer.Length)
			{
				return 0;
			}
			c = this.buffer[this.position];
			this.position++;
			return c;
		}

		public byte[] Read(ref byte[] buf)
		{
			if (this.buffer == null || this.position >= this.buffer.Length)
			{
				return null;
			}
			int num = 0;
			this.Read(ref num);
			if (num > 0)
			{
				buf = new byte[num];
				Array.Copy(this.buffer, this.position, buf, 0, num);
				this.position += num;
				return buf;
			}
			return null;
		}

		public short Read(ref short v)
		{
			if (this.buffer == null || this.position >= this.buffer.Length)
			{
				return 0;
			}
			v = BitConverter.ToInt16(this.buffer, this.position);
			this.position += 2;
			v = ByteConverter.ReverseEndian(v);
			return v;
		}

		public ushort Read(ref ushort v)
		{
			if (this.buffer == null || this.position >= this.buffer.Length)
			{
				return 0;
			}
			v = BitConverter.ToUInt16(this.buffer, this.position);
			this.position += 2;
			v = ByteConverter.ReverseEndian(v);
			return v;
		}

		public int Read(ref int v)
		{
			if (this.buffer == null || this.position >= this.buffer.Length)
			{
				return 0;
			}
			v = BitConverter.ToInt32(this.buffer, this.position);
			this.position += 4;
			v = ByteConverter.ReverseEndian(v);
			return v;
		}

		public uint Read(ref uint v)
		{
			if (this.buffer == null || this.position >= this.buffer.Length)
			{
				return 0u;
			}
			v = BitConverter.ToUInt32(this.buffer, this.position);
			this.position += 4;
			v = ByteConverter.ReverseEndian(v);
			return v;
		}

		public long Read(ref long v)
		{
			if (this.buffer == null || this.position >= this.buffer.Length)
			{
				return 0L;
			}
			v = BitConverter.ToInt64(this.buffer, this.position);
			this.position += 8;
			v = ByteConverter.ReverseEndian(v);
			return v;
		}

		public ulong Read(ref ulong v)
		{
			if (this.buffer == null || this.position >= this.buffer.Length)
			{
				return 0uL;
			}
			v = BitConverter.ToUInt64(this.buffer, this.position);
			this.position += 8;
			v = ByteConverter.ReverseEndian(v);
			return v;
		}

		public string Read(ref string s)
		{
			if (this.buffer == null || this.position >= this.buffer.Length)
			{
				return null;
			}
			byte[] array = null;
			array = this.Read(ref array);
			if (array != null)
			{
				s = Encoding.UTF8.GetString(array);
				return s;
			}
			return null;
		}

		public IList<T> Read<T>(ref IList<T> v)
		{
			return this.Read<T>(ref v);
		}

		public IList ReadList<T>(ref T l)
		{
			int num = 0;
			this.Read(ref num);
			IList list = l as IList;
			if (list == null)
			{
				ADebug.LogError("ReadList list == null");
				return null;
			}
			list.Clear();
			for (int i = 0; i < num; i++)
			{
				object value = BasicClassTypeUtil.CreateListItem(list.GetType());
				this.Read<object>(ref value);
				list.Add(value);
			}
			return list;
		}

		public IDictionary<K, V> Read<K, V>(ref IDictionary<K, V> map)
		{
			return this.ReadMap<IDictionary<K, V>>(ref map) as IDictionary<K, V>;
		}

		public IDictionary ReadMap<T>(ref T map)
		{
			IDictionary dictionary = BasicClassTypeUtil.CreateObject(map.GetType()) as IDictionary;
			if (dictionary == null)
			{
				return null;
			}
			dictionary.Clear();
			int num = 0;
			this.Read(ref num);
			if (num <= 0)
			{
				return null;
			}
			Type type = dictionary.GetType();
			Type[] genericArguments = type.GetGenericArguments();
			if (genericArguments == null || genericArguments.Length < 2)
			{
				return null;
			}
			for (int i = 0; i < num; i++)
			{
				object key = BasicClassTypeUtil.CreateObject(genericArguments[0]);
				object value = BasicClassTypeUtil.CreateObject(genericArguments[1]);
				key = this.Read<object>(ref key);
				value = this.Read<object>(ref value);
				dictionary.Add(key, value);
			}
			map = (T)((object)dictionary);
			return dictionary;
		}

		public ApolloBufferBase Read(ref ApolloBufferBase ab)
		{
			if (ab != null)
			{
				ab.ReadFrom(this);
			}
			return ab;
		}

		public object Read<T>(ref T o)
		{
			if (o == null)
			{
				o = (T)((object)BasicClassTypeUtil.CreateObject<T>());
			}
			if (o is byte || o is char)
			{
				byte b = 0;
				o = (T)((object)this.Read(ref b));
			}
			else if (o is char)
			{
				byte b2 = 0;
				o = (T)((object)this.Read(ref b2));
			}
			else if (o is bool)
			{
				bool flag = false;
				o = (T)((object)this.Read(ref flag));
			}
			else if (o is short)
			{
				short num = 0;
				o = (T)((object)this.Read(ref num));
			}
			else if (o is ushort)
			{
				ushort num2 = 0;
				o = (T)((object)this.Read(ref num2));
			}
			else if (o is int)
			{
				int num3 = 0;
				o = (T)((object)this.Read(ref num3));
			}
			else
			{
				if (o is uint)
				{
					uint num4 = 0u;
					o = (T)((object)this.Read(ref num4));
					return o;
				}
				if (o is long)
				{
					long num5 = 0L;
					o = (T)((object)this.Read(ref num5));
					return o;
				}
				if (o is Enum)
				{
					int num6 = 0;
					o = (T)((object)this.Read(ref num6));
					return o;
				}
				if (o is ulong)
				{
					ulong num7 = 0uL;
					object obj = this.Read(ref num7);
					o = (T)((object)obj);
					return obj;
				}
				if (o is string)
				{
					string empty = string.Empty;
					o = (T)((object)this.Read(ref empty));
				}
				else if (o is ApolloBufferBase)
				{
					ApolloBufferBase apolloBufferBase = o as ApolloBufferBase;
					o = (T)((object)this.Read(ref apolloBufferBase));
				}
				else if (o == null || !o.GetType().IsArray)
				{
					if (o is IList)
					{
						return this.ReadList<T>(ref o);
					}
					if (o is IDictionary)
					{
						return this.ReadMap<T>(ref o);
					}
					throw new Exception(string.Concat(new object[]
					{
						"read object error: unsupport type:",
						o.GetType(),
						" value:",
						o.ToString()
					}));
				}
			}
			return o;
		}
	}
}
