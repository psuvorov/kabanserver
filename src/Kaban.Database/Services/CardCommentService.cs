using System;
using System.Collections.Generic;
using System.Linq;
using Kaban.Database.Exceptions;
using Kaban.Domain.Interfaces;
using Kaban.Domain.Models;

namespace Kaban.Database.Services
{
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
            
            return _context.CardComments.Where(comment => comment.CardId == card.Id).ToList();
        }

        public CardComment Create(CardComment comment)
        {
            if (comment is null)
                throw new ArgumentNullException(nameof(comment));
            if (string.IsNullOrEmpty(comment.Text))
                throw new Exception("Comment Text cannot be null or empty.");
                
            _context.CardComments.Add(comment);
            _context.SaveChanges();

            return comment;
        }

        public void Update(CardComment comment)
        {
            if (comment is null)
                throw new ArgumentNullException(nameof(comment));
            if (_context.CardComments.SingleOrDefault(x => x.Id == comment.Id) is null)
                throw new CardCommentNotExistException("Card Comment not found.");
            
            _context.CardComments.Update(comment);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var comment = _context.CardComments.Find(id);
            if (comment is null)
                return;

            _context.CardComments.Remove(comment);
            _context.SaveChanges();
        }
    }
}