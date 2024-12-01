using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class UpdateBalance
    {
        [Required]
        public EditBalanceDTO EditBalanceDTO { get; set; }

        public String? UpdateOption { get; set; }

        public Decimal? UpdateValue { get; set; }
    }
}
