using System;
using UnityEngine;

[Serializable]
public class LTRect
{
	public Rect _rect;

	public float alpha = 1f;

	public float rotation;

	public Vector2 pivot;

	public Vector2 margin;

	public Rect relativeRect = new Rect(0f, 0f, float.PositiveInfinity, float.PositiveInfinity);

	public bool rotateEnabled;

	[HideInInspector]
	public bool rotateFinished;

	public bool alphaEnabled;

	public string labelStr;

	public LTGUI.Element_Type type;

	public GUIStyle style;

	public bool useColor;

	public Color color = Color.white;

	public bool fontScaleToFit;

	public bool useSimpleScale;

	public bool sizeByHeight;

	public Texture texture;

	private int _id = -1;

	[HideInInspector]
	public int counter;

	public static bool colorTouched;

	public bool hasInitiliazed
	{
		get
		{
			return this._id != -1;
		}
	}

	public int id
	{
		get
		{
			return this._id | this.counter << 16;
		}
	}

	public float x
	{
		get
		{
			return this._rect.x;
		}
		set
		{
			this._rect.x = value;
		}
	}

	public float y
	{
		get
		{
			return this._rect.y;
		}
		set
		{
			this._rect.y = value;
		}
	}

	public float width
	{
		get
		{
			return this._rect.width;
		}
		set
		{
			this._rect.width = value;
		}
	}

	public float height
	{
		get
		{
			return this._rect.height;
		}
		set
		{
			this._rect.height = value;
		}
	}

	public Rect rect
	{
		get
		{
			if (LTRect.colorTouched)
			{
				LTRect.colorTouched = false;
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 1f);
			}
			if (this.rotateEnabled)
			{
				if (this.rotateFinished)
				{
					this.rotateFinished = false;
					this.rotateEnabled = false;
					this.pivot = Vector2.zero;
				}
				else
				{
					GUIUtility.RotateAroundPivot(this.rotation, this.pivot);
				}
			}
			if (this.alphaEnabled)
			{
				GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.alpha);
				LTRect.colorTouched = true;
			}
			if (this.fontScaleToFit)
			{
				if (this.useSimpleScale)
				{
					this.style.fontSize = (int)(this._rect.height * this.relativeRect.height);
				}
				else
				{
					this.style.fontSize = (int)this._rect.height;
				}
			}
			return this._rect;
		}
		set
		{
			this._rect = value;
		}
	}

	public LTRect()
	{
		this.reset();
		this.rotateEnabled = (this.alphaEnabled = true);
		this._rect = new Rect(0f, 0f, 1f, 1f);
	}

	public LTRect(Rect rect)
	{
		this._rect = rect;
		this.reset();
	}

	public LTRect(float x, float y, float width, float height)
	{
		this._rect = new Rect(x, y, width, height);
		this.alpha = 1f;
		this.rotation = 0f;
		this.rotateEnabled = (this.alphaEnabled = false);
	}

	public LTRect(float x, float y, float width, float height, float alpha)
	{
		this._rect = new Rect(x, y, width, height);
		this.alpha = alpha;
		this.rotation = 0f;
		this.rotateEnabled = (this.alphaEnabled = false);
	}

	public LTRect(float x, float y, float width, float height, float alpha, float rotation)
	{
		this._rect = new Rect(x, y, width, height);
		this.alpha = alpha;
		this.rotation = rotation;
		this.rotateEnabled = (this.alphaEnabled = false);
		if (rotation != 0f)
		{
			this.rotateEnabled = true;
			this.resetForRotation();
		}
	}

	public void setId(int id, int counter)
	{
		this._id = id;
		this.counter = counter;
	}

	public void reset()
	{
		this.alpha = 1f;
		this.rotation = 0f;
		this.rotateEnabled = (this.alphaEnabled = false);
		this.margin = Vector2.zero;
		this.sizeByHeight = false;
		this.useColor = false;
	}

	public void resetForRotation()
	{
		Vector3 vector = new Vector3(GUI.matrix[0, 0], GUI.matrix[1, 1], GUI.matrix[2, 2]);
		if (this.pivot == Vector2.zero)
		{
			this.pivot = new Vector2((this._rect.x + this._rect.width * 0.5f) * vector.x + GUI.matrix[0, 3], (this._rect.y + this._rect.height * 0.5f) * vector.y + GUI.matrix[1, 3]);
		}
	}

	public LTRect setStyle(GUIStyle style)
	{
		this.style = style;
		return this;
	}

	public LTRect setFontScaleToFit(bool fontScaleToFit)
	{
		this.fontScaleToFit = fontScaleToFit;
		return this;
	}

	public LTRect setColor(Color color)
	{
		this.color = color;
		this.useColor = true;
		return this;
	}

	public LTRect setAlpha(float alpha)
	{
		this.alpha = alpha;
		return this;
	}

	public LTRect setLabel(string str)
	{
		this.labelStr = str;
		return this;
	}

	public LTRect setUseSimpleScale(bool useSimpleScale, Rect relativeRect)
	{
		this.useSimpleScale = useSimpleScale;
		this.relativeRect = relativeRect;
		return this;
	}

	public LTRect setUseSimpleScale(bool useSimpleScale)
	{
		this.useSimpleScale = useSimpleScale;
		this.relativeRect = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);
		return this;
	}

	public LTRect setSizeByHeight(bool sizeByHeight)
	{
		this.sizeByHeight = sizeByHeight;
		return this;
	}

	public override string ToString()
	{
		return string.Concat(new object[]
		{
			"x:",
			this._rect.x,
			" y:",
			this._rect.y,
			" width:",
			this._rect.width,
			" height:",
			this._rect.height
		});
	}
}
