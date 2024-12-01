using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class DeletePaymentDTO
    {
        [Required]
        public String PaymentID { get; set; }

        [Required]
        [RegularExpression(@"^[\w\s\S]{1,50}$")]
        public String PaymentType { get; set; }

        [Required]
        public int IsDeleted { get; set; }

        public String? DeletedBy { get; set; }
    }
}
