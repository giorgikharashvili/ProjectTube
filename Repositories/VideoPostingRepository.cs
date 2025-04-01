using Microsoft.EntityFrameworkCore;
using ProjectTube.Data;
using ProjectTube.Models;

namespace ProjectTube.Repositories
{
    public class VideoPostingRepository : IRepository<VideoPosting>
    {

        private readonly ApplicationDbContext _context;
        public VideoPostingRepository(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task AddAsync(VideoPosting entity)
        {
            await _context.VideoPostings.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var videoPosting = await _context.VideoPostings.FindAsync(id);

            if(videoPosting == null)
            {
                throw new KeyNotFoundException();
            }
            // await does not needed here because Remove() is not an asynchronous method
            _context.VideoPostings.Remove(videoPosting);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<VideoPosting>> GetAllAsync()
        {
            // Retrives all data from VideoPostings table in database
            return await _context.VideoPostings.ToListAsync();
        }

        public async Task<VideoPosting> GetByIdAsync(int id)
        {
            var videoPosting = await _context.VideoPostings.FindAsync(id);

            if(videoPosting == null)
            {
                throw new KeyNotFoundException();
            }

            return videoPosting;
        }

        public async Task UpdateAsync(VideoPosting entity)
        {
            _context.VideoPostings.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
