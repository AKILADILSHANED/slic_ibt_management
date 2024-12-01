using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class ReverseRepoBalanceAdjIndividual
    {
        [Required]
        public String AdjustmentID { get; set; }

        [Required]
        public Decimal AdjustedAmount { get; set; }

        [Required]
        public String RepoID { get; set; }
    }
}
