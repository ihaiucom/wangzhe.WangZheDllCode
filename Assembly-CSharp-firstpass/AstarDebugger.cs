using Pathfinding;
using Pathfinding.Util;
using System;
using System.Text;
using UnityEngine;

[AddComponentMenu("Pathfinding/Debugger"), ExecuteInEditMode]
public class AstarDebugger : MonoBehaviour
{
	private struct GraphPoint
	{
		public float fps;

		public float memory;

		public bool collectEvent;
	}

	private struct PathTypeDebug
	{
		private string name;

		private Func<int> getSize;

		private Func<int> getTotalCreated;

		public PathTypeDebug(string name, Func<int> getSize, Func<int> getTotalCreated)
		{
			this.name = name;
			this.getSize = getSize;
			this.getTotalCreated = getTotalCreated;
		}

		public void Print(StringBuilder text)
		{
			int num = this.getTotalCreated.Invoke();
			if (num > 0)
			{
				text.Append("\n").Append(("  " + this.name).PadRight(25)).Append(this.getSize.Invoke()).Append("/").Append(num);
			}
		}
	}

	public int yOffset = 5;

	public bool show = true;

	public bool showInEditor;

	public bool showFPS;

	public bool showPathProfile;

	public bool showMemProfile;

	public bool showGraph;

	public int graphBufferSize = 200;

	public Font font;

	public int fontSize = 12;

	private StringBuilder text = new StringBuilder();

	private string cachedText;

	private float lastUpdate = -999f;

	private AstarDebugger.GraphPoint[] graph;

	private float delayedDeltaTime = 1f;

	private float lastCollect;

	private float lastCollectNum;

	private float delta;

	private float lastDeltaTime;

	private int allocRate;

	private int lastAllocMemory;

	private float lastAllocSet = -9999f;

	private int allocMem;

	private int collectAlloc;

	private int peakAlloc;

	private int fpsDropCounterSize = 200;

	private float[] fpsDrops;

	private Rect boxRect;

	private GUIStyle style;

	private Camera cam;

	private float graphWidth = 100f;

	private float graphHeight = 100f;

	private float graphOffset = 50f;

	private int maxVecPool;

	private int maxNodePool;

	private AstarDebugger.PathTypeDebug[] debugTypes = new AstarDebugger.PathTypeDebug[]
	{
		new AstarDebugger.PathTypeDebug("ABPath", new Func<int>(PathPool<ABPath>.GetSize), new Func<int>(PathPool<ABPath>.GetTotalCreated))
	};

	public void Start()
	{
		base.useGUILayout = false;
		this.fpsDrops = new float[this.fpsDropCounterSize];
		if (base.GetComponent<Camera>() != null)
		{
			this.cam = base.GetComponent<Camera>();
		}
		else
		{
			this.cam = Camera.main;
		}
		this.graph = new AstarDebugger.GraphPoint[this.graphBufferSize];
		for (int i = 0; i < this.fpsDrops.Length; i++)
		{
			this.fpsDrops[i] = 1f / Time.deltaTime;
		}
	}

	public void Update()
	{
		if (!this.show || (!Application.isPlaying && !this.showInEditor))
		{
			return;
		}
		int num = GC.CollectionCount(0);
		if (this.lastCollectNum != (float)num)
		{
			this.lastCollectNum = (float)num;
			this.delta = Time.realtimeSinceStartup - this.lastCollect;
			this.lastCollect = Time.realtimeSinceStartup;
			this.lastDeltaTime = Time.deltaTime;
			this.collectAlloc = this.allocMem;
		}
		this.allocMem = (int)GC.GetTotalMemory(false);
		bool flag = this.allocMem < this.peakAlloc;
		this.peakAlloc = ((!flag) ? this.allocMem : this.peakAlloc);
		if (Time.realtimeSinceStartup - this.lastAllocSet > 0.3f || !Application.isPlaying)
		{
			int num2 = this.allocMem - this.lastAllocMemory;
			this.lastAllocMemory = this.allocMem;
			this.lastAllocSet = Time.realtimeSinceStartup;
			this.delayedDeltaTime = Time.deltaTime;
			if (num2 >= 0)
			{
				this.allocRate = num2;
			}
		}
		if (Application.isPlaying)
		{
			this.fpsDrops[Time.frameCount % this.fpsDrops.Length] = ((Time.deltaTime != 0f) ? (1f / Time.deltaTime) : float.PositiveInfinity);
			int num3 = Time.frameCount % this.graph.Length;
			this.graph[num3].fps = ((Time.deltaTime < 1.401298E-45f) ? 0f : (1f / Time.deltaTime));
			this.graph[num3].collectEvent = flag;
			this.graph[num3].memory = (float)this.allocMem;
		}
		if (Application.isPlaying && this.cam != null && this.showGraph)
		{
			this.graphWidth = this.cam.pixelWidth * 0.8f;
			float num4 = float.PositiveInfinity;
			float num5 = 0f;
			float num6 = float.PositiveInfinity;
			float num7 = 0f;
			for (int i = 0; i < this.graph.Length; i++)
			{
				num4 = Mathf.Min(this.graph[i].memory, num4);
				num5 = Mathf.Max(this.graph[i].memory, num5);
				num6 = Mathf.Min(this.graph[i].fps, num6);
				num7 = Mathf.Max(this.graph[i].fps, num7);
			}
			int num8 = Time.frameCount % this.graph.Length;
			Matrix4x4 m = Matrix4x4.TRS(new Vector3((this.cam.pixelWidth - this.graphWidth) / 2f, this.graphOffset, 1f), Quaternion.identity, new Vector3(this.graphWidth, this.graphHeight, 1f));
			for (int j = 0; j < this.graph.Length - 1; j++)
			{
				if (j != num8)
				{
					this.DrawGraphLine(j, m, (float)j / (float)this.graph.Length, (float)(j + 1) / (float)this.graph.Length, AstarMath.MapTo(num4, num5, this.graph[j].memory), AstarMath.MapTo(num4, num5, this.graph[j + 1].memory), Color.blue);
					this.DrawGraphLine(j, m, (float)j / (float)this.graph.Length, (float)(j + 1) / (float)this.graph.Length, AstarMath.MapTo(num6, num7, this.graph[j].fps), AstarMath.MapTo(num6, num7, this.graph[j + 1].fps), Color.green);
				}
			}
		}
	}

	public void DrawGraphLine(int index, Matrix4x4 m, float x1, float x2, float y1, float y2, Color col)
	{
		Debug.DrawLine(this.cam.ScreenToWorldPoint(m.MultiplyPoint3x4(new Vector3(x1, y1))), this.cam.ScreenToWorldPoint(m.MultiplyPoint3x4(new Vector3(x2, y2))), col);
	}

	public void Cross(Vector3 p)
	{
		p = this.cam.get_cameraToWorldMatrix().MultiplyPoint(p);
		Debug.DrawLine(p - Vector3.up * 0.2f, p + Vector3.up * 0.2f, Color.red);
		Debug.DrawLine(p - Vector3.right * 0.2f, p + Vector3.right * 0.2f, Color.red);
	}

	public void OnGUI()
	{
		if (!this.show || (!Application.isPlaying && !this.showInEditor))
		{
			return;
		}
		if (this.style == null)
		{
			this.style = new GUIStyle();
			this.style.normal.textColor = Color.white;
			this.style.set_padding(new RectOffset(5, 5, 5, 5));
		}
		if (Time.realtimeSinceStartup - this.lastUpdate > 0.5f || this.cachedText == null || !Application.isPlaying)
		{
			this.lastUpdate = Time.realtimeSinceStartup;
			this.boxRect = new Rect(5f, (float)this.yOffset, 310f, 40f);
			this.text.set_Length(0);
			this.text.AppendLine("A* Pathfinding Project Debugger");
			this.text.Append("A* Version: ").Append(AstarPath.Version.ToString());
			if (this.showMemProfile)
			{
				this.boxRect.height = this.boxRect.height + 200f;
				this.text.AppendLine();
				this.text.AppendLine();
				this.text.Append("Currently allocated".PadRight(25));
				this.text.Append(((float)this.allocMem / 1000000f).ToString("0.0 MB"));
				this.text.AppendLine();
				this.text.Append("Peak allocated".PadRight(25));
				this.text.Append(((float)this.peakAlloc / 1000000f).ToString("0.0 MB")).AppendLine();
				this.text.Append("Last collect peak".PadRight(25));
				this.text.Append(((float)this.collectAlloc / 1000000f).ToString("0.0 MB")).AppendLine();
				this.text.Append("Allocation rate".PadRight(25));
				this.text.Append(((float)this.allocRate / 1000000f).ToString("0.0 MB")).AppendLine();
				this.text.Append("Collection frequency".PadRight(25));
				this.text.Append(this.delta.ToString("0.00"));
				this.text.Append("s\n");
				this.text.Append("Last collect fps".PadRight(25));
				this.text.Append((1f / this.lastDeltaTime).ToString("0.0 fps"));
				this.text.Append(" (");
				this.text.Append(this.lastDeltaTime.ToString("0.000 s"));
				this.text.Append(")");
			}
			if (this.showFPS)
			{
				this.text.AppendLine();
				this.text.AppendLine();
				this.text.Append("FPS".PadRight(25)).Append((1f / this.delayedDeltaTime).ToString("0.0 fps"));
				float num = float.PositiveInfinity;
				for (int i = 0; i < this.fpsDrops.Length; i++)
				{
					if (this.fpsDrops[i] < num)
					{
						num = this.fpsDrops[i];
					}
				}
				this.text.AppendLine();
				this.text.Append(("Lowest fps (last " + this.fpsDrops.Length + ")").PadRight(25)).Append(num.ToString("0.0"));
			}
			if (this.showPathProfile)
			{
				AstarPath active = AstarPath.active;
				this.text.AppendLine();
				if (active == null)
				{
					this.text.Append("\nNo AstarPath Object In The Scene");
				}
				else
				{
					if (ListPool<Vector3>.GetSize() > this.maxVecPool)
					{
						this.maxVecPool = ListPool<Vector3>.GetSize();
					}
					if (ListPool<GraphNode>.GetSize() > this.maxNodePool)
					{
						this.maxNodePool = ListPool<GraphNode>.GetSize();
					}
					this.text.Append("\nPool Sizes (size/total created)");
					for (int j = 0; j < this.debugTypes.Length; j++)
					{
						this.debugTypes[j].Print(this.text);
					}
				}
			}
			this.cachedText = this.text.ToString();
		}
		if (this.font != null)
		{
			this.style.set_font(this.font);
			this.style.fontSize = this.fontSize;
		}
		this.boxRect.height = this.style.CalcHeight(new GUIContent(this.cachedText), this.boxRect.width);
		GUI.Box(this.boxRect, string.Empty);
		GUI.Label(this.boxRect, this.cachedText, this.style);
		if (this.showGraph)
		{
			float num2 = float.PositiveInfinity;
			float num3 = 0f;
			float num4 = float.PositiveInfinity;
			float num5 = 0f;
			for (int k = 0; k < this.graph.Length; k++)
			{
				num2 = Mathf.Min(this.graph[k].memory, num2);
				num3 = Mathf.Max(this.graph[k].memory, num3);
				num4 = Mathf.Min(this.graph[k].fps, num4);
				num5 = Mathf.Max(this.graph[k].fps, num5);
			}
			GUI.color = Color.blue;
			float num6 = (float)Mathf.RoundToInt(num3 / 100000f);
			GUI.Label(new Rect(5f, (float)Screen.height - AstarMath.MapTo(num2, num3, 0f + this.graphOffset, this.graphHeight + this.graphOffset, num6 * 1000f * 100f) - 10f, 100f, 20f), (num6 / 10f).ToString("0.0 MB"));
			num6 = Mathf.Round(num2 / 100000f);
			GUI.Label(new Rect(5f, (float)Screen.height - AstarMath.MapTo(num2, num3, 0f + this.graphOffset, this.graphHeight + this.graphOffset, num6 * 1000f * 100f) - 10f, 100f, 20f), (num6 / 10f).ToString("0.0 MB"));
			GUI.color = Color.green;
			num6 = Mathf.Round(num5);
			GUI.Label(new Rect(55f, (float)Screen.height - AstarMath.MapTo(num4, num5, 0f + this.graphOffset, this.graphHeight + this.graphOffset, num6) - 10f, 100f, 20f), num6.ToString("0 FPS"));
			num6 = Mathf.Round(num4);
			GUI.Label(new Rect(55f, (float)Screen.height - AstarMath.MapTo(num4, num5, 0f + this.graphOffset, this.graphHeight + this.graphOffset, num6) - 10f, 100f, 20f), num6.ToString("0 FPS"));
		}
	}
}
