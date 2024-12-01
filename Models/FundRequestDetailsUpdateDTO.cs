using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class FundRequestDetailsUpdateDTO
    {
        [Required]
        public String RequestID { get; set; }

        [Required]
        public DateTime RequestDate { get; set; }

        [Required]
        public DateTime RequiredDate { get; set; }

        [Required]
        [RegularExpression(@"^(0|[1-9]\d*)(\.\d+)?$")]
        public Decimal RequestAmount { get; set; }

        [Required]
        public Decimal RemainingRequestAmount { get; set; }

        public int? IsDeleted { get; set; }

        [Required]
        public String IsApproved { get; set; }

        [Required]
        public String RequestBy { get; set; }

        [Required]
        public String ApprovedBy { get; set; }

        [Required]
        public String RequestReason { get; set; }

        [Required]
        public String AccountNumber { get; set; }
    }
}
