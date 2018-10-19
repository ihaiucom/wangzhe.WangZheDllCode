using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

public class BehaviacDecodeEditor : Editor
{
    /// <summary>
    /// 行为树生成代码源文件路径
    /// </summary>
    public static string BtSourceCodePath = Application.dataPath + "/Scripts/behaviac";

    [MenuItem("BehaviacTree/一键生成")]
    public static void Generate()
    {
        if (!Directory.Exists(BtSourceCodePath))
        {
            EditorUtility.DisplayDialog("错误", BtSourceCodePath + "文件夹路径不存在，请在BehaviacDecoderEditor修改BtSourceCodePath", "确认");
            return;
        }

        string btsSavePath = EditorUtility.SaveFolderPanel("请选择保存生成的路径", Application.dataPath, "");
        if (string.IsNullOrEmpty(btsSavePath))
        {
            EditorUtility.DisplayDialog("确定", "已取消", "确认");
            return;
        }

        DirectoryInfo di = new DirectoryInfo(BtSourceCodePath);
        FileInfo[] files = di.GetFiles();

        if (files != null && files.Length > 0)
        {
            for (int i = 0, imax = files.Length; i < imax; i++)
            {
                try
                {
                    EditorUtility.DisplayProgressBar("进度", files[i].Name, i * 1f / imax);
                    string ext = Path.GetExtension(files[i].Name);
                    if (ext == ".cs")
                    {
                        string errorstr;
                        string srcPath = files[i].FullName;
                        string savePath = btsSavePath + "/" + Path.GetFileNameWithoutExtension(files[i].Name) + ".xml";
                        Singleton<DecoderMgr>.GetInstance().TryParseAndSaveFile(srcPath, savePath, out errorstr);
                    }

                }
                catch (Exception e)
                {
                    Debug.LogError(files[i].FullName);
                    Debug.LogError(e.Message);
                    Debug.LogError(e.StackTrace);
                }
            }
            Singleton<DecoderMgr>.GetInstance().SaveMeta(btsSavePath + "/behaviac_meta/meta.xml");
            EditorUtility.DisplayProgressBar("进度", "", 1);
        }
        EditorUtility.ClearProgressBar();
        Singleton<DecoderMgr>.DestroyInstance();
        AssetDatabase.Refresh();
    }
}
    