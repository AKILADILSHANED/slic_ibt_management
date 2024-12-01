using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SLICGL_IBT_Management.DB_Connection;
using SLICGL_IBT_Management.Models;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SLICGL_IBT_Management.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashBoardController(ApplicationDbContext context)
        {
            this._context = context;
        }


        public IActionResult HomePage(UserLogin LoginDetails)
        {
            if (ModelState.IsValid)
            {   
                //Check login user available in the database.
                var AuthenticatedUser = _context.Users.Where(log => log.UserName == LoginDetails.UserName &&
                log.Password == LoginDetails.Password && log.IsDeleted == 0).Select(log => new { log.UserName, log.Password, log.User_Id,
                log.FirstName, log.LastName}).FirstOrDefault();

                if (AuthenticatedUser != null)
                {   
                    //Keep User EPF in the current session.
                    HttpContext.Session.SetString("UserID", AuthenticatedUser.User_Id.ToString());

                    TempData["message"] = "Welcome " + AuthenticatedUser.FirstName + " " + AuthenticatedUser.LastName + ".";
                    return View();
                }
                else
                {
                    TempData["message"] = "Incorrect User Name or Password. Please Re-Check.";
                    return RedirectToAction("Index", "Home");
                }
                
            }
            else
            {
                TempData["message"] = "Not Valid";
                return RedirectToAction("Index", "Home");
            }
            
        }
    }
}
