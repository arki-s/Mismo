using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mismo.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<ApplicationUser> Users { get; set; }
    }

    
}
