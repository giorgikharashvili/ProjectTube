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
using Microsoft.AspNetCore.Authorization;
using ProjectTube.Constants;

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


        #region VideoPostingMethod
        // Handle form submission (POST request)
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> VideoPosting(VideoPostingViewModel videoPostingVm, IFormFile videoFile, IFormFile thumbnailFile)
        {
            #region VideoUpload
            if (videoFile == null || videoFile.Length == 0)
            {
                _logger.LogError("No video file uploaded.");
                ModelState.AddModelError("VideoFile", "Please upload a valid video file.");
                return View(videoPostingVm);
            }

            string uniqueFileName = string.Empty;
            string relativePath = string.Empty;
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(videoFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

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
            videoPostingVm.VideoPath = relativePath;
            #endregion

            #region ThumbnailUpload
            if (thumbnailFile == null || thumbnailFile.Length == 0)
            {
                _logger.LogError("No thumbnail file uploaded.");
                ModelState.AddModelError("ThumbnailFile", "Please upload a valid thumbnail file.");
                return View(videoPostingVm); // Thumbnail error handling
            }

            var uniqueThumbnailName = Guid.NewGuid().ToString() + Path.GetExtension(thumbnailFile.FileName);
            var thumbnailUploadFolder = Path.Combine(uploadsFolder, "thumbnails");

            if (!Directory.Exists(thumbnailUploadFolder))
            {
                Directory.CreateDirectory(thumbnailUploadFolder);
            }

            var thumbnailPath = Path.Combine(thumbnailUploadFolder, uniqueThumbnailName);

            try
            {
                using (var stream = thumbnailFile.OpenReadStream())
                {
                    var fileStream = System.IO.File.Create(thumbnailPath);
                    await stream.CopyToAsync(fileStream);
                    fileStream.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                ModelState.AddModelError("ThumbnailFile", "Error uploading file. Try again.");
                return View(videoPostingVm);
            }

            var relativePathForThumbnail = "/uploads/thumbnails/" + uniqueThumbnailName;
            videoPostingVm.ThumbnailPath = relativePathForThumbnail; // Assign ThumbnailPath
            #endregion

            ModelState.Clear(); // Clear previous model errors
            if (TryValidateModel(videoPostingVm)) // Revalidate model after setting paths
            {
                var videoPosting = new VideoPosting
                {
                    Title = videoPostingVm.Title,
                    Description = videoPostingVm.Description,
                    UserId = _userManager.GetUserId(User),
                    VideoPath = relativePath,
                    ThumbnailPath = relativePathForThumbnail
                };

                ViewBag.CurrentUserId = _userManager.GetUserId(User);
                await _videoPostingRepository.AddAsync(videoPosting);
                return RedirectToAction("Index", "Home");
            }

            return View(videoPostingVm);
        }
        #endregion

        #region DeleteVideoPosting
        [HttpDelete]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> DeletePosting(int id)
        {
            var videoPosting = await _videoPostingRepository.GetByIdAsync(id);
            var currentUserId = _userManager.GetUserId(User);

            if(videoPosting == null)
            {
                return View("Error");
            }

            if(!User.IsInRole(Roles.Admin) && videoPosting.UserId != currentUserId)
            {
                return Forbid();
            }

            await _videoPostingRepository.DeleteAsync(videoPosting.Id);
            

            return Ok();
            
        }


        #endregion

    }
}
