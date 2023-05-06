using KinoSystem.Models.Utilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KinoSystem.Models
{
    public class Person
    {
        public int IdPerson { get; set; }
        public string? Name { get; set; }
        public string? Sername { get; set; }
        public string? MiddleName { get; set; }
        [Phone(ErrorMessage = "Номер введен в не правильном формате")]
        [RegularExpression("^(\\+7|7|8)?[\\s\\-]?\\(?[489][0-9]{2}\\)?[\\s\\-]?[0-9]{3}[\\s\\-]?[0-9]{2}[\\s\\-]?[0-9]{2}$", ErrorMessage = "Номер введен в не правильном формате")]
        public string? PhoneNumber { get; set; }
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
