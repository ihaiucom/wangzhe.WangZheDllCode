using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections;

public class DecoratorWeightDecoder : BehaviorNodeDecoder {

    public override string ClassName()
    {
        return "DecoratorWeight";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.DecoratorWeight";
    }

    public override void DecoderClassFile(string filePath, DecoderMgr.Node node)
    {
        //DecorateWhenChildEnds
        string line;
        if (DecoderMgr.SearchLine(filePath, @"m\_bDecorateWhenChildEnds", out line))
        {
            string arg = line.Substring(line.LastIndexOf("=") + 1);
            arg = arg.Replace(" ", "");
            arg = arg.Replace(";", "");

            node.Properties["DecorateWhenChildEnds"] =  arg;
        }
        else
        {
            node.Properties["DecorateWhenChildEnds"] = "true";
        }
        //Weight
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
                        node.Properties["Weight"] = string.Format("int Self.{0}::{1}", DecoderMgr.Node.AgentType, pname);
                        //return string.Format("int Self.{1}::{2}",  DecoderMgr.Node.AgentType, pname);
                    }
                    else
                    {
                        Debug.LogError("没有通过UID : " + uid + " 找到对应的属性名字");
                    }
                }
                else
                {
                    Debug.LogError("这里没有写完，注意补全");
                }
            }
            else
            {
                //常量
                int value = 0;
                if (int.TryParse(returnArg, out value))
                {
                    node.Properties["Weight"] = "const int " + value.ToString();
                }
                else
                {
                    Debug.LogError(returnArg + "无法转换成 int 类型， 请检查!");
                    node.Properties["Weight"] = "const int " + value.ToString();
                }
            }
        }
        else
        {
            Debug.LogError(filePath + "没有找到返回方法!");
        }
    }
}
