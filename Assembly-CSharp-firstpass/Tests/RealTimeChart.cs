using System;
using UnityEngine;

namespace Tests
{
	public class RealTimeChart : MonoSingleton<RealTimeChart>
	{
		private ListView<Track> Tracks = new ListView<Track>();

		private bool bVisible;

		private Random RandomSupport = new Random();

		public bool isVisible
		{
			get
			{
				return this.bVisible;
			}
			set
			{
				this.bVisible = value;
			}
		}

		protected override void Init()
		{
		}

		public Track FindTrack(string InTag)
		{
			for (int i = 0; i < this.Tracks.Count; i++)
			{
				if (this.Tracks[i].tag == InTag)
				{
					return this.Tracks[i];
				}
			}
			return null;
		}

		public void RemoveTrack(string InTag)
		{
			for (int i = 0; i < this.Tracks.Count; i++)
			{
				if (this.Tracks[i].tag == InTag)
				{
					this.Tracks.RemoveAt(i);
					break;
				}
			}
		}

		public void RemoveTrack(Track InTrack)
		{
			if (InTrack != null)
			{
				this.RemoveTrack(InTrack.tag);
			}
		}

		public Track AddTrack(string InTag, Color InColor)
		{
			Track track = this.FindTrack(InTag);
			if (track == null)
			{
				track = new Track(InTag, InColor);
				this.Tracks.Add(track);
			}
			else
			{
				track.drawColor = InColor;
			}
			return track;
		}

		public Track AddTrack(string InTag, Color InColor, bool bFixedRange, float InMin, float InMax)
		{
			Track track = this.FindTrack(InTag);
			if (track == null)
			{
				track = new Track(InTag, InColor, InMin, InMax);
				this.Tracks.Add(track);
			}
			else
			{
				track.drawColor = InColor;
			}
			track.SetFixedRange(bFixedRange, InMin, InMax);
			return track;
		}

		public void AddSample(string InTag, float InValue)
		{
			Track track = this.FindTrack(InTag);
			if (track != null)
			{
				track.AddSample(InValue);
			}
			else
			{
				DebugHelper.Assert(false, "no valid track with tag {0}", new object[]
				{
					InTag
				});
			}
		}

		public void SetSample(string InTag, float InValue, int reverseIndex)
		{
			Track track = this.FindTrack(InTag);
			if (track != null)
			{
				track.SetSample(InValue, reverseIndex);
			}
			else
			{
				DebugHelper.Assert(false, "no valid track with tag {0}", new object[]
				{
					InTag
				});
			}
		}

		private void OnGUI()
		{
			if (!this.bVisible)
			{
				return;
			}
			this.DrawBase();
			this.DrawTracks();
		}

		protected void DrawBase()
		{
			Track.DrawLine(new Vector2(0f, 0f), new Vector2(0f, (float)Screen.height * (1f - Track.Clip * 2f)), Color.white, 2f);
			Track.DrawLine(new Vector2(0f, 0f), new Vector2((float)Screen.width * (1f - Track.Clip * 2f), 0f), Color.white, 2f);
		}

		protected void DrawTracks()
		{
			float num = 3.40282347E+38f;
			float num2 = -3.40282347E+38f;
			bool flag = false;
			for (int i = 0; i < this.Tracks.Count; i++)
			{
				if (this.Tracks[i].hasSamples)
				{
					flag = true;
					float num3;
					float num4;
					if (!this.Tracks[i].isFixedRange)
					{
						num3 = this.Tracks[i].minValue;
						num4 = this.Tracks[i].maxValue;
					}
					else
					{
						num3 = this.Tracks[i].fixedMinSampleValue;
						num4 = this.Tracks[i].fixedMaxSampleValue;
					}
					if (num3 < num)
					{
						num = num3;
					}
					if (num4 > num2)
					{
						num2 = num4;
					}
				}
			}
			if (flag)
			{
				for (int j = 0; j < this.Tracks.Count; j++)
				{
					if (this.Tracks[j].hasSamples && this.Tracks[j].isVisiable)
					{
						this.Tracks[j].OnRender(num, num2);
					}
				}
			}
		}
	}
}
