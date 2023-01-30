using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MeetCoVideoStreamingWebApi.ApplicationExtentions;
using MeetCoVideoStreamingWebApi.Dtos;
using MeetCoVideoStreamingWebApi.Entities;
using MeetCoVideoStreamingWebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetCoVideoStreamingWebApi.SignalRHub
{
    [Authorize]
    public class ChatHub : Hub
    {
        //IMapper _mapper;
        IHubContext<ActiveHub> _presenceHub;
        PresenceTracker _presenceTracker;
        IUnitOfWork _unitOfWork;
        UserShareScreenTracker _shareScreenTracker;

        public ChatHub(IUnitOfWork unitOfWork, UserShareScreenTracker shareScreenTracker, PresenceTracker presenceTracker, IHubContext<ActiveHub> presenceHub)
        {
            //_mapper = mapper;
            _unitOfWork = unitOfWork;
            _presenceTracker = presenceTracker;
            _presenceHub = presenceHub;
            _shareScreenTracker = shareScreenTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var MeetingId = httpContext.Request.Query["MeetingId"].ToString();
            var MeetingIdInt = int.Parse(MeetingId);
            var username = Context.User.GetUsername();

            await _presenceTracker.UserConnected(new UserConnectionInfo(username, MeetingIdInt), Context.ConnectionId);

            await Groups.AddToGroupAsync(Context.ConnectionId, MeetingId);//khi user click vao Meeting se join vao
            await AddConnectionToGroup(MeetingIdInt); // luu db DbSet<Connection> de khi disconnect biet

            //var usersOnline = await _unitOfWork.UserRepository.GetUsersOnlineAsync(currentUsers);
            var oneUserOnline = await _unitOfWork.UserRepository.GetMemberAsync(username);
            await Clients.Group(MeetingId).SendAsync("UserOnlineInGroup", oneUserOnline);

            var currentUsers = await _presenceTracker.GetOnlineUsers(MeetingIdInt);
            await _unitOfWork.MeetingRepository.UpdateCountMember(MeetingIdInt, currentUsers.Length);
            await _unitOfWork.Complete();

            var currentConnections = await _presenceTracker.GetConnectionsForUser(new UserConnectionInfo(username, MeetingIdInt));
            await _presenceHub.Clients.AllExcept(currentConnections).SendAsync("CountMemberInGroup",
                   new { MeetingId = MeetingIdInt, countMember = currentUsers.Length });

            //share screen user vao sau cung
            var userIsSharing = await _shareScreenTracker.GetUserIsSharing(MeetingIdInt);
            if (userIsSharing != null)
            {
                var currentBeginConnectionsUser = await _presenceTracker.GetConnectionsForUser(userIsSharing);
                if (currentBeginConnectionsUser.Count > 0)
                    await Clients.Clients(currentBeginConnectionsUser).SendAsync("OnShareScreenLastUser", new { usernameTo = username, isShare = true });
                await Clients.Caller.SendAsync("OnUserIsSharing", userIsSharing.UserName);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var username = Context.User.GetUsername();
            var group = await RemoveConnectionFromGroup();
            var isOffline = await _presenceTracker.UserDisconnected(new UserConnectionInfo(username, group.MeetingId), Context.ConnectionId);

            await _shareScreenTracker.DisconnectedByUser(username, group.MeetingId);
            if (isOffline)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, group.MeetingId.ToString());
                var temp = await _unitOfWork.UserRepository.GetMemberAsync(username);
                await Clients.Group(group.MeetingId.ToString()).SendAsync("UserOfflineInGroup", temp);

                var currentUsers = await _presenceTracker.GetOnlineUsers(group.MeetingId);

                await _unitOfWork.MeetingRepository.UpdateCountMember(group.MeetingId, currentUsers.Length);
                await _unitOfWork.Complete();

                await _presenceHub.Clients.All.SendAsync("CountMemberInGroup",
                       new { MeetingId = group.MeetingId, countMember = currentUsers.Length });
            }
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var userName = Context.User.GetUsername();
            var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(userName);

            var group = await _unitOfWork.MeetingRepository.GetMeetingForConnection(Context.ConnectionId);

            if (group != null)
            {
                var message = new MessageDto
                {
                    SenderUsername = userName,
                    SenderDisplayName = sender.DisplayName,
                    Content = createMessageDto.Content,
                    MessageSent = DateTime.Now
                };
                //Luu message vao db
                //code here
                //send meaasge to group
                await Clients.Group(group.MeetingId.ToString()).SendAsync("NewMessage", message);
            }
        }

        public async Task MuteMicro(bool muteMicro)
        {
            var group = await _unitOfWork.MeetingRepository.GetMeetingForConnection(Context.ConnectionId);
            if (group != null)
            {
                await Clients.Group(group.MeetingId.ToString()).SendAsync("OnMuteMicro", new { username = Context.User.GetUsername(), mute = muteMicro });
            }
            else
            {
                throw new HubException("group == null");
            }
        }

        public async Task MuteCamera(bool muteCamera)
        {
            var group = await _unitOfWork.MeetingRepository.GetMeetingForConnection(Context.ConnectionId);
            if (group != null)
            {
                await Clients.Group(group.MeetingId.ToString()).SendAsync("OnMuteCamera", new { username = Context.User.GetUsername(), mute = muteCamera });
            }
            else
            {
                throw new HubException("group == null");
            }
        }

        public async Task ShareScreen(int MeetingId, bool isShareScreen)
        {
            if (isShareScreen)//true is doing share
            {
                await _shareScreenTracker.UserConnectedToShareScreen(new UserConnectionInfo(Context.User.GetUsername(), MeetingId));
                await Clients.Group(MeetingId.ToString()).SendAsync("OnUserIsSharing", Context.User.GetUsername());
            }
            else
            {
                await _shareScreenTracker.UserDisconnectedShareScreen(new UserConnectionInfo(Context.User.GetUsername(), MeetingId));
            }
            await Clients.Group(MeetingId.ToString()).SendAsync("OnShareScreen", isShareScreen);
            //var group = await _unitOfWork.MeetingRepository.GetMeetingForConnection(Context.ConnectionId);
        }

        public async Task ShareScreenToUser(int MeetingId, string username, bool isShare)
        {
            var currentBeginConnectionsUser = await _presenceTracker.GetConnectionsForUser(new UserConnectionInfo(username, MeetingId));
            if (currentBeginConnectionsUser.Count > 0)
                await Clients.Clients(currentBeginConnectionsUser).SendAsync("OnShareScreen", isShare);
        }

        private async Task<Meeting> RemoveConnectionFromGroup()
        {
            var group = await _unitOfWork.MeetingRepository.GetMeetingForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            _unitOfWork.MeetingRepository.RemoveConnection(connection);

            if (await _unitOfWork.Complete()) return group;

            throw new HubException("Fail to remove connection from Meeting");
        }

        private async Task<Meeting> AddConnectionToGroup(int MeetingId)
        {
            var group = await _unitOfWork.MeetingRepository.GetMeetingById(MeetingId);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());
            if (group != null)
            {
                group.Connections.Add(connection);
            }

            if (await _unitOfWork.Complete()) return group;

            throw new HubException("Failed to add connection to Meeting");
        }
    }
}
