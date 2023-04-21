using KinoSystem.Models.Utilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KinoSystem.Models
{
    public class Person
    {
        public int IdPerson { get; set; }
        public string? Name { get; set; }
        public string? Sername { get; set; }
        public string? MiddleName { get; set; }
        [Required(ErrorMessage ="Почта должна быть введена")]
        [EmailAddress(ErrorMessage ="Почта введена неправильно")]
        public string Login { get; set; }
        [PasswordPropertyText(true)]
        [Required(ErrorMessage = "Пароль должен быть введен")]
        public string Password { get; set; }
        public AccessRight AccessRight { get; set; }
        public int? Salary { get; set; }
        public DateTime? HiringTime { get; set; }
        public List<Seat> Seats { get; set; } = new();
    }
}
