using UnityEngine;
using System.Collections;

public class ParallelDecoder : BehaviorNodeDecoder
{

    public override string ClassName()
    {
        return "Parallel";
    }

    public override string FullClassName()
    {
        return "PluginBehaviac.Nodes.Parallel";
    }

    public override bool UseConnectorMultiple()
    {
        return true;
    }

    public override void DecoderClassFile(string filePath, DecoderMgr.Node node)
    {
        //FailurePolicy
        string line;
        if (DecoderMgr.SearchLine(filePath, @"m\_failPolicy", out line))
        {
            string arg = line.Substring(line.IndexOf("=") + 1);
            arg = arg.Replace(" ", "");
            arg = arg.Replace(";", "");

            node.Properties["FailurePolicy"] = arg.Substring(arg.IndexOf('.') + 1);
        }
        else
        {
            node.Properties["FailurePolicy"] = "FAIL_ON_ONE";
        }
        //SuccessPolicy
        if (DecoderMgr.SearchLine(filePath, @"m\_succeedPolicy", out line))
        {
            string arg = line.Substring(line.IndexOf("=") + 1);
            arg = arg.Replace(" ", "");
            arg = arg.Replace(";", "");

            node.Properties["SuccessPolicy"] = arg.Substring(arg.IndexOf('.') + 1);
        }
        else
        {
            node.Properties["SuccessPolicy"] = "SUCCEED_ON_ALL";
        }
        //ExitPolicy
        if (DecoderMgr.SearchLine(filePath, @"m\_exitPolicy", out line))
        {
            string arg = line.Substring(line.IndexOf("=") + 1);
            arg = arg.Replace(" ", "");
            arg = arg.Replace(";", "");

            node.Properties["ExitPolicy"] = arg.Substring(arg.IndexOf('.') + 1);
        }
        else
        {
            node.Properties["ExitPolicy"] = "EXIT_ABORT_RUNNINGSIBLINGS";
        }
        //ChildFinishPolicy
        if (DecoderMgr.SearchLine(filePath, @"m\_childFinishPolicy", out line))
        {
            string arg = line.Substring(line.IndexOf("=") + 1);
            arg = arg.Replace(" ", "");
            arg = arg.Replace(";", "");

            node.Properties["ChildFinishPolicy"] = arg.Substring(arg.IndexOf('.') + 1);
        }
        else
        {
            node.Properties["ChildFinishPolicy"] = "CHILDFINISH_LOOP";
        }
    }
}
