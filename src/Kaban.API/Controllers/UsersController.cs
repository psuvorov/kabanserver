using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Kaban.API.Dto.Users;
using Kaban.API.Helpers;
using Kaban.Domain.Interfaces;
using Kaban.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Kaban.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public UsersController(IUserService userService, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _userService = userService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate-user")]
        public IActionResult Authenticate([FromBody]AuthenticateDto authenticateDto)
        {
            var currentUser = _userService.Authenticate(authenticateDto.Email, authenticateDto.Password);
            if (currentUser is null)
                return BadRequest(new { message = "Email or password is incorrect" });
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, currentUser.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var serializedToken = tokenHandler.WriteToken(securityToken);

            var authenticatedUserDto = _mapper.Map<AuthenticatedUserDto>(currentUser);
            authenticatedUserDto.Token = serializedToken;

            // return basic user info and authentication token
            return Ok(authenticatedUserDto);
        }

        [AllowAnonymous]
        [HttpPost("register-user")]
        public IActionResult Register([FromBody] RegisterUserDto registerDto)
        {
            // map model to entity
            var user = _mapper.Map<User>(registerDto);

            try
            {
                var createdUser = _userService.Create(user, registerDto.Password);

                var res = new ObjectResult(new { userId = createdUser.Id });
                res.StatusCode = StatusCodes.Status201Created;

                return res;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpGet("test-action")]
        public IActionResult TestAction()
        {
            return Ok("Okkk");
        }
        
    }
}