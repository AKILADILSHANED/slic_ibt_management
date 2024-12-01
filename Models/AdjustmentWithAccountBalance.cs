using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class AdjustmentWithAccountBalance
    {
        [Required]
        public String TransferID { get; set; }

        [Required]
        public String? AdjustmentID { get; set; }

        [Required]
        public Decimal? AdjustedAmount { get; set; }

        [Required]
        public String? Balance { get; set; }
    }
}
