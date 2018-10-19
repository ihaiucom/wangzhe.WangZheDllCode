using UnityEngine;
using System.Collections;
using System;
using System.Text.RegularExpressions;

public class AssignmentDecoder : BehaviorNodeDecoder {

    public override string ClassName()
    {
        return "Assignment";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.Assignment";
    }

    public override void DecoderClassFile(string filePath, DecoderMgr.Node node)
    {
        //强转，无视
        node.Properties["CastRight"] = "false";

       
        string line = "";
        if (DecoderMgr.SearchLine(filePath, @"pAgent\.SetVariable\<.*\>", out line))
        {
            //Opl
            string typestr = DecoderMgr.FindFirstMatch(line, @"\<.*\>").Value;
            typestr = typestr.Substring(1, typestr.Length - 2);
            if (!DecoderMgr.isSupportedClass(typestr))
            {
                typestr = "string";
                string paramstr = DecoderMgr.FindFirstMatch(line, "\".*?\"").Value;
                paramstr = paramstr.Substring(1, paramstr.Length - 2);

                node.Properties["Opl"] = string.Format("{0} Self.{1}::{2}", typestr, DecoderMgr.Node.AgentType, paramstr);

                //Opr
                string args = DecoderMgr.FindLastMatch(line, @"\(.*\)").Value;
                string arg = args.Substring(args.IndexOf(",") + 1, args.LastIndexOf(",") - args.IndexOf(",") - 1);

                string argline = "";
                if (DecoderMgr.SearchLine(filePath, arg + @" \=", out argline))
                {
                    node.Properties["Opr"] = "const string \"" + argline.Split('=')[1].Replace(" ", "") + "\"";
                }
                else
                {
                    Debug.LogError(filePath + "找不到 " + arg + "的赋值");
                }
            }
            else
            {
                string paramstr = DecoderMgr.FindFirstMatch(line, "\".*?\"").Value;
                paramstr = paramstr.Substring(1, paramstr.Length - 2);

                node.Properties["Opl"] = string.Format("{0} Self.{1}::{2}", typestr, DecoderMgr.Node.AgentType, paramstr);

                //Opr
                string args = DecoderMgr.FindLastMatch(line, @"\(.*\)").Value;
                string arg = args.Substring(args.IndexOf(",") + 1, args.LastIndexOf(",") - args.IndexOf(",") - 1);
                arg = arg.Replace(" ", "");

                string argline = "";
                if (DecoderMgr.SearchLine(filePath, arg + @" \=", out argline))
                {
                    //这里找到的是int a = 1; 或者是 int a = self.fun(); 或者 int a = self.getVariable("asda") 所以需要进一步判断
                    Match m = DecoderMgr.FindFirstMatch(argline, @"\..*\(.*\)\;");
                    if (m != null && m.Length > 0)
                    {
                        //到这里满足了赋值的右边是调用了一个函数，但无法确定是调用了Agent的自有方法还是GetVariable来取了agent的自有属性
                        string functionname = DecoderMgr.FindFirstMatch(argline, @"\..*\(").Value;
                        functionname = functionname.Substring(1, functionname.Length - 2);
                        
                        if (functionname == "GetVariable")
                        {
                            //这里赋值的右边是取得agent的某个属性
                            string uid = DecoderMgr.FindLastMatch(argline, functionname + @"\(.*?\)").Value;
                            uid = uid.Substring(uid.IndexOf("(") + 1, uid.LastIndexOf(")") - uid.IndexOf("(") - 2);//多减少一位是因为最后还有一位字母u
                            string pname = "";
                            if (Singleton<DecoderMgr>.GetInstance().tryGetVariable(filePath, uid, out pname))
                            {
                                string type = DecoderMgr.FindFirstMatch(argline, @"[\S]* ").Value.Replace(" ", "");
                                node.Properties["Opr"] = string.Format("{0} Self.{1}::{2}", type, DecoderMgr.Node.AgentType, pname);
                            }
                            else
                            {
                                Debug.LogError("没有通过UID : " + uid + " 找到对应的属性名字");
                            }
                        }
                        else
                        {
                            //这里是取得agent的某个方法的返回值
                            Singleton<DecoderMgr>.GetInstance().AddMethod("ObjAgent", functionname);
                            string argsstr = DecoderMgr.FindLastMatch(argline, @"\([^()]*\)").Value;
                            node.Properties["Opr"] = string.Format("Self.{0}::{1}({2})", DecoderMgr.Node.AgentType, functionname, DecoderMgr.ParseArgsString(filePath, argsstr));
                        }
                    }
                    else
                    {
                        //到这里应该是常量类型赋值 
                        string type = DecoderMgr.FindFirstMatch(argline, @"[\S]* ").Value.Replace(" ", "");
                        if (Regex.IsMatch(argline, " " + arg + " "))
                        {
                            //格式int a = ... 这样能直接取到类型
                            //方法就能取好 type
                        }
                        else
                        {
                            //否则就是这样的格式，十分恶心,还得全文搜索一次
                            //int a;
                            //a = 1;
                            string typeline;
                            if (DecoderMgr.SearchLine(filePath, arg.Replace("this.", "") + @"\;", out typeline))
                            {
                                //type = DecoderMgr.FindFirstMatch(typeline, @"[\S]* ").Value.Replace(" ", "");
                                type = typeline.Substring(0, typeline.LastIndexOf(arg.Replace("this.", "")));
                                type = type.Replace("private", "");
                                type = type.Replace(" ", "");
                                type = type.Replace("\t", "");
                            }
                            else
                            {
                                Debug.LogError(filePath + "未能找到" + arg + "的类型，请检查" + " argline " + argline + " regex |" + " " + arg + " " + "|");
                            }
                        }
                        if (!DecoderMgr.isSupportedClass(type))
                        {
                            type = "string";
                        }
                        string value = DecoderMgr.FindLastMatch(argline, @" [\S]*\;").Value.Replace(";", "").Replace(" ", "");
                        value = DecoderMgr.FixPropertyString(type, value);
                        node.Properties["Opr"] = "const " + type + " " + value;
                    }
                    //node.Properties["Opr"] = "const string \"" + argline.Split('=')[1].Replace(" ", "") + "\"";
                }
            }
        }
    }
}
