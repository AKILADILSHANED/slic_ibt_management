using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class DisplayAllRepoDetailsDTO
    {
        [Required]
        public String RepoID { get; set; }

        [Required]
        public String RepoType { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public Decimal OpeningBalance { get; set; }

        [Required]
        public Decimal ClosedBalance { get; set; }

        [Required]
        public String Status { get; set; }

        
        public String? Deleter { get; set; }

        [Required]
        public String AccountNumber { get; set; }

        [Required]
        public String Creator { get; set; }
    }
}
