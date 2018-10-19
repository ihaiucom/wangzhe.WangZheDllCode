using System;
using System.Collections;
using Photon.Hive;
using Photon.Hive.Plugin;

namespace TestPlugins
{
    class RaiseEventChecksPlugin : PluginBase
    {
        public override string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            var res = this.CheckPreConditions(info);
            if (!string.IsNullOrEmpty(res))
            {
                info.Fail(res);
                return;
            }
            try
            {
                base.OnRaiseEvent(info);
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastErrorInfoEvent(e.ToString(), info);
                return;
            }

            res = this.CheckPostConditions(info);
            if (!string.IsNullOrEmpty(res))
            {
                this.PluginHost.BroadcastErrorInfoEvent(res, info);
            }
        }

        private string CheckPostConditions(IRaiseEventCallInfo info)
        {
            var game = (HiveGame)this.PluginHost;
            if (info.Request.EvCode == 1)
            {
                var exectedCount = (int) ((Hashtable) info.Request.Data)[0];
                var slice = game.EventCache.Slice;
                if (exectedCount != game.EventCache.GetSliceSize(slice))
                {
                    return string.Format("Slice {0} does not contain expected count ({1}) of events. It has {2}",
                        slice, exectedCount, game.EventCache.GetSliceSize(slice));
                }
            }
            else if (info.Request.EvCode == 2)
            {
                var expectedCount = (int)((Hashtable)info.Request.Data)[0];
                if (expectedCount != game.EventCache.Count)
                {
                    return string.Format("Expected slices count = {0}. but it was {1}", expectedCount, game.EventCache.Count);
                }
            }
            else if (info.Request.EvCode == 3)
            {
                var expectedSlice = (int)((Hashtable)info.Request.Data)[0];
                if (expectedSlice != game.EventCache.Slice)
                {
                    return string.Format("Unexpected slice index {0}. Expected is {1}", game.EventCache.Slice, expectedSlice);
                }
            }
            return string.Empty;
        }

        private string CheckPreConditions(IRaiseEventCallInfo info)
        {
            return string.Empty;
        }
    }
}
