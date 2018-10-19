Shader "Hidden/Shadow-ScreenBlur" {
Properties {
 _MainTex ("Base", 2D) = "white" {}
}
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
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
uniform highp vec4 _BlurOffsets0;
uniform highp vec4 _BlurOffsets1;
uniform highp vec4 _BlurOffsets2;
uniform highp vec4 _BlurOffsets3;
uniform highp vec4 _BlurOffsets4;
uniform highp vec4 _BlurOffsets5;
uniform highp vec4 _BlurOffsets6;
uniform highp vec4 _BlurOffsets7;
uniform highp vec4 unity_ShadowBlurParams;
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 theSample_7_2;
  highp vec4 theSample_6_3;
  highp vec4 theSample_5_4;
  highp vec4 theSample_4_5;
  highp vec4 theSample_3_6;
  highp vec4 theSample_2_7;
  highp vec4 theSample_1_8;
  highp vec4 theSample_9;
  highp vec4 mask_10;
  highp vec4 coord_11;
  mediump vec4 tmpvar_12;
  tmpvar_12.zw = vec2(0.0, 0.0);
  tmpvar_12.xy = xlv_TEXCOORD0;
  coord_11 = tmpvar_12;
  lowp vec4 tmpvar_13;
  tmpvar_13 = texture2D (_MainTex, coord_11.xy);
  mask_10 = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = (mask_10.z + (mask_10.w / 255.0));
  highp float tmpvar_15;
  tmpvar_15 = clamp ((unity_ShadowBlurParams.y / (1.0 - tmpvar_14)), 0.0, 1.0);
  mask_10.xy = (mask_10.xy * unity_ShadowBlurParams.x);
  lowp vec4 tmpvar_16;
  highp vec2 P_17;
  P_17 = (coord_11 + (tmpvar_15 * _BlurOffsets0)).xy;
  tmpvar_16 = texture2D (_MainTex, P_17);
  theSample_9 = tmpvar_16;
  mask_10.xy = (mask_10.xy + (clamp (
    (unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_9.z + 
      (theSample_9.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_9.xy));
  lowp vec4 tmpvar_18;
  highp vec2 P_19;
  P_19 = (coord_11 + (tmpvar_15 * _BlurOffsets1)).xy;
  tmpvar_18 = texture2D (_MainTex, P_19);
  theSample_1_8 = tmpvar_18;
  mask_10.xy = (mask_10.xy + (clamp (
    (unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_1_8.z + 
      (theSample_1_8.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_1_8.xy));
  lowp vec4 tmpvar_20;
  highp vec2 P_21;
  P_21 = (coord_11 + (tmpvar_15 * _BlurOffsets2)).xy;
  tmpvar_20 = texture2D (_MainTex, P_21);
  theSample_2_7 = tmpvar_20;
  mask_10.xy = (mask_10.xy + (clamp (
    (unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_2_7.z + 
      (theSample_2_7.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_2_7.xy));
  lowp vec4 tmpvar_22;
  highp vec2 P_23;
  P_23 = (coord_11 + (tmpvar_15 * _BlurOffsets3)).xy;
  tmpvar_22 = texture2D (_MainTex, P_23);
  theSample_3_6 = tmpvar_22;
  mask_10.xy = (mask_10.xy + (clamp (
    (unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_3_6.z + 
      (theSample_3_6.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_3_6.xy));
  lowp vec4 tmpvar_24;
  highp vec2 P_25;
  P_25 = (coord_11 + (tmpvar_15 * _BlurOffsets4)).xy;
  tmpvar_24 = texture2D (_MainTex, P_25);
  theSample_4_5 = tmpvar_24;
  mask_10.xy = (mask_10.xy + (clamp (
    (unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_4_5.z + 
      (theSample_4_5.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_4_5.xy));
  lowp vec4 tmpvar_26;
  highp vec2 P_27;
  P_27 = (coord_11 + (tmpvar_15 * _BlurOffsets5)).xy;
  tmpvar_26 = texture2D (_MainTex, P_27);
  theSample_5_4 = tmpvar_26;
  mask_10.xy = (mask_10.xy + (clamp (
    (unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_5_4.z + 
      (theSample_5_4.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_5_4.xy));
  lowp vec4 tmpvar_28;
  highp vec2 P_29;
  P_29 = (coord_11 + (tmpvar_15 * _BlurOffsets6)).xy;
  tmpvar_28 = texture2D (_MainTex, P_29);
  theSample_6_3 = tmpvar_28;
  mask_10.xy = (mask_10.xy + (clamp (
    (unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_6_3.z + 
      (theSample_6_3.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_6_3.xy));
  lowp vec4 tmpvar_30;
  highp vec2 P_31;
  P_31 = (coord_11 + (tmpvar_15 * _BlurOffsets7)).xy;
  tmpvar_30 = texture2D (_MainTex, P_31);
  theSample_7_2 = tmpvar_30;
  mask_10.xy = (mask_10.xy + (clamp (
    (unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_7_2.z + 
      (theSample_7_2.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_7_2.xy));
  highp vec4 tmpvar_32;
  tmpvar_32 = vec4((mask_10.x / mask_10.y));
  tmpvar_1 = tmpvar_32;
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
out mediump vec2 xlv_TEXCOORD0;
void main ()
{
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
uniform highp vec4 _BlurOffsets0;
uniform highp vec4 _BlurOffsets1;
uniform highp vec4 _BlurOffsets2;
uniform highp vec4 _BlurOffsets3;
uniform highp vec4 _BlurOffsets4;
uniform highp vec4 _BlurOffsets5;
uniform highp vec4 _BlurOffsets6;
uniform highp vec4 _BlurOffsets7;
uniform highp vec4 unity_ShadowBlurParams;
in mediump vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  highp vec4 theSample_7_2;
  highp vec4 theSample_6_3;
  highp vec4 theSample_5_4;
  highp vec4 theSample_4_5;
  highp vec4 theSample_3_6;
  highp vec4 theSample_2_7;
  highp vec4 theSample_1_8;
  highp vec4 theSample_9;
  highp vec4 mask_10;
  highp vec4 coord_11;
  mediump vec4 tmpvar_12;
  tmpvar_12.zw = vec2(0.0, 0.0);
  tmpvar_12.xy = xlv_TEXCOORD0;
  coord_11 = tmpvar_12;
  lowp vec4 tmpvar_13;
  tmpvar_13 = texture (_MainTex, coord_11.xy);
  mask_10 = tmpvar_13;
  highp float tmpvar_14;
  tmpvar_14 = (mask_10.z + (mask_10.w / 255.0));
  highp float tmpvar_15;
  tmpvar_15 = clamp ((unity_ShadowBlurParams.y / (1.0 - tmpvar_14)), 0.0, 1.0);
  mask_10.xy = (mask_10.xy * unity_ShadowBlurParams.x);
  lowp vec4 tmpvar_16;
  highp vec2 P_17;
  P_17 = (coord_11 + (tmpvar_15 * _BlurOffsets0)).xy;
  tmpvar_16 = texture (_MainTex, P_17);
  theSample_9 = tmpvar_16;
  mask_10.xy = (mask_10.xy + (clamp (
    (unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_9.z + 
      (theSample_9.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_9.xy));
  lowp vec4 tmpvar_18;
  highp vec2 P_19;
  P_19 = (coord_11 + (tmpvar_15 * _BlurOffsets1)).xy;
  tmpvar_18 = texture (_MainTex, P_19);
  theSample_1_8 = tmpvar_18;
  mask_10.xy = (mask_10.xy + (clamp (
    (unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_1_8.z + 
      (theSample_1_8.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_1_8.xy));
  lowp vec4 tmpvar_20;
  highp vec2 P_21;
  P_21 = (coord_11 + (tmpvar_15 * _BlurOffsets2)).xy;
  tmpvar_20 = texture (_MainTex, P_21);
  theSample_2_7 = tmpvar_20;
  mask_10.xy = (mask_10.xy + (clamp (
    (unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_2_7.z + 
      (theSample_2_7.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_2_7.xy));
  lowp vec4 tmpvar_22;
  highp vec2 P_23;
  P_23 = (coord_11 + (tmpvar_15 * _BlurOffsets3)).xy;
  tmpvar_22 = texture (_MainTex, P_23);
  theSample_3_6 = tmpvar_22;
  mask_10.xy = (mask_10.xy + (clamp (
    (unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_3_6.z + 
      (theSample_3_6.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_3_6.xy));
  lowp vec4 tmpvar_24;
  highp vec2 P_25;
  P_25 = (coord_11 + (tmpvar_15 * _BlurOffsets4)).xy;
  tmpvar_24 = texture (_MainTex, P_25);
  theSample_4_5 = tmpvar_24;
  mask_10.xy = (mask_10.xy + (clamp (
    (unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_4_5.z + 
      (theSample_4_5.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_4_5.xy));
  lowp vec4 tmpvar_26;
  highp vec2 P_27;
  P_27 = (coord_11 + (tmpvar_15 * _BlurOffsets5)).xy;
  tmpvar_26 = texture (_MainTex, P_27);
  theSample_5_4 = tmpvar_26;
  mask_10.xy = (mask_10.xy + (clamp (
    (unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_5_4.z + 
      (theSample_5_4.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_5_4.xy));
  lowp vec4 tmpvar_28;
  highp vec2 P_29;
  P_29 = (coord_11 + (tmpvar_15 * _BlurOffsets6)).xy;
  tmpvar_28 = texture (_MainTex, P_29);
  theSample_6_3 = tmpvar_28;
  mask_10.xy = (mask_10.xy + (clamp (
    (unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_6_3.z + 
      (theSample_6_3.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_6_3.xy));
  lowp vec4 tmpvar_30;
  highp vec2 P_31;
  P_31 = (coord_11 + (tmpvar_15 * _BlurOffsets7)).xy;
  tmpvar_30 = texture (_MainTex, P_31);
  theSample_7_2 = tmpvar_30;
  mask_10.xy = (mask_10.xy + (clamp (
    (unity_ShadowBlurParams.x - abs((tmpvar_14 - (theSample_7_2.z + 
      (theSample_7_2.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_7_2.xy));
  highp vec4 tmpvar_32;
  tmpvar_32 = vec4((mask_10.x / mask_10.y));
  tmpvar_1 = tmpvar_32;
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