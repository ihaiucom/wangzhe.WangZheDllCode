Shader "Sprites/Default2" {
Properties {
[PerRendererData]  _MainTex ("Sprite Texture", 2D) = "white" {}
 _Color ("Tint", Color) = (1,1,1,1)
[MaterialToggle]  PixelSnap ("Pixel snap", Float) = 0
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  Blend One OneMinusSrcAlpha
Program "vp" {
SubProgram "gles " {
Keywords { "DUMMY" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform lowp vec4 _Color;
varying lowp vec4 xlv_COLOR;
varying mediump vec4 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 tmpvar_2;
  mediump vec2 tex_3;
  highp vec2 tmpvar_4;
  tmpvar_4 = _glesMultiTexCoord0.xy;
  tex_3 = tmpvar_4;
  tex_3.y = (tex_3.y * 0.5);
  tmpvar_2.xy = tex_3;
  tmpvar_2.zw = (tex_3 + vec2(0.0, 0.5));
  highp vec4 tmpvar_5;
  tmpvar_5 = (_glesColor * _Color);
  tmpvar_1 = tmpvar_5;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_COLOR = tmpvar_1;
  xlv_TEXCOORD0 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
varying lowp vec4 xlv_COLOR;
varying mediump vec4 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 c_1;
  c_1.xyz = texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz;
  c_1.w = texture2D (_MainTex, xlv_TEXCOORD0.zw).x;
  lowp vec4 tmpvar_2;
  tmpvar_2 = (c_1 * xlv_COLOR);
  c_1.w = tmpvar_2.w;
  c_1.xyz = (tmpvar_2.xyz * tmpvar_2.w);
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "DUMMY" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesColor;
in vec4 _glesMultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp;
uniform lowp vec4 _Color;
out lowp vec4 xlv_COLOR;
out mediump vec4 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 tmpvar_2;
  mediump vec2 tex_3;
  highp vec2 tmpvar_4;
  tmpvar_4 = _glesMultiTexCoord0.xy;
  tex_3 = tmpvar_4;
  tex_3.y = (tex_3.y * 0.5);
  tmpvar_2.xy = tex_3;
  tmpvar_2.zw = (tex_3 + vec2(0.0, 0.5));
  highp vec4 tmpvar_5;
  tmpvar_5 = (_glesColor * _Color);
  tmpvar_1 = tmpvar_5;
  gl_Position = (glstate_matrix_mvp * _glesVertex);
  xlv_COLOR = tmpvar_1;
  xlv_TEXCOORD0 = tmpvar_2;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
in lowp vec4 xlv_COLOR;
in mediump vec4 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 c_1;
  c_1.xyz = texture (_MainTex, xlv_TEXCOORD0.xy).xyz;
  c_1.w = texture (_MainTex, xlv_TEXCOORD0.zw).x;
  lowp vec4 tmpvar_2;
  tmpvar_2 = (c_1 * xlv_COLOR);
  c_1.w = tmpvar_2.w;
  c_1.xyz = (tmpvar_2.xyz * tmpvar_2.w);
  _glesFragData[0] = c_1;
}



#endif"
}
SubProgram "gles " {
Keywords { "PIXELSNAP_ON" }
"!!GLES


#ifdef VERTEX

attribute vec4 _glesVertex;
attribute vec4 _glesColor;
attribute vec4 _glesMultiTexCoord0;
uniform highp vec4 _ScreenParams;
uniform highp mat4 glstate_matrix_mvp;
uniform lowp vec4 _Color;
varying lowp vec4 xlv_COLOR;
varying mediump vec4 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 tmpvar_2;
  mediump vec2 tex_3;
  highp vec2 tmpvar_4;
  tmpvar_4 = _glesMultiTexCoord0.xy;
  tex_3 = tmpvar_4;
  tex_3.y = (tex_3.y * 0.5);
  highp vec4 tmpvar_5;
  tmpvar_5 = (glstate_matrix_mvp * _glesVertex);
  tmpvar_2.xy = tex_3;
  tmpvar_2.zw = (tex_3 + vec2(0.0, 0.5));
  highp vec4 tmpvar_6;
  tmpvar_6 = (_glesColor * _Color);
  tmpvar_1 = tmpvar_6;
  highp vec4 pos_7;
  pos_7.zw = tmpvar_5.zw;
  highp vec2 tmpvar_8;
  tmpvar_8 = (_ScreenParams.xy * 0.5);
  pos_7.xy = ((floor(
    (((tmpvar_5.xy / tmpvar_5.w) * tmpvar_8) + vec2(0.5, 0.5))
  ) / tmpvar_8) * tmpvar_5.w);
  gl_Position = pos_7;
  xlv_COLOR = tmpvar_1;
  xlv_TEXCOORD0 = tmpvar_2;
}



#endif
#ifdef FRAGMENT

uniform sampler2D _MainTex;
varying lowp vec4 xlv_COLOR;
varying mediump vec4 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 c_1;
  c_1.xyz = texture2D (_MainTex, xlv_TEXCOORD0.xy).xyz;
  c_1.w = texture2D (_MainTex, xlv_TEXCOORD0.zw).x;
  lowp vec4 tmpvar_2;
  tmpvar_2 = (c_1 * xlv_COLOR);
  c_1.w = tmpvar_2.w;
  c_1.xyz = (tmpvar_2.xyz * tmpvar_2.w);
  gl_FragData[0] = c_1;
}



#endif"
}
SubProgram "gles3 " {
Keywords { "PIXELSNAP_ON" }
"!!GLES3#version 300 es


#ifdef VERTEX


in vec4 _glesVertex;
in vec4 _glesColor;
in vec4 _glesMultiTexCoord0;
uniform highp vec4 _ScreenParams;
uniform highp mat4 glstate_matrix_mvp;
uniform lowp vec4 _Color;
out lowp vec4 xlv_COLOR;
out mediump vec4 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 tmpvar_1;
  mediump vec4 tmpvar_2;
  mediump vec2 tex_3;
  highp vec2 tmpvar_4;
  tmpvar_4 = _glesMultiTexCoord0.xy;
  tex_3 = tmpvar_4;
  tex_3.y = (tex_3.y * 0.5);
  highp vec4 tmpvar_5;
  tmpvar_5 = (glstate_matrix_mvp * _glesVertex);
  tmpvar_2.xy = tex_3;
  tmpvar_2.zw = (tex_3 + vec2(0.0, 0.5));
  highp vec4 tmpvar_6;
  tmpvar_6 = (_glesColor * _Color);
  tmpvar_1 = tmpvar_6;
  highp vec4 pos_7;
  pos_7.zw = tmpvar_5.zw;
  highp vec2 tmpvar_8;
  tmpvar_8 = (_ScreenParams.xy * 0.5);
  pos_7.xy = ((floor(
    (((tmpvar_5.xy / tmpvar_5.w) * tmpvar_8) + vec2(0.5, 0.5))
  ) / tmpvar_8) * tmpvar_5.w);
  gl_Position = pos_7;
  xlv_COLOR = tmpvar_1;
  xlv_TEXCOORD0 = tmpvar_2;
}



#endif
#ifdef FRAGMENT


layout(location=0) out mediump vec4 _glesFragData[4];
uniform sampler2D _MainTex;
in lowp vec4 xlv_COLOR;
in mediump vec4 xlv_TEXCOORD0;
void main ()
{
  lowp vec4 c_1;
  c_1.xyz = texture (_MainTex, xlv_TEXCOORD0.xy).xyz;
  c_1.w = texture (_MainTex, xlv_TEXCOORD0.zw).x;
  lowp vec4 tmpvar_2;
  tmpvar_2 = (c_1 * xlv_COLOR);
  c_1.w = tmpvar_2.w;
  c_1.xyz = (tmpvar_2.xyz * tmpvar_2.w);
  _glesFragData[0] = c_1;
}



#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "DUMMY" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DUMMY" }
"!!GLES3"
}
SubProgram "gles " {
Keywords { "PIXELSNAP_ON" }
"!!GLES"
}
SubProgram "gles3 " {
Keywords { "PIXELSNAP_ON" }
"!!GLES3"
}
}
 }
}
}