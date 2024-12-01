using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class Adjustment
    {
        [Required]
        public String AjustmentID { get; set; }

        [Required]
        public DateTime AdjustDate { get; set; }

        [Required]
        public DateTime AdjustTime { get; set; }

        [Required]
        public Decimal AdjustAmount { get; set; }

        [Required]
        public String RepoID { get; set; }
    }
}
