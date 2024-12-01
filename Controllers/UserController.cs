using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SLICGL_IBT_Management.DB_Connection;
using SLICGL_IBT_Management.Models;
using System.Linq;
using System.Text.RegularExpressions;

namespace SLICGL_IBT_Management.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            this._context = context;
        }

        public IActionResult UserDetailsForRegister()
        {
            return View();
        }

        public IActionResult RegisterUser(RegisterUser UserDetails)
        {
            
            if (ModelState.IsValid)
            {
                //Check whether same User Name already available in the Database.

                bool ExistingUserName = _context.Users.Where(name => name.UserName == UserDetails.UserName
                && name.IsDeleted == 0).Any();

                if (ExistingUserName == false)
                {
                    //Identify last user_id in the user table.

                    User last_user = _context.Users.OrderByDescending(User => User.Create_Date_Time).FirstOrDefault();
                    String string_user_id;

                    if (last_user == null)
                    {
                        //Creation of new user_id.

                        string_user_id = "USR/1";
                    }
                    else
                    {
                        //Creation of new user_id.

                        var regex = new Regex(@"\d+");
                        var match = regex.Match(last_user.User_Id);
                        var generate_user_id = int.Parse(match.Value) + 1;
                        string_user_id = "USR/" + generate_user_id.ToString();
                    }

                    //Create new instance of User.
                    User user = new User();

                    user.User_Id = string_user_id;
                    user.FirstName = UserDetails.FirstName;
                    user.LastName = UserDetails.LastName;
                    user.Epf = UserDetails.Epf;
                    user.Position = UserDetails.Position;
                    user.Email = UserDetails.Email;
                    user.UserName = UserDetails.UserName;
                    user.Password = UserDetails.Password;
                    user.IsDeleted = 0;
                    user.Create_Date_Time = DateTime.Now;
                    user.CreateBy = HttpContext.Session.GetString("UserID");

                    _context.Users.Add(user);
                    _context.SaveChanges();

                    /*Once the user registered successfully, system will checks which user categories the relevant user available.
                     Then, system will update the 'UserCategoryAvailability' table according to the selection in the registration
                     form. */
            
                    bool Admin = UserDetails.UserCategoryAdmin;
                    bool Initiator = UserDetails.UserCategoryInitiator;
                    bool Supervisor = UserDetails.UserCategorySupervisor;
                    bool Approver = UserDetails.UserCategoryApprover;

                    Dictionary<String, String> CategoryList = _context.UserCategory.ToDictionary(category => category.CategoryID,
                    category => category.CategoryType);

                    UserCategoryAvailability Availlability = new UserCategoryAvailability();

                    if (Admin == true)
                    {

                        Availlability.CategoryID = CategoryList.FirstOrDefault(category => category.Value == "Administrator").Key;
                        Availlability.User_Id = string_user_id;
                        _context.UserCategoryAvailability.Add(Availlability);
                        _context.SaveChanges();
                                                
                    }
                    else
                    {
                        //No code block to run.
                    }

                    if (Initiator == true)
                    {
                        Availlability.CategoryID = CategoryList.FirstOrDefault(category => category.Value == "Initiator").Key;
                        Availlability.User_Id = string_user_id;
                        _context.UserCategoryAvailability.Add(Availlability);
                        _context.SaveChanges();
                        
                    }
                    else
                    {
                        //No code block to run.
                    }

                    if (Supervisor == true)
                    {
                        Availlability.CategoryID = CategoryList.FirstOrDefault(category => category.Value == "Supervisor").Key;
                        Availlability.User_Id = string_user_id;
                        _context.UserCategoryAvailability.Add(Availlability);
                        _context.SaveChanges();
                         
                    }
                    else
                    {
                        //No code block to run.
                    }

                    if (Approver == true)
                    {
                        Availlability.CategoryID = CategoryList.FirstOrDefault(category => category.Value == "Approver").Key;
                        Availlability.User_Id = string_user_id;
                        _context.UserCategoryAvailability.Add(Availlability);
                        _context.SaveChanges();
                    }
                    else
                    {
                        //No code block to run.
                    }
                    
                    TempData["message"] = "Success: User registered successfully with User ID: " + string_user_id + "!";
                    return RedirectToAction("UserDetailsForRegister", "User");
                }
                else
                {
                    TempData["message"] = "Information: User registration failled. Same User Name available in the Database. Please try another User Name!";
                    return RedirectToAction("UserDetailsForRegister", "User");
                }
                                
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("UserDetailsForRegister", "User");
            }

        }

        //--------------------------------------------------------------------------------------------------------------------


        public IActionResult SearchUser()
        {
            return View();
        }

        public IActionResult DisplayUserDetailsSearch(String UserID)
        {
            if (ModelState.IsValid)
            {
                //Check User ID with Users table.

                String SQLQuery = "SELECT user1.FirstName, user1.LastName, user1.Epf, user1.Position, user1.Email, user1.Create_Date_Time, user1.IsDeleted, user2.FirstName as Creator\r\nFROM Users user1\r\nLEFT JOIN Users user2 on user1.CreateBy = user2.User_Id WHERE user1.User_Id = @Id";

                var DetailsUser = _context.Database.SqlQueryRaw<SearchUserDTO>(SQLQuery, new SqlParameter("@Id", UserID)).FirstOrDefault();

                //Get Assigned roles of relevant user id.

                String SqlAssignedRoles = "SELECT UserCategory.CategoryType\r\nFROM UserCategory\r\nLEFT JOIN UserCategoryAvailability ON UserCategory.CategoryID = UserCategoryAvailability.CategoryID\r\nLEFT JOIN Users ON Users.User_Id = UserCategoryAvailability.User_Id\r\nWHERE Users.User_Id = @Id";
                Dictionary<String,String> RolesList = new Dictionary<String, String>();
                List<SearchAssignedRoles> SearchAssignedRoles = _context.Database.SqlQueryRaw<SearchAssignedRoles>(SqlAssignedRoles, new SqlParameter("@Id", UserID)).ToList();

                foreach (SearchAssignedRoles ListofRoles in SearchAssignedRoles)
                {
                    RolesList.Add(ListofRoles.CategoryType, ListofRoles.CategoryType);
                }

                //Create an instance of SearchUserWithRoles model and initialize the property values.

                SearchUserWithRoles SearchUserWithRoles = new SearchUserWithRoles()
                {
                    SearchAssignedRoles = RolesList,
                    SearchUserDTO = DetailsUser
                };

                //Check if the User is available in the database.

                if (DetailsUser != null)
                {
                    //Check User is already deleted or not.

                    if (DetailsUser.IsDeleted == 0)
                    {
                                               
                        return View(SearchUserWithRoles);
                    }
                    else
                    {
                        //If User record is not available in the database.

                        TempData["message"] = "Information: This User is no longer available!";
                        return RedirectToAction("SearchUser", "User");
                    }

                }
                else
                {   
                    //If User is not available for provided User ID.

                    TempData["message"] = "Information: User not available for provided User ID!";
                    return RedirectToAction("SearchUser", "User");
                }
                
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("SearchUser", "User");
            }
            
        }

        //-----------------------------------------------------------------------------------------------------

        public IActionResult DeleteUser()
        {
            return View();
        }

        public IActionResult DisplayUserDetailsDelete(String UserID)
        {
            if (ModelState.IsValid)
            {
                //Check User ID with Users table.

                String SQLQuery = "SELECT user1.User_Id, user1.FirstName, user1.LastName, user1.Position, user1.Epf, user1.Email, user1.Create_Date_Time, user1.IsDeleted, user2.FirstName as Creator\r\nFROM Users user1\r\nLEFT JOIN Users user2 on user1.CreateBy = user2.User_Id WHERE user1.User_Id = {0}";

                var DetailsUser = _context.Database.SqlQueryRaw<DeleteUserDTO>(SQLQuery, UserID).FirstOrDefault();

                //Check if the User is available in the database.

                if (DetailsUser != null)
                {
                    //Check User is already deleted or not.

                    if (DetailsUser.IsDeleted == 0)
                    {
                        //Check user logged User ID in the session.

                        if (HttpContext.Session.GetString("UserID") != UserID.ToUpper())
                        {
                            return View(DetailsUser);
                        }
                        else
                        {
                            TempData["message"] = "Information: Unable to delete this User, since you have already singed in with this User ID. Please sign with another User ID!";
                            return RedirectToAction("DeleteUser", "User");
                        }
                                                
                    }
                    else
                    {
                        //If User record is not available in the database.

                        TempData["message"] = "Information: This User is no longer available!";
                        return RedirectToAction("DeleteUser", "User");
                    }

                }
                else
                {
                    //If User is not available for provided User ID.

                    TempData["message"] = "Information: User not available for provided User ID!";
                    return RedirectToAction("DeleteUser", "User");
                }

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("DeleteUser", "User");
            }
        }


        public IActionResult ConfirmUserDelete(DeleteUserDTO DeleteUser)
        {
            if (ModelState.IsValid)
            {
                String SqlQuery = "UPDATE Users SET IsDeleted = 1 WHERE User_Id = @ID ";
                _context.Database.ExecuteSqlRaw(SqlQuery, new SqlParameter("@ID", DeleteUser.User_Id));

                TempData["message"] = "Success: User deleted successfully!";
                return View("DeleteUser", "User");
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("DeleteUser", "User");
            }
            
        }

        //----------------------------------------------------------------------------------------------------

        public IActionResult UpdateUser()
        {
            return View();
        }

        public IActionResult DisplayUserDetailsUpdate(String UserID)
        {
            if (ModelState.IsValid)
            {
                //Check User ID with Users table.

                String SQLQuery = "SELECT user1.User_Id, user1.FirstName, user1.LastName, user1.Epf, user1.Position, user1.Email, user1.Create_Date_Time, user1.IsDeleted, user2.FirstName as Creator\r\nFROM Users user1\r\nLEFT JOIN Users user2 on user1.CreateBy = user2.User_Id WHERE user1.User_Id = @Id";

                var DetailsUser = _context.Database.SqlQueryRaw<UpdateUserDTO>(SQLQuery, new SqlParameter("@Id", UserID)).FirstOrDefault();

                //Check if the User is available in the database.

                if (DetailsUser != null)
                {
                    //Check User is already deleted or not.

                    if (DetailsUser.IsDeleted == 0)
                    {
                        return View(DetailsUser);
                    }
                    else
                    {
                        //If User record is not available in the database.

                        TempData["message"] = "Information: This User is no longer available!";
                        return RedirectToAction("UpdateUser", "User");
                    }

                }
                else
                {
                    //If User is not available for provided User ID.

                    TempData["message"] = "Information: User not available for provided User ID!";
                    return RedirectToAction("UpdateUser", "User");
                }

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("UpdateUser", "User");
            }

        }

        public IActionResult ConfirmUserUpdate(UpdateUserDTO UpdateUser)
        {
            if (ModelState.IsValid)
            {
                String UpdateUserQuery = "UPDATE Users SET FirstName = @Fname, LastName = @Lname, Epf = @Epf, Position = @Position, Email = @Email WHERE User_Id = @Id";
                _context.Database.ExecuteSqlRaw(UpdateUserQuery, 

                    new SqlParameter("@Fname", UpdateUser.FirstName),
                    new SqlParameter("@Lname", UpdateUser.LastName),
                    new SqlParameter("@Epf", UpdateUser.Epf),
                    new SqlParameter("@Email", UpdateUser.Email),
                    new SqlParameter("@Id",UpdateUser.User_Id),
                    new SqlParameter("@Position", UpdateUser.Position)
                    );
                                

                TempData["message"] = "Success: User updated successfully!";
                return View("UpdateUser", "User");
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("UpdateUser", "User");
            }

        }

        //---------------------------------------------------------------------------------------------

        public IActionResult DisplayAllUsers()
        {
            String SQLQuery = "SELECT user1.User_Id, user1.FirstName, user1.LastName, user1.Epf, user1.Position, user1.Email, user1.Create_Date_Time, user1.IsDeleted, user2.FirstName as Creator\r\nFROM Users user1\r\nLEFT JOIN Users user2 on user1.CreateBy = user2.User_Id";

            List<DisplayAllUsersDTO> DetailsUserList = _context.Database.SqlQueryRaw<DisplayAllUsersDTO>(SQLQuery).OrderBy(orderby => orderby.Create_Date_Time).ToList();

            return View(DetailsUserList);
        }
    }
}
