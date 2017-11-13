using Assets.Scripts.Common;
using Assets.Scripts.GameLogic;
using CSProtocol;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Assets.Scripts.GameSystem
{
	public class GameFowManager : Singleton<GameFowManager>
	{
		[UnmanagedFunctionPointer]
		private delegate bool IsAreaPermanentLitCallBack(int x, int y, COM_PLAYERCAMP camp);

		public const byte FOW_ALPHA_MAX = 255;

		public const byte FOW_ALPHA_UNEXPLORED = 0;

		public const int DEBUG_DRAW_DELTA_Z = 2800;

		private const int HalfSizeShrink = 100;

		private const int ms_actorInfoCnt = 80;

		private const int ms_bulletInfoCnt = 20;

		private uint m_gpuInterpFrameInterval = 8u;

		private float m_gpuInterpReciprocal = 1f;

		public int m_halfSizeX;

		public int m_halfSizeY;

		private byte[] m_commitBitMap;

		private int[] ms_Actor_Info_Data;

		private byte[] ActorInfoDataExtra_;

		private IntPtr m_tempCpyActorDataPtr = IntPtr.Zero;

		private IntPtr ActorInfoDataExtraPtr_ = IntPtr.Zero;

		private int[] ms_Bullet_Info_Data;

		private byte[] BulletInfoDataExtra_;

		private IntPtr m_tempCpyBulletDataPtr = IntPtr.Zero;

		private IntPtr BulletInfoDataExtraPtr_ = IntPtr.Zero;

		public FieldObj m_pFieldObj
		{
			get;
			private set;
		}

		public FowLos m_los
		{
			get;
			private set;
		}

		public GameFowCollector m_collector
		{
			get;
			private set;
		}

		public uint InterpolateFrameInterval
		{
			get
			{
				return 8u;
			}
		}

		public uint InterpolateFrameIntervalHero
		{
			get
			{
				return 2u;
			}
		}

		public uint GPUInterpolateFrameInterval
		{
			get
			{
				return this.m_gpuInterpFrameInterval;
			}
		}

		public float GPUInterpolateReciprocal
		{
			get
			{
				return this.m_gpuInterpReciprocal;
			}
		}

		public uint InterpolateFrameIntervalBullet(bool bMovingBullet)
		{
			return bMovingBullet ? 2u : 4u;
		}

		public override void Init()
		{
			base.Init();
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
			Singleton<EventRouter>.instance.AddEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroLevelChange));
		}

		public override void UnInit()
		{
			base.UnInit();
			Singleton<EventRouter>.instance.RemoveEventHandler<PoolObjHandle<ActorRoot>, int>("HeroSoulLevelChange", new Action<PoolObjHandle<ActorRoot>, int>(this.OnHeroLevelChange));
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.OnActorDead));
		}

		public void SetCallBack()
		{
			GameFowManager.SetIsAreaPermanentLitCallBack(new GameFowManager.IsAreaPermanentLitCallBack(GameFowManager.IsAreaPermanentLit));
		}

		public void InitSurface(bool bDoInit, FieldObj inFieldObj, int inFakeSightRange)
		{
			this.UninitSurface();
			GC.Collect();
			this.m_pFieldObj = inFieldObj;
			this.m_halfSizeX = this.m_pFieldObj.PaneX / 2 - 100;
			this.m_halfSizeY = this.m_pFieldObj.PaneY / 2 - 100;
			DebugHelper.Assert(this.m_halfSizeX > 0);
			DebugHelper.Assert(this.m_halfSizeY > 0);
			if (Application.isPlaying)
			{
				this.SetCallBack();
				GameFowManager.InitGameFowMapData(this.m_pFieldObj.NumX, this.m_pFieldObj.NumY, 0, 255, this.InterpolateFrameInterval, inFakeSightRange, Singleton<WatchController>.instance.HorizonCamp);
				this.m_commitBitMap = new byte[this.m_pFieldObj.NumX * this.m_pFieldObj.NumY];
				this.m_collector = new GameFowCollector();
				this.m_collector.InitSurface();
			}
			this.m_los = new FowLos();
			this.m_los.Init();
			this.m_gpuInterpFrameInterval = (uint)MonoSingleton<GlobalConfig>.instance.GPUInterpolateFrameInterval;
			this.m_gpuInterpReciprocal = 1f / this.m_gpuInterpFrameInterval;
		}

		public void UninitSurface()
		{
			if (this.m_pFieldObj != null)
			{
				this.m_pFieldObj.UninitField();
				this.m_pFieldObj = null;
			}
			if (this.m_los != null)
			{
				this.m_los.Uninit();
				this.m_los = null;
			}
			this.UninitCollectorDataInfo();
			GameFowManager.UninitCollectorInfoDLL();
			GameFowManager.UninitSurfCellsArray();
			GameFowManager.UninitGameFowMapData();
			this.m_commitBitMap = null;
			if (this.m_collector != null)
			{
				this.m_collector.UninitSurface();
				this.m_collector = null;
			}
		}

		public void OnStartFight()
		{
			if (FogOfWar.enable)
			{
				this.ResetBaseMapData(true);
			}
		}

		private void OnHeroLevelChange(PoolObjHandle<ActorRoot> hero, int level)
		{
			if (FogOfWar.enable)
			{
				SkillSlot skillSlot = hero.handle.SkillControl.GetSkillSlot(SkillSlotType.SLOT_SKILL_7);
				if (skillSlot != null)
				{
					skillSlot.SetSkillLevel(level);
				}
			}
		}

		private void OnActorDead(ref GameDeadEventParam prm)
		{
			if (FogOfWar.enable)
			{
				PoolObjHandle<ActorRoot> src = prm.src;
				if (src && src.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ)
				{
					this.ResetBaseMapData(src.handle.TheActorMeta.ActorCamp, false);
				}
			}
		}

		private void UpdateBaseMapData(COM_PLAYERCAMP camp)
		{
			List<PoolObjHandle<ActorRoot>> gameActors = Singleton<GameObjMgr>.GetInstance().GameActors;
			int count = gameActors.get_Count();
			for (int i = 0; i < count; i++)
			{
				PoolObjHandle<ActorRoot> ptr = gameActors.get_Item(i);
				if (ptr && ptr.handle.TheActorMeta.ActorType == ActorTypeDef.Actor_Type_Organ && camp == ptr.handle.TheActorMeta.ActorCamp && !ptr.handle.ActorControl.IsDeadState)
				{
					byte actorSubType = ptr.handle.ActorControl.GetActorSubType();
					if (actorSubType == 4 || actorSubType == 1 || actorSubType == 2)
					{
						this.m_los.ExploreCellsFast(new VInt3(ptr.handle.location.x, ptr.handle.location.z, 0), ptr.handle.HorizonMarker.SightRange, this, this.m_pFieldObj, camp, true, false);
					}
				}
			}
		}

		public void ResetBaseMapData(bool bSubmit)
		{
			for (int i = 1; i < 3; i++)
			{
				this.ResetBaseMapData((COM_PLAYERCAMP)i, bSubmit);
			}
		}

		public void ResetBaseMapData(COM_PLAYERCAMP camp, bool bSubmit)
		{
			if (camp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				return;
			}
			GameFowManager.SyncPermanentToBaseData(camp);
			this.UpdateBaseMapData(camp);
			GameFowManager.SyncBaseDataToVisible(camp);
			this.ForceUpdate(camp, bSubmit);
		}

		public void ResetBaseMapDataAsync(COM_PLAYERCAMP camp)
		{
			if (camp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				return;
			}
			GameFowManager.SyncPermanentToBaseData(camp);
			this.UpdateBaseMapData(camp);
			GameFowManager.SyncBaseDataToVisible(camp);
		}

		public bool IsInsideSurface(int sx, int sy)
		{
			return sx >= 0 && sy >= 0 && sx < this.m_pFieldObj.NumX && sy < this.m_pFieldObj.NumY;
		}

		public FieldObj.EViewBlockType QueryAttr(VInt3 inActorLoc)
		{
			inActorLoc = new VInt3(inActorLoc.x, inActorLoc.z, 0);
			VInt2 zero = VInt2.zero;
			this.WorldPosToGrid(inActorLoc, out zero.x, out zero.y);
			FieldObj.SViewBlockAttr sViewBlockAttr = default(FieldObj.SViewBlockAttr);
			if (this.m_pFieldObj.QueryAttr(zero, out sViewBlockAttr))
			{
				return (FieldObj.EViewBlockType)sViewBlockAttr.BlockType;
			}
			return FieldObj.EViewBlockType.None;
		}

		public bool IsSurfaceCellVisibleConsiderNeighbor(VInt3 worldLoc, COM_PLAYERCAMP camp)
		{
			VInt2 vInt = VInt2.zero;
			this.WorldPosToGrid(worldLoc, out vInt.x, out vInt.y);
			FieldObj.SViewBlockAttr sViewBlockAttr = default(FieldObj.SViewBlockAttr);
			if (this.m_pFieldObj.QueryAttr(vInt, out sViewBlockAttr) && sViewBlockAttr.BlockType == 2)
			{
				VInt2 zero = VInt2.zero;
				if (this.m_pFieldObj.FindNearestGrid(vInt, worldLoc, FieldObj.EViewBlockType.Brick, true, 3, null, out zero))
				{
					vInt = zero;
				}
			}
			DebugHelper.Assert(camp != COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT);
			return this.IsVisible(vInt.x, vInt.y, camp);
		}

		public bool IsVisible(VInt3 worldLoc, COM_PLAYERCAMP camp)
		{
			int y = 0;
			int x;
			this.WorldPosToGrid(worldLoc, out x, out y);
			return this.IsVisible(x, y, camp);
		}

		public bool IsVisible(int x, int y, COM_PLAYERCAMP camp)
		{
			if (camp == COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT)
			{
				return this.IsVisibleInternal(x, y, COM_PLAYERCAMP.COM_PLAYERCAMP_1) || this.IsVisibleInternal(x, y, COM_PLAYERCAMP.COM_PLAYERCAMP_2);
			}
			return this.IsVisibleInternal(x, y, camp);
		}

		private bool IsVisibleInternal(int x, int y, COM_PLAYERCAMP inCamp)
		{
			DebugHelper.Assert(inCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT);
			DebugHelper.Assert(inCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID);
			return inCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID && inCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_COUNT && GameFowManager.IsVisibleDll(x, y, inCamp);
		}

		private void UninitCollectorDataInfo()
		{
			this.ms_Actor_Info_Data = null;
			this.ActorInfoDataExtra_ = null;
			this.ms_Bullet_Info_Data = null;
			this.BulletInfoDataExtra_ = null;
			if (this.m_tempCpyActorDataPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_tempCpyActorDataPtr);
				this.m_tempCpyActorDataPtr = IntPtr.Zero;
			}
			if (this.ActorInfoDataExtraPtr_ != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.ActorInfoDataExtraPtr_);
				this.ActorInfoDataExtraPtr_ = IntPtr.Zero;
			}
			if (this.m_tempCpyBulletDataPtr != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_tempCpyBulletDataPtr);
				this.m_tempCpyBulletDataPtr = IntPtr.Zero;
			}
			if (this.BulletInfoDataExtraPtr_ != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.BulletInfoDataExtraPtr_);
				this.BulletInfoDataExtraPtr_ = IntPtr.Zero;
			}
		}

		private void CopyActorInfoData(List<ACTOR_INFO> actorInfoList)
		{
			int dataSize = ACTOR_INFO.GetDataSize();
			int num = dataSize * 80;
			int dataSizeBytes = ACTOR_INFO.GetDataSizeBytes();
			int num2 = dataSizeBytes * 80;
			if (this.ms_Actor_Info_Data == null)
			{
				this.ms_Actor_Info_Data = new int[num];
			}
			if (this.ActorInfoDataExtra_ == null)
			{
				this.ActorInfoDataExtra_ = new byte[num2];
			}
			int num3 = actorInfoList.get_Count();
			if (num3 > 80)
			{
				num3 = 80;
			}
			for (int i = 0; i < num3; i++)
			{
				int num4 = i * dataSize;
				int num5 = HorizonMarkerByFow.TranslateCampToIndex(COM_PLAYERCAMP.COM_PLAYERCAMP_1);
				this.ms_Actor_Info_Data[num4] = actorInfoList.get_Item(i).location[num5].x;
				this.ms_Actor_Info_Data[num4 + 1] = actorInfoList.get_Item(i).location[num5].y;
				this.ms_Actor_Info_Data[num4 + 4] = actorInfoList.get_Item(i).camps[num5];
				int num6 = HorizonMarkerByFow.TranslateCampToIndex(COM_PLAYERCAMP.COM_PLAYERCAMP_2);
				this.ms_Actor_Info_Data[num4 + 2] = actorInfoList.get_Item(i).location[num6].x;
				this.ms_Actor_Info_Data[num4 + 3] = actorInfoList.get_Item(i).location[num6].y;
				this.ms_Actor_Info_Data[num4 + 5] = actorInfoList.get_Item(i).camps[num6];
				int num7 = i * dataSizeBytes;
				this.ActorInfoDataExtra_[num7] = (actorInfoList.get_Item(i).bDistOnly ? 1 : 0);
			}
			if (IntPtr.Zero == this.m_tempCpyActorDataPtr)
			{
				int num8 = num * 4;
				this.m_tempCpyActorDataPtr = Marshal.AllocHGlobal(num8);
			}
			if (IntPtr.Zero == this.ActorInfoDataExtraPtr_)
			{
				this.ActorInfoDataExtraPtr_ = Marshal.AllocHGlobal(num2);
			}
			if (num3 > 0)
			{
				Marshal.Copy(this.ms_Actor_Info_Data, 0, this.m_tempCpyActorDataPtr, num);
				Marshal.Copy(this.ActorInfoDataExtra_, 0, this.ActorInfoDataExtraPtr_, num2);
			}
			GameFowManager.MemCopyActorInfoData(num3, dataSize, this.m_tempCpyActorDataPtr, dataSizeBytes, this.ActorInfoDataExtraPtr_);
		}

		private void CopyBulletInfoData(List<BULLET_INFO> bulletInfoList)
		{
			int dataSize = BULLET_INFO.GetDataSize();
			int num = dataSize * 20;
			int dataSizeBytes = BULLET_INFO.GetDataSizeBytes();
			int num2 = dataSizeBytes * 20;
			if (this.ms_Bullet_Info_Data == null)
			{
				this.ms_Bullet_Info_Data = new int[num];
			}
			if (this.BulletInfoDataExtra_ == null)
			{
				this.BulletInfoDataExtra_ = new byte[num2];
			}
			int num3 = bulletInfoList.get_Count();
			if (num3 > 20)
			{
				num3 = 20;
			}
			for (int i = 0; i < num3; i++)
			{
				int num4 = i * dataSize;
				this.ms_Bullet_Info_Data[num4] = bulletInfoList.get_Item(i).location.x;
				this.ms_Bullet_Info_Data[num4 + 1] = bulletInfoList.get_Item(i).location.y;
				this.ms_Bullet_Info_Data[num4 + 2] = bulletInfoList.get_Item(i).radius;
				int num5 = i * dataSizeBytes;
				this.BulletInfoDataExtra_[num5] = (bulletInfoList.get_Item(i).bDistOnly ? 1 : 0);
			}
			if (IntPtr.Zero == this.m_tempCpyBulletDataPtr)
			{
				int num6 = num * 4;
				this.m_tempCpyBulletDataPtr = Marshal.AllocHGlobal(num6);
			}
			if (IntPtr.Zero == this.BulletInfoDataExtraPtr_)
			{
				this.BulletInfoDataExtraPtr_ = Marshal.AllocHGlobal(num2);
			}
			if (num3 > 0)
			{
				Marshal.Copy(this.ms_Bullet_Info_Data, 0, this.m_tempCpyBulletDataPtr, num);
				Marshal.Copy(this.BulletInfoDataExtra_, 0, this.BulletInfoDataExtraPtr_, num2);
			}
			GameFowManager.MemCopyBulletInfoData(num3, dataSize, this.m_tempCpyBulletDataPtr, dataSizeBytes, this.BulletInfoDataExtraPtr_);
		}

		private void ComputeFowWithAutoDec(COM_PLAYERCAMP inCamp)
		{
			DebugHelper.Assert(inCamp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID);
			if (inCamp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				return;
			}
			this.CopyBulletInfoData(this.m_collector.GetExplorerBulletList(inCamp));
			GameFowManager.ComputeFowByCampWithAutoDec(inCamp);
		}

		private void ComputeFow(COM_PLAYERCAMP camp)
		{
			DebugHelper.Assert(camp != COM_PLAYERCAMP.COM_PLAYERCAMP_MID);
			if (camp == COM_PLAYERCAMP.COM_PLAYERCAMP_MID)
			{
				return;
			}
			this.CopyBulletInfoData(this.m_collector.GetExplorerBulletList(camp));
			GameFowManager.ComputeFowByCamp(camp);
		}

		private void ForceUpdate(COM_PLAYERCAMP camp, bool bSubmit)
		{
			this.m_collector.CollectExplorer(true);
			this.CopyActorInfoData(this.m_collector.m_explorerPosList);
			this.ComputeFow(camp);
			FogOfWar.CopyBitmap();
			if (bSubmit)
			{
				FogOfWar.CommitToMaterials();
			}
			if (bSubmit)
			{
				this.m_collector.UpdateFowVisibility(true);
			}
		}

		public void ForceUpdate(bool bSubmit)
		{
			this.m_collector.CollectExplorer(true);
			this.CopyActorInfoData(this.m_collector.m_explorerPosList);
			for (int i = 1; i < 3; i++)
			{
				this.ComputeFow((COM_PLAYERCAMP)i);
			}
			FogOfWar.CopyBitmap();
			if (bSubmit)
			{
				FogOfWar.CommitToMaterials();
			}
			if (bSubmit)
			{
				this.m_collector.UpdateFowVisibility(true);
			}
		}

		public byte[] GetCommitPixels()
		{
			IntPtr commitBitmapData = GameFowManager.GetCommitBitmapData();
			Marshal.Copy(commitBitmapData, this.m_commitBitMap, 0, this.m_commitBitMap.Length);
			return this.m_commitBitMap;
		}

		public void UpdateComputing()
		{
			this.CopyActorInfoData(this.m_collector.m_explorerPosList);
			this.m_collector.ClearExplorerPosList();
			for (int i = 1; i < 3; i++)
			{
				COM_PLAYERCAMP inCamp = (COM_PLAYERCAMP)i;
				this.ComputeFowWithAutoDec(inCamp);
			}
			this.m_collector.ClearExplorerBulletList();
		}

		public bool LoadPrecomputeData()
		{
			bool result = false;
			if (Application.isPlaying && this.m_pFieldObj != null)
			{
				this.m_pFieldObj.m_fowCells = null;
				FOGameFowOfflineSerializer fOGameFowOfflineSerializer = new FOGameFowOfflineSerializer();
				if (fOGameFowOfflineSerializer.TryLoad(this.m_pFieldObj))
				{
					result = true;
				}
				this.m_pFieldObj.fowOfflineData = null;
			}
			return result;
		}

		public void WorldPosToGrid(VInt3 inWorldPos, out int outCellX, out int outCellY)
		{
			this.m_pFieldObj.LevelGrid.WorldPosToGrid(inWorldPos, out outCellX, out outCellY);
		}

		public bool WorldPosToGridMayFail(VInt3 inWorldPos, out int outCellX, out int outCellY)
		{
			return this.m_pFieldObj.LevelGrid.WorldPosToGridMayFail(inWorldPos, out outCellX, out outCellY);
		}

		public void GridToWorldPos(int inCellX, int inCellY, out VInt3 outWorldPos)
		{
			this.m_pFieldObj.LevelGrid.GridToWorldPos(inCellX, inCellY, out outWorldPos);
		}

		public void ReviseWorldPosToCenter(VInt3 inWorldPos, out VInt3 outWorldPos)
		{
			this.m_pFieldObj.LevelGrid.ReviseWorldPosToCenter(inWorldPos, out outWorldPos);
		}

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		public static extern void InitGameFowMapData(int w, int h, int inUnexploredAlpha, int maxFowAlpha, uint inInterpolateFrameInterval, int inFakeSightRange, COM_PLAYERCAMP hostCamp);

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ResetHostCamp(COM_PLAYERCAMP hostCamp);

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		public static extern void UninitGameFowMapData();

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetVisible(bool bVisible, int w, int h, COM_PLAYERCAMP camp);

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetBaseDataVisible(bool bVisible, int w, int h, COM_PLAYERCAMP camp);

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool IsVisibleDll(int w, int h, COM_PLAYERCAMP camp);

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		private static extern void SyncBaseDataToVisible(COM_PLAYERCAMP camp);

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		public static extern void SyncPermanentToBaseData(COM_PLAYERCAMP camp);

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr GetCommitBitmapData();

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		private static extern void SetIsAreaPermanentLitCallBack([MarshalAs(38)] GameFowManager.IsAreaPermanentLitCallBack callback);

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		public static extern void InitSurfCellsArray(int cellCnt);

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		public static extern void UninitSurfCellsArray();

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		public static extern void InitSurfCell(int index, int xMin, int xMax, int yMin, int yMax, bool bValid);

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		public static extern void InitLevelGrid(int cellCnt, int iCellNumX, int iCellNumY, int iCellSizeX, int iCellSizeY, int iGridPosX, int iGridPosY);

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		public static extern void InitLevelGridCell(int index, int inBlockType, int inLightType);

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetSurfCellData(int index, IntPtr data);

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		public static extern void SetSurfCellVisible(int index, int x, int y, COM_PLAYERCAMP camp, bool bVisible);

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		public static extern void MemCopyActorInfoData(int actorCnt, int dataSize, IntPtr actorInfoData, int dataSizeExtra, IntPtr actorInfoDataExtra);

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		public static extern void MemCopyBulletInfoData(int bulletCnt, int dataSize, IntPtr bulletInfoData, int dataSizeExtra, IntPtr bulletInfoDataExtra);

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ComputeFowByCamp(COM_PLAYERCAMP camp);

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ComputeFowByCampWithAutoDec(COM_PLAYERCAMP camp);

		[DllImport("SGameFowProject", CallingConvention = CallingConvention.Cdecl)]
		public static extern void UninitCollectorInfoDLL();

		public static bool IsAreaPermanentLit(int x, int y, COM_PLAYERCAMP camp)
		{
			return Singleton<GameFowManager>.instance.m_pFieldObj.IsAreaPermanentLit(x, y, camp);
		}
	}
}
