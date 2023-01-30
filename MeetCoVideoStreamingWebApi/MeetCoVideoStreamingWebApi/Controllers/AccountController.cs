using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetCoVideoStreamingWebApi.Dtos;
using MeetCoVideoStreamingWebApi.Entities;
using MeetCoVideoStreamingWebApi.Interfaces;

namespace MeetCoVideoStreamingWebApi.Controllers
{
    [ApiController]
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<Users> _userManager;
        private readonly SignInManager<Users> _signInManager;

        public AccountController(ITokenService tokenService, IMapper mapper, UserManager<Users> userManager, SignInManager<Users> signInManager)
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _userManager= userManager;
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        //api/account/register?username=Test&password=hoainam10th with Register(string username, string password)
        public async Task<ActionResult<UserDto>> Register(RegisterDto register)
        {
            if (await UserExists(register.UserName))
                return BadRequest("Username is already exist.");

            var user = _mapper.Map<Users>(register);

            user.UserName = register.UserName.ToLower();

            var result = await _userManager.CreateAsync(user, register.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, register.RoleName);
            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            var userDto = new UserDto
            {
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                LastActive = user.LastActive,
                Token = await _tokenService.CreateTokenAsync(user),
                PhotoUrl = null
            };

            return Ok(userDto);
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users
                //.Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName == loginDto.UserName.ToLower());

            if (user == null)
                return Unauthorized("Invalid Username");

            if (user.Locked)//true = locked
                return BadRequest("This account is loked by admin");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized("Invalid password");

            var userDto = new UserDto
            {
                UserName = user.UserName,
                DisplayName = user.DisplayName,
                LastActive = user.LastActive,
                Token = await _tokenService.CreateTokenAsync(user),
                PhotoUrl = user.PhotoUrl
            };
            return Ok(userDto);
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}
