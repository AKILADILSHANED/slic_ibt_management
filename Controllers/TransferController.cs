using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using SLICGL_IBT_Management.DB_Connection;
using SLICGL_IBT_Management.Models;
using System.Text.RegularExpressions;

namespace SLICGL_IBT_Management.Controllers
{
    public class TransferController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransferController(ApplicationDbContext context)
        {
            this._context = context;
        }

        public IActionResult NewTransferOption()
        {
            return View();
        }

        public IActionResult SaveNewTransferOption(NewTransferOption OptionDetails)
        {
            if (ModelState.IsValid)
            {
                //Identify last OptionId in the TransferOption table.

                TransferOption LastOption = _context.TransferOption.OrderByDescending(optn => optn.CreatedDate).FirstOrDefault();
                String string_option_id;

                if (LastOption == null)
                {
                    //Creation of new user_id.

                    string_option_id = "OPTN/1";
                }
                else
                {
                    //Creation of new user_id.

                    var regex = new Regex(@"\d+");
                    var match = regex.Match(LastOption.OptionID);
                    var generate_option_id = int.Parse(match.Value) + 1;
                    string_option_id = "OPTN/" + generate_option_id.ToString();
                }

                //Check same priority level available in a current Transfer Option.
                bool OptionAvailability = _context.TransferOption.Where(priority => priority.Priority == OptionDetails.Priority
                && priority.IsDeleted == 0).Any();

                if (OptionAvailability == false)
                {
                    //Create new instance of Transfer Option and add values to it.
                    TransferOption Option = new TransferOption()
                    {
                        OptionID = string_option_id,
                        OptionType = OptionDetails.OptionType,
                        OptionName = OptionDetails.OptionName,
                        OptionDescription = OptionDetails.OptionDescription,
                        Priority = OptionDetails.Priority,
                        CreatedDate = DateTime.Now,
                        CreateBy = HttpContext.Session.GetString("UserID").ToString(),
                        IsDeleted = 0,
                        DeletedBy = null
                        
                    };

                    _context.TransferOption.Add(Option);
                    _context.SaveChanges();

                    TempData["message"] = "Success: Transfer Option registered successfully!";
                    return RedirectToAction("NewTransferOption", "Transfer");
                }
                else
                {
                    TempData["message"] = "Information: Provided Priority Value is already used by another Transfer Option!";
                    return RedirectToAction("NewTransferOption", "Transfer");
                }
                                
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("NewTransferOption", "Transfer");
            }
            
        }

        public IActionResult RemoveOption()
        {
            return View();
        }

        public IActionResult DislayDetailsRemoveTransfer(String OptionID)
        {

            if (ModelState.IsValid)
            {
                String Sqlquery = "SELECT Tfr.OptionID, Tfr.OptionType, Tfr.OptionName, Tfr.OptionDescription, Tfr.Priority, Tfr.CreatedDate, Tfr.IsDeleted, Usr.FirstName AS CreateBy\r\nFROM TransferOption Tfr \r\nLEFT JOIN Users Usr ON Tfr.CreateBy = Usr.User_Id WHERE Tfr.OptionID = @OptionID ";
                DislayDetailsRemoveTransferDTO OptionObj = _context.Database.SqlQueryRaw<DislayDetailsRemoveTransferDTO>(Sqlquery, new SqlParameter("OptionID", OptionID)).FirstOrDefault();

                //Check relevant Option ID is available in the table.
                if (OptionObj != null)
                {
                    //Check whether the Option is already deleted or not.
                    if (OptionObj.IsDeleted == 0)
                    {
                        return View(OptionObj);
                    }
                    else
                    {
                        TempData["message"] = "Information: This Option ID is no longer available!";
                        return RedirectToAction("RemoveOption", "Transfer");
                    }
                }
                else
                {
                    TempData["message"] = "Information: Option record can not be found. Please Re-check!";
                    return RedirectToAction("RemoveOption", "Transfer");
                }
                                
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("RemoveOption", "Transfer");
            }
        }

        public IActionResult ConfirmDeleteOption(String OptionID)
        {
            if (ModelState.IsValid)
            {
                String Sqlquery = "UPDATE TransferOption SET IsDeleted = 1, DeletedBy = @Deleter WHERE OptionID = @OptionID ";
                _context.Database.ExecuteSqlRaw(Sqlquery, new SqlParameter("@Deleter", HttpContext.Session.GetString("UserID")),
                    new SqlParameter("OptionID", OptionID));

                TempData["message"] = "Success: Option record deleted successfully!";
                return RedirectToAction("RemoveOption", "Transfer");
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("RemoveOption", "Transfer");
            }
        }

        public IActionResult DisplayAllOptions()
        {
            String Sqlquery = "SELECT Tfr.OptionID, Tfr.OptionType, Tfr.OptionName, Tfr.OptionDescription, Tfr.Priority, Tfr.CreatedDate, Usr1.FirstName AS CreateBy,\r\nCASE WHEN Tfr.IsDeleted = 0 THEN 'Active'\r\nELSE 'Deleted' END AS Status,\r\nCASE WHEN DeletedBy IS NULL THEN 'N/A'\r\nELSE Usr2.FirstName END AS DeletedBy\r\nFROM TransferOption Tfr \r\nLEFT JOIN Users Usr1 ON Tfr.CreateBy = Usr1.User_Id \r\nLEFT JOIN Users Usr2 ON Tfr.DeletedBy = Usr2.User_ID ORDER BY Priority";
            List<DisplayAllOptionsDTO> AllOptions = _context.Database.SqlQueryRaw<DisplayAllOptionsDTO>(Sqlquery).ToList();
            return View(AllOptions);
        }

        public IActionResult SetOptionPriority()
        {
            String Sqlquery = "SELECT Tfr.OptionID, Tfr.OptionType, Tfr.OptionName, Tfr.OptionDescription, Tfr.Priority, Tfr.CreatedDate, Tfr.IsDeleted, Usr1.FirstName AS CreateBy\r\nFROM TransferOption Tfr \r\nLEFT JOIN Users Usr1 ON Tfr.CreateBy = Usr1.User_Id WHERE Tfr.IsDeleted = 0\r\nORDER BY Priority";
            Dictionary<String, SetOptionPriority> AllOptions = _context.Database.SqlQueryRaw<SetOptionPriority>(Sqlquery).ToDictionary(optn => optn.OptionID, optn =>  new SetOptionPriority()
            {
                OptionID = optn.OptionID,
                OptionType = optn.OptionType,
                OptionName = optn.OptionName,
                OptionDescription = optn.OptionDescription,
                Priority = optn.Priority,
                CreatedDate = optn.CreatedDate,
                CreateBy = optn.CreateBy,
                IsDeleted = optn.IsDeleted,
                
            });
            return View(AllOptions);
        }

        public IActionResult SaveOptionPriority(String OptionID, String Priority)
        {
            if (ModelState.IsValid)
            {
                String Sqlquery = "UPDATE TransferOption SET Priority = @Priority WHERE OptionID = @OptionID ";
                _context.Database.ExecuteSqlRaw(Sqlquery, new SqlParameter("@Priority", Priority),
                    new SqlParameter("@OptionID", OptionID));

                TempData["message"] = "Success: Option prioritized successfully!";
                return RedirectToAction("SetOptionPriority", "Transfer");
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("SetOptionPriority", "Transfer");
            }
        }

        public IActionResult UpdateTransferOption()
        {
            return View();
        }

        public IActionResult DisplayDetailsForUpdateTransferOption(String OptionID)
        {
            if (ModelState.IsValid)
            {
                String Sqlquery = "SELECT Tfr.OptionID, Tfr.OptionType, Tfr.OptionName, Tfr.OptionDescription, Tfr.Priority, Tfr.IsDeleted, Tfr.CreatedDate, Usr.FirstName AS CreatedBy\r\nFROM TransferOption Tfr\r\nLEFT JOIN Users Usr ON Tfr.CreateBy = Usr.User_Id WHERE Tfr.IsDeleted = 0 AND OptionID = @OptionID ";
                UpdateTransferOptionDTO OptionObj = _context.Database.SqlQueryRaw<UpdateTransferOptionDTO>(Sqlquery, new SqlParameter("@OptionID", OptionID)).FirstOrDefault();

                //Check whether an Option is available or not.
                if (OptionObj != null)
                {
                    //Check whether the Option is already deleted or not.
                    if (OptionObj.IsDeleted == 0)
                    {
                        return View(OptionObj);
                    }
                    else
                    {
                        TempData["message"] = "Information: This Option is no longer available!";
                        return RedirectToAction("UpdateTransferOption", "Transfer");
                    }
                }
                else
                {
                    TempData["message"] = "Information: No Option available for provided Option ID!";
                    return RedirectToAction("UpdateTransferOption", "Transfer");
                }
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("UpdateTransferOption", "Transfer");
            }
            
        }

        public IActionResult ConfirmUpdateOption(UpdateTransferOptionDTO UpdateOption)
        {

            if (ModelState.IsValid)
            {
                String Sqlquery = "UPDATE TransferOption SET OptionType = @Type , OptionName = @Name , OptionDescription = @Description  WHERE OptionID = @OptionID ";
                _context.Database.ExecuteSqlRaw(Sqlquery, new SqlParameter("@Type", UpdateOption.OptionType),
                    new SqlParameter("@Name", UpdateOption.OptionName),
                    new SqlParameter("@Description", UpdateOption.OptionDescription),
                    new SqlParameter("@OptionID", UpdateOption.OptionID));

                TempData["message"] = "Success: Option updated successfully!";
                return RedirectToAction("UpdateTransferOption", "Transfer");

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("UpdateTransferOption", "Transfer");
            }
        
        }

        public IActionResult ConfigureTransferMethod()
        {
            //Get the list of All Bank Accounts and Transfer Options and assigned for TransferOptionsWithBankAccounts model.
            TransferOptionsWithBankAccounts Model = new TransferOptionsWithBankAccounts();

            Model.BankAccountList = _context.BankAccount.OrderBy(order => order.RegisteredDate).Where(Acc => Acc.IsDeleted == 0).ToDictionary(Acc => Acc.AccountID,
               AccDic => new BankAccountForTransferMethod() { AccountID = AccDic.AccountID ,AccountNumber = AccDic.AccountNumber} );

            Model.TransferOptionList = _context.TransferOption.Where(optn => optn.IsDeleted == 0).ToDictionary(optn => optn.OptionID, 
                optn => new TransferOptionsForTransferMethod() { OptionID = optn.OptionID ,OptionDescription = optn.OptionDescription});
            
            return View(Model);
        }

        public IActionResult ConfigurationDetails(TransferOptionsWithBankAccounts OptionWithBank)
        {
            if (ModelState.IsValid)
            {
                Dictionary<String, String> AccountList = _context.BankAccount.OrderBy(order => order.RegisteredDate).Where(acc => acc.AccountID != OptionWithBank.SendingAccountID && acc.IsDeleted == 0)
                    .ToDictionary(acc => acc.AccountID, acc => acc.AccountNumber);

                /*Loop each element in the dictionary and check whether the relevant AccountID is available in the 
                 TransferMethod table as a ReceivingAccount with user selected OptionID. If it is available, the relevant
                AccountID will be removed from the dictionary and remaining BankAccounts will be considered as to set up
                the Payment Methods */

                foreach (var AccountObject in AccountList)
                {
                    bool Availability = _context.TransferMethod.Where(Method => Method.ReceivingAccount == AccountObject.Key && 
                    Method.SendingAccount == OptionWithBank.SendingAccountID &&
                    Method.TransferOption == OptionWithBank.OptionID && Method.IsDeleted == 0).Any();

                    if (Availability == true)
                    {
                        AccountList.Remove(AccountObject.Key);
                    }
                    else
                    {
                        //No code block to be run in this part.
                    }
                }
                //Get Sending Account Number.
                String SendingAccountNumber = _context.BankAccount.Find(OptionWithBank.SendingAccountID).AccountNumber;

                //Get Transfer Description.
                String TransferDescription = _context.TransferOption.Find(OptionWithBank.OptionID).OptionDescription;

                //Create an instance of ConfigurationDetails and initialize the values.

                ConfigurationDetails Config = new ConfigurationDetails()
                {
                    SendingAccountID = OptionWithBank.SendingAccountID,
                    SendingAccountNumber = SendingAccountNumber,
                    OptionID = OptionWithBank.OptionID,
                    ReceivingAccountList = AccountList,
                    OptionDescription = TransferDescription,
                };

                return View(Config);
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("ConfigurationDetails", "Transfer");
            }
        }

        public IActionResult SaveTransferMethod(String SendAccountID, String ReceiveAccountID, String OptionID)
        {
            //Identify last Method in the TransferMethod table.

            TransferMethod last_method = _context.TransferMethod.OrderByDescending(method => method.CreatedOn).FirstOrDefault();
            String string_method_id;

            if (last_method == null)
            {
                //Creation of new user_id.

                string_method_id = "METHOD/1";
            }
            else
            {
                //Creation of new user_id.

                var regex = new Regex(@"\d+");
                var match = regex.Match(last_method.MethodID);
                var generate_method_id = int.Parse(match.Value) + 1;
                string_method_id = "METHOD/" + generate_method_id.ToString();
            }
            
            //Create an instance of TransferMethod and initialize the values.
            TransferMethod Method = new TransferMethod()
            {
                MethodID = string_method_id,
                SendingAccount = SendAccountID,
                ReceivingAccount = ReceiveAccountID,
                TransferOption = OptionID,
                CreateBy = HttpContext.Session.GetString("UserID").ToString(),
                CreatedOn = DateTime.Now,
                IsDeleted = 0,
                DeletedBy = null,
                IsActive = 1
            };

            _context.TransferMethod.Add(Method);
            _context.SaveChanges();
            
            TempData["message"] = "Success: Transfer Method successfully configured with Method ID: " + string_method_id + "!";
            return RedirectToAction("ConfigureTransferMethod", "Transfer");
        }

        public IActionResult DeleteTransferMethod()
        {
            return View();
        }

        public IActionResult DeleteTransferMethodDetails(String MethodID)
        {
            if (ModelState.IsValid)
            {
                //Check whether relevant method id is already available in the Transfer Method table.
                String Sqlquery = "SELECT Methd.MethodID, Usr.FirstName, Methd.CreatedOn, Methd.IsDeleted, Acc1.AccountNumber AS SendingAccount, Acc2.AccountNumber AS ReceivingAccount,\r\nOptn.OptionDescription AS Description\r\nFROM TransferMethod Methd\r\nLEFT JOIN BankAccount Acc1 ON Methd.SendingAccount = Acc1.AccountID\r\nLEFT JOIN BankAccount Acc2 ON Methd.ReceivingAccount = Acc2.AccountID\r\nLEFT JOIN Users Usr ON Methd.CreateBy = Usr.User_Id\r\nLEFT JOIN TransferOption Optn ON Methd.TransferOption = Optn.OptionID WHERE Methd.MethodID = @MethodID ";
                DeleteTransferMethodDetailsDTO Method = _context.Database.SqlQueryRaw<DeleteTransferMethodDetailsDTO>(Sqlquery, new SqlParameter("@MethodID", MethodID)).FirstOrDefault();
                
                if (Method != null)
                {
                    //Check whether the relevant record is already deleted or not.

                    if (Method.IsDeleted == 0)
                    {
                        return View(Method);
                    }
                    else
                    {
                        TempData["message"] = "Information: This record is no longer available!";
                        return RedirectToAction("DeleteTransferMethod", "Transfer");
                    }
                }
                else
                {
                    TempData["message"] = "Information: No record found for provided Method ID!";
                    return RedirectToAction("DeleteTransferMethod", "Transfer");
                }
                
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("DeleteTransferMethod", "Transfer");
            }
            
        }

        public IActionResult SaveDeleteTransferMethod(String MethodID)
        {
            if (ModelState.IsValid)
            {
                String Sqlquery = "UPDATE TransferMethod SET IsDeleted = 1, DeletedBy = @DeleteBy, IsActive = 0 WHERE MethodID = @MethodID ";
                _context.Database.ExecuteSqlRaw(Sqlquery, new SqlParameter("@DeleteBy", HttpContext.Session.GetString("UserID")),
                    new SqlParameter("@MethodID", MethodID));

                TempData["message"] = "Success: Method ID is deleted successfully!!";
                return RedirectToAction("DeleteTransferMethod", "Transfer");

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("DeleteTransferMethod", "Transfer");
            }
        }

        public IActionResult ActiveTransferMethod()
        {
            //Get the list of Non-Active Transfer Methods.
            String Sqlquery = "SELECT Methd.MethodID, Usr.FirstName, Methd.CreatedOn, Methd.IsDeleted, Acc1.AccountNumber AS SendingAccount, Acc2.AccountNumber AS ReceivingAccount,\r\nOptn.OptionDescription AS Description\r\nFROM TransferMethod Methd\r\nLEFT JOIN BankAccount Acc1 ON Methd.SendingAccount = Acc1.AccountID\r\nLEFT JOIN BankAccount Acc2 ON Methd.ReceivingAccount = Acc2.AccountID\r\nLEFT JOIN Users Usr ON Methd.CreateBy = Usr.User_Id\r\nLEFT JOIN TransferOption Optn ON Methd.TransferOption = Optn.OptionID WHERE Methd.IsDeleted = 0 AND Methd.IsActive = 0 ORDER BY Methd.CreatedOn";
            
            return View(_context.Database.SqlQueryRaw<ActiveTransferMethodDTO>(Sqlquery).ToList());
        }

        public IActionResult SaveActivation(String MethodID)
        {
            String Sqlquery = "UPDATE TransferMethod SET IsActive = 1 WHERE MethodID = @MethodID ";
            _context.Database.ExecuteSqlRaw(Sqlquery, new SqlParameter("@MethodID", MethodID));

            TempData["message"] = "Success: Method ID is activated successfully!!";
            return RedirectToAction("ActiveTransferMethod", "Transfer");
        }

        public IActionResult DeActiveTransferMethod()
        {
            //Get the list of Non-Active Transfer Methods.
            String Sqlquery = "SELECT Methd.MethodID, Usr.FirstName, Methd.CreatedOn, Methd.IsDeleted, Acc1.AccountNumber AS SendingAccount, Acc2.AccountNumber AS ReceivingAccount,\r\nOptn.OptionDescription AS Description\r\nFROM TransferMethod Methd\r\nLEFT JOIN BankAccount Acc1 ON Methd.SendingAccount = Acc1.AccountID\r\nLEFT JOIN BankAccount Acc2 ON Methd.ReceivingAccount = Acc2.AccountID\r\nLEFT JOIN Users Usr ON Methd.CreateBy = Usr.User_Id\r\nLEFT JOIN TransferOption Optn ON Methd.TransferOption = Optn.OptionID WHERE Methd.IsDeleted = 0 AND Methd.IsActive = 1 ORDER BY Methd.CreatedOn";

            return View(_context.Database.SqlQueryRaw<DeActiveTransferMethodDTO>(Sqlquery).ToList());
        }

        public IActionResult SaveDeActivation(String MethodID)
        {
            String Sqlquery = "UPDATE TransferMethod SET IsActive = 0 WHERE MethodID = @MethodID ";
            _context.Database.ExecuteSqlRaw(Sqlquery, new SqlParameter("@MethodID", MethodID));

            TempData["message"] = "Success: Method ID is De-Activated successfully!!";
            return RedirectToAction("DeActiveTransferMethod", "Transfer");
        }

        public IActionResult DisplayAllTransferMethods()
        {
            String Sqlquery = "SELECT \r\nMthd.MethodID, \r\nAcc1.AccountNumber AS SendingAccountNumber,\r\nAcc2.AccountNumber AS ReceivingAccountNumber,\r\nOptn.OptionDescription AS Description,\r\nUsr1.FirstName AS CreatedBy,\r\nCASE WHEN Mthd.IsDeleted = 0 THEN 'Not-Deleted' ELSE 'Deleted' END AS DeleteStatus,\r\nCASE WHEN Mthd.DeletedBy IS NULL THEN 'N/A' ELSE Usr2.FirstName END AS DeleteBy,\r\nCASE WHEN Mthd.IsActive = 1 THEN 'Active' ELSE 'De-Active' END AS Status\r\nFROM TransferMethod Mthd\r\nLEFT JOIN BankAccount Acc1 ON Mthd.SendingAccount = Acc1.AccountID\r\nLEFT JOIN BankAccount Acc2 ON Mthd.ReceivingAccount = Acc2.AccountID\r\nLEFT JOIN Users Usr1 ON Mthd.CreateBy = Usr1.User_Id\r\nLEFT JOIN Users Usr2 ON Mthd.DeletedBy = Usr2.User_Id\r\nLEFT JOIN TransferOption Optn ON Mthd.TransferOption = Optn.OptionID";
            List<DisplayAllTransferMethodsDTO> AllMethods = _context.Database.SqlQueryRaw<DisplayAllTransferMethodsDTO>(Sqlquery).ToList();
            return View(AllMethods);

        }


        public IActionResult AutomatedTransfers()
        {
            //Check whether an IBTSheet is already created or not.

            bool SheetAvailability = _context.IBTSheet.Where(sht => sht.CreatedDate.Date == DateTime.Today.Date).Any();

            if (SheetAvailability == false)
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

                //Create new instance of IBTSheet and initialize the value.
                IBTSheet Sheet = new IBTSheet()
                {
                    SheetID = NewSheetID,
                    CreatedDate = DateTime.Now,
                    CreateBy = HttpContext.Session.GetString("UserID").ToString()
                };

                _context.Add(Sheet);
                _context.SaveChanges();

                //Get relevant IBT Sheet data for current date.
                String Sqlquery = "SELECT Sht.SheetID, SHT.CreatedDate, Usr.FirstName AS CreateBy\r\nFROM IBTSheet Sht\r\nLEFT JOIN Users Usr ON Sht.CreateBy = Usr.User_Id WHERE CreatedDate LIKE @Date ";
                IBTSheetDetailsDTO IBTSheetObj = _context.Database.SqlQueryRaw<IBTSheetDetailsDTO>(Sqlquery, new SqlParameter("@Date", DateTime.Today.Date.ToString("yyyy-MM-dd") + "%")).FirstOrDefault();

                return View(IBTSheetObj);

            }
            else
            {
                //Get relevant IBT Sheet data for current date.
                String Sqlquery = "SELECT Sht.SheetID, SHT.CreatedDate, Usr.FirstName AS CreateBy\r\nFROM IBTSheet Sht\r\nLEFT JOIN Users Usr ON Sht.CreateBy = Usr.User_Id WHERE CreatedDate LIKE @Date ";
                IBTSheetDetailsDTO IBTSheetObj = _context.Database.SqlQueryRaw<IBTSheetDetailsDTO>(Sqlquery, new SqlParameter("@Date", DateTime.Today.Date.ToString("yyyy-MM-dd") + "%")).FirstOrDefault();

                return View(IBTSheetObj);
            }

        }

        //This method has been created to generate a new AdjustmentID for a perticular Account Balance.
        public String CreateBalanceAdjustmentID()
        {
            //Generate new Adjusmrnt ID.
            BalanceAdjustment LastAdjustment = _context.BalanceAdjustment.OrderByDescending(date => date.AdjustedDate).FirstOrDefault();

            String NewAdjustmentID;

            if (LastAdjustment == null)
            {
                NewAdjustmentID = "BAL/ADJ/"  + DateTime.Now.Year + "/" + 1;
            }
            else
            {
                //Creation of new AdjustmentID.
                string[] parts = LastAdjustment.AdjustmentID.Split('/');
                int lastPart = int.Parse(parts.Last());
                int NewNumericValue;
                NewAdjustmentID = null;
                //Get current Year.
                int CurrentYear = DateTime.Now.Year;
                if (parts[2] == CurrentYear.ToString())
                {
                    NewNumericValue = lastPart + 1;
                    NewAdjustmentID = "BAL/ADJ/" + CurrentYear + "/" + NewNumericValue;
                }
                else
                {
                    NewNumericValue = 1;
                    NewAdjustmentID = "BAL/ADJ/" + CurrentYear + "/" + NewNumericValue;
                }
            }
            return NewAdjustmentID;
        }

        //This method has been created to generate a new RequestAdjustmentID for a perticular Account Fund Request.
        public String RequestAdjustmentID()
        {
            //Generate new Adjusmrnt ID.
            FundRequestAdjustments LastAdjustment = _context.FundRequestAdjustments.OrderByDescending(date => date.AdjustedDate).FirstOrDefault();

            String NewAdjustmentID;

            if (LastAdjustment == null)
            {
                NewAdjustmentID = "REQ/ADJ/" + DateTime.Now.Year + "/" + 1;
            }
            else
            {
                //Creation of new AdjustmentID.
                string[] parts = LastAdjustment.AdjustmentID.Split('/');
                int lastPart = int.Parse(parts.Last());
                int NewNumericValue;
                NewAdjustmentID = null;
                //Get current Year.
                int CurrentYear = DateTime.Now.Year;
                if (parts[2] == CurrentYear.ToString())
                {
                    NewNumericValue = lastPart + 1;
                    NewAdjustmentID = "REQ/ADJ/" + CurrentYear + "/" + NewNumericValue;
                }
                else
                {
                    NewNumericValue = 1;
                    NewAdjustmentID = "REQ/ADJ/" + CurrentYear + "/" + NewNumericValue;
                }
            }
            return NewAdjustmentID;
        }

        //This method has been created to generate a new RepoAdjustmentID for a perticular Repo Balance.
        public String CreateRepoBalanceAdjustmentID()
        {
            //Generate new Adjusmrnt ID.
            RepoBalanceAdjustment LastAdjustment = _context.RepoBalanceAdjustment.OrderByDescending(date => date.AdjustedDate).FirstOrDefault();

            String NewAdjustmentID;

            if (LastAdjustment == null)
            {
                NewAdjustmentID = "REPO/ADJ/" + DateTime.Now.Year + "/" + 1;
            }
            else
            {
                //Creation of new AdjustmentID.
                string[] parts = LastAdjustment.AdjustmentID.Split('/');
                int lastPart = int.Parse(parts.Last());
                int NewNumericValue;
                NewAdjustmentID = null;
                //Get current Year.
                int CurrentYear = DateTime.Now.Year;
                if (parts[2] == CurrentYear.ToString())
                {
                    NewNumericValue = lastPart + 1;
                    NewAdjustmentID = "REPO/ADJ/" + CurrentYear + "/" + NewNumericValue;
                }
                else
                {
                    NewNumericValue = 1;
                    NewAdjustmentID = "REPO/ADJ/" + CurrentYear + "/" + NewNumericValue;
                }
            }
            return NewAdjustmentID;
        }

        public String CreateOverdraftAdjustmentID()
        {
            //Generate new Adjusmrnt ID.
            OverdraftRecoverAdjustment LastAdjustment = _context.OverdraftRecoverAdjustment.OrderByDescending(date => date.AdjustedDate).FirstOrDefault();

            String NewAdjustmentID;

            if (LastAdjustment == null)
            {
                NewAdjustmentID = "OD/ADJ/" + DateTime.Now.Year + "/" + 1;
            }
            else
            {
                //Creation of new AdjustmentID.
                string[] parts = LastAdjustment.AdjustmentID.Split('/');
                int lastPart = int.Parse(parts.Last());
                int NewNumericValue;
                NewAdjustmentID = null;
                //Get current Year.
                int CurrentYear = DateTime.Now.Year;
                if (parts[2] == CurrentYear.ToString())
                {
                    NewNumericValue = lastPart + 1;
                    NewAdjustmentID = "OD/ADJ/" + CurrentYear + "/" + NewNumericValue;
                }
                else
                {
                    NewNumericValue = 1;
                    NewAdjustmentID = "OD/ADJ/" + CurrentYear + "/" + NewNumericValue;
                }
            }
            return NewAdjustmentID;
        }


        public String CreateTransferID()
        {
            //Generate new Transfer ID.
            Transfers LastTransferObj = _context.Transfers.OrderByDescending(date => date.TransferDate).FirstOrDefault();

            String NewTransferID;

            if (LastTransferObj == null)
            {
                NewTransferID = "TFR" + "/" + DateTime.Now.Year + "/" + 1;
            }
            else
            {
                //Creation of new BalanceID.
                string[] parts = LastTransferObj.TransferId.Split('/');
                int lastPart = int.Parse(parts.Last());
                int NewNumericValue;
                NewTransferID = null;
                //Get current Year.
                int CurrentYear = DateTime.Now.Year;
                if (parts[1] == CurrentYear.ToString())
                {
                    NewNumericValue = lastPart + 1;
                    NewTransferID = "TFR/" + CurrentYear + "/" + NewNumericValue;
                }
                else
                {
                    NewNumericValue = 1;
                    NewTransferID = "TFR/" + CurrentYear + "/" + NewNumericValue;
                }
            }
            return NewTransferID;
        }


        public IActionResult InitiateTransfers(String SheetID)
        {
            //Check whether an Excess Repo is already created for current date.
            bool RepoAvailability = _context.Repo.Where(repo => repo.RepoType == "Excess Repo" && repo.CreatedDate.Date == DateTime.Today.Date
            && repo.IsDeleted == 0).Any();

            if (RepoAvailability == true)
            {
                //An Excess Repo is available.
                /* Create a Dictionary of all Fund Requests related to the current date and store the list of requests in the Fund
                Request Table. */

                String SqlqueryForRequest = "SELECT Req.RequestID,\r\nReq.PaymentID, \r\nReq.AccountID, \r\nReq.RemainingRequestAmount, \r\nBal.BalanceAmount,\r\nBal.OutstandingBalance,\r\nReq.RequestAmount, \r\nPmnt.PaymentType \r\nFROM FundRequest Req\r\nLEFT JOIN Payment Pmnt ON Req.PaymentID = Pmnt.PaymentID\r\nLEFT JOIN AccountBalance Bal ON Req.AccountID = Bal.AccountID\r\nWHERE Req.IsDeleted = 0 AND\r\nReq.IsApproved = 1 AND \r\nReq.RequiredDate LIKE @CurrentRequiredDate AND\r\nBal.BalanceDate LIKE @CurrentBalanceDate";

                Dictionary<String, FundRequestForTransfers> FundRequestList = _context.Database.SqlQueryRaw<FundRequestForTransfers>(SqlqueryForRequest, new SqlParameter("@CurrentRequiredDate", DateTime.Today.Date.ToString("yyyy-MM-dd") + "%"),
                    new SqlParameter("CurrentBalanceDate", DateTime.Today.Date.ToString("yyyy-MM-dd") + "%") ).ToDictionary(req => req.RequestID,
                    req => new FundRequestForTransfers()
                    {
                        RequestID = req.RequestID,
                        PaymentID = req.PaymentID,
                        AccountID = req.AccountID,
                        OutstandingBalance = req.OutstandingBalance,
                        PaymentType = req.PaymentType,
                        RequestAmount = req.RequestAmount,
                        RemainingRequestAmount = req.RemainingRequestAmount
                    });

                /* Create a Dictionary of all Account Balance related to the current date and store the list of balances in the Account
                Balance Table. */

                String SqlqueryForBalances = "SELECT Bal.BalanceID, Acc.AccountID, Acc.AccountNumber, Bal.OutstandingBalance, Bal.OutstandingBalance AS TransferableBalance \r\nFROM AccountBalance Bal\r\nLEFT JOIN BankAccount Acc ON Bal.AccountID = Acc.AccountID\r\nWHERE Bal.IsDeleted = 0 AND BalanceDate LIKE @CurrentDate ";

                Dictionary<String, AccountBalanceForTransfers> BalanceList = _context.Database.SqlQueryRaw<AccountBalanceForTransfers>(SqlqueryForBalances, new SqlParameter("@CurrentDate", DateTime.Today.Date.ToString("yyyy-MM-dd") + "%"))
                    .ToDictionary(acc => acc.AccountID, bal => new AccountBalanceForTransfers()
                    {
                        BalanceID = bal.BalanceID,
                        AccountID = bal.AccountID,
                        AccountNumber = bal.AccountNumber,
                        OutstandingBalance = bal.OutstandingBalance,
                        TransferableBalance = bal.OutstandingBalance
                    });

                /* Create a Dictionary of all Transfer Options and store the list of Options in the Transfer Option table under
                 the priority level */

                String SqlqueryForTransferOptions = "SELECT OptionID, OptionName, Priority FROM TransferOption WHERE IsDeleted = 0 ORDER BY Priority Asc";
                Dictionary<String, TransferOptionsForTransfers> OptionList = _context.Database.SqlQueryRaw<TransferOptionsForTransfers>(SqlqueryForTransferOptions).ToDictionary(optn => optn.OptionID,
                    optn => new TransferOptionsForTransfers()
                    {
                        OptionID = optn.OptionID,
                        OptionName = optn.OptionName,
                        Priority = optn.Priority
                    });


                /* Loop each Fund Request Dictionary and get the Acount ID of each Fund Requests. Then each Account ID will be
                checked in the Account Balance Dictionary and make a comparison between the Outstanding Balance and the Fund Request
                Amount. */

                foreach (FundRequestForTransfers Request in FundRequestList.Values)
                {
                    Decimal AccountBalance = BalanceList[Request.AccountID].TransferableBalance;
                    Decimal DifferenceAccountBalance = AccountBalance - Request.RequestAmount;

                    switch (DifferenceAccountBalance)
                    {
                        case 0:

                            //Create an instance of FundRequestAdjustments and initialize the values.
                            FundRequestAdjustments AdjustmentCase01 = new FundRequestAdjustments()
                            {
                                AdjustmentID = RequestAdjustmentID(),
                                AdjustedDate = DateTime.Now,
                                AdjustedAmount = AccountBalance * -1,
                                Description = "Recovered from available balance",
                                AdjustBy = HttpContext.Session.GetString("UserID"),
                                IsReversed = 0,
                                ReversedBy = null,
                                RequestID = Request.RequestID,
                                TransferID = null
                            };
                            _context.FundRequestAdjustments.Add(AdjustmentCase01);
                            _context.SaveChanges();

                            //Update the Dictionary of BalanceList and Fund Request.
                            BalanceList[Request.AccountID].TransferableBalance = 0;
                            FundRequestList[Request.RequestID].RequestAmount = 0;

                            continue;

                        case < 0:
                                                        
                            //Create an instance of BalanceAdjustment and Update BalanceAdjustment table accordingly.

                            if (AccountBalance > 0)
                            {
                                //Create an instance of FundRequestAdjustments and initialize the values.
                                FundRequestAdjustments AdjustmentCase02 = new FundRequestAdjustments()
                                {
                                    AdjustmentID = RequestAdjustmentID(),
                                    AdjustedDate = DateTime.Now,
                                    AdjustedAmount = AccountBalance * -1,
                                    Description = "Recovered from available balance",
                                    AdjustBy = HttpContext.Session.GetString("UserID"),
                                    IsReversed = 0,
                                    ReversedBy = null,
                                    RequestID = Request.RequestID,
                                    TransferID = null
                                };
                                _context.FundRequestAdjustments.Add(AdjustmentCase02);
                                _context.SaveChanges();

                                //Update the Dictionary of BalanceList and Fund Request.
                                BalanceList[Request.AccountID].TransferableBalance = 0;
                                FundRequestList[Request.RequestID].RequestAmount = DifferenceAccountBalance * -1;

                                continue;
                            }
                            else if (AccountBalance < 0)
                            {

                                //Create an instance of OverdraftRecoverAdjustment and initialize the values.
                                OverdraftRecoverAdjustment Adjustment = new OverdraftRecoverAdjustment()
                                {
                                    AdjustmentID = CreateOverdraftAdjustmentID(),
                                    AdjustedDate = DateTime.Now,
                                    AdjustedAmount = AccountBalance * -1,
                                    Description = "Recovered Over Draft balance",
                                    AdjustBy = HttpContext.Session.GetString("UserID"),
                                    IsReversed = 0,
                                    ReversedBy = null,
                                    RequestID = Request.RequestID,
                                    BalanceID = BalanceList[Request.AccountID].BalanceID
                                };
                                _context.OverdraftRecoverAdjustment.Add(Adjustment);
                                _context.SaveChanges();

                                //Update the Dictionary of BalanceList and Fund Request.
                                BalanceList[Request.AccountID].TransferableBalance = 0;
                                FundRequestList[Request.RequestID].RequestAmount = DifferenceAccountBalance * -1;

                                continue;
                            }
                            else
                            {
                                //No code block to be run.
                                continue;
                            }


                        case > 0:

                            //Update the Dictionary of BalanceList and Fund Request.
                            //BalanceList[Request.AccountID].OutstandingBalance = Request.RequestAmount;
                            BalanceList[Request.AccountID].TransferableBalance = DifferenceAccountBalance;
                            FundRequestList[Request.RequestID].RequestAmount = 0;

                            continue;


                    }
                }


                // Loop Transfer Options Dictionary under priority level.
                foreach (TransferOptionsForTransfers Option in OptionList.Values)
                {
                    /* Re-loop each Fund Requests of the Fund Request Dictionary and initiate transfers according to the priority
                        of Transfer Options. */
                    foreach (FundRequestForTransfers Request in FundRequestList.Values)
                    {
                        if (Request.RequestAmount > 0)
                        {
                            //Check whether any Transfer Methods have been available for relevant Transfer Option.
                            String Sqlquery = "SELECT Mthd.MethodID, Mthd.SendingAccount, Mthd.ReceivingAccount, AccRec.AccountNumber AS ReceivingAccountNumber, AccSend.AccountNumber AS SendingAccountNumber, Mthd.TransferOption \r\nFROM TransferMethod Mthd\r\nLEFT JOIN BankAccount AccSend ON Mthd.SendingAccount = AccSend.AccountID\r\nLEFT JOIN BankAccount AccRec ON Mthd.ReceivingAccount = AccRec.AccountID WHERE ReceivingAccount = @AccountReceive AND Mthd.IsDeleted = 0 AND Mthd.IsActive = 1 AND TransferOption = @Option ";
                            Dictionary<String, TransferMethodsForTransfers> MethodList = _context.Database.SqlQueryRaw<TransferMethodsForTransfers>(Sqlquery, new SqlParameter("@AccountReceive", Request.AccountID),
                                new SqlParameter("@Option", Option.OptionID)).ToDictionary(method => method.MethodID,
                                method => new TransferMethodsForTransfers()
                                {
                                    MethodID = method.MethodID,
                                    SendingAccount = method.SendingAccount,
                                    ReceivingAccount = method.ReceivingAccount,
                                    SendingAccountNumber = method.SendingAccountNumber,
                                    ReceivingAccountNumber = method.ReceivingAccountNumber,
                                    TransferOption = method.TransferOption
                                });

                            //Check wheher any Transfer Methods available for provided Option ID under Fund Request Account as a Receving Account.
                            if (MethodList.Count > 0)
                            {
                                //If Methods available, loop each Transfer Metods and initiate Transfers under this methods.
                                //Before initiation of each Transfer, need to check Request Balance has become 0 from previous transfer.

                                foreach (TransferMethodsForTransfers MethodObj in MethodList.Values)
                                {
                                    /* Check whether a Balance has been updated for the Sending Bank Account included in MethodObj. 
                                     If the balance has not been entered to the Sending Bank Account included in MethodObj, the relevant
                                     Account Number will be ignored for fund transfer, since no account balance available */

                                    if (Request.RequestAmount > 0 && BalanceList.ContainsKey(MethodObj.SendingAccount) == true && BalanceList[MethodObj.SendingAccount].TransferableBalance > 0)
                                    {
                                        //Initiate the Transfer under the relevant Transfer Method.
                                        Decimal TransferableAccountBalance = BalanceList[MethodObj.SendingAccount].TransferableBalance;
                                        Decimal RequestBalance = Request.RequestAmount;
                                        Decimal DifferenceBalances = TransferableAccountBalance - RequestBalance;

                                        //Call back CreateTransferID method to create a new Transfer ID.

                                        String TransferID = this.CreateTransferID();


                                        switch (DifferenceBalances)
                                        {
                                            case 0:
                                                //If Account Balance and Fund Request Balance is eaquel.

                                                //Initiate the Transfer.

                                                //Create an Instance of Transfer and initialize the values.
                                                Transfers TransfersCase01 = new Transfers()
                                                {
                                                    TransferId = TransferID,
                                                    TransferDate = DateTime.Now,
                                                    TransferAmount = TransferableAccountBalance,
                                                    FromBankAccount = MethodObj.SendingAccount,
                                                    ToBankAccount = MethodObj.ReceivingAccount,
                                                    TransferMethod = MethodObj.MethodID,
                                                    IsApproved = 0,
                                                    ApprovedBy = null,
                                                    IsDeleted = 0,
                                                    DeletedBy = null,
                                                    AccountBalance = BalanceList[MethodObj.SendingAccount].BalanceID,
                                                    CreateBy = HttpContext.Session.GetString("UserID").ToString(),
                                                    Payment = Request.PaymentID,
                                                    IBTSheet = SheetID,
                                                };
                                                _context.Transfers.Add(TransfersCase01);
                                                _context.SaveChanges();



                                                //Create an instance of FundRequestAdjustments and initialize the values.
                                                FundRequestAdjustments AdjustmentCase03 = new FundRequestAdjustments()
                                                {
                                                    AdjustmentID = RequestAdjustmentID(),
                                                    AdjustedDate = DateTime.Now,
                                                    AdjustedAmount = TransferableAccountBalance * -1,
                                                    Description = "Funds received from: " + MethodObj.SendingAccountNumber,
                                                    AdjustBy = HttpContext.Session.GetString("UserID"),
                                                    IsReversed = 0,
                                                    ReversedBy = null,
                                                    RequestID = Request.RequestID,
                                                    TransferID = TransferID
                                                };
                                                _context.FundRequestAdjustments.Add(AdjustmentCase03);
                                                _context.SaveChanges();


                                                //Create an instance of BalanceAdjustment and Update BalanceAdjustment table for fund sending account balance accordingly.
                                                BalanceAdjustment AdjustmentSending01 = new BalanceAdjustment()
                                                {
                                                    AdjustmentID = this.CreateBalanceAdjustmentID(),
                                                    AdjustedDate = DateTime.Now,
                                                    AdjustedAmount = TransferableAccountBalance * -1,
                                                    Description = "Fund transfered to A/C: " + MethodObj.ReceivingAccountNumber,
                                                    AdjustBy = HttpContext.Session.GetString("UserID"),
                                                    IsReversed = 0,
                                                    ReversedBy = null,
                                                    Balance = BalanceList[MethodObj.SendingAccount].BalanceID,
                                                    TransferID = TransferID
                                                };
                                                _context.BalanceAdjustment.Add(AdjustmentSending01);
                                                _context.SaveChanges();

                                                //Create an instance of BalanceAdjustment and Update BalanceAdjustment table for fund receiving account balance accordingly.
                                                BalanceAdjustment AdjustmentReceiving01 = new BalanceAdjustment()
                                                {
                                                    AdjustmentID = this.CreateBalanceAdjustmentID(),
                                                    AdjustedDate = DateTime.Now,
                                                    AdjustedAmount = TransferableAccountBalance,
                                                    Description = "Fund transfered from A/C: " + MethodObj.SendingAccountNumber,
                                                    AdjustBy = HttpContext.Session.GetString("UserID"),
                                                    IsReversed = 0,
                                                    ReversedBy = null,
                                                    Balance = BalanceList[MethodObj.ReceivingAccount].BalanceID,
                                                    TransferID = TransferID
                                                };
                                                _context.BalanceAdjustment.Add(AdjustmentReceiving01);
                                                _context.SaveChanges();


                                                //Update FundRequest Dictionary accordingly.
                                                FundRequestList[Request.RequestID].RequestAmount = 0;
                                                BalanceList[MethodObj.SendingAccount].OutstandingBalance = BalanceList[MethodObj.SendingAccount].OutstandingBalance - TransferableAccountBalance;
                                                BalanceList[MethodObj.ReceivingAccount].OutstandingBalance = BalanceList[MethodObj.ReceivingAccount].OutstandingBalance + TransferableAccountBalance;
                                                BalanceList[MethodObj.SendingAccount].TransferableBalance = 0;
                                                continue;

                                            case > 0:
                                                //If higher Account Balance available than Fund Request Balance.

                                                //Initiate the Transfer.

                                                //Create an Instance of Transfer and initialize the values.
                                                Transfers TransfersCase02 = new Transfers()
                                                {
                                                    TransferId = TransferID,
                                                    TransferDate = DateTime.Now,
                                                    TransferAmount = RequestBalance,
                                                    FromBankAccount = MethodObj.SendingAccount,
                                                    ToBankAccount = MethodObj.ReceivingAccount,
                                                    TransferMethod = MethodObj.MethodID,
                                                    IsApproved = 0,
                                                    ApprovedBy = null,
                                                    IsDeleted = 0,
                                                    DeletedBy = null,
                                                    AccountBalance = BalanceList[MethodObj.SendingAccount].BalanceID,
                                                    CreateBy = HttpContext.Session.GetString("UserID").ToString(),
                                                    Payment = Request.PaymentID,
                                                    IBTSheet = SheetID,
                                                };
                                                _context.Transfers.Add(TransfersCase02);
                                                _context.SaveChanges();


                                                //Create an instance of FundRequestAdjustments and initialize the values.
                                                FundRequestAdjustments AdjustmentCase04 = new FundRequestAdjustments()
                                                {
                                                    AdjustmentID = RequestAdjustmentID(),
                                                    AdjustedDate = DateTime.Now,
                                                    AdjustedAmount = RequestBalance * -1,
                                                    Description = "Funds received from: " + MethodObj.SendingAccountNumber,
                                                    AdjustBy = HttpContext.Session.GetString("UserID"),
                                                    IsReversed = 0,
                                                    ReversedBy = null,
                                                    RequestID = Request.RequestID,
                                                    TransferID = TransferID
                                                };
                                                _context.FundRequestAdjustments.Add(AdjustmentCase04);
                                                _context.SaveChanges();


                                                //Create an instance of BalanceAdjustment and Update BalanceAdjustment for sending account table accordingly.
                                                BalanceAdjustment AdjustmentSending02 = new BalanceAdjustment()
                                                {
                                                    AdjustmentID = this.CreateBalanceAdjustmentID(),
                                                    AdjustedDate = DateTime.Now,
                                                    AdjustedAmount = RequestBalance * -1,
                                                    Description = "Fund transfered to A/C: " + MethodObj.ReceivingAccountNumber,
                                                    AdjustBy = HttpContext.Session.GetString("UserID"),
                                                    IsReversed = 0,
                                                    ReversedBy = null,
                                                    Balance = BalanceList[MethodObj.SendingAccount].BalanceID,
                                                    TransferID = TransferID
                                                };
                                                _context.BalanceAdjustment.Add(AdjustmentSending02);
                                                _context.SaveChanges();


                                                //Create an instance of BalanceAdjustment and Update BalanceAdjustment for receiving account table accordingly.
                                                BalanceAdjustment AdjustmentReceiving02 = new BalanceAdjustment()
                                                {
                                                    AdjustmentID = this.CreateBalanceAdjustmentID(),
                                                    AdjustedDate = DateTime.Now,
                                                    AdjustedAmount = RequestBalance,
                                                    Description = "Fund received from A/C: " + MethodObj.SendingAccountNumber,
                                                    AdjustBy = HttpContext.Session.GetString("UserID"),
                                                    IsReversed = 0,
                                                    ReversedBy = null,
                                                    Balance = BalanceList[MethodObj.ReceivingAccount].BalanceID,
                                                    TransferID = TransferID
                                                };
                                                _context.BalanceAdjustment.Add(AdjustmentReceiving02);
                                                _context.SaveChanges();

                                                //Update BalanceList Dictionary and FundRequest Dictionary accordingly.
                                                FundRequestList[Request.RequestID].RequestAmount = 0;
                                                BalanceList[MethodObj.SendingAccount].OutstandingBalance = BalanceList[MethodObj.SendingAccount].OutstandingBalance - RequestBalance;
                                                BalanceList[MethodObj.ReceivingAccount].OutstandingBalance = BalanceList[MethodObj.ReceivingAccount].OutstandingBalance + RequestBalance;
                                                BalanceList[MethodObj.SendingAccount].TransferableBalance = DifferenceBalances;

                                                continue;

                                            case < 0:
                                                //If lower Account Balance available than Fund Request Balance.

                                                if (TransferableAccountBalance > 0)
                                                {
                                                    //Initiate the Transfer.

                                                    //Create an Instance of Transfer and initialize the values.
                                                    Transfers TransfersCase03 = new Transfers()
                                                    {
                                                        TransferId = TransferID,
                                                        TransferDate = DateTime.Now,
                                                        TransferAmount = TransferableAccountBalance,
                                                        FromBankAccount = MethodObj.SendingAccount,
                                                        ToBankAccount = MethodObj.ReceivingAccount,
                                                        TransferMethod = MethodObj.MethodID,
                                                        IsApproved = 0,
                                                        ApprovedBy = null,
                                                        IsDeleted = 0,
                                                        DeletedBy = null,
                                                        AccountBalance = BalanceList[MethodObj.SendingAccount].BalanceID,
                                                        CreateBy = HttpContext.Session.GetString("UserID").ToString(),
                                                        Payment = Request.PaymentID,
                                                        IBTSheet = SheetID,
                                                    };
                                                    _context.Transfers.Add(TransfersCase03);
                                                    _context.SaveChanges();


                                                    //Create an instance of FundRequestAdjustments and initialize the values.
                                                    FundRequestAdjustments AdjustmentCase05 = new FundRequestAdjustments()
                                                    {
                                                        AdjustmentID = RequestAdjustmentID(),
                                                        AdjustedDate = DateTime.Now,
                                                        AdjustedAmount = TransferableAccountBalance * -1,
                                                        Description = "Funds received from: " + MethodObj.SendingAccountNumber,
                                                        AdjustBy = HttpContext.Session.GetString("UserID"),
                                                        IsReversed = 0,
                                                        ReversedBy = null,
                                                        RequestID = Request.RequestID,
                                                        TransferID = TransferID
                                                    };
                                                    _context.FundRequestAdjustments.Add(AdjustmentCase05);
                                                    _context.SaveChanges();


                                                    //Create an instance of BalanceAdjustment and Update BalanceAdjustment table for sending account accordingly.
                                                    BalanceAdjustment AdjustmentSending03 = new BalanceAdjustment()
                                                    {
                                                        AdjustmentID = this.CreateBalanceAdjustmentID(),
                                                        AdjustedDate = DateTime.Now,
                                                        AdjustedAmount = TransferableAccountBalance * -1,
                                                        Description = "Fund transfered to A/C: " + MethodObj.ReceivingAccountNumber,
                                                        AdjustBy = HttpContext.Session.GetString("UserID"),
                                                        IsReversed = 0,
                                                        ReversedBy = null,
                                                        Balance = BalanceList[MethodObj.SendingAccount].BalanceID,
                                                        TransferID = TransferID
                                                    };
                                                    _context.BalanceAdjustment.Add(AdjustmentSending03);
                                                    _context.SaveChanges();


                                                    //Create an instance of BalanceAdjustment and Update BalanceAdjustment table for receiving account accordingly.
                                                    BalanceAdjustment AdjustmentReceiving03 = new BalanceAdjustment()
                                                    {
                                                        AdjustmentID = this.CreateBalanceAdjustmentID(),
                                                        AdjustedDate = DateTime.Now,
                                                        AdjustedAmount = TransferableAccountBalance,
                                                        Description = "Fund received from A/C: " + MethodObj.SendingAccountNumber,
                                                        AdjustBy = HttpContext.Session.GetString("UserID"),
                                                        IsReversed = 0,
                                                        ReversedBy = null,
                                                        Balance = BalanceList[MethodObj.ReceivingAccount].BalanceID,
                                                        TransferID = TransferID
                                                    };
                                                    _context.BalanceAdjustment.Add(AdjustmentReceiving03);
                                                    _context.SaveChanges();


                                                    //Update BalanceList Dictionary and FundRequest Dictionary accordingly.
                                                    FundRequestList[Request.RequestID].RequestAmount = DifferenceBalances * -1;
                                                    BalanceList[MethodObj.SendingAccount].OutstandingBalance = BalanceList[MethodObj.SendingAccount].OutstandingBalance - TransferableAccountBalance;
                                                    BalanceList[MethodObj.ReceivingAccount].OutstandingBalance = BalanceList[MethodObj.ReceivingAccount].OutstandingBalance + TransferableAccountBalance;
                                                    BalanceList[MethodObj.SendingAccount].TransferableBalance = 0;

                                                    continue;
                                                }
                                                else if (TransferableAccountBalance < 0)
                                                {
                                                    /*Code will not be come to this part, since it ckecks the outstanding balance of the
                                                      Sending Bank Account whether it is higher than 0 */
                                                    continue;
                                                }
                                                else
                                                {
                                                    //No code block to be run.
                                                    continue;
                                                }

                                        }

                                    }
                                    else
                                    {
                                        //No code block to be run, since the Request Amount of the Fund Request has become 0.
                                    }
                                }
                            }
                            else
                            {
                                //No Transfer Methods available under provided Option ID and Funs Request Bank Account.
                                //No code block to be run.
                            }
                        }
                        else
                        {
                            //If Fund Request is 0, no transfers need to be initiated and no code block will be run.
                        }
                    }
                }

                //Get the Excess Repo details available for the current date and Initiate Transfers from Repo Balance.
                String SqlqueryGetRepo = "SELECT Repo.RepoID, Repo.ClosedBalance, Repo.AccountID, Acc.AccountNumber AS RepoAccountNumber\r\nFROM Repo Repo\r\nLEFT JOIN BankAccount Acc ON Repo.AccountID = Acc.AccountID\r\nWHERE Repo.IsDeleted = 0 AND Repo.CreatedDate LIKE @CretedDate";
                TransferRepoFunds? RepoDetails = _context.Database.SqlQueryRaw<TransferRepoFunds>(SqlqueryGetRepo,
                    new SqlParameter("@CretedDate", DateTime.Today.Date.ToString("yyyy-MM-dd") + "%")).FirstOrDefault();

                Decimal RepoClosedBalance = RepoDetails.ClosedBalance;

                //Re-Loop each Transfer Options Dictionary under priority level.
                foreach (TransferOptionsForTransfers Option in OptionList.Values)
                {
                    /* Re-loop each Fund Requests of the Fund Request Dictionary and initiate transfers according to the priority
                       of Transfer Options. */

                    foreach (FundRequestForTransfers Request in FundRequestList.Values)
                    {
                        //Check wheher any Transfer Methods available for provided Option ID under Fund Request Account as a Receving Account.
                        if (Request.RequestAmount > 0 && RepoClosedBalance > 0)
                        {
                            //Check whether any Transfer Methods have been available for relevant Transfer Option.
                            String Sqlquery = "SELECT Mthd.MethodID, Mthd.SendingAccount, Mthd.ReceivingAccount, AccRec.AccountNumber AS ReceivingAccountNumber, AccSend.AccountNumber AS SendingAccountNumber, Mthd.TransferOption \r\nFROM TransferMethod Mthd\r\nLEFT JOIN BankAccount AccSend ON Mthd.SendingAccount = AccSend.AccountID\r\nLEFT JOIN BankAccount AccRec ON Mthd.ReceivingAccount = AccRec.AccountID WHERE SendingAccount = @AccountSend AND ReceivingAccount = @AccountReceive AND Mthd.IsDeleted = 0 AND Mthd.IsActive = 1 AND TransferOption = @Option ";
                            Dictionary<String, TransferMethodsForTransfers> MethodList = _context.Database.SqlQueryRaw<TransferMethodsForTransfers>(Sqlquery, new SqlParameter("@AccountSend", RepoDetails.AccountID),
                                new SqlParameter("@AccountReceive", Request.AccountID),
                                new SqlParameter("@Option", Option.OptionID)).ToDictionary(method => method.MethodID,
                                method => new TransferMethodsForTransfers()
                                {
                                    MethodID = method.MethodID,
                                    SendingAccount = method.SendingAccount,
                                    ReceivingAccount = method.ReceivingAccount,
                                    ReceivingAccountNumber = method.ReceivingAccountNumber,
                                    SendingAccountNumber = method.SendingAccountNumber,
                                    TransferOption = method.TransferOption
                                });

                            if (MethodList.Count > 0)
                            {
                                //If Methods available, loop each Transfer Metods and initiate Transfers under this methods.
                                //Before initiation of each Transfer, need to check Request Balance has become 0 from previous transfer.

                                foreach (TransferMethodsForTransfers MethodObj in MethodList.Values)
                                {
                                    /* Check whether the Fund Request has available request amount and the closed balance of the Repo Account
                                       higher than 0 */

                                    if (Request.RequestAmount > 0 && RepoClosedBalance > 0)
                                    {
                                        //Initiate the Transfer from Repo Bank Account.

                                        Decimal RequestBalance = Request.RequestAmount;
                                        Decimal DifferenceBalance = RepoClosedBalance - RequestBalance;

                                        //Call back CreateTransferID method to create a new Transfer ID.

                                        String TransferID = this.CreateTransferID();

                                        switch (DifferenceBalance)
                                        {
                                            case 0:
                                                //If Repo Closed Balance and Fund Request Balance is eaquel.

                                                //Initiate the Transfer.

                                                //Create an Instance of Transfer and initialize the values.
                                                Transfers TransfersCase01 = new Transfers()
                                                {
                                                    TransferId = TransferID,
                                                    TransferDate = DateTime.Now,
                                                    TransferAmount = RepoClosedBalance,
                                                    FromBankAccount = RepoDetails.AccountID,
                                                    ToBankAccount = MethodObj.ReceivingAccount,
                                                    TransferMethod = MethodObj.MethodID,
                                                    IsApproved = 0,
                                                    ApprovedBy = null,
                                                    IsDeleted = 0,
                                                    DeletedBy = null,
                                                    AccountBalance = null,
                                                    CreateBy = HttpContext.Session.GetString("UserID").ToString(),
                                                    Payment = Request.PaymentID,
                                                    IBTSheet = SheetID,
                                                };
                                                _context.Transfers.Add(TransfersCase01);
                                                _context.SaveChanges();


                                                //Create an instance of FundRequestAdjustments and initialize the values.
                                                FundRequestAdjustments AdjustmentCase06 = new FundRequestAdjustments()
                                                {
                                                    AdjustmentID = RequestAdjustmentID(),
                                                    AdjustedDate = DateTime.Now,
                                                    AdjustedAmount = RepoClosedBalance * -1,
                                                    Description = "Funds received from: " + MethodObj.SendingAccountNumber,
                                                    AdjustBy = HttpContext.Session.GetString("UserID"),
                                                    IsReversed = 0,
                                                    ReversedBy = null,
                                                    RequestID = Request.RequestID,
                                                    TransferID = TransferID
                                                };
                                                _context.FundRequestAdjustments.Add(AdjustmentCase06);
                                                _context.SaveChanges();


                                                //Create an instance of RepoBalanceAdjustment and Update RepoBalanceAdjustment table accordingly.
                                                RepoBalanceAdjustment AdjustmentSending01 = new RepoBalanceAdjustment()
                                                {
                                                    AdjustmentID = this.CreateRepoBalanceAdjustmentID(),
                                                    AdjustedDate = DateTime.Now,
                                                    AdjustedAmount = RepoClosedBalance * -1,
                                                    Description = "Fund transfered to A/C: " + MethodObj.ReceivingAccountNumber,
                                                    AdjustBy = HttpContext.Session.GetString("UserID"),
                                                    IsReversed = 0,
                                                    ReversedBy = null,
                                                    Repo = RepoDetails.RepoID,
                                                    TransferID = TransferID
                                                };
                                                _context.RepoBalanceAdjustment.Add(AdjustmentSending01);
                                                _context.SaveChanges();

                                                //Create an instance of BalanceAdjustment and Update BalanceAdjustment table for receiving account accordingly.
                                                BalanceAdjustment AdjustmentReceiving01 = new BalanceAdjustment()
                                                {
                                                    AdjustmentID = this.CreateBalanceAdjustmentID(),
                                                    AdjustedDate = DateTime.Now,
                                                    AdjustedAmount = RepoClosedBalance,
                                                    Description = "Fund received from A/C: " + MethodObj.SendingAccountNumber + " [Repo Balance]",
                                                    AdjustBy = HttpContext.Session.GetString("UserID"),
                                                    IsReversed = 0,
                                                    ReversedBy = null,
                                                    Balance = BalanceList[MethodObj.ReceivingAccount].BalanceID,
                                                    TransferID = TransferID
                                                };
                                                _context.BalanceAdjustment.Add(AdjustmentReceiving01);
                                                _context.SaveChanges();

                                                //Update Repo Closed Balance and FundRequest Dictionary accordingly.
                                                
                                                FundRequestList[Request.RequestID].RequestAmount = 0;
                                                RepoClosedBalance = 0;
                                                BalanceList[MethodObj.ReceivingAccount].OutstandingBalance = BalanceList[MethodObj.ReceivingAccount].OutstandingBalance + RepoClosedBalance;

                                                continue;

                                            case > 0:
                                                //If Repo Closed Balance is higher than Fund Request Balance.

                                                //Initiate the Transfer.

                                                //Create an Instance of Transfer and initialize the values.
                                                Transfers TransfersCase02 = new Transfers()
                                                {
                                                    TransferId = TransferID,
                                                    TransferDate = DateTime.Now,
                                                    TransferAmount = RequestBalance,
                                                    FromBankAccount = RepoDetails.AccountID,
                                                    ToBankAccount = MethodObj.ReceivingAccount,
                                                    TransferMethod = MethodObj.MethodID,
                                                    IsApproved = 0,
                                                    ApprovedBy = null,
                                                    IsDeleted = 0,
                                                    DeletedBy = null,
                                                    AccountBalance = null,
                                                    CreateBy = HttpContext.Session.GetString("UserID").ToString(),
                                                    Payment = Request.PaymentID,
                                                    IBTSheet = SheetID,
                                                };
                                                _context.Transfers.Add(TransfersCase02);
                                                _context.SaveChanges();


                                                //Create an instance of FundRequestAdjustments and initialize the values.
                                                FundRequestAdjustments AdjustmentCase07 = new FundRequestAdjustments()
                                                {
                                                    AdjustmentID = RequestAdjustmentID(),
                                                    AdjustedDate = DateTime.Now,
                                                    AdjustedAmount = RequestBalance * -1,
                                                    Description = "Funds received from: " + MethodObj.SendingAccountNumber + " [Repo Balance]",
                                                    AdjustBy = HttpContext.Session.GetString("UserID"),
                                                    IsReversed = 0,
                                                    ReversedBy = null,
                                                    RequestID = Request.RequestID,
                                                    TransferID = TransferID
                                                };
                                                _context.FundRequestAdjustments.Add(AdjustmentCase07);
                                                _context.SaveChanges();


                                                //Create an instance of RepoBalanceAdjustment and Update RepoBalanceAdjustment table accordingly.
                                                RepoBalanceAdjustment AdjustmentSending02 = new RepoBalanceAdjustment()
                                                {
                                                    AdjustmentID = this.CreateRepoBalanceAdjustmentID(),
                                                    AdjustedDate = DateTime.Now,
                                                    AdjustedAmount = RequestBalance * -1,
                                                    Description = "Fund transfered to A/C: " + MethodObj.ReceivingAccountNumber,
                                                    AdjustBy = HttpContext.Session.GetString("UserID"),
                                                    IsReversed = 0,
                                                    ReversedBy = null,
                                                    Repo = RepoDetails.RepoID,
                                                    TransferID = TransferID
                                                };
                                                _context.RepoBalanceAdjustment.Add(AdjustmentSending02);
                                                _context.SaveChanges();


                                                //Create an instance of BalanceAdjustment and Update BalanceAdjustment table for receiving account accordingly.
                                                BalanceAdjustment AdjustmentReceiving02 = new BalanceAdjustment()
                                                {
                                                    AdjustmentID = this.CreateBalanceAdjustmentID(),
                                                    AdjustedDate = DateTime.Now,
                                                    AdjustedAmount = RequestBalance,
                                                    Description = "Fund received from A/C: " + MethodObj.SendingAccountNumber + " [Repo Balance]",
                                                    AdjustBy = HttpContext.Session.GetString("UserID"),
                                                    IsReversed = 0,
                                                    ReversedBy = null,
                                                    Balance = BalanceList[MethodObj.ReceivingAccount].BalanceID,
                                                    TransferID = TransferID
                                                };
                                                _context.BalanceAdjustment.Add(AdjustmentReceiving02);
                                                _context.SaveChanges();

                                                //Update Repo closed Balance and FundRequest Dictionary accordingly.
                                                RepoClosedBalance = DifferenceBalance;
                                                FundRequestList[Request.RequestID].RequestAmount = 0;
                                                BalanceList[MethodObj.ReceivingAccount].OutstandingBalance = BalanceList[MethodObj.ReceivingAccount].OutstandingBalance + RequestBalance;

                                                continue;

                                            case < 0:

                                                if (RepoClosedBalance > 0)
                                                {
                                                    //If Repo Closed Balance is lower than Fund Request Balance and Repo closed balances is higher than 0.

                                                    //Initiate the Transfer.

                                                    //Create an Instance of Transfer and initialize the values.
                                                    Transfers TransfersCase03 = new Transfers()
                                                    {
                                                        TransferId = TransferID,
                                                        TransferDate = DateTime.Now,
                                                        TransferAmount = RepoClosedBalance,
                                                        FromBankAccount = RepoDetails.AccountID,
                                                        ToBankAccount = MethodObj.ReceivingAccount,
                                                        TransferMethod = MethodObj.MethodID,
                                                        IsApproved = 0,
                                                        ApprovedBy = null,
                                                        IsDeleted = 0,
                                                        DeletedBy = null,
                                                        AccountBalance = null,
                                                        CreateBy = HttpContext.Session.GetString("UserID").ToString(),
                                                        Payment = Request.PaymentID,
                                                        IBTSheet = SheetID,
                                                    };
                                                    _context.Transfers.Add(TransfersCase03);
                                                    _context.SaveChanges();


                                                    //Create an instance of FundRequestAdjustments and initialize the values.
                                                    FundRequestAdjustments AdjustmentCase08 = new FundRequestAdjustments()
                                                    {
                                                        AdjustmentID = RequestAdjustmentID(),
                                                        AdjustedDate = DateTime.Now,
                                                        AdjustedAmount = RepoClosedBalance * -1,
                                                        Description = "Funds received from: " + MethodObj.SendingAccountNumber,
                                                        AdjustBy = HttpContext.Session.GetString("UserID"),
                                                        IsReversed = 0,
                                                        ReversedBy = null,
                                                        RequestID = Request.RequestID,
                                                        TransferID = TransferID
                                                    };
                                                    _context.FundRequestAdjustments.Add(AdjustmentCase08);
                                                    _context.SaveChanges();


                                                    //Create an instance of RepoBalanceAdjustment and Update RepoBalanceAdjustment table accordingly.
                                                    RepoBalanceAdjustment AdjustmentSending03 = new RepoBalanceAdjustment()
                                                    {
                                                        AdjustmentID = this.CreateRepoBalanceAdjustmentID(),
                                                        AdjustedDate = DateTime.Now,
                                                        AdjustedAmount = RepoClosedBalance * -1,
                                                        Description = "Fund transfered to A/C: " + MethodObj.ReceivingAccountNumber,
                                                        AdjustBy = HttpContext.Session.GetString("UserID"),
                                                        IsReversed = 0,
                                                        ReversedBy = null,
                                                        Repo = RepoDetails.RepoID,
                                                        TransferID = TransferID

                                                    };

                                                    _context.RepoBalanceAdjustment.Add(AdjustmentSending03);
                                                    _context.SaveChanges();


                                                    //Create an instance of BalanceAdjustment and Update BalanceAdjustment table for receiving account accordingly.
                                                    BalanceAdjustment AdjustmentReceiving03 = new BalanceAdjustment()
                                                    {
                                                        AdjustmentID = this.CreateBalanceAdjustmentID(),
                                                        AdjustedDate = DateTime.Now,
                                                        AdjustedAmount = RepoClosedBalance,
                                                        Description = "Fund received from A/C: " + MethodObj.SendingAccountNumber + " [Repo Balance]",
                                                        AdjustBy = HttpContext.Session.GetString("UserID"),
                                                        IsReversed = 0,
                                                        ReversedBy = null,
                                                        Balance = BalanceList[MethodObj.ReceivingAccount].BalanceID,
                                                        TransferID = TransferID
                                                    };
                                                    _context.BalanceAdjustment.Add(AdjustmentReceiving03);
                                                    _context.SaveChanges();


                                                    //Update BalanceList Dictionary and FundRequest Dictionary accordingly.
                                                    FundRequestList[Request.RequestID].RequestAmount = DifferenceBalance * -1;
                                                    BalanceList[MethodObj.ReceivingAccount].OutstandingBalance = BalanceList[MethodObj.ReceivingAccount].OutstandingBalance + RepoClosedBalance;
                                                    RepoClosedBalance = 0;

                                                    continue;
                                                }
                                                else if (RepoClosedBalance < 0)
                                                {
                                                    /* Code will not be come to this part, since the ClosedBalance of the Repo checks always
                                                       whether it is higher than 0 */
                                                    continue;
                                                }
                                                else
                                                {
                                                    //No code block to be run.
                                                    continue;
                                                }

                                        }

                                    }
                                    else
                                    {
                                        //No code block to be run.
                                    }
                                }
                            }
                            else
                            {
                                //No code block to be run.
                            }
                        }
                        else
                        {
                            //No code block to be run.
                        }
                    }
                }


                foreach (TransferOptionsForTransfers Option in OptionList.Values)
                {
                    /*Loop each Account Balances in Balance List Dictionary and transfer the excess balances available
                      in the Bank Accounts to the Repo Account for investments.*/

                    foreach (AccountBalanceForTransfers BalanceObj in BalanceList.Values)
                    {
                        
                        if (BalanceObj.TransferableBalance > 0)
                        {
                            //Check whether any Transfer Methods have been available for relevant Transfer Option.
                            String Sqlquery = "SELECT Mthd.MethodID, Mthd.SendingAccount, Mthd.ReceivingAccount, AccRec.AccountNumber AS ReceivingAccountNumber, AccSend.AccountNumber AS SendingAccountNumber, Mthd.TransferOption \r\nFROM TransferMethod Mthd\r\nLEFT JOIN BankAccount AccSend ON Mthd.SendingAccount = AccSend.AccountID\r\nLEFT JOIN BankAccount AccRec ON Mthd.ReceivingAccount = AccRec.AccountID WHERE SendingAccount = @AccountSend AND ReceivingAccount = @AccountReceive AND Mthd.IsDeleted = 0 AND Mthd.IsActive = 1 AND TransferOption = @Option ";
                            Dictionary<String, TransferMethodsForTransfers> MethodList = _context.Database.SqlQueryRaw<TransferMethodsForTransfers>(Sqlquery, new SqlParameter("@AccountSend", BalanceObj.AccountID),
                                new SqlParameter("@AccountReceive", RepoDetails.AccountID),
                                new SqlParameter("@Option", Option.OptionID)).ToDictionary(method => method.MethodID,
                                method => new TransferMethodsForTransfers()
                                {
                                    MethodID = method.MethodID,
                                    SendingAccount = method.SendingAccount,
                                    ReceivingAccount = method.ReceivingAccount,
                                    ReceivingAccountNumber = method.ReceivingAccountNumber,
                                    SendingAccountNumber = method.SendingAccountNumber,
                                    TransferOption = method.TransferOption
                                });

                            if (MethodList.Count > 0)
                            {
                                foreach (TransferMethodsForTransfers MethodObj in MethodList.Values)
                                {
                                    String TransferID = this.CreateTransferID();

                                    //Initiate the Transfer.

                                    //Create an Instance of Transfer and initialize the values.
                                    Transfers TransfersCase03 = new Transfers()
                                    {
                                        TransferId = TransferID,
                                        TransferDate = DateTime.Now,
                                        TransferAmount = BalanceObj.TransferableBalance,
                                        FromBankAccount = BalanceObj.AccountID,
                                        ToBankAccount = RepoDetails.AccountID,
                                        TransferMethod = MethodObj.MethodID,
                                        IsApproved = 0,
                                        ApprovedBy = null,
                                        IsDeleted = 0,
                                        DeletedBy = null,
                                        AccountBalance = BalanceObj.BalanceID,
                                        CreateBy = HttpContext.Session.GetString("UserID").ToString(),
                                        Payment = null,
                                        IBTSheet = SheetID,
                                    };
                                    _context.Transfers.Add(TransfersCase03);
                                    _context.SaveChanges();

                                    
                                    //Create an instance of BalanceAdjustment table accordingly.
                                    BalanceAdjustment AdjustmentSending01 = new BalanceAdjustment()
                                    {
                                        AdjustmentID = this.CreateBalanceAdjustmentID(),
                                        AdjustedDate = DateTime.Now,
                                        AdjustedAmount = BalanceObj.TransferableBalance * -1,
                                        Description = "Fund transfered to A/C: " + MethodObj.ReceivingAccountNumber,
                                        AdjustBy = HttpContext.Session.GetString("UserID"),
                                        IsReversed = 0,
                                        ReversedBy = null,
                                        Balance = BalanceObj.BalanceID,
                                        TransferID = TransferID
                                    };

                                    _context.BalanceAdjustment.Add(AdjustmentSending01);
                                    _context.SaveChanges();


                                    //Create an instance of RepoBalanceAdjustment table accordingly.
                                    RepoBalanceAdjustment AdjustmentReceiving01 = new RepoBalanceAdjustment()
                                    {
                                        AdjustmentID = this.CreateRepoBalanceAdjustmentID(),
                                        AdjustedDate = DateTime.Now,
                                        AdjustedAmount = BalanceObj.TransferableBalance,
                                        Description = "Fund received from A/C: " + MethodObj.SendingAccountNumber,
                                        AdjustBy = HttpContext.Session.GetString("UserID"),
                                        IsReversed = 0,
                                        ReversedBy = null,
                                        Repo = RepoDetails.RepoID,
                                        TransferID = TransferID
                                    };
                                    _context.RepoBalanceAdjustment.Add(AdjustmentReceiving01);
                                    _context.SaveChanges();

                                    //Update BalanceList Dictionary and Closed Balance of the RepoDetails accordingly.
                                    RepoClosedBalance = RepoClosedBalance + BalanceObj.TransferableBalance;
                                    BalanceObj.OutstandingBalance = BalanceObj.OutstandingBalance - BalanceObj.TransferableBalance;
                                    BalanceObj.TransferableBalance = BalanceObj.TransferableBalance - BalanceObj.TransferableBalance;
                                                                       
                                    
                                }
                            }
                            else
                            {
                                //No code block to be run.
                            }

                        }
                        else
                        {
                            //No code block to be run.
                        }

                    }
                }

                //Update Outstanding balances of each Account Balance.
                foreach (AccountBalanceForTransfers UpdateBalanceObj in BalanceList.Values)
                {
                    String SqlqueryUpdateOutstandingBalance = "UPDATE AccountBalance SET OutStandingBalance = @Balance WHERE BalanceID = @BalanceID";
                    _context.Database.ExecuteSqlRaw(SqlqueryUpdateOutstandingBalance, new SqlParameter("@Balance", UpdateBalanceObj.OutstandingBalance),
                        new SqlParameter("BalanceID", UpdateBalanceObj.BalanceID));
                }

                //Update RepoClosed Balance.
                String SqlqueryUpdateRepoClosedBalance = "UPDATE Repo SET ClosedBalance = @Balance WHERE RepoID = @RepoID";
                _context.Database.ExecuteSqlRaw(SqlqueryUpdateRepoClosedBalance, new SqlParameter("@Balance", RepoClosedBalance),
                    new SqlParameter("RepoID", RepoDetails.RepoID));

                //Update Remaining Request Amount of each Fund Request.
                foreach (FundRequestForTransfers RequestObj in FundRequestList.Values)
                {
                    String SqlQueryUpdateRemainingRequest = "UPDATE FundRequest SET RemainingRequestAmount = @RequestAmount WHERE RequestID = @RequestID";
                    _context.Database.ExecuteSqlRaw(SqlQueryUpdateRemainingRequest, new SqlParameter("@RequestAmount", RequestObj.RequestAmount),
                        new SqlParameter("@RequestID", RequestObj.RequestID));
                }


                TempData["message"] = "Success: Transfers initiated successfully!";
                return RedirectToAction("AutomatedTransfers", "Transfer");
            }
            else
            {
                //No Excess Repo is available.
                TempData["message"] = "Information: No Excess Repo is created for current date. Please create an Excess Repo before initiate the transfers!";
                return RedirectToAction("AutomatedTransfers", "Transfer");
            }

        }

        public IActionResult TransfersDetails()
        {
            return View();
        }

        public IActionResult DisplayTransfers(DateTime TransferDate)
        {
            if (ModelState.IsValid)
            {
                String Sqlquery = "SELECT DISTINCT Tfr.TransferID, \r\nTfr.TransferDate,\r\nAccFrom.AccountNumber AS FromAccount,\r\nAccTo.AccountNumber AS ToAccount,\r\nTfr.TransferAmount,\r\nOptn.OptionType AS TransferOption,\r\nCASE WHEN IsApproved = 0 THEN 'Pending'\r\nELSE 'Approved' END AS ApproveStatus,\r\nCASE WHEN Tfr.ApprovedBy IS NULL THEN 'N/A'\r\nELSE UsrApprove.FirstName END AS ApprovedBy,\r\nCASE WHEN Tfr.IsDeleted = 0 THEN 'Not-Delete'\r\nELSE 'Deleted' END AS DeleteStatus,\r\nCASE WHEN Tfr.DeletedBy IS NULL THEN 'N/A'\r\nELSE UsrDelete.FirstName END AS DeleteBy,\r\nCASE WHEN Tfr.AccountBalance IS NULL THEN 'Repo Balance'\r\nELSE Tfr.AccountBalance END AS Balance,\r\nCASE WHEN Tfr.Payment IS NULL THEN 'Excess Repo'\r\nELSE Pmnt.PaymentType END AS PaymentType,\r\nUsrCreate.FirstName AS InitiateBy\r\nFROM Transfers Tfr\r\nLEFT JOIN BankAccount AccFrom ON Tfr.FromBankAccount = AccFrom.AccountID\r\nLEFT JOIN BankAccount AccTo ON Tfr.ToBankAccount = AccTo.AccountID\r\nLEFT JOIN Users UsrApprove ON Tfr.ApprovedBy = UsrApprove.User_Id\r\nLEFT JOIN Users UsrDelete ON Tfr.DeletedBy = UsrDelete.User_Id\r\nLEFT JOIN Payment Pmnt ON Tfr.Payment = Pmnt.PaymentID\r\nLEFT JOIN Users UsrCreate ON Tfr.CreateBy = UsrCreate.User_Id\r\nLEFT JOIN TransferMethod Mthd ON Tfr.TransferMethod = Mthd.MethodID\r\nLEFT JOIN TransferOption Optn ON Mthd.TransferOption = Optn.OptionID\r\nWHERE Tfr.TransferDate LIKE @TransferDate ORDER BY Tfr.TransferDate";
                List<DisplayTransfers> TransferList = _context.Database.SqlQueryRaw<DisplayTransfers>(Sqlquery,
                    new SqlParameter("@TransferDate", TransferDate.Date.ToString("yyyy-MM-dd") + "%")).ToList();

                if (TransferList.Count > 0)
                {
                    return View(TransferList);
                }
                else
                {
                    TempData["message"] = "Information: No transfers available for selected date!";
                    return RedirectToAction("TransfersDetails", "Transfer");
                }

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("TransfersDetails", "Transfer");
            }

        }


        public IActionResult ReverseIndividualTransfer()
        {
            return View();
        }

        public IActionResult DisplayIndividualTransfer(String TransferID)
        {
            if (ModelState.IsValid)
            {
                String Sqlquery = "SELECT Tfr.TransferDate, Tfr.TransferID, AccFrom.AccountNumber AS FromAccount, AccTo.AccountNumber AS ToAccount, Tfr.TransferAmount, Tfr.IsDeleted, Tfr.IsApproved\r\nFROM Transfers Tfr\r\nLEFT JOIN BankAccount AccFrom ON Tfr.FromBankAccount = AccFrom.AccountID\r\nLEFT JOIN BankAccount AccTo ON Tfr.ToBankAccount = AccTo.AccountID WHERE Tfr.TransferID = @TransferID";
                DisplayIndividualTransferForReverse? ReverseObj = _context.Database.SqlQueryRaw<DisplayIndividualTransferForReverse>(Sqlquery, new SqlParameter("@TransferID", TransferID)).FirstOrDefault();

                //Check whether any Transfer record is available in the Transfer Table.
                if (ReverseObj != null)
                {
                    //Check whether the record is already reversed or not.
                    if (ReverseObj.IsDeleted == 0)
                    {
                        //Check whether the record is already approved or not.
                        if (ReverseObj.IsApproved == 0)
                        {
                            //Check whether the record is previous day record.
                            if (ReverseObj.TransferDate.Date == DateTime.Today.Date)
                            {
                                return View(ReverseObj);
                            }
                            else
                            {
                                TempData["message"] = "Information: You have no authority for delete previous day transfer record!";
                                return RedirectToAction("ReverseIndividualTransfer", "Transfer");
                            }
                            
                        }
                        else
                        {
                            TempData["message"] = "Information: This record is already approved. No authority for delete!";
                            return RedirectToAction("ReverseIndividualTransfer", "Transfer");
                        }
                    }
                    else
                    {
                        TempData["message"] = "Information: This Transfer record is no longer available!";
                        return RedirectToAction("ReverseIndividualTransfer", "Transfer");
                    }
                }
                else
                {
                    TempData["message"] = "Information: No Transfer record is available for provided Transfer ID!";
                    return RedirectToAction("ReverseIndividualTransfer", "Transfer");
                }
                                
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("ReverseIndividualTransfer", "Transfer");
            }
            
        }

        public IActionResult SaveDeleteTransfer(String TransferID)
        {
            if (ModelState.IsValid)
            {
                //Update Transfers Table as Deleted accordingly.
                String SqlqueryReverseTransferIndividual = "UPDATE Transfers SET IsDeleted = 1, DeletedBy = @DeleteBy WHERE TransferID = @ID";
                _context.Database.ExecuteSqlRaw(SqlqueryReverseTransferIndividual, new SqlParameter("@DeleteBy", HttpContext.Session.GetString("UserID")),
                    new SqlParameter("ID", TransferID));


                //Check whether any adjustments have been impact for Account Balances and if any, reverse those adjustments as well.
                String SqlqueryReverseBalanceAdjIndividual = "SELECT Adj.AdjustmentID, Adj.AdjustedAmount, Bal.BalanceID\r\nFROM BalanceAdjustment Adj\r\nLEFT JOIN AccountBalance Bal ON Adj.Balance = Bal.BalanceID\r\nWHERE TransferID = @TransferID ";
                List<ReverseBalanceAdjIndividual> AdjustmentList = _context.Database.SqlQueryRaw<ReverseBalanceAdjIndividual>(SqlqueryReverseBalanceAdjIndividual, new SqlParameter("@TransferID", TransferID) ).ToList();
                if (AdjustmentList.Count > 0)
                {
                    foreach (ReverseBalanceAdjIndividual AdjustmentObj in AdjustmentList)
                    {
                        //Check whether the Adjustment amount of the AdjustmentObj is less than 0 or higher than 0.
                        switch (AdjustmentObj.AdjustedAmount)
                        {
                            case < 0:
                                Decimal ExistingBalanceCase01 = _context.AccountBalance.Find(AdjustmentObj.BalanceID).OutstandingBalance;
                                ExistingBalanceCase01 += AdjustmentObj.AdjustedAmount * -1;
                                String UpdateAccountBalanceCase01 = "UPDATE AccountBalance SET OutstandingBalance = @OutstandingBalance WHERE BalanceID = @BalanceID";
                                _context.Database.ExecuteSqlRaw(UpdateAccountBalanceCase01, new SqlParameter("@OutstandingBalance", ExistingBalanceCase01),
                                    new SqlParameter("@BalanceID", AdjustmentObj.BalanceID));
                                break;

                            case > 0:
                                Decimal ExistingBalanceCase02 = _context.AccountBalance.Find(AdjustmentObj.BalanceID).OutstandingBalance;
                                ExistingBalanceCase02 -= AdjustmentObj.AdjustedAmount;
                                String UpdateAccountBalanceCase02 = "UPDATE AccountBalance SET OutstandingBalance = @OutstandingBalance WHERE BalanceID = @BalanceID";
                                _context.Database.ExecuteSqlRaw(UpdateAccountBalanceCase02, new SqlParameter("@OutstandingBalance", ExistingBalanceCase02),
                                    new SqlParameter("@BalanceID", AdjustmentObj.BalanceID));
                                break;

                        }

                        //Mark the relevant Adjustment ID as Reversed Adjustment.
                        String SqlqueryReverseAdjustmentID = "UPDATE BalanceAdjustment SET IsReversed = 1, ReversedBy = @ReversedBy WHERE AdjustmentID = @AdjID ";
                        _context.Database.ExecuteSqlRaw(SqlqueryReverseAdjustmentID, new SqlParameter("@ReversedBy", HttpContext.Session.GetString("UserID")),
                            new SqlParameter("@AdjID", AdjustmentObj.AdjustmentID));
                        
                        
                    }
                }
                else
                {
                    //No code block to be run here.
                }


                //Check whether any adjustments have been impact for Repo Balances and if any, reverse those adjustments as well.
                String SqlqueryReverseRepoBalanceAdjIndividual = "SELECT Adj.AdjustmentID, Adj.AdjustedAmount, Repo.RepoID\r\nFROM RepoBalanceAdjustment Adj\r\nLEFT JOIN Repo Repo ON Adj.Repo = Repo.RepoID\r\nWHERE TransferID = @TransferID";
                List<ReverseRepoBalanceAdjIndividual> RepoAdjustmentList = _context.Database.SqlQueryRaw<ReverseRepoBalanceAdjIndividual>(SqlqueryReverseRepoBalanceAdjIndividual, new SqlParameter("@TransferID",TransferID) ).ToList();
                if (AdjustmentList.Count > 0)
                {
                    foreach (ReverseRepoBalanceAdjIndividual RepoAdjustmentObj in RepoAdjustmentList)
                    {
                        //Check whether the Adjustment amount of the AdjustmentObj is less than 0 or higher than 0.
                        switch (RepoAdjustmentObj.AdjustedAmount)
                        {
                            case < 0:
                                Decimal ExistingBalanceCase01 = _context.Repo.Find(RepoAdjustmentObj.RepoID).ClosedBalance;
                                ExistingBalanceCase01 += RepoAdjustmentObj.AdjustedAmount * -1;
                                String UpdateAccountBalanceCase01 = "UPDATE Repo SET ClosedBalance = @ClosedBalance WHERE RepoID = @RepoID";
                                _context.Database.ExecuteSqlRaw(UpdateAccountBalanceCase01, new SqlParameter("@ClosedBalance", ExistingBalanceCase01),
                                    new SqlParameter("@RepoID", RepoAdjustmentObj.RepoID));
                                break;

                            case > 0:
                                Decimal ExistingBalanceCase02 = _context.Repo.Find(RepoAdjustmentObj.RepoID).ClosedBalance;
                                ExistingBalanceCase02 -= RepoAdjustmentObj.AdjustedAmount;
                                String UpdateAccountBalanceCase02 = "UPDATE Repo SET ClosedBalance = @ClosedBalance WHERE RepoID = @RepoID";
                                _context.Database.ExecuteSqlRaw(UpdateAccountBalanceCase02, new SqlParameter("@ClosedBalance", ExistingBalanceCase02),
                                    new SqlParameter("@RepoID", RepoAdjustmentObj.RepoID));
                                break;

                        }

                        //Mark the relevant Repo Adjustment ID as Reversed Adjustment.
                        String SqlqueryReverseRepoAdjustmentID = "UPDATE RepoBalanceAdjustment SET IsReversed = 1, ReversedBy = @ReversedBy WHERE AdjustmentID = @AdjID ";
                        _context.Database.ExecuteSqlRaw(SqlqueryReverseRepoAdjustmentID, new SqlParameter("@ReversedBy", HttpContext.Session.GetString("UserID")),
                            new SqlParameter("@AdjID", RepoAdjustmentObj.AdjustmentID));

                    }
                }
                else
                {
                    //No code block to be run here.
                }


                //Check whether any adjustments have been impact for FundRequest and if any, reverse those adjustments as well.
                String SqlqueryReverseRequestAdjIndividual = "SELECT Adj.AdjustmentID, Adj.AdjustedAmount, Req.RequestID\r\nFROM FundRequestAdjustments Adj\r\nLEFT JOIN FundRequest Req ON Adj.RequestID = Req.RequestID\r\nWHERE TransferID = @TransferID";
                List<ReverseRequestAdjIndividual> RequestAdjustmentList = _context.Database.SqlQueryRaw<ReverseRequestAdjIndividual>(SqlqueryReverseRequestAdjIndividual, new SqlParameter("@TransferID", TransferID)).ToList();
                if (RequestAdjustmentList.Count > 0)
                {
                    foreach (ReverseRequestAdjIndividual RequestAdjustmentObj in RequestAdjustmentList)
                    {
                        //Check whether the Adjustment amount of the AdjustmentObj is less than 0 or higher than 0.
                        switch (RequestAdjustmentObj.AdjustedAmount)
                        {
                            case < 0:
                                Decimal ExistingBalanceCase01 = _context.FundRequest.Find(RequestAdjustmentObj.RequestID).RemainingRequestAmount;
                                ExistingBalanceCase01 += RequestAdjustmentObj.AdjustedAmount * -1;
                                String UpdateAccountBalanceCase01 = "UPDATE FundRequest SET RemainingRequestAmount = @RequestAmount WHERE RequestID = @RequestID";
                                _context.Database.ExecuteSqlRaw(UpdateAccountBalanceCase01, new SqlParameter("@RequestAmount", ExistingBalanceCase01),
                                    new SqlParameter("@RequestID", RequestAdjustmentObj.RequestID));
                                break;

                            case > 0:
                                Decimal ExistingBalanceCase02 = _context.FundRequest.Find(RequestAdjustmentObj.RequestID).RemainingRequestAmount;
                                ExistingBalanceCase02 -= RequestAdjustmentObj.AdjustedAmount;
                                String UpdateAccountBalanceCase02 = "UPDATE FundRequest SET RemainingRequestAmount = @RequestAmount WHERE RequestID = @RequestID";
                                _context.Database.ExecuteSqlRaw(UpdateAccountBalanceCase02, new SqlParameter("@RequestAmount", ExistingBalanceCase02),
                                    new SqlParameter("@RequestID", RequestAdjustmentObj.RequestID));
                                break;

                        }

                        //Mark the relevant Repo Adjustment ID as Reversed Adjustment.
                        String SqlqueryReverseRequestAdjustmentID = "UPDATE FundRequestAdjustments SET IsReversed = 1, ReversedBy = @ReversedBy WHERE AdjustmentID = @AdjID ";
                        _context.Database.ExecuteSqlRaw(SqlqueryReverseRequestAdjustmentID, new SqlParameter("@ReversedBy", HttpContext.Session.GetString("UserID")),
                            new SqlParameter("@AdjID", RequestAdjustmentObj.AdjustmentID));

                    }
                }
                else
                {
                    //No code block to be run here.
                }

                TempData["message"] = "Success: Transaction reversed successfully!";
                return RedirectToAction("ReverseIndividualTransfer", "Transfer");

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("ReverseIndividualTransfer", "Transfer");
            }
        }




        public IActionResult ReverseBulkTransfer()
        {
            return View();
        }

        public IActionResult DisplayBulkTransfer(String SheetID)
        {
            if (ModelState.IsValid)
            {
                //Get all Transfer data related to provided IBT Sheet.
                String Sqlquery = "SELECT COUNT(TransferID) AS TotalCount,\r\nIBTSheet,\r\nSUM(TransferAmount) AS TotalTransferAmount, \r\nCOUNT(CASE WHEN IsDeleted = 0 THEN 0 END) AS NotDeletedCount,\r\nCOUNT(CASE WHEN IsDeleted = 1 THEN 1 END) AS DeletedCount,\r\nCOUNT(CASE WHEN IsApproved = 0 THEN 0 END) AS NotApprovedCount,\r\nCOUNT(CASE WHEN IsApproved = 1 THEN 1 END) AS ApprovedCount\r\nFROM Transfers WHERE IBTSheet = @Sheet \r\nGROUP BY IBTSheet";
                DisplayBulkTransfer? BulkObj = _context.Database.SqlQueryRaw<DisplayBulkTransfer>(Sqlquery, new SqlParameter("@Sheet", SheetID)).FirstOrDefault();
                return View(BulkObj);
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("ReverseBulkTransfer", "Transfer");
            }

        }
        
        public IActionResult SaveDeleteBulkTransfer(String SheetID, int ApprovedRecords)
        {
            if (ModelState.IsValid)
            {
                if (ApprovedRecords == 0)
                {
                    String SqlQueryBulkReverse = "SELECT TransferID FROM Transfers WHERE IsApproved = 0 AND IsDeleted = 0 AND IBTSheet = @SheetID ";
                    List<String> BulkList = _context.Database.SqlQueryRaw<String>(SqlQueryBulkReverse, new SqlParameter("@SheetID", SheetID)).ToList();
                    if (BulkList.Count > 0)
                    {
                        foreach (String ReverseObj in BulkList)
                        {
                            //Update Transfers Table as Deleted accordingly.
                            String SqlqueryReverseTransferIndividual = "UPDATE Transfers SET IsDeleted = 1, DeletedBy = @DeleteBy WHERE TransferID = @ID";
                            _context.Database.ExecuteSqlRaw(SqlqueryReverseTransferIndividual, new SqlParameter("@DeleteBy", HttpContext.Session.GetString("UserID")),
                                new SqlParameter("ID", ReverseObj));


                            //Check whether any adjustments have been impact for Account Balances and if any, reverse those adjustments as well.
                            String SqlqueryReverseBalanceAdjIndividual = "SELECT Adj.AdjustmentID, Adj.AdjustedAmount, Bal.BalanceID\r\nFROM BalanceAdjustment Adj\r\nLEFT JOIN AccountBalance Bal ON Adj.Balance = Bal.BalanceID\r\nWHERE TransferID = @TransferID ";
                            List<ReverseBalanceAdjIndividual> AdjustmentList = _context.Database.SqlQueryRaw<ReverseBalanceAdjIndividual>(SqlqueryReverseBalanceAdjIndividual, new SqlParameter("@TransferID", ReverseObj)).ToList();
                            if (AdjustmentList.Count > 0)
                            {
                                foreach (ReverseBalanceAdjIndividual AdjustmentObj in AdjustmentList)
                                {
                                    //Check whether the Adjustment amount of the AdjustmentObj is less than 0 or higher than 0.
                                    switch (AdjustmentObj.AdjustedAmount)
                                    {
                                        case < 0:
                                            Decimal ExistingBalanceCase01 = _context.AccountBalance.Find(AdjustmentObj.BalanceID).OutstandingBalance;
                                            ExistingBalanceCase01 += AdjustmentObj.AdjustedAmount * -1;
                                            String UpdateAccountBalanceCase01 = "UPDATE AccountBalance SET OutstandingBalance = @OutstandingBalance WHERE BalanceID = @BalanceID";
                                            _context.Database.ExecuteSqlRaw(UpdateAccountBalanceCase01, new SqlParameter("@OutstandingBalance", ExistingBalanceCase01),
                                                new SqlParameter("@BalanceID", AdjustmentObj.BalanceID));
                                            break;

                                        case > 0:
                                            Decimal ExistingBalanceCase02 = _context.AccountBalance.Find(AdjustmentObj.BalanceID).OutstandingBalance;
                                            ExistingBalanceCase02 -= AdjustmentObj.AdjustedAmount;
                                            String UpdateAccountBalanceCase02 = "UPDATE AccountBalance SET OutstandingBalance = @OutstandingBalance WHERE BalanceID = @BalanceID";
                                            _context.Database.ExecuteSqlRaw(UpdateAccountBalanceCase02, new SqlParameter("@OutstandingBalance", ExistingBalanceCase02),
                                                new SqlParameter("@BalanceID", AdjustmentObj.BalanceID));
                                            break;

                                    }

                                    //Mark the relevant Adjustment ID as Reversed Adjustment.
                                    String SqlqueryReverseAdjustmentID = "UPDATE BalanceAdjustment SET IsReversed = 1, ReversedBy = @ReversedBy WHERE AdjustmentID = @AdjID ";
                                    _context.Database.ExecuteSqlRaw(SqlqueryReverseAdjustmentID, new SqlParameter("@ReversedBy", HttpContext.Session.GetString("UserID")),
                                        new SqlParameter("@AdjID", AdjustmentObj.AdjustmentID));


                                }
                            }
                            else
                            {
                                //No code block to be run here.
                            }


                            //Check whether any adjustments have been impact for Repo Balances and if any, reverse those adjustments as well.
                            String SqlqueryReverseRepoBalanceAdjIndividual = "SELECT Adj.AdjustmentID, Adj.AdjustedAmount, Repo.RepoID\r\nFROM RepoBalanceAdjustment Adj\r\nLEFT JOIN Repo Repo ON Adj.Repo = Repo.RepoID\r\nWHERE TransferID = @TransferID";
                            List<ReverseRepoBalanceAdjIndividual> RepoAdjustmentList = _context.Database.SqlQueryRaw<ReverseRepoBalanceAdjIndividual>(SqlqueryReverseRepoBalanceAdjIndividual, new SqlParameter("@TransferID", ReverseObj)).ToList();
                            if (AdjustmentList.Count > 0)
                            {
                                foreach (ReverseRepoBalanceAdjIndividual RepoAdjustmentObj in RepoAdjustmentList)
                                {
                                    //Check whether the Adjustment amount of the AdjustmentObj is less than 0 or higher than 0.
                                    switch (RepoAdjustmentObj.AdjustedAmount)
                                    {
                                        case < 0:
                                            Decimal ExistingBalanceCase01 = _context.Repo.Find(RepoAdjustmentObj.RepoID).ClosedBalance;
                                            ExistingBalanceCase01 += RepoAdjustmentObj.AdjustedAmount * -1;
                                            String UpdateAccountBalanceCase01 = "UPDATE Repo SET ClosedBalance = @ClosedBalance WHERE RepoID = @RepoID";
                                            _context.Database.ExecuteSqlRaw(UpdateAccountBalanceCase01, new SqlParameter("@ClosedBalance", ExistingBalanceCase01),
                                                new SqlParameter("@RepoID", RepoAdjustmentObj.RepoID));
                                            break;

                                        case > 0:
                                            Decimal ExistingBalanceCase02 = _context.Repo.Find(RepoAdjustmentObj.RepoID).ClosedBalance;
                                            ExistingBalanceCase02 -= RepoAdjustmentObj.AdjustedAmount;
                                            String UpdateAccountBalanceCase02 = "UPDATE Repo SET ClosedBalance = @ClosedBalance WHERE RepoID = @RepoID";
                                            _context.Database.ExecuteSqlRaw(UpdateAccountBalanceCase02, new SqlParameter("@ClosedBalance", ExistingBalanceCase02),
                                                new SqlParameter("@RepoID", RepoAdjustmentObj.RepoID));
                                            break;

                                    }

                                    //Mark the relevant Repo Adjustment ID as Reversed Adjustment.
                                    String SqlqueryReverseRepoAdjustmentID = "UPDATE RepoBalanceAdjustment SET IsReversed = 1, ReversedBy = @ReversedBy WHERE AdjustmentID = @AdjID ";
                                    _context.Database.ExecuteSqlRaw(SqlqueryReverseRepoAdjustmentID, new SqlParameter("@ReversedBy", HttpContext.Session.GetString("UserID")),
                                        new SqlParameter("@AdjID", RepoAdjustmentObj.AdjustmentID));

                                }
                            }
                            else
                            {
                                //No code block to be run here.
                            }


                            //Check whether any adjustments have been impact for FundRequest and if any, reverse those adjustments as well.
                            String SqlqueryReverseRequestAdjIndividual = "SELECT Adj.AdjustmentID, Adj.AdjustedAmount, Req.RequestID\r\nFROM FundRequestAdjustments Adj\r\nLEFT JOIN FundRequest Req ON Adj.RequestID = Req.RequestID\r\nWHERE TransferID = @TransferID";
                            List<ReverseRequestAdjIndividual> RequestAdjustmentList = _context.Database.SqlQueryRaw<ReverseRequestAdjIndividual>(SqlqueryReverseRequestAdjIndividual, new SqlParameter("@TransferID", ReverseObj)).ToList();
                            if (RequestAdjustmentList.Count > 0)
                            {
                                foreach (ReverseRequestAdjIndividual RequestAdjustmentObj in RequestAdjustmentList)
                                {
                                    //Check whether the Adjustment amount of the AdjustmentObj is less than 0 or higher than 0.
                                    switch (RequestAdjustmentObj.AdjustedAmount)
                                    {
                                        case < 0:
                                            Decimal ExistingBalanceCase01 = _context.FundRequest.Find(RequestAdjustmentObj.RequestID).RemainingRequestAmount;
                                            ExistingBalanceCase01 += RequestAdjustmentObj.AdjustedAmount * -1;
                                            String UpdateAccountBalanceCase01 = "UPDATE FundRequest SET RemainingRequestAmount = @RequestAmount WHERE RequestID = @RequestID";
                                            _context.Database.ExecuteSqlRaw(UpdateAccountBalanceCase01, new SqlParameter("@RequestAmount", ExistingBalanceCase01),
                                                new SqlParameter("@RequestID", RequestAdjustmentObj.RequestID));
                                            break;

                                        case > 0:
                                            Decimal ExistingBalanceCase02 = _context.FundRequest.Find(RequestAdjustmentObj.RequestID).RemainingRequestAmount;
                                            ExistingBalanceCase02 -= RequestAdjustmentObj.AdjustedAmount;
                                            String UpdateAccountBalanceCase02 = "UPDATE FundRequest SET RemainingRequestAmount = @RequestAmount WHERE RequestID = @RequestID";
                                            _context.Database.ExecuteSqlRaw(UpdateAccountBalanceCase02, new SqlParameter("@RequestAmount", ExistingBalanceCase02),
                                                new SqlParameter("@RequestID", RequestAdjustmentObj.RequestID));
                                            break;

                                    }

                                    //Mark the relevant Repo Adjustment ID as Reversed Adjustment.
                                    String SqlqueryReverseRequestAdjustmentID = "UPDATE FundRequestAdjustments SET IsReversed = 1, ReversedBy = @ReversedBy WHERE AdjustmentID = @AdjID ";
                                    _context.Database.ExecuteSqlRaw(SqlqueryReverseRequestAdjustmentID, new SqlParameter("@ReversedBy", HttpContext.Session.GetString("UserID")),
                                        new SqlParameter("@AdjID", RequestAdjustmentObj.AdjustmentID));

                                }
                            }
                            else
                            {
                                //No code block to be run here.
                            }
                             
                        }

                        /* Check whether any adjustment is available in the Fund Request Adjustment table where the TransferID is
                               null. If there any record available with null Transfer ID, it means, there are some other adjustments
                               have been occurred during the initiation of the transfers without any transfer. (Ex -: Recovering
                               the available Fund Requests using available balances of the relevant bank account.) */

                        String SqlqueryMinusAdjustments = "SELECT RequestID, AdjustedAmount, AdjustmentID\r\nFROM FundRequestAdjustments\r\nWHERE IsReversed = 0 AND TransferID IS NULL AND AdjustedDate LIKE @Date";
                        List<RequestAdjustmentsWithoutTransfers> AdjustmentObjList01 = _context.Database.SqlQueryRaw<RequestAdjustmentsWithoutTransfers>(SqlqueryMinusAdjustments, new SqlParameter("@Date", DateTime.Today.Date.ToString("yyyy-MM-dd") + "%")).ToList();
                        foreach (RequestAdjustmentsWithoutTransfers LoopedObj in AdjustmentObjList01)
                        {
                            switch (LoopedObj.AdjustedAmount)
                            {
                                case < 0:
                                    //Update Fund Request accordingly.
                                    String SqlqueryAdd = "UPDATE FundRequest SET RemainingRequestAmount = RemainingRequestAmount + @AddingValue \r\nWHERE RequestID = @Request";
                                    _context.Database.ExecuteSqlRaw(SqlqueryAdd, new SqlParameter("@AddingValue", (LoopedObj.AdjustedAmount * -1)),
                                        new SqlParameter("@Request", LoopedObj.RequestID));

                                    //Update Request Adjustment table as reversed record.
                                    String SqlqueryUpdateCase01 = "UPDATE FundRequestAdjustments SET IsReversed = 1, ReversedBy = @ReversedBy WHERE AdjustmentID = @ID";
                                    _context.Database.ExecuteSqlRaw(SqlqueryUpdateCase01, new SqlParameter("@ReversedBy", HttpContext.Session.GetString("UserID")),
                                        new SqlParameter("@ID", LoopedObj.AdjustmentID));
                                    break;

                            }
                        }

                        /* Check whether any adjustment is available in the OverdraftRecoverAdjustment table. If there any record 
                           available in this table, it means, there are some other adjustments available where an overdraft amount
                           in the account balance has been recovered by adding this OD balance to the relevant FundRequest */

                        String SqlqueryPluseAdjustments = "SELECT RequestID,\r\nAdjustedAmount, \r\nAdjustmentID\r\nFROM OverdraftRecoverAdjustment\r\nWHERE IsReversed = 0 AND AdjustedDate LIKE @Date";
                        List<ODAdjustmentsWithoutTransfers> AdjustmentObjList02 = _context.Database.SqlQueryRaw<ODAdjustmentsWithoutTransfers>(SqlqueryPluseAdjustments, new SqlParameter("@Date", DateTime.Today.Date.ToString("yyyy-MM-dd") + "%")).ToList();
                        foreach (ODAdjustmentsWithoutTransfers LoopedObj in AdjustmentObjList02)
                        {
                            switch (LoopedObj.AdjustedAmount)
                            {
                                case > 0:
                                    //Update Fund Request accordingly.
                                    String SqlquerySubstract = "UPDATE FundRequest SET RemainingRequestAmount = RemainingRequestAmount - @AddingValue \r\nWHERE RequestID = @Request";
                                    _context.Database.ExecuteSqlRaw(SqlquerySubstract, new SqlParameter("@AddingValue", (LoopedObj.AdjustedAmount)),
                                        new SqlParameter("@Request", LoopedObj.RequestID));

                                    //Update OverdraftRecoverAdjustment table as reversed record.
                                    String SqlqueryUpdateCase01 = "UPDATE OverdraftRecoverAdjustment SET IsReversed = 1, ReversedBy = @ReversedBy WHERE AdjustmentID = @ID";
                                    _context.Database.ExecuteSqlRaw(SqlqueryUpdateCase01, new SqlParameter("@ReversedBy", HttpContext.Session.GetString("UserID")),
                                        new SqlParameter("@ID", LoopedObj.AdjustmentID));
                                    break;

                            }
                        }

                        TempData["message"] = "Information: All Transfers have been reversed successfully!";
                        return RedirectToAction("ReverseBulkTransfer", "Transfer");

                    }
                    else
                    {
                        TempData["message"] = "Information: No active Transfer Record available for provided IBT Sheet!";
                        return RedirectToAction("ReverseBulkTransfer", "Transfer");
                    }
                }
                else
                {
                    TempData["message"] = "Information: Already approved transfers available in the bulk list. Unable to delete transfers!";
                    return RedirectToAction("ReverseBulkTransfer", "Transfer");
                }
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("ReverseBulkTransfer", "Transfer");
            }
        }

    }
}
