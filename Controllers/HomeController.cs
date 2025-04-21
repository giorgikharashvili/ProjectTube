using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectTube.Data;
using ProjectTube.Models;
using ProjectTube.Repositories;
using ProjectTube.Services;

namespace ProjectTube.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<VideoPosting>  _videoPostingRepository;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context,  IRepository<VideoPosting> videoPostingRepository,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _videoPostingRepository = videoPostingRepository;
            _userManager = userManager;
           
        }

        public async Task<IActionResult> Index()
        {
         


            var videoPosting = await _videoPostingRepository.GetAllAsync();
            var userId = _userManager.GetUserId(User);
            ViewBag.CurrentUserId = userId;
            
            foreach(var video in videoPosting)
            {
                var user = await _userManager.FindByIdAsync(video.UserId);
                if(user != null)
                {
                    video.User = user;
                }
            }
            
            return View(videoPosting);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
