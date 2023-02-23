using System.ComponentModel.DataAnnotations;

namespace Thss0.DAL.Context
{
    public class Registration
    {
        [Required(ErrorMessage ="Required")]
        public string Name { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Required")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Wrong input")]
        public string PasswordRepeat { get; set; }
    }
}
