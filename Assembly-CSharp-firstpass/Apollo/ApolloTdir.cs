using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Apollo
{
	public class ApolloTdir : ApolloObject, IApolloTdir
	{
		private List<TdirTreeNode> m_treenodes;

		private DictionaryView<int, List<TdirUserRoleInfo>> m_treeRoleInfoDic;

		private TreeCommonData m_treeCommomData;

		private int m_appID;

		private string[] m_ipList;

		private int[] m_portList;

		private string m_lastSuccessIP;

		private int m_lastSuccessPort;

		private string m_openID;

		private bool mLog;

		private TdirResult mErrorCode;

		private string mErrorString;

		private TdirServiceTable mServiceTable;

		private bool isServiceTableEnable;

		public ApolloTdir()
		{
			this.mLog = false;
			this.isServiceTableEnable = false;
			this.mErrorCode = TdirResult.TdirNoError;
			this.mErrorString = "no error";
			this.m_treeRoleInfoDic = new DictionaryView<int, List<TdirUserRoleInfo>>();
		}

		public void TdirLog(string msg)
		{
			if (this.mLog)
			{
				Debug.Log(msg);
			}
		}

		public TdirResult EnableLog()
		{
			this.mLog = true;
			return ApolloTdir.tcls_enable_log(base.ObjectId);
		}

		public TdirResult DisableLog()
		{
			this.mLog = false;
			return ApolloTdir.tcls_disable_log(base.ObjectId);
		}

		public TdirResult GetErrorCode()
		{
			if (this.mErrorCode == TdirResult.TdirNoError)
			{
				return ApolloTdir.tcls_get_errCode(base.ObjectId);
			}
			return this.mErrorCode;
		}

		public string GetErrorString()
		{
			if ("no error" == this.mErrorString)
			{
				IntPtr intPtr = ApolloTdir.tcls_get_errString(base.ObjectId);
				return Marshal.PtrToStringAnsi(intPtr);
			}
			return this.mErrorString;
		}

		public TdirResult Query(int appID, string[] ipList, int[] portList, string lastSuccessIP, int lastSuccessPort, string openID, bool isOnlyTACC)
		{
			if (ipList == null || ipList.Length == 0)
			{
				this.mErrorCode = TdirResult.ParamError;
				this.mErrorString = "the input IP list is null or empty";
				this.TdirLog("the input IP list is null or empty");
				return TdirResult.ParamError;
			}
			if (portList == null || portList.Length == 0)
			{
				this.mErrorCode = TdirResult.ParamError;
				this.mErrorString = "the input port list is null or empty";
				this.TdirLog("the input port list is null or empty");
				return TdirResult.ParamError;
			}
			this.mErrorCode = TdirResult.TdirNoError;
			this.mErrorString = "no error";
			this.isServiceTableEnable = false;
			this.m_appID = appID;
			this.m_ipList = ipList;
			this.m_portList = portList;
			this.m_lastSuccessIP = lastSuccessIP;
			this.m_lastSuccessPort = lastSuccessPort;
			this.m_openID = openID;
			this.m_treenodes = new List<TdirTreeNode>();
			this.m_treeRoleInfoDic.Clear();
			this.m_treeCommomData.ispCode = -1;
			this.m_treeCommomData.provinceCode = -1;
			string ipList2 = string.Join("|", ipList);
			string portList2 = string.Join("|", this.ToStringArray(portList));
			this.Dump();
			return ApolloTdir.tcls_init(base.ObjectId, appID, ipList2, portList2, lastSuccessIP, Convert.ToString(lastSuccessPort), openID, isOnlyTACC);
		}

		public TdirResult QueryISP(int appID, string[] ipList, int[] portList, string lastSuccessIP = "", int lastSuccessPort = 0, string openID = "")
		{
			if (ipList == null || ipList.Length == 0)
			{
				this.mErrorCode = TdirResult.ParamError;
				this.mErrorString = "the input IP list is null or empty";
				this.TdirLog("the input IP list is null or empty");
				return TdirResult.ParamError;
			}
			if (portList == null || portList.Length == 0)
			{
				this.mErrorCode = TdirResult.ParamError;
				this.mErrorString = "the input port list is null or empty";
				this.TdirLog("the input port list is null or empty");
				return TdirResult.ParamError;
			}
			this.mErrorCode = TdirResult.TdirNoError;
			this.mErrorString = "no error";
			this.isServiceTableEnable = false;
			this.m_appID = appID;
			this.m_ipList = ipList;
			this.m_portList = portList;
			this.m_lastSuccessIP = lastSuccessIP;
			this.m_lastSuccessPort = lastSuccessPort;
			this.m_openID = openID;
			this.m_treenodes = new List<TdirTreeNode>();
			this.m_treeRoleInfoDic.Clear();
			this.m_treeCommomData.ispCode = -1;
			this.m_treeCommomData.provinceCode = -1;
			string ipList2 = string.Join("|", ipList);
			string portList2 = string.Join("|", this.ToStringArray(portList));
			this.Dump();
			this.TdirLog("QueryISP");
			return ApolloTdir.tcls_init_query_isp(base.ObjectId, appID, ipList2, portList2, lastSuccessIP, Convert.ToString(lastSuccessPort), openID);
		}

		private string[] ToStringArray(int[] portList)
		{
			string[] array = new string[portList.Length];
			for (int i = 0; i < portList.Length; i++)
			{
				array[i] = portList[i].ToString();
			}
			return array;
		}

		private void Dump()
		{
			Console.WriteLine("AppID:" + this.m_appID);
			Console.WriteLine("ip: " + string.Join("|", this.m_ipList));
			Console.WriteLine("port: " + string.Join("|", this.ToStringArray(this.m_portList)));
			Console.WriteLine("lastSuccessIP: " + this.m_lastSuccessIP);
			Console.WriteLine("lastSuccessPort: " + this.m_lastSuccessPort);
			Console.WriteLine("openID: " + this.m_openID);
		}

		public TdirResult Status()
		{
			return ApolloTdir.tcls_status(base.ObjectId);
		}

		public TdirResult Recv(int timeout)
		{
			if (this.Status() == TdirResult.WaitForQuery)
			{
				return ApolloTdir.tcls_query(base.ObjectId, timeout);
			}
			if (this.Status() == TdirResult.WaitForRecv)
			{
				return ApolloTdir.tcls_recv(base.ObjectId, timeout);
			}
			return this.Status();
		}

		public TdirResult SetSvrTimeout(int timeout)
		{
			return ApolloTdir.tcls_set_svr_timeout(base.ObjectId, timeout);
		}

		public TdirResult GetTreeNodes(ref List<TdirTreeNode> nodeList)
		{
			if (ApolloTdir.tcls_status(base.ObjectId) == TdirResult.RecvDone)
			{
				nodeList = this.m_treenodes;
				return TdirResult.TdirNoError;
			}
			return TdirResult.NeedRecvDoneStatus;
		}

		public TdirResult GetServiceTable(ref TdirServiceTable table)
		{
			if (ApolloTdir.tcls_status(base.ObjectId) != TdirResult.RecvDone)
			{
				return TdirResult.NeedRecvDoneStatus;
			}
			if (this.isServiceTableEnable)
			{
				table = this.mServiceTable;
				return TdirResult.TdirNoError;
			}
			return TdirResult.NoServiceTable;
		}

		public TreeCommonData GetTreeCommonData()
		{
			return this.m_treeCommomData;
		}

		public TdirResult SetProtocolVersion(int version)
		{
			return ApolloTdir.tcls_set_protocol_version(base.ObjectId, version);
		}

		private void RecvNode(IntPtr ptr)
		{
			SubTdirTreeNode subTdirTreeNode = (SubTdirTreeNode)Marshal.PtrToStructure(ptr, typeof(SubTdirTreeNode));
			TdirTreeNode tdirTreeNode = default(TdirTreeNode);
			tdirTreeNode.userRoleInfo = new List<TdirUserRoleInfo>();
			tdirTreeNode.nodeID = subTdirTreeNode.nodeID;
			tdirTreeNode.parentID = subTdirTreeNode.parentID;
			tdirTreeNode.flag = subTdirTreeNode.flag;
			tdirTreeNode.name = subTdirTreeNode.name;
			tdirTreeNode.status = subTdirTreeNode.status;
			tdirTreeNode.nodeType = subTdirTreeNode.nodeType;
			tdirTreeNode.svrFlag = subTdirTreeNode.svrFlag;
			tdirTreeNode.staticInfo = subTdirTreeNode.staticInfo;
			tdirTreeNode.dynamicInfo = subTdirTreeNode.dynamicInfo;
			if (this.m_treeRoleInfoDic.ContainsKey(tdirTreeNode.nodeID))
			{
				tdirTreeNode.userRoleInfo = this.m_treeRoleInfoDic[tdirTreeNode.nodeID];
				this.TdirLog(string.Format("the size of node ID[{0}]'s user role info is [{1}]", Convert.ToString(tdirTreeNode.nodeID), tdirTreeNode.userRoleInfo.get_Count()));
			}
			else
			{
				this.TdirLog(string.Format("node ID[{0}]'s user role info is empty", Convert.ToString(tdirTreeNode.nodeID)));
			}
			this.m_treenodes.Add(tdirTreeNode);
		}

		private void RecvTreeCommomData(IntPtr ptr)
		{
			this.m_treeCommomData = (TreeCommonData)Marshal.PtrToStructure(ptr, typeof(TreeCommonData));
		}

		private void RecvRoleInfo(byte[] data)
		{
			CTdirUserRoleInfo cTdirUserRoleInfo = new CTdirUserRoleInfo();
			cTdirUserRoleInfo.appBuff = new byte[256];
			cTdirUserRoleInfo.Decode(data);
			TdirUserRoleInfo tdirUserRoleInfo = default(TdirUserRoleInfo);
			tdirUserRoleInfo.zoneID = cTdirUserRoleInfo.zoneID;
			tdirUserRoleInfo.roleID = cTdirUserRoleInfo.roleID;
			tdirUserRoleInfo.lastLoginTime = cTdirUserRoleInfo.lastLoginTime;
			tdirUserRoleInfo.roleName = cTdirUserRoleInfo.roleName;
			tdirUserRoleInfo.roleLevel = cTdirUserRoleInfo.roleLevel;
			tdirUserRoleInfo.appLen = cTdirUserRoleInfo.appLen;
			if (0u < cTdirUserRoleInfo.appLen)
			{
				tdirUserRoleInfo.appBuff = new byte[tdirUserRoleInfo.appLen];
				int num = 0;
				while ((long)num < (long)((ulong)cTdirUserRoleInfo.appLen))
				{
					tdirUserRoleInfo.appBuff[num] = cTdirUserRoleInfo.appBuff[num];
					num++;
				}
			}
			if (this.m_treeRoleInfoDic.ContainsKey(tdirUserRoleInfo.zoneID))
			{
				this.m_treeRoleInfoDic[tdirUserRoleInfo.zoneID].Add(tdirUserRoleInfo);
			}
			else
			{
				List<TdirUserRoleInfo> list = new List<TdirUserRoleInfo>();
				list.Add(tdirUserRoleInfo);
				this.m_treeRoleInfoDic.Add(tdirUserRoleInfo.zoneID, list);
			}
		}

		private void RecvServiceTable(byte[] data)
		{
			CTdirServiceTable cTdirServiceTable = new CTdirServiceTable();
			cTdirServiceTable.appBuff = new byte[512];
			cTdirServiceTable.Decode(data);
			this.mServiceTable.updateTime = cTdirServiceTable.updateTime;
			this.mServiceTable.bitMap = cTdirServiceTable.bitMap;
			this.mServiceTable.userAttr = cTdirServiceTable.userAttr;
			this.mServiceTable.zoneID = cTdirServiceTable.zoneID;
			this.mServiceTable.appLen = cTdirServiceTable.appLen;
			if (0u < cTdirServiceTable.appLen)
			{
				this.mServiceTable.appBuff = new byte[cTdirServiceTable.appLen];
				int num = 0;
				while ((long)num < (long)((ulong)cTdirServiceTable.appLen))
				{
					this.mServiceTable.appBuff[num] = cTdirServiceTable.appBuff[num];
					num++;
				}
			}
			this.isServiceTableEnable = true;
		}

		private void Log(string msg)
		{
			this.TdirLog(msg);
		}

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern TdirResult tcls_init(ulong objId, int appID, [MarshalAs(20)] string ipList, [MarshalAs(20)] string portList, [MarshalAs(20)] string lastSuccessIP, [MarshalAs(20)] string lastSuccessPort, [MarshalAs(20)] string openID, bool isOnlyTACC);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern TdirResult tcls_init_query_isp(ulong objId, int appID, [MarshalAs(20)] string ipList, [MarshalAs(20)] string portList, [MarshalAs(20)] string lastSuccessIP, [MarshalAs(20)] string lastSuccessPort, [MarshalAs(20)] string openID);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern TdirResult tcls_query(ulong objId, int timeout);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern TdirResult tcls_recv(ulong objId, int timeout);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern TdirResult tcls_status(ulong objId);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern TdirResult tcls_disable_log(ulong objId);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern TdirResult tcls_enable_log(ulong objId);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern TdirResult tcls_set_svr_timeout(ulong objId, int timeout);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern TdirResult tcls_get_errCode(ulong objId);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr tcls_get_errString(ulong objId);

		[DllImport("apollo", CallingConvention = CallingConvention.Cdecl)]
		private static extern TdirResult tcls_set_protocol_version(ulong objId, int version);
	}
}
