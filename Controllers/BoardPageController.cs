using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using AutoMapper;
using KabanServer.Data;
using KabanServer.Dto.Boards;
using KabanServer.Dto.CardComments;
using KabanServer.Dto.Cards;
using KabanServer.Dto.Lists;
using KabanServer.Entities;
using KabanServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace KabanServer.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/boardpage")]
    public class BoardPageController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IBoardService _boardService;
        private readonly IListService _listService;
        private readonly ICardService _cardService;
        private readonly ICardCommentService _cardCommentService;
        private readonly IMapper _mapper;
        
        public BoardPageController(IUserService userService, IBoardService boardService, IListService listService, ICardService cardService, ICardCommentService cardCommentService, IMapper mapper)
        {
            _userService = userService;
            _boardService = boardService;
            _listService = listService;
            _cardService = cardService;
            _cardCommentService = cardCommentService;
            _mapper = mapper;
        }
        
        [HttpGet("get-board")]
        public IActionResult GetBoard([FromQuery] Guid boardId)
        {
            var boardEntity = _boardService.Get(boardId);
            var boardDto = _mapper.Map<BoardDto>(boardEntity);

            foreach (var listDto in boardDto.Lists)
            {
                foreach (var cardDto in listDto.Cards)
                {
                    var cardCoverInfo = _cardService.GetCardCoverInfo(boardId, cardDto.Id);
                    cardDto.CoverImagePath = cardCoverInfo.Item1;
                    cardDto.CoverImageOrientation = cardCoverInfo.Item2;
                }
            }

            var wallpaperPath = _boardService.GetWallpaperPath(boardId);
            boardDto.WallpaperPath = wallpaperPath;

            return Ok(boardDto);
        }
        
        [HttpGet("get-list")]
        public IActionResult GetList([FromQuery] Guid listId, [FromQuery] Guid boardId)
        {
            var listEntity = _listService.Get(listId);
            var listDto = _mapper.Map<ListDto>(listEntity);
            
            foreach (var cardDto in listDto.Cards)
            {
                var cardCoverInfo = _cardService.GetCardCoverInfo(boardId, cardDto.Id);
                cardDto.CoverImagePath = cardCoverInfo.Item1;
                cardDto.CoverImageOrientation = cardCoverInfo.Item2;
            }

            return Ok(listDto);
        }
        
        [HttpGet("get-card-details")]
        public IActionResult GetCardDetails([FromQuery] Guid cardId, [FromQuery] Guid boardId)
        {
            var cardEntity = _cardService.Get(cardId);

            var cardComments = _cardCommentService.GetAll(cardEntity);
            var cardCommentDtos = _mapper.Map<IEnumerable<CardCommentDto>>(cardComments);

            var cardDetailsDto = _mapper.Map<CardDetailsDto>(cardEntity);
            cardDetailsDto.Comments = cardCommentDtos;

            return Ok(cardDetailsDto);
        }

        [HttpGet("get-board-details")]
        public IActionResult GetBoardDetails([FromQuery] Guid boardId)
        {
            var boardEntity = _boardService.GetInfo(boardId);
            
            var boardDetailsDto = new BoardDetailsDto();
            boardDetailsDto.Id = boardEntity.Id;
            boardDetailsDto.Name = boardEntity.Name;
            boardDetailsDto.Description = boardEntity.Description;

            var author = boardEntity.CreatedBy;
            var boardUserDto = new BoardUserDto();
            boardUserDto.Id = author.Id;
            boardUserDto.FirstName = author.FirstName;
            boardUserDto.LastName = author.LastName;
            boardUserDto.Username = author.Username;
            boardUserDto.Email = author.Email;
            boardUserDto.UserPicture = "qwe";
            boardDetailsDto.Author = boardUserDto;

            boardDetailsDto.Participants = new HashSet<BoardUserDto>();

            boardDetailsDto.Created = boardEntity.Created;
            boardDetailsDto.LastModified = boardEntity.LastModified;

            return Ok(boardDetailsDto);
        }

        [HttpGet("get-archived-cards")]
        public IActionResult GetArchivedCards([FromQuery] Guid boardId)
        {
            var archivedCards = _cardService.GetArchivedCards(boardId);
            var archivedCardDtos = _mapper.Map<IEnumerable<ArchivedCardDto>>(archivedCards);

            return Ok(archivedCardDtos);
        }
        
        [HttpGet("get-archived-lists")]
        public IActionResult GetArchivedLists([FromQuery] Guid boardId)
        {
            var archivedLists = _listService.GetArchivedLists(boardId);
            var archivedListDtos = _mapper.Map<IEnumerable<ArchivedListDto>>(archivedLists);

            return Ok(archivedListDtos);
        }
        
        [HttpPost("create-list")]
        public IActionResult CreateList([FromBody] CreateListDto createListDto)
        {
            var list = _mapper.Map<List>(createListDto);

            try
            {
                var createdList = _listService.Create(list);

                var res = new ObjectResult(new { listId = createdList.Id });
                res.StatusCode = StatusCodes.Status201Created;
                
                return res;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPost("copy-list")]
        public IActionResult CopyList([FromBody] CopyListDto copyListDto)
        {
            try
            {
                var srcList = _listService.Get(copyListDto.Id);
                var copiedList = _listService.Copy(srcList);

                var res = new ObjectResult(new { listId = copiedList.Id });
                res.StatusCode = StatusCodes.Status201Created;
                
                return res;
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPost("create-card")]
        public IActionResult CreateCard([FromBody] CreateCardDto createCardDto)
        {
            var card = _mapper.Map<Card>(createCardDto);

            try
            {
                var createdCard = _cardService.Create(card);
                
                var res = new ObjectResult(new { cardId = createdCard.Id });
                res.StatusCode = StatusCodes.Status201Created;
                
                return res;

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPost("create-card-comment")]
        public IActionResult CreateCardComment([FromBody] CreateCardCommentDto createCardCommentDto)
        {
            var cardComment = _mapper.Map<CardComment>(createCardCommentDto);

            try
            {
                var createdComment = _cardCommentService.Create(cardComment);

                var res = new ObjectResult(new { cardCommentId = createdComment.Id });
                res.StatusCode = StatusCodes.Status201Created;
                
                return res;

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("set-card-cover")]
        public IActionResult SetCardCover([FromForm(Name = "imageFile")] IFormFile imageFile, [FromQuery] Guid boardId, [FromQuery] Guid cardId)
        {
            try
            {
                _cardService.SetCardCover(imageFile, boardId, cardId);
                var cardCoverInfo = _cardService.GetCardCoverInfo(boardId, cardId);

                return Ok(new { coverImagePath = cardCoverInfo.Item1, orientation = cardCoverInfo.Item2 });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPost("set-board-wallpaper")]
        public IActionResult SetBoardWallpaper([FromForm(Name = "imageFile")] IFormFile imageFile, [FromQuery] Guid boardId)
        {
            try
            {
                _boardService.SetBoardWallpaper(imageFile, boardId);
                var wallpaperPath = _boardService.GetWallpaperPath(boardId);

                return Ok(new { wallpaperPath = wallpaperPath });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPut("update-board-info")]
        public IActionResult UpdateBoardInfo([FromBody] UpdateBoardDto updateBoardDto)
        {
            try
            {
                var boardEntity = _boardService.Get(updateBoardDto.Id);
                if (!(updateBoardDto.Name is null))
                    boardEntity.Name = updateBoardDto.Name;
                if (!(updateBoardDto.Description is null))
                    boardEntity.Description = updateBoardDto.Description;
                _boardService.Update(boardEntity);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); 
            }
        }
        
        [HttpPut("update-list")]
        public IActionResult UpdateList([FromBody] UpdateListDto updateListDto)
        {
            try
            {
                var listEntity = _listService.Get(updateListDto.Id);
                if (!(updateListDto.Name is null))
                    listEntity.Name = updateListDto.Name;
                if (updateListDto.OrderNumber.HasValue)
                    listEntity.OrderNumber = updateListDto.OrderNumber.Value;
                if (updateListDto.IsArchived.HasValue)
                {
                    // TODO: archive/restore its cards
                    
                    if (updateListDto.IsArchived.Value)
                    {
                        // Archive card
                        listEntity.IsArchived = true;
                        listEntity.Archived = DateTime.Now;
                    }
                    else
                    {
                        // Restore card
                        listEntity.IsArchived = false;
                        listEntity.Archived = null;
                    }
                }
                _listService.Update(listEntity);
                
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPut("update-card")]
        public IActionResult UpdateCard([FromBody] UpdateCardDto updateCardDto)
        {
            try
            {
                var cardEntity = _cardService.Get(updateCardDto.Id);
                if (!(updateCardDto.Name is null))
                    cardEntity.Name = updateCardDto.Name;
                if (!(updateCardDto.Description is null))
                    cardEntity.Description = updateCardDto.Description;
                if (updateCardDto.OrderNumber.HasValue)
                    cardEntity.OrderNumber = updateCardDto.OrderNumber.Value;
                if (updateCardDto.ListId.HasValue)
                    cardEntity.ListId = updateCardDto.ListId.Value;
                if (updateCardDto.IsArchived.HasValue)
                {
                    cardEntity.IsArchived = updateCardDto.IsArchived.Value;
                    cardEntity.Archived = DateTime.Now;
                }
                _cardService.Update(cardEntity);
                
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPut("renumber-all-lists")]
        public IActionResult RenumberAllLists([FromQuery] Guid boardId, [FromBody] IEnumerable<RenumberListDto> renumberedLists)
        {
            try
            {
                foreach (var renumberListDto in renumberedLists)
                {
                    var storedList = _listService.Get(renumberListDto.Id);
                    if (storedList is null)
                        return BadRequest(new { message = $"List with '{renumberListDto.Id}' id not found." });
                    // if (storedList.Board.Id != boardId)
                    //     return BadRequest(new { message = $"List with '{renumberListDto.Id}' id doesn't belong to board with '{boardId}'." });
                    
                    storedList.OrderNumber = renumberListDto.OrderNumber;
                    _listService.Update(storedList);
                }
                
                return Ok();                
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPut("renumber-all-cards")]
        public IActionResult RenumberAllCards([FromQuery] Guid boardId, [FromBody] IEnumerable<RenumberCardDto> renumberedCards)
        {
            try
            {
                foreach (var renumberCardDto in renumberedCards)
                {
                    var storedCard = _cardService.Get(renumberCardDto.Id);
                    if (storedCard is null)
                        return BadRequest(new { message = $"Card with '{renumberCardDto.Id}' id not found." });
                    // TODO: add check similar to RenumberAllLists
                    
                    storedCard.OrderNumber = renumberCardDto.OrderNumber;
                    _cardService.Update(storedCard);
                }
                return Ok();                
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpDelete("delete-board")]
        public IActionResult DeleteBoard([FromQuery] Guid boardId)
        {
            _boardService.Delete(boardId);
            
            return NoContent();
        }
        
        [HttpDelete("delete-list")]
        public IActionResult DeleteList([FromQuery] Guid listId)
        {
            _listService.Delete(listId);
            
            return NoContent();
        }
        
        [HttpDelete("delete-card")]
        public IActionResult DeleteCard([FromQuery] Guid cardId)
        {
            _cardService.Delete(cardId);
            
            return NoContent();
        }
        
        [HttpDelete("delete-card-comment")]
        public IActionResult DeleteCardComment([FromQuery] Guid cardCommentId)
        {
            _cardCommentService.Delete(cardCommentId);
            
            return NoContent();
        }

       
    }
}