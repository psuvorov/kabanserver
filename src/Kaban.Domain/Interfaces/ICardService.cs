using System;
using System.Collections.Generic;
using System.IO;
using Kaban.Domain.Enums;
using Kaban.Domain.Models;

namespace Kaban.Domain.Interfaces
{
    public interface ICardService
    {
        IEnumerable<Card> GetAll();
        
        IEnumerable<Card> GetAll(List list);
        
        Card Get(Guid id);

        IEnumerable<Card> GetArchivedCards(Guid boardId);
        
        Tuple<string, CoverImageOrientation> GetCardCoverInfo(Guid boardId, Guid cardId);

        void SetCardCover(Stream formFile, Guid boardId, Guid cardId);
        
        Card Create(Card card);
        
        void Update(Card card);
        
        void Delete(Guid id);
    }
}