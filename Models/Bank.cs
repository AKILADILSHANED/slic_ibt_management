using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class Bank
    {
        [Required]
        public String BankID { get; set; }

        [Required]
        public String BankName { get; set; }
    }
}
