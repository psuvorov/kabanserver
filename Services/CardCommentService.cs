using System;
using System.Collections.Generic;
using System.Linq;
using KabanServer.Data;
using KabanServer.Entities;
using KabanServer.Exceptions;

namespace KabanServer.Services
{
    public interface ICardCommentService
    {
        IEnumerable<CardComment> GetAll(Card card);

        CardComment Create(CardComment comment);

        void Update(CardComment comment);

        void Delete(Guid id);
    } 
    
    public class CardCommentService : ICardCommentService
    {
        private readonly DataContext _context;

        public CardCommentService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<CardComment> GetAll(Card card)
        {
            if (card is null)
                throw new ArgumentNullException(nameof(card));
            
            return _context.CardComments.Where(comment => comment.CardId == card.Id);
        }

        public CardComment Create(CardComment comment)
        {
            if (comment is null)
                throw new ArgumentNullException(nameof(comment));
                
            _context.CardComments.Add(comment);
            _context.SaveChanges();

            return comment;
        }

        public void Update(CardComment comment)
        {
            if (comment is null)
                throw new ArgumentNullException(nameof(comment));
            if (_context.CardComments.SingleOrDefault(x => x.Id == comment.Id) is null)
                throw new AppException("Card Comment not found.");
            
            _context.CardComments.Update(comment);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var comment = _context.CardComments.Find(id);
            if (comment is null)
                return;

            _context.CardComments.Remove(comment);
        }
    }
}