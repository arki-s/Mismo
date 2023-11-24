using Mismo.Models;

namespace Mismo.ViewModel
{
    public class UserGoals
    {
        public ApplicationUser User { get; set; }
        public List<Goal> GoalList { get; set; }
        public List<ApplicationUser> Users { get; set; }

    }
}
