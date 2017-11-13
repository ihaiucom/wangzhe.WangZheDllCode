using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace TMPro
{
	public static class TMPro_EventManager
	{
		public delegate void COMPUTE_DT_EVENT_HANDLER(object Sender, Compute_DT_EventArgs e);

		public delegate void MaterialProperty_Event_Handler(bool isChanged, Material mat);

		public delegate void FontProperty_Event_Handler(bool isChanged, TextMeshProFont font);

		public delegate void SpriteAssetProperty_Event_Handler(bool isChanged, Object obj);

		public delegate void TextMeshProProperty_Event_Handler(bool isChanged, TextMeshPro obj);

		public delegate void DragAndDrop_Event_Handler(GameObject sender, Material currentMaterial, Material newMaterial);

		public delegate void TextMeshProUGUIProperty_Event_Handler(bool isChanged, TextMeshProUGUI obj);

		public delegate void BaseMaterial_Event_Handler(Material mat);

		public delegate void OnPreRenderObject_Event_Handler();

		public static event TMPro_EventManager.COMPUTE_DT_EVENT_HANDLER COMPUTE_DT_EVENT
		{
			[MethodImpl(32)]
			add
			{
				TMPro_EventManager.COMPUTE_DT_EVENT = (TMPro_EventManager.COMPUTE_DT_EVENT_HANDLER)Delegate.Combine(TMPro_EventManager.COMPUTE_DT_EVENT, value);
			}
			[MethodImpl(32)]
			remove
			{
				TMPro_EventManager.COMPUTE_DT_EVENT = (TMPro_EventManager.COMPUTE_DT_EVENT_HANDLER)Delegate.Remove(TMPro_EventManager.COMPUTE_DT_EVENT, value);
			}
		}

		public static event TMPro_EventManager.MaterialProperty_Event_Handler MATERIAL_PROPERTY_EVENT
		{
			[MethodImpl(32)]
			add
			{
				TMPro_EventManager.MATERIAL_PROPERTY_EVENT = (TMPro_EventManager.MaterialProperty_Event_Handler)Delegate.Combine(TMPro_EventManager.MATERIAL_PROPERTY_EVENT, value);
			}
			[MethodImpl(32)]
			remove
			{
				TMPro_EventManager.MATERIAL_PROPERTY_EVENT = (TMPro_EventManager.MaterialProperty_Event_Handler)Delegate.Remove(TMPro_EventManager.MATERIAL_PROPERTY_EVENT, value);
			}
		}

		public static event TMPro_EventManager.FontProperty_Event_Handler FONT_PROPERTY_EVENT
		{
			[MethodImpl(32)]
			add
			{
				TMPro_EventManager.FONT_PROPERTY_EVENT = (TMPro_EventManager.FontProperty_Event_Handler)Delegate.Combine(TMPro_EventManager.FONT_PROPERTY_EVENT, value);
			}
			[MethodImpl(32)]
			remove
			{
				TMPro_EventManager.FONT_PROPERTY_EVENT = (TMPro_EventManager.FontProperty_Event_Handler)Delegate.Remove(TMPro_EventManager.FONT_PROPERTY_EVENT, value);
			}
		}

		public static event TMPro_EventManager.SpriteAssetProperty_Event_Handler SPRITE_ASSET_PROPERTY_EVENT
		{
			[MethodImpl(32)]
			add
			{
				TMPro_EventManager.SPRITE_ASSET_PROPERTY_EVENT = (TMPro_EventManager.SpriteAssetProperty_Event_Handler)Delegate.Combine(TMPro_EventManager.SPRITE_ASSET_PROPERTY_EVENT, value);
			}
			[MethodImpl(32)]
			remove
			{
				TMPro_EventManager.SPRITE_ASSET_PROPERTY_EVENT = (TMPro_EventManager.SpriteAssetProperty_Event_Handler)Delegate.Remove(TMPro_EventManager.SPRITE_ASSET_PROPERTY_EVENT, value);
			}
		}

		public static event TMPro_EventManager.TextMeshProProperty_Event_Handler TEXTMESHPRO_PROPERTY_EVENT
		{
			[MethodImpl(32)]
			add
			{
				TMPro_EventManager.TEXTMESHPRO_PROPERTY_EVENT = (TMPro_EventManager.TextMeshProProperty_Event_Handler)Delegate.Combine(TMPro_EventManager.TEXTMESHPRO_PROPERTY_EVENT, value);
			}
			[MethodImpl(32)]
			remove
			{
				TMPro_EventManager.TEXTMESHPRO_PROPERTY_EVENT = (TMPro_EventManager.TextMeshProProperty_Event_Handler)Delegate.Remove(TMPro_EventManager.TEXTMESHPRO_PROPERTY_EVENT, value);
			}
		}

		public static event TMPro_EventManager.DragAndDrop_Event_Handler DRAG_AND_DROP_MATERIAL_EVENT
		{
			[MethodImpl(32)]
			add
			{
				TMPro_EventManager.DRAG_AND_DROP_MATERIAL_EVENT = (TMPro_EventManager.DragAndDrop_Event_Handler)Delegate.Combine(TMPro_EventManager.DRAG_AND_DROP_MATERIAL_EVENT, value);
			}
			[MethodImpl(32)]
			remove
			{
				TMPro_EventManager.DRAG_AND_DROP_MATERIAL_EVENT = (TMPro_EventManager.DragAndDrop_Event_Handler)Delegate.Remove(TMPro_EventManager.DRAG_AND_DROP_MATERIAL_EVENT, value);
			}
		}

		public static event TMPro_EventManager.TextMeshProUGUIProperty_Event_Handler TEXTMESHPRO_UGUI_PROPERTY_EVENT
		{
			[MethodImpl(32)]
			add
			{
				TMPro_EventManager.TEXTMESHPRO_UGUI_PROPERTY_EVENT = (TMPro_EventManager.TextMeshProUGUIProperty_Event_Handler)Delegate.Combine(TMPro_EventManager.TEXTMESHPRO_UGUI_PROPERTY_EVENT, value);
			}
			[MethodImpl(32)]
			remove
			{
				TMPro_EventManager.TEXTMESHPRO_UGUI_PROPERTY_EVENT = (TMPro_EventManager.TextMeshProUGUIProperty_Event_Handler)Delegate.Remove(TMPro_EventManager.TEXTMESHPRO_UGUI_PROPERTY_EVENT, value);
			}
		}

		public static event TMPro_EventManager.BaseMaterial_Event_Handler BASE_MATERIAL_EVENT
		{
			[MethodImpl(32)]
			add
			{
				TMPro_EventManager.BASE_MATERIAL_EVENT = (TMPro_EventManager.BaseMaterial_Event_Handler)Delegate.Combine(TMPro_EventManager.BASE_MATERIAL_EVENT, value);
			}
			[MethodImpl(32)]
			remove
			{
				TMPro_EventManager.BASE_MATERIAL_EVENT = (TMPro_EventManager.BaseMaterial_Event_Handler)Delegate.Remove(TMPro_EventManager.BASE_MATERIAL_EVENT, value);
			}
		}

		public static event TMPro_EventManager.OnPreRenderObject_Event_Handler OnPreRenderObject_Event
		{
			[MethodImpl(32)]
			add
			{
				TMPro_EventManager.OnPreRenderObject_Event = (TMPro_EventManager.OnPreRenderObject_Event_Handler)Delegate.Combine(TMPro_EventManager.OnPreRenderObject_Event, value);
			}
			[MethodImpl(32)]
			remove
			{
				TMPro_EventManager.OnPreRenderObject_Event = (TMPro_EventManager.OnPreRenderObject_Event_Handler)Delegate.Remove(TMPro_EventManager.OnPreRenderObject_Event, value);
			}
		}

		public static void ON_PRE_RENDER_OBJECT_CHANGED()
		{
			if (TMPro_EventManager.OnPreRenderObject_Event != null)
			{
				TMPro_EventManager.OnPreRenderObject_Event();
			}
		}

		public static void ON_MATERIAL_PROPERTY_CHANGED(bool isChanged, Material mat)
		{
			if (TMPro_EventManager.MATERIAL_PROPERTY_EVENT != null)
			{
				TMPro_EventManager.MATERIAL_PROPERTY_EVENT(isChanged, mat);
			}
		}

		public static void ON_FONT_PROPERTY_CHANGED(bool isChanged, TextMeshProFont font)
		{
			if (TMPro_EventManager.FONT_PROPERTY_EVENT != null)
			{
				TMPro_EventManager.FONT_PROPERTY_EVENT(isChanged, font);
			}
		}

		public static void ON_SPRITE_ASSET_PROPERTY_CHANGED(bool isChanged, Object obj)
		{
			if (TMPro_EventManager.SPRITE_ASSET_PROPERTY_EVENT != null)
			{
				TMPro_EventManager.SPRITE_ASSET_PROPERTY_EVENT(isChanged, obj);
			}
		}

		public static void ON_TEXTMESHPRO_PROPERTY_CHANGED(bool isChanged, TextMeshPro obj)
		{
			if (TMPro_EventManager.TEXTMESHPRO_PROPERTY_EVENT != null)
			{
				TMPro_EventManager.TEXTMESHPRO_PROPERTY_EVENT(isChanged, obj);
			}
		}

		public static void ON_DRAG_AND_DROP_MATERIAL_CHANGED(GameObject sender, Material currentMaterial, Material newMaterial)
		{
			if (TMPro_EventManager.DRAG_AND_DROP_MATERIAL_EVENT != null)
			{
				TMPro_EventManager.DRAG_AND_DROP_MATERIAL_EVENT(sender, currentMaterial, newMaterial);
			}
		}

		public static void ON_TEXTMESHPRO_UGUI_PROPERTY_CHANGED(bool isChanged, TextMeshProUGUI obj)
		{
			if (TMPro_EventManager.TEXTMESHPRO_UGUI_PROPERTY_EVENT != null)
			{
				TMPro_EventManager.TEXTMESHPRO_UGUI_PROPERTY_EVENT(isChanged, obj);
			}
		}

		public static void ON_BASE_MATERIAL_CHANGED(Material mat)
		{
			if (TMPro_EventManager.BASE_MATERIAL_EVENT != null)
			{
				TMPro_EventManager.BASE_MATERIAL_EVENT(mat);
			}
		}

		public static void ON_COMPUTE_DT_EVENT(object Sender, Compute_DT_EventArgs e)
		{
			if (TMPro_EventManager.COMPUTE_DT_EVENT != null)
			{
				TMPro_EventManager.COMPUTE_DT_EVENT(Sender, e);
			}
		}
	}
}
