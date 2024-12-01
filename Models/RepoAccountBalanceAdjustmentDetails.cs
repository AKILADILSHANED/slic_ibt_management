using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class RepoAccountBalanceAdjustmentDetails
    {
        [Required]
        public RepoAccountBalanceForAdjustments RepoAccountBalanceForAdjustments { get; set; }

        [Required]
        public Dictionary<String, RepoAdjustmentDetailsBalancesDTO> AllRepoAdjustments { get; set; }
    }
}
