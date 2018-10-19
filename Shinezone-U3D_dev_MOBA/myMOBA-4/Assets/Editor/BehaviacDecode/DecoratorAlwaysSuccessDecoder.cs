using UnityEngine;
using System.Collections;

public class DecoratorAlwaysSuccessDecoder : BehaviorNodeDecoder {
    public override string ClassName()
    {
        return "DecoratorAlwaysSuccess";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.DecoratorAlwaysSuccess";
    }
}
