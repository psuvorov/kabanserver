using System;
using System.Collections.Generic;
using AutoMapper;
using Kaban.API.Controllers.Responses.Boards;
using Kaban.API.Controllers.Responses.CardComments;
using Kaban.API.Controllers.Responses.Cards;
using Kaban.API.Controllers.Responses.Lists;
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
        
        // TODO: break up this fatty controller
        
        [HttpGet(ApiRoutes.BoardPage.GetBoard)]
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
                    cardDto.CoverImageOrientation = _mapper.Map<CoverImageOrientationDto>(cardCoverInfo.Item2);
                }
            }

            var wallpaperPath = _boardService.GetWallpaperPath(boardId);
            boardDto.WallpaperPath = wallpaperPath;

            return Ok(boardDto);
        }
        
        [HttpGet(ApiRoutes.BoardPage.GetList)]
        public IActionResult GetList([FromQuery] Guid listId, [FromQuery] Guid boardId)
        {
            var listEntity = _listService.Get(listId);
            var listDto = _mapper.Map<ListDto>(listEntity);
            
            foreach (var cardDto in listDto.Cards)
            {
                var cardCoverInfo = _cardService.GetCardCoverInfo(boardId, cardDto.Id);
                cardDto.CoverImagePath = cardCoverInfo.Item1;
                cardDto.CoverImageOrientation = _mapper.Map<CoverImageOrientationDto>(cardCoverInfo.Item2);
            }

            return Ok(listDto);
        }
        
        [HttpGet(ApiRoutes.BoardPage.GetCardDetails)]
        public IActionResult GetCardDetails([FromQuery] Guid cardId, [FromQuery] Guid boardId)
        {
            var cardEntity = _cardService.Get(cardId);

            var cardComments = _cardCommentService.GetAll(cardEntity);
            var cardCommentDtos = _mapper.Map<IEnumerable<CardCommentDto>>(cardComments);

            var cardDetailsDto = _mapper.Map<CardDetailsDto>(cardEntity);
            cardDetailsDto.Comments = cardCommentDtos;

            return Ok(cardDetailsDto);
        }

        [HttpGet(ApiRoutes.BoardPage.GetBoardDetails)]
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

        [HttpGet(ApiRoutes.BoardPage.GetArchivedCards)]
        public IActionResult GetArchivedCards([FromQuery] Guid boardId)
        {
            var archivedCards = _cardService.GetArchivedCards(boardId);
            var archivedCardDtos = _mapper.Map<IEnumerable<ArchivedCardDto>>(archivedCards);

            return Ok(archivedCardDtos);
        }
        
        [HttpGet(ApiRoutes.BoardPage.GetArchivedLists)]
        public IActionResult GetArchivedLists([FromQuery] Guid boardId)
        {
            var archivedLists = _listService.GetArchivedLists(boardId);
            var archivedListDtos = _mapper.Map<IEnumerable<ArchivedListDto>>(archivedLists);

            return Ok(archivedListDtos);
        }
        
        [HttpPost(ApiRoutes.BoardPage.CreateList)]
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
        
        [HttpPost(ApiRoutes.BoardPage.CopyList)]
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
        
        [HttpPost(ApiRoutes.BoardPage.CreateCard)]
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
        
        [HttpPost(ApiRoutes.BoardPage.CreateCardComment)]
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

        [HttpPost(ApiRoutes.BoardPage.SetCardCover)]
        public IActionResult SetCardCover([FromForm(Name = "imageFile")] IFormFile imageFile, [FromQuery] Guid boardId, [FromQuery] Guid cardId)
        {
            try
            {
                _cardService.SetCardCover(imageFile.OpenReadStream(), boardId, cardId);
                var cardCoverInfo = _cardService.GetCardCoverInfo(boardId, cardId);

                return Ok(new { coverImagePath = cardCoverInfo.Item1, orientation = cardCoverInfo.Item2 });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPost(ApiRoutes.BoardPage.SetBoardWallpaper)]
        public IActionResult SetBoardWallpaper([FromForm(Name = "imageFile")] IFormFile imageFile, [FromQuery] Guid boardId)
        {
            try
            {
                _boardService.SetBoardWallpaper(imageFile.OpenReadStream(), boardId);
                var wallpaperPath = _boardService.GetWallpaperPath(boardId);

                return Ok(new { wallpaperPath });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        
        [HttpPut(ApiRoutes.BoardPage.UpdateBoardInfo)]
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
        
        [HttpPut(ApiRoutes.BoardPage.UpdateList)]
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
        
        [HttpPut(ApiRoutes.BoardPage.UpdateCard)]
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
        
        [HttpPut(ApiRoutes.BoardPage.RenumberAllLists)]
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
        
        [HttpPut(ApiRoutes.BoardPage.RenumberAllCards)]
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
        
        [HttpDelete(ApiRoutes.BoardPage.DeleteBoard)]
        public IActionResult DeleteBoard([FromQuery] Guid boardId)
        {
            _boardService.Delete(boardId);
            
            return NoContent();
        }
        
        [HttpDelete(ApiRoutes.BoardPage.DeleteList)]
        public IActionResult DeleteList([FromQuery] Guid listId)
        {
            _listService.Delete(listId);
            
            return NoContent();
        }
        
        [HttpDelete(ApiRoutes.BoardPage.DeleteCard)]
        public IActionResult DeleteCard([FromQuery] Guid cardId)
        {
            _cardService.Delete(cardId);
            
            return NoContent();
        }
        
        [HttpDelete(ApiRoutes.BoardPage.DeleteCardComment)]
        public IActionResult DeleteCardComment([FromQuery] Guid cardCommentId)
        {
            _cardCommentService.Delete(cardCommentId);
            
            return NoContent();
        }
    }
}