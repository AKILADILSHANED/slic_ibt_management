using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class RepoBalanceAdjustment
    {
        [Required]
        public String AdjustmentID { get; set; }

        [Required]
        public DateTime AdjustedDate { get; set; }

        [Required]
        public Decimal AdjustedAmount { get; set; }

        [Required]
        public String Description { get; set; }

        [Required]
        public String AdjustBy { get; set; }

        [ForeignKey("AdjustBy")]
        public virtual User Adjuster { get; set; }

        [Required]
        public int IsReversed { get; set; }

        public String? ReversedBy { get; set; }

        [ForeignKey("ReversedBy")]
        public virtual User ReversedUser { get; set; }

        [Required]
        public String Repo { get; set; }

        [ForeignKey("Repo")]
        public virtual Repo IDRepo { get; set; }

        public String? TransferID { get; set; }

        [ForeignKey("TransferID")]
        public Transfers Transfer { get; set; }

    }
}
