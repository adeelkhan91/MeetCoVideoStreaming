using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MeetCoVideoStreamingWebApi.ApplicationExtentions;
using MeetCoVideoStreamingWebApi.Dtos;
using MeetCoVideoStreamingWebApi.Entities;
using MeetCoVideoStreamingWebApi.Helpers;
using MeetCoVideoStreamingWebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetCoVideoStreamingWebApi.Controllers
{
    [Authorize]
    public class MeetingController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MeetingController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MeetingDto>>> GetAlMeetings([FromQuery] MeetingParams MeetingParams)
        {
            var comments = await _unitOfWork.MeetingRepository.GetAllMeetingAsync(MeetingParams);
            Response.AddPaginationHeader(comments.CurrentPage, comments.PageSize, comments.TotalCount, comments.TotalPages);

            return Ok(comments);
        }

        [HttpPost]
        public async Task<ActionResult> AddMeeting(string name)
        {
            var Meeting = new Meeting { MeetingName = name, UserId = User.GetUserId() };

            _unitOfWork.MeetingRepository.AddMeeting(Meeting);

            if (await _unitOfWork.Complete())
            {
                return Ok(await _unitOfWork.MeetingRepository.GetMeetingDtoById(Meeting.MeetingId));
            }

            return BadRequest("Meeting Added");
        }

        [HttpPut]
        public async Task<ActionResult> EditMeeting(int id, string editName)
        {
            var Meeting = await _unitOfWork.MeetingRepository.EditMeeting(id, editName);
            if (Meeting != null)
            {
                if (_unitOfWork.HasChanges())
                {
                    if (await _unitOfWork.Complete())
                        return Ok(new MeetingDto { MeetingId = Meeting.MeetingId, MeetingName = Meeting.MeetingName, UserId = Meeting.UserId.ToString() });
                    return BadRequest("Edit Meeting.");
                }
                else
                {
                    return NoContent();
                }
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMeeting(int id)
        {
            var entity = await _unitOfWork.MeetingRepository.DeleteMeeting(id);

            if (entity != null)
            {
                if (await _unitOfWork.Complete())
                    return Ok(new MeetingDto { MeetingId = entity.MeetingId, MeetingName = entity.MeetingName, UserId = entity.UserId.ToString() });
                return BadRequest("Meeting deleted");
            }
            else
            {
                return NotFound();
            }
        }

        [HttpDelete("delete-all")]
        public async Task<ActionResult> DeleteAllMeeting()
        {
            await _unitOfWork.MeetingRepository.DeleteAllMeeting();

            if (_unitOfWork.HasChanges())
            {
                if (await _unitOfWork.Complete())
                    return Ok();//xoa thanh cong
                return BadRequest("All Meeting deleted");
            }
            else
            {
                return NoContent();//ko co gi de xoa
            }
        }
    }
}
