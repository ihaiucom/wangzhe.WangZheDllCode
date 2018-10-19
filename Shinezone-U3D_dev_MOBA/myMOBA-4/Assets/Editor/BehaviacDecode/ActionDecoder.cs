using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Text.RegularExpressions;

public class ActionDecoder : BehaviorNodeDecoder {
    public override string ClassName()
    {
        return "Action";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.Action";
    }

    public override void DecoderClassFile(string filePath, DecoderMgr.Node node)
    {
        //这个大部分都用不到，所以先偷懒不解析这个
        node.Properties["ResultFunctor"] = "\"\"";

        string line = "";
        if (DecoderMgr.SearchLine(filePath, @"m\_resultOption \=", out line))
        {
            node.Properties["ResultOption"] = line.Substring(line.LastIndexOf('.') + 1, line.LastIndexOf(';') - line.LastIndexOf('.') - 1);
        }
        else if (DecoderMgr.SearchLine(filePath, @"return EBTStatus", out line))
        {
            node.Properties["ResultOption"] = line.Substring(line.LastIndexOf('.') + 1, line.LastIndexOf(';') - line.LastIndexOf('.') - 1);
        }
        else
        {
            node.Properties["ResultOption"] = "BT_SUCCESS";
        }

        if (DecoderMgr.SearchLine(filePath, @"\(\(.*\)pAgent\)\..*\(.*\);", out line))
        {
            string agentname = DecoderMgr.FindFirstMatch(line, @"\([^()]*\)").Value;
            agentname = agentname.Substring(1, agentname.Length - 2);
            string functionname = DecoderMgr.FindFirstMatch(line, @"\.[^()]*\(").Value;
            functionname = functionname.Substring(1, functionname.Length - 2);
            #region 向WrapperAgent 内添加这个function
            Singleton<DecoderMgr>.GetInstance().AddMethod( agentname, functionname);
            #endregion
            string argsstr = DecoderMgr.FindLastMatch(line, @"\([^()]*\)").Value;
            argsstr = argsstr.Substring(1, argsstr.Length - 2);
            argsstr = argsstr.Replace(" ","");
            string[] args = argsstr.Split(',');
            if (args != null && args.Length > 0)
            {
                for (int i = 0, imax = args.Length; i < imax; i++)
                {
                    if (string.IsNullOrEmpty(args[i]))
                    {
                        continue;
                    }
                    if (args[i].StartsWith("\"") && args[i].EndsWith("\""))
                    {
                        //字符串
                    }
                    else
                    {
                        float tmp;
                        if (float.TryParse(args[i], out tmp))
                        {
                            //数字类型
                            //把C#浮点中的f给去了或者无符号的的u给去了
                            args[i] = DecoderMgr.FixPropertyString(args[i]);
                        }
                        else
                        {
                            //C# 的 数据类型后缀，这里要把最后一位强行删除后再判断下
                            if (float.TryParse(args[i].Substring(0, args[i].Length - 1), out tmp))
                            {
                                //数字类型
                                //把C#浮点中的f给去了或者无符号的的u给去了
                                args[i] = DecoderMgr.FixPropertyString(args[i]);
                            }
                            else
                            {
                                string argline = "";

                                if (DecoderMgr.SearchLine(filePath, args[i].Replace(".", @"\.") + @".*\=.*;", out argline))
                                {
                                    //这里behaviac不支持把一个方法的返回值作为另外一个方法的参数，所以只用判断 参数是一个常量还是一个Agent的属性即可
                                    Match m = DecoderMgr.FindFirstMatch(argline, @"\..*\(.*\)\;");
                                    if (m != null && m.Length > 0)
                                    {
                                        string argfunctionname = DecoderMgr.FindFirstMatch(argline, @"\.[^()]*\(").Value;
                                        argfunctionname = argfunctionname.Substring(1, argfunctionname.Length - 2);
                                        if (argfunctionname == "GetVariable")
                                        {
                                            //这里赋值的右边是取得agent的某个属性
                                            string uid = DecoderMgr.FindLastMatch(argline, argfunctionname + @"\(.*?\)").Value;
                                            uid = uid.Substring(uid.IndexOf("(") + 1, uid.LastIndexOf(")") - uid.IndexOf("(") - 2);//多减少一位是因为最后还有一位字母u
                                            string pname = "";
                                            if (Singleton<DecoderMgr>.GetInstance().tryGetVariable(filePath, uid, out pname))
                                            {
                                                string type = DecoderMgr.FindFirstMatch(argline, @"[\S]* ").Value.Replace(" ", "");
                                                if (!DecoderMgr.isSupportedClass(type))
                                                {
                                                    type = "string";
                                                }
                                                //node.Properties["Opr"] = string.Format("{0} Self.{1}::{2}", type, DecoderMgr.Node.AgentType, pname);
                                                args[i] = string.Format("{0} Self.{1}::{2}", type, DecoderMgr.Node.AgentType, pname);
                                            }
                                            else
                                            {
                                                Debug.LogError("没有通过UID : " + uid + " 找到对应的属性名字");
                                            }
                                        }
                                        else
                                        {
                                            Debug.LogError("理论是不可能走到这里的!");
                                        }
                                    }
                                    else
                                    {
                                        //到这里应该是常量类型赋值 
                                        string type = DecoderMgr.FindFirstMatch(argline, @"[\S]* ").Value.Replace(" ", "");
                                        if (Regex.IsMatch(argline, " " + args[i] + " "))
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
                                            if (DecoderMgr.SearchLine(filePath, args[i].Replace("this.", "") + @"\;", out typeline))
                                            {
                                                //type = DecoderMgr.FindFirstMatch(typeline, @"[\S]* ").Value.Replace(" ", "");
                                                type = typeline.Substring(0, typeline.LastIndexOf(args[i].Replace("this.", "")));
                                                type = type.Replace("private", "");
                                                type = type.Replace(" ", "");
                                                type = type.Replace("\t", "");
                                            }
                                            else
                                            {
                                                Debug.LogError(filePath + "未能找到" + args[i] + "的类型，请检查" + " regex |" + args[i] + @"\;" + "|");
                                            }
                                        }
                                        if (!DecoderMgr.isSupportedClass(type))
                                        {
                                            type = "string";
                                        }
                                        string value = DecoderMgr.FindLastMatch(argline, @" [\S]*\;").Value.Replace(";", "").Replace(" ", "");
                                        value = DecoderMgr.FixPropertyString(type, value);
                                        args[i] = /*"const " + type + " " +*/ value;
                                    }
                                }
                                else
                                {
                                    Debug.LogError("文件:" + filePath + " 未能找到属性 " + args[i] + "正则:" + args[i].Replace(".", @"\.") + @".*\=.*;");
                                }
                            }
                        }
                    }
                }
                argsstr = "";
                for (int i = 0, imax = args.Length; i < imax; i ++)
                {
                    argsstr += args[i];
                    if (i != imax - 1)
                    {
                        argsstr += ",";
                    }
                }
            }

            string action = string.Format("Self.{0}::{1}({2})",/*agentname*/DecoderMgr.Node.AgentType , functionname, argsstr);

            node.Properties["Method"] = action;

            //Singleton<DecoderMgr>.GetInstance().AddMethod("string", functionname, args);
        }
        else
        {
            Debug.LogError(node.Name + "找不到对应的方法调用！");
        }
    }
}
