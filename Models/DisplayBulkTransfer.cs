using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class DisplayBulkTransfer
    {
        [Required]
        public String IBTSheet { get; set; }

        [Required]
        public int TotalCount { get; set; }

        [Required]
        public Decimal TotalTransferAmount { get; set; }

        [Required]
        public int NotDeletedCount { get; set; }

        [Required]
        public int DeletedCount { get; set; }

        [Required]
        public int NotApprovedCount { get; set; }

        [Required]
        public int ApprovedCount { get; set; }
    }
}
