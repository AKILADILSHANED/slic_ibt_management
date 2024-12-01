using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class AllFundRequests
    {
        [Required]
        public String RequestID { get; set; }

        [Required]
        public DateTime RequestDate { get; set; }

        [Required]
        public DateTime RequiredDate { get; set; }

        [Required]
        public Decimal RequestAmount { get; set; }

        [Required]
        public Decimal RemainingRequestAmount { get; set; }

        [Required]
        public String IsDeleted { get; set; }

        [Required]
        public String DeletedBy { get; set; }

        [Required]
        public String IsApproved { get; set; }

        [Required]
        public String RequestBy { get; set; }

        [Required]
        public String ApprovedBy { get; set; }

        [Required]
        public String RequestReason { get; set; }

        [Required]
        public String AccountNumber { get; set; }
    }
}
