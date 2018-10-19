Shader "S_Game_Effects/Billboard_Rays" {
Properties {
 _MainTex ("Base texture", 2D) = "white" {}
 _Multiplier ("Color multiplier", Float) = 1
 _Bias ("Bias", Float) = 0
 _TimeOnDuration ("ON duration", Float) = 0.5
 _TimeOffDuration ("OFF duration", Float) = 0.5
 _BlinkingTimeOffsScale ("Blinking time offset scale (seconds)", Float) = 5
 _NoiseAmount ("Noise amount (when zero, pulse wave is used)", Range(0,0.5)) = 0
 _Color ("Color", Color) = (1,1,1,1)
}
SubShader { 
 LOD 100
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Fog {
   Color (0,0,0,0)
  }
  Blend One One
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 _Object2World;
uniform highp mat4 glstate_matrix_projection;
uniform highp float _Multiplier;
uniform highp float _Bias;
uniform highp float _TimeOnDuration;
uniform highp float _TimeOffDuration;
uniform highp float _BlinkingTimeOffsScale;
uniform highp float _NoiseAmount;
uniform highp vec4 _Color;
varying highp vec2 xlv_TEXCOORD0;
varying lowp vec4 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0;
  highp vec4 pos_2;
  lowp vec4 tmpvar_3;
  highp float tmpvar_4;
  tmpvar_4 = (_Time.y + (_BlinkingTimeOffsScale * _glesColor.z));
  highp float y_5;
  y_5 = (_TimeOnDuration + _TimeOffDuration);
  highp float tmpvar_6;
  tmpvar_6 = (tmpvar_4 / y_5);
  highp float tmpvar_7;
  tmpvar_7 = (fract(abs(tmpvar_6)) * y_5);
  highp float tmpvar_8;
  if ((tmpvar_6 >= 0.0)) {
    tmpvar_8 = tmpvar_7;
  } else {
    tmpvar_8 = -(tmpvar_7);
  };
  highp float tmpvar_9;
  tmpvar_9 = clamp ((tmpvar_8 / (_TimeOnDuration * 0.25)), 0.0, 1.0);
  highp float edge0_10;
  edge0_10 = (_TimeOnDuration * 0.75);
  highp float tmpvar_11;
  tmpvar_11 = clamp (((tmpvar_8 - edge0_10) / (_TimeOnDuration - edge0_10)), 0.0, 1.0);
  highp float tmpvar_12;
  tmpvar_12 = ((tmpvar_9 * (tmpvar_9 * 
    (3.0 - (2.0 * tmpvar_9))
  )) * (1.0 - (tmpvar_11 * 
    (tmpvar_11 * (3.0 - (2.0 * tmpvar_11)))
  )));
  highp float tmpvar_13;
  tmpvar_13 = (tmpvar_4 * (6.28319 / _TimeOnDuration));
  highp float tmpvar_14;
  tmpvar_14 = ((_NoiseAmount * (
    sin(tmpvar_13)
   * 
    ((0.5 * cos((
      (tmpvar_13 * 0.6366)
     + 56.7272))) + 0.5)
  )) + (1.0 - _NoiseAmount));
  highp float tmpvar_15;
  if ((_NoiseAmount < 0.01)) {
    tmpvar_15 = tmpvar_12;
  } else {
    tmpvar_15 = tmpvar_14;
  };
  highp vec4 tmpvar_16;
  tmpvar_16 = ((_Color * _Multiplier) * (tmpvar_15 + _Bias));
  tmpvar_3 = tmpvar_16;
  highp vec4 tmpvar_17;
  tmpvar_17 = (glstate_matrix_modelview0 * vec4(0.0, 0.0, 0.0, 1.0));
  pos_2.zw = tmpvar_17.zw;
  highp vec4 v_18;
  v_18.x = _Object2World[0].x;
  v_18.y = _Object2World[1].x;
  v_18.z = _Object2World[2].x;
  v_18.w = _Object2World[3].x;
  pos_2.xy = (tmpvar_17.xy + (_glesVertex.x * v_18.xy));
  highp vec4 v_19;
  v_19.x = _Object2World[0].y;
  v_19.y = _Object2World[1].y;
  v_19.z = _Object2World[2].y;
  v_19.w = _Object2World[3].y;
  pos_2.xy = (pos_2.xy + (_glesVertex.y * v_19.xy));
  gl_Position = (glstate_matrix_projection * pos_2);
  xlv_TEXCOORD0 = tmpvar_1.xy;
  xlv_TEXCOORD1 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
varying highp vec2 xlv_TEXCOORD0;
varying lowp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_TEXCOORD1);
  gl_FragData[0] = tmpvar_1;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesColor;
in vec4 _glesMultiTexCoord0;
uniform highp vec4 _Time;
uniform highp mat4 glstate_matrix_modelview0;
uniform highp mat4 _Object2World;
uniform highp mat4 glstate_matrix_projection;
uniform highp float _Multiplier;
uniform highp float _Bias;
uniform highp float _TimeOnDuration;
uniform highp float _TimeOffDuration;
uniform highp float _BlinkingTimeOffsScale;
uniform highp float _NoiseAmount;
uniform highp vec4 _Color;
out highp vec2 xlv_TEXCOORD0;
out lowp vec4 xlv_TEXCOORD1;
void main ()
{
  highp vec4 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0;
  highp vec4 pos_2;
  lowp vec4 tmpvar_3;
  highp float tmpvar_4;
  tmpvar_4 = (_Time.y + (_BlinkingTimeOffsScale * _glesColor.z));
  highp float y_5;
  y_5 = (_TimeOnDuration + _TimeOffDuration);
  highp float tmpvar_6;
  tmpvar_6 = (tmpvar_4 / y_5);
  highp float tmpvar_7;
  tmpvar_7 = (fract(abs(tmpvar_6)) * y_5);
  highp float tmpvar_8;
  if ((tmpvar_6 >= 0.0)) {
    tmpvar_8 = tmpvar_7;
  } else {
    tmpvar_8 = -(tmpvar_7);
  };
  highp float tmpvar_9;
  tmpvar_9 = clamp ((tmpvar_8 / (_TimeOnDuration * 0.25)), 0.0, 1.0);
  highp float edge0_10;
  edge0_10 = (_TimeOnDuration * 0.75);
  highp float tmpvar_11;
  tmpvar_11 = clamp (((tmpvar_8 - edge0_10) / (_TimeOnDuration - edge0_10)), 0.0, 1.0);
  highp float tmpvar_12;
  tmpvar_12 = ((tmpvar_9 * (tmpvar_9 * 
    (3.0 - (2.0 * tmpvar_9))
  )) * (1.0 - (tmpvar_11 * 
    (tmpvar_11 * (3.0 - (2.0 * tmpvar_11)))
  )));
  highp float tmpvar_13;
  tmpvar_13 = (tmpvar_4 * (6.28319 / _TimeOnDuration));
  highp float tmpvar_14;
  tmpvar_14 = ((_NoiseAmount * (
    sin(tmpvar_13)
   * 
    ((0.5 * cos((
      (tmpvar_13 * 0.6366)
     + 56.7272))) + 0.5)
  )) + (1.0 - _NoiseAmount));
  highp float tmpvar_15;
  if ((_NoiseAmount < 0.01)) {
    tmpvar_15 = tmpvar_12;
  } else {
    tmpvar_15 = tmpvar_14;
  };
  highp vec4 tmpvar_16;
  tmpvar_16 = ((_Color * _Multiplier) * (tmpvar_15 + _Bias));
  tmpvar_3 = tmpvar_16;
  highp vec4 tmpvar_17;
  tmpvar_17 = (glstate_matrix_modelview0 * vec4(0.0, 0.0, 0.0, 1.0));
  pos_2.zw = tmpvar_17.zw;
  highp vec4 v_18;
  v_18.x = _Object2World[0].x;
  v_18.y = _Object2World[1].x;
  v_18.z = _Object2World[2].x;
  v_18.w = _Object2World[3].x;
  pos_2.xy = (tmpvar_17.xy + (_glesVertex.x * v_18.xy));
  highp vec4 v_19;
  v_19.x = _Object2World[0].y;
  v_19.y = _Object2World[1].y;
  v_19.z = _Object2World[2].y;
  v_19.w = _Object2World[3].y;
  pos_2.xy = (pos_2.xy + (_glesVertex.y * v_19.xy));
  gl_Position = (glstate_matrix_projection * pos_2);
  xlv_TEXCOORD0 = tmpvar_1.xy;
  xlv_TEXCOORD1 = tmpvar_3;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
in highp vec2 xlv_TEXCOORD0;
in lowp vec4 xlv_TEXCOORD1;
void main ()
{
  lowp vec4 tmpvar_1;
  tmpvar_1 = (texture (_MainTex, xlv_TEXCOORD0) * xlv_TEXCOORD1);
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
}