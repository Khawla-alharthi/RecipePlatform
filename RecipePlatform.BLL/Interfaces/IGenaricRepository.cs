using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipePlatform.Models.Models;

namespace RecipePlatform.BLL.Interfaces
{
    public interface IGenaricRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(object id);
        Task<T> Add(T entity);
        Task<T> Update(T entity);
        Task Delete(object id);
        Task<bool> Exists(object id);
        IQueryable<T> GetQueryable();
    }
}
