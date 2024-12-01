using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class UpdatePaymentDTO
    {
        [Required]
        public String PaymentID { get; set; }

        [Required]
        [RegularExpression(@"^[\w\s\S]{1,50}$")]
        public String PaymentType { get; set; }

        [Required]
        public int IsDeleted { get; set; }
    }
}
