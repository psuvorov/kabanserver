using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kaban.UI.Data;
using Kaban.UI.Entities;
using Kaban.UI.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Kaban.UI.Services
{
    public interface IListService
    {
        IEnumerable<List> GetAll();
        
        IEnumerable<List> GetAll(Board board);
        
        List Get(Guid id);
        
        IEnumerable<List> GetArchivedLists(Guid boardId);
        
        List Create(List list);
        
        List Copy(List list);
        
        void Update(List list);
        
        void Delete(Guid id);
    }
    
    public class ListService : IListService
    {
        private readonly DataContext _context;
        private readonly ICardService _cardService;
        private readonly IWebHostEnvironment _env;

        public ListService(DataContext context, ICardService cardService, IWebHostEnvironment env)
        {
            _context = context;
            _cardService = cardService;
            _env = env;
        }

        public IEnumerable<List> GetAll()
        {
            return _context.Lists;
        }

        public IEnumerable<List> GetAll(Board board)
        {
            if (board is null)
                throw new ArgumentNullException(nameof(board));

            var lists = _context.Lists.Where(list => list.BoardId == board.Id);
            foreach (var list in lists)
            {
                _context.Entry(list).Collection(lst => lst.Cards).Query().OrderBy(card => card.OrderNumber).Load();
            }

            lists = lists.OrderBy(lst => lst.OrderNumber);

            return lists;
        }

        public List Get(Guid id)
        {
            var list = _context.Lists
                .Include(x => x.Board)
                .Include(x => x.Cards)
                .SingleOrDefault(x => x.Id == id);
            
            if (list is null)
                throw new AppException($"List with '{id}' not found.");

            list.Cards = list.Cards.OrderBy(x => x.OrderNumber).ToList(); 
            
            return list;
        }

        public IEnumerable<List> GetArchivedLists(Guid boardId)
        {
            var lists = _context.Lists
                .IgnoreQueryFilters()
                .Where(x => x.IsArchived && !x.IsDeleted)
                .OrderByDescending(x => x.Archived)
                .ToList();

            return lists;
        }

        public List Create(List list)
        {
            if (list is null)
                throw new ArgumentNullException(nameof(list));

            _context.Lists.Add(list);
            _context.SaveChanges();

            return list;
        }

        public List Copy(List list)
        {
            if (list is null)
                throw new ArgumentNullException(nameof(list));
            
            var copiedList = new List();
            copiedList.Name = list.Name;
            copiedList.OrderNumber = list.OrderNumber;
            copiedList.BoardId = list.BoardId;
            
            _context.Entry(list).Collection(lst => lst.Cards).Load();
            
            foreach (var srcCard in list.Cards)
            {
                var copiedCard = new Card
                {
                    Id = Guid.NewGuid(), 
                    Name = srcCard.Name, 
                    Description = srcCard.Description,
                    OrderNumber = srcCard.OrderNumber
                };
                copiedList.Cards.Add(copiedCard);
                CopyCardCover(list.BoardId, srcCard.Id, copiedCard.Id);
            }

            _context.Lists.Add(copiedList);
            _context.SaveChanges();

            return copiedList;
        }

        public void Update(List list)
        {
            if (list is null)
                throw new ArgumentNullException(nameof(list));
            if (_context.Lists.SingleOrDefault(x => x.Id == list.Id) is null)
                throw new AppException("List not found.");
            
            _context.Lists.Update(list);
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var list = _context.Lists.Find(id);
            if (list is null)
                return;

            _context.Lists.Remove(list);
            _context.SaveChanges();
        }
        
        private void CopyCardCover(Guid boardId, Guid srcCardId, Guid destCardId)
        {
            var boardDir = Path.Combine(_env.WebRootPath, "card-covers", "board-" + boardId);
            if (!Directory.Exists(boardDir))
                return;
            
            var boardDirFiles = Directory.GetFiles(boardDir);
            var srcCardCoverPath = boardDirFiles.SingleOrDefault(x => new FileInfo(x).Name.StartsWith("card-" + srcCardId));
            if (srcCardCoverPath is null)
                return;

            var destCardCoverPath = Path.Combine(boardDir, "card-" + destCardId + (new FileInfo(srcCardCoverPath).Extension));

            File.Copy(srcCardCoverPath, destCardCoverPath);
        }
    }
}