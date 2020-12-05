using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using KabanServer.Data;
using KabanServer.Entities;
using KabanServer.Exceptions;
using KabanServer.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KabanServer.Services
{
    public interface IBoardService
    {
        IEnumerable<Board> GetAll();

        Board Get(Guid id);

        Board GetInfo(Guid id);
        
        IEnumerable<Board> GetAll(User user);
        
        string GetWallpaperPath(Guid boardId);
        
        string GetBoardWallpaperPreviewPath(Guid boardId);
        
        Board Create(Board board);

        void Update(Board board);

        void SetBoardWallpaper(IFormFile formFile, Guid boardId);

        void Delete(Guid id);
    }
    
    public class BoardService : IBoardService
    {
        private readonly DataContext _context;
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _env;
    
        public BoardService(DataContext context, IUserService userService, IWebHostEnvironment env)
        {
            _context = context;
            _userService = userService;
            _env = env;
        }


        public IEnumerable<Board> GetAll()
        {
            throw new NotImplementedException();
        }

        public Board Get(Guid id)
        {
            var board = _context.Boards.Find(id);
            if (board is null)
                throw new AppException("Board not found.");

            var lists = _context.Lists.Where(l => l.BoardId == board.Id);
            foreach (var list in lists)
            {
                _context.Entry(list).Collection(lst => lst.Cards).Query().OrderBy(card => card.OrderNumber).Load();
            }

            lists = lists.OrderBy(lst => lst.OrderNumber);

            board.Lists = lists.ToList();
            
            return board;
        }

        public Board GetInfo(Guid id)
        {
            var board = _context.Boards.Find(id);
            if (board is null)
                throw new AppException("Board not found.");

            return board;
        }

        public IEnumerable<Board> GetAll(User user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user));
            
            return _context.Boards.Where(board => board.CreatedBy.Id == user.Id);
        }

        public string GetWallpaperPath(Guid boardId)
        {
            var boardDir = Path.Combine(_env.WebRootPath, "board-wallpapers");
            if (!Directory.Exists(boardDir))
                return string.Empty;
            
            var boardDirFiles = Directory.GetFiles(boardDir);
            var boardWallpaperFullPath = boardDirFiles.SingleOrDefault(x => new FileInfo(x).Name.StartsWith("board-" + boardId));
            if (boardWallpaperFullPath is null)
                return string.Empty;
            
            var staticFilesContentDir = new DirectoryInfo(_env.WebRootPath).Name;
            
            var boardWallpaperPath = boardWallpaperFullPath.Substring(boardWallpaperFullPath.IndexOf(staticFilesContentDir, StringComparison.Ordinal) +
                                                                      staticFilesContentDir.Length);

            boardWallpaperPath = boardWallpaperPath.Replace("\\", "/");
            boardWallpaperPath = boardWallpaperPath + "?" + DateTime.Now.Ticks;

            return boardWallpaperPath;
        }
        
        public string GetBoardWallpaperPreviewPath(Guid boardId)
        {
            var boardDir = Path.Combine(_env.WebRootPath, "board-wallpapers");
            if (!Directory.Exists(boardDir))
                return string.Empty;
            
            var boardDirFiles = Directory.GetFiles(boardDir);
            var boardWallpaperPreviewFullPath = boardDirFiles.SingleOrDefault(x => new FileInfo(x).Name.StartsWith("preview-board-" + boardId));
            if (boardWallpaperPreviewFullPath is null)
                return string.Empty;
            
            var staticFilesContentDir = new DirectoryInfo(_env.WebRootPath).Name;
            
            var boardWallpaperPreviewPath = boardWallpaperPreviewFullPath.Substring(boardWallpaperPreviewFullPath.IndexOf(staticFilesContentDir, StringComparison.Ordinal) +
                                                                      staticFilesContentDir.Length);

            boardWallpaperPreviewPath = boardWallpaperPreviewPath + "?" + DateTime.Now.Ticks;

            return boardWallpaperPreviewPath;
        }

        public Board Create(Board board)
        {
            if (board is null)
                throw new ArgumentNullException(nameof(board));
            if (_context.Boards.Any(b => b.Name == board.Name))
                throw new AppException($"Board with '{board.Name}' name already exists.");

            _context.Boards.Add(board);
            _context.SaveChanges();

            return board;
        }

        public void Update(Board board)
        {
            if (board is null)
                throw new ArgumentNullException(nameof(board));
            var storedBoard = _context.Boards.SingleOrDefault(x => x.Id == board.Id);
            if (storedBoard is null)
                throw new AppException("Board not found.");
            if (_context.Boards.Any(b => b.Id != board.Id && b.Name == board.Name && b.CreatedBy.Id == _userService.GetCurrentUser().Id))
                throw new AppException($"Board with '{board.Name}' name already exists.");
            
            _context.Boards.Update(board);
            _context.SaveChanges();
        }
        
        public void SetBoardWallpaper(IFormFile formFile, Guid boardId)
        {
            var targetPath = Path.Combine(_env.WebRootPath, "board-wallpapers");

            var boardWallpapersDir = Directory.CreateDirectory(targetPath);

            var fullImgFilePath = Path.Combine(targetPath, "board-" + boardId + new FileInfo(formFile.FileName).Extension);

            // TODO: refactor this

            // Remove previous wallpaper
            var fullImgFileInfos = boardWallpapersDir.GetFiles("board-" + boardId + "*");
            foreach (var fileInfo in fullImgFileInfos)
            {
                File.Delete(fileInfo.FullName);
            }
            
            // Remove previous and its preview
            var previewImgFileInfos = boardWallpapersDir.GetFiles("preview-board-" + boardId + "*");
            foreach (var fileInfo in previewImgFileInfos)
            {
                File.Delete(fileInfo.FullName);
            }
            

            using (var fileStream = new FileStream(fullImgFilePath, FileMode.Create))
            {
                formFile.CopyTo(fileStream);
            }
            
            var previewImgFilePath = Path.Combine(targetPath, "preview-board-" + boardId + new FileInfo(formFile.FileName).Extension);
            
            using (var memoryStream = new MemoryStream())
            {
                formFile.CopyTo(memoryStream);
                using (var image = Image.FromStream(memoryStream))
                {
                    int width = image.Width * 136 / image.Height;
                    var resizedImg = ImageHelper.Resize(image, width, 136);
                    resizedImg.Save(previewImgFilePath);
                }
            }
        }

        public void Delete(Guid id)
        {
            var board = _context.Boards.Find(id);
            if (board is null)
                return;

            _context.Boards.Remove(board);
            _context.SaveChanges();
        }
    }
}