using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class AccountBalanceAdjustmentDetails
    {
        [Required]
        public AccountBalanceForAdjustments AccountBalanceForAdjustments { get; set; }

        [Required]
        public Dictionary<String, AdjustmentDetailsBalancesDTO> AllAdjustments { get; set; }
    }
}
