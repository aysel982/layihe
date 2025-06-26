using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.ViewModels
{
    public class LoginVM
    {
        [MaxLength(256)]
        [MinLength(8)]

        public string UsernameOrEmail { get; set; }
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsPersistant { get; set; }
    }
}
