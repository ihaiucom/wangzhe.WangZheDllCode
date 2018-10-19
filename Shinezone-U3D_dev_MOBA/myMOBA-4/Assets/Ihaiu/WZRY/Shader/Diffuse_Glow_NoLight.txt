Shader "S_Game_Scene/Diffuse_Glow_NoLight" {
Properties {
 _MainTex ("Diffuse(RGB)", 2D) = "white" {}
 _MainTex1 ("Mask(RGB)", 2D) = "white" {}
 _MainTex2 ("Noise(RGB)", 2D) = "white" {}
 _Scroll2X ("Tex2 speed X", Float) = 1
 _Scroll2Y ("Tex2 speed Y", Float) = 0
 _Color ("Color", Color) = (1,1,1,1)
 _MMultiplier ("Layer Multiplier", Float) = 2
}
SubShader { 
 LOD 200
 Tags { "RenderType"="Opaque" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="Opaque" }
Program "vp" {
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD5;
void main ()
{
  highp vec3 shlight_1;
  mediump vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  mediump vec4 tmpvar_6;
  mediump vec4 tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = (_MMultiplier * _Color);
  tmpvar_6 = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9.x = _Scroll2X;
  tmpvar_9.y = _Scroll2Y;
  highp vec2 tmpvar_10;
  tmpvar_10 = fract((tmpvar_9 * _Time.x));
  tmpvar_7.zw = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.xy = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_2.zw = tmpvar_12;
  highp vec2 tmpvar_13;
  tmpvar_13 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_3 = tmpvar_13;
  highp mat3 tmpvar_14;
  tmpvar_14[0] = _Object2World[0].xyz;
  tmpvar_14[1] = _Object2World[1].xyz;
  tmpvar_14[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_14 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_4 = tmpvar_15;
  highp vec4 tmpvar_16;
  tmpvar_16.w = 1.0;
  tmpvar_16.xyz = tmpvar_15;
  mediump vec3 tmpvar_17;
  mediump vec4 normal_18;
  normal_18 = tmpvar_16;
  highp float vC_19;
  mediump vec3 x3_20;
  mediump vec3 x2_21;
  mediump vec3 x1_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (unity_SHAr, normal_18);
  x1_22.x = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (unity_SHAg, normal_18);
  x1_22.y = tmpvar_24;
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHAb, normal_18);
  x1_22.z = tmpvar_25;
  mediump vec4 tmpvar_26;
  tmpvar_26 = (normal_18.xyzz * normal_18.yzzx);
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHBr, tmpvar_26);
  x2_21.x = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHBg, tmpvar_26);
  x2_21.y = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = dot (unity_SHBb, tmpvar_26);
  x2_21.z = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = ((normal_18.x * normal_18.x) - (normal_18.y * normal_18.y));
  vC_19 = tmpvar_30;
  highp vec3 tmpvar_31;
  tmpvar_31 = (unity_SHC.xyz * vC_19);
  x3_20 = tmpvar_31;
  tmpvar_17 = ((x1_22 + x2_21) + x3_20);
  shlight_1 = tmpvar_17;
  tmpvar_5 = shlight_1;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_6;
  xlv_TEXCOORD3 = tmpvar_7;
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = tmpvar_5;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture2D (_MainTex1, xlv_TEXCOORD0.zw) * texture2D (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz);
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
out mediump vec4 xlv_TEXCOORD2;
out mediump vec4 xlv_TEXCOORD3;
out lowp vec3 xlv_TEXCOORD4;
out lowp vec3 xlv_TEXCOORD5;
void main ()
{
  highp vec3 shlight_1;
  mediump vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  mediump vec4 tmpvar_6;
  mediump vec4 tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = (_MMultiplier * _Color);
  tmpvar_6 = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9.x = _Scroll2X;
  tmpvar_9.y = _Scroll2Y;
  highp vec2 tmpvar_10;
  tmpvar_10 = fract((tmpvar_9 * _Time.x));
  tmpvar_7.zw = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.xy = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_2.zw = tmpvar_12;
  highp vec2 tmpvar_13;
  tmpvar_13 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_3 = tmpvar_13;
  highp mat3 tmpvar_14;
  tmpvar_14[0] = _Object2World[0].xyz;
  tmpvar_14[1] = _Object2World[1].xyz;
  tmpvar_14[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_14 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_4 = tmpvar_15;
  highp vec4 tmpvar_16;
  tmpvar_16.w = 1.0;
  tmpvar_16.xyz = tmpvar_15;
  mediump vec3 tmpvar_17;
  mediump vec4 normal_18;
  normal_18 = tmpvar_16;
  highp float vC_19;
  mediump vec3 x3_20;
  mediump vec3 x2_21;
  mediump vec3 x1_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (unity_SHAr, normal_18);
  x1_22.x = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (unity_SHAg, normal_18);
  x1_22.y = tmpvar_24;
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHAb, normal_18);
  x1_22.z = tmpvar_25;
  mediump vec4 tmpvar_26;
  tmpvar_26 = (normal_18.xyzz * normal_18.yzzx);
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHBr, tmpvar_26);
  x2_21.x = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHBg, tmpvar_26);
  x2_21.y = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = dot (unity_SHBb, tmpvar_26);
  x2_21.z = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = ((normal_18.x * normal_18.x) - (normal_18.y * normal_18.y));
  vC_19 = tmpvar_30;
  highp vec3 tmpvar_31;
  tmpvar_31 = (unity_SHC.xyz * vC_19);
  x3_20 = tmpvar_31;
  tmpvar_17 = ((x1_22 + x2_21) + x3_20);
  shlight_1 = tmpvar_17;
  tmpvar_5 = shlight_1;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_6;
  xlv_TEXCOORD3 = tmpvar_7;
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = tmpvar_5;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
in mediump vec4 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in mediump vec4 xlv_TEXCOORD2;
in mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture (_MainTex1, xlv_TEXCOORD0.zw) * texture (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture (_MainTex, xlv_TEXCOORD0.xy).xyz);
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp mat4 glstate_matrix_mvp;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying highp vec2 xlv_TEXCOORD4;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture2D (_MainTex1, xlv_TEXCOORD0.zw) * texture2D (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz);
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
in vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp mat4 glstate_matrix_mvp;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
out mediump vec4 xlv_TEXCOORD2;
out mediump vec4 xlv_TEXCOORD3;
out highp vec2 xlv_TEXCOORD4;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
in mediump vec4 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in mediump vec4 xlv_TEXCOORD2;
in mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture (_MainTex1, xlv_TEXCOORD0.zw) * texture (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture (_MainTex, xlv_TEXCOORD0.xy).xyz);
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp mat4 glstate_matrix_mvp;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying highp vec2 xlv_TEXCOORD4;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture2D (_MainTex1, xlv_TEXCOORD0.zw) * texture2D (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz);
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
in vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp mat4 glstate_matrix_mvp;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
out mediump vec4 xlv_TEXCOORD2;
out mediump vec4 xlv_TEXCOORD3;
out highp vec2 xlv_TEXCOORD4;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
in mediump vec4 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in mediump vec4 xlv_TEXCOORD2;
in mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture (_MainTex1, xlv_TEXCOORD0.zw) * texture (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture (_MainTex, xlv_TEXCOORD0.xy).xyz);
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp mat4 unity_World2Shadow[4];
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD5;
varying highp vec4 xlv_TEXCOORD6;
void main ()
{
  highp vec3 shlight_1;
  mediump vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  mediump vec4 tmpvar_6;
  mediump vec4 tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = (_MMultiplier * _Color);
  tmpvar_6 = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9.x = _Scroll2X;
  tmpvar_9.y = _Scroll2Y;
  highp vec2 tmpvar_10;
  tmpvar_10 = fract((tmpvar_9 * _Time.x));
  tmpvar_7.zw = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.xy = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_2.zw = tmpvar_12;
  highp vec2 tmpvar_13;
  tmpvar_13 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_3 = tmpvar_13;
  highp mat3 tmpvar_14;
  tmpvar_14[0] = _Object2World[0].xyz;
  tmpvar_14[1] = _Object2World[1].xyz;
  tmpvar_14[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_14 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_4 = tmpvar_15;
  highp vec4 tmpvar_16;
  tmpvar_16.w = 1.0;
  tmpvar_16.xyz = tmpvar_15;
  mediump vec3 tmpvar_17;
  mediump vec4 normal_18;
  normal_18 = tmpvar_16;
  highp float vC_19;
  mediump vec3 x3_20;
  mediump vec3 x2_21;
  mediump vec3 x1_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (unity_SHAr, normal_18);
  x1_22.x = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (unity_SHAg, normal_18);
  x1_22.y = tmpvar_24;
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHAb, normal_18);
  x1_22.z = tmpvar_25;
  mediump vec4 tmpvar_26;
  tmpvar_26 = (normal_18.xyzz * normal_18.yzzx);
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHBr, tmpvar_26);
  x2_21.x = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHBg, tmpvar_26);
  x2_21.y = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = dot (unity_SHBb, tmpvar_26);
  x2_21.z = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = ((normal_18.x * normal_18.x) - (normal_18.y * normal_18.y));
  vC_19 = tmpvar_30;
  highp vec3 tmpvar_31;
  tmpvar_31 = (unity_SHC.xyz * vC_19);
  x3_20 = tmpvar_31;
  tmpvar_17 = ((x1_22 + x2_21) + x3_20);
  shlight_1 = tmpvar_17;
  tmpvar_5 = shlight_1;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_6;
  xlv_TEXCOORD3 = tmpvar_7;
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = tmpvar_5;
  xlv_TEXCOORD6 = (unity_World2Shadow[0] * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture2D (_MainTex1, xlv_TEXCOORD0.zw) * texture2D (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz);
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp mat4 unity_World2Shadow[4];
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying highp vec2 xlv_TEXCOORD4;
varying highp vec4 xlv_TEXCOORD5;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD5 = (unity_World2Shadow[0] * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture2D (_MainTex1, xlv_TEXCOORD0.zw) * texture2D (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz);
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp mat4 unity_World2Shadow[4];
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying highp vec2 xlv_TEXCOORD4;
varying highp vec4 xlv_TEXCOORD5;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD5 = (unity_World2Shadow[0] * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture2D (_MainTex1, xlv_TEXCOORD0.zw) * texture2D (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz);
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[8];
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD5;
void main ()
{
  highp vec3 shlight_1;
  mediump vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  mediump vec4 tmpvar_6;
  mediump vec4 tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = (_MMultiplier * _Color);
  tmpvar_6 = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9.x = _Scroll2X;
  tmpvar_9.y = _Scroll2Y;
  highp vec2 tmpvar_10;
  tmpvar_10 = fract((tmpvar_9 * _Time.x));
  tmpvar_7.zw = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.xy = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_2.zw = tmpvar_12;
  highp vec2 tmpvar_13;
  tmpvar_13 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_3 = tmpvar_13;
  highp mat3 tmpvar_14;
  tmpvar_14[0] = _Object2World[0].xyz;
  tmpvar_14[1] = _Object2World[1].xyz;
  tmpvar_14[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_14 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_4 = tmpvar_15;
  highp vec4 tmpvar_16;
  tmpvar_16.w = 1.0;
  tmpvar_16.xyz = tmpvar_15;
  mediump vec3 tmpvar_17;
  mediump vec4 normal_18;
  normal_18 = tmpvar_16;
  highp float vC_19;
  mediump vec3 x3_20;
  mediump vec3 x2_21;
  mediump vec3 x1_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (unity_SHAr, normal_18);
  x1_22.x = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (unity_SHAg, normal_18);
  x1_22.y = tmpvar_24;
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHAb, normal_18);
  x1_22.z = tmpvar_25;
  mediump vec4 tmpvar_26;
  tmpvar_26 = (normal_18.xyzz * normal_18.yzzx);
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHBr, tmpvar_26);
  x2_21.x = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHBg, tmpvar_26);
  x2_21.y = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = dot (unity_SHBb, tmpvar_26);
  x2_21.z = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = ((normal_18.x * normal_18.x) - (normal_18.y * normal_18.y));
  vC_19 = tmpvar_30;
  highp vec3 tmpvar_31;
  tmpvar_31 = (unity_SHC.xyz * vC_19);
  x3_20 = tmpvar_31;
  tmpvar_17 = ((x1_22 + x2_21) + x3_20);
  shlight_1 = tmpvar_17;
  tmpvar_5 = shlight_1;
  highp vec3 tmpvar_32;
  tmpvar_32 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_33;
  tmpvar_33 = (unity_4LightPosX0 - tmpvar_32.x);
  highp vec4 tmpvar_34;
  tmpvar_34 = (unity_4LightPosY0 - tmpvar_32.y);
  highp vec4 tmpvar_35;
  tmpvar_35 = (unity_4LightPosZ0 - tmpvar_32.z);
  highp vec4 tmpvar_36;
  tmpvar_36 = (((tmpvar_33 * tmpvar_33) + (tmpvar_34 * tmpvar_34)) + (tmpvar_35 * tmpvar_35));
  highp vec4 tmpvar_37;
  tmpvar_37 = (max (vec4(0.0, 0.0, 0.0, 0.0), (
    (((tmpvar_33 * tmpvar_15.x) + (tmpvar_34 * tmpvar_15.y)) + (tmpvar_35 * tmpvar_15.z))
   * 
    inversesqrt(tmpvar_36)
  )) * (1.0/((1.0 + 
    (tmpvar_36 * unity_4LightAtten0)
  ))));
  highp vec3 tmpvar_38;
  tmpvar_38 = (tmpvar_5 + ((
    ((unity_LightColor[0].xyz * tmpvar_37.x) + (unity_LightColor[1].xyz * tmpvar_37.y))
   + 
    (unity_LightColor[2].xyz * tmpvar_37.z)
  ) + (unity_LightColor[3].xyz * tmpvar_37.w)));
  tmpvar_5 = tmpvar_38;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_6;
  xlv_TEXCOORD3 = tmpvar_7;
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = tmpvar_5;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture2D (_MainTex1, xlv_TEXCOORD0.zw) * texture2D (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz);
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[8];
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
out mediump vec4 xlv_TEXCOORD2;
out mediump vec4 xlv_TEXCOORD3;
out lowp vec3 xlv_TEXCOORD4;
out lowp vec3 xlv_TEXCOORD5;
void main ()
{
  highp vec3 shlight_1;
  mediump vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  mediump vec4 tmpvar_6;
  mediump vec4 tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = (_MMultiplier * _Color);
  tmpvar_6 = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9.x = _Scroll2X;
  tmpvar_9.y = _Scroll2Y;
  highp vec2 tmpvar_10;
  tmpvar_10 = fract((tmpvar_9 * _Time.x));
  tmpvar_7.zw = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.xy = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_2.zw = tmpvar_12;
  highp vec2 tmpvar_13;
  tmpvar_13 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_3 = tmpvar_13;
  highp mat3 tmpvar_14;
  tmpvar_14[0] = _Object2World[0].xyz;
  tmpvar_14[1] = _Object2World[1].xyz;
  tmpvar_14[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_14 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_4 = tmpvar_15;
  highp vec4 tmpvar_16;
  tmpvar_16.w = 1.0;
  tmpvar_16.xyz = tmpvar_15;
  mediump vec3 tmpvar_17;
  mediump vec4 normal_18;
  normal_18 = tmpvar_16;
  highp float vC_19;
  mediump vec3 x3_20;
  mediump vec3 x2_21;
  mediump vec3 x1_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (unity_SHAr, normal_18);
  x1_22.x = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (unity_SHAg, normal_18);
  x1_22.y = tmpvar_24;
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHAb, normal_18);
  x1_22.z = tmpvar_25;
  mediump vec4 tmpvar_26;
  tmpvar_26 = (normal_18.xyzz * normal_18.yzzx);
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHBr, tmpvar_26);
  x2_21.x = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHBg, tmpvar_26);
  x2_21.y = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = dot (unity_SHBb, tmpvar_26);
  x2_21.z = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = ((normal_18.x * normal_18.x) - (normal_18.y * normal_18.y));
  vC_19 = tmpvar_30;
  highp vec3 tmpvar_31;
  tmpvar_31 = (unity_SHC.xyz * vC_19);
  x3_20 = tmpvar_31;
  tmpvar_17 = ((x1_22 + x2_21) + x3_20);
  shlight_1 = tmpvar_17;
  tmpvar_5 = shlight_1;
  highp vec3 tmpvar_32;
  tmpvar_32 = (_Object2World * _glesVertex).xyz;
  highp vec4 tmpvar_33;
  tmpvar_33 = (unity_4LightPosX0 - tmpvar_32.x);
  highp vec4 tmpvar_34;
  tmpvar_34 = (unity_4LightPosY0 - tmpvar_32.y);
  highp vec4 tmpvar_35;
  tmpvar_35 = (unity_4LightPosZ0 - tmpvar_32.z);
  highp vec4 tmpvar_36;
  tmpvar_36 = (((tmpvar_33 * tmpvar_33) + (tmpvar_34 * tmpvar_34)) + (tmpvar_35 * tmpvar_35));
  highp vec4 tmpvar_37;
  tmpvar_37 = (max (vec4(0.0, 0.0, 0.0, 0.0), (
    (((tmpvar_33 * tmpvar_15.x) + (tmpvar_34 * tmpvar_15.y)) + (tmpvar_35 * tmpvar_15.z))
   * 
    inversesqrt(tmpvar_36)
  )) * (1.0/((1.0 + 
    (tmpvar_36 * unity_4LightAtten0)
  ))));
  highp vec3 tmpvar_38;
  tmpvar_38 = (tmpvar_5 + ((
    ((unity_LightColor[0].xyz * tmpvar_37.x) + (unity_LightColor[1].xyz * tmpvar_37.y))
   + 
    (unity_LightColor[2].xyz * tmpvar_37.z)
  ) + (unity_LightColor[3].xyz * tmpvar_37.w)));
  tmpvar_5 = tmpvar_38;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_6;
  xlv_TEXCOORD3 = tmpvar_7;
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = tmpvar_5;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
in mediump vec4 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in mediump vec4 xlv_TEXCOORD2;
in mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture (_MainTex1, xlv_TEXCOORD0.zw) * texture (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture (_MainTex, xlv_TEXCOORD0.xy).xyz);
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[8];
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp mat4 unity_World2Shadow[4];
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD5;
varying highp vec4 xlv_TEXCOORD6;
void main ()
{
  highp vec3 shlight_1;
  mediump vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  mediump vec4 tmpvar_6;
  mediump vec4 tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = (_MMultiplier * _Color);
  tmpvar_6 = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9.x = _Scroll2X;
  tmpvar_9.y = _Scroll2Y;
  highp vec2 tmpvar_10;
  tmpvar_10 = fract((tmpvar_9 * _Time.x));
  tmpvar_7.zw = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.xy = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_2.zw = tmpvar_12;
  highp vec2 tmpvar_13;
  tmpvar_13 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_3 = tmpvar_13;
  highp mat3 tmpvar_14;
  tmpvar_14[0] = _Object2World[0].xyz;
  tmpvar_14[1] = _Object2World[1].xyz;
  tmpvar_14[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_14 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_4 = tmpvar_15;
  highp vec4 tmpvar_16;
  tmpvar_16.w = 1.0;
  tmpvar_16.xyz = tmpvar_15;
  mediump vec3 tmpvar_17;
  mediump vec4 normal_18;
  normal_18 = tmpvar_16;
  highp float vC_19;
  mediump vec3 x3_20;
  mediump vec3 x2_21;
  mediump vec3 x1_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (unity_SHAr, normal_18);
  x1_22.x = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (unity_SHAg, normal_18);
  x1_22.y = tmpvar_24;
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHAb, normal_18);
  x1_22.z = tmpvar_25;
  mediump vec4 tmpvar_26;
  tmpvar_26 = (normal_18.xyzz * normal_18.yzzx);
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHBr, tmpvar_26);
  x2_21.x = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHBg, tmpvar_26);
  x2_21.y = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = dot (unity_SHBb, tmpvar_26);
  x2_21.z = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = ((normal_18.x * normal_18.x) - (normal_18.y * normal_18.y));
  vC_19 = tmpvar_30;
  highp vec3 tmpvar_31;
  tmpvar_31 = (unity_SHC.xyz * vC_19);
  x3_20 = tmpvar_31;
  tmpvar_17 = ((x1_22 + x2_21) + x3_20);
  shlight_1 = tmpvar_17;
  tmpvar_5 = shlight_1;
  highp vec4 cse_32;
  cse_32 = (_Object2World * _glesVertex);
  highp vec4 tmpvar_33;
  tmpvar_33 = (unity_4LightPosX0 - cse_32.x);
  highp vec4 tmpvar_34;
  tmpvar_34 = (unity_4LightPosY0 - cse_32.y);
  highp vec4 tmpvar_35;
  tmpvar_35 = (unity_4LightPosZ0 - cse_32.z);
  highp vec4 tmpvar_36;
  tmpvar_36 = (((tmpvar_33 * tmpvar_33) + (tmpvar_34 * tmpvar_34)) + (tmpvar_35 * tmpvar_35));
  highp vec4 tmpvar_37;
  tmpvar_37 = (max (vec4(0.0, 0.0, 0.0, 0.0), (
    (((tmpvar_33 * tmpvar_15.x) + (tmpvar_34 * tmpvar_15.y)) + (tmpvar_35 * tmpvar_15.z))
   * 
    inversesqrt(tmpvar_36)
  )) * (1.0/((1.0 + 
    (tmpvar_36 * unity_4LightAtten0)
  ))));
  highp vec3 tmpvar_38;
  tmpvar_38 = (tmpvar_5 + ((
    ((unity_LightColor[0].xyz * tmpvar_37.x) + (unity_LightColor[1].xyz * tmpvar_37.y))
   + 
    (unity_LightColor[2].xyz * tmpvar_37.z)
  ) + (unity_LightColor[3].xyz * tmpvar_37.w)));
  tmpvar_5 = tmpvar_38;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_6;
  xlv_TEXCOORD3 = tmpvar_7;
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = tmpvar_5;
  xlv_TEXCOORD6 = (unity_World2Shadow[0] * cse_32);
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture2D (_MainTex1, xlv_TEXCOORD0.zw) * texture2D (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz);
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES


#ifdef VERTEX

#extension GL_EXT_shadow_samplers : enable
attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp mat4 unity_World2Shadow[4];
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD5;
varying highp vec4 xlv_TEXCOORD6;
void main ()
{
  highp vec3 shlight_1;
  mediump vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  mediump vec4 tmpvar_6;
  mediump vec4 tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = (_MMultiplier * _Color);
  tmpvar_6 = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9.x = _Scroll2X;
  tmpvar_9.y = _Scroll2Y;
  highp vec2 tmpvar_10;
  tmpvar_10 = fract((tmpvar_9 * _Time.x));
  tmpvar_7.zw = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.xy = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_2.zw = tmpvar_12;
  highp vec2 tmpvar_13;
  tmpvar_13 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_3 = tmpvar_13;
  highp mat3 tmpvar_14;
  tmpvar_14[0] = _Object2World[0].xyz;
  tmpvar_14[1] = _Object2World[1].xyz;
  tmpvar_14[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_14 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_4 = tmpvar_15;
  highp vec4 tmpvar_16;
  tmpvar_16.w = 1.0;
  tmpvar_16.xyz = tmpvar_15;
  mediump vec3 tmpvar_17;
  mediump vec4 normal_18;
  normal_18 = tmpvar_16;
  highp float vC_19;
  mediump vec3 x3_20;
  mediump vec3 x2_21;
  mediump vec3 x1_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (unity_SHAr, normal_18);
  x1_22.x = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (unity_SHAg, normal_18);
  x1_22.y = tmpvar_24;
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHAb, normal_18);
  x1_22.z = tmpvar_25;
  mediump vec4 tmpvar_26;
  tmpvar_26 = (normal_18.xyzz * normal_18.yzzx);
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHBr, tmpvar_26);
  x2_21.x = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHBg, tmpvar_26);
  x2_21.y = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = dot (unity_SHBb, tmpvar_26);
  x2_21.z = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = ((normal_18.x * normal_18.x) - (normal_18.y * normal_18.y));
  vC_19 = tmpvar_30;
  highp vec3 tmpvar_31;
  tmpvar_31 = (unity_SHC.xyz * vC_19);
  x3_20 = tmpvar_31;
  tmpvar_17 = ((x1_22 + x2_21) + x3_20);
  shlight_1 = tmpvar_17;
  tmpvar_5 = shlight_1;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_6;
  xlv_TEXCOORD3 = tmpvar_7;
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = tmpvar_5;
  xlv_TEXCOORD6 = (unity_World2Shadow[0] * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT

#extension GL_EXT_shadow_samplers : enable
uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture2D (_MainTex1, xlv_TEXCOORD0.zw) * texture2D (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz);
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp mat4 unity_World2Shadow[4];
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
out mediump vec4 xlv_TEXCOORD2;
out mediump vec4 xlv_TEXCOORD3;
out lowp vec3 xlv_TEXCOORD4;
out lowp vec3 xlv_TEXCOORD5;
out highp vec4 xlv_TEXCOORD6;
void main ()
{
  highp vec3 shlight_1;
  mediump vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  mediump vec4 tmpvar_6;
  mediump vec4 tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = (_MMultiplier * _Color);
  tmpvar_6 = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9.x = _Scroll2X;
  tmpvar_9.y = _Scroll2Y;
  highp vec2 tmpvar_10;
  tmpvar_10 = fract((tmpvar_9 * _Time.x));
  tmpvar_7.zw = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.xy = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_2.zw = tmpvar_12;
  highp vec2 tmpvar_13;
  tmpvar_13 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_3 = tmpvar_13;
  highp mat3 tmpvar_14;
  tmpvar_14[0] = _Object2World[0].xyz;
  tmpvar_14[1] = _Object2World[1].xyz;
  tmpvar_14[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_14 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_4 = tmpvar_15;
  highp vec4 tmpvar_16;
  tmpvar_16.w = 1.0;
  tmpvar_16.xyz = tmpvar_15;
  mediump vec3 tmpvar_17;
  mediump vec4 normal_18;
  normal_18 = tmpvar_16;
  highp float vC_19;
  mediump vec3 x3_20;
  mediump vec3 x2_21;
  mediump vec3 x1_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (unity_SHAr, normal_18);
  x1_22.x = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (unity_SHAg, normal_18);
  x1_22.y = tmpvar_24;
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHAb, normal_18);
  x1_22.z = tmpvar_25;
  mediump vec4 tmpvar_26;
  tmpvar_26 = (normal_18.xyzz * normal_18.yzzx);
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHBr, tmpvar_26);
  x2_21.x = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHBg, tmpvar_26);
  x2_21.y = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = dot (unity_SHBb, tmpvar_26);
  x2_21.z = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = ((normal_18.x * normal_18.x) - (normal_18.y * normal_18.y));
  vC_19 = tmpvar_30;
  highp vec3 tmpvar_31;
  tmpvar_31 = (unity_SHC.xyz * vC_19);
  x3_20 = tmpvar_31;
  tmpvar_17 = ((x1_22 + x2_21) + x3_20);
  shlight_1 = tmpvar_17;
  tmpvar_5 = shlight_1;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_6;
  xlv_TEXCOORD3 = tmpvar_7;
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = tmpvar_5;
  xlv_TEXCOORD6 = (unity_World2Shadow[0] * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
in mediump vec4 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in mediump vec4 xlv_TEXCOORD2;
in mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture (_MainTex1, xlv_TEXCOORD0.zw) * texture (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture (_MainTex, xlv_TEXCOORD0.xy).xyz);
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES


#ifdef VERTEX

#extension GL_EXT_shadow_samplers : enable
attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp mat4 unity_World2Shadow[4];
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying highp vec2 xlv_TEXCOORD4;
varying highp vec4 xlv_TEXCOORD5;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD5 = (unity_World2Shadow[0] * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT

#extension GL_EXT_shadow_samplers : enable
uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture2D (_MainTex1, xlv_TEXCOORD0.zw) * texture2D (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz);
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
in vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp mat4 unity_World2Shadow[4];
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
out mediump vec4 xlv_TEXCOORD2;
out mediump vec4 xlv_TEXCOORD3;
out highp vec2 xlv_TEXCOORD4;
out highp vec4 xlv_TEXCOORD5;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD5 = (unity_World2Shadow[0] * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
in mediump vec4 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in mediump vec4 xlv_TEXCOORD2;
in mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture (_MainTex1, xlv_TEXCOORD0.zw) * texture (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture (_MainTex, xlv_TEXCOORD0.xy).xyz);
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES


#ifdef VERTEX

#extension GL_EXT_shadow_samplers : enable
attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp mat4 unity_World2Shadow[4];
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying highp vec2 xlv_TEXCOORD4;
varying highp vec4 xlv_TEXCOORD5;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD5 = (unity_World2Shadow[0] * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT

#extension GL_EXT_shadow_samplers : enable
uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture2D (_MainTex1, xlv_TEXCOORD0.zw) * texture2D (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz);
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
in vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp mat4 unity_World2Shadow[4];
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
out mediump vec4 xlv_TEXCOORD2;
out mediump vec4 xlv_TEXCOORD3;
out highp vec2 xlv_TEXCOORD4;
out highp vec4 xlv_TEXCOORD5;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD5 = (unity_World2Shadow[0] * (_Object2World * _glesVertex));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
in mediump vec4 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in mediump vec4 xlv_TEXCOORD2;
in mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture (_MainTex1, xlv_TEXCOORD0.zw) * texture (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture (_MainTex, xlv_TEXCOORD0.xy).xyz);
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
"!!GLES


#ifdef VERTEX

#extension GL_EXT_shadow_samplers : enable
attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[8];
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp mat4 unity_World2Shadow[4];
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying lowp vec3 xlv_TEXCOORD4;
varying lowp vec3 xlv_TEXCOORD5;
varying highp vec4 xlv_TEXCOORD6;
void main ()
{
  highp vec3 shlight_1;
  mediump vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  mediump vec4 tmpvar_6;
  mediump vec4 tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = (_MMultiplier * _Color);
  tmpvar_6 = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9.x = _Scroll2X;
  tmpvar_9.y = _Scroll2Y;
  highp vec2 tmpvar_10;
  tmpvar_10 = fract((tmpvar_9 * _Time.x));
  tmpvar_7.zw = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.xy = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_2.zw = tmpvar_12;
  highp vec2 tmpvar_13;
  tmpvar_13 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_3 = tmpvar_13;
  highp mat3 tmpvar_14;
  tmpvar_14[0] = _Object2World[0].xyz;
  tmpvar_14[1] = _Object2World[1].xyz;
  tmpvar_14[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_14 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_4 = tmpvar_15;
  highp vec4 tmpvar_16;
  tmpvar_16.w = 1.0;
  tmpvar_16.xyz = tmpvar_15;
  mediump vec3 tmpvar_17;
  mediump vec4 normal_18;
  normal_18 = tmpvar_16;
  highp float vC_19;
  mediump vec3 x3_20;
  mediump vec3 x2_21;
  mediump vec3 x1_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (unity_SHAr, normal_18);
  x1_22.x = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (unity_SHAg, normal_18);
  x1_22.y = tmpvar_24;
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHAb, normal_18);
  x1_22.z = tmpvar_25;
  mediump vec4 tmpvar_26;
  tmpvar_26 = (normal_18.xyzz * normal_18.yzzx);
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHBr, tmpvar_26);
  x2_21.x = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHBg, tmpvar_26);
  x2_21.y = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = dot (unity_SHBb, tmpvar_26);
  x2_21.z = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = ((normal_18.x * normal_18.x) - (normal_18.y * normal_18.y));
  vC_19 = tmpvar_30;
  highp vec3 tmpvar_31;
  tmpvar_31 = (unity_SHC.xyz * vC_19);
  x3_20 = tmpvar_31;
  tmpvar_17 = ((x1_22 + x2_21) + x3_20);
  shlight_1 = tmpvar_17;
  tmpvar_5 = shlight_1;
  highp vec4 cse_32;
  cse_32 = (_Object2World * _glesVertex);
  highp vec4 tmpvar_33;
  tmpvar_33 = (unity_4LightPosX0 - cse_32.x);
  highp vec4 tmpvar_34;
  tmpvar_34 = (unity_4LightPosY0 - cse_32.y);
  highp vec4 tmpvar_35;
  tmpvar_35 = (unity_4LightPosZ0 - cse_32.z);
  highp vec4 tmpvar_36;
  tmpvar_36 = (((tmpvar_33 * tmpvar_33) + (tmpvar_34 * tmpvar_34)) + (tmpvar_35 * tmpvar_35));
  highp vec4 tmpvar_37;
  tmpvar_37 = (max (vec4(0.0, 0.0, 0.0, 0.0), (
    (((tmpvar_33 * tmpvar_15.x) + (tmpvar_34 * tmpvar_15.y)) + (tmpvar_35 * tmpvar_15.z))
   * 
    inversesqrt(tmpvar_36)
  )) * (1.0/((1.0 + 
    (tmpvar_36 * unity_4LightAtten0)
  ))));
  highp vec3 tmpvar_38;
  tmpvar_38 = (tmpvar_5 + ((
    ((unity_LightColor[0].xyz * tmpvar_37.x) + (unity_LightColor[1].xyz * tmpvar_37.y))
   + 
    (unity_LightColor[2].xyz * tmpvar_37.z)
  ) + (unity_LightColor[3].xyz * tmpvar_37.w)));
  tmpvar_5 = tmpvar_38;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_6;
  xlv_TEXCOORD3 = tmpvar_7;
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = tmpvar_5;
  xlv_TEXCOORD6 = (unity_World2Shadow[0] * cse_32);
}



#endif
#ifdef FRAGMENT

#extension GL_EXT_shadow_samplers : enable
uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture2D (_MainTex1, xlv_TEXCOORD0.zw) * texture2D (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz);
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp vec4 unity_4LightPosX0;
uniform highp vec4 unity_4LightPosY0;
uniform highp vec4 unity_4LightPosZ0;
uniform highp vec4 unity_4LightAtten0;
uniform highp vec4 unity_LightColor[8];
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp mat4 unity_World2Shadow[4];
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
out mediump vec4 xlv_TEXCOORD2;
out mediump vec4 xlv_TEXCOORD3;
out lowp vec3 xlv_TEXCOORD4;
out lowp vec3 xlv_TEXCOORD5;
out highp vec4 xlv_TEXCOORD6;
void main ()
{
  highp vec3 shlight_1;
  mediump vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  lowp vec3 tmpvar_4;
  lowp vec3 tmpvar_5;
  mediump vec4 tmpvar_6;
  mediump vec4 tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = (_MMultiplier * _Color);
  tmpvar_6 = tmpvar_8;
  highp vec2 tmpvar_9;
  tmpvar_9.x = _Scroll2X;
  tmpvar_9.y = _Scroll2Y;
  highp vec2 tmpvar_10;
  tmpvar_10 = fract((tmpvar_9 * _Time.x));
  tmpvar_7.zw = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_2.xy = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_2.zw = tmpvar_12;
  highp vec2 tmpvar_13;
  tmpvar_13 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_3 = tmpvar_13;
  highp mat3 tmpvar_14;
  tmpvar_14[0] = _Object2World[0].xyz;
  tmpvar_14[1] = _Object2World[1].xyz;
  tmpvar_14[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_14 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_4 = tmpvar_15;
  highp vec4 tmpvar_16;
  tmpvar_16.w = 1.0;
  tmpvar_16.xyz = tmpvar_15;
  mediump vec3 tmpvar_17;
  mediump vec4 normal_18;
  normal_18 = tmpvar_16;
  highp float vC_19;
  mediump vec3 x3_20;
  mediump vec3 x2_21;
  mediump vec3 x1_22;
  highp float tmpvar_23;
  tmpvar_23 = dot (unity_SHAr, normal_18);
  x1_22.x = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (unity_SHAg, normal_18);
  x1_22.y = tmpvar_24;
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHAb, normal_18);
  x1_22.z = tmpvar_25;
  mediump vec4 tmpvar_26;
  tmpvar_26 = (normal_18.xyzz * normal_18.yzzx);
  highp float tmpvar_27;
  tmpvar_27 = dot (unity_SHBr, tmpvar_26);
  x2_21.x = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHBg, tmpvar_26);
  x2_21.y = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = dot (unity_SHBb, tmpvar_26);
  x2_21.z = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = ((normal_18.x * normal_18.x) - (normal_18.y * normal_18.y));
  vC_19 = tmpvar_30;
  highp vec3 tmpvar_31;
  tmpvar_31 = (unity_SHC.xyz * vC_19);
  x3_20 = tmpvar_31;
  tmpvar_17 = ((x1_22 + x2_21) + x3_20);
  shlight_1 = tmpvar_17;
  tmpvar_5 = shlight_1;
  highp vec4 cse_32;
  cse_32 = (_Object2World * _glesVertex);
  highp vec4 tmpvar_33;
  tmpvar_33 = (unity_4LightPosX0 - cse_32.x);
  highp vec4 tmpvar_34;
  tmpvar_34 = (unity_4LightPosY0 - cse_32.y);
  highp vec4 tmpvar_35;
  tmpvar_35 = (unity_4LightPosZ0 - cse_32.z);
  highp vec4 tmpvar_36;
  tmpvar_36 = (((tmpvar_33 * tmpvar_33) + (tmpvar_34 * tmpvar_34)) + (tmpvar_35 * tmpvar_35));
  highp vec4 tmpvar_37;
  tmpvar_37 = (max (vec4(0.0, 0.0, 0.0, 0.0), (
    (((tmpvar_33 * tmpvar_15.x) + (tmpvar_34 * tmpvar_15.y)) + (tmpvar_35 * tmpvar_15.z))
   * 
    inversesqrt(tmpvar_36)
  )) * (1.0/((1.0 + 
    (tmpvar_36 * unity_4LightAtten0)
  ))));
  highp vec3 tmpvar_38;
  tmpvar_38 = (tmpvar_5 + ((
    ((unity_LightColor[0].xyz * tmpvar_37.x) + (unity_LightColor[1].xyz * tmpvar_37.y))
   + 
    (unity_LightColor[2].xyz * tmpvar_37.z)
  ) + (unity_LightColor[3].xyz * tmpvar_37.w)));
  tmpvar_5 = tmpvar_38;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_6;
  xlv_TEXCOORD3 = tmpvar_7;
  xlv_TEXCOORD4 = tmpvar_4;
  xlv_TEXCOORD5 = tmpvar_5;
  xlv_TEXCOORD6 = (unity_World2Shadow[0] * cse_32);
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
in mediump vec4 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in mediump vec4 xlv_TEXCOORD2;
in mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 c_1;
  lowp vec4 c_2;
  mediump vec2 P_3;
  P_3 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_4;
  tmpvar_4 = (texture (_MainTex1, xlv_TEXCOORD0.zw) * texture (_MainTex2, P_3));
  c_2.w = tmpvar_4.w;
  mediump vec3 tmpvar_5;
  tmpvar_5 = (tmpvar_4.xyz * xlv_TEXCOORD2.xyz);
  c_2.xyz = tmpvar_5;
  c_1.w = 1.0;
  c_1.xyz = (c_2.xyz + texture (_MainTex, xlv_TEXCOORD0.xy).xyz);
  _glesFragData[0] = c_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_ON" "DIRLIGHTMAP_ON" }
"!!GLES3"
}
}
 }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardAdd" "RenderType"="Opaque" }
  ZWrite Off
  Fog {
   Color (0,0,0,0)
  }
  Blend One One
Program "vp" {
SubProgram "gles " {
Keywords { "POINT" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _Time;
uniform highp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp mat4 _LightMatrix0;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec4 xlv_TEXCOORD1;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD4;
void main ()
{
  lowp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_1 = tmpvar_9;
  highp vec3 tmpvar_10;
  highp vec4 cse_11;
  cse_11 = (_Object2World * _glesVertex);
  tmpvar_10 = (_WorldSpaceLightPos0.xyz - cse_11.xyz);
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_1;
  xlv_TEXCOORD3 = tmpvar_2;
  xlv_TEXCOORD4 = (_LightMatrix0 * cse_11).xyz;
}



#endif
#ifdef FRAGMENT

void main ()
{
  lowp vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 0.0;
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _Time;
uniform highp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp mat4 _LightMatrix0;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec4 xlv_TEXCOORD1;
out lowp vec3 xlv_TEXCOORD2;
out mediump vec3 xlv_TEXCOORD3;
out highp vec3 xlv_TEXCOORD4;
void main ()
{
  lowp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_1 = tmpvar_9;
  highp vec3 tmpvar_10;
  highp vec4 cse_11;
  cse_11 = (_Object2World * _glesVertex);
  tmpvar_10 = (_WorldSpaceLightPos0.xyz - cse_11.xyz);
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_1;
  xlv_TEXCOORD3 = tmpvar_2;
  xlv_TEXCOORD4 = (_LightMatrix0 * cse_11).xyz;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
void main ()
{
  lowp vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 0.0;
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _Time;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec4 xlv_TEXCOORD1;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD3;
void main ()
{
  lowp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_1 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = _WorldSpaceLightPos0.xyz;
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_1;
  xlv_TEXCOORD3 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

void main ()
{
  lowp vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 0.0;
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _Time;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec4 xlv_TEXCOORD1;
out lowp vec3 xlv_TEXCOORD2;
out mediump vec3 xlv_TEXCOORD3;
void main ()
{
  lowp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_1 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = _WorldSpaceLightPos0.xyz;
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_1;
  xlv_TEXCOORD3 = tmpvar_2;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
void main ()
{
  lowp vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 0.0;
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _Time;
uniform highp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp mat4 _LightMatrix0;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec4 xlv_TEXCOORD1;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD3;
varying highp vec4 xlv_TEXCOORD4;
void main ()
{
  lowp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_1 = tmpvar_9;
  highp vec3 tmpvar_10;
  highp vec4 cse_11;
  cse_11 = (_Object2World * _glesVertex);
  tmpvar_10 = (_WorldSpaceLightPos0.xyz - cse_11.xyz);
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_1;
  xlv_TEXCOORD3 = tmpvar_2;
  xlv_TEXCOORD4 = (_LightMatrix0 * cse_11);
}



#endif
#ifdef FRAGMENT

void main ()
{
  lowp vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 0.0;
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "SPOT" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _Time;
uniform highp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp mat4 _LightMatrix0;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec4 xlv_TEXCOORD1;
out lowp vec3 xlv_TEXCOORD2;
out mediump vec3 xlv_TEXCOORD3;
out highp vec4 xlv_TEXCOORD4;
void main ()
{
  lowp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_1 = tmpvar_9;
  highp vec3 tmpvar_10;
  highp vec4 cse_11;
  cse_11 = (_Object2World * _glesVertex);
  tmpvar_10 = (_WorldSpaceLightPos0.xyz - cse_11.xyz);
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_1;
  xlv_TEXCOORD3 = tmpvar_2;
  xlv_TEXCOORD4 = (_LightMatrix0 * cse_11);
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
void main ()
{
  lowp vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 0.0;
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _Time;
uniform highp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp mat4 _LightMatrix0;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec4 xlv_TEXCOORD1;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD3;
varying highp vec3 xlv_TEXCOORD4;
void main ()
{
  lowp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_1 = tmpvar_9;
  highp vec3 tmpvar_10;
  highp vec4 cse_11;
  cse_11 = (_Object2World * _glesVertex);
  tmpvar_10 = (_WorldSpaceLightPos0.xyz - cse_11.xyz);
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_1;
  xlv_TEXCOORD3 = tmpvar_2;
  xlv_TEXCOORD4 = (_LightMatrix0 * cse_11).xyz;
}



#endif
#ifdef FRAGMENT

void main ()
{
  lowp vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 0.0;
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _Time;
uniform highp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp mat4 _LightMatrix0;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec4 xlv_TEXCOORD1;
out lowp vec3 xlv_TEXCOORD2;
out mediump vec3 xlv_TEXCOORD3;
out highp vec3 xlv_TEXCOORD4;
void main ()
{
  lowp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_1 = tmpvar_9;
  highp vec3 tmpvar_10;
  highp vec4 cse_11;
  cse_11 = (_Object2World * _glesVertex);
  tmpvar_10 = (_WorldSpaceLightPos0.xyz - cse_11.xyz);
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_1;
  xlv_TEXCOORD3 = tmpvar_2;
  xlv_TEXCOORD4 = (_LightMatrix0 * cse_11).xyz;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
void main ()
{
  lowp vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 0.0;
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _Time;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp mat4 _LightMatrix0;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec4 xlv_TEXCOORD1;
varying lowp vec3 xlv_TEXCOORD2;
varying mediump vec3 xlv_TEXCOORD3;
varying highp vec2 xlv_TEXCOORD4;
void main ()
{
  lowp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_1 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = _WorldSpaceLightPos0.xyz;
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_1;
  xlv_TEXCOORD3 = tmpvar_2;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex)).xy;
}



#endif
#ifdef FRAGMENT

void main ()
{
  lowp vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 0.0;
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _Time;
uniform lowp vec4 _WorldSpaceLightPos0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp mat4 _LightMatrix0;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec4 xlv_TEXCOORD1;
out lowp vec3 xlv_TEXCOORD2;
out mediump vec3 xlv_TEXCOORD3;
out highp vec2 xlv_TEXCOORD4;
void main ()
{
  lowp vec3 tmpvar_1;
  mediump vec3 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp mat3 tmpvar_8;
  tmpvar_8[0] = _Object2World[0].xyz;
  tmpvar_8[1] = _Object2World[1].xyz;
  tmpvar_8[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_1 = tmpvar_9;
  highp vec3 tmpvar_10;
  tmpvar_10 = _WorldSpaceLightPos0.xyz;
  tmpvar_2 = tmpvar_10;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_3;
  xlv_TEXCOORD1 = tmpvar_4;
  xlv_TEXCOORD2 = tmpvar_1;
  xlv_TEXCOORD3 = tmpvar_2;
  xlv_TEXCOORD4 = (_LightMatrix0 * (_Object2World * _glesVertex)).xy;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
void main ()
{
  lowp vec4 c_1;
  c_1.xyz = vec3(0.0, 0.0, 0.0);
  c_1.w = 0.0;
  _glesFragData[0] = c_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "POINT" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "SPOT" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "SPOT" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL_COOKIE" }
"!!GLES3"
}
}
 }
 Pass {
  Name "PREPASS"
  Tags { "LIGHTMODE"="PrePassBase" "RenderType"="Opaque" }
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _Time;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec4 xlv_TEXCOORD1;
varying lowp vec3 xlv_TEXCOORD2;
void main ()
{
  lowp vec3 tmpvar_1;
  mediump vec4 tmpvar_2;
  mediump vec4 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4 = (_MMultiplier * _Color);
  tmpvar_2 = tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5.x = _Scroll2X;
  tmpvar_5.y = _Scroll2Y;
  highp vec2 tmpvar_6;
  tmpvar_6 = fract((tmpvar_5 * _Time.x));
  tmpvar_3.zw = tmpvar_6;
  highp mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_1 = tmpvar_8;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying lowp vec3 xlv_TEXCOORD2;
void main ()
{
  lowp vec4 res_1;
  res_1.xyz = ((xlv_TEXCOORD2 * 0.5) + 0.5);
  res_1.w = 0.0;
  gl_FragData[0] = res_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _Time;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec4 xlv_TEXCOORD1;
out lowp vec3 xlv_TEXCOORD2;
void main ()
{
  lowp vec3 tmpvar_1;
  mediump vec4 tmpvar_2;
  mediump vec4 tmpvar_3;
  highp vec4 tmpvar_4;
  tmpvar_4 = (_MMultiplier * _Color);
  tmpvar_2 = tmpvar_4;
  highp vec2 tmpvar_5;
  tmpvar_5.x = _Scroll2X;
  tmpvar_5.y = _Scroll2Y;
  highp vec2 tmpvar_6;
  tmpvar_6 = fract((tmpvar_5 * _Time.x));
  tmpvar_3.zw = tmpvar_6;
  highp mat3 tmpvar_7;
  tmpvar_7[0] = _Object2World[0].xyz;
  tmpvar_7[1] = _Object2World[1].xyz;
  tmpvar_7[2] = _Object2World[2].xyz;
  highp vec3 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * (normalize(_glesNormal) * unity_Scale.w));
  tmpvar_1 = tmpvar_8;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
  xlv_TEXCOORD2 = tmpvar_1;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
in lowp vec3 xlv_TEXCOORD2;
void main ()
{
  lowp vec4 res_1;
  res_1.xyz = ((xlv_TEXCOORD2 * 0.5) + 0.5);
  res_1.w = 0.0;
  _glesFragData[0] = res_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
"!!GLES"
}
SubProgram "gles3 " {
"!!GLES3"
}
}
 }
 Pass {
  Name "PREPASS"
  Tags { "LIGHTMODE"="PrePassFinal" "RenderType"="Opaque" }
  ZWrite Off
Program "vp" {
SubProgram "gles " {
Keywords { "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying highp vec4 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD5;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  highp vec3 tmpvar_3;
  mediump vec4 tmpvar_4;
  mediump vec4 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6 = (_MMultiplier * _Color);
  tmpvar_4 = tmpvar_6;
  highp vec2 tmpvar_7;
  tmpvar_7.x = _Scroll2X;
  tmpvar_7.y = _Scroll2Y;
  highp vec2 tmpvar_8;
  tmpvar_8 = fract((tmpvar_7 * _Time.x));
  tmpvar_5.zw = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9 = (glstate_matrix_mvp * _glesVertex);
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_12;
  highp vec4 o_13;
  highp vec4 tmpvar_14;
  tmpvar_14 = (tmpvar_9 * 0.5);
  highp vec2 tmpvar_15;
  tmpvar_15.x = tmpvar_14.x;
  tmpvar_15.y = (tmpvar_14.y * _ProjectionParams.x);
  o_13.xy = (tmpvar_15 + tmpvar_14.w);
  o_13.zw = tmpvar_9.zw;
  highp mat3 tmpvar_16;
  tmpvar_16[0] = _Object2World[0].xyz;
  tmpvar_16[1] = _Object2World[1].xyz;
  tmpvar_16[2] = _Object2World[2].xyz;
  highp vec4 tmpvar_17;
  tmpvar_17.w = 1.0;
  tmpvar_17.xyz = (tmpvar_16 * (normalize(_glesNormal) * unity_Scale.w));
  mediump vec3 tmpvar_18;
  mediump vec4 normal_19;
  normal_19 = tmpvar_17;
  highp float vC_20;
  mediump vec3 x3_21;
  mediump vec3 x2_22;
  mediump vec3 x1_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (unity_SHAr, normal_19);
  x1_23.x = tmpvar_24;
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHAg, normal_19);
  x1_23.y = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = dot (unity_SHAb, normal_19);
  x1_23.z = tmpvar_26;
  mediump vec4 tmpvar_27;
  tmpvar_27 = (normal_19.xyzz * normal_19.yzzx);
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHBr, tmpvar_27);
  x2_22.x = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = dot (unity_SHBg, tmpvar_27);
  x2_22.y = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = dot (unity_SHBb, tmpvar_27);
  x2_22.z = tmpvar_30;
  mediump float tmpvar_31;
  tmpvar_31 = ((normal_19.x * normal_19.x) - (normal_19.y * normal_19.y));
  vC_20 = tmpvar_31;
  highp vec3 tmpvar_32;
  tmpvar_32 = (unity_SHC.xyz * vC_20);
  x3_21 = tmpvar_32;
  tmpvar_18 = ((x1_23 + x2_22) + x3_21);
  tmpvar_3 = tmpvar_18;
  gl_Position = tmpvar_9;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = tmpvar_5;
  xlv_TEXCOORD4 = o_13;
  xlv_TEXCOORD5 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
uniform sampler2D _LightBuffer;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying highp vec4 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD5;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 c_2;
  mediump vec4 light_3;
  lowp vec4 c_4;
  mediump vec2 P_5;
  P_5 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_6;
  tmpvar_6 = (texture2D (_MainTex1, xlv_TEXCOORD0.zw) * texture2D (_MainTex2, P_5));
  c_4.w = tmpvar_6.w;
  mediump vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6.xyz * xlv_TEXCOORD2.xyz);
  c_4.xyz = tmpvar_7;
  lowp vec3 tmpvar_8;
  tmpvar_8 = (c_4.xyz + texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2DProj (_LightBuffer, xlv_TEXCOORD4);
  light_3 = tmpvar_9;
  mediump vec4 tmpvar_10;
  tmpvar_10 = -(log2(max (light_3, vec4(0.001, 0.001, 0.001, 0.001))));
  light_3.w = tmpvar_10.w;
  highp vec3 tmpvar_11;
  tmpvar_11 = (tmpvar_10.xyz + xlv_TEXCOORD5);
  light_3.xyz = tmpvar_11;
  lowp vec4 c_12;
  c_12.xyz = vec3(0.0, 0.0, 0.0);
  c_12.w = 1.0;
  c_2 = c_12;
  c_2.xyz = (c_2.xyz + tmpvar_8);
  tmpvar_1 = c_2;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
out mediump vec4 xlv_TEXCOORD2;
out mediump vec4 xlv_TEXCOORD3;
out highp vec4 xlv_TEXCOORD4;
out highp vec3 xlv_TEXCOORD5;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  highp vec3 tmpvar_3;
  mediump vec4 tmpvar_4;
  mediump vec4 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6 = (_MMultiplier * _Color);
  tmpvar_4 = tmpvar_6;
  highp vec2 tmpvar_7;
  tmpvar_7.x = _Scroll2X;
  tmpvar_7.y = _Scroll2Y;
  highp vec2 tmpvar_8;
  tmpvar_8 = fract((tmpvar_7 * _Time.x));
  tmpvar_5.zw = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9 = (glstate_matrix_mvp * _glesVertex);
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_12;
  highp vec4 o_13;
  highp vec4 tmpvar_14;
  tmpvar_14 = (tmpvar_9 * 0.5);
  highp vec2 tmpvar_15;
  tmpvar_15.x = tmpvar_14.x;
  tmpvar_15.y = (tmpvar_14.y * _ProjectionParams.x);
  o_13.xy = (tmpvar_15 + tmpvar_14.w);
  o_13.zw = tmpvar_9.zw;
  highp mat3 tmpvar_16;
  tmpvar_16[0] = _Object2World[0].xyz;
  tmpvar_16[1] = _Object2World[1].xyz;
  tmpvar_16[2] = _Object2World[2].xyz;
  highp vec4 tmpvar_17;
  tmpvar_17.w = 1.0;
  tmpvar_17.xyz = (tmpvar_16 * (normalize(_glesNormal) * unity_Scale.w));
  mediump vec3 tmpvar_18;
  mediump vec4 normal_19;
  normal_19 = tmpvar_17;
  highp float vC_20;
  mediump vec3 x3_21;
  mediump vec3 x2_22;
  mediump vec3 x1_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (unity_SHAr, normal_19);
  x1_23.x = tmpvar_24;
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHAg, normal_19);
  x1_23.y = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = dot (unity_SHAb, normal_19);
  x1_23.z = tmpvar_26;
  mediump vec4 tmpvar_27;
  tmpvar_27 = (normal_19.xyzz * normal_19.yzzx);
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHBr, tmpvar_27);
  x2_22.x = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = dot (unity_SHBg, tmpvar_27);
  x2_22.y = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = dot (unity_SHBb, tmpvar_27);
  x2_22.z = tmpvar_30;
  mediump float tmpvar_31;
  tmpvar_31 = ((normal_19.x * normal_19.x) - (normal_19.y * normal_19.y));
  vC_20 = tmpvar_31;
  highp vec3 tmpvar_32;
  tmpvar_32 = (unity_SHC.xyz * vC_20);
  x3_21 = tmpvar_32;
  tmpvar_18 = ((x1_23 + x2_22) + x3_21);
  tmpvar_3 = tmpvar_18;
  gl_Position = tmpvar_9;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = tmpvar_5;
  xlv_TEXCOORD4 = o_13;
  xlv_TEXCOORD5 = tmpvar_3;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
uniform sampler2D _LightBuffer;
in mediump vec4 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in mediump vec4 xlv_TEXCOORD2;
in mediump vec4 xlv_TEXCOORD3;
in highp vec4 xlv_TEXCOORD4;
in highp vec3 xlv_TEXCOORD5;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 c_2;
  mediump vec4 light_3;
  lowp vec4 c_4;
  mediump vec2 P_5;
  P_5 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_6;
  tmpvar_6 = (texture (_MainTex1, xlv_TEXCOORD0.zw) * texture (_MainTex2, P_5));
  c_4.w = tmpvar_6.w;
  mediump vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6.xyz * xlv_TEXCOORD2.xyz);
  c_4.xyz = tmpvar_7;
  lowp vec3 tmpvar_8;
  tmpvar_8 = (c_4.xyz + texture (_MainTex, xlv_TEXCOORD0.xy).xyz);
  lowp vec4 tmpvar_9;
  tmpvar_9 = textureProj (_LightBuffer, xlv_TEXCOORD4);
  light_3 = tmpvar_9;
  mediump vec4 tmpvar_10;
  tmpvar_10 = -(log2(max (light_3, vec4(0.001, 0.001, 0.001, 0.001))));
  light_3.w = tmpvar_10.w;
  highp vec3 tmpvar_11;
  tmpvar_11 = (tmpvar_10.xyz + xlv_TEXCOORD5);
  light_3.xyz = tmpvar_11;
  lowp vec4 c_12;
  c_12.xyz = vec3(0.0, 0.0, 0.0);
  c_12.w = 1.0;
  c_2 = c_12;
  c_2.xyz = (c_2.xyz + tmpvar_8);
  tmpvar_1 = c_2;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 _Object2World;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying highp vec4 xlv_TEXCOORD4;
varying highp vec2 xlv_TEXCOORD5;
varying highp vec4 xlv_TEXCOORD6;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  highp vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  mediump vec4 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6 = (_MMultiplier * _Color);
  tmpvar_4 = tmpvar_6;
  highp vec2 tmpvar_7;
  tmpvar_7.x = _Scroll2X;
  tmpvar_7.y = _Scroll2Y;
  highp vec2 tmpvar_8;
  tmpvar_8 = fract((tmpvar_7 * _Time.x));
  tmpvar_5.zw = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9 = (glstate_matrix_mvp * _glesVertex);
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_12;
  highp vec4 o_13;
  highp vec4 tmpvar_14;
  tmpvar_14 = (tmpvar_9 * 0.5);
  highp vec2 tmpvar_15;
  tmpvar_15.x = tmpvar_14.x;
  tmpvar_15.y = (tmpvar_14.y * _ProjectionParams.x);
  o_13.xy = (tmpvar_15 + tmpvar_14.w);
  o_13.zw = tmpvar_9.zw;
  tmpvar_3.xyz = (((_Object2World * _glesVertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w);
  tmpvar_3.w = (-((glstate_matrix_modelview0 * _glesVertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w));
  gl_Position = tmpvar_9;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = tmpvar_5;
  xlv_TEXCOORD4 = o_13;
  xlv_TEXCOORD5 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD6 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
uniform sampler2D _LightBuffer;
uniform sampler2D unity_Lightmap;
uniform sampler2D unity_LightmapInd;
uniform highp vec4 unity_LightmapFade;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying highp vec4 xlv_TEXCOORD4;
varying highp vec2 xlv_TEXCOORD5;
varying highp vec4 xlv_TEXCOORD6;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 c_2;
  mediump vec3 lmIndirect_3;
  mediump vec3 lmFull_4;
  mediump float lmFade_5;
  mediump vec4 light_6;
  lowp vec4 c_7;
  mediump vec2 P_8;
  P_8 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_9;
  tmpvar_9 = (texture2D (_MainTex1, xlv_TEXCOORD0.zw) * texture2D (_MainTex2, P_8));
  c_7.w = tmpvar_9.w;
  mediump vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_9.xyz * xlv_TEXCOORD2.xyz);
  c_7.xyz = tmpvar_10;
  lowp vec3 tmpvar_11;
  tmpvar_11 = (c_7.xyz + texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz);
  lowp vec4 tmpvar_12;
  tmpvar_12 = texture2DProj (_LightBuffer, xlv_TEXCOORD4);
  light_6 = tmpvar_12;
  mediump vec4 tmpvar_13;
  tmpvar_13 = -(log2(max (light_6, vec4(0.001, 0.001, 0.001, 0.001))));
  light_6.w = tmpvar_13.w;
  highp float tmpvar_14;
  tmpvar_14 = ((sqrt(
    dot (xlv_TEXCOORD6, xlv_TEXCOORD6)
  ) * unity_LightmapFade.z) + unity_LightmapFade.w);
  lmFade_5 = tmpvar_14;
  lowp vec3 tmpvar_15;
  tmpvar_15 = (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD5).xyz);
  lmFull_4 = tmpvar_15;
  lowp vec3 tmpvar_16;
  tmpvar_16 = (2.0 * texture2D (unity_LightmapInd, xlv_TEXCOORD5).xyz);
  lmIndirect_3 = tmpvar_16;
  light_6.xyz = (tmpvar_13.xyz + mix (lmIndirect_3, lmFull_4, vec3(clamp (lmFade_5, 0.0, 1.0))));
  lowp vec4 c_17;
  c_17.xyz = vec3(0.0, 0.0, 0.0);
  c_17.w = 1.0;
  c_2 = c_17;
  c_2.xyz = (c_2.xyz + tmpvar_11);
  tmpvar_1 = c_2;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
in vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 _Object2World;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
out mediump vec4 xlv_TEXCOORD2;
out mediump vec4 xlv_TEXCOORD3;
out highp vec4 xlv_TEXCOORD4;
out highp vec2 xlv_TEXCOORD5;
out highp vec4 xlv_TEXCOORD6;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  highp vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  mediump vec4 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6 = (_MMultiplier * _Color);
  tmpvar_4 = tmpvar_6;
  highp vec2 tmpvar_7;
  tmpvar_7.x = _Scroll2X;
  tmpvar_7.y = _Scroll2Y;
  highp vec2 tmpvar_8;
  tmpvar_8 = fract((tmpvar_7 * _Time.x));
  tmpvar_5.zw = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9 = (glstate_matrix_mvp * _glesVertex);
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_12;
  highp vec4 o_13;
  highp vec4 tmpvar_14;
  tmpvar_14 = (tmpvar_9 * 0.5);
  highp vec2 tmpvar_15;
  tmpvar_15.x = tmpvar_14.x;
  tmpvar_15.y = (tmpvar_14.y * _ProjectionParams.x);
  o_13.xy = (tmpvar_15 + tmpvar_14.w);
  o_13.zw = tmpvar_9.zw;
  tmpvar_3.xyz = (((_Object2World * _glesVertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w);
  tmpvar_3.w = (-((glstate_matrix_modelview0 * _glesVertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w));
  gl_Position = tmpvar_9;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = tmpvar_5;
  xlv_TEXCOORD4 = o_13;
  xlv_TEXCOORD5 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD6 = tmpvar_3;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
uniform sampler2D _LightBuffer;
uniform sampler2D unity_Lightmap;
uniform sampler2D unity_LightmapInd;
uniform highp vec4 unity_LightmapFade;
in mediump vec4 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in mediump vec4 xlv_TEXCOORD2;
in mediump vec4 xlv_TEXCOORD3;
in highp vec4 xlv_TEXCOORD4;
in highp vec2 xlv_TEXCOORD5;
in highp vec4 xlv_TEXCOORD6;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 c_2;
  mediump vec3 lmIndirect_3;
  mediump vec3 lmFull_4;
  mediump float lmFade_5;
  mediump vec4 light_6;
  lowp vec4 c_7;
  mediump vec2 P_8;
  P_8 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_9;
  tmpvar_9 = (texture (_MainTex1, xlv_TEXCOORD0.zw) * texture (_MainTex2, P_8));
  c_7.w = tmpvar_9.w;
  mediump vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_9.xyz * xlv_TEXCOORD2.xyz);
  c_7.xyz = tmpvar_10;
  lowp vec3 tmpvar_11;
  tmpvar_11 = (c_7.xyz + texture (_MainTex, xlv_TEXCOORD0.xy).xyz);
  lowp vec4 tmpvar_12;
  tmpvar_12 = textureProj (_LightBuffer, xlv_TEXCOORD4);
  light_6 = tmpvar_12;
  mediump vec4 tmpvar_13;
  tmpvar_13 = -(log2(max (light_6, vec4(0.001, 0.001, 0.001, 0.001))));
  light_6.w = tmpvar_13.w;
  highp float tmpvar_14;
  tmpvar_14 = ((sqrt(
    dot (xlv_TEXCOORD6, xlv_TEXCOORD6)
  ) * unity_LightmapFade.z) + unity_LightmapFade.w);
  lmFade_5 = tmpvar_14;
  lowp vec3 tmpvar_15;
  tmpvar_15 = (2.0 * texture (unity_Lightmap, xlv_TEXCOORD5).xyz);
  lmFull_4 = tmpvar_15;
  lowp vec3 tmpvar_16;
  tmpvar_16 = (2.0 * texture (unity_LightmapInd, xlv_TEXCOORD5).xyz);
  lmIndirect_3 = tmpvar_16;
  light_6.xyz = (tmpvar_13.xyz + mix (lmIndirect_3, lmFull_4, vec3(clamp (lmFade_5, 0.0, 1.0))));
  lowp vec4 c_17;
  c_17.xyz = vec3(0.0, 0.0, 0.0);
  c_17.w = 1.0;
  c_2 = c_17;
  c_2.xyz = (c_2.xyz + tmpvar_11);
  tmpvar_1 = c_2;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "LIGHTMAP_ON" "DIRLIGHTMAP_ON" "HDR_LIGHT_PREPASS_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying highp vec4 xlv_TEXCOORD4;
varying highp vec2 xlv_TEXCOORD5;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = (glstate_matrix_mvp * _glesVertex);
  highp vec2 tmpvar_9;
  tmpvar_9 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_11;
  highp vec4 o_12;
  highp vec4 tmpvar_13;
  tmpvar_13 = (tmpvar_8 * 0.5);
  highp vec2 tmpvar_14;
  tmpvar_14.x = tmpvar_13.x;
  tmpvar_14.y = (tmpvar_13.y * _ProjectionParams.x);
  o_12.xy = (tmpvar_14 + tmpvar_13.w);
  o_12.zw = tmpvar_8.zw;
  gl_Position = tmpvar_8;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = o_12;
  xlv_TEXCOORD5 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 c_2;
  lowp vec4 c_3;
  mediump vec2 P_4;
  P_4 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_5;
  tmpvar_5 = (texture2D (_MainTex1, xlv_TEXCOORD0.zw) * texture2D (_MainTex2, P_4));
  c_3.w = tmpvar_5.w;
  mediump vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5.xyz * xlv_TEXCOORD2.xyz);
  c_3.xyz = tmpvar_6;
  lowp vec3 tmpvar_7;
  tmpvar_7 = (c_3.xyz + texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz);
  lowp vec4 c_8;
  c_8.xyz = vec3(0.0, 0.0, 0.0);
  c_8.w = 1.0;
  c_2 = c_8;
  c_2.xyz = (c_2.xyz + tmpvar_7);
  tmpvar_1 = c_2;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "LIGHTMAP_ON" "DIRLIGHTMAP_ON" "HDR_LIGHT_PREPASS_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
in vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
out mediump vec4 xlv_TEXCOORD2;
out mediump vec4 xlv_TEXCOORD3;
out highp vec4 xlv_TEXCOORD4;
out highp vec2 xlv_TEXCOORD5;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = (glstate_matrix_mvp * _glesVertex);
  highp vec2 tmpvar_9;
  tmpvar_9 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_11;
  highp vec4 o_12;
  highp vec4 tmpvar_13;
  tmpvar_13 = (tmpvar_8 * 0.5);
  highp vec2 tmpvar_14;
  tmpvar_14.x = tmpvar_13.x;
  tmpvar_14.y = (tmpvar_13.y * _ProjectionParams.x);
  o_12.xy = (tmpvar_14 + tmpvar_13.w);
  o_12.zw = tmpvar_8.zw;
  gl_Position = tmpvar_8;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = o_12;
  xlv_TEXCOORD5 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
in mediump vec4 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in mediump vec4 xlv_TEXCOORD2;
in mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 c_2;
  lowp vec4 c_3;
  mediump vec2 P_4;
  P_4 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_5;
  tmpvar_5 = (texture (_MainTex1, xlv_TEXCOORD0.zw) * texture (_MainTex2, P_4));
  c_3.w = tmpvar_5.w;
  mediump vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5.xyz * xlv_TEXCOORD2.xyz);
  c_3.xyz = tmpvar_6;
  lowp vec3 tmpvar_7;
  tmpvar_7 = (c_3.xyz + texture (_MainTex, xlv_TEXCOORD0.xy).xyz);
  lowp vec4 c_8;
  c_8.xyz = vec3(0.0, 0.0, 0.0);
  c_8.w = 1.0;
  c_2 = c_8;
  c_2.xyz = (c_2.xyz + tmpvar_7);
  tmpvar_1 = c_2;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_ON" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying highp vec4 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD5;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  highp vec3 tmpvar_3;
  mediump vec4 tmpvar_4;
  mediump vec4 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6 = (_MMultiplier * _Color);
  tmpvar_4 = tmpvar_6;
  highp vec2 tmpvar_7;
  tmpvar_7.x = _Scroll2X;
  tmpvar_7.y = _Scroll2Y;
  highp vec2 tmpvar_8;
  tmpvar_8 = fract((tmpvar_7 * _Time.x));
  tmpvar_5.zw = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9 = (glstate_matrix_mvp * _glesVertex);
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_12;
  highp vec4 o_13;
  highp vec4 tmpvar_14;
  tmpvar_14 = (tmpvar_9 * 0.5);
  highp vec2 tmpvar_15;
  tmpvar_15.x = tmpvar_14.x;
  tmpvar_15.y = (tmpvar_14.y * _ProjectionParams.x);
  o_13.xy = (tmpvar_15 + tmpvar_14.w);
  o_13.zw = tmpvar_9.zw;
  highp mat3 tmpvar_16;
  tmpvar_16[0] = _Object2World[0].xyz;
  tmpvar_16[1] = _Object2World[1].xyz;
  tmpvar_16[2] = _Object2World[2].xyz;
  highp vec4 tmpvar_17;
  tmpvar_17.w = 1.0;
  tmpvar_17.xyz = (tmpvar_16 * (normalize(_glesNormal) * unity_Scale.w));
  mediump vec3 tmpvar_18;
  mediump vec4 normal_19;
  normal_19 = tmpvar_17;
  highp float vC_20;
  mediump vec3 x3_21;
  mediump vec3 x2_22;
  mediump vec3 x1_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (unity_SHAr, normal_19);
  x1_23.x = tmpvar_24;
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHAg, normal_19);
  x1_23.y = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = dot (unity_SHAb, normal_19);
  x1_23.z = tmpvar_26;
  mediump vec4 tmpvar_27;
  tmpvar_27 = (normal_19.xyzz * normal_19.yzzx);
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHBr, tmpvar_27);
  x2_22.x = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = dot (unity_SHBg, tmpvar_27);
  x2_22.y = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = dot (unity_SHBb, tmpvar_27);
  x2_22.z = tmpvar_30;
  mediump float tmpvar_31;
  tmpvar_31 = ((normal_19.x * normal_19.x) - (normal_19.y * normal_19.y));
  vC_20 = tmpvar_31;
  highp vec3 tmpvar_32;
  tmpvar_32 = (unity_SHC.xyz * vC_20);
  x3_21 = tmpvar_32;
  tmpvar_18 = ((x1_23 + x2_22) + x3_21);
  tmpvar_3 = tmpvar_18;
  gl_Position = tmpvar_9;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = tmpvar_5;
  xlv_TEXCOORD4 = o_13;
  xlv_TEXCOORD5 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
uniform sampler2D _LightBuffer;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying highp vec4 xlv_TEXCOORD4;
varying highp vec3 xlv_TEXCOORD5;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 c_2;
  mediump vec4 light_3;
  lowp vec4 c_4;
  mediump vec2 P_5;
  P_5 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_6;
  tmpvar_6 = (texture2D (_MainTex1, xlv_TEXCOORD0.zw) * texture2D (_MainTex2, P_5));
  c_4.w = tmpvar_6.w;
  mediump vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6.xyz * xlv_TEXCOORD2.xyz);
  c_4.xyz = tmpvar_7;
  lowp vec3 tmpvar_8;
  tmpvar_8 = (c_4.xyz + texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2DProj (_LightBuffer, xlv_TEXCOORD4);
  light_3 = tmpvar_9;
  mediump vec4 tmpvar_10;
  tmpvar_10 = max (light_3, vec4(0.001, 0.001, 0.001, 0.001));
  light_3.w = tmpvar_10.w;
  highp vec3 tmpvar_11;
  tmpvar_11 = (tmpvar_10.xyz + xlv_TEXCOORD5);
  light_3.xyz = tmpvar_11;
  lowp vec4 c_12;
  c_12.xyz = vec3(0.0, 0.0, 0.0);
  c_12.w = 1.0;
  c_2 = c_12;
  c_2.xyz = (c_2.xyz + tmpvar_8);
  tmpvar_1 = c_2;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_ON" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 unity_SHAr;
uniform highp vec4 unity_SHAg;
uniform highp vec4 unity_SHAb;
uniform highp vec4 unity_SHBr;
uniform highp vec4 unity_SHBg;
uniform highp vec4 unity_SHBb;
uniform highp vec4 unity_SHC;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 unity_Scale;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
out mediump vec4 xlv_TEXCOORD2;
out mediump vec4 xlv_TEXCOORD3;
out highp vec4 xlv_TEXCOORD4;
out highp vec3 xlv_TEXCOORD5;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  highp vec3 tmpvar_3;
  mediump vec4 tmpvar_4;
  mediump vec4 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6 = (_MMultiplier * _Color);
  tmpvar_4 = tmpvar_6;
  highp vec2 tmpvar_7;
  tmpvar_7.x = _Scroll2X;
  tmpvar_7.y = _Scroll2Y;
  highp vec2 tmpvar_8;
  tmpvar_8 = fract((tmpvar_7 * _Time.x));
  tmpvar_5.zw = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9 = (glstate_matrix_mvp * _glesVertex);
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_12;
  highp vec4 o_13;
  highp vec4 tmpvar_14;
  tmpvar_14 = (tmpvar_9 * 0.5);
  highp vec2 tmpvar_15;
  tmpvar_15.x = tmpvar_14.x;
  tmpvar_15.y = (tmpvar_14.y * _ProjectionParams.x);
  o_13.xy = (tmpvar_15 + tmpvar_14.w);
  o_13.zw = tmpvar_9.zw;
  highp mat3 tmpvar_16;
  tmpvar_16[0] = _Object2World[0].xyz;
  tmpvar_16[1] = _Object2World[1].xyz;
  tmpvar_16[2] = _Object2World[2].xyz;
  highp vec4 tmpvar_17;
  tmpvar_17.w = 1.0;
  tmpvar_17.xyz = (tmpvar_16 * (normalize(_glesNormal) * unity_Scale.w));
  mediump vec3 tmpvar_18;
  mediump vec4 normal_19;
  normal_19 = tmpvar_17;
  highp float vC_20;
  mediump vec3 x3_21;
  mediump vec3 x2_22;
  mediump vec3 x1_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (unity_SHAr, normal_19);
  x1_23.x = tmpvar_24;
  highp float tmpvar_25;
  tmpvar_25 = dot (unity_SHAg, normal_19);
  x1_23.y = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = dot (unity_SHAb, normal_19);
  x1_23.z = tmpvar_26;
  mediump vec4 tmpvar_27;
  tmpvar_27 = (normal_19.xyzz * normal_19.yzzx);
  highp float tmpvar_28;
  tmpvar_28 = dot (unity_SHBr, tmpvar_27);
  x2_22.x = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = dot (unity_SHBg, tmpvar_27);
  x2_22.y = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = dot (unity_SHBb, tmpvar_27);
  x2_22.z = tmpvar_30;
  mediump float tmpvar_31;
  tmpvar_31 = ((normal_19.x * normal_19.x) - (normal_19.y * normal_19.y));
  vC_20 = tmpvar_31;
  highp vec3 tmpvar_32;
  tmpvar_32 = (unity_SHC.xyz * vC_20);
  x3_21 = tmpvar_32;
  tmpvar_18 = ((x1_23 + x2_22) + x3_21);
  tmpvar_3 = tmpvar_18;
  gl_Position = tmpvar_9;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = tmpvar_5;
  xlv_TEXCOORD4 = o_13;
  xlv_TEXCOORD5 = tmpvar_3;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
uniform sampler2D _LightBuffer;
in mediump vec4 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in mediump vec4 xlv_TEXCOORD2;
in mediump vec4 xlv_TEXCOORD3;
in highp vec4 xlv_TEXCOORD4;
in highp vec3 xlv_TEXCOORD5;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 c_2;
  mediump vec4 light_3;
  lowp vec4 c_4;
  mediump vec2 P_5;
  P_5 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_6;
  tmpvar_6 = (texture (_MainTex1, xlv_TEXCOORD0.zw) * texture (_MainTex2, P_5));
  c_4.w = tmpvar_6.w;
  mediump vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6.xyz * xlv_TEXCOORD2.xyz);
  c_4.xyz = tmpvar_7;
  lowp vec3 tmpvar_8;
  tmpvar_8 = (c_4.xyz + texture (_MainTex, xlv_TEXCOORD0.xy).xyz);
  lowp vec4 tmpvar_9;
  tmpvar_9 = textureProj (_LightBuffer, xlv_TEXCOORD4);
  light_3 = tmpvar_9;
  mediump vec4 tmpvar_10;
  tmpvar_10 = max (light_3, vec4(0.001, 0.001, 0.001, 0.001));
  light_3.w = tmpvar_10.w;
  highp vec3 tmpvar_11;
  tmpvar_11 = (tmpvar_10.xyz + xlv_TEXCOORD5);
  light_3.xyz = tmpvar_11;
  lowp vec4 c_12;
  c_12.xyz = vec3(0.0, 0.0, 0.0);
  c_12.w = 1.0;
  c_2 = c_12;
  c_2.xyz = (c_2.xyz + tmpvar_8);
  tmpvar_1 = c_2;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_ON" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 _Object2World;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying highp vec4 xlv_TEXCOORD4;
varying highp vec2 xlv_TEXCOORD5;
varying highp vec4 xlv_TEXCOORD6;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  highp vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  mediump vec4 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6 = (_MMultiplier * _Color);
  tmpvar_4 = tmpvar_6;
  highp vec2 tmpvar_7;
  tmpvar_7.x = _Scroll2X;
  tmpvar_7.y = _Scroll2Y;
  highp vec2 tmpvar_8;
  tmpvar_8 = fract((tmpvar_7 * _Time.x));
  tmpvar_5.zw = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9 = (glstate_matrix_mvp * _glesVertex);
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_12;
  highp vec4 o_13;
  highp vec4 tmpvar_14;
  tmpvar_14 = (tmpvar_9 * 0.5);
  highp vec2 tmpvar_15;
  tmpvar_15.x = tmpvar_14.x;
  tmpvar_15.y = (tmpvar_14.y * _ProjectionParams.x);
  o_13.xy = (tmpvar_15 + tmpvar_14.w);
  o_13.zw = tmpvar_9.zw;
  tmpvar_3.xyz = (((_Object2World * _glesVertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w);
  tmpvar_3.w = (-((glstate_matrix_modelview0 * _glesVertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w));
  gl_Position = tmpvar_9;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = tmpvar_5;
  xlv_TEXCOORD4 = o_13;
  xlv_TEXCOORD5 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD6 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
uniform sampler2D _LightBuffer;
uniform sampler2D unity_Lightmap;
uniform sampler2D unity_LightmapInd;
uniform highp vec4 unity_LightmapFade;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying highp vec4 xlv_TEXCOORD4;
varying highp vec2 xlv_TEXCOORD5;
varying highp vec4 xlv_TEXCOORD6;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 c_2;
  mediump vec3 lmIndirect_3;
  mediump vec3 lmFull_4;
  mediump float lmFade_5;
  mediump vec4 light_6;
  lowp vec4 c_7;
  mediump vec2 P_8;
  P_8 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_9;
  tmpvar_9 = (texture2D (_MainTex1, xlv_TEXCOORD0.zw) * texture2D (_MainTex2, P_8));
  c_7.w = tmpvar_9.w;
  mediump vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_9.xyz * xlv_TEXCOORD2.xyz);
  c_7.xyz = tmpvar_10;
  lowp vec3 tmpvar_11;
  tmpvar_11 = (c_7.xyz + texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz);
  lowp vec4 tmpvar_12;
  tmpvar_12 = texture2DProj (_LightBuffer, xlv_TEXCOORD4);
  light_6 = tmpvar_12;
  mediump vec4 tmpvar_13;
  tmpvar_13 = max (light_6, vec4(0.001, 0.001, 0.001, 0.001));
  light_6.w = tmpvar_13.w;
  highp float tmpvar_14;
  tmpvar_14 = ((sqrt(
    dot (xlv_TEXCOORD6, xlv_TEXCOORD6)
  ) * unity_LightmapFade.z) + unity_LightmapFade.w);
  lmFade_5 = tmpvar_14;
  lowp vec3 tmpvar_15;
  tmpvar_15 = (2.0 * texture2D (unity_Lightmap, xlv_TEXCOORD5).xyz);
  lmFull_4 = tmpvar_15;
  lowp vec3 tmpvar_16;
  tmpvar_16 = (2.0 * texture2D (unity_LightmapInd, xlv_TEXCOORD5).xyz);
  lmIndirect_3 = tmpvar_16;
  light_6.xyz = (tmpvar_13.xyz + mix (lmIndirect_3, lmFull_4, vec3(clamp (lmFade_5, 0.0, 1.0))));
  lowp vec4 c_17;
  c_17.xyz = vec3(0.0, 0.0, 0.0);
  c_17.w = 1.0;
  c_2 = c_17;
  c_2.xyz = (c_2.xyz + tmpvar_11);
  tmpvar_1 = c_2;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_ON" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
in vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 _Object2World;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
out mediump vec4 xlv_TEXCOORD2;
out mediump vec4 xlv_TEXCOORD3;
out highp vec4 xlv_TEXCOORD4;
out highp vec2 xlv_TEXCOORD5;
out highp vec4 xlv_TEXCOORD6;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  highp vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  mediump vec4 tmpvar_5;
  highp vec4 tmpvar_6;
  tmpvar_6 = (_MMultiplier * _Color);
  tmpvar_4 = tmpvar_6;
  highp vec2 tmpvar_7;
  tmpvar_7.x = _Scroll2X;
  tmpvar_7.y = _Scroll2Y;
  highp vec2 tmpvar_8;
  tmpvar_8 = fract((tmpvar_7 * _Time.x));
  tmpvar_5.zw = tmpvar_8;
  highp vec4 tmpvar_9;
  tmpvar_9 = (glstate_matrix_mvp * _glesVertex);
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_11;
  highp vec2 tmpvar_12;
  tmpvar_12 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_12;
  highp vec4 o_13;
  highp vec4 tmpvar_14;
  tmpvar_14 = (tmpvar_9 * 0.5);
  highp vec2 tmpvar_15;
  tmpvar_15.x = tmpvar_14.x;
  tmpvar_15.y = (tmpvar_14.y * _ProjectionParams.x);
  o_13.xy = (tmpvar_15 + tmpvar_14.w);
  o_13.zw = tmpvar_9.zw;
  tmpvar_3.xyz = (((_Object2World * _glesVertex).xyz - unity_ShadowFadeCenterAndType.xyz) * unity_ShadowFadeCenterAndType.w);
  tmpvar_3.w = (-((glstate_matrix_modelview0 * _glesVertex).z) * (1.0 - unity_ShadowFadeCenterAndType.w));
  gl_Position = tmpvar_9;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_4;
  xlv_TEXCOORD3 = tmpvar_5;
  xlv_TEXCOORD4 = o_13;
  xlv_TEXCOORD5 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
  xlv_TEXCOORD6 = tmpvar_3;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
uniform sampler2D _LightBuffer;
uniform sampler2D unity_Lightmap;
uniform sampler2D unity_LightmapInd;
uniform highp vec4 unity_LightmapFade;
in mediump vec4 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in mediump vec4 xlv_TEXCOORD2;
in mediump vec4 xlv_TEXCOORD3;
in highp vec4 xlv_TEXCOORD4;
in highp vec2 xlv_TEXCOORD5;
in highp vec4 xlv_TEXCOORD6;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 c_2;
  mediump vec3 lmIndirect_3;
  mediump vec3 lmFull_4;
  mediump float lmFade_5;
  mediump vec4 light_6;
  lowp vec4 c_7;
  mediump vec2 P_8;
  P_8 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_9;
  tmpvar_9 = (texture (_MainTex1, xlv_TEXCOORD0.zw) * texture (_MainTex2, P_8));
  c_7.w = tmpvar_9.w;
  mediump vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_9.xyz * xlv_TEXCOORD2.xyz);
  c_7.xyz = tmpvar_10;
  lowp vec3 tmpvar_11;
  tmpvar_11 = (c_7.xyz + texture (_MainTex, xlv_TEXCOORD0.xy).xyz);
  lowp vec4 tmpvar_12;
  tmpvar_12 = textureProj (_LightBuffer, xlv_TEXCOORD4);
  light_6 = tmpvar_12;
  mediump vec4 tmpvar_13;
  tmpvar_13 = max (light_6, vec4(0.001, 0.001, 0.001, 0.001));
  light_6.w = tmpvar_13.w;
  highp float tmpvar_14;
  tmpvar_14 = ((sqrt(
    dot (xlv_TEXCOORD6, xlv_TEXCOORD6)
  ) * unity_LightmapFade.z) + unity_LightmapFade.w);
  lmFade_5 = tmpvar_14;
  lowp vec3 tmpvar_15;
  tmpvar_15 = (2.0 * texture (unity_Lightmap, xlv_TEXCOORD5).xyz);
  lmFull_4 = tmpvar_15;
  lowp vec3 tmpvar_16;
  tmpvar_16 = (2.0 * texture (unity_LightmapInd, xlv_TEXCOORD5).xyz);
  lmIndirect_3 = tmpvar_16;
  light_6.xyz = (tmpvar_13.xyz + mix (lmIndirect_3, lmFull_4, vec3(clamp (lmFade_5, 0.0, 1.0))));
  lowp vec4 c_17;
  c_17.xyz = vec3(0.0, 0.0, 0.0);
  c_17.w = 1.0;
  c_2 = c_17;
  c_2.xyz = (c_2.xyz + tmpvar_11);
  tmpvar_1 = c_2;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "LIGHTMAP_ON" "DIRLIGHTMAP_ON" "HDR_LIGHT_PREPASS_ON" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
varying highp vec4 xlv_TEXCOORD4;
varying highp vec2 xlv_TEXCOORD5;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = (glstate_matrix_mvp * _glesVertex);
  highp vec2 tmpvar_9;
  tmpvar_9 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_11;
  highp vec4 o_12;
  highp vec4 tmpvar_13;
  tmpvar_13 = (tmpvar_8 * 0.5);
  highp vec2 tmpvar_14;
  tmpvar_14.x = tmpvar_13.x;
  tmpvar_14.y = (tmpvar_13.y * _ProjectionParams.x);
  o_12.xy = (tmpvar_14 + tmpvar_13.w);
  o_12.zw = tmpvar_8.zw;
  gl_Position = tmpvar_8;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = o_12;
  xlv_TEXCOORD5 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
varying mediump vec4 xlv_TEXCOORD0;
varying mediump vec2 xlv_TEXCOORD1;
varying mediump vec4 xlv_TEXCOORD2;
varying mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 c_2;
  lowp vec4 c_3;
  mediump vec2 P_4;
  P_4 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_5;
  tmpvar_5 = (texture2D (_MainTex1, xlv_TEXCOORD0.zw) * texture2D (_MainTex2, P_4));
  c_3.w = tmpvar_5.w;
  mediump vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5.xyz * xlv_TEXCOORD2.xyz);
  c_3.xyz = tmpvar_6;
  lowp vec3 tmpvar_7;
  tmpvar_7 = (c_3.xyz + texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz);
  lowp vec4 c_8;
  c_8.xyz = vec3(0.0, 0.0, 0.0);
  c_8.w = 1.0;
  c_2 = c_8;
  c_2.xyz = (c_2.xyz + tmpvar_7);
  tmpvar_1 = c_2;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "LIGHTMAP_ON" "DIRLIGHTMAP_ON" "HDR_LIGHT_PREPASS_ON" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
in vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp float _Scroll2X;
uniform highp float _Scroll2Y;
uniform highp float _MMultiplier;
uniform highp vec4 _Color;
uniform highp vec4 unity_LightmapST;
uniform highp vec4 _MainTex_ST;
uniform highp vec4 _MainTex1_ST;
uniform highp vec4 _MainTex2_ST;
out mediump vec4 xlv_TEXCOORD0;
out mediump vec2 xlv_TEXCOORD1;
out mediump vec4 xlv_TEXCOORD2;
out mediump vec4 xlv_TEXCOORD3;
out highp vec4 xlv_TEXCOORD4;
out highp vec2 xlv_TEXCOORD5;
void main ()
{
  mediump vec4 tmpvar_1;
  mediump vec2 tmpvar_2;
  mediump vec4 tmpvar_3;
  mediump vec4 tmpvar_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = (_MMultiplier * _Color);
  tmpvar_3 = tmpvar_5;
  highp vec2 tmpvar_6;
  tmpvar_6.x = _Scroll2X;
  tmpvar_6.y = _Scroll2Y;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((tmpvar_6 * _Time.x));
  tmpvar_4.zw = tmpvar_7;
  highp vec4 tmpvar_8;
  tmpvar_8 = (glstate_matrix_mvp * _glesVertex);
  highp vec2 tmpvar_9;
  tmpvar_9 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  tmpvar_1.xy = tmpvar_9;
  highp vec2 tmpvar_10;
  tmpvar_10 = ((_glesMultiTexCoord0.xy * _MainTex1_ST.xy) + _MainTex1_ST.zw);
  tmpvar_1.zw = tmpvar_10;
  highp vec2 tmpvar_11;
  tmpvar_11 = ((_glesMultiTexCoord0.xy * _MainTex2_ST.xy) + _MainTex2_ST.zw);
  tmpvar_2 = tmpvar_11;
  highp vec4 o_12;
  highp vec4 tmpvar_13;
  tmpvar_13 = (tmpvar_8 * 0.5);
  highp vec2 tmpvar_14;
  tmpvar_14.x = tmpvar_13.x;
  tmpvar_14.y = (tmpvar_13.y * _ProjectionParams.x);
  o_12.xy = (tmpvar_14 + tmpvar_13.w);
  o_12.zw = tmpvar_8.zw;
  gl_Position = tmpvar_8;
  xlv_TEXCOORD0 = tmpvar_1;
  xlv_TEXCOORD1 = tmpvar_2;
  xlv_TEXCOORD2 = tmpvar_3;
  xlv_TEXCOORD3 = tmpvar_4;
  xlv_TEXCOORD4 = o_12;
  xlv_TEXCOORD5 = ((_glesMultiTexCoord1.xy * unity_LightmapST.xy) + unity_LightmapST.zw);
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform sampler2D _MainTex1;
uniform sampler2D _MainTex2;
in mediump vec4 xlv_TEXCOORD0;
in mediump vec2 xlv_TEXCOORD1;
in mediump vec4 xlv_TEXCOORD2;
in mediump vec4 xlv_TEXCOORD3;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 c_2;
  lowp vec4 c_3;
  mediump vec2 P_4;
  P_4 = (xlv_TEXCOORD1 + xlv_TEXCOORD3.zw);
  lowp vec4 tmpvar_5;
  tmpvar_5 = (texture (_MainTex1, xlv_TEXCOORD0.zw) * texture (_MainTex2, P_4));
  c_3.w = tmpvar_5.w;
  mediump vec3 tmpvar_6;
  tmpvar_6 = (tmpvar_5.xyz * xlv_TEXCOORD2.xyz);
  c_3.xyz = tmpvar_6;
  lowp vec3 tmpvar_7;
  tmpvar_7 = (c_3.xyz + texture (_MainTex, xlv_TEXCOORD0.xy).xyz);
  lowp vec4 c_8;
  c_8.xyz = vec3(0.0, 0.0, 0.0);
  c_8.w = 1.0;
  c_2 = c_8;
  c_2.xyz = (c_2.xyz + tmpvar_7);
  tmpvar_1 = c_2;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "LIGHTMAP_ON" "DIRLIGHTMAP_ON" "HDR_LIGHT_PREPASS_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "LIGHTMAP_ON" "DIRLIGHTMAP_ON" "HDR_LIGHT_PREPASS_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_ON" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_ON" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_ON" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "LIGHTMAP_ON" "DIRLIGHTMAP_OFF" "HDR_LIGHT_PREPASS_ON" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "LIGHTMAP_ON" "DIRLIGHTMAP_ON" "HDR_LIGHT_PREPASS_ON" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "LIGHTMAP_ON" "DIRLIGHTMAP_ON" "HDR_LIGHT_PREPASS_ON" }
"!!GLES3"
}
}
 }
}
}