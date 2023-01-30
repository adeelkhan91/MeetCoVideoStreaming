using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetCoVideoStreamingWebApi.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IMeetingRepository MeetingRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}
