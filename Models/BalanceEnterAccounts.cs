using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class BalanceEnterAccounts
    {
        [Required]
        public String AccountID { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$")]
        public String AccountNumber { get; set; }

        [Required]
        public string AccountType { get; set; }

        [Required]
        public string Currency { get; set; }

        [Required]
        public String BankName { get; set; }
    }
}
