using Microsoft.AspNetCore.Mvc;
using Simple_Bank_Teller.Controllers.Models;
using Simple_Bank_Teller.DataService;
using Simple_Bank_Teller.Models;
using System.Transactions;

namespace Simple_Bank_Teller.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AccountDataService accountDataService_;

        public DashboardController(AccountDataService accountDataService)
        {
            accountDataService_ = accountDataService;
        }


        public async Task<IActionResult> Dashboard()
        {
            DashboardModel model = new DashboardModel();

            var userName = HttpContext.Session.GetString("UserName");

            var userId = HttpContext.Session.GetString("UserId");

            model.UserAccount = await accountDataService_.GetAccount(Convert.ToInt64(userId));

            model.UserAccountInformation = await accountDataService_.GetAccountInformation(Convert.ToInt64(userId));

            model.MembersValueType = await accountDataService_.GetMembershipType((int)model.UserAccount.AccountType);

            model.TransactionValue = await accountDataService_.GetAccountTransaction(Convert.ToInt64(userId))
                             ?? new Simple_Bank_Teller.Models.Transaction { AvailableBal = "0", CurrentBal = "0" };

            if (string.IsNullOrEmpty(userName))
                return RedirectToAction("Login", "Account");

            ViewBag.UserName = userName;


            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> AccountTransaction([FromBody] TransactionModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Invalid data submitted." });
            }else
            {
                try
                {
                    var userName = HttpContext.Session.GetString("UserName");
                    model.CreatedBy = userName;

                    int response = await accountDataService_.StartTransaction(new Simple_Bank_Teller.Models.Transaction
                    {
                        AccountId = model.AccountId,
                        MembershipType = model.MembershipType,
                        CurrentBal = model.CurrentBal,
                        AvailableBal = model.AvailableBal,
                        TransactionType = model.TransactionType,
                        CreatedBy = userName

                    });

                    if (response == 0)
                    {

                        return Json(new { success = true, message = "Transaction Failed, Please try again." });
                    }
                    else
                    {
                        return Json(new { success = true, message = model.TransactionType == 1 ? "Deposit successful!" : "Withdrawal successful!" });
                    }
                }
                catch (InvalidOperationException ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred: " + ex.Message });
                }
            }
        }
    }
}
