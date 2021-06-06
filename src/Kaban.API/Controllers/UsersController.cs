using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Kaban.API.Controllers.Requests.Users;
using Kaban.API.Controllers.Responses;
using Kaban.API.Controllers.Responses.Users;
using Kaban.API.Helpers;
using Kaban.Domain.Interfaces;
using Kaban.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Kaban.API.Controllers
{
    [Authorize]
    [ApiController]
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
        [HttpPost(ApiRoutes.Users.AuthenticateUser)]
        public async Task<IActionResult> Authenticate([FromBody]AuthenticateRequest request)
        {
            var currentUser = await _userService.Authenticate(request.Email, request.Password);
            if (currentUser is null)
                return BadRequest(new OperationFailureResponse { Message = "Email or password is incorrect" });
            
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

            var authSuccessResponse = _mapper.Map<AuthSuccessResponse>(currentUser);
            authSuccessResponse.Token = serializedToken;

            // return basic user info and authentication token
            return Ok(authSuccessResponse);
        }

        [AllowAnonymous]
        [HttpPost(ApiRoutes.Users.RegisterUser)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            var user = _mapper.Map<User>(request);

            try
            {
                var createdUser = await _userService.Create(user, request.Password);

                return Ok(new EntityCreatingSuccessResponse
                {
                    EntityId = createdUser.Id
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse
                {
                    Message = ex.Message
                });
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