using ResData;
using System;
using System.Runtime.CompilerServices;

namespace Assets.Scripts.GameLogic
{
	internal class StarEvaluation : IStarEvaluation
	{
		private static StarSystemFactory ConditionFactory = new StarSystemFactory(typeof(StarConditionAttribute), typeof(IStarCondition));

		public ListView<IStarCondition> Conditions = new ListView<IStarCondition>(3);

		public ResEvaluateStarInfo StarInfo;

		public int Index;

		private string Description;

		public event OnEvaluationChangedDelegate OnChanged;

		public virtual string description
		{
			get
			{
				if (this.StarInfo.bHideDetail > 0)
				{
					return this.Description;
				}
				return this.Description + ((this.Conditions.Count <= 0 || this.Conditions[0] == null) ? string.Empty : this.Conditions[0].description);
			}
		}

		public string rawDescription
		{
			get
			{
				return this.Description;
			}
		}

		public ResEvaluateStarInfo configInfo
		{
			get
			{
				return this.StarInfo;
			}
		}

		public int index
		{
			get
			{
				return this.Index;
			}
		}

		public RES_LOGIC_OPERATION_TYPE logicType
		{
			get
			{
				return (RES_LOGIC_OPERATION_TYPE)this.StarInfo.bLogicType;
			}
		}

		public StarEvaluationStatus status
		{
			get
			{
				if (this.logicType == RES_LOGIC_OPERATION_TYPE.RES_LOGIC_OPERATION_AND)
				{
					bool flag = true;
					for (int i = 0; i < this.Conditions.Count; i++)
					{
						DebugHelper.Assert(this.Conditions[i] != null);
						if (this.Conditions[i].status == StarEvaluationStatus.Failure)
						{
							return StarEvaluationStatus.Failure;
						}
						if (this.Conditions[i].status == StarEvaluationStatus.InProgressing)
						{
							flag = false;
						}
					}
					return (!flag) ? StarEvaluationStatus.InProgressing : StarEvaluationStatus.Success;
				}
				if (this.logicType == RES_LOGIC_OPERATION_TYPE.RES_LOGIC_OPERATION_OR)
				{
					for (int j = 0; j < this.Conditions.Count; j++)
					{
						DebugHelper.Assert(this.Conditions[j] != null);
						if (this.Conditions[j].status == StarEvaluationStatus.Success)
						{
							return StarEvaluationStatus.Success;
						}
					}
					return StarEvaluationStatus.Failure;
				}
				DebugHelper.Assert(false, "未识别的逻辑关系");
				return StarEvaluationStatus.Failure;
			}
		}

		public bool isSuccess
		{
			get
			{
				return this.status == StarEvaluationStatus.Success;
			}
		}

		public bool isFailure
		{
			get
			{
				return this.status == StarEvaluationStatus.Failure;
			}
		}

		public bool isInProgressing
		{
			get
			{
				return this.status == StarEvaluationStatus.InProgressing;
			}
		}

		protected void AddCondition(ResDT_ConditionInfo InCondConfig)
		{
			StarCondition starCondition = StarEvaluation.ConditionFactory.Create((int)InCondConfig.dwType) as StarCondition;
			DebugHelper.Assert(starCondition != null);
			if (starCondition != null)
			{
				starCondition.OnStarConditionChanged += new OnStarConditionChangedDelegate(this.OnConditionChanged);
				starCondition.Initialize(InCondConfig);
				this.Conditions.Add(starCondition);
			}
		}

		protected void OnConditionChanged(IStarCondition InCondition)
		{
			DebugHelper.Assert(InCondition != null);
			if (this.OnChanged != null)
			{
				this.OnChanged(this, InCondition);
			}
		}

		public void Initialize(ResEvaluateStarInfo InStarInfo)
		{
			this.StarInfo = InStarInfo;
			this.Description = Utility.UTF8Convert(InStarInfo.szCondDesc);
			for (int i = 0; i < InStarInfo.astConditions.Length; i++)
			{
				ResDT_ConditionInfo resDT_ConditionInfo = InStarInfo.astConditions[i];
				if (resDT_ConditionInfo.dwType == 0u)
				{
					break;
				}
				this.AddCondition(resDT_ConditionInfo);
			}
		}

		public IStarCondition GetConditionAt(int Index)
		{
			IStarCondition arg_2C_0;
			if (Index >= 0 && Index < this.Conditions.Count)
			{
				IStarCondition starCondition = this.Conditions[Index];
				arg_2C_0 = starCondition;
			}
			else
			{
				arg_2C_0 = null;
			}
			return arg_2C_0;
		}

		public virtual void Start()
		{
			for (int i = 0; i < this.Conditions.Count; i++)
			{
				this.Conditions[i].Start();
			}
		}

		public virtual void OnActorDeath(ref GameDeadEventParam prm)
		{
			int num = this.Conditions.Count - 1;
			while (num >= 0 && num < this.Conditions.Count)
			{
				IStarCondition starCondition = this.Conditions[num];
				if (starCondition != null)
				{
					starCondition.OnActorDeath(ref prm);
				}
				num--;
			}
		}

		public virtual void OnCampScoreUpdated(ref SCampScoreUpdateParam prm)
		{
			int num = this.Conditions.Count - 1;
			while (num >= 0 && num < this.Conditions.Count)
			{
				IStarCondition starCondition = this.Conditions[num];
				if (starCondition != null)
				{
					starCondition.OnCampScoreUpdated(ref prm);
				}
				num--;
			}
		}

		public void Dispose()
		{
			for (int i = 0; i < this.Conditions.Count; i++)
			{
				if (this.Conditions[i] != null)
				{
					this.Conditions[i].Dispose();
				}
			}
			this.Conditions.Clear();
		}

		public ListView<IStarCondition>.Enumerator GetEnumerator()
		{
			return this.Conditions.GetEnumerator();
		}
	}
}
