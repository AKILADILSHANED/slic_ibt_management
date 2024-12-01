using System.ComponentModel.DataAnnotations;
namespace SLICGL_IBT_Management.Models
{
    public class UserLogin
    {
        [Required(ErrorMessage = "Required field.")]
        [StringLength(10, MinimumLength = 5, ErrorMessage = "Length of User Name should be between 5 and 10 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Special characters and spaces are not allowed.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Required field.")]
        [StringLength(10, MinimumLength = 5, ErrorMessage = "Length of Password should be between 5 and 10 characters.")]
        public string Password { get; set; }

        
    }
}
