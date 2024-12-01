using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class BankAccountForTransferMethod
    {
        [Required]
        public String AccountID { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$")]
        public String AccountNumber { get; set; }
    }
}
