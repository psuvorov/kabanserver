using System;
using System.Collections.Generic;
using AutoMapper;
using Kaban.API.Controllers.Requests.Boards;
using Kaban.API.Controllers.Requests.CardComments;
using Kaban.API.Controllers.Requests.Cards;
using Kaban.API.Controllers.Requests.Lists;
using Kaban.API.Controllers.Responses;
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
        public IActionResult GetBoard([FromRoute] Guid boardId)
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
        
        [HttpGet(ApiRoutes.BoardPage.GetList)]
        public IActionResult GetList([FromRoute] Guid boardId, [FromRoute] Guid listId)
        {
            var listEntity = _listService.Get(listId);
            var listDto = _mapper.Map<ListResponse>(listEntity);
            
            foreach (var cardDto in listDto.Cards)
            {
                var cardCoverInfo = _cardService.GetCardCoverInfo(boardId, cardDto.Id);
                cardDto.CoverImagePath = cardCoverInfo.Item1;
                cardDto.CoverImageOrientation = _mapper.Map<CoverImageOrientationDto>(cardCoverInfo.Item2);
            }

            return Ok(listDto);
        }
        
        [HttpGet(ApiRoutes.BoardPage.GetBoardDetails)]
        public IActionResult GetBoardDetails([FromRoute] Guid boardId)
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
            boardUserDto.UserPicture = "qwe";
            boardDetailsDto.Author = boardUserDto;

            boardDetailsDto.Participants = new HashSet<BoardUserResponse>();

            boardDetailsDto.Created = boardEntity.Created;
            boardDetailsDto.LastModified = boardEntity.LastModified;

            return Ok(boardDetailsDto);
        }
        
        [HttpGet(ApiRoutes.BoardPage.GetCardDetails)]
        public IActionResult GetCardDetails([FromRoute] Guid boardId, [FromRoute] Guid cardId)
        {
            var cardEntity = _cardService.Get(cardId);

            var cardComments = _cardCommentService.GetAll(cardEntity);
            var cardCommentDtos = _mapper.Map<IEnumerable<CardCommentResponse>>(cardComments);

            var cardDetailsDto = _mapper.Map<CardDetailsResponse>(cardEntity);
            cardDetailsDto.Comments = cardCommentDtos;

            return Ok(cardDetailsDto);
        }
        
        [HttpGet(ApiRoutes.BoardPage.GetArchivedLists)]
        public IActionResult GetArchivedLists([FromRoute] Guid boardId)
        {
            var archivedLists = _listService.GetArchivedLists(boardId);
            var archivedListDtos = _mapper.Map<IEnumerable<ArchivedListResponse>>(archivedLists);

            return Ok(archivedListDtos);
        }

        [HttpGet(ApiRoutes.BoardPage.GetArchivedCards)]
        public IActionResult GetArchivedCards([FromRoute] Guid boardId)
        {
            var archivedCards = _cardService.GetArchivedCards(boardId);
            var archivedCardDtos = _mapper.Map<IEnumerable<ArchivedCardResponse>>(archivedCards);

            return Ok(archivedCardDtos);
        }
        
        [HttpPost(ApiRoutes.BoardPage.CreateList)]
        public IActionResult CreateList([FromBody] CreateListRequest request)
        {
            var list = _mapper.Map<List>(request);

            try
            {
                var createdList = _listService.Create(list);

                return Ok(new EntityCreatingSuccessResponse { EntityId = createdList.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse { Message = ex.Message });
            }
        }
        
        [HttpPost(ApiRoutes.BoardPage.CreateCard)]
        public IActionResult CreateCard([FromBody] CreateCardRequest request)
        {
            var card = _mapper.Map<Card>(request);

            try
            {
                var createdCard = _cardService.Create(card);
                
                return Ok(new EntityCreatingSuccessResponse { EntityId = createdCard.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse { Message = ex.Message });
            }
        }
        
        [HttpPost(ApiRoutes.BoardPage.CreateCardComment)]
        public IActionResult CreateCardComment([FromBody] CreateCardCommentRequest request)
        {
            var cardComment = _mapper.Map<CardComment>(request);

            try
            {
                var createdComment = _cardCommentService.Create(cardComment);

                return Ok(new EntityCreatingSuccessResponse { EntityId = createdComment.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse { Message = ex.Message });
            }
        }
        
        [HttpPost(ApiRoutes.BoardPage.CopyList)]
        public IActionResult CopyList([FromBody] CopyListRequest request)
        {
            try
            {
                var srcList = _listService.Get(request.ListId);
                var copiedList = _listService.Copy(srcList);
                
                return Ok(new EntityCreatingSuccessResponse { EntityId = copiedList.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse { Message = ex.Message });
            }
        }

        [HttpPost(ApiRoutes.BoardPage.SetCardCover)]
        public IActionResult SetCardCover([FromForm(Name = "imageFile")] IFormFile imageFile, [FromQuery] Guid boardId, [FromQuery] Guid cardId)
        {
            try
            {
                _cardService.SetCardCover(imageFile.OpenReadStream(), boardId, cardId);
                var cardCoverInfo = _cardService.GetCardCoverInfo(boardId, cardId);

                // return Ok(new { coverImagePath = cardCoverInfo.Item1, orientation = cardCoverInfo.Item2 });
                return Ok(new SetCardCoverResponse { CoverImagePath = cardCoverInfo.Item1, ImageOrientation = _mapper.Map<CoverImageOrientationDto>(cardCoverInfo.Item2) });
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse { Message = ex.Message });
            }
        }
        
        [HttpPost(ApiRoutes.BoardPage.SetBoardWallpaper)]
        public IActionResult SetBoardWallpaper([FromForm(Name = "imageFile")] IFormFile imageFile, [FromQuery] Guid boardId)
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
        
        [HttpPut(ApiRoutes.BoardPage.UpdateBoardInfo)]
        public IActionResult UpdateBoardInfo([FromBody] UpdateBoardRequest request)
        {
            try
            {
                var boardEntity = _boardService.Get(request.BoardId);
                if (!(request.Name is null))
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
        
        [HttpPut(ApiRoutes.BoardPage.UpdateList)]
        public IActionResult UpdateList([FromBody] UpdateListRequest request)
        {
            try
            {
                var listEntity = _listService.Get(request.ListId);
                if (!(request.Name is null))
                    listEntity.Name = request.Name;
                if (request.OrderNumber.HasValue)
                    listEntity.OrderNumber = request.OrderNumber.Value;
                if (request.IsArchived.HasValue)
                {
                    // TODO: archive/restore its cards
                    
                    if (request.IsArchived.Value)
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
                return BadRequest(new OperationFailureResponse { Message = ex.Message });
            }
        }
        
        [HttpPut(ApiRoutes.BoardPage.UpdateCard)]
        public IActionResult UpdateCard([FromBody] UpdateCardRequest request)
        {
            try
            {
                var cardEntity = _cardService.Get(request.CardId);
                if (!(request.Name is null))
                    cardEntity.Name = request.Name;
                if (!(request.Description is null))
                    cardEntity.Description = request.Description;
                if (request.OrderNumber.HasValue)
                    cardEntity.OrderNumber = request.OrderNumber.Value;
                if (request.ListId.HasValue)
                    cardEntity.ListId = request.ListId.Value;
                if (request.IsArchived.HasValue)
                {
                    cardEntity.IsArchived = request.IsArchived.Value;
                    cardEntity.Archived = DateTime.Now;
                }
                _cardService.Update(cardEntity);
                
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse { Message = ex.Message });
            }
        }
        
        [HttpPut(ApiRoutes.BoardPage.RenumberLists)]
        public IActionResult RenumberLists([FromQuery] Guid boardId, [FromBody] IEnumerable<RenumberListRequest> renumberedLists)
        {
            try
            {
                foreach (var renumberListDto in renumberedLists)
                {
                    var storedList = _listService.Get(renumberListDto.ListId);
                    if (storedList is null)
                        return BadRequest(new { message = $"List with '{renumberListDto.ListId}' id not found." });
                    // if (storedList.Board.Id != boardId)
                    //     return BadRequest(new { message = $"List with '{renumberListDto.Id}' id doesn't belong to board with '{boardId}'." });
                    
                    storedList.OrderNumber = renumberListDto.OrderNumber;
                    _listService.Update(storedList);
                }
                
                return Ok();                
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse { Message = ex.Message });
            }
        }
        
        [HttpPut(ApiRoutes.BoardPage.RenumberCards)]
        public IActionResult RenumberCards([FromQuery] Guid boardId, [FromBody] IEnumerable<RenumberCardRequest> renumberedCards)
        {
            try
            {
                foreach (var renumberCardDto in renumberedCards)
                {
                    var storedCard = _cardService.Get(renumberCardDto.CardId);
                    if (storedCard is null)
                        return BadRequest(new { message = $"Card with '{renumberCardDto.CardId}' id not found." });
                    // TODO: add check similar to RenumberAllLists
                    
                    storedCard.OrderNumber = renumberCardDto.OrderNumber;
                    _cardService.Update(storedCard);
                }
                return Ok();                
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse { Message = ex.Message });
            }
        }
        
        [HttpDelete(ApiRoutes.BoardPage.DeleteBoard)]
        public IActionResult DeleteBoard([FromRoute] Guid boardId)
        {
            _boardService.Delete(boardId);
            
            return NoContent();
        }
        
        [HttpDelete(ApiRoutes.BoardPage.DeleteList)]
        public IActionResult DeleteList([FromRoute] Guid listId)
        {
            _listService.Delete(listId);
            
            return NoContent();
        }
        
        [HttpDelete(ApiRoutes.BoardPage.DeleteCard)]
        public IActionResult DeleteCard([FromRoute] Guid cardId)
        {
            _cardService.Delete(cardId);
            
            return NoContent();
        }
        
        [HttpDelete(ApiRoutes.BoardPage.DeleteCardComment)]
        public IActionResult DeleteCardComment([FromRoute] Guid cardCommentId)
        {
            _cardCommentService.Delete(cardCommentId);
            
            return NoContent();
        }
    }
}