SZClient
=======

## 1. 概述
**SZClient是一套基于Lua的Unity热更新客户端框架**

---------------------
更多功能开发中，文档仍需补全

## 2. 安装
复制SZClient到你的项目

## 3. 文档
SZClient\Doc

## 4. 例子
打开SZClient\Assets\LuaFramework\Scenes\main.unity，运行

## 5. 目录结构
### SZClient                                  ---- 框架顶层目录
    |__ Assets                             ---- Unity项目资源目录
        |__ EncryptMonoDll                  ---- Dll加密（采用il2cpp则无需使用）
        |__ Examples                        ---- 一些例子（无需合并到项目中）
        |__ Lua                             ---- 项目自身的Lua代码（框架默认读这里）
        |__ LuaFramework                    ---- Lua框架
        |__ NGUI                            ---- NGUI插件(NGUI分支独有)
        |__ Plugins                         ---- xLua.*（一些c库）
        |__ XLua                            ---- xLua插件
    |__ build                               ---- 框架相关源码（编译为xLua.*）                           
    |__ Doc                                 ---- 框架相关文档
    |__ Tools                               ---- 框架相关外部工具（非Unity代码）
    
## 6.  第三方库
### 6.1 rapidjson：
https://github.com/xpol/lua-rapidjson
### 6.2 protoc-gen-lua： 
https://github.com/sean-lin/protoc-gen-lua
### 6.3 xLua(ver 2.1.6)：
https://github.com/Tencent/xLua

myMOBA-4
=======

## 1. 概述
**myMOBA-4是王者荣耀安卓端反编译出来的源码**

---------------------
通过项目内源码编译出的包可以结合手机调试，了解王者荣耀游戏框架

## 2. 文档
**编译、调试文档：**myMOBA-4\Tools\使用方法.docx