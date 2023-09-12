using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using System.Linq;

namespace RunGroupWebApp.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ApplicationDbContext _context;
        public DashboardRepository(ApplicationDbContext applicationDbContext,IHttpContextAccessor httpContextAccessor)
        {
            _context= applicationDbContext;
            _contextAccessor= httpContextAccessor;
        }
        public  async Task<List<Club>> GetAllUserClubs()
        {
            var curUser = _contextAccessor.HttpContext.User.GetUserId();
            var userClubs =_context.Clubs.Where(r => r.AppUser.Id == curUser);
            return (userClubs.ToList());
        }

        public async Task<List<Race>> GetAllUserRaces()
        {
            var curUser = _contextAccessor.HttpContext.User.GetUserId();
            var userRaces = _context.Races.Where(r => r.AppUser.Id == curUser);
            return (userRaces.ToList());
        }
        public async Task<AppUser>GetUserById(string id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<AppUser> GetUserByIdNoTracking(string id)
        {
            return await _context.Users.Where(u=>u.Id==id).AsNoTracking().FirstOrDefaultAsync();
        }
        public bool Update(AppUser user)
        {
            _context.Users.Update(user);
            return Save();
        }
        public bool Save() {
              var  saved= _context.SaveChanges();
            return saved > 0 ? true : false;
        }

    }
}
