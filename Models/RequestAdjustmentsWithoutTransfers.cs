using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class RequestAdjustmentsWithoutTransfers
    {
        [Required]
        public String RequestID { get; set; }

        [Required]
        public Decimal AdjustedAmount { get; set; }

        [Required]
        public String AdjustmentID { get; set; }
    }
}
