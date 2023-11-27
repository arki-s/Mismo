using Mismo.Models;

namespace Mismo.ViewModel
{
    public class UserMoods
    {
        public ApplicationUser User { get; set; }
        public List<Mood>? MoodList { get; set; }
        //public List<ApplicationUser>? Users { get; set; }
    }
}
