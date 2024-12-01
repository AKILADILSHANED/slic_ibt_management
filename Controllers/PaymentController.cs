using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SLICGL_IBT_Management.DB_Connection;
using SLICGL_IBT_Management.Models;
using System.Text.RegularExpressions;

namespace SLICGL_IBT_Management.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            this._context = context;
        }

        public IActionResult NewPayment()
        {
            return View();
        }

        public IActionResult RegisterPayment(String PaymentType)
        {
            if (ModelState.IsValid)
            {
                //Identify last user_id in the user table and Generate new Payment ID..

                var last_payment = _context.Payment.OrderByDescending(pay => pay.RegisteredDate).FirstOrDefault();

                String string_payment_id;

                if (last_payment == null) {
                    string_payment_id = "PMNT/1";
                }
                else
                {
                    var regex = new Regex(@"\d+");
                    var match = regex.Match(last_payment.PaymentID);
                    var generate_payment_id = int.Parse(match.Value) + 1;
                    string_payment_id = "PMNT/" + generate_payment_id.ToString();
                }

                Payment Payment = new Payment()
                {
                    PaymentID = string_payment_id,
                    PaymentType = PaymentType,
                    RegisteredDate = DateTime.Now,
                    RegisteredBy = HttpContext.Session.GetString("UserID"),
                    IsDeleted = 0,
                    DeletedBy = null
                };

                _context.Payment.Add(Payment);
                _context.SaveChanges();
                TempData["message"] = "Success: Payment registered successfully with Payment ID: " + string_payment_id  + "!";
                return RedirectToAction("NewPayment", "Payment");

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("NewPayment", "Payment");
            }
            
        }

        public IActionResult UpdatePayment()
        {
            return View();
        }

        public IActionResult DisplayPaymentDetailsUpdate(String PaymentID)
        {
            if (ModelState.IsValid) 
            {
                String SqlQuery = "SELECT PaymentID, PaymentType, IsDeleted FROM Payment WHERE PaymentID = @ID";
                var PaymentDetail = _context.Database.SqlQueryRaw<UpdatePaymentDTO>(SqlQuery, new SqlParameter("@ID", PaymentID)).FirstOrDefault();
                if (PaymentDetail != null) 
                {
                    if (PaymentDetail.IsDeleted == 0)
                    {
                        return View(PaymentDetail);
                    }
                    else
                    {
                        TempData["message"] = "Information: This Payment ID is no longer available!";
                        return RedirectToAction("UpdatePayment", "Payment");
                    }
                }
                else
                {
                    TempData["message"] = "Information: No Payment Details available for provided Payment ID!";
                    return RedirectToAction("UpdatePayment", "Payment");
                }
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("UpdatePayment", "Payment");
            }
            
        }

        public IActionResult ConfirmPaymentUpdate(UpdatePaymentDTO UpdatePaymentObject)
        {
            if (ModelState.IsValid)
            {
                String SqlQuery = "UPDATE Payment SET PaymentType = @Type WHERE PaymentID = @ID ";
                _context.Database.ExecuteSqlRaw(SqlQuery,
                    new SqlParameter("@Type", UpdatePaymentObject.PaymentType),
                    new SqlParameter("@ID", UpdatePaymentObject.PaymentID)
                    );

                TempData["message"] = "Success: Payment updated successfully!";
                return RedirectToAction("UpdatePayment", "Payment");

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("UpdatePayment", "Payment");
            }
        }


        public IActionResult DeletePayment()
        {
            return View();
        }

        public IActionResult DisplayPaymentDetailsDelete(String PaymentID)
        {
            if (ModelState.IsValid)
            {
                String SqlQuery = "SELECT PaymentID, PaymentType, IsDeleted, CASE WHEN DeletedBy IS NULL THEN 'N/A' ELSE DeletedBy END AS DeletedBy FROM Payment WHERE PaymentID = @ID";
                var PaymentDetail = _context.Database.SqlQueryRaw<DeletePaymentDTO>(SqlQuery, new SqlParameter("@ID", PaymentID)).FirstOrDefault();
                if (PaymentDetail != null)
                {
                    if (PaymentDetail.IsDeleted == 0)
                    {
                        return View(PaymentDetail);
                    }
                    else
                    {
                        TempData["message"] = "Information: This Payment ID is no longer available!";
                        return RedirectToAction("UpdatePayment", "Payment");
                    }
                }
                else
                {
                    TempData["message"] = "Information: No Payment Details available for provided Payment ID!";
                    return RedirectToAction("UpdatePayment", "Payment");
                }
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("UpdatePayment", "Payment");
            }

        }

        public IActionResult ConfirmPaymentDelete(DeletePaymentDTO DeletePaymentObject)
        {
            if (ModelState.IsValid)
            {
                String SqlQuery = "UPDATE Payment SET IsDeleted = @Delete, DeletedBy = @DeleteBy WHERE PaymentID = @ID ";
                _context.Database.ExecuteSqlRaw(SqlQuery,
                    new SqlParameter("@Delete", 1),
                    new SqlParameter("@ID", DeletePaymentObject.PaymentID),
                    new SqlParameter("@DeleteBy", HttpContext.Session.GetString("UserID"))
                    );

                TempData["message"] = "Success: Payment deleted successfully!";
                return RedirectToAction("UpdatePayment", "Payment");

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("UpdatePayment", "Payment");
            }
        }

        public IActionResult DisplayAllPayment()
        {
            String SqlQuery = "SELECT Pay.PaymentID, Pay.PaymentType, Pay.RegisteredDate, Pay.IsDeleted AS Status, Usr1.FirstName AS RegisteredBy,\r\nCOALESCE(Usr2.FirstName, 'N/A') AS DeletedBy\r\nFROM Payment Pay\r\nLEFT JOIN Users Usr1 ON Pay.RegisteredBy = Usr1.User_Id\r\nLEFT JOIN Users Usr2 ON Pay.DeletedBy = Usr2.User_Id";
            List< DisplayAllPayment> PaymentList = _context.Database.SqlQueryRaw<DisplayAllPayment>(SqlQuery).ToList();

            return View(PaymentList);
        }
    }
}
