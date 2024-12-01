using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLICGL_IBT_Management.Models
{
    public class IBTSheet
    {
        [Required]
        public String SheetID { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public String CreateBy { get; set; }

        [ForeignKey("CreateBy")]
        public virtual User Creator { get; set; }

    }
}
