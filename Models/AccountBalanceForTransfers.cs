using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class AccountBalanceForTransfers
    {
        [Required]
        public String BalanceID { get; set; }

        [Required]
        public String AccountID { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$")]
        public String AccountNumber { get; set; }

        [Required]
        [RegularExpression(@"^-?\d+(\.\d+)?$")]
        public decimal OutstandingBalance { get; set; }

        [Required]
        [RegularExpression(@"^-?\d+(\.\d+)?$")]
        public decimal TransferableBalance { get; set; }
    }
}
