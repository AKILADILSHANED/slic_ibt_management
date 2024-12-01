using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class ReverseRequestAdjIndividual
    {
        [Required]
        public String AdjustmentID { get; set; }

        [Required]
        public Decimal AdjustedAmount { get; set; }

        [Required]
        public String RequestID { get; set; }
    }
}
