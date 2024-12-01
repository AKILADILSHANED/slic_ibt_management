using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLICGL_IBT_Management.Models
{
    public class TransferMethod
    {
        [Required]
        public String MethodID { get; set; }

        [Required]
        public String SendingAccount { get; set; }

        [ForeignKey("SendingAccount")]
        public virtual BankAccount AccountSender { get; set; }

        [Required]
        public String ReceivingAccount { get; set; }

        [ForeignKey("ReceivingAccount")]
        public virtual BankAccount AccountReceiver { get; set; }

        [Required]
        public String TransferOption { get; set; }

        [ForeignKey("TransferOption")]
        public virtual TransferOption Option { get; set; }

        [Required]
        public String CreateBy { get; set; }

        [ForeignKey("CreateBy")]
        public virtual User Creator { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public int IsDeleted { get; set; }

        public String? DeletedBy { get; set; }

        [ForeignKey("DeletedBy")]
        public virtual User Deleter { get; set; }

        [Required]
        public int IsActive { get; set; }
    }
}
