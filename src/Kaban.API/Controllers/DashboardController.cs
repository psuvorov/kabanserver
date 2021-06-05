using System;
using System.Collections.Generic;
using AutoMapper;
using Kaban.API.Dto.Boards;
using Kaban.Domain.Interfaces;
using Kaban.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kaban.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/dashboard")]
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
        
        [HttpGet("get-user-boards")]
        public IActionResult GetAllUserBoards()
        {
            var user = _userService.Get(_storedUser.GetUserId());
            
            var allUserBoards = _boardService.GetAll(user);
            var boardInfoDtos = _mapper.Map<IEnumerable<BoardShortInfoDto>>(allUserBoards);

            foreach (var boardShortInfoDto in boardInfoDtos)
            {
                var boardWallpaperPreviewPath = _boardService.GetBoardWallpaperPreviewPath(boardShortInfoDto.Id);
                boardShortInfoDto.WallpaperPreviewPath = boardWallpaperPreviewPath;
            }

            return Ok(boardInfoDtos);
        }
        
        [HttpPost("create-board")]
        public IActionResult CreateBoard([FromBody] CreateBoardDto createBoardDto)
        {
            var board = _mapper.Map<Board>(createBoardDto);

            try
            {
                var createdBoard = _boardService.Create(board);
                
                var res = new ObjectResult(new { boardId = createdBoard.Id });
                res.StatusCode = StatusCodes.Status201Created;
                
                return res;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); 
            }
        }
    }
}