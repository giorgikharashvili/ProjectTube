namespace ProjectTube.ViewModels;

public class VideoPostingViewModel
{
  
    public string Title { get; set; }
    
    public string Description { get; set; }
    
    public DateTime PostedDate { get; set; } = DateTime.UtcNow;
    
    public string VideoPath { get; set; }
    
    public string ThumbnailPath { get; set; }
}