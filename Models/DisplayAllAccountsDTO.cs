using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class DisplayAllAccountsDTO
    {
        [Required]
        public String AccountID { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$")]
        public String AccountNumber { get; set; }

        [Required]
        public string AccountType { get; set; }

        [Required]
        public String AccountBranch { get; set; }

        [Required]
        public string Currency { get; set; }

        [Required]
        [RegularExpression(@"^\d{7}$")]
        public int GLCode { get; set; }

        [Required]
        public DateTime RegisteredDate { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        public String BankName { get; set; }

        [Required]
        public string FirstName { get; set; }
    }
}
