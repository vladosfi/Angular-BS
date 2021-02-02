using System.Collections.Generic;
using System.Threading.Tasks;
using BS.API.Models;

namespace BS.API.Data
{
    public interface IBSRepository
    {
        void Add<T>(T entity) where T : class;

        void Delete<T>(T entity) where T : class;

        Task<bool> SaveAll();

        Task<IEnumerable<User>> GetUsers();

        Task<User> GetUser(int id);
    }
}