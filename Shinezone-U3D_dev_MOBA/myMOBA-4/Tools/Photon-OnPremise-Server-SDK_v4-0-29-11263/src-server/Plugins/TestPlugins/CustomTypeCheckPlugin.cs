using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Photon.Hive.Plugin;

namespace TestPlugins
{
    public class CustomTypeCheckPlugin : PluginBase
    {
        public override string Name
        {
            get { return this.GetType().Name; }
        }

        class CustomPluginType
        {
            public int intField;
            public byte byteField;
            public string stringField;

            public CustomPluginType(byte[] bytes)
            {
                using (var s = new MemoryStream(bytes))
                using (var br = new BinaryReader(s))
                {
                    this.intField = br.ReadInt32();
                    this.byteField = br.ReadByte();
                    this.stringField = br.ReadString();
                }
            }

            public byte[] Serialize()
            {
                using (var s = new MemoryStream())
                using (var bw = new BinaryWriter(s))
                {
                    bw.Write(intField);
                    bw.Write(byteField);
                    bw.Write(stringField);

                    return s.ToArray();
                }
            }
        }
        public override bool SetupInstance(IPluginHost host, Dictionary<string, string> config, out string errorMsg)
        {
            host.TryRegisterType(typeof (CustomPluginType), 1, SerializeFunction, DeserializeFunction);
            return base.SetupInstance(host, config, out errorMsg);
        }

        private object DeserializeFunction(byte[] bytes)
        {
            return new CustomPluginType(bytes);
        }

        private byte[] SerializeFunction(object o)
        {
            return ((CustomPluginType) o).Serialize();
        }

        public override void OnRaiseEvent(IRaiseEventCallInfo info)
        {
            try
            {
                var data = (Hashtable) info.Request.Data;
                var customObj = (CustomPluginType) data[0];
                Assert.AreEqual(1, customObj.byteField);
                Assert.AreEqual(2, customObj.intField);
                Assert.AreEqual("3", customObj.stringField);

                customObj.byteField = 2;
                customObj.intField = 3;
                customObj.stringField = "4";

                var d = new Dictionary<byte, object>
                {
                    {0, customObj},
                };

                this.BroadcastEvent(123, d);
            }
            catch (Exception e)
            {
                this.PluginHost.BroadcastErrorInfoEvent(e.ToString());
                return;
            }
            base.OnRaiseEvent(info);
        }
    }
}
