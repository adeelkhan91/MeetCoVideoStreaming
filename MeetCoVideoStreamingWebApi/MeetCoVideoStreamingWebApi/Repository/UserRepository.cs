using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MeetCoVideoStreamingWebApi.Data;
using MeetCoVideoStreamingWebApi.Dtos;
using MeetCoVideoStreamingWebApi.Entities;
using MeetCoVideoStreamingWebApi.Helpers;
using MeetCoVideoStreamingWebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetCoVideoStreamingWebApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Users> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<Users> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.SingleOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<MemberDto> GetMemberAsync(string username)
        {
            return await _context.Users.Where(x => x.UserName == username)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)//add CreateMap<AppUser, MemberDto>(); in AutoMapperProfiles
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MemberDto>> GetUsersOnlineAsync(UserConnectionInfo[] userOnlines)
        {
            var listUserOnline = new List<MemberDto>();
            foreach (var u in userOnlines)
            {
                var user = await _context.Users.Where(x => x.UserName == u.UserName)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

                listUserOnline.Add(user);
            }
            //return await Task.Run(() => listUserOnline.ToList());
            return await Task.FromResult(listUserOnline.ToList());
        }

        public async Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();
            query = query.Where(u => u.UserName != userParams.CurrentUsername).OrderByDescending(u => u.LastActive);

            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking(), userParams.PageNumber, userParams.PageSize);
        }

        public async Task<IEnumerable<MemberDto>> SearchMemberAsync(string displayname)
        {
            return await _context.Users.Where(u => u.DisplayName.ToLower().Contains(displayname.ToLower()))
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<Users> UpdateLocked(string username)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == username);
            if(user != null)
            {
                user.Locked = !user.Locked;
            }
            return user;
        }
    }
}
