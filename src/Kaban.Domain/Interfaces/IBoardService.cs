using System;
using System.Collections.Generic;
using System.IO;
using Kaban.Domain.Models;

namespace Kaban.Domain.Interfaces
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

        void SetBoardWallpaper(Stream formFile, Guid boardId);

        void Delete(Guid id);
    }
}