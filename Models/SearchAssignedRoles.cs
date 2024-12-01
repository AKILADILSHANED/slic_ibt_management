using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class SearchAssignedRoles
    {
        [Required]
        public String CategoryType { get; set; }
    }
}
