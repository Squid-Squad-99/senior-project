using System;
using Relay.Payload;

namespace RelayClient.Payload
{
    public class MsgPayload : BasePayload
    {
        public MsgPayload(byte[] body)
        {
            PayloadType = (Int32)Type.Msg;
            BodySize = body.Length;
            Body = body;
        }
    }
}