using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class DisplayIndividualTransferForReverse
    {
        [Required]
        public String TransferID { get; set; }

        [Required] 
        public DateTime TransferDate { get; set; }

        [Required]
        public String FromAccount { get; set; }

        [Required]
        public String ToAccount { get; set; }

        [Required]
        public Decimal TransferAmount { get; set; }

        [Required]
        public int IsDeleted { get; set; }

        [Required]
        public int IsApproved { get; set; }
    }
}
