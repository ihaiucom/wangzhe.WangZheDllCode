using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Apollo
{
	public class ApolloBufferWriter
	{
		private MemoryStream stream;

		private BinaryWriter writer;

		public ApolloBufferWriter()
		{
			this.stream = new MemoryStream(128);
			this.writer = new BinaryWriter(this.stream, Encoding.get_BigEndianUnicode());
		}

		public ApolloBufferWriter(int capacity)
		{
			this.stream = new MemoryStream(capacity);
			this.writer = new BinaryWriter(this.stream, Encoding.get_BigEndianUnicode());
		}

		public ApolloBufferWriter(MemoryStream ms)
		{
			this.stream = ms;
			this.writer = new BinaryWriter(this.stream, Encoding.get_BigEndianUnicode());
		}

		public void Write(bool b)
		{
			this.Write(b ? 1 : 0);
		}

		public void Write(byte c)
		{
			this.Reserve(1);
			this.writer.Write(c);
		}

		public void Write(short s)
		{
			this.Reserve(2);
			this.writer.Write(ByteConverter.ReverseEndian(s));
		}

		public void Write(ushort s)
		{
			this.Reserve(2);
			this.writer.Write(ByteConverter.ReverseEndian((short)s));
		}

		public void Write(int i)
		{
			this.Reserve(4);
			this.writer.Write(ByteConverter.ReverseEndian(i));
		}

		public void Write(uint i)
		{
			this.Reserve(4);
			this.writer.Write(ByteConverter.ReverseEndian(i));
		}

		public void Write(long l)
		{
			this.Reserve(8);
			this.writer.Write(ByteConverter.ReverseEndian(l));
		}

		public void Write(ulong l)
		{
			this.Reserve(8);
			this.writer.Write(ByteConverter.ReverseEndian(l));
		}

		public void Write(byte[] buffer)
		{
			if (buffer != null)
			{
				this.Write(buffer.Length);
				this.writer.Write(buffer);
			}
			else
			{
				this.Write(0);
			}
		}

		public void Write(string s)
		{
			byte[] array = ByteConverter.String2Bytes(s);
			if (array == null)
			{
				array = new byte[0];
			}
			int num = array.Length;
			this.Reserve(num + 4);
			this.Write(num);
			if (array.Length > 0)
			{
				this.writer.Write(array);
			}
		}

		public void Write<T>(List<T> v)
		{
			int i = (v != null) ? v.get_Count() : 0;
			this.Write(i);
			if (v != null)
			{
				for (int j = 0; j < v.get_Count(); j++)
				{
					this.Write(v.get_Item(j));
				}
			}
		}

		public void Write<K, V>(Dictionary<K, V> d)
		{
			if (d != null)
			{
				int count = d.get_Count();
				this.Write(count);
				using (Dictionary<K, V>.Enumerator enumerator = d.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<K, V> current = enumerator.get_Current();
						this.Write(current.get_Key());
						this.Write(current.get_Value());
					}
				}
			}
			else
			{
				this.Write(0);
			}
		}

		public void Write(ApolloBufferBase ab)
		{
			if (ab != null)
			{
				ab.WriteTo(this);
			}
		}

		public byte[] GetBufferData()
		{
			byte[] array = new byte[this.stream.get_Position()];
			Array.Copy(this.stream.GetBuffer(), 0L, array, 0L, this.stream.get_Position());
			return array;
		}

		public void Write(object o)
		{
			if (o is byte)
			{
				this.Write((byte)o);
			}
			else if (o is bool)
			{
				this.Write((bool)o);
			}
			else if (o is short)
			{
				this.Write((short)o);
			}
			else if (o is ushort)
			{
				this.Write((int)((ushort)o));
			}
			else if (o is int)
			{
				this.Write((int)o);
			}
			else if (o is uint)
			{
				this.Write((long)((ulong)((uint)o)));
			}
			else if (o is long)
			{
				this.Write((long)o);
			}
			else if (o is ulong)
			{
				this.Write((long)((ulong)o));
			}
			else if (o is float)
			{
				this.Write((float)o);
			}
			else if (o is double)
			{
				this.Write((double)o);
			}
			else if (o is string)
			{
				string s = o as string;
				this.Write(s);
			}
			else if (o is ApolloBufferBase)
			{
				this.Write((ApolloBufferBase)o);
			}
			else if (o is byte[])
			{
				this.Write((byte[])o);
			}
			else if (o is bool[])
			{
				this.Write((bool[])o);
			}
			else if (o is short[])
			{
				this.Write((short[])o);
			}
			else if (o is int[])
			{
				this.Write((int[])o);
			}
			else if (o is long[])
			{
				this.Write((long[])o);
			}
			else if (o is float[])
			{
				this.Write((float[])o);
			}
			else if (o is double[])
			{
				this.Write((double[])o);
			}
			else if (o.GetType().get_IsArray())
			{
				this.Write((object[])o);
			}
			else if (o is IList)
			{
				this.Write((IList)o);
			}
			else if (o is IDictionary)
			{
				this.Write((IDictionary)o);
			}
			else
			{
				if (!(o is IEnumerable))
				{
					throw new Exception("write object error: unsupport type. " + o.ToString() + "\n");
				}
				this.Write((int)o);
			}
		}

		private void Reserve(int len)
		{
			int num = this.stream.get_Capacity() - (int)this.stream.get_Length();
			if (num < len)
			{
				this.stream.set_Capacity(this.stream.get_Capacity() + len << 1);
			}
		}
	}
}
