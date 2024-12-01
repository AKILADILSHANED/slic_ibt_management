using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace SLICGL_IBT_Management.Models
{
    public class UserCategory
    {
        [Required]
        [Key]
        public String CategoryID { get; set; }

        [Required]
        public String CategoryType { get; set; }
    }
}
