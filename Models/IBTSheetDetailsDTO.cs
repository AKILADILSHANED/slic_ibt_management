using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class IBTSheetDetailsDTO
    {
        [Required]
        public String SheetID { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [Required]
        public String CreateBy { get; set; }

    }
}
