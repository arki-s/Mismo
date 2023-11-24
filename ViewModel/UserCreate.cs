using Mismo.Models;

namespace Mismo.ViewModel
{
    public class UserCreate
    {
        public Goal Goal { get; set; }
        public ApplicationUser User { get; set; }
        public List<ApplicationUser> Users { get; set; }
    }
}
