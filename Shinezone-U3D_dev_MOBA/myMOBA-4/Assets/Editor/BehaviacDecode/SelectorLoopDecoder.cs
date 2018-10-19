using UnityEngine;
using System.Collections;

public class SelectorLoopDecoder : BehaviorNodeDecoder
{
    public override string ClassName()
    {
        return "SelectorLoop";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.SelectorLoop";
    }

    public override bool UseConnectorMultiple()
    {
        return true;
    }
}
