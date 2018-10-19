using UnityEngine;
using System.Collections;

public class IfElseDecoder : BehaviorNodeDecoder
{
    public override string ClassName()
    {
        return "IfElse";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.IfElse";
    }

    public override string GetIdentifierName(int index)
    {
        switch (index)
        {
            case 0:
                return "_condition";
            case 1:
                return "_if";
            case 2:
            default:
                return "_else";
        }
    }
}
