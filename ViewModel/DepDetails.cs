using Mismo.Models;

namespace Mismo.ViewModel
{
    public class DepDetails
    {
        public Department? Department { get; set; }

        public List<ApplicationUser>? Managers { get; set; }

        public List<ApplicationUser>? Members { get; set; }
    }
}
