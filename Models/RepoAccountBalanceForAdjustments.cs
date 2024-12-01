using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class RepoAccountBalanceForAdjustments
    {
        [Required]
        public String RepoID { get; set; }

        [Required]
        public Decimal OpeningBalance { get; set; }

        [Required]
        public Decimal ClosedBalance { get; set; }

        [Required]
        public String AccountNumber { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public int IsDeleted { get; set; }
    }
}
