using Microsoft.AspNetCore.Mvc;
using ProjectTube.Models;
using ProjectTube.Data;
using ProjectTube.ViewModels;
using Microsoft.AspNetCore.Identity;
using ProjectTube.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace ProjectTube.Controllers
{
    public class PostPageController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<PostPageController> _logger;
        private readonly IRepository<VideoPosting> _videoPostingRepository;

        public PostPageController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment,
            ILogger<PostPageController> logger, UserManager<IdentityUser> userManager, IRepository<VideoPosting> videoPostingRepository)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            _userManager = userManager;
            _videoPostingRepository = videoPostingRepository;
        }

        // Show the video posting page (GET request)
        [HttpGet]
        public IActionResult VideoPosting()
        {
            return View(new VideoPostingViewModel());
        }

        // Handle form submission (POST request)
        [HttpPost]
        public async Task<IActionResult> VideoPosting(VideoPostingViewModel videoPostingVm, IFormFile videoFile)
        {
            if (videoFile == null || videoFile.Length == 0)
            {
                _logger.LogError("No video file uploaded.");
                ModelState.AddModelError("VideoFile", "Please upload a valid video file.");
                return View(videoPostingVm);
            }

            string uniqueFileName = string.Empty;
            string relativePath = string.Empty;

            // Handle file upload
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(videoFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            // s
            try
            {
                using (var stream = videoFile.OpenReadStream())
                {
                    var fileStream = System.IO.File.Create(filePath);
                    await stream.CopyToAsync(fileStream);
                    fileStream.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                ModelState.AddModelError("VideoFile", "Error uploading file. Try again.");
                return View(videoPostingVm);
            }

            relativePath = "/uploads/" + uniqueFileName;

            // Assign VideoPath **before** validation
            videoPostingVm.VideoPath = relativePath;

            ModelState.Clear(); // Clear previous model errors since we manually set `VideoPath`
            if (TryValidateModel(videoPostingVm)) // Revalidate model
            {
                var videoPosting = new VideoPosting
                {
                    Title = videoPostingVm.Title,
                    Description = videoPostingVm.Description,
                    UserId = _userManager.GetUserId(User),
                    VideoPath = relativePath
                };

                await _videoPostingRepository.AddAsync(videoPosting);
                return RedirectToAction("Index", "Home");
            }

            return View(videoPostingVm);
        }

    }
}
