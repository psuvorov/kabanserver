using System;
using System.Collections.Generic;
using Kaban.Domain.Models;

namespace Kaban.Domain.Interfaces
{
    public interface ICardCommentService
    {
        IEnumerable<CardComment> GetAll(Card card);

        CardComment Create(CardComment comment);

        void Update(CardComment comment);

        void Delete(Guid id);
    }
}