using System;
using System.Collections.Generic;

namespace Pathfinding.RVO
{
	public interface IAgent
	{
		VInt3 InterpolatedPosition
		{
			get;
		}

		VInt3 Position
		{
			get;
		}

		VInt3 DesiredVelocity
		{
			get;
			set;
		}

		VInt3 Velocity
		{
			get;
			set;
		}

		bool Locked
		{
			get;
			set;
		}

		VInt Radius
		{
			get;
			set;
		}

		VInt Height
		{
			get;
			set;
		}

		VInt MaxSpeed
		{
			get;
			set;
		}

		VInt NeighbourDist
		{
			get;
			set;
		}

		int AgentTimeHorizon
		{
			get;
			set;
		}

		int ObstacleTimeHorizon
		{
			get;
			set;
		}

		RVOLayer Layer
		{
			get;
			set;
		}

		RVOLayer CollidesWith
		{
			get;
			set;
		}

		bool DebugDraw
		{
			get;
			set;
		}

		int MaxNeighbours
		{
			get;
			set;
		}

		List<ObstacleVertex> NeighbourObstacles
		{
			get;
		}

		void SetYPosition(VInt yCoordinate);

		void Teleport(VInt3 pos);
	}
}
