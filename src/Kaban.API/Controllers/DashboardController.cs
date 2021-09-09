using System;
using System.Collections.Generic;
using AutoMapper;
using Kaban.API.Controllers.Requests.Boards;
using Kaban.API.Controllers.Responses;
using Kaban.API.Controllers.Responses.Boards;
using Kaban.Domain.Interfaces;
using Kaban.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kaban.API.Controllers
{
    [Authorize]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IStoredUser _storedUser;
        private readonly IUserService _userService;
        private readonly IBoardService _boardService;
        private readonly IListService _listService;
        private readonly ICardService _cardService;
        private readonly ICardCommentService _cardCommentService;
        private readonly IMapper _mapper;
        
        public DashboardController(IUserService userService, IBoardService boardService, IListService listService, ICardService cardService, ICardCommentService cardCommentService, IMapper mapper, IStoredUser storedUser)
        {
            _userService = userService;
            _boardService = boardService;
            _listService = listService;
            _cardService = cardService;
            _cardCommentService = cardCommentService;
            _mapper = mapper;
            _storedUser = storedUser;
        }
        
        [HttpGet(ApiRoutes.Dashboards.GetUserBoards)]
        public IActionResult GetUserBoards()
        {
            var user = _userService.Get(_storedUser.GetUserId());
            
            var allUserBoards = _boardService.GetAll(user);
            var boardInfoDtos = _mapper.Map<IEnumerable<BoardShortInfoResponse>>(allUserBoards);

            foreach (var boardShortInfoDto in boardInfoDtos)
            {
                var boardWallpaperPreviewPath = _boardService.GetBoardWallpaperPreviewPath(boardShortInfoDto.Id);
                boardShortInfoDto.WallpaperPreviewPath = boardWallpaperPreviewPath;
            }

            return Ok(boardInfoDtos);
        }
    }
}