using MeetCoVideoStreamingWebApi.Dtos;
using MeetCoVideoStreamingWebApi.Entities;
using MeetCoVideoStreamingWebApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetCoVideoStreamingWebApi.Interfaces
{
    public interface IUserRepository
    {
        Task<Users> GetUserByIdAsync(Guid id);
        Task<Users> GetUserByUsernameAsync(string username);
        Task<MemberDto> GetMemberAsync(string username);
        Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        Task<IEnumerable<MemberDto>> SearchMemberAsync(string displayname);
        Task<IEnumerable<MemberDto>> GetUsersOnlineAsync(UserConnectionInfo[] userOnlines);
        Task<Users> UpdateLocked(string username);
    }
}
