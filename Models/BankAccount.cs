using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLICGL_IBT_Management.Models
{
    public class BankAccount
    {
        [Required]
        public String AccountID { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$")]
        public String AccountNumber { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Length of Account branch should limit to 50 characters.")]
        public String AccountBranch { get; set; }

        [Required]
        public string AccountType { get; set; }

        [Required]
        public string Currency { get; set; }

        [Required]
        [RegularExpression(@"^\d{7}$")]
        public int GLCode { get; set; }

        [Required]
        public DateTime RegisteredDate { get; set; }

        [Required]
        public int IsDeleted { get; set; }

        [Required]
        [ForeignKey("BankID")]
        public String BankID { get; set; }

        [ForeignKey("BankID")]
        public virtual Bank IDBank { get; set; }

        [Required]
        public String User_Id { get; set; }

        [ForeignKey("User_Id")]
        public virtual User Creator { get; set; }
    }
}
