using Microsoft.AspNetCore.Identity;

namespace Mismo.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? Interval { get; set; } //interval of 1on1(days)
        public string Department { get; set; }


        public virtual ICollection<Mood> Moods { get; set; }
        public virtual ICollection<OneOnOne> OneOnOnes { get; set; }
        public virtual ICollection<Goal> Goals { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
