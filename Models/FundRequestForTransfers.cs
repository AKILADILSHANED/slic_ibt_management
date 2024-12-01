using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class FundRequestForTransfers
    {
        [Required]
        public String RequestID { get; set; }

        [Required]
        public String PaymentID { get; set; }

        [Required]
        public String AccountID { get; set; }

        [Required]
        public Decimal OutstandingBalance { get; set; }

        [Required]
        public Decimal RemainingRequestAmount { get; set; }

        [Required]
        public String PaymentType { get; set; }

        [Required]
        [RegularExpression(@"^(0|[1-9]\d*)(\.\d+)?$")]
        public Decimal RequestAmount { get; set; }
    }
}
