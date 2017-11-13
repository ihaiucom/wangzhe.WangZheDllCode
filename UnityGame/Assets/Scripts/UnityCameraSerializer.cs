using System;
using UnityEngine;

[ComponentTypeSerializer(typeof(Camera))]
public class UnityCameraSerializer : ICustomizedComponentSerializer
{
	private const string XML_ATTR_CLEAR_FLAGS = "clear_flags";

	private const string XML_ATTR_CULLING_MASK = "cullingMask";

	private const string XML_ATTR_FOV = "fieldOfView";

	private const string XML_ATTR_NEAR_PLANE = "nearClipPlane";

	private const string XML_ATTR_FAR_PLANE = "farClipPlane";

	private const string XML_ATTR_PIXEL_RECT = "pixelRect";

	private const string XML_ATTR_RECT = "rect";

	private const string XML_ATTR_DEPTH = "depth";

	private const string XML_ATTR_RENDER_PATH = "renderingPath";

	private const string XML_ATTR_OC = "useOcclusionCulling";

	private const string XML_ATTR_HDR = "hdr";

	public bool IsComponentSame(Component cmp, Component cmpPrefab)
	{
		Camera camera = (Camera)cmp;
		Camera camera2 = (Camera)cmpPrefab;
		return camera.clearFlags == camera2.clearFlags && camera.cullingMask == camera2.cullingMask && camera.fieldOfView == camera2.fieldOfView && camera.nearClipPlane == camera2.nearClipPlane && camera.farClipPlane == camera2.farClipPlane && camera.pixelRect == camera2.pixelRect && camera.rect == camera2.rect && camera.depth == camera2.depth && camera.renderingPath == camera2.renderingPath && camera.useOcclusionCulling == camera2.useOcclusionCulling && camera.hdr == camera2.hdr;
	}

	public void ComponentDeserialize(Component cmp, BinaryNode node)
	{
		Camera camera = (Camera)cmp;
		camera.clearFlags = (CameraClearFlags)int.Parse(GameSerializer.GetNodeAttr(node, "clear_flags"));
		camera.cullingMask = int.Parse(GameSerializer.GetNodeAttr(node, "cullingMask"));
		camera.fieldOfView = float.Parse(GameSerializer.GetNodeAttr(node, "fieldOfView"));
		camera.nearClipPlane = float.Parse(GameSerializer.GetNodeAttr(node, "nearClipPlane"));
		camera.farClipPlane = float.Parse(GameSerializer.GetNodeAttr(node, "farClipPlane"));
		camera.pixelRect = UnityBasetypeSerializer.StringToRect(GameSerializer.GetNodeAttr(node, "pixelRect"));
		camera.rect = UnityBasetypeSerializer.StringToRect(GameSerializer.GetNodeAttr(node, "rect"));
		camera.depth = float.Parse(GameSerializer.GetNodeAttr(node, "depth"));
		camera.renderingPath = (RenderingPath)int.Parse(GameSerializer.GetNodeAttr(node, "renderingPath"));
		camera.useOcclusionCulling = bool.Parse(GameSerializer.GetNodeAttr(node, "useOcclusionCulling"));
		camera.hdr = bool.Parse(GameSerializer.GetNodeAttr(node, "hdr"));
	}
}
