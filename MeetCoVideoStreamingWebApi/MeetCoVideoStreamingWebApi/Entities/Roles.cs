using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetCoVideoStreamingWebApi.Entities
{
    public class Roles : IdentityRole<Guid>
    {
        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
