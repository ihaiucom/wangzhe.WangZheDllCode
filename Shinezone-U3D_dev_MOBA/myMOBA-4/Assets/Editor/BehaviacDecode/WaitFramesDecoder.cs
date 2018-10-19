using UnityEngine;
using System.Collections;
using System;
using System.Text.RegularExpressions;

public class WaitFramesDecoder : BehaviorNodeDecoder {

    public override string ClassName()
    {
        return "WaitFrames";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.WaitFrames";
    }

    public override void DecoderClassFile(string filePath, DecoderMgr.Node node)
    {
        //Frames
        string line;
        if (DecoderMgr.SearchLine(filePath, @"return", out line))
        {
            string returnArg = line.Substring(line.IndexOf(@"return") + "return".Length);
            returnArg = returnArg.Replace(" ", "");
            returnArg = returnArg.Replace(";", "");
            if (Regex.IsMatch(returnArg, @"\..*\(.*\)"))
            {
                //调用了方法
                //GetVariable
                if (Regex.IsMatch(returnArg, @"\.GetVariable\("))
                {
                    string uid = DecoderMgr.FindLastMatch(line, @"GetVariable\(.*?\)").Value;
                    uid = uid.Substring(uid.IndexOf("(") + 1, uid.LastIndexOf(")") - uid.IndexOf("(") - 2);//多减少一位是因为最后还有一位字母u
                    string pname = "";
                    if (Singleton<DecoderMgr>.GetInstance().tryGetVariable(filePath, uid, out pname))
                    {
                        node.Properties["Frames"] = string.Format("int Self.{0}::{1}", DecoderMgr.Node.AgentType, pname);
                        //return string.Format("int Self.{1}::{2}",  DecoderMgr.Node.AgentType, pname);
                    }
                    else
                    {
                        Debug.LogError("没有通过UID : " + uid + " 找到对应的属性名字");
                    }
                }
                else
                {
                    //这里是取得agent的某个方法的返回值
                    string functionname = DecoderMgr.FindFirstMatch(returnArg, @"\..*\(").Value;
                    functionname = functionname.Substring(1, functionname.Length - 2);
                    Singleton<DecoderMgr>.GetInstance().AddMethod("ObjAgent", functionname);
                    string argsstr = DecoderMgr.FindLastMatch(returnArg, @"\([^()]*\)").Value;
                    node.Properties["Frames"] = string.Format("Self.{0}::{1}({2})", DecoderMgr.Node.AgentType, functionname, DecoderMgr.ParseArgsString(filePath, argsstr));
                }
            }
            else
            {
                //常量
                int value = 0;
                if (int.TryParse(returnArg, out value))
                {
                    node.Properties["Frames"] = "const int " + value.ToString();
                }
                else
                {
                    Debug.LogError(returnArg + "无法转换成 int 类型， 请检查!");
                    node.Properties["Frames"] = "const int " + value.ToString();
                }
            }
        }
        else
        {
            Debug.LogError(filePath + "没有找到返回方法!");
        }
    }
}
