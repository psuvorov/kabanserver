using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using Kaban.API.Helpers;
using Kaban.Domain.Enums;
using Kaban.Domain.Interfaces;
using Kaban.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Kaban.Database.Services
{
    public class CardService : ICardService
    {
        private readonly DataContext _context;
        private readonly string _webRootPath;

        public CardService(DataContext context, string webRootPath)
        {
            _context = context;
            _webRootPath = webRootPath;
        }

        public IEnumerable<Card> GetAll()
        {
            return _context.Cards;
        }

        public IEnumerable<Card> GetAll(List list)
        {
            if (list is null)
                throw new ArgumentNullException(nameof(list));
            
            return _context.Cards.Where(card => card.ListId == list.Id);
        }

        public Card Get(Guid id)
        {
            var card = _context.Cards
                .Include(x => x.List)
                .SingleOrDefault(x => x.Id == id);
            
            if (card is null)
                throw new Exception($"Card with '{id}' not found.");

            return card;
        }

        public IEnumerable<Card> GetArchivedCards(Guid boardId)
        {
            var cards = _context.Cards
                .IgnoreQueryFilters()
                .Include(x => x.List)
                .Where(x => x.IsArchived && !x.IsDeleted)
                .OrderByDescending(x => x.Archived)
                .ToList();

            return cards;
        }
        
        public Card Create(Card card)
        {
            if (card is null)
                throw new ArgumentNullException(nameof(card));

            _context.Cards.Add(card);
            _context.SaveChanges();

            return card;
        }

        public void Update(Card card)
        {
            if (card is null)
                throw new ArgumentNullException(nameof(card));

            var storedCard = _context.Cards.SingleOrDefault(x => x.Id == card.Id);
            if (storedCard is null)
                throw new Exception("Card not found.");

            _context.Cards.Update(card);
            _context.SaveChanges();
        }
        
        public void SetCardCover(Stream formFile, Guid boardId, Guid cardId)
        {
            var targetPath = Path.Combine(_webRootPath, "card-covers", "board-" + boardId);

            var boardCoversDir = Directory.CreateDirectory(targetPath);

            // var filePath = Path.Combine(targetPath, "card-" + cardId + new FileInfo(formFile.FileName).Extension);
            var filePath = Path.Combine(targetPath, "card-" + cardId + new FileInfo("fileName").Extension);


            var fileInfos = boardCoversDir.GetFiles("card-" + cardId + "*");

            // Remove previous card cover
            foreach (var fileInfo in fileInfos)
            {
                File.Delete(fileInfo.FullName);
            }

            using (var memoryStream = new MemoryStream())
            {
                formFile.CopyTo(memoryStream);
                Bitmap resizedImg;
                using (var image = Image.FromStream(memoryStream))
                {
                    if (image.Width >= image.Height)
                    {
                        int width = image.Width * 136 / image.Height;
                        resizedImg = ImageHelper.Resize(image, width, 136);
                    }
                    else
                    {
                        int width = image.Width * 240 / image.Height;
                        resizedImg = ImageHelper.Resize(image, width, 240);
                    }
                }
                            
                resizedImg.Save(filePath);
            }
        }
        
        public Tuple<string, CoverImageOrientation> GetCardCoverInfo(Guid boardId, Guid cardId)
        {
            var cardDir = Path.Combine(_webRootPath, "card-covers", "board-" + boardId);
            if (!Directory.Exists(cardDir))
                return new Tuple<string, CoverImageOrientation>(string.Empty, default(CoverImageOrientation));
            
            var cardDirFiles = Directory.GetFiles(cardDir);
            var cardCoverFullPath = cardDirFiles.SingleOrDefault(x => new FileInfo(x).Name.StartsWith("card-" + cardId));
            if (cardCoverFullPath is null)
                return new Tuple<string, CoverImageOrientation>(string.Empty, default(CoverImageOrientation));

            var staticFilesContentDir = new DirectoryInfo(_webRootPath).Name;

            var cardCoverPath = cardCoverFullPath.Substring(cardCoverFullPath.IndexOf(staticFilesContentDir, StringComparison.Ordinal) +
                                                    staticFilesContentDir.Length);

            cardCoverPath = cardCoverPath.Replace("\\", "/");
            cardCoverPath = cardCoverPath + "?" + DateTime.Now.Ticks; 

            CoverImageOrientation orientation;
            using (var image = Image.FromFile(cardCoverFullPath))
            {
                orientation = image.Width >= image.Height
                    ? CoverImageOrientation.Horizontal
                    : CoverImageOrientation.Vertical;
            }

            return new Tuple<string, CoverImageOrientation>(cardCoverPath, orientation);
        }

        public void Delete(Guid id)
        {
            var card = _context.Cards
                .IgnoreQueryFilters()
                .SingleOrDefault(x => x.Id == id);
            if (card is null)
                return;

            _context.Cards.Remove(card);
            _context.SaveChanges();
        }
    }
}