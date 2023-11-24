using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mismo.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<UserDepartment> UserDepartments { get; set; }
    }

    public class UserDepartment
    {
        public int DepartmentID { get; set; }

        public Department Department { get; set; }

        [ForeignKey("UserId")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

    }
}
