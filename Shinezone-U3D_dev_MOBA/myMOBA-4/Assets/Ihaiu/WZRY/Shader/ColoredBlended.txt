Shader "Lines/Colored Blended" {
SubShader { 
 Pass {
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
  }
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  Blend SrcAlpha OneMinusSrcAlpha
 }
}
}