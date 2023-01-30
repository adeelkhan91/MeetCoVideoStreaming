using MeetCoVideoStreamingWebApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetCoVideoStreamingWebApi.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(Users appUser);
    }
}
