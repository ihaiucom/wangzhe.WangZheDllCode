using UnityEngine;
using System.Collections;

public class OrDecoder : BehaviorNodeDecoder {
    public override string ClassName()
    {
        return "Or";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.Or";
    }
}
