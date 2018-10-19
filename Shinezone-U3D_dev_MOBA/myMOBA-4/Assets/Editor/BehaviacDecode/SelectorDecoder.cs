using UnityEngine;
using System.Collections;

public class SelectorDecoder : BehaviorNodeDecoder
{
    public override string ClassName()
    {
        return "Selector";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.Selector";
    }

    public override bool UseConnectorMultiple()
    {
        return true;
    }
}
