using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ProjectTube.Data;
using ProjectTube.Models;
using ProjectTube.Repositories;

namespace ProjectTube.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<VideoPosting>  _videoPostingRepository;
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context,  IRepository<VideoPosting> videoPostingRepository)
        {
            _logger = logger;
            _context = context;
            _videoPostingRepository = videoPostingRepository;
        }

        public IActionResult Index()
        {
            // var videoPosting = await _videoPostingRepository.GetAllAsync();

            
            return View();
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
