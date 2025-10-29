using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PD421_Dashboard_WEB_API.BLL.Dtos.Register
{
    public class RegisterDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        [Required(ErrorMessage = "Почта є обов'язковим полем")]
        public required string Email { get; set; }
        [Required(ErrorMessage ="Ім'я користувача є обов'язковим полем")]
        public required string UserName { get; set; }
        [Required(ErrorMessage = "Пароль є обов'язковим полем")]
        public required string Password { get; set; }
    }
}
