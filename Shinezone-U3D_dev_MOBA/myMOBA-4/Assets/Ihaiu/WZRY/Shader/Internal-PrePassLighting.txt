Shader "Hidden/Internal-PrePassLighting" {
Properties {
 _LightTexture0 ("", any) = "" {}
 _LightTextureB0 ("", 2D) = "" {}
 _ShadowMapTexture ("", any) = "" {}
}
SubShader { 
 Pass {
  Tags { "ShadowSupport"="True" }
  ZWrite Off
  Fog { Mode Off }
  Blend DstColor Zero
Program "vp" {
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _LightTextureB0;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_16;
  tmpvar_16 = -(normalize(tmpvar_15));
  lightDir_6 = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = (dot (tmpvar_15, tmpvar_15) * _LightPos.w);
  lowp float tmpvar_18;
  tmpvar_18 = texture2D (_LightTextureB0, vec2(tmpvar_17)).w;
  atten_5 = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = max (0.0, dot (lightDir_6, tmpvar_10));
  highp vec3 tmpvar_20;
  tmpvar_20 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = pow (max (0.0, dot (h_4, tmpvar_10)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = (spec_3 * clamp (atten_5, 0.0, 1.0));
  spec_3 = tmpvar_22;
  highp vec3 tmpvar_23;
  tmpvar_23 = (_LightColor.xyz * (tmpvar_19 * atten_5));
  res_2.xyz = tmpvar_23;
  lowp vec3 c_24;
  c_24 = _LightColor.xyz;
  lowp float tmpvar_25;
  tmpvar_25 = dot (c_24, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_26;
  tmpvar_26 = (tmpvar_22 * tmpvar_25);
  res_2.w = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = clamp ((1.0 - (
    (mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_28;
  tmpvar_28 = (res_2 * tmpvar_27);
  res_2 = tmpvar_28;
  mediump vec4 tmpvar_29;
  tmpvar_29 = exp2(-(tmpvar_28));
  tmpvar_1 = tmpvar_29;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT" "SHADOWS_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _LightTextureB0;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_16;
  tmpvar_16 = -(normalize(tmpvar_15));
  lightDir_6 = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = (dot (tmpvar_15, tmpvar_15) * _LightPos.w);
  lowp float tmpvar_18;
  tmpvar_18 = texture (_LightTextureB0, vec2(tmpvar_17)).w;
  atten_5 = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = max (0.0, dot (lightDir_6, tmpvar_10));
  highp vec3 tmpvar_20;
  tmpvar_20 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = pow (max (0.0, dot (h_4, tmpvar_10)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = (spec_3 * clamp (atten_5, 0.0, 1.0));
  spec_3 = tmpvar_22;
  highp vec3 tmpvar_23;
  tmpvar_23 = (_LightColor.xyz * (tmpvar_19 * atten_5));
  res_2.xyz = tmpvar_23;
  lowp vec3 c_24;
  c_24 = _LightColor.xyz;
  lowp float tmpvar_25;
  tmpvar_25 = dot (c_24, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_26;
  tmpvar_26 = (tmpvar_22 * tmpvar_25);
  res_2.w = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = clamp ((1.0 - (
    (mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_28;
  tmpvar_28 = (res_2 * tmpvar_27);
  res_2 = tmpvar_28;
  mediump vec4 tmpvar_29;
  tmpvar_29 = exp2(-(tmpvar_28));
  tmpvar_1 = tmpvar_29;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec4 nspec_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_CameraNormalsTexture, tmpvar_7);
  nspec_6 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = normalize(((nspec_6.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraDepthTexture, tmpvar_7);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_14;
  tmpvar_14 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_14;
  mediump float tmpvar_15;
  tmpvar_15 = max (0.0, dot (lightDir_5, tmpvar_9));
  highp vec3 tmpvar_16;
  tmpvar_16 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_16;
  mediump float tmpvar_17;
  tmpvar_17 = pow (max (0.0, dot (h_4, tmpvar_9)), (nspec_6.w * 128.0));
  spec_3 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (spec_3 * clamp (1.0, 0.0, 1.0));
  spec_3 = tmpvar_18;
  highp vec3 tmpvar_19;
  tmpvar_19 = (_LightColor.xyz * tmpvar_15);
  res_2.xyz = tmpvar_19;
  lowp vec3 c_20;
  c_20 = _LightColor.xyz;
  lowp float tmpvar_21;
  tmpvar_21 = dot (c_20, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_18 * tmpvar_21);
  res_2.w = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = clamp ((1.0 - (
    (mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_24;
  tmpvar_24 = (res_2 * tmpvar_23);
  res_2 = tmpvar_24;
  mediump vec4 tmpvar_25;
  tmpvar_25 = exp2(-(tmpvar_24));
  tmpvar_1 = tmpvar_25;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec4 nspec_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture (_CameraNormalsTexture, tmpvar_7);
  nspec_6 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = normalize(((nspec_6.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraDepthTexture, tmpvar_7);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_14;
  tmpvar_14 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_14;
  mediump float tmpvar_15;
  tmpvar_15 = max (0.0, dot (lightDir_5, tmpvar_9));
  highp vec3 tmpvar_16;
  tmpvar_16 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_16;
  mediump float tmpvar_17;
  tmpvar_17 = pow (max (0.0, dot (h_4, tmpvar_9)), (nspec_6.w * 128.0));
  spec_3 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (spec_3 * clamp (1.0, 0.0, 1.0));
  spec_3 = tmpvar_18;
  highp vec3 tmpvar_19;
  tmpvar_19 = (_LightColor.xyz * tmpvar_15);
  res_2.xyz = tmpvar_19;
  lowp vec3 c_20;
  c_20 = _LightColor.xyz;
  lowp float tmpvar_21;
  tmpvar_21 = dot (c_20, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_18 * tmpvar_21);
  res_2.w = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = clamp ((1.0 - (
    (mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_24;
  tmpvar_24 = (res_2 * tmpvar_23);
  res_2 = tmpvar_24;
  mediump vec4 tmpvar_25;
  tmpvar_25 = exp2(-(tmpvar_24));
  tmpvar_1 = tmpvar_25;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_15;
  tmpvar_15 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_16;
  tmpvar_16 = normalize(tmpvar_15);
  lightDir_6 = tmpvar_16;
  highp vec4 tmpvar_17;
  tmpvar_17.w = 1.0;
  tmpvar_17.xyz = tmpvar_13;
  highp vec4 tmpvar_18;
  tmpvar_18 = (_LightMatrix0 * tmpvar_17);
  lowp float tmpvar_19;
  tmpvar_19 = texture2DProj (_LightTexture0, tmpvar_18).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = (dot (tmpvar_15, tmpvar_15) * _LightPos.w);
  lowp vec4 tmpvar_21;
  tmpvar_21 = texture2D (_LightTextureB0, vec2(tmpvar_20));
  highp float tmpvar_22;
  tmpvar_22 = ((atten_5 * float(
    (tmpvar_18.w < 0.0)
  )) * tmpvar_21.w);
  atten_5 = tmpvar_22;
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_6, tmpvar_10));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, tmpvar_10)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  mediump vec4 tmpvar_33;
  tmpvar_33 = exp2(-(tmpvar_32));
  tmpvar_1 = tmpvar_33;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "SPOT" "SHADOWS_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_15;
  tmpvar_15 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_16;
  tmpvar_16 = normalize(tmpvar_15);
  lightDir_6 = tmpvar_16;
  highp vec4 tmpvar_17;
  tmpvar_17.w = 1.0;
  tmpvar_17.xyz = tmpvar_13;
  highp vec4 tmpvar_18;
  tmpvar_18 = (_LightMatrix0 * tmpvar_17);
  lowp float tmpvar_19;
  tmpvar_19 = textureProj (_LightTexture0, tmpvar_18).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = (dot (tmpvar_15, tmpvar_15) * _LightPos.w);
  lowp vec4 tmpvar_21;
  tmpvar_21 = texture (_LightTextureB0, vec2(tmpvar_20));
  highp float tmpvar_22;
  tmpvar_22 = ((atten_5 * float(
    (tmpvar_18.w < 0.0)
  )) * tmpvar_21.w);
  atten_5 = tmpvar_22;
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_6, tmpvar_10));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, tmpvar_10)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  mediump vec4 tmpvar_33;
  tmpvar_33 = exp2(-(tmpvar_32));
  tmpvar_1 = tmpvar_33;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _LightTexture0;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_16;
  tmpvar_16 = -(normalize(tmpvar_15));
  lightDir_6 = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = (dot (tmpvar_15, tmpvar_15) * _LightPos.w);
  lowp float tmpvar_18;
  tmpvar_18 = texture2D (_LightTextureB0, vec2(tmpvar_17)).w;
  atten_5 = tmpvar_18;
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = tmpvar_13;
  lowp vec4 tmpvar_20;
  highp vec3 P_21;
  P_21 = (_LightMatrix0 * tmpvar_19).xyz;
  tmpvar_20 = textureCube (_LightTexture0, P_21);
  highp float tmpvar_22;
  tmpvar_22 = (atten_5 * tmpvar_20.w);
  atten_5 = tmpvar_22;
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_6, tmpvar_10));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, tmpvar_10)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  mediump vec4 tmpvar_33;
  tmpvar_33 = exp2(-(tmpvar_32));
  tmpvar_1 = tmpvar_33;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" "SHADOWS_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _LightTexture0;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_16;
  tmpvar_16 = -(normalize(tmpvar_15));
  lightDir_6 = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = (dot (tmpvar_15, tmpvar_15) * _LightPos.w);
  lowp float tmpvar_18;
  tmpvar_18 = texture (_LightTextureB0, vec2(tmpvar_17)).w;
  atten_5 = tmpvar_18;
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = tmpvar_13;
  lowp vec4 tmpvar_20;
  highp vec3 P_21;
  P_21 = (_LightMatrix0 * tmpvar_19).xyz;
  tmpvar_20 = texture (_LightTexture0, P_21);
  highp float tmpvar_22;
  tmpvar_22 = (atten_5 * tmpvar_20.w);
  atten_5 = tmpvar_22;
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_6, tmpvar_10));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, tmpvar_10)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  mediump vec4 tmpvar_33;
  tmpvar_33 = exp2(-(tmpvar_32));
  tmpvar_1 = tmpvar_33;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTexture0;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec4 nspec_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_CameraNormalsTexture, tmpvar_7);
  nspec_6 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = normalize(((nspec_6.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraDepthTexture, tmpvar_7);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_14;
  tmpvar_14 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_14;
  highp vec4 tmpvar_15;
  tmpvar_15.w = 1.0;
  tmpvar_15.xyz = tmpvar_12;
  lowp vec4 tmpvar_16;
  highp vec2 P_17;
  P_17 = (_LightMatrix0 * tmpvar_15).xy;
  tmpvar_16 = texture2D (_LightTexture0, P_17);
  highp float tmpvar_18;
  tmpvar_18 = tmpvar_16.w;
  mediump float tmpvar_19;
  tmpvar_19 = max (0.0, dot (lightDir_5, tmpvar_9));
  highp vec3 tmpvar_20;
  tmpvar_20 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = pow (max (0.0, dot (h_4, tmpvar_9)), (nspec_6.w * 128.0));
  spec_3 = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = (spec_3 * clamp (tmpvar_18, 0.0, 1.0));
  spec_3 = tmpvar_22;
  highp vec3 tmpvar_23;
  tmpvar_23 = (_LightColor.xyz * (tmpvar_19 * tmpvar_18));
  res_2.xyz = tmpvar_23;
  lowp vec3 c_24;
  c_24 = _LightColor.xyz;
  lowp float tmpvar_25;
  tmpvar_25 = dot (c_24, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_26;
  tmpvar_26 = (tmpvar_22 * tmpvar_25);
  res_2.w = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = clamp ((1.0 - (
    (mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_28;
  tmpvar_28 = (res_2 * tmpvar_27);
  res_2 = tmpvar_28;
  mediump vec4 tmpvar_29;
  tmpvar_29 = exp2(-(tmpvar_28));
  tmpvar_1 = tmpvar_29;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTexture0;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec4 nspec_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture (_CameraNormalsTexture, tmpvar_7);
  nspec_6 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = normalize(((nspec_6.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraDepthTexture, tmpvar_7);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_14;
  tmpvar_14 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_14;
  highp vec4 tmpvar_15;
  tmpvar_15.w = 1.0;
  tmpvar_15.xyz = tmpvar_12;
  lowp vec4 tmpvar_16;
  highp vec2 P_17;
  P_17 = (_LightMatrix0 * tmpvar_15).xy;
  tmpvar_16 = texture (_LightTexture0, P_17);
  highp float tmpvar_18;
  tmpvar_18 = tmpvar_16.w;
  mediump float tmpvar_19;
  tmpvar_19 = max (0.0, dot (lightDir_5, tmpvar_9));
  highp vec3 tmpvar_20;
  tmpvar_20 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = pow (max (0.0, dot (h_4, tmpvar_9)), (nspec_6.w * 128.0));
  spec_3 = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = (spec_3 * clamp (tmpvar_18, 0.0, 1.0));
  spec_3 = tmpvar_22;
  highp vec3 tmpvar_23;
  tmpvar_23 = (_LightColor.xyz * (tmpvar_19 * tmpvar_18));
  res_2.xyz = tmpvar_23;
  lowp vec3 c_24;
  c_24 = _LightColor.xyz;
  lowp float tmpvar_25;
  tmpvar_25 = dot (c_24, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_26;
  tmpvar_26 = (tmpvar_22 * tmpvar_25);
  res_2.w = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = clamp ((1.0 - (
    (mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_28;
  tmpvar_28 = (res_2 * tmpvar_27);
  res_2 = tmpvar_28;
  mediump vec4 tmpvar_29;
  tmpvar_29 = exp2(-(tmpvar_28));
  tmpvar_1 = tmpvar_29;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_NONATIVE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform sampler2D _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize(tmpvar_16);
  lightDir_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_13;
  highp vec4 tmpvar_19;
  tmpvar_19 = (_LightMatrix0 * tmpvar_18);
  lowp float tmpvar_20;
  tmpvar_20 = texture2DProj (_LightTexture0, tmpvar_19).w;
  atten_5 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture2D (_LightTextureB0, vec2(tmpvar_21));
  atten_5 = ((atten_5 * float(
    (tmpvar_19.w < 0.0)
  )) * tmpvar_22.w);
  mediump float tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = clamp (((tmpvar_15 * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0);
  highp vec4 tmpvar_25;
  tmpvar_25.w = 1.0;
  tmpvar_25.xyz = tmpvar_13;
  highp vec4 tmpvar_26;
  tmpvar_26 = (unity_World2Shadow[0] * tmpvar_25);
  mediump float shadow_27;
  lowp vec4 tmpvar_28;
  tmpvar_28 = texture2DProj (_ShadowMapTexture, tmpvar_26);
  highp float tmpvar_29;
  if ((tmpvar_28.x < (tmpvar_26.z / tmpvar_26.w))) {
    tmpvar_29 = _LightShadowData.x;
  } else {
    tmpvar_29 = 1.0;
  };
  shadow_27 = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = clamp ((shadow_27 + tmpvar_24), 0.0, 1.0);
  tmpvar_23 = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = (atten_5 * tmpvar_23);
  atten_5 = tmpvar_31;
  mediump float tmpvar_32;
  tmpvar_32 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_33;
  tmpvar_33 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_33;
  mediump float tmpvar_34;
  tmpvar_34 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_34;
  highp float tmpvar_35;
  tmpvar_35 = (spec_3 * clamp (tmpvar_31, 0.0, 1.0));
  spec_3 = tmpvar_35;
  highp vec3 tmpvar_36;
  tmpvar_36 = (_LightColor.xyz * (tmpvar_32 * tmpvar_31));
  res_2.xyz = tmpvar_36;
  lowp vec3 c_37;
  c_37 = _LightColor.xyz;
  lowp float tmpvar_38;
  tmpvar_38 = dot (c_37, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_39;
  tmpvar_39 = (tmpvar_35 * tmpvar_38);
  res_2.w = tmpvar_39;
  highp float tmpvar_40;
  tmpvar_40 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_41;
  tmpvar_41 = (res_2 * tmpvar_40);
  res_2 = tmpvar_41;
  mediump vec4 tmpvar_42;
  tmpvar_42 = exp2(-(tmpvar_41));
  tmpvar_1 = tmpvar_42;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_NATIVE" }
"!!GLES


#ifdef VERTEX

#extension GL_EXT_shadow_samplers : enable
attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

#extension GL_EXT_shadow_samplers : enable
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp sampler2DShadow _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize(tmpvar_16);
  lightDir_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_13;
  highp vec4 tmpvar_19;
  tmpvar_19 = (_LightMatrix0 * tmpvar_18);
  lowp float tmpvar_20;
  tmpvar_20 = texture2DProj (_LightTexture0, tmpvar_19).w;
  atten_5 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture2D (_LightTextureB0, vec2(tmpvar_21));
  atten_5 = ((atten_5 * float(
    (tmpvar_19.w < 0.0)
  )) * tmpvar_22.w);
  mediump float tmpvar_23;
  highp vec4 tmpvar_24;
  tmpvar_24.w = 1.0;
  tmpvar_24.xyz = tmpvar_13;
  highp vec4 tmpvar_25;
  tmpvar_25 = (unity_World2Shadow[0] * tmpvar_24);
  mediump float shadow_26;
  lowp float tmpvar_27;
  tmpvar_27 = shadow2DProjEXT (_ShadowMapTexture, tmpvar_25);
  mediump float tmpvar_28;
  tmpvar_28 = tmpvar_27;
  highp float tmpvar_29;
  tmpvar_29 = (_LightShadowData.x + (tmpvar_28 * (1.0 - _LightShadowData.x)));
  shadow_26 = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = clamp ((shadow_26 + clamp (
    ((tmpvar_15 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_23 = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = (atten_5 * tmpvar_23);
  atten_5 = tmpvar_31;
  mediump float tmpvar_32;
  tmpvar_32 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_33;
  tmpvar_33 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_33;
  mediump float tmpvar_34;
  tmpvar_34 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_34;
  highp float tmpvar_35;
  tmpvar_35 = (spec_3 * clamp (tmpvar_31, 0.0, 1.0));
  spec_3 = tmpvar_35;
  highp vec3 tmpvar_36;
  tmpvar_36 = (_LightColor.xyz * (tmpvar_32 * tmpvar_31));
  res_2.xyz = tmpvar_36;
  lowp vec3 c_37;
  c_37 = _LightColor.xyz;
  lowp float tmpvar_38;
  tmpvar_38 = dot (c_37, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_39;
  tmpvar_39 = (tmpvar_35 * tmpvar_38);
  res_2.w = tmpvar_39;
  highp float tmpvar_40;
  tmpvar_40 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_41;
  tmpvar_41 = (res_2 * tmpvar_40);
  res_2 = tmpvar_41;
  mediump vec4 tmpvar_42;
  tmpvar_42 = exp2(-(tmpvar_41));
  tmpvar_1 = tmpvar_42;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_NATIVE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp sampler2DShadow _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize(tmpvar_16);
  lightDir_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_13;
  highp vec4 tmpvar_19;
  tmpvar_19 = (_LightMatrix0 * tmpvar_18);
  lowp float tmpvar_20;
  tmpvar_20 = textureProj (_LightTexture0, tmpvar_19).w;
  atten_5 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture (_LightTextureB0, vec2(tmpvar_21));
  atten_5 = ((atten_5 * float(
    (tmpvar_19.w < 0.0)
  )) * tmpvar_22.w);
  mediump float tmpvar_23;
  highp vec4 tmpvar_24;
  tmpvar_24.w = 1.0;
  tmpvar_24.xyz = tmpvar_13;
  highp vec4 tmpvar_25;
  tmpvar_25 = (unity_World2Shadow[0] * tmpvar_24);
  mediump float shadow_26;
  mediump float tmpvar_27;
  tmpvar_27 = textureProj (_ShadowMapTexture, tmpvar_25);
  highp float tmpvar_28;
  tmpvar_28 = (_LightShadowData.x + (tmpvar_27 * (1.0 - _LightShadowData.x)));
  shadow_26 = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = clamp ((shadow_26 + clamp (
    ((tmpvar_15 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_23 = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = (atten_5 * tmpvar_23);
  atten_5 = tmpvar_30;
  mediump float tmpvar_31;
  tmpvar_31 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_32;
  tmpvar_32 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_32;
  mediump float tmpvar_33;
  tmpvar_33 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_33;
  highp float tmpvar_34;
  tmpvar_34 = (spec_3 * clamp (tmpvar_30, 0.0, 1.0));
  spec_3 = tmpvar_34;
  highp vec3 tmpvar_35;
  tmpvar_35 = (_LightColor.xyz * (tmpvar_31 * tmpvar_30));
  res_2.xyz = tmpvar_35;
  lowp vec3 c_36;
  c_36 = _LightColor.xyz;
  lowp float tmpvar_37;
  tmpvar_37 = dot (c_36, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_38;
  tmpvar_38 = (tmpvar_34 * tmpvar_37);
  res_2.w = tmpvar_38;
  highp float tmpvar_39;
  tmpvar_39 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_40;
  tmpvar_40 = (res_2 * tmpvar_39);
  res_2 = tmpvar_40;
  mediump vec4 tmpvar_41;
  tmpvar_41 = exp2(-(tmpvar_40));
  tmpvar_1 = tmpvar_41;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec3 normal_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  normal_6 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_14;
  tmpvar_14 = mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_15;
  tmpvar_15 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_15;
  mediump float tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture2D (_ShadowMapTexture, tmpvar_8);
  highp float tmpvar_18;
  tmpvar_18 = clamp ((tmpvar_17.x + clamp (
    ((tmpvar_14 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_16 = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = tmpvar_16;
  mediump float tmpvar_20;
  tmpvar_20 = max (0.0, dot (lightDir_5, normal_6));
  highp vec3 tmpvar_21;
  tmpvar_21 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_21;
  mediump float tmpvar_22;
  tmpvar_22 = pow (max (0.0, dot (h_4, normal_6)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = (spec_3 * clamp (tmpvar_19, 0.0, 1.0));
  spec_3 = tmpvar_23;
  highp vec3 tmpvar_24;
  tmpvar_24 = (_LightColor.xyz * (tmpvar_20 * tmpvar_19));
  res_2.xyz = tmpvar_24;
  lowp vec3 c_25;
  c_25 = _LightColor.xyz;
  lowp float tmpvar_26;
  tmpvar_26 = dot (c_25, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_27;
  tmpvar_27 = (tmpvar_23 * tmpvar_26);
  res_2.w = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = clamp ((1.0 - (
    (tmpvar_14 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_29;
  tmpvar_29 = (res_2 * tmpvar_28);
  res_2 = tmpvar_29;
  mediump vec4 tmpvar_30;
  tmpvar_30 = exp2(-(tmpvar_29));
  tmpvar_1 = tmpvar_30;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTexture0;
uniform sampler2D _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec3 normal_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  normal_6 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_14;
  tmpvar_14 = mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_15;
  tmpvar_15 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_15;
  mediump float tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture2D (_ShadowMapTexture, tmpvar_8);
  highp float tmpvar_18;
  tmpvar_18 = clamp ((tmpvar_17.x + clamp (
    ((tmpvar_14 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_16 = tmpvar_18;
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = tmpvar_12;
  lowp vec4 tmpvar_20;
  highp vec2 P_21;
  P_21 = (_LightMatrix0 * tmpvar_19).xy;
  tmpvar_20 = texture2D (_LightTexture0, P_21);
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_16 * tmpvar_20.w);
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_5, normal_6));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, normal_6)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (tmpvar_14 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  mediump vec4 tmpvar_33;
  tmpvar_33 = exp2(-(tmpvar_32));
  tmpvar_1 = tmpvar_33;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture2D (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  mediump float tmpvar_21;
  highp vec4 packDist_22;
  lowp vec4 tmpvar_23;
  tmpvar_23 = textureCube (_ShadowMapTexture, tmpvar_16);
  packDist_22 = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (packDist_22, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp float tmpvar_25;
  if ((tmpvar_24 < tmpvar_20)) {
    tmpvar_25 = _LightShadowData.x;
  } else {
    tmpvar_25 = 1.0;
  };
  tmpvar_21 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (atten_5 * tmpvar_21);
  atten_5 = tmpvar_26;
  mediump float tmpvar_27;
  tmpvar_27 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_28;
  tmpvar_28 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_28;
  mediump float tmpvar_29;
  tmpvar_29 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = (spec_3 * clamp (tmpvar_26, 0.0, 1.0));
  spec_3 = tmpvar_30;
  highp vec3 tmpvar_31;
  tmpvar_31 = (_LightColor.xyz * (tmpvar_27 * tmpvar_26));
  res_2.xyz = tmpvar_31;
  lowp vec3 c_32;
  c_32 = _LightColor.xyz;
  lowp float tmpvar_33;
  tmpvar_33 = dot (c_32, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_34;
  tmpvar_34 = (tmpvar_30 * tmpvar_33);
  res_2.w = tmpvar_34;
  highp float tmpvar_35;
  tmpvar_35 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_36;
  tmpvar_36 = (res_2 * tmpvar_35);
  res_2 = tmpvar_36;
  mediump vec4 tmpvar_37;
  tmpvar_37 = exp2(-(tmpvar_36));
  tmpvar_1 = tmpvar_37;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT" "SHADOWS_CUBE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  mediump float tmpvar_21;
  highp vec4 packDist_22;
  lowp vec4 tmpvar_23;
  tmpvar_23 = texture (_ShadowMapTexture, tmpvar_16);
  packDist_22 = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (packDist_22, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp float tmpvar_25;
  if ((tmpvar_24 < tmpvar_20)) {
    tmpvar_25 = _LightShadowData.x;
  } else {
    tmpvar_25 = 1.0;
  };
  tmpvar_21 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (atten_5 * tmpvar_21);
  atten_5 = tmpvar_26;
  mediump float tmpvar_27;
  tmpvar_27 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_28;
  tmpvar_28 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_28;
  mediump float tmpvar_29;
  tmpvar_29 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = (spec_3 * clamp (tmpvar_26, 0.0, 1.0));
  spec_3 = tmpvar_30;
  highp vec3 tmpvar_31;
  tmpvar_31 = (_LightColor.xyz * (tmpvar_27 * tmpvar_26));
  res_2.xyz = tmpvar_31;
  lowp vec3 c_32;
  c_32 = _LightColor.xyz;
  lowp float tmpvar_33;
  tmpvar_33 = dot (c_32, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_34;
  tmpvar_34 = (tmpvar_30 * tmpvar_33);
  res_2.w = tmpvar_34;
  highp float tmpvar_35;
  tmpvar_35 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_36;
  tmpvar_36 = (res_2 * tmpvar_35);
  res_2 = tmpvar_36;
  mediump vec4 tmpvar_37;
  tmpvar_37 = exp2(-(tmpvar_36));
  tmpvar_1 = tmpvar_37;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _LightTexture0;
uniform lowp samplerCube _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture2D (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  mediump float tmpvar_21;
  highp vec4 packDist_22;
  lowp vec4 tmpvar_23;
  tmpvar_23 = textureCube (_ShadowMapTexture, tmpvar_16);
  packDist_22 = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (packDist_22, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp float tmpvar_25;
  if ((tmpvar_24 < tmpvar_20)) {
    tmpvar_25 = _LightShadowData.x;
  } else {
    tmpvar_25 = 1.0;
  };
  tmpvar_21 = tmpvar_25;
  highp vec4 tmpvar_26;
  tmpvar_26.w = 1.0;
  tmpvar_26.xyz = tmpvar_13;
  lowp vec4 tmpvar_27;
  highp vec3 P_28;
  P_28 = (_LightMatrix0 * tmpvar_26).xyz;
  tmpvar_27 = textureCube (_LightTexture0, P_28);
  highp float tmpvar_29;
  tmpvar_29 = ((atten_5 * tmpvar_21) * tmpvar_27.w);
  atten_5 = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_31;
  tmpvar_31 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_31;
  mediump float tmpvar_32;
  tmpvar_32 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_32;
  highp float tmpvar_33;
  tmpvar_33 = (spec_3 * clamp (tmpvar_29, 0.0, 1.0));
  spec_3 = tmpvar_33;
  highp vec3 tmpvar_34;
  tmpvar_34 = (_LightColor.xyz * (tmpvar_30 * tmpvar_29));
  res_2.xyz = tmpvar_34;
  lowp vec3 c_35;
  c_35 = _LightColor.xyz;
  lowp float tmpvar_36;
  tmpvar_36 = dot (c_35, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_37;
  tmpvar_37 = (tmpvar_33 * tmpvar_36);
  res_2.w = tmpvar_37;
  highp float tmpvar_38;
  tmpvar_38 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_39;
  tmpvar_39 = (res_2 * tmpvar_38);
  res_2 = tmpvar_39;
  mediump vec4 tmpvar_40;
  tmpvar_40 = exp2(-(tmpvar_39));
  tmpvar_1 = tmpvar_40;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _LightTexture0;
uniform lowp samplerCube _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  mediump float tmpvar_21;
  highp vec4 packDist_22;
  lowp vec4 tmpvar_23;
  tmpvar_23 = texture (_ShadowMapTexture, tmpvar_16);
  packDist_22 = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (packDist_22, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp float tmpvar_25;
  if ((tmpvar_24 < tmpvar_20)) {
    tmpvar_25 = _LightShadowData.x;
  } else {
    tmpvar_25 = 1.0;
  };
  tmpvar_21 = tmpvar_25;
  highp vec4 tmpvar_26;
  tmpvar_26.w = 1.0;
  tmpvar_26.xyz = tmpvar_13;
  lowp vec4 tmpvar_27;
  highp vec3 P_28;
  P_28 = (_LightMatrix0 * tmpvar_26).xyz;
  tmpvar_27 = texture (_LightTexture0, P_28);
  highp float tmpvar_29;
  tmpvar_29 = ((atten_5 * tmpvar_21) * tmpvar_27.w);
  atten_5 = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_31;
  tmpvar_31 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_31;
  mediump float tmpvar_32;
  tmpvar_32 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_32;
  highp float tmpvar_33;
  tmpvar_33 = (spec_3 * clamp (tmpvar_29, 0.0, 1.0));
  spec_3 = tmpvar_33;
  highp vec3 tmpvar_34;
  tmpvar_34 = (_LightColor.xyz * (tmpvar_30 * tmpvar_29));
  res_2.xyz = tmpvar_34;
  lowp vec3 c_35;
  c_35 = _LightColor.xyz;
  lowp float tmpvar_36;
  tmpvar_36 = dot (c_35, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_37;
  tmpvar_37 = (tmpvar_33 * tmpvar_36);
  res_2.w = tmpvar_37;
  highp float tmpvar_38;
  tmpvar_38 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_39;
  tmpvar_39 = (res_2 * tmpvar_38);
  res_2 = tmpvar_39;
  mediump vec4 tmpvar_40;
  tmpvar_40 = exp2(-(tmpvar_39));
  tmpvar_1 = tmpvar_40;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SHADOWS_NONATIVE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _ShadowOffsets[4];
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize(tmpvar_16);
  lightDir_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_13;
  highp vec4 tmpvar_19;
  tmpvar_19 = (_LightMatrix0 * tmpvar_18);
  lowp float tmpvar_20;
  tmpvar_20 = texture2DProj (_LightTexture0, tmpvar_19).w;
  atten_5 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture2D (_LightTextureB0, vec2(tmpvar_21));
  atten_5 = ((atten_5 * float(
    (tmpvar_19.w < 0.0)
  )) * tmpvar_22.w);
  mediump float tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = clamp (((tmpvar_15 * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0);
  highp vec4 tmpvar_25;
  tmpvar_25.w = 1.0;
  tmpvar_25.xyz = tmpvar_13;
  highp vec4 tmpvar_26;
  tmpvar_26 = (unity_World2Shadow[0] * tmpvar_25);
  highp vec4 shadowVals_27;
  highp vec3 tmpvar_28;
  tmpvar_28 = (tmpvar_26.xyz / tmpvar_26.w);
  highp vec2 P_29;
  P_29 = (tmpvar_28.xy + _ShadowOffsets[0].xy);
  lowp float tmpvar_30;
  tmpvar_30 = texture2D (_ShadowMapTexture, P_29).x;
  shadowVals_27.x = tmpvar_30;
  highp vec2 P_31;
  P_31 = (tmpvar_28.xy + _ShadowOffsets[1].xy);
  lowp float tmpvar_32;
  tmpvar_32 = texture2D (_ShadowMapTexture, P_31).x;
  shadowVals_27.y = tmpvar_32;
  highp vec2 P_33;
  P_33 = (tmpvar_28.xy + _ShadowOffsets[2].xy);
  lowp float tmpvar_34;
  tmpvar_34 = texture2D (_ShadowMapTexture, P_33).x;
  shadowVals_27.z = tmpvar_34;
  highp vec2 P_35;
  P_35 = (tmpvar_28.xy + _ShadowOffsets[3].xy);
  lowp float tmpvar_36;
  tmpvar_36 = texture2D (_ShadowMapTexture, P_35).x;
  shadowVals_27.w = tmpvar_36;
  bvec4 tmpvar_37;
  tmpvar_37 = lessThan (shadowVals_27, tmpvar_28.zzzz);
  highp vec4 tmpvar_38;
  tmpvar_38 = _LightShadowData.xxxx;
  highp float tmpvar_39;
  if (tmpvar_37.x) {
    tmpvar_39 = tmpvar_38.x;
  } else {
    tmpvar_39 = 1.0;
  };
  highp float tmpvar_40;
  if (tmpvar_37.y) {
    tmpvar_40 = tmpvar_38.y;
  } else {
    tmpvar_40 = 1.0;
  };
  highp float tmpvar_41;
  if (tmpvar_37.z) {
    tmpvar_41 = tmpvar_38.z;
  } else {
    tmpvar_41 = 1.0;
  };
  highp float tmpvar_42;
  if (tmpvar_37.w) {
    tmpvar_42 = tmpvar_38.w;
  } else {
    tmpvar_42 = 1.0;
  };
  mediump vec4 tmpvar_43;
  tmpvar_43.x = tmpvar_39;
  tmpvar_43.y = tmpvar_40;
  tmpvar_43.z = tmpvar_41;
  tmpvar_43.w = tmpvar_42;
  mediump float tmpvar_44;
  tmpvar_44 = dot (tmpvar_43, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_45;
  tmpvar_45 = clamp ((tmpvar_44 + tmpvar_24), 0.0, 1.0);
  tmpvar_23 = tmpvar_45;
  highp float tmpvar_46;
  tmpvar_46 = (atten_5 * tmpvar_23);
  atten_5 = tmpvar_46;
  mediump float tmpvar_47;
  tmpvar_47 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_48;
  tmpvar_48 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_48;
  mediump float tmpvar_49;
  tmpvar_49 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_49;
  highp float tmpvar_50;
  tmpvar_50 = (spec_3 * clamp (tmpvar_46, 0.0, 1.0));
  spec_3 = tmpvar_50;
  highp vec3 tmpvar_51;
  tmpvar_51 = (_LightColor.xyz * (tmpvar_47 * tmpvar_46));
  res_2.xyz = tmpvar_51;
  lowp vec3 c_52;
  c_52 = _LightColor.xyz;
  lowp float tmpvar_53;
  tmpvar_53 = dot (c_52, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_54;
  tmpvar_54 = (tmpvar_50 * tmpvar_53);
  res_2.w = tmpvar_54;
  highp float tmpvar_55;
  tmpvar_55 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_56;
  tmpvar_56 = (res_2 * tmpvar_55);
  res_2 = tmpvar_56;
  mediump vec4 tmpvar_57;
  tmpvar_57 = exp2(-(tmpvar_56));
  tmpvar_1 = tmpvar_57;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SHADOWS_NATIVE" }
"!!GLES


#ifdef VERTEX

#extension GL_EXT_shadow_samplers : enable
attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

#extension GL_EXT_shadow_samplers : enable
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp sampler2DShadow _ShadowMapTexture;
uniform highp vec4 _ShadowOffsets[4];
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize(tmpvar_16);
  lightDir_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_13;
  highp vec4 tmpvar_19;
  tmpvar_19 = (_LightMatrix0 * tmpvar_18);
  lowp float tmpvar_20;
  tmpvar_20 = texture2DProj (_LightTexture0, tmpvar_19).w;
  atten_5 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture2D (_LightTextureB0, vec2(tmpvar_21));
  atten_5 = ((atten_5 * float(
    (tmpvar_19.w < 0.0)
  )) * tmpvar_22.w);
  mediump float tmpvar_23;
  highp vec4 tmpvar_24;
  tmpvar_24.w = 1.0;
  tmpvar_24.xyz = tmpvar_13;
  highp vec4 tmpvar_25;
  tmpvar_25 = (unity_World2Shadow[0] * tmpvar_24);
  mediump vec4 shadows_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (tmpvar_25.xyz / tmpvar_25.w);
  highp vec3 coord_28;
  coord_28 = (tmpvar_27 + _ShadowOffsets[0].xyz);
  lowp float tmpvar_29;
  tmpvar_29 = shadow2DEXT (_ShadowMapTexture, coord_28);
  shadows_26.x = tmpvar_29;
  highp vec3 coord_30;
  coord_30 = (tmpvar_27 + _ShadowOffsets[1].xyz);
  lowp float tmpvar_31;
  tmpvar_31 = shadow2DEXT (_ShadowMapTexture, coord_30);
  shadows_26.y = tmpvar_31;
  highp vec3 coord_32;
  coord_32 = (tmpvar_27 + _ShadowOffsets[2].xyz);
  lowp float tmpvar_33;
  tmpvar_33 = shadow2DEXT (_ShadowMapTexture, coord_32);
  shadows_26.z = tmpvar_33;
  highp vec3 coord_34;
  coord_34 = (tmpvar_27 + _ShadowOffsets[3].xyz);
  lowp float tmpvar_35;
  tmpvar_35 = shadow2DEXT (_ShadowMapTexture, coord_34);
  shadows_26.w = tmpvar_35;
  highp vec4 tmpvar_36;
  tmpvar_36 = (_LightShadowData.xxxx + (shadows_26 * (1.0 - _LightShadowData.xxxx)));
  shadows_26 = tmpvar_36;
  mediump float tmpvar_37;
  tmpvar_37 = dot (shadows_26, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_38;
  tmpvar_38 = clamp ((tmpvar_37 + clamp (
    ((tmpvar_15 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_23 = tmpvar_38;
  highp float tmpvar_39;
  tmpvar_39 = (atten_5 * tmpvar_23);
  atten_5 = tmpvar_39;
  mediump float tmpvar_40;
  tmpvar_40 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_41;
  tmpvar_41 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_41;
  mediump float tmpvar_42;
  tmpvar_42 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_42;
  highp float tmpvar_43;
  tmpvar_43 = (spec_3 * clamp (tmpvar_39, 0.0, 1.0));
  spec_3 = tmpvar_43;
  highp vec3 tmpvar_44;
  tmpvar_44 = (_LightColor.xyz * (tmpvar_40 * tmpvar_39));
  res_2.xyz = tmpvar_44;
  lowp vec3 c_45;
  c_45 = _LightColor.xyz;
  lowp float tmpvar_46;
  tmpvar_46 = dot (c_45, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_47;
  tmpvar_47 = (tmpvar_43 * tmpvar_46);
  res_2.w = tmpvar_47;
  highp float tmpvar_48;
  tmpvar_48 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_49;
  tmpvar_49 = (res_2 * tmpvar_48);
  res_2 = tmpvar_49;
  mediump vec4 tmpvar_50;
  tmpvar_50 = exp2(-(tmpvar_49));
  tmpvar_1 = tmpvar_50;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SHADOWS_NATIVE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp sampler2DShadow _ShadowMapTexture;
uniform highp vec4 _ShadowOffsets[4];
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize(tmpvar_16);
  lightDir_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_13;
  highp vec4 tmpvar_19;
  tmpvar_19 = (_LightMatrix0 * tmpvar_18);
  lowp float tmpvar_20;
  tmpvar_20 = textureProj (_LightTexture0, tmpvar_19).w;
  atten_5 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture (_LightTextureB0, vec2(tmpvar_21));
  atten_5 = ((atten_5 * float(
    (tmpvar_19.w < 0.0)
  )) * tmpvar_22.w);
  mediump float tmpvar_23;
  highp vec4 tmpvar_24;
  tmpvar_24.w = 1.0;
  tmpvar_24.xyz = tmpvar_13;
  highp vec4 tmpvar_25;
  tmpvar_25 = (unity_World2Shadow[0] * tmpvar_24);
  mediump vec4 shadows_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (tmpvar_25.xyz / tmpvar_25.w);
  highp vec3 coord_28;
  coord_28 = (tmpvar_27 + _ShadowOffsets[0].xyz);
  mediump float tmpvar_29;
  tmpvar_29 = texture (_ShadowMapTexture, coord_28);
  shadows_26.x = tmpvar_29;
  highp vec3 coord_30;
  coord_30 = (tmpvar_27 + _ShadowOffsets[1].xyz);
  mediump float tmpvar_31;
  tmpvar_31 = texture (_ShadowMapTexture, coord_30);
  shadows_26.y = tmpvar_31;
  highp vec3 coord_32;
  coord_32 = (tmpvar_27 + _ShadowOffsets[2].xyz);
  mediump float tmpvar_33;
  tmpvar_33 = texture (_ShadowMapTexture, coord_32);
  shadows_26.z = tmpvar_33;
  highp vec3 coord_34;
  coord_34 = (tmpvar_27 + _ShadowOffsets[3].xyz);
  mediump float tmpvar_35;
  tmpvar_35 = texture (_ShadowMapTexture, coord_34);
  shadows_26.w = tmpvar_35;
  highp vec4 tmpvar_36;
  tmpvar_36 = (_LightShadowData.xxxx + (shadows_26 * (1.0 - _LightShadowData.xxxx)));
  shadows_26 = tmpvar_36;
  mediump float tmpvar_37;
  tmpvar_37 = dot (shadows_26, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_38;
  tmpvar_38 = clamp ((tmpvar_37 + clamp (
    ((tmpvar_15 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_23 = tmpvar_38;
  highp float tmpvar_39;
  tmpvar_39 = (atten_5 * tmpvar_23);
  atten_5 = tmpvar_39;
  mediump float tmpvar_40;
  tmpvar_40 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_41;
  tmpvar_41 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_41;
  mediump float tmpvar_42;
  tmpvar_42 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_42;
  highp float tmpvar_43;
  tmpvar_43 = (spec_3 * clamp (tmpvar_39, 0.0, 1.0));
  spec_3 = tmpvar_43;
  highp vec3 tmpvar_44;
  tmpvar_44 = (_LightColor.xyz * (tmpvar_40 * tmpvar_39));
  res_2.xyz = tmpvar_44;
  lowp vec3 c_45;
  c_45 = _LightColor.xyz;
  lowp float tmpvar_46;
  tmpvar_46 = dot (c_45, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_47;
  tmpvar_47 = (tmpvar_43 * tmpvar_46);
  res_2.w = tmpvar_47;
  highp float tmpvar_48;
  tmpvar_48 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_49;
  tmpvar_49 = (res_2 * tmpvar_48);
  res_2 = tmpvar_49;
  mediump vec4 tmpvar_50;
  tmpvar_50 = exp2(-(tmpvar_49));
  tmpvar_1 = tmpvar_50;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture2D (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  highp vec4 shadowVals_21;
  highp vec3 vec_22;
  vec_22 = (tmpvar_16 + vec3(0.0078125, 0.0078125, 0.0078125));
  highp vec4 packDist_23;
  lowp vec4 tmpvar_24;
  tmpvar_24 = textureCube (_ShadowMapTexture, vec_22);
  packDist_23 = tmpvar_24;
  shadowVals_21.x = dot (packDist_23, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_25;
  vec_25 = (tmpvar_16 + vec3(-0.0078125, -0.0078125, 0.0078125));
  highp vec4 packDist_26;
  lowp vec4 tmpvar_27;
  tmpvar_27 = textureCube (_ShadowMapTexture, vec_25);
  packDist_26 = tmpvar_27;
  shadowVals_21.y = dot (packDist_26, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_28;
  vec_28 = (tmpvar_16 + vec3(-0.0078125, 0.0078125, -0.0078125));
  highp vec4 packDist_29;
  lowp vec4 tmpvar_30;
  tmpvar_30 = textureCube (_ShadowMapTexture, vec_28);
  packDist_29 = tmpvar_30;
  shadowVals_21.z = dot (packDist_29, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_31;
  vec_31 = (tmpvar_16 + vec3(0.0078125, -0.0078125, -0.0078125));
  highp vec4 packDist_32;
  lowp vec4 tmpvar_33;
  tmpvar_33 = textureCube (_ShadowMapTexture, vec_31);
  packDist_32 = tmpvar_33;
  shadowVals_21.w = dot (packDist_32, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  bvec4 tmpvar_34;
  tmpvar_34 = lessThan (shadowVals_21, vec4(tmpvar_20));
  highp vec4 tmpvar_35;
  tmpvar_35 = _LightShadowData.xxxx;
  highp float tmpvar_36;
  if (tmpvar_34.x) {
    tmpvar_36 = tmpvar_35.x;
  } else {
    tmpvar_36 = 1.0;
  };
  highp float tmpvar_37;
  if (tmpvar_34.y) {
    tmpvar_37 = tmpvar_35.y;
  } else {
    tmpvar_37 = 1.0;
  };
  highp float tmpvar_38;
  if (tmpvar_34.z) {
    tmpvar_38 = tmpvar_35.z;
  } else {
    tmpvar_38 = 1.0;
  };
  highp float tmpvar_39;
  if (tmpvar_34.w) {
    tmpvar_39 = tmpvar_35.w;
  } else {
    tmpvar_39 = 1.0;
  };
  mediump vec4 tmpvar_40;
  tmpvar_40.x = tmpvar_36;
  tmpvar_40.y = tmpvar_37;
  tmpvar_40.z = tmpvar_38;
  tmpvar_40.w = tmpvar_39;
  mediump float tmpvar_41;
  tmpvar_41 = dot (tmpvar_40, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_42;
  tmpvar_42 = (atten_5 * tmpvar_41);
  atten_5 = tmpvar_42;
  mediump float tmpvar_43;
  tmpvar_43 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_44;
  tmpvar_44 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_44;
  mediump float tmpvar_45;
  tmpvar_45 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_45;
  highp float tmpvar_46;
  tmpvar_46 = (spec_3 * clamp (tmpvar_42, 0.0, 1.0));
  spec_3 = tmpvar_46;
  highp vec3 tmpvar_47;
  tmpvar_47 = (_LightColor.xyz * (tmpvar_43 * tmpvar_42));
  res_2.xyz = tmpvar_47;
  lowp vec3 c_48;
  c_48 = _LightColor.xyz;
  lowp float tmpvar_49;
  tmpvar_49 = dot (c_48, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_50;
  tmpvar_50 = (tmpvar_46 * tmpvar_49);
  res_2.w = tmpvar_50;
  highp float tmpvar_51;
  tmpvar_51 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_52;
  tmpvar_52 = (res_2 * tmpvar_51);
  res_2 = tmpvar_52;
  mediump vec4 tmpvar_53;
  tmpvar_53 = exp2(-(tmpvar_52));
  tmpvar_1 = tmpvar_53;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  highp vec4 shadowVals_21;
  highp vec3 vec_22;
  vec_22 = (tmpvar_16 + vec3(0.0078125, 0.0078125, 0.0078125));
  highp vec4 packDist_23;
  lowp vec4 tmpvar_24;
  tmpvar_24 = texture (_ShadowMapTexture, vec_22);
  packDist_23 = tmpvar_24;
  shadowVals_21.x = dot (packDist_23, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_25;
  vec_25 = (tmpvar_16 + vec3(-0.0078125, -0.0078125, 0.0078125));
  highp vec4 packDist_26;
  lowp vec4 tmpvar_27;
  tmpvar_27 = texture (_ShadowMapTexture, vec_25);
  packDist_26 = tmpvar_27;
  shadowVals_21.y = dot (packDist_26, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_28;
  vec_28 = (tmpvar_16 + vec3(-0.0078125, 0.0078125, -0.0078125));
  highp vec4 packDist_29;
  lowp vec4 tmpvar_30;
  tmpvar_30 = texture (_ShadowMapTexture, vec_28);
  packDist_29 = tmpvar_30;
  shadowVals_21.z = dot (packDist_29, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_31;
  vec_31 = (tmpvar_16 + vec3(0.0078125, -0.0078125, -0.0078125));
  highp vec4 packDist_32;
  lowp vec4 tmpvar_33;
  tmpvar_33 = texture (_ShadowMapTexture, vec_31);
  packDist_32 = tmpvar_33;
  shadowVals_21.w = dot (packDist_32, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  bvec4 tmpvar_34;
  tmpvar_34 = lessThan (shadowVals_21, vec4(tmpvar_20));
  highp vec4 tmpvar_35;
  tmpvar_35 = _LightShadowData.xxxx;
  highp float tmpvar_36;
  if (tmpvar_34.x) {
    tmpvar_36 = tmpvar_35.x;
  } else {
    tmpvar_36 = 1.0;
  };
  highp float tmpvar_37;
  if (tmpvar_34.y) {
    tmpvar_37 = tmpvar_35.y;
  } else {
    tmpvar_37 = 1.0;
  };
  highp float tmpvar_38;
  if (tmpvar_34.z) {
    tmpvar_38 = tmpvar_35.z;
  } else {
    tmpvar_38 = 1.0;
  };
  highp float tmpvar_39;
  if (tmpvar_34.w) {
    tmpvar_39 = tmpvar_35.w;
  } else {
    tmpvar_39 = 1.0;
  };
  mediump vec4 tmpvar_40;
  tmpvar_40.x = tmpvar_36;
  tmpvar_40.y = tmpvar_37;
  tmpvar_40.z = tmpvar_38;
  tmpvar_40.w = tmpvar_39;
  mediump float tmpvar_41;
  tmpvar_41 = dot (tmpvar_40, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_42;
  tmpvar_42 = (atten_5 * tmpvar_41);
  atten_5 = tmpvar_42;
  mediump float tmpvar_43;
  tmpvar_43 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_44;
  tmpvar_44 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_44;
  mediump float tmpvar_45;
  tmpvar_45 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_45;
  highp float tmpvar_46;
  tmpvar_46 = (spec_3 * clamp (tmpvar_42, 0.0, 1.0));
  spec_3 = tmpvar_46;
  highp vec3 tmpvar_47;
  tmpvar_47 = (_LightColor.xyz * (tmpvar_43 * tmpvar_42));
  res_2.xyz = tmpvar_47;
  lowp vec3 c_48;
  c_48 = _LightColor.xyz;
  lowp float tmpvar_49;
  tmpvar_49 = dot (c_48, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_50;
  tmpvar_50 = (tmpvar_46 * tmpvar_49);
  res_2.w = tmpvar_50;
  highp float tmpvar_51;
  tmpvar_51 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_52;
  tmpvar_52 = (res_2 * tmpvar_51);
  res_2 = tmpvar_52;
  mediump vec4 tmpvar_53;
  tmpvar_53 = exp2(-(tmpvar_52));
  tmpvar_1 = tmpvar_53;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _LightTexture0;
uniform lowp samplerCube _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture2D (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  highp vec4 shadowVals_21;
  highp vec3 vec_22;
  vec_22 = (tmpvar_16 + vec3(0.0078125, 0.0078125, 0.0078125));
  highp vec4 packDist_23;
  lowp vec4 tmpvar_24;
  tmpvar_24 = textureCube (_ShadowMapTexture, vec_22);
  packDist_23 = tmpvar_24;
  shadowVals_21.x = dot (packDist_23, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_25;
  vec_25 = (tmpvar_16 + vec3(-0.0078125, -0.0078125, 0.0078125));
  highp vec4 packDist_26;
  lowp vec4 tmpvar_27;
  tmpvar_27 = textureCube (_ShadowMapTexture, vec_25);
  packDist_26 = tmpvar_27;
  shadowVals_21.y = dot (packDist_26, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_28;
  vec_28 = (tmpvar_16 + vec3(-0.0078125, 0.0078125, -0.0078125));
  highp vec4 packDist_29;
  lowp vec4 tmpvar_30;
  tmpvar_30 = textureCube (_ShadowMapTexture, vec_28);
  packDist_29 = tmpvar_30;
  shadowVals_21.z = dot (packDist_29, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_31;
  vec_31 = (tmpvar_16 + vec3(0.0078125, -0.0078125, -0.0078125));
  highp vec4 packDist_32;
  lowp vec4 tmpvar_33;
  tmpvar_33 = textureCube (_ShadowMapTexture, vec_31);
  packDist_32 = tmpvar_33;
  shadowVals_21.w = dot (packDist_32, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  bvec4 tmpvar_34;
  tmpvar_34 = lessThan (shadowVals_21, vec4(tmpvar_20));
  highp vec4 tmpvar_35;
  tmpvar_35 = _LightShadowData.xxxx;
  highp float tmpvar_36;
  if (tmpvar_34.x) {
    tmpvar_36 = tmpvar_35.x;
  } else {
    tmpvar_36 = 1.0;
  };
  highp float tmpvar_37;
  if (tmpvar_34.y) {
    tmpvar_37 = tmpvar_35.y;
  } else {
    tmpvar_37 = 1.0;
  };
  highp float tmpvar_38;
  if (tmpvar_34.z) {
    tmpvar_38 = tmpvar_35.z;
  } else {
    tmpvar_38 = 1.0;
  };
  highp float tmpvar_39;
  if (tmpvar_34.w) {
    tmpvar_39 = tmpvar_35.w;
  } else {
    tmpvar_39 = 1.0;
  };
  mediump vec4 tmpvar_40;
  tmpvar_40.x = tmpvar_36;
  tmpvar_40.y = tmpvar_37;
  tmpvar_40.z = tmpvar_38;
  tmpvar_40.w = tmpvar_39;
  mediump float tmpvar_41;
  tmpvar_41 = dot (tmpvar_40, vec4(0.25, 0.25, 0.25, 0.25));
  highp vec4 tmpvar_42;
  tmpvar_42.w = 1.0;
  tmpvar_42.xyz = tmpvar_13;
  lowp vec4 tmpvar_43;
  highp vec3 P_44;
  P_44 = (_LightMatrix0 * tmpvar_42).xyz;
  tmpvar_43 = textureCube (_LightTexture0, P_44);
  highp float tmpvar_45;
  tmpvar_45 = ((atten_5 * tmpvar_41) * tmpvar_43.w);
  atten_5 = tmpvar_45;
  mediump float tmpvar_46;
  tmpvar_46 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_47;
  tmpvar_47 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_47;
  mediump float tmpvar_48;
  tmpvar_48 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_48;
  highp float tmpvar_49;
  tmpvar_49 = (spec_3 * clamp (tmpvar_45, 0.0, 1.0));
  spec_3 = tmpvar_49;
  highp vec3 tmpvar_50;
  tmpvar_50 = (_LightColor.xyz * (tmpvar_46 * tmpvar_45));
  res_2.xyz = tmpvar_50;
  lowp vec3 c_51;
  c_51 = _LightColor.xyz;
  lowp float tmpvar_52;
  tmpvar_52 = dot (c_51, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_53;
  tmpvar_53 = (tmpvar_49 * tmpvar_52);
  res_2.w = tmpvar_53;
  highp float tmpvar_54;
  tmpvar_54 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_55;
  tmpvar_55 = (res_2 * tmpvar_54);
  res_2 = tmpvar_55;
  mediump vec4 tmpvar_56;
  tmpvar_56 = exp2(-(tmpvar_55));
  tmpvar_1 = tmpvar_56;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _LightTexture0;
uniform lowp samplerCube _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  highp vec4 shadowVals_21;
  highp vec3 vec_22;
  vec_22 = (tmpvar_16 + vec3(0.0078125, 0.0078125, 0.0078125));
  highp vec4 packDist_23;
  lowp vec4 tmpvar_24;
  tmpvar_24 = texture (_ShadowMapTexture, vec_22);
  packDist_23 = tmpvar_24;
  shadowVals_21.x = dot (packDist_23, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_25;
  vec_25 = (tmpvar_16 + vec3(-0.0078125, -0.0078125, 0.0078125));
  highp vec4 packDist_26;
  lowp vec4 tmpvar_27;
  tmpvar_27 = texture (_ShadowMapTexture, vec_25);
  packDist_26 = tmpvar_27;
  shadowVals_21.y = dot (packDist_26, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_28;
  vec_28 = (tmpvar_16 + vec3(-0.0078125, 0.0078125, -0.0078125));
  highp vec4 packDist_29;
  lowp vec4 tmpvar_30;
  tmpvar_30 = texture (_ShadowMapTexture, vec_28);
  packDist_29 = tmpvar_30;
  shadowVals_21.z = dot (packDist_29, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_31;
  vec_31 = (tmpvar_16 + vec3(0.0078125, -0.0078125, -0.0078125));
  highp vec4 packDist_32;
  lowp vec4 tmpvar_33;
  tmpvar_33 = texture (_ShadowMapTexture, vec_31);
  packDist_32 = tmpvar_33;
  shadowVals_21.w = dot (packDist_32, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  bvec4 tmpvar_34;
  tmpvar_34 = lessThan (shadowVals_21, vec4(tmpvar_20));
  highp vec4 tmpvar_35;
  tmpvar_35 = _LightShadowData.xxxx;
  highp float tmpvar_36;
  if (tmpvar_34.x) {
    tmpvar_36 = tmpvar_35.x;
  } else {
    tmpvar_36 = 1.0;
  };
  highp float tmpvar_37;
  if (tmpvar_34.y) {
    tmpvar_37 = tmpvar_35.y;
  } else {
    tmpvar_37 = 1.0;
  };
  highp float tmpvar_38;
  if (tmpvar_34.z) {
    tmpvar_38 = tmpvar_35.z;
  } else {
    tmpvar_38 = 1.0;
  };
  highp float tmpvar_39;
  if (tmpvar_34.w) {
    tmpvar_39 = tmpvar_35.w;
  } else {
    tmpvar_39 = 1.0;
  };
  mediump vec4 tmpvar_40;
  tmpvar_40.x = tmpvar_36;
  tmpvar_40.y = tmpvar_37;
  tmpvar_40.z = tmpvar_38;
  tmpvar_40.w = tmpvar_39;
  mediump float tmpvar_41;
  tmpvar_41 = dot (tmpvar_40, vec4(0.25, 0.25, 0.25, 0.25));
  highp vec4 tmpvar_42;
  tmpvar_42.w = 1.0;
  tmpvar_42.xyz = tmpvar_13;
  lowp vec4 tmpvar_43;
  highp vec3 P_44;
  P_44 = (_LightMatrix0 * tmpvar_42).xyz;
  tmpvar_43 = texture (_LightTexture0, P_44);
  highp float tmpvar_45;
  tmpvar_45 = ((atten_5 * tmpvar_41) * tmpvar_43.w);
  atten_5 = tmpvar_45;
  mediump float tmpvar_46;
  tmpvar_46 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_47;
  tmpvar_47 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_47;
  mediump float tmpvar_48;
  tmpvar_48 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_48;
  highp float tmpvar_49;
  tmpvar_49 = (spec_3 * clamp (tmpvar_45, 0.0, 1.0));
  spec_3 = tmpvar_49;
  highp vec3 tmpvar_50;
  tmpvar_50 = (_LightColor.xyz * (tmpvar_46 * tmpvar_45));
  res_2.xyz = tmpvar_50;
  lowp vec3 c_51;
  c_51 = _LightColor.xyz;
  lowp float tmpvar_52;
  tmpvar_52 = dot (c_51, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_53;
  tmpvar_53 = (tmpvar_49 * tmpvar_52);
  res_2.w = tmpvar_53;
  highp float tmpvar_54;
  tmpvar_54 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_55;
  tmpvar_55 = (res_2 * tmpvar_54);
  res_2 = tmpvar_55;
  mediump vec4 tmpvar_56;
  tmpvar_56 = exp2(-(tmpvar_55));
  tmpvar_1 = tmpvar_56;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec3 normal_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  normal_6 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_14;
  tmpvar_14 = mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_15;
  tmpvar_15 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_15;
  mediump float tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture2D (_ShadowMapTexture, tmpvar_8);
  highp float tmpvar_18;
  tmpvar_18 = clamp ((tmpvar_17.x + clamp (
    ((tmpvar_14 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_16 = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = tmpvar_16;
  mediump float tmpvar_20;
  tmpvar_20 = max (0.0, dot (lightDir_5, normal_6));
  highp vec3 tmpvar_21;
  tmpvar_21 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_21;
  mediump float tmpvar_22;
  tmpvar_22 = pow (max (0.0, dot (h_4, normal_6)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = (spec_3 * clamp (tmpvar_19, 0.0, 1.0));
  spec_3 = tmpvar_23;
  highp vec3 tmpvar_24;
  tmpvar_24 = (_LightColor.xyz * (tmpvar_20 * tmpvar_19));
  res_2.xyz = tmpvar_24;
  lowp vec3 c_25;
  c_25 = _LightColor.xyz;
  lowp float tmpvar_26;
  tmpvar_26 = dot (c_25, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_27;
  tmpvar_27 = (tmpvar_23 * tmpvar_26);
  res_2.w = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = clamp ((1.0 - (
    (tmpvar_14 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_29;
  tmpvar_29 = (res_2 * tmpvar_28);
  res_2 = tmpvar_29;
  mediump vec4 tmpvar_30;
  tmpvar_30 = exp2(-(tmpvar_29));
  tmpvar_1 = tmpvar_30;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec3 normal_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  normal_6 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_14;
  tmpvar_14 = mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_15;
  tmpvar_15 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_15;
  mediump float tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture (_ShadowMapTexture, tmpvar_8);
  highp float tmpvar_18;
  tmpvar_18 = clamp ((tmpvar_17.x + clamp (
    ((tmpvar_14 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_16 = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = tmpvar_16;
  mediump float tmpvar_20;
  tmpvar_20 = max (0.0, dot (lightDir_5, normal_6));
  highp vec3 tmpvar_21;
  tmpvar_21 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_21;
  mediump float tmpvar_22;
  tmpvar_22 = pow (max (0.0, dot (h_4, normal_6)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = (spec_3 * clamp (tmpvar_19, 0.0, 1.0));
  spec_3 = tmpvar_23;
  highp vec3 tmpvar_24;
  tmpvar_24 = (_LightColor.xyz * (tmpvar_20 * tmpvar_19));
  res_2.xyz = tmpvar_24;
  lowp vec3 c_25;
  c_25 = _LightColor.xyz;
  lowp float tmpvar_26;
  tmpvar_26 = dot (c_25, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_27;
  tmpvar_27 = (tmpvar_23 * tmpvar_26);
  res_2.w = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = clamp ((1.0 - (
    (tmpvar_14 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_29;
  tmpvar_29 = (res_2 * tmpvar_28);
  res_2 = tmpvar_29;
  mediump vec4 tmpvar_30;
  tmpvar_30 = exp2(-(tmpvar_29));
  tmpvar_1 = tmpvar_30;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTexture0;
uniform sampler2D _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec3 normal_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  normal_6 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_14;
  tmpvar_14 = mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_15;
  tmpvar_15 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_15;
  mediump float tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture2D (_ShadowMapTexture, tmpvar_8);
  highp float tmpvar_18;
  tmpvar_18 = clamp ((tmpvar_17.x + clamp (
    ((tmpvar_14 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_16 = tmpvar_18;
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = tmpvar_12;
  lowp vec4 tmpvar_20;
  highp vec2 P_21;
  P_21 = (_LightMatrix0 * tmpvar_19).xy;
  tmpvar_20 = texture2D (_LightTexture0, P_21);
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_16 * tmpvar_20.w);
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_5, normal_6));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, normal_6)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (tmpvar_14 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  mediump vec4 tmpvar_33;
  tmpvar_33 = exp2(-(tmpvar_32));
  tmpvar_1 = tmpvar_33;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTexture0;
uniform sampler2D _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec3 normal_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  normal_6 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_14;
  tmpvar_14 = mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_15;
  tmpvar_15 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_15;
  mediump float tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture (_ShadowMapTexture, tmpvar_8);
  highp float tmpvar_18;
  tmpvar_18 = clamp ((tmpvar_17.x + clamp (
    ((tmpvar_14 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_16 = tmpvar_18;
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = tmpvar_12;
  lowp vec4 tmpvar_20;
  highp vec2 P_21;
  P_21 = (_LightMatrix0 * tmpvar_19).xy;
  tmpvar_20 = texture (_LightTexture0, P_21);
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_16 * tmpvar_20.w);
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_5, normal_6));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, normal_6)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (tmpvar_14 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  mediump vec4 tmpvar_33;
  tmpvar_33 = exp2(-(tmpvar_32));
  tmpvar_1 = tmpvar_33;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT" "SHADOWS_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "SPOT" "SHADOWS_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" "SHADOWS_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_NONATIVE" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_NATIVE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_NATIVE" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT" "SHADOWS_CUBE" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SHADOWS_NONATIVE" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SHADOWS_NATIVE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SHADOWS_NATIVE" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES3"
}
}
 }
 Pass {
  Tags { "ShadowSupport"="True" }
  ZWrite Off
  Fog { Mode Off }
  Blend One One
Program "vp" {
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _LightTextureB0;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_16;
  tmpvar_16 = -(normalize(tmpvar_15));
  lightDir_6 = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = (dot (tmpvar_15, tmpvar_15) * _LightPos.w);
  lowp float tmpvar_18;
  tmpvar_18 = texture2D (_LightTextureB0, vec2(tmpvar_17)).w;
  atten_5 = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = max (0.0, dot (lightDir_6, tmpvar_10));
  highp vec3 tmpvar_20;
  tmpvar_20 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = pow (max (0.0, dot (h_4, tmpvar_10)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = (spec_3 * clamp (atten_5, 0.0, 1.0));
  spec_3 = tmpvar_22;
  highp vec3 tmpvar_23;
  tmpvar_23 = (_LightColor.xyz * (tmpvar_19 * atten_5));
  res_2.xyz = tmpvar_23;
  lowp vec3 c_24;
  c_24 = _LightColor.xyz;
  lowp float tmpvar_25;
  tmpvar_25 = dot (c_24, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_26;
  tmpvar_26 = (tmpvar_22 * tmpvar_25);
  res_2.w = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = clamp ((1.0 - (
    (mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_28;
  tmpvar_28 = (res_2 * tmpvar_27);
  res_2 = tmpvar_28;
  tmpvar_1 = tmpvar_28;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT" "SHADOWS_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _LightTextureB0;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_16;
  tmpvar_16 = -(normalize(tmpvar_15));
  lightDir_6 = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = (dot (tmpvar_15, tmpvar_15) * _LightPos.w);
  lowp float tmpvar_18;
  tmpvar_18 = texture (_LightTextureB0, vec2(tmpvar_17)).w;
  atten_5 = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = max (0.0, dot (lightDir_6, tmpvar_10));
  highp vec3 tmpvar_20;
  tmpvar_20 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = pow (max (0.0, dot (h_4, tmpvar_10)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = (spec_3 * clamp (atten_5, 0.0, 1.0));
  spec_3 = tmpvar_22;
  highp vec3 tmpvar_23;
  tmpvar_23 = (_LightColor.xyz * (tmpvar_19 * atten_5));
  res_2.xyz = tmpvar_23;
  lowp vec3 c_24;
  c_24 = _LightColor.xyz;
  lowp float tmpvar_25;
  tmpvar_25 = dot (c_24, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_26;
  tmpvar_26 = (tmpvar_22 * tmpvar_25);
  res_2.w = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = clamp ((1.0 - (
    (mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_28;
  tmpvar_28 = (res_2 * tmpvar_27);
  res_2 = tmpvar_28;
  tmpvar_1 = tmpvar_28;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec4 nspec_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_CameraNormalsTexture, tmpvar_7);
  nspec_6 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = normalize(((nspec_6.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraDepthTexture, tmpvar_7);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_14;
  tmpvar_14 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_14;
  mediump float tmpvar_15;
  tmpvar_15 = max (0.0, dot (lightDir_5, tmpvar_9));
  highp vec3 tmpvar_16;
  tmpvar_16 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_16;
  mediump float tmpvar_17;
  tmpvar_17 = pow (max (0.0, dot (h_4, tmpvar_9)), (nspec_6.w * 128.0));
  spec_3 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (spec_3 * clamp (1.0, 0.0, 1.0));
  spec_3 = tmpvar_18;
  highp vec3 tmpvar_19;
  tmpvar_19 = (_LightColor.xyz * tmpvar_15);
  res_2.xyz = tmpvar_19;
  lowp vec3 c_20;
  c_20 = _LightColor.xyz;
  lowp float tmpvar_21;
  tmpvar_21 = dot (c_20, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_18 * tmpvar_21);
  res_2.w = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = clamp ((1.0 - (
    (mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_24;
  tmpvar_24 = (res_2 * tmpvar_23);
  res_2 = tmpvar_24;
  tmpvar_1 = tmpvar_24;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec4 nspec_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture (_CameraNormalsTexture, tmpvar_7);
  nspec_6 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = normalize(((nspec_6.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraDepthTexture, tmpvar_7);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_14;
  tmpvar_14 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_14;
  mediump float tmpvar_15;
  tmpvar_15 = max (0.0, dot (lightDir_5, tmpvar_9));
  highp vec3 tmpvar_16;
  tmpvar_16 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_16;
  mediump float tmpvar_17;
  tmpvar_17 = pow (max (0.0, dot (h_4, tmpvar_9)), (nspec_6.w * 128.0));
  spec_3 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (spec_3 * clamp (1.0, 0.0, 1.0));
  spec_3 = tmpvar_18;
  highp vec3 tmpvar_19;
  tmpvar_19 = (_LightColor.xyz * tmpvar_15);
  res_2.xyz = tmpvar_19;
  lowp vec3 c_20;
  c_20 = _LightColor.xyz;
  lowp float tmpvar_21;
  tmpvar_21 = dot (c_20, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_18 * tmpvar_21);
  res_2.w = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = clamp ((1.0 - (
    (mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_24;
  tmpvar_24 = (res_2 * tmpvar_23);
  res_2 = tmpvar_24;
  tmpvar_1 = tmpvar_24;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_15;
  tmpvar_15 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_16;
  tmpvar_16 = normalize(tmpvar_15);
  lightDir_6 = tmpvar_16;
  highp vec4 tmpvar_17;
  tmpvar_17.w = 1.0;
  tmpvar_17.xyz = tmpvar_13;
  highp vec4 tmpvar_18;
  tmpvar_18 = (_LightMatrix0 * tmpvar_17);
  lowp float tmpvar_19;
  tmpvar_19 = texture2DProj (_LightTexture0, tmpvar_18).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = (dot (tmpvar_15, tmpvar_15) * _LightPos.w);
  lowp vec4 tmpvar_21;
  tmpvar_21 = texture2D (_LightTextureB0, vec2(tmpvar_20));
  highp float tmpvar_22;
  tmpvar_22 = ((atten_5 * float(
    (tmpvar_18.w < 0.0)
  )) * tmpvar_21.w);
  atten_5 = tmpvar_22;
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_6, tmpvar_10));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, tmpvar_10)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  tmpvar_1 = tmpvar_32;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "SPOT" "SHADOWS_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_15;
  tmpvar_15 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_16;
  tmpvar_16 = normalize(tmpvar_15);
  lightDir_6 = tmpvar_16;
  highp vec4 tmpvar_17;
  tmpvar_17.w = 1.0;
  tmpvar_17.xyz = tmpvar_13;
  highp vec4 tmpvar_18;
  tmpvar_18 = (_LightMatrix0 * tmpvar_17);
  lowp float tmpvar_19;
  tmpvar_19 = textureProj (_LightTexture0, tmpvar_18).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = (dot (tmpvar_15, tmpvar_15) * _LightPos.w);
  lowp vec4 tmpvar_21;
  tmpvar_21 = texture (_LightTextureB0, vec2(tmpvar_20));
  highp float tmpvar_22;
  tmpvar_22 = ((atten_5 * float(
    (tmpvar_18.w < 0.0)
  )) * tmpvar_21.w);
  atten_5 = tmpvar_22;
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_6, tmpvar_10));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, tmpvar_10)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  tmpvar_1 = tmpvar_32;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _LightTexture0;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_16;
  tmpvar_16 = -(normalize(tmpvar_15));
  lightDir_6 = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = (dot (tmpvar_15, tmpvar_15) * _LightPos.w);
  lowp float tmpvar_18;
  tmpvar_18 = texture2D (_LightTextureB0, vec2(tmpvar_17)).w;
  atten_5 = tmpvar_18;
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = tmpvar_13;
  lowp vec4 tmpvar_20;
  highp vec3 P_21;
  P_21 = (_LightMatrix0 * tmpvar_19).xyz;
  tmpvar_20 = textureCube (_LightTexture0, P_21);
  highp float tmpvar_22;
  tmpvar_22 = (atten_5 * tmpvar_20.w);
  atten_5 = tmpvar_22;
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_6, tmpvar_10));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, tmpvar_10)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  tmpvar_1 = tmpvar_32;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" "SHADOWS_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _LightTexture0;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_16;
  tmpvar_16 = -(normalize(tmpvar_15));
  lightDir_6 = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = (dot (tmpvar_15, tmpvar_15) * _LightPos.w);
  lowp float tmpvar_18;
  tmpvar_18 = texture (_LightTextureB0, vec2(tmpvar_17)).w;
  atten_5 = tmpvar_18;
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = tmpvar_13;
  lowp vec4 tmpvar_20;
  highp vec3 P_21;
  P_21 = (_LightMatrix0 * tmpvar_19).xyz;
  tmpvar_20 = texture (_LightTexture0, P_21);
  highp float tmpvar_22;
  tmpvar_22 = (atten_5 * tmpvar_20.w);
  atten_5 = tmpvar_22;
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_6, tmpvar_10));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, tmpvar_10)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  tmpvar_1 = tmpvar_32;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTexture0;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec4 nspec_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_CameraNormalsTexture, tmpvar_7);
  nspec_6 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = normalize(((nspec_6.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraDepthTexture, tmpvar_7);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_14;
  tmpvar_14 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_14;
  highp vec4 tmpvar_15;
  tmpvar_15.w = 1.0;
  tmpvar_15.xyz = tmpvar_12;
  lowp vec4 tmpvar_16;
  highp vec2 P_17;
  P_17 = (_LightMatrix0 * tmpvar_15).xy;
  tmpvar_16 = texture2D (_LightTexture0, P_17);
  highp float tmpvar_18;
  tmpvar_18 = tmpvar_16.w;
  mediump float tmpvar_19;
  tmpvar_19 = max (0.0, dot (lightDir_5, tmpvar_9));
  highp vec3 tmpvar_20;
  tmpvar_20 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = pow (max (0.0, dot (h_4, tmpvar_9)), (nspec_6.w * 128.0));
  spec_3 = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = (spec_3 * clamp (tmpvar_18, 0.0, 1.0));
  spec_3 = tmpvar_22;
  highp vec3 tmpvar_23;
  tmpvar_23 = (_LightColor.xyz * (tmpvar_19 * tmpvar_18));
  res_2.xyz = tmpvar_23;
  lowp vec3 c_24;
  c_24 = _LightColor.xyz;
  lowp float tmpvar_25;
  tmpvar_25 = dot (c_24, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_26;
  tmpvar_26 = (tmpvar_22 * tmpvar_25);
  res_2.w = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = clamp ((1.0 - (
    (mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_28;
  tmpvar_28 = (res_2 * tmpvar_27);
  res_2 = tmpvar_28;
  tmpvar_1 = tmpvar_28;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTexture0;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec4 nspec_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture (_CameraNormalsTexture, tmpvar_7);
  nspec_6 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = normalize(((nspec_6.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraDepthTexture, tmpvar_7);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_14;
  tmpvar_14 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_14;
  highp vec4 tmpvar_15;
  tmpvar_15.w = 1.0;
  tmpvar_15.xyz = tmpvar_12;
  lowp vec4 tmpvar_16;
  highp vec2 P_17;
  P_17 = (_LightMatrix0 * tmpvar_15).xy;
  tmpvar_16 = texture (_LightTexture0, P_17);
  highp float tmpvar_18;
  tmpvar_18 = tmpvar_16.w;
  mediump float tmpvar_19;
  tmpvar_19 = max (0.0, dot (lightDir_5, tmpvar_9));
  highp vec3 tmpvar_20;
  tmpvar_20 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = pow (max (0.0, dot (h_4, tmpvar_9)), (nspec_6.w * 128.0));
  spec_3 = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = (spec_3 * clamp (tmpvar_18, 0.0, 1.0));
  spec_3 = tmpvar_22;
  highp vec3 tmpvar_23;
  tmpvar_23 = (_LightColor.xyz * (tmpvar_19 * tmpvar_18));
  res_2.xyz = tmpvar_23;
  lowp vec3 c_24;
  c_24 = _LightColor.xyz;
  lowp float tmpvar_25;
  tmpvar_25 = dot (c_24, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_26;
  tmpvar_26 = (tmpvar_22 * tmpvar_25);
  res_2.w = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = clamp ((1.0 - (
    (mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_28;
  tmpvar_28 = (res_2 * tmpvar_27);
  res_2 = tmpvar_28;
  tmpvar_1 = tmpvar_28;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_NONATIVE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform sampler2D _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize(tmpvar_16);
  lightDir_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_13;
  highp vec4 tmpvar_19;
  tmpvar_19 = (_LightMatrix0 * tmpvar_18);
  lowp float tmpvar_20;
  tmpvar_20 = texture2DProj (_LightTexture0, tmpvar_19).w;
  atten_5 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture2D (_LightTextureB0, vec2(tmpvar_21));
  atten_5 = ((atten_5 * float(
    (tmpvar_19.w < 0.0)
  )) * tmpvar_22.w);
  mediump float tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = clamp (((tmpvar_15 * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0);
  highp vec4 tmpvar_25;
  tmpvar_25.w = 1.0;
  tmpvar_25.xyz = tmpvar_13;
  highp vec4 tmpvar_26;
  tmpvar_26 = (unity_World2Shadow[0] * tmpvar_25);
  mediump float shadow_27;
  lowp vec4 tmpvar_28;
  tmpvar_28 = texture2DProj (_ShadowMapTexture, tmpvar_26);
  highp float tmpvar_29;
  if ((tmpvar_28.x < (tmpvar_26.z / tmpvar_26.w))) {
    tmpvar_29 = _LightShadowData.x;
  } else {
    tmpvar_29 = 1.0;
  };
  shadow_27 = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = clamp ((shadow_27 + tmpvar_24), 0.0, 1.0);
  tmpvar_23 = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = (atten_5 * tmpvar_23);
  atten_5 = tmpvar_31;
  mediump float tmpvar_32;
  tmpvar_32 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_33;
  tmpvar_33 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_33;
  mediump float tmpvar_34;
  tmpvar_34 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_34;
  highp float tmpvar_35;
  tmpvar_35 = (spec_3 * clamp (tmpvar_31, 0.0, 1.0));
  spec_3 = tmpvar_35;
  highp vec3 tmpvar_36;
  tmpvar_36 = (_LightColor.xyz * (tmpvar_32 * tmpvar_31));
  res_2.xyz = tmpvar_36;
  lowp vec3 c_37;
  c_37 = _LightColor.xyz;
  lowp float tmpvar_38;
  tmpvar_38 = dot (c_37, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_39;
  tmpvar_39 = (tmpvar_35 * tmpvar_38);
  res_2.w = tmpvar_39;
  highp float tmpvar_40;
  tmpvar_40 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_41;
  tmpvar_41 = (res_2 * tmpvar_40);
  res_2 = tmpvar_41;
  tmpvar_1 = tmpvar_41;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_NATIVE" }
"!!GLES


#ifdef VERTEX

#extension GL_EXT_shadow_samplers : enable
attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

#extension GL_EXT_shadow_samplers : enable
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp sampler2DShadow _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize(tmpvar_16);
  lightDir_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_13;
  highp vec4 tmpvar_19;
  tmpvar_19 = (_LightMatrix0 * tmpvar_18);
  lowp float tmpvar_20;
  tmpvar_20 = texture2DProj (_LightTexture0, tmpvar_19).w;
  atten_5 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture2D (_LightTextureB0, vec2(tmpvar_21));
  atten_5 = ((atten_5 * float(
    (tmpvar_19.w < 0.0)
  )) * tmpvar_22.w);
  mediump float tmpvar_23;
  highp vec4 tmpvar_24;
  tmpvar_24.w = 1.0;
  tmpvar_24.xyz = tmpvar_13;
  highp vec4 tmpvar_25;
  tmpvar_25 = (unity_World2Shadow[0] * tmpvar_24);
  mediump float shadow_26;
  lowp float tmpvar_27;
  tmpvar_27 = shadow2DProjEXT (_ShadowMapTexture, tmpvar_25);
  mediump float tmpvar_28;
  tmpvar_28 = tmpvar_27;
  highp float tmpvar_29;
  tmpvar_29 = (_LightShadowData.x + (tmpvar_28 * (1.0 - _LightShadowData.x)));
  shadow_26 = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = clamp ((shadow_26 + clamp (
    ((tmpvar_15 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_23 = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = (atten_5 * tmpvar_23);
  atten_5 = tmpvar_31;
  mediump float tmpvar_32;
  tmpvar_32 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_33;
  tmpvar_33 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_33;
  mediump float tmpvar_34;
  tmpvar_34 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_34;
  highp float tmpvar_35;
  tmpvar_35 = (spec_3 * clamp (tmpvar_31, 0.0, 1.0));
  spec_3 = tmpvar_35;
  highp vec3 tmpvar_36;
  tmpvar_36 = (_LightColor.xyz * (tmpvar_32 * tmpvar_31));
  res_2.xyz = tmpvar_36;
  lowp vec3 c_37;
  c_37 = _LightColor.xyz;
  lowp float tmpvar_38;
  tmpvar_38 = dot (c_37, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_39;
  tmpvar_39 = (tmpvar_35 * tmpvar_38);
  res_2.w = tmpvar_39;
  highp float tmpvar_40;
  tmpvar_40 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_41;
  tmpvar_41 = (res_2 * tmpvar_40);
  res_2 = tmpvar_41;
  tmpvar_1 = tmpvar_41;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_NATIVE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp sampler2DShadow _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize(tmpvar_16);
  lightDir_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_13;
  highp vec4 tmpvar_19;
  tmpvar_19 = (_LightMatrix0 * tmpvar_18);
  lowp float tmpvar_20;
  tmpvar_20 = textureProj (_LightTexture0, tmpvar_19).w;
  atten_5 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture (_LightTextureB0, vec2(tmpvar_21));
  atten_5 = ((atten_5 * float(
    (tmpvar_19.w < 0.0)
  )) * tmpvar_22.w);
  mediump float tmpvar_23;
  highp vec4 tmpvar_24;
  tmpvar_24.w = 1.0;
  tmpvar_24.xyz = tmpvar_13;
  highp vec4 tmpvar_25;
  tmpvar_25 = (unity_World2Shadow[0] * tmpvar_24);
  mediump float shadow_26;
  mediump float tmpvar_27;
  tmpvar_27 = textureProj (_ShadowMapTexture, tmpvar_25);
  highp float tmpvar_28;
  tmpvar_28 = (_LightShadowData.x + (tmpvar_27 * (1.0 - _LightShadowData.x)));
  shadow_26 = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = clamp ((shadow_26 + clamp (
    ((tmpvar_15 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_23 = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = (atten_5 * tmpvar_23);
  atten_5 = tmpvar_30;
  mediump float tmpvar_31;
  tmpvar_31 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_32;
  tmpvar_32 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_32;
  mediump float tmpvar_33;
  tmpvar_33 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_33;
  highp float tmpvar_34;
  tmpvar_34 = (spec_3 * clamp (tmpvar_30, 0.0, 1.0));
  spec_3 = tmpvar_34;
  highp vec3 tmpvar_35;
  tmpvar_35 = (_LightColor.xyz * (tmpvar_31 * tmpvar_30));
  res_2.xyz = tmpvar_35;
  lowp vec3 c_36;
  c_36 = _LightColor.xyz;
  lowp float tmpvar_37;
  tmpvar_37 = dot (c_36, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_38;
  tmpvar_38 = (tmpvar_34 * tmpvar_37);
  res_2.w = tmpvar_38;
  highp float tmpvar_39;
  tmpvar_39 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_40;
  tmpvar_40 = (res_2 * tmpvar_39);
  res_2 = tmpvar_40;
  tmpvar_1 = tmpvar_40;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec3 normal_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  normal_6 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_14;
  tmpvar_14 = mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_15;
  tmpvar_15 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_15;
  mediump float tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture2D (_ShadowMapTexture, tmpvar_8);
  highp float tmpvar_18;
  tmpvar_18 = clamp ((tmpvar_17.x + clamp (
    ((tmpvar_14 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_16 = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = tmpvar_16;
  mediump float tmpvar_20;
  tmpvar_20 = max (0.0, dot (lightDir_5, normal_6));
  highp vec3 tmpvar_21;
  tmpvar_21 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_21;
  mediump float tmpvar_22;
  tmpvar_22 = pow (max (0.0, dot (h_4, normal_6)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = (spec_3 * clamp (tmpvar_19, 0.0, 1.0));
  spec_3 = tmpvar_23;
  highp vec3 tmpvar_24;
  tmpvar_24 = (_LightColor.xyz * (tmpvar_20 * tmpvar_19));
  res_2.xyz = tmpvar_24;
  lowp vec3 c_25;
  c_25 = _LightColor.xyz;
  lowp float tmpvar_26;
  tmpvar_26 = dot (c_25, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_27;
  tmpvar_27 = (tmpvar_23 * tmpvar_26);
  res_2.w = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = clamp ((1.0 - (
    (tmpvar_14 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_29;
  tmpvar_29 = (res_2 * tmpvar_28);
  res_2 = tmpvar_29;
  tmpvar_1 = tmpvar_29;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTexture0;
uniform sampler2D _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec3 normal_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  normal_6 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_14;
  tmpvar_14 = mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_15;
  tmpvar_15 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_15;
  mediump float tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture2D (_ShadowMapTexture, tmpvar_8);
  highp float tmpvar_18;
  tmpvar_18 = clamp ((tmpvar_17.x + clamp (
    ((tmpvar_14 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_16 = tmpvar_18;
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = tmpvar_12;
  lowp vec4 tmpvar_20;
  highp vec2 P_21;
  P_21 = (_LightMatrix0 * tmpvar_19).xy;
  tmpvar_20 = texture2D (_LightTexture0, P_21);
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_16 * tmpvar_20.w);
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_5, normal_6));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, normal_6)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (tmpvar_14 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  tmpvar_1 = tmpvar_32;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture2D (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  mediump float tmpvar_21;
  highp vec4 packDist_22;
  lowp vec4 tmpvar_23;
  tmpvar_23 = textureCube (_ShadowMapTexture, tmpvar_16);
  packDist_22 = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (packDist_22, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp float tmpvar_25;
  if ((tmpvar_24 < tmpvar_20)) {
    tmpvar_25 = _LightShadowData.x;
  } else {
    tmpvar_25 = 1.0;
  };
  tmpvar_21 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (atten_5 * tmpvar_21);
  atten_5 = tmpvar_26;
  mediump float tmpvar_27;
  tmpvar_27 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_28;
  tmpvar_28 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_28;
  mediump float tmpvar_29;
  tmpvar_29 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = (spec_3 * clamp (tmpvar_26, 0.0, 1.0));
  spec_3 = tmpvar_30;
  highp vec3 tmpvar_31;
  tmpvar_31 = (_LightColor.xyz * (tmpvar_27 * tmpvar_26));
  res_2.xyz = tmpvar_31;
  lowp vec3 c_32;
  c_32 = _LightColor.xyz;
  lowp float tmpvar_33;
  tmpvar_33 = dot (c_32, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_34;
  tmpvar_34 = (tmpvar_30 * tmpvar_33);
  res_2.w = tmpvar_34;
  highp float tmpvar_35;
  tmpvar_35 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_36;
  tmpvar_36 = (res_2 * tmpvar_35);
  res_2 = tmpvar_36;
  tmpvar_1 = tmpvar_36;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT" "SHADOWS_CUBE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  mediump float tmpvar_21;
  highp vec4 packDist_22;
  lowp vec4 tmpvar_23;
  tmpvar_23 = texture (_ShadowMapTexture, tmpvar_16);
  packDist_22 = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (packDist_22, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp float tmpvar_25;
  if ((tmpvar_24 < tmpvar_20)) {
    tmpvar_25 = _LightShadowData.x;
  } else {
    tmpvar_25 = 1.0;
  };
  tmpvar_21 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (atten_5 * tmpvar_21);
  atten_5 = tmpvar_26;
  mediump float tmpvar_27;
  tmpvar_27 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_28;
  tmpvar_28 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_28;
  mediump float tmpvar_29;
  tmpvar_29 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = (spec_3 * clamp (tmpvar_26, 0.0, 1.0));
  spec_3 = tmpvar_30;
  highp vec3 tmpvar_31;
  tmpvar_31 = (_LightColor.xyz * (tmpvar_27 * tmpvar_26));
  res_2.xyz = tmpvar_31;
  lowp vec3 c_32;
  c_32 = _LightColor.xyz;
  lowp float tmpvar_33;
  tmpvar_33 = dot (c_32, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_34;
  tmpvar_34 = (tmpvar_30 * tmpvar_33);
  res_2.w = tmpvar_34;
  highp float tmpvar_35;
  tmpvar_35 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_36;
  tmpvar_36 = (res_2 * tmpvar_35);
  res_2 = tmpvar_36;
  tmpvar_1 = tmpvar_36;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _LightTexture0;
uniform lowp samplerCube _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture2D (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  mediump float tmpvar_21;
  highp vec4 packDist_22;
  lowp vec4 tmpvar_23;
  tmpvar_23 = textureCube (_ShadowMapTexture, tmpvar_16);
  packDist_22 = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (packDist_22, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp float tmpvar_25;
  if ((tmpvar_24 < tmpvar_20)) {
    tmpvar_25 = _LightShadowData.x;
  } else {
    tmpvar_25 = 1.0;
  };
  tmpvar_21 = tmpvar_25;
  highp vec4 tmpvar_26;
  tmpvar_26.w = 1.0;
  tmpvar_26.xyz = tmpvar_13;
  lowp vec4 tmpvar_27;
  highp vec3 P_28;
  P_28 = (_LightMatrix0 * tmpvar_26).xyz;
  tmpvar_27 = textureCube (_LightTexture0, P_28);
  highp float tmpvar_29;
  tmpvar_29 = ((atten_5 * tmpvar_21) * tmpvar_27.w);
  atten_5 = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_31;
  tmpvar_31 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_31;
  mediump float tmpvar_32;
  tmpvar_32 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_32;
  highp float tmpvar_33;
  tmpvar_33 = (spec_3 * clamp (tmpvar_29, 0.0, 1.0));
  spec_3 = tmpvar_33;
  highp vec3 tmpvar_34;
  tmpvar_34 = (_LightColor.xyz * (tmpvar_30 * tmpvar_29));
  res_2.xyz = tmpvar_34;
  lowp vec3 c_35;
  c_35 = _LightColor.xyz;
  lowp float tmpvar_36;
  tmpvar_36 = dot (c_35, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_37;
  tmpvar_37 = (tmpvar_33 * tmpvar_36);
  res_2.w = tmpvar_37;
  highp float tmpvar_38;
  tmpvar_38 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_39;
  tmpvar_39 = (res_2 * tmpvar_38);
  res_2 = tmpvar_39;
  tmpvar_1 = tmpvar_39;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _LightTexture0;
uniform lowp samplerCube _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  mediump float tmpvar_21;
  highp vec4 packDist_22;
  lowp vec4 tmpvar_23;
  tmpvar_23 = texture (_ShadowMapTexture, tmpvar_16);
  packDist_22 = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (packDist_22, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp float tmpvar_25;
  if ((tmpvar_24 < tmpvar_20)) {
    tmpvar_25 = _LightShadowData.x;
  } else {
    tmpvar_25 = 1.0;
  };
  tmpvar_21 = tmpvar_25;
  highp vec4 tmpvar_26;
  tmpvar_26.w = 1.0;
  tmpvar_26.xyz = tmpvar_13;
  lowp vec4 tmpvar_27;
  highp vec3 P_28;
  P_28 = (_LightMatrix0 * tmpvar_26).xyz;
  tmpvar_27 = texture (_LightTexture0, P_28);
  highp float tmpvar_29;
  tmpvar_29 = ((atten_5 * tmpvar_21) * tmpvar_27.w);
  atten_5 = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_31;
  tmpvar_31 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_31;
  mediump float tmpvar_32;
  tmpvar_32 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_32;
  highp float tmpvar_33;
  tmpvar_33 = (spec_3 * clamp (tmpvar_29, 0.0, 1.0));
  spec_3 = tmpvar_33;
  highp vec3 tmpvar_34;
  tmpvar_34 = (_LightColor.xyz * (tmpvar_30 * tmpvar_29));
  res_2.xyz = tmpvar_34;
  lowp vec3 c_35;
  c_35 = _LightColor.xyz;
  lowp float tmpvar_36;
  tmpvar_36 = dot (c_35, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_37;
  tmpvar_37 = (tmpvar_33 * tmpvar_36);
  res_2.w = tmpvar_37;
  highp float tmpvar_38;
  tmpvar_38 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_39;
  tmpvar_39 = (res_2 * tmpvar_38);
  res_2 = tmpvar_39;
  tmpvar_1 = tmpvar_39;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SHADOWS_NONATIVE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _ShadowOffsets[4];
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize(tmpvar_16);
  lightDir_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_13;
  highp vec4 tmpvar_19;
  tmpvar_19 = (_LightMatrix0 * tmpvar_18);
  lowp float tmpvar_20;
  tmpvar_20 = texture2DProj (_LightTexture0, tmpvar_19).w;
  atten_5 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture2D (_LightTextureB0, vec2(tmpvar_21));
  atten_5 = ((atten_5 * float(
    (tmpvar_19.w < 0.0)
  )) * tmpvar_22.w);
  mediump float tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = clamp (((tmpvar_15 * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0);
  highp vec4 tmpvar_25;
  tmpvar_25.w = 1.0;
  tmpvar_25.xyz = tmpvar_13;
  highp vec4 tmpvar_26;
  tmpvar_26 = (unity_World2Shadow[0] * tmpvar_25);
  highp vec4 shadowVals_27;
  highp vec3 tmpvar_28;
  tmpvar_28 = (tmpvar_26.xyz / tmpvar_26.w);
  highp vec2 P_29;
  P_29 = (tmpvar_28.xy + _ShadowOffsets[0].xy);
  lowp float tmpvar_30;
  tmpvar_30 = texture2D (_ShadowMapTexture, P_29).x;
  shadowVals_27.x = tmpvar_30;
  highp vec2 P_31;
  P_31 = (tmpvar_28.xy + _ShadowOffsets[1].xy);
  lowp float tmpvar_32;
  tmpvar_32 = texture2D (_ShadowMapTexture, P_31).x;
  shadowVals_27.y = tmpvar_32;
  highp vec2 P_33;
  P_33 = (tmpvar_28.xy + _ShadowOffsets[2].xy);
  lowp float tmpvar_34;
  tmpvar_34 = texture2D (_ShadowMapTexture, P_33).x;
  shadowVals_27.z = tmpvar_34;
  highp vec2 P_35;
  P_35 = (tmpvar_28.xy + _ShadowOffsets[3].xy);
  lowp float tmpvar_36;
  tmpvar_36 = texture2D (_ShadowMapTexture, P_35).x;
  shadowVals_27.w = tmpvar_36;
  bvec4 tmpvar_37;
  tmpvar_37 = lessThan (shadowVals_27, tmpvar_28.zzzz);
  highp vec4 tmpvar_38;
  tmpvar_38 = _LightShadowData.xxxx;
  highp float tmpvar_39;
  if (tmpvar_37.x) {
    tmpvar_39 = tmpvar_38.x;
  } else {
    tmpvar_39 = 1.0;
  };
  highp float tmpvar_40;
  if (tmpvar_37.y) {
    tmpvar_40 = tmpvar_38.y;
  } else {
    tmpvar_40 = 1.0;
  };
  highp float tmpvar_41;
  if (tmpvar_37.z) {
    tmpvar_41 = tmpvar_38.z;
  } else {
    tmpvar_41 = 1.0;
  };
  highp float tmpvar_42;
  if (tmpvar_37.w) {
    tmpvar_42 = tmpvar_38.w;
  } else {
    tmpvar_42 = 1.0;
  };
  mediump vec4 tmpvar_43;
  tmpvar_43.x = tmpvar_39;
  tmpvar_43.y = tmpvar_40;
  tmpvar_43.z = tmpvar_41;
  tmpvar_43.w = tmpvar_42;
  mediump float tmpvar_44;
  tmpvar_44 = dot (tmpvar_43, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_45;
  tmpvar_45 = clamp ((tmpvar_44 + tmpvar_24), 0.0, 1.0);
  tmpvar_23 = tmpvar_45;
  highp float tmpvar_46;
  tmpvar_46 = (atten_5 * tmpvar_23);
  atten_5 = tmpvar_46;
  mediump float tmpvar_47;
  tmpvar_47 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_48;
  tmpvar_48 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_48;
  mediump float tmpvar_49;
  tmpvar_49 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_49;
  highp float tmpvar_50;
  tmpvar_50 = (spec_3 * clamp (tmpvar_46, 0.0, 1.0));
  spec_3 = tmpvar_50;
  highp vec3 tmpvar_51;
  tmpvar_51 = (_LightColor.xyz * (tmpvar_47 * tmpvar_46));
  res_2.xyz = tmpvar_51;
  lowp vec3 c_52;
  c_52 = _LightColor.xyz;
  lowp float tmpvar_53;
  tmpvar_53 = dot (c_52, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_54;
  tmpvar_54 = (tmpvar_50 * tmpvar_53);
  res_2.w = tmpvar_54;
  highp float tmpvar_55;
  tmpvar_55 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_56;
  tmpvar_56 = (res_2 * tmpvar_55);
  res_2 = tmpvar_56;
  tmpvar_1 = tmpvar_56;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SHADOWS_NATIVE" }
"!!GLES


#ifdef VERTEX

#extension GL_EXT_shadow_samplers : enable
attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

#extension GL_EXT_shadow_samplers : enable
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp sampler2DShadow _ShadowMapTexture;
uniform highp vec4 _ShadowOffsets[4];
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize(tmpvar_16);
  lightDir_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_13;
  highp vec4 tmpvar_19;
  tmpvar_19 = (_LightMatrix0 * tmpvar_18);
  lowp float tmpvar_20;
  tmpvar_20 = texture2DProj (_LightTexture0, tmpvar_19).w;
  atten_5 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture2D (_LightTextureB0, vec2(tmpvar_21));
  atten_5 = ((atten_5 * float(
    (tmpvar_19.w < 0.0)
  )) * tmpvar_22.w);
  mediump float tmpvar_23;
  highp vec4 tmpvar_24;
  tmpvar_24.w = 1.0;
  tmpvar_24.xyz = tmpvar_13;
  highp vec4 tmpvar_25;
  tmpvar_25 = (unity_World2Shadow[0] * tmpvar_24);
  mediump vec4 shadows_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (tmpvar_25.xyz / tmpvar_25.w);
  highp vec3 coord_28;
  coord_28 = (tmpvar_27 + _ShadowOffsets[0].xyz);
  lowp float tmpvar_29;
  tmpvar_29 = shadow2DEXT (_ShadowMapTexture, coord_28);
  shadows_26.x = tmpvar_29;
  highp vec3 coord_30;
  coord_30 = (tmpvar_27 + _ShadowOffsets[1].xyz);
  lowp float tmpvar_31;
  tmpvar_31 = shadow2DEXT (_ShadowMapTexture, coord_30);
  shadows_26.y = tmpvar_31;
  highp vec3 coord_32;
  coord_32 = (tmpvar_27 + _ShadowOffsets[2].xyz);
  lowp float tmpvar_33;
  tmpvar_33 = shadow2DEXT (_ShadowMapTexture, coord_32);
  shadows_26.z = tmpvar_33;
  highp vec3 coord_34;
  coord_34 = (tmpvar_27 + _ShadowOffsets[3].xyz);
  lowp float tmpvar_35;
  tmpvar_35 = shadow2DEXT (_ShadowMapTexture, coord_34);
  shadows_26.w = tmpvar_35;
  highp vec4 tmpvar_36;
  tmpvar_36 = (_LightShadowData.xxxx + (shadows_26 * (1.0 - _LightShadowData.xxxx)));
  shadows_26 = tmpvar_36;
  mediump float tmpvar_37;
  tmpvar_37 = dot (shadows_26, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_38;
  tmpvar_38 = clamp ((tmpvar_37 + clamp (
    ((tmpvar_15 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_23 = tmpvar_38;
  highp float tmpvar_39;
  tmpvar_39 = (atten_5 * tmpvar_23);
  atten_5 = tmpvar_39;
  mediump float tmpvar_40;
  tmpvar_40 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_41;
  tmpvar_41 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_41;
  mediump float tmpvar_42;
  tmpvar_42 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_42;
  highp float tmpvar_43;
  tmpvar_43 = (spec_3 * clamp (tmpvar_39, 0.0, 1.0));
  spec_3 = tmpvar_43;
  highp vec3 tmpvar_44;
  tmpvar_44 = (_LightColor.xyz * (tmpvar_40 * tmpvar_39));
  res_2.xyz = tmpvar_44;
  lowp vec3 c_45;
  c_45 = _LightColor.xyz;
  lowp float tmpvar_46;
  tmpvar_46 = dot (c_45, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_47;
  tmpvar_47 = (tmpvar_43 * tmpvar_46);
  res_2.w = tmpvar_47;
  highp float tmpvar_48;
  tmpvar_48 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_49;
  tmpvar_49 = (res_2 * tmpvar_48);
  res_2 = tmpvar_49;
  tmpvar_1 = tmpvar_49;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SHADOWS_NATIVE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp sampler2DShadow _ShadowMapTexture;
uniform highp vec4 _ShadowOffsets[4];
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize(tmpvar_16);
  lightDir_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_13;
  highp vec4 tmpvar_19;
  tmpvar_19 = (_LightMatrix0 * tmpvar_18);
  lowp float tmpvar_20;
  tmpvar_20 = textureProj (_LightTexture0, tmpvar_19).w;
  atten_5 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture (_LightTextureB0, vec2(tmpvar_21));
  atten_5 = ((atten_5 * float(
    (tmpvar_19.w < 0.0)
  )) * tmpvar_22.w);
  mediump float tmpvar_23;
  highp vec4 tmpvar_24;
  tmpvar_24.w = 1.0;
  tmpvar_24.xyz = tmpvar_13;
  highp vec4 tmpvar_25;
  tmpvar_25 = (unity_World2Shadow[0] * tmpvar_24);
  mediump vec4 shadows_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (tmpvar_25.xyz / tmpvar_25.w);
  highp vec3 coord_28;
  coord_28 = (tmpvar_27 + _ShadowOffsets[0].xyz);
  mediump float tmpvar_29;
  tmpvar_29 = texture (_ShadowMapTexture, coord_28);
  shadows_26.x = tmpvar_29;
  highp vec3 coord_30;
  coord_30 = (tmpvar_27 + _ShadowOffsets[1].xyz);
  mediump float tmpvar_31;
  tmpvar_31 = texture (_ShadowMapTexture, coord_30);
  shadows_26.y = tmpvar_31;
  highp vec3 coord_32;
  coord_32 = (tmpvar_27 + _ShadowOffsets[2].xyz);
  mediump float tmpvar_33;
  tmpvar_33 = texture (_ShadowMapTexture, coord_32);
  shadows_26.z = tmpvar_33;
  highp vec3 coord_34;
  coord_34 = (tmpvar_27 + _ShadowOffsets[3].xyz);
  mediump float tmpvar_35;
  tmpvar_35 = texture (_ShadowMapTexture, coord_34);
  shadows_26.w = tmpvar_35;
  highp vec4 tmpvar_36;
  tmpvar_36 = (_LightShadowData.xxxx + (shadows_26 * (1.0 - _LightShadowData.xxxx)));
  shadows_26 = tmpvar_36;
  mediump float tmpvar_37;
  tmpvar_37 = dot (shadows_26, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_38;
  tmpvar_38 = clamp ((tmpvar_37 + clamp (
    ((tmpvar_15 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_23 = tmpvar_38;
  highp float tmpvar_39;
  tmpvar_39 = (atten_5 * tmpvar_23);
  atten_5 = tmpvar_39;
  mediump float tmpvar_40;
  tmpvar_40 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_41;
  tmpvar_41 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_41;
  mediump float tmpvar_42;
  tmpvar_42 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_42;
  highp float tmpvar_43;
  tmpvar_43 = (spec_3 * clamp (tmpvar_39, 0.0, 1.0));
  spec_3 = tmpvar_43;
  highp vec3 tmpvar_44;
  tmpvar_44 = (_LightColor.xyz * (tmpvar_40 * tmpvar_39));
  res_2.xyz = tmpvar_44;
  lowp vec3 c_45;
  c_45 = _LightColor.xyz;
  lowp float tmpvar_46;
  tmpvar_46 = dot (c_45, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_47;
  tmpvar_47 = (tmpvar_43 * tmpvar_46);
  res_2.w = tmpvar_47;
  highp float tmpvar_48;
  tmpvar_48 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_49;
  tmpvar_49 = (res_2 * tmpvar_48);
  res_2 = tmpvar_49;
  tmpvar_1 = tmpvar_49;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture2D (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  highp vec4 shadowVals_21;
  highp vec3 vec_22;
  vec_22 = (tmpvar_16 + vec3(0.0078125, 0.0078125, 0.0078125));
  highp vec4 packDist_23;
  lowp vec4 tmpvar_24;
  tmpvar_24 = textureCube (_ShadowMapTexture, vec_22);
  packDist_23 = tmpvar_24;
  shadowVals_21.x = dot (packDist_23, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_25;
  vec_25 = (tmpvar_16 + vec3(-0.0078125, -0.0078125, 0.0078125));
  highp vec4 packDist_26;
  lowp vec4 tmpvar_27;
  tmpvar_27 = textureCube (_ShadowMapTexture, vec_25);
  packDist_26 = tmpvar_27;
  shadowVals_21.y = dot (packDist_26, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_28;
  vec_28 = (tmpvar_16 + vec3(-0.0078125, 0.0078125, -0.0078125));
  highp vec4 packDist_29;
  lowp vec4 tmpvar_30;
  tmpvar_30 = textureCube (_ShadowMapTexture, vec_28);
  packDist_29 = tmpvar_30;
  shadowVals_21.z = dot (packDist_29, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_31;
  vec_31 = (tmpvar_16 + vec3(0.0078125, -0.0078125, -0.0078125));
  highp vec4 packDist_32;
  lowp vec4 tmpvar_33;
  tmpvar_33 = textureCube (_ShadowMapTexture, vec_31);
  packDist_32 = tmpvar_33;
  shadowVals_21.w = dot (packDist_32, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  bvec4 tmpvar_34;
  tmpvar_34 = lessThan (shadowVals_21, vec4(tmpvar_20));
  highp vec4 tmpvar_35;
  tmpvar_35 = _LightShadowData.xxxx;
  highp float tmpvar_36;
  if (tmpvar_34.x) {
    tmpvar_36 = tmpvar_35.x;
  } else {
    tmpvar_36 = 1.0;
  };
  highp float tmpvar_37;
  if (tmpvar_34.y) {
    tmpvar_37 = tmpvar_35.y;
  } else {
    tmpvar_37 = 1.0;
  };
  highp float tmpvar_38;
  if (tmpvar_34.z) {
    tmpvar_38 = tmpvar_35.z;
  } else {
    tmpvar_38 = 1.0;
  };
  highp float tmpvar_39;
  if (tmpvar_34.w) {
    tmpvar_39 = tmpvar_35.w;
  } else {
    tmpvar_39 = 1.0;
  };
  mediump vec4 tmpvar_40;
  tmpvar_40.x = tmpvar_36;
  tmpvar_40.y = tmpvar_37;
  tmpvar_40.z = tmpvar_38;
  tmpvar_40.w = tmpvar_39;
  mediump float tmpvar_41;
  tmpvar_41 = dot (tmpvar_40, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_42;
  tmpvar_42 = (atten_5 * tmpvar_41);
  atten_5 = tmpvar_42;
  mediump float tmpvar_43;
  tmpvar_43 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_44;
  tmpvar_44 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_44;
  mediump float tmpvar_45;
  tmpvar_45 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_45;
  highp float tmpvar_46;
  tmpvar_46 = (spec_3 * clamp (tmpvar_42, 0.0, 1.0));
  spec_3 = tmpvar_46;
  highp vec3 tmpvar_47;
  tmpvar_47 = (_LightColor.xyz * (tmpvar_43 * tmpvar_42));
  res_2.xyz = tmpvar_47;
  lowp vec3 c_48;
  c_48 = _LightColor.xyz;
  lowp float tmpvar_49;
  tmpvar_49 = dot (c_48, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_50;
  tmpvar_50 = (tmpvar_46 * tmpvar_49);
  res_2.w = tmpvar_50;
  highp float tmpvar_51;
  tmpvar_51 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_52;
  tmpvar_52 = (res_2 * tmpvar_51);
  res_2 = tmpvar_52;
  tmpvar_1 = tmpvar_52;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  highp vec4 shadowVals_21;
  highp vec3 vec_22;
  vec_22 = (tmpvar_16 + vec3(0.0078125, 0.0078125, 0.0078125));
  highp vec4 packDist_23;
  lowp vec4 tmpvar_24;
  tmpvar_24 = texture (_ShadowMapTexture, vec_22);
  packDist_23 = tmpvar_24;
  shadowVals_21.x = dot (packDist_23, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_25;
  vec_25 = (tmpvar_16 + vec3(-0.0078125, -0.0078125, 0.0078125));
  highp vec4 packDist_26;
  lowp vec4 tmpvar_27;
  tmpvar_27 = texture (_ShadowMapTexture, vec_25);
  packDist_26 = tmpvar_27;
  shadowVals_21.y = dot (packDist_26, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_28;
  vec_28 = (tmpvar_16 + vec3(-0.0078125, 0.0078125, -0.0078125));
  highp vec4 packDist_29;
  lowp vec4 tmpvar_30;
  tmpvar_30 = texture (_ShadowMapTexture, vec_28);
  packDist_29 = tmpvar_30;
  shadowVals_21.z = dot (packDist_29, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_31;
  vec_31 = (tmpvar_16 + vec3(0.0078125, -0.0078125, -0.0078125));
  highp vec4 packDist_32;
  lowp vec4 tmpvar_33;
  tmpvar_33 = texture (_ShadowMapTexture, vec_31);
  packDist_32 = tmpvar_33;
  shadowVals_21.w = dot (packDist_32, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  bvec4 tmpvar_34;
  tmpvar_34 = lessThan (shadowVals_21, vec4(tmpvar_20));
  highp vec4 tmpvar_35;
  tmpvar_35 = _LightShadowData.xxxx;
  highp float tmpvar_36;
  if (tmpvar_34.x) {
    tmpvar_36 = tmpvar_35.x;
  } else {
    tmpvar_36 = 1.0;
  };
  highp float tmpvar_37;
  if (tmpvar_34.y) {
    tmpvar_37 = tmpvar_35.y;
  } else {
    tmpvar_37 = 1.0;
  };
  highp float tmpvar_38;
  if (tmpvar_34.z) {
    tmpvar_38 = tmpvar_35.z;
  } else {
    tmpvar_38 = 1.0;
  };
  highp float tmpvar_39;
  if (tmpvar_34.w) {
    tmpvar_39 = tmpvar_35.w;
  } else {
    tmpvar_39 = 1.0;
  };
  mediump vec4 tmpvar_40;
  tmpvar_40.x = tmpvar_36;
  tmpvar_40.y = tmpvar_37;
  tmpvar_40.z = tmpvar_38;
  tmpvar_40.w = tmpvar_39;
  mediump float tmpvar_41;
  tmpvar_41 = dot (tmpvar_40, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_42;
  tmpvar_42 = (atten_5 * tmpvar_41);
  atten_5 = tmpvar_42;
  mediump float tmpvar_43;
  tmpvar_43 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_44;
  tmpvar_44 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_44;
  mediump float tmpvar_45;
  tmpvar_45 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_45;
  highp float tmpvar_46;
  tmpvar_46 = (spec_3 * clamp (tmpvar_42, 0.0, 1.0));
  spec_3 = tmpvar_46;
  highp vec3 tmpvar_47;
  tmpvar_47 = (_LightColor.xyz * (tmpvar_43 * tmpvar_42));
  res_2.xyz = tmpvar_47;
  lowp vec3 c_48;
  c_48 = _LightColor.xyz;
  lowp float tmpvar_49;
  tmpvar_49 = dot (c_48, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_50;
  tmpvar_50 = (tmpvar_46 * tmpvar_49);
  res_2.w = tmpvar_50;
  highp float tmpvar_51;
  tmpvar_51 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_52;
  tmpvar_52 = (res_2 * tmpvar_51);
  res_2 = tmpvar_52;
  tmpvar_1 = tmpvar_52;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _LightTexture0;
uniform lowp samplerCube _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture2D (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  highp vec4 shadowVals_21;
  highp vec3 vec_22;
  vec_22 = (tmpvar_16 + vec3(0.0078125, 0.0078125, 0.0078125));
  highp vec4 packDist_23;
  lowp vec4 tmpvar_24;
  tmpvar_24 = textureCube (_ShadowMapTexture, vec_22);
  packDist_23 = tmpvar_24;
  shadowVals_21.x = dot (packDist_23, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_25;
  vec_25 = (tmpvar_16 + vec3(-0.0078125, -0.0078125, 0.0078125));
  highp vec4 packDist_26;
  lowp vec4 tmpvar_27;
  tmpvar_27 = textureCube (_ShadowMapTexture, vec_25);
  packDist_26 = tmpvar_27;
  shadowVals_21.y = dot (packDist_26, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_28;
  vec_28 = (tmpvar_16 + vec3(-0.0078125, 0.0078125, -0.0078125));
  highp vec4 packDist_29;
  lowp vec4 tmpvar_30;
  tmpvar_30 = textureCube (_ShadowMapTexture, vec_28);
  packDist_29 = tmpvar_30;
  shadowVals_21.z = dot (packDist_29, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_31;
  vec_31 = (tmpvar_16 + vec3(0.0078125, -0.0078125, -0.0078125));
  highp vec4 packDist_32;
  lowp vec4 tmpvar_33;
  tmpvar_33 = textureCube (_ShadowMapTexture, vec_31);
  packDist_32 = tmpvar_33;
  shadowVals_21.w = dot (packDist_32, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  bvec4 tmpvar_34;
  tmpvar_34 = lessThan (shadowVals_21, vec4(tmpvar_20));
  highp vec4 tmpvar_35;
  tmpvar_35 = _LightShadowData.xxxx;
  highp float tmpvar_36;
  if (tmpvar_34.x) {
    tmpvar_36 = tmpvar_35.x;
  } else {
    tmpvar_36 = 1.0;
  };
  highp float tmpvar_37;
  if (tmpvar_34.y) {
    tmpvar_37 = tmpvar_35.y;
  } else {
    tmpvar_37 = 1.0;
  };
  highp float tmpvar_38;
  if (tmpvar_34.z) {
    tmpvar_38 = tmpvar_35.z;
  } else {
    tmpvar_38 = 1.0;
  };
  highp float tmpvar_39;
  if (tmpvar_34.w) {
    tmpvar_39 = tmpvar_35.w;
  } else {
    tmpvar_39 = 1.0;
  };
  mediump vec4 tmpvar_40;
  tmpvar_40.x = tmpvar_36;
  tmpvar_40.y = tmpvar_37;
  tmpvar_40.z = tmpvar_38;
  tmpvar_40.w = tmpvar_39;
  mediump float tmpvar_41;
  tmpvar_41 = dot (tmpvar_40, vec4(0.25, 0.25, 0.25, 0.25));
  highp vec4 tmpvar_42;
  tmpvar_42.w = 1.0;
  tmpvar_42.xyz = tmpvar_13;
  lowp vec4 tmpvar_43;
  highp vec3 P_44;
  P_44 = (_LightMatrix0 * tmpvar_42).xyz;
  tmpvar_43 = textureCube (_LightTexture0, P_44);
  highp float tmpvar_45;
  tmpvar_45 = ((atten_5 * tmpvar_41) * tmpvar_43.w);
  atten_5 = tmpvar_45;
  mediump float tmpvar_46;
  tmpvar_46 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_47;
  tmpvar_47 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_47;
  mediump float tmpvar_48;
  tmpvar_48 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_48;
  highp float tmpvar_49;
  tmpvar_49 = (spec_3 * clamp (tmpvar_45, 0.0, 1.0));
  spec_3 = tmpvar_49;
  highp vec3 tmpvar_50;
  tmpvar_50 = (_LightColor.xyz * (tmpvar_46 * tmpvar_45));
  res_2.xyz = tmpvar_50;
  lowp vec3 c_51;
  c_51 = _LightColor.xyz;
  lowp float tmpvar_52;
  tmpvar_52 = dot (c_51, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_53;
  tmpvar_53 = (tmpvar_49 * tmpvar_52);
  res_2.w = tmpvar_53;
  highp float tmpvar_54;
  tmpvar_54 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_55;
  tmpvar_55 = (res_2 * tmpvar_54);
  res_2 = tmpvar_55;
  tmpvar_1 = tmpvar_55;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _LightTexture0;
uniform lowp samplerCube _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  highp vec4 shadowVals_21;
  highp vec3 vec_22;
  vec_22 = (tmpvar_16 + vec3(0.0078125, 0.0078125, 0.0078125));
  highp vec4 packDist_23;
  lowp vec4 tmpvar_24;
  tmpvar_24 = texture (_ShadowMapTexture, vec_22);
  packDist_23 = tmpvar_24;
  shadowVals_21.x = dot (packDist_23, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_25;
  vec_25 = (tmpvar_16 + vec3(-0.0078125, -0.0078125, 0.0078125));
  highp vec4 packDist_26;
  lowp vec4 tmpvar_27;
  tmpvar_27 = texture (_ShadowMapTexture, vec_25);
  packDist_26 = tmpvar_27;
  shadowVals_21.y = dot (packDist_26, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_28;
  vec_28 = (tmpvar_16 + vec3(-0.0078125, 0.0078125, -0.0078125));
  highp vec4 packDist_29;
  lowp vec4 tmpvar_30;
  tmpvar_30 = texture (_ShadowMapTexture, vec_28);
  packDist_29 = tmpvar_30;
  shadowVals_21.z = dot (packDist_29, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_31;
  vec_31 = (tmpvar_16 + vec3(0.0078125, -0.0078125, -0.0078125));
  highp vec4 packDist_32;
  lowp vec4 tmpvar_33;
  tmpvar_33 = texture (_ShadowMapTexture, vec_31);
  packDist_32 = tmpvar_33;
  shadowVals_21.w = dot (packDist_32, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  bvec4 tmpvar_34;
  tmpvar_34 = lessThan (shadowVals_21, vec4(tmpvar_20));
  highp vec4 tmpvar_35;
  tmpvar_35 = _LightShadowData.xxxx;
  highp float tmpvar_36;
  if (tmpvar_34.x) {
    tmpvar_36 = tmpvar_35.x;
  } else {
    tmpvar_36 = 1.0;
  };
  highp float tmpvar_37;
  if (tmpvar_34.y) {
    tmpvar_37 = tmpvar_35.y;
  } else {
    tmpvar_37 = 1.0;
  };
  highp float tmpvar_38;
  if (tmpvar_34.z) {
    tmpvar_38 = tmpvar_35.z;
  } else {
    tmpvar_38 = 1.0;
  };
  highp float tmpvar_39;
  if (tmpvar_34.w) {
    tmpvar_39 = tmpvar_35.w;
  } else {
    tmpvar_39 = 1.0;
  };
  mediump vec4 tmpvar_40;
  tmpvar_40.x = tmpvar_36;
  tmpvar_40.y = tmpvar_37;
  tmpvar_40.z = tmpvar_38;
  tmpvar_40.w = tmpvar_39;
  mediump float tmpvar_41;
  tmpvar_41 = dot (tmpvar_40, vec4(0.25, 0.25, 0.25, 0.25));
  highp vec4 tmpvar_42;
  tmpvar_42.w = 1.0;
  tmpvar_42.xyz = tmpvar_13;
  lowp vec4 tmpvar_43;
  highp vec3 P_44;
  P_44 = (_LightMatrix0 * tmpvar_42).xyz;
  tmpvar_43 = texture (_LightTexture0, P_44);
  highp float tmpvar_45;
  tmpvar_45 = ((atten_5 * tmpvar_41) * tmpvar_43.w);
  atten_5 = tmpvar_45;
  mediump float tmpvar_46;
  tmpvar_46 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_47;
  tmpvar_47 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_47;
  mediump float tmpvar_48;
  tmpvar_48 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_48;
  highp float tmpvar_49;
  tmpvar_49 = (spec_3 * clamp (tmpvar_45, 0.0, 1.0));
  spec_3 = tmpvar_49;
  highp vec3 tmpvar_50;
  tmpvar_50 = (_LightColor.xyz * (tmpvar_46 * tmpvar_45));
  res_2.xyz = tmpvar_50;
  lowp vec3 c_51;
  c_51 = _LightColor.xyz;
  lowp float tmpvar_52;
  tmpvar_52 = dot (c_51, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_53;
  tmpvar_53 = (tmpvar_49 * tmpvar_52);
  res_2.w = tmpvar_53;
  highp float tmpvar_54;
  tmpvar_54 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_55;
  tmpvar_55 = (res_2 * tmpvar_54);
  res_2 = tmpvar_55;
  tmpvar_1 = tmpvar_55;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec3 normal_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  normal_6 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_14;
  tmpvar_14 = mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_15;
  tmpvar_15 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_15;
  mediump float tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture2D (_ShadowMapTexture, tmpvar_8);
  highp float tmpvar_18;
  tmpvar_18 = clamp ((tmpvar_17.x + clamp (
    ((tmpvar_14 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_16 = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = tmpvar_16;
  mediump float tmpvar_20;
  tmpvar_20 = max (0.0, dot (lightDir_5, normal_6));
  highp vec3 tmpvar_21;
  tmpvar_21 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_21;
  mediump float tmpvar_22;
  tmpvar_22 = pow (max (0.0, dot (h_4, normal_6)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = (spec_3 * clamp (tmpvar_19, 0.0, 1.0));
  spec_3 = tmpvar_23;
  highp vec3 tmpvar_24;
  tmpvar_24 = (_LightColor.xyz * (tmpvar_20 * tmpvar_19));
  res_2.xyz = tmpvar_24;
  lowp vec3 c_25;
  c_25 = _LightColor.xyz;
  lowp float tmpvar_26;
  tmpvar_26 = dot (c_25, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_27;
  tmpvar_27 = (tmpvar_23 * tmpvar_26);
  res_2.w = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = clamp ((1.0 - (
    (tmpvar_14 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_29;
  tmpvar_29 = (res_2 * tmpvar_28);
  res_2 = tmpvar_29;
  tmpvar_1 = tmpvar_29;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec3 normal_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  normal_6 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_14;
  tmpvar_14 = mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_15;
  tmpvar_15 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_15;
  mediump float tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture (_ShadowMapTexture, tmpvar_8);
  highp float tmpvar_18;
  tmpvar_18 = clamp ((tmpvar_17.x + clamp (
    ((tmpvar_14 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_16 = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = tmpvar_16;
  mediump float tmpvar_20;
  tmpvar_20 = max (0.0, dot (lightDir_5, normal_6));
  highp vec3 tmpvar_21;
  tmpvar_21 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_21;
  mediump float tmpvar_22;
  tmpvar_22 = pow (max (0.0, dot (h_4, normal_6)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = (spec_3 * clamp (tmpvar_19, 0.0, 1.0));
  spec_3 = tmpvar_23;
  highp vec3 tmpvar_24;
  tmpvar_24 = (_LightColor.xyz * (tmpvar_20 * tmpvar_19));
  res_2.xyz = tmpvar_24;
  lowp vec3 c_25;
  c_25 = _LightColor.xyz;
  lowp float tmpvar_26;
  tmpvar_26 = dot (c_25, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_27;
  tmpvar_27 = (tmpvar_23 * tmpvar_26);
  res_2.w = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = clamp ((1.0 - (
    (tmpvar_14 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_29;
  tmpvar_29 = (res_2 * tmpvar_28);
  res_2 = tmpvar_29;
  tmpvar_1 = tmpvar_29;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTexture0;
uniform sampler2D _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec3 normal_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  normal_6 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_14;
  tmpvar_14 = mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_15;
  tmpvar_15 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_15;
  mediump float tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture2D (_ShadowMapTexture, tmpvar_8);
  highp float tmpvar_18;
  tmpvar_18 = clamp ((tmpvar_17.x + clamp (
    ((tmpvar_14 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_16 = tmpvar_18;
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = tmpvar_12;
  lowp vec4 tmpvar_20;
  highp vec2 P_21;
  P_21 = (_LightMatrix0 * tmpvar_19).xy;
  tmpvar_20 = texture2D (_LightTexture0, P_21);
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_16 * tmpvar_20.w);
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_5, normal_6));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, normal_6)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (tmpvar_14 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  tmpvar_1 = tmpvar_32;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTexture0;
uniform sampler2D _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec3 normal_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  normal_6 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_14;
  tmpvar_14 = mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_15;
  tmpvar_15 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_15;
  mediump float tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture (_ShadowMapTexture, tmpvar_8);
  highp float tmpvar_18;
  tmpvar_18 = clamp ((tmpvar_17.x + clamp (
    ((tmpvar_14 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_16 = tmpvar_18;
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = tmpvar_12;
  lowp vec4 tmpvar_20;
  highp vec2 P_21;
  P_21 = (_LightMatrix0 * tmpvar_19).xy;
  tmpvar_20 = texture (_LightTexture0, P_21);
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_16 * tmpvar_20.w);
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_5, normal_6));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, normal_6)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (tmpvar_14 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  tmpvar_1 = tmpvar_32;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT" "SHADOWS_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "SPOT" "SHADOWS_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" "SHADOWS_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_NONATIVE" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_NATIVE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_NATIVE" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT" "SHADOWS_CUBE" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SHADOWS_NONATIVE" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SHADOWS_NATIVE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SHADOWS_NATIVE" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES3"
}
}
 }
 Pass {
  Tags { "ShadowSupport"="True" }
  ZWrite Off
  Fog { Mode Off }
  Blend One One
Program "vp" {
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _LightTextureB0;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_16;
  tmpvar_16 = -(normalize(tmpvar_15));
  lightDir_6 = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = (dot (tmpvar_15, tmpvar_15) * _LightPos.w);
  lowp float tmpvar_18;
  tmpvar_18 = texture2D (_LightTextureB0, vec2(tmpvar_17)).w;
  atten_5 = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = max (0.0, dot (lightDir_6, tmpvar_10));
  highp vec3 tmpvar_20;
  tmpvar_20 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = pow (max (0.0, dot (h_4, tmpvar_10)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = (spec_3 * clamp (atten_5, 0.0, 1.0));
  spec_3 = tmpvar_22;
  highp vec3 tmpvar_23;
  tmpvar_23 = (_LightColor.xyz * (tmpvar_19 * atten_5));
  res_2.xyz = tmpvar_23;
  lowp vec3 c_24;
  c_24 = _LightColor.xyz;
  lowp float tmpvar_25;
  tmpvar_25 = dot (c_24, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_26;
  tmpvar_26 = (tmpvar_22 * tmpvar_25);
  res_2.w = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = clamp ((1.0 - (
    (mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_28;
  tmpvar_28 = (res_2 * tmpvar_27);
  res_2 = tmpvar_28;
  tmpvar_1 = tmpvar_28.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT" "SHADOWS_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _LightTextureB0;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_16;
  tmpvar_16 = -(normalize(tmpvar_15));
  lightDir_6 = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = (dot (tmpvar_15, tmpvar_15) * _LightPos.w);
  lowp float tmpvar_18;
  tmpvar_18 = texture (_LightTextureB0, vec2(tmpvar_17)).w;
  atten_5 = tmpvar_18;
  mediump float tmpvar_19;
  tmpvar_19 = max (0.0, dot (lightDir_6, tmpvar_10));
  highp vec3 tmpvar_20;
  tmpvar_20 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = pow (max (0.0, dot (h_4, tmpvar_10)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = (spec_3 * clamp (atten_5, 0.0, 1.0));
  spec_3 = tmpvar_22;
  highp vec3 tmpvar_23;
  tmpvar_23 = (_LightColor.xyz * (tmpvar_19 * atten_5));
  res_2.xyz = tmpvar_23;
  lowp vec3 c_24;
  c_24 = _LightColor.xyz;
  lowp float tmpvar_25;
  tmpvar_25 = dot (c_24, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_26;
  tmpvar_26 = (tmpvar_22 * tmpvar_25);
  res_2.w = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = clamp ((1.0 - (
    (mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_28;
  tmpvar_28 = (res_2 * tmpvar_27);
  res_2 = tmpvar_28;
  tmpvar_1 = tmpvar_28.wxyz;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec4 nspec_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_CameraNormalsTexture, tmpvar_7);
  nspec_6 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = normalize(((nspec_6.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraDepthTexture, tmpvar_7);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_14;
  tmpvar_14 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_14;
  mediump float tmpvar_15;
  tmpvar_15 = max (0.0, dot (lightDir_5, tmpvar_9));
  highp vec3 tmpvar_16;
  tmpvar_16 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_16;
  mediump float tmpvar_17;
  tmpvar_17 = pow (max (0.0, dot (h_4, tmpvar_9)), (nspec_6.w * 128.0));
  spec_3 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (spec_3 * clamp (1.0, 0.0, 1.0));
  spec_3 = tmpvar_18;
  highp vec3 tmpvar_19;
  tmpvar_19 = (_LightColor.xyz * tmpvar_15);
  res_2.xyz = tmpvar_19;
  lowp vec3 c_20;
  c_20 = _LightColor.xyz;
  lowp float tmpvar_21;
  tmpvar_21 = dot (c_20, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_18 * tmpvar_21);
  res_2.w = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = clamp ((1.0 - (
    (mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_24;
  tmpvar_24 = (res_2 * tmpvar_23);
  res_2 = tmpvar_24;
  tmpvar_1 = tmpvar_24.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec4 nspec_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture (_CameraNormalsTexture, tmpvar_7);
  nspec_6 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = normalize(((nspec_6.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraDepthTexture, tmpvar_7);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_14;
  tmpvar_14 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_14;
  mediump float tmpvar_15;
  tmpvar_15 = max (0.0, dot (lightDir_5, tmpvar_9));
  highp vec3 tmpvar_16;
  tmpvar_16 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_16;
  mediump float tmpvar_17;
  tmpvar_17 = pow (max (0.0, dot (h_4, tmpvar_9)), (nspec_6.w * 128.0));
  spec_3 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (spec_3 * clamp (1.0, 0.0, 1.0));
  spec_3 = tmpvar_18;
  highp vec3 tmpvar_19;
  tmpvar_19 = (_LightColor.xyz * tmpvar_15);
  res_2.xyz = tmpvar_19;
  lowp vec3 c_20;
  c_20 = _LightColor.xyz;
  lowp float tmpvar_21;
  tmpvar_21 = dot (c_20, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_18 * tmpvar_21);
  res_2.w = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = clamp ((1.0 - (
    (mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_24;
  tmpvar_24 = (res_2 * tmpvar_23);
  res_2 = tmpvar_24;
  tmpvar_1 = tmpvar_24.wxyz;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_15;
  tmpvar_15 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_16;
  tmpvar_16 = normalize(tmpvar_15);
  lightDir_6 = tmpvar_16;
  highp vec4 tmpvar_17;
  tmpvar_17.w = 1.0;
  tmpvar_17.xyz = tmpvar_13;
  highp vec4 tmpvar_18;
  tmpvar_18 = (_LightMatrix0 * tmpvar_17);
  lowp float tmpvar_19;
  tmpvar_19 = texture2DProj (_LightTexture0, tmpvar_18).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = (dot (tmpvar_15, tmpvar_15) * _LightPos.w);
  lowp vec4 tmpvar_21;
  tmpvar_21 = texture2D (_LightTextureB0, vec2(tmpvar_20));
  highp float tmpvar_22;
  tmpvar_22 = ((atten_5 * float(
    (tmpvar_18.w < 0.0)
  )) * tmpvar_21.w);
  atten_5 = tmpvar_22;
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_6, tmpvar_10));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, tmpvar_10)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  tmpvar_1 = tmpvar_32.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "SPOT" "SHADOWS_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_15;
  tmpvar_15 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_16;
  tmpvar_16 = normalize(tmpvar_15);
  lightDir_6 = tmpvar_16;
  highp vec4 tmpvar_17;
  tmpvar_17.w = 1.0;
  tmpvar_17.xyz = tmpvar_13;
  highp vec4 tmpvar_18;
  tmpvar_18 = (_LightMatrix0 * tmpvar_17);
  lowp float tmpvar_19;
  tmpvar_19 = textureProj (_LightTexture0, tmpvar_18).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = (dot (tmpvar_15, tmpvar_15) * _LightPos.w);
  lowp vec4 tmpvar_21;
  tmpvar_21 = texture (_LightTextureB0, vec2(tmpvar_20));
  highp float tmpvar_22;
  tmpvar_22 = ((atten_5 * float(
    (tmpvar_18.w < 0.0)
  )) * tmpvar_21.w);
  atten_5 = tmpvar_22;
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_6, tmpvar_10));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, tmpvar_10)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  tmpvar_1 = tmpvar_32.wxyz;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _LightTexture0;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_16;
  tmpvar_16 = -(normalize(tmpvar_15));
  lightDir_6 = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = (dot (tmpvar_15, tmpvar_15) * _LightPos.w);
  lowp float tmpvar_18;
  tmpvar_18 = texture2D (_LightTextureB0, vec2(tmpvar_17)).w;
  atten_5 = tmpvar_18;
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = tmpvar_13;
  lowp vec4 tmpvar_20;
  highp vec3 P_21;
  P_21 = (_LightMatrix0 * tmpvar_19).xyz;
  tmpvar_20 = textureCube (_LightTexture0, P_21);
  highp float tmpvar_22;
  tmpvar_22 = (atten_5 * tmpvar_20.w);
  atten_5 = tmpvar_22;
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_6, tmpvar_10));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, tmpvar_10)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  tmpvar_1 = tmpvar_32.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" "SHADOWS_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _LightTexture0;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  mediump vec3 tmpvar_10;
  tmpvar_10 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_15;
  tmpvar_15 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_16;
  tmpvar_16 = -(normalize(tmpvar_15));
  lightDir_6 = tmpvar_16;
  highp float tmpvar_17;
  tmpvar_17 = (dot (tmpvar_15, tmpvar_15) * _LightPos.w);
  lowp float tmpvar_18;
  tmpvar_18 = texture (_LightTextureB0, vec2(tmpvar_17)).w;
  atten_5 = tmpvar_18;
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = tmpvar_13;
  lowp vec4 tmpvar_20;
  highp vec3 P_21;
  P_21 = (_LightMatrix0 * tmpvar_19).xyz;
  tmpvar_20 = texture (_LightTexture0, P_21);
  highp float tmpvar_22;
  tmpvar_22 = (atten_5 * tmpvar_20.w);
  atten_5 = tmpvar_22;
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_6, tmpvar_10));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, tmpvar_10)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  tmpvar_1 = tmpvar_32.wxyz;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_OFF" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTexture0;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec4 nspec_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_CameraNormalsTexture, tmpvar_7);
  nspec_6 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = normalize(((nspec_6.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraDepthTexture, tmpvar_7);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_14;
  tmpvar_14 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_14;
  highp vec4 tmpvar_15;
  tmpvar_15.w = 1.0;
  tmpvar_15.xyz = tmpvar_12;
  lowp vec4 tmpvar_16;
  highp vec2 P_17;
  P_17 = (_LightMatrix0 * tmpvar_15).xy;
  tmpvar_16 = texture2D (_LightTexture0, P_17);
  highp float tmpvar_18;
  tmpvar_18 = tmpvar_16.w;
  mediump float tmpvar_19;
  tmpvar_19 = max (0.0, dot (lightDir_5, tmpvar_9));
  highp vec3 tmpvar_20;
  tmpvar_20 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = pow (max (0.0, dot (h_4, tmpvar_9)), (nspec_6.w * 128.0));
  spec_3 = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = (spec_3 * clamp (tmpvar_18, 0.0, 1.0));
  spec_3 = tmpvar_22;
  highp vec3 tmpvar_23;
  tmpvar_23 = (_LightColor.xyz * (tmpvar_19 * tmpvar_18));
  res_2.xyz = tmpvar_23;
  lowp vec3 c_24;
  c_24 = _LightColor.xyz;
  lowp float tmpvar_25;
  tmpvar_25 = dot (c_24, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_26;
  tmpvar_26 = (tmpvar_22 * tmpvar_25);
  res_2.w = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = clamp ((1.0 - (
    (mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_28;
  tmpvar_28 = (res_2 * tmpvar_27);
  res_2 = tmpvar_28;
  tmpvar_1 = tmpvar_28.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_OFF" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTexture0;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec4 nspec_6;
  highp vec2 tmpvar_7;
  tmpvar_7 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture (_CameraNormalsTexture, tmpvar_7);
  nspec_6 = tmpvar_8;
  mediump vec3 tmpvar_9;
  tmpvar_9 = normalize(((nspec_6.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraDepthTexture, tmpvar_7);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp vec3 tmpvar_14;
  tmpvar_14 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_14;
  highp vec4 tmpvar_15;
  tmpvar_15.w = 1.0;
  tmpvar_15.xyz = tmpvar_12;
  lowp vec4 tmpvar_16;
  highp vec2 P_17;
  P_17 = (_LightMatrix0 * tmpvar_15).xy;
  tmpvar_16 = texture (_LightTexture0, P_17);
  highp float tmpvar_18;
  tmpvar_18 = tmpvar_16.w;
  mediump float tmpvar_19;
  tmpvar_19 = max (0.0, dot (lightDir_5, tmpvar_9));
  highp vec3 tmpvar_20;
  tmpvar_20 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_20;
  mediump float tmpvar_21;
  tmpvar_21 = pow (max (0.0, dot (h_4, tmpvar_9)), (nspec_6.w * 128.0));
  spec_3 = tmpvar_21;
  highp float tmpvar_22;
  tmpvar_22 = (spec_3 * clamp (tmpvar_18, 0.0, 1.0));
  spec_3 = tmpvar_22;
  highp vec3 tmpvar_23;
  tmpvar_23 = (_LightColor.xyz * (tmpvar_19 * tmpvar_18));
  res_2.xyz = tmpvar_23;
  lowp vec3 c_24;
  c_24 = _LightColor.xyz;
  lowp float tmpvar_25;
  tmpvar_25 = dot (c_24, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_26;
  tmpvar_26 = (tmpvar_22 * tmpvar_25);
  res_2.w = tmpvar_26;
  highp float tmpvar_27;
  tmpvar_27 = clamp ((1.0 - (
    (mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w) * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_28;
  tmpvar_28 = (res_2 * tmpvar_27);
  res_2 = tmpvar_28;
  tmpvar_1 = tmpvar_28.wxyz;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_NONATIVE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform sampler2D _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize(tmpvar_16);
  lightDir_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_13;
  highp vec4 tmpvar_19;
  tmpvar_19 = (_LightMatrix0 * tmpvar_18);
  lowp float tmpvar_20;
  tmpvar_20 = texture2DProj (_LightTexture0, tmpvar_19).w;
  atten_5 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture2D (_LightTextureB0, vec2(tmpvar_21));
  atten_5 = ((atten_5 * float(
    (tmpvar_19.w < 0.0)
  )) * tmpvar_22.w);
  mediump float tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = clamp (((tmpvar_15 * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0);
  highp vec4 tmpvar_25;
  tmpvar_25.w = 1.0;
  tmpvar_25.xyz = tmpvar_13;
  highp vec4 tmpvar_26;
  tmpvar_26 = (unity_World2Shadow[0] * tmpvar_25);
  mediump float shadow_27;
  lowp vec4 tmpvar_28;
  tmpvar_28 = texture2DProj (_ShadowMapTexture, tmpvar_26);
  highp float tmpvar_29;
  if ((tmpvar_28.x < (tmpvar_26.z / tmpvar_26.w))) {
    tmpvar_29 = _LightShadowData.x;
  } else {
    tmpvar_29 = 1.0;
  };
  shadow_27 = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = clamp ((shadow_27 + tmpvar_24), 0.0, 1.0);
  tmpvar_23 = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = (atten_5 * tmpvar_23);
  atten_5 = tmpvar_31;
  mediump float tmpvar_32;
  tmpvar_32 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_33;
  tmpvar_33 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_33;
  mediump float tmpvar_34;
  tmpvar_34 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_34;
  highp float tmpvar_35;
  tmpvar_35 = (spec_3 * clamp (tmpvar_31, 0.0, 1.0));
  spec_3 = tmpvar_35;
  highp vec3 tmpvar_36;
  tmpvar_36 = (_LightColor.xyz * (tmpvar_32 * tmpvar_31));
  res_2.xyz = tmpvar_36;
  lowp vec3 c_37;
  c_37 = _LightColor.xyz;
  lowp float tmpvar_38;
  tmpvar_38 = dot (c_37, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_39;
  tmpvar_39 = (tmpvar_35 * tmpvar_38);
  res_2.w = tmpvar_39;
  highp float tmpvar_40;
  tmpvar_40 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_41;
  tmpvar_41 = (res_2 * tmpvar_40);
  res_2 = tmpvar_41;
  tmpvar_1 = tmpvar_41.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_NATIVE" }
"!!GLES


#ifdef VERTEX

#extension GL_EXT_shadow_samplers : enable
attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

#extension GL_EXT_shadow_samplers : enable
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp sampler2DShadow _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize(tmpvar_16);
  lightDir_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_13;
  highp vec4 tmpvar_19;
  tmpvar_19 = (_LightMatrix0 * tmpvar_18);
  lowp float tmpvar_20;
  tmpvar_20 = texture2DProj (_LightTexture0, tmpvar_19).w;
  atten_5 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture2D (_LightTextureB0, vec2(tmpvar_21));
  atten_5 = ((atten_5 * float(
    (tmpvar_19.w < 0.0)
  )) * tmpvar_22.w);
  mediump float tmpvar_23;
  highp vec4 tmpvar_24;
  tmpvar_24.w = 1.0;
  tmpvar_24.xyz = tmpvar_13;
  highp vec4 tmpvar_25;
  tmpvar_25 = (unity_World2Shadow[0] * tmpvar_24);
  mediump float shadow_26;
  lowp float tmpvar_27;
  tmpvar_27 = shadow2DProjEXT (_ShadowMapTexture, tmpvar_25);
  mediump float tmpvar_28;
  tmpvar_28 = tmpvar_27;
  highp float tmpvar_29;
  tmpvar_29 = (_LightShadowData.x + (tmpvar_28 * (1.0 - _LightShadowData.x)));
  shadow_26 = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = clamp ((shadow_26 + clamp (
    ((tmpvar_15 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_23 = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = (atten_5 * tmpvar_23);
  atten_5 = tmpvar_31;
  mediump float tmpvar_32;
  tmpvar_32 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_33;
  tmpvar_33 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_33;
  mediump float tmpvar_34;
  tmpvar_34 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_34;
  highp float tmpvar_35;
  tmpvar_35 = (spec_3 * clamp (tmpvar_31, 0.0, 1.0));
  spec_3 = tmpvar_35;
  highp vec3 tmpvar_36;
  tmpvar_36 = (_LightColor.xyz * (tmpvar_32 * tmpvar_31));
  res_2.xyz = tmpvar_36;
  lowp vec3 c_37;
  c_37 = _LightColor.xyz;
  lowp float tmpvar_38;
  tmpvar_38 = dot (c_37, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_39;
  tmpvar_39 = (tmpvar_35 * tmpvar_38);
  res_2.w = tmpvar_39;
  highp float tmpvar_40;
  tmpvar_40 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_41;
  tmpvar_41 = (res_2 * tmpvar_40);
  res_2 = tmpvar_41;
  tmpvar_1 = tmpvar_41.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_NATIVE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp sampler2DShadow _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize(tmpvar_16);
  lightDir_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_13;
  highp vec4 tmpvar_19;
  tmpvar_19 = (_LightMatrix0 * tmpvar_18);
  lowp float tmpvar_20;
  tmpvar_20 = textureProj (_LightTexture0, tmpvar_19).w;
  atten_5 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture (_LightTextureB0, vec2(tmpvar_21));
  atten_5 = ((atten_5 * float(
    (tmpvar_19.w < 0.0)
  )) * tmpvar_22.w);
  mediump float tmpvar_23;
  highp vec4 tmpvar_24;
  tmpvar_24.w = 1.0;
  tmpvar_24.xyz = tmpvar_13;
  highp vec4 tmpvar_25;
  tmpvar_25 = (unity_World2Shadow[0] * tmpvar_24);
  mediump float shadow_26;
  mediump float tmpvar_27;
  tmpvar_27 = textureProj (_ShadowMapTexture, tmpvar_25);
  highp float tmpvar_28;
  tmpvar_28 = (_LightShadowData.x + (tmpvar_27 * (1.0 - _LightShadowData.x)));
  shadow_26 = tmpvar_28;
  highp float tmpvar_29;
  tmpvar_29 = clamp ((shadow_26 + clamp (
    ((tmpvar_15 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_23 = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = (atten_5 * tmpvar_23);
  atten_5 = tmpvar_30;
  mediump float tmpvar_31;
  tmpvar_31 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_32;
  tmpvar_32 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_32;
  mediump float tmpvar_33;
  tmpvar_33 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_33;
  highp float tmpvar_34;
  tmpvar_34 = (spec_3 * clamp (tmpvar_30, 0.0, 1.0));
  spec_3 = tmpvar_34;
  highp vec3 tmpvar_35;
  tmpvar_35 = (_LightColor.xyz * (tmpvar_31 * tmpvar_30));
  res_2.xyz = tmpvar_35;
  lowp vec3 c_36;
  c_36 = _LightColor.xyz;
  lowp float tmpvar_37;
  tmpvar_37 = dot (c_36, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_38;
  tmpvar_38 = (tmpvar_34 * tmpvar_37);
  res_2.w = tmpvar_38;
  highp float tmpvar_39;
  tmpvar_39 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_40;
  tmpvar_40 = (res_2 * tmpvar_39);
  res_2 = tmpvar_40;
  tmpvar_1 = tmpvar_40.wxyz;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec3 normal_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  normal_6 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_14;
  tmpvar_14 = mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_15;
  tmpvar_15 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_15;
  mediump float tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture2D (_ShadowMapTexture, tmpvar_8);
  highp float tmpvar_18;
  tmpvar_18 = clamp ((tmpvar_17.x + clamp (
    ((tmpvar_14 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_16 = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = tmpvar_16;
  mediump float tmpvar_20;
  tmpvar_20 = max (0.0, dot (lightDir_5, normal_6));
  highp vec3 tmpvar_21;
  tmpvar_21 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_21;
  mediump float tmpvar_22;
  tmpvar_22 = pow (max (0.0, dot (h_4, normal_6)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = (spec_3 * clamp (tmpvar_19, 0.0, 1.0));
  spec_3 = tmpvar_23;
  highp vec3 tmpvar_24;
  tmpvar_24 = (_LightColor.xyz * (tmpvar_20 * tmpvar_19));
  res_2.xyz = tmpvar_24;
  lowp vec3 c_25;
  c_25 = _LightColor.xyz;
  lowp float tmpvar_26;
  tmpvar_26 = dot (c_25, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_27;
  tmpvar_27 = (tmpvar_23 * tmpvar_26);
  res_2.w = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = clamp ((1.0 - (
    (tmpvar_14 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_29;
  tmpvar_29 = (res_2 * tmpvar_28);
  res_2 = tmpvar_29;
  tmpvar_1 = tmpvar_29.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTexture0;
uniform sampler2D _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec3 normal_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  normal_6 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_14;
  tmpvar_14 = mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_15;
  tmpvar_15 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_15;
  mediump float tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture2D (_ShadowMapTexture, tmpvar_8);
  highp float tmpvar_18;
  tmpvar_18 = clamp ((tmpvar_17.x + clamp (
    ((tmpvar_14 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_16 = tmpvar_18;
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = tmpvar_12;
  lowp vec4 tmpvar_20;
  highp vec2 P_21;
  P_21 = (_LightMatrix0 * tmpvar_19).xy;
  tmpvar_20 = texture2D (_LightTexture0, P_21);
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_16 * tmpvar_20.w);
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_5, normal_6));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, normal_6)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (tmpvar_14 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  tmpvar_1 = tmpvar_32.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture2D (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  mediump float tmpvar_21;
  highp vec4 packDist_22;
  lowp vec4 tmpvar_23;
  tmpvar_23 = textureCube (_ShadowMapTexture, tmpvar_16);
  packDist_22 = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (packDist_22, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp float tmpvar_25;
  if ((tmpvar_24 < tmpvar_20)) {
    tmpvar_25 = _LightShadowData.x;
  } else {
    tmpvar_25 = 1.0;
  };
  tmpvar_21 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (atten_5 * tmpvar_21);
  atten_5 = tmpvar_26;
  mediump float tmpvar_27;
  tmpvar_27 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_28;
  tmpvar_28 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_28;
  mediump float tmpvar_29;
  tmpvar_29 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = (spec_3 * clamp (tmpvar_26, 0.0, 1.0));
  spec_3 = tmpvar_30;
  highp vec3 tmpvar_31;
  tmpvar_31 = (_LightColor.xyz * (tmpvar_27 * tmpvar_26));
  res_2.xyz = tmpvar_31;
  lowp vec3 c_32;
  c_32 = _LightColor.xyz;
  lowp float tmpvar_33;
  tmpvar_33 = dot (c_32, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_34;
  tmpvar_34 = (tmpvar_30 * tmpvar_33);
  res_2.w = tmpvar_34;
  highp float tmpvar_35;
  tmpvar_35 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_36;
  tmpvar_36 = (res_2 * tmpvar_35);
  res_2 = tmpvar_36;
  tmpvar_1 = tmpvar_36.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT" "SHADOWS_CUBE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  mediump float tmpvar_21;
  highp vec4 packDist_22;
  lowp vec4 tmpvar_23;
  tmpvar_23 = texture (_ShadowMapTexture, tmpvar_16);
  packDist_22 = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (packDist_22, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp float tmpvar_25;
  if ((tmpvar_24 < tmpvar_20)) {
    tmpvar_25 = _LightShadowData.x;
  } else {
    tmpvar_25 = 1.0;
  };
  tmpvar_21 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (atten_5 * tmpvar_21);
  atten_5 = tmpvar_26;
  mediump float tmpvar_27;
  tmpvar_27 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_28;
  tmpvar_28 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_28;
  mediump float tmpvar_29;
  tmpvar_29 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_29;
  highp float tmpvar_30;
  tmpvar_30 = (spec_3 * clamp (tmpvar_26, 0.0, 1.0));
  spec_3 = tmpvar_30;
  highp vec3 tmpvar_31;
  tmpvar_31 = (_LightColor.xyz * (tmpvar_27 * tmpvar_26));
  res_2.xyz = tmpvar_31;
  lowp vec3 c_32;
  c_32 = _LightColor.xyz;
  lowp float tmpvar_33;
  tmpvar_33 = dot (c_32, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_34;
  tmpvar_34 = (tmpvar_30 * tmpvar_33);
  res_2.w = tmpvar_34;
  highp float tmpvar_35;
  tmpvar_35 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_36;
  tmpvar_36 = (res_2 * tmpvar_35);
  res_2 = tmpvar_36;
  tmpvar_1 = tmpvar_36.wxyz;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _LightTexture0;
uniform lowp samplerCube _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture2D (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  mediump float tmpvar_21;
  highp vec4 packDist_22;
  lowp vec4 tmpvar_23;
  tmpvar_23 = textureCube (_ShadowMapTexture, tmpvar_16);
  packDist_22 = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (packDist_22, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp float tmpvar_25;
  if ((tmpvar_24 < tmpvar_20)) {
    tmpvar_25 = _LightShadowData.x;
  } else {
    tmpvar_25 = 1.0;
  };
  tmpvar_21 = tmpvar_25;
  highp vec4 tmpvar_26;
  tmpvar_26.w = 1.0;
  tmpvar_26.xyz = tmpvar_13;
  lowp vec4 tmpvar_27;
  highp vec3 P_28;
  P_28 = (_LightMatrix0 * tmpvar_26).xyz;
  tmpvar_27 = textureCube (_LightTexture0, P_28);
  highp float tmpvar_29;
  tmpvar_29 = ((atten_5 * tmpvar_21) * tmpvar_27.w);
  atten_5 = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_31;
  tmpvar_31 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_31;
  mediump float tmpvar_32;
  tmpvar_32 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_32;
  highp float tmpvar_33;
  tmpvar_33 = (spec_3 * clamp (tmpvar_29, 0.0, 1.0));
  spec_3 = tmpvar_33;
  highp vec3 tmpvar_34;
  tmpvar_34 = (_LightColor.xyz * (tmpvar_30 * tmpvar_29));
  res_2.xyz = tmpvar_34;
  lowp vec3 c_35;
  c_35 = _LightColor.xyz;
  lowp float tmpvar_36;
  tmpvar_36 = dot (c_35, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_37;
  tmpvar_37 = (tmpvar_33 * tmpvar_36);
  res_2.w = tmpvar_37;
  highp float tmpvar_38;
  tmpvar_38 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_39;
  tmpvar_39 = (res_2 * tmpvar_38);
  res_2 = tmpvar_39;
  tmpvar_1 = tmpvar_39.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _LightTexture0;
uniform lowp samplerCube _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  mediump float tmpvar_21;
  highp vec4 packDist_22;
  lowp vec4 tmpvar_23;
  tmpvar_23 = texture (_ShadowMapTexture, tmpvar_16);
  packDist_22 = tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = dot (packDist_22, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp float tmpvar_25;
  if ((tmpvar_24 < tmpvar_20)) {
    tmpvar_25 = _LightShadowData.x;
  } else {
    tmpvar_25 = 1.0;
  };
  tmpvar_21 = tmpvar_25;
  highp vec4 tmpvar_26;
  tmpvar_26.w = 1.0;
  tmpvar_26.xyz = tmpvar_13;
  lowp vec4 tmpvar_27;
  highp vec3 P_28;
  P_28 = (_LightMatrix0 * tmpvar_26).xyz;
  tmpvar_27 = texture (_LightTexture0, P_28);
  highp float tmpvar_29;
  tmpvar_29 = ((atten_5 * tmpvar_21) * tmpvar_27.w);
  atten_5 = tmpvar_29;
  mediump float tmpvar_30;
  tmpvar_30 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_31;
  tmpvar_31 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_31;
  mediump float tmpvar_32;
  tmpvar_32 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_32;
  highp float tmpvar_33;
  tmpvar_33 = (spec_3 * clamp (tmpvar_29, 0.0, 1.0));
  spec_3 = tmpvar_33;
  highp vec3 tmpvar_34;
  tmpvar_34 = (_LightColor.xyz * (tmpvar_30 * tmpvar_29));
  res_2.xyz = tmpvar_34;
  lowp vec3 c_35;
  c_35 = _LightColor.xyz;
  lowp float tmpvar_36;
  tmpvar_36 = dot (c_35, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_37;
  tmpvar_37 = (tmpvar_33 * tmpvar_36);
  res_2.w = tmpvar_37;
  highp float tmpvar_38;
  tmpvar_38 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_39;
  tmpvar_39 = (res_2 * tmpvar_38);
  res_2 = tmpvar_39;
  tmpvar_1 = tmpvar_39.wxyz;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SHADOWS_NONATIVE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform sampler2D _ShadowMapTexture;
uniform highp vec4 _ShadowOffsets[4];
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize(tmpvar_16);
  lightDir_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_13;
  highp vec4 tmpvar_19;
  tmpvar_19 = (_LightMatrix0 * tmpvar_18);
  lowp float tmpvar_20;
  tmpvar_20 = texture2DProj (_LightTexture0, tmpvar_19).w;
  atten_5 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture2D (_LightTextureB0, vec2(tmpvar_21));
  atten_5 = ((atten_5 * float(
    (tmpvar_19.w < 0.0)
  )) * tmpvar_22.w);
  mediump float tmpvar_23;
  highp float tmpvar_24;
  tmpvar_24 = clamp (((tmpvar_15 * _LightShadowData.z) + _LightShadowData.w), 0.0, 1.0);
  highp vec4 tmpvar_25;
  tmpvar_25.w = 1.0;
  tmpvar_25.xyz = tmpvar_13;
  highp vec4 tmpvar_26;
  tmpvar_26 = (unity_World2Shadow[0] * tmpvar_25);
  highp vec4 shadowVals_27;
  highp vec3 tmpvar_28;
  tmpvar_28 = (tmpvar_26.xyz / tmpvar_26.w);
  highp vec2 P_29;
  P_29 = (tmpvar_28.xy + _ShadowOffsets[0].xy);
  lowp float tmpvar_30;
  tmpvar_30 = texture2D (_ShadowMapTexture, P_29).x;
  shadowVals_27.x = tmpvar_30;
  highp vec2 P_31;
  P_31 = (tmpvar_28.xy + _ShadowOffsets[1].xy);
  lowp float tmpvar_32;
  tmpvar_32 = texture2D (_ShadowMapTexture, P_31).x;
  shadowVals_27.y = tmpvar_32;
  highp vec2 P_33;
  P_33 = (tmpvar_28.xy + _ShadowOffsets[2].xy);
  lowp float tmpvar_34;
  tmpvar_34 = texture2D (_ShadowMapTexture, P_33).x;
  shadowVals_27.z = tmpvar_34;
  highp vec2 P_35;
  P_35 = (tmpvar_28.xy + _ShadowOffsets[3].xy);
  lowp float tmpvar_36;
  tmpvar_36 = texture2D (_ShadowMapTexture, P_35).x;
  shadowVals_27.w = tmpvar_36;
  bvec4 tmpvar_37;
  tmpvar_37 = lessThan (shadowVals_27, tmpvar_28.zzzz);
  highp vec4 tmpvar_38;
  tmpvar_38 = _LightShadowData.xxxx;
  highp float tmpvar_39;
  if (tmpvar_37.x) {
    tmpvar_39 = tmpvar_38.x;
  } else {
    tmpvar_39 = 1.0;
  };
  highp float tmpvar_40;
  if (tmpvar_37.y) {
    tmpvar_40 = tmpvar_38.y;
  } else {
    tmpvar_40 = 1.0;
  };
  highp float tmpvar_41;
  if (tmpvar_37.z) {
    tmpvar_41 = tmpvar_38.z;
  } else {
    tmpvar_41 = 1.0;
  };
  highp float tmpvar_42;
  if (tmpvar_37.w) {
    tmpvar_42 = tmpvar_38.w;
  } else {
    tmpvar_42 = 1.0;
  };
  mediump vec4 tmpvar_43;
  tmpvar_43.x = tmpvar_39;
  tmpvar_43.y = tmpvar_40;
  tmpvar_43.z = tmpvar_41;
  tmpvar_43.w = tmpvar_42;
  mediump float tmpvar_44;
  tmpvar_44 = dot (tmpvar_43, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_45;
  tmpvar_45 = clamp ((tmpvar_44 + tmpvar_24), 0.0, 1.0);
  tmpvar_23 = tmpvar_45;
  highp float tmpvar_46;
  tmpvar_46 = (atten_5 * tmpvar_23);
  atten_5 = tmpvar_46;
  mediump float tmpvar_47;
  tmpvar_47 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_48;
  tmpvar_48 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_48;
  mediump float tmpvar_49;
  tmpvar_49 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_49;
  highp float tmpvar_50;
  tmpvar_50 = (spec_3 * clamp (tmpvar_46, 0.0, 1.0));
  spec_3 = tmpvar_50;
  highp vec3 tmpvar_51;
  tmpvar_51 = (_LightColor.xyz * (tmpvar_47 * tmpvar_46));
  res_2.xyz = tmpvar_51;
  lowp vec3 c_52;
  c_52 = _LightColor.xyz;
  lowp float tmpvar_53;
  tmpvar_53 = dot (c_52, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_54;
  tmpvar_54 = (tmpvar_50 * tmpvar_53);
  res_2.w = tmpvar_54;
  highp float tmpvar_55;
  tmpvar_55 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_56;
  tmpvar_56 = (res_2 * tmpvar_55);
  res_2 = tmpvar_56;
  tmpvar_1 = tmpvar_56.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SHADOWS_NATIVE" }
"!!GLES


#ifdef VERTEX

#extension GL_EXT_shadow_samplers : enable
attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

#extension GL_EXT_shadow_samplers : enable
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp sampler2DShadow _ShadowMapTexture;
uniform highp vec4 _ShadowOffsets[4];
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize(tmpvar_16);
  lightDir_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_13;
  highp vec4 tmpvar_19;
  tmpvar_19 = (_LightMatrix0 * tmpvar_18);
  lowp float tmpvar_20;
  tmpvar_20 = texture2DProj (_LightTexture0, tmpvar_19).w;
  atten_5 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture2D (_LightTextureB0, vec2(tmpvar_21));
  atten_5 = ((atten_5 * float(
    (tmpvar_19.w < 0.0)
  )) * tmpvar_22.w);
  mediump float tmpvar_23;
  highp vec4 tmpvar_24;
  tmpvar_24.w = 1.0;
  tmpvar_24.xyz = tmpvar_13;
  highp vec4 tmpvar_25;
  tmpvar_25 = (unity_World2Shadow[0] * tmpvar_24);
  mediump vec4 shadows_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (tmpvar_25.xyz / tmpvar_25.w);
  highp vec3 coord_28;
  coord_28 = (tmpvar_27 + _ShadowOffsets[0].xyz);
  lowp float tmpvar_29;
  tmpvar_29 = shadow2DEXT (_ShadowMapTexture, coord_28);
  shadows_26.x = tmpvar_29;
  highp vec3 coord_30;
  coord_30 = (tmpvar_27 + _ShadowOffsets[1].xyz);
  lowp float tmpvar_31;
  tmpvar_31 = shadow2DEXT (_ShadowMapTexture, coord_30);
  shadows_26.y = tmpvar_31;
  highp vec3 coord_32;
  coord_32 = (tmpvar_27 + _ShadowOffsets[2].xyz);
  lowp float tmpvar_33;
  tmpvar_33 = shadow2DEXT (_ShadowMapTexture, coord_32);
  shadows_26.z = tmpvar_33;
  highp vec3 coord_34;
  coord_34 = (tmpvar_27 + _ShadowOffsets[3].xyz);
  lowp float tmpvar_35;
  tmpvar_35 = shadow2DEXT (_ShadowMapTexture, coord_34);
  shadows_26.w = tmpvar_35;
  highp vec4 tmpvar_36;
  tmpvar_36 = (_LightShadowData.xxxx + (shadows_26 * (1.0 - _LightShadowData.xxxx)));
  shadows_26 = tmpvar_36;
  mediump float tmpvar_37;
  tmpvar_37 = dot (shadows_26, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_38;
  tmpvar_38 = clamp ((tmpvar_37 + clamp (
    ((tmpvar_15 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_23 = tmpvar_38;
  highp float tmpvar_39;
  tmpvar_39 = (atten_5 * tmpvar_23);
  atten_5 = tmpvar_39;
  mediump float tmpvar_40;
  tmpvar_40 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_41;
  tmpvar_41 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_41;
  mediump float tmpvar_42;
  tmpvar_42 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_42;
  highp float tmpvar_43;
  tmpvar_43 = (spec_3 * clamp (tmpvar_39, 0.0, 1.0));
  spec_3 = tmpvar_43;
  highp vec3 tmpvar_44;
  tmpvar_44 = (_LightColor.xyz * (tmpvar_40 * tmpvar_39));
  res_2.xyz = tmpvar_44;
  lowp vec3 c_45;
  c_45 = _LightColor.xyz;
  lowp float tmpvar_46;
  tmpvar_46 = dot (c_45, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_47;
  tmpvar_47 = (tmpvar_43 * tmpvar_46);
  res_2.w = tmpvar_47;
  highp float tmpvar_48;
  tmpvar_48 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_49;
  tmpvar_49 = (res_2 * tmpvar_48);
  res_2 = tmpvar_49;
  tmpvar_1 = tmpvar_49.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SHADOWS_NATIVE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp mat4 unity_World2Shadow[4];
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform sampler2D _LightTexture0;
uniform lowp sampler2DShadow _ShadowMapTexture;
uniform highp vec4 _ShadowOffsets[4];
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (_LightPos.xyz - tmpvar_13);
  highp vec3 tmpvar_17;
  tmpvar_17 = normalize(tmpvar_16);
  lightDir_6 = tmpvar_17;
  highp vec4 tmpvar_18;
  tmpvar_18.w = 1.0;
  tmpvar_18.xyz = tmpvar_13;
  highp vec4 tmpvar_19;
  tmpvar_19 = (_LightMatrix0 * tmpvar_18);
  lowp float tmpvar_20;
  tmpvar_20 = textureProj (_LightTexture0, tmpvar_19).w;
  atten_5 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp vec4 tmpvar_22;
  tmpvar_22 = texture (_LightTextureB0, vec2(tmpvar_21));
  atten_5 = ((atten_5 * float(
    (tmpvar_19.w < 0.0)
  )) * tmpvar_22.w);
  mediump float tmpvar_23;
  highp vec4 tmpvar_24;
  tmpvar_24.w = 1.0;
  tmpvar_24.xyz = tmpvar_13;
  highp vec4 tmpvar_25;
  tmpvar_25 = (unity_World2Shadow[0] * tmpvar_24);
  mediump vec4 shadows_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (tmpvar_25.xyz / tmpvar_25.w);
  highp vec3 coord_28;
  coord_28 = (tmpvar_27 + _ShadowOffsets[0].xyz);
  mediump float tmpvar_29;
  tmpvar_29 = texture (_ShadowMapTexture, coord_28);
  shadows_26.x = tmpvar_29;
  highp vec3 coord_30;
  coord_30 = (tmpvar_27 + _ShadowOffsets[1].xyz);
  mediump float tmpvar_31;
  tmpvar_31 = texture (_ShadowMapTexture, coord_30);
  shadows_26.y = tmpvar_31;
  highp vec3 coord_32;
  coord_32 = (tmpvar_27 + _ShadowOffsets[2].xyz);
  mediump float tmpvar_33;
  tmpvar_33 = texture (_ShadowMapTexture, coord_32);
  shadows_26.z = tmpvar_33;
  highp vec3 coord_34;
  coord_34 = (tmpvar_27 + _ShadowOffsets[3].xyz);
  mediump float tmpvar_35;
  tmpvar_35 = texture (_ShadowMapTexture, coord_34);
  shadows_26.w = tmpvar_35;
  highp vec4 tmpvar_36;
  tmpvar_36 = (_LightShadowData.xxxx + (shadows_26 * (1.0 - _LightShadowData.xxxx)));
  shadows_26 = tmpvar_36;
  mediump float tmpvar_37;
  tmpvar_37 = dot (shadows_26, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_38;
  tmpvar_38 = clamp ((tmpvar_37 + clamp (
    ((tmpvar_15 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_23 = tmpvar_38;
  highp float tmpvar_39;
  tmpvar_39 = (atten_5 * tmpvar_23);
  atten_5 = tmpvar_39;
  mediump float tmpvar_40;
  tmpvar_40 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_41;
  tmpvar_41 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_41;
  mediump float tmpvar_42;
  tmpvar_42 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_42;
  highp float tmpvar_43;
  tmpvar_43 = (spec_3 * clamp (tmpvar_39, 0.0, 1.0));
  spec_3 = tmpvar_43;
  highp vec3 tmpvar_44;
  tmpvar_44 = (_LightColor.xyz * (tmpvar_40 * tmpvar_39));
  res_2.xyz = tmpvar_44;
  lowp vec3 c_45;
  c_45 = _LightColor.xyz;
  lowp float tmpvar_46;
  tmpvar_46 = dot (c_45, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_47;
  tmpvar_47 = (tmpvar_43 * tmpvar_46);
  res_2.w = tmpvar_47;
  highp float tmpvar_48;
  tmpvar_48 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_49;
  tmpvar_49 = (res_2 * tmpvar_48);
  res_2 = tmpvar_49;
  tmpvar_1 = tmpvar_49.wxyz;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture2D (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  highp vec4 shadowVals_21;
  highp vec3 vec_22;
  vec_22 = (tmpvar_16 + vec3(0.0078125, 0.0078125, 0.0078125));
  highp vec4 packDist_23;
  lowp vec4 tmpvar_24;
  tmpvar_24 = textureCube (_ShadowMapTexture, vec_22);
  packDist_23 = tmpvar_24;
  shadowVals_21.x = dot (packDist_23, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_25;
  vec_25 = (tmpvar_16 + vec3(-0.0078125, -0.0078125, 0.0078125));
  highp vec4 packDist_26;
  lowp vec4 tmpvar_27;
  tmpvar_27 = textureCube (_ShadowMapTexture, vec_25);
  packDist_26 = tmpvar_27;
  shadowVals_21.y = dot (packDist_26, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_28;
  vec_28 = (tmpvar_16 + vec3(-0.0078125, 0.0078125, -0.0078125));
  highp vec4 packDist_29;
  lowp vec4 tmpvar_30;
  tmpvar_30 = textureCube (_ShadowMapTexture, vec_28);
  packDist_29 = tmpvar_30;
  shadowVals_21.z = dot (packDist_29, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_31;
  vec_31 = (tmpvar_16 + vec3(0.0078125, -0.0078125, -0.0078125));
  highp vec4 packDist_32;
  lowp vec4 tmpvar_33;
  tmpvar_33 = textureCube (_ShadowMapTexture, vec_31);
  packDist_32 = tmpvar_33;
  shadowVals_21.w = dot (packDist_32, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  bvec4 tmpvar_34;
  tmpvar_34 = lessThan (shadowVals_21, vec4(tmpvar_20));
  highp vec4 tmpvar_35;
  tmpvar_35 = _LightShadowData.xxxx;
  highp float tmpvar_36;
  if (tmpvar_34.x) {
    tmpvar_36 = tmpvar_35.x;
  } else {
    tmpvar_36 = 1.0;
  };
  highp float tmpvar_37;
  if (tmpvar_34.y) {
    tmpvar_37 = tmpvar_35.y;
  } else {
    tmpvar_37 = 1.0;
  };
  highp float tmpvar_38;
  if (tmpvar_34.z) {
    tmpvar_38 = tmpvar_35.z;
  } else {
    tmpvar_38 = 1.0;
  };
  highp float tmpvar_39;
  if (tmpvar_34.w) {
    tmpvar_39 = tmpvar_35.w;
  } else {
    tmpvar_39 = 1.0;
  };
  mediump vec4 tmpvar_40;
  tmpvar_40.x = tmpvar_36;
  tmpvar_40.y = tmpvar_37;
  tmpvar_40.z = tmpvar_38;
  tmpvar_40.w = tmpvar_39;
  mediump float tmpvar_41;
  tmpvar_41 = dot (tmpvar_40, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_42;
  tmpvar_42 = (atten_5 * tmpvar_41);
  atten_5 = tmpvar_42;
  mediump float tmpvar_43;
  tmpvar_43 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_44;
  tmpvar_44 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_44;
  mediump float tmpvar_45;
  tmpvar_45 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_45;
  highp float tmpvar_46;
  tmpvar_46 = (spec_3 * clamp (tmpvar_42, 0.0, 1.0));
  spec_3 = tmpvar_46;
  highp vec3 tmpvar_47;
  tmpvar_47 = (_LightColor.xyz * (tmpvar_43 * tmpvar_42));
  res_2.xyz = tmpvar_47;
  lowp vec3 c_48;
  c_48 = _LightColor.xyz;
  lowp float tmpvar_49;
  tmpvar_49 = dot (c_48, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_50;
  tmpvar_50 = (tmpvar_46 * tmpvar_49);
  res_2.w = tmpvar_50;
  highp float tmpvar_51;
  tmpvar_51 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_52;
  tmpvar_52 = (res_2 * tmpvar_51);
  res_2 = tmpvar_52;
  tmpvar_1 = tmpvar_52.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  highp vec4 shadowVals_21;
  highp vec3 vec_22;
  vec_22 = (tmpvar_16 + vec3(0.0078125, 0.0078125, 0.0078125));
  highp vec4 packDist_23;
  lowp vec4 tmpvar_24;
  tmpvar_24 = texture (_ShadowMapTexture, vec_22);
  packDist_23 = tmpvar_24;
  shadowVals_21.x = dot (packDist_23, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_25;
  vec_25 = (tmpvar_16 + vec3(-0.0078125, -0.0078125, 0.0078125));
  highp vec4 packDist_26;
  lowp vec4 tmpvar_27;
  tmpvar_27 = texture (_ShadowMapTexture, vec_25);
  packDist_26 = tmpvar_27;
  shadowVals_21.y = dot (packDist_26, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_28;
  vec_28 = (tmpvar_16 + vec3(-0.0078125, 0.0078125, -0.0078125));
  highp vec4 packDist_29;
  lowp vec4 tmpvar_30;
  tmpvar_30 = texture (_ShadowMapTexture, vec_28);
  packDist_29 = tmpvar_30;
  shadowVals_21.z = dot (packDist_29, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_31;
  vec_31 = (tmpvar_16 + vec3(0.0078125, -0.0078125, -0.0078125));
  highp vec4 packDist_32;
  lowp vec4 tmpvar_33;
  tmpvar_33 = texture (_ShadowMapTexture, vec_31);
  packDist_32 = tmpvar_33;
  shadowVals_21.w = dot (packDist_32, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  bvec4 tmpvar_34;
  tmpvar_34 = lessThan (shadowVals_21, vec4(tmpvar_20));
  highp vec4 tmpvar_35;
  tmpvar_35 = _LightShadowData.xxxx;
  highp float tmpvar_36;
  if (tmpvar_34.x) {
    tmpvar_36 = tmpvar_35.x;
  } else {
    tmpvar_36 = 1.0;
  };
  highp float tmpvar_37;
  if (tmpvar_34.y) {
    tmpvar_37 = tmpvar_35.y;
  } else {
    tmpvar_37 = 1.0;
  };
  highp float tmpvar_38;
  if (tmpvar_34.z) {
    tmpvar_38 = tmpvar_35.z;
  } else {
    tmpvar_38 = 1.0;
  };
  highp float tmpvar_39;
  if (tmpvar_34.w) {
    tmpvar_39 = tmpvar_35.w;
  } else {
    tmpvar_39 = 1.0;
  };
  mediump vec4 tmpvar_40;
  tmpvar_40.x = tmpvar_36;
  tmpvar_40.y = tmpvar_37;
  tmpvar_40.z = tmpvar_38;
  tmpvar_40.w = tmpvar_39;
  mediump float tmpvar_41;
  tmpvar_41 = dot (tmpvar_40, vec4(0.25, 0.25, 0.25, 0.25));
  highp float tmpvar_42;
  tmpvar_42 = (atten_5 * tmpvar_41);
  atten_5 = tmpvar_42;
  mediump float tmpvar_43;
  tmpvar_43 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_44;
  tmpvar_44 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_44;
  mediump float tmpvar_45;
  tmpvar_45 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_45;
  highp float tmpvar_46;
  tmpvar_46 = (spec_3 * clamp (tmpvar_42, 0.0, 1.0));
  spec_3 = tmpvar_46;
  highp vec3 tmpvar_47;
  tmpvar_47 = (_LightColor.xyz * (tmpvar_43 * tmpvar_42));
  res_2.xyz = tmpvar_47;
  lowp vec3 c_48;
  c_48 = _LightColor.xyz;
  lowp float tmpvar_49;
  tmpvar_49 = dot (c_48, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_50;
  tmpvar_50 = (tmpvar_46 * tmpvar_49);
  res_2.w = tmpvar_50;
  highp float tmpvar_51;
  tmpvar_51 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_52;
  tmpvar_52 = (res_2 * tmpvar_51);
  res_2 = tmpvar_52;
  tmpvar_1 = tmpvar_52.wxyz;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _LightTexture0;
uniform lowp samplerCube _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture2D (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture2D (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  highp vec4 shadowVals_21;
  highp vec3 vec_22;
  vec_22 = (tmpvar_16 + vec3(0.0078125, 0.0078125, 0.0078125));
  highp vec4 packDist_23;
  lowp vec4 tmpvar_24;
  tmpvar_24 = textureCube (_ShadowMapTexture, vec_22);
  packDist_23 = tmpvar_24;
  shadowVals_21.x = dot (packDist_23, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_25;
  vec_25 = (tmpvar_16 + vec3(-0.0078125, -0.0078125, 0.0078125));
  highp vec4 packDist_26;
  lowp vec4 tmpvar_27;
  tmpvar_27 = textureCube (_ShadowMapTexture, vec_25);
  packDist_26 = tmpvar_27;
  shadowVals_21.y = dot (packDist_26, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_28;
  vec_28 = (tmpvar_16 + vec3(-0.0078125, 0.0078125, -0.0078125));
  highp vec4 packDist_29;
  lowp vec4 tmpvar_30;
  tmpvar_30 = textureCube (_ShadowMapTexture, vec_28);
  packDist_29 = tmpvar_30;
  shadowVals_21.z = dot (packDist_29, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_31;
  vec_31 = (tmpvar_16 + vec3(0.0078125, -0.0078125, -0.0078125));
  highp vec4 packDist_32;
  lowp vec4 tmpvar_33;
  tmpvar_33 = textureCube (_ShadowMapTexture, vec_31);
  packDist_32 = tmpvar_33;
  shadowVals_21.w = dot (packDist_32, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  bvec4 tmpvar_34;
  tmpvar_34 = lessThan (shadowVals_21, vec4(tmpvar_20));
  highp vec4 tmpvar_35;
  tmpvar_35 = _LightShadowData.xxxx;
  highp float tmpvar_36;
  if (tmpvar_34.x) {
    tmpvar_36 = tmpvar_35.x;
  } else {
    tmpvar_36 = 1.0;
  };
  highp float tmpvar_37;
  if (tmpvar_34.y) {
    tmpvar_37 = tmpvar_35.y;
  } else {
    tmpvar_37 = 1.0;
  };
  highp float tmpvar_38;
  if (tmpvar_34.z) {
    tmpvar_38 = tmpvar_35.z;
  } else {
    tmpvar_38 = 1.0;
  };
  highp float tmpvar_39;
  if (tmpvar_34.w) {
    tmpvar_39 = tmpvar_35.w;
  } else {
    tmpvar_39 = 1.0;
  };
  mediump vec4 tmpvar_40;
  tmpvar_40.x = tmpvar_36;
  tmpvar_40.y = tmpvar_37;
  tmpvar_40.z = tmpvar_38;
  tmpvar_40.w = tmpvar_39;
  mediump float tmpvar_41;
  tmpvar_41 = dot (tmpvar_40, vec4(0.25, 0.25, 0.25, 0.25));
  highp vec4 tmpvar_42;
  tmpvar_42.w = 1.0;
  tmpvar_42.xyz = tmpvar_13;
  lowp vec4 tmpvar_43;
  highp vec3 P_44;
  P_44 = (_LightMatrix0 * tmpvar_42).xyz;
  tmpvar_43 = textureCube (_LightTexture0, P_44);
  highp float tmpvar_45;
  tmpvar_45 = ((atten_5 * tmpvar_41) * tmpvar_43.w);
  atten_5 = tmpvar_45;
  mediump float tmpvar_46;
  tmpvar_46 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_47;
  tmpvar_47 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_47;
  mediump float tmpvar_48;
  tmpvar_48 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_48;
  highp float tmpvar_49;
  tmpvar_49 = (spec_3 * clamp (tmpvar_45, 0.0, 1.0));
  spec_3 = tmpvar_49;
  highp vec3 tmpvar_50;
  tmpvar_50 = (_LightColor.xyz * (tmpvar_46 * tmpvar_45));
  res_2.xyz = tmpvar_50;
  lowp vec3 c_51;
  c_51 = _LightColor.xyz;
  lowp float tmpvar_52;
  tmpvar_52 = dot (c_51, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_53;
  tmpvar_53 = (tmpvar_49 * tmpvar_52);
  res_2.w = tmpvar_53;
  highp float tmpvar_54;
  tmpvar_54 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_55;
  tmpvar_55 = (res_2 * tmpvar_54);
  res_2 = tmpvar_55;
  tmpvar_1 = tmpvar_55.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightPositionRange;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightPos;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTextureB0;
uniform lowp samplerCube _LightTexture0;
uniform lowp samplerCube _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  highp float atten_5;
  mediump vec3 lightDir_6;
  mediump vec3 normal_7;
  mediump vec4 nspec_8;
  highp vec2 tmpvar_9;
  tmpvar_9 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraNormalsTexture, tmpvar_9);
  nspec_8 = tmpvar_10;
  normal_7 = normalize(((nspec_8.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_11;
  tmpvar_11 = texture (_CameraDepthTexture, tmpvar_9);
  highp vec4 tmpvar_12;
  tmpvar_12.w = 1.0;
  tmpvar_12.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_11.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_13;
  tmpvar_13 = (_CameraToWorld * tmpvar_12).xyz;
  highp vec3 tmpvar_14;
  tmpvar_14 = (tmpvar_13 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_15;
  tmpvar_15 = mix (tmpvar_12.z, sqrt(dot (tmpvar_14, tmpvar_14)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_16;
  tmpvar_16 = (tmpvar_13 - _LightPos.xyz);
  highp vec3 tmpvar_17;
  tmpvar_17 = -(normalize(tmpvar_16));
  lightDir_6 = tmpvar_17;
  highp float tmpvar_18;
  tmpvar_18 = (dot (tmpvar_16, tmpvar_16) * _LightPos.w);
  lowp float tmpvar_19;
  tmpvar_19 = texture (_LightTextureB0, vec2(tmpvar_18)).w;
  atten_5 = tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = ((sqrt(
    dot (tmpvar_16, tmpvar_16)
  ) * _LightPositionRange.w) * 0.97);
  highp vec4 shadowVals_21;
  highp vec3 vec_22;
  vec_22 = (tmpvar_16 + vec3(0.0078125, 0.0078125, 0.0078125));
  highp vec4 packDist_23;
  lowp vec4 tmpvar_24;
  tmpvar_24 = texture (_ShadowMapTexture, vec_22);
  packDist_23 = tmpvar_24;
  shadowVals_21.x = dot (packDist_23, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_25;
  vec_25 = (tmpvar_16 + vec3(-0.0078125, -0.0078125, 0.0078125));
  highp vec4 packDist_26;
  lowp vec4 tmpvar_27;
  tmpvar_27 = texture (_ShadowMapTexture, vec_25);
  packDist_26 = tmpvar_27;
  shadowVals_21.y = dot (packDist_26, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_28;
  vec_28 = (tmpvar_16 + vec3(-0.0078125, 0.0078125, -0.0078125));
  highp vec4 packDist_29;
  lowp vec4 tmpvar_30;
  tmpvar_30 = texture (_ShadowMapTexture, vec_28);
  packDist_29 = tmpvar_30;
  shadowVals_21.z = dot (packDist_29, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  highp vec3 vec_31;
  vec_31 = (tmpvar_16 + vec3(0.0078125, -0.0078125, -0.0078125));
  highp vec4 packDist_32;
  lowp vec4 tmpvar_33;
  tmpvar_33 = texture (_ShadowMapTexture, vec_31);
  packDist_32 = tmpvar_33;
  shadowVals_21.w = dot (packDist_32, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08));
  bvec4 tmpvar_34;
  tmpvar_34 = lessThan (shadowVals_21, vec4(tmpvar_20));
  highp vec4 tmpvar_35;
  tmpvar_35 = _LightShadowData.xxxx;
  highp float tmpvar_36;
  if (tmpvar_34.x) {
    tmpvar_36 = tmpvar_35.x;
  } else {
    tmpvar_36 = 1.0;
  };
  highp float tmpvar_37;
  if (tmpvar_34.y) {
    tmpvar_37 = tmpvar_35.y;
  } else {
    tmpvar_37 = 1.0;
  };
  highp float tmpvar_38;
  if (tmpvar_34.z) {
    tmpvar_38 = tmpvar_35.z;
  } else {
    tmpvar_38 = 1.0;
  };
  highp float tmpvar_39;
  if (tmpvar_34.w) {
    tmpvar_39 = tmpvar_35.w;
  } else {
    tmpvar_39 = 1.0;
  };
  mediump vec4 tmpvar_40;
  tmpvar_40.x = tmpvar_36;
  tmpvar_40.y = tmpvar_37;
  tmpvar_40.z = tmpvar_38;
  tmpvar_40.w = tmpvar_39;
  mediump float tmpvar_41;
  tmpvar_41 = dot (tmpvar_40, vec4(0.25, 0.25, 0.25, 0.25));
  highp vec4 tmpvar_42;
  tmpvar_42.w = 1.0;
  tmpvar_42.xyz = tmpvar_13;
  lowp vec4 tmpvar_43;
  highp vec3 P_44;
  P_44 = (_LightMatrix0 * tmpvar_42).xyz;
  tmpvar_43 = texture (_LightTexture0, P_44);
  highp float tmpvar_45;
  tmpvar_45 = ((atten_5 * tmpvar_41) * tmpvar_43.w);
  atten_5 = tmpvar_45;
  mediump float tmpvar_46;
  tmpvar_46 = max (0.0, dot (lightDir_6, normal_7));
  highp vec3 tmpvar_47;
  tmpvar_47 = normalize((lightDir_6 - normalize(
    (tmpvar_13 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_47;
  mediump float tmpvar_48;
  tmpvar_48 = pow (max (0.0, dot (h_4, normal_7)), (nspec_8.w * 128.0));
  spec_3 = tmpvar_48;
  highp float tmpvar_49;
  tmpvar_49 = (spec_3 * clamp (tmpvar_45, 0.0, 1.0));
  spec_3 = tmpvar_49;
  highp vec3 tmpvar_50;
  tmpvar_50 = (_LightColor.xyz * (tmpvar_46 * tmpvar_45));
  res_2.xyz = tmpvar_50;
  lowp vec3 c_51;
  c_51 = _LightColor.xyz;
  lowp float tmpvar_52;
  tmpvar_52 = dot (c_51, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_53;
  tmpvar_53 = (tmpvar_49 * tmpvar_52);
  res_2.w = tmpvar_53;
  highp float tmpvar_54;
  tmpvar_54 = clamp ((1.0 - (
    (tmpvar_15 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_55;
  tmpvar_55 = (res_2 * tmpvar_54);
  res_2 = tmpvar_55;
  tmpvar_1 = tmpvar_55.wxyz;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec3 normal_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  normal_6 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_14;
  tmpvar_14 = mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_15;
  tmpvar_15 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_15;
  mediump float tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture2D (_ShadowMapTexture, tmpvar_8);
  highp float tmpvar_18;
  tmpvar_18 = clamp ((tmpvar_17.x + clamp (
    ((tmpvar_14 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_16 = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = tmpvar_16;
  mediump float tmpvar_20;
  tmpvar_20 = max (0.0, dot (lightDir_5, normal_6));
  highp vec3 tmpvar_21;
  tmpvar_21 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_21;
  mediump float tmpvar_22;
  tmpvar_22 = pow (max (0.0, dot (h_4, normal_6)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = (spec_3 * clamp (tmpvar_19, 0.0, 1.0));
  spec_3 = tmpvar_23;
  highp vec3 tmpvar_24;
  tmpvar_24 = (_LightColor.xyz * (tmpvar_20 * tmpvar_19));
  res_2.xyz = tmpvar_24;
  lowp vec3 c_25;
  c_25 = _LightColor.xyz;
  lowp float tmpvar_26;
  tmpvar_26 = dot (c_25, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_27;
  tmpvar_27 = (tmpvar_23 * tmpvar_26);
  res_2.w = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = clamp ((1.0 - (
    (tmpvar_14 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_29;
  tmpvar_29 = (res_2 * tmpvar_28);
  res_2 = tmpvar_29;
  tmpvar_1 = tmpvar_29.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform sampler2D _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec3 normal_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  normal_6 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_14;
  tmpvar_14 = mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_15;
  tmpvar_15 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_15;
  mediump float tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture (_ShadowMapTexture, tmpvar_8);
  highp float tmpvar_18;
  tmpvar_18 = clamp ((tmpvar_17.x + clamp (
    ((tmpvar_14 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_16 = tmpvar_18;
  highp float tmpvar_19;
  tmpvar_19 = tmpvar_16;
  mediump float tmpvar_20;
  tmpvar_20 = max (0.0, dot (lightDir_5, normal_6));
  highp vec3 tmpvar_21;
  tmpvar_21 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_21;
  mediump float tmpvar_22;
  tmpvar_22 = pow (max (0.0, dot (h_4, normal_6)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_22;
  highp float tmpvar_23;
  tmpvar_23 = (spec_3 * clamp (tmpvar_19, 0.0, 1.0));
  spec_3 = tmpvar_23;
  highp vec3 tmpvar_24;
  tmpvar_24 = (_LightColor.xyz * (tmpvar_20 * tmpvar_19));
  res_2.xyz = tmpvar_24;
  lowp vec3 c_25;
  c_25 = _LightColor.xyz;
  lowp float tmpvar_26;
  tmpvar_26 = dot (c_25, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_27;
  tmpvar_27 = (tmpvar_23 * tmpvar_26);
  res_2.w = tmpvar_27;
  highp float tmpvar_28;
  tmpvar_28 = clamp ((1.0 - (
    (tmpvar_14 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_29;
  tmpvar_29 = (res_2 * tmpvar_28);
  res_2 = tmpvar_29;
  tmpvar_1 = tmpvar_29.wxyz;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT

uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTexture0;
uniform sampler2D _ShadowMapTexture;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec3 normal_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture2D (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  normal_6 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_14;
  tmpvar_14 = mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_15;
  tmpvar_15 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_15;
  mediump float tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture2D (_ShadowMapTexture, tmpvar_8);
  highp float tmpvar_18;
  tmpvar_18 = clamp ((tmpvar_17.x + clamp (
    ((tmpvar_14 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_16 = tmpvar_18;
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = tmpvar_12;
  lowp vec4 tmpvar_20;
  highp vec2 P_21;
  P_21 = (_LightMatrix0 * tmpvar_19).xy;
  tmpvar_20 = texture2D (_LightTexture0, P_21);
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_16 * tmpvar_20.w);
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_5, normal_6));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, normal_6)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (tmpvar_14 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  tmpvar_1 = tmpvar_32.wxyz;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec3 _glesNormal;
uniform highp vec4 _ProjectionParams;
uniform highp mat4 glstate_matrix_mvp;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp float _LightAsQuad;
out highp vec4 xlv_TEXCOORD0;
out highp vec3 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
  highp vec4 o_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = (tmpvar_1 * 0.5);
  highp vec2 tmpvar_4;
  tmpvar_4.x = tmpvar_3.x;
  tmpvar_4.y = (tmpvar_3.y * _ProjectionParams.x);
  o_2.xy = (tmpvar_4 + tmpvar_3.w);
  o_2.zw = tmpvar_1.zw;
  gl_Position = tmpvar_1;
  xlv_TEXCOORD0 = o_2;
  xlv_TEXCOORD1 = mix (((glstate_matrix_modelview0 * _glesVertex).xyz * vec3(-1.0, -1.0, 1.0)), _glesNormal, vec3(_LightAsQuad));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec3 _WorldSpaceCameraPos;
uniform highp vec4 _ProjectionParams;
uniform highp vec4 _ZBufferParams;
uniform highp vec4 _LightShadowData;
uniform highp vec4 unity_ShadowFadeCenterAndType;
uniform sampler2D _CameraNormalsTexture;
uniform highp sampler2D _CameraDepthTexture;
uniform highp vec4 _LightDir;
uniform highp vec4 _LightColor;
uniform highp vec4 unity_LightmapFade;
uniform highp mat4 _CameraToWorld;
uniform highp mat4 _LightMatrix0;
uniform sampler2D _LightTexture0;
uniform sampler2D _ShadowMapTexture;
in highp vec4 xlv_TEXCOORD0;
in highp vec3 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 res_2;
  highp float spec_3;
  mediump vec3 h_4;
  mediump vec3 lightDir_5;
  mediump vec3 normal_6;
  mediump vec4 nspec_7;
  highp vec2 tmpvar_8;
  tmpvar_8 = (xlv_TEXCOORD0.xy / xlv_TEXCOORD0.w);
  lowp vec4 tmpvar_9;
  tmpvar_9 = texture (_CameraNormalsTexture, tmpvar_8);
  nspec_7 = tmpvar_9;
  normal_6 = normalize(((nspec_7.xyz * 2.0) - 1.0));
  highp vec4 tmpvar_10;
  tmpvar_10 = texture (_CameraDepthTexture, tmpvar_8);
  highp vec4 tmpvar_11;
  tmpvar_11.w = 1.0;
  tmpvar_11.xyz = ((xlv_TEXCOORD1 * (_ProjectionParams.z / xlv_TEXCOORD1.z)) * (1.0/((
    (_ZBufferParams.x * tmpvar_10.x)
   + _ZBufferParams.y))));
  highp vec3 tmpvar_12;
  tmpvar_12 = (_CameraToWorld * tmpvar_11).xyz;
  highp vec3 tmpvar_13;
  tmpvar_13 = (tmpvar_12 - unity_ShadowFadeCenterAndType.xyz);
  highp float tmpvar_14;
  tmpvar_14 = mix (tmpvar_11.z, sqrt(dot (tmpvar_13, tmpvar_13)), unity_ShadowFadeCenterAndType.w);
  highp vec3 tmpvar_15;
  tmpvar_15 = -(_LightDir.xyz);
  lightDir_5 = tmpvar_15;
  mediump float tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture (_ShadowMapTexture, tmpvar_8);
  highp float tmpvar_18;
  tmpvar_18 = clamp ((tmpvar_17.x + clamp (
    ((tmpvar_14 * _LightShadowData.z) + _LightShadowData.w)
  , 0.0, 1.0)), 0.0, 1.0);
  tmpvar_16 = tmpvar_18;
  highp vec4 tmpvar_19;
  tmpvar_19.w = 1.0;
  tmpvar_19.xyz = tmpvar_12;
  lowp vec4 tmpvar_20;
  highp vec2 P_21;
  P_21 = (_LightMatrix0 * tmpvar_19).xy;
  tmpvar_20 = texture (_LightTexture0, P_21);
  highp float tmpvar_22;
  tmpvar_22 = (tmpvar_16 * tmpvar_20.w);
  mediump float tmpvar_23;
  tmpvar_23 = max (0.0, dot (lightDir_5, normal_6));
  highp vec3 tmpvar_24;
  tmpvar_24 = normalize((lightDir_5 - normalize(
    (tmpvar_12 - _WorldSpaceCameraPos)
  )));
  h_4 = tmpvar_24;
  mediump float tmpvar_25;
  tmpvar_25 = pow (max (0.0, dot (h_4, normal_6)), (nspec_7.w * 128.0));
  spec_3 = tmpvar_25;
  highp float tmpvar_26;
  tmpvar_26 = (spec_3 * clamp (tmpvar_22, 0.0, 1.0));
  spec_3 = tmpvar_26;
  highp vec3 tmpvar_27;
  tmpvar_27 = (_LightColor.xyz * (tmpvar_23 * tmpvar_22));
  res_2.xyz = tmpvar_27;
  lowp vec3 c_28;
  c_28 = _LightColor.xyz;
  lowp float tmpvar_29;
  tmpvar_29 = dot (c_28, vec3(0.22, 0.707, 0.071));
  highp float tmpvar_30;
  tmpvar_30 = (tmpvar_26 * tmpvar_29);
  res_2.w = tmpvar_30;
  highp float tmpvar_31;
  tmpvar_31 = clamp ((1.0 - (
    (tmpvar_14 * unity_LightmapFade.z)
   + unity_LightmapFade.w)), 0.0, 1.0);
  mediump vec4 tmpvar_32;
  tmpvar_32 = (res_2 * tmpvar_31);
  res_2 = tmpvar_32;
  tmpvar_1 = tmpvar_32.wxyz;
  _glesFragData[0] = tmpvar_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT" "SHADOWS_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "SPOT" "SHADOWS_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" "SHADOWS_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_OFF" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_OFF" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_NONATIVE" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_NATIVE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_NATIVE" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT" "SHADOWS_CUBE" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SHADOWS_NONATIVE" }
"!!GLES"
}
SubProgram "gles " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SHADOWS_NATIVE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "SPOT" "SHADOWS_DEPTH" "SHADOWS_SOFT" "SHADOWS_NATIVE" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" "SHADOWS_CUBE" "SHADOWS_SOFT" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL_COOKIE" "SHADOWS_SCREEN" "SHADOWS_NATIVE" }
"!!GLES3"
}
}
 }
}
Fallback Off
}