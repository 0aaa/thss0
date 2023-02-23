using System.ComponentModel.DataAnnotations;

namespace Thss0.DAL.Context
{
    public class SignIn
    {
        [Required(ErrorMessage ="Required")]
        public string Name { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Required")]
        public string Password { get; set; }
        public string ReturnURL { get; set; }
    }
}
