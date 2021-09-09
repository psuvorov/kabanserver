using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Kaban.API.Controllers.Requests.Lists;
using Kaban.API.Controllers.Responses;
using Kaban.API.Controllers.Responses.Cards;
using Kaban.API.Controllers.Responses.Lists;
using Kaban.Database.Exceptions;
using Kaban.Domain.Interfaces;
using Kaban.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kaban.API.Controllers
{
    [Authorize]
    [ApiController]
    public class ListsController : ControllerBase
    {
        private readonly IListService _listService;
        private readonly ICardService _cardService;
        private readonly IBoardService _boardService;
        private readonly IMapper _mapper;
        
        public ListsController(IListService listService, ICardService cardService, IMapper mapper, IBoardService boardService)
        {
            _listService = listService;
            _cardService = cardService;
            _mapper = mapper;
            _boardService = boardService;
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
            catch (ListNotExistException ex)
            {
                return NotFound(new OperationFailureResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse { Message = ex.Message });
            }
        }
        
        [HttpGet(ApiRoutes.Lists.GetArchivedLists)]
        public IActionResult GetArchivedLists([FromRoute] Guid boardId)
        {
            try
            {
                var archivedLists = _listService.GetArchivedLists(boardId);
                var archivedListDtos = _mapper.Map<IEnumerable<ArchivedListResponse>>(archivedLists);

                return Ok(archivedListDtos);
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
        
        [HttpPost(ApiRoutes.Lists.CreateList)]
        public IActionResult CreateList([FromRoute] Guid boardId, [FromBody] CreateListRequest request)
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
        public IActionResult CopyList([FromRoute] Guid boardId, [FromRoute] Guid listId)
        {
            try
            {
                // TODO: Consider moving checks into services to make controller methods thin

                var board = _boardService.Get(boardId);
                if (board is null)
                    throw new BoardNotExistException("Board doesn't exist");
                if (board.Lists.FirstOrDefault(x => x.Id == listId) is null)
                    throw new ListNotExistException($"Board doesn't contain a list with '{listId}' Id.");

                var srcList = _listService.Get(listId);
                var copiedList = _listService.Copy(srcList);

                return Ok(new EntityCreatingSuccessResponse {EntityId = copiedList.Id});
            }
            catch (BoardNotExistException ex)
            {
                return NotFound(new OperationFailureResponse { Message = ex.Message });
            }
            catch (ListNotExistException ex)
            {
                return NotFound(new OperationFailureResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse { Message = ex.Message });
            }
        }
        
        [HttpPut(ApiRoutes.Lists.UpdateList)]
        public IActionResult UpdateList([FromRoute] Guid boardId, [FromBody] UpdateListRequest request)
        {
            try
            {
                var listEntity = _listService.Get(request.ListId);
                if (!string.IsNullOrEmpty(request.Name))
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
                        listEntity.Archived = DateTime.UtcNow;
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
        
        [HttpPut(ApiRoutes.Lists.ReorderLists)]
        public IActionResult ReorderLists([FromRoute] Guid boardId, [FromBody] IEnumerable<ReorderListRequest> reorderedLists)
        {
            try
            {
                foreach (var renumberListDto in reorderedLists)
                {
                    var storedList = _listService.Get(renumberListDto.ListId);

                    storedList.OrderNumber = renumberListDto.OrderNumber;
                    _listService.Update(storedList);
                }

                return Ok();
            }
            catch (ListNotExistException ex)
            {
                return NotFound(new OperationFailureResponse { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new OperationFailureResponse { Message = ex.Message });
            }
        }
        
        [HttpDelete(ApiRoutes.Lists.DeleteList)]
        public IActionResult DeleteList([FromRoute] Guid boardId, [FromRoute] Guid listId)
        {
            _listService.Delete(listId);
            
            return NoContent();
        }
    }
}