using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class DisplayAllPayment
    {
        [Required]
        public String PaymentID { get; set; }

        [Required]
        [RegularExpression(@"^[\w\s\S]{1,50}$")]
        public String PaymentType { get; set; }

        [Required]
        public DateTime RegisteredDate { get; set; }

        [Required]
        [ForeignKey("User_Id")]
        public String RegisteredBy { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        public String? DeletedBy { get; set; }
    }
}
