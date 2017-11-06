using Assets.Scripts.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
	[Serializable]
	public class CActorInfo : ScriptableObject
	{
		public Texture2D PortraitSprite;

		public string ActorName;

		public string Instruction;

		public string BgStory;

		public string[] ArtPrefabLOD = new string[3];

		public string[] ArtLobbyShowLOD = new string[2];

		public string[] SoundBanks = new string[0];

		public SkinElement[] SkinPrefab = new SkinElement[0];

		public AnimaSoundElement[] AnimaSound = new AnimaSoundElement[0];

		public string[] LobbySoundBanks = new string[0];

		public TransformConfig[] TransConfigs = new TransformConfig[2];

		public int ReviveTime;

		public int DyingDialogGroupId;

		public SkillElement[] MySkills = new SkillElement[0];

		public GameObject hudPrefab;

		public int hudHeight;

		public HudCompType HudType;

		public int MaxSpeed;

		public int Acceleration;

		public int RotateSpeed;

		public int DecelerateDistance;

		public int MinDecelerateSpeed;

		public int IgnoreDistance;

		public float LobbyScale = 1f;

		public CollisionShapeType collisionType;

		public VInt3 iCollisionCenter = VInt3.zero;

		public VInt3 iCollisionSize = new VInt3(400, 400, 400);

		public int iBulletHeight = 200;

		public int iAttackLineHeight = 4500;

		public string BtResourcePath;

		public string deadAgePath;

		public ActSound[] ActSounds = new ActSound[0];

		public int callMonsterConfigID;

		public static CActorInfo GetActorInfo(string path, enResourceType resourceType)
		{
			CResource resource = Singleton<CResourceManager>.GetInstance().GetResource(path, typeof(CActorInfo), resourceType, true, false);
			if (resource == null)
			{
				return null;
			}
			return resource.m_content as CActorInfo;
		}

		public TransformConfig GetTransformConfig(ETransformConfigUsage InUsage)
		{
			DebugHelper.Assert(this.TransConfigs != null && InUsage >= ETransformConfigUsage.NPCInStory && InUsage < (ETransformConfigUsage)this.TransConfigs.Length);
			return this.TransConfigs[(int)InUsage];
		}

		public bool HasTransformConfig(ETransformConfigUsage InUsage)
		{
			return this.TransConfigs != null && InUsage < (ETransformConfigUsage)this.TransConfigs.Length;
		}

		public TransformConfig GetTransformConfigIfHaveOne(ETransformConfigUsage InUsage)
		{
			return this.HasTransformConfig(InUsage) ? this.TransConfigs[(int)InUsage] : null;
		}

		public string GetArtPrefabName(int skinId = 0, int InLOD = -1)
		{
			int num;
			if (InLOD < 0 || InLOD > 2)
			{
				num = GameSettings.ModelLOD;
			}
			else
			{
				num = InLOD;
			}
			if (Singleton<BattleLogic>.GetInstance().GetCurLvelContext() != null && !Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsMobaMode())
			{
				num--;
			}
			num = Mathf.Clamp(num, 0, 2);
			if (skinId >= 1 && skinId <= this.SkinPrefab.Length)
			{
				return this.SkinPrefab[skinId - 1].ArtSkinPrefabLOD[num];
			}
			return this.ArtPrefabLOD[num];
		}

		public string GetArtPrefabNameLobby(int skinId = 0)
		{
			int num = GameSettings.ModelLOD;
			if (num == 2)
			{
			}
			num = 0;
			if (skinId >= 1 && skinId <= this.SkinPrefab.Length)
			{
				return this.SkinPrefab[skinId - 1].ArtSkinLobbyShowLOD[num];
			}
			return this.ArtLobbyShowLOD[num];
		}

		public bool GetAdvanceSkinPrefabName(out string prefabPath, uint skinId, int level, int inLOD = -1)
		{
			prefabPath = string.Empty;
			int num;
			if (inLOD < 0 || inLOD > 2)
			{
				num = GameSettings.ModelLOD;
			}
			else
			{
				num = inLOD;
			}
			if (Singleton<BattleLogic>.GetInstance().GetCurLvelContext() != null && !Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsMobaMode())
			{
				num--;
			}
			num = Mathf.Clamp(num, 0, 2);
			if (skinId >= 1u && (ulong)skinId <= (ulong)((long)this.SkinPrefab.Length))
			{
				SkinElement skinElement = this.SkinPrefab[(int)((uint)((UIntPtr)(skinId - 1u)))];
				if (skinElement != null)
				{
					for (int i = 0; i < skinElement.AdvanceSkin.Length; i++)
					{
						if (skinElement.AdvanceSkin[i] != null && skinElement.AdvanceSkin[i].Level == level)
						{
							prefabPath = skinElement.AdvanceSkin[i].ArtSkinPrefabLOD[num];
							return true;
						}
					}
				}
			}
			return false;
		}

		public void PreLoadAdvanceSkin(List<AssetLoadBase> mesPrefabs, uint skinId, int inLOD = -1)
		{
			if (mesPrefabs == null)
			{
				return;
			}
			int num;
			if (inLOD < 0 || inLOD > 2)
			{
				num = GameSettings.ModelLOD;
			}
			else
			{
				num = inLOD;
			}
			if (Singleton<BattleLogic>.GetInstance().GetCurLvelContext() != null && !Singleton<BattleLogic>.GetInstance().GetCurLvelContext().IsMobaMode())
			{
				num--;
			}
			num = Mathf.Clamp(num, 0, 2);
			if (skinId >= 1u && (ulong)skinId <= (ulong)((long)this.SkinPrefab.Length))
			{
				SkinElement skinElement = this.SkinPrefab[(int)((uint)((UIntPtr)(skinId - 1u)))];
				if (skinElement != null)
				{
					for (int i = 0; i < skinElement.AdvanceSkin.Length; i++)
					{
						if (skinElement.AdvanceSkin[i] != null && !string.IsNullOrEmpty(skinElement.AdvanceSkin[i].ArtSkinPrefabLOD[num]))
						{
							mesPrefabs.Add(new AssetLoadBase
							{
								assetPath = skinElement.AdvanceSkin[i].ArtSkinPrefabLOD[num]
							});
						}
					}
				}
			}
		}

		public int GetAdvanceSkinIndexByLevel(uint skinId, int level)
		{
			int result = 0;
			if (skinId >= 1u && (ulong)skinId <= (ulong)((long)this.SkinPrefab.Length))
			{
				SkinElement skinElement = this.SkinPrefab[(int)((uint)((UIntPtr)(skinId - 1u)))];
				if (skinElement != null)
				{
					for (int i = 0; i < skinElement.AdvanceSkin.Length; i++)
					{
						if (skinElement.AdvanceSkin[i] != null)
						{
							if (level < skinElement.AdvanceSkin[i].Level)
							{
								return i;
							}
							result = i + 1;
						}
					}
				}
			}
			return result;
		}

		public VCollisionShape CreateCollisionShape()
		{
			DebugHelper.Assert(!Singleton<BattleLogic>.instance.isFighting || Singleton<GameLogic>.instance.bInLogicTick || Singleton<FrameSynchr>.instance.isCmdExecuting);
			if (this.collisionType == CollisionShapeType.Box)
			{
				return new VCollisionBox
				{
					Pos = this.iCollisionCenter,
					Size = this.iCollisionSize
				};
			}
			if (this.collisionType == CollisionShapeType.Sphere)
			{
				return new VCollisionSphere
				{
					Pos = this.iCollisionCenter,
					Radius = this.iCollisionSize.x
				};
			}
			DebugHelper.Assert(false, "初始化碰撞体类型错误");
			return null;
		}
	}
}
