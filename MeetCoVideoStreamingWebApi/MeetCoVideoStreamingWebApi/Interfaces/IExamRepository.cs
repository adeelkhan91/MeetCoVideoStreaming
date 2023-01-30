using MeetCoVideoStreamingWebApi.Dtos;
using MeetCoVideoStreamingWebApi.Entities;
using MeetCoVideoStreamingWebApi.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetCoVideoStreamingWebApi.Interfaces
{
    public interface IMeetingRepository
    {
        Task<Meeting> GetMeetingById(int MeetingId);
        Task<Meeting> GetMeetingForConnection(string connectionId);
        void RemoveConnection(Connection connection);
        void AddMeeting(Meeting Meeting);
        Task<Meeting> DeleteMeeting(int id);
        Task<Meeting> EditMeeting(int id, string newName);
        Task DeleteAllMeeting();
        Task<PagedList<MeetingDto>> GetAllMeetingAsync(MeetingParams roomParams);
        Task<MeetingDto> GetMeetingDtoById(int roomId);
        Task UpdateCountMember(int roomId, int count);
    }
}
