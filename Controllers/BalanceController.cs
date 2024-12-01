using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SLICGL_IBT_Management.DB_Connection;
using SLICGL_IBT_Management.Models;
using System.Text.RegularExpressions;

namespace SLICGL_IBT_Management.Controllers
{
    public class BalanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BalanceController(ApplicationDbContext context)
        {
            this._context = context;
        }

        public IActionResult BalanceEnter()
        {
            //Get All bank list and passes to the relevant view file.

            String SQLQuery = "SELECT BankID, BankName FROM Bank";
            Dictionary<String, String> BankList = new Dictionary<String, String>();
            BankList = _context.Database.SqlQueryRaw<Bank>(SQLQuery).ToDictionary(bank => bank.BankID.ToString(),
                name => name.BankName.ToString());

            return View(BankList);
        }

        public IActionResult AccountBalanceEnter(String BankID) 
        {
            if (ModelState.IsValid)
            {
                Dictionary<String, BalanceEnterAccounts> AccountList = new Dictionary<String, BalanceEnterAccounts>();

                String SqlQuery = "SELECT Acc.AccountID, Acc.AccountNumber, Acc.AccountType, Acc.Currency, Bnk.BankName\r\nFROM BankAccount Acc LEFT JOIN Bank Bnk ON Acc.BankID = Bnk.BankID WHERE Bnk.BankID = @BankID AND Acc.IsDeleted = 0 ";
                List<BalanceEnterAccounts> UpdateAccountList = _context.Database.SqlQueryRaw<BalanceEnterAccounts>(SqlQuery, new SqlParameter("@BankID", BankID)).ToList();

                if (UpdateAccountList.Count > 0)
                {
                    //Loop each Bank accounts and check whether any account balance available in the Balance table and
                    //if any balance record is available in the table it will be ignored and remaining bank accounts will be
                    //paased to the view file.
                    foreach (var BalanceObject in UpdateAccountList)
                    {
                        bool BalanceAvailability = _context.AccountBalance.Where(Bal => Bal.BalanceDate.Date == DateTime.Today.Date &&
                        Bal.AccountID == BalanceObject.AccountID && Bal.IsDeleted == 0).Any();

                        if (BalanceAvailability == false)
                        {
                            //If no balance record available in the current date for the perticular account number,
                            //it will be added to the AccountList dictionary and passed to the relevant view file.
                            AccountList.Add(BalanceObject.AccountID.ToString(), BalanceObject);

                        }
                        else
                        {
                            //No code block to run, if balance record is available for current date.
                        }
                    }

                    return View(AccountList);
                }
                else
                {
                    TempData["message"] = "Information: No Registered Bank Accounts available for selected Bank!";
                    return RedirectToAction("BalanceEnter", "Balance");
                }

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("BalanceEnter", "Balance");
            }
        }

        public IActionResult SaveBalance(Decimal Balance, String AccountID)
        {
            if (ModelState.IsValid)
            {
                //Generate new Balance ID.
                AccountBalance LastBalanceObj = _context.AccountBalance.OrderByDescending(date => date.BalanceDate).FirstOrDefault();
                
                String NewBalanceID;

                if (LastBalanceObj == null)
                {
                    NewBalanceID = "BAL" + "/" + DateTime.Now.Year  + "/" + 1;
                }
                else
                {
                    //Creation of new BalanceID.
                    string[] parts = LastBalanceObj.BalanceID.Split('/');
                    int lastPart = int.Parse(parts.Last());
                    int NewNumericValue;
                    NewBalanceID = null;
                    //Get current Year.
                    int CurrentYear = DateTime.Now.Year;
                    if (parts[1] == CurrentYear.ToString())
                    {
                        NewNumericValue = lastPart + 1;
                        NewBalanceID = "BAL/" + CurrentYear + "/" + NewNumericValue;
                    }
                    else
                    {
                        NewNumericValue = 1;
                        NewBalanceID = "BAL/" + CurrentYear + "/" + NewNumericValue;
                    }
                }

                //Create an instance of AccountBalance and initiaize the values to insert to the AccountBalance table.

                AccountBalance BalanceObject = new AccountBalance()
                {
                    BalanceID = NewBalanceID,
                    BalanceDate = DateTime.Now,
                    BalanceAmount = Balance,
                    OutstandingBalance = Balance,
                    IsDeleted = 0,
                    AccountID = AccountID,
                    User_Id = HttpContext.Session.GetString("UserID")
                };

                _context.AccountBalance.Add(BalanceObject);
                _context.SaveChanges();

                TempData["message"] = "Success: Account Balance saved successfully with Balance ID: " + NewBalanceID + "!";
                return RedirectToAction("BalanceEnter", "Balance");
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("BalanceEnter", "Balance");
            }
            
        }

        //------------------------------------------------------------------------------------------------

        public IActionResult EditBalance()
        {
            return View();
        }

        public IActionResult DisplayEditBalance(String BalanceID)
        {
            if (ModelState.IsValid)
            {
                String SqlQuery = "SELECT Bal.BalanceID, Bal.BalanceDate, Bal.BalanceAmount, Bal.IsDeleted, Bnk.BankName,Acc.AccountNumber\r\nFROM AccountBalance Bal\r\nLEFT JOIN BankAccount Acc ON Bal.AccountID = Acc.AccountID\r\nLEFT JOIN Bank Bnk ON Bnk.BankID = Acc.BankID WHERE BalanceID = @BalanceID  ";
                var BalanceObject = _context.Database.SqlQueryRaw<EditBalanceDTO>(SqlQuery, new SqlParameter("@BalanceID", BalanceID)).FirstOrDefault();
                //Check whether the Balance Record is available or not.
                if (BalanceObject != null)
                {
                    //Check Balance date whether it is a previous day balance record.
                    if (BalanceObject.BalanceDate.Date == DateTime.Today.Date)
                    {
                        //Check hether the Balance record is already deleted or not.
                        if (BalanceObject.IsDeleted == 0)
                        {
                            //Create new instance of UpdateBalance.
                            UpdateBalance Update = new UpdateBalance()
                            {
                                EditBalanceDTO = BalanceObject,
                                UpdateOption = null,
                                UpdateValue = null
                            };
                            return View(Update);
                        }
                        else
                        {
                            TempData["message"] = "Information: This Balance ID is no longer available!";
                            return RedirectToAction("EditBalance", "Balance");
                        }
                    }
                    else
                    {
                        TempData["message"] = "Information: You have no authority to update previous day balance record!";
                        return RedirectToAction("EditBalance", "Balance");
                    }
                                        
                }
                else
                {
                    TempData["message"] = "Information: No record found for provided Balance ID!";
                    return RedirectToAction("EditBalance", "Balance");
                }

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("EditBalance", "Balance");
            }
        }

        public IActionResult SaveEditBalance(String Option, Decimal UpdateValue, String BalanceID, Decimal BalanceAmount)
        {
            if (ModelState.IsValid)
            {
                if (Option == "+")
                {
                    String SqlQuery = "UPDATE AccountBalance SET BalanceAmount = @Amount, OutstandingBalance = @Outstanding WHERE BalanceID = @BalanceID";
                    _context.Database.ExecuteSqlRaw(SqlQuery, new SqlParameter("@Amount", BalanceAmount + UpdateValue),
                        new SqlParameter("@Outstanding", BalanceAmount + UpdateValue),
                        new SqlParameter("@BalanceID", BalanceID));

                    TempData["message"] = "Success: Account Balance updated successfully!";
                    return RedirectToAction("EditBalance", "Balance");
                }
                else
                {
                    String SqlQuery = "UPDATE AccountBalance SET BalanceAmount = @Amount, OutstandingBalance = @Outstanding WHERE BalanceID = @BalanceID";
                    _context.Database.ExecuteSqlRaw(SqlQuery, new SqlParameter("@Amount", BalanceAmount - UpdateValue),
                        new SqlParameter("@Outstanding", BalanceAmount - UpdateValue),
                        new SqlParameter("@BalanceID", BalanceID));

                    TempData["message"] = "Success: Account Balance updated successfully!";
                    return RedirectToAction("EditBalance", "Balance");
                }
                                
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("EditBalance", "Balance");
            }
        }

        public IActionResult DeleteBalance()
        {
            return View();
        }

        public IActionResult DisplayDeleteBalance(String BalanceID)
        {
            if (ModelState.IsValid)
            {
                String SqlQuery = "SELECT Bal.BalanceID, Bal.BalanceDate, Bal.BalanceAmount, Bal.IsDeleted, Acc.AccountNumber, Bnk.BankName\r\nFROM AccountBalance Bal \r\nLEFT JOIN BankAccount Acc ON Bal.AccountID = Acc.AccountID\r\nLEFT JOIN Bank Bnk ON Acc.BankID = Bnk.BankID WHERE BalanceID = @BalanceID ";
                var DeleteBalance = _context.Database.SqlQueryRaw<DeleteBalanceDTO>(SqlQuery, new SqlParameter("@BalanceID", BalanceID)).FirstOrDefault();

                //Check any record is available for provided Balance ID.
                if (DeleteBalance != null)
                {
                    //Check whether the available record is already deleted or not
                    if (DeleteBalance.IsDeleted == 0)
                    {
                        //Check the balance is a previous day record or not.
                        if (DeleteBalance.BalanceDate.Date == DateTime.Now.Date)
                        {
                            return View(DeleteBalance);
                        }
                        else
                        {
                            TempData["message"] = "Information: You have no authority to delete previous day Balance Record!";
                            return RedirectToAction("DeleteBalance", "Balance");
                        }
                        
                    }
                    else
                    {
                        TempData["message"] = "Information: This Balanace ID is no longer available!";
                        return RedirectToAction("DeleteBalance", "Balance");
                    }
                }
                else
                {
                    TempData["message"] = "Information: No record found for provided Balance ID!";
                    return RedirectToAction("DeleteBalance", "Balance");
                }
                
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("DeleteBalance", "Balance");
            }
            
        }

        public IActionResult ConfirmDelete(DeleteBalanceDTO DeleteBalance)
        {
            if (ModelState.IsValid)
            {
                String SqlQuery = "UPDATE AccountBalance SET IsDeleted = 1 WHERE BalanceID = @BalanceID";
                _context.Database.ExecuteSqlRaw(SqlQuery, new SqlParameter("@BalanceID", DeleteBalance.BalanceID));

                TempData["message"] = "Success: Balance Record deleted successfully!";
                return RedirectToAction("DeleteBalance", "Balance");
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("DeleteBalance", "Balance");
            }
            
        }

        //--------------------------------------------------------------------------------------------

        public IActionResult DisplayBalances()
        {
            return View();
        }

        public IActionResult DisplayAllBalances(DateTime BalanceDate)
        {
            if (ModelState.IsValid)
            {
                String ConvertedDate =  BalanceDate.ToString("yyyy-MM-dd");

                String SqlQuery = "SELECT Bal.BalanceID, Bal.BalanceDate, Bal.BalanceAmount, Bal.OutstandingBalance, Bal.IsDeleted as Status, Bnk.BankName, Acc.AccountNumber, Usr.FirstName\r\nFROM AccountBalance Bal\r\nLEFT JOIN BankAccount Acc ON Bal.AccountID = Acc.AccountID\r\nLEFT JOIN Bank Bnk ON Bnk.BankID = Acc.BankID\r\nLEFT JOIN Users Usr ON Bal.User_Id = Usr.User_Id WHERE BalanceDate LIKE @Date ORDER BY BalanceDate Asc";
                List<DisplayAllBalancesDTO> BalanceList = _context.Database.SqlQueryRaw<DisplayAllBalancesDTO>(SqlQuery, new SqlParameter("@Date", ConvertedDate + "%")).ToList();

                if (BalanceList.Count > 0)
                {
                    return View(BalanceList);
                }
                else
                {
                    TempData["message"] = "Information: No Balance Records available for selected Balance Date!";
                    return RedirectToAction("DisplayBalances", "Balance");
                }
                
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("DisplayBalances", "Balance");
            }

        }

        public IActionResult BalanceAdjustments()
        {
            return View();
        }

        public IActionResult DisplayAdjustmentDetails(String BalanceID)
        {
            if (ModelState.IsValid)
            {
                //Get the relevant Adjustments related to provided BalanceID,
                String Sqlquery01 = "SELECT AdjustmentID, AdjustedDate, AdjustedAmount, Description, Balance\r\nFROM BalanceAdjustment WHERE IsReversed = 0 AND Balance = @BalanceID";
                Dictionary<String, AdjustmentDetailsBalancesDTO> Adjustments = _context.Database.SqlQueryRaw<AdjustmentDetailsBalancesDTO>(Sqlquery01, new SqlParameter("@BalanceID", BalanceID )).
                    ToDictionary(adj => adj.AdjustmentID, Adj => new AdjustmentDetailsBalancesDTO()
                    {
                        AdjustmentID = Adj.AdjustmentID,
                        AdjustedDate = Adj.AdjustedDate,
                        AdjustedAmount = Adj.AdjustedAmount,
                        Description = Adj.Description,
                        Balance = Adj.Balance
                    });

                //Get Opening and Closing balances of the relavant BalanceID.
                String Sqlquery02 = "SELECT Bal.BalanceID, Bal.BalanceAmount, Bal.OutstandingBalance, Bal.BalanceDate, Acc.AccountNumber, Bal.IsDeleted\r\nFROM AccountBalance Bal\r\nLEFT JOIN BankAccount Acc ON Bal.AccountID = Acc.AccountID WHERE BalanceID = @BalanceID";
                AccountBalanceForAdjustments? Balances = _context.Database.SqlQueryRaw<AccountBalanceForAdjustments>(Sqlquery02, new SqlParameter("@BalanceID", BalanceID)).FirstOrDefault();

                //Check whether any balance record is available in the Account Balance Table.
                if (Balances != null)
                {
                    //Create new instance of AccountBalanceAdjustmentDetails and initialize the values.
                    AccountBalanceAdjustmentDetails Details = new AccountBalanceAdjustmentDetails()
                    {
                        AccountBalanceForAdjustments = Balances,
                        AllAdjustments = Adjustments
                    };

                    return View(Details);
                }
                else
                {
                    TempData["message"] = "Information: No Account Balance can be identified for provided Balance ID!";
                    return RedirectToAction("BalanceAdjustments", "Balance");
                }
                                
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("BalanceAdjustments", "Balance");
            }
            
        }
    }
}
