Shader "Hidden/Camera-DepthTexture" {
Properties {
 _MainTex ("", 2D) = "white" {}
 _Cutoff ("", Float) = 0.5
 _Color ("", Color) = (1,1,1,1)
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
uniform highp mat4 glstate_matrix_mvp;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
}



#endif
#ifdef FRAGMENT

void main ()
{
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
uniform highp mat4 glstate_matrix_mvp;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
void main ()
{
  _glesFragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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
}
SubShader { 
 Tags { "RenderType"="TransparentCutout" }
 Pass {
  Tags { "RenderType"="TransparentCutout" }
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 _MainTex_ST;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
uniform lowp vec4 _Color;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  lowp float x_1;
  x_1 = ((texture2D (_MainTex, xlv_TEXCOORD0).w * _Color.w) - _Cutoff);
  if ((x_1 < 0.0)) {
    discard;
  };
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 _MainTex_ST;
out highp vec2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
uniform lowp vec4 _Color;
in highp vec2 xlv_TEXCOORD0;
void main ()
{
  lowp float x_1;
  x_1 = ((texture (_MainTex, xlv_TEXCOORD0).w * _Color.w) - _Cutoff);
  if ((x_1 < 0.0)) {
    discard;
  };
  _glesFragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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
}
SubShader { 
 Tags { "RenderType"="TreeBark" }
 Pass {
  Tags { "RenderType"="TreeBark" }
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 _Scale;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
uniform highp vec4 _Wind;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec4 tmpvar_2;
  tmpvar_2.w = _glesVertex.w;
  tmpvar_2.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_3;
  tmpvar_3.xy = tmpvar_1.xy;
  tmpvar_3.zw = _glesMultiTexCoord1.xy;
  highp vec4 pos_4;
  pos_4.w = tmpvar_2.w;
  highp vec3 bend_5;
  highp vec4 v_6;
  v_6.x = _Object2World[0].w;
  v_6.y = _Object2World[1].w;
  v_6.z = _Object2World[2].w;
  v_6.w = _Object2World[3].w;
  highp float tmpvar_7;
  tmpvar_7 = (dot (v_6.xyz, vec3(1.0, 1.0, 1.0)) + tmpvar_3.x);
  highp vec2 tmpvar_8;
  tmpvar_8.x = dot (tmpvar_2.xyz, vec3((tmpvar_3.y + tmpvar_7)));
  tmpvar_8.y = tmpvar_7;
  highp vec4 tmpvar_9;
  tmpvar_9 = abs(((
    fract((((
      fract(((_Time.yy + tmpvar_8).xxyy * vec4(1.975, 0.793, 0.375, 0.193)))
     * 2.0) - 1.0) + 0.5))
   * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = ((tmpvar_9 * tmpvar_9) * (3.0 - (2.0 * tmpvar_9)));
  highp vec2 tmpvar_11;
  tmpvar_11 = (tmpvar_10.xz + tmpvar_10.yw);
  bend_5.xz = ((tmpvar_3.y * 0.1) * _glesNormal).xz;
  bend_5.y = (_glesMultiTexCoord1.y * 0.3);
  pos_4.xyz = (tmpvar_2.xyz + ((
    (tmpvar_11.xyx * bend_5)
   + 
    ((_Wind.xyz * tmpvar_11.y) * _glesMultiTexCoord1.y)
  ) * _Wind.w));
  pos_4.xyz = (pos_4.xyz + (_glesMultiTexCoord1.x * _Wind.xyz));
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = mix ((pos_4.xyz - (
    (dot (_SquashPlaneNormal.xyz, pos_4.xyz) + _SquashPlaneNormal.w)
   * _SquashPlaneNormal.xyz)), pos_4.xyz, vec3(_SquashAmount));
  tmpvar_2 = tmpvar_12;
  gl_Position = (glstate_matrix_mvp * tmpvar_12);
}



#endif
#ifdef FRAGMENT

void main ()
{
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesColor;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 _Object2World;
uniform highp vec4 _Scale;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
uniform highp vec4 _Wind;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec4 tmpvar_2;
  tmpvar_2.w = _glesVertex.w;
  tmpvar_2.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_3;
  tmpvar_3.xy = tmpvar_1.xy;
  tmpvar_3.zw = _glesMultiTexCoord1.xy;
  highp vec4 pos_4;
  pos_4.w = tmpvar_2.w;
  highp vec3 bend_5;
  highp vec4 v_6;
  v_6.x = _Object2World[0].w;
  v_6.y = _Object2World[1].w;
  v_6.z = _Object2World[2].w;
  v_6.w = _Object2World[3].w;
  highp float tmpvar_7;
  tmpvar_7 = (dot (v_6.xyz, vec3(1.0, 1.0, 1.0)) + tmpvar_3.x);
  highp vec2 tmpvar_8;
  tmpvar_8.x = dot (tmpvar_2.xyz, vec3((tmpvar_3.y + tmpvar_7)));
  tmpvar_8.y = tmpvar_7;
  highp vec4 tmpvar_9;
  tmpvar_9 = abs(((
    fract((((
      fract(((_Time.yy + tmpvar_8).xxyy * vec4(1.975, 0.793, 0.375, 0.193)))
     * 2.0) - 1.0) + 0.5))
   * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = ((tmpvar_9 * tmpvar_9) * (3.0 - (2.0 * tmpvar_9)));
  highp vec2 tmpvar_11;
  tmpvar_11 = (tmpvar_10.xz + tmpvar_10.yw);
  bend_5.xz = ((tmpvar_3.y * 0.1) * _glesNormal).xz;
  bend_5.y = (_glesMultiTexCoord1.y * 0.3);
  pos_4.xyz = (tmpvar_2.xyz + ((
    (tmpvar_11.xyx * bend_5)
   + 
    ((_Wind.xyz * tmpvar_11.y) * _glesMultiTexCoord1.y)
  ) * _Wind.w));
  pos_4.xyz = (pos_4.xyz + (_glesMultiTexCoord1.x * _Wind.xyz));
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = mix ((pos_4.xyz - (
    (dot (_SquashPlaneNormal.xyz, pos_4.xyz) + _SquashPlaneNormal.w)
   * _SquashPlaneNormal.xyz)), pos_4.xyz, vec3(_SquashAmount));
  tmpvar_2 = tmpvar_12;
  gl_Position = (glstate_matrix_mvp * tmpvar_12);
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
void main ()
{
  _glesFragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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
}
SubShader { 
 Tags { "RenderType"="TreeLeaf" }
 Pass {
  Tags { "RenderType"="TreeLeaf" }
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesMultiTexCoord1;
attribute vec4 _glesTANGENT;
uniform highp vec4 _Time;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform highp mat4 _Object2World;
uniform highp vec4 _Scale;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
uniform highp vec4 _Wind;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec3 tmpvar_1;
  tmpvar_1 = _glesNormal;
  lowp vec4 tmpvar_2;
  tmpvar_2 = _glesColor;
  highp vec4 tmpvar_3;
  highp float tmpvar_4;
  tmpvar_4 = (1.0 - abs(_glesTANGENT.w));
  highp vec4 tmpvar_5;
  tmpvar_5.w = 0.0;
  tmpvar_5.xyz = tmpvar_1;
  highp vec4 tmpvar_6;
  tmpvar_6.zw = vec2(0.0, 0.0);
  tmpvar_6.xy = tmpvar_1.xy;
  highp vec4 tmpvar_7;
  tmpvar_7 = (_glesVertex + ((tmpvar_6 * glstate_matrix_invtrans_modelview0) * tmpvar_4));
  tmpvar_3.w = tmpvar_7.w;
  tmpvar_3.xyz = (tmpvar_7.xyz * _Scale.xyz);
  highp vec4 tmpvar_8;
  tmpvar_8.xy = tmpvar_2.xy;
  tmpvar_8.zw = _glesMultiTexCoord1.xy;
  highp vec4 pos_9;
  pos_9.w = tmpvar_3.w;
  highp vec3 bend_10;
  highp vec4 v_11;
  v_11.x = _Object2World[0].w;
  v_11.y = _Object2World[1].w;
  v_11.z = _Object2World[2].w;
  v_11.w = _Object2World[3].w;
  highp float tmpvar_12;
  tmpvar_12 = (dot (v_11.xyz, vec3(1.0, 1.0, 1.0)) + tmpvar_8.x);
  highp vec2 tmpvar_13;
  tmpvar_13.x = dot (tmpvar_3.xyz, vec3((tmpvar_8.y + tmpvar_12)));
  tmpvar_13.y = tmpvar_12;
  highp vec4 tmpvar_14;
  tmpvar_14 = abs(((
    fract((((
      fract(((_Time.yy + tmpvar_13).xxyy * vec4(1.975, 0.793, 0.375, 0.193)))
     * 2.0) - 1.0) + 0.5))
   * 2.0) - 1.0));
  highp vec4 tmpvar_15;
  tmpvar_15 = ((tmpvar_14 * tmpvar_14) * (3.0 - (2.0 * tmpvar_14)));
  highp vec2 tmpvar_16;
  tmpvar_16 = (tmpvar_15.xz + tmpvar_15.yw);
  bend_10.xz = ((tmpvar_8.y * 0.1) * mix (_glesNormal, normalize(
    (tmpvar_5 * glstate_matrix_invtrans_modelview0)
  ).xyz, vec3(tmpvar_4))).xz;
  bend_10.y = (_glesMultiTexCoord1.y * 0.3);
  pos_9.xyz = (tmpvar_3.xyz + ((
    (tmpvar_16.xyx * bend_10)
   + 
    ((_Wind.xyz * tmpvar_16.y) * _glesMultiTexCoord1.y)
  ) * _Wind.w));
  pos_9.xyz = (pos_9.xyz + (_glesMultiTexCoord1.x * _Wind.xyz));
  highp vec4 tmpvar_17;
  tmpvar_17.w = 1.0;
  tmpvar_17.xyz = mix ((pos_9.xyz - (
    (dot (_SquashPlaneNormal.xyz, pos_9.xyz) + _SquashPlaneNormal.w)
   * _SquashPlaneNormal.xyz)), pos_9.xyz, vec3(_SquashAmount));
  tmpvar_3 = tmpvar_17;
  gl_Position = (glstate_matrix_mvp * tmpvar_17);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  mediump float alpha_1;
  lowp float tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0).w;
  alpha_1 = tmpvar_2;
  mediump float x_3;
  x_3 = (alpha_1 - _Cutoff);
  if ((x_3 < 0.0)) {
    discard;
  };
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesColor;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
in vec4 _glesMultiTexCoord1;
in vec4 _glesTANGENT;
uniform highp vec4 _Time;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform highp mat4 _Object2World;
uniform highp vec4 _Scale;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
uniform highp vec4 _Wind;
out highp vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec3 tmpvar_1;
  tmpvar_1 = _glesNormal;
  lowp vec4 tmpvar_2;
  tmpvar_2 = _glesColor;
  highp vec4 tmpvar_3;
  highp float tmpvar_4;
  tmpvar_4 = (1.0 - abs(_glesTANGENT.w));
  highp vec4 tmpvar_5;
  tmpvar_5.w = 0.0;
  tmpvar_5.xyz = tmpvar_1;
  highp vec4 tmpvar_6;
  tmpvar_6.zw = vec2(0.0, 0.0);
  tmpvar_6.xy = tmpvar_1.xy;
  highp vec4 tmpvar_7;
  tmpvar_7 = (_glesVertex + ((tmpvar_6 * glstate_matrix_invtrans_modelview0) * tmpvar_4));
  tmpvar_3.w = tmpvar_7.w;
  tmpvar_3.xyz = (tmpvar_7.xyz * _Scale.xyz);
  highp vec4 tmpvar_8;
  tmpvar_8.xy = tmpvar_2.xy;
  tmpvar_8.zw = _glesMultiTexCoord1.xy;
  highp vec4 pos_9;
  pos_9.w = tmpvar_3.w;
  highp vec3 bend_10;
  highp vec4 v_11;
  v_11.x = _Object2World[0].w;
  v_11.y = _Object2World[1].w;
  v_11.z = _Object2World[2].w;
  v_11.w = _Object2World[3].w;
  highp float tmpvar_12;
  tmpvar_12 = (dot (v_11.xyz, vec3(1.0, 1.0, 1.0)) + tmpvar_8.x);
  highp vec2 tmpvar_13;
  tmpvar_13.x = dot (tmpvar_3.xyz, vec3((tmpvar_8.y + tmpvar_12)));
  tmpvar_13.y = tmpvar_12;
  highp vec4 tmpvar_14;
  tmpvar_14 = abs(((
    fract((((
      fract(((_Time.yy + tmpvar_13).xxyy * vec4(1.975, 0.793, 0.375, 0.193)))
     * 2.0) - 1.0) + 0.5))
   * 2.0) - 1.0));
  highp vec4 tmpvar_15;
  tmpvar_15 = ((tmpvar_14 * tmpvar_14) * (3.0 - (2.0 * tmpvar_14)));
  highp vec2 tmpvar_16;
  tmpvar_16 = (tmpvar_15.xz + tmpvar_15.yw);
  bend_10.xz = ((tmpvar_8.y * 0.1) * mix (_glesNormal, normalize(
    (tmpvar_5 * glstate_matrix_invtrans_modelview0)
  ).xyz, vec3(tmpvar_4))).xz;
  bend_10.y = (_glesMultiTexCoord1.y * 0.3);
  pos_9.xyz = (tmpvar_3.xyz + ((
    (tmpvar_16.xyx * bend_10)
   + 
    ((_Wind.xyz * tmpvar_16.y) * _glesMultiTexCoord1.y)
  ) * _Wind.w));
  pos_9.xyz = (pos_9.xyz + (_glesMultiTexCoord1.x * _Wind.xyz));
  highp vec4 tmpvar_17;
  tmpvar_17.w = 1.0;
  tmpvar_17.xyz = mix ((pos_9.xyz - (
    (dot (_SquashPlaneNormal.xyz, pos_9.xyz) + _SquashPlaneNormal.w)
   * _SquashPlaneNormal.xyz)), pos_9.xyz, vec3(_SquashAmount));
  tmpvar_3 = tmpvar_17;
  gl_Position = (glstate_matrix_mvp * tmpvar_17);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
in highp vec2 xlv_TEXCOORD0;
void main ()
{
  mediump float alpha_1;
  lowp float tmpvar_2;
  tmpvar_2 = texture (_MainTex, xlv_TEXCOORD0).w;
  alpha_1 = tmpvar_2;
  mediump float x_3;
  x_3 = (alpha_1 - _Cutoff);
  if ((x_3 < 0.0)) {
    discard;
  };
  _glesFragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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
}
SubShader { 
 Tags { "RenderType"="TreeOpaque" }
 Pass {
  Tags { "RenderType"="TreeOpaque" }
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 _Scale;
uniform highp mat4 _TerrainEngineBendTree;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec4 pos_2;
  pos_2.w = _glesVertex.w;
  highp float alpha_3;
  alpha_3 = tmpvar_1.w;
  pos_2.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_4;
  tmpvar_4.w = 0.0;
  tmpvar_4.xyz = pos_2.xyz;
  pos_2.xyz = mix (pos_2.xyz, (_TerrainEngineBendTree * tmpvar_4).xyz, vec3(alpha_3));
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = mix ((pos_2.xyz - (
    (dot (_SquashPlaneNormal.xyz, pos_2.xyz) + _SquashPlaneNormal.w)
   * _SquashPlaneNormal.xyz)), pos_2.xyz, vec3(_SquashAmount));
  pos_2 = tmpvar_5;
  gl_Position = (glstate_matrix_mvp * tmpvar_5);
}



#endif
#ifdef FRAGMENT

void main ()
{
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesColor;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 _Scale;
uniform highp mat4 _TerrainEngineBendTree;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec4 pos_2;
  pos_2.w = _glesVertex.w;
  highp float alpha_3;
  alpha_3 = tmpvar_1.w;
  pos_2.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_4;
  tmpvar_4.w = 0.0;
  tmpvar_4.xyz = pos_2.xyz;
  pos_2.xyz = mix (pos_2.xyz, (_TerrainEngineBendTree * tmpvar_4).xyz, vec3(alpha_3));
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = mix ((pos_2.xyz - (
    (dot (_SquashPlaneNormal.xyz, pos_2.xyz) + _SquashPlaneNormal.w)
   * _SquashPlaneNormal.xyz)), pos_2.xyz, vec3(_SquashAmount));
  pos_2 = tmpvar_5;
  gl_Position = (glstate_matrix_mvp * tmpvar_5);
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
void main ()
{
  _glesFragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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
}
SubShader { 
 Tags { "RenderType"="TreeTransparentCutout" }
 Pass {
  Tags { "RenderType"="TreeTransparentCutout" }
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 _Scale;
uniform highp mat4 _TerrainEngineBendTree;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec4 pos_2;
  pos_2.w = _glesVertex.w;
  highp float alpha_3;
  alpha_3 = tmpvar_1.w;
  pos_2.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_4;
  tmpvar_4.w = 0.0;
  tmpvar_4.xyz = pos_2.xyz;
  pos_2.xyz = mix (pos_2.xyz, (_TerrainEngineBendTree * tmpvar_4).xyz, vec3(alpha_3));
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = mix ((pos_2.xyz - (
    (dot (_SquashPlaneNormal.xyz, pos_2.xyz) + _SquashPlaneNormal.w)
   * _SquashPlaneNormal.xyz)), pos_2.xyz, vec3(_SquashAmount));
  pos_2 = tmpvar_5;
  gl_Position = (glstate_matrix_mvp * tmpvar_5);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  mediump float alpha_1;
  lowp float tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0).w;
  alpha_1 = tmpvar_2;
  mediump float x_3;
  x_3 = (alpha_1 - _Cutoff);
  if ((x_3 < 0.0)) {
    discard;
  };
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesColor;
in vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 _Scale;
uniform highp mat4 _TerrainEngineBendTree;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
out highp vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec4 pos_2;
  pos_2.w = _glesVertex.w;
  highp float alpha_3;
  alpha_3 = tmpvar_1.w;
  pos_2.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_4;
  tmpvar_4.w = 0.0;
  tmpvar_4.xyz = pos_2.xyz;
  pos_2.xyz = mix (pos_2.xyz, (_TerrainEngineBendTree * tmpvar_4).xyz, vec3(alpha_3));
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = mix ((pos_2.xyz - (
    (dot (_SquashPlaneNormal.xyz, pos_2.xyz) + _SquashPlaneNormal.w)
   * _SquashPlaneNormal.xyz)), pos_2.xyz, vec3(_SquashAmount));
  pos_2 = tmpvar_5;
  gl_Position = (glstate_matrix_mvp * tmpvar_5);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
in highp vec2 xlv_TEXCOORD0;
void main ()
{
  mediump float alpha_1;
  lowp float tmpvar_2;
  tmpvar_2 = texture (_MainTex, xlv_TEXCOORD0).w;
  alpha_1 = tmpvar_2;
  mediump float x_3;
  x_3 = (alpha_1 - _Cutoff);
  if ((x_3 < 0.0)) {
    discard;
  };
  _glesFragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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
}
SubShader { 
 Tags { "RenderType"="TreeBillboard" }
 Pass {
  Tags { "RenderType"="TreeBillboard" }
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesMultiTexCoord1;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec3 _TreeBillboardCameraRight;
uniform highp vec4 _TreeBillboardCameraUp;
uniform highp vec4 _TreeBillboardCameraFront;
uniform highp vec4 _TreeBillboardCameraPos;
uniform highp vec4 _TreeBillboardDistances;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0;
  highp vec2 tmpvar_2;
  highp vec4 pos_3;
  pos_3 = _glesVertex;
  highp vec2 offset_4;
  offset_4 = _glesMultiTexCoord1.xy;
  highp float offsetz_5;
  offsetz_5 = tmpvar_1.y;
  highp vec3 tmpvar_6;
  tmpvar_6 = (_glesVertex.xyz - _TreeBillboardCameraPos.xyz);
  highp float tmpvar_7;
  tmpvar_7 = dot (tmpvar_6, tmpvar_6);
  if ((tmpvar_7 > _TreeBillboardDistances.x)) {
    offsetz_5 = 0.0;
    offset_4 = vec2(0.0, 0.0);
  };
  pos_3.xyz = (_glesVertex.xyz + (_TreeBillboardCameraRight * offset_4.x));
  pos_3.xyz = (pos_3.xyz + (_TreeBillboardCameraUp.xyz * mix (offset_4.y, offsetz_5, _TreeBillboardCameraPos.w)));
  pos_3.xyz = (pos_3.xyz + ((_TreeBillboardCameraFront.xyz * 
    abs(offset_4.x)
  ) * _TreeBillboardCameraUp.w));
  tmpvar_2.x = tmpvar_1.x;
  tmpvar_2.y = float((_glesMultiTexCoord0.y > 0.0));
  gl_Position = (glstate_matrix_mvp * pos_3);
  xlv_TEXCOORD0 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  lowp float x_1;
  x_1 = (texture2D (_MainTex, xlv_TEXCOORD0).w - 0.001);
  if ((x_1 < 0.0)) {
    discard;
  };
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
in vec4 _glesMultiTexCoord1;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec3 _TreeBillboardCameraRight;
uniform highp vec4 _TreeBillboardCameraUp;
uniform highp vec4 _TreeBillboardCameraFront;
uniform highp vec4 _TreeBillboardCameraPos;
uniform highp vec4 _TreeBillboardDistances;
out highp vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0;
  highp vec2 tmpvar_2;
  highp vec4 pos_3;
  pos_3 = _glesVertex;
  highp vec2 offset_4;
  offset_4 = _glesMultiTexCoord1.xy;
  highp float offsetz_5;
  offsetz_5 = tmpvar_1.y;
  highp vec3 tmpvar_6;
  tmpvar_6 = (_glesVertex.xyz - _TreeBillboardCameraPos.xyz);
  highp float tmpvar_7;
  tmpvar_7 = dot (tmpvar_6, tmpvar_6);
  if ((tmpvar_7 > _TreeBillboardDistances.x)) {
    offsetz_5 = 0.0;
    offset_4 = vec2(0.0, 0.0);
  };
  pos_3.xyz = (_glesVertex.xyz + (_TreeBillboardCameraRight * offset_4.x));
  pos_3.xyz = (pos_3.xyz + (_TreeBillboardCameraUp.xyz * mix (offset_4.y, offsetz_5, _TreeBillboardCameraPos.w)));
  pos_3.xyz = (pos_3.xyz + ((_TreeBillboardCameraFront.xyz * 
    abs(offset_4.x)
  ) * _TreeBillboardCameraUp.w));
  tmpvar_2.x = tmpvar_1.x;
  tmpvar_2.y = float((_glesMultiTexCoord0.y > 0.0));
  gl_Position = (glstate_matrix_mvp * pos_3);
  xlv_TEXCOORD0 = tmpvar_2;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
in highp vec2 xlv_TEXCOORD0;
void main ()
{
  lowp float x_1;
  x_1 = (texture (_MainTex, xlv_TEXCOORD0).w - 0.001);
  if ((x_1 < 0.0)) {
    discard;
  };
  _glesFragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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
}
SubShader { 
 Tags { "RenderType"="GrassBillboard" }
 Pass {
  Tags { "RenderType"="GrassBillboard" }
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesTANGENT;
uniform highp mat4 glstate_matrix_mvp;
uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec4 _CameraPosition;
uniform highp vec3 _CameraRight;
uniform highp vec3 _CameraUp;
varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0;
  lowp vec4 tmpvar_2;
  tmpvar_2 = _glesColor;
  highp vec4 pos_3;
  pos_3 = _glesVertex;
  highp vec2 offset_4;
  offset_4 = _glesTANGENT.xy;
  highp vec3 tmpvar_5;
  tmpvar_5 = (_glesVertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_6;
  tmpvar_6 = dot (tmpvar_5, tmpvar_5);
  if ((tmpvar_6 > _WaveAndDistance.w)) {
    offset_4 = vec2(0.0, 0.0);
  };
  pos_3.xyz = (_glesVertex.xyz + (offset_4.x * _CameraRight));
  pos_3.xyz = (pos_3.xyz + (offset_4.y * _CameraUp));
  highp vec4 vertex_7;
  vertex_7.yw = pos_3.yw;
  lowp vec4 color_8;
  color_8.xyz = tmpvar_2.xyz;
  lowp vec3 waveColor_9;
  highp vec3 waveMove_10;
  highp vec4 tmpvar_11;
  tmpvar_11 = ((fract(
    (((pos_3.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos_3.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))
  ) * 6.40885) - 3.14159);
  highp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * tmpvar_11);
  highp vec4 tmpvar_13;
  tmpvar_13 = (tmpvar_12 * tmpvar_11);
  highp vec4 tmpvar_14;
  tmpvar_14 = (tmpvar_13 * tmpvar_12);
  highp vec4 tmpvar_15;
  tmpvar_15 = (((tmpvar_11 + 
    (tmpvar_13 * -0.161616)
  ) + (tmpvar_14 * 0.0083333)) + ((tmpvar_14 * tmpvar_12) * -0.00019841));
  highp vec4 tmpvar_16;
  tmpvar_16 = (tmpvar_15 * tmpvar_15);
  highp vec4 tmpvar_17;
  tmpvar_17 = (tmpvar_16 * tmpvar_16);
  highp vec4 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * _glesTANGENT.y);
  waveMove_10.y = 0.0;
  waveMove_10.x = dot (tmpvar_18, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove_10.z = dot (tmpvar_18, vec4(0.006, 0.02, -0.02, 0.1));
  vertex_7.xz = (pos_3.xz - (waveMove_10.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_19;
  tmpvar_19 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_17, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor_9 = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (vertex_7.xyz - _CameraPosition.xyz);
  highp float tmpvar_21;
  tmpvar_21 = clamp (((2.0 * 
    (_WaveAndDistance.w - dot (tmpvar_20, tmpvar_20))
  ) * _CameraPosition.w), 0.0, 1.0);
  color_8.w = tmpvar_21;
  lowp vec4 tmpvar_22;
  tmpvar_22.xyz = ((2.0 * waveColor_9) * _glesColor.xyz);
  tmpvar_22.w = color_8.w;
  gl_Position = (glstate_matrix_mvp * vertex_7);
  xlv_COLOR = tmpvar_22;
  xlv_TEXCOORD0 = tmpvar_1.xy;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  lowp float x_1;
  x_1 = ((texture2D (_MainTex, xlv_TEXCOORD0).w * xlv_COLOR.w) - _Cutoff);
  if ((x_1 < 0.0)) {
    discard;
  };
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesColor;
in vec4 _glesMultiTexCoord0;
in vec4 _glesTANGENT;
uniform highp mat4 glstate_matrix_mvp;
uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec4 _CameraPosition;
uniform highp vec3 _CameraRight;
uniform highp vec3 _CameraUp;
out lowp vec4 xlv_COLOR;
out highp vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0;
  lowp vec4 tmpvar_2;
  tmpvar_2 = _glesColor;
  highp vec4 pos_3;
  pos_3 = _glesVertex;
  highp vec2 offset_4;
  offset_4 = _glesTANGENT.xy;
  highp vec3 tmpvar_5;
  tmpvar_5 = (_glesVertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_6;
  tmpvar_6 = dot (tmpvar_5, tmpvar_5);
  if ((tmpvar_6 > _WaveAndDistance.w)) {
    offset_4 = vec2(0.0, 0.0);
  };
  pos_3.xyz = (_glesVertex.xyz + (offset_4.x * _CameraRight));
  pos_3.xyz = (pos_3.xyz + (offset_4.y * _CameraUp));
  highp vec4 vertex_7;
  vertex_7.yw = pos_3.yw;
  lowp vec4 color_8;
  color_8.xyz = tmpvar_2.xyz;
  lowp vec3 waveColor_9;
  highp vec3 waveMove_10;
  highp vec4 tmpvar_11;
  tmpvar_11 = ((fract(
    (((pos_3.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos_3.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))
  ) * 6.40885) - 3.14159);
  highp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * tmpvar_11);
  highp vec4 tmpvar_13;
  tmpvar_13 = (tmpvar_12 * tmpvar_11);
  highp vec4 tmpvar_14;
  tmpvar_14 = (tmpvar_13 * tmpvar_12);
  highp vec4 tmpvar_15;
  tmpvar_15 = (((tmpvar_11 + 
    (tmpvar_13 * -0.161616)
  ) + (tmpvar_14 * 0.0083333)) + ((tmpvar_14 * tmpvar_12) * -0.00019841));
  highp vec4 tmpvar_16;
  tmpvar_16 = (tmpvar_15 * tmpvar_15);
  highp vec4 tmpvar_17;
  tmpvar_17 = (tmpvar_16 * tmpvar_16);
  highp vec4 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * _glesTANGENT.y);
  waveMove_10.y = 0.0;
  waveMove_10.x = dot (tmpvar_18, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove_10.z = dot (tmpvar_18, vec4(0.006, 0.02, -0.02, 0.1));
  vertex_7.xz = (pos_3.xz - (waveMove_10.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_19;
  tmpvar_19 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_17, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor_9 = tmpvar_19;
  highp vec3 tmpvar_20;
  tmpvar_20 = (vertex_7.xyz - _CameraPosition.xyz);
  highp float tmpvar_21;
  tmpvar_21 = clamp (((2.0 * 
    (_WaveAndDistance.w - dot (tmpvar_20, tmpvar_20))
  ) * _CameraPosition.w), 0.0, 1.0);
  color_8.w = tmpvar_21;
  lowp vec4 tmpvar_22;
  tmpvar_22.xyz = ((2.0 * waveColor_9) * _glesColor.xyz);
  tmpvar_22.w = color_8.w;
  gl_Position = (glstate_matrix_mvp * vertex_7);
  xlv_COLOR = tmpvar_22;
  xlv_TEXCOORD0 = tmpvar_1.xy;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
in lowp vec4 xlv_COLOR;
in highp vec2 xlv_TEXCOORD0;
void main ()
{
  lowp float x_1;
  x_1 = ((texture (_MainTex, xlv_TEXCOORD0).w * xlv_COLOR.w) - _Cutoff);
  if ((x_1 < 0.0)) {
    discard;
  };
  _glesFragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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
}
SubShader { 
 Tags { "RenderType"="Grass" }
 Pass {
  Tags { "RenderType"="Grass" }
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec4 _CameraPosition;
varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec4 vertex_1;
  vertex_1.yw = _glesVertex.yw;
  lowp vec4 color_2;
  color_2.xyz = _glesColor.xyz;
  lowp vec3 waveColor_3;
  highp vec3 waveMove_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = ((fract(
    (((_glesVertex.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (_glesVertex.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))
  ) * 6.40885) - 3.14159);
  highp vec4 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * tmpvar_5);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * tmpvar_5);
  highp vec4 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_6);
  highp vec4 tmpvar_9;
  tmpvar_9 = (((tmpvar_5 + 
    (tmpvar_7 * -0.161616)
  ) + (tmpvar_8 * 0.0083333)) + ((tmpvar_8 * tmpvar_6) * -0.00019841));
  highp vec4 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * tmpvar_9);
  highp vec4 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * tmpvar_10);
  highp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * (_glesColor.w * _WaveAndDistance.z));
  waveMove_4.y = 0.0;
  waveMove_4.x = dot (tmpvar_12, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove_4.z = dot (tmpvar_12, vec4(0.006, 0.02, -0.02, 0.1));
  vertex_1.xz = (_glesVertex.xz - (waveMove_4.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_13;
  tmpvar_13 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_11, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor_3 = tmpvar_13;
  highp vec3 tmpvar_14;
  tmpvar_14 = (vertex_1.xyz - _CameraPosition.xyz);
  highp float tmpvar_15;
  tmpvar_15 = clamp (((2.0 * 
    (_WaveAndDistance.w - dot (tmpvar_14, tmpvar_14))
  ) * _CameraPosition.w), 0.0, 1.0);
  color_2.w = tmpvar_15;
  lowp vec4 tmpvar_16;
  tmpvar_16.xyz = ((2.0 * waveColor_3) * _glesColor.xyz);
  tmpvar_16.w = color_2.w;
  gl_Position = (glstate_matrix_mvp * vertex_1);
  xlv_COLOR = tmpvar_16;
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  lowp float x_1;
  x_1 = ((texture2D (_MainTex, xlv_TEXCOORD0).w * xlv_COLOR.w) - _Cutoff);
  if ((x_1 < 0.0)) {
    discard;
  };
  gl_FragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesColor;
in vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec4 _CameraPosition;
out lowp vec4 xlv_COLOR;
out highp vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec4 vertex_1;
  vertex_1.yw = _glesVertex.yw;
  lowp vec4 color_2;
  color_2.xyz = _glesColor.xyz;
  lowp vec3 waveColor_3;
  highp vec3 waveMove_4;
  highp vec4 tmpvar_5;
  tmpvar_5 = ((fract(
    (((_glesVertex.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (_glesVertex.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))
  ) * 6.40885) - 3.14159);
  highp vec4 tmpvar_6;
  tmpvar_6 = (tmpvar_5 * tmpvar_5);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * tmpvar_5);
  highp vec4 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_6);
  highp vec4 tmpvar_9;
  tmpvar_9 = (((tmpvar_5 + 
    (tmpvar_7 * -0.161616)
  ) + (tmpvar_8 * 0.0083333)) + ((tmpvar_8 * tmpvar_6) * -0.00019841));
  highp vec4 tmpvar_10;
  tmpvar_10 = (tmpvar_9 * tmpvar_9);
  highp vec4 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * tmpvar_10);
  highp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * (_glesColor.w * _WaveAndDistance.z));
  waveMove_4.y = 0.0;
  waveMove_4.x = dot (tmpvar_12, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove_4.z = dot (tmpvar_12, vec4(0.006, 0.02, -0.02, 0.1));
  vertex_1.xz = (_glesVertex.xz - (waveMove_4.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_13;
  tmpvar_13 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_11, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor_3 = tmpvar_13;
  highp vec3 tmpvar_14;
  tmpvar_14 = (vertex_1.xyz - _CameraPosition.xyz);
  highp float tmpvar_15;
  tmpvar_15 = clamp (((2.0 * 
    (_WaveAndDistance.w - dot (tmpvar_14, tmpvar_14))
  ) * _CameraPosition.w), 0.0, 1.0);
  color_2.w = tmpvar_15;
  lowp vec4 tmpvar_16;
  tmpvar_16.xyz = ((2.0 * waveColor_3) * _glesColor.xyz);
  tmpvar_16.w = color_2.w;
  gl_Position = (glstate_matrix_mvp * vertex_1);
  xlv_COLOR = tmpvar_16;
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
in lowp vec4 xlv_COLOR;
in highp vec2 xlv_TEXCOORD0;
void main ()
{
  lowp float x_1;
  x_1 = ((texture (_MainTex, xlv_TEXCOORD0).w * xlv_COLOR.w) - _Cutoff);
  if ((x_1 < 0.0)) {
    discard;
  };
  _glesFragData[0] = vec4(0.0, 0.0, 0.0, 0.0);
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
}
Fallback Off
}