using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class TransferRepoFunds
    {
        [Required]
        public String RepoID { get; set; }

        [Required]
        public Decimal ClosedBalance { get; set; }

        [Required]
        public String AccountID { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$")]
        public String RepoAccountNumber { get; set; }
    }
}
