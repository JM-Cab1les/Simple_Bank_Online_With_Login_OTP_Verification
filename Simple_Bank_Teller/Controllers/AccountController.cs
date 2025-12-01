using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Simple_Bank_Teller.Controllers.Models;
using Simple_Bank_Teller.DataService;
using Simple_Bank_Teller.Helper;
using Simple_Bank_Teller.Models;
using Microsoft.AspNetCore.Session;
using System;

namespace Simple_Bank_Teller.Controllers
{
    public class AccountController : Controller
    {
        private readonly LoginDataService loginDataService_;
        private readonly AccountDataService accountDataService_;
        private readonly MaintenanceDataService maintenanceDataService_;
        public AccountController(LoginDataService loginDataService, AccountDataService accountDataService, MaintenanceDataService maintenanceDataService)
        {
            loginDataService_ = loginDataService;
            accountDataService_ = accountDataService;
            maintenanceDataService_ = maintenanceDataService;
        }


        public async Task<IActionResult> CreateAccount()
        {
            var model = new MaintenanceModel
            {
                Gender = await maintenanceDataService_.GetGenderListAsync(),
                MemberType = await maintenanceDataService_.GetMembershipTypeListAsync()
                
            };
            return View(model);
        }

        public ActionResult LoginAccount()
        {
            return View();
        }


        [HttpPost]
        public async Task<JsonResult> Login([FromBody] LoginAccountModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "UserName and Password is required" });
            }

            else
            {
                try
                {
                    var response = await accountDataService_.LoginAccount(model.UserName, model.Password);

                    if (response == null)
                        return Json(new { success = false, message = "Invalid Credentials" });

                    var account = await accountDataService_.GetAccountInformation(response.AccountInformationId);

                    var otp = new Random().Next(100000, 999999).ToString();

                    HttpContext.Session.SetString("OTP", otp);

                    HttpContext.Session.SetString("UserId", response.AccountInformationId.ToString());

                    HttpContext.Session.SetString("UserName", response.UserName);

                    string phone = account.ContactNo;

                    string msg = "Your login OTP is " + otp;

                    await SMSHelper.SendSmsAsync(phone, msg);

                    return Json(new { success = true, message = "OTP Sent", requireOtp = true });

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        [HttpPost]
        public JsonResult VerifyOtp([FromBody] OtpVerifyModel model)
        {
            string sessionOtp = HttpContext.Session.GetString("OTP");

            if (sessionOtp == null)
                return Json(new { success = false, message = "OTP expired, Login again" });


            if(model.Otp == sessionOtp)
            {
                HttpContext.Session.Remove("OTP");

                return Json(new { success = true, message = "OTP Verified" });
            }

            return Json(new { success = false, message = "Invalid OTP" });
        }


        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Json(new { success = true, message = "Logged out successfully" });
        }

        [HttpPost]
        public async Task<JsonResult> AddAccount([FromBody] CreateAccountModel model)
        {
            if (model == null)
            {
                return Json(new { success = false, message = "Invalid data submitted." });
            }
            else
            {
                try
                {
                    int response = await accountDataService_.CreateAccountInformation(new AccountInformation
                    {
                        FirstName = model.FirstName,
                        MiddleName = model.MiddleName,
                        LastName = model.LastName,
                        Gender = model.Gender,
                        ContactNo = model.ContactNo,
                        Address = model.Address,
                        EmailAddress = model.EmailAddress
                    });

                    if(response == 0)
                    {

                        return Json(new { success = true, message = "Failed to create Account Information." });
                    }
                    else
                    {
                        var accountNo = Convert.ToInt64(AccountNoGenerator.GenerateAccountNumber());
                        int accountID = await accountDataService_.CreateAccount(new Account
                        {
                            UserName = model.UserName,
                            Password = model.Password,
                            AccountNo = accountNo,
                            AccountType = model.AccountType,
                            AccountInformationId = response
                        });
                        if (accountID == 0)
                        {
                            return Json(new { success = false, message = "Account Info saved but failed to create login account." });
                        }

                        return Json(new { success = true, message = "Account created successfully!", accountNo = accountNo });
                    }
                }catch(Exception ex)
                {
                    return Json(new { success = false, message = "An error occurred: " + ex.Message });
                }
            }
                
        }
    }
}
