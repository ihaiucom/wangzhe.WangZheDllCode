using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using Pathfinding.RVO.Sampled;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding.RVO
{
	[AddComponentMenu("Pathfinding/Local Avoidance/RVO Controller")]
	public class RVOController : MonoBehaviour, IPooledMonoBehaviour
	{
		[Tooltip("Radius of the agent")]
		public VInt radius = 400;

		[Tooltip("Max speed of the agent. In world units/second")]
		public VInt maxSpeed = 10000;

		[Tooltip("Height of the agent. In world units")]
		public VInt height = 2000;

		[Tooltip("A locked unit cannot move. Other units will still avoid it. But avoidance quailty is not the best")]
		public bool locked;

		[Tooltip("Automatically set #locked to true when desired velocity is approximately zero")]
		public bool lockWhenNotMoving;

		[Tooltip("How far in the time to look for collisions with other agents")]
		public int agentTimeHorizon = 2000;

		[HideInInspector]
		public int obstacleTimeHorizon = 2000;

		[Tooltip("Maximum distance to other agents to take them into account for collisions.\nDecreasing this value can lead to better performance, increasing it can lead to better quality of the simulation")]
		public VInt neighbourDist = 2000;

		[Tooltip("Max number of other agents to take into account.\nA smaller value can reduce CPU load, a higher value can lead to better local avoidance quality.")]
		public int maxNeighbours = 6;

		[Tooltip("Layer mask for the ground. The RVOController will raycast down to check for the ground to figure out where to place the agent")]
		public LayerMask mask = -1;

		public RVOLayer layer = RVOLayer.DefaultAgent;

		[AstarEnumFlag]
		public RVOLayer collidesWith = (RVOLayer)(-1);

		[HideInInspector]
		public float wallAvoidForce = 1f;

		[HideInInspector]
		public float wallAvoidFalloff = 1f;

		[Tooltip("Center of the agent relative to the pivot point of this game object")]
		public VInt3 center = new VInt3(0, 1000, 0);

		public bool enableRotation;

		public float rotationSpeed = 30f;

		private Simulator simulator;

		private VInt adjustedY = 0;

		private PoolObjHandle<ActorRoot> actor;

		private VInt3 desiredVelocity;

		[HideInInspector]
		public bool checkNavNode;

		public bool debug;

		private VInt3 lastPosition;

		private static readonly Color GizmoColor = new Color(0.9411765f, 0.8352941f, 0.117647059f);

		public Agent rvoAgent
		{
			get;
			private set;
		}

		public VInt3 position
		{
			get
			{
				return this.rvoAgent.InterpolatedPosition;
			}
		}

		public VInt3 velocity
		{
			get
			{
				return this.rvoAgent.Velocity;
			}
		}

		public void OnCreate()
		{
		}

		public void OnGet()
		{
			this.radius = 400;
			this.maxSpeed = 10000;
			this.height = 2000;
			this.locked = false;
			this.lockWhenNotMoving = false;
			this.agentTimeHorizon = 2000;
			this.obstacleTimeHorizon = 2000;
			this.neighbourDist = 2000;
			this.maxNeighbours = 6;
			this.mask = -1;
			this.layer = RVOLayer.DefaultAgent;
			this.collidesWith = (RVOLayer)(-1);
			this.wallAvoidForce = 1f;
			this.wallAvoidFalloff = 1f;
			this.center = new VInt3(0, 1000, 0);
			this.enableRotation = false;
			this.rotationSpeed = 30f;
			this.simulator = null;
			this.adjustedY = 0;
			this.actor.Release();
			this.desiredVelocity = VInt3.zero;
			this.checkNavNode = false;
			this.lastPosition = VInt3.zero;
		}

		public void OnRecycle()
		{
			if (this.rvoAgent != null)
			{
				this.rvoAgent.owner = null;
				if (this.simulator != null)
				{
					this.simulator.SafeRemoveAgent(this.rvoAgent);
				}
			}
		}

		public void OnDestroy()
		{
			if (this.rvoAgent != null)
			{
				this.rvoAgent.owner = null;
				this.rvoAgent = null;
			}
		}

		public void OnDisable()
		{
			if (this.simulator == null)
			{
				return;
			}
			this.simulator.SafeRemoveAgent(this.rvoAgent);
		}

		public void EnsureActorAndSimulator()
		{
			if (!this.actor)
			{
				this.actor = ActorHelper.GetActorRoot(base.gameObject);
			}
			if (this.simulator == null)
			{
				RVOSimulator instance = RVOSimulator.GetInstance();
				if (instance == null)
				{
					return;
				}
				this.simulator = instance.GetSimulator();
			}
			if (this.simulator == null)
			{
				return;
			}
			VInt3 vInt;
			if (this.actor)
			{
				vInt = this.actor.handle.location;
			}
			else
			{
				vInt = (VInt3)base.transform.position;
			}
			if (this.rvoAgent != null)
			{
				if (!this.simulator.GetAgents().Contains(this.rvoAgent))
				{
					this.simulator.AddAgent(this.rvoAgent);
				}
			}
			else
			{
				this.rvoAgent = this.simulator.AddAgent(vInt);
			}
			if (this.rvoAgent != null)
			{
				this.rvoAgent.owner = base.gameObject;
			}
			this.UpdateAgentProperties();
			this.rvoAgent.Teleport(vInt);
			this.adjustedY = this.rvoAgent.Position.y;
		}

		public void OnEnable()
		{
			this.EnsureActorAndSimulator();
			if (this.rvoAgent != null)
			{
				this.rvoAgent.desiredVelocity = VInt3.zero;
				this.rvoAgent.DesiredVelocity = VInt3.zero;
				this.rvoAgent.newVelocity = VInt3.zero;
			}
		}

		protected void UpdateAgentProperties()
		{
			this.rvoAgent.Radius = this.radius;
			this.rvoAgent.MaxSpeed = this.maxSpeed;
			this.rvoAgent.Height = this.height;
			this.rvoAgent.AgentTimeHorizon = this.agentTimeHorizon;
			this.rvoAgent.ObstacleTimeHorizon = this.obstacleTimeHorizon;
			this.rvoAgent.Locked = this.locked;
			this.rvoAgent.MaxNeighbours = this.maxNeighbours;
			this.rvoAgent.DebugDraw = this.debug;
			this.rvoAgent.NeighbourDist = this.neighbourDist;
			this.rvoAgent.Layer = this.layer;
			this.rvoAgent.CollidesWith = this.collidesWith;
		}

		public void Move(VInt3 vel)
		{
			this.desiredVelocity = vel;
		}

		public void Teleport(VInt3 pos)
		{
			this.actor.handle.location = pos;
			this.lastPosition = pos;
			this.rvoAgent.Teleport(pos);
			this.adjustedY = pos.y;
		}

		public void DoUpdate(float dt)
		{
			if (this.rvoAgent == null)
			{
				return;
			}
			ActorRoot handle = this.actor.handle;
			if (this.lastPosition != handle.location)
			{
				this.Teleport(handle.location);
			}
			if (this.lockWhenNotMoving)
			{
				this.locked = (this.desiredVelocity == VInt3.zero);
			}
			this.UpdateAgentProperties();
			VInt3 interpolatedPosition = this.rvoAgent.InterpolatedPosition;
			this.rvoAgent.SetYPosition(this.adjustedY);
			this.rvoAgent.DesiredVelocity = this.desiredVelocity;
			VInt3 vInt = interpolatedPosition - this.center;
			vInt.y += this.height.i >> 1;
			if (this.checkNavNode)
			{
				VInt3 delta = vInt - handle.location;
				VInt groundY;
				VInt3 rhs = PathfindingUtility.Move(this.actor, delta, out groundY, out handle.hasReachedNavEdge, null);
				VInt3 vInt2 = handle.location + rhs;
				handle.location = vInt2;
				handle.groundY = groundY;
				this.rvoAgent.Teleport(vInt2);
				this.adjustedY = vInt2.y;
			}
			else
			{
				handle.location = vInt;
			}
			this.lastPosition = handle.location;
			if (this.enableRotation && this.velocity != VInt3.zero)
			{
				Vector3 forward = (Vector3)this.velocity;
				Transform transform = base.transform;
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(forward), dt * this.rotationSpeed * Mathf.Min((float)this.velocity.magnitude, 0.2f));
			}
		}

		public void Update()
		{
			if (!RVOSimulator.IsFrameMode)
			{
				this.DoUpdate(Time.deltaTime);
			}
		}

		public void UpdateLogic(int dt)
		{
			if (RVOSimulator.IsFrameMode)
			{
				this.DoUpdate((float)dt * 0.001f);
			}
		}

		public void OnDrawGizmos()
		{
			float d = (float)this.height;
			float d2 = (float)this.radius;
			Vector3 b = (Vector3)this.center;
			Gizmos.color = RVOController.GizmoColor;
			Gizmos.DrawWireSphere(base.transform.position + b - Vector3.up * d * 0.5f + Vector3.up * d2 * 0.5f, d2);
			Gizmos.DrawLine(base.transform.position + b - Vector3.up * d * 0.5f, base.transform.position + b + Vector3.up * d * 0.5f);
			Gizmos.DrawWireSphere(base.transform.position + b + Vector3.up * d * 0.5f - Vector3.up * d2 * 0.5f, d2);
		}

		public List<GameObject> GetNeighbours(bool colliding)
		{
			if (!base.enabled || this.rvoAgent == null)
			{
				return null;
			}
			Agent rvoAgent = this.rvoAgent;
			if (rvoAgent.neighbours.Count == 0)
			{
				return null;
			}
			List<GameObject> list = new List<GameObject>();
			for (int i = 0; i < rvoAgent.neighbours.Count; i++)
			{
				Agent agent = rvoAgent.neighbours[i];
				if (agent != rvoAgent && agent.owner != null)
				{
					if (colliding)
					{
						int num = Math.Min(rvoAgent.position.y + rvoAgent.height.i, agent.position.y + agent.height.i);
						int num2 = Math.Max(rvoAgent.position.y, agent.position.y);
						if (num - num2 >= 0)
						{
							VInt3 vInt = agent.position - rvoAgent.position;
							vInt.y = 0;
							long num3 = (long)(rvoAgent.radius.i + agent.radius.i);
							num3 *= num3;
							if (vInt.sqrMagnitudeLong < num3)
							{
								list.Add(agent.owner as GameObject);
							}
						}
					}
					else
					{
						list.Add(agent.owner as GameObject);
					}
				}
			}
			return list;
		}
	}
}
