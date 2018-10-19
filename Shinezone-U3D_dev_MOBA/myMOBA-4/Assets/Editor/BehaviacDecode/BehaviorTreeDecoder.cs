using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class BehaviorTreeDecoder : BehaviorNodeDecoder {

    public override string ClassName()
    {
        return "Behavior";
    }

    public override string FullClassName()
    {
        return "Behaviac.Design.Nodes.Behavior";
    }

    public override Dictionary<string, string> PropertiesTemplate()
    {
        Dictionary<string, string> properties = new Dictionary<string, string>();

        properties["Class"] = FullClassName();
        properties["AgentType"] = "BattleLogic::HeroAI";
        properties["Domains"] = "";
        properties["Enable"] = "True";
        properties["HasOwnPrefabData"] = "false";
        properties["Id"] = "0";
        properties["PrefabName"] = "";
        properties["PrefabNodeId"] = "-1";

        return properties;
    }
}
