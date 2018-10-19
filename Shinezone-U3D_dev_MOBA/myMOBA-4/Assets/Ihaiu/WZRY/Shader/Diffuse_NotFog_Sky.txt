Shader "S_Game_Scene/Diffuse_NotFog_Sky" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Geometry+100" }
 Pass {
  Tags { "QUEUE"="Geometry+100" }
  Fog { Mode Off }
  SetTexture [_MainTex] { combine texture, texture alpha }
 }
}
}