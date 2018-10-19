Shader "Hidden/Internal-PrePassCollectShadows" {
Properties {
 _ShadowMapTexture ("", any) = "" {}
}
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
Keywords { "SHADOWS_NONATIVE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = _glesNormal;
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightSplitsNear;
uniform highp vec4 _LightSplitsFar;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp sampler2D _CameraDepthTexture;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _ShadowMapTexture;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 res_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraDepthTexture, xlv_TEXCOORD0);
  highp float tmpvar_4;
  tmpvar_4 = (1.0/(((_ZBufferParams.x * tmpvar_3.x) + _ZBufferParams.y)));
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = (xlv_TEXCOORD1 * tmpvar_4);
  highp vec4 tmpvar_6;
  tmpvar_6 = (_CameraToWorld * tmpvar_5);
  mediump float shadow_7;
  highp vec4 zFar_8;
  highp vec4 zNear_9;
  bvec4 tmpvar_10;
  tmpvar_10 = greaterThanEqual (tmpvar_5.zzzz, _LightSplitsNear);
  lowp vec4 tmpvar_11;
  tmpvar_11 = vec4(tmpvar_10);
  zNear_9 = tmpvar_11;
  bvec4 tmpvar_12;
  tmpvar_12 = lessThan (tmpvar_5.zzzz, _LightSplitsFar);
  lowp vec4 tmpvar_13;
  tmpvar_13 = vec4(tmpvar_12);
  zFar_8 = tmpvar_13;
  highp vec4 tmpvar_14;
  tmpvar_14 = (zNear_9 * zFar_8);
  highp vec4 tmpvar_15;
  tmpvar_15.w = 1.0;
  tmpvar_15.xyz = (((
    ((unity_World2Shadow[0] * tmpvar_6).xyz * tmpvar_14.x)
   + 
    ((unity_World2Shadow[1] * tmpvar_6).xyz * tmpvar_14.y)
  ) + (
    (unity_World2Shadow[2] * tmpvar_6)
  .xyz * tmpvar_14.z)) + ((unity_World2Shadow[3] * tmpvar_6).xyz * tmpvar_14.w));
  lowp vec4 tmpvar_16;
  tmpvar_16 = texture2D (_ShadowMapTexture, tmpvar_15.xy);
  highp float tmpvar_17;
  if ((tmpvar_16.x < tmpvar_15.z)) {
    tmpvar_17 = _LightShadowData.x;
  } else {
    tmpvar_17 = 1.0;
  };
  shadow_7 = tmpvar_17;
  res_2.x = shadow_7;
  res_2.y = 1.0;
  highp vec2 enc_18;
  highp vec2 tmpvar_19;
  tmpvar_19 = fract((vec2(1.0, 255.0) * (1.0 - tmpvar_4)));
  enc_18.y = tmpvar_19.y;
  enc_18.x = (tmpvar_19.x - (tmpvar_19.y * 0.00392157));
  res_2.zw = enc_18;
  tmpvar_1 = res_2;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SHADOWS_NATIVE" }
"!!GLES


#ifdef VERTEX

#extension GL_EXT_shadow_samplers : enable
attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = _glesNormal;
}



#endif
#ifdef FRAGMENT

#extension GL_EXT_shadow_samplers : enable
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightSplitsNear;
uniform highp vec4 _LightSplitsFar;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp sampler2D _CameraDepthTexture;
uniform highp mat4 _CameraToWorld;
uniform lowp sampler2DShadow _ShadowMapTexture;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 res_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraDepthTexture, xlv_TEXCOORD0);
  highp float tmpvar_4;
  tmpvar_4 = (1.0/(((_ZBufferParams.x * tmpvar_3.x) + _ZBufferParams.y)));
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = (xlv_TEXCOORD1 * tmpvar_4);
  highp vec4 tmpvar_6;
  tmpvar_6 = (_CameraToWorld * tmpvar_5);
  mediump float shadow_7;
  highp vec4 zFar_8;
  highp vec4 zNear_9;
  bvec4 tmpvar_10;
  tmpvar_10 = greaterThanEqual (tmpvar_5.zzzz, _LightSplitsNear);
  lowp vec4 tmpvar_11;
  tmpvar_11 = vec4(tmpvar_10);
  zNear_9 = tmpvar_11;
  bvec4 tmpvar_12;
  tmpvar_12 = lessThan (tmpvar_5.zzzz, _LightSplitsFar);
  lowp vec4 tmpvar_13;
  tmpvar_13 = vec4(tmpvar_12);
  zFar_8 = tmpvar_13;
  highp vec4 tmpvar_14;
  tmpvar_14 = (zNear_9 * zFar_8);
  highp vec4 tmpvar_15;
  tmpvar_15.w = 1.0;
  tmpvar_15.xyz = (((
    ((unity_World2Shadow[0] * tmpvar_6).xyz * tmpvar_14.x)
   + 
    ((unity_World2Shadow[1] * tmpvar_6).xyz * tmpvar_14.y)
  ) + (
    (unity_World2Shadow[2] * tmpvar_6)
  .xyz * tmpvar_14.z)) + ((unity_World2Shadow[3] * tmpvar_6).xyz * tmpvar_14.w));
  lowp float tmpvar_16;
  tmpvar_16 = shadow2DEXT (_ShadowMapTexture, tmpvar_15.xyz);
  mediump float tmpvar_17;
  tmpvar_17 = tmpvar_16;
  highp float tmpvar_18;
  tmpvar_18 = mix (_LightShadowData.x, 1.0, tmpvar_17);
  shadow_7 = tmpvar_18;
  res_2.x = shadow_7;
  res_2.y = 1.0;
  highp vec2 enc_19;
  highp vec2 tmpvar_20;
  tmpvar_20 = fract((vec2(1.0, 255.0) * (1.0 - tmpvar_4)));
  enc_19.y = tmpvar_20.y;
  enc_19.x = (tmpvar_20.x - (tmpvar_20.y * 0.00392157));
  res_2.zw = enc_19;
  tmpvar_1 = res_2;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "SHADOWS_NATIVE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
out highp vec2 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = _glesNormal;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightSplitsNear;
uniform highp vec4 _LightSplitsFar;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp sampler2D _CameraDepthTexture;
uniform highp mat4 _CameraToWorld;
uniform lowp sampler2DShadow _ShadowMapTexture;
in highp vec2 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 res_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = texture (_CameraDepthTexture, xlv_TEXCOORD0);
  highp float tmpvar_4;
  tmpvar_4 = (1.0/(((_ZBufferParams.x * tmpvar_3.x) + _ZBufferParams.y)));
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = (xlv_TEXCOORD1 * tmpvar_4);
  highp vec4 tmpvar_6;
  tmpvar_6 = (_CameraToWorld * tmpvar_5);
  mediump float shadow_7;
  highp vec4 zFar_8;
  highp vec4 zNear_9;
  bvec4 tmpvar_10;
  tmpvar_10 = greaterThanEqual (tmpvar_5.zzzz, _LightSplitsNear);
  lowp vec4 tmpvar_11;
  tmpvar_11 = vec4(tmpvar_10);
  zNear_9 = tmpvar_11;
  bvec4 tmpvar_12;
  tmpvar_12 = lessThan (tmpvar_5.zzzz, _LightSplitsFar);
  lowp vec4 tmpvar_13;
  tmpvar_13 = vec4(tmpvar_12);
  zFar_8 = tmpvar_13;
  highp vec4 tmpvar_14;
  tmpvar_14 = (zNear_9 * zFar_8);
  highp vec4 tmpvar_15;
  tmpvar_15.w = 1.0;
  tmpvar_15.xyz = (((
    ((unity_World2Shadow[0] * tmpvar_6).xyz * tmpvar_14.x)
   + 
    ((unity_World2Shadow[1] * tmpvar_6).xyz * tmpvar_14.y)
  ) + (
    (unity_World2Shadow[2] * tmpvar_6)
  .xyz * tmpvar_14.z)) + ((unity_World2Shadow[3] * tmpvar_6).xyz * tmpvar_14.w));
  mediump float tmpvar_16;
  tmpvar_16 = texture (_ShadowMapTexture, tmpvar_15.xyz);
  highp float tmpvar_17;
  tmpvar_17 = mix (_LightShadowData.x, 1.0, tmpvar_16);
  shadow_7 = tmpvar_17;
  res_2.x = shadow_7;
  res_2.y = 1.0;
  highp vec2 enc_18;
  highp vec2 tmpvar_19;
  tmpvar_19 = fract((vec2(1.0, 255.0) * (1.0 - tmpvar_4)));
  enc_18.y = tmpvar_19.y;
  enc_18.x = (tmpvar_19.x - (tmpvar_19.y * 0.00392157));
  res_2.zw = enc_18;
  tmpvar_1 = res_2;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SHADOWS_SPLIT_SPHERES" "SHADOWS_NONATIVE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = _glesNormal;
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowSplitSpheres[4];
uniform highp vec4 unity_ShadowSplitSqRadii;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp sampler2D _CameraDepthTexture;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _ShadowMapTexture;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 res_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraDepthTexture, xlv_TEXCOORD0);
  highp float tmpvar_4;
  tmpvar_4 = (1.0/(((_ZBufferParams.x * tmpvar_3.x) + _ZBufferParams.y)));
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = (xlv_TEXCOORD1 * tmpvar_4);
  highp vec4 tmpvar_6;
  tmpvar_6 = (_CameraToWorld * tmpvar_5);
  mediump float shadow_7;
  highp vec4 weights_8;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_6.xyz - unity_ShadowSplitSpheres[0].xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_6.xyz - unity_ShadowSplitSpheres[1].xyz);
  highp vec3 tmpvar_11;
  tmpvar_11 = (tmpvar_6.xyz - unity_ShadowSplitSpheres[2].xyz);
  highp vec3 tmpvar_12;
  tmpvar_12 = (tmpvar_6.xyz - unity_ShadowSplitSpheres[3].xyz);
  highp vec4 tmpvar_13;
  tmpvar_13.x = dot (tmpvar_9, tmpvar_9);
  tmpvar_13.y = dot (tmpvar_10, tmpvar_10);
  tmpvar_13.z = dot (tmpvar_11, tmpvar_11);
  tmpvar_13.w = dot (tmpvar_12, tmpvar_12);
  bvec4 tmpvar_14;
  tmpvar_14 = lessThan (tmpvar_13, unity_ShadowSplitSqRadii);
  lowp vec4 tmpvar_15;
  tmpvar_15 = vec4(tmpvar_14);
  weights_8 = tmpvar_15;
  weights_8.yzw = clamp ((weights_8.yzw - weights_8.xyz), 0.0, 1.0);
  highp vec4 tmpvar_16;
  tmpvar_16.w = 1.0;
  tmpvar_16.xyz = (((
    ((unity_World2Shadow[0] * tmpvar_6).xyz * weights_8.x)
   + 
    ((unity_World2Shadow[1] * tmpvar_6).xyz * weights_8.y)
  ) + (
    (unity_World2Shadow[2] * tmpvar_6)
  .xyz * weights_8.z)) + ((unity_World2Shadow[3] * tmpvar_6).xyz * weights_8.w));
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture2D (_ShadowMapTexture, tmpvar_16.xy);
  highp float tmpvar_18;
  if ((tmpvar_17.x < tmpvar_16.z)) {
    tmpvar_18 = _LightShadowData.x;
  } else {
    tmpvar_18 = 1.0;
  };
  shadow_7 = tmpvar_18;
  res_2.x = shadow_7;
  res_2.y = 1.0;
  highp vec2 enc_19;
  highp vec2 tmpvar_20;
  tmpvar_20 = fract((vec2(1.0, 255.0) * (1.0 - tmpvar_4)));
  enc_19.y = tmpvar_20.y;
  enc_19.x = (tmpvar_20.x - (tmpvar_20.y * 0.00392157));
  res_2.zw = enc_19;
  tmpvar_1 = res_2;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SHADOWS_SPLIT_SPHERES" "SHADOWS_NATIVE" }
"!!GLES


#ifdef VERTEX

#extension GL_EXT_shadow_samplers : enable
attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = _glesNormal;
}



#endif
#ifdef FRAGMENT

#extension GL_EXT_shadow_samplers : enable
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowSplitSpheres[4];
uniform highp vec4 unity_ShadowSplitSqRadii;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp sampler2D _CameraDepthTexture;
uniform highp mat4 _CameraToWorld;
uniform lowp sampler2DShadow _ShadowMapTexture;
varying highp vec2 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 res_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraDepthTexture, xlv_TEXCOORD0);
  highp float tmpvar_4;
  tmpvar_4 = (1.0/(((_ZBufferParams.x * tmpvar_3.x) + _ZBufferParams.y)));
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = (xlv_TEXCOORD1 * tmpvar_4);
  highp vec4 tmpvar_6;
  tmpvar_6 = (_CameraToWorld * tmpvar_5);
  mediump float shadow_7;
  highp vec4 weights_8;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_6.xyz - unity_ShadowSplitSpheres[0].xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_6.xyz - unity_ShadowSplitSpheres[1].xyz);
  highp vec3 tmpvar_11;
  tmpvar_11 = (tmpvar_6.xyz - unity_ShadowSplitSpheres[2].xyz);
  highp vec3 tmpvar_12;
  tmpvar_12 = (tmpvar_6.xyz - unity_ShadowSplitSpheres[3].xyz);
  highp vec4 tmpvar_13;
  tmpvar_13.x = dot (tmpvar_9, tmpvar_9);
  tmpvar_13.y = dot (tmpvar_10, tmpvar_10);
  tmpvar_13.z = dot (tmpvar_11, tmpvar_11);
  tmpvar_13.w = dot (tmpvar_12, tmpvar_12);
  bvec4 tmpvar_14;
  tmpvar_14 = lessThan (tmpvar_13, unity_ShadowSplitSqRadii);
  lowp vec4 tmpvar_15;
  tmpvar_15 = vec4(tmpvar_14);
  weights_8 = tmpvar_15;
  weights_8.yzw = clamp ((weights_8.yzw - weights_8.xyz), 0.0, 1.0);
  highp vec4 tmpvar_16;
  tmpvar_16.w = 1.0;
  tmpvar_16.xyz = (((
    ((unity_World2Shadow[0] * tmpvar_6).xyz * weights_8.x)
   + 
    ((unity_World2Shadow[1] * tmpvar_6).xyz * weights_8.y)
  ) + (
    (unity_World2Shadow[2] * tmpvar_6)
  .xyz * weights_8.z)) + ((unity_World2Shadow[3] * tmpvar_6).xyz * weights_8.w));
  lowp float tmpvar_17;
  tmpvar_17 = shadow2DEXT (_ShadowMapTexture, tmpvar_16.xyz);
  mediump float tmpvar_18;
  tmpvar_18 = tmpvar_17;
  highp float tmpvar_19;
  tmpvar_19 = mix (_LightShadowData.x, 1.0, tmpvar_18);
  shadow_7 = tmpvar_19;
  res_2.x = shadow_7;
  res_2.y = 1.0;
  highp vec2 enc_20;
  highp vec2 tmpvar_21;
  tmpvar_21 = fract((vec2(1.0, 255.0) * (1.0 - tmpvar_4)));
  enc_20.y = tmpvar_21.y;
  enc_20.x = (tmpvar_21.x - (tmpvar_21.y * 0.00392157));
  res_2.zw = enc_20;
  tmpvar_1 = res_2;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "SHADOWS_SPLIT_SPHERES" "SHADOWS_NATIVE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
in vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
out highp vec2 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
  xlv_TEXCOORD1 = _glesNormal;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowSplitSpheres[4];
uniform highp vec4 unity_ShadowSplitSqRadii;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp sampler2D _CameraDepthTexture;
uniform highp mat4 _CameraToWorld;
uniform lowp sampler2DShadow _ShadowMapTexture;
in highp vec2 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 res_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = texture (_CameraDepthTexture, xlv_TEXCOORD0);
  highp float tmpvar_4;
  tmpvar_4 = (1.0/(((_ZBufferParams.x * tmpvar_3.x) + _ZBufferParams.y)));
  highp vec4 tmpvar_5;
  tmpvar_5.w = 1.0;
  tmpvar_5.xyz = (xlv_TEXCOORD1 * tmpvar_4);
  highp vec4 tmpvar_6;
  tmpvar_6 = (_CameraToWorld * tmpvar_5);
  mediump float shadow_7;
  highp vec4 weights_8;
  highp vec3 tmpvar_9;
  tmpvar_9 = (tmpvar_6.xyz - unity_ShadowSplitSpheres[0].xyz);
  highp vec3 tmpvar_10;
  tmpvar_10 = (tmpvar_6.xyz - unity_ShadowSplitSpheres[1].xyz);
  highp vec3 tmpvar_11;
  tmpvar_11 = (tmpvar_6.xyz - unity_ShadowSplitSpheres[2].xyz);
  highp vec3 tmpvar_12;
  tmpvar_12 = (tmpvar_6.xyz - unity_ShadowSplitSpheres[3].xyz);
  highp vec4 tmpvar_13;
  tmpvar_13.x = dot (tmpvar_9, tmpvar_9);
  tmpvar_13.y = dot (tmpvar_10, tmpvar_10);
  tmpvar_13.z = dot (tmpvar_11, tmpvar_11);
  tmpvar_13.w = dot (tmpvar_12, tmpvar_12);
  bvec4 tmpvar_14;
  tmpvar_14 = lessThan (tmpvar_13, unity_ShadowSplitSqRadii);
  lowp vec4 tmpvar_15;
  tmpvar_15 = vec4(tmpvar_14);
  weights_8 = tmpvar_15;
  weights_8.yzw = clamp ((weights_8.yzw - weights_8.xyz), 0.0, 1.0);
  highp vec4 tmpvar_16;
  tmpvar_16.w = 1.0;
  tmpvar_16.xyz = (((
    ((unity_World2Shadow[0] * tmpvar_6).xyz * weights_8.x)
   + 
    ((unity_World2Shadow[1] * tmpvar_6).xyz * weights_8.y)
  ) + (
    (unity_World2Shadow[2] * tmpvar_6)
  .xyz * weights_8.z)) + ((unity_World2Shadow[3] * tmpvar_6).xyz * weights_8.w));
  mediump float tmpvar_17;
  tmpvar_17 = texture (_ShadowMapTexture, tmpvar_16.xyz);
  highp float tmpvar_18;
  tmpvar_18 = mix (_LightShadowData.x, 1.0, tmpvar_17);
  shadow_7 = tmpvar_18;
  res_2.x = shadow_7;
  res_2.y = 1.0;
  highp vec2 enc_19;
  highp vec2 tmpvar_20;
  tmpvar_20 = fract((vec2(1.0, 255.0) * (1.0 - tmpvar_4)));
  enc_19.y = tmpvar_20.y;
  enc_19.x = (tmpvar_20.x - (tmpvar_20.y * 0.00392157));
  res_2.zw = enc_19;
  tmpvar_1 = res_2;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "SHADOWS_NONATIVE" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SHADOWS_NATIVE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "SHADOWS_NATIVE" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "SHADOWS_SPLIT_SPHERES" "SHADOWS_NONATIVE" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SHADOWS_SPLIT_SPHERES" "SHADOWS_NATIVE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "SHADOWS_SPLIT_SPHERES" "SHADOWS_NATIVE" }
"!!GLES3"
}
}
 }
}
Fallback Off
}