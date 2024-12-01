using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLICGL_IBT_Management.Models
{
    public class Payment
    {
        [Required]
        public String PaymentID { get; set; }

        [Required]
        [RegularExpression(@"^[\w\s\S]{1,50}$")]
        public String PaymentType { get; set; }

        [Required]
        public DateTime RegisteredDate { get; set; }

        [Required]
        public String RegisteredBy { get; set; }

        [ForeignKey("RegisteredBy")]
        public virtual User IDUserRegistered { get; set; }

        [Required]
        public int IsDeleted { get; set; }

        public String? DeletedBy { get; set; }

        [ForeignKey("DeletedBy")]
        public virtual User IDUserDelete { get; set; }

    }
}
