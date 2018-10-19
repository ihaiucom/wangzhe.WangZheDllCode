using UnityEngine;
using System.Collections;

public class DecoratorLoopUntilDecoder : BehaviorNodeDecoder
{

    public override string ClassName()
    {
        return "DecoratorLoopUntil";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.DecoratorLoopUntil";
    }

    public override void DecoderClassFile(string filePath, DecoderMgr.Node node)
    {
        //Until
        string line;
        if (DecoderMgr.SearchLine(filePath, @"m\_until", out line))
        {
            string arg = line.Substring(line.LastIndexOf("=") + 1);
            arg = arg.Replace(" ", "");
            arg = arg.Replace(";", "");

            node.Properties["Until"] = arg;
        }
        else
        {
            node.Properties["Until"] = "true";
        }
        //DoneWithinFrame
        if (DecoderMgr.SearchLine(filePath, @"m\_bDecorateWhenChildEnds", out line))
        {
            string arg = line.Substring(line.LastIndexOf("=") + 1);
            arg = arg.Replace(" ", "");
            arg = arg.Replace(";", "");

            node.Properties["DoneWithinFrame"] =  arg;
        }
        else
        {
            node.Properties["DoneWithinFrame"] = "true";
        }
        //Count
        if (DecoderMgr.SearchLine(filePath, @"return", out line))
        {
            string arg = line.Substring(line.LastIndexOf("return") + "return".Length);
            arg = arg.Replace(" ", "");
            arg = arg.Replace(";", "");
            int count = -1;
            if (int.TryParse(arg, out count))
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
