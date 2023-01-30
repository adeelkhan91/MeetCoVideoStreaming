using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetCoVideoStreamingWebApi.Dtos
{
    public class MeetingDto
    {
        public int MeetingId { get; set; }
        public string MeetingName { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public int CountMember { get; set; }
    }
}
