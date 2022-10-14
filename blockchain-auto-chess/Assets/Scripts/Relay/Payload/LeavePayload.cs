using System;

namespace Relay.Payload
{
    public class LeavePayload : BasePayload
    {
        public LeavePayload(int roomId)
        {
            PayloadType = (Int32)Type.Leave;
            BodySize = 4;
            Body = BitConverter.GetBytes(roomId);
        }
    }
}