using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class UpdateAccountDTO
    {
        [Required]
        public String AccountID { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$")]
        public String AccountNumber { get; set; }

        [Required]
        public String AccountBranch { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$")]
        public string AccountType { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$")]
        public string Currency { get; set; }

        [Required]
        [RegularExpression(@"^\d{7}$")]
        public int GLCode { get; set; }

        [Required]
        public DateTime RegisteredDate { get; set; }

        [Required]
        public String BankName { get; set; }

        [Required]
        public String BankID { get; set; }

        [Required]
        public String User_Id { get; set; }

        [Required(ErrorMessage = "Required field.")]
        [StringLength(50, ErrorMessage = "Length of First name should limit to 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Special characters and spaces are not allowed.")]
        public string FirstName { get; set; }

        [Required]
        public int IsDeleted { get; set; }
    }
}
