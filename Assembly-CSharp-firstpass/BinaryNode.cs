using System;
using System.Text;

public class BinaryNode
{
	public static readonly int INT_LEN = 4;

	private byte[] m_data;

	private BinaryDomDocument m_owner;

	private BinaryNode m_parent;

	private int m_nameOffset;

	private int m_nameLen;

	private int m_attrOffset;

	private int m_attrNum;

	private int m_valueOffset;

	private int m_childOffset;

	private int m_childNum;

	public BinaryDomDocument OwnerDocument
	{
		get
		{
			return this.m_owner;
		}
	}

	public BinaryNode ParentNode
	{
		get
		{
			return this.m_parent;
		}
	}

	internal BinaryNode(byte[] data, int offset, BinaryDomDocument owner, BinaryNode parent)
	{
		this.m_data = data;
		this.m_owner = owner;
		this.m_parent = parent;
		BitConverter.ToInt32(data, offset);
		this.m_nameOffset = offset + BinaryNode.INT_LEN;
		this.m_nameLen = BitConverter.ToInt32(data, this.m_nameOffset) - BinaryNode.INT_LEN;
		this.m_attrOffset = this.m_nameOffset + this.m_nameLen + BinaryNode.INT_LEN;
		int num = BitConverter.ToInt32(data, this.m_attrOffset);
		if (num > BinaryNode.INT_LEN)
		{
			this.m_attrNum = BitConverter.ToInt32(data, this.m_attrOffset + BinaryNode.INT_LEN);
		}
		else
		{
			this.m_attrNum = 0;
		}
		this.m_valueOffset = this.m_attrOffset + num;
		int num2 = BitConverter.ToInt32(data, this.m_valueOffset);
		this.m_childOffset = this.m_valueOffset + num2;
		int num3 = BitConverter.ToInt32(data, this.m_childOffset);
		if (num3 > BinaryNode.INT_LEN)
		{
			this.m_childNum = BitConverter.ToInt32(data, this.m_childOffset + BinaryNode.INT_LEN);
		}
		else
		{
			this.m_childNum = 0;
		}
	}

	public BinaryNode SelectSingleNode(string name)
	{
		for (int i = 0; i < this.GetChildNum(); i++)
		{
			BinaryNode child = this.GetChild(i);
			if (child.GetName() == name)
			{
				return child;
			}
		}
		return null;
	}

	public int GetAttrNum()
	{
		return this.m_attrNum;
	}

	public BinaryAttr GetAttr(int index)
	{
		DebugHelper.Assert(index < this.m_attrNum);
		int num = this.m_attrOffset + BinaryNode.INT_LEN + BinaryNode.INT_LEN;
		for (int i = 0; i < index; i++)
		{
			int num2 = BitConverter.ToInt32(this.m_data, num);
			num += num2;
		}
		return new BinaryAttr(this.m_data, num);
	}

	public int GetChildNum()
	{
		return this.m_childNum;
	}

	public BinaryNode GetChild(int index)
	{
		DebugHelper.Assert(index < this.m_childNum);
		int num = this.m_childOffset + BinaryNode.INT_LEN + BinaryNode.INT_LEN;
		for (int i = 0; i < index; i++)
		{
			int num2 = BitConverter.ToInt32(this.m_data, num);
			num += num2;
		}
		return new BinaryNode(this.m_data, num, this.OwnerDocument, this);
	}

	public byte[] GetValue()
	{
		int num = BitConverter.ToInt32(this.m_data, this.m_valueOffset);
		if (num == BinaryNode.INT_LEN)
		{
			return null;
		}
		byte[] array = new byte[num - BinaryNode.INT_LEN];
		Array.Copy(this.m_data, this.m_valueOffset + BinaryNode.INT_LEN, array, 0, array.Length);
		return array;
	}

	public string GetValueString()
	{
		return this.GetValueString(Encoding.get_UTF8());
	}

	public string GetValueString(Encoding encoding)
	{
		int num = BitConverter.ToInt32(this.m_data, this.m_valueOffset);
		if (num == BinaryNode.INT_LEN)
		{
			return null;
		}
		return Encoding.get_UTF8().GetString(this.m_data, this.m_valueOffset + BinaryNode.INT_LEN, num - BinaryNode.INT_LEN);
	}

	public string GetName()
	{
		return Encoding.get_UTF8().GetString(this.m_data, this.m_nameOffset + BinaryNode.INT_LEN, this.m_nameLen);
	}
}
