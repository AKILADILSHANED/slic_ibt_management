using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLICGL_IBT_Management.Models
{
    public class FundRequest
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
        [RegularExpression(@"^(0|[1-9]\d*)(\.\d+)?$")]
        public Decimal RemainingRequestAmount { get; set; }

        [Required]
        public String RequestBy { get; set; }

        [ForeignKey("RequestBy")]
        public virtual User Requester { get; set; }

        [Required]
        public int IsDeleted { get; set; }
                
        public String? DeletedBy { get; set; }

        [ForeignKey("DeletedBy")]
        public virtual User Deleter { get; set; }

        [Required]
        public int IsApproved { get; set; }

        public String? ApprovedBy { get; set; }

        [ForeignKey("ApprovedBy")]
        public virtual User Approver { get; set; }

        [Required]
        public String PaymentID { get; set; }

        [ForeignKey("PaymentID")]
        public virtual Payment PaymentRequest { get; set; }

        [Required]
        public String AccountID { get; set; }

        [ForeignKey("AccountID")]
        public virtual BankAccount RequestedAccount { get; set; }

        public ICollection<FundRequestAdjustments> FundRequestAdjustments { get; set; }

        public virtual OverdraftRecoverAdjustment OverdraftRecoverAdjustment { get; set; }
    }
}
