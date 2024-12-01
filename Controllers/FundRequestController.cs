using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SLICGL_IBT_Management.DB_Connection;
using SLICGL_IBT_Management.Models;
using System.Text.RegularExpressions;

namespace SLICGL_IBT_Management.Controllers
{
    public class FundRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FundRequestController(ApplicationDbContext context)
        {
            this._context = context;
        }

        public IActionResult NewRequest()
        {

            //Create new instance of NewFundRequest model.
            NewFundRequest Request = new NewFundRequest()
            {
                //Get all Payment list from the Payment table.

                PaymentList = _context.Payment.Where(pay => pay.IsDeleted == 0).ToDictionary(PayID => PayID.PaymentID.ToString(),
                PayType => PayType.PaymentType.ToString()),


                //Get all Account list from the BankAccount table.

                AccountList = _context.BankAccount.Where(acc => acc.IsDeleted == 0).ToDictionary(AccID => AccID.AccountID.ToString(),
                AccNo => AccNo.AccountNumber.ToString()),

                RequestAmount = 0
            };

            return View(Request);
        }

        public IActionResult SaveRequest(NewFundRequest RequestDetails)
        {
            if (ModelState.IsValid)
            {
                //Check whether a Fund Request has been already created for selected Payment.
                bool PaymentAvailability = _context.FundRequest.Where(req => req.PaymentID == RequestDetails.PaymentID && req.IsDeleted == 0 && req.RequiredDate.Date == DateTime.Today.Date).Any();
                if (PaymentAvailability == false)
                {
                    //Create new variable to store newly created RequestID.
                    String NewRequestID;

                    //Check whether any record is available in the FundRequest table.
                    FundRequest LastRequest = _context.FundRequest.OrderByDescending(req => req.RequestDate).FirstOrDefault();

                    //Check whether the Account Balance has been updated for current date for Fund Requested Bank Account.

                    var AccountBalanceRecord = _context.AccountBalance.Where(bal => bal.BalanceDate.Date == DateTime.Today.Date &&
                    bal.AccountID == RequestDetails.AccountID && bal.IsDeleted == 0).FirstOrDefault();

                    if (AccountBalanceRecord != null)
                    {
                        if (LastRequest == null)
                        {
                            NewRequestID = "REQ/" + DateTime.Today.Year + "/1";
                        }
                        else
                        {
                            string[] parts = LastRequest.RequestID.Split('/');
                            int lastPart = int.Parse(parts.Last());
                            int NewNumericValue;

                            //Get current Year.
                            int CurrentYear = DateTime.Now.Year;

                            if (parts[1] == CurrentYear.ToString())
                            {
                                NewNumericValue = lastPart + 1;
                                NewRequestID = "REQ/" + CurrentYear + "/" + NewNumericValue;
                            }
                            else
                            {
                                NewNumericValue = 1;
                                NewRequestID = "REQ/" + CurrentYear + "/" + NewNumericValue;
                            }
                        }

                        //Create an instance of FundRequest and initialize the values in order to save to the table.

                        FundRequest Request = new FundRequest()
                        {
                            RequestID = NewRequestID,
                            RequestDate = DateTime.Now,
                            RequiredDate = DateTime.Now.Date,
                            RequestAmount = RequestDetails.RequestAmount,
                            RemainingRequestAmount = RequestDetails.RequestAmount,
                            RequestBy = HttpContext.Session.GetString("UserID"),
                            IsDeleted = 0,
                            DeletedBy = null,
                            IsApproved = 0,
                            ApprovedBy = null,
                            PaymentID = RequestDetails.PaymentID,
                            AccountID = RequestDetails.AccountID
                        };

                        _context.FundRequest.Add(Request);
                        _context.SaveChanges();
                        TempData["message"] = "Success: Funds have been requested succesfully with Request ID: " + NewRequestID + " !";
                        return RedirectToAction("NewRequest", "FundRequest");
                    }
                    else
                    {
                        TempData["message"] = "Information: Account Balance has not been entered for selected Bank Account. Please enter the Account Balance first!";
                        return RedirectToAction("NewRequest", "FundRequest");
                    }
                }
                else
                {
                    TempData["message"] = "Information: A Request is already initiated for provided Payment Type. If you need more Fund Request, please update the existing Fund Request!";
                    return RedirectToAction("NewRequest", "FundRequest");
                }
                 
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("NewRequest", "FundRequest");
            }

        }

        public IActionResult NewForecastRequest()
        {

            //Create new instance of NewFundRequest model.
            NewForecastedRequest Request = new NewForecastedRequest()
            {
                //Get all Payment list from the Payment table.

                PaymentList = _context.Payment.Where(pay => pay.IsDeleted == 0).ToDictionary(PayID => PayID.PaymentID.ToString(),
                PayType => PayType.PaymentType.ToString()),


                //Get all Account list from the BankAccount table.

                AccountList = _context.BankAccount.Where(acc => acc.IsDeleted == 0).ToDictionary(AccID => AccID.AccountID.ToString(),
                AccNo => AccNo.AccountNumber.ToString()),

                RequestAmount = 0,

                RequiredDate = DateTime.Today,
            };

            return View(Request);
        }

        public IActionResult SaveForecastRequest(NewForecastedRequest RequestDetails)
        {
            if (ModelState.IsValid)
            {
                //Check whether the Required Date is a previouse date or not.

                if (RequestDetails.RequiredDate.Date > DateTime.Today.Date)
                {
                    //Create new variable to store newly created RequestID.
                    String NewRequestID;

                    //Check whether any record is available in the FundRequest table.
                    FundRequest LastRequest = _context.FundRequest.OrderByDescending(req => req.RequestDate).FirstOrDefault();

                    if (LastRequest == null)
                    {
                        NewRequestID = "REQ/" + DateTime.Today.Year + "/1";
                    }
                    else
                    {
                        string[] parts = LastRequest.RequestID.Split('/');
                        int lastPart = int.Parse(parts.Last());
                        int NewNumericValue;

                        //Get current Year.
                        int CurrentYear = DateTime.Now.Year;

                        if (parts[1] == CurrentYear.ToString())
                        {
                            NewNumericValue = lastPart + 1;
                            NewRequestID = "REQ/" + CurrentYear + "/" + NewNumericValue;
                        }
                        else
                        {
                            NewNumericValue = 1;
                            NewRequestID = "REQ/" + CurrentYear + "/" + NewNumericValue;
                        }
                    }

                    //Create an instance of FundRequest and initialize the values in order to save to the table.

                    FundRequest Request = new FundRequest()
                    {
                        RequestID = NewRequestID,
                        RequestDate = DateTime.Now,
                        RequiredDate = RequestDetails.RequiredDate.Date,
                        RequestAmount = RequestDetails.RequestAmount,
                        RemainingRequestAmount = RequestDetails.RequestAmount,
                        RequestBy = HttpContext.Session.GetString("UserID"),
                        IsDeleted = 0,
                        DeletedBy = null,
                        IsApproved = 0,
                        ApprovedBy = null,
                        PaymentID = RequestDetails.PaymentID,
                        AccountID = RequestDetails.AccountID
                    };

                    _context.FundRequest.Add(Request);
                    _context.SaveChanges();

                    TempData["message"] = "Success: Funds have been requested succesfully with Request ID: " + NewRequestID + " !";
                    return RedirectToAction("NewForecastRequest", "FundRequest");

                }
                else
                {
                    TempData["message"] = "Information: Selected Required Date not a future date. Please select a future date as a forecasted date!";
                    return RedirectToAction("NewForecastRequest", "FundRequest");
                }

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("NewForecastRequest", "FundRequest");
            }

        }

        public IActionResult SearchRequest()
        {
            return View();
        }

        public IActionResult DisplayRequestDetailsSearch(String RequestID)
        {
            if (ModelState.IsValid)
            {
                String SqlQuery = "SELECT Req.RequestID AS RequestID, \r\nReq.RequestDate AS RequestDate,\r\nReq.RequiredDate AS RequiredDate, \r\nReq.IsDeleted AS IsDeleted, \r\nReq.RequestAmount AS RequestAmount, \r\nReq.RemainingRequestAmount AS RemainingRequestAmount, \r\nCASE WHEN IsApproved = '0' THEN 'Not-Approved'\r\nELSE 'Approved'\r\nEND AS IsApproved,\r\nUsr1.FirstName AS RequestBy,\r\nCASE WHEN ApprovedBy IS NULL THEN 'N/A'\r\nELSE Usr2.FirstName\r\nEND AS ApprovedBy,\r\nPmnt.PaymentType AS RequestReason,\r\nAcc.AccountNumber As AccountNumber\r\nFROM FundRequest Req LEFT JOIN Users Usr1 ON Req.RequestBy = Usr1.User_Id\r\nLEFT JOIN Users Usr2 ON Req.ApprovedBy = Usr2.User_Id\r\nLEFT JOIN Payment Pmnt ON Req.PaymentID = Pmnt.PaymentID\r\nLEFT JOIN BankAccount Acc ON Req.AccountID = Acc.AccountID\r\nWHERE Req.RequestID = @ReqID ";
                var DetailObject = _context.Database.SqlQueryRaw<FundRequestDetailsDTO>(SqlQuery, new SqlParameter("@ReqID", RequestID)).FirstOrDefault();

                //Check whether the record is available for provided Request ID.
                if (DetailObject != null)
                {
                    //Check whether the record is already deleted or not.
                    if (DetailObject.IsDeleted == 0)
                    {
                            return View(DetailObject);                        
                    }
                    else
                    {
                        TempData["message"] = "Information: This record is no longer available!";
                        return RedirectToAction("SearchRequest", "FundRequest");
                    }
                }
                else
                {
                    TempData["message"] = "Information: No record found for provided Request ID!";
                    return RedirectToAction("SearchRequest", "FundRequest");

                }

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("SearchRequest", "FundRequest");
            }

        }

        public IActionResult UpdateRequest()
        {
            return View();
        }

        public IActionResult DisplayRequestDetailsUpdate(String RequestID)
        {
            if (ModelState.IsValid)
            {
                String SqlQuery = "SELECT Req.RequestID AS RequestID, \r\nReq.RequestDate AS RequestDate,\r\nReq.RequiredDate AS RequiredDate, \r\nReq.RequestAmount AS RequestAmount, \r\nReq.RemainingRequestAmount AS RemainingRequestAmount,  \r\nReq.IsDeleted AS IsDeleted, \r\nCASE WHEN IsApproved = '0' THEN 'Not-Approved'\r\nELSE 'Approved'\r\nEND AS IsApproved,\r\nUsr1.FirstName AS RequestBy,\r\nCASE WHEN ApprovedBy IS NULL THEN 'N/A'\r\nELSE Usr2.FirstName\r\nEND AS ApprovedBy,\r\nPmnt.PaymentType AS RequestReason,\r\nAcc.AccountNumber As AccountNumber\r\nFROM FundRequest Req LEFT JOIN Users Usr1 ON Req.RequestBy = Usr1.User_Id\r\nLEFT JOIN Users Usr2 ON Req.ApprovedBy = Usr2.User_Id\r\nLEFT JOIN Payment Pmnt ON Req.PaymentID = Pmnt.PaymentID\r\nLEFT JOIN BankAccount Acc ON Req.AccountID = Acc.AccountID\r\nWHERE Req.RequestID = @ReqID ";
                var DetailObject = _context.Database.SqlQueryRaw<FundRequestDetailsUpdateDTO>(SqlQuery, new SqlParameter("@ReqID", RequestID)).FirstOrDefault();

                //Check whether the record is available for provided Request ID.
                if (DetailObject != null)
                {
                    //Check whether the record is already deleted or not.
                    if (DetailObject.IsDeleted == 0)
                    {
                        //Check whether the record is already approved.
                        if (DetailObject.IsApproved == "Not-Approved")
                        {
                            return View(DetailObject);
                        }
                        else
                        {
                            TempData["message"] = "Information: This record is already approved. You have no authority to update!";
                            return RedirectToAction("UpdateRequest", "FundRequest");
                        }
                        
                    }
                    else
                    {
                        TempData["message"] = "Information: This record is no longer available!";
                        return RedirectToAction("UpdateRequest", "FundRequest");
                    }
                }
                else
                {
                    TempData["message"] = "Information: No record found for provided Request ID!";
                    return RedirectToAction("UpdateRequest", "FundRequest");
                }


            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("UpdateRequest", "FundRequest");
            }

        }

        public IActionResult SaveUpdateFundRequest(String RequestID, String Option, Decimal UpdateValue, DateTime RequiredDate, String RequestAmount, String RemainingAmount)
        {
            if (ModelState.IsValid)
            {

                //Check whether the Funds Required Date is a previouse date or not.
                if (RequiredDate.Date >= DateTime.Today.Date)
                {
                    switch (Option)
                    {
                        case "+":
                            String SqlqueryIncrease = "\r\nUPDATE FundRequest SET RequestAmount = @ReqAmnt, RemainingRequestAmount = @RemainingAmount WHERE RequestID = @ReqID";
                            _context.Database.ExecuteSqlRaw(SqlqueryIncrease, new SqlParameter("@ReqAmnt", Decimal.Parse(RequestAmount) + UpdateValue),
                                new SqlParameter("@RemainingAmount", Decimal.Parse(RemainingAmount) + UpdateValue),
                                new SqlParameter("@ReqID", RequestID));
                            break;

                        case "-":
                            String SqlqueryDecrease = "\r\nUPDATE FundRequest SET RequestAmount = @ReqAmnt, RemainingRequestAmount = @RemainingAmount WHERE RequestID = @ReqID";
                            _context.Database.ExecuteSqlRaw(SqlqueryDecrease, new SqlParameter("@ReqAmnt", Decimal.Parse(RequestAmount) - UpdateValue),
                                new SqlParameter("@RemainingAmount", Decimal.Parse(RemainingAmount) - UpdateValue),
                                new SqlParameter("@ReqID", RequestID));
                            break;
                    }
                    
                    TempData["message"] = "Successfully: Fund Request updated successfully!";
                    return RedirectToAction("UpdateRequest", "FundRequest");
                }
                else
                {
                    TempData["message"] = "Information: You have no authority to update previouse day Fund Request!";
                    return RedirectToAction("UpdateRequest", "FundRequest");
                }

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("UpdateRequest", "FundRequest");
            }

        }

        public IActionResult DeleteRequest()
        {
            return View();
        }

        public IActionResult DisplayRequestDetailsDelete(String RequestID)
        {
            if (ModelState.IsValid)
            {

                String SqlQuery = "SELECT Req.RequestID AS RequestID, \r\nReq.RequestDate AS RequestDate,\r\nReq.RequiredDate AS RequiredDate, \r\nReq.RequestAmount AS RequestAmount, \r\nReq.RemainingRequestAmount AS RemainingRequestAmount,\r\nReq.IsDeleted AS IsDeleted, \r\nCASE WHEN IsApproved = '0' THEN 'Not-Approved'\r\nELSE 'Approved'\r\nEND AS IsApproved,\r\nUsr1.FirstName AS RequestBy,\r\nCASE WHEN ApprovedBy IS NULL THEN 'N/A'\r\nELSE Usr2.FirstName\r\nEND AS ApprovedBy,\r\nPmnt.PaymentType AS RequestReason,\r\nAcc.AccountNumber As AccountNumber\r\nFROM FundRequest Req LEFT JOIN Users Usr1 ON Req.RequestBy = Usr1.User_Id\r\nLEFT JOIN Users Usr2 ON Req.ApprovedBy = Usr2.User_Id\r\nLEFT JOIN Payment Pmnt ON Req.PaymentID = Pmnt.PaymentID\r\nLEFT JOIN BankAccount Acc ON Req.AccountID = Acc.AccountID\r\nWHERE Req.RequestID = @ReqID ";
                var DetailObject = _context.Database.SqlQueryRaw<FundRequestDetailsDeleteDTO>(SqlQuery, new SqlParameter("@ReqID", RequestID)).FirstOrDefault();

                //Check whether the record is available for provided Request ID.
                if (DetailObject != null)
                {

                    //Check whether the record is already deleted or not.
                    if (DetailObject.IsDeleted == 0)
                    {
                        //Check whether record is already approved.
                        if (DetailObject.IsApproved == "Not-Approved")
                        {
                            return View(DetailObject);
                        }
                        else
                        {
                            TempData["message"] = "Information: This record is already approved. You have no authority for delete!";
                            return RedirectToAction("DeleteRequest", "FundRequest");
                        }
                        
                    }
                    else
                    {
                        TempData["message"] = "Information: This record is no longer available!";
                        return RedirectToAction("DeleteRequest", "FundRequest");
                    }
                }
                else
                {
                    TempData["message"] = "Information: No record found for provided Request ID!";
                    return RedirectToAction("DeleteRequest", "FundRequest");
                }

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("DeleteRequest", "FundRequest");
            }

        }

        public IActionResult SaveDeleteFundRequest(FundRequestDetailsDeleteDTO DeleteRequest)
        {
            if (ModelState.IsValid)
            {
                //Check whether the record is already deleted or not.
                if (DeleteRequest.IsDeleted == 0)
                {
                    //Check whether the Funds Required Date is a previouse date or not.

                    if (DeleteRequest.RequiredDate.Date >= DateTime.Today.Date)
                    {

                        String Sqlquery = "UPDATE FundRequest SET IsDeleted = 1, DeletedBy = @DeleteBy WHERE RequestID = @ReqId ";
                        _context.Database.ExecuteSqlRaw(Sqlquery, new SqlParameter("@DeleteBy", HttpContext.Session.GetString("UserID").ToString()),
                            new SqlParameter("@ReqId", DeleteRequest.RequestID));


                        TempData["message"] = "Success: Fund Request deleted successfully!";
                        return RedirectToAction("DeleteRequest", "FundRequest");
                    }
                    else
                    {
                        TempData["message"] = "Information: You have no authority to update previouse day Fund Request!";
                        return RedirectToAction("DeleteRequest", "FundRequest");
                    }

                }
                else
                {
                    TempData["message"] = "Information: This record is no longer available!";
                    return RedirectToAction("DeleteRequest", "FundRequest");
                }

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("DeleteRequest", "FundRequest");
            }
        }

        public IActionResult DisplayAllRequests()
        {
            return View();
        }

        public IActionResult DisplayFundRequestList(DateOnly RequiredDate)
        {
            if (ModelState.IsValid)
            {
                String Sqlquery = "SELECT Req.RequestID AS RequestID, \r\nReq.RequestDate AS RequestDate,\r\nReq.RequiredDate AS RequiredDate, \r\nReq.RequestAmount AS RequestAmount, \r\nReq.RemainingRequestAmount AS RemainingRequestAmount, \r\nUsr1.FirstName AS RequestBy,\r\nCASE WHEN Req.IsApproved = 0 THEN 'Not-Approved'\r\nELSE 'Approved'\r\nEND AS IsApproved,\r\nCASE WHEN Req.ApprovedBy IS NULL THEN 'N/A'\r\nELSE Usr2.FirstName\r\nEND AS ApprovedBy,\r\nCASE WHEN Req.IsDeleted = 1 THEN 'Deleted'\r\nELSE 'Active'\r\nEND AS IsDeleted,\r\nCASE WHEN Req.DeletedBy IS NULL THEN 'N/A'\r\nELSE Usr3.FirstName\r\nEND AS DeletedBy,\r\nPmnt.PaymentType AS RequestReason,\r\nAcc.AccountNumber As AccountNumber\r\nFROM FundRequest Req LEFT JOIN Users Usr1 ON Req.RequestBy = Usr1.User_Id\r\nLEFT JOIN Users Usr2 ON Req.ApprovedBy = Usr2.User_Id\r\nLEFT JOIN Users Usr3 ON Req.DeletedBy = Usr3.User_Id\r\nLEFT JOIN Payment Pmnt ON Req.PaymentID = Pmnt.PaymentID\r\nLEFT JOIN BankAccount Acc ON Req.AccountID = Acc.AccountID WHERE RequiredDate = @ReqDate";
                List<AllFundRequests> RequestList = _context.Database.SqlQueryRaw<AllFundRequests>(Sqlquery,
                    new SqlParameter("@ReqDate", RequiredDate)).ToList();

                if (RequestList.Count > 0)
                {
                    return View(RequestList);
                }
                else
                {
                    TempData["message"] = "Information: No Fund Requests available for provided date!";
                    return RedirectToAction("DisplayAllRequests", "FundRequest");
                }
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("DeleteRequest", "FundRequest");
            }
            
        }

        public IActionResult ApproveRequests()
        {
            String Sqlquery = "SELECT Req.RequestID, Req.RequestDate, Req.RequiredDate, Req.RequestAmount, Acc.AccountNumber, Pmnt.PaymentType, Usr.FirstName\r\nFROM FundRequest Req \r\nLEFT JOIN BankAccount Acc ON Req.AccountID = Acc.AccountID\r\nLEFT JOIN Payment Pmnt ON Req.PaymentID = Pmnt.PaymentID\r\nLEFT JOIN Users Usr ON Req.RequestBy = Usr.User_Id WHERE Req.IsDeleted = 0 AND Req.IsApproved = 0 AND Req.RequiredDate = @Date ";

            List<ApproveRequests> RequestList = _context.Database.SqlQueryRaw<ApproveRequests>(Sqlquery, new SqlParameter("@Date", DateTime.Today.Date)).ToList();

                return View(RequestList);
                                   
        }

        public IActionResult SaveApproval(ApproveRequests ApproveObject)
        {
            
            if (ModelState.IsValid)
            {
                String SessionUserID = HttpContext.Session.GetString("UserID").ToString();

                String Sqlquery = "UPDATE FundRequest SET IsApproved = 1, ApprovedBy = @Approver WHERE RequestID = @Id ";
                _context.Database.ExecuteSqlRaw(Sqlquery, new SqlParameter("@Approver", SessionUserID),
                    new SqlParameter("@Id", ApproveObject.RequestID));
                                
                TempData["message"] = "Success: Fund Request approved successfully!";
                return RedirectToAction("ApproveRequests", "FundRequest");

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("ApproveRequests", "FundRequest");
            }
        }


        public IActionResult ApproveRequestsReverse()
        {
            String Sqlquery = "SELECT Req.RequestID, Req.RequestDate, Req.RequiredDate, Req.RequestAmount, Acc.AccountNumber, Pmnt.PaymentType, Usr.FirstName\r\nFROM FundRequest Req \r\nLEFT JOIN BankAccount Acc ON Req.AccountID = Acc.AccountID\r\nLEFT JOIN Payment Pmnt ON Req.PaymentID = Pmnt.PaymentID\r\nLEFT JOIN Users Usr ON Req.RequestBy = Usr.User_Id WHERE Req.IsDeleted = 0 AND Req.IsApproved = 1 AND Req.RequiredDate = @Date ";

            List<ApproveRequests> RequestList = _context.Database.SqlQueryRaw<ApproveRequests>(Sqlquery, new SqlParameter("@Date", DateTime.Today.Date)).ToList();

            return View(RequestList);

        }

        public IActionResult SaveApprovalReversal(ApproveRequests ApproveObject)
        {

            if (ModelState.IsValid)
            {
                //Check whether any Transfer has been already initiated for the provided Fund Request under the Fund Required Date.
                String SqlqueryRequst = "SELECT Req.RequestID,Tfr.TransferID\r\nFROM FundRequest Req\r\nLEFT JOIN Transfers Tfr ON Req.PaymentID = Tfr.Payment\r\nWHERE CAST(Req.RequiredDate AS date) = CAST(Tfr.TransferDate AS date) AND Tfr.IsDeleted = 0 AND Req.RequestID = @RequestID ";
                ReverseRequestApproval? ReverseObj = _context.Database.SqlQueryRaw<ReverseRequestApproval>(SqlqueryRequst, new SqlParameter("@RequestID", ApproveObject.RequestID)).FirstOrDefault();

                if (ReverseObj == null)
                {
                    String Sqlquery = "UPDATE FundRequest SET IsApproved = 0, ApprovedBy = null WHERE RequestID = @Id ";
                    _context.Database.ExecuteSqlRaw(Sqlquery, new SqlParameter("@Id", ApproveObject.RequestID));

                    TempData["message"] = "Success: Fund Request approval reversed successfully!";
                    return RedirectToAction("ApproveRequestsReverse", "FundRequest");
                }
                else
                {
                    TempData["message"] = "Information: Transfers have been already initiated for this Fund Request. No authority to reverse!";
                    return RedirectToAction("ApproveRequestsReverse", "FundRequest");
                }
                                
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("ApproveRequestsReverse", "FundRequest");
            }
        }
    }
}
