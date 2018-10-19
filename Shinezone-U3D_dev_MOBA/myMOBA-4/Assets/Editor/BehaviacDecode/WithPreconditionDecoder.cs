using UnityEngine;
using System.Collections;
using System;

public class WithPreconditionDecoder : BehaviorNodeDecoder {

    public override string ClassName()
    {
        return "WithPrecondition";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.WithPrecondition";
    }

    public override string GetIdentifierName(int index)
    {
        switch(index)
        {
            case 0:
                return "Precondition";
            case 1:
            default:
                return "Action";
        }
    }
}
