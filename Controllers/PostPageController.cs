using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProjectTube.Models;
using ProjectTube.Data;
using ProjectTube.ViewModels;
using Microsoft.AspNetCore.Identity;
using ProjectTube.Repositories;

namespace ProjectTube.Controllers;

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

    // IFromFile gets the video that was submitted with this line 
    // <input type="file" name="videoFile" id="videoinput" class="form-control" accept="video/*"/>
    [HttpGet]                                                            // In HTML
    public async Task<IActionResult> VideoPosting(VideoPostingViewModel videoPostingVm, IFormFile videoFile)
    {
        string uniqueFileName = null; // Ensure its initialized
        var filePath = string.Empty;
        string relativePath = string.Empty;
        // videoFile.Lenght Ensures that videoFile has any content in it
        if (videoFile != null && videoFile.Length > 0)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            uniqueFileName = Guid.NewGuid().ToString() + "_" + videoFile.FileName;
            filePath = Path.Combine(uploadsFolder, uniqueFileName);

            if (videoFile != null)
            {
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
                }
            }

            relativePath = Path.Combine("/uploads/" + uniqueFileName);
        }

        

        if (ModelState.IsValid)
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