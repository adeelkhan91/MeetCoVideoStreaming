using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MeetCoVideoStreamingWebApi.ApplicationExtentions;
using MeetCoVideoStreamingWebApi.Dtos;
using MeetCoVideoStreamingWebApi.Helpers;
using MeetCoVideoStreamingWebApi.Interfaces;
using MeetCoVideoStreamingWebApi.SignalRHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetCoVideoStreamingWebApi.Controllers
{
    [Authorize]
    public class MemberController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<ActiveHub> _activeHubHub;
        private readonly PresenceTracker _presenceTracker;

        public MemberController(IUnitOfWork unitOfWork, IHubContext<ActiveHub> activeHubHub, PresenceTracker presenceTracker)
        {
            _unitOfWork = unitOfWork;
            _activeHubHub = activeHubHub;
            _presenceTracker = presenceTracker;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetAllMembers([FromQuery] UserParams userParams)
        {
            userParams.CurrentUsername = User.GetUsername();
            var comments = await _unitOfWork.UserRepository.GetMembersAsync(userParams);
            Response.AddPaginationHeader(comments.CurrentPage, comments.PageSize, comments.TotalCount, comments.TotalPages);

            return Ok(comments);
        }

        [HttpGet("{username}")] // member/username
        public async Task<ActionResult<MemberDto>> GetMember(string username)
        {
            return Ok(await _unitOfWork.UserRepository.GetMemberAsync(username));
        }

        [HttpPut("{username}")]
        public async Task<ActionResult> LockedUser(string username)
        {
            var u = await _unitOfWork.UserRepository.UpdateLocked(username);
            if (u != null)
            {
                var connections = await _presenceTracker.GetConnectionsForUsername(username);
                await _activeHubHub.Clients.Clients(connections).SendAsync("OnLockedUser", true);
                return NoContent();
            }
            else
            {
                return BadRequest("Can not find given username");
            }
        }
    }
}
