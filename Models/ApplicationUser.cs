using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Mismo.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public int? Interval { get; set; } //interval of 1on1(days)
        [Required]
        public string Department { get; set; }
        public virtual ICollection<Mood> Moods { get; set; }
        public virtual ICollection<OneOnOne> OneOnOnes { get; set; }
        public virtual ICollection<Goal> Goals { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
