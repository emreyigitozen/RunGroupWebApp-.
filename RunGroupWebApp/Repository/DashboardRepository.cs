using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;

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
            var curUser = _contextAccessor.HttpContext.User;
            var userClubs =_context.Clubs.Where(r => r.AppUser.Id == curUser.ToString());
            return (userClubs.ToList());
        }

        public async Task<List<Race>> GetAllUserRaces()
        {
            var curUser = _contextAccessor.HttpContext.User;
            var userRaces = _context.Races.Where(r => r.AppUser.Id == curUser.ToString());
            return (userRaces.ToList());
        }
    }
}
