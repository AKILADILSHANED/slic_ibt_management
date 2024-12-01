using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class ApproveRequests
    {
        [Required]
        public String RequestID { get; set; }

        [Required]
        public DateTime RequestDate { get; set; }

        [Required]
        public DateTime RequiredDate { get; set; }

        [Required]
        [RegularExpression(@"^(0|[1-9]\d*)(\.\d+)?$")]
        public Decimal RequestAmount { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$")]
        public String AccountNumber { get; set; }

        [Required]
        [RegularExpression(@"^[\w\s\S]{1,50}$")]
        public String PaymentType { get; set; }

        [Required(ErrorMessage = "Required field.")]
        [StringLength(50, ErrorMessage = "Length of First name should limit to 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "Special characters and spaces are not allowed.")]
        public String FirstName { get; set; }
    }
}
