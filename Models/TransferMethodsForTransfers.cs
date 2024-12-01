using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class TransferMethodsForTransfers
    {
        [Required]
        public String MethodID { get; set; }

        [Required]
        public String SendingAccount { get; set; }

        [Required]
        public String ReceivingAccount { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$")]
        public String ReceivingAccountNumber { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$")]
        public String SendingAccountNumber { get; set; }

        [Required]
        public String TransferOption { get; set; }
    }
}
