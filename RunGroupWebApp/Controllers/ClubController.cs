using Microsoft.AspNetCore.Mvc;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class ClubController :Controller
    {
        private readonly IClubRepository _clubRepository;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpcontextAccessor;

        public ClubController(IClubRepository clubRepository, IPhotoService photoService, IHttpContextAccessor contextAccessor)
        {
            _clubRepository = clubRepository;
            _photoService = photoService;
            _httpcontextAccessor = contextAccessor;
        }
        public async Task<IActionResult> Index() //C
        {

            IEnumerable<Club> clubs = await _clubRepository.GetAll();//M
            return View(clubs); //V
        }
        public async Task<IActionResult> Detail(int id)
        {
            Club club = await _clubRepository.GetByIdAsync(id);
            return View(club);
        }
        public IActionResult Create()
        {
            var curUserId = _httpcontextAccessor.HttpContext.User.GetUserId();
            var createClubViewModel = new CreateClubViewModel
            {
                AppUserId = curUserId,

            };
            return View(createClubViewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateRaceViewModel clubvm)
        {
            if (ModelState.IsValid)
            {
                var result = await _photoService.AddPhotoAsync(clubvm.Image);
                var club = new Club
                {
                    Title = clubvm.Title,
                    Description = clubvm.Description,
                    Image = result.Url.ToString(),
                    AppUserId=clubvm.AppUserId,
                    Address = new Address
                    {
                        City = clubvm.Address.City,
                        Street = clubvm.Address.Street,
                        State = clubvm.Address.State

                    }

                };
                _clubRepository.Add(club);
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Photo Upload failed");
            }
            return View(clubvm);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var club = await _clubRepository.GetByIdAsync(id);
            if (club == null)
            {
                return View("Error");
            }
            var clubVM = new EditClubViewModel
            {
                Title = club.Title,
                Description = club.Description,
                AddressId = (int)club.AddressId,
                Address = club.Address,
                URL = club.Image
                ,
                ClubCategory = club.ClubCategory
            };
            return View(clubVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditClubViewModel clubVM)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Failed to edit.");
                return View("Edit", clubVM);
            }
            var userClub = await _clubRepository.GetByIdAsyncNoTracking(id);
            if (userClub != null)
            {
                try
                {
                    await _photoService.DeletePhotoAsync(userClub.Image);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Could not delete the photo");
                    return View(clubVM);
                }
                var photoResult = await _photoService.AddPhotoAsync(clubVM.Image);
                var club = new Club
                {
                    Id = id,
                    Title = clubVM.Title
                    ,
                    Description = clubVM.Description,
                    Image = photoResult.Url.ToString(),
                    AddressId = clubVM.AddressId,
                    Address = clubVM.Address,

                };

                _clubRepository.Update(club);
                return RedirectToAction("Index");


            }
            else
            {
                return View(clubVM);
            }

        }
    }
}
