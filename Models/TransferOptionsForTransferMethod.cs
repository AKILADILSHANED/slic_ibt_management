using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class TransferOptionsForTransferMethod
    {
        [Required]
        public String OptionID { get; set; }

        [Required]
        [StringLength(50)]
        public String OptionDescription { get; set; }
    }
}
