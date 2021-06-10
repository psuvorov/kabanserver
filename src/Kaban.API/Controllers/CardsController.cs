using System;
using System.Collections.Generic;
using AutoMapper;
using Kaban.API.Controllers.Requests.CardComments;
using Kaban.API.Controllers.Requests.Cards;
using Kaban.API.Controllers.Responses;
using Kaban.API.Controllers.Responses.CardComments;
using Kaban.API.Controllers.Responses.Cards;
using Kaban.Domain.Interfaces;
using Kaban.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kaban.API.Controllers
{
    [Authorize]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly ICardService _cardService;
        private readonly ICardCommentService _cardCommentService;
        private readonly IMapper _mapper;
        
        public CardsController(ICardService cardService, ICardCommentService cardCommentService, IMapper mapper)
        {
            _cardService = cardService;
            _cardCommentService = cardCommentService;
            _mapper = mapper;
        }
        
        [HttpGet(ApiRoutes.Cards.GetCardDetails)]
        public IActionResult GetCardDetails([FromRoute] Guid boardId, [FromRoute] Guid cardId)
        {
            try
            {
                var cardEntity = _cardService.Get(cardId);

                var cardComments = _cardCommentService.GetAll(cardEntity);
                var cardCommentDtos = _mapper.Map<IEnumerable<CardCommentResponse>>(cardComments);

                var cardDetailsDto = _mapper.Map<CardDetailsResponse>(cardEntity);
                cardDetailsDto.Comments = cardCommentDtos;

                return Ok(cardDetailsDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse { Message = ex.Message });
            }
        }
        [HttpGet(ApiRoutes.Cards.GetArchivedCards)]
        public IActionResult GetArchivedCards([FromRoute] Guid boardId)
        {
            var archivedCards = _cardService.GetArchivedCards(boardId);
            var archivedCardDtos = _mapper.Map<IEnumerable<ArchivedCardResponse>>(archivedCards);

            return Ok(archivedCardDtos);
        }
        
        [HttpPost(ApiRoutes.Cards.CreateCard)]
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
        
        [HttpPost(ApiRoutes.Cards.CreateCardComment)]
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
        
        [HttpPost(ApiRoutes.Cards.SetCardCover)]
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
        
        [HttpPut(ApiRoutes.Cards.UpdateCard)]
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
        
        [HttpPut(ApiRoutes.Cards.RenumberCards)]
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
        
        [HttpDelete(ApiRoutes.Cards.DeleteCard)]
        public IActionResult DeleteCard([FromRoute] Guid cardId)
        {
            _cardService.Delete(cardId);
            
            return NoContent();
        }
        
        [HttpDelete(ApiRoutes.Cards.DeleteCardComment)]
        public IActionResult DeleteCardComment([FromRoute] Guid cardCommentId)
        {
            _cardCommentService.Delete(cardCommentId);
            
            return NoContent();
        }
    }
}