using Microsoft.AspNetCore.Mvc;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _repository;
        public DashboardController(IDashboardRepository dashboardRepository)
        {
          _repository = dashboardRepository;
        }
        public async Task<IActionResult>Index()
        {
            var userRaces = await _repository.GetAllUserRaces();
            var userClubs= await _repository.GetAllUserClubs();
            var dashViewModel = new DashboardViewModel()
            {
                Races = userRaces,
                Clubs = userClubs,
            };
            return View(dashViewModel);
        }
    }
}
