Shader "Hidden/Shadow-ScreenBlurRotated" {
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
uniform highp mat4 glstate_matrix_texture0;
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  mediump vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  mediump vec2 tmpvar_2;
  highp vec2 tmpvar_3;
  highp vec2 inUV_4;
  inUV_4 = tmpvar_1;
  highp vec4 tmpvar_5;
  tmpvar_5.zw = vec2(0.0, 0.0);
  tmpvar_5.xy = inUV_4;
  tmpvar_3 = (glstate_matrix_texture0 * tmpvar_5).xy;
  tmpvar_2 = tmpvar_3;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

uniform highp vec4 _ScreenParams;
uniform sampler2D _MainTex;
uniform sampler2D unity_RandomRotation16;
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
  mediump vec4 theSample_7_2;
  mediump vec4 theSample_6_3;
  mediump vec4 theSample_5_4;
  mediump vec4 theSample_4_5;
  mediump vec4 theSample_3_6;
  mediump vec4 theSample_2_7;
  mediump vec4 theSample_1_8;
  mediump vec4 theSample_9;
  mediump float diffTolerance_10;
  mediump vec4 mask_11;
  mediump vec4 rotation_12;
  highp vec4 coord_13;
  mediump vec4 tmpvar_14;
  tmpvar_14.zw = vec2(0.0, 0.0);
  tmpvar_14.xy = xlv_TEXCOORD0;
  coord_13 = tmpvar_14;
  highp vec2 P_15;
  P_15 = ((coord_13.xy * _ScreenParams.xy) / 16.0);
  lowp vec4 tmpvar_16;
  tmpvar_16 = ((2.0 * texture2D (unity_RandomRotation16, P_15)) - 1.0);
  rotation_12 = tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture2D (_MainTex, coord_13.xy);
  mask_11 = tmpvar_17;
  mediump float tmpvar_18;
  tmpvar_18 = (mask_11.z + (mask_11.w / 255.0));
  mediump float tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = clamp ((unity_ShadowBlurParams.y / (1.0 - tmpvar_18)), 0.0, 1.0);
  tmpvar_19 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = unity_ShadowBlurParams.x;
  diffTolerance_10 = tmpvar_21;
  mask_11.xy = (mask_11.xy * diffTolerance_10);
  highp vec4 rotation_22;
  rotation_22 = rotation_12;
  highp vec2 offset_23;
  offset_23.x = dot (_BlurOffsets0.xy, rotation_22.xy);
  offset_23.y = dot (_BlurOffsets0.xy, rotation_22.zw);
  lowp vec4 tmpvar_24;
  highp vec2 P_25;
  P_25 = (coord_13.xy + (tmpvar_19 * offset_23));
  tmpvar_24 = texture2D (_MainTex, P_25);
  theSample_9 = tmpvar_24;
  mask_11.xy = (mask_11.xy + (clamp (
    (diffTolerance_10 - abs((tmpvar_18 - (theSample_9.z + 
      (theSample_9.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_9.xy));
  highp vec4 rotation_26;
  rotation_26 = rotation_12;
  highp vec2 offset_27;
  offset_27.x = dot (_BlurOffsets1.xy, rotation_26.xy);
  offset_27.y = dot (_BlurOffsets1.xy, rotation_26.zw);
  lowp vec4 tmpvar_28;
  highp vec2 P_29;
  P_29 = (coord_13.xy + (tmpvar_19 * offset_27));
  tmpvar_28 = texture2D (_MainTex, P_29);
  theSample_1_8 = tmpvar_28;
  mask_11.xy = (mask_11.xy + (clamp (
    (diffTolerance_10 - abs((tmpvar_18 - (theSample_1_8.z + 
      (theSample_1_8.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_1_8.xy));
  highp vec4 rotation_30;
  rotation_30 = rotation_12;
  highp vec2 offset_31;
  offset_31.x = dot (_BlurOffsets2.xy, rotation_30.xy);
  offset_31.y = dot (_BlurOffsets2.xy, rotation_30.zw);
  lowp vec4 tmpvar_32;
  highp vec2 P_33;
  P_33 = (coord_13.xy + (tmpvar_19 * offset_31));
  tmpvar_32 = texture2D (_MainTex, P_33);
  theSample_2_7 = tmpvar_32;
  mask_11.xy = (mask_11.xy + (clamp (
    (diffTolerance_10 - abs((tmpvar_18 - (theSample_2_7.z + 
      (theSample_2_7.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_2_7.xy));
  highp vec4 rotation_34;
  rotation_34 = rotation_12;
  highp vec2 offset_35;
  offset_35.x = dot (_BlurOffsets3.xy, rotation_34.xy);
  offset_35.y = dot (_BlurOffsets3.xy, rotation_34.zw);
  lowp vec4 tmpvar_36;
  highp vec2 P_37;
  P_37 = (coord_13.xy + (tmpvar_19 * offset_35));
  tmpvar_36 = texture2D (_MainTex, P_37);
  theSample_3_6 = tmpvar_36;
  mask_11.xy = (mask_11.xy + (clamp (
    (diffTolerance_10 - abs((tmpvar_18 - (theSample_3_6.z + 
      (theSample_3_6.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_3_6.xy));
  highp vec4 rotation_38;
  rotation_38 = rotation_12;
  highp vec2 offset_39;
  offset_39.x = dot (_BlurOffsets4.xy, rotation_38.xy);
  offset_39.y = dot (_BlurOffsets4.xy, rotation_38.zw);
  lowp vec4 tmpvar_40;
  highp vec2 P_41;
  P_41 = (coord_13.xy + (tmpvar_19 * offset_39));
  tmpvar_40 = texture2D (_MainTex, P_41);
  theSample_4_5 = tmpvar_40;
  mask_11.xy = (mask_11.xy + (clamp (
    (diffTolerance_10 - abs((tmpvar_18 - (theSample_4_5.z + 
      (theSample_4_5.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_4_5.xy));
  highp vec4 rotation_42;
  rotation_42 = rotation_12;
  highp vec2 offset_43;
  offset_43.x = dot (_BlurOffsets5.xy, rotation_42.xy);
  offset_43.y = dot (_BlurOffsets5.xy, rotation_42.zw);
  lowp vec4 tmpvar_44;
  highp vec2 P_45;
  P_45 = (coord_13.xy + (tmpvar_19 * offset_43));
  tmpvar_44 = texture2D (_MainTex, P_45);
  theSample_5_4 = tmpvar_44;
  mask_11.xy = (mask_11.xy + (clamp (
    (diffTolerance_10 - abs((tmpvar_18 - (theSample_5_4.z + 
      (theSample_5_4.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_5_4.xy));
  highp vec4 rotation_46;
  rotation_46 = rotation_12;
  highp vec2 offset_47;
  offset_47.x = dot (_BlurOffsets6.xy, rotation_46.xy);
  offset_47.y = dot (_BlurOffsets6.xy, rotation_46.zw);
  lowp vec4 tmpvar_48;
  highp vec2 P_49;
  P_49 = (coord_13.xy + (tmpvar_19 * offset_47));
  tmpvar_48 = texture2D (_MainTex, P_49);
  theSample_6_3 = tmpvar_48;
  mask_11.xy = (mask_11.xy + (clamp (
    (diffTolerance_10 - abs((tmpvar_18 - (theSample_6_3.z + 
      (theSample_6_3.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_6_3.xy));
  highp vec4 rotation_50;
  rotation_50 = rotation_12;
  highp vec2 offset_51;
  offset_51.x = dot (_BlurOffsets7.xy, rotation_50.xy);
  offset_51.y = dot (_BlurOffsets7.xy, rotation_50.zw);
  lowp vec4 tmpvar_52;
  highp vec2 P_53;
  P_53 = (coord_13.xy + (tmpvar_19 * offset_51));
  tmpvar_52 = texture2D (_MainTex, P_53);
  theSample_7_2 = tmpvar_52;
  mask_11.xy = (mask_11.xy + (clamp (
    (diffTolerance_10 - abs((tmpvar_18 - (theSample_7_2.z + 
      (theSample_7_2.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_7_2.xy));
  mediump vec4 tmpvar_54;
  tmpvar_54 = vec4((mask_11.x / mask_11.y));
  tmpvar_1 = tmpvar_54;
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
uniform highp mat4 glstate_matrix_texture0;
out mediump vec2 xlv_TEXCOORD0;
void main ()
{
  mediump vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  mediump vec2 tmpvar_2;
  highp vec2 tmpvar_3;
  highp vec2 inUV_4;
  inUV_4 = tmpvar_1;
  highp vec4 tmpvar_5;
  tmpvar_5.zw = vec2(0.0, 0.0);
  tmpvar_5.xy = inUV_4;
  tmpvar_3 = (glstate_matrix_texture0 * tmpvar_5).xy;
  tmpvar_2 = tmpvar_3;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_TEXCOORD0 = tmpvar_2;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform highp vec4 _ScreenParams;
uniform sampler2D _MainTex;
uniform sampler2D unity_RandomRotation16;
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
  mediump vec4 theSample_7_2;
  mediump vec4 theSample_6_3;
  mediump vec4 theSample_5_4;
  mediump vec4 theSample_4_5;
  mediump vec4 theSample_3_6;
  mediump vec4 theSample_2_7;
  mediump vec4 theSample_1_8;
  mediump vec4 theSample_9;
  mediump float diffTolerance_10;
  mediump vec4 mask_11;
  mediump vec4 rotation_12;
  highp vec4 coord_13;
  mediump vec4 tmpvar_14;
  tmpvar_14.zw = vec2(0.0, 0.0);
  tmpvar_14.xy = xlv_TEXCOORD0;
  coord_13 = tmpvar_14;
  highp vec2 P_15;
  P_15 = ((coord_13.xy * _ScreenParams.xy) / 16.0);
  lowp vec4 tmpvar_16;
  tmpvar_16 = ((2.0 * texture (unity_RandomRotation16, P_15)) - 1.0);
  rotation_12 = tmpvar_16;
  lowp vec4 tmpvar_17;
  tmpvar_17 = texture (_MainTex, coord_13.xy);
  mask_11 = tmpvar_17;
  mediump float tmpvar_18;
  tmpvar_18 = (mask_11.z + (mask_11.w / 255.0));
  mediump float tmpvar_19;
  highp float tmpvar_20;
  tmpvar_20 = clamp ((unity_ShadowBlurParams.y / (1.0 - tmpvar_18)), 0.0, 1.0);
  tmpvar_19 = tmpvar_20;
  highp float tmpvar_21;
  tmpvar_21 = unity_ShadowBlurParams.x;
  diffTolerance_10 = tmpvar_21;
  mask_11.xy = (mask_11.xy * diffTolerance_10);
  highp vec4 rotation_22;
  rotation_22 = rotation_12;
  highp vec2 offset_23;
  offset_23.x = dot (_BlurOffsets0.xy, rotation_22.xy);
  offset_23.y = dot (_BlurOffsets0.xy, rotation_22.zw);
  lowp vec4 tmpvar_24;
  highp vec2 P_25;
  P_25 = (coord_13.xy + (tmpvar_19 * offset_23));
  tmpvar_24 = texture (_MainTex, P_25);
  theSample_9 = tmpvar_24;
  mask_11.xy = (mask_11.xy + (clamp (
    (diffTolerance_10 - abs((tmpvar_18 - (theSample_9.z + 
      (theSample_9.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_9.xy));
  highp vec4 rotation_26;
  rotation_26 = rotation_12;
  highp vec2 offset_27;
  offset_27.x = dot (_BlurOffsets1.xy, rotation_26.xy);
  offset_27.y = dot (_BlurOffsets1.xy, rotation_26.zw);
  lowp vec4 tmpvar_28;
  highp vec2 P_29;
  P_29 = (coord_13.xy + (tmpvar_19 * offset_27));
  tmpvar_28 = texture (_MainTex, P_29);
  theSample_1_8 = tmpvar_28;
  mask_11.xy = (mask_11.xy + (clamp (
    (diffTolerance_10 - abs((tmpvar_18 - (theSample_1_8.z + 
      (theSample_1_8.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_1_8.xy));
  highp vec4 rotation_30;
  rotation_30 = rotation_12;
  highp vec2 offset_31;
  offset_31.x = dot (_BlurOffsets2.xy, rotation_30.xy);
  offset_31.y = dot (_BlurOffsets2.xy, rotation_30.zw);
  lowp vec4 tmpvar_32;
  highp vec2 P_33;
  P_33 = (coord_13.xy + (tmpvar_19 * offset_31));
  tmpvar_32 = texture (_MainTex, P_33);
  theSample_2_7 = tmpvar_32;
  mask_11.xy = (mask_11.xy + (clamp (
    (diffTolerance_10 - abs((tmpvar_18 - (theSample_2_7.z + 
      (theSample_2_7.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_2_7.xy));
  highp vec4 rotation_34;
  rotation_34 = rotation_12;
  highp vec2 offset_35;
  offset_35.x = dot (_BlurOffsets3.xy, rotation_34.xy);
  offset_35.y = dot (_BlurOffsets3.xy, rotation_34.zw);
  lowp vec4 tmpvar_36;
  highp vec2 P_37;
  P_37 = (coord_13.xy + (tmpvar_19 * offset_35));
  tmpvar_36 = texture (_MainTex, P_37);
  theSample_3_6 = tmpvar_36;
  mask_11.xy = (mask_11.xy + (clamp (
    (diffTolerance_10 - abs((tmpvar_18 - (theSample_3_6.z + 
      (theSample_3_6.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_3_6.xy));
  highp vec4 rotation_38;
  rotation_38 = rotation_12;
  highp vec2 offset_39;
  offset_39.x = dot (_BlurOffsets4.xy, rotation_38.xy);
  offset_39.y = dot (_BlurOffsets4.xy, rotation_38.zw);
  lowp vec4 tmpvar_40;
  highp vec2 P_41;
  P_41 = (coord_13.xy + (tmpvar_19 * offset_39));
  tmpvar_40 = texture (_MainTex, P_41);
  theSample_4_5 = tmpvar_40;
  mask_11.xy = (mask_11.xy + (clamp (
    (diffTolerance_10 - abs((tmpvar_18 - (theSample_4_5.z + 
      (theSample_4_5.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_4_5.xy));
  highp vec4 rotation_42;
  rotation_42 = rotation_12;
  highp vec2 offset_43;
  offset_43.x = dot (_BlurOffsets5.xy, rotation_42.xy);
  offset_43.y = dot (_BlurOffsets5.xy, rotation_42.zw);
  lowp vec4 tmpvar_44;
  highp vec2 P_45;
  P_45 = (coord_13.xy + (tmpvar_19 * offset_43));
  tmpvar_44 = texture (_MainTex, P_45);
  theSample_5_4 = tmpvar_44;
  mask_11.xy = (mask_11.xy + (clamp (
    (diffTolerance_10 - abs((tmpvar_18 - (theSample_5_4.z + 
      (theSample_5_4.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_5_4.xy));
  highp vec4 rotation_46;
  rotation_46 = rotation_12;
  highp vec2 offset_47;
  offset_47.x = dot (_BlurOffsets6.xy, rotation_46.xy);
  offset_47.y = dot (_BlurOffsets6.xy, rotation_46.zw);
  lowp vec4 tmpvar_48;
  highp vec2 P_49;
  P_49 = (coord_13.xy + (tmpvar_19 * offset_47));
  tmpvar_48 = texture (_MainTex, P_49);
  theSample_6_3 = tmpvar_48;
  mask_11.xy = (mask_11.xy + (clamp (
    (diffTolerance_10 - abs((tmpvar_18 - (theSample_6_3.z + 
      (theSample_6_3.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_6_3.xy));
  highp vec4 rotation_50;
  rotation_50 = rotation_12;
  highp vec2 offset_51;
  offset_51.x = dot (_BlurOffsets7.xy, rotation_50.xy);
  offset_51.y = dot (_BlurOffsets7.xy, rotation_50.zw);
  lowp vec4 tmpvar_52;
  highp vec2 P_53;
  P_53 = (coord_13.xy + (tmpvar_19 * offset_51));
  tmpvar_52 = texture (_MainTex, P_53);
  theSample_7_2 = tmpvar_52;
  mask_11.xy = (mask_11.xy + (clamp (
    (diffTolerance_10 - abs((tmpvar_18 - (theSample_7_2.z + 
      (theSample_7_2.w / 255.0)
    ))))
  , 0.0, 1.0) * theSample_7_2.xy));
  mediump vec4 tmpvar_54;
  tmpvar_54 = vec4((mask_11.x / mask_11.y));
  tmpvar_1 = tmpvar_54;
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