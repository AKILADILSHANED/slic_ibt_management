using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SLICGL_IBT_Management.DB_Connection;
using SLICGL_IBT_Management.Models;
using System.Linq;
using System.Text.RegularExpressions;

namespace SLICGL_IBT_Management.Controllers
{
    public class BankAccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BankAccountController(ApplicationDbContext context)
        {
            this._context = context;
        }

        public IActionResult AccountDetailsForRegister()
        {
            //Get all registered banks from bank table and pass to the relevant AccountDetailsForRegister view file.
            
            String SQLQuery = "SELECT BankID, BankName FROM Bank";
            Dictionary<String, String> BankList = new Dictionary<String, String>();
            BankList = _context.Database.SqlQueryRaw<Bank>(SQLQuery).ToDictionary(bank => bank.BankID.ToString(),
                name => name.BankName.ToString());

            //Create an instance of RegisterBankAccount and passes to the view model with bank list.

            RegisterBankAccount RegisterBankAccount = new RegisterBankAccount()
            {
                BankList = BankList,
                AccountNumber = null,
                AccountType = null,
                Currency = null,
                GLCode = 0,
                BankID = null
            };
            return View(RegisterBankAccount);
        }

        public IActionResult RegisterAccount(RegisterBankAccount BankAccount)
        {
            if (ModelState.IsValid)
            {
                //Check, Same Bank account number available with same Bank Name.
                bool AccountAvailability = _context.BankAccount.Where(acc => acc.AccountNumber == BankAccount.AccountNumber && acc.IsDeleted == 0).Any();
                if (AccountAvailability == false)
                {
                    //Generate new Account ID.
                    //Identify last Account ID in the BankAccount table.

                    BankAccount LastAccountID = _context.BankAccount.OrderByDescending(acc => acc.RegisteredDate).FirstOrDefault();

                    String NewAccountID;

                    if (LastAccountID == null)
                    {
                        //Creation of new user_id.

                        NewAccountID = "ACC/1";
                    }
                    else
                    {
                        //Creation of new user_id.

                        var regex = new Regex(@"\d+");
                        var match = regex.Match(LastAccountID.AccountID);
                        var generate_acc_id = int.Parse(match.Value) + 1;
                        NewAccountID = "ACC/" + generate_acc_id.ToString();
                    }


                    //Create new instance of bank account and initialize the values.
                    BankAccount RegisterAccount = new BankAccount()
                    {
                        AccountID = NewAccountID,
                        AccountNumber = BankAccount.AccountNumber,
                        AccountType = BankAccount.AccountType,
                        AccountBranch = BankAccount.AccountBranch,
                        Currency = BankAccount.Currency,
                        GLCode = BankAccount.GLCode,
                        RegisteredDate = DateTime.Now,
                        IsDeleted = 0,
                        BankID = BankAccount.BankID,
                        User_Id = HttpContext.Session.GetString("UserID")
                    };

                    _context.BankAccount.Add(RegisterAccount);
                    _context.SaveChanges();

                    TempData["message"] = "Success: Bank Account registered successfully with Account ID: " + NewAccountID + "!";
                    return RedirectToAction("AccountDetailsForRegister", "BankAccount");
                }
                else
                {
                    TempData["message"] = "Information: This Account Number is already available!";
                    return RedirectToAction("AccountDetailsForRegister", "BankAccount");
                }
                                
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("AccountDetailsForRegister", "BankAccount");
            }
            
        }

        //---------------------------------------------------------------------------------------------------

        public IActionResult SearchAccount()
        {
            return View();
        }

        public IActionResult DisplayAccountDetailsSearch(String AccountID)
        {
            if (ModelState.IsValid)
            {
                String SQLQuery = "SELECT A.AccountNumber, A.AccountType, A.AccountBranch, A.Currency, A.GLCode, A.RegisteredDate, A.IsDeleted, B.BankName, U.FirstName FROM BankAccount A LEFT JOIN Bank B ON A.BankID = B.BankID LEFT JOIN Users U ON A.User_Id = U.User_Id WHERE AccountID = @AccID";
                var SearchedAccount = _context.Database.SqlQueryRaw<SearchAccountDTO>(SQLQuery, new SqlParameter("@AccID", AccountID.ToUpper())).FirstOrDefault();

                //Check whether any record available for provided Account ID.
                if (SearchedAccount != null)
                {
                    if (SearchedAccount.IsDeleted == 0)
                    {
                        return View(SearchedAccount);
                    }
                    else
                    {
                        TempData["message"] = "Information: This Account ID is no longer available!";
                        return RedirectToAction("SearchAccount", "BankAccount");
                    }
                }
                else
                {
                    TempData["message"] = "Information: No Bank Account available for provided Account ID!";
                    return RedirectToAction("SearchAccount", "BankAccount");
                }

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("SearchAccount", "BankAccount");
            }
            
        }

        public IActionResult DeleteAccount()
        {
            return View();
        }

        public IActionResult DisplayAccountDetailsDelete(String AccountID)
        {
            if (ModelState.IsValid)
            {
                String SQLQuery = "SELECT A.AccountID, A.AccountNumber, A.AccountType, A.AccountBranch, A.Currency, A.GLCode, A.RegisteredDate, A.IsDeleted, B.BankName, U.FirstName FROM BankAccount A LEFT JOIN Bank B ON A.BankID = B.BankID LEFT JOIN Users U ON A.User_Id = U.User_Id WHERE AccountID = @AccID";
                var DeletededAccount = _context.Database.SqlQueryRaw<DeleteAccountDTO>(SQLQuery, new SqlParameter("@AccID", AccountID.ToUpper())).FirstOrDefault();

                //Check whether any record available for provided Account ID.
                if (DeletededAccount != null)
                {
                    //Check whether the record is already deleted or not.
                    if (DeletededAccount.IsDeleted == 0)
                    {
                        //Check whether a Transfer Method has been already configured or not for provided Bank Account
                        bool MethodAvailability = _context.TransferMethod.Where(mthd => (mthd.SendingAccount == AccountID || mthd.ReceivingAccount == AccountID)
                        && mthd.IsDeleted == 0).Any();

                        if (MethodAvailability == false)
                        {
                            return View(DeletededAccount);
                        }
                        else
                        {
                            TempData["message"] = "Information: Transfer Methods have been already configured for this Bank Account. Before delete this, please reverse the all configurations related to this Bank Account!";
                            return RedirectToAction("DeleteAccount", "BankAccount");
                        }
                        
                    }
                    else
                    {
                        TempData["message"] = "Information: This Account ID is no longer available!";
                        return RedirectToAction("DeleteAccount", "BankAccount");
                    }
                }
                else
                {
                    TempData["message"] = "Information: No Bank Account available for provided Account ID!";
                    return RedirectToAction("DeleteAccount", "BankAccount");
                }
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("DeleteAccount", "BankAccount");
            }
            
        }

        public IActionResult ConfirmDelete(DeleteAccountDTO DeleteAccount)
        {
            if (ModelState.IsValid)
            {
                String SQLQuery = "UPDATE BankAccount SET IsDeleted = 1 WHERE AccountID = @AccID";
                _context.Database.ExecuteSqlRaw(SQLQuery, new SqlParameter("@AccID", DeleteAccount.AccountID.ToUpper()));
                TempData["message"] = "Success: Bank Account deleted successfully!";
                return RedirectToAction("DeleteAccount", "BankAccount");
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("DeleteAccount", "BankAccount");
            }
        }

        //--------------------------------------------------------------------------------------------

        public IActionResult UpdateAccount()
        {
            return View();
        }

        public IActionResult DisplayAccountDetailsUpdate(String AccountID)
        {
            if (ModelState.IsValid)
            {
                //Get all available Bank List from Bank table and store in a Dictionary.
                String SqlQuery = "SELECT * FROM Bank";
                //Create New Dictionary.
                Dictionary<String, String> BankList = new Dictionary<String, String>();
                BankList = _context.Database.SqlQueryRaw<Bank>(SqlQuery).ToDictionary(ID => ID.BankID, Name => Name.BankName);

                String SQLQuery = "SELECT A.AccountID, A.AccountNumber, A.AccountType, A.AccountBranch, A.Currency, A.GLCode, A.RegisteredDate, A.IsDeleted, B.BankName, B.BankID, U.User_Id, U.FirstName FROM BankAccount A LEFT JOIN Bank B ON A.BankID = B.BankID LEFT JOIN Users U ON A.User_Id = U.User_Id WHERE AccountID = @AccID";
                UpdateAccountDTO UpdateAccount = _context.Database.SqlQueryRaw<UpdateAccountDTO>(SQLQuery, new SqlParameter("@AccID", AccountID.ToUpper())).FirstOrDefault();

                //Create new instance of UpdateAccountDTO and initialize the values for relevant propertise.
                UpdateAccountWithBankListDTO Account = new UpdateAccountWithBankListDTO()
                {
                    BankList = BankList,
                    AccountID = UpdateAccount.AccountID.ToUpper(),
                    AccountNumber = UpdateAccount.AccountNumber,
                    AccountType = UpdateAccount.AccountType,
                    AccountBranch = UpdateAccount.AccountBranch,
                    Currency = UpdateAccount.Currency,
                    GLCode = UpdateAccount.GLCode,
                    RegisteredDate = UpdateAccount.RegisteredDate,
                    BankName = UpdateAccount.BankName,
                    BankID = UpdateAccount.BankID,
                    User_Id = UpdateAccount.User_Id,
                    FirstName = UpdateAccount.FirstName,
                    IsDeleted = UpdateAccount.IsDeleted
                };

                //Check whether any record available for provided Account ID.
                if (UpdateAccount != null)
                {
                    if (UpdateAccount.IsDeleted == 0)
                    {
                        return View(Account);
                    }
                    else
                    {
                        TempData["message"] = "Information: This Account ID is no longer available!";
                        return RedirectToAction("DeleteAccount", "BankAccount");
                    }
                }
                else
                {
                    TempData["message"] = "Information: No Bank Account available for provided Account ID!";
                    return RedirectToAction("DeleteAccount", "BankAccount");
                }
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("DeleteAccount", "BankAccount");
            }

        }

        public IActionResult ConfirmUpdate(UpdateAccountWithBankListDTO UpdatedAccount)
        {
            if (ModelState.IsValid)
            {
                //Create new instance with Bank Account and initialze the updated values in order to update to the relevant table.
                BankAccount UpdatedAccountObj = new BankAccount()
                {
                    AccountID = UpdatedAccount.AccountID,
                    AccountNumber = UpdatedAccount.AccountNumber,
                    AccountType = UpdatedAccount.AccountType,
                    AccountBranch = UpdatedAccount.AccountBranch,
                    Currency = UpdatedAccount.Currency,
                    GLCode = UpdatedAccount.GLCode,
                    RegisteredDate = UpdatedAccount.RegisteredDate,
                    IsDeleted = UpdatedAccount.IsDeleted,
                    BankID = UpdatedAccount.BankID,
                    User_Id = UpdatedAccount.User_Id

                };

                _context.BankAccount.Update(UpdatedAccountObj);
                _context.SaveChanges();
                TempData["message"] = "Success: Bank Account updated successfully!";
                return RedirectToAction("UpdateAccount", "BankAccount");
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("UpdateAccount", "BankAccount");
            }
            
        }

        //------------------------------------------------------------------------------------------

        public IActionResult DisplayAllAccounts()
        {
            String SQLQuery = "SELECT A.AccountID, A.AccountNumber, A.AccountType, A.AccountBranch, A.Currency, A.GLCode, A.RegisteredDate, A.IsDeleted as Status, B.BankName, U.FirstName \r\nFROM BankAccount A LEFT JOIN Bank B ON A.BankID = B.BankID LEFT JOIN Users U ON A.User_Id = U.User_Id";
            
            List< DisplayAllAccountsDTO> AccountList = _context.Database.SqlQueryRaw<DisplayAllAccountsDTO>(SQLQuery).OrderBy(acc => acc.RegisteredDate).ToList();
            return View(AccountList);
        }
    }

}
