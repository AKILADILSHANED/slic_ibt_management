using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class UpdateUserDTO
    {
        public String User_Id { get; set; }

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

        public DateTime Create_Date_Time { get; set; }

        public String Creator { get; set; }

        public int IsDeleted { get; set; }
    }
}
