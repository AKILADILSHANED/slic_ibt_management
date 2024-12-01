using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SLICGL_IBT_Management.DB_Connection;
using SLICGL_IBT_Management.Models;
using System.Text.RegularExpressions;

namespace SLICGL_IBT_Management.Controllers
{
    public class RepoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RepoController(ApplicationDbContext context)
        {
            this._context = context;
        }

        public IActionResult CreateNewRepo()
        {
            //Get the list of all bank acounts.
            Dictionary<String, String> AccountList = new Dictionary<string, String>();
            AccountList = _context.BankAccount.Where(acc => acc.IsDeleted == 0).ToDictionary(accid => accid.AccountID, accno => accno.AccountNumber);

            //Create new insance of CreateRepo.

            CreateRepo createRepo = new CreateRepo()
            {
                AccountList = AccountList,
                RepoType = null,
                OpeningBalance = 0,
            }; 
            
            return View(createRepo);
        }

        public IActionResult SaveRepoDetails(CreateRepo RepoDetails)
        {
            if (ModelState.IsValid)
            {
                //Identify last Repo_id in the Repo table.

                Repo last_repo = _context.Repo.OrderByDescending(Repo => Repo.CreatedDate).FirstOrDefault();
                String string_repo_id;

                if (last_repo == null)
                {
                    //Creation of new repo_id.

                    string_repo_id = "REPO/" + DateTime.Now.Year + "/1";
                }
                else
                {
                    //Creation of new repo id.

                    string[] parts = last_repo.RepoID.Split('/');
                    int lastPart = int.Parse(parts.Last());
                    int NewNumericValue;
                    //Get current Year.
                    int CurrentYear = DateTime.Now.Year;
                    if (parts[1] == CurrentYear.ToString())
                    {
                        NewNumericValue = lastPart + 1;
                        string_repo_id = "REPO/" + CurrentYear + "/" + NewNumericValue;
                    }
                    else
                    {
                        NewNumericValue = 1;
                        string_repo_id = "BAL/" + CurrentYear + "/" + NewNumericValue;
                    }
                }

                //Create an instance of Repo.
                Repo repo = new Repo()
                {
                    RepoID = string_repo_id,
                    RepoType = RepoDetails.RepoType,
                    CreatedDate = DateTime.Now,
                    OpeningBalance = RepoDetails.OpeningBalance,
                    ClosedBalance = RepoDetails.OpeningBalance,
                    AccountID = RepoDetails.AccountID,
                    CreateBy = HttpContext.Session.GetString("UserID").ToString() ,
                    IsDeleted = 0,
                    DeleteBy = null
                };

                _context.Add(repo);
                _context.SaveChanges();

                TempData["message"] = "Success: Repo details saved successfully with Repo ID: " + string_repo_id + "!";
                return RedirectToAction("CreateNewRepo", "Repo");

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("CreateNewRepo", "Repo");
            }
            
        }

        public IActionResult EditRepo()
        {
            return View();
        }

        public IActionResult DisplayDetailsEditRepo(String RepoID)
        {
            if (ModelState.IsValid)
            {
                //Check whether any Repo is available in the database.
                Repo RepoObject = _context.Repo.Find(RepoID);
                if (RepoObject != null)
                {
                    //Check whether the Repo is already deleted
                    if (RepoObject.IsDeleted == 0)
                    {
                        //Check whether the Repo is a previouse day Repo.
                        if (RepoObject.CreatedDate.Date >= DateTime.Today.Date)
                        {
                            //Get the list of all bank acounts.
                            Dictionary<String, String> AccountList = new Dictionary<string, String>();
                            AccountList = _context.BankAccount.Where(acc => acc.IsDeleted == 0).ToDictionary(accid => accid.AccountID, accno => accno.AccountNumber);

                            //Create new insance of DisplayDetailsEditRepo.

                            DisplayDetailsEditRepo EditRepo = new DisplayDetailsEditRepo()
                            {
                                AccountList = AccountList,
                                RepoType = RepoObject.RepoType,
                                OpeningBalance = RepoObject.OpeningBalance,
                                ClosedBalance = RepoObject.ClosedBalance,
                                RepoID = RepoID,
                                AccountID = RepoObject.AccountID
                            };

                            return View(EditRepo);
                        }
                        else
                        {
                            TempData["message"] = "Information: You have no authority to update a previouse day Repo!";
                            return RedirectToAction("EditRepo", "Repo");
                        }

                        
                    }
                    else
                    {
                        TempData["message"] = "Information: Repo ID is no longer available!";
                        return RedirectToAction("EditRepo", "Repo");
                    }
                }
                else
                {
                    TempData["message"] = "Information: No Repo details available for provided Repo ID!";
                    return RedirectToAction("EditRepo", "Repo");
                }

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("EditRepo", "Repo");
            }

        }

        public IActionResult SaveRepoUpdatedDetails(DisplayDetailsEditRepo UpdatedRepo)
        {
            if (ModelState.IsValid)
            {
                String SqlQuery = "UPDATE Repo SET RepoType = @Type , AccountID = @Account WHERE RepoID = @Repo ";
                _context.Database.ExecuteSqlRaw(SqlQuery, new SqlParameter("@Type", UpdatedRepo.RepoType),
                    new SqlParameter("@Account", UpdatedRepo.AccountID),
                    new SqlParameter("@Repo", UpdatedRepo.RepoID));

                TempData["message"] = "Success: Repo details updated successfully!";
                return RedirectToAction("EditRepo", "Repo");

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("EditRepo", "Repo");
            }
        }

        public IActionResult SearchRepo()
        {
            return View();
        }

        public IActionResult DisplayDetailsSearchepo(String RepoID)
        {
            if (ModelState.IsValid)
            {
                //Check whether any Repo is available in the database.
                Repo RepoObject = _context.Repo.Find(RepoID);
                if (RepoObject != null)
                {
                    //Check whether the Repo is already deleted
                    if (RepoObject.IsDeleted == 0)
                    {
                        //Get the list of all bank acounts.
                        Dictionary<String, String> AccountList = new Dictionary<string, String>();
                        AccountList = _context.BankAccount.Where(acc => acc.IsDeleted == 0).ToDictionary(accid => accid.AccountID, accno => accno.AccountNumber);

                        //Create new insance of DisplayDetailsEditRepo.

                        DisplayDetailsSearchRepoDTO SearchRepo = new DisplayDetailsSearchRepoDTO()
                        {
                            AccountList = AccountList,
                            RepoType = RepoObject.RepoType,
                            OpeningBalance = RepoObject.OpeningBalance,
                            ClosedBalance = RepoObject.ClosedBalance,
                            RepoID = RepoID,
                            AccountID = RepoObject.AccountID
                        };

                        return View(SearchRepo);
                    }
                    else
                    {
                        TempData["message"] = "Information: Repo ID is no longer available!";
                        return RedirectToAction("SearchRepo", "Repo");
                    }
                }
                else
                {
                    TempData["message"] = "Information: No Repo details available for provided Repo ID!";
                    return RedirectToAction("SearchRepo", "Repo");
                }

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("SearchRepo", "Repo");
            }
        }


        public IActionResult DeleteRepo()
        {
            return View();
        }

        public IActionResult DisplayDetailsDeleteRepo(String RepoID)
        {
            if (ModelState.IsValid)
            {
                //Check whether any adjustment (Through Transfers or Manual adjustments) has been hit to the Repo.
                Repo? RepoObj = _context.Repo.Find(RepoID);
                if (RepoObj.OpeningBalance == RepoObj.ClosedBalance)
                {
                    //Check whether any Repo is available in the database.
                    Repo RepoObject = _context.Repo.Find(RepoID);
                    if (RepoObject != null)
                    {
                        //Check whether the Repo is already deleted
                        if (RepoObject.IsDeleted == 0)
                        {
                            //Check whether the Repo is a previouse day Repo.
                            if (RepoObject.CreatedDate.Date == DateTime.Today.Date)
                            {
                                //Get the list of all bank acounts.
                                Dictionary<String, String> AccountList = new Dictionary<string, String>();
                                AccountList = _context.BankAccount.Where(acc => acc.IsDeleted == 0).ToDictionary(accid => accid.AccountID, accno => accno.AccountNumber);

                                //Create new insance of DisplayDetailsDeleteRepoDTO.

                                DisplayDetailsDeleteRepoDTO DeleteRepo = new DisplayDetailsDeleteRepoDTO()
                                {
                                    AccountList = AccountList,
                                    RepoType = RepoObject.RepoType,
                                    OpeningBalance = RepoObject.OpeningBalance,
                                    ClosedBalance = RepoObject.ClosedBalance,
                                    RepoID = RepoID,
                                    AccountID = RepoObject.AccountID
                                };

                                return View(DeleteRepo);
                            }
                            else
                            {
                                TempData["message"] = "Information: You have no authority to delete a previouse day Repo!";
                                return RedirectToAction("DeleteRepo", "Repo");
                            }

                        }
                        else
                        {
                            TempData["message"] = "Information: Repo ID is no longer available!";
                            return RedirectToAction("DeleteRepo", "Repo");
                        }
                    }
                    else
                    {
                        TempData["message"] = "Information: No Repo details available for provided Repo ID!";
                        return RedirectToAction("DeleteRepo", "Repo");
                    }
                }
                else
                {
                    TempData["message"] = "Information: No authority for delete. Adjustments have been already initiated for this Repo!";
                    return RedirectToAction("DeleteRepo", "Repo");
                }
                                
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("DeleteRepo", "Repo");
            }

        }

        public IActionResult SaveRepoDeletedDetails(DisplayDetailsDeleteRepoDTO UpdatedRepo)
        {
            if (ModelState.IsValid)
            {
                String SqlQuery = "UPDATE Repo SET IsDeleted = 1 , DeleteBy = @Deleter WHERE RepoID = @Repo ";
                _context.Database.ExecuteSqlRaw(SqlQuery, new SqlParameter("@Deleter", HttpContext.Session.GetString("UserID").ToString()),
                    new SqlParameter("@Repo", UpdatedRepo.RepoID));

                TempData["message"] = "Success: Repo deleted successfully!";
                return RedirectToAction("DeleteRepo", "Repo");

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("DeleteRepo", "Repo");
            }
        }

        public IActionResult DisplayAllRepo()
        {
            return View();
        }

        public IActionResult DisplayAllRepoDetails(DateTime RepoDate)
        {
            String FormattedDate = RepoDate.Date.ToString("yyyy-MM-dd");
            if (ModelState.IsValid)
            {
                //Get the list of all repo.
                String Sqlquery = "SELECT Repo.RepoID, Repo.RepoType, Repo.CreatedDate, Repo.OpeningBalance, Repo.ClosedBalance,\r\nCASE WHEN Repo.IsDeleted = 0 \r\nTHEN 'Active' \r\nELSE 'Deleted' END AS Status,\r\nCASE WHEN Repo.DeleteBy IS NULL THEN 'N/A'\r\nELSE Usr1.FirstName \r\nEND AS Deleter, Acc.AccountNumber, Usr2.FirstName AS Creator FROM\r\nRepo LEFT JOIN Users Usr1 ON Repo.DeleteBy = Usr1.User_Id \r\nLEFT JOIN BankAccount Acc ON Repo.AccountID = Acc.AccountID\r\nLEFT JOIN Users Usr2 ON Repo.CreateBy = Usr2.User_Id WHERE CAST(Repo.CreatedDate AS date) = @Date ";
                List<DisplayAllRepoDetailsDTO> RepoList = _context.Database.SqlQueryRaw<DisplayAllRepoDetailsDTO>(Sqlquery, new SqlParameter("@Date", FormattedDate)).ToList();

                //Chek whether any Repo record is available for provided repo date.
                if (RepoList.Count > 0)
                {
                    return View(RepoList);
                }
                else
                {
                    TempData["message"] = "Information: No Repo details available for selected Repo Date!";
                    return RedirectToAction("DisplayAllRepo", "Repo");
                }
                
            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("DisplayAllRepo", "Repo");
            }

            /*TempData["message"] = RepoDate.Date.ToString("yyyy-MM-dd");
            return RedirectToAction("DisplayAllRepo", "Repo");*/

        }

        public IActionResult RepoAdjustments()
        {
            return View();
        }

        public IActionResult DisplayAdjustmentDetails(String RepoID)
        {
            if (ModelState.IsValid)
            {
                //Get the relevant Adjustments related to provided RepoID,
                String Sqlquery01 = "SELECT AdjustmentID, AdjustedDate, AdjustedAmount, Description, Repo\r\nFROM RepoBalanceAdjustment WHERE IsReversed = 0 AND Repo = @RepoID";
                Dictionary<String, RepoAdjustmentDetailsBalancesDTO> Adjustments = _context.Database.SqlQueryRaw<RepoAdjustmentDetailsBalancesDTO>(Sqlquery01, new SqlParameter("@RepoID", RepoID)).
                    ToDictionary(adj => adj.AdjustmentID, Adj => new RepoAdjustmentDetailsBalancesDTO()
                    {
                        AdjustmentID = Adj.AdjustmentID,
                        AdjustedDate = Adj.AdjustedDate,
                        AdjustedAmount = Adj.AdjustedAmount,
                        Description = Adj.Description,
                        Repo = Adj.Repo
                    });

                //Get Opening and Closing balances of the relavant RepoID.
                String Sqlquery02 = "SELECT Repo.RepoID, Repo.OpeningBalance, Repo.ClosedBalance, Repo.CreatedDate, Acc.AccountNumber, Repo.IsDeleted\r\nFROM Repo Repo\r\nLEFT JOIN BankAccount Acc ON Repo.AccountID = Acc.AccountID WHERE RepoID = @RepoID";
                RepoAccountBalanceForAdjustments? Repos = _context.Database.SqlQueryRaw<RepoAccountBalanceForAdjustments>(Sqlquery02, new SqlParameter("@RepoID", RepoID)).FirstOrDefault();

                //Check whether any Repo record is available in the Repo Table.
                if (Repos != null)
                {
                    //Create new instance of RepoAccountBalanceAdjustmentDetails and initialize the values.
                    RepoAccountBalanceAdjustmentDetails Details = new RepoAccountBalanceAdjustmentDetails()
                    {
                        RepoAccountBalanceForAdjustments = Repos,
                        AllRepoAdjustments = Adjustments
                    };

                    return View(Details);
                }
                else
                {
                    TempData["message"] = "Information: No Repo can be identified for provided Repo ID!";
                    return RedirectToAction("RepoAdjustments", "Repo");
                }

            }
            else
            {
                TempData["message"] = "Error: Provided data does not valid with model criteria!";
                return RedirectToAction("RepoAdjustments", "Repo");
            }
        }
    }
}
