using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class RegisterUser
    {

        [Required(ErrorMessage = "Required field.")]
        [StringLength(50, ErrorMessage = "Length of First name should limit to 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Special characters and spaces are not allowed.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Required field.")]
        [StringLength(50, ErrorMessage = "Length of Last name should limit to 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Special characters and spaces are not allowed.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Required field.")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Only numeric values are allowed.")]
        public int Epf { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Length of Last name should limit to 50 characters.")]
        public String Position { get; set; }

        [Required(ErrorMessage = "Required field.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Required field.")]
        [StringLength(10, MinimumLength = 5, ErrorMessage = "Length of User Name should be between 5 and 10 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Special characters and spaces are not allowed.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Required field.")]
        [StringLength(10, MinimumLength = 5, ErrorMessage = "Length of Password should be between 5 and 10 characters.")]
        public string Password { get; set; }

        [Required]
        public bool UserCategoryAdmin { get; set; }

        [Required]
        public bool UserCategoryInitiator { get; set; }

        [Required]
        public bool UserCategorySupervisor { get; set; }

        [Required]
        public bool UserCategoryApprover { get; set; }

    }
}
