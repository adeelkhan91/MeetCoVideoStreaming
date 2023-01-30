using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MeetCoVideoStreamingWebApi.Entities
{
    public class Users : IdentityUser<Guid>
    {
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string DisplayName { get; set; }
        public bool Locked { get; set; } = false;// true = locked

        public string? PhotoUrl { get; set; }//Nullable<string>
        public ICollection<UserRoles> UserRoles { get; set; }
        public ICollection<Meeting> Meetings { get; set; }
    }
}