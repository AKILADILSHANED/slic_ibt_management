using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class NewFundRequest
    {
        public Dictionary<String, String>? PaymentList { get; set; }

        public Dictionary<String, String>? AccountList { get; set; }

        [Required]
        [RegularExpression(@"^(0|[1-9]\d*)(\.\d+)?$")]
        public Decimal RequestAmount { get; set; }

        [Required]
        public String PaymentID { get; set; }

        [Required]
        public String AccountID { get; set; }
    }
}
