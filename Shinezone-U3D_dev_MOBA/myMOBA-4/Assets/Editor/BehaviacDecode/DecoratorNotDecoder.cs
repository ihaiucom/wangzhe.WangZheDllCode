using UnityEngine;
using System.Collections;

public class DecoratorNotDecoder : BehaviorNodeDecoder {
    public override string ClassName()
    {
        return "DecoratorNot";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.DecoratorNot";
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

            node.Properties["DecorateWhenChildEnds"] = arg;
        }
        else
        {
            node.Properties["DecorateWhenChildEnds"] = "true";
        }
    }
}
