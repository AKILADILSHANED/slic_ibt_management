using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLICGL_IBT_Management.Models
{
    public class Repo
    {
        [Required]
        public String RepoID { get; set; }

        [Required]
        [RegularExpression(@"^[\s\S]{1,50}$")]
        public String RepoType { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public Decimal OpeningBalance { get; set; }

        [Required]
        public Decimal ClosedBalance { get; set; }

        [Required]
        public int IsDeleted { get; set; }

        public String? DeleteBy { get; set; }

        [ForeignKey("DeleteBy")]
        public virtual User Deleter { get; set; }

        [Required]
        public String AccountID { get; set; }

        [ForeignKey("AccountID")]
        public virtual BankAccount Account { get; set; }

        [Required]
        public String CreateBy { get; set; }

        [ForeignKey("CreateBy")]
        public virtual User Creator { get; set; }

    }
}
