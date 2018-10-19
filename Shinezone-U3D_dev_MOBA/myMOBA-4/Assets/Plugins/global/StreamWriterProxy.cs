using System;
using System.IO;

public class StreamWriterProxy : IDisposable
{
	private StreamWriter Writer;

	private bool bValid;

	public bool isValid
	{
		get
		{
			return this.bValid;
		}
	}

	public StreamWriterProxy(string InFilePath, bool bInCreateNew)
	{
		StreamWriterProxy __f__this = this;
		Singleton<BackgroundWorker>.instance.AddBackgroudOperation(delegate
		{
			__f__this.Reset_MT(InFilePath, bInCreateNew);
		});
		this.bValid = true;
	}

	public void Flush()
	{
		Singleton<BackgroundWorker>.instance.AddBackgroudOperation(delegate
		{
			this.Flush_MT();
		});
	}

	public void Dispose()
	{
		Singleton<BackgroundWorker>.instance.AddBackgroudOperation(delegate
		{
			this.Dispose_MT();
		});
		this.bValid = false;
	}

	public void Close()
	{
		Singleton<BackgroundWorker>.instance.AddBackgroudOperation(delegate
		{
			this.Close_MT();
		});
	}

	public void WriteLine(string InText)
	{
		DebugHelper.Assert(this.bValid, "Should not WriteLine When the Proxy is not valid!");
		Singleton<BackgroundWorker>.instance.AddBackgroudOperation(delegate
		{
			this.WriteLine_MT(InText);
		});
	}

	protected void Flush_MT()
	{
		if (this.Writer != null)
		{
			this.Writer.Flush();
		}
	}

	protected void Reset_MT(string InFilePath, bool bInCreateNew)
	{
		try
		{
			FileStream stream;
			if (bInCreateNew)
			{
				stream = new FileStream(InFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
			}
			else
			{
				stream = new FileStream(InFilePath, FileMode.Append, FileAccess.Write, FileShare.Read);
			}
			this.Writer = new StreamWriter(stream);
			this.bValid = true;
		}
		catch (Exception)
		{
		}
	}

	protected void Dispose_MT()
	{
		if (this.Writer != null)
		{
			this.Writer.Dispose();
		}
	}

	protected void Close_MT()
	{
		if (this.Writer != null)
		{
			this.Writer.Close();
		}
	}

	protected void WriteLine_MT(string InText)
	{
		if (this.Writer != null)
		{
			this.Writer.WriteLine(InText);
		}
	}
}
