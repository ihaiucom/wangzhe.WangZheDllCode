using Pathfinding.RVO.Sampled;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Pathfinding.RVO
{
	public class Simulator
	{
		public enum SamplingAlgorithm
		{
			AdaptiveSampling,
			GradientDecent
		}

		internal class WorkerContext
		{
			public const int KeepCount = 3;

			public Agent.VO[] vos = new Agent.VO[20];

			public VInt2[] bestPos = new VInt2[3];

			public long[] bestSizes = new long[3];

			public VFactor[] bestScores = new VFactor[4];

			public VInt2[] samplePos = new VInt2[50];

			public long[] sampleSize = new long[50];
		}

		private class Worker
		{
			[NonSerialized]
			public Thread thread;

			public int start;

			public int end;

			public int task;

			public AutoResetEvent runFlag = new AutoResetEvent(false);

			public ManualResetEvent waitFlag = new ManualResetEvent(true);

			public Simulator simulator;

			private bool terminate;

			private Simulator.WorkerContext context = new Simulator.WorkerContext();

			public Worker(Simulator sim)
			{
				this.simulator = sim;
				this.thread = new Thread(new ThreadStart(this.Run));
				this.thread.set_IsBackground(true);
				this.thread.set_Name("RVO Simulator Thread");
				this.thread.Start();
			}

			public void Execute(int task)
			{
				this.task = task;
				this.waitFlag.Reset();
				this.runFlag.Set();
			}

			public void WaitOne()
			{
				this.waitFlag.WaitOne();
			}

			public void Terminate()
			{
				this.terminate = true;
			}

			public void Run()
			{
				this.runFlag.WaitOne();
				while (!this.terminate)
				{
					try
					{
						List<Agent> agents = this.simulator.GetAgents();
						if (this.task == 0)
						{
							for (int i = this.start; i < this.end; i++)
							{
								agents.get_Item(i).CalculateNeighbours();
								agents.get_Item(i).CalculateVelocity(this.context);
							}
						}
						else if (this.task == 1)
						{
							for (int j = this.start; j < this.end; j++)
							{
								agents.get_Item(j).Update();
								agents.get_Item(j).BufferSwitch();
							}
						}
						else
						{
							if (this.task != 2)
							{
								Debug.LogError("Invalid Task Number: " + this.task);
								throw new Exception("Invalid Task Number: " + this.task);
							}
							this.simulator.BuildQuadtree();
						}
					}
					catch (Exception message)
					{
						Debug.LogError(message);
					}
					this.waitFlag.Set();
					this.runFlag.WaitOne();
				}
			}
		}

		private bool doubleBuffering = true;

		private float desiredDeltaTime = 0.05f;

		private bool interpolation = true;

		private Simulator.Worker[] workers;

		public List<Agent> agents;

		public List<ObstacleVertex> obstacles;

		public Simulator.SamplingAlgorithm algorithm;

		private RVOQuadtree quadtree = new RVOQuadtree();

		public VFactor qualityCutoff = new VFactor
		{
			nom = 1L,
			den = 20L
		};

		public float stepScale = 1.5f;

		private float lastStep = -99999f;

		private float lastStepInterpolationReference = -9999f;

		private bool doUpdateObstacles;

		private bool doCleanObstacles;

		private bool oversampling;

		private VFactor wallThickness = VFactor.one;

		public int DeltaTimeMS;

		private Simulator.WorkerContext coroutineWorkerContext = new Simulator.WorkerContext();

		public static long frameNum;

		public RVOQuadtree Quadtree
		{
			get
			{
				return this.quadtree;
			}
		}

		public bool Multithreading
		{
			get
			{
				return this.workers != null && this.workers.Length > 0;
			}
		}

		public VFactor WallThickness
		{
			get
			{
				return this.wallThickness;
			}
			set
			{
				this.wallThickness = value;
			}
		}

		public bool Interpolation
		{
			get
			{
				return this.interpolation;
			}
			set
			{
				this.interpolation = value;
			}
		}

		public bool Oversampling
		{
			get
			{
				return this.oversampling;
			}
			set
			{
				this.oversampling = value;
			}
		}

		public Simulator(int workers, bool doubleBuffering)
		{
			this.workers = new Simulator.Worker[workers];
			this.doubleBuffering = doubleBuffering;
			for (int i = 0; i < workers; i++)
			{
				this.workers[i] = new Simulator.Worker(this);
			}
			this.agents = new List<Agent>();
			this.obstacles = new List<ObstacleVertex>();
		}

		public List<Agent> GetAgents()
		{
			return this.agents;
		}

		public List<ObstacleVertex> GetObstacles()
		{
			return this.obstacles;
		}

		public void ClearAgents()
		{
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			for (int j = 0; j < this.agents.get_Count(); j++)
			{
				this.agents.get_Item(j).simulator = null;
			}
			this.agents.Clear();
		}

		public void OnDestroy()
		{
			if (this.workers != null)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].Terminate();
				}
			}
		}

		~Simulator()
		{
			this.OnDestroy();
		}

		public Agent AddAgent(Agent agent)
		{
			if (agent == null)
			{
				throw new ArgumentNullException("Agent must not be null");
			}
			if (agent.simulator != null && agent.simulator == this)
			{
				throw new ArgumentException("The agent is already in the simulation");
			}
			if (agent.simulator != null)
			{
				throw new ArgumentException("The agent is already added to another simulation");
			}
			agent.simulator = this;
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			this.agents.Add(agent);
			return agent;
		}

		public Agent AddAgent(VInt3 position)
		{
			Agent agent = new Agent(position);
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			this.agents.Add(agent);
			agent.simulator = this;
			return agent;
		}

		public void SafeRemoveAgent(IAgent agent)
		{
			if (agent == null)
			{
				return;
			}
			Agent agent2 = agent as Agent;
			if (agent2 == null || agent2.simulator != this)
			{
				return;
			}
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			agent2.simulator = null;
			this.agents.Remove(agent2);
		}

		public void RemoveAgent(IAgent agent)
		{
			if (agent == null)
			{
				throw new ArgumentNullException("Agent must not be null");
			}
			Agent agent2 = agent as Agent;
			if (agent2 == null)
			{
				throw new ArgumentException("The agent must be of type Agent. Agent was of type " + agent.GetType());
			}
			if (agent2.simulator != this)
			{
				throw new ArgumentException("The agent is not added to this simulation");
			}
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			agent2.simulator = null;
			if (!this.agents.Remove(agent2))
			{
				throw new ArgumentException("Critical Bug! This should not happen. Please report this.");
			}
		}

		public ObstacleVertex AddObstacle(ObstacleVertex v)
		{
			if (v == null)
			{
				throw new ArgumentNullException("Obstacle must not be null");
			}
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			this.obstacles.Add(v);
			this.UpdateObstacles();
			return v;
		}

		public ObstacleVertex AddObstacle(VInt3[] vertices, int height)
		{
			return this.AddObstacle(vertices, height, Matrix4x4.identity, RVOLayer.DefaultObstacle);
		}

		public ObstacleVertex AddObstacle(VInt3[] vertices, int height, Matrix4x4 matrix, RVOLayer layer = RVOLayer.DefaultObstacle)
		{
			if (vertices == null)
			{
				throw new ArgumentNullException("Vertices must not be null");
			}
			if (vertices.Length < 2)
			{
				throw new ArgumentException("Less than 2 vertices in an obstacle");
			}
			ObstacleVertex obstacleVertex = null;
			ObstacleVertex obstacleVertex2 = null;
			bool flag = matrix == Matrix4x4.identity;
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			for (int j = 0; j < vertices.Length; j++)
			{
				ObstacleVertex obstacleVertex3 = new ObstacleVertex();
				if (obstacleVertex == null)
				{
					obstacleVertex = obstacleVertex3;
				}
				else
				{
					obstacleVertex2.next = obstacleVertex3;
				}
				obstacleVertex3.prev = obstacleVertex2;
				obstacleVertex3.layer = layer;
				obstacleVertex3.position = (flag ? vertices[j] : ((VInt3)matrix.MultiplyPoint3x4((Vector3)vertices[j])));
				obstacleVertex3.height = height;
				obstacleVertex2 = obstacleVertex3;
			}
			obstacleVertex2.next = obstacleVertex;
			obstacleVertex.prev = obstacleVertex2;
			ObstacleVertex obstacleVertex4 = obstacleVertex;
			do
			{
				obstacleVertex4.dir = (obstacleVertex4.next.position - obstacleVertex4.position).xz.normalized;
				obstacleVertex4 = obstacleVertex4.next;
			}
			while (obstacleVertex4 != obstacleVertex);
			this.obstacles.Add(obstacleVertex);
			this.UpdateObstacles();
			return obstacleVertex;
		}

		public ObstacleVertex AddObstacle(VInt3 a, VInt3 b, VInt height)
		{
			ObstacleVertex obstacleVertex = new ObstacleVertex();
			ObstacleVertex obstacleVertex2 = new ObstacleVertex();
			obstacleVertex.prev = obstacleVertex2;
			obstacleVertex2.prev = obstacleVertex;
			obstacleVertex.next = obstacleVertex2;
			obstacleVertex2.next = obstacleVertex;
			obstacleVertex.position = a;
			obstacleVertex2.position = b;
			obstacleVertex.height = height;
			obstacleVertex2.height = height;
			obstacleVertex2.ignore = true;
			VInt2 vInt = new VInt2(b.x - a.x, b.z - a.z);
			vInt.Normalize();
			obstacleVertex.dir = vInt;
			obstacleVertex2.dir = -vInt;
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			this.obstacles.Add(obstacleVertex);
			this.UpdateObstacles();
			return obstacleVertex;
		}

		public void UpdateObstacle(ObstacleVertex obstacle, VInt3[] vertices, Matrix4x4 matrix)
		{
			if (vertices == null)
			{
				throw new ArgumentNullException("Vertices must not be null");
			}
			if (obstacle == null)
			{
				throw new ArgumentNullException("Obstacle must not be null");
			}
			if (vertices.Length < 2)
			{
				throw new ArgumentException("Less than 2 vertices in an obstacle");
			}
			if (obstacle.split)
			{
				throw new ArgumentException("Obstacle is not a start vertex. You should only pass those ObstacleVertices got from AddObstacle method calls");
			}
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			int num = 0;
			ObstacleVertex obstacleVertex = obstacle;
			while (true)
			{
				while (obstacleVertex.next.split)
				{
					obstacleVertex.next = obstacleVertex.next.next;
					obstacleVertex.next.prev = obstacleVertex;
				}
				if (num >= vertices.Length)
				{
					break;
				}
				obstacleVertex.position = (VInt3)matrix.MultiplyPoint3x4((Vector3)vertices[num]);
				num++;
				obstacleVertex = obstacleVertex.next;
				if (obstacleVertex == obstacle)
				{
					goto Block_9;
				}
			}
			Debug.DrawLine((Vector3)obstacleVertex.prev.position, (Vector3)obstacleVertex.position, Color.red);
			throw new ArgumentException("Obstacle has more vertices than supplied for updating (" + vertices.Length + " supplied)");
			Block_9:
			obstacleVertex = obstacle;
			do
			{
				obstacleVertex.dir = (obstacleVertex.next.position - obstacleVertex.position).xz.normalized;
				obstacleVertex = obstacleVertex.next;
			}
			while (obstacleVertex != obstacle);
			this.ScheduleCleanObstacles();
			this.UpdateObstacles();
		}

		private void ScheduleCleanObstacles()
		{
			this.doCleanObstacles = true;
		}

		private void CleanObstacles()
		{
			for (int i = 0; i < this.obstacles.get_Count(); i++)
			{
				ObstacleVertex obstacleVertex = this.obstacles.get_Item(i);
				ObstacleVertex obstacleVertex2 = obstacleVertex;
				do
				{
					while (obstacleVertex2.next.split)
					{
						obstacleVertex2.next = obstacleVertex2.next.next;
						obstacleVertex2.next.prev = obstacleVertex2;
					}
					obstacleVertex2 = obstacleVertex2.next;
				}
				while (obstacleVertex2 != obstacleVertex);
			}
		}

		public void RemoveObstacle(ObstacleVertex v)
		{
			if (v == null)
			{
				throw new ArgumentNullException("Vertex must not be null");
			}
			if (this.Multithreading && this.doubleBuffering)
			{
				for (int i = 0; i < this.workers.Length; i++)
				{
					this.workers[i].WaitOne();
				}
			}
			this.obstacles.Remove(v);
			this.UpdateObstacles();
		}

		public void UpdateObstacles()
		{
			this.doUpdateObstacles = true;
		}

		private void BuildQuadtree()
		{
			this.quadtree.Clear();
			if (this.agents.get_Count() > 0)
			{
				VRect bounds = VRect.MinMaxRect(this.agents.get_Item(0).position.x, this.agents.get_Item(0).position.y, this.agents.get_Item(0).position.x, this.agents.get_Item(0).position.y);
				for (int i = 1; i < this.agents.get_Count(); i++)
				{
					VInt3 position = this.agents.get_Item(i).position;
					bounds = VRect.MinMaxRect(Mathf.Min(bounds.xMin, position.x), Mathf.Min(bounds.yMin, position.z), Mathf.Max(bounds.xMax, position.x), Mathf.Max(bounds.yMax, position.z));
				}
				this.quadtree.SetBounds(bounds);
				for (int j = 0; j < this.agents.get_Count(); j++)
				{
					this.quadtree.Insert(this.agents.get_Item(j));
				}
			}
		}

		private void CalculateAgentNeighbours()
		{
			for (int i = 0; i < this.agents.get_Count(); i++)
			{
				Agent agent = this.agents.get_Item(i);
				agent.neighbours.Clear();
				agent.neighbourDists.Clear();
			}
			for (int j = 0; j < this.agents.get_Count(); j++)
			{
				Agent agent2 = this.agents.get_Item(j);
				long num = (long)(agent2.neighbourDist.i * agent2.neighbourDist.i);
				for (int k = j + 1; k < this.agents.get_Count(); k++)
				{
					Agent agent3 = this.agents.get_Item(k);
					if (!agent2.locked || !agent3.locked)
					{
						long num2 = (long)(agent3.neighbourDist.i * agent3.neighbourDist.i);
						long num3 = agent2.position.XZSqrMagnitude(ref agent3.position);
						if (num3 < num && !agent2.locked)
						{
							agent2.InsertNeighbour(agent3, num3);
						}
						if (num3 < num2 && !agent3.locked)
						{
							agent3.InsertNeighbour(agent2, num3);
						}
					}
				}
			}
		}

		public void UpdateLogic(int nDelta)
		{
			this.DeltaTimeMS = nDelta;
			if (this.doCleanObstacles)
			{
				this.CleanObstacles();
				this.doCleanObstacles = false;
				this.doUpdateObstacles = true;
			}
			if (this.doUpdateObstacles)
			{
				this.doUpdateObstacles = false;
			}
			for (int i = 0; i < this.agents.get_Count(); i++)
			{
				Agent agent = this.agents.get_Item(i);
				agent.Update();
				agent.BufferSwitch();
			}
			this.CalculateAgentNeighbours();
			for (int j = 0; j < this.agents.get_Count(); j++)
			{
				Agent agent2 = this.agents.get_Item(j);
				agent2.computeNewVelocity();
			}
			for (int k = 0; k < this.agents.get_Count(); k++)
			{
				Agent agent3 = this.agents.get_Item(k);
				agent3.Interpolate();
			}
		}
	}
}
