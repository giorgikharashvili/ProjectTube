using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ProjectTube.Models;

public class VideoPosting
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public DateTime PostedDate { get; set; } = DateTime.UtcNow;
    [Required]
    public string VideoPath { get; set; }
    public string UserId { get; set; } // depending on this UserId we are able to get an access to the identity user of this job posting 
    // vgulisxmob mtlian ROW-s databaseshi
    [ForeignKey(nameof(UserId))] // what is a ForeignKey
    public IdentityUser User { get; set; }

}