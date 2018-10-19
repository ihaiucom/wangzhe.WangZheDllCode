using UnityEngine;
using System.Collections;

public class DecoratorAlwaysFailureDecoder : BehaviorNodeDecoder
{
    public override string ClassName()
    {
        return "DecoratorAlwaysFailure";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.DecoratorAlwaysFailure";
    }
}
