using System;

namespace Relay.Payload
{
    public class PingPayload : BasePayload
    {
        public PingPayload()
        {
            PayloadType = (Int32)Type.Ping;
            BodySize = 0;
            Body = new byte[] { };
        }
    }
}