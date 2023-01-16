using System.ComponentModel.DataAnnotations;

namespace Thss0.Web.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Required")]
        public string Name { get; set; } = "";

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Required")]
        public string Password { get; set; } = "";

        public string ReturnURL { get; set; } = "";
    }
}