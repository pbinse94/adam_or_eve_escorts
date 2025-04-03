using Shared.Model.Base;
using Shared.Model.DTO;

namespace Shared.Model.Entities
{

    public class StreamRequest
    {
        public required string ChannelArn { get; set; }
    }

    public class ChatTokenRequest
    {
        public required string RoomIdentifier { get; set; }
        public required string UserId { get; set; }
        public System.Collections.Generic.Dictionary<string, string>? Attributes { get; set; }
        public System.Collections.Generic.List<string>? Capabilities { get; set; }
        public int? DurationInMinutes { get; set; }
    }
    public class ChatEventRequest
    {
        public string? Arn { get; set; }
        public string? EventName { get; set; }
        public System.Collections.Generic.Dictionary<string, string>? EventAttributes { get; set; }
    }
}
