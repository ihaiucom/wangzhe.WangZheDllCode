using System;

namespace Assets.Scripts.GameSystem
{
	public class SampleData
	{
		public const int AUTO_GROW_STEP = 32;

		private int[] _datas;

		public float step
		{
			get;
			set;
		}

		public int count
		{
			get;
			private set;
		}

		public int min
		{
			get;
			private set;
		}

		public int max
		{
			get;
			private set;
		}

		public int curDataLeft
		{
			get;
			private set;
		}

		public int curDataRight
		{
			get;
			private set;
		}

		public int this[int index]
		{
			get
			{
				return this._datas[index];
			}
		}

		public SampleData(float step)
		{
			this.step = step;
			this._datas = new int[32];
			this.count = 0;
			this.min = 0;
			this.max = 0;
		}

		public void Add(int data)
		{
			if (this.count >= this._datas.Length)
			{
				int[] array = new int[this.count + 32];
				Buffer.BlockCopy(this._datas, 0, array, 0, this.count * 4);
				this._datas = array;
			}
			this._datas[this.count++] = data;
			if (data < this.min)
			{
				this.min = data;
			}
			if (data > this.max)
			{
				this.max = data;
			}
		}

		public void SetCurData(int left, int right)
		{
			this.curDataLeft = left;
			this.curDataRight = right;
			this.Add(left - right);
		}

		public void Clear(bool keepSize)
		{
			this.count = 0;
			this.min = 0;
			this.max = 0;
			if (!keepSize)
			{
				this._datas = new int[32];
			}
		}
	}
}
