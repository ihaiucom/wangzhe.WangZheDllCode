using System;

public class BinaryDomDocument
{
	private BinaryNode m_root;

	public BinaryNode Root
	{
		get
		{
			return this.m_root;
		}
	}

	public BinaryDomDocument(byte[] data)
	{
		this.m_root = new BinaryNode(data, 0, this, null);
	}
}
