using System;
using System.Collections.Generic;
using UnityEngine;

namespace ApolloUpdate
{
	public class ApolloUpdateSpeedCounter
	{
		private uint currentSize;

		private uint lastCurrentSize;

		private LinkedList<uint> mSpeedCountList = new LinkedList<uint>();

		private uint speed;

		private bool doTimer;

		public float timer = 1f;

		public void StartSpeedCounter()
		{
			this.doTimer = true;
			this.mSpeedCountList.Clear();
			this.lastCurrentSize = 0u;
			this.timer = 0f;
			this.speed = 0u;
		}

		public void StopSpeedCounter()
		{
			this.doTimer = false;
			this.mSpeedCountList.Clear();
			this.lastCurrentSize = 0u;
			this.timer = 0f;
			this.speed = 0u;
		}

		public void SetSize(uint size)
		{
			this.currentSize = size;
		}

		public void SpeedCounter()
		{
			if (!this.doTimer)
			{
				return;
			}
			this.timer -= Time.deltaTime;
			if (this.timer <= 0f)
			{
				this.timer = 1f;
				uint value = this.currentSize - this.lastCurrentSize;
				this.lastCurrentSize = this.currentSize;
				if (this.mSpeedCountList.Count >= 5)
				{
					this.mSpeedCountList.RemoveFirst();
					this.mSpeedCountList.AddLast(value);
				}
				else
				{
					this.mSpeedCountList.AddLast(value);
				}
				this.speed = value;
			}
		}

		public uint GetSpeed()
		{
			uint num = 1u;
			uint num2 = 0u;
			ulong num3 = 0uL;
			foreach (uint current in this.mSpeedCountList)
			{
				num3 += (ulong)(current * num * num);
				num2 += num * num;
				num += 1u;
			}
			return (uint)(num3 / (ulong)((num2 <= 0u) ? 1u : num2));
		}

		public uint GetCurrentSpeed()
		{
			return this.speed;
		}
	}
}
