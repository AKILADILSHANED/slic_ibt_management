using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class AdjustmentDetailsBalancesDTO
    {
        [Required]
        public String AdjustmentID { get; set; }

        [Required]
        public DateTime AdjustedDate { get; set; }

        [Required]
        public Decimal AdjustedAmount { get; set; }

        [Required]
        public String Description { get; set; }

        [Required]
        public String Balance { get; set; }
    }
}
