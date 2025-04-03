using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Escort
{
    public class LiveStreamInfo
    {
        public string? ChannelName { get; set; }
        public string? ChannelArn { get; set; }
        public string? StreamKey { get; set; }
        public int? ViewerCount { get; set; }
        public DateTime? StartTime { get; set; }
        public string? PlaybackUrl { get; set; }
        public string? Profile { get; set; }
        public string? DisplayName { get; set; }
        public int? EscortId { get; set; }
        public int Age { get; set; }

        public string? ReturnUrl { get; set; }
    }

    public class ReturnUrlParameters
    {
        public int Id { get; set; }
#nullable disable
        public string Arn { get; set; }
        public string PlaybackUrl { get; set; }
        public string ProfileImage { get; set; }
        public string Name { get; set; }
     
    }
}
