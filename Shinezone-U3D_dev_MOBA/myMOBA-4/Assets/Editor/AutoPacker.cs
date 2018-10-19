using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Diagnostics;
using System;

public class AutoPacker : EditorWindow {

	[MenuItem("AutoPacker/Pack")]
	public static void Pack()
	{
		string apkpath = EditorUtility.SaveFilePanel ("请选择Apk存放位置", Application.dataPath, "wangzhe", "apk");



		if (!string.IsNullOrEmpty(apkpath))
		{
			//路径有效
			BuildPipeline.BuildPlayer (new[] {"Assets/test/main.unity"}, apkpath, BuildTarget.Android, BuildOptions.AllowDebugging | BuildOptions.Development);

			Process proc = null;
			try
			{
				string batpath = Application.dataPath + "/../Tools/";
				batpath = batpath.Replace("/", "\\");
				proc = new Process();
				proc.StartInfo.WorkingDirectory = batpath;
				proc.StartInfo.FileName = "autopacker.bat";
				proc.StartInfo.Arguments = apkpath;
				proc.Start();
				proc.WaitForExit();
			}
			catch(Exception e)
			{
				UnityEngine.Debug.LogError(e.Message);
			}
		}
		else
		{
			//路径无效
		}
	}
}
