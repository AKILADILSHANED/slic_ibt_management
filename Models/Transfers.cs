using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLICGL_IBT_Management.Models
{
    public class Transfers
    {
        [Required]
        public String TransferId { get; set; }

        [Required]
        public DateTime TransferDate { get; set; }

        [Required]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Transfer amount must be a non-negative numeric value with up to two decimal places.")]
        public decimal TransferAmount { get; set; }

        [Required]
        public String FromBankAccount { get; set; }

        [ForeignKey("FromBankAccount")]
        public virtual BankAccount FromAccount { get; set; }

        [Required]
        public String ToBankAccount { get; set; }

        [ForeignKey("ToBankAccount")]
        public virtual BankAccount ToAccount { get; set; }

        [Required]
        public String TransferMethod { get; set; }

        [ForeignKey("TransferMethod")]
        public virtual TransferMethod Method { get; set; }

        [Required]
        public int IsApproved { get; set; }

        public String? ApprovedBy { get; set; }

        [ForeignKey("ApprovedBy")]
        public virtual User Approver { get; set; }

        [Required]
        public int IsDeleted { get; set; }

        public String? DeletedBy { get; set; }

        [ForeignKey("DeletedBy")]
        public virtual User Deleter { get; set; }

        public String? AccountBalance { get; set; }

        [ForeignKey("AccountBalance")]
        public virtual AccountBalance Balance { get; set; }

        [Required]
        public String CreateBy { get; set; }

        [ForeignKey("CreateBy")]
        public virtual User Creator { get; set; }

        public String? Payment { get; set; }

        [ForeignKey("Payment")]
        public virtual Payment TransferPayment { get; set; }

        [Required]
         public string IBTSheet { get; set; }

        [ForeignKey("IBTSheet")]
        public virtual IBTSheet Sheet { get; set; }

        public virtual FundRequestAdjustments RequestAdjustment { get; set; }

    }
}
