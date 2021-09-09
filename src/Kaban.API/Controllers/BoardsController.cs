using System;
using System.Collections.Generic;
using AutoMapper;
using Kaban.API.Controllers.Requests.Boards;
using Kaban.API.Controllers.Responses;
using Kaban.API.Controllers.Responses.Boards;
using Kaban.API.Controllers.Responses.Cards;
using Kaban.Database.Exceptions;
using Kaban.Domain.Interfaces;
using Kaban.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kaban.API.Controllers
{
    [Authorize]
    [ApiController]
    public class BoardPageController : ControllerBase
    {
        private readonly IBoardService _boardService;
        private readonly ICardService _cardService;
        private readonly IMapper _mapper;
        
        public BoardPageController(IBoardService boardService, ICardService cardService, IMapper mapper)
        {
            _boardService = boardService;
            _cardService = cardService;
            _mapper = mapper;
        }
        
        [HttpGet(ApiRoutes.Boards.GetBoard)]
        public IActionResult GetBoard([FromRoute] Guid boardId)
        {
            try
            {
                var boardEntity = _boardService.Get(boardId);
                var boardDto = _mapper.Map<BoardResponse>(boardEntity);

                foreach (var listDto in boardDto.Lists)
                {
                    foreach (var cardDto in listDto.Cards)
                    {
                        var cardCoverInfo = _cardService.GetCardCoverInfo(boardId, cardDto.Id);
                        cardDto.CoverImagePath = cardCoverInfo.Item1;
                        cardDto.CoverImageOrientation = _mapper.Map<CoverImageOrientationDto>(cardCoverInfo.Item2);
                    }
                }

                var wallpaperPath = _boardService.GetWallpaperPath(boardId);
                boardDto.WallpaperPath = wallpaperPath;

                return Ok(boardDto);
            }
            catch (BoardNotExistException ex)
            {
                return NotFound(new OperationFailureResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse { Message = ex.Message });
            }
        }
        
        [HttpGet(ApiRoutes.Boards.GetBoardDetails)]
        public IActionResult GetBoardDetails([FromRoute] Guid boardId)
        {
            try
            {
                var boardEntity = _boardService.GetInfo(boardId);

                var boardDetailsDto = new BoardDetailsResponse();
                boardDetailsDto.Id = boardEntity.Id;
                boardDetailsDto.Name = boardEntity.Name;
                boardDetailsDto.Description = boardEntity.Description;

                var author = boardEntity.CreatedBy;
                var boardUserDto = new BoardUserResponse();
                boardUserDto.Id = author.Id;
                boardUserDto.FirstName = author.FirstName;
                boardUserDto.LastName = author.LastName;
                boardUserDto.Username = author.Username;
                boardUserDto.Email = author.Email;
                boardUserDto.UserPicture = "qwe"; // TODO: !!
                boardDetailsDto.Author = boardUserDto;

                boardDetailsDto.Participants = new HashSet<BoardUserResponse>();

                boardDetailsDto.Created = boardEntity.Created;
                boardDetailsDto.LastModified = boardEntity.LastModified;

                return Ok(boardDetailsDto);
            }
            catch (BoardNotExistException ex)
            {
                return NotFound(new OperationFailureResponse {Message = ex.Message});
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse {Message = ex.Message});
            }
        }
        
        [HttpPost(ApiRoutes.Boards.CreateBoard)]
        public IActionResult CreateBoard([FromBody] CreateBoardRequest request)
        {
            var board = _mapper.Map<Board>(request);

            try
            {
                var createdBoard = _boardService.Create(board);
                
                return Ok(new EntityCreatingSuccessResponse { EntityId = createdBoard.Id});
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse { Message = ex.Message}); 
            }
        }
        
        [HttpPost(ApiRoutes.Boards.SetBoardWallpaper)]
        public IActionResult SetBoardWallpaper([FromRoute] Guid boardId, [FromForm(Name = "imageFile")] IFormFile imageFile)
        {
            try
            {
                _boardService.SetBoardWallpaper(imageFile.OpenReadStream(), boardId);
                var wallpaperPath = _boardService.GetWallpaperPath(boardId);

                return Ok(new SetBoardWallpaperResponse { WallpaperPath = wallpaperPath });
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse { Message = ex.Message });
            }
        }
        
        [HttpPut(ApiRoutes.Boards.UpdateBoardInfo)]
        public IActionResult UpdateBoardInfo([FromRoute] Guid boardId, [FromBody] UpdateBoardRequest request)
        {
            try
            {
                var boardEntity = _boardService.Get(request.BoardId);
                if (!(string.IsNullOrEmpty(request.Name)))
                    boardEntity.Name = request.Name;
                if (!(request.Description is null))
                    boardEntity.Description = request.Description;
                _boardService.Update(boardEntity);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse { Message = ex.Message }); 
            }
        }
        
        [HttpDelete(ApiRoutes.Boards.DeleteBoard)]
        public IActionResult DeleteBoard([FromRoute] Guid boardId)
        {
            _boardService.Delete(boardId);
            
            return NoContent();
        }
    }
}