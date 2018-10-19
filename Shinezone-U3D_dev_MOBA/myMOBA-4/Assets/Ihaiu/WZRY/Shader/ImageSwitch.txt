Shader "UI/ImageSwitch" {
Properties {
[PerRendererData]  _MainTex ("Sprite Texture", 2D) = "white" {}
 _Color ("Tint", Color) = (1,1,1,1)
 _Tex1 ("Sprite1", 2D) = "transparent" {}
 _Tex2 ("Sprite2", 2D) = "transparent" {}
 _Percent ("Anim Percent", Float) = 0
 _StencilComp ("Stencil Comparison", Float) = 8
 _Stencil ("Stencil ID", Float) = 0
 _StencilOp ("Stencil Operation", Float) = 0
 _StencilWriteMask ("Stencil Write Mask", Float) = 255
 _StencilReadMask ("Stencil Read Mask", Float) = 255
 _ColorMask ("Color Mask", Float) = 15
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  ZTest [unity_GUIZTestMode]
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  Stencil {
   Ref [_Stencil]
   ReadMask [_StencilReadMask]
   WriteMask [_StencilWriteMask]
   Comp [_StencilComp]
   Pass [_StencilOp]
  }
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask [_ColorMask]
Program "vp" {
SubProgram "gles " {
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform lowp vec4 _Color;
varying lowp vec4 xlv_COLOR;
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  lowp vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  tmpvar_3 = tmpvar_1;
  highp vec4 tmpvar_4;
  tmpvar_4 = (_glesColor * _Color);
  tmpvar_2 = tmpvar_4;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_COLOR = tmpvar_2;
  xlv_TEXCOORD0 = tmpvar_3;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _Tex1;
uniform sampler2D _Tex2;
uniform highp float _Percent;
varying mediump vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 color_2;
  color_2 = vec4(1.0, 1.0, 1.0, 0.0);
  if ((_Percent == 0.0)) {
    lowp vec4 tmpvar_3;
    tmpvar_3 = texture2D (_Tex1, xlv_TEXCOORD0);
    color_2 = tmpvar_3;
  } else {
    if ((_Percent == 1.0)) {
      lowp vec4 tmpvar_4;
      tmpvar_4 = texture2D (_Tex2, xlv_TEXCOORD0);
      color_2 = tmpvar_4;
    } else {
      lowp vec4 tmpvar_5;
      tmpvar_5 = texture2D (_Tex1, xlv_TEXCOORD0);
      lowp vec4 tmpvar_6;
      tmpvar_6 = texture2D (_Tex2, xlv_TEXCOORD0);
      highp vec4 tmpvar_7;
      tmpvar_7 = mix (tmpvar_5, tmpvar_6, vec4(_Percent));
      color_2 = tmpvar_7;
    };
  };
  mediump float x_8;
  x_8 = (color_2.w - 0.01);
  if ((x_8 < 0.0)) {
    discard;
  };
  tmpvar_1 = color_2;
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
uniform highp mat4 glstate_matrix_mvp;
uniform lowp vec4 _Color;
out lowp vec4 xlv_COLOR;
out mediump vec2 xlv_TEXCOORD0;
void main ()
{
  highp vec2 tmpvar_1;
  tmpvar_1 = _glesMultiTexCoord0.xy;
  lowp vec4 tmpvar_2;
  mediump vec2 tmpvar_3;
  tmpvar_3 = tmpvar_1;
  highp vec4 tmpvar_4;
  tmpvar_4 = (_glesColor * _Color);
  tmpvar_2 = tmpvar_4;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_COLOR = tmpvar_2;
  xlv_TEXCOORD0 = tmpvar_3;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _Tex1;
uniform sampler2D _Tex2;
uniform highp float _Percent;
in mediump vec2 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 color_2;
  color_2 = vec4(1.0, 1.0, 1.0, 0.0);
  if ((_Percent == 0.0)) {
    lowp vec4 tmpvar_3;
    tmpvar_3 = texture (_Tex1, xlv_TEXCOORD0);
    color_2 = tmpvar_3;
  } else {
    if ((_Percent == 1.0)) {
      lowp vec4 tmpvar_4;
      tmpvar_4 = texture (_Tex2, xlv_TEXCOORD0);
      color_2 = tmpvar_4;
    } else {
      lowp vec4 tmpvar_5;
      tmpvar_5 = texture (_Tex1, xlv_TEXCOORD0);
      lowp vec4 tmpvar_6;
      tmpvar_6 = texture (_Tex2, xlv_TEXCOORD0);
      highp vec4 tmpvar_7;
      tmpvar_7 = mix (tmpvar_5, tmpvar_6, vec4(_Percent));
      color_2 = tmpvar_7;
    };
  };
  mediump float x_8;
  x_8 = (color_2.w - 0.01);
  if ((x_8 < 0.0)) {
    discard;
  };
  tmpvar_1 = color_2;
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