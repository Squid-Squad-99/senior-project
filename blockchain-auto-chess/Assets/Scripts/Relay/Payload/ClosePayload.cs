using System;

namespace Relay.Payload
{
    public class ClosePayload : BasePayload
    {
        public ClosePayload()
        {
            PayloadType = (Int32)Type.Close;
            BodySize = 0;
            Body = new byte[] { };
        }
    }
}