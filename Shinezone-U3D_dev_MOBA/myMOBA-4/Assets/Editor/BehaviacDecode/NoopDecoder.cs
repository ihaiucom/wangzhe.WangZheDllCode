using UnityEngine;
using System.Collections;

public class NoopDecoder : BehaviorNodeDecoder
{
    public override string ClassName()
    {
        return "Noop";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.Noop";
    }
}
