using System;
using UnityEngine;

internal class GlobalConfig : MonoSingleton<GlobalConfig>
{
	public int WaypointIgnoreDist = 99999;

	[FriendlyName("刷单个怪之间的间隔")]
	public int SoldierWaveInterval = 1000;

	[FriendlyName("摇杆最大移动距离")]
	public int JoysticMaxExtendDist = 200;

	[FriendlyName("摇杆初始位置xy偏移")]
	public Vector2 JoysticRootPos = new Vector2(240f, 240f);

	[FriendlyName("拾取范围")]
	public int PickupRange = 3000;

	[FriendlyName("掉落物飞翔时间")]
	public int DropItemFlyTime = 1200;

	[FriendlyName("掉落物飞起高度")]
	public int DropItemFlyHeight = 10000;

	[FriendlyName("机关掉落概率")]
	public int OrganDropItemProbability = 50;

	[FriendlyName("普通怪掉落概率")]
	public int NormalMonsterDropItemProbability = 10;

	[FriendlyName("宝箱怪掉落概率")]
	public int ChestMonsterDropItemProbability = 75;

	[FriendlyName("是否开启游戏内控制台")]
	public bool bEnableInGameConsole;

	[FriendlyName("是否开启特效裁剪优化")]
	public bool bEnableParticleCullOptimize = true;

	[FriendlyName("PVE真实时间Tick")]
	public bool bEnableRealTimeTickInPVE;

	[FriendlyName("模拟丢包")]
	public bool bSimulateLosePackage;

	[FriendlyName("人为操作模拟")]
	public bool bSimulateHumanOperation;

	[HideInInspector]
	public bool bOnExternalSpeedPicker;

	[FriendlyName("右摇杆移动速度")]
	public float CameraMoveSpeed = 10000f;

	[FriendlyName("右摇杆移动加速度")]
	public float CameraMoveAcceleration = 10000f;

	[FriendlyName("右摇杆最大速度")]
	public float CameraMoveSpeedMax = 30000f;

	[FriendlyName("右摇杆遇零重置速度")]
	public bool bResetCameraSpeedWhenZero;

	[FriendlyName("右摇杆控制是否合并玩家移动")]
	public bool bComposePlayerMovement = true;

	[FriendlyName("右Panel移动速度")]
	public float PanelCameraMoveSpeed = 50f;

	[HideInInspector]
	public int UnityMainThreadID;

	[FriendlyName("机关反隐效果间隔")]
	public int DefenseAntiHiddenInterval = 400;

	[FriendlyName("机关反隐分帧数")]
	public int DefenseAntiHiddenFrameInterval = 4;

	[FriendlyName("机关反隐伤害ID")]
	public int DefenseAntiHiddenHurtId = 1000004;

	[FriendlyName("迷雾渲染插值帧数")]
	public int GPUInterpolateFrameInterval = 6;

	[FriendlyName("草丛附近插眼吸附距离")]
	public int GrassEyeAbsorbDist = 300;

	[FriendlyName("主宰先锋一波重复总次数")]
	public int DominateVanguardRepeatTotal = 3;
}
