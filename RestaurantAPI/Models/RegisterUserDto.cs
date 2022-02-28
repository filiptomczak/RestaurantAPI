using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI.Models
{
    public class RegisterUserDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Nationality { get; set; }
        public int RoleId { get; set; } = 1;

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
