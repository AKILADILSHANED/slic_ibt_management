using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class RegisterBankAccount
    {
        public Dictionary<String, String>? BankList { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$")]
        public String AccountNumber { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$")]
        public string AccountType { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Length of Account branch should limit to 50 characters.")]
        public String AccountBranch { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$")]
        public string Currency { get; set; }

        [Required]
        [RegularExpression(@"^\d{7}$")]
        public int GLCode { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z0-9/]+$")]
        public string BankID { get; set; }

    }
}
