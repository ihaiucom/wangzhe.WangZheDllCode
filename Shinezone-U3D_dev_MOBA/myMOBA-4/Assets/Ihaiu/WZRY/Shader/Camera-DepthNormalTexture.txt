Shader "Hidden/Camera-DepthNormalTexture" {
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
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
varying highp vec4 xlv_TEXCOORD0;
void main ()
{
  highp vec4 tmpvar_1;
  highp mat3 tmpvar_2;
  tmpvar_2[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_2[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_2[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  tmpvar_1.xyz = (tmpvar_2 * normalize(_glesNormal));
  tmpvar_1.w = -(((glstate_matrix_modelview0 * _glesVertex).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 enc_2;
  enc_2.xy = (((
    (xlv_TEXCOORD0.xy / (xlv_TEXCOORD0.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_3;
  highp vec2 tmpvar_4;
  tmpvar_4 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD0.w));
  enc_3.y = tmpvar_4.y;
  enc_3.x = (tmpvar_4.x - (tmpvar_4.y * 0.00392157));
  enc_2.zw = enc_3;
  tmpvar_1 = enc_2;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
out highp vec4 xlv_TEXCOORD0;
void main ()
{
  highp vec4 tmpvar_1;
  highp mat3 tmpvar_2;
  tmpvar_2[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_2[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_2[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  tmpvar_1.xyz = (tmpvar_2 * normalize(_glesNormal));
  tmpvar_1.w = -(((glstate_matrix_modelview0 * _glesVertex).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_1;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
in highp vec4 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 enc_2;
  enc_2.xy = (((
    (xlv_TEXCOORD0.xy / (xlv_TEXCOORD0.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_3;
  highp vec2 tmpvar_4;
  tmpvar_4 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD0.w));
  enc_3.y = tmpvar_4.y;
  enc_3.x = (tmpvar_4.x - (tmpvar_4.y * 0.00392157));
  enc_2.zw = enc_3;
  tmpvar_1 = enc_2;
  _glesFragData[0] = tmpvar_1;
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
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform highp vec4 _MainTex_ST;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  highp mat3 tmpvar_2;
  tmpvar_2[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_2[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_2[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  tmpvar_1.xyz = (tmpvar_2 * normalize(_glesNormal));
  tmpvar_1.w = -(((glstate_matrix_modelview0 * _glesVertex).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
uniform lowp vec4 _Color;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp float x_2;
  x_2 = ((texture2D (_MainTex, xlv_TEXCOORD0).w * _Color.w) - _Cutoff);
  if ((x_2 < 0.0)) {
    discard;
  };
  highp vec4 enc_3;
  enc_3.xy = (((
    (xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_4.y = tmpvar_5.y;
  enc_4.x = (tmpvar_5.x - (tmpvar_5.y * 0.00392157));
  enc_3.zw = enc_4;
  tmpvar_1 = enc_3;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform highp vec4 _MainTex_ST;
out highp vec2 xlv_TEXCOORD0;
out highp vec4 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  highp mat3 tmpvar_2;
  tmpvar_2[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_2[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_2[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  tmpvar_1.xyz = (tmpvar_2 * normalize(_glesNormal));
  tmpvar_1.w = -(((glstate_matrix_modelview0 * _glesVertex).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
  xlv_TEXCOORD1 = tmpvar_1;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
uniform lowp vec4 _Color;
in highp vec2 xlv_TEXCOORD0;
in highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp float x_2;
  x_2 = ((texture (_MainTex, xlv_TEXCOORD0).w * _Color.w) - _Cutoff);
  if ((x_2 < 0.0)) {
    discard;
  };
  highp vec4 enc_3;
  enc_3.xy = (((
    (xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_4.y = tmpvar_5.y;
  enc_4.x = (tmpvar_5.x - (tmpvar_5.y * 0.00392157));
  enc_3.zw = enc_4;
  tmpvar_1 = enc_3;
  _glesFragData[0] = tmpvar_1;
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
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesMultiTexCoord1;
uniform highp vec4 _Time;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform highp mat4 _Object2World;
uniform highp vec4 _Scale;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
uniform highp vec4 _Wind;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec4 tmpvar_2;
  highp vec4 tmpvar_3;
  tmpvar_3.w = _glesVertex.w;
  tmpvar_3.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_4;
  tmpvar_4.xy = tmpvar_1.xy;
  tmpvar_4.zw = _glesMultiTexCoord1.xy;
  highp vec4 pos_5;
  pos_5.w = tmpvar_3.w;
  highp vec3 bend_6;
  highp vec4 v_7;
  v_7.x = _Object2World[0].w;
  v_7.y = _Object2World[1].w;
  v_7.z = _Object2World[2].w;
  v_7.w = _Object2World[3].w;
  highp float tmpvar_8;
  tmpvar_8 = (dot (v_7.xyz, vec3(1.0, 1.0, 1.0)) + tmpvar_4.x);
  highp vec2 tmpvar_9;
  tmpvar_9.x = dot (tmpvar_3.xyz, vec3((tmpvar_4.y + tmpvar_8)));
  tmpvar_9.y = tmpvar_8;
  highp vec4 tmpvar_10;
  tmpvar_10 = abs(((
    fract((((
      fract(((_Time.yy + tmpvar_9).xxyy * vec4(1.975, 0.793, 0.375, 0.193)))
     * 2.0) - 1.0) + 0.5))
   * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = ((tmpvar_10 * tmpvar_10) * (3.0 - (2.0 * tmpvar_10)));
  highp vec2 tmpvar_12;
  tmpvar_12 = (tmpvar_11.xz + tmpvar_11.yw);
  bend_6.xz = ((tmpvar_4.y * 0.1) * _glesNormal).xz;
  bend_6.y = (_glesMultiTexCoord1.y * 0.3);
  pos_5.xyz = (tmpvar_3.xyz + ((
    (tmpvar_12.xyx * bend_6)
   + 
    ((_Wind.xyz * tmpvar_12.y) * _glesMultiTexCoord1.y)
  ) * _Wind.w));
  pos_5.xyz = (pos_5.xyz + (_glesMultiTexCoord1.x * _Wind.xyz));
  highp vec4 tmpvar_13;
  tmpvar_13.w = 1.0;
  tmpvar_13.xyz = mix ((pos_5.xyz - (
    (dot (_SquashPlaneNormal.xyz, pos_5.xyz) + _SquashPlaneNormal.w)
   * _SquashPlaneNormal.xyz)), pos_5.xyz, vec3(_SquashAmount));
  tmpvar_3 = tmpvar_13;
  highp mat3 tmpvar_14;
  tmpvar_14[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_14[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_14[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  tmpvar_2.xyz = (tmpvar_14 * normalize(_glesNormal));
  tmpvar_2.w = -(((glstate_matrix_modelview0 * tmpvar_13).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * tmpvar_13);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 enc_2;
  enc_2.xy = (((
    (xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_3;
  highp vec2 tmpvar_4;
  tmpvar_4 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_3.y = tmpvar_4.y;
  enc_3.x = (tmpvar_4.x - (tmpvar_4.y * 0.00392157));
  enc_2.zw = enc_3;
  tmpvar_1 = enc_2;
  gl_FragData[0] = tmpvar_1;
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
uniform highp vec4 _Time;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform highp mat4 _Object2World;
uniform highp vec4 _Scale;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
uniform highp vec4 _Wind;
out highp vec2 xlv_TEXCOORD0;
out highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec4 tmpvar_2;
  highp vec4 tmpvar_3;
  tmpvar_3.w = _glesVertex.w;
  tmpvar_3.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_4;
  tmpvar_4.xy = tmpvar_1.xy;
  tmpvar_4.zw = _glesMultiTexCoord1.xy;
  highp vec4 pos_5;
  pos_5.w = tmpvar_3.w;
  highp vec3 bend_6;
  highp vec4 v_7;
  v_7.x = _Object2World[0].w;
  v_7.y = _Object2World[1].w;
  v_7.z = _Object2World[2].w;
  v_7.w = _Object2World[3].w;
  highp float tmpvar_8;
  tmpvar_8 = (dot (v_7.xyz, vec3(1.0, 1.0, 1.0)) + tmpvar_4.x);
  highp vec2 tmpvar_9;
  tmpvar_9.x = dot (tmpvar_3.xyz, vec3((tmpvar_4.y + tmpvar_8)));
  tmpvar_9.y = tmpvar_8;
  highp vec4 tmpvar_10;
  tmpvar_10 = abs(((
    fract((((
      fract(((_Time.yy + tmpvar_9).xxyy * vec4(1.975, 0.793, 0.375, 0.193)))
     * 2.0) - 1.0) + 0.5))
   * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = ((tmpvar_10 * tmpvar_10) * (3.0 - (2.0 * tmpvar_10)));
  highp vec2 tmpvar_12;
  tmpvar_12 = (tmpvar_11.xz + tmpvar_11.yw);
  bend_6.xz = ((tmpvar_4.y * 0.1) * _glesNormal).xz;
  bend_6.y = (_glesMultiTexCoord1.y * 0.3);
  pos_5.xyz = (tmpvar_3.xyz + ((
    (tmpvar_12.xyx * bend_6)
   + 
    ((_Wind.xyz * tmpvar_12.y) * _glesMultiTexCoord1.y)
  ) * _Wind.w));
  pos_5.xyz = (pos_5.xyz + (_glesMultiTexCoord1.x * _Wind.xyz));
  highp vec4 tmpvar_13;
  tmpvar_13.w = 1.0;
  tmpvar_13.xyz = mix ((pos_5.xyz - (
    (dot (_SquashPlaneNormal.xyz, pos_5.xyz) + _SquashPlaneNormal.w)
   * _SquashPlaneNormal.xyz)), pos_5.xyz, vec3(_SquashAmount));
  tmpvar_3 = tmpvar_13;
  highp mat3 tmpvar_14;
  tmpvar_14[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_14[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_14[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  tmpvar_2.xyz = (tmpvar_14 * normalize(_glesNormal));
  tmpvar_2.w = -(((glstate_matrix_modelview0 * tmpvar_13).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * tmpvar_13);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_2;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
in highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 enc_2;
  enc_2.xy = (((
    (xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_3;
  highp vec2 tmpvar_4;
  tmpvar_4 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_3.y = tmpvar_4.y;
  enc_3.x = (tmpvar_4.x - (tmpvar_4.y * 0.00392157));
  enc_2.zw = enc_3;
  tmpvar_1 = enc_2;
  _glesFragData[0] = tmpvar_1;
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
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform highp mat4 _Object2World;
uniform highp vec4 _Scale;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
uniform highp vec4 _Wind;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  highp vec3 tmpvar_1;
  tmpvar_1 = _glesNormal;
  lowp vec4 tmpvar_2;
  tmpvar_2 = _glesColor;
  highp vec4 tmpvar_3;
  highp vec4 tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0 - abs(_glesTANGENT.w));
  highp vec4 tmpvar_6;
  tmpvar_6.w = 0.0;
  tmpvar_6.xyz = tmpvar_1;
  highp vec4 tmpvar_7;
  tmpvar_7.zw = vec2(0.0, 0.0);
  tmpvar_7.xy = tmpvar_1.xy;
  highp vec4 tmpvar_8;
  tmpvar_8 = (_glesVertex + ((tmpvar_7 * glstate_matrix_invtrans_modelview0) * tmpvar_5));
  highp vec3 tmpvar_9;
  tmpvar_9 = mix (_glesNormal, normalize((tmpvar_6 * glstate_matrix_invtrans_modelview0)).xyz, vec3(tmpvar_5));
  tmpvar_4.w = tmpvar_8.w;
  tmpvar_4.xyz = (tmpvar_8.xyz * _Scale.xyz);
  highp vec4 tmpvar_10;
  tmpvar_10.xy = tmpvar_2.xy;
  tmpvar_10.zw = _glesMultiTexCoord1.xy;
  highp vec4 pos_11;
  pos_11.w = tmpvar_4.w;
  highp vec3 bend_12;
  highp vec4 v_13;
  v_13.x = _Object2World[0].w;
  v_13.y = _Object2World[1].w;
  v_13.z = _Object2World[2].w;
  v_13.w = _Object2World[3].w;
  highp float tmpvar_14;
  tmpvar_14 = (dot (v_13.xyz, vec3(1.0, 1.0, 1.0)) + tmpvar_10.x);
  highp vec2 tmpvar_15;
  tmpvar_15.x = dot (tmpvar_4.xyz, vec3((tmpvar_10.y + tmpvar_14)));
  tmpvar_15.y = tmpvar_14;
  highp vec4 tmpvar_16;
  tmpvar_16 = abs(((
    fract((((
      fract(((_Time.yy + tmpvar_15).xxyy * vec4(1.975, 0.793, 0.375, 0.193)))
     * 2.0) - 1.0) + 0.5))
   * 2.0) - 1.0));
  highp vec4 tmpvar_17;
  tmpvar_17 = ((tmpvar_16 * tmpvar_16) * (3.0 - (2.0 * tmpvar_16)));
  highp vec2 tmpvar_18;
  tmpvar_18 = (tmpvar_17.xz + tmpvar_17.yw);
  bend_12.xz = ((tmpvar_10.y * 0.1) * tmpvar_9).xz;
  bend_12.y = (_glesMultiTexCoord1.y * 0.3);
  pos_11.xyz = (tmpvar_4.xyz + ((
    (tmpvar_18.xyx * bend_12)
   + 
    ((_Wind.xyz * tmpvar_18.y) * _glesMultiTexCoord1.y)
  ) * _Wind.w));
  pos_11.xyz = (pos_11.xyz + (_glesMultiTexCoord1.x * _Wind.xyz));
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = mix ((pos_11.xyz - (
    (dot (_SquashPlaneNormal.xyz, pos_11.xyz) + _SquashPlaneNormal.w)
   * _SquashPlaneNormal.xyz)), pos_11.xyz, vec3(_SquashAmount));
  tmpvar_4 = tmpvar_19;
  highp mat3 tmpvar_20;
  tmpvar_20[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_20[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_20[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  tmpvar_3.xyz = (tmpvar_20 * normalize(tmpvar_9));
  tmpvar_3.w = -(((glstate_matrix_modelview0 * tmpvar_19).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * tmpvar_19);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump float alpha_2;
  lowp float tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, xlv_TEXCOORD0).w;
  alpha_2 = tmpvar_3;
  mediump float x_4;
  x_4 = (alpha_2 - _Cutoff);
  if ((x_4 < 0.0)) {
    discard;
  };
  highp vec4 enc_5;
  enc_5.xy = (((
    (xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_6.y = tmpvar_7.y;
  enc_6.x = (tmpvar_7.x - (tmpvar_7.y * 0.00392157));
  enc_5.zw = enc_6;
  tmpvar_1 = enc_5;
  gl_FragData[0] = tmpvar_1;
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
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform highp mat4 _Object2World;
uniform highp vec4 _Scale;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
uniform highp vec4 _Wind;
out highp vec2 xlv_TEXCOORD0;
out highp vec4 xlv_TEXCOORD1;
void main ()
{
  highp vec3 tmpvar_1;
  tmpvar_1 = _glesNormal;
  lowp vec4 tmpvar_2;
  tmpvar_2 = _glesColor;
  highp vec4 tmpvar_3;
  highp vec4 tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0 - abs(_glesTANGENT.w));
  highp vec4 tmpvar_6;
  tmpvar_6.w = 0.0;
  tmpvar_6.xyz = tmpvar_1;
  highp vec4 tmpvar_7;
  tmpvar_7.zw = vec2(0.0, 0.0);
  tmpvar_7.xy = tmpvar_1.xy;
  highp vec4 tmpvar_8;
  tmpvar_8 = (_glesVertex + ((tmpvar_7 * glstate_matrix_invtrans_modelview0) * tmpvar_5));
  highp vec3 tmpvar_9;
  tmpvar_9 = mix (_glesNormal, normalize((tmpvar_6 * glstate_matrix_invtrans_modelview0)).xyz, vec3(tmpvar_5));
  tmpvar_4.w = tmpvar_8.w;
  tmpvar_4.xyz = (tmpvar_8.xyz * _Scale.xyz);
  highp vec4 tmpvar_10;
  tmpvar_10.xy = tmpvar_2.xy;
  tmpvar_10.zw = _glesMultiTexCoord1.xy;
  highp vec4 pos_11;
  pos_11.w = tmpvar_4.w;
  highp vec3 bend_12;
  highp vec4 v_13;
  v_13.x = _Object2World[0].w;
  v_13.y = _Object2World[1].w;
  v_13.z = _Object2World[2].w;
  v_13.w = _Object2World[3].w;
  highp float tmpvar_14;
  tmpvar_14 = (dot (v_13.xyz, vec3(1.0, 1.0, 1.0)) + tmpvar_10.x);
  highp vec2 tmpvar_15;
  tmpvar_15.x = dot (tmpvar_4.xyz, vec3((tmpvar_10.y + tmpvar_14)));
  tmpvar_15.y = tmpvar_14;
  highp vec4 tmpvar_16;
  tmpvar_16 = abs(((
    fract((((
      fract(((_Time.yy + tmpvar_15).xxyy * vec4(1.975, 0.793, 0.375, 0.193)))
     * 2.0) - 1.0) + 0.5))
   * 2.0) - 1.0));
  highp vec4 tmpvar_17;
  tmpvar_17 = ((tmpvar_16 * tmpvar_16) * (3.0 - (2.0 * tmpvar_16)));
  highp vec2 tmpvar_18;
  tmpvar_18 = (tmpvar_17.xz + tmpvar_17.yw);
  bend_12.xz = ((tmpvar_10.y * 0.1) * tmpvar_9).xz;
  bend_12.y = (_glesMultiTexCoord1.y * 0.3);
  pos_11.xyz = (tmpvar_4.xyz + ((
    (tmpvar_18.xyx * bend_12)
   + 
    ((_Wind.xyz * tmpvar_18.y) * _glesMultiTexCoord1.y)
  ) * _Wind.w));
  pos_11.xyz = (pos_11.xyz + (_glesMultiTexCoord1.x * _Wind.xyz));
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = mix ((pos_11.xyz - (
    (dot (_SquashPlaneNormal.xyz, pos_11.xyz) + _SquashPlaneNormal.w)
   * _SquashPlaneNormal.xyz)), pos_11.xyz, vec3(_SquashAmount));
  tmpvar_4 = tmpvar_19;
  highp mat3 tmpvar_20;
  tmpvar_20[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_20[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_20[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  tmpvar_3.xyz = (tmpvar_20 * normalize(tmpvar_9));
  tmpvar_3.w = -(((glstate_matrix_modelview0 * tmpvar_19).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * tmpvar_19);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_3;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
in highp vec2 xlv_TEXCOORD0;
in highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump float alpha_2;
  lowp float tmpvar_3;
  tmpvar_3 = texture (_MainTex, xlv_TEXCOORD0).w;
  alpha_2 = tmpvar_3;
  mediump float x_4;
  x_4 = (alpha_2 - _Cutoff);
  if ((x_4 < 0.0)) {
    discard;
  };
  highp vec4 enc_5;
  enc_5.xy = (((
    (xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_6.y = tmpvar_7.y;
  enc_6.x = (tmpvar_7.x - (tmpvar_7.y * 0.00392157));
  enc_5.zw = enc_6;
  tmpvar_1 = enc_5;
  _glesFragData[0] = tmpvar_1;
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
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform highp vec4 _Scale;
uniform highp mat4 _TerrainEngineBendTree;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
varying highp vec4 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec4 tmpvar_2;
  highp vec4 pos_3;
  pos_3.w = _glesVertex.w;
  highp float alpha_4;
  alpha_4 = tmpvar_1.w;
  pos_3.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_5;
  tmpvar_5.w = 0.0;
  tmpvar_5.xyz = pos_3.xyz;
  pos_3.xyz = mix (pos_3.xyz, (_TerrainEngineBendTree * tmpvar_5).xyz, vec3(alpha_4));
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = mix ((pos_3.xyz - (
    (dot (_SquashPlaneNormal.xyz, pos_3.xyz) + _SquashPlaneNormal.w)
   * _SquashPlaneNormal.xyz)), pos_3.xyz, vec3(_SquashAmount));
  pos_3 = tmpvar_6;
  highp mat3 tmpvar_7;
  tmpvar_7[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_7[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_7[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  tmpvar_2.xyz = (tmpvar_7 * normalize(_glesNormal));
  tmpvar_2.w = -(((glstate_matrix_modelview0 * tmpvar_6).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * tmpvar_6);
  xlv_TEXCOORD0 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

varying highp vec4 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 enc_2;
  enc_2.xy = (((
    (xlv_TEXCOORD0.xy / (xlv_TEXCOORD0.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_3;
  highp vec2 tmpvar_4;
  tmpvar_4 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD0.w));
  enc_3.y = tmpvar_4.y;
  enc_3.x = (tmpvar_4.x - (tmpvar_4.y * 0.00392157));
  enc_2.zw = enc_3;
  tmpvar_1 = enc_2;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesColor;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform highp vec4 _Scale;
uniform highp mat4 _TerrainEngineBendTree;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
out highp vec4 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec4 tmpvar_2;
  highp vec4 pos_3;
  pos_3.w = _glesVertex.w;
  highp float alpha_4;
  alpha_4 = tmpvar_1.w;
  pos_3.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_5;
  tmpvar_5.w = 0.0;
  tmpvar_5.xyz = pos_3.xyz;
  pos_3.xyz = mix (pos_3.xyz, (_TerrainEngineBendTree * tmpvar_5).xyz, vec3(alpha_4));
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = mix ((pos_3.xyz - (
    (dot (_SquashPlaneNormal.xyz, pos_3.xyz) + _SquashPlaneNormal.w)
   * _SquashPlaneNormal.xyz)), pos_3.xyz, vec3(_SquashAmount));
  pos_3 = tmpvar_6;
  highp mat3 tmpvar_7;
  tmpvar_7[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_7[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_7[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  tmpvar_2.xyz = (tmpvar_7 * normalize(_glesNormal));
  tmpvar_2.w = -(((glstate_matrix_modelview0 * tmpvar_6).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * tmpvar_6);
  xlv_TEXCOORD0 = tmpvar_2;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
in highp vec4 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 enc_2;
  enc_2.xy = (((
    (xlv_TEXCOORD0.xy / (xlv_TEXCOORD0.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_3;
  highp vec2 tmpvar_4;
  tmpvar_4 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD0.w));
  enc_3.y = tmpvar_4.y;
  enc_3.x = (tmpvar_4.x - (tmpvar_4.y * 0.00392157));
  enc_2.zw = enc_3;
  tmpvar_1 = enc_2;
  _glesFragData[0] = tmpvar_1;
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
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform highp vec4 _Scale;
uniform highp mat4 _TerrainEngineBendTree;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec4 tmpvar_2;
  highp vec4 pos_3;
  pos_3.w = _glesVertex.w;
  highp float alpha_4;
  alpha_4 = tmpvar_1.w;
  pos_3.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_5;
  tmpvar_5.w = 0.0;
  tmpvar_5.xyz = pos_3.xyz;
  pos_3.xyz = mix (pos_3.xyz, (_TerrainEngineBendTree * tmpvar_5).xyz, vec3(alpha_4));
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = mix ((pos_3.xyz - (
    (dot (_SquashPlaneNormal.xyz, pos_3.xyz) + _SquashPlaneNormal.w)
   * _SquashPlaneNormal.xyz)), pos_3.xyz, vec3(_SquashAmount));
  pos_3 = tmpvar_6;
  highp mat3 tmpvar_7;
  tmpvar_7[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_7[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_7[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  tmpvar_2.xyz = (tmpvar_7 * normalize(_glesNormal));
  tmpvar_2.w = -(((glstate_matrix_modelview0 * tmpvar_6).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * tmpvar_6);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump float alpha_2;
  lowp float tmpvar_3;
  tmpvar_3 = texture2D (_MainTex, xlv_TEXCOORD0).w;
  alpha_2 = tmpvar_3;
  mediump float x_4;
  x_4 = (alpha_2 - _Cutoff);
  if ((x_4 < 0.0)) {
    discard;
  };
  highp vec4 enc_5;
  enc_5.xy = (((
    (xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_6.y = tmpvar_7.y;
  enc_6.x = (tmpvar_7.x - (tmpvar_7.y * 0.00392157));
  enc_5.zw = enc_6;
  tmpvar_1 = enc_5;
  gl_FragData[0] = tmpvar_1;
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
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform highp vec4 _Scale;
uniform highp mat4 _TerrainEngineBendTree;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
out highp vec2 xlv_TEXCOORD0;
out highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec4 tmpvar_2;
  highp vec4 pos_3;
  pos_3.w = _glesVertex.w;
  highp float alpha_4;
  alpha_4 = tmpvar_1.w;
  pos_3.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_5;
  tmpvar_5.w = 0.0;
  tmpvar_5.xyz = pos_3.xyz;
  pos_3.xyz = mix (pos_3.xyz, (_TerrainEngineBendTree * tmpvar_5).xyz, vec3(alpha_4));
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = mix ((pos_3.xyz - (
    (dot (_SquashPlaneNormal.xyz, pos_3.xyz) + _SquashPlaneNormal.w)
   * _SquashPlaneNormal.xyz)), pos_3.xyz, vec3(_SquashAmount));
  pos_3 = tmpvar_6;
  highp mat3 tmpvar_7;
  tmpvar_7[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_7[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_7[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  tmpvar_2.xyz = (tmpvar_7 * normalize(_glesNormal));
  tmpvar_2.w = -(((glstate_matrix_modelview0 * tmpvar_6).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * tmpvar_6);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_2;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
in highp vec2 xlv_TEXCOORD0;
in highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump float alpha_2;
  lowp float tmpvar_3;
  tmpvar_3 = texture (_MainTex, xlv_TEXCOORD0).w;
  alpha_2 = tmpvar_3;
  mediump float x_4;
  x_4 = (alpha_2 - _Cutoff);
  if ((x_4 < 0.0)) {
    discard;
  };
  highp vec4 enc_5;
  enc_5.xy = (((
    (xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_6.y = tmpvar_7.y;
  enc_6.x = (tmpvar_7.x - (tmpvar_7.y * 0.00392157));
  enc_5.zw = enc_6;
  tmpvar_1 = enc_5;
  _glesFragData[0] = tmpvar_1;
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
  Tags { "RenderType"="TreeTransparentCutout" }
  Cull Front
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform highp vec4 _Scale;
uniform highp mat4 _TerrainEngineBendTree;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec4 tmpvar_2;
  highp vec4 pos_3;
  pos_3.w = _glesVertex.w;
  highp float alpha_4;
  alpha_4 = tmpvar_1.w;
  pos_3.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_5;
  tmpvar_5.w = 0.0;
  tmpvar_5.xyz = pos_3.xyz;
  pos_3.xyz = mix (pos_3.xyz, (_TerrainEngineBendTree * tmpvar_5).xyz, vec3(alpha_4));
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = mix ((pos_3.xyz - (
    (dot (_SquashPlaneNormal.xyz, pos_3.xyz) + _SquashPlaneNormal.w)
   * _SquashPlaneNormal.xyz)), pos_3.xyz, vec3(_SquashAmount));
  pos_3 = tmpvar_6;
  highp mat3 tmpvar_7;
  tmpvar_7[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_7[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_7[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  tmpvar_2.xyz = -((tmpvar_7 * normalize(_glesNormal)));
  tmpvar_2.w = -(((glstate_matrix_modelview0 * tmpvar_6).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * tmpvar_6);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp float x_2;
  x_2 = (texture2D (_MainTex, xlv_TEXCOORD0).w - _Cutoff);
  if ((x_2 < 0.0)) {
    discard;
  };
  highp vec4 enc_3;
  enc_3.xy = (((
    (xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_4.y = tmpvar_5.y;
  enc_4.x = (tmpvar_5.x - (tmpvar_5.y * 0.00392157));
  enc_3.zw = enc_4;
  tmpvar_1 = enc_3;
  gl_FragData[0] = tmpvar_1;
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
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform highp vec4 _Scale;
uniform highp mat4 _TerrainEngineBendTree;
uniform highp vec4 _SquashPlaneNormal;
uniform highp float _SquashAmount;
out highp vec2 xlv_TEXCOORD0;
out highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = _glesColor;
  highp vec4 tmpvar_2;
  highp vec4 pos_3;
  pos_3.w = _glesVertex.w;
  highp float alpha_4;
  alpha_4 = tmpvar_1.w;
  pos_3.xyz = (_glesVertex.xyz * _Scale.xyz);
  highp vec4 tmpvar_5;
  tmpvar_5.w = 0.0;
  tmpvar_5.xyz = pos_3.xyz;
  pos_3.xyz = mix (pos_3.xyz, (_TerrainEngineBendTree * tmpvar_5).xyz, vec3(alpha_4));
  highp vec4 tmpvar_6;
  tmpvar_6.w = 1.0;
  tmpvar_6.xyz = mix ((pos_3.xyz - (
    (dot (_SquashPlaneNormal.xyz, pos_3.xyz) + _SquashPlaneNormal.w)
   * _SquashPlaneNormal.xyz)), pos_3.xyz, vec3(_SquashAmount));
  pos_3 = tmpvar_6;
  highp mat3 tmpvar_7;
  tmpvar_7[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_7[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_7[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  tmpvar_2.xyz = -((tmpvar_7 * normalize(_glesNormal)));
  tmpvar_2.w = -(((glstate_matrix_modelview0 * tmpvar_6).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * tmpvar_6);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_2;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
in highp vec2 xlv_TEXCOORD0;
in highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp float x_2;
  x_2 = (texture (_MainTex, xlv_TEXCOORD0).w - _Cutoff);
  if ((x_2 < 0.0)) {
    discard;
  };
  highp vec4 enc_3;
  enc_3.xy = (((
    (xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_4.y = tmpvar_5.y;
  enc_4.x = (tmpvar_5.x - (tmpvar_5.y * 0.00392157));
  enc_3.zw = enc_4;
  tmpvar_1 = enc_3;
  _glesFragData[0] = tmpvar_1;
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
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp vec3 _TreeBillboardCameraRight;
uniform highp vec4 _TreeBillboardCameraUp;
uniform highp vec4 _TreeBillboardCameraFront;
uniform highp vec4 _TreeBillboardCameraPos;
uniform highp vec4 _TreeBillboardDistances;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0;
  highp vec2 tmpvar_2;
  highp vec4 tmpvar_3;
  highp vec4 pos_4;
  pos_4 = _glesVertex;
  highp vec2 offset_5;
  offset_5 = _glesMultiTexCoord1.xy;
  highp float offsetz_6;
  offsetz_6 = tmpvar_1.y;
  highp vec3 tmpvar_7;
  tmpvar_7 = (_glesVertex.xyz - _TreeBillboardCameraPos.xyz);
  highp float tmpvar_8;
  tmpvar_8 = dot (tmpvar_7, tmpvar_7);
  if ((tmpvar_8 > _TreeBillboardDistances.x)) {
    offsetz_6 = 0.0;
    offset_5 = vec2(0.0, 0.0);
  };
  pos_4.xyz = (_glesVertex.xyz + (_TreeBillboardCameraRight * offset_5.x));
  pos_4.xyz = (pos_4.xyz + (_TreeBillboardCameraUp.xyz * mix (offset_5.y, offsetz_6, _TreeBillboardCameraPos.w)));
  pos_4.xyz = (pos_4.xyz + ((_TreeBillboardCameraFront.xyz * 
    abs(offset_5.x)
  ) * _TreeBillboardCameraUp.w));
  tmpvar_2.x = tmpvar_1.x;
  tmpvar_2.y = float((_glesMultiTexCoord0.y > 0.0));
  tmpvar_3.xyz = vec3(0.0, 0.0, 1.0);
  tmpvar_3.w = -(((glstate_matrix_modelview0 * pos_4).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * pos_4);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp float x_2;
  x_2 = (texture2D (_MainTex, xlv_TEXCOORD0).w - 0.001);
  if ((x_2 < 0.0)) {
    discard;
  };
  highp vec4 enc_3;
  enc_3.xy = (((
    (xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_4.y = tmpvar_5.y;
  enc_4.x = (tmpvar_5.x - (tmpvar_5.y * 0.00392157));
  enc_3.zw = enc_4;
  tmpvar_1 = enc_3;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
in vec4 _glesMultiTexCoord1;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp vec3 _TreeBillboardCameraRight;
uniform highp vec4 _TreeBillboardCameraUp;
uniform highp vec4 _TreeBillboardCameraFront;
uniform highp vec4 _TreeBillboardCameraPos;
uniform highp vec4 _TreeBillboardDistances;
out highp vec2 xlv_TEXCOORD0;
out highp vec4 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0;
  highp vec2 tmpvar_2;
  highp vec4 tmpvar_3;
  highp vec4 pos_4;
  pos_4 = _glesVertex;
  highp vec2 offset_5;
  offset_5 = _glesMultiTexCoord1.xy;
  highp float offsetz_6;
  offsetz_6 = tmpvar_1.y;
  highp vec3 tmpvar_7;
  tmpvar_7 = (_glesVertex.xyz - _TreeBillboardCameraPos.xyz);
  highp float tmpvar_8;
  tmpvar_8 = dot (tmpvar_7, tmpvar_7);
  if ((tmpvar_8 > _TreeBillboardDistances.x)) {
    offsetz_6 = 0.0;
    offset_5 = vec2(0.0, 0.0);
  };
  pos_4.xyz = (_glesVertex.xyz + (_TreeBillboardCameraRight * offset_5.x));
  pos_4.xyz = (pos_4.xyz + (_TreeBillboardCameraUp.xyz * mix (offset_5.y, offsetz_6, _TreeBillboardCameraPos.w)));
  pos_4.xyz = (pos_4.xyz + ((_TreeBillboardCameraFront.xyz * 
    abs(offset_5.x)
  ) * _TreeBillboardCameraUp.w));
  tmpvar_2.x = tmpvar_1.x;
  tmpvar_2.y = float((_glesMultiTexCoord0.y > 0.0));
  tmpvar_3.xyz = vec3(0.0, 0.0, 1.0);
  tmpvar_3.w = -(((glstate_matrix_modelview0 * pos_4).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * pos_4);
  xlv_TEXCOORD0 = tmpvar_2;
  xlv_TEXCOORD1 = tmpvar_3;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
in highp vec2 xlv_TEXCOORD0;
in highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp float x_2;
  x_2 = (texture (_MainTex, xlv_TEXCOORD0).w - 0.001);
  if ((x_2 < 0.0)) {
    discard;
  };
  highp vec4 enc_3;
  enc_3.xy = (((
    (xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_4.y = tmpvar_5.y;
  enc_4.x = (tmpvar_5.x - (tmpvar_5.y * 0.00392157));
  enc_3.zw = enc_4;
  tmpvar_1 = enc_3;
  _glesFragData[0] = tmpvar_1;
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
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
attribute vec4 _glesTANGENT;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec4 _CameraPosition;
uniform highp vec3 _CameraRight;
uniform highp vec3 _CameraUp;
varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0;
  lowp vec4 tmpvar_2;
  tmpvar_2 = _glesColor;
  highp vec4 tmpvar_3;
  highp vec4 pos_4;
  pos_4 = _glesVertex;
  highp vec2 offset_5;
  offset_5 = _glesTANGENT.xy;
  highp vec3 tmpvar_6;
  tmpvar_6 = (_glesVertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_7;
  tmpvar_7 = dot (tmpvar_6, tmpvar_6);
  if ((tmpvar_7 > _WaveAndDistance.w)) {
    offset_5 = vec2(0.0, 0.0);
  };
  pos_4.xyz = (_glesVertex.xyz + (offset_5.x * _CameraRight));
  pos_4.xyz = (pos_4.xyz + (offset_5.y * _CameraUp));
  highp vec4 vertex_8;
  vertex_8.yw = pos_4.yw;
  lowp vec4 color_9;
  color_9.xyz = tmpvar_2.xyz;
  lowp vec3 waveColor_10;
  highp vec3 waveMove_11;
  highp vec4 tmpvar_12;
  tmpvar_12 = ((fract(
    (((pos_4.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos_4.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))
  ) * 6.40885) - 3.14159);
  highp vec4 tmpvar_13;
  tmpvar_13 = (tmpvar_12 * tmpvar_12);
  highp vec4 tmpvar_14;
  tmpvar_14 = (tmpvar_13 * tmpvar_12);
  highp vec4 tmpvar_15;
  tmpvar_15 = (tmpvar_14 * tmpvar_13);
  highp vec4 tmpvar_16;
  tmpvar_16 = (((tmpvar_12 + 
    (tmpvar_14 * -0.161616)
  ) + (tmpvar_15 * 0.0083333)) + ((tmpvar_15 * tmpvar_13) * -0.00019841));
  highp vec4 tmpvar_17;
  tmpvar_17 = (tmpvar_16 * tmpvar_16);
  highp vec4 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * tmpvar_17);
  highp vec4 tmpvar_19;
  tmpvar_19 = (tmpvar_18 * _glesTANGENT.y);
  waveMove_11.y = 0.0;
  waveMove_11.x = dot (tmpvar_19, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove_11.z = dot (tmpvar_19, vec4(0.006, 0.02, -0.02, 0.1));
  vertex_8.xz = (pos_4.xz - (waveMove_11.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_20;
  tmpvar_20 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_18, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor_10 = tmpvar_20;
  highp vec3 tmpvar_21;
  tmpvar_21 = (vertex_8.xyz - _CameraPosition.xyz);
  highp float tmpvar_22;
  tmpvar_22 = clamp (((2.0 * 
    (_WaveAndDistance.w - dot (tmpvar_21, tmpvar_21))
  ) * _CameraPosition.w), 0.0, 1.0);
  color_9.w = tmpvar_22;
  lowp vec4 tmpvar_23;
  tmpvar_23.xyz = ((2.0 * waveColor_10) * _glesColor.xyz);
  tmpvar_23.w = color_9.w;
  highp mat3 tmpvar_24;
  tmpvar_24[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_24[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_24[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  tmpvar_3.xyz = (tmpvar_24 * _glesNormal);
  tmpvar_3.w = -(((glstate_matrix_modelview0 * vertex_8).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * vertex_8);
  xlv_COLOR = tmpvar_23;
  xlv_TEXCOORD0 = tmpvar_1.xy;
  xlv_TEXCOORD1 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp float x_2;
  x_2 = ((texture2D (_MainTex, xlv_TEXCOORD0).w * xlv_COLOR.w) - _Cutoff);
  if ((x_2 < 0.0)) {
    discard;
  };
  highp vec4 enc_3;
  enc_3.xy = (((
    (xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_4.y = tmpvar_5.y;
  enc_4.x = (tmpvar_5.x - (tmpvar_5.y * 0.00392157));
  enc_3.zw = enc_4;
  tmpvar_1 = enc_3;
  gl_FragData[0] = tmpvar_1;
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
in vec4 _glesTANGENT;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec4 _CameraPosition;
uniform highp vec3 _CameraRight;
uniform highp vec3 _CameraUp;
out lowp vec4 xlv_COLOR;
out highp vec2 xlv_TEXCOORD0;
out highp vec4 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0;
  lowp vec4 tmpvar_2;
  tmpvar_2 = _glesColor;
  highp vec4 tmpvar_3;
  highp vec4 pos_4;
  pos_4 = _glesVertex;
  highp vec2 offset_5;
  offset_5 = _glesTANGENT.xy;
  highp vec3 tmpvar_6;
  tmpvar_6 = (_glesVertex.xyz - _CameraPosition.xyz);
  highp float tmpvar_7;
  tmpvar_7 = dot (tmpvar_6, tmpvar_6);
  if ((tmpvar_7 > _WaveAndDistance.w)) {
    offset_5 = vec2(0.0, 0.0);
  };
  pos_4.xyz = (_glesVertex.xyz + (offset_5.x * _CameraRight));
  pos_4.xyz = (pos_4.xyz + (offset_5.y * _CameraUp));
  highp vec4 vertex_8;
  vertex_8.yw = pos_4.yw;
  lowp vec4 color_9;
  color_9.xyz = tmpvar_2.xyz;
  lowp vec3 waveColor_10;
  highp vec3 waveMove_11;
  highp vec4 tmpvar_12;
  tmpvar_12 = ((fract(
    (((pos_4.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (pos_4.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))
  ) * 6.40885) - 3.14159);
  highp vec4 tmpvar_13;
  tmpvar_13 = (tmpvar_12 * tmpvar_12);
  highp vec4 tmpvar_14;
  tmpvar_14 = (tmpvar_13 * tmpvar_12);
  highp vec4 tmpvar_15;
  tmpvar_15 = (tmpvar_14 * tmpvar_13);
  highp vec4 tmpvar_16;
  tmpvar_16 = (((tmpvar_12 + 
    (tmpvar_14 * -0.161616)
  ) + (tmpvar_15 * 0.0083333)) + ((tmpvar_15 * tmpvar_13) * -0.00019841));
  highp vec4 tmpvar_17;
  tmpvar_17 = (tmpvar_16 * tmpvar_16);
  highp vec4 tmpvar_18;
  tmpvar_18 = (tmpvar_17 * tmpvar_17);
  highp vec4 tmpvar_19;
  tmpvar_19 = (tmpvar_18 * _glesTANGENT.y);
  waveMove_11.y = 0.0;
  waveMove_11.x = dot (tmpvar_19, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove_11.z = dot (tmpvar_19, vec4(0.006, 0.02, -0.02, 0.1));
  vertex_8.xz = (pos_4.xz - (waveMove_11.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_20;
  tmpvar_20 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_18, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor_10 = tmpvar_20;
  highp vec3 tmpvar_21;
  tmpvar_21 = (vertex_8.xyz - _CameraPosition.xyz);
  highp float tmpvar_22;
  tmpvar_22 = clamp (((2.0 * 
    (_WaveAndDistance.w - dot (tmpvar_21, tmpvar_21))
  ) * _CameraPosition.w), 0.0, 1.0);
  color_9.w = tmpvar_22;
  lowp vec4 tmpvar_23;
  tmpvar_23.xyz = ((2.0 * waveColor_10) * _glesColor.xyz);
  tmpvar_23.w = color_9.w;
  highp mat3 tmpvar_24;
  tmpvar_24[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_24[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_24[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  tmpvar_3.xyz = (tmpvar_24 * _glesNormal);
  tmpvar_3.w = -(((glstate_matrix_modelview0 * vertex_8).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * vertex_8);
  xlv_COLOR = tmpvar_23;
  xlv_TEXCOORD0 = tmpvar_1.xy;
  xlv_TEXCOORD1 = tmpvar_3;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
in lowp vec4 xlv_COLOR;
in highp vec2 xlv_TEXCOORD0;
in highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp float x_2;
  x_2 = ((texture (_MainTex, xlv_TEXCOORD0).w * xlv_COLOR.w) - _Cutoff);
  if ((x_2 < 0.0)) {
    discard;
  };
  highp vec4 enc_3;
  enc_3.xy = (((
    (xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_4.y = tmpvar_5.y;
  enc_4.x = (tmpvar_5.x - (tmpvar_5.y * 0.00392157));
  enc_3.zw = enc_4;
  tmpvar_1 = enc_3;
  _glesFragData[0] = tmpvar_1;
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
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec4 _CameraPosition;
varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 vertex_2;
  vertex_2.yw = _glesVertex.yw;
  lowp vec4 color_3;
  color_3.xyz = _glesColor.xyz;
  lowp vec3 waveColor_4;
  highp vec3 waveMove_5;
  highp vec4 tmpvar_6;
  tmpvar_6 = ((fract(
    (((_glesVertex.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (_glesVertex.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))
  ) * 6.40885) - 3.14159);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * tmpvar_6);
  highp vec4 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_6);
  highp vec4 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * tmpvar_7);
  highp vec4 tmpvar_10;
  tmpvar_10 = (((tmpvar_6 + 
    (tmpvar_8 * -0.161616)
  ) + (tmpvar_9 * 0.0083333)) + ((tmpvar_9 * tmpvar_7) * -0.00019841));
  highp vec4 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * tmpvar_10);
  highp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * tmpvar_11);
  highp vec4 tmpvar_13;
  tmpvar_13 = (tmpvar_12 * (_glesColor.w * _WaveAndDistance.z));
  waveMove_5.y = 0.0;
  waveMove_5.x = dot (tmpvar_13, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove_5.z = dot (tmpvar_13, vec4(0.006, 0.02, -0.02, 0.1));
  vertex_2.xz = (_glesVertex.xz - (waveMove_5.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_14;
  tmpvar_14 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_12, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor_4 = tmpvar_14;
  highp vec3 tmpvar_15;
  tmpvar_15 = (vertex_2.xyz - _CameraPosition.xyz);
  highp float tmpvar_16;
  tmpvar_16 = clamp (((2.0 * 
    (_WaveAndDistance.w - dot (tmpvar_15, tmpvar_15))
  ) * _CameraPosition.w), 0.0, 1.0);
  color_3.w = tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17.xyz = ((2.0 * waveColor_4) * _glesColor.xyz);
  tmpvar_17.w = color_3.w;
  highp mat3 tmpvar_18;
  tmpvar_18[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_18[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_18[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  tmpvar_1.xyz = (tmpvar_18 * normalize(_glesNormal));
  tmpvar_1.w = -(((glstate_matrix_modelview0 * vertex_2).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * vertex_2);
  xlv_COLOR = tmpvar_17;
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_1;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
varying lowp vec4 xlv_COLOR;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp float x_2;
  x_2 = ((texture2D (_MainTex, xlv_TEXCOORD0).w * xlv_COLOR.w) - _Cutoff);
  if ((x_2 < 0.0)) {
    discard;
  };
  highp vec4 enc_3;
  enc_3.xy = (((
    (xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_4.y = tmpvar_5.y;
  enc_4.x = (tmpvar_5.x - (tmpvar_5.y * 0.00392157));
  enc_3.zw = enc_4;
  tmpvar_1 = enc_3;
  gl_FragData[0] = tmpvar_1;
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
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 glstate_matrix_invtrans_modelview0;
uniform lowp vec4 _WavingTint;
uniform highp vec4 _WaveAndDistance;
uniform highp vec4 _CameraPosition;
out lowp vec4 xlv_COLOR;
out highp vec2 xlv_TEXCOORD0;
out highp vec4 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  highp vec4 vertex_2;
  vertex_2.yw = _glesVertex.yw;
  lowp vec4 color_3;
  color_3.xyz = _glesColor.xyz;
  lowp vec3 waveColor_4;
  highp vec3 waveMove_5;
  highp vec4 tmpvar_6;
  tmpvar_6 = ((fract(
    (((_glesVertex.x * (vec4(0.012, 0.02, 0.06, 0.024) * _WaveAndDistance.y)) + (_glesVertex.z * (vec4(0.006, 0.02, 0.02, 0.05) * _WaveAndDistance.y))) + (_WaveAndDistance.x * vec4(1.2, 2.0, 1.6, 4.8)))
  ) * 6.40885) - 3.14159);
  highp vec4 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * tmpvar_6);
  highp vec4 tmpvar_8;
  tmpvar_8 = (tmpvar_7 * tmpvar_6);
  highp vec4 tmpvar_9;
  tmpvar_9 = (tmpvar_8 * tmpvar_7);
  highp vec4 tmpvar_10;
  tmpvar_10 = (((tmpvar_6 + 
    (tmpvar_8 * -0.161616)
  ) + (tmpvar_9 * 0.0083333)) + ((tmpvar_9 * tmpvar_7) * -0.00019841));
  highp vec4 tmpvar_11;
  tmpvar_11 = (tmpvar_10 * tmpvar_10);
  highp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 * tmpvar_11);
  highp vec4 tmpvar_13;
  tmpvar_13 = (tmpvar_12 * (_glesColor.w * _WaveAndDistance.z));
  waveMove_5.y = 0.0;
  waveMove_5.x = dot (tmpvar_13, vec4(0.024, 0.04, -0.12, 0.096));
  waveMove_5.z = dot (tmpvar_13, vec4(0.006, 0.02, -0.02, 0.1));
  vertex_2.xz = (_glesVertex.xz - (waveMove_5.xz * _WaveAndDistance.z));
  highp vec3 tmpvar_14;
  tmpvar_14 = mix (vec3(0.5, 0.5, 0.5), _WavingTint.xyz, vec3((dot (tmpvar_12, vec4(0.6742, 0.6742, 0.26968, 0.13484)) * 0.7)));
  waveColor_4 = tmpvar_14;
  highp vec3 tmpvar_15;
  tmpvar_15 = (vertex_2.xyz - _CameraPosition.xyz);
  highp float tmpvar_16;
  tmpvar_16 = clamp (((2.0 * 
    (_WaveAndDistance.w - dot (tmpvar_15, tmpvar_15))
  ) * _CameraPosition.w), 0.0, 1.0);
  color_3.w = tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17.xyz = ((2.0 * waveColor_4) * _glesColor.xyz);
  tmpvar_17.w = color_3.w;
  highp mat3 tmpvar_18;
  tmpvar_18[0] = glstate_matrix_invtrans_modelview0[0].xyz;
  tmpvar_18[1] = glstate_matrix_invtrans_modelview0[1].xyz;
  tmpvar_18[2] = glstate_matrix_invtrans_modelview0[2].xyz;
  tmpvar_1.xyz = (tmpvar_18 * normalize(_glesNormal));
  tmpvar_1.w = -(((glstate_matrix_modelview0 * vertex_2).z * _ProjectionParams.w));
  gl_Position = (glstate_matrix_mvp * vertex_2);
  xlv_COLOR = tmpvar_17;
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = tmpvar_1;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform lowp float _Cutoff;
in lowp vec4 xlv_COLOR;
in highp vec2 xlv_TEXCOORD0;
in highp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  lowp float x_2;
  x_2 = ((texture (_MainTex, xlv_TEXCOORD0).w * xlv_COLOR.w) - _Cutoff);
  if ((x_2 < 0.0)) {
    discard;
  };
  highp vec4 enc_3;
  enc_3.xy = (((
    (xlv_TEXCOORD1.xy / (xlv_TEXCOORD1.z + 1.0))
   / 1.7777) * 0.5) + 0.5);
  highp vec2 enc_4;
  highp vec2 tmpvar_5;
  tmpvar_5 = fract((vec2(1.0, 255.0) * xlv_TEXCOORD1.w));
  enc_4.y = tmpvar_5.y;
  enc_4.x = (tmpvar_5.x - (tmpvar_5.y * 0.00392157));
  enc_3.zw = enc_4;
  tmpvar_1 = enc_3;
  _glesFragData[0] = tmpvar_1;
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