using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardRepository _repository;
        private readonly IHttpContextAccessor _httpcontextAccessor;
        private readonly IPhotoService _photoService;

        public DashboardController(IDashboardRepository dashboardRepository, IHttpContextAccessor contextAccessor,IPhotoService photoService)
        {
            _repository = dashboardRepository;
            _httpcontextAccessor = contextAccessor;
            _photoService = photoService;
        }
        private void MapUserEdit(AppUser user,EditUserDashboardViewModel editVM,ImageUploadResult photoresult)
        {
            user.Id=editVM
                .Id;
            user.Pace=editVM.Pace;
            user.Mileage=editVM.Mileage;
            user.ProfileImageUrl=photoresult.Url.ToString();
            user.City=editVM.City;
            user.State=editVM.State;
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
        public async Task<IActionResult> EditUserProfile()
        {
            var curUserId = _httpcontextAccessor.HttpContext.User.GetUserId();
            var user= await _repository.GetUserById(curUserId);
            if(user == null)
            {
                return View("Error");
            }
            var editUserViewModel = new EditUserDashboardViewModel()
            {
                Id=curUserId,
                Pace=user.Pace,
                Mileage=user.Mileage,
                ProfileImageUrl=user.ProfileImageUrl,
                City=user.City,
                State=user.State
            };
            return View(editUserViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> EditUserProfile(EditUserDashboardViewModel editVM)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit");
                return View("EditUserProfile", editVM);
            }
            var user = await _repository.GetUserByIdNoTracking(editVM.Id);
            if(user.ProfileImageUrl=="" || user.ProfileImageUrl == null)
            {
                var photoResult = await _photoService.AddPhotoAsync(editVM.Image);
                MapUserEdit(user, editVM, photoResult);
                _repository.Update(user);
                return RedirectToAction("Index");
            }
            else
            {
                try
                {
                    await _photoService.DeletePhotoAsync(user.ProfileImageUrl);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete the photo");
                    return View(editVM);
                }
                var photoResult = await _photoService.AddPhotoAsync(editVM.Image);
                MapUserEdit(user, editVM, photoResult);
                _repository.Update(user);
                return RedirectToAction("Index");
            }
        }
    }
}
