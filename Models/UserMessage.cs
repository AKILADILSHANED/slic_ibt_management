using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class UserMessage
    {
        [Required]
        public String MessageID { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
