using AGE;
using Assets.Scripts.Common;
using Assets.Scripts.Framework;
using Assets.Scripts.GameLogic.GameKernal;
using Assets.Scripts.GameSystem;
using Assets.Scripts.UI;
using ResData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic
{
	public class DialogueProcessor : MonoSingleton<DialogueProcessor>
	{
		public struct SActorLineNode
		{
			public ObjData PortraitImgPrefab;

			public ObjNameData PortraitImgPath;

			public bool b3dPortrait;

			public int CharCfgId;

			public ActorTypeDef ActorType;

			public string AnimName;

			public bool bAnimLoop;

			public string DialogTitle;

			public string DialogContent;

			public int DialogStyle;

			public byte DialogPos;

			public string VoiceEvent;

			public bool bFadeIn;

			public bool bFadeOut;
		}

		private const string m_contentGoName = "Txt_Dialog";

		private const string m_nameGoName = "CharacterName";

		private const string m_imgGoName = "Pic_Npc";

		private const string m_3dPortraitAnimDefault = "Idleshow";

		private const float FadeInTime = 0.5f;

		private const float FadeOutTime = 0.5f;

		private const float NextPageDelay = 0.2f;

		private const float AlphaMax = 1f;

		private const float AlphaMin = 0.2f;

		private const string m_3dPortraitPanelName = "PanelCenter";

		private const string m_3dPortraitRawImgName = "3DImage";

		private DialogueProcessor.SActorLineNode[] m_actorLines;

		private HashSet<object> m_actorLinesRaw;

		private string m_ageActName;

		private PoolObjHandle<AGE.Action> m_curAction = default(PoolObjHandle<AGE.Action>);

		private int m_curIndex = -1;

		private GameObject m_3dPortraitName;

		private DictionaryView<int, CUIFormScript> m_uiFormMap = new DictionaryView<int, CUIFormScript>();

		private CUIFormScript m_curUiForm;

		private CUIFormScript m_curUiFormPortrait;

		private DictionaryView<GameObject, Coroutine> Portrait3dAnimCoMap = new DictionaryView<GameObject, Coroutine>();

		private ListView<GameObject> m_bgGoList = new ListView<GameObject>();

		private Coroutine m_fadingCoroutine;

		public bool bAutoNextPage;

		[NonSerialized]
		public float AutoNextPageTime = 3f;

		private float m_nextPageProgressTime;

		private bool m_bIsPlayingAutoNext;

		private int PreDialogId;

		private static string[] BlackDialogBgPaths = new string[]
		{
			"ClickFg",
			"Txt_Dialog",
			"CharacterName"
		};

		private static string[] ImageDialogBgPaths = new string[]
		{
			"Bg_Down",
			"Bg_Down/ImageLine",
			"Bg_Down/ImageArrow",
			"Txt_Dialog",
			"CharacterName"
		};

		public bool PrepareFight()
		{
			return false;
		}

		private void TranslateNodeFromRaw(ref DialogueProcessor.SActorLineNode outNode, ref ResActorLinesInfo inRecord)
		{
			outNode.DialogStyle = inRecord.iDialogStyle;
			string text = Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().Name;
			if (string.IsNullOrEmpty(text))
			{
				text = "Unknown";
			}
			outNode.DialogContent = StringHelper.UTF8BytesToString(ref inRecord.szDialogContent);
			if (string.IsNullOrEmpty(outNode.DialogContent))
			{
				outNode.DialogContent = string.Empty;
			}
			else
			{
				outNode.DialogContent = outNode.DialogContent.Replace("[c]", text);
			}
			outNode.DialogTitle = StringHelper.UTF8BytesToString(ref inRecord.szDialogTitle);
			if (string.IsNullOrEmpty(outNode.DialogTitle))
			{
				outNode.DialogTitle = string.Empty;
			}
			else
			{
				outNode.DialogTitle = outNode.DialogTitle.Replace("[c]", text);
			}
			outNode.bFadeIn = (inRecord.bFadeInType > 0);
			outNode.bFadeOut = (inRecord.bFadeOutType > 0);
			outNode.b3dPortrait = (inRecord.iUse3dPortrait > 0);
			outNode.ActorType = (ActorTypeDef)inRecord.iActorType;
			outNode.CharCfgId = inRecord.iCharacterCfgId;
			outNode.AnimName = StringHelper.UTF8BytesToString(ref inRecord.szAnimName);
			outNode.bAnimLoop = (inRecord.iAnimLoop > 0);
			outNode.PortraitImgPrefab = default(ObjData);
			if (outNode.b3dPortrait)
			{
				if (inRecord.iCharacterCfgId > 0)
				{
					switch (inRecord.iActorType)
					{
					case 0:
						outNode.PortraitImgPath = CUICommonSystem.GetHero3DObjPath((uint)inRecord.iCharacterCfgId, true);
						break;
					case 1:
						outNode.PortraitImgPath = CUICommonSystem.GetMonster3DObjPath(inRecord.iCharacterCfgId, true);
						break;
					case 2:
						outNode.PortraitImgPath = CUICommonSystem.GetOrgan3DObjPath(inRecord.iCharacterCfgId, true);
						break;
					}
				}
			}
			else
			{
				string text2 = StringHelper.UTF8BytesToString(ref inRecord.szImagePath);
				if (text2 == "9999")
				{
					text2 = "90" + (Singleton<GamePlayerCenter>.GetInstance().GetHostPlayer().HeadIconId + 1);
					text2 = CUIUtility.s_Sprite_Dynamic_Dialog_Dir_Portrait + text2;
					outNode.PortraitImgPrefab.Object = (Singleton<CResourceManager>.GetInstance().GetResource(text2, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject);
				}
				else
				{
					text2 = CUIUtility.s_Sprite_Dynamic_Dialog_Dir_Portrait + text2;
					outNode.PortraitImgPrefab.Object = (Singleton<CResourceManager>.GetInstance().GetResource(text2, typeof(GameObject), enResourceType.UIPrefab, false, false).m_content as GameObject);
				}
			}
		}

		public void PlayDrama(int inGroupId, GameObject inSrc, GameObject inAtker, bool bDialogTriggerStart = false)
		{
			this.EndDialogue();
			this.m_actorLinesRaw = GameDataMgr.actorLinesDatabin.GetDataByKey(inGroupId);
			if (this.m_actorLinesRaw == null)
			{
				return;
			}
			ResActorLinesInfo dataByKeySingle = GameDataMgr.actorLinesDatabin.GetDataByKeySingle((uint)inGroupId);
			if (dataByKeySingle == null)
			{
				return;
			}
			if (bDialogTriggerStart)
			{
				this.PreDialogId = inGroupId;
			}
			this.m_ageActName = StringHelper.UTF8BytesToString(ref dataByKeySingle.szAgeActionName);
			bool flag = this.m_ageActName != null && this.m_ageActName.Length > 0;
			if (flag)
			{
				this.PlayAgeActionInternal(inSrc, inAtker, inGroupId);
			}
			else
			{
				this.StartDialogue(inGroupId);
			}
		}

		public void StartDialogue(int inGroupId)
		{
			string ageActName = this.m_ageActName;
			this.EndDialogue();
			this.m_ageActName = ageActName;
			this.m_actorLinesRaw = GameDataMgr.actorLinesDatabin.GetDataByKey(inGroupId);
			if (this.m_actorLinesRaw == null)
			{
				return;
			}
			ResActorLinesInfo dataByKeySingle = GameDataMgr.actorLinesDatabin.GetDataByKeySingle((uint)inGroupId);
			if (dataByKeySingle == null)
			{
				return;
			}
			if (CSysDynamicBlock.bDialogBlock && dataByKeySingle.bIOSHide != 0)
			{
				return;
			}
			this.m_actorLines = new DialogueProcessor.SActorLineNode[this.m_actorLinesRaw.Count];
			DialogueProcessor.SActorLineNode sActorLineNode = default(DialogueProcessor.SActorLineNode);
			this.TranslateNodeFromRaw(ref sActorLineNode, ref dataByKeySingle);
			this.m_actorLines[0] = sActorLineNode;
			HashSet<object>.Enumerator enumerator = this.m_actorLinesRaw.GetEnumerator();
			if (enumerator.MoveNext())
			{
				int num = 1;
				while (enumerator.MoveNext())
				{
					ResActorLinesInfo resActorLinesInfo = enumerator.Current as ResActorLinesInfo;
					DialogueProcessor.SActorLineNode sActorLineNode2 = default(DialogueProcessor.SActorLineNode);
					this.TranslateNodeFromRaw(ref sActorLineNode2, ref resActorLinesInfo);
					this.m_actorLines[num] = sActorLineNode2;
					sActorLineNode = sActorLineNode2;
					num++;
				}
			}
			Singleton<CBattleGuideManager>.GetInstance().PauseGame(this, true);
			this.m_curIndex = -1;
			if (this.bAutoNextPage)
			{
				base.StartCoroutine(this.NextPageNoTimeScale());
			}
			else
			{
				this.NextPageInternal();
			}
			if (this.PreDialogId != 0 && inGroupId == this.PreDialogId)
			{
				PreDialogStartedEventParam preDialogStartedEventParam = new PreDialogStartedEventParam(this.PreDialogId);
				Singleton<GameEventSys>.instance.SendEvent<PreDialogStartedEventParam>(GameEventDef.Event_PreDialogStarted, ref preDialogStartedEventParam);
				this.PreDialogId = 0;
			}
		}

		public bool IsInDialog()
		{
			return this.m_actorLines != null;
		}
		

		private void OnDestroyPortrait(GameObject inPrefabObj)
		{
			if (inPrefabObj == null)
			{
				return;
			}
			if (this.Portrait3dAnimCoMap.ContainsKey(inPrefabObj))
			{
				base.StopCoroutine(this.Portrait3dAnimCoMap[inPrefabObj]);
				this.Portrait3dAnimCoMap.Remove(inPrefabObj);
			}
		}

		private void EndDialogue()
		{
			if (this.m_actorLines != null)
			{
				this.m_actorLines = null;
			}
			if (this.m_curUiFormPortrait != null)
			{
				Transform transform = this.m_curUiFormPortrait.gameObject.transform.FindChild("PanelCenter");
				if (transform != null)
				{
					Transform transform2 = transform.FindChild("3DImage");
					if (transform2 != null)
					{
						CUI3DImageScript component = transform2.gameObject.GetComponent<CUI3DImageScript>();
						component.RemoveGameObject(this.m_3dPortraitName);
					}
				}
			}
			this.OnDestroyPortrait(this.m_3dPortraitName);
			this.m_3dPortraitName = null;
			this.Portrait3dAnimCoMap.Clear();
			this.m_actorLinesRaw = null;
			this.m_curIndex = -1;
			Singleton<CBattleGuideManager>.GetInstance().ResumeGame(this);
		}

		private void EndDialogueComplete()
		{
			if (this.bAutoNextPage)
			{
				this.m_nextPageProgressTime = 0f;
				this.m_bIsPlayingAutoNext = false;
			}
			this.m_bgGoList.Clear();
			if (this.m_fadingCoroutine != null)
			{
				base.StopCoroutine(this.m_fadingCoroutine);
				this.m_fadingCoroutine = null;
			}
			this.EndDialogue();
			this.ClearUiForms();
		}

		private void ClearUiForms()
		{
			DictionaryView<int, CUIFormScript>.Enumerator enumerator = this.m_uiFormMap.GetEnumerator();
			while (enumerator.MoveNext())
			{
				CUIManager arg_25_0 = Singleton<CUIManager>.GetInstance();
				KeyValuePair<int, CUIFormScript> current = enumerator.Current;
				arg_25_0.CloseForm(current.Value);
			}
			this.m_uiFormMap.Clear();
			this.m_curUiForm = null;
			if (this.m_curUiFormPortrait != null)
			{
				Singleton<CUIManager>.GetInstance().CloseForm(this.m_curUiFormPortrait);
				this.m_curUiFormPortrait = null;
			}
		}	

        private IEnumerator NextPageNoTimeScale()
        {
            NextPageInternal();
            m_bIsPlayingAutoNext = true;
            m_nextPageProgressTime = 0f;
            var timeAtLastFrame = 0f;
            var timeAtCurrentFrame = 0f;
            var deltaTime = 0f;
            timeAtLastFrame = Time.realtimeSinceStartup;
            while (m_bIsPlayingAutoNext)
            {
                if (!IsFading())
                {
                    timeAtCurrentFrame = Time.realtimeSinceStartup;
                    deltaTime = timeAtCurrentFrame - timeAtLastFrame;
                    timeAtLastFrame = timeAtCurrentFrame;
                    m_nextPageProgressTime += deltaTime;
                    if (m_nextPageProgressTime >= AutoNextPageTime)
                    {
                        m_bIsPlayingAutoNext = !NextPageInternal();
                        m_nextPageProgressTime = 0f;
                    }
                }
                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator StartFadingOut()
        {
            UpdateBgImageAlpha(1f);
            var isPlaying = true;
            var progressTime = 0f;
            var timeAtLastFrame = Time.realtimeSinceStartup;
            while (isPlaying)
            {
                var timeAtCurrentFrame = Time.realtimeSinceStartup;
                var deltaTime = timeAtCurrentFrame - timeAtLastFrame;
                timeAtLastFrame = timeAtCurrentFrame;
                progressTime += deltaTime;
                if (progressTime >= 0.5f)
                {
                    isPlaying = false;
                    progressTime = 0.5f;
                }
                var bgAlpha = ((1f - (progressTime / 0.5f)) * 0.8f) + 0.2f;
                UpdateBgImageAlpha(bgAlpha);
                yield return new WaitForEndOfFrame();
            }
            
            m_fadingCoroutine = null;
            if (m_curIndex >= m_actorLines.Length)
            {
                EndDialogueComplete();
            }
            else
            {
                DoNextPageInternal();
            }
        }

        private IEnumerator StartFadingIn()
        {
            UpdateBgImageAlpha(0.2f);
            bool isPlaying = true;
            float progressTime = 0f;
            float timeAtLastFrame = Time.realtimeSinceStartup;

            while (isPlaying)
            {
                float timeAtCurrentFrame = Time.realtimeSinceStartup;
                float deltaTime = timeAtCurrentFrame - timeAtLastFrame;
                timeAtLastFrame = timeAtCurrentFrame;
                progressTime += deltaTime;
                if (progressTime >= 0.5f)
                {
                    isPlaying = false;
                    progressTime = 0.5f;
                }
                float bgAlpha = ((progressTime / 0.5f) * 0.8f) + 0.2f;
                UpdateBgImageAlpha(bgAlpha);
                yield return new WaitForEndOfFrame();
            }
            
            m_fadingCoroutine = null;
        }

        public static IEnumerator PlayAnimNoTimeScale(Animation animation, string clipName, bool bLoop, System.Action onComplete)
        {
            DebugHelper.Assert(animation != null);
            DebugHelper.Assert(clipName != null);
            DebugHelper.Assert(!string.IsNullOrEmpty(clipName));
            var currState = animation[clipName];
            if (currState == null)
            {
                yield return null;
                if (onComplete != null)
                {
                    onComplete();
                }
                yield break;
            }
            currState.wrapMode = !bLoop ? WrapMode.Once : WrapMode.Loop;
            var isPlaying = true;
            var progressTime = 0f;
            var timeAtLastFrame = 0f;
            var timeAtCurrentFrame = 0f;
            var deltaTime = 0f;
            animation.Play(clipName);
            timeAtLastFrame = Time.realtimeSinceStartup;

            while (isPlaying)
            {
                timeAtCurrentFrame = Time.realtimeSinceStartup;
                deltaTime = timeAtCurrentFrame - timeAtLastFrame;
                timeAtLastFrame = timeAtCurrentFrame;
                progressTime += deltaTime;
                currState.normalizedTime = progressTime / currState.length;
                animation.Sample();
                if (progressTime >= currState.length)
                {
                    if (currState.wrapMode != WrapMode.Loop)
                    {
                        isPlaying = false;
                    }
                    else
                    {
                        progressTime = 0f;
                    }
                }
                yield return new WaitForEndOfFrame();
            }

            yield return null;
            if (onComplete != null)
            {
                onComplete();
            }
        }

		private void UpdateBgImageAlpha(float inAlpha)
		{
			ListView<GameObject>.Enumerator enumerator = this.m_bgGoList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (!(enumerator.Current == null))
				{
					GameObject current = enumerator.Current;
					Image component = current.GetComponent<Image>();
					Text component2 = current.GetComponent<Text>();
					if (component != null)
					{
						Color color = component.color;
						component.color = new Color(color.r, color.g, color.b, inAlpha);
					}
					if (component2 != null)
					{
						Color color2 = component2.color;
						component2.color = new Color(color2.r, color2.g, color2.b, inAlpha);
					}
				}
			}
		}



		private void UpdatePortrait3d(bool bActive, DialogueProcessor.SActorLineNode node, DialogueProcessor.SActorLineNode preNode)
		{
			if (this.m_curUiFormPortrait == null)
			{
				return;
			}
			Transform transform = this.m_curUiFormPortrait.gameObject.transform.FindChild("PanelCenter");
			if (transform == null)
			{
				return;
			}
			transform.gameObject.CustomSetActive(bActive);
			if (bActive)
			{
				if (preNode.AnimName != node.AnimName || preNode.bAnimLoop != node.bAnimLoop || preNode.b3dPortrait != node.b3dPortrait || preNode.ActorType != node.ActorType || preNode.CharCfgId != node.CharCfgId)
				{
					CUI3DImageScript component = transform.FindChild("3DImage").gameObject.GetComponent<CUI3DImageScript>();
					this.OnDestroyPortrait(this.m_3dPortraitName);
					component.RemoveGameObject(this.m_3dPortraitName);
					this.m_3dPortraitName = null;
					CActorInfo actorInfo = node.PortraitImgPath.ActorInfo;
					TransformConfig transformConfig = (!(actorInfo != null)) ? null : actorInfo.GetTransformConfig(ETransformConfigUsage.NPCInStory);
					GameObject gameObject;
					if (transformConfig != null)
					{
						Vector2 a = new Vector2(transformConfig.Offset.x, transformConfig.Offset.y);
						a += component.GetPivotScreenPosition();
						gameObject = component.AddGameObject(node.PortraitImgPath.ObjectName, false, ref a, true, true, null);
						if (gameObject != null)
						{
							gameObject.transform.localScale = gameObject.transform.localScale * transformConfig.Scale;
						}
					}
					else
					{
						gameObject = component.AddGameObject(node.PortraitImgPath.ObjectName, false, true);
					}
					CResourcePackerInfo resourceBelongedPackerInfo = Singleton<CResourceManager>.GetInstance().GetResourceBelongedPackerInfo(node.PortraitImgPath.ObjectName);
					if (resourceBelongedPackerInfo != null && resourceBelongedPackerInfo.IsAssetBundleLoaded())
					{
						resourceBelongedPackerInfo.UnloadAssetBundle(false);
					}
					this.m_3dPortraitName = gameObject;
					if (gameObject)
					{
						Animation component2 = gameObject.GetComponent<Animation>();
						if (component2)
						{
							string clipName = "Idleshow";
							if (node.AnimName != null && !string.IsNullOrEmpty(node.AnimName))
							{
								clipName = node.AnimName;
							}
							Coroutine value = base.StartCoroutine(DialogueProcessor.PlayAnimNoTimeScale(component2, clipName, node.bAnimLoop, new System.Action(this.OnPortrait3dAnimComplete)));
							this.Portrait3dAnimCoMap.Add(gameObject, value);
						}
					}
				}
			}
			else
			{
				CUI3DImageScript component3 = transform.FindChild("3DImage").gameObject.GetComponent<CUI3DImageScript>();
				this.OnDestroyPortrait(this.m_3dPortraitName);
				component3.RemoveGameObject(this.m_3dPortraitName);
				this.m_3dPortraitName = null;
			}
		}

		private void OnPortrait3dAnimComplete()
		{
			if (this.m_curUiFormPortrait == null)
			{
				return;
			}
			Transform x = this.m_curUiFormPortrait.gameObject.transform.FindChild("PanelCenter");
			if (x == null)
			{
				return;
			}
			GameObject _3dPortraitName = this.m_3dPortraitName;
			if (_3dPortraitName)
			{
				Animation component = _3dPortraitName.GetComponent<Animation>();
				if (component)
				{
					Coroutine value = base.StartCoroutine(DialogueProcessor.PlayAnimNoTimeScale(component, "Idleshow", true, null));
					if (this.Portrait3dAnimCoMap.ContainsKey(_3dPortraitName))
					{
						this.Portrait3dAnimCoMap[_3dPortraitName] = value;
					}
					else
					{
						this.Portrait3dAnimCoMap.Add(_3dPortraitName, value);
					}
				}
			}
		}

		private void UpdatePortraitImg(bool bActive, DialogueProcessor.SActorLineNode node, DialogueProcessor.SActorLineNode preNode)
		{
			if (this.m_curUiForm == null)
			{
				return;
			}
			Transform transform = this.m_curUiForm.gameObject.transform.Find("Pic_Npc");
			if (transform)
			{
				transform.gameObject.CustomSetActive(bActive);
				if (bActive)
				{
					if (node.PortraitImgPrefab.Object != null)
					{
						GameObject gameObject = transform.gameObject;
						Image component = gameObject.GetComponent<Image>();
						component.SetSprite(node.PortraitImgPrefab.Object, false);
					}
					else
					{
						transform.gameObject.CustomSetActive(false);
					}
				}
			}
		}

		public static AGE.Action PlayAgeAction(string inActionName, string inHelperName, GameObject inSrc, GameObject inAtker, ActionStopDelegate inCallback = null, int inHelperIndex = -1)
		{
			ActionHelperStorage actionHelperStorage = null;
			ActionHelper actionHelper = null;
			if (inSrc)
			{
				actionHelper = inSrc.GetComponent<ActionHelper>();
			}
			if (actionHelper == null && inAtker)
			{
				actionHelper = inAtker.GetComponent<ActionHelper>();
			}
			if (actionHelper != null && inHelperName != null && inHelperName.Length > 0)
			{
				actionHelperStorage = actionHelper.GetAction(inHelperName);
			}
			if (actionHelperStorage == null && inHelperIndex >= 0 && actionHelper != null)
			{
				actionHelperStorage = actionHelper.GetAction(inHelperIndex);
			}
			AGE.Action action = null;
			if (actionHelperStorage != null)
			{
				int num = (actionHelperStorage.targets.Length >= 2) ? actionHelperStorage.targets.Length : 2;
				GameObject[] array = new GameObject[num];
				if (actionHelperStorage.targets.Length == 0)
				{
					array[0] = inSrc;
					array[1] = inAtker;
				}
				else if (actionHelperStorage.targets.Length == 1)
				{
					if (actionHelperStorage.targets[0] == null)
					{
						array[0] = inSrc;
					}
					else
					{
						array[0] = actionHelperStorage.targets[0];
					}
					array[1] = inAtker;
				}
				else
				{
					if (actionHelperStorage.targets[0] == null)
					{
						array[0] = inSrc;
					}
					else
					{
						array[0] = actionHelperStorage.targets[0];
					}
					if (actionHelperStorage.targets[1] == null)
					{
						array[1] = inAtker;
					}
					else
					{
						array[1] = actionHelperStorage.targets[1];
					}
					for (int i = 2; i < num; i++)
					{
						array[i] = actionHelperStorage.targets[i];
					}
				}
				actionHelperStorage.autoPlay = true;
				action = actionHelperStorage.PlayActionEx(array);
			}
			if (action == null && inActionName != null && inActionName.Length > 0)
			{
				action = ActionManager.Instance.PlayAction(inActionName, true, false, new GameObject[]
				{
					inSrc,
					inAtker
				});
			}
			if (action != null && inCallback != null)
			{
				action.onActionStop += inCallback;
			}
			return action;
		}

		private bool PlayAgeActionInternal(GameObject inSrc, GameObject inAtker, int inGroupId)
		{
			if (this.m_curAction && !ActionManager.Instance.IsActionValid(this.m_curAction))
			{
				return false;
			}
			this.m_curAction = new PoolObjHandle<AGE.Action>(DialogueProcessor.PlayAgeAction(this.m_ageActName, this.m_ageActName, inSrc, inAtker, new ActionStopDelegate(this.OnActionStoped), -1));
			if (this.m_curAction)
			{
				this.m_curAction.handle.refParams.AddRefParam("DialogGroupIdRaw", inGroupId);
			}
			return this.m_curAction;
		}

		private void OnActionStoped(ref PoolObjHandle<AGE.Action> action)
		{
			if (!action)
			{
				return;
			}
			action.handle.onActionStop -= new ActionStopDelegate(this.OnActionStoped);
			if (action == this.m_curAction)
			{
				this.m_curAction.Release();
				this.m_ageActName = null;
			}
		}

		private void SkipPages(CUIEvent inUiEvent)
		{
			this.EndDialogueComplete();
		}

		private void NextPage(CUIEvent inUiEvent)
		{
			if (this.IsFading())
			{
				return;
			}
			this.NextPageInternal();
		}

		private bool IsFading()
		{
			return this.m_fadingCoroutine != null;
		}

		private void DoNextPageInternal()
		{
			DialogueProcessor.SActorLineNode node = this.m_actorLines[this.m_curIndex];
			DialogueProcessor.SActorLineNode preNode = default(DialogueProcessor.SActorLineNode);
			preNode.CharCfgId = -1;
			if (this.m_curIndex > 0)
			{
				preNode = this.m_actorLines[this.m_curIndex - 1];
			}
			CUIFormScript cUIFormScript = null;
			if (node.DialogStyle >= 0)
			{
				cUIFormScript = this.QueryUiForm(node.DialogStyle);
			}
			if (this.m_curUiForm != cUIFormScript && cUIFormScript != null)
			{
				if (this.m_curUiForm != null)
				{
					this.m_curUiForm.Hide(enFormHideFlag.HideByCustom, true);
					if (this.m_curUiFormPortrait != null)
					{
						this.m_curUiFormPortrait.Hide(enFormHideFlag.HideByCustom, true);
					}
				}
				this.m_curUiForm = cUIFormScript;
				if (this.m_curUiForm != null)
				{
					this.m_curUiForm.Appear(enFormHideFlag.HideByCustom, true);
					if (node.DialogStyle == 0 && this.m_curUiFormPortrait != null)
					{
						this.m_curUiFormPortrait.Appear(enFormHideFlag.HideByCustom, true);
					}
				}
			}
			if (this.m_curUiForm != null)
			{
				string[] array = (node.DialogStyle != 1) ? DialogueProcessor.ImageDialogBgPaths : DialogueProcessor.BlackDialogBgPaths;
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string name = array2[i];
					Transform transform = this.m_curUiForm.gameObject.transform.FindChild(name);
					if (transform != null)
					{
						this.m_bgGoList.Add(transform.gameObject);
					}
				}
				GameObject gameObject = this.m_curUiForm.gameObject.transform.Find("Txt_Dialog").gameObject;
				Text component = gameObject.GetComponent<Text>();
				component.text = node.DialogContent;
				GameObject gameObject2 = this.m_curUiForm.gameObject.transform.Find("CharacterName").gameObject;
				Text component2 = gameObject2.GetComponent<Text>();
				component2.text = node.DialogTitle;
				if (node.b3dPortrait)
				{
					this.UpdatePortrait3d(true, node, preNode);
					this.UpdatePortraitImg(false, node, preNode);
				}
				else
				{
					this.UpdatePortrait3d(false, node, preNode);
					this.UpdatePortraitImg(true, node, preNode);
				}
			}
			if (node.bFadeIn)
			{
				this.m_fadingCoroutine = base.StartCoroutine(this.StartFadingIn());
			}
			else
			{
				this.UpdateBgImageAlpha(1f);
			}
		}

		private bool NextPageInternal()
		{
			if (this.m_actorLines == null)
			{
				this.EndDialogueComplete();
				return true;
			}
			if (++this.m_curIndex >= this.m_actorLines.Length)
			{
				DialogueProcessor.SActorLineNode sActorLineNode = this.m_actorLines[this.m_curIndex - 1];
				if (!sActorLineNode.bFadeOut)
				{
					this.EndDialogueComplete();
				}
				else
				{
					this.m_fadingCoroutine = base.StartCoroutine(this.StartFadingOut());
				}
				return true;
			}
			if (this.bAutoNextPage)
			{
				this.m_nextPageProgressTime = 0f;
			}
			SActorLineNode node2 = new SActorLineNode {
				CharCfgId = -1
			};
			if (this.m_curIndex > 0)
			{
				node2 = this.m_actorLines[this.m_curIndex - 1];
				if (node2.bFadeOut)
				{
					this.m_fadingCoroutine = base.StartCoroutine(this.StartFadingOut());
					return false;
				}
			}
			this.DoNextPageInternal();
			return false;
		}

		protected override void Init()
		{
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Dialogue_NextPage, new CUIEventManager.OnUIEventHandler(this.NextPage));
			Singleton<CUIEventManager>.GetInstance().AddUIEventListener(enUIEventID.Dialogue_SkipPages, new CUIEventManager.OnUIEventHandler(this.SkipPages));
			Singleton<GameEventSys>.instance.AddEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
		}

		public void Uninit()
		{
			this.EndDialogueComplete();
			this.PreDialogId = 0;
		}

		protected override void OnDestroy()
		{
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Dialogue_NextPage, new CUIEventManager.OnUIEventHandler(this.NextPage));
			Singleton<CUIEventManager>.GetInstance().RemoveUIEventListener(enUIEventID.Dialogue_SkipPages, new CUIEventManager.OnUIEventHandler(this.SkipPages));
			Singleton<GameEventSys>.instance.RmvEventHandler<GameDeadEventParam>(GameEventDef.Event_ActorDead, new RefAction<GameDeadEventParam>(this.onActorDead));
			base.OnDestroy();
		}

		private void onActorDead(ref GameDeadEventParam prm)
		{
			if (prm.src.handle.CharInfo && prm.src.handle.CharInfo.DyingDialogGroupId > 0)
			{
				GameObject inAtker = (!prm.orignalAtker) ? null : prm.orignalAtker.handle.gameObject;
				this.PlayDrama(prm.src.handle.CharInfo.DyingDialogGroupId, prm.src.handle.gameObject, inAtker, false);
			}
		}

		private CUIFormScript QueryUiForm(int inDialogStyle)
		{
			if (this.m_uiFormMap.ContainsKey(inDialogStyle))
			{
				return this.m_uiFormMap[inDialogStyle];
			}
			string formPath = this.QueryDialogTempPath(inDialogStyle);
			CUIFormScript cUIFormScript = Singleton<CUIManager>.GetInstance().OpenForm(formPath, true, false);
			DebugHelper.Assert(cUIFormScript != null);
			if (cUIFormScript != null)
			{
				this.m_uiFormMap.Add(inDialogStyle, cUIFormScript);
				cUIFormScript.Hide(enFormHideFlag.HideByCustom, true);
				if (inDialogStyle == 0)
				{
					this.m_curUiFormPortrait = Singleton<CUIManager>.GetInstance().OpenForm("UGUI/Form/System/Dialog/Form_NpcDialogPortrait", true, false);
					if (this.m_curUiFormPortrait != null)
					{
						this.m_curUiFormPortrait.Hide(enFormHideFlag.HideByCustom, true);
					}
				}
			}
			return cUIFormScript;
		}

		private string QueryDialogTempPath(int inDialogStyle)
		{
			string result;
			if (inDialogStyle != 0)
			{
				if (inDialogStyle == 1)
				{
					result = "UGUI/Form/System/Dialog/Form_DialogBlack";
					return result;
				}
			}
			result = "UGUI/Form/System/Dialog/Form_NpcDialog";
			return result;
		}
	}
}
