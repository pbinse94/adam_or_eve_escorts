using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Entities
{
    public class ClientStreamingViewModel
    {
        public int? Id { get; set; }
#nullable disable
        public int IpAddress { get; set; }
        public string RoomId { get; set; }
        public string PlaybackUrl { get; set; }
        public string UserName { get; set; }
        public string ProfileImage { get; set; }
       
        public string Name { get; set; }
        public int CreditBalance { get; set; }
    }
    public class StreamCheckModalViewModel
    {
        public string ChannelArn { get; set; }
        public string RoomArn { get; set; }
    }
}
