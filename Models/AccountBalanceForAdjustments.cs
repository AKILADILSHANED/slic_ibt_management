using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class AccountBalanceForAdjustments
    {
        [Required]
        public String BalanceID { get; set; }

        [Required]
        public Decimal BalanceAmount { get; set; }

        [Required]
        public Decimal OutstandingBalance { get; set; }

        [Required]
        public String AccountNumber { get; set; }

        [Required]
        public DateTime BalanceDate { get; set; }

        [Required]
        public int IsDeleted { get; set; }
    }
}
