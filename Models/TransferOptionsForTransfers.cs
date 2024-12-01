using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class TransferOptionsForTransfers
    {
        [Required]
        public String OptionID { get; set; }

        [Required]
        [StringLength(20)]
        public String OptionName { get; set; }

        [Required]
        public int Priority { get; set; }
    }
}
