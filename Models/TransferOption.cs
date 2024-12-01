using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLICGL_IBT_Management.Models
{
    public class TransferOption
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
        public String CreateBy { get; set; }

        [ForeignKey("CreateBy")]
        public virtual User Creator { get; set; }

        [Required]
        public int IsDeleted { get; set; }
                
        public String? DeletedBy { get; set; }

        [ForeignKey("DeletedBy")]
        public virtual User Deleter { get; set; }
    }
}
