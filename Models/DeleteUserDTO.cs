using System.ComponentModel.DataAnnotations;

namespace SLICGL_IBT_Management.Models
{
    public class DeleteUserDTO
    {
        public String User_Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Epf { get; set; }
                
        public String Position { get; set; }

        public string Email { get; set; }

        public DateTime Create_Date_Time { get; set; }

        public String Creator { get; set; }

        public int IsDeleted { get; set; }
    }
}
