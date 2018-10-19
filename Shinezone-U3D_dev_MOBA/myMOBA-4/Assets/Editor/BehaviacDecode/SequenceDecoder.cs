using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SequenceDecoder : BehaviorNodeDecoder {

    public override string ClassName()
    {
        return "Sequence";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.Sequence";
    }

    public override bool UseConnectorMultiple()
    {
        return true;
    }

    //public override void Process(DecoderMgr.Node node, string methodname, string argsStr)
    //{
    //    switch (methodname)
    //    {
    //        case "SetId":
    //            node.Properties["Id"] = argsStr;
    //            break;
    //        case "AddPar":
    //            break;
    //        case "SetClassNameString":
    //            break;
    //        case "SetName":
    //            break;
    //        default:
    //            Debug.LogWarning("BehaviorTreeDecoder 对 " + methodname + " 没有对应的处理方法");
    //            break;
    //    }
    //}
}
