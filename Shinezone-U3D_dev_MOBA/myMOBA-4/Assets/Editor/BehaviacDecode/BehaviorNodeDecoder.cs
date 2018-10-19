using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public abstract class BehaviorNodeDecoder  {

    public virtual void DecoderClassFile(string filePath, DecoderMgr.Node node)
    {
    }

    public virtual bool UseConnectorMultiple()
    {
        return false;
    }

    public virtual string GetIdentifierName(int index)
    {
        return "GenericChildren";
    }

    public abstract string ClassName();

    public abstract string FullClassName();

    public virtual Dictionary<string, string> PropertiesTemplate()
    {
        Dictionary<string, string> properties = new Dictionary<string, string>();

        properties.Add("Class", FullClassName());
        properties.Add("Enable", "true");
        properties.Add("HasOwnPrefabData", "false");
        properties.Add("Id", "0");
        properties.Add("PrefabName", "");
        properties.Add("PrefabNodeId", "-1");

        return properties;
    }

    public virtual void Process(List<DecoderMgr.Node> nodes, DecoderMgr.Node node, string methodname, string argsStr)
    {
        switch (methodname)
        {
            case "SetId":
                node.Properties["Id"] = argsStr;
                break;
            case "AddPar":
                string[] args = argsStr.Split(',');
                if (args != null && args.Length == 4)
                {
                    Singleton<DecoderMgr>.GetInstance().AddProperty(args[0].Replace(" ",""), args[1].Replace(" ", ""), args[2].Replace(" ", ""));
                }
                else
                {
                    Debug.LogError("AddPar 参数格式错误 : " + argsStr);
                }
                break;
            case "SetClassNameString":
                break;
            case "SetName":
                break;
            case "SetHasEvents":
                break;
            case "AddChild":
                {
                    if (node.Connectors == null)
                    {
                        node.Connectors = new List<DecoderMgr.Connector>();
                    }

                    if (this.UseConnectorMultiple())
                    {
                        if (node.Connectors == null || node.Connectors.Count == 0)
                        {
                            DecoderMgr.Connector connector = new DecoderMgr.Connector();
                            node.Connectors.Add(connector);
                        }
                        node.Connectors[0].Identifier = GetIdentifierName(0);
                        if (node.Connectors[0].Nodes == null)
                        {
                            node.Connectors[0].Nodes = new List<DecoderMgr.Node>();
                        }
                        node.Connectors[0].Nodes.Add(nodes.Find(x => x.Name == argsStr));
                    }
                    else
                    {

                        DecoderMgr.Connector connector = new DecoderMgr.Connector();
                        connector.Identifier = GetIdentifierName(node.Connectors.Count);
                        connector.Nodes = new List<DecoderMgr.Node>();
                        connector.Nodes.Add(nodes.Find(x => x.Name == argsStr));
                        //connector.node = nodes.Find(x => x.Name == argsStr);
                        node.Connectors.Add(connector);
                    }


                }
                break;
            default:
                Debug.LogWarning(methodname + " 没有对应的处理方法");
                break;
        }
    }
}
