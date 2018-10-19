using UnityEngine;
using System.Collections;

public class ConditionDecoder : BehaviorNodeDecoder {


    public override string ClassName()
    {
        return "Condition";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.Condition";
    }

    public override string GetIdentifierName(int index)
    {
        switch (index)
        {
            case 0:
                return "ConditionTrue";
            case 1:
            default:
                return "ConditionFalse";
        }
    }

    public override void DecoderClassFile(string filePath, DecoderMgr.Node node)
    {
        //Opl
        //Operator
        //Opr
        string line;
        if (DecoderMgr.SearchLine(filePath, @"return.*\?.*EBTStatus.*\EBTStatus.*", out line))
        {
            string returnarg = DecoderMgr.FindFirstMatch(line, @"\(\!.*\)").Value;
            returnarg = returnarg.Substring(2, returnarg.Length - 3);

            string rightEquation;
            if (DecoderMgr.SearchLine(filePath, returnarg + @" \=", out rightEquation))
            {
                rightEquation = rightEquation.Substring(rightEquation.IndexOf("=") + 1);
                string op, opinxml;
                if (DecoderMgr.FindOperator(rightEquation, out op, out opinxml))
                {
                    string opl = rightEquation.Substring(0, rightEquation.IndexOf(op) - 1);
                    opl = opl.Replace(" ", "");
                    opl = opl.Replace(";", "");
                    string opr = rightEquation.Substring(rightEquation.IndexOf(op) + op.Length);
                    opr = opr.Replace(" ", "");
                    opr = opr.Replace(";", "");


                    node.Properties["Operator"] = opinxml;
                    node.Properties["Opl"] = DecoderMgr.FindVariableAndParse(filePath, opl, true);
                    node.Properties["Opr"] = DecoderMgr.FindVariableAndParse(filePath, opr, true);
                }
                else
                {
                    Debug.LogError("未能在\"" + rightEquation + "\"中找到标志标记");
                }
            }
            else
            {
                Debug.LogError("未能在\"" + filePath + "\"中找到" + returnarg + @" \=");
            }
        }
        else
        {
            Debug.LogError(filePath + " 找不到正确的返回语句 Regex : " + @"return.*\?.*EBTStatus.*\EBTStatus.*");
        }
    }
}
