using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class ConfigurationDetails
    {
        [Required]
        public String SendingAccountID { get; set; }

        [Required]
        [RegularExpression("^[0-9]+$")]
        public String SendingAccountNumber { get; set; }

        [Required]
        public Dictionary<String, String> ReceivingAccountList { get; set; }

        [Required]
        public String OptionID { get; set; }

        [Required]
        [StringLength(50)]
        public String OptionDescription { get; set; }
    }
}
