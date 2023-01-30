using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using AutoMapper.QueryableExtensions;
using System.Linq;
using System.Threading.Tasks;
using MeetCoVideoStreamingWebApi.Interfaces;
using MeetCoVideoStreamingWebApi.Data;
using MeetCoVideoStreamingWebApi.Dtos;
using MeetCoVideoStreamingWebApi.Entities;
using MeetCoVideoStreamingWebApi.Helpers;

namespace MeetCoVideoStreamingWebApi.Repository
{
    public class MeetingRepository : IMeetingRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public MeetingRepository(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Meeting> GetMeetingById(int MeetingId)
        {
            return await _context.Meetings.Include(x => x.Connections).FirstOrDefaultAsync(x => x.MeetingId == MeetingId);
        }

        public async Task<MeetingDto> GetMeetingDtoById(int MeetingId)
        {
            return await _context.Meetings.Where(r => r.MeetingId == MeetingId).ProjectTo<MeetingDto>(_mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();//using Microsoft.EntityFrameworkCore;
        }

        public async Task<Meeting> GetMeetingForConnection(string connectionId)
        {
            return await _context.Meetings.Include(x => x.Connections)
                .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public void AddMeeting(Meeting Meeting)
        {
            _context.Meetings.Add(Meeting);
        }

        public async Task<Meeting> DeleteMeeting(int id)
        {
            var Meeting = await _context.Meetings.FindAsync(id);
            if (Meeting != null)
            {
                _context.Meetings.Remove(Meeting);
            }
            return Meeting;
        }

        public async Task<Meeting> EditMeeting(int id, string newName)
        {
            var Meeting = await _context.Meetings.FindAsync(id);
            if (Meeting != null)
            {
                Meeting.MeetingName = newName;
            }
            return Meeting;
        }

        public async Task DeleteAllMeeting()
        {
            var list = await _context.Meetings.ToListAsync();
            _context.RemoveRange(list);
        }

        public async Task<PagedList<MeetingDto>> GetAllMeetingAsync(MeetingParams MeetingParams)
        {
            var list = _context.Meetings.AsQueryable();
            return await PagedList<MeetingDto>.CreateAsync(list.ProjectTo<MeetingDto>(_mapper.ConfigurationProvider).AsNoTracking(), MeetingParams.PageNumber, MeetingParams.PageSize);
        }

        public async Task UpdateCountMember(int MeetingId, int count)
        {
            var Meeting = await _context.Meetings.FindAsync(MeetingId);
            if (Meeting != null)
            {
                Meeting.CountMember = count;
            }
        }
    }
}
