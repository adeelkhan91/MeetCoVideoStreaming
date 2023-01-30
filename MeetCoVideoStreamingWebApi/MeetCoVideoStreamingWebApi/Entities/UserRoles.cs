using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetCoVideoStreamingWebApi.Entities
{
    public class UserRoles : IdentityUserRole<Guid>
    {
        public Users User { get; set; }
        public Roles Role { get; set; }
    }
}
