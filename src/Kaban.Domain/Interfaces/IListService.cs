using System;
using System.Collections.Generic;
using Kaban.Domain.Models;

namespace Kaban.Domain.Interfaces
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
}