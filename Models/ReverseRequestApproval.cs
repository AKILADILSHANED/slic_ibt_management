using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class ReverseRequestApproval
    {
        [Required]
        public String RequestID { get; set; }

        [Required]
        public String TransferID { get; set; }
    }
}
