Shader "SGame_Post/ShadowBlur" {
SubShader { 
 LOD 200
 Tags { "IGNOREPROJECTOR"="true" "RenderType"="Opaque" }
 Pass {
  Tags { "IGNOREPROJECTOR"="true" "RenderType"="Opaque" }
  ZTest Always
  ZWrite Off
  Fog { Mode Off }
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 _BlurDirection;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD2;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xyxy;
  xlv_TEXCOORD1 = (_glesMultiTexCoord0.xyxy + _BlurDirection);
  xlv_TEXCOORD2 = (_glesMultiTexCoord0.xyxy + (_BlurDirection * 2.0));
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
varying highp vec4 xlv_TEXCOORD0;
varying highp vec4 xlv_TEXCOORD1;
varying highp vec4 xlv_TEXCOORD2;
void main ()
{
  highp vec4 c_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0.xy);
  c_1 = tmpvar_2;
  highp vec4 c_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture2D (_MainTex, xlv_TEXCOORD1.xy);
  c_3 = tmpvar_4;
  highp vec4 c_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture2D (_MainTex, xlv_TEXCOORD1.zw);
  c_5 = tmpvar_6;
  highp vec4 c_7;
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture2D (_MainTex, xlv_TEXCOORD2.xy);
  c_7 = tmpvar_8;
  highp vec4 c_9;
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture2D (_MainTex, xlv_TEXCOORD2.zw);
  c_9 = tmpvar_10;
  highp vec4 tmpvar_11;
  tmpvar_11 = fract((vec4(1.0, 255.0, 65025.0, 1.65814e+07) * (
    ((dot (c_1, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08)) * 0.25138) + ((dot (c_3, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08)) + dot (c_5, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08))) * 0.22184))
   + 
    ((dot (c_7, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08)) + dot (c_9, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08))) * 0.15247)
  )));
  highp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 - (tmpvar_11.yzww * vec4(0.00392157, 0.00392157, 0.00392157, 0.0)));
  gl_FragData[0] = tmpvar_12;
}



#endif"
}
SubProgram "gles3 " {
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform highp vec4 _BlurDirection;
out highp vec4 xlv_TEXCOORD0;
out highp vec4 xlv_TEXCOORD1;
out highp vec4 xlv_TEXCOORD2;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xyxy;
  xlv_TEXCOORD1 = (_glesMultiTexCoord0.xyxy + _BlurDirection);
  xlv_TEXCOORD2 = (_glesMultiTexCoord0.xyxy + (_BlurDirection * 2.0));
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
in highp vec4 xlv_TEXCOORD0;
in highp vec4 xlv_TEXCOORD1;
in highp vec4 xlv_TEXCOORD2;
void main ()
{
  highp vec4 c_1;
  lowp vec4 tmpvar_2;
  tmpvar_2 = texture (_MainTex, xlv_TEXCOORD0.xy);
  c_1 = tmpvar_2;
  highp vec4 c_3;
  lowp vec4 tmpvar_4;
  tmpvar_4 = texture (_MainTex, xlv_TEXCOORD1.xy);
  c_3 = tmpvar_4;
  highp vec4 c_5;
  lowp vec4 tmpvar_6;
  tmpvar_6 = texture (_MainTex, xlv_TEXCOORD1.zw);
  c_5 = tmpvar_6;
  highp vec4 c_7;
  lowp vec4 tmpvar_8;
  tmpvar_8 = texture (_MainTex, xlv_TEXCOORD2.xy);
  c_7 = tmpvar_8;
  highp vec4 c_9;
  lowp vec4 tmpvar_10;
  tmpvar_10 = texture (_MainTex, xlv_TEXCOORD2.zw);
  c_9 = tmpvar_10;
  highp vec4 tmpvar_11;
  tmpvar_11 = fract((vec4(1.0, 255.0, 65025.0, 1.65814e+07) * (
    ((dot (c_1, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08)) * 0.25138) + ((dot (c_3, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08)) + dot (c_5, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08))) * 0.22184))
   + 
    ((dot (c_7, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08)) + dot (c_9, vec4(1.0, 0.00392157, 1.53787e-05, 6.03086e-08))) * 0.15247)
  )));
  highp vec4 tmpvar_12;
  tmpvar_12 = (tmpvar_11 - (tmpvar_11.yzww * vec4(0.00392157, 0.00392157, 0.00392157, 0.0)));
  _glesFragData[0] = tmpvar_12;
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