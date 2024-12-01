using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLICGL_IBT_Management.Models
{
    public class DisplayTransfers
    {
        [Required]
        public String TransferId { get; set; }

        [Required]
        public DateTime TransferDate { get; set; }

        [Required]
        public String FromAccount { get; set; }

        [Required]
        public String ToAccount{ get; set; }

        [Required]
        public decimal TransferAmount { get; set; }

        [Required]
        public String TransferOption { get; set; }

        [Required]
        public String ApproveStatus { get; set; }

        [Required]
        public String ApprovedBy { get; set; }

        [Required]
        public String DeleteStatus { get; set; }

        [Required]
        public String DeleteBy { get; set; }

        [Required]
        public String Balance { get; set; }

        [Required]
        public String PaymentType { get; set; }

        [Required]
        public String InitiateBy { get; set; }
    }
}
