using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetCoVideoStreamingWebApi.Dtos
{
    public class UserConnectionInfo
    {
        public UserConnectionInfo() { }
        public UserConnectionInfo(string userName, int MeetingId)
        {
            UserName = userName;
            MeetingId = MeetingId;
        }
        public string UserName { get; set; }
        public int MeetingId { get; set; }
    }
}
