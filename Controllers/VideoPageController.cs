using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectTube.Data;
using ProjectTube.Models;
using ProjectTube.Repositories;

namespace ProjectTube.Controllers
{
    public class VideoPageController : Controller
    {
        private readonly IRepository<VideoPosting> _videoPostingRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        public VideoPageController(IRepository<VideoPosting> videoPostingRepository, UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _videoPostingRepository = videoPostingRepository;
        }


        public async Task<IActionResult> VideoPage(int id)
        {
            var video = await _videoPostingRepository.GetByIdAsync(id);
            
            var currentUserId = _userManager.GetUserId(User);
            if (video == null)
            {
                return NotFound();
            }

            return View(video);
        }

    }
}
