using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace SLICGL_IBT_Management.Models
{
    public class AccountBalance
    {
        [Required]
        public String BalanceID { get; set; }

        [Required]
        public DateTime BalanceDate { get; set; }

        [Required]
        [RegularExpression(@"^-?\d+(\.\d+)?$")]
        public decimal BalanceAmount { get; set; }

        [Required]
        [RegularExpression(@"^-?\d+(\.\d+)?$")]
        public decimal OutstandingBalance { get; set; }

        [Required]
        public int IsDeleted { get; set; }

        [Required]
        public String AccountID { get; set; }

        [ForeignKey("AccountID")]
        public virtual BankAccount IDAccount { get; set; }

        [Required]
        public String User_Id { get; set; }

        [ForeignKey("User_Id")]
        public virtual User IDUser { get; set; }

        public virtual OverdraftRecoverAdjustment OverdraftRecoverAdjustment { get; set; }
    }
}
