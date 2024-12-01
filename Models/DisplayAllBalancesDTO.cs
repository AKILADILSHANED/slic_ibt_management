using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class DisplayAllBalancesDTO
    {
        [Required]
        public String BalanceID { get; set; }

        [Required]
        public DateTime BalanceDate { get; set; }

        [Required]
        [RegularExpression(@"^-?\d+(\.\d+)?$")]
        public Decimal BalanceAmount { get; set; }

        [Required]
        [RegularExpression(@"^-?\d+(\.\d+)?$")]
        public Decimal OutstandingBalance { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        public String BankName { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$")]
        public String AccountNumber { get; set; }

        [Required]
        public string FirstName { get; set; }
    }
}
