using System;
using System.Collections.Generic;
using AutoMapper;
using Kaban.UI.Dto.Boards;
using Kaban.UI.Entities;
using Kaban.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kaban.UI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IBoardService _boardService;
        private readonly IListService _listService;
        private readonly ICardService _cardService;
        private readonly ICardCommentService _cardCommentService;
        private readonly IMapper _mapper;
        
        public DashboardController(IUserService userService, IBoardService boardService, IListService listService, ICardService cardService, ICardCommentService cardCommentService, IMapper mapper)
        {
            _userService = userService;
            _boardService = boardService;
            _listService = listService;
            _cardService = cardService;
            _cardCommentService = cardCommentService;
            _mapper = mapper;
        }
        
        [HttpGet("get-user-boards")]
        public IActionResult GetAllUserBoards()
        {
            var user = _userService.GetCurrentUser();
            
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