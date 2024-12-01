using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SLICGL_IBT_Management.Models
{
    public class UserCategoryAvailability
    {
        [Required]
        [ForeignKey("User")]
        public String User_Id { get; set; }

        [Required]
        [ForeignKey("UserCategory")]
        public String CategoryID { get; set; }

    }
}
