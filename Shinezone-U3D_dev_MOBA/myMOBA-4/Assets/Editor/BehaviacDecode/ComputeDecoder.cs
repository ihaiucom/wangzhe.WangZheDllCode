using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ComputeDecoder : BehaviorNodeDecoder
{
    public override string ClassName()
    {
        return "Compute";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.Compute";
    }

    public override void DecoderClassFile(string filePath, DecoderMgr.Node node)
    {
        //Opl
        string line = "";
        if (DecoderMgr.SearchLine(filePath, @"pAgent\.SetVariable\<.*\>", out line))
        {
            string typestr = DecoderMgr.FindFirstMatch(line, @"\<.*\>").Value;
            typestr = typestr.Substring(1, typestr.Length - 2);
            if (!DecoderMgr.isSupportedClass(typestr))
            {
                typestr = "string";
                string paramstr = DecoderMgr.FindFirstMatch(line, "\".*?\"").Value;
                paramstr = paramstr.Substring(1, paramstr.Length - 2);

                node.Properties["Opl"] = string.Format("{0} Self.{1}::{2}", typestr, DecoderMgr.Node.AgentType, paramstr);
            }
            else
            {
                string paramstr = DecoderMgr.FindFirstMatch(line, "\".*?\"").Value;
                paramstr = paramstr.Substring(1, paramstr.Length - 2);

                node.Properties["Opl"] = string.Format("{0} Self.{1}::{2}", typestr, DecoderMgr.Node.AgentType, paramstr);
            }

            string argStr = DecoderMgr.FindLastMatch(line, @"\(.*\)").Value;
            argStr = argStr.Substring(1, argStr.Length - 2);
            string opArg = argStr.Split(',')[1];
            opArg = opArg.Replace(" ", "");
            string opArgStr;
            if (DecoderMgr.SearchLine(filePath, opArg + " =", out opArgStr))
            {
                string op = opArgStr.Substring(opArgStr.LastIndexOf(" ") - 1, 1);
                //Operator
                node.Properties["Operator"] = DecoderMgr.ParseOperator(op);

                string arg1 = opArgStr.Substring(opArgStr.IndexOf("=") + 1, opArgStr.LastIndexOf(op) - opArgStr.IndexOf("=") - 1);
                arg1 = arg1.Replace(" ", "");
                Match m = DecoderMgr.FindFirstMatch(arg1, @"\(.*\)");
                if (m != null && m.Length > 0)
                {
                    arg1 = arg1.Replace(m.Value, "");
                    arg1 = arg1.Replace("(", "");
                    arg1 = arg1.Replace(")", "");
                }
                string arg2 = opArgStr.Substring(opArgStr.LastIndexOf(op) + 1, opArgStr.LastIndexOf(";") - opArgStr.LastIndexOf(op) - 1);
                arg2 = arg2.Replace(" ", "");
                arg2 = arg2.Replace("(", "");
                arg2 = arg2.Replace(")", "");

                //Opr1
                node.Properties["Opr1"] = DecoderMgr.FindVariableAndParse(filePath, arg1, true);
                //Opr2
                node.Properties["Opr2"] = DecoderMgr.FindVariableAndParse(filePath, arg2, true);
            }
            else
            {
                Debug.LogError(filePath + "找不到" + opArg + "的赋值语句");
            }
        }
        else
        {
            Debug.LogError(filePath + "找不到赋值方法，请检查");
        }
    }
}
