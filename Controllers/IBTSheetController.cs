using Microsoft.AspNetCore.Mvc;
using SLICGL_IBT_Management.DB_Connection;
using SLICGL_IBT_Management.Models;
using System.Text.RegularExpressions;

namespace SLICGL_IBT_Management.Controllers
{
    public class IBTSheetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IBTSheetController(ApplicationDbContext context)
        {
            this._context = context;
        }

        public IActionResult DisplaySheet(IBTSheet SheetDetails)
        {
            
            return View(SheetDetails);
        }

        public IActionResult GenerateIBTSheet()
        {
            bool SheetAvailablity = _context.IBTSheet.Where(sht => sht.CreatedDate.Date == DateTime.Today.Date).Any();
            if (SheetAvailablity == false)
            {
                //Generate new Sheet ID.
                IBTSheet LastSheet = _context.IBTSheet.OrderByDescending(date => date.CreatedDate).FirstOrDefault();

                String NewSheetID;

                if (LastSheet == null)
                {
                    NewSheetID = "SHT" + "/" + DateTime.Now.Year + "/" + 1;
                }
                else
                {
                    //Creation of new BalanceID.
                    string[] parts = LastSheet.SheetID.Split('/');
                    int lastPart = int.Parse(parts.Last());
                    int NewNumericValue;
                    NewSheetID = null;
                    //Get current Year.
                    int CurrentYear = DateTime.Now.Year;
                    if (parts[1] == CurrentYear.ToString())
                    {
                        NewNumericValue = lastPart + 1;
                        NewSheetID = "SHT/" + CurrentYear + "/" + NewNumericValue;
                    }
                    else
                    {
                        NewNumericValue = 1;
                        NewSheetID = "SHT/" + CurrentYear + "/" + NewNumericValue;
                    }
                }

                //Create new instance of IBT Sheet.
                IBTSheet Sheet = new IBTSheet()
                {
                    SheetID = NewSheetID,
                    CreatedDate = DateTime.Now,
                    CreateBy = HttpContext.Session.GetString("UserID").ToString()
                };

                _context.IBTSheet.Add(Sheet);
                _context.SaveChanges();

                //Get IBT Sheet.
                IBTSheet? SheetDetails = _context.IBTSheet.Where(sht => sht.CreatedDate.Date == DateTime.Today.Date).FirstOrDefault();
                return RedirectToAction("DisplaySheet", "IBTSheet", SheetDetails);
            }
            else
            {
                //Get IBT Sheet.
                IBTSheet? SheetDetails = _context.IBTSheet.Where(sht => sht.CreatedDate.Date == DateTime.Today.Date).FirstOrDefault();
                return RedirectToAction("DisplaySheet", "IBTSheet", SheetDetails);
            }
            
        }
    }
}
