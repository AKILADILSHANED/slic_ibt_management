using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class DisplayDetailsSearchRepoDTO
    {
        public Dictionary<String, String>? AccountList { get; set; }

        [Required]
        public String RepoID { get; set; }

        [Required]
        [RegularExpression(@"^[\s\S]{1,50}$")]
        public String RepoType { get; set; }

        [Required]
        public Decimal OpeningBalance { get; set; }

        [Required]
        public Decimal ClosedBalance { get; set; }

        [Required]
        public String AccountID { get; set; }
    }
}
