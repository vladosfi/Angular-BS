using System.Collections.Generic;
using System.Threading.Tasks;
using BS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BS.API.Data
{
    public class BSRepository : IBSRepository
    {
        private readonly DataContext context;

        public BSRepository(DataContext context )
        {
            this.context = context;
        }

        public void Add<T>(T entity) where T : class
        {
            this.context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            this.context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await this.context.Users.Include(p=>p.Photos).FirstOrDefaultAsync(u=>u.Id == id);

            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var users = await this.context.Users.Include(p=>p.Photos).ToListAsync();

            return users;
        }

        public async Task<bool> SaveAll()
        {
            return await this.context.SaveChangesAsync() > 0;
        }
    }
}