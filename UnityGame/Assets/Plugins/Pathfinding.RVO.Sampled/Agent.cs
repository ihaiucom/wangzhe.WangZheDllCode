using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Pathfinding.RVO.Sampled
{
	public class Agent : IAgent
	{
		public struct VO
		{
			public VInt2 origin;

			public VInt2 center;

			private VInt2 line1;

			private VInt2 line2;

			private VInt2 dir1;

			private VInt2 dir2;

			private VInt2 cutoffLine;

			private VInt2 cutoffDir;

			private long sqrCutoffDistance;

			private bool leftSide;

			private bool colliding;

			private int radius;

			private VFactor weightFactor;

			public VO(VInt2 offset, VInt2 p0, VInt2 dir, VFactor weightFactor)
			{
				this.colliding = true;
				this.line1 = p0;
				this.dir1 = -dir;
				this.origin = VInt2.zero;
				this.center = VInt2.zero;
				this.line2 = VInt2.zero;
				this.dir2 = VInt2.zero;
				this.cutoffLine = VInt2.zero;
				this.cutoffDir = VInt2.zero;
				this.sqrCutoffDistance = 0L;
				this.leftSide = false;
				this.radius = 0;
				this.weightFactor.nom = weightFactor.nom;
				this.weightFactor.den = weightFactor.den << 1;
			}

			public VO(VInt2 offset, VInt2 p1, VInt2 p2, VInt2 tang1, VInt2 tang2, VFactor weightFactor)
			{
				this.weightFactor.nom = weightFactor.nom;
				this.weightFactor.den = weightFactor.den << 1;
				this.colliding = false;
				this.cutoffLine = p1;
				this.cutoffDir = (p2 - p1).normalized;
				this.line1 = p1;
				this.dir1 = tang1;
				this.line2 = p2;
				this.dir2 = tang2;
				this.dir2 = -this.dir2;
				this.cutoffDir = -this.cutoffDir;
				this.origin = VInt2.zero;
				this.center = VInt2.zero;
				this.sqrCutoffDistance = 0L;
				this.leftSide = false;
				this.radius = 0;
				weightFactor.nom = 5L;
				weightFactor.den = 1L;
			}

			public VO(VInt2 center, VInt2 offset, int radius, VInt2 sideChooser, VFactor inverseDt, VFactor weightFactor)
			{
				this.weightFactor.nom = weightFactor.nom;
				this.weightFactor.den = weightFactor.den << 1;
				this.origin = offset;
				weightFactor.nom = 1L;
				weightFactor.den = 2L;
				long sqrMagnitudeLong = center.sqrMagnitudeLong;
				int num = IntMath.Sqrt(sqrMagnitudeLong);
				VInt2 a = center;
				if (num > 0)
				{
					a.x = IntMath.Divide(a.x * 1000, num);
					a.y = IntMath.Divide(a.y * 1000, num);
				}
				long num2 = (long)radius;
				num2 *= num2;
				if (sqrMagnitudeLong < num2)
				{
					this.colliding = true;
					this.leftSide = false;
					this.line1 = IntMath.Divide(a, (long)(num - radius), 1000L);
					VInt2 vInt = new VInt2(this.line1.y, -this.line1.x);
					this.dir1 = vInt.normalized;
					this.line1 += offset;
					this.cutoffDir = VInt2.zero;
					this.cutoffLine = VInt2.zero;
					this.sqrCutoffDistance = 0L;
					this.dir2 = VInt2.zero;
					this.line2 = VInt2.zero;
					this.center = VInt2.zero;
					this.radius = 0;
				}
				else
				{
					this.colliding = false;
					center *= inverseDt;
					radius *= inverseDt;
					num = center.magnitude;
					VInt2 b = center + offset;
					this.sqrCutoffDistance = (long)(num - radius);
					this.center = center;
					this.cutoffLine = IntMath.Divide(a, this.sqrCutoffDistance, 1000L);
					VInt2 vInt2 = new VInt2(-this.cutoffLine.y, this.cutoffLine.x);
					this.cutoffDir = vInt2.normalized;
					this.cutoffLine += offset;
					this.sqrCutoffDistance *= this.sqrCutoffDistance;
					VFactor a2 = IntMath.atan2(-center.y, -center.x);
					VFactor b2 = IntMath.acos((long)radius, (long)num);
					this.radius = radius;
					this.leftSide = Polygon.Left(VInt2.zero, center, sideChooser);
					VFactor f;
					VFactor f2;
					IntMath.sincos(out f, out f2, a2 + b2);
					this.line1 = new VInt2(radius * f2, radius * f);
					VInt2 vInt3 = new VInt2(this.line1.y, -this.line1.x);
					this.dir1 = vInt3.normalized;
					IntMath.sincos(out f, out f2, a2 - b2);
					this.line2 = new VInt2(radius * f2, radius * f);
					VInt2 vInt4 = new VInt2(this.line2.y, -this.line2.x);
					this.dir2 = vInt4.normalized;
					this.line1 += b;
					this.line2 += b;
				}
			}

			public static bool Left(VInt2 a, VInt2 dir, VInt2 p)
			{
				return dir.x * (p.y - a.y) - (p.x - a.x) * dir.y <= 0;
			}

			public static long Det(VInt2 a, VInt2 dir, VInt2 p)
			{
				return (long)(p.x - a.x) * (long)dir.y - (long)dir.x * (long)(p.y - a.y);
			}

			public VInt2 Sample(VInt2 p, out VFactor weight)
			{
				if (this.colliding)
				{
					long num = Agent.VO.Det(this.line1, this.dir1, p);
					if (num >= 0L)
					{
						weight.nom = this.weightFactor.nom * num;
						weight.den = this.weightFactor.den * 1000L;
						return IntMath.Divide(new VInt2(-this.dir1.y, this.dir1.x), weight.nom * (long)Agent.GlobalIncompressibility, weight.den);
					}
					weight = VFactor.zero;
					return VInt2.zero;
				}
				else
				{
					long num2 = Agent.VO.Det(this.cutoffLine, this.cutoffDir, p);
					if (num2 <= 0L)
					{
						weight = VFactor.zero;
						return VInt2.zero;
					}
					long num3 = Agent.VO.Det(this.line1, this.dir1, p);
					long num4 = Agent.VO.Det(this.line2, this.dir2, p);
					if (num3 < 0L || num4 < 0L)
					{
						weight = VFactor.zero;
						return VInt2.zero;
					}
					if (this.leftSide)
					{
						if (num2 < (long)(this.radius * 1000))
						{
							weight.nom = this.weightFactor.nom * num2;
							weight.den = this.weightFactor.den * 1000L;
							return new VInt2(-this.cutoffDir.y, this.cutoffDir.x) * weight;
						}
						weight.nom = this.weightFactor.nom * num3;
						weight.den = this.weightFactor.den * 1000L;
						return new VInt2(-this.dir1.y, this.dir1.x) * weight;
					}
					else
					{
						if (num2 < (long)(this.radius * 1000))
						{
							weight.nom = this.weightFactor.nom * num2;
							weight.den = this.weightFactor.den * 1000L;
							return new VInt2(-this.cutoffDir.y, this.cutoffDir.x) * weight;
						}
						weight.nom = this.weightFactor.nom * num4;
						weight.den = this.weightFactor.den * 1000L;
						return new VInt2(-this.dir2.y, this.dir2.x) * weight;
					}
				}
			}

			public VFactor ScalarSample(VInt2 p)
			{
				if (this.colliding)
				{
					long num = Agent.VO.Det(this.line1, this.dir1, p);
					if (num >= 0L)
					{
						return new VFactor(num * (long)Agent.GlobalIncompressibility * this.weightFactor.nom, this.weightFactor.den * 1000L);
					}
					return VFactor.zero;
				}
				else
				{
					long num2 = Agent.VO.Det(this.cutoffLine, this.cutoffDir, p);
					if (num2 <= 0L)
					{
						return VFactor.zero;
					}
					long num3 = Agent.VO.Det(this.line1, this.dir1, p);
					long num4 = Agent.VO.Det(this.line2, this.dir2, p);
					if (num3 < 0L || num4 < 0L)
					{
						return VFactor.zero;
					}
					if (this.leftSide)
					{
						if (num2 < (long)(this.radius * 1000))
						{
							return new VFactor(num2 * this.weightFactor.nom, this.weightFactor.den * 1000L);
						}
						return new VFactor(num3 * this.weightFactor.nom, this.weightFactor.den * 1000L);
					}
					else
					{
						if (num2 < (long)this.radius)
						{
							return new VFactor(num2 * this.weightFactor.nom, this.weightFactor.den * 1000L);
						}
						return new VFactor(num4 * this.weightFactor.nom, this.weightFactor.den * 1000L);
					}
				}
			}
		}

		private VInt3 smoothPos;

		public VInt agentTimeHorizon;

		public VInt obstacleTimeHorizon;

		public VInt radius;

		public VInt height;

		public VInt maxSpeed;

		public VInt neighbourDist;

		public VInt weight;

		public bool locked;

		private RVOLayer layer;

		private RVOLayer collidesWith;

		public int maxNeighbours;

		public VInt3 position;

		public VInt3 desiredVelocity;

		public VInt3 prevSmoothPos;

		public bool hasCollided;

		public Agent next;

		public VInt3 velocity;

		public VInt3 newVelocity;

		public Simulator simulator;

		public List<Agent> neighbours = new List<Agent>(8);

		public List<long> neighbourDists = new List<long>(8);

		private List<ObstacleVertex> obstaclesBuffered = new List<ObstacleVertex>();

		private List<ObstacleVertex> obstacles = new List<ObstacleVertex>();

		private List<float> obstacleDists = new List<float>();

		[NonSerialized]
		public object owner;

		public static Stopwatch watch1;

		public static Stopwatch watch2;

		private static int[] adpSamp_Steps1_Size_Nom;

		private static long[] adpSamp_Steps1_Pos_Nom1;

		private static long[] adpSamp_Steps1_Pos_Nom2;

		private static long[] adpSamp_Steps1_Pos_Den;

		private static long[] adpSamp_Steps2_Nom_rw;

		private static long[] adpSamp_Steps2_Nom_fw;

		private static long[] adpSamp_Steps2_Den;

		private static VFactor adpSamp_Steps2_InnerScale;

		private static VFactor[] adpSamp_Steps3_Sin;

		private static VFactor[] adpSamp_Steps3_Cos;

		public static float DesiredVelocityWeight;

		public static float DesiredVelocityScale;

		public static int GlobalIncompressibility;

		public static VInt WallWeight;

		private static List<VLine> orcaLines;

		private static List<VLine> projLines;

		public VInt3 Position
		{
			get;
			private set;
		}

		public VInt3 InterpolatedPosition
		{
			get
			{
				return this.smoothPos;
			}
		}

		public VInt3 DesiredVelocity
		{
			get;
			set;
		}

		public RVOLayer Layer
		{
			get;
			set;
		}

		public RVOLayer CollidesWith
		{
			get;
			set;
		}

		public bool Locked
		{
			get;
			set;
		}

		public VInt Radius
		{
			get;
			set;
		}

		public VInt Height
		{
			get;
			set;
		}

		public VInt MaxSpeed
		{
			get;
			set;
		}

		public VInt NeighbourDist
		{
			get;
			set;
		}

		public int AgentTimeHorizon
		{
			get;
			set;
		}

		public int ObstacleTimeHorizon
		{
			get;
			set;
		}

		public VInt3 Velocity
		{
			get;
			set;
		}

		public bool DebugDraw
		{
			get;
			set;
		}

		public int MaxNeighbours
		{
			get;
			set;
		}

		public List<ObstacleVertex> NeighbourObstacles
		{
			get
			{
				return null;
			}
		}

		public Agent(VInt3 pos)
		{
			this.MaxSpeed = 2000;
			this.NeighbourDist = 15;
			this.AgentTimeHorizon = 2000;
			this.ObstacleTimeHorizon = 2000;
			this.Height = 5000;
			this.Radius = 5000;
			this.MaxNeighbours = 10;
			this.Locked = false;
			this.position = pos;
			this.Position = this.position;
			this.prevSmoothPos = this.position;
			this.smoothPos = this.position;
			this.Layer = RVOLayer.DefaultAgent;
			this.CollidesWith = (RVOLayer)(-1);
		}

		static Agent()
		{
			Agent.orcaLines = new List<VLine>();
			Agent.projLines = new List<VLine>();
			Agent.watch1 = new Stopwatch();
			Agent.watch2 = new Stopwatch();
			Agent.adpSamp_Steps1_Size_Nom = null;
			Agent.adpSamp_Steps1_Pos_Nom1 = null;
			Agent.adpSamp_Steps1_Pos_Nom2 = null;
			Agent.adpSamp_Steps1_Pos_Den = null;
			Agent.adpSamp_Steps2_Nom_rw = null;
			Agent.adpSamp_Steps2_Nom_fw = null;
			Agent.adpSamp_Steps2_Den = null;
			Agent.adpSamp_Steps2_InnerScale = new VFactor(3L, 5L);
			Agent.adpSamp_Steps3_Sin = null;
			Agent.adpSamp_Steps3_Cos = null;
			Agent.DesiredVelocityWeight = 0.02f;
			Agent.DesiredVelocityScale = 0.1f;
			Agent.GlobalIncompressibility = 30;
			Agent.WallWeight = 5000;
			Agent.adpSamp_Steps1_Pos_Nom1 = new long[8];
			Agent.adpSamp_Steps1_Pos_Nom2 = new long[8];
			Agent.adpSamp_Steps1_Pos_Den = new long[8];
			Agent.adpSamp_Steps1_Size_Nom = new int[8];
			for (int i = 0; i < 8; i++)
			{
				VFactor vFactor;
				VFactor vFactor2;
				IntMath.sincos(out vFactor, out vFactor2, (long)(i * 62832), 80000L);
				vFactor2.nom += vFactor2.den;
				Agent.adpSamp_Steps1_Pos_Nom1[i] = vFactor.nom * vFactor2.den;
				Agent.adpSamp_Steps1_Pos_Nom2[i] = vFactor.den * vFactor2.nom;
				Agent.adpSamp_Steps1_Pos_Den[i] = vFactor.den * vFactor2.den;
				Agent.adpSamp_Steps1_Size_Nom[i] = 8 - Mathf.Abs(i - 4);
			}
			VFactor inverse = Agent.adpSamp_Steps2_InnerScale.Inverse;
			Agent.adpSamp_Steps2_Nom_rw = new long[6];
			Agent.adpSamp_Steps2_Nom_fw = new long[6];
			Agent.adpSamp_Steps2_Den = new long[6];
			for (int j = 0; j < 6; j++)
			{
				VFactor a;
				VFactor vFactor3;
				IntMath.sincos(out a, out vFactor3, (long)((2 * j + 1) * 31416), 60000L);
				a += inverse;
				Agent.adpSamp_Steps2_Den[j] = vFactor3.den * a.den * Agent.adpSamp_Steps2_InnerScale.den;
				Agent.adpSamp_Steps2_Nom_rw[j] = vFactor3.nom * a.den;
				Agent.adpSamp_Steps2_Nom_fw[j] = a.nom * vFactor3.den;
			}
			Agent.adpSamp_Steps3_Sin = new VFactor[6];
			Agent.adpSamp_Steps3_Cos = new VFactor[6];
			for (int k = 0; k < 6; k++)
			{
				VFactor vFactor4;
				VFactor vFactor5;
				IntMath.sincos(out vFactor4, out vFactor5, (long)((2 * k + 1) * 31416), 60000L);
				vFactor4.den *= 5L;
				vFactor5.den *= 5L;
				Agent.adpSamp_Steps3_Sin[k] = vFactor4;
				Agent.adpSamp_Steps3_Cos[k] = vFactor5;
			}
		}

		public void Teleport(VInt3 pos)
		{
			this.Position = pos;
			this.smoothPos = pos;
			this.prevSmoothPos = pos;
		}

		public void SetYPosition(VInt yCoordinate)
		{
			this.Position = new VInt3(this.Position.x, yCoordinate.i, this.Position.z);
			this.smoothPos.y = yCoordinate.i;
			this.prevSmoothPos.y = yCoordinate.i;
		}

		public void BufferSwitch()
		{
			this.radius = this.Radius;
			this.height = this.Height;
			this.maxSpeed = this.MaxSpeed;
			this.neighbourDist = this.NeighbourDist;
			this.agentTimeHorizon = this.AgentTimeHorizon;
			this.obstacleTimeHorizon = this.ObstacleTimeHorizon;
			this.maxNeighbours = this.MaxNeighbours;
			this.desiredVelocity = this.DesiredVelocity;
			this.locked = this.Locked;
			this.collidesWith = this.CollidesWith;
			this.layer = this.Layer;
			this.Velocity = this.velocity;
			List<ObstacleVertex> list = this.obstaclesBuffered;
			this.obstaclesBuffered = this.obstacles;
			this.obstacles = list;
		}

		public void Update()
		{
			this.velocity = this.newVelocity;
			this.prevSmoothPos = this.smoothPos;
			this.position = this.prevSmoothPos;
			this.position += IntMath.Divide(this.velocity, (long)this.simulator.DeltaTimeMS, 1000L);
			this.Position = this.position;
		}

		public void Interpolate(float t)
		{
			if (t == 1f)
			{
				this.smoothPos = this.Position;
			}
			else
			{
				this.smoothPos = this.prevSmoothPos + (this.Position - this.prevSmoothPos) * t;
			}
		}

		public void Interpolate()
		{
			this.smoothPos = this.Position;
		}

		public void CalculateNeighbours()
		{
			this.neighbours.Clear();
			this.neighbourDists.Clear();
			if (this.locked)
			{
				return;
			}
			if (this.MaxNeighbours > 0)
			{
				this.simulator.Quadtree.Query(this.position.xz, (long)this.neighbourDist.i, this);
			}
			this.obstacles.Clear();
			this.obstacleDists.Clear();
		}

		public long InsertAgentNeighbour(Agent agent, long rangeSq)
		{
			if (this == agent)
			{
				return rangeSq;
			}
			if ((agent.layer & this.collidesWith) == (RVOLayer)0)
			{
				return rangeSq;
			}
			long num = agent.position.XZSqrMagnitude(ref this.position);
			if (num < rangeSq)
			{
				if (this.neighbours.get_Count() < this.maxNeighbours)
				{
					this.neighbours.Add(agent);
					this.neighbourDists.Add(num);
				}
				int num2 = this.neighbours.get_Count() - 1;
				if (num < this.neighbourDists.get_Item(num2))
				{
					while (num2 != 0 && num < this.neighbourDists.get_Item(num2 - 1))
					{
						this.neighbours.set_Item(num2, this.neighbours.get_Item(num2 - 1));
						this.neighbourDists.set_Item(num2, this.neighbourDists.get_Item(num2 - 1));
						num2--;
					}
					this.neighbours.set_Item(num2, agent);
					this.neighbourDists.set_Item(num2, num);
				}
				if (this.neighbours.get_Count() == this.maxNeighbours)
				{
					rangeSq = this.neighbourDists.get_Item(this.neighbourDists.get_Count() - 1);
				}
			}
			return rangeSq;
		}

		public void InsertObstacleNeighbour(ObstacleVertex ob1, long rangeSq)
		{
			ObstacleVertex obstacleVertex = ob1.next;
			long num = AstarMath.DistancePointSegmentStrict(ob1.position, obstacleVertex.position, this.Position);
			if (num < rangeSq)
			{
				this.obstacles.Add(ob1);
				this.obstacleDists.Add((float)num);
				int num2 = this.obstacles.get_Count() - 1;
				while (num2 != 0 && (float)num < this.obstacleDists.get_Item(num2 - 1))
				{
					this.obstacles.set_Item(num2, this.obstacles.get_Item(num2 - 1));
					this.obstacleDists.set_Item(num2, this.obstacleDists.get_Item(num2 - 1));
					num2--;
				}
				this.obstacles.set_Item(num2, ob1);
				this.obstacleDists.set_Item(num2, (float)num);
			}
		}

		private static Vector3 To3D(Vector2 p)
		{
			return new Vector3(p.x, 0f, p.y);
		}

		private static void DrawCircle(Vector2 _p, float radius, Color col)
		{
			Agent.DrawCircle(_p, radius, 0f, 6.28318548f, col);
		}

		private static void DrawCircle(Vector2 _p, float radius, float a0, float a1, Color col)
		{
			Vector3 a2 = Agent.To3D(_p);
			while (a0 > a1)
			{
				a0 -= 6.28318548f;
			}
			Vector3 b = new Vector3(Mathf.Cos(a0) * radius, 0f, Mathf.Sin(a0) * radius);
			int num = 0;
			while ((float)num <= 40f)
			{
				Vector3 vector = new Vector3(Mathf.Cos(Mathf.Lerp(a0, a1, (float)num / 40f)) * radius, 0f, Mathf.Sin(Mathf.Lerp(a0, a1, (float)num / 40f)) * radius);
				Debug.DrawLine(a2 + b, a2 + vector, col);
				b = vector;
				num++;
			}
		}

		private static void DrawVO(Vector2 circleCenter, float radius, Vector2 origin)
		{
			float num = Mathf.Atan2((origin - circleCenter).y, (origin - circleCenter).x);
			float num2 = radius / (origin - circleCenter).magnitude;
			float num3 = (num2 <= 1f) ? Mathf.Abs(Mathf.Acos(num2)) : 0f;
			Agent.DrawCircle(circleCenter, radius, num - num3, num + num3, Color.black);
			Vector2 vector = new Vector2(Mathf.Cos(num - num3), Mathf.Sin(num - num3)) * radius;
			Vector2 vector2 = new Vector2(Mathf.Cos(num + num3), Mathf.Sin(num + num3)) * radius;
			Vector2 p = -new Vector2(-vector.y, vector.x);
			Vector2 p2 = new Vector2(-vector2.y, vector2.x);
			vector += circleCenter;
			vector2 += circleCenter;
			Debug.DrawRay(Agent.To3D(vector), Agent.To3D(p).normalized * 100f, Color.black);
			Debug.DrawRay(Agent.To3D(vector2), Agent.To3D(p2).normalized * 100f, Color.black);
		}

		private static void DrawCross(Vector2 p, float size = 1f)
		{
			Agent.DrawCross(p, Color.white, size);
		}

		private static void DrawCross(Vector2 p, Color col, float size = 1f)
		{
			size *= 0.5f;
			Debug.DrawLine(new Vector3(p.x, 0f, p.y) - Vector3.right * size, new Vector3(p.x, 0f, p.y) + Vector3.right * size, col);
			Debug.DrawLine(new Vector3(p.x, 0f, p.y) - Vector3.forward * size, new Vector3(p.x, 0f, p.y) + Vector3.forward * size, col);
		}

		internal void CalculateVelocity(Simulator.WorkerContext context)
		{
			if (this.locked)
			{
				this.newVelocity = VInt3.zero;
				return;
			}
			if (context.vos.Length < this.neighbours.get_Count() + this.simulator.obstacles.get_Count())
			{
				context.vos = new Agent.VO[Mathf.Max(context.vos.Length * 2, this.neighbours.get_Count() + this.simulator.obstacles.get_Count())];
			}
			VInt2 b = new VInt2(this.position.x, this.position.z);
			Agent.VO[] vos = context.vos;
			int num = 0;
			VInt2 vInt = new VInt2(this.velocity.x, this.velocity.z);
			VFactor vFactor = new VFactor(1000L, (long)this.agentTimeHorizon.i);
			VFactor one = VFactor.one;
			if (this.simulator.algorithm != Simulator.SamplingAlgorithm.GradientDecent)
			{
				one.nom = (long)Agent.WallWeight.i;
				one.den = 1000L;
			}
			VFactor vFactor2 = one;
			vFactor2.nom <<= 1;
			VFactor vFactor3 = one;
			vFactor3.den *= 20L;
			this.CalcVelocity_Neighbours(ref b, vos, ref num, ref vInt, ref vFactor);
			VInt2 vInt2 = VInt2.zero;
			if (num > 0)
			{
				if (this.simulator.algorithm == Simulator.SamplingAlgorithm.GradientDecent)
				{
					throw new NotImplementedException("GradientDecent");
				}
				VInt2[] samplePos = context.samplePos;
				long[] sampleSize = context.sampleSize;
				int num2 = 0;
				VInt2 xz = this.desiredVelocity.xz;
				int num3 = Mathf.Max(this.radius.i, Mathf.Max(xz.magnitude, this.Velocity.magnitude));
				samplePos[num2] = xz;
				sampleSize[num2] = (long)(num3 * 3 / 10);
				num2++;
				samplePos[num2] = vInt;
				sampleSize[num2] = (long)(num3 * 3 / 10);
				num2++;
				VInt2 fw = new VInt2(vInt.x >> 1, vInt.y >> 1);
				VInt2 rw = new VInt2(fw.y, -fw.x);
				Agent.CalcSamplePos_Step1(samplePos, sampleSize, ref num2, num3, rw, fw);
				Agent.CalcSamplePos_Step2(samplePos, sampleSize, ref num2, num3, ref fw, ref rw);
				Agent.CalcSamplePos_Step3(ref vInt, samplePos, sampleSize, ref num2, num3);
				samplePos[num2] = IntMath.Divide(vInt, 2L);
				sampleSize[num2] = (long)IntMath.Divide(num3 * 2, 5);
				num2++;
				this.CalcSamplePosScore(context, ref b, vos, num, ref vInt, ref vInt2, samplePos, sampleSize, ref num2, ref xz);
			}
			else
			{
				vInt2 = this.desiredVelocity.xz;
			}
			if (this.DebugDraw)
			{
				Agent.DrawCross((Vector2)(vInt2 + b), 1f);
			}
			vInt2 = VInt2.ClampMagnitude(vInt2, this.maxSpeed.i);
			this.newVelocity = new VInt3(vInt2.x, 0, vInt2.y);
		}

		private void CalcVelocity_Neighbours(ref VInt2 position2D, Agent.VO[] vos, ref int voCount, ref VInt2 optimalVelocity, ref VFactor inverseAgentTimeHorizon)
		{
			for (int i = 0; i < this.neighbours.get_Count(); i++)
			{
				Agent agent = this.neighbours.get_Item(i);
				if (agent != this)
				{
					int num = Math.Min(this.position.y + this.height.i, agent.position.y + agent.height.i);
					int num2 = Math.Max(this.position.y, agent.position.y);
					if (num - num2 >= 0)
					{
						VInt2 xz = agent.Velocity.xz;
						VInt vInt = this.radius + agent.radius;
						VInt2 vInt2 = agent.position.xz - position2D;
						VInt2 sideChooser = optimalVelocity - xz;
						VInt2 vInt3;
						if (agent.locked)
						{
							vInt3 = xz;
						}
						else
						{
							vInt3 = optimalVelocity + xz;
							vInt3.x >>= 1;
							vInt3.y >>= 1;
						}
						vos[voCount] = new Agent.VO(vInt2, vInt3, vInt.i, sideChooser, inverseAgentTimeHorizon, VFactor.one);
						voCount++;
						if (this.DebugDraw)
						{
							VInt2 b = IntMath.Divide(vInt2, inverseAgentTimeHorizon.nom, inverseAgentTimeHorizon.den);
							VInt ob = (int)IntMath.Divide((long)vInt.i * inverseAgentTimeHorizon.nom, inverseAgentTimeHorizon.den);
							Agent.DrawVO((Vector2)(position2D + b + vInt3), (float)ob, (Vector2)(position2D + vInt3));
						}
					}
				}
			}
		}

		private void CalcVelocity_Obstacles(ref VInt2 position2D, Agent.VO[] vos, ref int voCount, ref VFactor wallThickness, ref VFactor wallWeight, ref VFactor wallWeight2, ref VFactor wallWeight3)
		{
			for (int i = 0; i < this.simulator.obstacles.get_Count(); i++)
			{
				ObstacleVertex obstacleVertex = this.simulator.obstacles.get_Item(i);
				ObstacleVertex obstacleVertex2 = obstacleVertex;
				do
				{
					if (obstacleVertex2.ignore || this.position.y > obstacleVertex2.position.y + obstacleVertex2.height.i || this.position.y + this.height.i < obstacleVertex2.position.y || (obstacleVertex2.layer & this.collidesWith) == (RVOLayer)0)
					{
						obstacleVertex2 = obstacleVertex2.next;
					}
					else
					{
						long num = Agent.VO.Det(obstacleVertex2.position.xz, obstacleVertex2.dir, position2D);
						long num2 = num;
						long b = VInt2.DotLong(obstacleVertex2.dir, position2D - obstacleVertex2.position.xz);
						bool flag = wallWeight3 >= b || wallWeight3 + b >= (long)(obstacleVertex2.position.xz - obstacleVertex2.next.position.xz).magnitude;
						if (Math.Abs(num2) < (long)(this.neighbourDist.i * 1000))
						{
							if (num2 <= 0L && !flag && -wallThickness * 1000L < num2)
							{
								vos[voCount] = new Agent.VO(position2D, obstacleVertex2.position.xz - position2D, obstacleVertex2.dir, wallWeight2);
								voCount++;
							}
							else if (num2 > 0L)
							{
								VInt2 p = obstacleVertex2.position.xz - position2D;
								VInt2 p2 = obstacleVertex2.next.position.xz - position2D;
								VInt2 normalized = p.normalized;
								VInt2 normalized2 = p2.normalized;
								vos[voCount] = new Agent.VO(position2D, p, p2, normalized, normalized2, wallWeight);
								voCount++;
							}
						}
						obstacleVertex2 = obstacleVertex2.next;
					}
				}
				while (obstacleVertex2 != obstacleVertex);
			}
		}

		private static void CalcSamplePos_Step3(ref VInt2 optimalVelocity, VInt2[] samplePos, long[] sampleSize, ref int samplePosCount, int sampleScale)
		{
			int num = IntMath.Divide(sampleScale * 2, 5);
			for (int i = 0; i < 6; i++)
			{
				samplePos[samplePosCount].x = optimalVelocity.x + sampleScale * Agent.adpSamp_Steps3_Cos[i];
				samplePos[samplePosCount].y = optimalVelocity.y + sampleScale * Agent.adpSamp_Steps3_Sin[i];
				sampleSize[samplePosCount] = (long)num;
				samplePosCount++;
			}
		}

		private static void CalcSamplePos_Step1(VInt2[] samplePos, long[] sampleSize, ref int samplePosCount, int sampleScale, VInt2 rw, VInt2 fw)
		{
			VFactor vFactor = default(VFactor);
			vFactor.den = 16L;
			for (int i = 0; i < 8; i++)
			{
				vFactor.nom = (long)(Agent.adpSamp_Steps1_Size_Nom[i] * sampleScale);
				samplePos[samplePosCount].x = (int)IntMath.Divide((long)rw.x * Agent.adpSamp_Steps1_Pos_Nom1[i] + (long)fw.x * Agent.adpSamp_Steps1_Pos_Nom2[i], Agent.adpSamp_Steps1_Pos_Den[i]);
				samplePos[samplePosCount].y = (int)IntMath.Divide((long)rw.y * Agent.adpSamp_Steps1_Pos_Nom1[i] + (long)fw.y * Agent.adpSamp_Steps1_Pos_Nom2[i], Agent.adpSamp_Steps1_Pos_Den[i]);
				sampleSize[samplePosCount] = (long)vFactor.roundInt;
				samplePosCount++;
			}
		}

		private static void CalcSamplePos_Step2(VInt2[] samplePos, long[] sampleSize, ref int samplePosCount, int sampleScale, ref VInt2 fw, ref VInt2 rw)
		{
			int num = IntMath.Divide(sampleScale * 3, 10);
			for (int i = 0; i < 6; i++)
			{
				samplePos[samplePosCount].x = (int)IntMath.Divide(((long)rw.x * Agent.adpSamp_Steps2_Nom_rw[i] + (long)fw.x * Agent.adpSamp_Steps2_Nom_fw[i]) * Agent.adpSamp_Steps2_InnerScale.nom, Agent.adpSamp_Steps2_Den[i]);
				samplePos[samplePosCount].y = (int)IntMath.Divide(((long)rw.y * Agent.adpSamp_Steps2_Nom_rw[i] + (long)fw.y * Agent.adpSamp_Steps2_Nom_fw[i]) * Agent.adpSamp_Steps2_InnerScale.nom, Agent.adpSamp_Steps2_Den[i]);
				sampleSize[samplePosCount] = (long)num;
				samplePosCount++;
			}
		}

		private void CalcSamplePosScore(Simulator.WorkerContext context, ref VInt2 position2D, Agent.VO[] vos, int voCount, ref VInt2 optimalVelocity, ref VInt2 result, VInt2[] samplePos, long[] sampleSize, ref int samplePosCount, ref VInt2 desired2D)
		{
			VInt2[] bestPos = context.bestPos;
			long[] bestSizes = context.bestSizes;
			VFactor[] bestScores = context.bestScores;
			for (int i = 0; i < 3; i++)
			{
				bestScores[i] = new VFactor(2147483647L, 1L);
			}
			bestScores[3] = new VFactor(-2147483648L, 1L);
			VInt2 vInt = optimalVelocity;
			VFactor b = new VFactor(2147483647L, 1L);
			for (int j = 0; j < 3; j++)
			{
				for (int k = 0; k < samplePosCount; k++)
				{
					VFactor vFactor = VFactor.zero;
					for (int l = 0; l < voCount; l++)
					{
						VFactor vFactor2 = vos[l].ScalarSample(samplePos[k]);
						vFactor = ((vFactor > vFactor2) ? vFactor : vFactor2);
					}
					int magnitude = (samplePos[k] - desired2D).magnitude;
					VFactor vFactor3 = vFactor + new VFactor((long)magnitude, 5L);
					vFactor += new VFactor((long)magnitude, 1000L);
					if (this.DebugDraw)
					{
						Agent.DrawCross((Vector2)(position2D + samplePos[k]), Agent.Rainbow(Mathf.Log(vFactor.single + 1f) * 5f), (float)sampleSize[k] * 0.5f);
					}
					if (vFactor3 < bestScores[0])
					{
						for (int m = 0; m < 3; m++)
						{
							if (vFactor3 >= bestScores[m + 1])
							{
								for (int n = 0; n < m; n++)
								{
									bestScores[n] = bestScores[n + 1];
									bestSizes[n] = bestSizes[n + 1];
									bestPos[n] = bestPos[n + 1];
								}
								bestScores[m] = vFactor3;
								bestSizes[m] = sampleSize[k];
								bestPos[m] = samplePos[k];
								break;
							}
						}
					}
					if (vFactor < b)
					{
						vInt = samplePos[k];
						b = vFactor;
						if (vFactor.IsZero)
						{
							j = 100;
							break;
						}
					}
				}
				samplePosCount = 0;
				for (int num = 0; num < 3; num++)
				{
					VInt2 vInt2 = bestPos[num];
					long num2 = bestSizes[num];
					bestScores[num] = new VFactor(2147483647L, 1L);
					int num3 = (int)IntMath.Divide(num2 * 3L, 10L);
					samplePos[samplePosCount].x = vInt2.x + num3;
					samplePos[samplePosCount].y = vInt2.y + num3;
					samplePos[samplePosCount + 1].x = vInt2.x - num3;
					samplePos[samplePosCount + 1].y = vInt2.y + num3;
					samplePos[samplePosCount + 2].x = vInt2.x - num3;
					samplePos[samplePosCount + 2].y = vInt2.y - num3;
					samplePos[samplePosCount + 3].x = vInt2.x + num3;
					samplePos[samplePosCount + 3].y = vInt2.y - num3;
					num2 = IntMath.Divide(num2 * num2 * 3L, 5000L);
					sampleSize[samplePosCount] = num2;
					sampleSize[samplePosCount + 1] = num2;
					sampleSize[samplePosCount + 2] = num2;
					sampleSize[samplePosCount + 3] = num2;
					samplePosCount += 4;
				}
			}
			result = vInt;
		}

		private static Color Rainbow(float v)
		{
			Color result = new Color(v, 0f, 0f);
			if (result.r > 1f)
			{
				result.g = result.r - 1f;
				result.r = 1f;
			}
			if (result.g > 1f)
			{
				result.b = result.g - 1f;
				result.g = 1f;
			}
			return result;
		}

		private VInt2 Trace(Agent.VO[] vos, int voCount, VInt2 p, VFactor cutoff, out float score)
		{
			score = 0f;
			float stepScale = this.simulator.stepScale;
			float num = float.PositiveInfinity;
			VInt2 result = p;
			for (int i = 0; i < 50; i++)
			{
				float num2 = 1f - (float)i / 50f;
				num2 *= stepScale;
				VInt2 vInt = VInt2.zero;
				VFactor vFactor = VFactor.zero;
				for (int j = 0; j < voCount; j++)
				{
					VFactor vFactor2;
					VInt2 b = vos[j].Sample(p, out vFactor2);
					vInt += b;
					if (vFactor2 > vFactor)
					{
						vFactor = vFactor2;
					}
				}
				VInt2 a = this.desiredVelocity.xz - p;
				VFactor vFactor3 = new VFactor((long)a.magnitude, 500L);
				vInt += IntMath.Divide(a, 10L);
				vFactor = ((vFactor > vFactor3) ? vFactor : vFactor3);
				score = vFactor.single;
				if (score < num)
				{
					num = score;
				}
				result = p;
				if (score <= cutoff.single && i > 10)
				{
					break;
				}
				int sqrMagnitude = vInt.sqrMagnitude;
				if (sqrMagnitude > 0)
				{
					VFactor f = vFactor;
					f.den *= (long)IntMath.Sqrt((long)sqrMagnitude);
					vInt *= f;
				}
				vInt = (VInt2)((Vector2)vInt * num2);
				VInt2 ob = p + vInt;
				if (this.DebugDraw)
				{
					Debug.DrawLine(Agent.To3D((Vector2)ob) + (Vector3)this.position, Agent.To3D((Vector2)p) + (Vector3)this.position, Agent.Rainbow(0.1f / score) * new Color(1f, 1f, 1f, 0.2f));
				}
			}
			score = num;
			return result;
		}

		public static bool IntersectionFactor(VInt2 start1, VInt2 dir1, VInt2 start2, VInt2 dir2, out VFactor factor)
		{
			long num = (long)dir2.x;
			long num2 = (long)dir2.y;
			long num3 = num2 * (long)dir1.x - num * (long)dir1.y;
			if (num3 == 0L)
			{
				factor = VFactor.zero;
				return false;
			}
			long nom = num * (long)(start1.y - start2.y) - num2 * (long)(start1.x - start2.x);
			factor.nom = nom;
			factor.den = num3;
			return true;
		}

		public void InsertNeighbour(Agent agent, long distSq)
		{
			if (this.neighbours.get_Count() > 0)
			{
				if (this.neighbours.get_Count() == this.maxNeighbours)
				{
					int num = this.neighbours.get_Count() - 1;
					if (this.neighbourDists.get_Item(num) <= distSq)
					{
						return;
					}
					this.neighbours.set_Item(num, agent);
					this.neighbourDists.set_Item(num, distSq);
				}
				else
				{
					this.neighbours.Add(agent);
					this.neighbourDists.Add(distSq);
				}
				int num2 = this.neighbours.get_Count() - 1;
				while (num2 != 0 && distSq < this.neighbourDists.get_Item(num2 - 1))
				{
					this.neighbours.set_Item(num2, this.neighbours.get_Item(num2 - 1));
					this.neighbourDists.set_Item(num2, this.neighbourDists.get_Item(num2 - 1));
					num2--;
				}
				this.neighbours.set_Item(num2, agent);
				this.neighbourDists.set_Item(num2, distSq);
			}
			else
			{
				this.neighbours.Add(agent);
				this.neighbourDists.Add(distSq);
			}
		}

		public VInt3 FixVelocity(VInt2 result)
		{
			int sqrMagnitude = this.desiredVelocity.xz.sqrMagnitude;
			int num;
			if (sqrMagnitude == 0)
			{
				num = this.maxSpeed.i / 2;
			}
			else
			{
				num = IntMath.Sqrt((long)sqrMagnitude) * 3 / 2;
			}
			VInt2 vInt = result;
			long sqrMagnitudeLong = result.sqrMagnitudeLong;
			if (sqrMagnitudeLong > (long)(num * num))
			{
				vInt = IntMath.Divide(result, (long)num, (long)IntMath.Sqrt(sqrMagnitudeLong));
			}
			return new VInt3(vInt.x, 0, vInt.y);
		}

		public void computeNewVelocity()
		{
			Agent.orcaLines.Clear();
			this.hasCollided = false;
			int numObstLines = 0;
			VFactor f = new VFactor(1000L, (long)this.AgentTimeHorizon);
			VInt2 zero = VInt2.zero;
			VLine vLine = default(VLine);
			VInt2 a = default(VInt2);
			for (int i = 0; i < this.neighbours.get_Count(); i++)
			{
				Agent agent = this.neighbours.get_Item(i);
				VInt2 xz = (agent.position - this.position).xz;
				VInt2 xz2 = (this.velocity - agent.velocity).xz;
				long num = VInt2.DotLong(ref xz, ref xz);
				long num2 = (long)(this.radius.i + agent.radius.i);
				long num3 = num2 * num2;
				if (num > num3)
				{
					if (!this.hasCollided && num * 2L <= num3 * 3L)
					{
						this.hasCollided = true;
					}
					VInt2 vInt = xz2 - xz * f;
					long sqrMagnitudeLong = vInt.sqrMagnitudeLong;
					long num4 = VInt2.DotLong(ref vInt, ref xz);
					if (num4 < 0L && num4 * num4 > num3 * sqrMagnitudeLong)
					{
						VInt2 vInt2 = vInt;
						vInt2.Normalize();
						vLine.direction.x = vInt2.y;
						vLine.direction.y = -vInt2.x;
						long num5 = IntMath.Divide(f.nom * num2, f.den) - (long)IntMath.Sqrt(sqrMagnitudeLong);
						a.x = (int)IntMath.Divide((long)vInt2.x * num5, 1000L);
						a.y = (int)IntMath.Divide((long)vInt2.y * num5, 1000L);
					}
					else
					{
						long num6 = IntMath.SqrtLong(num - num3);
						if (VInt2.DetLong(ref xz, ref vInt) > 0L)
						{
							vLine.direction.x = (int)IntMath.Divide(((long)xz.x * num6 - (long)xz.y * num2) * 1000L, num);
							vLine.direction.y = (int)IntMath.Divide(((long)xz.y * num2 - (long)xz.y * num6) * 1000L, num);
						}
						else
						{
							vLine.direction.x = -(int)IntMath.Divide(((long)xz.x * num6 + (long)xz.y * num2) * 1000L, num);
							vLine.direction.y = -(int)IntMath.Divide((-(long)xz.x * num2 + (long)xz.y * num6) * 1000L, num);
						}
						vLine.direction.Normalize();
						long num7 = VInt2.DotLong(ref xz2, ref vLine.direction);
						a.x = (int)(IntMath.Divide((long)vLine.direction.x * num7, 1000000L) - (long)xz2.x);
						a.y = (int)(IntMath.Divide((long)vLine.direction.y * num7, 1000000L) - (long)xz2.y);
					}
				}
				else
				{
					this.hasCollided = true;
					VFactor f2 = new VFactor(1000L, 30L);
					VInt2 vInt3 = xz2 - xz * f2;
					VInt2 normalized = vInt3.normalized;
					vLine.direction = new VInt2(normalized.y, -normalized.x);
					long num8 = IntMath.Divide(num2 * f2.nom, f2.den) - (long)vInt3.magnitude;
					a.x = (int)IntMath.Divide((long)normalized.x * num8, 1000L);
					a.y = (int)IntMath.Divide((long)normalized.y * num8, 1000L);
				}
				vLine.point = this.velocity.xz + IntMath.Divide(a, 2L);
				Agent.orcaLines.Add(vLine);
			}
			int num9 = Agent.linearProgram2(Agent.orcaLines, this.maxSpeed.i, this.desiredVelocity.xz, false, ref zero);
			if (num9 < Agent.orcaLines.get_Count())
			{
				Agent.linearProgram3(Agent.orcaLines, numObstLines, num9, this.maxSpeed.i, ref zero);
			}
			this.newVelocity = this.FixVelocity(zero);
			Agent.orcaLines.Clear();
		}

		private void insertAgentNeighbor(Agent agent, ref float rangeSq)
		{
		}

		private static bool linearProgram1(List<VLine> lines, int lineNo, int radius, VInt2 optVelocity, bool directionOpt, ref VInt2 result)
		{
			VLine vLine = lines.get_Item(lineNo);
			long num = IntMath.Divide(VInt2.DotLong(ref vLine.point, ref vLine.direction), 1000L);
			long num2 = num * num + (long)radius * (long)radius - (long)vLine.point.sqrMagnitude;
			if (num2 < 0L)
			{
				return false;
			}
			long num3 = IntMath.SqrtLong(num2);
			VFactor vFactor = new VFactor(-num - num3, 1L);
			VFactor vFactor2 = new VFactor(-num + num3, 1L);
			VFactor vFactor3 = default(VFactor);
			for (int i = 0; i < lineNo; i++)
			{
				VLine vLine2 = lines.get_Item(i);
				vFactor3.den = VInt2.DetLong(ref vLine.direction, ref vLine2.direction);
				vFactor3.nom = VInt2.DetLong(vLine2.direction, vLine.point - vLine2.point);
				if (vFactor3.den == 0L)
				{
					if (vFactor3.nom < 0L)
					{
						return false;
					}
				}
				else
				{
					if (vFactor3.den > 0L)
					{
						vFactor2 = ((vFactor2 > vFactor3) ? vFactor2 : vFactor3);
					}
					else
					{
						vFactor = ((vFactor > vFactor3) ? vFactor : vFactor3);
					}
					if (vFactor > vFactor2)
					{
						return false;
					}
				}
			}
			if (directionOpt)
			{
				if (VInt2.DotLong(ref optVelocity, ref vLine.direction) > 0L)
				{
					result = vLine.point + IntMath.Divide(vLine.direction * vFactor2, 1000L);
				}
				else
				{
					result = vLine.point + IntMath.Divide(vLine.direction * vFactor, 1000L);
				}
			}
			else
			{
				VFactor b = new VFactor(VInt2.DotLong(vLine.direction, optVelocity - vLine.point), 1000L);
				if (vFactor > b)
				{
					result = vLine.point + IntMath.Divide(vLine.direction * vFactor, 1000L);
				}
				else if (vFactor2 < b)
				{
					result = vLine.point + IntMath.Divide(vLine.direction * vFactor2, 1000L);
				}
				else
				{
					result = vLine.point;
					result.x += (int)IntMath.Divide((long)vLine.direction.x * b.nom, b.den * 1000L);
					result.y += (int)IntMath.Divide((long)vLine.direction.y * b.nom, b.den * 1000L);
				}
			}
			return true;
		}

		private static int linearProgram2(List<VLine> lines, int radius, VInt2 optVelocity, bool directionOpt, ref VInt2 result)
		{
			if (directionOpt)
			{
				result = IntMath.Divide(optVelocity, (long)radius, 1000L);
			}
			else if (optVelocity.sqrMagnitudeLong > (long)radius * (long)radius)
			{
				result = IntMath.Divide(optVelocity.normalized, (long)radius, 1000L);
			}
			else
			{
				result = optVelocity;
			}
			for (int i = 0; i < lines.get_Count(); i++)
			{
				VLine vLine = lines.get_Item(i);
				if (VInt2.DetLong(vLine.direction, vLine.point - result) > 0L)
				{
					VInt2 vInt = result;
					if (!Agent.linearProgram1(lines, i, radius, optVelocity, directionOpt, ref result))
					{
						result = vInt;
						return i;
					}
				}
			}
			return lines.get_Count();
		}

		private static void linearProgram3(List<VLine> lines, int numObstLines, int beginLine, int radius, ref VInt2 result)
		{
			long num = 0L;
			for (int i = beginLine; i < lines.get_Count(); i++)
			{
				VLine vLine = lines.get_Item(i);
				if (VInt2.DetLong(vLine.direction, vLine.point - result) > num)
				{
					Agent.projLines.Clear();
					for (int j = 0; j < numObstLines; j++)
					{
						Agent.projLines.Add(lines.get_Item(j));
					}
					int k = numObstLines;
					while (k < i)
					{
						VLine vLine2 = lines.get_Item(k);
						long num2 = VInt2.DetLong(vLine.direction, vLine2.direction);
						VLine vLine3;
						if (num2 != 0L)
						{
							VInt2 b = IntMath.Divide(vLine.direction, VInt2.DetLong(vLine2.direction, vLine.point - vLine2.point), num2);
							vLine3.point = vLine.point + b;
							goto IL_153;
						}
						if (VInt2.DetLong(vLine.direction, vLine2.direction) <= 0L)
						{
							vLine3.point = vLine.point + vLine2.point;
							vLine3.point.x = vLine3.point.x / 2;
							vLine3.point.y = vLine3.point.y / 2;
							goto IL_153;
						}
						IL_148:
						k++;
						continue;
						IL_153:
						vLine3.direction = (vLine2.direction - vLine.direction).normalized;
						Agent.projLines.Add(vLine3);
						goto IL_148;
					}
					VInt2 vInt = result;
					if (Agent.linearProgram2(Agent.projLines, radius, new VInt2(-vLine.direction.y, vLine.direction.x), true, ref result) < Agent.projLines.get_Count())
					{
						result = vInt;
					}
					Agent.projLines.Clear();
					num = VInt2.DetLong(vLine.direction, vLine.point - result);
				}
			}
		}
	}
}
