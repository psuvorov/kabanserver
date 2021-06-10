using System;
using System.Collections.Generic;
using AutoMapper;
using Kaban.API.Controllers.Requests.Lists;
using Kaban.API.Controllers.Responses;
using Kaban.API.Controllers.Responses.Cards;
using Kaban.API.Controllers.Responses.Lists;
using Kaban.Domain.Interfaces;
using Kaban.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kaban.API.Controllers
{
    public class ListsController : ControllerBase
    {
        private readonly IListService _listService;
        private readonly ICardService _cardService;
        private readonly IMapper _mapper;
        
        public ListsController(IListService listService, ICardService cardService, IMapper mapper)
        {
            _listService = listService;
            _cardService = cardService;
            _mapper = mapper;
        }

        [HttpGet(ApiRoutes.Lists.GetList)]
        public IActionResult GetList([FromRoute] Guid boardId, [FromRoute] Guid listId)
        {
            try
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
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse { Message = ex.Message });
            }
        }
        
        [HttpGet(ApiRoutes.Lists.GetArchivedLists)]
        public IActionResult GetArchivedLists([FromRoute] Guid boardId)
        {
            var archivedLists = _listService.GetArchivedLists(boardId);
            var archivedListDtos = _mapper.Map<IEnumerable<ArchivedListResponse>>(archivedLists);

            return Ok(archivedListDtos);
        }
        
        [HttpPost(ApiRoutes.Lists.CreateList)]
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
        
        [HttpPost(ApiRoutes.Lists.CopyList)]
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
        
        [HttpPut(ApiRoutes.Lists.UpdateList)]
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
        
        [HttpPut(ApiRoutes.Lists.RenumberLists)]
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
        
        [HttpDelete(ApiRoutes.Lists.DeleteList)]
        public IActionResult DeleteList([FromRoute] Guid listId)
        {
            _listService.Delete(listId);
            
            return NoContent();
        }
    }
}