
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;

namespace Photon.Common.Authentication
{
    public class AuthenticationToken : DataContract
    {
        public AuthenticationToken(IRpcProtocol protocol, IDictionary<byte, object> dataMembers)
            : base(protocol, dataMembers)
        {
        }

        private static readonly IRpcProtocol serializationProtocol = Protocol.GpBinaryV17;

        // we are using the Version as "EventCode". 
        public AuthenticationToken()
        {
        }

        public byte Version
        {
            get
            {
                return 1;
            }
        }

        // converts the internal "ValidToTicks" expiration timestamp to a DateTime (based on UTC.Now) 
        public DateTime ValidTo
        {
            get
            {
                return new DateTime(this.ValidToTicks); 
            }
        }


        [Photon.SocketServer.Rpc.DataMember(Code = 1, IsOptional = false)]
        public long ValidToTicks { get; set; }
        
        [Photon.SocketServer.Rpc.DataMember(Code = 4, IsOptional = true)]
        public string UserId { get; set; }

        [Photon.SocketServer.Rpc.DataMember(Code = 8, IsOptional = true)]
        public Dictionary<string, object> AuthCookie { get; set; }

        [Photon.SocketServer.Rpc.DataMember(Code = 10, IsOptional = true)]
        public string SessionId { get; set; }

        [Photon.SocketServer.Rpc.DataMember(Code = 11, IsOptional = true)]
        public int Flags { get; set; }

        public virtual bool AreEqual(AuthenticationToken rhs)
        {
            return this.UserId == rhs.UserId && this.SessionId == rhs.SessionId;
        }

        public virtual byte[] Serialize()
        {
            return serializationProtocol.SerializeEventData(new EventData(this.Version, this));
        }

        public static AuthenticationToken Deserialize(byte[] data)
        {
            EventData eventData;
            if (!serializationProtocol.TryParseEventData(data, out eventData))
            {
                throw new SerializationException("Could not deserialize authentication token.");
            }

            var token = new AuthenticationToken(serializationProtocol, eventData.Parameters);

            return token;
        }

        public static bool TryDeserialize(byte[] data, out AuthenticationToken token)
        {
            token = null;
            EventData eventData;
            if (!serializationProtocol.TryParseEventData(data, out eventData))
            {
                return false;
            }

            // code = version
            switch (eventData.Code)
            {
                default:
                    return false;

                case 1:
                    token = new AuthenticationToken(serializationProtocol, eventData.Parameters);
                    return true;
            }
        }
    }
}
