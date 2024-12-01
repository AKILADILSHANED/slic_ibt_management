using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class NewTransferOption
    {
        
        [Required]
        [StringLength(20)]
        public string OptionType { get; set; }

        [Required]
        [StringLength(20)]
        public String OptionName { get; set; }

        [Required]
        public int Priority { get; set; }

        [Required]
        [StringLength(50)]
        public String OptionDescription { get; set; }


    }
}
