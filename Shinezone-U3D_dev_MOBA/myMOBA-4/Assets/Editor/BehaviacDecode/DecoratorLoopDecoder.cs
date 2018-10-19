using UnityEngine;
using System.Collections;
using System;

public class DecoratorLoopDecoder : BehaviorNodeDecoder {

    public override string ClassName()
    {
        return "DecoratorLoop";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.DecoratorLoop";
    }

    public override void DecoderClassFile(string filePath, DecoderMgr.Node node)
    {
        //DoneWithinFrame
        string line;
        if (DecoderMgr.SearchLine(filePath, @"m\_bDecorateWhenChildEnds", out line))
        {
            string arg = line.Substring(line.LastIndexOf("=") + 1);
            arg = arg.Replace(" ","");
            arg = arg.Replace(";", "");
            node.Properties["DoneWithinFrame"] = "false";
            if (arg == "false")
            {
                node.Properties["DoneWithinFrame"] = "true";
            }
        }
        else
        {
            node.Properties["DoneWithinFrame"] = "false";
        }
        //Count
        if (DecoderMgr.SearchLine(filePath, @"return", out line))
        {
            string arg = line.Substring(line.LastIndexOf("return") + "return".Length);
            arg = arg.Replace(" ","");
            arg = arg.Replace(";", "");
            int count = -1;
            if (int.TryParse(arg,out count))
            {
                node.Properties["Count"] = "const int " + count.ToString();
            }
            else
            {
                Debug.LogError("loop返回值为非数字:" + arg);
                node.Properties["Count"] = "const int " + count.ToString();
            }
        }
        else
        {
            node.Properties["Count"] = "const int -1";
        }
    }
}
