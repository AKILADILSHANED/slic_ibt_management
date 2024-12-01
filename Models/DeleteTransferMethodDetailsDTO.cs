using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class DeleteTransferMethodDetailsDTO
    {
        [Required]
        public String MethodID { get; set; }

        [Required]
        public String FirstName { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$")]
        public String SendingAccount { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$")]
        public String ReceivingAccount { get; set; }

        [Required]
        public String Description { get; set; }

        [Required]
        public int IsDeleted { get; set; }
    }
}
