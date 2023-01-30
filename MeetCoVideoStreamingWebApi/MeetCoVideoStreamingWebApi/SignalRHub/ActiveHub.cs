using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MeetCoVideoStreamingWebApi.ApplicationExtentions;
using MeetCoVideoStreamingWebApi.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetCoVideoStreamingWebApi.SignalRHub
{
    [Authorize]
    public class ActiveHub : Hub
    {
        private readonly PresenceTracker _tracker;
        public ActiveHub(PresenceTracker tracker)
        {
            _tracker = tracker;
        }
        public override async Task OnConnectedAsync()
        {
            var isOnline = await _tracker.UserConnected(new UserConnectionInfo(Context.User.GetUsername(), 0), Context.ConnectionId);            
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var isOffline = await _tracker.UserDisconnected(new UserConnectionInfo(Context.User.GetUsername(), 0), Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
