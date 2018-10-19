using System;
using System.Collections.Generic;

public class SLogObj
{
	private StreamWriterProxy streamWriter;

	private string filePath;

	private List<string> sLogTxt = new List<string>();

	private string targetPath;

	private string lastTargetPath;

	public string TargetPath
	{
		get
		{
			return this.targetPath;
		}
		set
		{
			this.targetPath = value;
			if (!string.IsNullOrEmpty(this.targetPath))
			{
				this.lastTargetPath = this.targetPath;
			}
			this.filePath = null;
		}
	}

	public string LastTargetPath
	{
		get
		{
			return this.lastTargetPath;
		}
	}

	public void Close()
	{
	}

	public void Log(string str)
	{
	}

	public void Flush()
	{
	}
}
