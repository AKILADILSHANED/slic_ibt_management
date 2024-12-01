using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class CreateRepo
    {
        public Dictionary<String, String>? AccountList { get; set; }

        [Required]
        [RegularExpression(@"^[\s\S]{1,50}$")]
        public String RepoType { get; set; }

        [Required]
        public Decimal OpeningBalance { get; set; }

        [Required]
        public String AccountID { get; set; }
    }
}
