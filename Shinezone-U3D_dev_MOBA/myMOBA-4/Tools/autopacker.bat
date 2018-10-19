::创建一个临时文件夹，放解压后的文件
md %cd%\UnityOrignalApk

::解压到临时文件夹
start /wait winrar e -o+ %1 %cd%\UnityOrignalApk

::删除打的包
del /s /q %~f1

::复制4个关键文件到指定目录
copy  %cd%\UnityOrignalApk\Assembly-CSharp.dll %cd%\apktool_2.2.2\wangzhe\assets\bin\Data\Managed\
copy  %cd%\UnityOrignalApk\Assembly-CSharp.dll.mdb %cd%\apktool_2.2.2\wangzhe\assets\bin\Data\Managed\
copy  %cd%\UnityOrignalApk\Assembly-CSharp-firstpass.dll %cd%\apktool_2.2.2\wangzhe\assets\bin\Data\Managed\
copy  %cd%\UnityOrignalApk\Assembly-CSharp-firstpass.dll.mdb %cd%\apktool_2.2.2\wangzhe\assets\bin\Data\Managed\

::打包
call %cd%\apktool_2.2.2\apktool.bat b %cd%\apktool_2.2.2\wangzhe


::签名
Echo StartSign...
::java -jar %cd%\autosign\signapk.jar %cd%\autosign\testkey.x509.pem %cd%\autosign\testkey.pk8 %cd%\apktool_2.2.2\wangzhe\dist\wangzhe.apk %cd%\apktool_2.2.2\wangzhe\dist\wangzhe_signed.apk
java -jar %cd%\autosign\signapk.jar %cd%\autosign\testkey.x509.pem %cd%\autosign\testkey.pk8 %cd%\apktool_2.2.2\wangzhe\dist\wangzhe.apk %1

::删除多余的未签名包和解压的文件夹
del %cd%\apktool_2.2.2\wangzhe\dist\wangzhe.apk
rd /s /q %cd%\UnityOrignalApk

::打开窗口
Echo Complete!
start explorer %~dp1
pause