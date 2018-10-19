using UnityEngine;
using System.Collections;

public class FalseDecoder : BehaviorNodeDecoder
{
    public override string ClassName()
    {
        return "False";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.False";
    }
}
