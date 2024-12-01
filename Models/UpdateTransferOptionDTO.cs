using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class UpdateTransferOptionDTO
    {
        [Required]
        public String OptionID { get; set; }

        [Required]
        [StringLength(20)]
        public string OptionType { get; set; }

        [Required]
        [StringLength(20)]
        public String OptionName { get; set; }

        [Required]
        [StringLength(50)]
        public String OptionDescription { get; set; }

        [Required]
        public int Priority { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public String CreatedBy { get; set; }

        [Required]
        public int IsDeleted { get; set; }
    }
}
