using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;

namespace RunGroupWebApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }
        public bool Add(AppUser user)
        {
            _context.Add(user);
            return Save();
        }

        public bool Delete(AppUser user)
        {
            _context.Remove(user);
            return Save();
        }

        public async Task<IEnumerable<AppUser>> GetAllUsers()
        {
           return await _context.Users.ToListAsync();
        }

        public async Task<AppUser> GetUserId(string id)
        {
            return await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
        }

        public bool Save()
        {
           var saved=_context.SaveChanges();
           return saved>0? true: false;
        }

        public bool Update(AppUser user)
        {
            _context.Update(user);
            return Save();
        }
    }
}
