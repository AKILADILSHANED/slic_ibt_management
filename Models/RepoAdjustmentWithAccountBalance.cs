using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class RepoAdjustmentWithAccountBalance
    {
        [Required]
        public String TransferID { get; set; }

        [Required]
        public String? AdjustmentID { get; set; }

        [Required]
        public Decimal? AdjustedAmount { get; set; }

        [Required]
        public String? Repo { get; set; }
    }
}
