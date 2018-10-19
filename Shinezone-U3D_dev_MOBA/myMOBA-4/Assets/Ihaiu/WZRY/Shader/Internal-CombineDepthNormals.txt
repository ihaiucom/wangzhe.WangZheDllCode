Shader "Hidden/Internal-CombineDepthNormals" {
SubShader { 
 Pass {
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 _CameraNormalsTexture_ST;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _CameraNormalsTexture_ST.xy) + _CameraNormalsTexture_ST.zw);
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _ZBufferParams;
uniform highp sampler2D _CameraDepthTexture;
uniform sampler2D _CameraNormalsTexture;
uniform highp mat4 _WorldToCamera;
varying highp vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec3 n_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = texture2D (_CameraDepthTexture, xlv_TEXCOORD0);
  lowp vec3 tmpvar_4;
  tmpvar_4 = ((texture2D (_CameraNormalsTexture, xlv_TEXCOORD0) * 2.0) - 1.0).xyz;
  n_2 = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * tmpvar_3.x) + _ZBufferParams.y)));
  highp mat3 tmpvar_6;
  tmpvar_6[0] = _WorldToCamera[0].xyz;
  tmpvar_6[1] = _WorldToCamera[1].xyz;
  tmpvar_6[2] = _WorldToCamera[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * n_2);
  n_2.xy = tmpvar_7.xy;
  n_2.z = -(tmpvar_7.z);
  highp vec4 tmpvar_8;
  if ((tmpvar_5 < 0.999985)) {
    highp vec4 enc_9;
    enc_9.xy = (((
      (tmpvar_7.xy / (n_2.z + 1.0))
     / 1.7777) * 0.5) + 0.5);
    highp vec2 enc_10;
    highp vec2 tmpvar_11;
    tmpvar_11 = fract((vec2(1.0, 255.0) * tmpvar_5));
    enc_10.y = tmpvar_11.y;
    enc_10.x = (tmpvar_11.x - (tmpvar_11.y * 0.00392157));
    enc_9.zw = enc_10;
    tmpvar_8 = enc_9;
  } else {
    tmpvar_8 = vec4(0.5, 0.5, 1.0, 1.0);
  };
  tmpvar_1 = tmpvar_8;
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 _CameraNormalsTexture_ST;
out highp vec2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _CameraNormalsTexture_ST.xy) + _CameraNormalsTexture_ST.zw);
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _ZBufferParams;
uniform highp sampler2D _CameraDepthTexture;
uniform sampler2D _CameraNormalsTexture;
uniform highp mat4 _WorldToCamera;
in highp vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec3 n_2;
  highp vec4 tmpvar_3;
  tmpvar_3 = texture (_CameraDepthTexture, xlv_TEXCOORD0);
  lowp vec3 tmpvar_4;
  tmpvar_4 = ((texture (_CameraNormalsTexture, xlv_TEXCOORD0) * 2.0) - 1.0).xyz;
  n_2 = tmpvar_4;
  highp float tmpvar_5;
  tmpvar_5 = (1.0/(((_ZBufferParams.x * tmpvar_3.x) + _ZBufferParams.y)));
  highp mat3 tmpvar_6;
  tmpvar_6[0] = _WorldToCamera[0].xyz;
  tmpvar_6[1] = _WorldToCamera[1].xyz;
  tmpvar_6[2] = _WorldToCamera[2].xyz;
  highp vec3 tmpvar_7;
  tmpvar_7 = (tmpvar_6 * n_2);
  n_2.xy = tmpvar_7.xy;
  n_2.z = -(tmpvar_7.z);
  highp vec4 tmpvar_8;
  if ((tmpvar_5 < 0.999985)) {
    highp vec4 enc_9;
    enc_9.xy = (((
      (tmpvar_7.xy / (n_2.z + 1.0))
     / 1.7777) * 0.5) + 0.5);
    highp vec2 enc_10;
    highp vec2 tmpvar_11;
    tmpvar_11 = fract((vec2(1.0, 255.0) * tmpvar_5));
    enc_10.y = tmpvar_11.y;
    enc_10.x = (tmpvar_11.x - (tmpvar_11.y * 0.00392157));
    enc_9.zw = enc_10;
    tmpvar_8 = enc_9;
  } else {
    tmpvar_8 = vec4(0.5, 0.5, 1.0, 1.0);
  };
  tmpvar_1 = tmpvar_8;
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