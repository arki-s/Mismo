﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Mismo.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        public string Role {  get; set; }
        public int? Interval { get; set; } //interval of 1on1(days)

        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        public virtual ICollection<Mood> Moods { get; set; }
        public virtual ICollection<OneOnOne> OneOnOnes { get; set; }
        public virtual ICollection<Goal> Goals { get; set; }
        public virtual ICollection<Message> Messages { get; set; }

        

    }
}
