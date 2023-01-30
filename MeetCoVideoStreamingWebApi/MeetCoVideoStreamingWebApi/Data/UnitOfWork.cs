using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MeetCoVideoStreamingWebApi.Interfaces;
using MeetCoVideoStreamingWebApi.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetCoVideoStreamingWebApi.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        DataContext _context;
        IMapper _mapper;

        public UnitOfWork(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IUserRepository UserRepository => new UserRepository(_context, _mapper);
        public IMeetingRepository MeetingRepository => new MeetingRepository(_context, _mapper);

        public async Task<bool> Complete()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
