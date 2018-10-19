
@ECHO OFF
Echo Auto-sign Created By Dave Da illest 1 
Echo wangzhe.apk is now being signed and will be renamed to wangzhe_signed.apk

java -jar signapk.jar testkey.x509.pem testkey.pk8 wangzhe.apk wangzhe_signed.apk

Echo Signing Complete 
 
Pause
EXIT