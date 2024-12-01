namespace SLICGL_IBT_Management.Models
{
    public class SearchUserWithRoles
    {
        public SearchUserDTO SearchUserDTO { get; set; }
        public Dictionary<String,String> SearchAssignedRoles { get; set; }
    }
}
