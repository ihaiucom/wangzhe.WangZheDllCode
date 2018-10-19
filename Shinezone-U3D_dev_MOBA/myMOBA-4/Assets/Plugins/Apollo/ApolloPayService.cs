using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Apollo
{
	internal class ApolloPayService : ApolloObject, IApolloServiceBase, IApolloPayService
	{
		public static readonly ApolloPayService Instance = new ApolloPayService();

		private DictionaryView<int, ApolloActionDelegate> actionCallbackCollection = new DictionaryView<int, ApolloActionDelegate>();

		private bool inited;

		public event OnApolloPaySvrEvenHandle PayEvent;

		private ApolloPayService()
		{
			Console.WriteLine("ApolloPayService Create!{0}", base.ObjectId);
		}

		public bool Initialize(ApolloBufferBase registerInfo)
		{
			ADebug.Log("ApolloPayService Initialize!");
			this.inited = true;
			byte[] array;
			registerInfo.Encode(out array);
			return ApolloPayService.apollo_pay_Initialize(array, array.Length);
		}

		public bool Pay(ApolloActionBufferBase payInfo)
		{
			byte[] array;
			payInfo.Encode(out array);
			return this.inited && ApolloPayService.apollo_pay_Pay(array, array.Length);
		}

		public bool Pay4Mounth(ApolloBufferBase pay4MountInfo)
		{
			byte[] array;
			pay4MountInfo.Encode(out array);
			return this.inited && ApolloPayService.apollo_pay_Pay4Mounth(array, array.Length);
		}

		public bool Dipose()
		{
			this.inited = false;
			return ApolloPayService.apollo_pay_Dipose();
		}

		public IApolloExtendPayServiceBase GetExtendService()
		{
			PluginBase currentPlugin = PluginManager.Instance.GetCurrentPlugin();
			if (currentPlugin == null)
			{
				return null;
			}
			return currentPlugin.GetPayExtendService();
		}

		[Obsolete("Obsolete since V1.1.6", true)]
		public bool ApolloPaySvrInit(ApolloBufferBase registerInfo)
		{
			return this.Initialize(registerInfo);
		}

		[Obsolete("Obsolete since V1.1.6", true)]
		public bool ApolloPay(ApolloPayInfoBase payInfo)
		{
			return this.Pay(payInfo);
		}

		[Obsolete("Obsolete since V1.1.6", true)]
		public bool ApolloPaySvrUninit()
		{
			return this.Dipose();
		}

		public void OnApolloPaySvrNotify(byte[] data)
		{
			ADebug.Log("ApolloPay OnApolloPaySvrNotify!");
			if (this.PayEvent != null)
			{
				PluginBase currentPlugin = PluginManager.Instance.GetCurrentPlugin();
				if (currentPlugin == null)
				{
					ADebug.LogError("OnApolloPaySvrNotify plugin is null");
					return;
				}
				ApolloAction apolloAction = new ApolloAction();
				if (!apolloAction.Decode(data))
				{
					ADebug.LogError("OnApolloPaySvrNotify Action Decode failed");
					return;
				}
				ApolloBufferBase apolloBufferBase = currentPlugin.CreatePayResponseInfo(apolloAction.Action);
				if (apolloBufferBase != null)
				{
					if (!apolloBufferBase.Decode(data))
					{
						ADebug.LogError("OnApolloPaySvrNotify Decode failed");
						return;
					}
					this.PayEvent(apolloBufferBase);
				}
				else
				{
					ADebug.LogError("OnApolloPaySvrNotify info is null");
				}
			}
			else
			{
				ADebug.Log("PayEvent is null");
			}
		}

		public void Action(ApolloActionBufferBase info, ApolloActionDelegate callback)
		{
			if (info == null)
			{
				ADebug.LogError("PayService Action Info == null");
				return;
			}
			byte[] array;
			if (!info.Encode(out array))
			{
				ADebug.LogError("Action Encode error!");
				return;
			}
			if (this.actionCallbackCollection.ContainsKey(info.Action))
			{
				this.actionCallbackCollection[info.Action] = callback;
			}
			else
			{
				this.actionCallbackCollection.Add(info.Action, callback);
			}
			ApolloPayService.apollo_pay_action(array, array.Length);
		}

		public void OnApolloPayActionProc(int ret, byte[] data)
		{
			ADebug.Log("OnApolloPayActionProc!");
			PluginBase currentPlugin = PluginManager.Instance.GetCurrentPlugin();
			if (currentPlugin == null)
			{
				ADebug.LogError("OnApolloPayActionProc plugin is null");
				return;
			}
			ApolloAction apolloAction = new ApolloAction();
			if (!apolloAction.Decode(data))
			{
				ADebug.LogError("OnApolloPayActionProc Action Decode failed");
				return;
			}
			if (this.actionCallbackCollection.ContainsKey(apolloAction.Action))
			{
				ApolloActionBufferBase apolloActionBufferBase = currentPlugin.CreatePayResponseAction(apolloAction.Action);
				if (apolloActionBufferBase != null)
				{
					if (!apolloActionBufferBase.Decode(data))
					{
						ADebug.LogError("OnApolloPayActionProc Decode failed");
						return;
					}
					ApolloActionDelegate apolloActionDelegate = this.actionCallbackCollection[apolloAction.Action];
					if (apolloActionDelegate != null)
					{
						try
						{
							apolloActionDelegate((ApolloResult)ret, apolloActionBufferBase);
						}
						catch (Exception arg)
						{
							ADebug.LogError("OnApolloPayActionProc exception:" + arg);
						}
					}
					else
					{
						ADebug.LogError("OnApolloPayActionProc callback is null while action == " + apolloAction.Action);
					}
				}
				else
				{
					ADebug.LogError("OnApolloPayActionProc info is null");
				}
			}
			else
			{
				ADebug.LogError("OnApolloPayActionProc not exist action:" + apolloAction.Action);
			}
		}

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool apollo_pay_Initialize([MarshalAs(UnmanagedType.LPArray)] byte[] data, int len);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool apollo_pay_Pay([MarshalAs(UnmanagedType.LPArray)] byte[] data, int len);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool apollo_pay_Pay4Mounth([MarshalAs(UnmanagedType.LPArray)] byte[] data, int len);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool apollo_pay_Dipose();

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern void apollo_pay_action([MarshalAs(UnmanagedType.LPArray)] byte[] data, int len);
	}
}
