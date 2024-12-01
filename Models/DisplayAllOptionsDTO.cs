using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class DisplayAllOptionsDTO
    {
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
        public String CreateBy { get; set; }

        [Required]
        public String Status { get; set; }

        public String? DeletedBy { get; set; }

    }
}
