using System;
using UnityEngine;
using UnityEngine.UI;

public class LTDescr
{
	public bool toggle;

	public bool useEstimatedTime;

	public bool useFrames;

	public bool useManualTime;

	public bool hasInitiliazed;

	public bool hasPhysics;

	public bool onCompleteOnRepeat;

	public bool onCompleteOnStart;

	public float passed;

	public float delay;

	public float time;

	public float lastVal;

	private uint _id;

	public int loopCount;

	public uint counter;

	public float direction;

	public float directionLast;

	public bool destroyOnComplete;

	public Transform trans;

	public LTRect ltRect;

	public Vector3 from;

	public Vector3 to;

	public Vector3 diff;

	public Vector3 point;

	public Vector3 axis;

	public Quaternion origRotation;

	public LTBezierPath path;

	public LTSpline spline;

	public TweenAction type;

	public LeanTweenType tweenType;

	public AnimationCurve animationCurve;

	public LeanTweenType loopType;

	public bool hasUpdateCallback;

	public Action<float> onUpdateFloat;

	public Action<float, object> onUpdateFloatObject;

	public Action<Vector2> onUpdateVector2;

	public Action<Vector3> onUpdateVector3;

	public Action<Vector3, object> onUpdateVector3Object;

	public Action<Color> onUpdateColor;

	public Action onComplete;

	public Action<object> onCompleteObject;

	public object onCompleteParam;

	public object onUpdateParam;

	public RectTransform rectTransform;

	public Text uiText;

	public Image uiImage;

	public Sprite[] sprites;

	private static uint global_counter;

	public int uniqueId
	{
		get
		{
			return (int)(this._id | this.counter << 16);
		}
	}

	public int id
	{
		get
		{
			return this.uniqueId;
		}
	}

	public override string ToString()
	{
		return string.Concat(new object[]
		{
			(!(this.trans != null)) ? "gameObject:null" : ("gameObject:" + this.trans.gameObject),
			" toggle:",
			this.toggle,
			" passed:",
			this.passed,
			" time:",
			this.time,
			" delay:",
			this.delay,
			" from:",
			this.from,
			" to:",
			this.to,
			" type:",
			this.type,
			" ease:",
			this.tweenType,
			" useEstimatedTime:",
			this.useEstimatedTime,
			" id:",
			this.id,
			" hasInitiliazed:",
			this.hasInitiliazed
		});
	}

	public LTDescr cancel()
	{
		LeanTween.removeTween((int)this._id);
		return this;
	}

	public void reset()
	{
		this.toggle = true;
		this.trans = null;
		this.passed = (this.delay = (this.lastVal = 0f));
		this.hasUpdateCallback = (this.useEstimatedTime = (this.useFrames = (this.hasInitiliazed = (this.onCompleteOnRepeat = (this.destroyOnComplete = (this.onCompleteOnStart = (this.useManualTime = false)))))));
		this.animationCurve = null;
		this.tweenType = LeanTweenType.linear;
		this.loopType = LeanTweenType.once;
		this.loopCount = 0;
		this.direction = (this.directionLast = 1f);
		this.onUpdateFloat = null;
		this.onUpdateVector2 = null;
		this.onUpdateVector3 = null;
		this.onUpdateFloatObject = null;
		this.onUpdateVector3Object = null;
		this.onUpdateColor = null;
		this.onComplete = null;
		this.onCompleteObject = null;
		this.onCompleteParam = null;
		this.point = Vector3.zero;
		this.rectTransform = null;
		this.uiText = null;
		this.uiImage = null;
		this.sprites = null;
		LTDescr.global_counter += 1u;
		if (LTDescr.global_counter > 32768u)
		{
			LTDescr.global_counter = 0u;
		}
	}

	public void init()
	{
		this.hasInitiliazed = true;
		switch (this.type)
		{
		case TweenAction.MOVE_X:
			this.from.x = this.trans.position.x;
			break;
		case TweenAction.MOVE_Y:
			this.from.x = this.trans.position.y;
			break;
		case TweenAction.MOVE_Z:
			this.from.x = this.trans.position.z;
			break;
		case TweenAction.MOVE_LOCAL_X:
			this.from.x = this.trans.localPosition.x;
			break;
		case TweenAction.MOVE_LOCAL_Y:
			this.from.x = this.trans.localPosition.y;
			break;
		case TweenAction.MOVE_LOCAL_Z:
			this.from.x = this.trans.localPosition.z;
			break;
		case TweenAction.MOVE_CURVED:
		case TweenAction.MOVE_CURVED_LOCAL:
		case TweenAction.MOVE_SPLINE:
		case TweenAction.MOVE_SPLINE_LOCAL:
			this.from.x = 0f;
			break;
		case TweenAction.SCALE_X:
			this.from.x = this.trans.localScale.x;
			break;
		case TweenAction.SCALE_Y:
			this.from.x = this.trans.localScale.y;
			break;
		case TweenAction.SCALE_Z:
			this.from.x = this.trans.localScale.z;
			break;
		case TweenAction.ROTATE_X:
			this.from.x = this.trans.eulerAngles.x;
			this.to.x = LeanTween.closestRot(this.from.x, this.to.x);
			break;
		case TweenAction.ROTATE_Y:
			this.from.x = this.trans.eulerAngles.y;
			this.to.x = LeanTween.closestRot(this.from.x, this.to.x);
			break;
		case TweenAction.ROTATE_Z:
			this.from.x = this.trans.eulerAngles.z;
			this.to.x = LeanTween.closestRot(this.from.x, this.to.x);
			break;
		case TweenAction.ROTATE_AROUND:
			this.lastVal = 0f;
			this.from.x = 0f;
			this.origRotation = this.trans.rotation;
			break;
		case TweenAction.ROTATE_AROUND_LOCAL:
			this.lastVal = 0f;
			this.from.x = 0f;
			this.origRotation = this.trans.localRotation;
			break;
		case TweenAction.CANVAS_ROTATEAROUND:
		case TweenAction.CANVAS_ROTATEAROUND_LOCAL:
			this.lastVal = 0f;
			this.from.x = 0f;
			this.origRotation = this.rectTransform.rotation;
			break;
		case TweenAction.CANVAS_PLAYSPRITE:
			this.uiImage = this.trans.gameObject.GetComponent<Image>();
			this.from.x = 0f;
			break;
		case TweenAction.ALPHA:
		{
			SpriteRenderer component = this.trans.gameObject.GetComponent<SpriteRenderer>();
			if (component != null)
			{
				this.from.x = component.color.a;
			}
			else if (this.trans.gameObject.GetComponent<Renderer>() != null && this.trans.gameObject.GetComponent<Renderer>().material.HasProperty("_Color"))
			{
				this.from.x = this.trans.gameObject.GetComponent<Renderer>().material.color.a;
			}
			else if (this.trans.gameObject.GetComponent<Renderer>() != null && this.trans.gameObject.GetComponent<Renderer>().material.HasProperty("_TintColor"))
			{
				this.from.x = this.trans.gameObject.GetComponent<Renderer>().material.GetColor("_TintColor").a;
			}
			else if (this.trans.childCount > 0)
			{
				foreach (Transform transform in this.trans)
				{
					if (transform.gameObject.GetComponent<Renderer>() != null)
					{
						this.from.x = transform.gameObject.GetComponent<Renderer>().material.color.a;
						break;
					}
				}
			}
			break;
		}
		case TweenAction.TEXT_ALPHA:
			this.uiText = this.trans.gameObject.GetComponent<Text>();
			if (this.uiText != null)
			{
				this.from.x = this.uiText.color.a;
			}
			break;
		case TweenAction.CANVAS_ALPHA:
			this.uiImage = this.trans.gameObject.GetComponent<Image>();
			if (this.uiImage != null)
			{
				this.from.x = this.uiImage.color.a;
			}
			break;
		case TweenAction.ALPHA_VERTEX:
			this.from.x = (float)this.trans.GetComponent<MeshFilter>().mesh.colors32[0].a;
			break;
		case TweenAction.COLOR:
			if (this.trans.gameObject.GetComponent<Renderer>() != null && this.trans.gameObject.GetComponent<Renderer>().material.HasProperty("_Color"))
			{
				Color color = this.trans.gameObject.GetComponent<Renderer>().material.color;
				this.setFromColor(color);
			}
			else if (this.trans.gameObject.GetComponent<Renderer>() != null && this.trans.gameObject.GetComponent<Renderer>().material.HasProperty("_TintColor"))
			{
				Color color2 = this.trans.gameObject.GetComponent<Renderer>().material.GetColor("_TintColor");
				this.setFromColor(color2);
			}
			else if (this.trans.childCount > 0)
			{
				foreach (Transform transform2 in this.trans)
				{
					if (transform2.gameObject.GetComponent<Renderer>() != null)
					{
						Color color3 = transform2.gameObject.GetComponent<Renderer>().material.color;
						this.setFromColor(color3);
						break;
					}
				}
			}
			break;
		case TweenAction.CALLBACK_COLOR:
			this.diff = new Vector3(1f, 0f, 0f);
			break;
		case TweenAction.TEXT_COLOR:
			this.uiText = this.trans.gameObject.GetComponent<Text>();
			if (this.uiText != null)
			{
				this.setFromColor(this.uiText.color);
			}
			break;
		case TweenAction.CANVAS_COLOR:
			this.uiImage = this.trans.gameObject.GetComponent<Image>();
			if (this.uiImage != null)
			{
				this.setFromColor(this.uiImage.color);
			}
			break;
		case TweenAction.CALLBACK:
			if (this.onCompleteOnStart)
			{
				if (this.onComplete != null)
				{
					this.onComplete();
				}
				else if (this.onCompleteObject != null)
				{
					this.onCompleteObject(this.onCompleteParam);
				}
			}
			break;
		case TweenAction.MOVE:
			this.from = this.trans.position;
			break;
		case TweenAction.MOVE_LOCAL:
			this.from = this.trans.localPosition;
			break;
		case TweenAction.ROTATE:
			this.from = this.trans.eulerAngles;
			this.to = new Vector3(LeanTween.closestRot(this.from.x, this.to.x), LeanTween.closestRot(this.from.y, this.to.y), LeanTween.closestRot(this.from.z, this.to.z));
			break;
		case TweenAction.ROTATE_LOCAL:
			this.from = this.trans.localEulerAngles;
			this.to = new Vector3(LeanTween.closestRot(this.from.x, this.to.x), LeanTween.closestRot(this.from.y, this.to.y), LeanTween.closestRot(this.from.z, this.to.z));
			break;
		case TweenAction.SCALE:
			this.from = this.trans.localScale;
			break;
		case TweenAction.GUI_MOVE:
			this.from = new Vector3(this.ltRect.rect.x, this.ltRect.rect.y, 0f);
			break;
		case TweenAction.GUI_MOVE_MARGIN:
			this.from = new Vector2(this.ltRect.margin.x, this.ltRect.margin.y);
			break;
		case TweenAction.GUI_SCALE:
			this.from = new Vector3(this.ltRect.rect.width, this.ltRect.rect.height, 0f);
			break;
		case TweenAction.GUI_ALPHA:
			this.from.x = this.ltRect.alpha;
			break;
		case TweenAction.GUI_ROTATE:
			if (!this.ltRect.rotateEnabled)
			{
				this.ltRect.rotateEnabled = true;
				this.ltRect.resetForRotation();
			}
			this.from.x = this.ltRect.rotation;
			break;
		case TweenAction.CANVAS_MOVE:
			this.from = this.rectTransform.anchoredPosition3D;
			break;
		case TweenAction.CANVAS_SCALE:
			this.from = this.rectTransform.localScale;
			break;
		}
		if (this.type != TweenAction.CALLBACK_COLOR && this.type != TweenAction.COLOR && this.type != TweenAction.TEXT_COLOR && this.type != TweenAction.CANVAS_COLOR)
		{
			this.diff = this.to - this.from;
		}
	}

	public LTDescr setFromColor(Color col)
	{
		this.from = new Vector3(0f, col.a, 0f);
		this.diff = new Vector3(1f, 0f, 0f);
		this.axis = new Vector3(col.r, col.g, col.b);
		return this;
	}

	public LTDescr pause()
	{
		if (this.direction != 0f)
		{
			this.directionLast = this.direction;
			this.direction = 0f;
		}
		return this;
	}

	public LTDescr resume()
	{
		this.direction = this.directionLast;
		return this;
	}

	public LTDescr setAxis(Vector3 axis)
	{
		this.axis = axis;
		return this;
	}

	public LTDescr setDelay(float delay)
	{
		if (this.useEstimatedTime)
		{
			this.delay = delay;
		}
		else
		{
			this.delay = delay;
		}
		return this;
	}

	public LTDescr setEase(LeanTweenType easeType)
	{
		this.tweenType = easeType;
		return this;
	}

	public LTDescr setEase(AnimationCurve easeCurve)
	{
		this.animationCurve = easeCurve;
		return this;
	}

	public LTDescr setTo(Vector3 to)
	{
		if (this.hasInitiliazed)
		{
			this.to = to;
			this.diff = to - this.from;
		}
		else
		{
			this.to = to;
		}
		return this;
	}

	public LTDescr setFrom(Vector3 from)
	{
		if (this.trans)
		{
			this.init();
		}
		this.from = from;
		this.diff = this.to - this.from;
		return this;
	}

	public LTDescr setFrom(float from)
	{
		return this.setFrom(new Vector3(from, 0f, 0f));
	}

	public LTDescr setDiff(Vector3 diff)
	{
		this.diff = diff;
		return this;
	}

	public LTDescr setHasInitialized(bool has)
	{
		this.hasInitiliazed = has;
		return this;
	}

	public LTDescr setId(uint id)
	{
		this._id = id;
		this.counter = LTDescr.global_counter;
		return this;
	}

	public LTDescr setTime(float time)
	{
		this.time = time;
		return this;
	}

	public LTDescr setRepeat(int repeat)
	{
		this.loopCount = repeat;
		if ((repeat > 1 && this.loopType == LeanTweenType.once) || (repeat < 0 && this.loopType == LeanTweenType.once))
		{
			this.loopType = LeanTweenType.clamp;
		}
		if (this.type == TweenAction.CALLBACK || this.type == TweenAction.CALLBACK_COLOR)
		{
			this.setOnCompleteOnRepeat(true);
		}
		return this;
	}

	public LTDescr setLoopType(LeanTweenType loopType)
	{
		this.loopType = loopType;
		return this;
	}

	public LTDescr setUseEstimatedTime(bool useEstimatedTime)
	{
		this.useEstimatedTime = useEstimatedTime;
		return this;
	}

	public LTDescr setUseFrames(bool useFrames)
	{
		this.useFrames = useFrames;
		return this;
	}

	public LTDescr setUseManualTime(bool useManualTime)
	{
		this.useManualTime = useManualTime;
		return this;
	}

	public LTDescr setLoopCount(int loopCount)
	{
		this.loopCount = loopCount;
		return this;
	}

	public LTDescr setLoopOnce()
	{
		this.loopType = LeanTweenType.once;
		return this;
	}

	public LTDescr setLoopClamp()
	{
		this.loopType = LeanTweenType.clamp;
		if (this.loopCount == 0)
		{
			this.loopCount = -1;
		}
		return this;
	}

	public LTDescr setLoopPingPong()
	{
		this.loopType = LeanTweenType.pingPong;
		if (this.loopCount == 0)
		{
			this.loopCount = -1;
		}
		return this;
	}

	public LTDescr setOnComplete(Action onComplete)
	{
		this.onComplete = onComplete;
		return this;
	}

	public LTDescr setOnComplete(Action<object> onComplete)
	{
		this.onCompleteObject = onComplete;
		return this;
	}

	public LTDescr setOnComplete(Action<object> onComplete, object onCompleteParam)
	{
		this.onCompleteObject = onComplete;
		if (onCompleteParam != null)
		{
			this.onCompleteParam = onCompleteParam;
		}
		return this;
	}

	public LTDescr setOnCompleteParam(object onCompleteParam)
	{
		this.onCompleteParam = onCompleteParam;
		return this;
	}

	public LTDescr setOnUpdate(Action<float> onUpdate)
	{
		this.onUpdateFloat = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdateObject(Action<float, object> onUpdate)
	{
		this.onUpdateFloatObject = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdateVector2(Action<Vector2> onUpdate)
	{
		this.onUpdateVector2 = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdateVector3(Action<Vector3> onUpdate)
	{
		this.onUpdateVector3 = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdateColor(Action<Color> onUpdate)
	{
		this.onUpdateColor = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdate(Action<Color> onUpdate)
	{
		this.onUpdateColor = onUpdate;
		this.hasUpdateCallback = true;
		return this;
	}

	public LTDescr setOnUpdate(Action<float, object> onUpdate, object onUpdateParam = null)
	{
		this.onUpdateFloatObject = onUpdate;
		this.hasUpdateCallback = true;
		if (onUpdateParam != null)
		{
			this.onUpdateParam = onUpdateParam;
		}
		return this;
	}

	public LTDescr setOnUpdate(Action<Vector3, object> onUpdate, object onUpdateParam = null)
	{
		this.onUpdateVector3Object = onUpdate;
		this.hasUpdateCallback = true;
		if (onUpdateParam != null)
		{
			this.onUpdateParam = onUpdateParam;
		}
		return this;
	}

	public LTDescr setOnUpdate(Action<Vector2> onUpdate, object onUpdateParam = null)
	{
		this.onUpdateVector2 = onUpdate;
		this.hasUpdateCallback = true;
		if (onUpdateParam != null)
		{
			this.onUpdateParam = onUpdateParam;
		}
		return this;
	}

	public LTDescr setOnUpdate(Action<Vector3> onUpdate, object onUpdateParam = null)
	{
		this.onUpdateVector3 = onUpdate;
		this.hasUpdateCallback = true;
		if (onUpdateParam != null)
		{
			this.onUpdateParam = onUpdateParam;
		}
		return this;
	}

	public LTDescr setOnUpdateParam(object onUpdateParam)
	{
		this.onUpdateParam = onUpdateParam;
		return this;
	}

	public LTDescr setOrientToPath(bool doesOrient)
	{
		if (this.type == TweenAction.MOVE_CURVED || this.type == TweenAction.MOVE_CURVED_LOCAL)
		{
			if (this.path == null)
			{
				this.path = new LTBezierPath();
			}
			this.path.orientToPath = doesOrient;
		}
		else
		{
			this.spline.orientToPath = doesOrient;
		}
		return this;
	}

	public LTDescr setOrientToPath2d(bool doesOrient2d)
	{
		this.setOrientToPath(doesOrient2d);
		if (this.type == TweenAction.MOVE_CURVED || this.type == TweenAction.MOVE_CURVED_LOCAL)
		{
			this.path.orientToPath2d = doesOrient2d;
		}
		else
		{
			this.spline.orientToPath2d = doesOrient2d;
		}
		return this;
	}

	public LTDescr setRect(LTRect rect)
	{
		this.ltRect = rect;
		return this;
	}

	public LTDescr setRect(Rect rect)
	{
		this.ltRect = new LTRect(rect);
		return this;
	}

	public LTDescr setPath(LTBezierPath path)
	{
		this.path = path;
		return this;
	}

	public LTDescr setPoint(Vector3 point)
	{
		this.point = point;
		return this;
	}

	public LTDescr setDestroyOnComplete(bool doesDestroy)
	{
		this.destroyOnComplete = doesDestroy;
		return this;
	}

	public LTDescr setAudio(object audio)
	{
		this.onCompleteParam = audio;
		return this;
	}

	public LTDescr setOnCompleteOnRepeat(bool isOn)
	{
		this.onCompleteOnRepeat = isOn;
		return this;
	}

	public LTDescr setOnCompleteOnStart(bool isOn)
	{
		this.onCompleteOnStart = isOn;
		return this;
	}

	public LTDescr setRect(RectTransform rect)
	{
		this.rectTransform = rect;
		return this;
	}

	public LTDescr setSprites(Sprite[] sprites)
	{
		this.sprites = sprites;
		return this;
	}

	public LTDescr setFrameRate(float frameRate)
	{
		this.time = (float)this.sprites.Length / frameRate;
		return this;
	}
}
