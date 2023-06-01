using System.ComponentModel.DataAnnotations;

namespace Thss0.Web.Models.ViewModels.Auth
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Required")]
        public string Name { get; set; } = "";

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Required")]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Wrong input")]
        public string PasswordRepeat { get; set; } = "";
    }
}