using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class DisplayAllTransferMethodsDTO
    {
        [Required]
        public String MethodID { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$")]
        public String SendingAccountNumber { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$")]
        public String ReceivingAccountNumber { get; set; }

        [Required]
        public String Description { get; set; }

        [Required]
        public String CreatedBy { get; set; }

        [Required]
        public String DeleteStatus { get; set; }

        [Required]
        public String DeleteBy { get; set; }

        [Required]
        public String Status { get; set; }

    }
}
