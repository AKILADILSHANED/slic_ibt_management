using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class ReverseBalanceAdjIndividual
    {
        [Required]
        public String AdjustmentID { get; set; }

        [Required]
        public Decimal AdjustedAmount { get; set; }

        [Required]
        public String BalanceID { get; set; }
    }
}
