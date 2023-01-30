using MeetCoVideoStreamingWebApi.Dtos;
using MeetCoVideoStreamingWebApi.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetCoVideoStreamingWebApi.SignalRHub
{
    public class UserShareScreenTracker
    {
        private static readonly List<UserConnectionInfo> usersShareScreen = new List<UserConnectionInfo>();

        public Task<bool> UserConnectedToShareScreen(UserConnectionInfo user)
        {
            bool isOnline = false;
            lock (usersShareScreen)
            {
                var temp = usersShareScreen.FirstOrDefault(x => x.UserName == user.UserName && x.MeetingId == user.MeetingId);

                if (temp == null)
                {
                    usersShareScreen.Add(user);
                    isOnline = true;
                }
            }
            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnectedShareScreen(UserConnectionInfo user)
        {
            bool isOffline = false;
            lock (usersShareScreen)
            {
                var temp = usersShareScreen.FirstOrDefault(x => x.UserName == user.UserName && x.MeetingId == user.MeetingId);
                if (temp == null)
                    return Task.FromResult(isOffline);
                else
                {
                    usersShareScreen.Remove(temp);
                    isOffline = true;
                }
            }
            return Task.FromResult(isOffline);
        }

        public Task<UserConnectionInfo> GetUserIsSharing(int MeetingId)
        {
            UserConnectionInfo temp = null;
            lock (usersShareScreen)
            {
                temp = usersShareScreen.FirstOrDefault(x => x.MeetingId == MeetingId);
            }
            return Task.FromResult(temp);
        }

        public Task<bool> DisconnectedByUser(string username, int MeetingId)
        {
            bool isOffline = false;
            lock (usersShareScreen)
            {
                var temp = usersShareScreen.FirstOrDefault(x => x.UserName == username && x.MeetingId == MeetingId);
                if (temp != null)
                {
                    isOffline = true;
                    usersShareScreen.Remove(temp);
                }
            }
            return Task.FromResult(isOffline);
        }
    }
}
